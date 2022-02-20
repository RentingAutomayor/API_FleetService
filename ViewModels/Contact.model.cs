using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public interface IContactModel : IPersonModel
		{
				string email { get; set; }
				JobTitleViewModel jobTitle { get; set; }
				CityViewModel city { get; set; }
				BranchViewModel branch { get; set; }
				
		}

		public interface IContactModelDTO : IContactModel {
				int? Client_id { get; set; }
				int? Dealer_id { get; set; }
		}
}

