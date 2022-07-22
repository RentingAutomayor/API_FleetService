using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public class TransactionViewModel
		{
				public Nullable<int> id;
				public Nullable<int> consecutive;
				public string code;
				public MovementViewModel movement;
				public double valueWithoutDiscount;
				public double discountValue;
				public double valueWithDiscountWithoutTaxes;
				public double taxesValue;
				public double value;				
				public ClientViewModel client;
				public TransactionStateViewModel transactionState;
				public TransactionDetailViewModel headerDetails;
				public List<MaintenanceItemViewModel> lsItems;
				public List<TransactionObservationViewModel> lsObservations;
				//TODO change this for model of user
				public int usu_id;
				public UserAccessViewModel user;
				public Nullable<bool> state;
				public Nullable<DateTime> registrationDate;
				public Nullable<DateTime> updateDate;
				public BillWiewModel bill { get; set; }
		}
}