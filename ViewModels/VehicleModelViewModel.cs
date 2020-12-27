using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class VehicleModelViewModel
		{
				public int? id;
				public string shortName;
				public string longName;
				public bool? state;
				public DateTime? registrationDate;
				public BrandViewModel brand;
				public VehicleTypeViewModel type;
		}
}