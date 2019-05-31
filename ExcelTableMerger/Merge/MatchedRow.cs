using ExcelTableMerger.Excel;
using System;

namespace ExcelTableMerger.Merge
{
    public sealed class MatchedRow
    {
        public MatchedRow(ExcelRow main, ExcelRow lookup)
        {
            if (main == null && lookup == null)
            {
                throw new ArgumentNullException(nameof(main), "Main or lookup must be specified.");
            }

            this.Main = main;
            this.Lookup = lookup;
        }

        public ExcelRow Main { get; }

        public ExcelRow Lookup { get; }
    }
}
