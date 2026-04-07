using API_BigFOOD.Model;

namespace API_BigFOOD.Services
{
    public interface ITipoCambioService
    {
        Task<TipoCambioDTO?> ObtenerTipoCambioDolarAsync();
    }
}
