using ImporterPOS.WPF.Resources;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImporterPOS.WPF.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public static string ExcelFile { get; set; }


        public static OleDbConnection _oleDbConnection;
        public static OleDbCommand Command;

        public ExcelService()
        {
            
        }

        public Task<List<string>> GetListOfSheets(string excelFile)
        {
            throw new NotImplementedException();
        }

        public async Task<string> OpenDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:";
            openFileDialog.Title = Translations.OpenDialogTitle;
            openFileDialog.Filter = Informations.OpenDialogFilter;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFile = openFileDialog.FileName;
                return await Task.FromResult(ExcelFile);
            }
            return null;
        }
    }
}
