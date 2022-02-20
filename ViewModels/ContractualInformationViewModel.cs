using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API_FleetService.Models;

namespace API_FleetService.ViewModels
{
		
		public class ContractualInformationViewModel : IContractualInformationModel, IBussinessObject
		{	
				public int id { get; set; }
				public int clientId { get; set; }
				public string contractCode { get; set; }
				public DateTime? serviceInitDate { get; set; }
				public DateTime? serviceEndDate { get; set; }
				public DateTime? quotaApprovalDate { get; set; }
				public DateTime? quotaEndingDate { get; set; }
				public QuotaTypeViewModel quotaType { get; set; }
				public FinancialInformationViewModel quotaDetails { get; set; }				
				public int paymentAgreement { get; set; }
				public int adminPercentage { get; set; }
				public bool state { get; set; }
				public DateTime? registrationDate { get; set; }
				public DateTime? updateDate { get; set; }
				public DateTime? deleteDate { get; set; }

				public UserAccessViewModel user;
				
		}
}