using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Onion.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        object Add(T obj);
        void Update(T obj);
        void Delete(object id);
        void Delete(T entity);
        void RemoveRange(List<T> entities);


       
        T GetOne(
            Expression<Func<T, bool>> filter = null);

        Task<T> GetOneAsync(
            Expression<Func<T, bool>> filter = null);

        T GetFirst(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<T> GetFirstAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        T GetById(object id);

        int GetCount(Expression<Func<T, bool>> filter = null);

        Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null);

        bool GetExists(Expression<Func<T, bool>> filter = null);

        Task<bool> GetExistsAsync(Expression<Func<T, bool>> filter = null);

        IQueryable<T> GetQueryable(
             Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             int? skip = null,
             int? take = null);

        IQueryable<T> Query();
    }

}
