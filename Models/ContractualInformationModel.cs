using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_FleetService.ViewModels;

namespace API_FleetService.Models
{
		public interface IContractualInformationModel
		{
				int id { get; set; }
				int clientId { get; set; }
				string contractCode { get; set; }				
				DateTime? serviceInitDate { get; set; }
				DateTime? serviceEndDate { get; set; }
				DateTime? quotaApprovalDate { get; set; }
				DateTime? quotaEndingDate { get; set; }
				FinancialInformationViewModel quotaDetails { get; set; }
				QuotaTypeViewModel quotaType { get; set; }
				int paymentAgreement { get; set; }
				int adminPercentage { get; set; }

		}
}
