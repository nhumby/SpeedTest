public class NetworkSpeedReader
{
    private readonly ISpeedTestService speedTestService;

    public NetworkSpeedReader(ISpeedTestService speedTestService)
    {
        this.speedTestService = speedTestService ?? 
            throw new ArgumentNullException(nameof(speedTestService));
    }

    public IEnumerable<SpeedTestRecord> GetRecords()
    {
        return speedTestService.GetSpeedRecords();
    }
}