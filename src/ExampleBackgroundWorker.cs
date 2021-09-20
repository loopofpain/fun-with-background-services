using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestService.Csv;

namespace TestService
{
    public class ExampleBackgroundWorker : BackgroundService
    {
        private readonly ILogger<ExampleBackgroundWorker> logger;
        private readonly CustomCsvWriter customCsvWriter = new CustomCsvWriter();
        private readonly IHostApplicationLifetime hostApplicationLifetime ;

        public ExampleBackgroundWorker(ILogger<ExampleBackgroundWorker> logger,
        IHostApplicationLifetime hostApplicationLifetime)
        {
            this.hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task DoWorkAsync()
        {
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

                await Task.Delay(1 * 1000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try{
                await this.DoWorkAsync();
            }catch {

            }finally {
                this.hostApplicationLifetime.StopApplication();
            }
        }
    }
}