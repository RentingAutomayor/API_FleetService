using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class DealerViewModel : PersonViewModel
		{
				public static Dealer PrepareDealerToInsertDB(DealerViewModel pDealer) {
						
						if (DealerViewModel.dealerExistInDB(pDealer.document)) {
								throw new Exception("El concesionario que intenta ingresar ya se encuentra almacenado en la base de datos");
						}
						Dealer oDealerDB = DealerViewModel.setDataToDealer(pDealer);
						return oDealerDB;
				}

				private static bool dealerExistInDB(string pDealer_document)
				{
						try
						{
								bool rta = false;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var dealerDB = db.Dealer.Where(dlr => dlr.deal_document == pDealer_document).FirstOrDefault();
										if (dealerDB != null)
										{
												rta = true;
										}
										return rta;
								}
						}
						catch (Exception ex)
						{
								return false;
						}
				}

				public static Dealer setDataToDealer(DealerViewModel pDealer) {
						if (pDealer.document.Trim() == "")
						{
								throw new Exception("El NIT del concesionario es obligatorio");
						}

						if (pDealer.name.Trim() == "")
						{
								throw new Exception("El nombre del concesionario no puede ir vacío");
						}

						Dealer oDealerDB = new Dealer();
						oDealerDB.deal_document = pDealer.document;
						oDealerDB.deal_name = pDealer.name;
						oDealerDB.deal_state = true;
						oDealerDB.deal_registrationDate = DateTime.Now;
						return oDealerDB;
				}

				public static bool InsertIntoDB(Dealer pDealer) {
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
		}
}