using AutoMapper;
using Azure.Core;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Generic
{
    public class BaseCRUDService<T, TSearch> : ICRUDService<T, TSearch> where T : class where TSearch : class
    {
        public DatabaseContextFactory _factory { get; set; }

        public BaseCRUDService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public virtual ICollection<T> Get(TSearch search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<T>();

                var list = entity.ToList();

                return list;
            }
        }

        public virtual T GetById(Guid id)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<T>();

                var item = entity.Find(id);

                return item;
            }
        }

        public virtual T Create(T request)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var set = Context.Set<T>();
               
                set.Add(request);
                Context.SaveChanges();

                return request;
            }
        }

        public virtual T Update(Guid id, T request)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var set = Context.Set<T>();

                var entity = set.Find(id);

                Context.SaveChanges();

                return entity;
            }
        }

        public virtual ICollection<T> Delete(Guid id)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var set = Context.Set<T>();

                var entity = set.Find(id);

                if(entity != null)
                    set.Remove(entity);

                Context.SaveChanges();

                return set.ToList();
            }
        }

       
    }
}
