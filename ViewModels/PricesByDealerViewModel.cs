using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class PricesByDealerViewModel
		{
				public DealerViewModel dealer;
				public List<MaintenanceItemViewModel> lsMaintenanceItems;
		}
}