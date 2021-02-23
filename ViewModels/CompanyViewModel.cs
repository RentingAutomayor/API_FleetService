using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class CompanyViewModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public string nit { get; set; }

        public Nullable<bool> state ;
    }
}