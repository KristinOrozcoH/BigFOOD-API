using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_BigFOOD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        [HttpGet("SoloAdministrador")]
        [Authorize]
        public IActionResult SoloAdministrador()
        {
            if (User.Identity?.Name != "admin@gmail.com")
                return Content("Solo el administrador puede acceder a esta funcionalidad.");

            return Ok("Acceso permitido: administrador autenticado.");
        }

        [HttpGet("SoloUsuarioActivo")]
        [Authorize]
        public IActionResult SoloUsuarioActivo()
        {
            if (User.Identity?.Name == null)
                return Unauthorized("Token inválido.");

            return Ok($"Bienvenido, {User.Identity.Name}");
        }
    }
}
