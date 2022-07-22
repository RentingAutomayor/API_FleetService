using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class RoleController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Save(RoleViewModel roleViewModel)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var isRepeat = db.Groups.Where(role => role.grp_name == roleViewModel.name).Count();
                    if(isRepeat > 0)
                    {
                        string repeatText = "Ya existe un rol con ese nombre. Intente con otro";
                        return BadRequest(new ResponseApiViewModel().Message = repeatText);
                    }
                    Groups groups = new Groups();
                    groups.grp_name = roleViewModel.name;
                    groups.grp_description = roleViewModel.description;
                    int roleId = db.Groups.Add(groups).grp_id;
                    roleViewModel.modulesIds.ForEach(moduleId =>
                    {
                        GroupModuleAction action = new GroupModuleAction();
                        action.grp_id = roleId;
                        action.mdl_id = moduleId;
                        action.act_id = 1;
                        db.GroupModuleAction.Add(action);
                    });
                    db.SaveChanges();
                    return Ok();
                } 
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(RoleViewModel roleViewModel)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var role = db.Groups.Where(rol => rol.grp_id == roleViewModel.id).FirstOrDefault();
                    if(role is null)
                    {
                        string notFound = "El rol que desea editar no existe. Intente con otro";
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    }
                    List<GroupModuleAction> actions = db.GroupModuleAction
                        .Where(action => action.grp_id == roleViewModel.id).ToList();
                    actions.ForEach(action => db.GroupModuleAction.Remove(action));
                    role.grp_name = roleViewModel.name;
                    role.grp_description = roleViewModel.description;
                    roleViewModel.modulesIds.ForEach(action =>
                    {
                        GroupModuleAction groupModuleAction = new GroupModuleAction();
                        groupModuleAction.act_id = 1;
                        groupModuleAction.mdl_id = action;
                        groupModuleAction.grp_id = roleViewModel.id;    
                        db.GroupModuleAction.Add(groupModuleAction);
                    });
                    db.SaveChanges();
                    return Ok();
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var roles = db.Groups.Select(role => new RoleViewModel() {
                        id = role.grp_id,
                        name = role.grp_name,
                        description = role.grp_description,
                        modulesIds = role.GroupModuleAction.Select(action => action.mdl_id)
                            .Distinct().ToList()
                    }).ToList();
                    return Ok(roles);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                List<GroupModuleAction> actions = new List<GroupModuleAction>();
                RoleViewModel roleViewModel = new RoleViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var role = db.Groups.Find(id);
                    if(role is null)
                    {
                        string notFound = "El rol que desea buscar no se encuentra. Intente con otro";
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    }
                    actions = db.GroupModuleAction
                        .Where(action => action.grp_id == id).ToList();
                    roleViewModel.id = role.grp_id;
                    roleViewModel.name = role.grp_name;
                    roleViewModel.description = role.grp_description;
                    roleViewModel.modulesIds = actions.Select(action => action.mdl_id)
                        .Distinct().ToList();
                }
                return Ok(roleViewModel);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetModuleByRole(int id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    string sql = "SELECT Distinct M.* FROM GroupModuleAction G INNER JOIN Modules M ON M.mdl_id = G.mdl_id WHERE G.grp_id = " + id;
                    var modules = db.Modules.SqlQuery(sql)
                        .Select(module => new ModuleViewModel()
                        {
                            id = module.mdl_id,
                            name = module.mdl_name
                        }).ToList();
                    return Ok(modules);
                }
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteById(int id)
        {
            try
            {
               using(DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var usingRole = db.Users.Where(user => user.grp_id == id).Count();
                    var role = db.Groups.Find(id);
                    if(role is null)
                    {
                        string notFound = "El rol que desea eliminar no existe. Intente con otro.";
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    }else if (usingRole > 0)
                    {
                        string notDelete = "El rol que desea eliminar se encuentra en uso. Intente con otro";
                        return BadRequest(new ResponseApiViewModel().Message = notDelete);
                    }
                    List<GroupModuleAction> actions = db.GroupModuleAction
                        .Where(action => action.grp_id == id).ToList();
                    actions.ForEach(action => db.GroupModuleAction.Remove(action));
                    db.Groups.Remove(role);
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