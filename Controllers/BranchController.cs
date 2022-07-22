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
																			city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,	registrationDate = bra.bra_registrationDate,
																			Client_id = (bra.cli_id != null) ? bra.cli_id : null,
																			Dealer_id = (bra.deal_id != null) ? bra.deal_id : null,

																	}).FirstOrDefault();

										return Ok(oBranch);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				public static BranchViewModel GetBranchById(int pId)
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
																			city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,
																			registrationDate = bra.bra_registrationDate,
																			Client_id = (bra.cli_id != null) ? bra.cli_id : null,
																			Dealer_id = (bra.deal_id != null) ? bra.deal_id : null,

																	}).FirstOrDefault();

										return oBranch;
								}
						}
						catch (Exception ex)
						{
								throw ex;
						}
				}


				[HttpPost]
				public IHttpActionResult Insert(BranchViewModel pBranch)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var branchInserted = BranchController.InsertBranch(pBranch);									
										return Ok(branchInserted);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				
				public static BranchViewModel InsertBranch(BranchViewModel pBranch)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										branch oBranchDB = new branch();
										BranchController.setDataToBranch(pBranch, ref oBranchDB, true);
										db.branch.Add(oBranchDB);
										db.SaveChanges();
										return BranchController.getLastBranchInserted();
								}
						}
						catch (Exception ex)
						{
								throw ex;
						}
				}

				public static BranchViewModel getLastBranchInserted() {
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lastBranch = db.branch
										.Select(bra => new BranchViewModel
										{
												id = bra.bra_id,
												name = bra.bra_name,
												phone = bra.bra_phone,
												cellphone = bra.bra_cellphone,
												address = bra.bra_adress,
												city = (bra.cty_id != null) ? new CityViewModel { id = bra.cty_id, name = bra.Cities.cty_name, departmentId = bra.Cities.dpt_id } : null,
												registrationDate = bra.bra_registrationDate,
												Client_id = (bra.cli_id != null) ? bra.cli_id : null,
												Dealer_id = (bra.deal_id != null) ? bra.deal_id : null,

										}).OrderByDescending(brn => brn.id).
										FirstOrDefault();

								return lastBranch;
						}
				}


				[HttpPost]
				public IHttpActionResult Update(BranchViewModel pBranch)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var branchUpdated = BranchController.UpdateBranch(pBranch);
										return Ok(branchUpdated);

								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				public static BranchViewModel UpdateBranch(BranchViewModel pBranch)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oBranchDB = db.branch.Where(bra => bra.bra_id == pBranch.id).FirstOrDefault();
										BranchController.setDataToBranch(pBranch,ref oBranchDB,false);
										db.SaveChanges();									
										return BranchController.GetBranchById((int)pBranch.id);

								}

						}
						catch (Exception ex)
						{
								throw ex;
						}
				}

				private static void setDataToBranch(BranchViewModel branch, ref branch branchDB, bool isToInsert) {
						branchDB.bra_name = branch.name;
						branchDB.bra_phone = branch.phone;
						branchDB.bra_cellphone = branch.cellphone;
						branchDB.bra_adress = branch.address;
						branchDB.cty_id = (branch.city != null) ? branch.city.id : null;
						if (branch.Client_id != 0)
						{
								branchDB.cli_id = branch.Client_id;
						}
						if (branch.Dealer_id != 0)
						{
								branchDB.deal_id = branch.Dealer_id;
						}
						if (isToInsert) {
								branchDB.bra_state = true;
								branchDB.bra_registrationDate = DateTime.Now;
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
										if (BranchController.deleteBranchById((int)pBranch.id))
										{
												rta.response = true;
												rta.message = "Se ha eliminado la sucursal " + pBranch.name;											
										}

										return Ok(rta);

								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				public static bool deleteBranchById(int branchId) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oBranchDB = db.branch
												.Where(bra => bra.bra_id == branchId)
												.FirstOrDefault();

										db.branch.Remove(oBranchDB);
										db.SaveChanges();
										return true;
								}

						}
						catch (Exception ex)
						{
								throw ex;
						}
								
				}

				
				public static bool DeleteBranchById(int branchId)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oBranchDB = db.branch.Where(bra => bra.bra_id == branchId).FirstOrDefault();
										db.branch.Remove(oBranchDB);
										db.SaveChanges();										
										return true;
								}

						}
						catch (Exception ex)
						{

								return false;
						}
				}



				public static  List<BranchViewModel> getListOfBranchsByClientId(int clientId)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsBranch = db.branch
												.Where(bra => bra.cli_id == clientId && bra.bra_state == true)
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

								return lsBranch;
						}
				}


				public static List<BranchViewModel> getListOfBranchsByDealer(int dealerId)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsBranch = db.branch
												.Where(bra => bra.deal_id == dealerId && bra.bra_state == true)
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

								return lsBranch;
						}
				}


		}
}
