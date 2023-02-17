using Microsoft.EntityFrameworkCore;
using Resourcerer.Api;
using Resourcerer.DataAccess.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(cfg =>
    cfg.UseSqlite(AppInitializer.GetDbConnection(builder.Environment)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

