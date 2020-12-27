using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class PricesByContractViewModel
		{
				public ContractViewModel contract;
				public List<MaintenanceItemViewModel> lsMaintenanceItems;
		}
}