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
		public class MovementController : ApiController
		{
				[HttpGet]
				public IHttpActionResult GetTypes() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsMovementTypes = db.MovementType
																						.Select(mt => new MovementTypeViewModel
																						{
																								id = mt.mtp_id,
																								name = mt.mtp_name,
																								description = mt.mtp_description
																						}).ToList();
										return Ok(lsMovementTypes);
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
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsMovements = db.Movement.Where(mv => mv.m_state == true)
																				.Select(mv => new MovementViewModel
																				{
																						id = mv.m_id,
																						name = mv.m_name,
																						description = mv.m_description,
																						type = new MovementTypeViewModel
																						{
																								id = mv.mtp_id,
																								name = mv.MovementType.mtp_name,
																								description = mv.MovementType.mtp_description
																						},
																						state = mv.m_state,
																						registrationDate = mv.m_registrationDate
																				}).ToList();

										return Ok(lsMovements);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetById(int pMovement_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oMovement = db.Movement.Where(mv => mv.m_state == true && mv.m_id == pMovement_id)
																				.Select(mv => new MovementViewModel
																				{
																						id = mv.m_id,
																						name = mv.m_name,
																						description = mv.m_description,
																						type = new MovementTypeViewModel
																						{
																								id = mv.mtp_id,
																								name = mv.MovementType.mtp_name,
																								description = mv.MovementType.mtp_description
																						},
																						state = mv.m_state,
																						registrationDate = mv.m_registrationDate
																				}).FirstOrDefault();

										return Ok(oMovement);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Insert(MovementViewModel pMovement)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var transactionType = "INSERT";
										var rta = new ResponseApiViewModel();
										var oMovementDB = new Movement();
										this.setDataMovement(pMovement, transactionType, ref oMovementDB);
										db.Movement.Add(oMovementDB);
										db.SaveChanges();

										rta.response = true;
										rta.message = "Se ha insertado el movimiento: " + pMovement.name;

										return Ok(rta);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Update(MovementViewModel pMovement)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var transactionType = "UPDATE";
										var rta = new ResponseApiViewModel();
										var oMovementDB = db.Movement.Where(mv => mv.m_id == pMovement.id).FirstOrDefault();
										this.setDataMovement(pMovement, transactionType, ref oMovementDB);
										db.SaveChanges();

										rta.response = true;
										rta.message = "Se ha actualizado el movimiento: " + pMovement.name;

										return Ok(rta);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Delete(MovementViewModel pMovement)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										
										var rta = new ResponseApiViewModel();
										var oMovementDB = db.Movement.Where(mv => mv.m_id == pMovement.id).FirstOrDefault();

										oMovementDB.m_state = false;
										oMovementDB.m_deleteDate = DateTime.Now;

										db.SaveChanges();

										rta.response = true;
										rta.message = "Se ha eliminado el movimiento: " + pMovement.name;

										return Ok(rta);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				private void setDataMovement(MovementViewModel pMovement,string transactionType, ref Movement movementDB) {

						if (transactionType == "INSERT")
						{
								movementDB.m_registrationDate = DateTime.Now;
								movementDB.m_state = true;
						}
						else if (transactionType == "UPDATE") {
								movementDB.m_id = (int)pMovement.id;
								movementDB.m_updateDate = DateTime.Now;
						}

						movementDB.m_name = pMovement.name;
						movementDB.m_description = pMovement.description;
						movementDB.mtp_id = pMovement.type.id;						
				}
		}
}
