namespace ExcelTableMerger.Merge
{
    public struct MergedCell
    {
        public MergedCell(object oldValue, object newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public object OldValue { get; }

        public object NewValue { get; }

        public MergeKind Kind
        {
            get
            {
                if (Cell.Equal(this.OldValue, this.NewValue))
                {
                    return MergeKind.Unmodified;
                }

                if (Cell.Empty(this.OldValue))
                {
                    if (Cell.Empty(this.NewValue))
                    {
                        return MergeKind.Unmodified;
                    }
                    else
                    {
                        return MergeKind.Added;
                    }
                }

                if (Cell.Empty(this.NewValue))
                {
                    return MergeKind.Removed;
                }

                return MergeKind.Modified;
            }
        }
    }
}
