using System.Collections.Generic;

namespace ExcelTableMerger.Configuration
{
    public sealed class Config
    {
        public string LastMainFilePath { get; set; }

        public string LastLookupFilePath { get; set; }

        public Dictionary<string, LastConfiguration> LastConfigurations { get; set; } = new Dictionary<string, LastConfiguration>();
    }
}
