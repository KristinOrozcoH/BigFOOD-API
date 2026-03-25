using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetFacturasController : Controller
    {
        private readonly DbContextBigFOOD _context;

        public DetFacturasController(DbContextBigFOOD pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            var lista = _context.Det_Facturas.ToList();
            return Ok(lista);
        }

        [HttpGet("Search")]
        [Authorize]
        public IActionResult Search(int pNumFactura, int pCodInterno)
        {
            var temp = _context.Det_Facturas.Find(pNumFactura, pCodInterno);

            if (temp == null)
            {
                temp = new DetFactura()
                {
                    numFactura = pNumFactura,
                    codInterno = pCodInterno,
                    cantidad = 0,
                    PrecioUnitario = 0,
                    PorImp = 0,
                    PorDescuento = 0
                };
            }

            return Ok(temp);
        }

        [HttpGet("SearchFactura")]
        [Authorize]
        public IActionResult SearchFactura(int pNumFactura)
        {
            var lista = _context.Det_Facturas
                .Where(y => y.numFactura == pNumFactura)
                .ToList();

            return Ok(lista);
        }
    }
}

