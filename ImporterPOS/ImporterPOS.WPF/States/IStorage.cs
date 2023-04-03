using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.States
{
    public enum StorageType
    {
        Articles,
        Economato
    }
    public interface IStorage
    {
        BaseViewModel? CurrentDataGrid { get; set; }
    }
}
