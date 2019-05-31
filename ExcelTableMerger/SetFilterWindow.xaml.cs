using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ExcelTableMerger
{
    public partial class SetFilterWindow : Window
    {
        public SetFilterWindow(IEnumerable<ExcelColumn> columns)
        {
            InitializeComponent();
            foreach (ExcelColumn column in columns)
            {
                this.ColumnsComboBox.Items.Add(column);
            }

            this.ColumnsComboBox.Focus();
            this.ColumnsComboBox.IsDropDownOpen = true;
        }

        public Filter Filter { get; private set; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Cancel();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                this.Set();
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Cancel();

        private void SetButton_Click(object sender, RoutedEventArgs e) => this.Set();

        private void Cancel()
        {
            this.Close();
        }

        private void Set()
        {
            ExcelColumn column = (ExcelColumn)this.ColumnsComboBox.SelectedItem;
            if (column == null)
            {
                return;
            }

            this.Filter = new Filter(column, ValueTextBox.Text);
            this.DialogResult = true;
            this.Close();
        }
    }
}
