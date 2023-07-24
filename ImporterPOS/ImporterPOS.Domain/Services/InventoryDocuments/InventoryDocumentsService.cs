using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public class InventoryDocumentsService : BaseCRUDService<InventoryDocument, InventoryDocumentSearchObject>, IInventoryDocumentsService
    {
        public InventoryDocumentsService(DatabaseContextFactory factory) : base(factory)
        {

        }
        public override ICollection<InventoryDocument> Get(InventoryDocumentSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<InventoryDocument>().AsQueryable();

                if (search != null)
                {
                    Supplier? supplier = Context.Suppliers.FirstOrDefault(x => x.Name == search.Supplier);

                    if (!string.IsNullOrWhiteSpace(search.Id))
                    {
                        Guid id = Guid.Parse(search.Id);
                        entity = entity.Where(x => x.Id == id);
                    }

                    if (supplier != null)
                    {
                        entity = entity.Where(x => x.SupplierId == supplier.Id);
                    }
                }

                 return entity.ToList();
            }
        }

        public Task<int> GetInventoryOrderNumber()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                return Task.FromResult(context.InventoryDocuments.Count());
            }
        }


        public Task<decimal?> GetTotalInventoryItems(string _documentId)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                decimal? total = context.InventoryItemBases.Where(x => x.InventoryDocumentId.ToString() == _documentId).Sum(x => x.Total);

                return Task.FromResult(total);
            }
        }

        public Task<decimal> GetTotalSellingPrice(InventoryDocument inventoryDocument)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                List<Guid?> listofGoodIds = context.InventoryItemBases
                    .Where(x => x.InventoryDocumentId == inventoryDocument.Id)
                    .Select(x => x.GoodId)
                    .ToList();

                decimal total = 0;

                foreach (var item in listofGoodIds)
                {
                    InventoryItemBasis? inventoryItemBase = context.InventoryItemBases
                        .Where(x => x.InventoryDocumentId == inventoryDocument.Id && x.GoodId == item)
                        .FirstOrDefault();

                    Guid? articleId = context.ArticleGoods
                        .Where(x => x.GoodId == item)
                        .Select(x => x.ArticleId)
                        .FirstOrDefault();

                    decimal price = context.Articles
                        .Where(x => x.Id == articleId)
                        .Select(x => x.Price)
                        .FirstOrDefault();

                    decimal quantity = inventoryItemBase.Quantity;

                    total += price * quantity;
                }

                return Task.FromResult(Math.Round(total, 2));
            }
        }

        public Task<decimal> GetTotalBasePrices(InventoryDocument inventoryDocument)
        {
            decimal totalBasePrice = 0;
            try
            {
                using (DatabaseContext context = _factory.CreateDbContext())
                {
                    List<Guid?> goodIds = context.InventoryItemBases
                        .Where(x => x.InventoryDocumentId == inventoryDocument.Id)
                        .Select(x => x.GoodId)
                        .ToList();

                    foreach (Guid? goodID in goodIds)
                    {
                        Guid? articleId = context.ArticleGoods
                            .Where(x => x.GoodId == goodID)
                            .Select(x => x.ArticleId)
                            .FirstOrDefault();

                        if (articleId != null)
                        {
                            InventoryItemBasis? inventoryItemBase = context.InventoryItemBases
                           .Where(x => x.InventoryDocumentId == inventoryDocument.Id && x.GoodId == goodID)
                           .FirstOrDefault();

                            decimal sellingPrice = context.Articles.Where(x => x.Id == articleId).FirstOrDefault().Price;

                            decimal taxValue = (decimal)context.Taxes.SingleOrDefault(x => x.Value == 25)?.Value;

                            if (taxValue != null)
                            {
                                decimal basePrice = sellingPrice / (1 + (taxValue / 100));
                                basePrice *= inventoryItemBase.Quantity;// izračunaj osnovnu cijenu
                                totalBasePrice += basePrice;
                            }
                        }
                    }

                    return Task.FromResult(Math.Round(totalBasePrice, 2));
                }
            }
            catch
            {
                return Task.FromResult(totalBasePrice);

            }

        }
    }
}
