using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
Log.Information("Starting up {Assembly.GetExecutingAssembly().FullName}", Assembly.GetExecutingAssembly().FullName);

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();

    // Add services to the container.
    //builder.Services.AddSingleton<List<SpeedTestRecord>>();
    // https://nblumhardt.com/2024/04/serilog-net8-0-minimal/
    // https://stackoverflow.com/questions/51480324/proper-way-to-register-hostedservice-in-asp-net-core-addhostedservice-vs-addsin
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service

    builder.Services.AddHostedService<SpeedTestService>();

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