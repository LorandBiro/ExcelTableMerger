using ExcelTableMerger.Excel;
using ExcelTableMerger.Properties;
using ExcelTableMerger.ViewModels.Common;
using Microsoft.Win32;
using System;
using System.IO;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class WorkbookSelectorViewModel
    {
        private readonly string filePathConfigurationKey;

        public WorkbookSelectorViewModel(string filePathConfigurationKey)
        {
            this.filePathConfigurationKey = filePathConfigurationKey ?? throw new ArgumentNullException(nameof(filePathConfigurationKey));

            this.Workbook = new ObservableProperty<ExcelWorkbook>();
            this.OpenFileCommand = new Command(this.OpenFile);
        }

        public ObservableProperty<ExcelWorkbook> Workbook { get; }
        public Command OpenFileCommand { get; }

        private void OpenFile()
        {
            string filePath = (string)Settings.Default[this.filePathConfigurationKey];
            OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Excel Files (*.xlsx; *.xlsm)|*.xlsx; *.xlsm",
                    FileName = Path.GetFileName(filePath),
                    InitialDirectory = Path.GetDirectoryName(filePath)
                };

            if (ofd.ShowDialog() == true)
            {
                if (this.Workbook.Value != null)
                {
                    this.Workbook.Value.Dispose();
                }

                Settings.Default[this.filePathConfigurationKey] = ofd.FileName;
                Settings.Default.Save();

                this.Workbook.Value = new ExcelWorkbook(ofd.FileName);
            }
        }
    }
}
