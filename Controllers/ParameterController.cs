using System;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;
using System.Collections.Generic;

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

        [HttpGet]
        public IHttpActionResult GetBrands()
        {
            try
            {
                using(DB_FleetServiceEntities db = new DB_FleetServiceEntities()){
                    var brands = db.VehicleBrand.Select(brand => new BasicLookup() { 
                        id = brand.vb_id,
                        name = brand.vb_name
                    }).ToList();
                    return Ok(brands);
                }
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetVehiculeType()
        {
            try
            {
                using(DB_FleetServiceEntities db = new DB_FleetServiceEntities()){
                    var types = db.VehicleType.Select(type => new BasicLookup()
                    {
                        id = type.vt_id,
                        name = type.vt_name
                    }).ToList();
                    return Ok(types);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetVehiculeModel(int brandId, int typeId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
                    var vehiculeModels = db.VehicleModel
                        .SqlQuery("SELECT * FROM VehicleModel WHERE vb_id = " + brandId + "AND vt_id = " + typeId)
                        .Select(model => new BasicLookup() { 
                            id = model.vm_id,
                            name = model.vm_shortName
                        }).ToList();
                    return Ok(vehiculeModels);
                }
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetReports()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var reports = db.Reports.Select(report => new ReportViewModel()
                    {
                        id = (int)report.id,
                        name = report.name,
                        method = report.method,
                        service = report.service
                    }).ToList();
                    return Ok(reports);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}