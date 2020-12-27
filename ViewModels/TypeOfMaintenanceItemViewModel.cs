using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TypeOfMaintenanceItemViewModel
		{
				public Nullable<int> id;
				public string name;
				public Nullable<bool> sate;
				public Nullable<DateTime> registrationDate;
		}
}