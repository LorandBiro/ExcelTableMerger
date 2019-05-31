namespace ExcelTableMerger.Merge
{
    public static class Cell
    {
        public static bool Equal(object a, object b)
        {
            return Equals(a, b);
        }

        public static bool Empty(object value)
        {
            return value == null || string.IsNullOrWhiteSpace(value.ToString());
        }
    }
}
