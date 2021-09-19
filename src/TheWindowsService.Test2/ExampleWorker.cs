using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TheWindowsService.Test2
{
    public class ExampleWorker : IHostedService, IDisposable
    {
        private readonly ILogger<ExampleWorker> logger;
        private readonly ExampleWorkerOptions exampleWorkerOptions;
        private Timer timer;

        public ExampleWorker(ILogger<ExampleWorker> logger, IOptions<ExampleWorkerOptions> exampleWorkerOptions)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.exampleWorkerOptions = exampleWorkerOptions.Value ?? throw new ArgumentNullException(nameof(exampleWorkerOptions));
        }


        public void Dispose()
        {
            this.timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Hosted Service running.");


            if (this.exampleWorkerOptions.ExecuteTaskOnStartup)
            {
                this.logger.LogInformation("Excecuting task on startup...");
                this.timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            }
            else
            {
                this.logger.LogInformation($"Next execution time: {DateTimeOffset.UtcNow.AddSeconds(this.exampleWorkerOptions.IntervalInSeconds)}");
                this.timer = new Timer(DoWorkAsync, null, TimeSpan.FromSeconds(this.exampleWorkerOptions.IntervalInSeconds), TimeSpan.FromSeconds(this.exampleWorkerOptions.IntervalInSeconds));
            }

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            this.logger.LogInformation("Executing stuff....");
            
            await Task.Delay(77*1000);

            this.logger.LogInformation($"Next execution time: {DateTimeOffset.UtcNow.AddSeconds(this.exampleWorkerOptions.IntervalInSeconds)}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Hosted Service is stopping.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}