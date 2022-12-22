using Microsoft.Extensions.Options;

namespace postgresql_worker

{
    public sealed class InsertWorker : Worker
    {

        public InsertWorker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker>? logger, IOptions<PostgreSQLConfiguration> options) :
        base(hostApplicationLifetime, logger, options)
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (; stoppingToken.IsCancellationRequested == false;)
            {
                try
                {
                    await insert();
                    await Task.Delay(InsertDelayTime);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.StackTrace);
                    continue;
                }
            }
            _hostApplicationLifetime.StopApplication();
        }
    }
}