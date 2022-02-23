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

				public static MaintenanceItem setDataToItem(MaintenanceItemViewModel pItem) {
						if (pItem.code.Trim() == "") {
								throw new Exception("El código de referencia del árticulo de mantenimiento no puede ser vacío");
						}

						if (pItem.name.Trim() == "") {
								throw new Exception("El nombre del árticulo de mantenimiento no puede ser vacío");
						}

						if (pItem.presentationUnit == null)
						{
								throw new Exception("El árticulo de mantenimiento debe tener asignada una presentación");
						}

						if (pItem.type == null)
						{
								throw new Exception("El árticulo de mantenimiento debe tener asociado un tipo. Sí es Material o Mano de obra");
						}

						if (pItem.category == null) {
								throw new Exception("El árticulo de mantenimiento debe tener asociado una categoría. Sí es Mandatorio o Consultivo");
						}

						MaintenanceItem ItemDB = new MaintenanceItem();

						ItemDB.mi_code = pItem.code.ToUpper();
						ItemDB.mi_name = pItem.name.ToUpper();
						ItemDB.mi_description = (pItem.description != null) ? pItem.description.ToUpper() : "";
						ItemDB.tmi_id = (int) pItem.type.id;				
						ItemDB.pu_id = (int)pItem.presentationUnit.id;						
						
						if (pItem.category != null)
						{
								ItemDB.mict_id = (int)pItem.category.id;
						}
						ItemDB.mi_referencePrice = pItem.referencePrice;
						ItemDB.mi_state = true;
						ItemDB.mi_handleTax = (bool) pItem.handleTax;

						ItemDB.mi_registrationDate = DateTime.Now;										
						return ItemDB;
				}


				public static bool InsertIntoDB(MaintenanceItem pItem) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										db.MaintenanceItem.Add(pItem);
										db.SaveChanges();
										return true;
								}
						}
						catch (Exception ex)
						{
								throw new Exception(ex.InnerException.Message);							
						}
				}

				public static int GetMaintenanceItemId(string code) {
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
								var oItemDB = db.MaintenanceItem.Where(itm => itm.mi_code == code).OrderByDescending(itm => itm.mi_id).FirstOrDefault();
								return oItemDB.mi_id;
						}							
				}

				public static bool InsertMaintenanceItemByVehicleType(int itemId, List<VehicleTypeViewModel> lsVehicleTypes)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										foreach (var vehicleType in lsVehicleTypes)
										{
												MaintenanceItemsByVehicleTypes oItemDB = new MaintenanceItemsByVehicleTypes();
												oItemDB.mi_id = itemId;
												oItemDB.vt_id = vehicleType.id;
												oItemDB.mivt_registrationDate = DateTime.Now;

												db.MaintenanceItemsByVehicleTypes.Add(oItemDB);
												db.SaveChanges();
										}
								}
										
								return true;
						}
						catch (Exception ex)
						{

								throw new Exception(ex.Message);
						}
						
				}


				public static bool InsertMaintenanceItemByVehicleModel(int itemId, List<VehicleModelViewModel> lsVehicleModels) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {

										foreach (var model in lsVehicleModels) {
												MaintenanceItemsByVehicleModels oItemDB = new MaintenanceItemsByVehicleModels();
												oItemDB.mi_id = itemId;
												oItemDB.vm_id = model.id;
												oItemDB.mivm_registrationDate = DateTime.Now;
												
												db.MaintenanceItemsByVehicleModels.Add(oItemDB);
												db.SaveChanges();
										}
								}
								return true;
						}
						catch (Exception ex)
						{
								throw new Exception(ex.Message);
						}
				}


				public static bool DeleteMaintenanceItemOfVehicleTypesAndModels(int itemId) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsItemByType = db.MaintenanceItemsByVehicleTypes.Where(it => it.mi_id == itemId).ToList();

										if (lsItemByType.Count > 0) {
												db.MaintenanceItemsByVehicleTypes.RemoveRange(lsItemByType);
												db.SaveChanges();
										}								

										var lsItemByModel = db.MaintenanceItemsByVehicleModels.Where(it => it.mi_id == itemId).ToList();

										if (lsItemByModel.Count > 0) {
												db.MaintenanceItemsByVehicleModels.RemoveRange(lsItemByModel);
												db.SaveChanges();
										}

								}
								return true;
						}
						catch (Exception ex)
						{

								throw new Exception(ex.Message);
						}
				}

		}
}