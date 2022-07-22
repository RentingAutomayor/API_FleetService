using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class LogTransactionViewModel
		{
				public Nullable<int> id;
				public TransactionViewModel transaction;
				public FinancialInformationViewModel initValues;
				public FinancialInformationViewModel endValues;
		}
}