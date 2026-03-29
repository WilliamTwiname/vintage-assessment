using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace ProductAPI.Tests;

[TestFixture]
public class WeatherForecastIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsOk()
    {
        var response = await _client.GetAsync("/weatherforecast");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsJsonContentType()
    {
        var response = await _client.GetAsync("/weatherforecast");

        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task GetWeatherForecast_ReturnsFiveItems()
    {
        var forecasts = await _client.GetFromJsonAsync<WeatherForecastDto[]>("/weatherforecast");

        Assert.That(forecasts, Is.Not.Null);
        Assert.That(forecasts, Has.Length.EqualTo(5));
    }

    [Test]
    public async Task GetWeatherForecast_ItemsHaveSummary()
    {
        var forecasts = await _client.GetFromJsonAsync<WeatherForecastDto[]>("/weatherforecast");

        Assert.That(forecasts, Is.Not.Null);
        Assert.That(forecasts, Has.All.Matches<WeatherForecastDto>(f => f.Summary != null));
    }

    [Test]
    public async Task GetOpenApiDoc_ReturnsOk()
    {
        var response = await _client.GetAsync("/openapi/v1.json");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private record WeatherForecastDto(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);
}
