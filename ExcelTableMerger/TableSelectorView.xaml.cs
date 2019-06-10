using ExcelTableMerger.Merge;
using ExcelTableMerger.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public partial class TableSelectorView : UserControl, IView
    {
        private readonly DataSelectorViewModel viewModel;

        public TableSelectorView(bool isMain)
        {
            this.viewModel = new DataSelectorViewModel(isMain ? "Main table" : "Lookup table", isMain);

            this.InitializeComponent();
            this.DataContext = viewModel;
            this.viewModel.IsReady.Changed += isReady => this.IsReadyChanged?.Invoke();
            this.viewModel.Preview.Columns.Changed += columns =>
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
            this.viewModel.Preview.Rows.Changed += rows => this.PreviewDataGrid.ItemsSource = rows;
        }

        public event Action IsReadyChanged;

        public string Title => this.viewModel.Title;

        public bool IsReady => this.viewModel.IsReady.Value;

        public DataSource DataSource => this.viewModel.DataSource.Value;

        public void Prepare() { }

        public void OnNext()
        {
            this.viewModel.OnNext();
        }
    }
}
