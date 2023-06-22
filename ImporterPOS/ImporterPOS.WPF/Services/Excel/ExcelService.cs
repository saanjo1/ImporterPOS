using DocumentFormat.OpenXml.Spreadsheet;
using ImporterPOS.WPF.Helpers;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.ViewModels;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ImporterPOS.WPF.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public static string ExcelFile { get; set; }
        private static ObservableCollection<ExcelArticlesListViewModel> _articleQtycViewModels = new ObservableCollection<ExcelArticlesListViewModel>();
        private static ObservableCollection<ArticleDiscountViewModel> _discountViewModels = new ObservableCollection<ArticleDiscountViewModel>();



        public static OleDbConnection _oleDbConnection;
        public static OleDbCommand Command;

        public ExcelService()
        {
            
        }

        public async Task<List<string>> GetListOfSheets(string excelFile)
        {
            OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
            String strExtendedProperties = String.Empty;
            sbConnection.DataSource = excelFile;
            if (Path.GetExtension(ExcelFile).Equals(".xls"))//for 97-03 Excel file
            {
                sbConnection.Provider = "Microsoft.ACE.OLEDB.16.0";
                strExtendedProperties = "Excel 8.0;HDR=Yes;IMEX=1";//HDR=ColumnHeader,IMEX=InterMixed
            }
            else if (Path.GetExtension(ExcelFile).Equals(".xlsx"))  //for 2007 Excel file
            {
                sbConnection.Provider = "Microsoft.ACE.OLEDB.16.0";
                strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
            }
            sbConnection.Add("Extended Properties", strExtendedProperties);

            List<string> listSheet = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
            {
                await conn.OpenAsync();
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                foreach (DataRow drSheet in dtSheet.Rows)
                {
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))
                    {
                        listSheet.Add(drSheet["TABLE_NAME"].ToString());
                    }
                }
            }
            return await Task.FromResult(listSheet);
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

        public async Task<ObservableCollection<ExcelArticlesListViewModel>> ReadColumnsFromExcel(string filePath, string sheetValue, ExcelArticlesListViewModel viewModel)
        {
            string _connection =
                @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + filePath + ";" +
                @"Extended Properties='Excel 8.0;HDR=Yes;'";

            _oleDbConnection = new OleDbConnection(_connection);

            await _oleDbConnection.OpenAsync();

            Command = new OleDbCommand();
            Command.Connection = _oleDbConnection;
            Command.CommandText = "select * from [" + sheetValue + "]";

            System.Data.Common.DbDataReader Reader = await Command.ExecuteReaderAsync();

            // Učitavanje JSON filea
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "articleColumnNames.json";
            string jsonFilePath = System.IO.Path.Combine(folderPath, fileName);

            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath);
                var columnNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                while (Reader.Read())
                {
                    string articleName = columnNames["ArticleName"].ToString();
                    string[] nameParts = articleName.Split(',');
                    string name = string.Join(" ", nameParts.Select(part => part.Trim()));

                    List<string> columnValues = new List<string>();

                    foreach (string columnName in nameParts)
                    {
                        string columnValue = Reader[columnName].ToString();
                        columnValues.Add(columnValue);
                    }

                    string joinedColumnValues = string.Join(" ", columnValues);

                    _articleQtycViewModels.Add(new ExcelArticlesListViewModel
                    {
                        Name = joinedColumnValues,
                        BarCode = Reader[columnNames["ArticleBarcode"]].ToString(),
                        ArticlePrice = Reader[columnNames["ArticlePrice"]].ToString(),
                        Quantity = Reader[columnNames["GoodQuantity"]].ToString(),
                        PricePerUnit = Reader[columnNames["GoodPurchasePrice"]].ToString(),
                        TotalPrice = Reader[columnNames["GoodTotalPrice"]].ToString(),
                    });
                }
            }
            else
            {
                
            }



            Reader.Close();
            _oleDbConnection.Close();

            return await Task.FromResult(_articleQtycViewModels);
        }

        public async Task<List<string>> ListColumnNames(string sheetName)
        {
            var connection = WPF.Helpers.Extensions.SetOleDbConnection(ExcelFile);

            _oleDbConnection = new OleDbConnection(connection);

            var lines = new List<string>();

            await _oleDbConnection.OpenAsync();

            Command = new OleDbCommand();
            Command.Connection = _oleDbConnection;
            Command.CommandText = "select top 1 * from [Sheet1$]";

            var Reader = await Command.ExecuteReaderAsync();

            while (Reader.Read())
            {
                var fieldCount = Reader.FieldCount;

                var fieldIncrementor = 1;
                var fields = new List<string>();
                while (fieldCount >= fieldIncrementor)
                {
                    string test = Reader[fieldIncrementor - 1].ToString();
                    fields.Add(test);
                    fieldIncrementor++;
                }

                lines = fields;
            }

            Reader.Close();
            _oleDbConnection.Close();


            return await Task.FromResult(lines);
        }

        public async Task<ObservableCollection<ArticleDiscountViewModel>> ReadDiscountColumns(string path, ArticleDiscountViewModel viewModel)
        {
            if (_discountViewModels.Count > 0)
                _discountViewModels.Clear();

            FixedDiscountColumns templateViewModel = new FixedDiscountColumns();

            string _connection =
 @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + path + ";" +
 @"Extended Properties='Excel 8.0;HDR=Yes;'";


            _oleDbConnection = new OleDbConnection(_connection);

            await _oleDbConnection.OpenAsync();

            Command = new OleDbCommand();
            Command.Connection = _oleDbConnection;
            Command.CommandText = "select * from [Sheet1$]";

            System.Data.Common.DbDataReader Reader = await Command.ExecuteReaderAsync();

            while (Reader.Read())
            {

                _discountViewModels.Add(new ArticleDiscountViewModel
                {
                    Name = Reader[templateViewModel.BarCode].ToString() + " " + Reader[templateViewModel.Item].ToString() + " " + Reader[templateViewModel.Description].ToString() + " " + Reader[templateViewModel.ColorDescription].ToString() + " " + Reader[templateViewModel.ItemSize].ToString(),
                    Category = Reader[templateViewModel.Category].ToString(),
                    BarCode = Reader[templateViewModel.BarCode].ToString(),
                    Price = Reader["Retail price"].ToString(),
                    Discount = Helpers.Extensions.DisplayDiscountInPercentage(Reader[templateViewModel.Discount].ToString()),
                    NewPrice = Helpers.Extensions.GetDecimal(Reader[templateViewModel.DiscountedPrice].ToString()),
                    Storage = Translations.Articles
                });
            }

            Reader.Close();
            _oleDbConnection.Close();

            return await Task.FromResult(_discountViewModels);

        }
    }
}
