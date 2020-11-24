using ExcelTableMerger.Configuration;
using ExcelTableMerger.Excel;
using ExcelTableMerger.ViewModels.Common;
using Microsoft.Win32;
using System.IO;

namespace ExcelTableMerger.ViewModels.DataSelector
{
    public sealed class WorkbookSelectorViewModel
    {
        private readonly bool isMain;

        public WorkbookSelectorViewModel(bool isMain)
        {
            this.isMain = isMain;

            this.Workbook = new ObservableProperty<ExcelWorkbook>();
            this.OpenFileCommand = new Command(this.OpenFile);
        }

        public ObservableProperty<ExcelWorkbook> Workbook { get; }
        public Command OpenFileCommand { get; }

        private void OpenFile()
        {
            Config config = ConfigRepository.Instance.Get();
            string filePath = this.isMain ? config.LastMainFilePath : config.LastLookupFilePath;
            OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "Excel Files (*.xlsx; *.xlsm)|*.xlsx; *.xlsm",
                    FileName = Path.GetFileName(filePath),
                    InitialDirectory = Path.GetDirectoryName(filePath)
                };

            if (ofd.ShowDialog() == true)
            {
                if (this.isMain)
                {
                    config.LastMainFilePath = ofd.FileName;
                }
                else
                {
                    config.LastLookupFilePath = ofd.FileName;
                }

                ConfigRepository.Instance.Set(config);
                this.Workbook.Value = new ExcelWorkbook(ofd.FileName);
            }
        }
    }
}
