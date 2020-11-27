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
		public class JobTitleController : ApiController
		{
				[HttpGet]
				public IHttpActionResult getJobTitleByDescription(string pDescription) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {

										var lsJobTitles = db.JobTitlesClient.Where(jt => jt.jtcl_description.ToUpper().Contains(pDescription.ToUpper()))
														.Select(jt => new JobTitleViewModel
														{
																id = jt.jtcl_id,
																description = jt.jtcl_description,
																state = jt.jtcl_state
														})
														.ToList()
														.Take(5);

										return Ok(lsJobTitles);
								}
								
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}
		}
}
