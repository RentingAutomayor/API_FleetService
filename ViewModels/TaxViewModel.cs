using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TaxViewModel
		{
				public Nullable<int> id;
				public string name;
				public string description;
				public int percentValue;
				public Nullable<DateTime> registrationDate;
		}
}