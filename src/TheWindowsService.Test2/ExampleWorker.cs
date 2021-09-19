using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheWindowsService.Test2.Csv;

namespace TheWindowsService.Test2
{
    public class ExampleWorker : IHostedService, IDisposable
    {
        private readonly ILogger<ExampleWorker> logger;
        private readonly ExampleWorkerOptions exampleWorkerOptions;
        private Timer timer;
        private readonly CustomCsvWriter customCsvWriter = new CustomCsvWriter();

        private bool isProcessing = false;

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
                var dueTime = TimeSpan.FromSeconds(this.exampleWorkerOptions.IntervalInSeconds);

                if (this.exampleWorkerOptions.TimedStart is not null)
                {
                    dueTime = DateTime.Today.AddHours(this.exampleWorkerOptions.TimedStart.Hours).AddMinutes(this.exampleWorkerOptions.TimedStart.Minutes) - DateTime.Now;
                }

                this.logger.LogInformation($"Next execution time: {DateTime.Now.Add(dueTime)}");
                this.timer = new Timer(DoWorkAsync, null, dueTime, TimeSpan.FromSeconds(this.exampleWorkerOptions.IntervalInSeconds));
            }

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            if (!this.isProcessing)
            {
                this.isProcessing = true;
                this.logger.LogInformation("Doing stuff....");

                var entries = new List<CsvEntry>();

                for (int i = 0; i < 500; i++)
                {
                    entries.Add(new CsvEntry());
                }

                await this.customCsvWriter.InitializeNewFileAsync();

                for (int i = 0; i < entries.Count(); i++)
                {
                    await this.customCsvWriter.AppendRecordAsync(entries[i]);
                }

                this.customCsvWriter.FinalizeFile();

                await Task.Delay(30 * 1000);

                this.isProcessing = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Hosted Service is stopping.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}