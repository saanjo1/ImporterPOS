using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.Services.Excel
{
    public interface IExcelService
    {
        Task<string> OpenDialog();

        Task<List<string>> GetListOfSheets(string excelFile);

    }
}
