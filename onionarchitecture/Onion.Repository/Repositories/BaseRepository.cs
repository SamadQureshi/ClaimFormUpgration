using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using Onion.Interfaces;

namespace Onion.Repository.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        internal readonly IApplicationDBContext _context;
        private IDbSet<T> table;

        public BaseRepository(IApplicationDBContext context)
        {
            this._context = context;
            table = _context.Set<T>();
        }

        public T GetById(object id)
        {
            var entity = table.Find(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public object Add(T obj , string userName)
        {
           
           table.Add(obj);
           _context.SaveChanges(userName);             
            return obj;
        }
        public void Update(T obj, string userName)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            _context.SaveChanges(userName);
        }

        public void Delete(object id, string userName)
        {
            T existing = table.Find(id);
            table.Remove(existing);
            _context.SaveChanges(userName);
        }

        public void Delete(T existing, string userName)
        {
            table.Remove(existing);
            _context.SaveChanges(userName);
        }

        public void RemoveRange(List<T> entities)
        {
            foreach (var list in entities)
            {
                table.Attach(list);
                table.Remove(list);
            }
            _context.SaveChanges();
        }

        public IQueryable<T> GetQueryable(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           int? skip = null,
           int? take = null)

        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        

        public virtual T GetOne(
            Expression<Func<T, bool>> filter = null)

        {
            return GetQueryable(filter, null).SingleOrDefault();
        }

        public virtual async Task<T> GetOneAsync(
            Expression<Func<T, bool>> filter = null)

        {
            return await GetQueryable(filter, null).SingleOrDefaultAsync();
        }

        public virtual T GetFirst(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)

        {
            return GetQueryable(filter, orderBy).FirstOrDefault();
        }

        public virtual async Task<T> GetFirstAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)

        {
            return await GetQueryable(filter, orderBy).FirstOrDefaultAsync();
        }


        public virtual int GetCount(Expression<Func<T, bool>> filter = null)

        {
            return GetQueryable(filter).Count();
        }

        public virtual Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null)

        {
            return GetQueryable(filter).CountAsync();
        }

        public virtual bool GetExists(Expression<Func<T, bool>> filter = null)

        {
            return GetQueryable(filter).Any();
        }

        public virtual Task<bool> GetExistsAsync(Expression<Func<T, bool>> filter = null)

        {
            return GetQueryable(filter).AnyAsync();
        }

        public virtual IQueryable<T> Query()
        {
            return table;
        }
    }
}
