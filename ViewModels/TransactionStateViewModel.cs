using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TransactionStateViewModel
		{
				public Nullable<int> id;
				public string name;
				public string description;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
		}
}