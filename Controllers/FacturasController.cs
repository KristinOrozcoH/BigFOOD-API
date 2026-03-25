using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;
using API_BigFOOD.Services;
using Microsoft.EntityFrameworkCore;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacturasController : Controller
    {
        private readonly DbContextBigFOOD _context;
        private readonly ITipoCambioService _tipoCambioService;
        private readonly FacturaPdfService _facturaPdfService;

        public FacturasController(DbContextBigFOOD pContext, ITipoCambioService tipoCambioService, FacturaPdfService facturaPdfService)
        {
            _context = pContext;
            _tipoCambioService = tipoCambioService;
            _facturaPdfService = facturaPdfService;
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromBody] FacturaConDetallesDTO data)
        {
            var username = User.Identity?.Name;
            var usuarioDb = _context.Usuarios.FirstOrDefault(u => u.login == username);
            if (usuarioDb == null)
                return Unauthorized("Usuario autenticado no encontrado.");

            var factura = data.Factura;
            var detalles = data.Detalles;

            try
            {
                factura.Usuario = usuarioDb.Id;

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                decimal subtotal = 0;
                decimal totalDescuento = 0;
                decimal totalImpuesto = 0;

                foreach (var item in detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.codInterno);
                    if (producto == null)
                        return BadRequest($"Producto {item.codInterno} no existe.");

                    if (producto.Existencia < item.cantidad)
                        return BadRequest($"Producto {producto.Descripcion} no tiene suficiente stock.");

                    decimal precioBruto = item.PrecioUnitario * item.cantidad;
                    decimal montoDescuento = precioBruto * (item.PorDescuento / 100);
                    decimal montoImpuesto = (precioBruto - montoDescuento) * (item.PorImp / 100);

                    subtotal += precioBruto;
                    totalDescuento += montoDescuento;
                    totalImpuesto += montoImpuesto;

                    item.numFactura = factura.numero;
                    _context.Det_Facturas.Add(item);

                    producto.Existencia -= item.cantidad;
                    _context.Productos.Update(producto);
                }

                factura.Subtotal = subtotal;
                factura.MontoDescuento = totalDescuento;
                factura.MontoImpuesto = totalImpuesto;
                factura.Total = subtotal - totalDescuento + totalImpuesto;

                _context.Facturas.Update(factura);
                await _context.SaveChangesAsync();

                await RegistrarBitacoraAsync("Facturas", usuarioDb.Id, "I", $"Factura #{factura.numero}");

                if (factura.Condicion == "C")
                {
                    var cuenta = new CuentasPorCobrar
                    {
                        numFactura = factura.numero,
                        codCliente = factura.codCliente,
                        FechaFactura = factura.Fecha,
                        FechaRegistro = DateTime.Now,
                        montoFactura = factura.Total,
                        Usuario = usuarioDb.Id,
                        estado = "A"
                    };

                    _context.CuentasPorCobrar.Add(cuenta);
                    await _context.SaveChangesAsync();
                }

                string resultadoEnvio;
                try
                {
                    resultadoEnvio = await _facturaPdfService.GenerarYEnviarFacturaPDF(factura.numero);
                }
                catch (Exception ex)
                {
                    resultadoEnvio = $"Error al enviar la factura por correo: {ex.Message}";
                }

                return Ok(new
                {
                    mensaje = $"Factura #{factura.numero} registrada correctamente.",
                    factura.Subtotal,
                    factura.MontoDescuento,
                    factura.MontoImpuesto,
                    factura.Total,
                    envioCorreo = resultadoEnvio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            return Ok(_context.Facturas.ToList());
        }

        [HttpGet("Search")]
        [Authorize]
        public IActionResult Search(int pNumero)
        {
            var temp = _context.Facturas.FirstOrDefault(x => x.numero == pNumero);
            if (temp == null)
            {
                temp = new Factura
                {
                    numero = pNumero,
                    Fecha = DateTime.Now,
                    codCliente = "N/A",
                    Subtotal = 0,
                    MontoDescuento = 0,
                    MontoImpuesto = 0,
                    Total = 0,
                    estado = "I",
                    Usuario = 0,
                    TipoPago = "N",
                    Condicion = "N"
                };
            }
            return Ok(temp);
        }

        [HttpGet("SearchCliente")]
        [Authorize]
        public IActionResult SearchCliente(string pCedula)
        {
            var lista = _context.Facturas
                .Where(y => y.codCliente.StartsWith(pCedula))
                .ToList();
            return Ok(lista);
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




