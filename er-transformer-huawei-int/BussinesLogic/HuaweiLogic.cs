using er_transformer_huawei_int.Data;
using er_transformer_huawei_int.Models;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace er_transformer_huawei_int.BussinesLogic
{
    public class HuaweiLogic
    {
        private readonly MongoService mongoService;
        private readonly IConfiguration _configuration;

        public HuaweiLogic(IConfiguration configuration)
        {
            _configuration = configuration;
            mongoService = new MongoService(configuration);
        }


        public async Task<ResponseModel<string>> GetFiveMinutesResult(FiveMinutesRequest request, bool refresh = false)
        {
            var xsrfToken = await GetXsrfTokenAsync(refresh);

            if (string.IsNullOrEmpty(xsrfToken) || xsrfToken.Contains("Error"))
            {
                return new ResponseModel<string> { ErrorCode = 401, Success = false };
            }

            var url = "https://la5.fusionsolar.huawei.com/thirdData/getDevFiveMinutes";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("XSRF-TOKEN", xsrfToken);
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return new ResponseModel<string> { ErrorCode = -1, ErrorMessage = "No hay resultados disponibles.", Success = false };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return new ResponseModel<string> { Data = jsonResponse, Success = true };
        }

        public async Task<ResponseModel<string>> GetRealTimeResult(FiveMinutesRequest request, bool refresh = false)
        {
            var xsrfToken = await GetXsrfTokenAsync(refresh);

            if (string.IsNullOrEmpty(xsrfToken) || xsrfToken.Contains("Error"))
            {
                return new ResponseModel<string> { ErrorCode = 401, Success = false };
            }

            var url = "https://la5.fusionsolar.huawei.com/thirdData/getDevRealKpi";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("XSRF-TOKEN", xsrfToken);
            var content = new StringContent(JsonSerializer.Serialize(new { request.devIds, request.devTypeId }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return new ResponseModel<string> { ErrorCode = -1, ErrorMessage = "No hay resultados disponibles.", Success = false };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return new ResponseModel<string> { Data = jsonResponse, Success = true };
        }

        public async Task<ResponseModel<string>> GetHealtCheck(string StationCode, bool refresh = false)
        {
            var xsrfToken = await GetXsrfTokenAsync(refresh);

            if (string.IsNullOrEmpty(xsrfToken) || xsrfToken.Contains("Error"))
            {
                return new ResponseModel<string> { ErrorCode = 401, Success = false };
            }

            var url = "https://la5.fusionsolar.huawei.com/thirdData/getStationRealKpi";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("XSRF-TOKEN", xsrfToken);
            var content = new StringContent(JsonSerializer.Serialize(new { stationCodes = StationCode }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return new ResponseModel<string> { ErrorCode = (int)response.StatusCode, ErrorMessage = "No hay resultados disponibles.=> " + response.ReasonPhrase, Success = false };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return new ResponseModel<string> { Data = jsonResponse, Success = true };
        }

        public async Task<PlantList> GetPlantList()
        {
            var xsrfToken = await GetXsrfTokenAsync();

            if (string.IsNullOrEmpty(xsrfToken) || xsrfToken.Contains("Error"))
            {
                return null;
            }

            var url = "https://la5.fusionsolar.huawei.com/thirdData/stations";
            var jsonRequest = new { pageNo = 1 };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("XSRF-TOKEN", xsrfToken);
            var content = new StringContent(JsonSerializer.Serialize(jsonRequest), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PlantList>(jsonResponse);
        }

        public async Task<string> GetXsrfTokenAsync(bool refreshToken = false, bool requestToken = false)
        {
            // valida primero si hay un token en la bd de mongo
            // como tenemos dos usuarios par adiferentes nedpoint de manera provisional se generan dos consultas depediendo al variable
            var user = refreshToken ? "EnergiaReal" : "Ereal_interno";
            var tokenResponse = await this.mongoService.GetToken(user);
            
            if (tokenResponse.Count > 0 || requestToken)
            {
                // TODO: Pasar credenciales a un archivo de configuración y posteriormente a un configserver, tambien hay que quitar la logica del resfresh ya que el usuario en prod deberia de servir para todos los endpoint
                var loginJson = refreshToken ? "{\"userName\":\"EnergiaReal\",\"systemCode\":\"ERAPI@2021\"}" : "{\"userName\":\"Ereal_interno\",\"systemCode\":\"Prueba2024\"}";

                var loginUrl = "https://la5.fusionsolar.huawei.com/thirdData/login";

                using var loginClient = new HttpClient();
                var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

                var loginResponse = await loginClient.PostAsync(loginUrl, loginContent);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return $"Error al obtener el token XSRF-TOKEN: {loginResponse.StatusCode}";
                }

                try
                {
                    var responsetoken = loginResponse.Headers.GetValues("XSRF-TOKEN").FirstOrDefault() ?? string.Empty;
                    await this.mongoService.SetToken(responsetoken, user);
                    return responsetoken;
                }
                catch (Exception)
                {
                    return "Error -> No hay token disponible";
                }
            }

            return tokenResponse.First().Value;
        }

    }
}
