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
                    var lsPresentationUnits = db.PresentationUnit.Where(pu => pu.pu_state == true)
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
                    var lsCategories = db.MaintenanceItemCategory.Where(ct => ct.mict_state == true)
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
                    var lsType = db.TypeOfMaintenanceItem.Where(tp => tp.tmi_state == true)
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
        public IHttpActionResult Insert(MaintenanceItemViewModel pItem)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();

                    var itemExists = MaintenanceItemController.isItemHostedIntoDb(pItem.code);

                    if (itemExists)
                    {
                        throw new Exception("No se puede guardar el item de mantenimiento puesto que el código: " + pItem.code + " Ya se encuentra almacenado en la base de datos");
                    }

                    var oMaintenanceItem = MaintenanceItemViewModel.setDataToItem(pItem);
                    var ItemWasSaved = MaintenanceItemViewModel.InsertIntoDB(oMaintenanceItem);
                    if (ItemWasSaved)
                    {
                        var itemId = MaintenanceItemViewModel.GetMaintenanceItemId(pItem.code);

                        if (itemId != 0)
                        {
                            var lsVehicleType = pItem.lsVehicleType;
                            if (lsVehicleType.Count > 0)
                            {
                                MaintenanceItemViewModel.InsertMaintenanceItemByVehicleType(itemId, lsVehicleType);
                            }

                            var lsVehicleModel = pItem.lsVehicleModel;
                            if (lsVehicleModel.Count > 0)
                            {
                                MaintenanceItemViewModel.InsertMaintenanceItemByVehicleModel(itemId, lsVehicleModel);
                            }


                            if (pItem.handleTax == true)
                            {
                                foreach (var tax in pItem.lsTaxes)
                                {
                                    TaxesByMaintenanceItem txmi = new TaxesByMaintenanceItem();

                                    txmi.mi_id = (int)itemId;
                                    txmi.tax_id = (int)tax.id;
                                    txmi.txmi_registrationDate = DateTime.Now;

                                    db.TaxesByMaintenanceItem.Add(txmi);
                                    db.SaveChanges();
                                }
                            }

                            if (pItem.dealer != null)
                            {
                                var itemTemp = db.MaintenanceItem.Where(mi => mi.mi_id == itemId).FirstOrDefault();
                                itemTemp.deal_id = pItem.dealer.id;
                                db.SaveChanges();
                            }


                        }


                        rta.response = true;
                        rta.message = "El artículo de mantenimiento " + pItem.code + " fue almacenado correctamente en la base de datos";
                        return Ok(rta);
                    }
                    else
                    {
                        return BadRequest("Sucedio algo en la inserción del artículo de mantenimiento");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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


        [HttpPost]
        public IHttpActionResult Update(MaintenanceItemViewModel pItem)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    var oItemDB = db.MaintenanceItem.Where(it => it.mi_id == pItem.id).FirstOrDefault();
                    oItemDB.mi_code = pItem.code.ToUpper();
                    oItemDB.mi_name = pItem.name.ToUpper();
                    oItemDB.mi_description = (pItem.description != null) ? pItem.description.ToUpper() : "";
                    oItemDB.mi_referencePrice = pItem.referencePrice;
                    oItemDB.mi_handleTax = (bool)pItem.handleTax;

                    if (pItem.type == null)
                    {
                        throw new Exception("No se puede actualizar el artículo debido a que no tiene un tipo definido.");
                    }
                    else
                    {
                        oItemDB.tmi_id = (int)pItem.type.id;
                    }

                    if (pItem.presentationUnit == null)
                    {
                        throw new Exception("No se puede actualizar el artículo debido a que no tiene una presentación definida.");
                    }
                    else
                    {
                        oItemDB.pu_id = (int)pItem.presentationUnit.id;
                    }

                    if (pItem.category != null)
                    {
                        oItemDB.mict_id = (int)pItem.category.id;
                    }

                    if (pItem.dealer != null)
                    {
                        if (oItemDB.deal_id != null)
                        {
                            oItemDB.deal_id = pItem.dealer.id;
                        }
                    }
                    oItemDB.mi_updateDate = DateTime.Now;
                    db.SaveChanges();


                    MaintenanceItemViewModel.DeleteMaintenanceItemOfVehicleTypesAndModels((int)pItem.id);

                    var lsVehicleType = pItem.lsVehicleType;
                    if (lsVehicleType.Count > 0)
                    {
                        MaintenanceItemViewModel.InsertMaintenanceItemByVehicleType((int)pItem.id, lsVehicleType);
                    }

                    var lsVehicleModel = pItem.lsVehicleModel;
                    if (lsVehicleModel.Count > 0)
                    {
                        MaintenanceItemViewModel.InsertMaintenanceItemByVehicleModel((int)pItem.id, lsVehicleModel);
                    }
                    //First remove the old taxes
                    this.deleteTaxesByItem((int)pItem.id);
                    //Update the taxes
                    if (pItem.handleTax == true)
                    {
                        foreach (var tax in pItem.lsTaxes)
                        {
                            TaxesByMaintenanceItem txmi = new TaxesByMaintenanceItem();

                            txmi.mi_id = (int)pItem.id;
                            txmi.tax_id = (int)tax.id;
                            txmi.txmi_registrationDate = DateTime.Now;

                            db.TaxesByMaintenanceItem.Add(txmi);
                            db.SaveChanges();
                        }
                    }

                    rta.response = true;
                    rta.message = "Se ha actualizado el artículo de mantenimiento: " + pItem.name + " de la base de datos";
                    return Ok(rta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult Delete(MaintenanceItemViewModel pItem)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ResponseApiViewModel rta = new ResponseApiViewModel();
                    var oItemDB = db.MaintenanceItem.Where(it => it.mi_id == pItem.id).FirstOrDefault();
                    oItemDB.mi_state = false;
                    oItemDB.mi_deleteDate = DateTime.Now;
                    db.SaveChanges();

                    MaintenanceItemViewModel.DeleteMaintenanceItemOfVehicleTypesAndModels((int)pItem.id);

                    rta.response = true;
                    rta.message = "Se ha eliminado el artículo de mantenimiento: " + pItem.name + " de la base de datos";
                    return Ok(rta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                                    .Select(mi => new MaintenanceItemViewModel
                                    {
                                        id = mi.mi_id,
                                        code = mi.mi_code,
                                        name = mi.mi_name,
                                        description = mi.mi_description,
                                        type = new TypeOfMaintenanceItemViewModel
                                        {
                                            id = mi.tmi_id,
                                            name = mi.TypeOfMaintenanceItem.tmi_name
                                        },
                                        presentationUnit = new PresentationUnitViewModel
                                        {
                                            id = mi.pu_id,
                                            shortName = mi.PresentationUnit.pu_shortName,
                                            longName = mi.PresentationUnit.pu_longName
                                        },
                                        category = (mi.mict_id != null) ? new CategoryViewModel
                                        {
                                            id = mi.mict_id,
                                            name = mi.MaintenanceItemCategory.mict_name
                                        } : null,
                                        referencePrice = mi.mi_referencePrice,
                                        state = mi.mi_state,
                                        handleTax = mi.mi_handleTax,
                                        registrationDate = mi.mi_registrationDate,
                                        updateDate = mi.mi_updateDate,
                                        deleteDate = mi.mi_deleteDate
                                    }).ToList();

                    return lsMaintenanceItems;
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
                        var lsTaxes = MaintenanceItemController.getTaxesByItem((int)maintenanceItem.id);
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

        private static List<TaxViewModel> getTaxesByItem(int itemId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsTaxes = db.TaxesByMaintenanceItem
                                                    .Where(tx => tx.mi_id == itemId)
                                                    .Select(tx => new TaxViewModel
                                                    {
                                                        id = tx.tax_id,
                                                        name = tx.Taxes.tax_name,
                                                        description = tx.Taxes.tax_desccription,
                                                        percentValue = tx.Taxes.tax_percentValue,
                                                        registrationDate = tx.Taxes.tax_registrationDate
                                                    }).ToList();

                    return lsTaxes;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        [HttpGet]
        public IHttpActionResult GetById(int itemId)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var maintenanceItem = db.MaintenanceItem
                                    .Where(mi => mi.mi_state == true && mi.mi_id == itemId)
                                    .Select(mi => new MaintenanceItemViewModel
                                    {
                                        id = mi.mi_id,
                                        code = mi.mi_code,
                                        name = mi.mi_name,
                                        description = mi.mi_description,
                                        type = new TypeOfMaintenanceItemViewModel
                                        {
                                            id = mi.tmi_id,
                                            name = mi.TypeOfMaintenanceItem.tmi_name
                                        },
                                        presentationUnit = new PresentationUnitViewModel
                                        {
                                            id = mi.pu_id,
                                            shortName = mi.PresentationUnit.pu_shortName,
                                            longName = mi.PresentationUnit.pu_longName
                                        },
                                        category = (mi.mict_id != null) ? new CategoryViewModel
                                        {
                                            id = mi.mict_id,
                                            name = mi.MaintenanceItemCategory.mict_name
                                        } : null,
                                        referencePrice = mi.mi_referencePrice,
                                        state = mi.mi_state,
                                        handleTax = mi.mi_handleTax,
                                        registrationDate = mi.mi_registrationDate
                                    }).FirstOrDefault();

                    maintenanceItem.lsVehicleType = db.MaintenanceItemsByVehicleTypes
                                    .Where(mi => mi.mi_id == itemId)
                                    .Select(mi => new VehicleTypeViewModel
                                    {
                                        id = mi.vt_id,
                                        name = mi.VehicleType.vt_name,
                                        state = mi.VehicleType.vt_state,
                                        registrationDate = mi.mivt_registrationDate
                                    }).ToList();

                    maintenanceItem.lsVehicleModel = db.MaintenanceItemsByVehicleModels
                                    .Where(mi => mi.mi_id == itemId)
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

                    if (maintenanceItem.handleTax == true)
                    {
                        var lsTaxes = db.TaxesByMaintenanceItem
                                        .Where(tx => tx.mi_id == maintenanceItem.id)
                                        .Select(tx => new TaxViewModel
                                        {
                                            id = tx.tax_id,
                                            name = tx.Taxes.tax_name,
                                            description = tx.Taxes.tax_desccription,
                                            percentValue = tx.Taxes.tax_percentValue,
                                            registrationDate = tx.Taxes.tax_registrationDate
                                        }).ToList();

                        if (lsTaxes != null)
                        {
                            maintenanceItem.lsTaxes = lsTaxes;
                        }
                    }


                    return Ok(maintenanceItem);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }




        [HttpGet]
        public IHttpActionResult GetItemsByVehicleModel(int pVehicleModel_id = 0)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var oVehicleModel = db.VehicleModel.Where(vm => vm.vm_id == pVehicleModel_id)
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
                        .Where(vm => vm.vm_id == pVehicleModel_id && vm.MaintenanceItem.mi_state == true)
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
                                    .Select(mi => new MaintenanceItemViewModel
                                    {
                                        id = mi.mi_id,
                                        code = mi.mi_code,
                                        name = mi.mi_name,
                                        description = mi.mi_description,
                                        type = new TypeOfMaintenanceItemViewModel
                                        {
                                            id = mi.tmi_id,
                                            name = mi.TypeOfMaintenanceItem.tmi_name
                                        },
                                        presentationUnit = new PresentationUnitViewModel
                                        {
                                            id = mi.pu_id,
                                            shortName = mi.PresentationUnit.pu_shortName,
                                            longName = mi.PresentationUnit.pu_longName
                                        },
                                        category = (mi.mict_id != null) ? new CategoryViewModel
                                        {
                                            id = mi.mict_id,
                                            name = mi.MaintenanceItemCategory.mict_name
                                        } : null,
                                        referencePrice = mi.mi_referencePrice,
                                        state = mi.mi_state,
                                        handleTax = mi.mi_handleTax,
                                        registrationDate = mi.mi_registrationDate,
                                        updateDate = mi.mi_updateDate,
                                        deleteDate = mi.mi_deleteDate
                                    }).ToList();


                    foreach (var maintenanceItem in lsMaintenanceItems)
                    {
                        if (maintenanceItem.handleTax == true)
                        {
                            var lsTaxes = db.TaxesByMaintenanceItem
                                            .Where(tx => tx.mi_id == maintenanceItem.id)
                                            .Select(tx => new TaxViewModel
                                            {
                                                id = tx.tax_id,
                                                name = tx.Taxes.tax_name,
                                                description = tx.Taxes.tax_desccription,
                                                percentValue = tx.Taxes.tax_percentValue,
                                                registrationDate = tx.Taxes.tax_registrationDate
                                            }).ToList();

                            if (lsTaxes != null)
                            {
                                maintenanceItem.lsTaxes = lsTaxes;
                            }
                        }

                    }


                    return Ok(lsMaintenanceItems);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }




        [HttpGet]
        public IHttpActionResult GetPricesByDealer(int pDealer_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    PricesByDealerViewModel lsPricesByItem = new PricesByDealerViewModel();


                    lsPricesByItem.dealer = db.Dealer
                                    .Where(deal => deal.deal_id == pDealer_id)
                                    .Select(deal => new DealerViewModel
                                    {
                                        id = deal.deal_id,
                                        name = deal.deal_name
                                    }).FirstOrDefault();

                    var maintenanceItemsByDealer = MaintenanceItemController.getMaintenanceItems(pDealer_id);
                    lsPricesByItem.lsMaintenanceItems = maintenanceItemsByDealer;


                    return Ok(lsPricesByItem);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

        public static void createPriceByMainrenanceItemAndDealer(int itemId, float price, int dealerId)
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


        [HttpGet]
        public IHttpActionResult GetPricesByContract(int pContract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var priceByContract = new PricesByContractViewModel();

                    priceByContract.contract = db.Contract.Where(ct => ct.cntr_id == pContract_id)
                    .Select(ct => new ContractViewModel
                    {
                        id = ct.cntr_id,
                        consecutive = ct.cntr_consecutive,
                        code = ct.cntr_code,
                        name = ct.cntr_name,
                        observation = ct.cntr_observation,
                        dealer = new DealerViewModel
                        {
                            id = ct.deal_id,
                            document = ct.Dealer.deal_document,
                            name = ct.Dealer.deal_name
                        },
                        client = new ClientViewModel
                        {
                            id = ct.cli_id,
                            document = ct.Client.cli_document,
                            name = ct.Client.cli_name
                        },
                        contractState = new ContractStateViewModel
                        {
                            id = ct.cntrst_id,
                            name = ct.ContractState.cntrst_name,
                            description = ct.ContractState.cntrst_description
                        },
                        discountType = new DiscountTypeViewModel
                        {
                            id = ct.dst_id,
                            name = ct.DiscountType.dst_name
                        },
                        discountValue = ct.cntr_discountValue,
                        amountOfMaintenances = ct.cntr_amountOfMaintenances,
                        amountVehicles = ct.cntr_amountVehicles,
                        startingDate = ct.cntr_startingDate,
                        endingDate = ct.cntr_endingDate,
                        duration = ct.cntr_duration,
                        registrationDate = ct.cntr_registrationDate
                    }
                    ).FirstOrDefault();

                    priceByContract.lsMaintenanceItems = db.PricesByContract.Where(ct => ct.cntr_id == pContract_id)
                    .Select(
                    pbc => new MaintenanceItemViewModel
                    {
                        id = pbc.mi_id,
                        code = pbc.MaintenanceItem.mi_code,
                        name = pbc.MaintenanceItem.mi_name,
                        description = pbc.MaintenanceItem.mi_description,
                        type = new TypeOfMaintenanceItemViewModel
                        {
                            id = pbc.MaintenanceItem.tmi_id,
                            name = pbc.MaintenanceItem.TypeOfMaintenanceItem.tmi_name
                        },
                        presentationUnit = new PresentationUnitViewModel
                        {
                            id = pbc.MaintenanceItem.pu_id,
                            shortName = pbc.MaintenanceItem.PresentationUnit.pu_shortName,
                            longName = pbc.MaintenanceItem.PresentationUnit.pu_longName
                        },
                        category = (pbc.MaintenanceItem.mict_id != null) ? new CategoryViewModel
                        {
                            id = pbc.MaintenanceItem.mict_id,
                            name = pbc.MaintenanceItem.MaintenanceItemCategory.mict_name
                        } : null,
                        referencePrice = pbc.mi_referencePrice,
                        state = pbc.MaintenanceItem.mi_state,
                        handleTax = pbc.MaintenanceItem.mi_handleTax

                    }
                    ).ToList();

                    foreach (var maintenanceItem in priceByContract.lsMaintenanceItems)
                    {
                        if (maintenanceItem.handleTax == true)
                        {
                            var lsTaxes = db.TaxesByMaintenanceItem.Where(tx => tx.mi_id == maintenanceItem.id)
                                .Select(tx => new TaxViewModel
                                {
                                    id = tx.tax_id,
                                    name = tx.Taxes.tax_name,
                                    description = tx.Taxes.tax_desccription,
                                    percentValue = tx.Taxes.tax_percentValue,
                                    registrationDate = tx.Taxes.tax_registrationDate
                                }).ToList();

                            if (lsTaxes != null)
                            {
                                maintenanceItem.lsTaxes = lsTaxes;
                            }
                        }

                    }

                    return Ok(priceByContract);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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


        [HttpGet]
        public IHttpActionResult GetTaxesList()
        {

            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsTaxes = db.Taxes.Where(tx => tx.tax_state == true)
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


        private bool deleteTaxesByItem(int item_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsTaxes = db.TaxesByMaintenanceItem.Where(tx => tx.mi_id == item_id).ToList();
                    db.TaxesByMaintenanceItem.RemoveRange(lsTaxes);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {

                return false;
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
                            MaintenanceItemController.createPriceByMainrenanceItemAndDealer((int)item.id, (float)item.referencePrice, dealerId);
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
                                MaintenanceItemController.createPriceByMainrenanceItemAndDealer((int)item.id, (float)item.referencePrice, dealerId);
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



    }



}