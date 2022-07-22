using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_FleetService.ViewModels;

namespace API_FleetService.Models
{
    public interface IDealerModel : IPersonModel
    {
        List<ContactViewModel> contacts { get; set; }
        List<BranchViewModel> branches { get; set; }
        List<MaintenanceItemViewModel> maintenanceItems { get; set; }
    }

    public interface IDealerModelDTO : IDealerModel, IBussinessObject
    {
    }
}