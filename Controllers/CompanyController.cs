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
    public class CompanyController : ApiController
    {
		[HttpGet]
		public IHttpActionResult Get()
		{
			try
			{
				using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
				{
					var lsCompany = db.Company.Where(cpn => cpn.cpn_state == true)
							.Select(cpn => new CompanyViewModel
							{
								id = cpn.cpn_id,
								name = cpn.cpn_razonSocial,
								nit = cpn.cpn_nit,
								state = cpn.cpn_state
							}).ToList()
							.Take(100)
							.OrderByDescending(cpn => cpn.name);

					return Ok(lsCompany);
				}

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
