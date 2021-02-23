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
                        groupName = grp.grp_name,
                        description = grp.grp_description
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

        [HttpGet]
        public IHttpActionResult GetById(int pId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oGroupDB = db.Groups.Where(gr => gr.grp_id == pId)
                        .Select(grp => new Group
                        {
                            id_group = grp.grp_id,
                            groupName = grp.grp_name,
                            description = grp.grp_description
                        }).ToList();


                    oGroupDB = buildModulesActions(oGroupDB);

                    return Ok(oGroupDB[0]);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult Modules()
        {
            try
            {
                var lsModule = new List<Module>();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    lsModule = db.Modules.Select(mdl => new Module
                    {
                        id_module = mdl.mdl_id,
                        moduleName = mdl.mdl_name
                    }).ToList();

                    return Ok(lsModule);
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

                

                foreach (var group in groups)
                {

                    var groupMA = access.Where(ac => ac.grp_id == group.id_group).GroupBy(g => g.mdl_id);

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
                    var idGroup = GroupViewModel.GroupID(pGroup.groupName);

                    List<GroupModuleAction> gma = new List<GroupModuleAction>();

                    foreach (var module in pGroup.moduleAction)
                    {
                        foreach (var action in module.actions)
                        {
                            var groupModuleAction = new GroupModuleAction();
                            groupModuleAction.grp_id = idGroup;
                            groupModuleAction.mdl_id = module.id_module;
                            groupModuleAction.act_id = action.act_id;

                            gma.Add(groupModuleAction);
                        }
                    }

                    bool MGAWasInserted = GroupViewModel.InsertGroupModuleAction(gma);

                    if (MGAWasInserted)
                    {
                        rta.response = true;
                        rta.message = "El grupo " + pGroup.groupName + " y sus acciones fueron insertados correctamente en la base de datos.";
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "Ha ocurrido un error intentado insertar las acciones para el grupo:  " + pGroup.groupName;
                        return BadRequest(rta.message);
                    }


                }
                else
                {
                    rta.response = false;
                    rta.message = "Ha ocurrido un error intentado insertar el grupo:  " + pGroup.groupName;
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
                var ogroupDB = new Groups();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ogroupDB = db.Groups.Include("GroupModuleAction").Where(gr => gr.grp_id == pGroup.id_group || gr.grp_name == pGroup.groupName)
                                                                    .FirstOrDefault();
                    if (ogroupDB != null)
                    {
                        if (pGroup.groupName.Trim() == "")
                        {
                            throw new Exception("El nombre del grupo no es válido");
                        }

                        ogroupDB.grp_name = pGroup.groupName;
                        ogroupDB.grp_description = string.IsNullOrEmpty(pGroup.description) ? "": pGroup.description ;

                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha actualizado el grupo: " + pGroup.groupName;
                        
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el grupo: " + pGroup.groupName + " en la base de datos, por favor rectifique los datos";
                        return BadRequest(rta.message);
                    }                   

                }

                try
                {
                    var oModAct = actualizarModulos(pGroup);
                }
                catch (Exception)
                {

                    throw;
                }


                return Ok(rta);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public bool actualizarModulos(GroupViewModel pgroupDB) {
            var respuesta = true;

            try
            {
                DB_FleetServiceEntities gmaCnn = new DB_FleetServiceEntities();
                List<GroupModuleAction> gmaList = gmaCnn.GroupModuleAction.Where(gma => gma.grp_id == pgroupDB.id_group).ToList();

                foreach (var gmaMod in pgroupDB.moduleAction)
                {
                    foreach (var gmaAct in gmaMod.actions)
                    {
                        if (!gmaCnn.GroupModuleAction.Any(gma => gma.grp_id == pgroupDB.id_group &&
                                                             gma.mdl_id == gmaMod.id_module &&
                                                             gma.act_id == gmaAct.act_id))
                        {
                            var gma = new GroupModuleAction()
                            {
                                grp_id = pgroupDB.id_group,
                                mdl_id = gmaMod.id_module,
                                act_id = gmaAct.act_id
                            };

                            using (DB_FleetServiceEntities gmaAdd = new DB_FleetServiceEntities())
                            {
                                try
                                {
                                    gmaAdd.GroupModuleAction.Add(gma);
                                    gmaAdd.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    return false;
                                    
                                }                                       
                            }
                        }
                    }

                    
                }

                foreach (var gmaMod in pgroupDB.moduleAction)
                {
                    foreach (var gmaAct in gmaMod.actions)
                    {
                        var item = gmaList.FirstOrDefault(gma => gma.mdl_id == gmaMod.id_module && gma.act_id == gma.act_id);
                        if (item != null)
                        {
                            gmaList.Remove(item);
                        }
                       
                    }
                }

                foreach (var gmaOld in gmaList)
                {
                    try
                    {
                        using (DB_FleetServiceEntities gmaRemove = new DB_FleetServiceEntities())
                        {
                            var item = gmaRemove.GroupModuleAction.Where(mga => mga.grpmdlact_id == gmaOld.grpmdlact_id).FirstOrDefault();

                            if (item != null)
                            {
                                gmaRemove.GroupModuleAction.Remove(item);
                                gmaRemove.SaveChanges();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        [HttpPost]
        public IHttpActionResult Delete(Group pGroup)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oGroupDB = db.Groups.Where(gr => gr.grp_id == pGroup.id_group).FirstOrDefault();
                    if (oGroupDB != null)
                    {
                        //oUserDB.cli_document = "";
                        //oUserDB.cli_state = false;
                        //oUserDB.cli_deleteDate = DateTime.Now;
                        db.Groups.Remove(oGroupDB);
                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha eliminado el grupo: " + oGroupDB.grp_name + " y sus dependencias de la base de datos";
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el grupo por lo tanto no se puede eliminar";
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
