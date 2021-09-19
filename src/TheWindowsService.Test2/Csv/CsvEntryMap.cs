using System;
using CsvHelper.Configuration;

namespace TheWindowsService.Test2.Csv {
    public class CsvEntryMap:ClassMap<CsvEntry> {
        public CsvEntryMap()
        {
            this.Map(m => m.Id).Index(0).Name("id");
            this.Map(m => m.Name).Index(0).Name("name");
        }
    }
}