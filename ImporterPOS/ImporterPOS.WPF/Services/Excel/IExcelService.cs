using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.Services.Excel
{
    public interface IExcelService
    {
        Task<string> OpenDialog();

        Task<List<string>> GetListOfSheets(string excelFile);

        Task<ObservableCollection<ExcelArticlesListViewModel>> ReadColumnsFromExcel(ConcurrentDictionary<string, string> model, ExcelArticlesListViewModel viewModel);
        Task<ObservableCollection<DiscountColumnsViewModel>> ReadDiscountColumns(ConcurrentDictionary<string, string> model, DiscountColumnsViewModel viewModel);
        Task<ObservableCollection<WriteOffViewModel>> ReadFromWriteOff(string excelFile, string sheet);
        Task<List<string>> ListColumnNames(string sheetName);
        Task<ObservableCollection<StockCorrectionViewModel>> ReadStockCorrectionDocument(string excelFile);
        Task<ObservableCollection<StockCorrectionViewModel>> ReadFromTxtFile(string excelFile);
    }
}
