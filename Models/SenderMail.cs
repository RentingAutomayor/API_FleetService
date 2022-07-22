using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.Models
{
    public class SenderMail
    {
        public string nameMessage { get; set; }
        public List<string> emailReceiver { get; set; }
        public int typemessage { get; set; }
        public string nOrderwork { get; set; }
        public string nDealer { get; set; }


    }
}