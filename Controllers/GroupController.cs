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
    public class GroupController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsGroup = (from grp in db.Groups
                                   select new
                                   {
                                       id_group = grp.grp_id,
                                       groupName = grp.grp_name,
                                       modules = from mdl in db.Modules
                                                 join gma in db.GroupModuleAction on mdl.mdl_id equals gma.mdl_id
                                                 where gma.grp_id == grp.grp_id
                                                 group mdl by mdl.mdl_id into moduls
                                                 select new
                                                 {
                                                     id_module = moduls.Key,
                                                     modulsGroup = moduls.ToList()
                                                 }

                                   }).ToList();


                    //{

                    //    id_module = mdl.mdl_id,
                    //                                 moduleName = mdl.mdl_name,
                    //                                 path = mdl.mdl_path
                    //                             }

                    //actions = from act in db.Actions
                    //          join gma in db.GroupModuleAction on act.act_id equals gma.act_id
                    //          where gma.grp_id == grp.grp_id
                    //          select new
                    //          {
                    //              id_action = act.act_id,
                    //              actionName = act.act_description
                    //          }


                    //lsGroup = lsGroup.GroupBy(mdl => mdl.modules).ToList();


                    return Ok(lsGroup);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(GroupViewModel pGroup)
        {
            //TODO: Agregar usuario que hizo la acción
            ResponseApiViewModel rta = new ResponseApiViewModel();
            try
            {
                Groups GroupToInsert = GroupViewModel.PrepareGroupToInsertDB(pGroup);
                bool GroupWasInserted = GroupViewModel.InsertIntoDB(GroupToInsert);

                if (GroupWasInserted)
                {
                    rta.response = true;
                    rta.message = "El grupo " + pGroup.name + " fue insertado correctamente en la base de datos.";
                    return Ok(rta);
                }
                else
                {
                    rta.response = false;
                    rta.message = "Ha ocurrido un error intentado insertar el grupo:  " + pGroup.name;
                    return BadRequest(rta.message);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(GroupViewModel pGroup)
        {
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var ogroupDB = db.Groups.Where(gr => gr.grp_id == pGroup.idGroup || gr.grp_name == pGroup.name)
                                                                    .FirstOrDefault();
                    if (ogroupDB != null)
                    {
                        if (pGroup.name.Trim() == "")
                        {
                            throw new Exception("El nombre del grupo no es válido");
                        }

                        ogroupDB.grp_name = pGroup.name;
                        ogroupDB.grp_description = string.IsNullOrEmpty(pGroup.description) ? pGroup.description : "";

                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha actualizado el grupo: " + pGroup.name;
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el grupo: " + pGroup.name + " en la base de datos, por favor rectifique los datos";
                        return BadRequest(rta.message);
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // opcion de eliminar/desabilitar ? campo en tabla
    }
}
