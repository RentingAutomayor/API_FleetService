using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API_FleetService.ViewModels;

namespace API_FleetService.ViewModels
{
		public class ClientViewModel:PersonViewModel
		{
				public CityViewModel city;
				public List<ContactViewModel> contacts;
				public List<BranchViewModel> branchs;
				public List<VehicleViewModel> vehicles;
				public FinancialInformationViewModel financialInformation;
				public ContractualInformationViewModel contractualInformation;		
		}
}