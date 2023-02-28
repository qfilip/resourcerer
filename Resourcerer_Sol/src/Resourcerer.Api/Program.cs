using Microsoft.EntityFrameworkCore;
using Resourcerer.Api;
using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(cfg =>
    cfg.UseSqlite(AppInitializer.GetDbConnection(builder.Environment)));

builder.Services.AddAppServices(builder.Environment);
builder.Services.Add3rdParyServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

Users.MapEndpoints(app);
Categories.MapEndpoints(app);
Mocks.MapEndpoints(app);

app.Run();

