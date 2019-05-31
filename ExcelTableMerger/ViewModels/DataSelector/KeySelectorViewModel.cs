using ExcelTableMerger.Excel;
using ExcelTableMerger.ViewModels.Common;
using System;
using System.Collections.Generic;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class KeySelectorViewModel
    {
        public KeySelectorViewModel(ObservableProperty<ExcelTable> table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            this.IsEnabled = new ObservableProperty<bool>();
            this.Keys = new ObservableProperty<IReadOnlyList<ExcelColumn>>();
            this.Key = new ObservableProperty<ExcelColumn>();

            table.Changed += Table_Changed;
        }

        private void Table_Changed(ExcelTable table)
        {
            if (table == null)
            {
                this.IsEnabled.Value = false;
                this.Keys.Value = null;
            }
            else
            {
                this.IsEnabled.Value = true;
                this.Keys.Value = table.Columns;
            }
        }

        public ObservableProperty<bool> IsEnabled { get; }

        public ObservableProperty<IReadOnlyList<ExcelColumn>> Keys { get; }

        public ObservableProperty<ExcelColumn> Key { get; }
    }
}
