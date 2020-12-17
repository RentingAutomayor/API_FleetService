using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class FinancialInformationViewModel
		{
				public Nullable<int> id;
				public ClientViewModel client;
				public double approvedQuota;
				public double consumedQuota;
				public double currentQuota;
				public double inTransitQuota;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
				public Nullable<DateTime> updateDate;
		}
}