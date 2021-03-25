using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class ReportController : ApiController
    {
        [HttpGet]
        public IHttpActionResult getTotalWorkOrderByDealerAndClient(int? dealer_id = null, int? client_id = null, DateTime? init_date = null, DateTime? end_date = null) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var infoReport = db.STRPRC_GET_COUNT_TOTAL_WORKORDER_BY_DEALER_AND_CLIENT(
														dealer_id,
														client_id,
														init_date,
														end_date
												);
										return Ok(infoReport.ToArray());
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
        }


				[HttpGet]
				public IHttpActionResult getWorkOrderApprovedByVehicle(int? client_id = null, int? dealer_id = null,string license_plate = null, DateTime? init_date = null, DateTime? end_date = null) {
						try
						{
								license_plate = (license_plate == "null" || license_plate == "") ? null : license_plate;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var infoReport = db.STRPRC_GET_WORKORDERS_APPROVED_BY_VEHICLE(
														client_id,
														dealer_id,
														license_plate,
														init_date,
														end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult getWorkOrderCanceledByVehicle(int? client_id = null, int? dealer_id = null, string license_plate = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								license_plate = (license_plate == "null" || license_plate == "") ? null : license_plate;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_WORKORDERS_CANCELED_BY_VEHICLE(
																							client_id,
																							dealer_id,
																							license_plate,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult getWorkOrderPendingByVehicle(int? client_id = null, int? dealer_id = null, string license_plate = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								license_plate = (license_plate == "null" || license_plate == "") ? null : license_plate;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_WORKORDERS_PENDING_BY_VEHICLE(
																							client_id,
																							dealer_id,
																							license_plate,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult getWorkOrderFinisedByVehicle(int? client_id = null, int? dealer_id = null, string license_plate = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								license_plate = (license_plate == "null" || license_plate == "") ? null : license_plate;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_WORKORDERS_FINISHED_BY_VEHICLE(
																							client_id,
																							dealer_id,
																							license_plate,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult getWorkOrderAnnulByVehicle(int? client_id = null, int? dealer_id = null, string license_plate = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								license_plate = (license_plate == "null" || license_plate == "") ? null : license_plate;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_WORKORDERS_ANNUL_BY_VEHICLE(
																							client_id,
																							dealer_id,
																							license_plate,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}






				[HttpGet]
				public IHttpActionResult getWorkOrdersValueByMonth(int? client_id = null, int? dealer_id = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_WORKORDERS_VALUES_BY_MONTH(
																							client_id,
																							dealer_id,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult getAmountWorkOrdersByClientAndMonth(int? client_id = null, int? dealer_id = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_AMOUNT_WORKORDERS_BY_CLIENT_AND_MONTH(
																							client_id,
																							dealer_id,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult getAmountWorkOrdersByDealerAndMonth(int? client_id = null, int? dealer_id = null, DateTime? init_date = null, DateTime? end_date = null)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var infoReport = db.STRPRC_GET_AMOUNT_WORKORDERS_BY_DEALER_AND_MONTH(
																							client_id,
																							dealer_id,
																							init_date,
																							end_date);
										return Ok(infoReport.ToArray());
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
		}
}
