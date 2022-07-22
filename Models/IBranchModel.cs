using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_FleetService.ViewModels;

namespace API_FleetService.Models
{
		public interface IBranchModel: IPersonModel
		{
				bool isMain { get; set; }
				CityViewModel city { get; set; }
		}

		public interface IBranchModelDTO : IBranchModel
		{
				int? Client_id { get; set; }
				int? Dealer_id { get; set; }
		}
}
