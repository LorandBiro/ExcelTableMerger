using ExcelTableMerger.Merge;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExcelTableMerger
{
    public partial class MergedCellControl : UserControl
    {
        private static readonly SolidColorBrush UnmodifiedBackground = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush UnmodifiedForeground = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush ModifiedBackground = new SolidColorBrush(Color.FromRgb(255, 235, 156));
        private static readonly SolidColorBrush ModifiedForeground = new SolidColorBrush(Color.FromRgb(156, 87, 0));
        private static readonly SolidColorBrush RemovedBackground = new SolidColorBrush(Color.FromRgb(255, 199, 206));
        private static readonly SolidColorBrush RemovedForeground = new SolidColorBrush(Color.FromRgb(156, 0, 6));
        private static readonly SolidColorBrush AddedBackground = new SolidColorBrush(Color.FromRgb(198, 239, 206));
        private static readonly SolidColorBrush AddedForeground = new SolidColorBrush(Color.FromRgb(0, 97, 0));

        public MergedCellControl()
        {
            this.InitializeComponent();
        }

        public MergedCell Cell
        {
            get { return (MergedCell)GetValue(CellProperty); }
            set { SetValue(CellProperty, value); }
        }
        
        public static readonly DependencyProperty CellProperty = DependencyProperty.Register("Cell", typeof(MergedCell), typeof(MergedCellControl), new PropertyMetadata(default(MergedCell), OnCellChanged));

        private static void OnCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MergedCellControl control = (MergedCellControl)d;
            MergedCell cell = (MergedCell)e.NewValue;
            switch (cell.Kind)
            {
                case MergeKind.Unmodified:
                    control.Background = UnmodifiedBackground;
                    control.OldValueTextBlock.Visibility = Visibility.Collapsed;
                    control.NewValueTextBlock.Foreground = UnmodifiedForeground;
                    control.NewValueTextBlock.Visibility = Visibility.Visible;
                    control.NewValueTextBlock.Text = cell.NewValue.ToString();
                    break;
                case MergeKind.Modified:
                    control.Background = ModifiedBackground;
                    control.OldValueTextBlock.Foreground = ModifiedForeground;
                    control.OldValueTextBlock.Visibility = Visibility.Visible;
                    control.OldValueTextBlock.Text = cell.OldValue.ToString();
                    control.NewValueTextBlock.Foreground = ModifiedForeground;
                    control.NewValueTextBlock.Visibility = Visibility.Visible;
                    control.NewValueTextBlock.Text = cell.NewValue.ToString();
                    break;
                case MergeKind.Added:
                    control.Background = AddedBackground;
                    control.OldValueTextBlock.Visibility = Visibility.Collapsed;
                    control.NewValueTextBlock.Foreground = AddedForeground;
                    control.NewValueTextBlock.Visibility = Visibility.Visible;
                    control.NewValueTextBlock.Text = cell.NewValue.ToString();
                    break;
                case MergeKind.Removed:
                    control.Background = RemovedBackground;
                    control.OldValueTextBlock.Foreground = RemovedForeground;
                    control.OldValueTextBlock.Visibility = Visibility.Visible;
                    control.OldValueTextBlock.Text = cell.OldValue.ToString();
                    control.NewValueTextBlock.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
