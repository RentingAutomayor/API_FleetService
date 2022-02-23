using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class ContactViewModel :  IContactModelDTO
		{
				int? IPersonModel.id { get; set; }
				public string email { get ; set ; }
				public JobTitleViewModel jobTitle { get ; set ; }
				public CityViewModel city { get ; set ; }				
				public int id { get ; set ; }
				public string document { get ; set ; }
				public string name { get ; set ; }
				public string lastname { get ; set ; }
				public string phone { get ; set ; }
				public string cellphone { get ; set ; }
				public string address { get ; set ; }
				public string website { get ; set ; }
				public bool? state { get ; set ; }
				public DateTime? registrationDate { get ; set ; }
				public DateTime? updateDate { get ; set ; }
				public DateTime? deleteDate { get ; set ; }
				public BranchViewModel branch { get ; set ; }
				public int? Client_id { get ; set ; }
				public int? Dealer_id { get ; set ; }
		}
}