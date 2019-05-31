using System;

namespace ExcelTableMerger.Excel
{
    public sealed class ExcelColumn
    {
        public ExcelColumn(int index, string name)
        {
            this.Index = index;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Index { get; }

        public string Name { get; }

        public override string ToString() => this.Name;
    }
}
