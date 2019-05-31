using System;
using System.ComponentModel;

namespace ExcelTableMerger.ViewModels.Common
{
    public static class ObservableProperty
    {
        public static IObservableProperty<T> Create<T, A>(IObservableProperty<A> property1, Func<A, T> func)
        {
            ObservableProperty<T> property = new ObservableProperty<T>(func(property1.Value));
            property1.Changed += value => property.Value = func(value);
            return property;
        }

        public static IObservableProperty<T> Create<T, A, B>(IObservableProperty<A> property1, IObservableProperty<B> property2, Func<A, B, T> func)
        {
            ObservableProperty<T> property = new ObservableProperty<T>(func(property1.Value, property2.Value));
            property1.Changed += value => property.Value = func(value, property2.Value);
            property2.Changed += value => property.Value = func(property1.Value, value);
            return property;
        }

        public static IObservableProperty<T> Create<T, A, B, C>(IObservableProperty<A> property1, IObservableProperty<B> property2, IObservableProperty<C> property3, Func<A, B, C, T> func)
        {
            ObservableProperty<T> property = new ObservableProperty<T>(func(property1.Value, property2.Value, property3.Value));
            property1.Changed += value => property.Value = func(value, property2.Value, property3.Value);
            property2.Changed += value => property.Value = func(property1.Value, value, property3.Value);
            property3.Changed += value => property.Value = func(property1.Value, property2.Value, value);
            return property;
        }
    }

    public sealed class ObservableProperty<T> : IObservableProperty<T>
    {
        private T value;
        private event PropertyChangedEventHandler propertyChanged;

        public ObservableProperty() { }

        public ObservableProperty(T value) => this.value = value;

        public T Value
        {
            get => this.value;
            set
            {
                if (object.Equals(this.value, value))
                {
                    return;
                }

                this.value = value;
                this.Changed?.Invoke(value);
                this.propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => this.propertyChanged += value;
            remove => this.propertyChanged -= value;
        }

        public event Action<T> Changed;
    }
}
