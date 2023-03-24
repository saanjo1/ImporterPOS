﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.Domain.Models;
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
        private ConcurrentDictionary<string, string> _myDictionary;




        ResourceManager rm = new ResourceManager(typeof(Translations));

        [ObservableProperty]
        private string databaseConnection;

        [ObservableProperty]
        private string serverInstance;

        [ObservableProperty]
        private string port;

        [ObservableProperty]
        private string excelFile;


        public SettingsViewModel(Notifier notifier, IExcelService excelService, ConcurrentDictionary<string, string> myDictionary)
        {
            _notifier = notifier;
            _myDictionary = myDictionary;
            _excelService = excelService;
            GetDatabaseInfo();
        }


        [RelayCommand]
        public void UploadExcelFile()
        {
            try
            {
                ExcelFile = _excelService.OpenDialog().Result;

                if (ExcelFile != null)
                {
                    if (_myDictionary.TryGetValue(Translations.CurrentExcelFile, out string value) == false)
                    {
                        bool success = _myDictionary.TryAdd(Translations.CurrentExcelFile, excelFile);

                        if (success)
                            _notifier.ShowSuccess(Translations.SelectExcelFileSuccess);
                        else
                            _notifier.ShowError(Translations.ErrorMessage);
                    }
                    else
                    {
                        _myDictionary.TryGetValue(Translations.CurrentExcelFile, out string value1);
                        bool success = _myDictionary.TryUpdate(Translations.CurrentExcelFile, excelFile, value1);

                        if (value1 == excelFile)
                            _notifier.ShowInformation(Translations.UpdatedSameFile);

                        if (success && value1 != excelFile)
                            _notifier.ShowInformation(Translations.UpdatedExcelFile);
                    }
                }
            }
            catch
            {

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
