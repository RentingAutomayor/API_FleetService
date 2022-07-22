using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class QuotaTypeViewModel
		{
				public Nullable<int> id;
				public string name;
				public string description;
				public bool state;
				public Nullable<DateTime> registrationDate;
				public Nullable<DateTime> updateDate;
				public Nullable<DateTime> deleteDate;
		}
}