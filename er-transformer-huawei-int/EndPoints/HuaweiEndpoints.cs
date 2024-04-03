using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using er_transformer_huawei_int.Models;
using er_transformer_huawei_int.Enums;

namespace er_transformer_huawei_int.EndPoints
{
    [Authorize]
    public static class HuaweiEndpoints
    {
        public static void RegisterHuaweiEndpoints(this IEndpointRouteBuilder routes)
        {
            var routeBuilder = routes.MapGroup("/api/v1/integrators/huawei");

            GetPlantListMethod(routeBuilder);
            GetDevListMethod(routeBuilder);
            GetPlantMethod(routeBuilder);
            GetTheLastFiveMinutes(routeBuilder);
            GetRealTimeInfo(routeBuilder);
        }

        private static void GetPlantListMethod(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/GetPlantList", async (HttpContext context, [FromBody] int PageNo) =>
            {
                var plantList = await GetPlantList();

                if (plantList is null)
                {
                    return Results.NoContent();
                }

                return Results.Ok(plantList);
            })
            .Produces(200, typeof(PlantList))
            .Produces(204)
            .WithTags("huawei")
            .WithName("GetPlantList")
            .WithOpenApi();
        }

        private static void GetDevListMethod(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/getDevList", async (HttpContext context, [FromBody] string StationCode) =>
            {
                var xsrfToken = await GetXsrfTokenAsync();

                if (string.IsNullOrEmpty(xsrfToken) || xsrfToken.Contains("Error"))
                {
                    return Results.Unauthorized();
                }

                var url = "https://la5.fusionsolar.huawei.com/thirdData/getDevList";
                var jsonRequest = new { stationCodes = StationCode };

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("XSRF-TOKEN", xsrfToken);
                var content = new StringContent(JsonSerializer.Serialize(jsonRequest), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deviceData = JsonSerializer.Deserialize<DeviceData>(jsonResponse);
                    return Results.Ok(deviceData);
                }
                else
                {
                    await context.Response.WriteAsync($"Error: {response.StatusCode}");
                    return Results.NoContent();
                }
            })
            .Produces(200, typeof(DeviceData))
            .Produces(204)
            .WithTags("huawei")
            .WithName("getDevList")
            .WithOpenApi();
        }


        private static void GetPlantMethod(RouteGroupBuilder rgb)
        {
            rgb.MapGet("/GetPlant", async (HttpContext context, [FromHeader] string StationCode) =>
            {
                var plantList = await GetPlantList();

                if (plantList is null)
                {
                    return Results.NoContent();
                }

                var filteredPlant = plantList.Data.List.FirstOrDefault(a => a.plantCode == StationCode);

                if (filteredPlant is null)
                {
                    return Results.NoContent();
                }

                return Results.Ok(filteredPlant);
            })
            .Produces(200, typeof(PlantList))
            .Produces(204)
            .WithTags("huawei")
            .WithName("GetPlant")
            .WithOpenApi();
        }


        private static async Task<PlantList> GetPlantList()
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

        private static async Task<string> GetXsrfTokenAsync(bool refreshToken = false)
        {
            // TODO: Pasar credenciales a un archivo de configuración y posteriormente a un configserver
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
                return loginResponse.Headers.GetValues("XSRF-TOKEN").FirstOrDefault() ?? string.Empty;
            }
            catch (Exception)
            {
                return "Error -> No hay token disponible";
            }
        }

        private static void GetTheLastFiveMinutes(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/LastFiveMinutes", async (HttpContext context, [FromBody] FiveMinutesRequest request) =>
            {
                var result = await GetFiveMinutesResult(request);

                if (result.ErrorCode == 401)
                {
                    result = await GetFiveMinutesResult(request, true);
                }

                return Results.Ok(result);
            })
            .Produces(200, typeof(ResponseModel<string>))
            .Produces(204)
            .WithTags("huawei")
            .WithName("LastFiveMinutes")
            .WithOpenApi();
        }

        private static void GetRealTimeInfo(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/realTimeInfo", async (HttpContext context, [FromBody] FiveMinutesRequest request) =>
            {
                var result = await GetRealTimeResult(request);

                if (result.ErrorCode == 401)
                {
                    result = await GetRealTimeResult(request, true);
                }

                return Results.Ok(result);
            })
            .Produces(200, typeof(ResponseModel<string>))
            .Produces(204)
            .WithTags("huawei")
            .WithName("realTimeInfo")
            .WithOpenApi();
        }

        private static async Task<ResponseModel<string>> GetFiveMinutesResult(FiveMinutesRequest request, bool refresh = false)
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

        private static async Task<ResponseModel<string>> GetRealTimeResult(FiveMinutesRequest request, bool refresh = false)
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
    }
}
