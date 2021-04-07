using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;
using System.Text.RegularExpressions;

namespace API_FleetService.ViewModels
{
    public class UserLoginViewModel
    {
        public int usr_id;

        public string usr_names;

        public string usr_last_names;

        public int cli_id { get; set; }

        public int cpn_id { get; set; }

        public int deal_id { get; set; }

        public string usr_email { get; set; }

        public string usr_user;

        public string usr_pass;

        public int group;

        public static Users PrepareUserToInsertDB(UserLoginViewModel pUser)
        {

            if (pUser.usr_user == null || pUser.usr_user == "" || UserLoginViewModel.UserExistInDB(pUser.usr_user.Trim()))
            {
                throw new Exception("El usuario que intenta ingresar, ya se encuentra almacenado en la base de datos");
            }

            if (pUser.usr_email == null || pUser.usr_email == "" || UserLoginViewModel.EmailExistInDB(pUser.usr_email.Trim()))
            {
                throw new Exception("El correo que intenta ingresar, ya se encuentra almacenado en la base de datos");
            }

            Users userToInsert = UserLoginViewModel.SetDataToUsers(pUser);

            return userToInsert;
        }

        private static void UsersHasError(UserLoginViewModel pUser)
        {
            if (pUser.usr_names == null || pUser.usr_names.Trim() == "")
            {
                throw new Exception("Los nombres son obligatorios");
            }

            if (pUser.usr_last_names == null || pUser.usr_last_names.Trim() == "")
            {
                throw new Exception("Los apellidos son obligatorios");
            }

            if (pUser.usr_email == null || pUser.usr_email.Trim() == "")
            {
                throw new Exception("El email es obligatorio");
            }
            else
            {
                if (!validateEmail(pUser.usr_email))
                {
                    throw new Exception("El email no es valido");
                }
            }

            if (pUser.usr_user == null || pUser.usr_user.Trim() == "")
            {
                throw new Exception("El nombre del usuario no es válido");
            }

            if (pUser.usr_pass == null || pUser.usr_pass.Trim() == "")
            {
                throw new Exception("El password del usuario no es válido");
            }

            if (pUser.group == 0)
            {
                throw new Exception("El usuario debe pertenecer a un grupo");
            }
        }

        private static Users SetDataToUsers(UserLoginViewModel pUser)
        {

            UserLoginViewModel.UsersHasError(pUser);

            Users usersToInsert = new Users();
            usersToInsert.usr_firstName = pUser.usr_names;
            usersToInsert.usr_lastName = pUser.usr_last_names;
            usersToInsert.usr_name = pUser.usr_user;
            usersToInsert.usr_password = pUser.usr_pass;
            usersToInsert.email = pUser.usr_email;
            usersToInsert.grp_id = pUser.group;
            usersToInsert.activo = true;



            if (pUser.cli_id != 0)
            {
                usersToInsert.cli_id = pUser.cli_id;
            }

            if (pUser.cpn_id != 0)
            {
                usersToInsert.cpn_id = pUser.cpn_id;
            }

            if (pUser.deal_id != 0)
            {
                usersToInsert.deal_id = pUser.deal_id;
            }

            return usersToInsert;
        }

        private static bool UserExistInDB(string pUser_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var userDB = db.Users.Where(us => us.usr_name == pUser_name && us.activo == true).FirstOrDefault();
                    if (userDB != null)
                    {
                        rta = true;
                    }
                    return rta;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool EmailExistInDB(string pEmail_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var userDB = db.Users.Where(us => us.email == pEmail_name && us.activo == true).FirstOrDefault();
                    if (userDB != null)
                    {
                        rta = true;
                    }
                    return rta;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static bool InsertIntoDB(Users user)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool validateEmail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}