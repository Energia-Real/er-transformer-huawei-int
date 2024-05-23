using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using er_transformer_huawei_int.Models;
using er_transformer_huawei_int.Enums;
using er_transformer_huawei_int.BussinesLogic;
using System.Runtime.CompilerServices;

namespace er_transformer_huawei_int.EndPoints
{
    [Authorize]
    public static class HuaweiEndpoints
    {
        private static IConfiguration _configuration;
        public static void Setconfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void RegisterHuaweiEndpoints(this IEndpointRouteBuilder routes)
        {
            var routeBuilder = routes.MapGroup("/api/v1/integrators/huawei");

            GetPlantListMethod(routeBuilder);
            GetDevListMethod(routeBuilder);
            GetPlantMethod(routeBuilder);
            GetTheLastFiveMinutes(routeBuilder);
            GetRealTimeInfo(routeBuilder);
            GetStationHealtCheck(routeBuilder);
            GetMonthResume(routeBuilder);
        }

        private static void GetPlantListMethod(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/GetPlantList", async (HttpContext context, [FromBody] int PageNo) =>
            {

                var plantList = await new HuaweiLogic(_configuration).GetPlantList();

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
                var xsrfToken = await new HuaweiLogic(_configuration).GetXsrfTokenAsync();

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
                var plantList = await new HuaweiLogic(_configuration).GetPlantList();

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

        private static void GetTheLastFiveMinutes(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/LastFiveMinutes", async (HttpContext context, [FromBody] FiveMinutesRequest request) =>
            {
                var result = await new HuaweiLogic(_configuration).GetFiveMinutesResult(request);

                if (result.ErrorCode == 401)
                {
                    result = await new HuaweiLogic(_configuration).GetFiveMinutesResult(request, true);
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
                var result = await new HuaweiLogic(_configuration).GetRealTimeResult(request);

                if (result.ErrorCode == 401)
                {
                    result = await new HuaweiLogic(_configuration).GetRealTimeResult(request, true);
                }

                return Results.Ok(result);
            })
            .Produces(200, typeof(ResponseModel<string>))
            .Produces(204)
            .WithTags("huawei")
            .WithName("realTimeInfo")
            .WithOpenApi();
        }

        private static void GetStationHealtCheck(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/getStationHealtCheck", async (HttpContext context, [FromHeader] string StationCode) =>
            {
                var result = await new HuaweiLogic(_configuration).GetHealtCheck(StationCode);

                if (result.ErrorCode == 401)
                {
                    result = await new HuaweiLogic(_configuration).GetHealtCheck(StationCode, true);
                }

                return Results.Ok(result);
            })
            .Produces(200, typeof(ResponseModel<string>))
            .Produces(204)
            .WithTags("huawei")
            .WithName("getStationHealtCheck")
            .WithOpenApi();
        }

        private static void GetMonthResume(RouteGroupBuilder rgb)
        {
            rgb.MapPost("/GetMonthResume", async (HttpContext context, [FromBody] StationAndCollectTimeRequest request) =>
            {
                var result = await new HuaweiLogic(_configuration).GetMonthResumeResult(request);

                if (result.ErrorCode == 401)
                {
                    result = await new HuaweiLogic(_configuration).GetMonthResumeResult(request, true);
                }

                return Results.Ok(result);
            })
            .Produces(200, typeof(ResponseModel<string>))
            .Produces(204)
            .WithTags("huawei")
            .WithName("GetMonthResume")
            .WithOpenApi();
        }

    }
}
