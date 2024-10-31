public class SpeedTestService : ISpeedTestService
{
    private readonly ILogger logger;
    private HttpClient? _httpClient;

    public SpeedTestService(ILoggerFactory loggerFactory)
    {
        SpeedTestRecords.Records.Clear();
        ArgumentException.ThrowIfNullOrEmpty(nameof(loggerFactory));
        logger = loggerFactory.CreateLogger<SpeedTestService>();
        logger.LogInformation("Created.");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _httpClient = null;
        logger.LogInformation("Disposed.");
    }

    public IEnumerable<SpeedTestRecord> GetSpeedRecords()
    {
        logger.LogInformation("GetSpeedRecords is returning {_speedRecords.Count} records.", SpeedTestRecords.Records.Count);
        return SpeedTestRecords.Records.AsReadOnly();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting up.");
        _httpClient = new HttpClient();
        var count = 0;

        while (count++ < 3)
        {
            SpeedTestRecords.Records.Add(new(DateTime.UtcNow, await CheckInternetSpeed()));
            logger.LogInformation("Record added.");
            Thread.Sleep(1000);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {        
        Dispose();
        return Task.CompletedTask;
    }

    private async Task<double> CheckInternetSpeed()
    {
        if (_httpClient is null)
        {
            throw new Exception("HttpClient is null");
        }

        //DateTime Variable To Store Download Start Time.
        DateTime dt1 = DateTime.UtcNow;

        //Number Of Bytes Downloaded Are Stored In ‘data’
        byte[] data = await _httpClient.GetByteArrayAsync("http://google.com");

        //DateTime Variable To Store Download End Time.
        DateTime dt2 = DateTime.UtcNow;

        //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
        return Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
    }
}

public record struct SpeedTestRecord(DateTime DateTimeUtc, double kbps);

public static class SpeedTestRecords
{
    public static List<SpeedTestRecord> Records { get; set; } = [];
}