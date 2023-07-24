﻿using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Services;

namespace Resourcerer.Api;

public class Webapi
{
    public static WebApplication Create(string[] args)
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

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        EndpointMapper.Map(app);

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseMiddleware<AppHttpMiddleware>();

        return app;
    }
}
