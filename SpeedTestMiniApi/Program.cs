using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
var fullName = Assembly.GetExecutingAssembly().FullName;
Log.Information("Starting up {fullName}", fullName);

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();
    
    // Add services to the container.
    builder.Services.AddSingleton<ISpeedTestService, SpeedTestService>();
    // https://nblumhardt.com/2024/04/serilog-net8-0-minimal/
    // https://stackoverflow.com/questions/51480324/proper-way-to-register-hostedservice-in-asp-net-core-addhostedservice-vs-addsin
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#integration-with-hosts-and-dependency-injection
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-8.0
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/timer-service
    // http://localhost:5133/networkspeed/

    // Need somewhere to store the records.

    // builder.Services.AddHostedService<SpeedTestService>();
    builder.Services.AddHostedService(p => p.GetRequiredService<ISpeedTestService>());

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapGets();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Unhandled exception");
}
finally
{
    await Log.CloseAndFlushAsync();
}