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
		public class ClientController : ApiController, ICRUDElement
		{
				
			
				[HttpGet]
				public IHttpActionResult Get()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsClient = db.Client.Where(cl => cl.cli_state == true)
												.Select(cl => new ClientViewModel
												{
														id = cl.cli_id,
														document = cl.cli_document,
														name = cl.cli_name,
														phone = cl.cli_phone,
														cellphone = cl.cli_cellphone,
														address = cl.cli_adress,
														website = cl.cli_website,
														city = (cl.cty_id != null )? new CityViewModel { id = cl.cty_id, name = cl.Cities.cty_name, departmentId = cl.Cities.dpt_id } : null,
														state = cl.cli_state,
														registrationDate = cl.cli_registrationDate,
														updateDate = cl.cli_updateDate,
														deleteDate = cl.cli_deleteDate
												}).ToList()
												.Take(100)
												.OrderByDescending(cl => cl.registrationDate);

										return Ok(lsClient);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetById(int pId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oClientDB = db.Client.Where(cl => cl.cli_id == pId)
												.Select(cl => new ClientViewModel
												{
														id = cl.cli_id,
														document = cl.cli_document,
														name = cl.cli_name,
														phone = cl.cli_phone,
														cellphone = cl.cli_cellphone,
														address = cl.cli_adress,
														website = cl.cli_website,
														city = (cl.cty_id != null) ? new CityViewModel { id = cl.cty_id, name = cl.Cities.cty_name, departmentId = cl.Cities.dpt_id } : null,
														state = cl.cli_state,
														registrationDate = cl.cli_registrationDate,
														updateDate = cl.cli_updateDate,
														deleteDate = cl.cli_deleteDate
														
												}).FirstOrDefault();

										oClientDB.contacts = ContactController.getListOfContactsByClientId((int)oClientDB.id);
										oClientDB.branchs = BranchController.getListOfBranchsByClientId((int)oClientDB.id);
										oClientDB.vehicles = VehicleController.getListOfVehiclesByClientId((int)oClientDB.id);
										oClientDB.contractualInformation = ContractualInformationController.getContractualInformationByClient((int)oClientDB.id);
										oClientDB.financialInformation = FinancialInformationController.GetFinancialInformationByClientId((int)oClientDB.id);

										return Ok(oClientDB);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetByDocument(string pDocument)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oClientDB = db.Client.Where(cl => cl.cli_document == pDocument && cl.cli_state == true)
												.Select(cl => new ClientViewModel
												{
														id = cl.cli_id,
														document = cl.cli_document,
														name = cl.cli_name,
														phone = cl.cli_phone,
														cellphone = cl.cli_cellphone,
														address = cl.cli_adress,
														website = cl.cli_website,
														city = (cl.cty_id != null) ? new CityViewModel { id = cl.cty_id, name = cl.Cities.cty_name, departmentId = cl.Cities.dpt_id } : null,
														state = cl.cli_state,
														registrationDate = cl.cli_registrationDate,
														updateDate = cl.cli_updateDate,
														deleteDate = cl.cli_deleteDate
												}).FirstOrDefault();
										return Ok(oClientDB);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetByDescription(string sDescription) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsClientDB = db.Client.Where(cl =>cl.cli_state == true && ( cl.cli_document.Contains(sDescription) || cl.cli_name.ToUpper().Contains(sDescription.ToUpper())))
												.Select(cl => new ClientViewModel
												{
														id = cl.cli_id,
														document = cl.cli_document,
														name = cl.cli_name,
														phone = cl.cli_phone,
														cellphone = cl.cli_cellphone,
														address = cl.cli_adress,
														website = cl.cli_website,
														city = (cl.cty_id != null) ? new CityViewModel { id = cl.cty_id, name = cl.Cities.cty_name, departmentId = cl.Cities.dpt_id } : null,
														state = cl.cli_state,
														registrationDate = cl.cli_registrationDate
												}).ToList().Take(10);


										return Ok(lsClientDB);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public  IHttpActionResult Insert(ClientViewModel pClient)
				{
						//TODO: Agregar usuario que hizo la acción
						ResponseApiViewModel rta = new ResponseApiViewModel();
						try
						{							
								Client ClientToInsert = ClientController.PrepareCLientToInsertDB(pClient);
								bool ClientWasInserted = ClientController.InsertIntoDB(ClientToInsert);
								Client lastClient = ClientController.getLastClientInserted();

								if (ClientWasInserted)
								{
										ClientController.insertContactsByClient((int)lastClient.cli_id,pClient.contacts);
										ClientController.insertBranchsByClient((int)lastClient.cli_id, pClient.branchs);
										ClientController.insertVehiclesByClient((int)lastClient.cli_id, pClient.vehicles);
										ClientController.insertContractualInformationByClient((int)lastClient.cli_id, pClient.contractualInformation);
										rta.response = true;
										rta.message = "El cliente " + pClient.document + " fue insertado correctamente en la base de datos.";
										return Ok(rta);
								}
								else {
										rta.response = false;
										rta.message = "Ha ocurrido un error intentado insertar el cliente:  " + pClient.document;
										return BadRequest(rta.message);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}



				[HttpPost]
				public IHttpActionResult Update(ClientViewModel client) {
						//TODO: Agregar usuario que hizo la acción
						try {
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										
										var oClientDB = db.Client
												.Where(cl => cl.cli_id == client.id || cl.cli_document == client.document)
												.FirstOrDefault();

										if (oClientDB != null)
										{
												ClientController.SetDataToUpdateClient(client, ref oClientDB);
												ClientController.updateContactsByClient((int)client.id, client.contacts);
												ClientController.updateBranchesByClient((int)client.id, client.branchs);
												ClientController.updateVehiclesByClient((int)client.id, client.vehicles);
												ClientController.updateContractualInformationByClient((int)client.id, client.contractualInformation);
												db.SaveChanges();
												rta.response = true;
												rta.message = "Se ha actualizado el cliente: " + client.document + " - " + client.name;
												return Ok(rta);
										}
										else {
												rta.response = false;
												rta.message = "No se encontró el cliente: " + client.document + " - " + client.name + " en la base de datos, por favor rectifique los datos";
												return BadRequest(rta.message);
										}
										
								}
										
						}
						catch (Exception ex){
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Delete(ClientViewModel pClient)
				{
						//TODO: Agregar usuario que hizo la acción
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var oClientDB = db.Client.Where(cl => cl.cli_id == pClient.id).FirstOrDefault();
										if (oClientDB != null)
										{
												oClientDB.cli_document = "";
												oClientDB.cli_state = false;
												oClientDB.cli_deleteDate = DateTime.Now;
												db.SaveChanges();
												rta.response = true;
												rta.message = "Se ha eliminado el cliente: " + oClientDB.cli_name + " de la base de datos";
												return Ok(rta);
										}
										else
										{
												rta.response = false;
												rta.message = "No se encontró el cliente: " + pClient.document + " " + pClient.name +" por lo tanto no se puede eliminar";
												return BadRequest(rta.message);
										}

								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				public static Client PrepareCLientToInsertDB(ClientViewModel pClient)
				{


						if (ClientController.ClientExistInDB(pClient.document.Trim()))
						{
								throw new Exception("El cliente que intenta ingresar, ya se encuentra almacenado en la base de datos");
						}

						Client clientToInsert = ClientController.SetDataToClient(pClient);

						return clientToInsert;


				}

				private static void ClientHasErros(ClientViewModel pClient)
				{
						if (pClient.document.Trim() == "")
						{
								throw new Exception("El documento del cliente no es válido");
						}


						if (pClient.name.Trim() == "")
						{
								throw new Exception("El nombre del cliente es obligatorio, no se puede insertar a la base de datos sin esta información");
						}
				}

				private static bool ClientExistInDB(string pClient_document)
				{
						try
						{
								bool rta = false;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var clientDB = db.Client.Where(cl => cl.cli_document == pClient_document).FirstOrDefault();
										if (clientDB != null)
										{
												rta = true;
										}
										return rta;
								}
						}
						catch (Exception ex)
						{
								return false;
						}
				}


				private static Client SetDataToClient(ClientViewModel pClient)
				{

						ClientController.ClientHasErros(pClient);

						Client clientToInsert = new Client();
						clientToInsert.cli_document = pClient.document;
						clientToInsert.cli_name = pClient.name.ToUpper();
						clientToInsert.cli_phone = pClient.phone;
						clientToInsert.cli_cellphone = pClient.cellphone;
						clientToInsert.cli_adress = pClient.address.ToUpper();
						clientToInsert.cli_website = (pClient.website != null) ? pClient.website.ToUpper() : null;
						clientToInsert.cty_id = (pClient.city != null) ? pClient.city.id : null;
						clientToInsert.cli_state = true;
						clientToInsert.cli_registrationDate = DateTime.Now;

						return clientToInsert;
				}

				private static void SetDataToUpdateClient(ClientViewModel pClient, ref Client clientDB)
				{
						ClientController.ClientHasErros(pClient);						
						clientDB.cli_document = pClient.document;
						clientDB.cli_name = pClient.name.ToUpper();
						clientDB.cli_phone = pClient.phone;
						clientDB.cli_cellphone = pClient.cellphone;
						clientDB.cli_adress = pClient.address.ToUpper();
						clientDB.cli_website = (pClient.website != null) ? pClient.website.ToUpper() : null;
						clientDB.cty_id = (pClient.city != null) ? pClient.city.id : null;
						clientDB.cli_state = true;
						clientDB.cli_updateDate = DateTime.Now;						
				}



				public static bool InsertIntoDB(Client client)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										db.Client.Add(client);
										db.SaveChanges();
										return true;
								}
						}
						catch (Exception ex)
						{
								return false;
						}
				}

				public static Client getLastClientInserted() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										return db.Client.OrderByDescending(cl => cl.cli_id).FirstOrDefault();
								}
						}
						catch (Exception ex)
						{
								return null;
						}
				}

				public static bool insertContactsByClient(int clientId, List<ContactViewModel> lsContact)
				{
						try
						{
								foreach (var contact in lsContact)
								{
										contact.Client_id = clientId;
										ContactController.InsertContact(contact);
								}
								return true;
						}
						catch (Exception)
						{

								return false;
						}

				}

				public static bool updateContactsByClient(int clientId, List<ContactViewModel> lsContact) {
						try
						{
								var lsContactsToValidate = ContactController.getListOfContactsByClientId(clientId);
								var lsContactsToInsert = lsContact.FindAll(cnt => cnt.id <= 0);
								var lsContactsToUpdate = lsContact.FindAll(cnt => cnt.id > 0);
								var lsContactsToDelete = lsContactsToValidate.Where(cnt => !lsContactsToUpdate.Any(cntD => cntD.id == cnt.id));

								ClientController.insertContactsByClient(clientId, lsContactsToInsert);

								foreach (var contact in lsContactsToUpdate)
								{
										contact.Client_id = clientId;
										ContactController.updateContact(contact);
								}

								foreach (var contactToDelete in lsContactsToDelete)
								{										
										ContactController.DeleteById(contactToDelete.id);
								}

								return true;
						}
						catch (Exception)
						{

								return false;
						}
				}

				public static bool insertBranchsByClient(int clientId, List<BranchViewModel> lsBranch)
				{
						try
						{
								foreach (var branch in lsBranch)
								{
										branch.Client_id = clientId;
										BranchController.InsertBranch(branch);
								}
								return true;
						}
						catch (Exception)
						{

								return false;
						}

				}

				public static bool insertVehiclesByClient(int clientId, List<VehicleViewModel> lsVehicles)
				{
						try
						{
								foreach (var vehicle in lsVehicles)
								{
										vehicle.Client_id = clientId;
										VehicleController.InsertVehicle(vehicle);
								}
								return true;
						}
						catch (Exception)
						{

								return false;
						}

				}

				public static bool insertContractualInformationByClient(int clientId, ContractualInformationViewModel info)
				{
						try
						{
								info.clientId = clientId;
								ContractualInformationController.insertContractualInformation(info);
								return true;
						}
						catch (Exception)
						{
								return false;
						}

				}

				public static bool updateContractualInformationByClient(int clientId, ContractualInformationViewModel info)
				{
						try
						{
								info.clientId = clientId;
								ContractualInformationController.updateContractualInformation(info);
								return true;
						}
						catch (Exception ex)
						{
								throw ex;
						}

				}

				public static bool updateBranchesByClient(int clientId, List<BranchViewModel> lsBranches)
				{
						try
						{
								var lsBranchesToValidate = BranchController.getListOfBranchsByClientId(clientId);
								var lsBranchesToInsert = lsBranches.FindAll(brn => brn.id <= 0);
								var lsBranchesToUpdate = lsBranches.FindAll(brn => brn.id > 0);
								var lsBranchesToDelete = lsBranchesToValidate.Where(brn => !lsBranchesToUpdate.Any(brnD => brnD.id == brn.id));

								ClientController.insertBranchsByClient(clientId, lsBranchesToInsert);

								foreach (var branch in lsBranchesToUpdate)
								{
										branch.Client_id = clientId;
										BranchController.UpdateBranch(branch);										
								}

								foreach (var branchToDelete in lsBranchesToDelete)
								{
										BranchController.DeleteBranchById((int)branchToDelete.id);
								}

								return true;
						}
						catch (Exception)
						{

								return false;
						}
				}

				public static bool updateVehiclesByClient(int clientId, List<VehicleViewModel> lsVehicles)
				{
						try
						{
								var lsVehiclesToValidate = VehicleController.getListOfVehiclesByClientId(clientId);
								var lsVehiclesToInsert = lsVehicles.FindAll(brn => brn.id <= 0);
								var lsVehiclesToUpdate = lsVehicles.FindAll(brn => brn.id > 0);
								var lsVehiclesToDelete = lsVehiclesToValidate.Where(brn => !lsVehiclesToUpdate.Any(brnD => brnD.id == brn.id));

								ClientController.insertVehiclesByClient(clientId, lsVehiclesToInsert);

								foreach (var vehicle in lsVehiclesToUpdate)
								{
										vehicle.Client_id = clientId;
										VehicleController.UpdateVehicle(vehicle);
								}

								foreach (var vehicleToDelete in lsVehiclesToDelete)
								{
										VehicleController.DeleteVehicleById((int)vehicleToDelete.id);
								}

								return true;
						}
						catch (Exception)
						{

								return false;
						}
				}


		}
}
