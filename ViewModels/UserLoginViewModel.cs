using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
    public class UserLoginViewModel
    {
        public int idUser;

        public string user;

        public string password;

        public int group;

        public static Users PrepareUserToInsertDB(UserLoginViewModel pUser)
        {

            if (UserLoginViewModel.UserExistInDB(pUser.user.Trim()))
            {
                throw new Exception("El usuario que intenta ingresar, ya se encuentra almacenado en la base de datos");
            }

            Users userToInsert = UserLoginViewModel.SetDataToUsers(pUser);

            return userToInsert;
        }

        private static void UsersHasError(UserLoginViewModel pUser)
        {
            if (pUser.user.Trim() == "")
            {
                throw new Exception("El nombre del usuario no es válido");
            }

            if (pUser.password.Trim() == "")
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
            usersToInsert.usr_name = pUser.user;
            usersToInsert.usr_password = pUser.password;
            usersToInsert.grp_id = pUser.group;

            return usersToInsert;
        }

        private static bool UserExistInDB(string pUser_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var userDB = db.Users.Where(us => us.usr_name == pUser_name).FirstOrDefault();
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
    }
}