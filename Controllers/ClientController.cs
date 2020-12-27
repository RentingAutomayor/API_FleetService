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
														registrationDate = cl.cli_registrationDate
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
														registrationDate = cl.cli_registrationDate
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
														registrationDate = cl.cli_registrationDate
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
								
								Client ClientToInsert = ClientViewModel.PrepareCLientToInsertDB(pClient);
								bool ClientWasInserted = ClientViewModel.InsertIntoDB(ClientToInsert);

								if (ClientWasInserted)
								{
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
				public IHttpActionResult Update(ClientViewModel pClient) {
						//TODO: Agregar usuario que hizo la acción
						try {
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										
										var oClientDB = db.Client.Where(cl => cl.cli_id == pClient.id || cl.cli_document == pClient.document)
																						.FirstOrDefault();
										if (oClientDB != null)
										{
												if (pClient.document.Trim() == "")
												{
														throw new Exception("El documento del cliente no es válido");
												}

												long numberOfDocument;
												bool documentIsValid = long.TryParse(pClient.document, out numberOfDocument);

												if (!documentIsValid)
												{
														throw new Exception("El documento del cliente no es válido, intente ingresarlo sin dígito de verificación, sin puntos ni comas.");
												}

												if (pClient.name.Trim() == "")
												{
														throw new Exception("El nombre del cliente es obligatorio, no se puede insertar a la base de datos sin esta información");
												}

												oClientDB.cli_document = pClient.document;
												oClientDB.cli_name = pClient.name.ToUpper();
												oClientDB.cli_phone = pClient.phone;
												oClientDB.cli_cellphone = pClient.cellphone;
												oClientDB.cli_adress = pClient.address.ToUpper();
												oClientDB.cli_website = (pClient.website != null) ? pClient.website.ToUpper() : null;
												oClientDB.cty_id = (pClient.city != null) ? pClient.city.id : null;
												oClientDB.cli_updateDate = DateTime.Now;
												

												db.SaveChanges();
												rta.response = true;
												rta.message = "Se ha actualizado el cliente: " + pClient.document + " - " + pClient.name;
												return Ok(rta);
										}
										else {
												rta.response = false;
												rta.message = "No se encontró el cliente: " + pClient.document + " - " + pClient.name + " en la base de datos, por favor rectifique los datos";
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


		}
}
