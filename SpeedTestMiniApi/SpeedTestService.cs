public class SpeedTestService : ISpeedTestService
{
    private readonly ILogger<SpeedTestService> logger;
    private HttpClient? _httpClient;
    private Timer? _timer;

    public SpeedTestService(ILogger<SpeedTestService> logger)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(logger));
        this.logger = logger;
        this.logger.LogInformation("New instance of SpeedTestService created.");
    }

    public void Dispose()
    {
        _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _timer?.Dispose();
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("SpeedTestService starting up.");
        SpeedTestRecords.Records.Clear();
        _httpClient = new HttpClient();
        _timer = new Timer(TimerElapsed, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private async void TimerElapsed(object? state)
    {
        var recordedSpeed = await CheckInternetSpeed();
        SpeedTestRecords.Records.Add(new(DateTime.UtcNow, recordedSpeed));
        logger.LogInformation("SpeedTestService record added {recordedSpeed}MBps.", recordedSpeed);
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
        byte[] data = await _httpClient.GetByteArrayAsync("http://google.com"); //"https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.403-windows-x64-installer"););

        //DateTime Variable To Store Download End Time.
        DateTime dt2 = DateTime.UtcNow;

        //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
        return Math.Round(((double)data.Length / 1024d / 1024d) / (dt2 - dt1).TotalSeconds, 2);
    }
}

public record struct SpeedTestRecord(DateTime DateTimeUtc, double MBps);

public static class SpeedTestRecords
{
    public static List<SpeedTestRecord> Records { get; set; } = [];
}