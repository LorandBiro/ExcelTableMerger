using ExcelTableMerger.Excel;
using System;
using System.Collections.Generic;

namespace ExcelTableMerger.Merge
{
    public sealed class DataSource
    {
        public DataSource(ExcelTable table, ExcelColumn key, Filter filter)
        {
            this.Table = table ?? throw new ArgumentNullException(nameof(table));
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
            this.Filter = filter;
        }

        public ExcelTable Table { get; }

        public ExcelColumn Key { get; }

        public Filter Filter { get; }

        public IEnumerable<ExcelRow> GetRows() => Filter.GetRows(this.Table, this.Filter);
    }
}
