using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
    public class GroupViewModel
    {
        public int id_group { get; set; }
        public string groupName { get; set; }
        public string description { get; set; }
        public List<grpModules> moduleAction { get; set; }

        public static Groups PrepareGroupToInsertDB(GroupViewModel pGroup)
        {

            if (GroupViewModel.GroupExistInDB(pGroup.groupName.Trim()))
            {
                throw new Exception("El grupo que intenta ingresar, ya se encuentra almacenado en la base de datos");
            }

            Groups groupToInsert = GroupViewModel.SetDataToUsers(pGroup);

            return groupToInsert;
        }

        private static void GroupsHasError(GroupViewModel pGroup)
        {
            if (pGroup.groupName.Trim() == "")
            {
                throw new Exception("El nombre del grupo no es válido");
            }
        }

        private static Groups SetDataToUsers(GroupViewModel pGorup)
        {
            GroupViewModel.GroupsHasError(pGorup);

            Groups groupToInsert = new Groups();
            groupToInsert.grp_name = pGorup.groupName;
            groupToInsert.grp_description = pGorup.description;
            groupToInsert.activo = true;

            return groupToInsert;
        }

        private static bool GroupExistInDB(string pGroup_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var groupDB = db.Groups.Where(gr => gr.grp_name == pGroup_name && gr.activo == true).FirstOrDefault();
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

        public static int GroupID(string pGroup_name)
        {
            try
            {
                int rta = 0;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var groupDB = db.Groups.Where(gr => gr.grp_name == pGroup_name && gr.activo == true).FirstOrDefault();
                    if (groupDB != null)
                    {
                        rta = groupDB.grp_id;
                    }
                    return rta;
                }
            }
            catch (Exception ex)
            {
                return 0;
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

        public static bool InsertGroupModuleAction(List<GroupModuleAction> moduleActions)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    
                    db.GroupModuleAction.AddRange(moduleActions);
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

    public class grpModules {
        public int id_module { get; set; }

        public List<grpAction> actions { get; set; }


    }

    public class grpAction
    {
        public int act_id { get; set; }

        public string act_name { get; set; }
    }
}