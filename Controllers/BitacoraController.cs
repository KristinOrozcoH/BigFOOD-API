using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_BigFOOD.Model;

namespace API_BigFOOD.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BitacoraController : Controller
    {
        private readonly DbContextBigFOOD _context;

        public BitacoraController(DbContextBigFOOD pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        [Authorize]
        public IActionResult List()
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede ver la bitácora.");

            return Ok(_context.Bitacora.ToList());
        }

        [HttpGet("SearchTabla")]
        [Authorize]
        public IActionResult SearchTabla(string pTabla)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede buscar en la bitácora por tabla.");

            var resultados = _context.Bitacora
                .Where(y => y.Tabla.StartsWith(pTabla))
                .ToList();

            return Ok(resultados);
        }

        [HttpGet("SearchUsuario")]
        [Authorize]
        public IActionResult SearchUsuario(int pUsuario)
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede buscar en la bitácora por usuario.");

            var resultados = _context.Bitacora
                .Where(y => y.Usuario == pUsuario)
                .ToList();

            return Ok(resultados);
        }
    }
}
