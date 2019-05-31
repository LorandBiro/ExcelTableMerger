using ExcelTableMerger.Excel;
using ExcelTableMerger.Merge;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ExcelTableMerger
{
    public partial class MappingControl : UserControl
    {
        private readonly ExcelColumn mainColumn;
        
        public MappingControl(ExcelColumn mainColumn, IEnumerable<ExcelColumn> lookupColumns, ExcelColumn selectedColumn)
        {
            if (lookupColumns == null)
            {
                throw new ArgumentNullException(nameof(lookupColumns));
            }

            this.mainColumn = mainColumn ?? throw new ArgumentNullException(nameof(mainColumn));

            this.InitializeComponent();

            this.MainColumnTextBlock.Text = mainColumn.Name;
            this.LookupColumnComboBox.Items.Add(string.Empty);
            this.OverwriteCheckBox.Visibility = Visibility.Collapsed;
            foreach (ExcelColumn lookupColumn in lookupColumns)
            {
                this.LookupColumnComboBox.Items.Add(lookupColumn);
            }

            if (selectedColumn != null)
            {
                this.LookupColumnComboBox.SelectedItem = selectedColumn;
            }
        }

        public event Action MappingChanged;

        public Mapping Mapping { get; private set; }

        private void LookupColumnComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string column = this.LookupColumnComboBox.SelectedItem as string;
            if (column == string.Empty)
            {
                this.OverwriteCheckBox.IsChecked = false;
                this.OverwriteCheckBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.OverwriteCheckBox.Visibility = Visibility.Visible;
            }

            this.UpdateMapping();
        }

        private void OverwriteCheckBox_IsCheckedChanged(object sender, RoutedEventArgs e) => this.UpdateMapping();

        private void UpdateMapping()
        {
            if (this.LookupColumnComboBox.SelectedItem as string == string.Empty)
            {
                if (this.Mapping == null)
                {
                    return;
                }

                this.Mapping = null;
            }
            else
            {
                this.Mapping = new Mapping(this.mainColumn, (ExcelColumn)this.LookupColumnComboBox.SelectedItem, OverwriteCheckBox.IsChecked.Value);
            }

            this.MappingChanged?.Invoke();
        }
    }
}
