using ImporterPOS.Domain.Models1;
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
        Task<ObservableCollection<ExcelArticlesListViewModel>> ReadColumnsFromExcel(string filePath, string sheetValue, ExcelArticlesListViewModel viewModel);
        Task<ObservableCollection<ArticleDiscountViewModel>> ReadDiscountColumns(string path, ArticleDiscountViewModel viewModel);
        Task<List<string>> ListColumnNames(string sheetName);

    }
}
