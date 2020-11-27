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
				public IHttpActionResult GetMaintenanceRoutines()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsMaintenanceRoutines = db.maintenanceRoutine.Where(mr => mr.mr_state == true)
																												.Select(mr => new MaintenanceRoutineViewModel
																												{
																														id = mr.mr_id,
																														name = mr.mr_name,
																														description = mr.mr_description,
																														vehicleModel = new VehicleModelViewModel
																														{
																																id = mr.vm_id,
																																shortName = mr.VehicleModel.vm_shortName,
																																longName = mr.VehicleModel.vm_longName,
																																brand = new BrandViewModel
																																{
																																		id = mr.VehicleModel.vb_id,
																																		name = mr.VehicleModel.VehicleBrand.vb_name
																																},
																																type = new VehicleTypeViewModel
																																{
																																		id = mr.VehicleModel.vt_id,
																																		name = mr.VehicleModel.VehicleType.vt_name
																																}
																														},
																														frequency = new FrequencyViewModel
																														{
																																id = mr.fq_id,
																																name = mr.frequency.fq_name
																														},
																														state = mr.mr_state,
																														referencePrice = mr.mr_referencePrice,
																														registrationDate = mr.mr_registrationDate

																												}).ToList()
																												.Take(100);
										return Ok(lsMaintenanceRoutines);
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
										var oRoutine = db.maintenanceRoutine.Where(mr => mr.mr_id == pRoutine_id)
																				.Select(mr => new MaintenanceRoutineViewModel
																				{
																						id = mr.mr_id,
																						name = mr.mr_name,
																						description = mr.mr_description,
																						vehicleModel = new VehicleModelViewModel
																						{
																								id = mr.vm_id,
																								shortName = mr.VehicleModel.vm_shortName,
																								longName = mr.VehicleModel.vm_longName,
																								brand = new BrandViewModel
																								{
																										id = mr.VehicleModel.vb_id,
																										name = mr.VehicleModel.VehicleBrand.vb_name
																								},
																								type = new VehicleTypeViewModel
																								{
																										id = mr.VehicleModel.vt_id,
																										name = mr.VehicleModel.VehicleType.vt_name
																								}
																						},
																						frequency = new FrequencyViewModel
																						{
																								id = mr.fq_id,
																								name = mr.frequency.fq_name
																						},
																						state = mr.mr_state,
																						referencePrice = mr.mr_referencePrice,
																						registrationDate = mr.mr_registrationDate
																				}).FirstOrDefault();

										var oLsItems = db.ItemsByRoutines.Where(ibr => ibr.mr_id == pRoutine_id)
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
																												referencePrice = ibr.MaintenanceItem.mi_referencePrice,
																												state = ibr.MaintenanceItem.mi_state,
																												amount = ibr.mi_amount,
																												registrationDate = ibr.MaintenanceItem.mi_registrationDate
																										}).ToList();

										oRoutine.lsItems = oLsItems;

										return Ok(oRoutine);
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
										var lsRoutines = db.maintenanceRoutine.Where(mr => mr.vm_id == model_id && mr.mr_state == true)
																				.Select(mr => new MaintenanceRoutineViewModel
																				{
																						id = mr.mr_id,
																						name = mr.mr_name,
																						description = mr.mr_description,
																						vehicleModel = new VehicleModelViewModel
																						{
																								id = mr.vm_id,
																								shortName = mr.VehicleModel.vm_shortName,
																								longName = mr.VehicleModel.vm_longName,
																								brand = new BrandViewModel
																								{
																										id = mr.VehicleModel.vb_id,
																										name = mr.VehicleModel.VehicleBrand.vb_name
																								},
																								type = new VehicleTypeViewModel
																								{
																										id = mr.VehicleModel.vt_id,
																										name = mr.VehicleModel.VehicleType.vt_name
																								}
																						},
																						frequency = new FrequencyViewModel
																						{
																								id = mr.fq_id,
																								name = mr.frequency.fq_name
																						},
																						state = mr.mr_state,
																						referencePrice = mr.mr_referencePrice,
																						registrationDate = mr.mr_registrationDate
																				}).ToList();


										foreach (var oRoutine in lsRoutines) {
												var oLsItems = db.ItemsByRoutines.Where(ibr => ibr.mr_id == oRoutine.id)
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
																														referencePrice = ibr.MaintenanceItem.mi_referencePrice,
																														state = ibr.MaintenanceItem.mi_state,
																														amount = ibr.mi_amount,
																														registrationDate = ibr.MaintenanceItem.mi_registrationDate
																												}).ToList();

												oRoutine.lsItems = oLsItems;
										}



										return Ok(lsRoutines);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]

				public IHttpActionResult Insert(MaintenanceRoutineViewModel pRoutine)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();
										maintenanceRoutine routineDB = new maintenanceRoutine();

										if (pRoutine.name == "")
										{
												throw new Exception("El nombre de la rutina no puede ser vacío");
										}

										if (pRoutine.vehicleModel == null)
										{
												throw new Exception("La rutina de mantenimiento debe tener una línea asociada");
										}


										if (pRoutine.frequency == null)
										{
												throw new Exception("La rutina de mantenimiento debe tener una frecuencia asociada");
										}


										routineDB.mr_name = pRoutine.name.ToUpper();
										routineDB.mr_description = pRoutine.description;
										routineDB.vm_id = pRoutine.vehicleModel.id;
										routineDB.fq_id = pRoutine.frequency.id;
										routineDB.mr_referencePrice = pRoutine.referencePrice;
										routineDB.mr_state = true;
										routineDB.mr_registrationDate = DateTime.Now;

										db.maintenanceRoutine.Add(routineDB);
										db.SaveChanges();

										var oRoutineDB = db.maintenanceRoutine.Where(mr => mr.mr_name == pRoutine.name.ToUpper()
																																&& mr.vm_id == pRoutine.vehicleModel.id
																																&& mr.fq_id == pRoutine.frequency.id)
																																.OrderByDescending(mr => mr.mr_registrationDate)
																																.FirstOrDefault();

										foreach (var item in pRoutine.lsItems)
										{
												ItemsByRoutines oItemByRoutine = new ItemsByRoutines();
												oItemByRoutine.mr_id = oRoutineDB.mr_id;
												oItemByRoutine.mi_id = item.id;
												oItemByRoutine.mi_amount = item.amount;
												oItemByRoutine.ibr_registrationDate = DateTime.Now;
												db.ItemsByRoutines.Add(oItemByRoutine);
												db.SaveChanges();
										}


										rta.response = true;
										rta.message = "Se ha insertado en la base de datos la rutina: " + pRoutine.name;

										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]

				public IHttpActionResult Update(MaintenanceRoutineViewModel pRoutine)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();
										

										if (pRoutine.name == "")
										{
												throw new Exception("El nombre de la rutina no puede ser vacío");
										}

										if (pRoutine.vehicleModel == null)
										{
												throw new Exception("La rutina de mantenimiento debe tener una línea asociada");
										}


										if (pRoutine.frequency == null)
										{
												throw new Exception("La rutina de mantenimiento debe tener una frecuencia asociada");
										}

										var routineDB = db.maintenanceRoutine.Where(mr => mr.mr_id == pRoutine.id).FirstOrDefault();

										routineDB.mr_name = pRoutine.name.ToUpper();
										routineDB.mr_description = pRoutine.description;
										routineDB.vm_id = pRoutine.vehicleModel.id;
										routineDB.fq_id = pRoutine.frequency.id;
										routineDB.mr_referencePrice = pRoutine.referencePrice;										
										routineDB.mr_updateDate = DateTime.Now;
										db.SaveChanges();

										var lsOldItems = db.ItemsByRoutines.Where(ibr => ibr.mr_id == pRoutine.id).ToList();

										foreach (var oldItem in lsOldItems) {
												db.ItemsByRoutines.Remove(oldItem);
												db.SaveChanges();
										}
										
									

										foreach (var item in pRoutine.lsItems)
										{
												ItemsByRoutines oItemByRoutine = new ItemsByRoutines();
												oItemByRoutine.mr_id = routineDB.mr_id;
												oItemByRoutine.mi_id = item.id;
												oItemByRoutine.mi_amount = (float)item.amount;
												oItemByRoutine.ibr_registrationDate = DateTime.Now;
												db.ItemsByRoutines.Add(oItemByRoutine);
												db.SaveChanges();
										}


										rta.response = true;
										rta.message = "La rutina " + pRoutine.name + " se ha actualizado en la base de datos.";

										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}



				[HttpPost]
				public IHttpActionResult Delete(MaintenanceRoutineViewModel pRoutine) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var oRoutineDb = db.maintenanceRoutine.Where(mr => mr.mr_id == pRoutine.id).FirstOrDefault();
										oRoutineDb.mr_state = false;
										oRoutineDb.mr_deleteDate = DateTime.Now;
										db.SaveChanges();
										var rta = new ResponseApiViewModel();
										rta.response = true;
										rta.message = "Se ha eliminado de la base de datos la rutina: " + pRoutine.name; 
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
		}

}
