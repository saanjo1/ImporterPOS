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
        Task<List<string>> ListColumnNames(string sheetName);

    }
}
