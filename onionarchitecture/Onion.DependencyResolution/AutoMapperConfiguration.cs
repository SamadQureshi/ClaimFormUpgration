using AutoMapper;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.DependencyResolution
{
    public class AutomapperConfiguration
    {
        public static void IntializeAutomapper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap(typeof(User), typeof(UserVM));
                config.CreateMap(typeof(Department), typeof(DepartmentVM));

            });
        }
    }
}