using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelTableMerger.Excel
{
    public sealed class ExcelTable
    {
        private readonly XSSFSheet sheet;
        private readonly XSSFTable table;
        private readonly CT_Table ctTable;

        private readonly List<ExcelColumn> columns;
        private readonly List<ExcelRow> rows;
        private readonly int firstRow;
        private readonly int firstColumn;

        public ExcelTable(ExcelWorkbook workbook, XSSFTable table)
        {
            this.Workbook = workbook ?? throw new ArgumentNullException(nameof(workbook));
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            this.sheet = this.table.GetXSSFSheet();
            this.ctTable = this.table.GetCTTable();
            this.columns = this.ctTable.tableColumns.tableColumn.Select((name, i) => new ExcelColumn(i, name.name)).ToList();

            CellReference start = this.table.GetStartCellReference();
            CellReference end = this.table.GetEndCellReference();
            int headerRowCount = (int)this.ctTable.headerRowCount;
            int rowCount = this.table.RowCount - headerRowCount + 1;

            this.firstColumn = start.Col;
            this.firstRow = start.Row + headerRowCount;
            this.rows = new List<ExcelRow>();
            for (int i = 0; i < rowCount; i++)
            {
                IRow row = this.sheet.GetRow(this.firstRow + i);
                if (row == null)
                {
                    continue;
                }

                object[] rowData = new object[this.columns.Count];
                for (int j = 0; j < this.columns.Count; j++)
                {
                    rowData[j] = ReadCell(row.GetCell(this.firstColumn + j));
                }

                if (rowData.Any(x => x != null))
                {
                    this.rows.Add(new ExcelRow(i, rowData));
                }
            }
        }

        public IReadOnlyList<ExcelColumn> Columns => this.columns;

        public IReadOnlyList<ExcelRow> Rows => this.rows;

        public ExcelWorkbook Workbook { get; }

        public string Name => this.table.Name;

        public void SetCell(int rowIndex, int columnIndex, object value)
        {
            IRow row = this.sheet.GetRow(this.firstRow + rowIndex);
            if (row == null)
            {
                if (value == null)
                {
                    return;
                }

                row = this.sheet.CreateRow(this.firstRow + rowIndex);
            }

            ICell cell = row.GetCell(this.firstColumn + columnIndex);
            if (cell == null)
            {
                if (value == null)
                {
                    return;
                }

                cell = row.CreateCell(this.firstColumn + columnIndex);
            }

            if (value == null)
            {
                row.RemoveCell(cell);
                return;
            }

            if (value is bool boolValue)
            {
                cell.SetCellValue(boolValue);
            }
            else if (value is double numericValue)
            {
                cell.SetCellValue(numericValue);
            }
            else if (value is DateTime dateValue)
            {
                cell.SetCellValue(dateValue);
            }
            else if (value is string stringValue)
            {
                cell.SetCellValue(stringValue);
            }
            else
            {
                throw new ArgumentException("Unsupported value type.", nameof(value));
            }
        }

        public void RemoveRow(int rowIndex)
        {
            rowIndex += this.firstRow;
            IRow row = this.sheet.GetRow(rowIndex);
            if (row == null)
            {
                return;
            }

            sheet.RemoveRow(row);
            sheet.ShiftRows(rowIndex + 1, sheet.LastRowNum, -1);
        }

        public override string ToString() => $"{this.table.Name} - {this.Rows.Count} rows";

        private static object ReadCell(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            CellType type = cell.CellType == CellType.Formula ? cell.CachedFormulaResultType : cell.CellType;
            switch (type)
            {
                case CellType.Blank: return null;
                case CellType.Numeric: return DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue : (object)cell.NumericCellValue;
                case CellType.String: return cell.StringCellValue;
                case CellType.Boolean: return cell.BooleanCellValue;
                default:
                    throw new ArgumentException($"Unsupported cell type: {type} (Formula: {cell.CellType == CellType.Formula}, Row: {cell.RowIndex}, Column: {cell.ColumnIndex})");
            }
        }
    }
}
