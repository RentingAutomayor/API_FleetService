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
        [HttpPost]
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

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    //var lsUser = db.Users.Where(cl => cl.cli_state == true)
                    var lsUser = db.Users.Select(usr => new UserAccessViewModel
                    {
                        id_user = usr.usr_id,
                        user = usr.usr_name,
                        id_group = usr.grp_id
                    }).ToList();

                    return Ok(lsUser);
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
                var oUserDB = new UserAccessViewModel();
                var access = new List<GroupModuleAction>();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    oUserDB = db.Users.Where(us => us.usr_id == pId)
                            .Select(usr => new UserAccessViewModel
                            {
                                id_user = usr.usr_id,
                                user = usr.usr_name,
                                id_group = usr.grp_id
                            }).FirstOrDefault();

                    access = db.GroupModuleAction.Where(gma => gma.grp_id.Equals(oUserDB.id_group)).ToList();

                }

                oUserDB = buildGroup(oUserDB);

                oUserDB = buildModulesActions(oUserDB, access);

                return Ok(oUserDB);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(UserLoginViewModel pUser)
        {
            //TODO: Agregar usuario que hizo la acción
            ResponseApiViewModel rta = new ResponseApiViewModel();
            try
            {
                Users UserToInsert = UserLoginViewModel.PrepareUserToInsertDB(pUser);
                bool UserWasInserted = UserLoginViewModel.InsertIntoDB(UserToInsert);

                if (UserWasInserted)
                {
                    rta.response = true;
                    rta.message = "El usuario " + pUser.user + " fue insertado correctamente en la base de datos.";
                    return Ok(rta);
                }
                else
                {
                    rta.response = false;
                    rta.message = "Ha ocurrido un error intentado insertar el usuario:  " + pUser.user;
                    return BadRequest(rta.message);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(UserLoginViewModel pUser)
        {
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var oUserDB = db.Users.Where(us => us.usr_id == pUser.idUser || us.usr_name == pUser.user)
                                                                    .FirstOrDefault();
                    if (oUserDB != null)
                    {
                        if (pUser.user.Trim() == "")
                        {
                            throw new Exception("El name del usuario no es válido");
                        }

                        if (pUser.password.Trim() == "")
                        {
                            throw new Exception("El password del usuario no es válido");
                        }

                        if (pUser.group == 0)
                        {
                            throw new Exception("El usuario debe pertenecer a un grupo");
                        }

                        oUserDB.usr_name = pUser.user;
                        oUserDB.usr_password = pUser.password;
                        oUserDB.grp_id = pUser.group;

                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha actualizado el usuario: " + pUser.user;
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el usuario: " + pUser.user + " en la base de datos, por favor rectifique los datos";
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
        public IHttpActionResult Delete(UserLoginViewModel pUser)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oUserDB = db.Users.Where(us => us.usr_id == pUser.idUser).FirstOrDefault();
                    if (oUserDB != null)
                    {
                        //oUserDB.cli_document = "";
                        //oUserDB.cli_state = false;
                        //oUserDB.cli_deleteDate = DateTime.Now;
                        db.Users.Remove(oUserDB);
                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha eliminado el usuario: " + oUserDB.usr_name + " de la base de datos";
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el usuario por lo tanto no se puede eliminar";
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