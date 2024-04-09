using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Services;

namespace Resourcerer.Api;

public class Webapi
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AppInitializer.LoadConfiguration(builder.Configuration);

        builder.Services.AddAspNetServices();
        builder.Services.AddAppServices();
        builder.Services.Add3rdParyServices(builder.Environment);

        builder.Host.UseDefaultServiceProvider((_, options) =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        return builder.Build();
    }

    public static void Run(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        EndpointMapper.Map(app);

        app.UseMiddleware<AppHttpMiddleware>();

        app.Run();
    }
}
