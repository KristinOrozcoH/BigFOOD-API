using API_BigFOOD.Model;

namespace API_BigFOOD.Services
{
    public interface IGometaService
    {
        Task<ClienteInfo?> ObtenerClientePorCedulaAsync(string cedula);
    }
}
