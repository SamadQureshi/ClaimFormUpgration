using Ninject.Modules;
using Onion.Data;
using Onion.Domain.Models;
using Onion.Interfaces;
using Onion.Repository.Repositories;

namespace Onion.DependencyResolution
{
    public class RepositoryModule : NinjectModule
    {
        // Get config service
        public override void Load()
        {
            Bind<IApplicationDBContext>().To<ApplicationDBContext>().WithConstructorArgument("Name", "ApplicationConnectionString");
            Bind<IBaseRepository<User>>().To<BaseRepository<User>>();
            Bind<IBaseRepository<OpdExpense>>().To<BaseRepository<OpdExpense>>();
            Bind<IBaseRepository<OpdExpenseImage>>().To<BaseRepository<OpdExpenseImage>>();
            Bind<IBaseRepository<OpdExpensePatient>>().To<BaseRepository<OpdExpensePatient>>();
            Bind<IBaseRepository<Department>>().To<BaseRepository<Department>>();
            Bind<IBaseRepository<RelationShipEmployee>>().To<BaseRepository<RelationShipEmployee>>();
            Bind<IBaseRepository<TravelExpense>>().To<BaseRepository<TravelExpense>>();
            Bind<IBaseRepository<ExpenseType>>().To<BaseRepository<ExpenseType>>();
        }
    }
}
