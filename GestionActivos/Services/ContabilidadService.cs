using GestionActivos.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GestionActivos.Services
{
    public class ContabilidadService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ContabilidadService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var urlBase = _configuration["IsoBackend:UrlBase"];
            if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith("/")) urlBase += "/";
            _httpClient.BaseAddress = new Uri(urlBase);
        }

        private async Task<string> AutenticarAsync()
        {
            var loginData = new LoginRequest
            {
                username = _configuration["IsoBackend:Username"],
                password = _configuration["IsoBackend:Password"]
            };

            var jsonContent = JsonSerializer.Serialize(loginData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/auth/login", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Falló el Login ({response.StatusCode}): {jsonResponse}");
            }

            var result = JsonSerializer.Deserialize<LoginResponse>(jsonResponse);
            return result?.data?.token;
        }

        // --- VERSIÓN CORREGIDA Y ROBUSTA ---
        public async Task<int> ObtenerCuentaIdValidaAsync()
        {
            var token = await AutenticarAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/v1/accounts");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al leer cuentas ({response.StatusCode}): {json}");
            }

            // Usamos JsonDocument para inspeccionar la respuesta sin importar su forma
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                JsonElement listaCuentas;

                // Caso A: El servidor devuelve la lista directa [...]
                if (root.ValueKind == JsonValueKind.Array)
                {
                    listaCuentas = root;
                }
                // Caso B: El servidor devuelve un objeto { "data": [...] } (Lo más probable)
                else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
                {
                    listaCuentas = dataProp;
                }
                else
                {
                    // Si el formato es raro, mostramos el JSON para saber qué pasó
                    throw new Exception($"El servidor devolvió un formato desconocido: {json}");
                }

                // Si encontramos cuentas, devolvemos la ID de la primera
                if (listaCuentas.GetArrayLength() > 0)
                {
                    // Buscamos el primer elemento y sacamos su "id"
                    return listaCuentas[0].GetProperty("id").GetInt32();
                }
            }

            throw new Exception("El catálogo de cuentas está vacío o no pude leerlo.");
        }

        public async Task<bool> EnviarAsientoAsync(AsientoRequest asiento)
        {
            var token = await AutenticarAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonSerializer.Serialize(asiento);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/accounting-entries", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Rechazado por el servidor ({response.StatusCode}): {error}");
            }

            return true;
        }
    }
}