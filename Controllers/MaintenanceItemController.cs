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
    public class MaintenanceItemController : ApiController
    {
				[HttpGet]
				public IHttpActionResult GetPresentationUnits() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsPresentationUnits = db.PresentationUnit.Where(pu => pu.pu_state == true)
												.Select(pu => new PresentationUnitViewModel	{
														id = pu.pu_id,
														shortName = pu.pu_shortName,
														longName  = pu.pu_longName
												}).ToList();
										return Ok(lsPresentationUnits);
								}
										
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);	
						}
				}

				[HttpGet]
				public IHttpActionResult GetCategories() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsCategories = db.MaintenanceItemCategory.Where(ct => ct.mict_state == true)
																						.Select(ct => new CategoryViewModel
																						{
																								id = ct.mict_id,
																								name = ct.mict_name,
																								sate = ct.mict_state,
																								registrationDate = ct.mict_registrationDate
																						}).ToList();
										return Ok(lsCategories);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetTypeOfMaintenanceItem()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsType = db.TypeOfMaintenanceItem.Where(tp => tp.tmi_state == true)
												.Select(tp => new TypeOfMaintenanceItemViewModel
												{
													id = tp.tmi_id,
													name = tp.tmi_name,
												}).ToList();
										return Ok(lsType);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
        public IHttpActionResult Get() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
									
										var lsMaintenanceItems = db.MaintenanceItem.Where(mi => mi.mi_state == true)
																								.Select( mi => new MaintenanceItemViewModel { 
																										id = mi.mi_id,
																										code = mi.mi_code,
																										name = mi.mi_name,
																										description = mi.mi_description,
																										type = new TypeOfMaintenanceItemViewModel { 
																												id = mi.tmi_id,
																												name = mi.TypeOfMaintenanceItem.tmi_name},
																										presentationUnit = new PresentationUnitViewModel { 
																												id = mi.pu_id,
																												shortName = mi.PresentationUnit.pu_shortName,
																												longName = mi.PresentationUnit.pu_longName},																							
																										category = (mi.mict_id != null)? new CategoryViewModel { 
																												id = mi.mict_id,
																												name = mi.MaintenanceItemCategory.mict_name																												
																										} :null,
																										referencePrice = mi.mi_referencePrice,
																										state = mi.mi_state,
																										registrationDate = mi.mi_registrationDate
																								}).ToList()
																								.Take(100);

									
										return Ok(lsMaintenanceItems);
								}
										
						}		
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
        }

				[HttpGet]
				public IHttpActionResult GetById(int itemId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var maintenanceItem = db.MaintenanceItem.Where(mi => mi.mi_state == true && mi.mi_id == itemId)
																								.Select(mi => new MaintenanceItemViewModel
																								{
																										id = mi.mi_id,
																										code = mi.mi_code,
																										name = mi.mi_name,
																										description = mi.mi_description,
																										type = new TypeOfMaintenanceItemViewModel
																										{
																												id = mi.tmi_id,
																												name = mi.TypeOfMaintenanceItem.tmi_name
																										},
																										presentationUnit = new PresentationUnitViewModel
																										{
																												id = mi.pu_id,
																												shortName = mi.PresentationUnit.pu_shortName,
																												longName = mi.PresentationUnit.pu_longName
																										},
																										category = (mi.mict_id != null) ? new CategoryViewModel
																										{
																												id = mi.mict_id,
																												name = mi.MaintenanceItemCategory.mict_name
																										} : null,
																										referencePrice = mi.mi_referencePrice,
																										state = mi.mi_state,
																										registrationDate = mi.mi_registrationDate
																								}).FirstOrDefault();

										maintenanceItem.lsVehicleType = db.MaintenanceItemsByVehicleTypes.Where(mi => mi.mi_id == itemId)
																												.Select(mi => new VehicleTypeViewModel {
																														id = mi.vt_id,
																														name = mi.VehicleType.vt_name,
																														state = mi.VehicleType.vt_state,
																														registrationDate = mi.mivt_registrationDate
																												}).ToList();

										maintenanceItem.lsVehicleModel = db.MaintenanceItemsByVehicleModels.Where(mi => mi.mi_id == itemId)
																												.Select(mi => new VehicleModelViewModel
																												{
																														id = mi.vm_id,
																														shortName = mi.VehicleModel.vm_shortName,
																														longName = mi.VehicleModel.vm_longName,
																														state = mi.VehicleModel.vm_state,
																														registrationDate = mi.VehicleModel.vm_registrationDate,
																														brand = new BrandViewModel { id = mi.VehicleModel.vb_id, name = mi.VehicleModel.VehicleBrand.vb_name },
																														type = new VehicleTypeViewModel { id = mi.VehicleModel.vt_id, name = mi.VehicleModel.VehicleType.vt_name }
																												}).ToList();


										return Ok(maintenanceItem);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetByType(int typeId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var lsMaintenanceItems = db.MaintenanceItem.Where(mi => mi.mi_state == true && mi.tmi_id == typeId)
																								.Select(mi => new MaintenanceItemViewModel
																								{
																										id = mi.mi_id,
																										code = mi.mi_code,
																										name = mi.mi_name,
																										description = mi.mi_description,
																										type = new TypeOfMaintenanceItemViewModel
																										{
																												id = mi.tmi_id,
																												name = mi.TypeOfMaintenanceItem.tmi_name
																										},
																										presentationUnit = new PresentationUnitViewModel
																										{
																												id = mi.pu_id,
																												shortName = mi.PresentationUnit.pu_shortName,
																												longName = mi.PresentationUnit.pu_longName
																										},
																										category = (mi.mict_id != null) ? new CategoryViewModel
																										{
																												id = mi.mict_id,
																												name = mi.MaintenanceItemCategory.mict_name
																										} : null,
																										referencePrice = mi.mi_referencePrice,
																										state = mi.mi_state,
																										registrationDate = mi.mi_registrationDate
																								}).ToList();																						

										return Ok(lsMaintenanceItems);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}



				[HttpGet]
				public IHttpActionResult GetItemsByVehicleModel(int pVehicleModel_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var oVehicleModel = db.VehicleModel.Where(vm => vm.vm_id == pVehicleModel_id)
																						.Select(vm => new VehicleModelViewModel
																						{
																								id = vm.vm_id,
																								shortName = vm.vm_shortName,
																								longName = vm.vm_longName,
																								state = vm.vm_state,
																								registrationDate = vm.vm_registrationDate,
																								brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
																								type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
																						}).FirstOrDefault();

										var lsItemsConfiguratedByVehicleModel = db.MaintenanceItemsByVehicleModels
																																.Where(vm => vm.vm_id == pVehicleModel_id && vm.MaintenanceItem.mi_state == true)
																																.ToList();

										var lsItemsConfiguratedByVehicleType = db.MaintenanceItemsByVehicleTypes
																																.Where(vt => vt.vt_id == oVehicleModel.type.id && vt.MaintenanceItem.mi_state == true)
																																.ToList();



										var lsItemConfigurated = new List<int>();

										foreach (var item in lsItemsConfiguratedByVehicleType) {						
												lsItemConfigurated.Add((int) item.mi_id);
										}

										foreach (var item in lsItemsConfiguratedByVehicleModel)
										{
												lsItemConfigurated.Add((int)item.mi_id);
										}

										var lsMaintenanceItems = db.MaintenanceItem.Where(mi => mi.mi_state == true && lsItemConfigurated.Any(item => item == mi.mi_id))												
																								.Select(mi => new MaintenanceItemViewModel
																								{
																										id = mi.mi_id,
																										code = mi.mi_code,
																										name = mi.mi_name,
																										description = mi.mi_description,
																										type = new TypeOfMaintenanceItemViewModel
																										{
																												id = mi.tmi_id,
																												name = mi.TypeOfMaintenanceItem.tmi_name
																										},
																										presentationUnit = new PresentationUnitViewModel
																										{
																												id = mi.pu_id,
																												shortName = mi.PresentationUnit.pu_shortName,
																												longName = mi.PresentationUnit.pu_longName
																										},																										
																										category = (mi.mict_id != null) ? new CategoryViewModel
																										{
																												id = mi.mict_id,
																												name = mi.MaintenanceItemCategory.mict_name
																										} : null,
																										referencePrice = mi.mi_referencePrice,
																										state = mi.mi_state,
																										registrationDate = mi.mi_registrationDate
																								}).ToList();

										
										return Ok(lsMaintenanceItems);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Insert(MaintenanceItemViewModel pItem) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										ResponseApiViewModel rta = new ResponseApiViewModel();
										var oMaintenanceItem = MaintenanceItemViewModel.setDataToItem(pItem);
										var ItemWasSaved = MaintenanceItemViewModel.InsertIntoDB(oMaintenanceItem);
										if (ItemWasSaved)
										{
												var itemId = MaintenanceItemViewModel.GetMaintenanceItemId(pItem.code);

												if (itemId != 0) {
														var lsVehicleType = pItem.lsVehicleType;
														if (lsVehicleType.Count > 0) {
																MaintenanceItemViewModel.InsertMaintenanceItemByVehicleType(itemId, lsVehicleType);
														}

														var lsVehicleModel = pItem.lsVehicleModel;
														if (lsVehicleModel.Count > 0) {
																MaintenanceItemViewModel.InsertMaintenanceItemByVehicleModel(itemId, lsVehicleModel);
														}							
												}


												rta.response = true;
												rta.message = "El artículo de mantenimiento " + pItem.code + " fue almacenado correctamente en la base de datos";
												return Ok(rta);
										}
										else {
												return BadRequest("Sucedio algo en la inserción del artículo de mantenimiento");
										}										
								}								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Update(MaintenanceItemViewModel pItem)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();
										var oItemDB = db.MaintenanceItem.Where(it => it.mi_id == pItem.id).FirstOrDefault();
										oItemDB.mi_code = pItem.code.ToUpper();
										oItemDB.mi_name = pItem.name.ToUpper();
										oItemDB.mi_description = pItem.description.ToUpper();
										oItemDB.mi_referencePrice = pItem.referencePrice;

										if (pItem.type == null)
										{
												throw new Exception("No se puede actualizar el artículo debido a que no tiene un tipo definido.");
										}
										else {
												oItemDB.tmi_id = (int)pItem.type.id;
										}

										if (pItem.presentationUnit == null)
										{
												throw new Exception("No se puede actualizar el artículo debido a que no tiene una presentación definida.");
										}
										else
										{
												oItemDB.pu_id = (int)pItem.presentationUnit.id;
										}
										
										if (pItem.category != null)
										{
												oItemDB.mict_id = (int)pItem.category.id;
										}

										oItemDB.mi_updateDate = DateTime.Now;							
										db.SaveChanges();


										MaintenanceItemViewModel.DeleteMaintenanceItemOfVehicleTypesAndModels((int)pItem.id);

										var lsVehicleType = pItem.lsVehicleType;
										if (lsVehicleType.Count > 0)
										{
												MaintenanceItemViewModel.InsertMaintenanceItemByVehicleType((int)pItem.id, lsVehicleType);
										}

										var lsVehicleModel = pItem.lsVehicleModel;
										if (lsVehicleModel.Count > 0)
										{
												MaintenanceItemViewModel.InsertMaintenanceItemByVehicleModel((int)pItem.id, lsVehicleModel);
										}

										rta.response = true;
										rta.message = "Se ha actualizado el artículo de mantenimiento: " + pItem.name + " de la base de datos";
										return Ok(rta);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Delete(MaintenanceItemViewModel pItem)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										ResponseApiViewModel rta = new ResponseApiViewModel();
										var oItemDB = db.MaintenanceItem.Where(it => it.mi_id == pItem.id).FirstOrDefault();
										oItemDB.mi_state = false;
										oItemDB.mi_deleteDate = DateTime.Now;
										db.SaveChanges();

										MaintenanceItemViewModel.DeleteMaintenanceItemOfVehicleTypesAndModels((int)pItem.id);

										rta.response = true;
										rta.message = "Se ha eliminado el artículo de mantenimiento: " + pItem.name + " de la base de datos";
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
