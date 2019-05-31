using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using ExcelTableMerger.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public partial class TableSelectorView : UserControl, IView
    {
        private readonly bool isMain;

        private DataSource dataSource;

        public TableSelectorView(bool isMain)
        {
            this.isMain = isMain;

            this.InitializeComponent();
            this.Workbook_Initialize();
            this.Table_Initialize();
            this.Filter_Initialize();
            this.Key_Initialize();
            this.workbookChanged += this.Table_OnWorkbookChanged;
            this.tableChanged += this.Filter_OnTableChanged;
            this.tableChanged += this.Key_OnTableChanged;
            this.tableChanged += this.Preview_OnTableOrFilterChanged;
            this.tableChanged += this.OnTableOrFilterOrKeyChanged;
            this.filterChanged += this.Preview_OnTableOrFilterChanged;
            this.filterChanged += this.OnTableOrFilterOrKeyChanged;
            this.keyChanged += this.OnTableOrFilterOrKeyChanged;

            this.OpenButton.Focus();
        }

        public event Action IsReadyChanged;

        public string Title => this.isMain ? "Main table" : "Lookup table";

        public bool IsReady { get; private set; }

        public DataSource DataSource { get; private set; }

        public void Prepare() { }

        private void OnTableOrFilterOrKeyChanged()
        {
            this.DataSource = this.table != null && this.key != null ? new DataSource(this.table, this.key, this.filter) : null;
            this.IsReady = this.DataSource != null;
            this.IsReadyChanged?.Invoke();
        }

        #region Workbook

        private ExcelWorkbook workbook;
        private event Action workbookChanged;

        private void Workbook_Initialize()
        {
            this.FilePathTextBlock.Visibility = Visibility.Collapsed;
        }

        private void Workbook_OnClick(object sender, RoutedEventArgs e)
        {
            string filePath = this.isMain ? Settings.Default.MainWorkbookFilePath : Settings.Default.LookupWorkbookFilePath;
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Excel Files (*.xlsx; *.xlsm)|*.xlsx; *.xlsm",
                FileName = Path.GetFileName(filePath),
                InitialDirectory = Path.GetDirectoryName(filePath)
            };
            if (ofd.ShowDialog() == true)
            {
                if (this.workbook != null)
                {
                    this.workbook.Dispose();
                    this.workbook = null;
                }

                this.workbook = new ExcelWorkbook(ofd.FileName);
                if (this.isMain)
                {
                    Settings.Default.MainWorkbookFilePath = ofd.FileName;
                }
                else
                {
                    Settings.Default.LookupWorkbookFilePath = ofd.FileName;
                }

                Settings.Default.Save();
                this.FilePathTextBlock.Visibility = Visibility.Visible;
                this.FilePathTextBlock.Text = this.workbook.FilePath;

                this.workbookChanged?.Invoke();
            }
        }

        #endregion

        #region Table

        private ExcelTable table;
        private event Action tableChanged;

        private void Table_Initialize() => this.Table_OnWorkbookChanged();

        private void Table_OnWorkbookChanged()
        {
            if (this.workbook == null)
            {
                this.TablesComboBox.ItemsSource = null;
                this.TablesComboBox.IsEnabled = false;
                return;
            }

            this.TablesComboBox.ItemsSource = this.workbook.Tables;
            this.TablesComboBox.IsEnabled = true;
            if (this.workbook.Tables.Count == 1)
            {
                this.TablesComboBox.SelectedIndex = 0;
            }
            else
            {
                this.TablesComboBox.Focus();
                this.TablesComboBox.IsDropDownOpen = true;
            }
        }

        private void TablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExcelTable table = (ExcelTable)this.TablesComboBox.SelectedItem;
            this.table = table;
            this.tableChanged?.Invoke();
        }

        #endregion

        #region Filter

        private Filter filter;
        private event Action filterChanged;

        private void Filter_Initialize() => this.Filter_OnTableChanged();

        private void Filter_OnTableChanged()
        {
            this.SetFilterButton.IsEnabled = false;
            this.FilterTextBlock.Visibility = Visibility.Collapsed;
            this.ClearFilterButton.Visibility = Visibility.Collapsed;
            this.SetFilterButton.IsEnabled = this.table != null;

            this.filter = null;
            this.filterChanged?.Invoke();
        }

        private void SetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SetFilterWindow setFilterWindow = new SetFilterWindow(this.table.Columns) { Owner = Application.Current.MainWindow };
            if (setFilterWindow.ShowDialog() == true)
            {
                this.filter = setFilterWindow.Filter;

                this.FilterTextBlock.Text = this.filter.ToString();
                this.FilterTextBlock.Visibility = Visibility.Visible;
                this.SetFilterButton.Visibility = Visibility.Collapsed;
                this.ClearFilterButton.Visibility = Visibility.Visible;

                this.filterChanged?.Invoke();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            this.filter = null;

            this.FilterTextBlock.Visibility = Visibility.Collapsed;
            this.SetFilterButton.Visibility = Visibility.Visible;
            this.ClearFilterButton.Visibility = Visibility.Collapsed;

            this.filterChanged?.Invoke();
        }

        #endregion

        #region Key

        private ExcelColumn key;
        private event Action keyChanged;

        private void Key_Initialize()
        {
            this.KeyComboBox.IsEditable = false;
        }

        private void Key_OnTableChanged()
        {
            if (this.table == null)
            {
                this.KeyComboBox.ItemsSource = null;
                this.KeyComboBox.IsEnabled = false;
            }
            else
            {
                this.KeyComboBox.ItemsSource = this.table.Columns;
                this.KeyComboBox.IsEnabled = true;
            }

            this.key = null;
            this.keyChanged?.Invoke();
        }

        private void KeyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.key = (ExcelColumn)this.KeyComboBox.SelectedItem;
            this.keyChanged?.Invoke();
        }

        #endregion

        #region Preview

        private void Preview_OnTableOrFilterChanged()
        {
            this.PreviewDataGrid.Items.Clear();
            this.PreviewDataGrid.Columns.Clear();
            if (this.table == null)
            {
                return;
            }

            for (int i = 0; i < this.table.Columns.Count; i++)
            {
                this.PreviewDataGrid.Columns.Add(new DataGridTextColumn { Header = this.table.Columns[i], Binding = new Binding($"[{i}]"), MaxWidth = 200.0 });
            }

            foreach (ExcelRow row in Filter.GetRows(this.table, this.filter))
            {
                this.PreviewDataGrid.Items.Add(row);
            }
        }

        #endregion
    }
}
