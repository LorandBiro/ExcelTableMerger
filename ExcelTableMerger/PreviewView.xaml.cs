using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public partial class PreviewView : UserControl, IView
    {
        private readonly TableSelectorView mainDataSelectorView;
        private readonly TableSelectorView lookupDataSelectorView;
        private readonly MappingView mappingView;

        public PreviewView(TableSelectorView mainDataSelectorView, TableSelectorView lookupDataSelectorView, MappingView mappingView)
        {
            this.mainDataSelectorView = mainDataSelectorView ?? throw new ArgumentNullException(nameof(mainDataSelectorView));
            this.lookupDataSelectorView = lookupDataSelectorView ?? throw new ArgumentNullException(nameof(lookupDataSelectorView));
            this.mappingView = mappingView ?? throw new ArgumentNullException(nameof(mappingView));

            this.InitializeComponent();
        }

        public string Title => "Preview";

        public bool IsReady => false;

        public event Action IsReadyChanged;

        public void OnNext()
        {
        }

        public void Prepare()
        {
            this.JoinedDataGrid.ItemsSource = null;
            this.JoinedDataGrid.Columns.Clear();
            foreach (ExcelColumn column in this.mainDataSelectorView.DataSource.Table.Columns)
            {
                this.JoinedDataGrid.Columns.Add(new DataGridBoundTemplateColumn() { Header = column.Name, Binding = new Binding($"Cells[{column.Index}]"), CellTemplate = (DataTemplate)this.Resources["MergedCellTemplate"] });
            }

            this.JoinedDataGrid.ItemsSource = Merger.Merge(
                this.mainDataSelectorView.DataSource,
                this.lookupDataSelectorView.DataSource,
                this.mappingView.Mappings,
                this.mappingView.AddEnabled,
                this.mappingView.RemoveEnabled);
        }
    }
}
