using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels.Common;
using System;
using System.Windows;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class FilterSelectorViewModel
    {
        private readonly ObservableProperty<ExcelTable> table;

        public FilterSelectorViewModel(ObservableProperty<ExcelTable> table)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));

            this.Filter = new ObservableProperty<Filter>();
            this.SetFilterCommand = new Command(this.SetFilter, false);
            this.ClearFilterCommand = new Command(this.ClearFilter, false);

            this.table.Changed += this.Table_Changed;
        }

        public ObservableProperty<Filter> Filter { get; }

        public Command SetFilterCommand { get; }

        public Command ClearFilterCommand { get; }

        private void Table_Changed(ExcelTable table)
        {
            this.Filter.Value = null;
            this.SetFilterCommand.CanExecute = table != null;
            this.ClearFilterCommand.CanExecute = false;
        }

        private void SetFilter()
        {
            SetFilterWindow setFilterWindow = new SetFilterWindow(this.table.Value.Columns) { Owner = Application.Current.MainWindow };
            if (setFilterWindow.ShowDialog() == true)
            {
                this.Filter.Value = setFilterWindow.Filter;
                this.SetFilterCommand.CanExecute = false;
                this.ClearFilterCommand.CanExecute = true;
            }
        }

        private void ClearFilter()
        {
            this.Filter.Value = null;
            this.SetFilterCommand.CanExecute = true;
            this.ClearFilterCommand.CanExecute = false;
        }
    }
}
