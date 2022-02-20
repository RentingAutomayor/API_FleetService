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
				public IHttpActionResult GetTodayTransactions()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var rowsTransactionsDB = db.STRPRC_GET_TRANSACTIONS_TODAY1();
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
												transaction.code = trxDB.trx_code;
												transaction.consecutive = trxDB.trx_consecutive;
												transaction.client = clientTmp;
												transaction.movement = movementTmp;
												transaction.transactionState = trxState;
												transaction.value = Double.Parse(trxDB.trx_value.ToString());
												transaction.registrationDate = trxDB.trx_registrationDate;
												
												transaction.user = new UserAccessViewModel();
												transaction.user.id_user = trxDB.usu_id;
												transaction.user.name = trxDB.usu_name;
												transaction.user.lastName = trxDB.usu_lastName;

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

									  decimal valueWithoutDscount = Convert.ToDecimal(string.Format("{0:F2}", transaction.valueWithoutDiscount));
										decimal discountValue = Convert.ToDecimal(string.Format("{0:F2}", transaction.discountValue));
										decimal taxesValue = Convert.ToDecimal(string.Format("{0:F2}", transaction.taxesValue));
										decimal valueWithDiscountWithoutTaxes = Convert.ToDecimal(string.Format("{0:F2}", transaction.valueWithDiscountWithoutTaxes));
										decimal valueTRX = Convert.ToDecimal(string.Format("{0:F2}", transaction.value));

										consecutive = db.STRPRC_PROCESS_TRANSACTION_V2(
														transaction.client.id,
														transaction.movement.id,
														valueWithoutDscount,
														discountValue,
														valueWithDiscountWithoutTaxes,
														taxesValue,
														valueTRX,
														(transaction.transactionState != null) ? transaction.transactionState.id : null,
														transaction.usu_id
												);

										if (consecutive > 0)
										{


												var trx_tmp = db.transactions.Where(trx => trx.cli_id == transaction.client.id && trx.m_id == transaction.movement.id)
																.OrderByDescending(trx => trx.trx_registrationDate).FirstOrDefault();

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
																if (transaction.headerDetails.vehicle != null)
																{
																		VehicleController.UpdateVehicle(transaction.headerDetails.vehicle);
																}
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


																if (transaction.movement.id == (int)EnumMovement.FINALIZACION_ORDEN_DE_TRABAJO)
																{
																		var trxRelated = db.transactions.Where(tr => tr.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelated.trxst_id = (int)EnumTransactionState.FINALIZADA;
																		var trxRelatedDetail = db.transactionDetail.Where(trx => trx.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelatedDetail.usu_ending = transaction.usu_id;
																		trxRelatedDetail.trx_endingDate = DateTime.Now;
																}


																if (transaction.movement.id == (int)EnumMovement.ANULACION_ORDEN_DE_TRABAJO)
																{
																		var trxRelated = db.transactions.Where(tr => tr.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelated.trxst_id = (int)EnumTransactionState.ANULADA;
																		var trxRelatedDetail = db.transactionDetail.Where(trx => trx.trx_id == trxDetail.trx_relation_id).FirstOrDefault();
																		trxRelatedDetail.usu_anulation = transaction.usu_id;
																		trxRelatedDetail.trx_anulationDate = DateTime.Now;
																}





																db.transactionDetail.Add(trxDetail);
																db.SaveChanges();

														}


														if (transaction.lsItems != null)
														{
																if (transaction.lsItems.Count > 0)
																{
																		foreach (var item in transaction.lsItems)
																		{
																				var taxValue = (item.taxesValue != null) ? item.taxesValue : 0;
																				transactionItems oItemDB = new transactionItems();
																				oItemDB.trx_id = trx_id;
																				oItemDB.mi_id = item.id;
																				oItemDB.mi_amount = item.amount;
																				oItemDB.mi_referencePrice = item.referencePrice;
																				oItemDB.mi_valueWithoutDiscount = item.valueWithoutDiscount;
																				oItemDB.mi_discountValue = item.discountValue;
																				oItemDB.mi_valueWithDiscountWithoutTaxes = item.valueWithDiscountWithoutTaxes;
																				oItemDB.mi_totalPrice = item.valueWithDiscountWithoutTaxes + taxValue;
																				oItemDB.mi_taxesValue = taxValue;
																				db.transactionItems.Add(oItemDB);
																				db.SaveChanges();
																		}
																}
														}


														if (transaction.lsObservations != null)
														{
																if (transaction.lsObservations.Count > 0)
																{
																		foreach (var observation in transaction.lsObservations)
																		{
																				observationsByTransaction observationDb = new observationsByTransaction();
																				observationDb.trx_id = trx_id;
																				observationDb.obstrx_description = observation.description;
																				observationDb.usr_id = transaction.usu_id;
																				observationDb.obstrx_registrationDate = DateTime.Now;
																				db.observationsByTransaction.Add(observationDb);

																				db.SaveChanges();
																		}
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


				[HttpGet]
				public IHttpActionResult GetTransactionsToApproveByClient(int client_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsTrxToApprove = db.STRPRC_GET_WORKORDERS_TO_APPROVE_BY_CLIENT1(client_id);

										var lsTransactions = new List<TransactionViewModel>();

										foreach (var trx in lsTrxToApprove)
										{
												TransactionViewModel trxToApprove = new TransactionViewModel();
												trxToApprove.id = trx.trx_id;
												trxToApprove.consecutive = trx.trx_consecutive;
												trxToApprove.code = trx.trx_code;
												trxToApprove.value = (double)trx.trx_value;
												trxToApprove.registrationDate = trx.trx_registrationDate;
												trxToApprove.client = new ClientViewModel();
												trxToApprove.client.id = trx.cli_id;
												trxToApprove.client.document = trx.cli_document;
												trxToApprove.client.name = trx.cli_name;

												trxToApprove.headerDetails = new TransactionDetailViewModel();

												trxToApprove.headerDetails.vehicle = new VehicleViewModel();
												trxToApprove.headerDetails.vehicle.id = trx.veh_id;
												trxToApprove.headerDetails.vehicle.licensePlate = trx.veh_licensePlate;

												trxToApprove.headerDetails.dealer = new DealerViewModel();
												trxToApprove.headerDetails.dealer.id = trx.deal_id;
												trxToApprove.headerDetails.dealer.name = trx.deal_name;

												trxToApprove.headerDetails.branch = new BranchViewModel();
												trxToApprove.headerDetails.branch.id = trx.bra_id;
												trxToApprove.headerDetails.branch.name = trx.bra_name;

												trxToApprove.headerDetails.contract = new ContractViewModel();
												trxToApprove.headerDetails.contract.id = trx.cntr_id;
												trxToApprove.headerDetails.contract.code = trx.cntr_code;
												trxToApprove.headerDetails.contract.name = trx.cntr_name;

												trxToApprove.headerDetails.maintenanceRoutine = new MaintenanceRoutineViewModel();
												trxToApprove.headerDetails.maintenanceRoutine.id = trx.mr_id;
												trxToApprove.headerDetails.maintenanceRoutine.name = trx.mr_name;

												lsTransactions.Add(trxToApprove);

										}

										return Ok(lsTransactions);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetTransactionsByDealerOrClient(int? dealer_id = null, int? client_id = null, DateTime? init_date = null, DateTime? end_date = null,	string code = null, string license_plate = null, int?state_trx = null )
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										code = (code != "" && code != "null" && code != null) ? code : null;
										license_plate = (license_plate != "" && license_plate != "null" && license_plate != null) ? license_plate : null;

										var lsTransactionByDealer = db.STRPRC_GET_TRANSACTIONS_BY_CLIENT_OR_DEALER(
														dealer_id,
														client_id,
														init_date,
														end_date,
														code,
														license_plate,
														state_trx);
										
										var lsTransactions = new List<TransactionViewModel>();

										foreach (var trx in lsTransactionByDealer)
										{
												var transaction = new TransactionViewModel();
												transaction.id = trx.trx_id;
												transaction.consecutive = trx.trx_consecutive;
												transaction.code = trx.trx_code;
												transaction.value = (double)trx.trx_value;
												if (trx.trxst_id != null)
												{
														transaction.transactionState = new TransactionStateViewModel();
														transaction.transactionState.id = trx.trxst_id;
														transaction.transactionState.name = trx.trxst_name;
												}
												
												transaction.registrationDate = trx.trx_registrationDate;

												transaction.client = new ClientViewModel();
												transaction.client.id = trx.cli_id;
												transaction.client.name = trx.cli_name;

												transaction.headerDetails = new TransactionDetailViewModel();

												transaction.headerDetails.vehicle = new VehicleViewModel();
												transaction.headerDetails.vehicle.id = trx.veh_id;
												transaction.headerDetails.vehicle.licensePlate = trx.veh_licensePlate;

												transaction.headerDetails.vehicle.vehicleModel = new VehicleModelViewModel();
												transaction.headerDetails.vehicle.vehicleModel.id = trx.vm_id;
												transaction.headerDetails.vehicle.vehicleModel.shortName = trx.vm_shortName;

												transaction.headerDetails.maintenanceRoutine = new MaintenanceRoutineViewModel();
												transaction.headerDetails.maintenanceRoutine.id = trx.mr_id;
												transaction.headerDetails.maintenanceRoutine.name = trx.mr_name;


												transaction.headerDetails.branch = new BranchViewModel();
												transaction.headerDetails.branch.id = trx.bra_id;
												transaction.headerDetails.branch.name = trx.bra_name;

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

				[HttpGet]
				public IHttpActionResult GetTransactionsByClient(int client_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsLogtransaction = db.STRPRC_GET_TRANSACTIONS_BY_CLIENT(client_id);
										var lsLogTrx = new List<LogTransactionViewModel>();

										foreach (var trx in lsLogtransaction)
										{
												TransactionViewModel logTrx = new TransactionViewModel();
												logTrx.id = trx.trx_id;
												logTrx.consecutive = trx.trx_consecutive;

												logTrx.value = (double)trx.trx_value;
												logTrx.registrationDate = trx.trx_registrationDate;


												logTrx.movement = new MovementViewModel();
												logTrx.movement.id = trx.m_id;
												logTrx.movement.name = trx.m_name;

												if (trx.trxst_id != null)
												{
														logTrx.transactionState = new TransactionStateViewModel();
														logTrx.transactionState.id = trx.trxst_id;
														logTrx.transactionState.name = trx.trxst_name;
												}

												logTrx.headerDetails = new TransactionDetailViewModel();

												if (trx.deal_id != null)
												{
														logTrx.headerDetails.dealer = new DealerViewModel();
														logTrx.headerDetails.dealer.id = trx.deal_id;
														logTrx.headerDetails.dealer.name = trx.deal_name;
												}

												if (trx.bra_id != null) {
														logTrx.headerDetails.branch = new BranchViewModel();
														logTrx.headerDetails.branch.id = trx.bra_id;
														logTrx.headerDetails.branch.name = trx.bra_name;
												}

												FinancialInformationViewModel initValues = new FinancialInformationViewModel();

												initValues.currentQuota = (double)trx.ltrx_initCurrentQuota;
												initValues.consumedQuota = (double)trx.ltrx_initConsumedQuota;
												initValues.inTransitQuota = (double)trx.ltrx_initInTransitQuota;

												FinancialInformationViewModel endValues = new FinancialInformationViewModel();
												endValues.currentQuota = (double)trx.ltrx_endCurrentQuota;
												endValues.consumedQuota = (double)trx.ltrx_endConsumedQuota;
												endValues.inTransitQuota = (double)trx.ltrx_endInTransitQuota;

												LogTransactionViewModel logTransaction = new LogTransactionViewModel();
												logTransaction.transaction = logTrx;
												logTransaction.initValues = initValues;
												logTransaction.endValues = endValues;

												lsLogTrx.Add(logTransaction);

										}

										return Ok(lsLogTrx);

								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}

				}


				[HttpGet]
				public IHttpActionResult GetTransactionById(int trx_id) {
						try
						{
								TransactionViewModel transaction = new TransactionViewModel();
								transaction = this.getTransactionHeader(trx_id);
								transaction.headerDetails = this.getTransactionDetails(trx_id);
								transaction.lsItems = this.getTransactionItems(trx_id);
								transaction.lsObservations = this.getTransactionObservations(trx_id);

								return Ok(transaction);
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);	
						}
				}


				private TransactionViewModel getTransactionHeader(int trx_id) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var transactionHeader = db.transactions.Where(trx => trx.trx_id == trx_id)
																								.Select(trx => new TransactionViewModel
																								{
																										id = trx.trx_id,
																										consecutive = trx.trx_consecutive,
																										code = trx.trx_code,
																										valueWithoutDiscount = (double)trx.trx_valueWithoutDiscount,
																										discountValue = (double)trx.trx_discountValue,
																										valueWithDiscountWithoutTaxes = (double) trx.trx_valueWithDiscountWithoutTaxes,
																										taxesValue = (double)trx.trx_taxesValue,
																										value = (double)trx.trx_value,
																										registrationDate = trx.trx_registrationDate,
																										client = new ClientViewModel() { 
																												id = trx.cli_id,
																												document = trx.Client.cli_document,
																												name = trx.Client.cli_name
																										},
																										transactionState = new TransactionStateViewModel()
																										{
																												id = trx.trxst_id,
																												name = trx.transactionState.trxst_name
																										},
																										usu_id = (int)trx.usu_id,
																										movement = new MovementViewModel() { 
																												id = trx.m_id,
																												name = trx.Movement.m_name
																										}
																								}).FirstOrDefault();
										return transactionHeader;
								}
								
						}
						catch (Exception ex)
						{
								throw;
						}
						
				}

				private TransactionDetailViewModel getTransactionDetails(int trx_id) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var transactionDetail = db.transactionDetail.Where(trx => trx.trx_id == trx_id)
																								.Select(trx => new TransactionDetailViewModel {
																										vehicle = new VehicleViewModel() { 
																												id = trx.veh_id,
																												licensePlate = trx.Vehicle.veh_licensePlate,
																												year = trx.Vehicle.veh_year,
																												mileage = trx.Vehicle.veh_mileage,
																												vehicleModel = new VehicleModelViewModel() { 
																														id = trx.Vehicle.vm_id,
																														shortName = trx.Vehicle.VehicleModel.vm_shortName,
																														type = new VehicleTypeViewModel() { 
																																id = trx.Vehicle.VehicleModel.VehicleType.vt_id,
																																name = trx.Vehicle.VehicleModel.VehicleType.vt_name
																														},
																														brand = new BrandViewModel() {
																																id = trx.Vehicle.VehicleModel.vb_id,
																																name = trx.Vehicle.VehicleModel.VehicleBrand.vb_name
																														}
																														
																												}
																										},
																										dealer = new DealerViewModel() { 
																												id = trx.deal_id,
																												name = trx.Dealer.deal_name
																										},
																										branch = new BranchViewModel() { 
																												id = trx.bra_id,
																												name = trx.branch.bra_name
																										},
																										maintenanceRoutine = new MaintenanceRoutineViewModel() { 
																												id = trx.mr_id,
																												name = trx.maintenanceRoutine.mr_name
																										},
																										contract = new ContractViewModel() { 
																												id = trx.cntr_id,
																												code = trx.Contract.cntr_code
																										},
																										userApprobation = (int)trx.usu_approbation,
																										userReject = (int)trx.usu_reject,
																										userAnulation = (int)trx.usu_anulation,
																										approbationDate = trx.trx_approbationDate,
																										rejectDate = trx.trx_rejectDate,
																										anulationDate = trx.trx_anulationDate,
																										relatedTransaction = new TransactionViewModel() { 
																												id = trx.trx_relation_id
																										}
																								})
																								.FirstOrDefault();
										return transactionDetail;
								}
								
						}
						catch (Exception)
						{
								throw;
						}
					
				}

				private List<MaintenanceItemViewModel> getTransactionItems(int trx_id)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsMaintenanceItems = db.transactionItems.Where(trx => trx.trx_id == trx_id)
																								.Select(mi => new MaintenanceItemViewModel
																								{
																										id = mi.mi_id,
																										code = mi.MaintenanceItem.mi_code,
																										name = mi.MaintenanceItem.mi_name,
																										type = new TypeOfMaintenanceItemViewModel() { 
																												id = mi.MaintenanceItem.tmi_id,
																												name = mi.MaintenanceItem.TypeOfMaintenanceItem.tmi_name
																										},
																										presentationUnit = new PresentationUnitViewModel() { 
																												id = mi.MaintenanceItem.pu_id,
																												longName = mi.MaintenanceItem.PresentationUnit.pu_longName,
																												shortName = mi.MaintenanceItem.PresentationUnit.pu_shortName
																										},
																										category = new CategoryViewModel() { 
																												id = mi.MaintenanceItem.mict_id,
																												name = mi.MaintenanceItem.MaintenanceItemCategory.mict_name
																										},
																										referencePrice = mi.mi_referencePrice,
																										valueWithoutDiscount = (float) mi.mi_valueWithoutDiscount,
																										discountValue = (float) mi.mi_discountValue,
																										valueWithDiscountWithoutTaxes = (float) mi.mi_valueWithDiscountWithoutTaxes,
																										taxesValue = (float)mi.mi_taxesValue,
																										amount = (float)mi.mi_amount,
																										handleTax = mi.MaintenanceItem.mi_handleTax																								

																								}).ToList();

								foreach (var maintenanceItem in lsMaintenanceItems)
								{
										if (maintenanceItem.handleTax == true)
										{
												var lsTaxes = db.TaxesByMaintenanceItem.Where(tx => tx.mi_id == maintenanceItem.id)
																															.Select(tx => new TaxViewModel
																															{
																																	id = tx.tax_id,
																																	name = tx.Taxes.tax_name,
																																	description = tx.Taxes.tax_desccription,
																																	percentValue = tx.Taxes.tax_percentValue,
																																	registrationDate = tx.Taxes.tax_registrationDate
																															}).ToList();

												if (lsTaxes != null)
												{
														maintenanceItem.lsTaxes = lsTaxes;
												}
										}
								}

								return lsMaintenanceItems;
						}
						
				}


				private List<TransactionObservationViewModel> getTransactionObservations(int trx_id)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsTrxRelated = db.transactionDetail
																		.Where(trx => trx.trx_relation_id == trx_id)
																		.Select(trx => trx.trx_id).ToList();

								lsTrxRelated.Add(trx_id);

								var lsObservations = db.observationsByTransaction.Where(trx => lsTrxRelated.Any(tr => tr == trx.trx_id))
																										.Select(trx => new TransactionObservationViewModel
																										{
																												id = trx.obstrx_id,
																												description = trx.obstrx_description,
																												registrationDate = trx.obstrx_registrationDate,
																												user = new UserAccessViewModel() { 
																														id_user = trx.usr_id,
																														name = trx.Users.usr_firstName,
																														lastName = trx.Users.usr_lastName
																												}
																										}).OrderBy(trx => trx.registrationDate)
																										.ToList();
								return lsObservations;
						}
						
				}


				[HttpGet]
				public IHttpActionResult getTransactionStates() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsStates = db.transactionState.Where(st => st.trxst_state == true)
																				.Select(st => new TransactionStateViewModel
																				{
																						id = st.trxst_id,
																						name = st.trxst_name,
																						description = st.trxst_description
																				}).ToList();

										
										var pendingState = new TransactionStateViewModel() { id = 0, name = "PENDIENTE" };
										
										lsStates.Add(pendingState);

										return Ok(lsStates);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}




		}
}
