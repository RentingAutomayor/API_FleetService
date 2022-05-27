using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class ContactType
		{
				public Nullable<int> id { get; set; }
				public string name { get; set; }

				public Nullable<bool> state { get; set; }
				public DateTime registrationDate { get; set; }
				public DateTime? updateDate { get; set; }
				public DateTime? deleteDate { get; set; }
				public List<ContactsWithTypesViewModel> contactsWithTypes { get; set; }
	}
}