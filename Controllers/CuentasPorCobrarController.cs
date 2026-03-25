using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;
using Microsoft.EntityFrameworkCore;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CuentasPorCobrarController : Controller
    {
        private readonly DbContextBigFOOD _context;

        public CuentasPorCobrarController(DbContextBigFOOD pContext)
        {
            _context = pContext;
        } 

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            return Ok(_context.CuentasPorCobrar.ToList());
        }

        [HttpGet("Search")]
        [Authorize]
        public IActionResult Search(int pFactura)
        {
            var temp = _context.CuentasPorCobrar.FirstOrDefault(x => x.numFactura == pFactura);

            if (temp == null)
            {
                temp = new CuentasPorCobrar()
                {
                    numFactura = pFactura,
                    codCliente = "N/A",
                    FechaFactura = DateTime.Now,
                    FechaRegistro = DateTime.Now,
                    montoFactura = 0,
                    Usuario = 0,
                    estado = "I"
                };
            }

            return Ok(temp);
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> Update(CuentasPorCobrar pCuenta)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede modificar cuentas por cobrar.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.login == User.Identity.Name);

            if (usuario == null)
                return Unauthorized("Usuario no válido o no encontrado.");

            try
            {
                var temp = _context.CuentasPorCobrar.FirstOrDefault(t => t.numFactura == pCuenta.numFactura);

                if (temp != null)
                {
                    temp.codCliente = pCuenta.codCliente;
                    temp.FechaFactura = pCuenta.FechaFactura;
                    temp.FechaRegistro = pCuenta.FechaRegistro;
                    temp.montoFactura = pCuenta.montoFactura;
                    temp.Usuario = usuario.Id;
                    temp.estado = pCuenta.estado;

                    _context.CuentasPorCobrar.Update(temp);
                    await _context.SaveChangesAsync();

                    await RegistrarBitacoraAsync("CuentasPorCobrar", usuario.Id, "U", $"Cuenta #{temp.numFactura}");
                    return Ok($"Cuenta #{temp.numFactura} actualizada correctamente");
                }
                else
                {
                    return NotFound($"No existe cuenta #{pCuenta.numFactura}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SearchCliente")]
        [Authorize]
        public IActionResult SearchCliente(string pCedula)
        {
            var list = _context.CuentasPorCobrar
                .Where(y => y.codCliente.StartsWith(pCedula))
                .ToList();

            return Ok(list);
        }

        private async Task RegistrarBitacoraAsync(string tabla, int usuario, string tipo, string registro)
        {
            string sql = @"
                INSERT INTO Bitacora (Tabla, Usuario, Maquina, Fecha, TipoMov, Registro)
                VALUES ({0}, {1}, {2}, GETDATE(), {3}, {4})
            ";

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                tabla,
                usuario,
                Environment.MachineName,
                tipo,
                registro
            );
        }
    }
}


