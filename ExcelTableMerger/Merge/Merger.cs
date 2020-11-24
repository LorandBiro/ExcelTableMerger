using ExcelTableMerger.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.Merge
{
    public static class Merger
    {
        public static IEnumerable<MergedRow> Merge(DataSource main, DataSource lookup, IEnumerable<Mapping> mappings, bool add, bool remove)
        {
            int columnCount = main.Table.Columns.Count;
            Func<ExcelRow, ExcelRow, object>[] getters = CreateSetters(columnCount, mappings);
            foreach (MatchedRow matchedRow in Matcher.Match(main, lookup))
            {
                MergedCell[] cells = new MergedCell[columnCount];
                MergeKind kind = MergeKind.Unmodified;
                int index = -1;
                if (matchedRow.Main == null)
                {
                    if (add)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            cells[i] = new MergedCell(null, getters[i](null, matchedRow.Lookup));
                        }

                        kind = MergeKind.Added;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (matchedRow.Lookup == null)
                {
                    index = matchedRow.Main.Index;
                    if (remove)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            cells[i] = new MergedCell(matchedRow.Main[i], null);
                        }

                        kind = MergeKind.Removed;
                    }
                    else
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            cells[i] = new MergedCell(matchedRow.Main[i], matchedRow.Main[i]);
                        }
                    }
                }
                else
                {
                    index = matchedRow.Main.Index;
                    for (int i = 0; i < columnCount; i++)
                    {
                        cells[i] = new MergedCell(matchedRow.Main[i], getters[i](matchedRow.Main, matchedRow.Lookup));
                    }

                    for (int i = 0; i < columnCount; i++)
                    {
                        if (cells[i].Kind != MergeKind.Unmodified)
                        {
                            kind = MergeKind.Modified;
                            break;
                        }
                    }
                }
                
                yield return new MergedRow(index, cells, kind);
            }
        }

        private static Func<ExcelRow, ExcelRow, object>[] CreateSetters(int count, IEnumerable<Mapping> mappings)
        {
            Mapping[] mappingsByIndex = new Mapping[count];
            foreach (Mapping mapping in mappings.Where(x => x != null))
            {
                mappingsByIndex[mapping.MainColumn.Index] = mapping;
            }

            Func<ExcelRow, ExcelRow, object>[] setters = new Func<ExcelRow, ExcelRow, object>[count];
            for (int i = 0; i < count; i++)
            {
                Mapping mapping = mappingsByIndex[i];
                if (mapping == null)
                {
                    int i2 = i;
                    setters[i] = (main, lookup) => main == null ? null : main[i2];
                }
                else
                {
                    if (mapping.Overwrite)
                    {
                        setters[i] = (main, lookup) => lookup[mapping.LookupColumn];
                    }
                    else
                    {
                        setters[i] = (main, lookup) =>
                        {
                            if (main == null)
                            {
                                return lookup[mapping.LookupColumn];
                            }

                            object mainValue = main[mapping.MainColumn];
                            return Cell.Empty(mainValue) ? lookup[mapping.LookupColumn] : mainValue;
                        };
                    }
                }
            }

            return setters;
        }
    }
}
