using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public partial interface IPersonModel
		{
				int? id { get; set; }
				string document { get; set; }
				string name { get; set; }
				string lastname { get; set; }
				string phone { get; set; }
				string cellphone { get; set; }
				string address { get; set; }
				string website { get; set; }
				bool? state { get; set; }
				DateTime? registrationDate { get; set; }
				DateTime? updateDate { get; set; }
				DateTime? deleteDate { get; set; }
		}
}