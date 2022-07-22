using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class MaintenanceRoutineViewModel
		{
				public Nullable<int> id;
				public string name;
				public string description;
				public VehicleModelViewModel vehicleModel;
				public FrequencyViewModel frequency;
				public Nullable<bool> state;
				public Nullable<float> referencePrice;
				public List<MaintenanceItemViewModel> lsItems;
				public Nullable<DateTime> registrationDate;
		}
}