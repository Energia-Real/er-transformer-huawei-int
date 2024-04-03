namespace er_transformer_huawei_int
{
    using er_transformer_huawei_int.Configurations.Attributes;

    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ValidationFilterAttribute>();

            builder.Services.AddControllers();
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
        }
    }
}