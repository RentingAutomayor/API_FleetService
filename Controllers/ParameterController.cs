using System;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class ParameterController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetCompanies()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var companies = db.Company.Select(company => new CompanyViewModel()
                    {
                        id = company.cpn_id,
                        name = company.cpn_razonSocial,
                        nit = company.cpn_nit
                    }).ToList();
                    return Ok(companies);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetModules()
        {
            try
            {
                using(DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var modules = db.Modules.Select(module => new ModuleViewModel()
                    {
                        id = module.mdl_id,
                        name = module.mdl_name
                    }).ToList();
                    return Ok(modules);
                } 
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDealers()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var dealers = db.Dealer.Select(dealer => new BasicLookup()
                    {
                        id = dealer.deal_id,
                        name = dealer.deal_name
                    }).ToList();
                    return Ok(dealers);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetClients()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var clients = db.Client.Select(client => new BasicLookup()
                    {
                        id = client.cli_id,
                        name = client.cli_name
                    }).ToList();
                    return Ok(clients);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}