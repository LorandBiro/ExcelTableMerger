using ExcelTableMerger.Excel;
using System;

namespace ExcelTableMerger.Merge
{
    public sealed class Mapping
    {
        public Mapping(ExcelColumn mainColumn, ExcelColumn lookupColumn, bool overwrite)
        {
            this.MainColumn = mainColumn ?? throw new ArgumentNullException(nameof(mainColumn));
            this.LookupColumn = lookupColumn ?? throw new ArgumentNullException(nameof(lookupColumn));
            this.Overwrite = overwrite;
        }

        public ExcelColumn MainColumn { get; }

        public ExcelColumn LookupColumn { get; }

        public bool Overwrite { get; }
    }
}
