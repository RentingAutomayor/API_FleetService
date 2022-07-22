using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class PersonViewModel
		{
				public Nullable<int> id;
				public string document;
				public string name;
				public string lastname;
				public string phone;
				public string cellphone;
				public string address;
				public string website;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
				public Nullable<DateTime> updateDate;
				public Nullable<DateTime> deleteDate;
		}
}