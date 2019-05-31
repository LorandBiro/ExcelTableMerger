using System;
using System.Collections.Generic;

namespace ExcelTableMerger.Merge
{
    public sealed class MergedRow
    {
        public MergedRow(int index, IReadOnlyList<MergedCell> cells, MergeKind kind)
        {
            this.Index = index;
            this.Cells = cells ?? throw new ArgumentNullException(nameof(cells));
            this.Kind = kind;
        }

        public int Index { get; }

        public IReadOnlyList<MergedCell> Cells { get; }

        public MergeKind Kind { get; }
    }
}
