public interface ISpeedTestService : IHostedService, IDisposable
{
    IEnumerable<double> GetSpeedRecords();
}
