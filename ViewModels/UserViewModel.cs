namespace API_FleetService.ViewModels
{
    public class UserViewModel { 
        public int id { get; set; }
        public string name{ get; set; }
        public string lastName { get; set; }
        public string password{ get; set; }
        public string email { get; set; }
        public CompanyViewModel company { get; set; }
        public int roleId { get; set; }
        public int? clientId { get; set; }
        public int? dealerId { get; set; }
        public string status { get; set; }
    }
}