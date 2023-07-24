using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Taxes
{
    public interface ITaxService : ICRUDService<Taxis, TaxSearchObject>
    {
        public void CreateTaxArticle(TaxArticle taxArticle);
    }
}
