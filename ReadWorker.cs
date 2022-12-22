using Microsoft.Extensions.Options;

namespace postgresql_worker

{
    public sealed class ReadWorker : Worker
    {

        public ReadWorker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker>? logger, IOptions<PostgreSQLConfiguration> options) :
        base(hostApplicationLifetime, logger, options)
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (; stoppingToken.IsCancellationRequested == false;)
            {
                await read();
                await Task.Delay(ReadDelayTime);
            }
            _hostApplicationLifetime.StopApplication();
        }
    }
}