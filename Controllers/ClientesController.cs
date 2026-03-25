using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;
using API_BigFOOD.Services;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientesController : Controller
    {
        private readonly DbContextBigFOOD _context;
        private readonly IGometaService _gometaService;

        public ClientesController(DbContextBigFOOD pContext, IGometaService gometaService)
        {
            _context = pContext;
            _gometaService = gometaService;
        }

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            return Ok(_context.Clientes.ToList());
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Cliente temp)
        { 
            try
            {
                bool requiereGometa =
                    string.IsNullOrWhiteSpace(temp.NombreCompleto) ||
                    temp.NombreCompleto.Trim().ToLower() == "string" ||
                    string.IsNullOrWhiteSpace(temp.tipoCedula) ||
                    temp.tipoCedula.Trim().ToLower() == "string";

                if (requiereGometa)
                {
                    var info = await _gometaService.ObtenerClientePorCedulaAsync(temp.cedulaLegal);
                    if (info != null)
                    {
                        temp.NombreCompleto = info.fullname ?? temp.NombreCompleto;
                        temp.tipoCedula = info.guess_type ?? temp.tipoCedula;
                        if (string.IsNullOrWhiteSpace(temp.Email))
                            temp.Email = "N/A";
                    }
                }

                temp.fechaRegistro = DateTime.Now;
                temp.estado = "A";

                var usuario = _context.Usuarios.FirstOrDefault(u => u.login == User.Identity.Name);
                temp.Usuario = usuario?.Id ?? 0;

                _context.Clientes.Add(temp);
                await _context.SaveChangesAsync();

                return Ok($"Cliente {temp.NombreCompleto} registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SearchCedula")]
        [Authorize]
        public async Task<IActionResult> SearchCedula(string pCedula)
        {
            var temp = _context.Clientes.FirstOrDefault(x => x.cedulaLegal == pCedula);
            if (temp != null)
                return Ok(temp);

            var info = await _gometaService.ObtenerClientePorCedulaAsync(pCedula);
            if (info != null)
            {
                return Ok(new Cliente
                {
                    cedulaLegal = pCedula,
                    tipoCedula = info.guess_type,
                    NombreCompleto = info.fullname,
                    Email = "N/A",
                    fechaRegistro = DateTime.Now,
                    estado = "A",
                    Usuario = 0
                });
            }

            return Ok(new Cliente
            {
                cedulaLegal = pCedula,
                tipoCedula = "NA",
                NombreCompleto = "No existe",
                Email = "N/A",
                fechaRegistro = DateTime.Now,
                estado = "I",
                Usuario = 0
            });
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> Update(Cliente pCliente)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede modificar clientes.");

            var temp = _context.Clientes.FirstOrDefault(t => t.cedulaLegal == pCliente.cedulaLegal);
            if (temp == null)
                return NotFound($"No existe cliente con cédula {pCliente.cedulaLegal}");

            temp.tipoCedula = pCliente.tipoCedula;
            temp.NombreCompleto = pCliente.NombreCompleto;
            temp.Email = pCliente.Email;
            temp.fechaRegistro = pCliente.fechaRegistro;
            temp.estado = pCliente.estado;

            var usuario = _context.Usuarios.FirstOrDefault(u => u.login == User.Identity.Name);
            temp.Usuario = usuario?.Id ?? 0;

            _context.Clientes.Update(temp);
            await _context.SaveChangesAsync();
            return Ok($"Cliente {temp.NombreCompleto} actualizado correctamente.");
        }

        [HttpGet("SearchNombre")]
        [Authorize]
        public IActionResult SearchNombre(string pNombre)
        {
            var list = _context.Clientes
                .Where(y => y.NombreCompleto.StartsWith(pNombre))
                .ToList();

            return Ok(list);
        }
    }
}
