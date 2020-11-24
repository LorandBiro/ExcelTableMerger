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
            Dictionary<string, int> lookupRowIndices = new Dictionary<string, int>();
            for (int i = 0; i < lookupRows.Count; i++)
            {
                lookupRowIndices[lookupRows[i][lookup.Key].ToString().Trim()] = i;
            }

            List<MatchedRow> matchedRows = new List<MatchedRow>();
            foreach (ExcelRow mainRow in mainRows)
            {
                string key = mainRow[main.Key].ToString().Trim();
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
