using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<MergedRow> mergedRows;

        public PreviewView(TableSelectorView mainDataSelectorView, TableSelectorView lookupDataSelectorView, MappingView mappingView)
        {
            this.mainDataSelectorView = mainDataSelectorView ?? throw new ArgumentNullException(nameof(mainDataSelectorView));
            this.lookupDataSelectorView = lookupDataSelectorView ?? throw new ArgumentNullException(nameof(lookupDataSelectorView));
            this.mappingView = mappingView ?? throw new ArgumentNullException(nameof(mappingView));

            this.InitializeComponent();
        }

        public string Title => "Preview";

        public bool IsReady => true;

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

            this.mergedRows = Merger.Merge(
                this.mainDataSelectorView.DataSource,
                this.lookupDataSelectorView.DataSource,
                this.mappingView.Mappings,
                this.mappingView.AddEnabled,
                this.mappingView.RemoveEnabled).Where(x => x.Kind != MergeKind.Unmodified).ToList();
            this.JoinedDataGrid.ItemsSource = this.mergedRows;
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(App.Current.MainWindow, "Are you sure?", "Are you sure?", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
            {
                return;
            }

            ExcelTable table = this.mainDataSelectorView.DataSource.Table;
            foreach (MergedRow mergedRow in this.mergedRows.Where(x => x.Kind == MergeKind.Modified))
            {
                for (int i = 0; i < mergedRow.Cells.Count; i++)
                {
                    MergedCell mergedCell = mergedRow.Cells[i];
                    if (mergedCell.Kind != MergeKind.Unmodified)
                    {
                        table.SetCell(mergedRow.Index, i, mergedCell.NewValue);
                    }
                }
            }

            int newRowCount = 0;
            foreach (MergedRow mergedRow in this.mergedRows.Where(x => x.Kind == MergeKind.Added))
            {
                for (int i = 0; i < mergedRow.Cells.Count; i++)
                {
                    table.SetCell(table.Rows.Count + newRowCount, i, mergedRow.Cells[i].NewValue);
                }

                newRowCount++;
            }

            int deletedRowCount = 0;
            foreach (MergedRow mergedRow in this.mergedRows.Where(x => x.Kind == MergeKind.Removed).OrderByDescending(x => x.Index))
            {
                table.RemoveRow(mergedRow.Index);

                deletedRowCount++;
            }

            table.Workbook.Save();
        }
    }
}
