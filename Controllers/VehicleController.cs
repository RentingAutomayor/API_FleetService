﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
		public class VehicleController : ApiController
		{
				[HttpGet]
				public IHttpActionResult GetBrands()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsBrand = db.VehicleBrand.Where(brn => brn.vb_state == true)
																								.Select(brn => new BrandViewModel
																								{
																										id = brn.vb_id,
																										name = brn.vb_name,
																										state = brn.vb_state,
																										registrationDate = brn.vb_registrationDate
																								}).ToList();
										return Ok(lsBrand);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetVehicleType()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsVehicleType = db.VehicleType.Where(vt => vt.vt_state == true)
																								.Select(vt => new VehicleTypeViewModel
																								{
																										id = vt.vt_id,
																										name = vt.vt_name,
																										description = vt.vt_description,
																										state = vt.vt_state,
																										registrationDate = vt.vt_registrationDate
																								}).ToList();
										return Ok(lsVehicleType);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetVehicleModelsByTypes(string sLsTypes)
				{
						try
						{
								string[] aVehicleTypes = sLsTypes.Split(',');
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										List<VehicleModelViewModel> lsVehicleModels = new List<VehicleModelViewModel>();
										foreach (var item in aVehicleTypes)
										{
												var idType = int.Parse(item);
												var lsByType = db.VehicleModel.Where(vt => vt.vt_id == idType)
																						.Select(vm => new VehicleModelViewModel
																						{
																								id = vm.vm_id,
																								shortName = vm.vm_shortName,
																								longName = vm.vm_longName,
																								state = vm.vm_state,
																								registrationDate = vm.vm_registrationDate,
																								brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
																								type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
																						}).ToList();

												lsVehicleModels.AddRange(lsByType);
										}

										return Ok(lsVehicleModels);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
				[HttpGet]
				public IHttpActionResult GetVehicleModelByBrandAndType(int pId_brand = 0, int pId_vehicleType = 0)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsVehicleModel = new List<VehicleModelViewModel>();

										if (pId_brand != 0 && pId_vehicleType != 0)
										{
												lsVehicleModel = db.VehicleModel.Where(vm => vm.vm_state == true && vm.vb_id == pId_brand && vm.vt_id == pId_vehicleType)
																						.Select(vm => new VehicleModelViewModel
																						{
																								id = vm.vm_id,
																								shortName = vm.vm_shortName,
																								longName = vm.vm_longName,
																								state = vm.vm_state,
																								registrationDate = vm.vm_registrationDate,
																								brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
																								type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
																						}).ToList();
										}
										else if (pId_brand != 0 || pId_vehicleType != 0)
										{

												lsVehicleModel = db.VehicleModel.Where(vm => vm.vm_state == true && vm.vb_id == pId_brand || vm.vt_id == pId_vehicleType)
																					.Select(vm => new VehicleModelViewModel
																					{
																							id = vm.vm_id,
																							shortName = vm.vm_shortName,
																							longName = vm.vm_longName,
																							state = vm.vm_state,
																							registrationDate = vm.vm_registrationDate,
																							brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
																							type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
																					}).ToList();

										}
										else
										{
												lsVehicleModel = db.VehicleModel.Where(vm => vm.vm_state == true)
																								.Select(vm => new VehicleModelViewModel
																								{
																										id = vm.vm_id,
																										shortName = vm.vm_shortName,
																										longName = vm.vm_longName,
																										state = vm.vm_state,
																										registrationDate = vm.vm_registrationDate,
																										brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
																										type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
																								}).ToList();
										}

										return Ok(lsVehicleModel);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetVehicleStates()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsStates = db.vehicleState.Where(st => st.vs_state == true)
																				.Select(st => new VehicleStateViewModel
																				{
																						id = st.vs_id,
																						name = st.vs_name,
																						state = st.vs_state,
																						registrationDate = st.vs_registrationDate
																				}).ToList();
										return Ok(lsStates);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}


				

				[HttpGet]
				public IHttpActionResult GetVehiclesByClient(int pClient_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsVehicles = VehicleController.getListOfVehiclesByClientId(pClient_id);
										return Ok(lsVehicles);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetVehiclesByLicensePlate(string pLicensePlate)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var lsVehicles = db.Vehicle
												.Where(vh => vh.veh_state == true
																	&& vh.veh_licensePlate.ToUpper().Contains(pLicensePlate.ToUpper()))
																.Select(vh => new VehicleViewModel

																{
																		id = vh.veh_id,
																		licensePlate = vh.veh_licensePlate,
																		chasisCode = vh.veh_chasisCode,
																		vehicleState = new VehicleStateViewModel { id = vh.vehicleState.vs_id, name = vh.vehicleState.vs_name },
																		vehicleModel = new VehicleModelViewModel
																		{
																				id = vh.vm_id,
																				shortName = vh.VehicleModel.vm_shortName,
																				longName = vh.VehicleModel.vm_longName,
																				type = new VehicleTypeViewModel
																				{
																						id = vh.VehicleModel.vt_id,
																						name = vh.VehicleModel.VehicleType.vt_name
																				},
																				brand = new BrandViewModel
																				{
																						id = vh.VehicleModel.vb_id,
																						name = vh.VehicleModel.VehicleBrand.vb_name
																				}
																		},
																		year = vh.veh_year,
																		mileage = vh.veh_mileage,
																		state = vh.veh_state,
																		Client_id = vh.cli_id,
																		registrationDate = vh.veh_registrationDate
																}
																).ToList()
																.Take(10);
										return Ok(lsVehicles);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetVehiclesByClientAndModel(int pClient_id, string sModels, int contract_id = 0)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										string[] aModels = sModels.Split(',');
										List<int> lsModelsId = new List<int>();
										foreach (var item in aModels)
										{
												lsModelsId.Add(int.Parse(item));
										}

										var lsVehiclesDB = db.STRPRC_GET_VEHICLES_WITHOUT_CONTRACT(pClient_id, contract_id);
										var lsVehicles = new List<VehicleViewModel>();

										foreach (var vehicleDb in lsVehiclesDB)
										{
												VehicleViewModel vehicle = new VehicleViewModel()
												{
														id = vehicleDb.veh_id,
														licensePlate = vehicleDb.veh_licensePlate,
														chasisCode = vehicleDb.veh_chasisCode,
														vehicleState = new VehicleStateViewModel { id = vehicleDb.vs_id, name = vehicleDb.vs_name },
														vehicleModel = new VehicleModelViewModel
														{
																id = vehicleDb.vm_id,
																shortName = vehicleDb.vm_shortName,
																type = new VehicleTypeViewModel
																{
																		id = vehicleDb.vt_id,
																		name = vehicleDb.vt_name
																},
																brand = new BrandViewModel
																{
																		id = vehicleDb.vb_id,
																		name = vehicleDb.vb_name
																}
														},
														year = vehicleDb.veh_year,
														mileage = vehicleDb.veh_mileage,
														registrationDate = vehicleDb.veh_registrationDate
												};

												lsVehicles.Add(vehicle);

										}

										IEnumerable<VehicleViewModel> lsVehiclesFiltered = from vehicles in lsVehicles
																																			 where lsModelsId.Any(id => id == vehicles.vehicleModel.id)
																																			 select vehicles;


										return Ok(lsVehiclesFiltered);
								}
						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Insert(VehicleViewModel pVehicle)
				{
						try
						{
								var vehicleSaved = VehicleController.InsertVehicle(pVehicle);
								return Ok(vehicleSaved);													
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPut]
				public IHttpActionResult Update(VehicleViewModel pVehicle)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var vehicleUpdated = VehicleController.UpdateVehicle(pVehicle);
										return Ok(vehicleUpdated);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpDelete]
				public IHttpActionResult Delete(int vehicleId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var rta = new ResponseApiViewModel();
										if (VehicleController.DeleteVehicleById(vehicleId)) {
												rta.response = true;
												rta.message = "Se ha eliminado el vehículo de la base de datos";
										}								
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				private static bool validateVehicle(VehicleViewModel vehicle)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										if (vehicle.licensePlate.Trim() == "")
										{
												throw new Exception("El campo placa no puede ser vacío");
										}

										if (vehicle.vehicleModel == null)
										{
												throw new Exception("El vehículo que intenta guardar no tiene una línea asociada, recuerde que este dato es importante para poder asociar las rutinas de mantenimiento");
										}

										var oOtherVehicle = db.Vehicle
												.Where(vh => vh.veh_licensePlate == vehicle.licensePlate && vh.veh_id != vehicle.id && vh.veh_state == true)
												.FirstOrDefault();

										if (oOtherVehicle != null)
										{												
												throw new Exception("La placa con la cual intenta insertar ó actualizar el vehículo, ya se encuentra almacenada en la base de datos");
										}

										var oOtherVehicleWithChasisCode = db.Vehicle
											.Where(vh => vh.veh_chasisCode == vehicle.chasisCode && vh.veh_id != vehicle.id && vh.veh_state == true)
											.FirstOrDefault();

										if (oOtherVehicleWithChasisCode != null)
										{
												throw new Exception("Existe un vehículo creado con el mismo serial, por favor verifique");
										}

								}
								return true;
						}
						catch (Exception ex)
						{
								throw ex;
						}
				}


				private static void setDataToVehicle(VehicleViewModel pVehicle, ref Vehicle vehicleDB, bool isToInsert)
				{
						vehicleDB.veh_licensePlate = pVehicle.licensePlate;
						vehicleDB.veh_chasisCode = pVehicle.chasisCode;					
						vehicleDB.vm_id = pVehicle.vehicleModel.id;
						vehicleDB.veh_year = pVehicle.year;
						vehicleDB.veh_mileage = pVehicle.mileage;
						vehicleDB.cli_id = pVehicle.Client_id;
						vehicleDB.vs_id = (pVehicle.vehicleState != null) ? pVehicle.vehicleState.id : null;

						if (isToInsert)
						{
								vehicleDB.veh_state = true;
								vehicleDB.veh_registrationDate = DateTime.Now;
						}
						else {
								vehicleDB.veh_updateDate = DateTime.Now;
						}						
				}

				public static VehicleViewModel InsertVehicle(VehicleViewModel vehicle)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										Vehicle oVehicleDB = new Vehicle();
										if (VehicleController.validateVehicle(vehicle))
										{
												var vehicleDB = new Vehicle();
												VehicleController.setDataToVehicle(vehicle, ref vehicleDB, true);
												db.Vehicle.Add(vehicleDB);
												db.SaveChanges();

										}
										return VehicleController.getLastVehicleInserted();
								}
							
						}
						catch (Exception ex)
						{
								throw ex;
						}
				}		

				public static VehicleViewModel UpdateVehicle(VehicleViewModel pVehicle)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oVehicleDB = db.Vehicle.Where(vh => vh.veh_id == pVehicle.id).FirstOrDefault();
										if (VehicleController.validateVehicle(pVehicle)) {
												VehicleController.setDataToVehicle(pVehicle, ref oVehicleDB,false);
												db.SaveChanges();
										}													
										return VehicleController.getVehicleById((int)pVehicle.id);
								}
						}
						catch (Exception ex)
						{
								throw ex;
						}
				}

				public static bool DeleteVehicleById(int vehicleID)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{										
										var oVehicleToDelete = db.Vehicle.Where(vh => vh.veh_id == vehicleID).FirstOrDefault();
										oVehicleToDelete.veh_licensePlate = "";
										oVehicleToDelete.veh_chasisCode = "";
										oVehicleToDelete.veh_state = false;
										oVehicleToDelete.veh_deleteDate = DateTime.Now;
										db.SaveChanges();									
										return true;
								}
						}
						catch (Exception ex)
						{
								return false;
						}
				}


				

				public static List<VehicleViewModel> getListOfVehiclesByClientId(int clientId)
				{
						
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsVehicles = db.Vehicle.Where(vh => vh.veh_state == true && vh.cli_id == clientId)
																				.Select(vh => new VehicleViewModel
																				{
																						id = vh.veh_id,
																						licensePlate = vh.veh_licensePlate,
																						chasisCode = vh.veh_chasisCode,
																						vehicleState = new VehicleStateViewModel { id = vh.vehicleState.vs_id, name = vh.vehicleState.vs_name },
																						vehicleModel = new VehicleModelViewModel
																						{
																								id = vh.vm_id,
																								shortName = vh.VehicleModel.vm_shortName,
																								longName = vh.VehicleModel.vm_longName,
																								type = new VehicleTypeViewModel
																								{
																										id = vh.VehicleModel.vt_id,
																										name = vh.VehicleModel.VehicleType.vt_name
																								},
																								brand = new BrandViewModel
																								{
																										id = vh.VehicleModel.vb_id,
																										name = vh.VehicleModel.VehicleBrand.vb_name
																								}
																						},
																						year = vh.veh_year,
																						mileage = vh.veh_mileage,
																						state = vh.veh_state,
																						registrationDate = vh.veh_registrationDate,
																						updateDate = vh.veh_updateDate,
																						deleteDate = vh.veh_deleteDate
																				}).ToList();

										return lsVehicles;
								}

						}
						catch (Exception ex)
						{
								Console.Write(ex);
								return null;
						}
				}

				private static VehicleViewModel getVehicleById(int vehicleId) {
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var vehicle = db.Vehicle
										.Where(vh => vh.veh_id == vehicleId)
										.Select(vh => new VehicleViewModel
										{
												id = vh.veh_id,
												licensePlate = vh.veh_licensePlate,
												chasisCode = vh.veh_chasisCode,
												vehicleState = new VehicleStateViewModel { id = vh.vehicleState.vs_id, name = vh.vehicleState.vs_name },
												vehicleModel = new VehicleModelViewModel
												{
														id = vh.vm_id,
														shortName = vh.VehicleModel.vm_shortName,
														longName = vh.VehicleModel.vm_longName,
														type = new VehicleTypeViewModel
														{
																id = vh.VehicleModel.vt_id,
																name = vh.VehicleModel.VehicleType.vt_name
														},
														brand = new BrandViewModel
														{
																id = vh.VehicleModel.vb_id,
																name = vh.VehicleModel.VehicleBrand.vb_name
														}
												},
												year = vh.veh_year,
												mileage = vh.veh_mileage,
												state = vh.veh_state,
												registrationDate = vh.veh_registrationDate,
												updateDate = vh.veh_updateDate,
												deleteDate = vh.veh_deleteDate												
										}).FirstOrDefault();

								return vehicle;
						}
				}

				private static VehicleViewModel getLastVehicleInserted() {
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var vehicle = db.Vehicle
																		.Select(vh => new VehicleViewModel
																		{
																				id = vh.veh_id,
																				licensePlate = vh.veh_licensePlate,
																				chasisCode = vh.veh_chasisCode,
																				vehicleState = new VehicleStateViewModel { id = vh.vehicleState.vs_id, name = vh.vehicleState.vs_name },
																				vehicleModel = new VehicleModelViewModel
																				{
																						id = vh.vm_id,
																						shortName = vh.VehicleModel.vm_shortName,
																						longName = vh.VehicleModel.vm_longName,
																						type = new VehicleTypeViewModel
																						{
																								id = vh.VehicleModel.vt_id,
																								name = vh.VehicleModel.VehicleType.vt_name
																						},
																						brand = new BrandViewModel
																						{
																								id = vh.VehicleModel.vb_id,
																								name = vh.VehicleModel.VehicleBrand.vb_name
																						}
																				},
																				year = vh.veh_year,
																				mileage = vh.veh_mileage,
																				state = vh.veh_state,
																				registrationDate = vh.veh_registrationDate

																		}).OrderByDescending(vh => vh.id)
																		.FirstOrDefault();

								return vehicle;
						}
				}
		}
}
