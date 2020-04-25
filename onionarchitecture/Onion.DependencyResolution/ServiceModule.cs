using Ninject.Modules;
using Onion.Interfaces.Services;
using Onion.Services;

namespace Onion.DependencyResolution
{
    public class ServiceModule : NinjectModule
    {
        // Get config service
        public override void Load()
        {
            Bind<IUserService>().To<UserService>();
            Bind<IDepartmentService>().To<DepartmentService>();
            Bind<IOpdExpenseService>().To<OpdExpenseService>();
            Bind<IOpdExpenseImageService>().To<OpdExpenseImageService>();
            Bind<IOpdExpensePatientService>().To<OpdExpensePatientService>();
            Bind<ITravelExpenseService>().To<TravelExpenseService>();
            Bind<IExpenseTypeService>().To<ExpenseTypeService>();
        }
    }
}