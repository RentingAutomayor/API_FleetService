using System;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;
using System.Collections.Generic;

namespace API_FleetService.Controllers
{
    public class SettingsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult UpdateTables(SettingsViewModel settings)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    if (settings.domain.Equals("brands"))
                    {
                        settings.rows.ForEach(row => {
                            int code = int.Parse(row.code);
                            var brand = db.VehicleBrand.Where(item => item.vb_id == code).FirstOrDefault();
                            if (brand is null)
                            {
                                VehicleBrand vehicleBrand = new VehicleBrand();
                                vehicleBrand.vb_name = row.name;
                                vehicleBrand.vb_state = true;
                                db.VehicleBrand.Add(vehicleBrand);
                            }
                            else {
                                brand.vb_name = row.name;
                            }
                            db.SaveChanges();
                        });
                    } else if (settings.domain.Equals("types"))
                    {
                        settings.rows.ForEach(row => {
                            int code = int.Parse(row.code);
                            var type = db.VehicleType.Where(item => item.vt_id == code).FirstOrDefault();
                            if(type is null)
                            {
                                VehicleType vehicleType = new VehicleType();
                                vehicleType.vt_name = row.name;
                                vehicleType.vt_state = true;
                                db.VehicleType.Add(vehicleType);
                            }
                            else
                            {
                                type.vt_name = row.name;
                            }
                            db.SaveChanges();
                        });
                    }
                    else
                    {
                        settings.rows.ForEach(row => {
                            int code = int.Parse(row.code);
                            var model = db.VehicleModel.Where(item => item.vm_id == code).FirstOrDefault();
                            if(model is null)
                            {
                                VehicleModel vehicleModel = new VehicleModel();
                                vehicleModel.vm_shortName = row.name;
                                vehicleModel.vm_state = true;
                                vehicleModel.vt_id = settings.typeId;
                                vehicleModel.vb_id = settings.brandId;
                                db.VehicleModel.Add(vehicleModel);
                            }
                            else
                            {
                                model.vm_shortName = row.name;
                            }
                            db.SaveChanges();
                        });
                    }
                    return Ok();
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(EmailViewModel email)
        {
            try{
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    string notFound = "No se encontro el elemento. Intente con otro";
                    var settings = db.EmailSettings.Find(email.id);
                    if (settings is null)
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    settings.email = email.email;
                    settings.password = email.password;
                    db.SaveChanges();
                    return Ok();
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSettings()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    string notFound = "No se encontro el elemento. Intente con otro";
                    var settings = db.EmailSettings.Find(1);
                    if (settings is null)
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    return Ok(settings);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(string domain, int rowId)
        {
            try
            {
                string notFound = "No existe el elemento que desea eliminar";
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    if (domain.Equals("brands"))
                    {
                        var brand = db.VehicleBrand.Where(item => item.vb_id == rowId).FirstOrDefault();
                        if (brand is null)
                            return BadRequest(new ResponseApiViewModel().Message = notFound); 
                        db.VehicleBrand.Remove(brand);
                    }else if (domain.Equals("types"))
                    {
                        var type = db.VehicleType.Where(item => item.vt_id == rowId).FirstOrDefault();
                        if(type is null)
                            return BadRequest(new ResponseApiViewModel().Message = notFound);
                        db.VehicleType.Remove(type);
                    }
                    else
                    {
                        var model = db.VehicleModel.Where(item => item.vm_id == rowId).FirstOrDefault();
                        if(model is null)
                            return BadRequest(new ResponseApiViewModel().Message = notFound);
                        db.VehicleModel.Remove(model);
                    }
                    db.SaveChanges();
                    return Ok();
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}