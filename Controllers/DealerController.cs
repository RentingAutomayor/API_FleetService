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
    public class DealerController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsDealer = db.Dealer.Where(dlr => dlr.deal_state == true)
                               .Select(dlr => new DealerViewModel
                               {
                                   id = dlr.deal_id,
                                   document = dlr.deal_document,
                                   name = dlr.deal_name,
                                   state = dlr.deal_state,
                                   registrationDate = dlr.deal_registrationDate
                               }).ToList()
                               .OrderByDescending(dlr => dlr.registrationDate);

                    return Ok(lsDealer);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetById(int dealerId)
        {
            try
            {
                var dealer = DealerController.getDealerById(dealerId);
                return Ok(dealer);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetByDocument(string document)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oDealerDB = db.Dealer.Where(dlr => dlr.deal_document == document && dlr.deal_state == true)
                               .Select(dlr => new DealerViewModel
                               {
                                   id = dlr.deal_id,
                                   document = dlr.deal_document,
                                   name = dlr.deal_name,
                                   state = dlr.deal_state,
                                   registrationDate = dlr.deal_registrationDate
                               }).FirstOrDefault();

                    return Ok(oDealerDB);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetByDescription(string description)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsDealerDB = db.Dealer.Where(dlr => dlr.deal_state == true &&
                 (dlr.deal_document.Contains(description) || dlr.deal_name.ToUpper().Contains(description.ToUpper())))
                               .Select(dlr => new DealerViewModel
                               {
                                   id = dlr.deal_id,
                                   document = dlr.deal_document,
                                   name = dlr.deal_name,
                                   state = dlr.deal_state,
                                   registrationDate = dlr.deal_registrationDate
                               }).ToList();

                    return Ok(lsDealerDB);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(DealerViewModel dealer)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                Dealer dealerToInsert = DealerController.PrepareDealerToInsertDB(dealer);
                if (DealerController.InsertIntoDB(dealerToInsert))
                {
                    var dealerInserted = DealerController.getLastDealerInserted();
                    var lsContacts = DealerController.insertContactsByDealer((int)dealerInserted.id, dealer.contacts);
                    var lsBranches = DealerController.insertBranchesByDealer((int)dealerInserted.id, dealer.branches);
                    DealerController.setPricesByItem((int)dealerInserted.id, dealer.maintenanceItems, true);

                    dealerInserted.contacts = lsContacts;
                    dealerInserted.branches = lsBranches;
                    return Ok(dealerInserted);
                }
                else
                {
                    return BadRequest("Sucedió algo en la insersión del concesionario");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(DealerViewModel dealer)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oDealerDB = db.Dealer
                               .Where(dlr => dlr.deal_id == dealer.id && dlr.deal_state == true)
                               .FirstOrDefault();

                    if (oDealerDB != null)
                    {
                        DealerController.updateDealer(dealer);
                        DealerController.updateContactsByDealer((int)dealer.id, dealer.contacts);
                        DealerController.updateBranchesByDealer((int)dealer.id, dealer.branches);
                        DealerController.setPricesByItem((int)dealer.id, dealer.maintenanceItems, false);
                    }

                    return Ok(DealerController.getDealerById((int)dealer.id));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int dealerId)
        {
            //TODO: Agregar usuario que hizo la acción
            try
            {
                ResponseApiViewModel rta = new ResponseApiViewModel();
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oDealerDB = db.Dealer.Where(dlr => dlr.deal_id == dealerId).FirstOrDefault();

                    if (oDealerDB != null)
                    {
                        oDealerDB.deal_document = "";
                        oDealerDB.deal_state = false;
                        oDealerDB.deal_deleteDate = DateTime.Now;
                        db.SaveChanges();
                        rta.response = true;
                        rta.message = "Se ha eliminado el concesionario: " + oDealerDB.deal_name + " de la base de datos";
                    }
                    return Ok(rta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static DealerViewModel getDealerById(int dealerId)
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                var dealer = db.Dealer.Where(dlr => dlr.deal_id == dealerId)
                                           .Select(dlr => new DealerViewModel
                                           {
                                               id = dlr.deal_id,
                                               document = dlr.deal_document,
                                               name = dlr.deal_name,
                                               state = dlr.deal_state,
                                               registrationDate = dlr.deal_registrationDate,
                                               updateDate = dlr.deal_updateDate,
                                           }).FirstOrDefault();

                dealer.contacts = ContactController.getListOfContactsByDealerId(dealerId);
                dealer.branches = BranchController.getListOfBranchsByDealer(dealerId);
                dealer.maintenanceItems = MaintenanceItemController.getMaintenanceItems(dealerId);

                return dealer;
            }

        }

        public static Dealer PrepareDealerToInsertDB(DealerViewModel dealer)
        {
            if (DealerController.dealerExistInDB(dealer.document))
            {
                throw new Exception("El concesionario que intenta ingresar ya se encuentra almacenado en la base de datos");
            }

            if (DealerController.isDealerValid(dealer))
            {
                Dealer dealerDb = new Dealer();
                DealerController.setDataToDealer(dealer, ref dealerDb, true);
                return dealerDb;
            }
            return null;
        }

        public static void updateDealer(DealerViewModel dealer)
        {
            if (DealerController.dealerExistInDBWithDifferentId(dealer.document, (int)dealer.id))
            {
                throw new Exception("Existe otro concesionario con el documento que intenta actualizar");
            }
            else
            {
                if (DealerController.isDealerValid(dealer))
                {

                    using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                    {
                        var dealerDb = db.Dealer
                        .Where(dl => dl.deal_id == dealer.id && dl.deal_state == true)
                        .FirstOrDefault();

                        DealerController.setDataToDealer(dealer, ref dealerDb, false);
                        db.SaveChanges();
                    }


                }
            }
        }

        private static bool dealerExistInDBWithDifferentId(string document, int id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var dealerDB = db.Dealer
                               .Where(dlr => dlr.deal_document == document && dlr.deal_id != id && dlr.deal_state == true)
                               .FirstOrDefault();

                    if (dealerDB != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool dealerExistInDB(string pDealer_document)
        {
            try
            {

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var dealerDB = db.Dealer
                               .Where(dlr => dlr.deal_document == pDealer_document)
                               .FirstOrDefault();
                    if (dealerDB != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void setDataToDealer(DealerViewModel dealer, ref Dealer dealerDB, bool isToInsert)
        {

            dealerDB.deal_document = dealer.document;
            dealerDB.deal_name = dealer.name;

            if (isToInsert)
            {
                dealerDB.deal_state = true;
                dealerDB.deal_registrationDate = DateTime.Now;
            }
            else
            {
                dealerDB.deal_updateDate = DateTime.Now;
            }


        }

        public static bool isDealerValid(DealerViewModel dealer)
        {
            if (dealer.document.Trim() == "")
            {
                throw new Exception("El NIT del concesionario es obligatorio");
            }

            if (dealer.name.Trim() == "")
            {
                throw new Exception("El nombre del concesionario no puede ir vacío");
            }


            return true;
        }

        public static bool InsertIntoDB(Dealer pDealer)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    db.Dealer.Add(pDealer);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DealerViewModel getLastDealerInserted()
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                var lastDealer = db.Dealer
                                .Select(
                                dl =>
                                           new DealerViewModel
                                           {
                                               id = dl.deal_id,
                                               document = dl.deal_document,
                                               name = dl.deal_name,
                                               state = dl.deal_state,
                                               registrationDate = dl.deal_registrationDate
                                           }
                                ).OrderByDescending(dl => dl.id)
                                .FirstOrDefault();

                return lastDealer;
            }

        }

        public static List<ContactViewModel> insertContactsByDealer(int dealerId, List<ContactViewModel> contacts)
        {
            List<ContactViewModel> contacsSaved = new List<ContactViewModel>();

            foreach (var contact in contacts)
            {
                contact.Dealer_id = dealerId;
                contacsSaved.Add(ContactController.InsertContact(contact));
            }
            return contacsSaved;

        }

        public static bool updateContactsByDealer(int dealerId, List<ContactViewModel> lsContact)
        {
            try
            {
                var lsContactsToValidate = ContactController.getListOfContactsByDealerId(dealerId);
                var lsContactsToInsert = lsContact.FindAll(cnt => cnt.id <= 0);
                var lsContactsToUpdate = lsContact.FindAll(cnt => cnt.id > 0);
                var lsContactsToDelete = lsContactsToValidate.Where(cnt => !lsContactsToUpdate.Any(cntD => cntD.id == cnt.id));

                DealerController.insertContactsByDealer(dealerId, lsContactsToInsert);

                foreach (var contact in lsContactsToUpdate)
                {
                    contact.Dealer_id = dealerId;
                    ContactController.updateContact(contact);
                }

                foreach (var contactToDelete in lsContactsToDelete)
                {
                    ContactController.DeleteById(contactToDelete.id);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static List<BranchViewModel> insertBranchesByDealer(int dealerId, List<BranchViewModel> branches)
        {
            List<BranchViewModel> branchesSaved = new List<BranchViewModel>();

            foreach (var branch in branches)
            {
                branch.Dealer_id = dealerId; branches.Add(BranchController.InsertBranch(branch));
            }
            return branchesSaved;
        }


        public static bool updateBranchesByDealer(int dealerId, List<BranchViewModel> lsBranches)
        {
            try
            {
                var lsBranchesToValidate = BranchController.getListOfBranchsByDealer(dealerId);
                var lsBranchesToInsert = lsBranches.FindAll(brn => brn.id <= 0);
                var lsBranchesToUpdate = lsBranches.FindAll(brn => brn.id > 0);
                var lsBranchesToDelete = lsBranchesToValidate.Where(brn => !lsBranchesToUpdate.Any(brnD => brnD.id == brn.id));

                DealerController.insertBranchesByDealer(dealerId, lsBranchesToInsert);

                foreach (var branch in lsBranchesToUpdate)
                {
                    branch.Dealer_id = dealerId;
                    BranchController.UpdateBranch(branch);
                }

                foreach (var branchToDelete in lsBranchesToDelete)
                {
                    BranchController.DeleteBranchById((int)branchToDelete.id);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private static void setPricesByItem(int dealerId, List<MaintenanceItemViewModel> maintenanceItems, bool isToInsert)
        {
            try
            {
                MaintenanceItemController.configurePricesByMaintenanceItemAndDealer(dealerId, maintenanceItems, isToInsert);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}