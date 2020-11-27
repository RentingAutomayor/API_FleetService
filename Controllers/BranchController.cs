using API_FleetService.ViewModels;
using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_FleetService.Controllers
{
    public class BranchController : ApiController
    {
				[HttpGet]
				public IHttpActionResult Get(int pOwner_id, string pKindOfOwner)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsBranch = new List<BranchViewModel>();

										switch (pKindOfOwner.ToUpper())
										{
												case "CLIENT":
														lsBranch = db.branch
																	.Where(bra => bra.cli_id == pOwner_id && bra.bra_state == true)
																	.Select(bra => new BranchViewModel
																	{
																			id = bra.bra_id,
																			name = bra.bra_name,																			
																			phone = bra.bra_phone,
																			cellphone = bra.bra_cellphone,																			
																			address = bra.bra_adress,
																			city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,	registrationDate = bra.bra_registrationDate

																	}).ToList();
														break;
												case "DEALER":
														lsBranch = db.branch
																	.Where(bra => bra.deal_id == pOwner_id && bra.bra_state == true)
																	.Select(bra => new BranchViewModel
																	{
																			id = bra.bra_id,
																			name = bra.bra_name,																			
																			phone = bra.bra_phone,
																			cellphone = bra.bra_cellphone,																			
																			address = bra.bra_adress,
																			city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,	
																			registrationDate = bra.bra_registrationDate

																	}).ToList();
														break;
												default:
														lsBranch = null;
														break;
										}

										return Ok(lsBranch);
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
										var oBranch = db.branch
																	.Where(bra => bra.bra_id == pId)
																	.Select(bra => new BranchViewModel
																	{
																			id = bra.bra_id,
																			name = bra.bra_name,																			
																			phone = bra.bra_phone,
																			cellphone = bra.bra_cellphone,																			
																			address = bra.bra_adress,
																			city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,	registrationDate = bra.bra_registrationDate

																	}).FirstOrDefault();

										return Ok(oBranch);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Insert(BranchViewModel pBranch)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										branch oBranchDB = BranchViewModel.setDataBranch(pBranch);
										db.branch.Add(oBranchDB);
										db.SaveChanges();
										rta.response = true;
										rta.message = "Sucursal insertada correctamente a la bd y es asociada al cliente: [" + pBranch.Client_id + "] ó al concesionario: [" + pBranch.Dealer_id + "]";
										return Ok(rta);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Update(BranchViewModel pBranch)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oBranchDB = db.branch.Where(bra => bra.bra_id == pBranch.id).FirstOrDefault();
										oBranchDB.bra_name = pBranch.name;										
										oBranchDB.bra_phone = pBranch.phone;
										oBranchDB.bra_cellphone = pBranch.cellphone;										
										oBranchDB.bra_adress = pBranch.address;										
										oBranchDB.cty_id = (pBranch.city != null) ? pBranch.city.id : null;										
										if (pBranch.Client_id != 0)
										{
												oBranchDB.cli_id = pBranch.Client_id;
										}
										if (pBranch.Dealer_id != 0)
										{
												oBranchDB.deal_id = pBranch.Dealer_id;
										}
										db.SaveChanges();
										rta.response = true;
										rta.message = "Se ha actualizado la sucursal: " + oBranchDB.bra_name;
										return Ok(rta);

								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Delete(BranchViewModel pBranch)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oBranchDB = db.branch.Where(bra => bra.bra_id == pBranch.id).FirstOrDefault();
										db.branch.Remove(oBranchDB);
										db.SaveChanges();
										rta.response = true;
										rta.message = "Se ha eliminado la sucursal " + oBranchDB.bra_name;
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
