using System;

namespace TheWindowsService.Test2.Csv {
    public class CsvEntry {

        public CsvEntry()
        {
            this.Id = Guid.NewGuid();
            this.Name = "My name is "+ this.Id;
        }

        public Guid Id {get ;set;}
        public string Name {get;set;}
    }
}