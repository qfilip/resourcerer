using Microsoft.AspNetCore.Http.Json;
using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Middlewares;
using Scalar.AspNetCore;

namespace Resourcerer.Api;

public class Webapi
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<JsonOptions>(x =>
        {
            // don't return recursive types
            // ReferenceHandler adds $id property
            // x.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });

        DependencyInjection.RegisterAppServices(builder);

        builder.Host.UseDefaultServiceProvider((_, options) =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        return builder.Build();
    }

    public static async void Run(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            var descriptions = app.DescribeApiVersions();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        EndpointMapper.Map(app);

        app.UseMiddleware<AppHttpMiddleware>();

        app.Run();
        // var task = app.RunAsync();

        //var services = app.Services.GetServices<EndpointDataSource>()
        //    .SelectMany(e => e.Endpoints)
        //    .ToArray();

        //await task;
    }
}
