using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class MaintenanceItemController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetPresentationUnits()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsPresentationUnits = db.PresentationUnit
                        .Where(pu => pu.pu_state == true)
                        .Select(pu => new PresentationUnitViewModel
                        {
                            id = pu.pu_id,
                            shortName = pu.pu_shortName,
                            longName = pu.pu_longName
                        }).ToList();

                    return Ok(lsPresentationUnits);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetCategories()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsCategories = db.MaintenanceItemCategory
                        .Where(ct => ct.mict_state == true)
                        .Select(ct => new CategoryViewModel
                        {
                            id = ct.mict_id,
                            name = ct.mict_name,
                            sate = ct.mict_state,
                            registrationDate = ct.mict_registrationDate
                        }).ToList();

                    return Ok(lsCategories);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetTypeOfMaintenanceItem()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsType = db.TypeOfMaintenanceItem
                        .Where(tp => tp.tmi_state == true)
                        .Select(tp => new TypeOfMaintenanceItemViewModel
                        {
                            id = tp.tmi_id,
                            name = tp.tmi_name,
                        }).ToList();
                    return Ok(lsType);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(int dealer_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsMaintenanceItems = MaintenanceItemController.getMaintenanceItems(dealer_id);
                    return Ok(lsMaintenanceItems);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(MaintenanceItemViewModel item)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();

                    var itemExists = MaintenanceItemController.isItemHostedIntoDb(item.code);

                    if (itemExists)
                    {
                        throw new Exception("No se puede guardar el item de mantenimiento puesto que el código: " + item.code + " Ya se encuentra almacenado en la base de datos");
                    }

                    if (MaintenanceItemController.validateDataMaintenanceItem(item))
                    {
                        MaintenanceItem itemDB = new MaintenanceItem();
                        MaintenanceItemController.setDataToItemDB(item, ref itemDB, true);

                        if (MaintenanceItemController.InsertIntoDB(itemDB))
                        {
                            var itemIserted = MaintenanceItemController.getLastMaintenanceItemInserted();

                            MaintenanceItemController.configureItemByVehicleTypeAndVehicleModel(itemIserted.mi_id, item);                           

                            if (item.handleTax == true)
                            {
                                MaintenanceItemController.configureTaxesByItemId(itemIserted.mi_id, item.lsTaxes, true);
                            }                         
                        }                     
                    }
                    rta.response = true;
                    rta.message = "El artículo de mantenimiento " + item.code + " fue almacenado correctamente en la base de datos";
                    return Ok(rta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        public IHttpActionResult Update(MaintenanceItemViewModel item)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    if (MaintenanceItemController.validateDataMaintenanceItem(item)) {
                        var itemDB = db.MaintenanceItem.Where(it => it.mi_id == item.id).FirstOrDefault();
                        MaintenanceItemController.setDataToItemDB(item, ref itemDB, false);
                        db.SaveChanges();
                        MaintenanceItemController.configureItemByVehicleTypeAndVehicleModel(itemDB.mi_id, item);

                        if (item.handleTax == true)
                        {
                            MaintenanceItemController.configureTaxesByItemId(itemDB.mi_id, item.lsTaxes, false);
                        }
                    }              
                   
                    rta.response = true;
                    rta.message = "Se ha actualizado el artículo de mantenimiento: " + item.name + " de la base de datos";
                    return Ok(rta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int maintenanceItemId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    var oItemDB = db.MaintenanceItem.Where(it => it.mi_id == maintenanceItemId).FirstOrDefault();
                    oItemDB.mi_state = false;
                    oItemDB.mi_deleteDate = DateTime.Now;
                    db.SaveChanges();
                    MaintenanceItemController.DeleteMaintenanceItemOfVehicleTypesAndModels(maintenanceItemId);
                    rta.response = true;
                    rta.message = "Se ha eliminado el artículo de mantenimiento de la base de datos";
                    return Ok(rta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult GetById(int itemId)
        {
            try
            {          
                var maintenanceItem = MaintenanceItemController.getMaintenanceItemById(itemId);
                return Ok(maintenanceItem);               
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetByTypeId(int typeId)
        {
            try
            {
                var maintenanceItem = MaintenanceItemController.getMaintenanceItemByTypeId(typeId);
                return Ok(maintenanceItem);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult GetItemsByVehicleModel(int vehicleModel_id = 0)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var oVehicleModel = db.VehicleModel.Where(vm => vm.vm_id == vehicleModel_id)
                        .Select(vm => new VehicleModelViewModel
                        {
                            id = vm.vm_id,
                            shortName = vm.vm_shortName,
                            longName = vm.vm_longName,
                            state = vm.vm_state,
                            registrationDate = vm.vm_registrationDate,
                            brand = new BrandViewModel { id = vm.vb_id, name = vm.VehicleBrand.vb_name },
                            type = new VehicleTypeViewModel { id = vm.vt_id, name = vm.VehicleType.vt_name }
                        }).FirstOrDefault();

                    var lsItemsConfiguratedByVehicleModel = db.MaintenanceItemsByVehicleModels
                        .Where(vm => vm.vm_id == vehicleModel_id && vm.MaintenanceItem.mi_state == true)
                        .ToList();

                    var lsItemsConfiguratedByVehicleType = db.MaintenanceItemsByVehicleTypes
                        .Where(vt => vt.vt_id == oVehicleModel.type.id && vt.MaintenanceItem.mi_state == true)
                        .ToList();



                    var lsItemConfigurated = new List<int>();

                    foreach (var item in lsItemsConfiguratedByVehicleType)
                    {
                        lsItemConfigurated.Add((int)item.mi_id);
                    }

                    foreach (var item in lsItemsConfiguratedByVehicleModel)
                    {
                        lsItemConfigurated.Add((int)item.mi_id);
                    }

                    var lsMaintenanceItems = db.MaintenanceItem
                        .Where(mi => mi.mi_state == true && lsItemConfigurated.Any(item => item == mi.mi_id) && (mi.deal_id == null))
                        .ToList();

                    var lsMaintenanceItemsFormated = new List<MaintenanceItemViewModel>();

										foreach (var item in lsMaintenanceItems)
										{
                        var maintenanceItem = MaintenanceItemController.formatData(item);
                        if (maintenanceItem != null) {
                            lsMaintenanceItemsFormated.Add(maintenanceItem);
                        }                       
                    }

                    foreach (var maintenanceItem in lsMaintenanceItemsFormated)
                    {
                        if (maintenanceItem.handleTax == true)
                        {
                            var lsTaxes = MaintenanceItemController.getTaxesByItemId((int)maintenanceItem.id);

                            if (lsTaxes != null)
                            {
                                maintenanceItem.lsTaxes = lsTaxes;
                            }
                        }

                    }

                    return Ok(lsMaintenanceItemsFormated);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetPricesByDealer(int dealer_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    PricesByDealerViewModel lsPricesByItem = new PricesByDealerViewModel();


                    lsPricesByItem.dealer = db.Dealer
                                    .Where(deal => deal.deal_id == dealer_id)
                                    .Select(deal => new DealerViewModel
                                    {
                                        id = deal.deal_id,
                                        name = deal.deal_name
                                    }).FirstOrDefault();

                    var maintenanceItemsByDealer = MaintenanceItemController.getMaintenanceItems(dealer_id);
                    lsPricesByItem.lsMaintenanceItems = maintenanceItemsByDealer;


                    return Ok(lsPricesByItem);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult SetPricesByDealer(PricesByDealerViewModel pricesByDealer)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    MaintenanceItemController.configurePricesByMaintenanceItemAndDealer((int)pricesByDealer.dealer.id, pricesByDealer.lsMaintenanceItems, false);
                    rta.response = true;
                    rta.message = "Se han asociado de manera correcta los precios para el concesionario: " + pricesByDealer.dealer.name;

                    return Ok(rta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetPricesByContract(int contract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var pricesByContract = MaintenanceItemController.getPricesByContractId(contract_id);
                    return Ok(pricesByContract);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetTaxesList()
        {

            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsTaxes = db.Taxes
                        .Where(tx => tx.tax_state == true)
                        .Select(tx => new TaxViewModel
                        {
                            id = tx.tax_id,
                            name = tx.tax_name,
                            description = tx.tax_desccription,
                            percentValue = tx.tax_percentValue,
                            registrationDate = tx.tax_registrationDate
                        }).ToList();

                    return Ok(lsTaxes);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(SettingsViewModel settings)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    settings.rows.ForEach(row =>
                    {
                        var maintenance = db.MaintenanceItem.Where(item => item.mi_code == row.code).FirstOrDefault();
                        if (maintenance is null) {
                            MaintenanceItem maintenanceItem = new MaintenanceItem();
                            maintenanceItem.mi_code = row.code;
                            maintenanceItem.mi_name = row.name;
                            maintenanceItem.mi_referencePrice = row.price;
                            //maintenanceItem.mi_handleTax = true;
                            maintenanceItem.pu_id = (int)row.unitId;
                            maintenanceItem.tmi_id = (int)row.typeId;
                            maintenanceItem.mict_id = (int)row.categoryId;
                            maintenanceItem.mi_state = true;
                            db.MaintenanceItem.Add(maintenanceItem);
                        }
                        else
                        {
                            maintenance.mi_referencePrice = row.price;
                            maintenance.mict_id = (int)row.categoryId;
                            maintenance.pu_id = (int)row.unitId;
                            maintenance.mict_id = (int)row.categoryId;
                            maintenance.mi_name = row.name;
                        }
                        db.SaveChanges();
                    });
                }
                    return Ok();
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private static MaintenanceItemViewModel formatData(MaintenanceItem itemDB) {
						try
						{
                var maintenanceItem = new MaintenanceItemViewModel
                {
                    id = itemDB.mi_id,
                    code = itemDB.mi_code,
                    name = itemDB.mi_name,
                    description = itemDB.mi_description,
                    type = new TypeOfMaintenanceItemViewModel
                    {
                        id = itemDB.tmi_id,
                        name = itemDB.TypeOfMaintenanceItem.tmi_name
                    },
                    presentationUnit = 
                    (itemDB.PresentationUnit != null)
                    ? 
                        new PresentationUnitViewModel
                        {
                            id = itemDB.pu_id,
                            shortName = itemDB.PresentationUnit.pu_shortName,
                            longName = itemDB.PresentationUnit.pu_longName
                        } 
                    :  null,
                    category = (itemDB.mict_id != null) 
                    ? 
                    new CategoryViewModel
                        {
                            id = itemDB.mict_id,
                            name = itemDB.MaintenanceItemCategory.mict_name
                        }
                    : null,
                    referencePrice = itemDB.mi_referencePrice,
                    state = itemDB.mi_state,
                    handleTax = itemDB.mi_handleTax,
                    registrationDate = itemDB.mi_registrationDate,
                    updateDate = itemDB.mi_updateDate,
                    deleteDate = itemDB.mi_deleteDate
                };

						    using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						    {
                    maintenanceItem.lsVehicleType = db.MaintenanceItemsByVehicleTypes
                                     .Where(mi => mi.mi_id == itemDB.mi_id)
                                     .Select(mi => new VehicleTypeViewModel
                                     {
                                         id = mi.vt_id,
                                         name = mi.VehicleType.vt_name,
                                         state = mi.VehicleType.vt_state,
                                         registrationDate = mi.mivt_registrationDate
                                     }).ToList();

                    maintenanceItem.lsVehicleModel = db.MaintenanceItemsByVehicleModels
                                    .Where(mi => mi.mi_id == itemDB.mi_id)
                                    .Select(mi => new VehicleModelViewModel
                                    {
                                        id = mi.vm_id,
                                        shortName = mi.VehicleModel.vm_shortName,
                                        longName = mi.VehicleModel.vm_longName,
                                        state = mi.VehicleModel.vm_state,
                                        registrationDate = mi.VehicleModel.vm_registrationDate,
                                        brand = new BrandViewModel { id = mi.VehicleModel.vb_id, name = mi.VehicleModel.VehicleBrand.vb_name },
                                        type = new VehicleTypeViewModel { id = mi.VehicleModel.vt_id, name = mi.VehicleModel.VehicleType.vt_name }
                                    }).ToList();
                }          

                return maintenanceItem;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /**
        * Validations to new MaintenanceItem
        */
        private static bool isItemHostedIntoDb(string code)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var item = db.MaintenanceItem.Where(mi => mi.mi_code == code)
                                    .FirstOrDefault();

                    if (item != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static bool validateDataMaintenanceItem(MaintenanceItemViewModel maintenanceItem) {
            if (maintenanceItem.code.Trim() == "")
            {
                throw new Exception("El código de referencia del árticulo de mantenimiento no puede ser vacío");
            }

            if (maintenanceItem.name.Trim() == "")
            {
                throw new Exception("El nombre del árticulo de mantenimiento no puede ser vacío");
            }

            if (maintenanceItem.presentationUnit == null)
            {
                throw new Exception("El árticulo de mantenimiento debe tener asignada una presentación");
            }

            if (maintenanceItem.type == null)
            {
                throw new Exception("El árticulo de mantenimiento debe tener asociado un tipo. Sí es Material o Mano de obra");
            }

            if (maintenanceItem.category == null)
            {
                throw new Exception("El árticulo de mantenimiento debe tener asociado una categoría. Sí es Mandatorio o Consultivo");
            }

            return true;
        }

        private static void setDataToItemDB(MaintenanceItemViewModel maintenanceItem, ref MaintenanceItem itemDB, bool isToInsert) {
            
            itemDB.mi_code = maintenanceItem.code.ToUpper();
            itemDB.mi_name = maintenanceItem.name.ToUpper();
            itemDB.mi_description = (maintenanceItem.description != null) ? maintenanceItem.description.ToUpper() : "";
            itemDB.tmi_id = (int)maintenanceItem.type.id;
            itemDB.pu_id = (int)maintenanceItem.presentationUnit.id;
            itemDB.mict_id = (int)maintenanceItem.category.id;
            itemDB.mi_referencePrice = maintenanceItem.referencePrice;           
            itemDB.mi_handleTax = (bool)maintenanceItem.handleTax;
            itemDB.deal_id = (maintenanceItem.dealer != null) ? maintenanceItem.dealer.id : null;         

            if (isToInsert)
            {
                itemDB.mi_state = true;
                itemDB.mi_registrationDate = DateTime.Now;
            }
            else {
                itemDB.mi_updateDate = DateTime.Now;
            }
                 
        }

        public static bool InsertIntoDB(MaintenanceItem pItem)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
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

        public static MaintenanceItem getLastMaintenanceItemInserted()
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {                
                return db.MaintenanceItem.OrderByDescending(itm => itm.mi_id).FirstOrDefault();
            }
        }

        public static MaintenanceItemViewModel getMaintenanceItemByTypeId(int typeId)
        {

            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var maintenanceItem = db.MaintenanceItem
                         .Where(mi => mi.mi_state == true && mi.tmi_id == typeId)
                         .FirstOrDefault();

                    if (maintenanceItem != null)
                    {
                        var maintenanceItemFormated = MaintenanceItemController.formatData(maintenanceItem);

                        if (maintenanceItemFormated.handleTax == true)
                        {
                            var lsTaxes = MaintenanceItemController.getTaxesByItemId(maintenanceItem.mi_id);

                            if (lsTaxes != null)
                            {
                                maintenanceItemFormated.lsTaxes = lsTaxes;
                            }
                        }
                        return maintenanceItemFormated;
                    }
                    else
                    {
                        return null;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static MaintenanceItemViewModel getMaintenanceItemById(int itemId) {

						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
                    var maintenanceItem = db.MaintenanceItem
                         .Where(mi => mi.mi_state == true && mi.mi_id == itemId)
                         .FirstOrDefault();

                    if (maintenanceItem != null)
                    {
                        var maintenanceItemFormated = MaintenanceItemController.formatData(maintenanceItem);

                        if (maintenanceItemFormated.handleTax == true)
                        {
                            var lsTaxes = MaintenanceItemController.getTaxesByItemId(itemId);

                            if (lsTaxes != null)
                            {
                                maintenanceItemFormated.lsTaxes = lsTaxes;
                            }
                        }
                        return maintenanceItemFormated;
                    }
                    else {
                        return null;
                    }                  

                }
               
            }
						catch (Exception ex)
						{

								throw ex;
						}
           
        }


        public static List<MaintenanceItemViewModel> getMaintenanceItems(int dealerId)
        {
            try
            {
                List<MaintenanceItemViewModel> lsMaintenanceItems = new List<MaintenanceItemViewModel>();
                List<MaintenanceItemViewModel> lsItemsByDealer = new List<MaintenanceItemViewModel>();
                //When we send 0 like paramter those are the items created by renting automayor
                lsMaintenanceItems = MaintenanceItemController.getMaintenanceItemsByDealerId(0);

                if (dealerId > 0)
                {
                    //Get Only the maintenance items configured by the dealers
                    lsItemsByDealer = MaintenanceItemController.getMaintenanceItemsByDealerId(dealerId);
                }

                if (lsItemsByDealer.Count > 0)
                {
                    lsMaintenanceItems.AddRange(lsItemsByDealer);
                }

                lsMaintenanceItems = MaintenanceItemController.setTaxesToMaintanceItems(lsMaintenanceItems);

                if (dealerId > 0)
                {
                    //Just configure the prices if there is a dealer
                    lsMaintenanceItems = MaintenanceItemController.setPricesByItemAndDealer(lsMaintenanceItems, dealerId);
                }

                return lsMaintenanceItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static List<MaintenanceItemViewModel> getMaintenanceItemsByDealerId(int dealerId)
        {
            try
            {
                int? dealer_id;

                if (dealerId == 0)
                {
                    dealer_id = null;
                }
                else
                {
                    dealer_id = dealerId;
                }

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsMaintenanceItems = db.MaintenanceItem
                        .Where(mi => mi.mi_state == true && mi.deal_id == dealer_id)
                        .ToList();

                    var lsMaintenanceItemsFormated = new List<MaintenanceItemViewModel>();

										foreach (var item in lsMaintenanceItems)
										{
                        lsMaintenanceItemsFormated.Add(MaintenanceItemController.formatData(item));

                    }

                    return lsMaintenanceItemsFormated;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static List<MaintenanceItemViewModel> setTaxesToMaintanceItems(List<MaintenanceItemViewModel> lsMaintenanceItems)
        {
            try
            {
                foreach (var maintenanceItem in lsMaintenanceItems)
                {
                    if (maintenanceItem.handleTax == true)
                    {
                        var lsTaxes = MaintenanceItemController.getTaxesByItemId((int)maintenanceItem.id);
                        if (lsTaxes != null)
                        {
                            maintenanceItem.lsTaxes = lsTaxes;
                        }
                    }
                }

                return lsMaintenanceItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<MaintenanceItemViewModel> setPricesByItemAndDealer(List<MaintenanceItemViewModel> maintenanceItems, int dealerId)
        {
            try
            {
                foreach (var maintenanceItem in maintenanceItems)
                {
                    maintenanceItem.referencePrice = MaintenanceItemController.getPriceByMaintenanceItemAndDealer((int)maintenanceItem.id, dealerId);
                }

                return maintenanceItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static float getPriceByMaintenanceItemAndDealer(int maintenanceItemId, int dealerId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var priceByItem = db.PricesByDealer
                                    .Where(mi => mi.deal_id == dealerId && mi.mi_id == maintenanceItemId)
                                    .Select(mi => mi.mi_referencePrice)
                                    .FirstOrDefault();

                    float? priceToReturn = 0;

                    if (priceByItem != null)
                    {
                        priceToReturn = priceByItem;
                    }
                    else
                    {
                        var referencePrice = db.MaintenanceItem
                                        .Where(mi => mi.mi_id == maintenanceItemId)
                                        .Select(mi => mi.mi_referencePrice)
                                        .FirstOrDefault();

                        priceToReturn = referencePrice;
                    }

                    return (float)priceToReturn;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
        private static void setDataPrice(int dealerId, int itemId, float referencePrice, bool isToInsert, ref PricesByDealer priceByDealer)
        {
            priceByDealer.deal_id = dealerId;
            priceByDealer.mi_id = itemId;
            priceByDealer.mi_referencePrice = referencePrice;

            if (isToInsert)
            {
                priceByDealer.pbd_registrationDate = DateTime.Now;
            }
            else
            {
                priceByDealer.pbd_updateDate = DateTime.Now;
            }
        }

        public static void createPriceByMaintenanceItemAndDealer(int itemId, float price, int dealerId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    PricesByDealer priceByDealer = new PricesByDealer();
                    MaintenanceItemController.setDataPrice(dealerId, itemId, (float)price, true, ref priceByDealer);
                    db.PricesByDealer.Add(priceByDealer);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void updatePriceByMaintenanceItemAndDealer(int itemId, float price, int dealerId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var priceByDealer = db.PricesByDealer.
                                    Where(mi => mi.mi_id == itemId)
                                    .FirstOrDefault();

                    MaintenanceItemController.setDataPrice((int)dealerId, itemId, (float)price, false, ref priceByDealer);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResponseApiViewModel SetPricesByContract(PricesByContractViewModel pricesByContract)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    string transactionType = "";
                    int contract_id = (int)pricesByContract.contract.id;
                    var lsOldPrices = db.PricesByContract.Where(pbc => pbc.cntr_id == pricesByContract.contract.id).ToList();

                    IEnumerable<MaintenanceItemViewModel> lsNewItems = from lsItems in pricesByContract.lsMaintenanceItems

                                                                       where !lsOldPrices.Any(itm => itm.mi_id == lsItems.id)

                                                                       select lsItems;

                    if (lsOldPrices.Count > 0)
                    {
                        foreach (var oldPrice in lsOldPrices)
                        {
                            var newPrice = pricesByContract.lsMaintenanceItems.Find(it => it.id == oldPrice.mi_id);

                            if (oldPrice.mi_referencePrice != newPrice.referencePrice)
                            {
                                transactionType = "UPDATE";
                                PricesByContract priceByContract = db.PricesByContract.Where(pbc => pbc.pbc_id == oldPrice.pbc_id).FirstOrDefault();
                                this.setDataPricesByContract(contract_id, (int)oldPrice.mi_id, (float)newPrice.referencePrice, transactionType, ref priceByContract);
                                db.SaveChanges();
                            }

                            foreach (var item in lsNewItems)
                            {
                                PricesByContract priceByContract = new PricesByContract();
                                this.setDataPricesByContract((int)pricesByContract.contract.id, (int)item.id, (float)item.referencePrice, "INSERT", ref priceByContract);
                                db.PricesByContract.Add(priceByContract);
                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        transactionType = "INSERT";
                        foreach (var item in pricesByContract.lsMaintenanceItems)
                        {
                            PricesByContract priceByContract = new PricesByContract();
                            this.setDataPricesByContract(contract_id, (int)item.id, (float)item.referencePrice, transactionType, ref priceByContract);
                            db.PricesByContract.Add(priceByContract);
                            db.SaveChanges();
                        }

                    }

                    rta.response = true;
                    rta.message = "Se han asignado los precios del contrato: " + pricesByContract.contract.code;
                    return rta;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void setDataPricesByContract(int contract_id, int maintenanceItem_id, float referencePrice, string transacctionType, ref PricesByContract priceByContract)
        {

            priceByContract.cntr_id = contract_id;
            priceByContract.mi_id = maintenanceItem_id;
            priceByContract.mi_referencePrice = referencePrice;

            if (transacctionType == "INSERT")
            {
                priceByContract.pbc_registrationDate = DateTime.Now;
            }

            if (transacctionType == "UPDATE")
            {
                priceByContract.pbc_updateDate = DateTime.Now;
            }
        }

       

        public static void configurePricesByMaintenanceItemAndDealer(int dealerId, List<MaintenanceItemViewModel> maintenanceItems, bool isToInsert)
        {

            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    if (isToInsert)
                    {
                        foreach (var item in maintenanceItems)
                        {
                            MaintenanceItemController.createPriceByMaintenanceItemAndDealer((int)item.id, (float)item.referencePrice, dealerId);
                        }
                    }
                    else
                    {
                        var lsOldPrices = db.PricesByDealer
                                        .Where(pbd => pbd.deal_id == dealerId).ToList();

                        IEnumerable<MaintenanceItemViewModel> lsNewItems = from lsItems in maintenanceItems

                                                                           where !lsOldPrices.Any(itm => itm.mi_id == lsItems.id)

                                                                           select lsItems;

                        if (lsOldPrices.Count > 0)
                        {
                            foreach (var oldPrice in lsOldPrices)
                            {
                                var newPrice = maintenanceItems.Find(mi => mi.id == oldPrice.mi_id && mi.state == true);
                                if (newPrice != null)
                                {
                                    if (oldPrice.mi_referencePrice != newPrice.referencePrice)
                                    {
                                        MaintenanceItemController.updatePriceByMaintenanceItemAndDealer((int)oldPrice.mi_id, (float)newPrice.referencePrice, dealerId);
                                    }
                                }
                            }

                            foreach (var item in lsNewItems)
                            {
                                MaintenanceItemController.createPriceByMaintenanceItemAndDealer((int)item.id, (float)item.referencePrice, dealerId);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<TaxViewModel> getTaxesByItemId(int itemId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var taxes = db.TaxesByMaintenanceItem
                        .Where(tx => tx.mi_id == itemId)
                        .Select(tx => new TaxViewModel
                        {
                            id = tx.tax_id,
                            name = tx.Taxes.tax_name,
                            description = tx.Taxes.tax_desccription,
                            percentValue = tx.Taxes.tax_percentValue,
                            registrationDate = tx.Taxes.tax_registrationDate
                        }).ToList();

                    return taxes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void configureTaxesByItemId(int itemID, List<TaxViewModel> taxes, bool isToInsert) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
                    if (!isToInsert) {
                        MaintenanceItemController.deleteTaxesByItem(itemID);
                    }

                    foreach (var tax in taxes)
                    {
                        TaxesByMaintenanceItem txmi = new TaxesByMaintenanceItem();
                        txmi.mi_id = itemID;
                        txmi.tax_id = (int)tax.id;
                        txmi.txmi_registrationDate = DateTime.Now;                 

                        db.TaxesByMaintenanceItem.Add(txmi);
                        db.SaveChanges();
                    }                    
                }
						}
						catch (Exception ex)
						{
								throw ex;
						}
        }

        private static void deleteTaxesByItem(int item_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsTaxes = db.TaxesByMaintenanceItem.Where(tx => tx.mi_id == item_id).ToList();
                    db.TaxesByMaintenanceItem.RemoveRange(lsTaxes);
                    db.SaveChanges();                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static bool InsertMaintenanceItemByVehicleType(int itemId, List<VehicleTypeViewModel> lsVehicleTypes)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
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

                throw ex;
            }

        }

        public static bool InsertMaintenanceItemByVehicleModel(int itemId, List<VehicleModelViewModel> lsVehicleModels)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    foreach (var model in lsVehicleModels)
                    {
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
                throw ex;
            }
        }

        public static bool DeleteMaintenanceItemOfVehicleTypesAndModels(int itemId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsItemByType = db.MaintenanceItemsByVehicleTypes.Where(it => it.mi_id == itemId).ToList();

                    if (lsItemByType.Count > 0)
                    {
                        db.MaintenanceItemsByVehicleTypes.RemoveRange(lsItemByType);
                        db.SaveChanges();
                    }

                    var lsItemByModel = db.MaintenanceItemsByVehicleModels.Where(it => it.mi_id == itemId).ToList();

                    if (lsItemByModel.Count > 0)
                    {
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

        public static void configureItemByVehicleTypeAndVehicleModel(int itemId,MaintenanceItemViewModel item) {

            MaintenanceItemController.DeleteMaintenanceItemOfVehicleTypesAndModels(itemId);

            if (item.lsVehicleType.Count > 0)
            {
                MaintenanceItemController.InsertMaintenanceItemByVehicleType(itemId, item.lsVehicleType);
            }

            if (item.lsVehicleModel.Count > 0)
            {
                MaintenanceItemController.InsertMaintenanceItemByVehicleModel(itemId, item.lsVehicleModel);
            }
        }

        public static PricesByContractViewModel getPricesByContractId(int contractId) {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var priceByContract = new PricesByContractViewModel();
                    var contract = ContractController.getContractById(contractId);

                    if (contract != null)
                    {
                        var maintenanceItemsConfigurated = new List<MaintenanceItemViewModel>();

                        var pricesConfigurated = db.PricesByContract
                            .Where(ct => ct.cntr_id == contractId)
                            .Select(pbc => new { pbc.mi_id, pbc.mi_referencePrice })
                            .AsEnumerable()
                            .Select(c => (itemId: c.mi_id, itemPrice: c.mi_referencePrice))
                            .ToList();

                        foreach (var priceByItem in pricesConfigurated)
                        {
                            var maintenanceItem = MaintenanceItemController.getMaintenanceItemById((int)priceByItem.itemId);
                            if (maintenanceItem != null)
                            {
                                maintenanceItem.referencePrice = priceByItem.itemPrice;
                                maintenanceItemsConfigurated.Add(maintenanceItem);
                            }

                        }

                        priceByContract.contract = contract;
                        priceByContract.lsMaintenanceItems = maintenanceItemsConfigurated;
                        return priceByContract;
                    }
                    else {
                        throw new Exception("El contrato al cual deseas acceder no se encuentra creado o no se encuentra disponible.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }



}