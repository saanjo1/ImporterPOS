using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImporterPOS.WPF.Services.Excel;
using ImporterPOS.WPF.States;
using System;
using System.Collections.Concurrent;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ImporterPOS.WPF.ViewModels
{
    public partial class MainViewModel
    {
        public INavigator Navigator { get; set; }

        public ConcurrentDictionary<string, string> myDictionary;


        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });


        public MainViewModel(IExcelService excelService)
        {
            myDictionary = new ConcurrentDictionary<string, string>();
            Navigator = new Navigator(notifier, excelService, myDictionary);

        }

        [RelayCommand]
        private void Close()
        {
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void Minimize(MainWindow window)
        {
            if (window != null)
            {
                window.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        public void LoadDictionary()
        {
            myDictionary.TryAdd("Name", "Name");
        }

    }
}
