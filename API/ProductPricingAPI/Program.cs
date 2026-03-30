using Microsoft.AspNetCore.Diagnostics;
using ProductPricingAPI.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IProductDataRepository, ProductDataRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// This is added, so we can allow the Blazor web app to call the API and restrict it to that web app only (allowed origins in appsettings.json).
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader());
});
var app = builder.Build();

// This is added, so we can handle and display exceptions in a friendly way when the API is called - (there may be a better way to do this)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        (int statusCode, string message) = exception switch
        {
            KeyNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            Exception ex => (StatusCodes.Status500InternalServerError, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new { error = message });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
