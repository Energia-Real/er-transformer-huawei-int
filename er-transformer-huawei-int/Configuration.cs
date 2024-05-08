namespace er_transformer_huawei_int
{
    using er_transformer_huawei_int.BussinesLogic;
    using er_transformer_huawei_int.Configurations.Attributes;
    using er_transformer_huawei_int.Data;
    using er_transformer_huawei_int.EndPoints;
    using Microsoft.Extensions.Logging.ApplicationInsights;

    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) =>
            config.ConnectionString = "InstrumentationKey=a6821504-b42b-4326-87c0-c734c97e963f;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=117de288-5e68-49f0-8e27-34d357b348b1",
            configureApplicationInsightsLoggerOptions: (options) => { }
    );

            builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("HuaweiInverter", LogLevel.Trace);
            builder.Services.AddSingleton<IMongoRepository, MongoService>();
            builder.Services.AddScoped<ValidationFilterAttribute>();

            builder.Services.AddControllers();

            HuaweiEndpoints.Setconfiguration(builder.Services.BuildServiceProvider().GetService<IConfiguration>());

            HuaweiLogic.Setconfiguration(builder.Services.BuildServiceProvider().GetService<ILogger<HuaweiLogic>>());
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
        }
    }
}