using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public partial class TableSelectorView : UserControl, IView
    {
        public TableSelectorView(bool isMain)
        {
            DataSelectorViewModel viewModel = new DataSelectorViewModel(isMain ? "MainWorkbookFilePath" : "LookupWorkbookFilePath");

            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Title = isMain ? "Main table" : "Lookup table";
            viewModel.DataSource.Changed += dataSource =>
                {
                    this.IsReady = dataSource != null;
                    this.IsReadyChanged?.Invoke();
                    this.DataSource = dataSource;
                };
            viewModel.Preview.Columns.Changed += columns =>
            {
                this.PreviewDataGrid.Columns.Clear();
                if (columns == null)
                {
                    return;
                }
                
                for (int i = 0; i < columns.Count; i++)
                {
                    this.PreviewDataGrid.Columns.Add(new DataGridTextColumn { Header = columns[i], Binding = new Binding($"[{i}]"), MaxWidth = 200.0 });
                }
            };
            viewModel.Preview.Rows.Changed += rows => this.PreviewDataGrid.ItemsSource = rows;
        }

        public event Action IsReadyChanged;

        public string Title { get; }

        public bool IsReady { get; private set; }

        public DataSource DataSource { get; private set; }

        public void Prepare() { }
    }
}
