using System.Collections.Generic;
using Onion.Domain.Models;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IDepartmentService
    {

        List<DepartmentVM> GetAllDepartments();

    }
}