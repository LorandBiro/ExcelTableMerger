using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExcelTableMerger
{
    public partial class MainWindow : Window
    {
        private readonly IView[] views;
        private IView activeView;

        public MainWindow()
        {
            this.InitializeComponent();

            TableSelectorView mainTableSelector = new TableSelectorView(true);
            TableSelectorView lookupTableSelector = new TableSelectorView(false);
            MappingView mappingView = new MappingView(mainTableSelector, lookupTableSelector);
            PreviewView previewView = new PreviewView(mainTableSelector, lookupTableSelector, mappingView);
            this.views = new IView[] { mainTableSelector, lookupTableSelector, mappingView, previewView };
            foreach (UserControl view in this.views.Cast<UserControl>())
            {
                view.Visibility = Visibility.Collapsed;
                this.ContentRoot.Children.Add(view);
            }

            this.SetActiveView(this.views[0]);
            // Hack to force focus
            this.Loaded += (sender, e) => this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void View_IsReadyChanged()
        {
            this.NextButton.IsEnabled = this.activeView.IsReady;
        }

        private void SetActiveView(IView view)
        {
            if (this.activeView != null)
            {
                this.activeView.IsReadyChanged -= this.View_IsReadyChanged;
                ((UserControl)this.activeView).Visibility = Visibility.Collapsed;
            }

            view.IsReadyChanged += this.View_IsReadyChanged;
            int index = Array.IndexOf(this.views, view);
            if (index == this.views.Length - 1)
            {
                this.NextButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.NextButton.Visibility = Visibility.Visible;
                this.NextButton.IsEnabled = view.IsReady;
            }

            this.PreviousButton.Visibility = index == 0 ? Visibility.Collapsed : Visibility.Visible;

            view.Prepare();
            ((UserControl)view).Visibility = Visibility.Visible;
            this.activeView = view;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.activeView.OnNext();
            this.SetActiveView(this.views[Array.IndexOf(this.views, this.activeView) + 1]);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetActiveView(this.views[Array.IndexOf(this.views, this.activeView) - 1]);
        }
    }
}
