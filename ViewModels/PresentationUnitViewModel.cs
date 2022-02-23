using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class PresentationUnitViewModel
		{
				public Nullable<int> id;
				public string shortName;
				public string longName;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
		}
}