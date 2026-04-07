using API_BigFOOD.Model;

namespace API_BigFOOD.Services
{
    public class TipoCambioService : ITipoCambioService
    {
        private readonly HttpClient _httpClient;

        public TipoCambioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://apis.gometa.org/tdc/");
        }

        public async Task<TipoCambioDTO?> ObtenerTipoCambioDolarAsync()
        {
            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<TipoCambioDTO>("tdc.json");
                return resultado;
            }
            catch
            {
                return null;
            }
        }
    }
}
