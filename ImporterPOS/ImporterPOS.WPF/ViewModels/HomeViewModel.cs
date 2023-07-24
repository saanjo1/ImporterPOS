using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Articles;
using ImporterPOS.Domain.Services.Goods;
using ImporterPOS.Domain.Services.InventoryDocuments;
using ImporterPOS.Domain.Services.InventoryItems;
using ImporterPOS.Domain.Services.Suppliers;
using ImporterPOS.WPF.Resources;
using ImporterPOS.WPF.Services.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using ModalControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using ToastNotifications;
using ToastNotifications.Messages;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ImporterPOS.WPF.ViewModels
{
    [ObservableObject]
    public partial class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            
        }
    }
}

