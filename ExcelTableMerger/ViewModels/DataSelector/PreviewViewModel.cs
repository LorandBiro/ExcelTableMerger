using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class PreviewViewModel
    {
        private readonly ObservableProperty<ExcelTable> table;
        private readonly ObservableProperty<Filter> filter;

        public PreviewViewModel(ObservableProperty<ExcelTable> table, ObservableProperty<Filter> filter)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));

            this.Columns = new ObservableProperty<IReadOnlyList<ExcelColumn>>();
            this.Rows = new ObservableProperty<IReadOnlyList<ExcelRow>>();

            this.table.Changed += x => this.TableOrFilter_Changed();
            this.filter.Changed += x => this.TableOrFilter_Changed();
        }

        public ObservableProperty<IReadOnlyList<ExcelColumn>> Columns { get; }

        public ObservableProperty<IReadOnlyList<ExcelRow>> Rows { get; }

        private void TableOrFilter_Changed()
        {
            this.Rows.Value = null;
            this.Columns.Value = null;
            if (this.table.Value == null)
            {
                return;
            }

            this.Columns.Value = this.table.Value.Columns;
            this.Rows.Value = Filter.GetRows(this.table.Value, this.filter.Value).ToList();
        }
    }
}
