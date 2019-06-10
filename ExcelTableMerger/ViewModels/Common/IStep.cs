namespace ExcelTableMerger.ViewModels.Common
{
    public interface IStep
    {
        string Title { get; }

        IObservableProperty<bool> IsReady { get; }

        void OnNext();
    }
}
