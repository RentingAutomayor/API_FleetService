using System.Collections.Generic;

namespace API_FleetService.ViewModels
{
    public class ReportViewModel : BasicLookup
    {
        public string method { get; set; }
        public string service { get; set; }
    }
}