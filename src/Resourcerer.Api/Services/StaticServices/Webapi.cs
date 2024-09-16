﻿using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Middlewares;

namespace Resourcerer.Api.Services.StaticServices;

public class Webapi
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AppInitializer.LoadAuthConfiguration(builder.Configuration);

        builder.Services.AddAllAppServices(builder.Environment, builder.Configuration);

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
            app.UseSwaggerUI(o =>
            {
                var descriptions = app.DescribeApiVersions();
                foreach(var description in descriptions)
                {
                    string url = $"/swagger/{description.GroupName}/swagger.json";
                    string name = description.GroupName.ToUpperInvariant();

                    o.SwaggerEndpoint(url, name);
                }
            });
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