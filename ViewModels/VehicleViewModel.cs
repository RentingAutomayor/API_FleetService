using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class VehicleViewModel
		{
				public int? id;
				public string licensePlate;
				public string chasisCode;
				public VehicleStateViewModel vehicleState;
				public VehicleModelViewModel vehicleModel;
				public string year;
				public Nullable<int> mileage;
				public Nullable<bool> state;
				public DateTime? registrationDate;
				public DateTime? updateDate;
				public DateTime? deleteDate;
				public Nullable<int> Client_id;	
		}
}