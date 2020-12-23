using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
    public class GroupViewModel
    {
        public int idGroup { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public static Groups PrepareGroupToInsertDB(GroupViewModel pGroup)
        {

            if (GroupViewModel.GroupExistInDB(pGroup.name.Trim()))
            {
                throw new Exception("El grupo que intenta ingresar, ya se encuentra almacenado en la base de datos");
            }

            Groups groupToInsert = GroupViewModel.SetDataToUsers(pGroup);

            return groupToInsert;
        }

        private static void GroupsHasError(GroupViewModel pGroup)
        {
            if (pGroup.name.Trim() == "")
            {
                throw new Exception("El nombre del grupo no es válido");
            }
        }

        private static Groups SetDataToUsers(GroupViewModel pGorup)
        {
            GroupViewModel.GroupsHasError(pGorup);

            Groups groupToInsert = new Groups();
            groupToInsert.grp_name = pGorup.name;
            groupToInsert.grp_description = pGorup.description;

            return groupToInsert;
        }

        private static bool GroupExistInDB(string pGroup_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var groupDB = db.Groups.Where(gr => gr.grp_name == pGroup_name).FirstOrDefault();
                    if (groupDB != null)
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

        public static bool InsertIntoDB(Groups group)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    db.Groups.Add(group);
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