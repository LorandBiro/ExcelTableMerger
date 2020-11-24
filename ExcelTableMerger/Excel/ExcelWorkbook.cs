using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelTableMerger.Excel
{
    public sealed class ExcelWorkbook
    {
        private readonly XSSFWorkbook workbook;
        
        public ExcelWorkbook(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            List<ExcelTable> tables = new List<ExcelTable>();
            this.workbook = new XSSFWorkbook(fileStream);
            for (int i = 0; i < this.workbook.NumberOfSheets; i++)
            {
                XSSFSheet sheet = (XSSFSheet)this.workbook.GetSheetAt(i);
                tables.AddRange(sheet.GetTables().Select(x => new ExcelTable(this, x)));
            }

            this.FilePath = filePath;
            this.Tables = tables;
        }

        public string FilePath { get; }

        public IReadOnlyCollection<ExcelTable> Tables { get; private set; }

        public void Save()
        {
            using (FileStream fs = new FileStream(this.FilePath, FileMode.Create))
            {
                this.workbook.Write(fs);
            }
        }

        public override string ToString() => this.FilePath;
    }
}
