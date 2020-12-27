using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TransactionObservationViewModel
		{
				public Nullable<int> id;
				public string description;
				//TODO: change this by the model of users
				public int usu_id;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
		}
}