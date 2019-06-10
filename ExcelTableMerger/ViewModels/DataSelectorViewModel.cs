using ExcelTableMerger.Configuration;
using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels.Common;
using ExcelTableMerger.ViewModels.DataSelector;
using System;
using System.Linq;

namespace ExcelTableMerger.ViewModels
{
    public sealed class DataSelectorViewModel : IStep
    {
        public DataSelectorViewModel(string title, bool isMain)
        {
            this.WorkbookSelector = new WorkbookSelectorViewModel(isMain);
            this.TableSelector = new TableSelectorViewModel(this.WorkbookSelector.Workbook);
            this.FilterSelector = new FilterSelectorViewModel(this.TableSelector.Table);
            this.KeySelector = new KeySelectorViewModel(this.TableSelector.Table);
            this.Preview = new PreviewViewModel(this.TableSelector.Table, this.FilterSelector.Filter);
            this.DataSource = ObservableProperty.Create(
                this.TableSelector.Table,
                this.KeySelector.Key,
                this.FilterSelector.Filter,
                (table, key, filter) => table != null && key != null ? new DataSource(table, key, filter) : null);
            this.IsReady = ObservableProperty.Create(this.DataSource, dataSource => dataSource != null);

            this.WorkbookSelector.Workbook.Changed += this.Workbook_Changed;
        }

        public string Title { get; }

        public WorkbookSelectorViewModel WorkbookSelector { get; }

        public TableSelectorViewModel TableSelector { get; }

        public FilterSelectorViewModel FilterSelector { get; }

        public KeySelectorViewModel KeySelector { get; }

        public PreviewViewModel Preview { get; }

        public IObservableProperty<DataSource> DataSource { get; }

        public IObservableProperty<bool> IsReady { get; }

        public void OnNext()
        {
            DataSource dataSource = this.DataSource.Value;
            LastConfiguration lastConfiguration = new LastConfiguration
            {
                Table = dataSource.Table.Name,
                Key = dataSource.Key.Name
            };

            Filter filter = dataSource.Filter;
            if (filter != null)
            {
                lastConfiguration.FilterColumn = filter.Column.Name;
                lastConfiguration.FilterValue = filter.Value;
            }

            Config config = ConfigRepository.Instance.Get();
            config.LastConfigurations[dataSource.Table.Workbook.FilePath] = lastConfiguration;
            ConfigRepository.Instance.Set(config);
        }

        private void Workbook_Changed(ExcelWorkbook workbook)
        {
            Config config = ConfigRepository.Instance.Get();
            if (!config.LastConfigurations.TryGetValue(workbook.FilePath, out LastConfiguration lastConfiguration))
            {
                return;
            }

            ExcelTable table = workbook.Tables.FirstOrDefault(x => string.Equals(x.Name, lastConfiguration.Table, StringComparison.OrdinalIgnoreCase));
            if (table == null)
            {
                return;
            }

            this.TableSelector.Table.Value = table;
            this.KeySelector.Key.Value = table.Columns.FirstOrDefault(x => string.Equals(x.Name, lastConfiguration.Key, StringComparison.OrdinalIgnoreCase));

            ExcelColumn filterKey = table.Columns.FirstOrDefault(x => string.Equals(x.Name, lastConfiguration.FilterColumn, StringComparison.OrdinalIgnoreCase));
            if (filterKey != null)
            {
                this.FilterSelector.Filter.Value = new Filter(filterKey, lastConfiguration.FilterValue);
            }
        }
    }
}
