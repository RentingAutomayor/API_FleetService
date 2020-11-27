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
    public class DealerController : ApiController
    {
				[HttpGet]
				public IHttpActionResult Get()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsDealer = db.Dealer.Where(dlr => dlr.deal_state == true)
												.Select(dlr => new DealerViewModel
												{
														id = dlr.deal_id,
														document = dlr.deal_document,
														name = dlr.deal_name,																							
														state = dlr.deal_state,
														registrationDate = dlr.deal_registrationDate
												}).ToList()
												.Take(100)
												.OrderByDescending(dlr => dlr.registrationDate);

										return Ok(lsDealer);
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
										var odealentDB = db.Dealer.Where(dlr => dlr.deal_id == pId)
												.Select(dlr => new DealerViewModel
												{
														id = dlr.deal_id,
														document = dlr.deal_document,
														name = dlr.deal_name,													
														state = dlr.deal_state,
														registrationDate = dlr.deal_registrationDate
												}).FirstOrDefault();

										return Ok(odealentDB);
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
										var oDealerDB = db.Dealer.Where(dlr => dlr.deal_document == pDocument && dlr.deal_state == true)
												.Select(dlr => new DealerViewModel
												{
														id = dlr.deal_id,
														document = dlr.deal_document,
														name = dlr.deal_name,
													
														state = dlr.deal_state,
														registrationDate = dlr.deal_registrationDate
												}).FirstOrDefault();

										return Ok(oDealerDB);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult GetByDescription(string sDescription)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsDealerDB = 	db.Dealer.Where(dlr => dlr.deal_state == true && 
										(dlr.deal_document.Contains(sDescription) || dlr.deal_name.ToUpper().Contains(sDescription.ToUpper())))			.Select(dlr => new DealerViewModel
												{
														id = dlr.deal_id,
														document = dlr.deal_document,
														name = dlr.deal_name,
														state = dlr.deal_state,
														registrationDate = dlr.deal_registrationDate
												}).ToList();

										return Ok(lsDealerDB);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Insert(DealerViewModel pDealer)
				{
						//TODO: Agregar usuario que hizo la acción
						ResponseApiViewModel rta = new ResponseApiViewModel();
						try
						{
								Dealer dealerToInsert = DealerViewModel.PrepareDealerToInsertDB(pDealer);
								bool dealerWasInserted = DealerViewModel.InsertIntoDB(dealerToInsert);

								if (dealerWasInserted)
								{
										rta.response = true;
										rta.message = "El concesionario " + pDealer.document + " fue insertado correctamente en la base de datos.";
										return Ok(rta);
								}
								else
								{
										rta.response = false;
										rta.message = "Ha ocurrido un error intentado insertar el dealente:  " + pDealer.document;
										return BadRequest(rta.message);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Update(DealerViewModel pDealer)
				{
						//TODO: Agregar usuario que hizo la acción
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var oDealerDB = db.Dealer.Where(dlr => dlr.deal_id == pDealer.id || dlr.deal_document == pDealer.document)
																						.FirstOrDefault();
										if (oDealerDB != null)
										{
												if (pDealer.document.Trim() == "")
												{
														throw new Exception("El documento del concesionario no es válido");
												}

												long numberOfDocument;
												bool documentIsValid = long.TryParse(pDealer.document, out numberOfDocument);

												if (!documentIsValid)
												{
														throw new Exception("El documento del concesionario no es válido, intente ingresarlo sin dígito de verificación, sin puntos ni comas.");
												}

												if (pDealer.name.Trim() == "")
												{
														throw new Exception("El nombre del concesionario es obligatorio, no se puede insertar a la base de datos sin esta información");
												}

												oDealerDB.deal_document = pDealer.document;
												oDealerDB.deal_name = pDealer.name.ToUpper();
												oDealerDB.deal_updateDate = DateTime.Now;
												db.SaveChanges();
												rta.response = true;
												rta.message = "Se ha actualizado el concesionario: " + pDealer.document + " - " + pDealer.name;
												return Ok(rta);
										}
										else
										{
												rta.response = false;
												rta.message = "No se encontró el concesionario: " + pDealer.document + " - " + pDealer.name + " en la base de datos, por favor rectifique los datos";
												return BadRequest(rta.message);
										}

								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Delete(DealerViewModel pDealer)
				{
						//TODO: Agregar usuario que hizo la acción
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{

										var oDealerDB = db.Dealer.Where(dlr => dlr.deal_id == pDealer.id).FirstOrDefault();
										if (oDealerDB != null)
										{
												oDealerDB.deal_document = "";
												oDealerDB.deal_state = false;
												oDealerDB.deal_deleteDate = DateTime.Now;
												db.SaveChanges();
												rta.response = true;
												rta.message = "Se ha eliminado el concesionario: " + oDealerDB.deal_name + " de la base de datos";
												return Ok(rta);
										}
										else
										{
												rta.response = false;
												rta.message = "No se encontró el concesionario: " + pDealer.document + " " + pDealer.name + " por lo tanto no se puede eliminar";
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
