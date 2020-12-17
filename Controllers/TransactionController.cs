using API_FleetService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
		public class TransactionController : ApiController
		{
				[HttpGet]
				public IHttpActionResult GetTodayTransactions() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var rowsTransactionsDB = db.STRPRC_GET_TRANSACTIONS_TODAY();
										List<TransactionViewModel> lsTransactions = new List<TransactionViewModel>();
										foreach (var trxDB in rowsTransactionsDB)
										{
												TransactionViewModel transaction = new TransactionViewModel();

												var clientTmp = new ClientViewModel();
												clientTmp.id = trxDB.cli_id;
												clientTmp.document = trxDB.cli_document;
												clientTmp.name = trxDB.cli_name;
												clientTmp.phone = trxDB.cli_phone;
												clientTmp.cellphone = trxDB.cli_cellphone;
												clientTmp.website = trxDB.cli_website;
												clientTmp.city = (trxDB.cty_id != null) ? new CityViewModel { id = trxDB.cty_id } : null;

												var movementTmp = new MovementViewModel();
												movementTmp.id = trxDB.m_id;
												movementTmp.name = trxDB.m_name;

												var trxState = new TransactionStateViewModel();
												trxState.id = trxDB.trxst_id;
												trxState.name = trxDB.trxst_name;

												transaction.id = trxDB.trx_id;
												transaction.consecutive = trxDB.trx_consecutive;
												transaction.client = clientTmp;
												transaction.movement = movementTmp;
												transaction.transactionState = trxState;
												transaction.value = Double.Parse(trxDB.trx_value.ToString());
												transaction.registrationDate = trxDB.trx_registrationDate;
												//TODO:: change this for correct user
												transaction.usu_id = 0;

												lsTransactions.Add(transaction);

										}
										return Ok(lsTransactions);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);	
						}
				}
				[HttpPost]
				public IHttpActionResult ProcessTransaction(TransactionViewModel transaction)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var rta = new ResponseApiViewModel();

										var consecutive = 0;

										consecutive = db.STRPRC_PROCESS_TRANSACTION(
														transaction.client.id,
														transaction.movement.id,
														transaction.value,
														(transaction.transactionState != null) ? transaction.transactionState.id : null,
														transaction.usu_id
												);

										if (consecutive > 0)
										{


												var trx_tmp = db.transactions.Where(trx => trx.cli_id == transaction.client.id && trx.m_id == transaction.movement.id)
																.OrderByDescending(trx=> trx.trx_registrationDate).FirstOrDefault();

												if (trx_tmp != null)
												{
														var trx_id = trx_tmp.trx_id;


														transaction.id = trx_id;
														rta.response = true;
														rta.message = "Se ha procesado la: " + transaction.movement.name.ToLower() + " correctamente";


														if (transaction.headerDetails != null)
														{
																transactionDetail trxDetail = new transactionDetail();
																trxDetail.trx_id = trx_id;
																trxDetail.trx_relation_id = (transaction.headerDetails.relatedTransaction != null) ? transaction.headerDetails.relatedTransaction.id : null;
																trxDetail.veh_id = (transaction.headerDetails.vehicle != null) ? transaction.headerDetails.vehicle.id : null;
																trxDetail.deal_id = (transaction.headerDetails.dealer != null) ? transaction.headerDetails.dealer.id : null;
																trxDetail.bra_id = (transaction.headerDetails.branch != null) ? transaction.headerDetails.branch.id : null;
																trxDetail.mr_id = (transaction.headerDetails.maintenanceRoutine != null) ? transaction.headerDetails.maintenanceRoutine.id : null;
																trxDetail.cntr_id = (transaction.headerDetails.contract != null) ? transaction.headerDetails.contract.id : null;

																if (transaction.movement.id == (int)EnumMovement.APROBACION_ORDEN_DE_TRABAJO)
																{
																		var trxRelated = db.transactions.Where(tr => tr.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelated.trxst_id = (int)EnumTransactionState.APROBADA;
																		var trxRelatedDetail = db.transactionDetail.Where(trx => trx.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelatedDetail.usu_approbation = transaction.usu_id;
																		trxRelatedDetail.trx_approbationDate = DateTime.Now;
																}

																if (transaction.movement.id == (int)EnumMovement.CANCELACION_ORDEN_DE_TRABAJO)
																{
																		var trxRelated = db.transactions.Where(tr => tr.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelated.trxst_id = (int)EnumTransactionState.RECHAZADA;
																		var trxRelatedDetail = db.transactionDetail.Where(trx => trx.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelatedDetail.usu_reject = transaction.usu_id;
																		trxRelatedDetail.trx_rejectDate = DateTime.Now;
																}

														}

														if (transaction.lsObservations.Count > 0)
														{
																foreach (var observation in transaction.lsObservations)
																{
																		observationsByTransaction observationDb = new observationsByTransaction();
																		observationDb.trx_id = trx_id;
																		observationDb.obstrx_description = observation.description;
																		observationDb.usu_id = transaction.usu_id;
																		observationDb.obstrx_registrationDate = DateTime.Now;
																		db.observationsByTransaction.Add(observationDb);
																		
																		db.SaveChanges();
																}
														}
												}
										}
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
