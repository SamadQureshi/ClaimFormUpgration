using System.Data.Entity;
using Onion.Domain.Models;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Threading;

namespace Onion.Interfaces
{
    public interface IApplicationDBContext
    {
       
        IDbSet<T> Set<T>() where T : class;
        DbEntityEntry Entry(object entity);
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();

        void SaveChanges(string username);

    }
}
