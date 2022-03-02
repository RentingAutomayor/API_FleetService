using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
		public class MaintenanceRoutineController : ApiController
		{
				[HttpGet]
				public IHttpActionResult GetFrequency()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsFrequency = db.frequency.Where(fq => fq.fq_state == true)
																				.Select(fq => new FrequencyViewModel
																				{
																						id = fq.fq_id,
																						name = fq.fq_name,
																						shortName = fq.fq_shortName,
																						state = fq.fq_state,
																						registrationDate = fq.fq_registrationDate
																				}).ToList();
										return Ok(lsFrequency);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetMaintenanceRoutines(int vehicleModel_id = 0, int frequency_id = 0)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										List<MaintenanceRoutineViewModel> maintenanceRoutines = new List<MaintenanceRoutineViewModel>();
										IQueryable<maintenanceRoutine> lsMaintenanceRoutinesDB = null;
										if (vehicleModel_id == 0 && frequency_id == 0){
												lsMaintenanceRoutinesDB = db.maintenanceRoutine.Where(mr => mr.mr_state == true);																																							
										}
										else {
												lsMaintenanceRoutinesDB = db.maintenanceRoutine.Where(mr => mr.mr_state == true && (mr.vm_id == vehicleModel_id || mr.frequency.fq_id == frequency_id));																								
										}

										foreach (var routineDb in lsMaintenanceRoutinesDB)
										{
												maintenanceRoutines.Add(MaintenanceRoutineController.formatData(routineDb));
										}

										return Ok(maintenanceRoutines);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetMaintenanceRoutineById(int pRoutine_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var routineDB = db.maintenanceRoutine.Where(mr => mr.mr_id == pRoutine_id).FirstOrDefault();
										var routine = MaintenanceRoutineController.formatData(routineDB);
										var maintenanceItems = MaintenanceRoutineController.getItemsByRoutineId((int)routine.id);
										routine.lsItems = maintenanceItems;
										return Ok(routine);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetMaintenanceRoutineByModelId(int model_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsRoutines = MaintenanceRoutineController.getListMaintenanceRoutinesByVehicleModelId(model_id);


										foreach (var routine in lsRoutines) {
												var maintenanceItems = MaintenanceRoutineController.getItemsByRoutineId((int)routine.id);
												routine.lsItems = maintenanceItems;
																																
										}
										//This number 21 represent the id of frequency of corrective maintenance
										//The setting of maintenance routine it's because that MR doesn't have maintenance items configurates
										//and we need to send an empty array
										int correctiveMaintenance = 21;

										var lsRoutinesFiltered = from routine in lsRoutines
																						 where (routine.lsItems != null)
																						 && (routine.frequency.id != correctiveMaintenance)
																						 select routine;

										var correctiveRoutine = (from routine in lsRoutines
																						where routine.frequency.id == correctiveMaintenance
																						select routine).First();

										correctiveRoutine.lsItems = new List<MaintenanceItemViewModel>();
										var lsToReturn = lsRoutinesFiltered.ToList();										
										lsToReturn.Add(correctiveRoutine);
										return Ok(lsToReturn);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]

				public IHttpActionResult Insert(MaintenanceRoutineViewModel routine)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();
										maintenanceRoutine routineDB = new maintenanceRoutine();

										if (MaintenanceRoutineController.validateDataMaintenanceRoutine(routine))
										{
												MaintenanceRoutineController.setDataToDB(routine, ref routineDB, true);
												db.maintenanceRoutine.Add(routineDB);
												db.SaveChanges();
												var routineSaved = MaintenanceRoutineController.getLastRoutineInserted();
												MaintenanceRoutineController.configureItemsByRoutineId((int)routineSaved.mr_id, routine.lsItems, true);
												rta.response = true;
												rta.message = "Se ha insertado en la base de datos la rutina: " + routine.name;
										}						
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPut]

				public IHttpActionResult Update(MaintenanceRoutineViewModel routine)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();

										if (MaintenanceRoutineController.validateDataMaintenanceRoutine(routine)) {								
												var routineDB = db.maintenanceRoutine.Where(mr => mr.mr_id == routine.id).FirstOrDefault();
												MaintenanceRoutineController.setDataToDB(routine, ref routineDB, false);
												db.SaveChanges();
												MaintenanceRoutineController.configureItemsByRoutineId((int)routine.id, routine.lsItems, false);
												rta.response = true;
												rta.message = "La rutina " + routine.name + " se ha actualizado en la base de datos.";
										}
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpDelete]
				public IHttpActionResult Delete(int routineId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oRoutineDb = db.maintenanceRoutine.Where(mr => mr.mr_id == routineId).FirstOrDefault();
										oRoutineDb.mr_state = false;
										oRoutineDb.mr_deleteDate = DateTime.Now;
										db.SaveChanges();
										var rta = new ResponseApiViewModel();
										rta.response = true;
										rta.message = "Se ha eliminado de la base de datos la rutina: " + oRoutineDb.mr_name;
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
				private static bool validateDataMaintenanceRoutine(MaintenanceRoutineViewModel routine)
				{
						if (routine.name == "")
						{
								throw new Exception("El nombre de la rutina no puede ser vacío");
						}

						if (routine.vehicleModel == null)
						{
								throw new Exception("La rutina de mantenimiento debe tener una línea asociada");
						}


						if (routine.frequency == null)
						{
								throw new Exception("La rutina de mantenimiento debe tener una frecuencia asociada");
						}

						return true;
				}

				private static void setDataToDB(MaintenanceRoutineViewModel routine, ref maintenanceRoutine routineDB, bool isToInsert) {
						try
						{
								routineDB.mr_name = routine.name.ToUpper();
								routineDB.mr_description = routine.description;
								routineDB.vm_id = routine.vehicleModel.id;
								routineDB.fq_id = routine.frequency.id;
								routineDB.mr_referencePrice = routine.referencePrice;
								if (isToInsert)
								{
										routineDB.mr_state = true;
										routineDB.mr_registrationDate = DateTime.Now;
								}
								else
								{
										routineDB.mr_updateDate = DateTime.Now;
								}
						}
						catch (Exception ex)
						{
								throw ex;
						}					
				}		

				[HttpGet]
				public IHttpActionResult ValidateRoutineAndFrequency(int vehicleModel_id, int frequency_id ) {
						try
						{
								using (DB_FleetServiceEntities db  = new DB_FleetServiceEntities())
								{
										var rta = new ResponseApiViewModel();
										var oMaintenanceRoutine = db.maintenanceRoutine.Where(mr => mr.mr_state == true && mr.vm_id == vehicleModel_id && mr.fq_id == frequency_id).FirstOrDefault();
										if (oMaintenanceRoutine != null)
										{
												rta.response = false;
												rta.message = "Ya existe una rutina para la línea seleccionada con ese kilometraje";
										}
										else {
												rta.response = true;
												rta.message = "";
										}

										return Ok(rta);
								}
						}		
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				private static List<MaintenanceRoutineViewModel> getListMaintenanceRoutinesByVehicleModelId(int vehicleModelId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var routines = db.maintenanceRoutine
												.Where(mr => mr.vm_id == vehicleModelId && mr.mr_state == true)
												.ToList();

										var routinesFormmated = new List<MaintenanceRoutineViewModel>();
										foreach (var routine in routines)
										{
												routinesFormmated.Add(MaintenanceRoutineController.formatData(routine));
										}

										return routinesFormmated;
								}
						}
						catch (Exception ex)
						{

								throw ex;
						}

				}

				private static MaintenanceRoutineViewModel formatData(maintenanceRoutine maintenanceRoutineDB) {

						var routine = new MaintenanceRoutineViewModel
						{
								id = maintenanceRoutineDB.mr_id,
								name = maintenanceRoutineDB.mr_name,
								description = maintenanceRoutineDB.mr_description,
								vehicleModel = new VehicleModelViewModel
								{
										id = maintenanceRoutineDB.vm_id,
										shortName = maintenanceRoutineDB.VehicleModel.vm_shortName,
										longName = maintenanceRoutineDB.VehicleModel.vm_longName,
										brand = new BrandViewModel
										{
												id = maintenanceRoutineDB.VehicleModel.vb_id,
												name = maintenanceRoutineDB.VehicleModel.VehicleBrand.vb_name
										},
										type = new VehicleTypeViewModel
										{
												id = maintenanceRoutineDB.VehicleModel.vt_id,
												name = maintenanceRoutineDB.VehicleModel.VehicleType.vt_name
										}
								},
								frequency = new FrequencyViewModel
								{
										id = maintenanceRoutineDB.fq_id,
										name = maintenanceRoutineDB.frequency.fq_name
								},
								state = maintenanceRoutineDB.mr_state,
								referencePrice = maintenanceRoutineDB.mr_referencePrice,
								registrationDate = maintenanceRoutineDB.mr_registrationDate
						};

						return routine;
				}

				private static List<MaintenanceItemViewModel> getItemsByRoutineId(int routineId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var items = db.ItemsByRoutines
												.Where(ibr => ibr.mr_id == routineId)
												.Select(ibr => new MaintenanceItemViewModel
												{
														id = ibr.mi_id,
														code = ibr.MaintenanceItem.mi_code,
														name = ibr.MaintenanceItem.mi_name,
														description = ibr.MaintenanceItem.mi_description,
														type = new TypeOfMaintenanceItemViewModel
														{
																id = ibr.MaintenanceItem.tmi_id,
																name = ibr.MaintenanceItem.TypeOfMaintenanceItem.tmi_name
														},
														presentationUnit = new PresentationUnitViewModel
														{
																id = ibr.MaintenanceItem.pu_id,
																shortName = ibr.MaintenanceItem.PresentationUnit.pu_shortName,
																longName = ibr.MaintenanceItem.PresentationUnit.pu_longName
														},
														category = new CategoryViewModel
														{
																id = ibr.MaintenanceItem.mict_id,
																name = ibr.MaintenanceItem.MaintenanceItemCategory.mict_name
														},
														referencePrice = ibr.MaintenanceItem.mi_referencePrice,
														state = ibr.MaintenanceItem.mi_state,
														amount = ibr.mi_amount,
														handleTax = ibr.MaintenanceItem.mi_handleTax,
														registrationDate = ibr.MaintenanceItem.mi_registrationDate
												}).ToList();


										foreach (var maintenanceItem in items)
										{
												if (maintenanceItem.handleTax == true)
												{
														var lsTaxes = MaintenanceItemController.getTaxesByItemId((int)maintenanceItem.id);
														if (lsTaxes != null)
														{
																maintenanceItem.lsTaxes = lsTaxes;
														}
												}

										}
										return items;
								}
						}
						catch (Exception ex)
						{

								throw ex;
						}
				}

				private static maintenanceRoutine getLastRoutineInserted() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oRoutineDB = db.maintenanceRoutine.OrderByDescending(mr => mr.mr_id).FirstOrDefault();
										return oRoutineDB;
								}
						}
						catch (Exception ex)
						{

								throw ex;
						}
				}

				private static void configureItemsByRoutineId(int routineId, List<MaintenanceItemViewModel> maintenanceItems, bool isToInsert) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										if (!isToInsert) {
												var lsOldItems = db.ItemsByRoutines.Where(ibr => ibr.mr_id == routineId).ToList();
												foreach (var oldItem in lsOldItems)
												{
														db.ItemsByRoutines.Remove(oldItem);
														db.SaveChanges();
												}
										}							

										foreach (var item in maintenanceItems)
										{
												ItemsByRoutines oItemByRoutine = new ItemsByRoutines();
												oItemByRoutine.mr_id = routineId;
												oItemByRoutine.mi_id = item.id;
												oItemByRoutine.mi_amount = (float)item.amount;
												oItemByRoutine.ibr_registrationDate = DateTime.Now;
												db.ItemsByRoutines.Add(oItemByRoutine);
												db.SaveChanges();
										}
								}
						}
						catch (Exception ex)
						{

								throw ex;
						}
				
				}
		}

}
