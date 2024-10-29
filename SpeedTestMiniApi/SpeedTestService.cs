public class SpeedTestService : ISpeedTestService
{
    private readonly List<double> _speedRecords;
    private HttpClient? _httpClient;

    public SpeedTestService()
    {
        _speedRecords = [];
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _httpClient = null;
    }

    public IEnumerable<double> GetSpeedRecords()
    {
        return _speedRecords.AsReadOnly();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _httpClient = new HttpClient();

        while (true)
        {
            _speedRecords.Add(await CheckInternetSpeed());
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