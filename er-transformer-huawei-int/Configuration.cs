namespace er_transformer_huawei_int
{
    using er_transformer_huawei_int.Configurations.Attributes;
    using er_transformer_huawei_int.Data;
    using er_transformer_huawei_int.EndPoints;

    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IMongoRepository, MongoService>();
            builder.Services.AddScoped<ValidationFilterAttribute>();

            builder.Services.AddControllers();

            HuaweiEndpoints.Setconfiguration(builder.Services.BuildServiceProvider().GetService<IConfiguration>());
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
        }
    }
}