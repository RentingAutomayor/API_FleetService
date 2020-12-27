using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class ContractViewModel
		{
				public Nullable<int> id;
				public Nullable<int> consecutive;
				public string code;
				public string name;
				public string observation;
				public DealerViewModel dealer;
				public ClientViewModel client;
				public ContractStateViewModel contractState;
				public DiscountTypeViewModel discountType;
				public Nullable<int> discountValue;
				public Nullable<int> amountOfMaintenances;
				public Nullable<int> amountVehicles;
				public Nullable<DateTime> startingDate;
				public Nullable<DateTime> endingDate;
				public Nullable<int> duration;
				public Nullable<bool> state;
				public List<VehicleModelViewModel> lsVehicleModels;
				public List<VehicleViewModel> lsVehicles;
				public Nullable<DateTime> registrationDate;
		}
}