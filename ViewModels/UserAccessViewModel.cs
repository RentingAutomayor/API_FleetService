using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class UserAccessViewModel
    {
        public Nullable<int> id_user { get; set; }
        public string name { get; set; }

        public string lastName { get; set; }
        public string user { get; set; }

        public virtual int id_group { get; set; }

        public Group group { get; set; }

        public Company company { get; set; }

        public int? dealerid { get;set;}
    }

    public class Group
    {
        public int id_group { get; set; }

        public string groupName { get; set; }

        public List<Module> modules { get; set; }
    }

    public class Module
    {
        public int id_module { get; set; }

        public int id_moduleF { get; set; }

        public string moduleName { get; set; }

        public string moduleDescription { get; set; }

        public string path { get; set; }

        public List<ActionModule> actions { get; set; }
    }

    public class ActionModule
    {
        public int id_action { get; set; }

        public string actionName { get; set; }
    }

    public class Company {
        public Nullable<int> id;
        public CompanyType type;
    }

    public enum CompanyType { 
        COMPANY = 1,
        CLIENT = 2,
        DEALER = 3
    }
}