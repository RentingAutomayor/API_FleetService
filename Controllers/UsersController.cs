using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetAuthenticate(UserLoginViewModel userAut) //UserLoginViewModel pItem
        {
            try
            {

                var user = new UserAccessViewModel();
                var access = new List<GroupModuleAction>();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    user = db.Users.Where(usr => usr.usr_name.Equals(userAut.user) && usr.usr_password.Equals(userAut.password))
                                        .Select(usr => new UserAccessViewModel
                                        {
                                            id_user = usr.usr_id,
                                            user = usr.usr_name,
                                            id_group = usr.grp_id
                                        }).FirstOrDefault();

                    access = db.GroupModuleAction.Where(gma => gma.grp_id.Equals(user.id_group)).ToList();
                }

                user = buildGroup(user);

                user = buildModulesActions(user, access);

                return Ok(user);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private UserAccessViewModel buildGroup(UserAccessViewModel user)
        {
            try
            {
                var group = new Group();
                user.group = new Group();
                                
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    // group data 
                    group = db.Groups.Where(grp => grp.grp_id == user.id_group)
                                        .Select(grp => new Group
                                        {
                                            id_group = grp.grp_id,
                                            groupName = grp.grp_name
                                        }).FirstOrDefault();

                    user.group = group;   
                }

            }
            catch (Exception ex)
            {
                return new UserAccessViewModel();
            }

            return user;
        }

        private UserAccessViewModel buildModulesActions(UserAccessViewModel user, List<GroupModuleAction> groupMA)
        {
            try
            {
                //var groupModuls = new List<Module>();
                user.group.modules = new List<Module>();

                var groupModuleById = groupMA.GroupBy(g => g.mdl_id);

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    // group data 

                    foreach (var module in groupModuleById)
                    {
                        var moduleItem = db.Modules.Where(mdl => mdl.mdl_id == module.Key)
                                               .Select(mdl => new Module
                                               {
                                                   id_module = mdl.mdl_id,
                                                   moduleName = mdl.mdl_name,
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
                        
                        user.group.modules.Add(moduleItem);
                    }                    
                }

            }
            catch (Exception ex)
            {
                return new UserAccessViewModel();
            }

            return user;
        }

    }
}