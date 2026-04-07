using System.Text.Json;
using API_BigFOOD.Model;

namespace API_BigFOOD.Services
{
    public class GometaService : IGometaService
    {
        private readonly HttpClient _httpClient;

        public GometaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://apis.gometa.org/");
        }

        public async Task<ClienteInfo?> ObtenerClientePorCedulaAsync(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"cedulas/{cedula}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var contenido = await response.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<ClienteGometaDTO>(
                    contenido,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado != null && resultado.resultcount > 0 && resultado.results.Count > 0)
                    return resultado.results[0];

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
