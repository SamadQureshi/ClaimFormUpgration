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
            Bind<IOpdExpense_ImageService>().To<OpdExpense_ImageService>();
            Bind<IOpdExpense_PatientService>().To<OpdExpense_PatientService>();           
        }
    }
}