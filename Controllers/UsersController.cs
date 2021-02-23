using API_FleetService.ViewModels;
using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Net;
using System.Net.Mail;


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
                    var decodeData = decode(userAut.usr_pass);
                    userAut.usr_pass = decodeData;
                    user = db.Users.Where(usr => usr.usr_name.Equals(userAut.usr_user) && usr.usr_password.Equals(userAut.usr_pass))
                                        .Select(usr => new UserAccessViewModel
                                        {
                                            id_user = usr.usr_id,
                                            user = usr.usr_name,
                                            profile = usr.cli_id != null ? "cli-" + usr.cli_id : usr.deal_id != null ? "deal-" + usr.deal_id : usr.cpn_id != null ? "cpn-" + usr.cpn_id : "0",
                                            id_group = usr.grp_id
                                        }).FirstOrDefault();



                    if (user == null)
                    {
                        var empty = new UserLoginViewModel();
                        return Ok(empty);
                    }

                    access = db.GroupModuleAction.Where(gma => gma.grp_id.Equals(user.id_group)).ToList();

                    if (access == null)
                    {
                        return Ok(user);
                    }
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

        public IHttpActionResult recoverPass(UserLoginViewModel userAut)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oUserDB = db.Users.FirstOrDefault(usr => usr.usr_name == userAut.usr_user);

                    if (oUserDB != null)
                    {
                        var send = sendMail(oUserDB);
                        if (send)
                        {
                            return Ok("ok");
                        }
                        else
                        {
                            return Ok("errorMail");
                        }
                    }
                    else
                    {
                        return Ok("noUser");
                    }

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public bool sendMail(Users user)
        {
            var answer = false;

            // diseño del html
            string body =
                "<body>" +
                "<label>Los datos proporcionados son de indole privado y esta prohibida su difucion.</label>" +
                "<br>" +
                "<br>" +
                "<label>Usuario: " + user.usr_name + "</label>" +
                "<br>" +
                "<label>Contraseña: " + user.usr_password + "</label>" +
                "<br>" +
                "<label>No responder este email.</label>" +
                "</body>";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("recoverPass@flotiAppp.com", "Recover Pass");
            mail.To.Add(new MailAddress(user.email));
            mail.Subject = "Olvide mi contraseña";
            mail.Body = body;
            mail.IsBodyHtml = true;
            //mail.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            // puertos en caso de que no funcione 465 - 587, a mi me funciono el 25
            smtp.Port = 25; 
            // email y contraseña desde donde se va a enviar el correo          
            smtp.Credentials = new NetworkCredential("dubier1992@gmail.com", "********"); 
            smtp.EnableSsl = true;           

            try
            {
                smtp.Send(mail);
                answer = true;
            }
            catch (Exception ex)
            {
                answer = false;
            }
            finally
            {
                smtp.Dispose();
            }

            return answer;
        }

        public static string decode(string pass)
        {
            byte[] encodeDataAsBytes = System.Convert.FromBase64String(pass);
            string passAns = System.Text.ASCIIEncoding.ASCII.GetString(encodeDataAsBytes);
            return passAns;
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
                    var lsUser = db.Users.Include(u => u.Groups)//.ThenInclude(g => g.GroupModuleAction).
                        .Include(c => c.Client)
                        .Include(d => d.Dealer)
                        .Include(e => e.Company).Select(usr => new
                        {
                            usr_id = usr.usr_id,
                            usr_firstName = usr.usr_firstName,
                            usr_lastName = usr.usr_lastName,
                            usr_name = usr.usr_name,
                            usr_password = usr.usr_password,
                            profile = usr.cli_id != null ? "cli-" + usr.cli_id : usr.deal_id != null ? "deal-" + usr.deal_id : usr.cpn_id != null ? "cpn-" + usr.cpn_id : "Admin",
                            group = new
                            {
                                grp_id = usr.grp_id,
                                grp_name = usr.Groups.grp_name
                            }
                        }).ToList();


                    //var lsUser = db.Users.Select(usr => new UserAccessViewModel
                    //{
                    //    id_user = usr.usr_id,
                    //    user = usr.usr_name,
                    //    id_group = usr.grp_id
                    //}).ToList();

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
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oUserDB = db.Users.Where(usr => usr.usr_id == pId)
                        .Include(u => u.Groups)//.ThenInclude(g => g.GroupModuleAction).
                        .Include(c => c.Client)
                        .Include(d => d.Dealer)
                        .Include(e => e.Company).Select(usr => new
                        {
                            usr_id = usr.usr_id,
                            usr_names = usr.usr_firstName,
                            usr_last_names = usr.usr_lastName,
                            usr_user = usr.usr_name,
                            usr_pass = usr.usr_password,
                            usr_email = usr.email,
                            profile = usr.cli_id != null ? "Cliente-" + usr.cli_id : usr.deal_id != null ? "Dealer-" + usr.deal_id : usr.cpn_id != null ? "Compañia-" + usr.cpn_id : "Admin",
                            groupLoad = new
                            {
                                id_group = usr.grp_id,
                                groupName = usr.Groups.grp_name
                            }
                        }).FirstOrDefault();

                    return Ok(oUserDB);
                }



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
                    rta.message = "El usuario " + pUser.usr_user + " fue insertado correctamente en la base de datos.";
                    return Ok(rta);
                }
                else
                {
                    rta.response = false;
                    rta.message = "Ha ocurrido un error intentado insertar el usuario:  " + pUser.usr_user;
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

                    var oUserDB = db.Users.Where(us => us.usr_id == pUser.usr_id || us.usr_name == pUser.usr_user)
                                                                    .FirstOrDefault();
                    if (oUserDB != null)
                    {
                        if (pUser.usr_user.Trim() == "")
                        {
                            throw new Exception("El name del usuario no es válido");
                        }

                        if (pUser.usr_pass.Trim() == "")
                        {
                            throw new Exception("El password del usuario no es válido");
                        }

                        if (pUser.group == 0)
                        {
                            throw new Exception("El usuario debe pertenecer a un grupo");
                        }

                        oUserDB.usr_firstName = pUser.usr_names;
                        oUserDB.usr_lastName = pUser.usr_last_names;
                        oUserDB.email = pUser.usr_email;
                        oUserDB.usr_name = pUser.usr_user;
                        oUserDB.usr_password = pUser.usr_pass;
                        oUserDB.grp_id = pUser.group;

                        if (pUser.cli_id == 0)
                        {
                            oUserDB.cli_id = null;
                        }
                        else
                        {
                            oUserDB.cli_id = pUser.cli_id;
                        }

                        if (pUser.cpn_id == 0)
                        {
                            oUserDB.cpn_id = null;
                        }
                        else
                        {
                            oUserDB.cpn_id = pUser.cpn_id;
                        }

                        if (pUser.deal_id == 0)
                        {
                            oUserDB.deal_id = null;
                        }
                        else
                        {
                            oUserDB.deal_id = pUser.deal_id;
                        }

                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha actualizado el usuario: " + pUser.usr_user;
                        return Ok(rta);
                    }
                    else
                    {
                        rta.response = false;
                        rta.message = "No se encontró el usuario: " + pUser.usr_user + " en la base de datos, por favor rectifique los datos";
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
        public IHttpActionResult Delete(Users pUser)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oUserDB = db.Users.Where(us => us.usr_id == pUser.usr_id).FirstOrDefault();
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
