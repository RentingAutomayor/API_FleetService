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
				public Nullable<int> Client_id;


				public static Vehicle setDataToVehicle(VehicleViewModel pVehicle) {
						if (pVehicle.licensePlate.Trim() == "") {
								throw new Exception("El campo de placa no puede estar vacío");
						}

						if (VehicleViewModel.vehicleExists(pVehicle)) {
								throw new Exception("El vehículo que intenta guardar, ya se encuentra almacenado en la base de datos");
						}

						if (pVehicle.vehicleModel == null) {
								throw new Exception("El vehículo que intenta guardar no tiene una línea asociada, recuerde que este dato es importante para poder asociar las rutinas de mantenimiento");
						}

						Vehicle oVehicle = new Vehicle();
						oVehicle.veh_licensePlate = pVehicle.licensePlate;
						oVehicle.veh_chasisCode = pVehicle.chasisCode;
						if (pVehicle.vehicleState != null) {
								oVehicle.vs_id = pVehicle.vehicleState.id;
						}
						oVehicle.vm_id = pVehicle.vehicleModel.id;
						oVehicle.veh_year = pVehicle.year;
						oVehicle.veh_mileage = pVehicle.mileage;
						oVehicle.veh_state = true;
						oVehicle.veh_registrationDate = DateTime.Now;
						oVehicle.cli_id = pVehicle.Client_id;


						return oVehicle;
				}


				public static bool InsertIntoDB(Vehicle pVehicle) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										db.Vehicle.Add(pVehicle);
										db.SaveChanges();
										return true;
								}
						}
						catch (Exception ex)
						{
								throw new Exception(ex.InnerException.Message);
						}
				}

				public static bool vehicleExists(VehicleViewModel pVehicle) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{


										var vehicle = new Vehicle();
										if (pVehicle.chasisCode != "")
										{
												vehicle = db.Vehicle.Where(vh => vh.veh_licensePlate == pVehicle.licensePlate || vh.veh_chasisCode == pVehicle.chasisCode).FirstOrDefault();
										}
										else {
												vehicle = db.Vehicle.Where(vh => vh.veh_licensePlate == pVehicle.licensePlate).FirstOrDefault();
										}
											

										if (vehicle != null)
										{
												return true;
										}
										else {
												return false;
										}
										
								}
						}
						catch (Exception ex)
						{
								throw new Exception(ex.InnerException.Message);
						}
				}

		}
}