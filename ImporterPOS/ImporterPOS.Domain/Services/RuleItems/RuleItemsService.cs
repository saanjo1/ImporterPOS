using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.RuleItems
{
    public class RuleItemsService : BaseCRUDService<RuleItem, object>, IRuleItemsService
    {
        public RuleItemsService(DatabaseContextFactory factory) : base(factory)
        {
        }

    }
}
