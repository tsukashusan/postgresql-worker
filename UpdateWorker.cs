using Microsoft.Extensions.Options;

namespace postgresql_worker

{
    public sealed class UpdateWorker : Worker
    {

        public UpdateWorker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker>? logger, IOptions<PostgreSQLConfiguration> options) :
        base(hostApplicationLifetime, logger, options)
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (; stoppingToken.IsCancellationRequested == false;)
            {
                try
                {
                    await update();
                    await Task.Delay(UpdateDelayTime);
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