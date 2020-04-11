using System.Data.Entity;
using Onion.Domain.Models;
using System.Data.Entity.Infrastructure;

namespace Onion.Interfaces
{
    public interface IApplicationDBContext
    {
        IDbSet<User> Users { get; set; }
        IDbSet<T> Set<T>() where T : class;
        DbEntityEntry Entry(object entity);
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}
