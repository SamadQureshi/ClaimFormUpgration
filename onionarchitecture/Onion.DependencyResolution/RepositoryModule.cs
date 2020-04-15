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
            Bind<IBaseRepository<OpdExpense_Image>>().To<BaseRepository<OpdExpense_Image>>();
            Bind<IBaseRepository<OpdExpense_Patient>>().To<BaseRepository<OpdExpense_Patient>>();
            Bind<IBaseRepository<Department>>().To<BaseRepository<Department>>();
            Bind<IBaseRepository<RelationShip_Employee>>().To<BaseRepository<RelationShip_Employee>>();
            Bind<IBaseRepository<TravelExpense>>().To<BaseRepository<TravelExpense>>();
        }
    }
}
