using System;

namespace ExcelTableMerger
{
    public interface IView
    {
        string Title { get; }

        event Action IsReadyChanged;

        bool IsReady { get; }

        void Prepare();

        void OnNext();
    }
}
