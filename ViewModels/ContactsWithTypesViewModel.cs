using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class ContactsWithTypesViewModel
    {
        public int id { get; set; }

        public int cnt_id { get; set; }

        public int cnttp_id { get; set; }

        public List<ContactTypeViewModel> types { get; set; }

        public virtual ContactViewModel contact { get; set; }

        public virtual ContactTypeViewModel contactType { get; set; }

    }
}