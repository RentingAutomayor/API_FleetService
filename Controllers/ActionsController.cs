using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class ActionsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()  
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {                    
                    var lsActions = db.Actions.Where(ac => ac.activo == true).Select(ac => new ActionViewModel
                    { 
                        act_id = ac.act_id,
                        act_name = ac.act_name,
                        act_description = ac.act_description                        
                    }).ToList();
                    return Ok(lsActions);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(Actions pAction)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oAccionDB = db.Actions.Where(ac => ac.act_id == pAction.act_id).FirstOrDefault();
                    if (oAccionDB != null)
                    {
                        var relaciones = db.GroupModuleAction.Where(gma => gma.act_id == oAccionDB.act_id).ToList();
                        //db.GroupModuleAction.RemoveRange(relaciones);
                        //db.Actions.Remove(oAccionDB);
                        oAccionDB.activo = false;
                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha eliminado la accion: " + oAccionDB.act_name + " de la base de datos";
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró la accion por lo tanto no se puede eliminar";
                        return BadRequest(rta.message);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(ActionViewModel pAction)
        {
            //TODO: Agregar usuario que hizo la acción
            ResponseApiViewModel rta = new ResponseApiViewModel();
            try
            {

                Actions ActionToInsert = ActionViewModel.PrepareActionToInsertDB(pAction);
                bool ActionWasInserted = ActionViewModel.InsertIntoDB(ActionToInsert);

                if (ActionWasInserted)
                {
                    rta.response = true;
                    rta.message = "La accion " + pAction.act_name + " fue insertada correctamente en la base de datos.";
                    return Ok(rta);
                }
                else
                {
                    rta.response = false;
                    rta.message = "Ha ocurrido un error intentado insertar la accion:  " + pAction.act_name;
                    return BadRequest(rta.message);
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
                    var oActionDB = db.Actions.Where(ac => ac.act_id == pId)
                        .Select(ac => new ActionViewModel
                        {
                            act_id = ac.act_id,
                            act_name = ac.act_name,
                            act_description = ac.act_description
                        }).FirstOrDefault();
                    return Ok(oActionDB);
                }  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(ActionViewModel pAction)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                if (pAction.act_id == 0)
                {
                    throw new Exception("La accion no es valida.");
                }

                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var oActionDB = db.Actions.Where(act => act.act_id == pAction.act_id).FirstOrDefault();
                    if (oActionDB != null)
                    {
                        if (pAction.act_name.Trim() == "")
                        {
                            throw new Exception("El nombre de la accion es obligatorio, no se puede insertar a la base de datos sin esta información");
                        }

                        oActionDB.act_name = pAction.act_name;
                        oActionDB.act_description = pAction.act_description;
                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha actualizado la accion: " + pAction.act_id + " - " + pAction.act_name;
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró laaccion: " + pAction.act_name + " en la base de datos, por favor rectifique los datos";
                        return BadRequest(rta.message);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
