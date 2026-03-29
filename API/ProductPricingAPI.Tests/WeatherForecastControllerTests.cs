using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ProductAPI.Controllers;

namespace ProductAPI.Tests;

[TestFixture]
public class WeatherForecastControllerTests
{
    private WeatherForecastController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        var logger = new LoggerFactory().CreateLogger<WeatherForecastController>();
        _controller = new WeatherForecastController(logger);
    }

    [Test]
    public void Get_ReturnsWeatherForecasts()
    {
        var result = _controller.Get();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void Get_ReturnsFiveForecasts()
    {
        var result = _controller.Get().ToList();

        Assert.That(result, Has.Count.EqualTo(5));
    }

    [Test]
    public void Get_ForecastDatesAreFuture()
    {
        var result = _controller.Get().ToList();
        var today = DateOnly.FromDateTime(DateTime.Now);

        Assert.That(result, Has.All.Matches<WeatherForecast>(f => f.Date > today));
    }

    [Test]
    public void Get_TemperatureFConversionIsCorrect()
    {
        var result = _controller.Get().ToList();

        Assert.Multiple(() =>
        {
            foreach (var forecast in result)
            {
                var expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
                Assert.That(forecast.TemperatureF, Is.EqualTo(expectedF));
            }
        });
    }

    [Test]
    public void Get_SummaryIsNotNull()
    {
        var result = _controller.Get().ToList();

        Assert.That(result, Has.All.Matches<WeatherForecast>(f => f.Summary != null));
    }
}
