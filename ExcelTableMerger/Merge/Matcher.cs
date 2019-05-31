using ExcelTableMerger.Excel;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.Merge
{
    public static class Matcher
    {
        public static IEnumerable<MatchedRow> Match(DataSource main, DataSource lookup)
        {
            List<ExcelRow> mainRows = main.GetRows().ToList();
            List<ExcelRow> lookupRows = lookup.GetRows().ToList();
            Dictionary<object, int> lookupRowIndices = new Dictionary<object, int>();
            for (int i = 0; i < lookupRows.Count; i++)
            {
                lookupRowIndices[lookupRows[i][lookup.Key]] = i;
            }

            List<MatchedRow> matchedRows = new List<MatchedRow>();
            foreach (ExcelRow mainRow in mainRows)
            {
                object key = mainRow[main.Key];
                ExcelRow lookupRow = null;
                if (lookupRowIndices.TryGetValue(key, out int lookupRowIndex))
                {
                    lookupRow = lookupRows[lookupRowIndex];
                    lookupRows[lookupRowIndex] = null;
                }

                yield return new MatchedRow(mainRow, lookupRow);
            }

            foreach (ExcelRow lookupRow in lookupRows.Where(x => x != null))
            {
                yield return new MatchedRow(null, lookupRow);
            }
        }
    }
}
