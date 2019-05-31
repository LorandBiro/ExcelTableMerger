using ExcelTableMerger.Excel;
using ExcelTableMerger.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class TableSelectorViewModel
    {
        public TableSelectorViewModel(ObservableProperty<ExcelWorkbook> workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException(nameof(workbook));
            }

            this.IsEnabled = new ObservableProperty<bool>();
            this.Tables = new ObservableProperty<IReadOnlyList<ExcelTable>>();
            this.Table = new ObservableProperty<ExcelTable>();

            workbook.Changed += this.Workbook_Changed;
        }

        private void Workbook_Changed(ExcelWorkbook workbook)
        {
            List<ExcelTable> tables = workbook.Tables.OrderBy(x => x.Name).ToList();
            this.Tables.Value = tables;
            if (tables.Count == 1)
            {
                this.Table.Value = tables[0];
            }
            else
            {
                this.Table.Value = null;
            }

            this.IsEnabled.Value = true;
        }

        public ObservableProperty<bool> IsEnabled { get; }

        public ObservableProperty<IReadOnlyList<ExcelTable>> Tables { get; }

        public ObservableProperty<ExcelTable> Table { get; }
    }
}
