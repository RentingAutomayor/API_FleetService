using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
                var responseUser = LoginUser(userAut.user);

                if (responseUser.usr_id == 0)
                {
                    HttpStatusCode codeNotDefined = (HttpStatusCode)429;
                    return Content(codeNotDefined, "Usuario no existe en el sistema");
                }

                if (Decrypt(responseUser.usr_password).Equals(userAut.password)) {
                    var user = new UserAccessViewModel();
                    var access = new List<GroupModuleAction>();

                    using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                    {
                        user = db.Users.Where(usr => usr.usr_name.Equals(userAut.user))
                                            .Select(usr => new UserAccessViewModel
                                            {
                                                id_user = usr.usr_id,
                                                name = usr.usr_firstName,
                                                lastName = usr.usr_lastName,
                                                user = usr.usr_name,
                                                id_group = usr.grp_id,
                                                company = new ViewModels.Company
                                                {
                                                    id = (usr.cpn_id != null) ? usr.cpn_id : ((usr.cli_id != null) ? usr.cli_id : (usr.deal_id != null) ? usr.deal_id : 0),
                                                    type = (usr.cpn_id != null) ? CompanyType.COMPANY : ((usr.cli_id != null) ? CompanyType.CLIENT : (usr.deal_id != null) ? CompanyType.DEALER : 0)
                                                }

                                            }).FirstOrDefault();

                        access = db.GroupModuleAction.Where(gma => gma.grp_id.Equals(user.id_group)).ToList();
                    }

                    user = buildGroup(user);

                    user = buildModulesActions(user, access);

                    return Ok(user);
                }
                else
                {
                    HttpStatusCode codeNotDefined = (HttpStatusCode)429;
                    return Content(codeNotDefined, "Contraseña incorrecta");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult ChangePassword(UserLoginViewModel userAut)
        {
            ResponseApiViewModel rta = new ResponseApiViewModel();
            var responseUser = LoginUser(userAut.user);
            
            if (responseUser.usr_name.Trim() == "")
            {
                throw new Exception("El name del usuario no es válido");
            }

            if (userAut.password.Trim() == "")
            {
                throw new Exception("El password del usuario no es válido");
            }

            var newpassword = Encrypt(userAut.password);

            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
                
                var oUserDB = db.Users.Where(us => us.usr_name == responseUser.usr_name).FirstOrDefault();
                if (oUserDB != null)
                {
                    oUserDB.usr_name = responseUser.usr_name;
                    oUserDB.usr_password = newpassword;
                    oUserDB.grp_id = responseUser.grp_id;

                    db.SaveChanges();
                    rta.response = true;
                    rta.message = "Se ha cambiado la contraseña al usuario: " + responseUser.usr_name;
                    return Ok(rta);
                }
                else
                {
                    rta.response = false;
                    rta.message = "No se encontró el usuario: " + responseUser.usr_name + " en la base de datos, por favor rectifique los datos";
                    return BadRequest(rta.message);
                }
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
                        name = usr.usr_name,
                        lastName = usr.usr_lastName,
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

        #region Metodos

        public Users LoginUser(string user)
        {
            Users dataUser = new Users();
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
                dataUser = db.Users.Where(x => x.usr_name.Equals(user)).FirstOrDefault();
            }
            return dataUser;
        }

        public static string Encrypt(string encryptString)
        {
            string EncryptionKey = "Renting2022@";  //we can change the code converstion key as per our requirement    
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "Renting2022@";  //we can change the code converstion key as per our requirement, but the decryption key should be same as encryption key    
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

    }
}