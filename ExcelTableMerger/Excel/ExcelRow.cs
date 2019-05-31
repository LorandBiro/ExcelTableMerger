using System;
using System.Collections;
using System.Collections.Generic;

namespace ExcelTableMerger.Excel
{
    public sealed class ExcelRow : IEnumerable<object>
    {
        private readonly IReadOnlyList<object> values;

        public ExcelRow(int index, IReadOnlyList<object> values)
        {
            this.Index = index;
            this.values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public int Index { get; }

        public int Length => this.values.Count;

        public object this[int i] => this.values[i];

        public object this[ExcelColumn column] => this.values[column.Index];

        public IEnumerator<object> GetEnumerator() => this.values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
