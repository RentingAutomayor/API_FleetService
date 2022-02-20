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
		public class CityController : ApiController
		{
				[HttpGet]
				public IHttpActionResult GetDepartments() {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var lsDepartments = db.Departments.Select(
												dpt => new DepartmentViewModel {
														id = dpt.dpt_id,
														name = dpt.dpt_name,
														state = dpt.dpt_state,
														country_id = dpt.ctry_id
																										
										}).ToList();
										return Ok(lsDepartments);
								}
										
						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetCitiesByDepartmentId(int pDepartment_id)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsDepartments = db.Cities.Where(cty => cty.dpt_id == pDepartment_id)
												.Select(
												cty => new CityViewModel
												{
														id = cty.cty_id,
														name = cty.cty_name,
														state = cty.cty_state,
														departmentId = cty.dpt_id

												}).ToList();
										return Ok(lsDepartments);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				[HttpGet]
				public IHttpActionResult Get()
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsDepartments = db.Cities
												.Select(
												cty => new CityViewModel
												{
														id = cty.cty_id,
														name = cty.cty_name,
														state = cty.cty_state,
														departmentId = cty.dpt_id

												}).ToList();
										return Ok(lsDepartments);
								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}



		}
}
