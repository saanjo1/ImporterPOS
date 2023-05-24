using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.WPF.Modals;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly Notifier _notifier;
        private IExcelService _excelService;
        private IArticleService _articleService;
        private IGoodService _goodService;

        [ObservableProperty]
        private string databaseConnection;

        [ObservableProperty]
        private string serverInstance;

        [ObservableProperty]
        private string port;
        [ObservableProperty]
        private bool isOpen;



        public SettingsViewModel(Notifier notifier, IExcelService excelService, IArticleService articleService, IGoodService goodService)
        {
            _notifier = notifier;
            _excelService = excelService;
            _articleService = articleService;
            _goodService = goodService;
            GetDatabaseInfo();
        }


       
        [RelayCommand]
        public async Task CreateGoodsBasedOnArticleName()
        {
            try
            {
                await _articleService.CreateGoodsBasedOnArticleName();

            }
            catch 
            {
                _notifier.ShowError(Translations.ErrorMessage);
                
            }
        }

        [RelayCommand]
        public async Task SetMainStockToZero()
        {
            try
            {
                await _goodService.SetMainStockToZero();
            }
            catch
            {
                _notifier.ShowError(Translations.ErrorMessage);
                throw;
            }
        }


        public void GetDatabaseInfo()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            int index = appDataPath.IndexOf("Roaming");
            appDataPath = appDataPath.Substring(0, index) + Informations.POSFolderPath;

            XmlDocument doc = new XmlDocument();
            doc.Load(appDataPath);

            XmlNode databaseNode = doc.SelectSingleNode(Informations.DatabasePath);
            XmlNode serverInstanceNode = doc.SelectSingleNode(Informations.ServerInstancePath);
            XmlNode portNode = doc.SelectSingleNode(Informations.PortPath);
            DatabaseConnection = databaseNode.InnerText;
            ServerInstance = serverInstanceNode.InnerText;
            Port = portNode.InnerText;
        }

    }
}
