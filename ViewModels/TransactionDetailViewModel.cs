using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TransactionDetailViewModel
		{
				public Nullable<int> id;
				public VehicleViewModel vehicle;
				public DealerViewModel dealer;
				public BranchViewModel branch;
				//This transaction rela
				public TransactionViewModel relatedTransaction;
				public MaintenanceRoutineViewModel maintenanceRoutine;
				public ContractViewModel contract;
				//TODO: Change this for model of users
				//Maybe this can change because this informations is for transaction
				public Nullable<int> userApprobation;
				public Nullable<int> userReject;
				public Nullable<int> userAnulation;
				public Nullable<DateTime> approbationDate;
				public Nullable<DateTime> rejectDate;
				public Nullable<DateTime> anulationDate;
		}
}