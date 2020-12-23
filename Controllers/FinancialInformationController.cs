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
		public class FinancialInformationController : ApiController
		{

				[HttpGet]
				public IHttpActionResult GetClientsWithQuota()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsFinancialInformation = db.STRPRC_GET_FINANCIAL_INFORMATION_BY_CLIENT();
										List<FinancialInformationViewModel> lsFinancialInformationByClient = new List<FinancialInformationViewModel>();
										foreach (var financialInformationByClient in lsFinancialInformation)
										{
												FinancialInformationViewModel finantialInformation = new FinancialInformationViewModel();

												var clientTmp = new ClientViewModel();
												clientTmp.id = financialInformationByClient.cli_id;
												clientTmp.document = financialInformationByClient.cli_document;
												clientTmp.name = financialInformationByClient.cli_name;
												clientTmp.phone = financialInformationByClient.cli_phone;
												clientTmp.cellphone = financialInformationByClient.cli_cellphone;
												clientTmp.website = financialInformationByClient.cli_website;
												clientTmp.city = (financialInformationByClient.cty_id != null) ? new CityViewModel { id = financialInformationByClient.cty_id } : null;

												finantialInformation.client = clientTmp;
												finantialInformation.approvedQuota = double.Parse(financialInformationByClient.ficl_approvedQuota.ToString());
												finantialInformation.currentQuota = double.Parse(financialInformationByClient.ficl_currentQuota.ToString());
												finantialInformation.consumedQuota = double.Parse(financialInformationByClient.ficl_consumedQuota.ToString());
												finantialInformation.inTransitQuota = double.Parse(financialInformationByClient.ficl_inTransitQuota.ToString());

												lsFinancialInformationByClient.Add(finantialInformation);
										}
										return Ok(lsFinancialInformationByClient);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetClientsWithoutQuota()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsClientsWithoutQuota = db.STRPRC_GET_CLIENTS_WITHOUT_QUOTA();
										List<ClientViewModel> lsClient = new List<ClientViewModel>();
										foreach (var client in lsClientsWithoutQuota)
										{
												var clientTmp = new ClientViewModel();
												clientTmp.id = client.cli_id;
												clientTmp.document = client.cli_document;
												clientTmp.name = client.cli_name;
												clientTmp.phone = client.cli_phone;
												clientTmp.cellphone = client.cli_cellphone;
												clientTmp.website = client.cli_website;
												clientTmp.city = (client.cty_id != null) ? new CityViewModel { id = client.cty_id } : null;
												lsClient.Add(clientTmp);
										}
										return Ok(lsClient);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult ValidatePaymentVsConsumedQuota(int pClient_id, double paymentValue) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var financialInformation = db.FinancialInformationByClient.Where(fi => fi.ficl_state == true && fi.cli_id == pClient_id).FirstOrDefault();
										var rta = new ResponseApiViewModel();

										if (paymentValue > financialInformation.ficl_consumedQuota)
										{
												rta.response = false;
												rta.message = "El pago no puede superar el valor del cupo consumido";
										}
										else {
												rta.response = true;
												rta.message = "El pago que va a generar es válido";
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
				public IHttpActionResult GetFinancialInformationByClient(int client_id) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var oFinancialInformation = db.FinancialInformationByClient.Where(cl => cl.cli_id == client_id && cl.ficl_state == true)
																										.Select(fi => new FinancialInformationViewModel { 
																												id = fi.ficl_id,
																												approvedQuota = (double) fi.ficl_approvedQuota,
																												currentQuota = (double) fi.ficl_currentQuota,
																												consumedQuota = (double) fi.ficl_consumedQuota,
																												inTransitQuota = (double) fi.ficl_inTransitQuota
																										})
																										.FirstOrDefault();
										return Ok(oFinancialInformation);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
		}
}

