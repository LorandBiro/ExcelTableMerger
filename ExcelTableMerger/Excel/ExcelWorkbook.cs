using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelTableMerger.Excel
{
    public sealed class ExcelWorkbook : IDisposable
    {
        private readonly FileStream fileStream;

        public ExcelWorkbook(string filePath)
        {
            this.fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            List<ExcelTable> tables = new List<ExcelTable>();
            XSSFWorkbook workbook = new XSSFWorkbook(this.fileStream);
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(i);
                tables.AddRange(sheet.GetTables().Select(x => new ExcelTable(this, x)));
            }

            this.FilePath = filePath;
            this.Tables = tables;
        }

        public string FilePath { get; }

        public IReadOnlyCollection<ExcelTable> Tables { get; private set; }

        public void Dispose()
        {
            this.fileStream.Dispose();
        }

        public override string ToString() => this.FilePath;
    }
}
