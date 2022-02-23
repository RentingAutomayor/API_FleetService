using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;
using API_FleetService.Models;

namespace API_FleetService.ViewModels
{
		public class DealerViewModel : IPersonModel, IBussinessObject
		{
				public int? id { get; set; }
				public string document { get; set; }
				public string name { get; set; }
				public string lastname { get; set; }
				public string phone { get; set; }
				public string cellphone { get; set; }
				public string address { get; set; }
				public string website { get; set; }
				public bool? state { get; set; }
				public DateTime? registrationDate { get; set; }
				public DateTime? updateDate { get; set; }
				public DateTime? deleteDate { get; set; }
				bool IBussinessObject.state { get; set; }

				public List<ContactViewModel> contacts;

				public List<BranchViewModel> branches;

				public List<MaintenanceItemViewModel> maintenanceItems;

		}
}