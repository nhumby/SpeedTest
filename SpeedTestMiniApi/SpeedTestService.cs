public class SpeedTestService : ISpeedTestService
{
    private readonly ILogger<SpeedTestService> logger;
    private HttpClient? _httpClient;

    public SpeedTestService(ILogger<SpeedTestService> logger)
    {
        SpeedTestRecords.Records.Clear();
        ArgumentException.ThrowIfNullOrEmpty(nameof(logger));
        this.logger = logger;
        this.logger.LogInformation("New instance of SpeedTestService created.");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _httpClient = null;
        logger.LogInformation("SpeedTestService disposed.");
    }

    public IEnumerable<SpeedTestRecord> GetSpeedRecords()
    {
        var recordsCount = SpeedTestRecords.Records.Count;
        logger.LogInformation("SpeedTestService: GetSpeedRecords is returning {recordsCount} records.", recordsCount);
        return SpeedTestRecords.Records.AsReadOnly();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("SpeedTestService starting up.");
        _httpClient = new HttpClient();
        var count = 0;

        while (count++ < 3)
        {
            var recordedSpeed = await CheckInternetSpeed();
            SpeedTestRecords.Records.Add(new(DateTime.UtcNow, recordedSpeed));
            logger.LogInformation("SpeedTestService record added {recordedSpeed}.", recordedSpeed);
            Thread.Sleep(10000);
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