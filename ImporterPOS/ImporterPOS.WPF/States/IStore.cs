using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.States
{
    public interface IStore
    {
        BaseViewModel? CurrentDataGrid { get; set; }
    }
}
