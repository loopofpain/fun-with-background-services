using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace TheWindowsService.Test2.Csv
{
    public class CustomCsvWriter
    {
        private string pathToCurrentCsvFileName;

        public void InitializeNewFile()
        {
            if (!Directory.Exists("export-csv"))
            {
                Directory.CreateDirectory("export-csv");
            }

            this.pathToCurrentCsvFileName = $"export-csv//{DateTime.UtcNow.ToString("o")}-export-batch.csv";
        
            using (var writer = new StreamWriter(this.pathToCurrentCsvFileName,false))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CsvEntryMap>();
                csv.WriteHeader<CsvEntry>();
            }
        }

        public void FinalizeFile()
        {
            this.pathToCurrentCsvFileName = null;
        }

        public void AppendRecordAsync(CsvEntry entries)
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
            }
        }
    }
}