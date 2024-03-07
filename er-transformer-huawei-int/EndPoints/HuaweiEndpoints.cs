using er.library.dto.Response;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using er_transformer_huawei_int.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

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

                var filteredPlant = plantList.Data.List.FirstOrDefault(a=>a.plantCode == StationCode);

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
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<PlantList>(jsonResponse);
            }
            else
            {
                return null;
            }
        }

        private static async Task<string> GetXsrfTokenAsync()
        {
            var loginUrl = "https://la5.fusionsolar.huawei.com/thirdData/login";
            var loginJson = "{\"userName\":\"Ereal_interno\",\"systemCode\":\"Prueba2024\"}";

            using var loginClient = new HttpClient();
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await loginClient.PostAsync(loginUrl, loginContent);
            if (!loginResponse.IsSuccessStatusCode)
            {
                return $"Error al obtener el token XSRF-TOKEN: {loginResponse.StatusCode}";
            }

            return loginResponse.Headers.GetValues("XSRF-TOKEN").FirstOrDefault() ?? string.Empty;
        }
    }
}
