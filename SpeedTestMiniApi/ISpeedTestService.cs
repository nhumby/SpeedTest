public interface ISpeedTestService : IHostedService, IDisposable
{
    IEnumerable<SpeedTestRecord> GetSpeedRecords();
}
