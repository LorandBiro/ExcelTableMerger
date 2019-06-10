using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public partial class MappingView : UserControl, IView
    {
        private readonly TableSelectorView mainTableSelector;
        private readonly TableSelectorView lookupTableSelector;

        private DataSource mainDataSource;
        private DataSource lookupDataSource;

        public MappingView(TableSelectorView mainTableSelector, TableSelectorView lookupTableSelector)
        {
            this.mainTableSelector = mainTableSelector ?? throw new ArgumentNullException(nameof(mainTableSelector));
            this.lookupTableSelector = lookupTableSelector ?? throw new ArgumentNullException(nameof(lookupTableSelector));

            this.InitializeComponent();
        }

        public event Action IsReadyChanged;

        public string Title => "Mapping";

        public bool IsReady => true;

        public IReadOnlyCollection<Mapping> Mappings => this.MappingRoot.Children.Cast<MappingControl>().Select(x => x.Mapping).ToList();

        public bool AddEnabled => this.AddRowsCheckBox.IsEnabled == true;

        public bool RemoveEnabled => this.DeleteRowsCheckBox.IsEnabled == true;

        public void Prepare()
        {
            if (this.mainDataSource == this.mainTableSelector.DataSource && this.lookupDataSource == this.lookupTableSelector.DataSource)
            {
                return;
            }

            this.mainDataSource = this.mainTableSelector.DataSource;
            this.lookupDataSource = this.lookupTableSelector.DataSource;
            
            this.DeleteRowsCheckBox.IsChecked = false;
            this.AddRowsCheckBox.IsChecked = false;

            this.MappingRoot.Children.Clear();
            foreach (ExcelColumn column in this.mainDataSource.Table.Columns)
            {
                bool isEnabled = true;
                ExcelColumn selectedColumn = null;
                if (column == this.mainDataSource.Key)
                {
                    isEnabled = false;
                    selectedColumn = this.lookupDataSource.Key;
                }

                this.MappingRoot.Children.Add(new MappingControl(column, this.lookupDataSource.Table.Columns, selectedColumn) { IsEnabled = isEnabled });
            }

            this.JoinedDataGrid.Columns.Clear();
            this.MainUnmatchedDataGrid.Columns.Clear();
            this.LookupUnmatchedDataGrid.Columns.Clear();
            for (int i = 0; i < this.mainDataSource.Table.Columns.Count; i++)
            {
                this.JoinedDataGrid.Columns.Add(new DataGridTextColumn { Header = "Main." + this.mainDataSource.Table.Columns[i], Binding = new Binding($"Main[{i}]") });
                this.MainUnmatchedDataGrid.Columns.Add(new DataGridTextColumn { Header = this.mainDataSource.Table.Columns[i], Binding = new Binding($"[{i}]") });
            }

            for (int i = 0; i < this.lookupDataSource.Table.Columns.Count; i++)
            {
                this.JoinedDataGrid.Columns.Add(new DataGridTextColumn { Header = "Lookup." + this.lookupDataSource.Table.Columns[i], Binding = new Binding($"Lookup[{i}]") });
                this.LookupUnmatchedDataGrid.Columns.Add(new DataGridTextColumn { Header = this.lookupDataSource.Table.Columns[i], Binding = new Binding($"[{i}]") });
            }

            List<MatchedRow> matchedRows = new List<MatchedRow>();
            List<ExcelRow> mainUnmatchedRows = new List<ExcelRow>();
            List<ExcelRow> lookupUnmatchedRows = new List<ExcelRow>();
            foreach (MatchedRow matchedRow in Matcher.Match(this.mainDataSource, this.lookupDataSource))
            {
                if (matchedRow.Main == null)
                {
                    lookupUnmatchedRows.Add(matchedRow.Lookup);
                }
                else if (matchedRow.Lookup == null)
                {
                    mainUnmatchedRows.Add(matchedRow.Main);
                }
                else
                {
                    matchedRows.Add(matchedRow);
                }
            }

            this.JoinedRowCountTextBlock.Text = matchedRows.Count.ToString();
            this.MainUnmatchedRowCountTextBlock.Text = mainUnmatchedRows.Count.ToString();
            this.LookupUnmatchedRowCountTextBlock.Text = lookupUnmatchedRows.Count.ToString();

            this.JoinedDataGrid.ItemsSource = matchedRows;
            this.MainUnmatchedDataGrid.ItemsSource = mainUnmatchedRows;
            this.LookupUnmatchedDataGrid.ItemsSource = lookupUnmatchedRows;
        }

        public void OnNext()
        {
        }
    }
}
