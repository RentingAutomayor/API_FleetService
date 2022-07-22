using System.Collections.Generic;

namespace API_FleetService.ViewModels
{
    public class RoleViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<int> modulesIds { get; set; }
    }
}