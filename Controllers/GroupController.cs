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
                var lsGroup = new List<Group>();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    lsGroup = db.Groups.Select(grp => new Group
                                       {
                                           id_group = grp.grp_id,
                                           groupName = grp.grp_name
                                       }).ToList();


                    lsGroup = buildModulesActions(lsGroup);

                    return Ok(lsGroup);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private List<Group> buildModulesActions(List<Group> groups)
        {
            try
            {
                var access = new List<GroupModuleAction>();
                // trae todas las relaciones de grupos y modulos posibles
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    access = db.GroupModuleAction.ToList();                    
                }

                var groupMA = access.GroupBy(g => g.mdl_id);

                foreach (var group in groups)
                {
                    group.modules = new List<Module>();

                    using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                    {
                        foreach (var module in groupMA)
                        {
                            var moduleItem = db.Modules.Where(mdl => mdl.mdl_id == module.Key)
                                              .Select(mdl => new Module
                                              {
                                                  id_module = mdl.mdl_id,
                                                  moduleName = mdl.mdl_name,
                                                  id_moduleF = mdl.mdl_father,
                                                  moduleDescription = mdl.mdl_description,
                                                  path = mdl.mdl_path
                                              }).FirstOrDefault();

                            moduleItem.actions = new List<ActionModule>();

                            foreach (var action in module)
                            {
                                var actionItem = db.Actions.Where(act => act.act_id == action.act_id)
                                                    .Select(act => new ActionModule
                                                    {
                                                        id_action = act.act_id,
                                                        actionName = act.act_name
                                                    }).FirstOrDefault();

                                moduleItem.actions.Add(actionItem);
                            }

                            group.modules.Add(moduleItem);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return new List<Group>();
            }

            return groups;
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
