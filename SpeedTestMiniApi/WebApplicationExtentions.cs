using Microsoft.AspNetCore.Mvc;

public static class WebApplicationExtentions
{
    public static WebApplication MapGets(this WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapGet("/networkspeed", ([FromServices] ISpeedTestService speedTestService) =>
        {
            return new NetworkSpeedReader(speedTestService).GetRecords();
        })
        .WithName("NetworkSpeed")
        .WithOpenApi();

        return app;
    }
}