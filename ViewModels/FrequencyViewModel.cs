using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class FrequencyViewModel
		{
				public Nullable<int> id;
				public string name;
				public string shortName;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
		}
}