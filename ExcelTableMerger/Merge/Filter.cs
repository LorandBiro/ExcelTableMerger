using ExcelTableMerger.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.Merge
{
    public sealed class Filter
    {
        public Filter(ExcelColumn column, string value)
        {
            this.Column = column ?? throw new ArgumentNullException(nameof(column));
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ExcelColumn Column { get; }

        public string Value { get; }

        public override string ToString() => $"[@{this.Column}] = \"{this.Value}\"";

        public static IEnumerable<ExcelRow> GetRows(ExcelTable table, Filter filter)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            IEnumerable<ExcelRow> rows = table.Rows;
            if (filter != null)
            {
                rows = rows.Where(x => Cell.Equal(x[filter.Column], filter.Value));
            }

            return rows;
        }
    }
}
