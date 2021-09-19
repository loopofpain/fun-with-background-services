using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace TheWindowsService.Test2.Csv
{
    public class CustomCsvWriter
    {
        private string pathToCurrentCsvFileName;

        public async Task InitializeNewFileAsync()
        {
            if (!Directory.Exists("export-csv"))
            {
                Directory.CreateDirectory("export-csv");
            }

            this.pathToCurrentCsvFileName = $"export-csv//{DateTime.Now.ToString("s")}-export-batch.csv";
        
            using (var writer = new StreamWriter(this.pathToCurrentCsvFileName,false))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CsvEntryMap>();
                csv.WriteHeader<CsvEntry>();

                await writer.WriteAsync(csv.Configuration.NewLine);
            }
        }

        public void FinalizeFile()
        {
            this.pathToCurrentCsvFileName = null;
        }

        public async Task AppendRecordAsync(CsvEntry entries)
        {
            // Append to the file.
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
            };
            using (var stream = File.Open(this.pathToCurrentCsvFileName, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<CsvEntryMap>();
                csv.WriteRecord(entries);
                
                await writer.WriteAsync(csv.Configuration.NewLine);
            }
        }
    }
}