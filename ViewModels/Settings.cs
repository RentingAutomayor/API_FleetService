using System.Collections.Generic;

namespace API_FleetService.ViewModels {
    public class SettingsViewModel { 
        public string domain { set; get; }
        public List<Rows> rows { set; get; }
        public int? brandId { set; get; }
        public int? typeId { set; get; }
    }

    public class Rows { 
        public string code { set; get; }
        public string name { set; get; }
        public float price { set; get; }
        public int? unitId { set; get; }
        public int? typeId { set; get; }
        public int? categoryId { set; get; }

    }
}