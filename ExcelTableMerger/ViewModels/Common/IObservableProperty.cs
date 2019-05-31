using System;
using System.ComponentModel;

namespace ExcelTableMerger.ViewModels.Common
{
    public interface IObservableProperty<T> : INotifyPropertyChanged
    {
        T Value { get; }


        event Action<T> Changed;
    }
}
