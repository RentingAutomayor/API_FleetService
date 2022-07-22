using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class MaintenanceItemViewModel
		{
				public Nullable<int> id;
				public string code;
				public string name;
				public string description;
				public TypeOfMaintenanceItemViewModel type;
				public PresentationUnitViewModel presentationUnit;
				public List<VehicleTypeViewModel> lsVehicleType;
				public List<VehicleModelViewModel> lsVehicleModel;
				public CategoryViewModel category;				
				public Nullable<float> referencePrice;
				public Nullable<float> valueWithoutDiscount;
				public Nullable<float> discountValue;
				public Nullable<float> valueWithDiscountWithoutTaxes;
				public Nullable<float> taxesValue;
				public Nullable<bool> state;
				public Nullable<float> amount;
				public Nullable<bool> handleTax;
				public List<TaxViewModel> lsTaxes;
				public DealerViewModel dealer;
				public Nullable<DateTime> registrationDate;
				public Nullable<DateTime> updateDate;
				public Nullable<DateTime> deleteDate;
				public Nullable<float> valueDiscount;
		}
}