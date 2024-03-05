using er_transformer_huawei_int.Configurations.Swagger;
using er_transformer_huawei_int.Configurations;
using er_transformer_huawei_int;
using er_transformer_huawei_int.EndPoints;

var builder = WebApplication.CreateBuilder(args);
Authentication.Config(ref builder);
Swagger.Config(ref builder);
builder.RegisterServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.RegisterMiddlewares();
app.RegisterHuaweiEndpoints();
app.Run();