using Resourcerer.Api.Endpoints;
using Resourcerer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAspNetServices();
builder.Services.AddAppServices(builder.Environment);
builder.Services.Add3rdParyServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Users.MapEndpoints(app);
Categories.MapEndpoints(app);
Mocks.MapEndpoints(app);

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

