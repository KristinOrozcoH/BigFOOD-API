using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;
using Microsoft.EntityFrameworkCore;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosController : Controller
    {
        private readonly DbContextBigFOOD _context;

        public ProductosController(DbContextBigFOOD context)
        {
            _context = context;
        }

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            var lista = _context.Productos.ToList();
            return Ok(lista);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Producto temp)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede registrar nuevos productos.");

            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.login == User.Identity.Name);
                temp.Usuario = usuario?.Id ?? 0;

                _context.Productos.Add(temp);
                await _context.SaveChangesAsync();

                await RegistrarBitacoraAsync("Productos", temp.Usuario, "I", $"Producto: {temp.Descripcion}");

                return Ok($"Producto {temp.Descripcion} registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> Update(Producto pProducto)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede modificar productos.");

            try
            {
                var temp = _context.Productos.FirstOrDefault(t => t.CodigoInterno == pProducto.CodigoInterno);
                if (temp == null)
                    return NotFound($"No existe producto con código {pProducto.CodigoInterno}");

                temp.CodigoBarra = pProducto.CodigoBarra;
                temp.Descripcion = pProducto.Descripcion;
                temp.PrecioVenta = pProducto.PrecioVenta;
                temp.Descuento = pProducto.Descuento;
                temp.Impuesto = pProducto.Impuesto;
                temp.UnidadMedida = pProducto.UnidadMedida;
                temp.PrecioCompra = pProducto.PrecioCompra;
                temp.Existencia = pProducto.Existencia;

                var usuario = _context.Usuarios.FirstOrDefault(u => u.login == User.Identity.Name);
                temp.Usuario = usuario?.Id ?? 0;

                _context.Productos.Update(temp);
                await _context.SaveChangesAsync();

                await RegistrarBitacoraAsync("Productos", temp.Usuario, "U", $"Producto: {temp.Descripcion}");

                return Ok($"Producto {temp.Descripcion} actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("SearchDescripcion")]
        [Authorize]
        public IActionResult SearchDescripcion(string descripcion)
        {
            var lista = _context.Productos
                .Where(y => y.Descripcion.StartsWith(descripcion))
                .ToList();

            return Ok(lista);
        }

        [HttpGet("SearchCodigo")]
        [Authorize]
        public IActionResult SearchCodigo(int pCodigo)
        {
            var temp = _context.Productos
                .FirstOrDefault(x => x.CodigoInterno == pCodigo);

            if (temp == null) 
            {
                temp = new Producto()
                {
                    CodigoInterno = pCodigo,
                    CodigoBarra = "N/A",
                    Descripcion = "No existe",
                    PrecioVenta = 0,
                    Descuento = 0,
                    Impuesto = 0,
                    UnidadMedida = "N/A",
                    PrecioCompra = 0,
                    Usuario = 0,
                    Existencia = 0
                };
            }

            return Ok(temp);
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



