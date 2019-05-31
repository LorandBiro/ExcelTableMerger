using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels.Common;
using ExcelTableMerger.ViewModels.DataSelector;

namespace ExcelTableMerger.ViewModels
{
    public sealed class DataSelectorViewModel
    {
        public DataSelectorViewModel(string filePathConfigurationKey)
        {
            this.WorkbookSelector = new WorkbookSelectorViewModel(filePathConfigurationKey);
            this.TableSelector = new TableSelectorViewModel(this.WorkbookSelector.Workbook);
            this.FilterSelector = new FilterSelectorViewModel(this.TableSelector.Table);
            this.KeySelector = new KeySelectorViewModel(this.TableSelector.Table);
            this.Preview = new PreviewViewModel(this.TableSelector.Table, this.FilterSelector.Filter);
            this.DataSource = ObservableProperty.Create(
                this.TableSelector.Table,
                this.KeySelector.Key,
                this.FilterSelector.Filter,
                (table, key, filter) => table != null && key != null ? new DataSource(table, key, filter) : null);
        }

        public WorkbookSelectorViewModel WorkbookSelector { get; }

        public TableSelectorViewModel TableSelector { get; }

        public FilterSelectorViewModel FilterSelector { get; }

        public KeySelectorViewModel KeySelector { get; }

        public PreviewViewModel Preview { get; }

        public IObservableProperty<DataSource> DataSource { get; }
    }
}
