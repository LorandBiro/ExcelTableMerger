using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExcelTableMerger
{
    public class DataGridBoundTemplateColumn : DataGridBoundColumn
    {
        public DataTemplate CellTemplate { get; set; }
        public DataTemplate CellEditingTemplate { get; set; }

        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return Generate(dataItem, CellEditingTemplate);
        }

        private FrameworkElement Generate(object dataItem, DataTemplate template)
        {
            var contentControl = new ContentControl { ContentTemplate = template, Content = dataItem };
            BindingOperations.SetBinding(contentControl, ContentControl.ContentProperty, Binding);
            return contentControl;
        }

        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return Generate(dataItem, CellTemplate);
        }
    }
}
