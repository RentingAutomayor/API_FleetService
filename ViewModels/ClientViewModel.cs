using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class ClientViewModel:PersonViewModel
		{
				public CityViewModel city;
				public static Client PrepareCLientToInsertDB(ClientViewModel pClient) {

					
						if (ClientViewModel.ClientExistInDB(pClient.document.Trim()))
						{
								throw new Exception("El cliente que intenta ingresar, ya se encuentra almacenado en la base de datos");								
						}

						Client clientToInsert = ClientViewModel.SetDataToClient(pClient);

						return clientToInsert;
					
					
				}
			
				private static void ClientHasErros(ClientViewModel pClient) {
						if (pClient.document.Trim() == "")
						{
								throw new Exception("El documento del cliente no es válido");
						}

						long numberOfDocument;
						bool documentIsValid = long.TryParse(pClient.document, out numberOfDocument);

						if (!documentIsValid)
						{
								throw new Exception("El documento del cliente no es válido, intente ingresarlo sin dígito de verificación, sin puntos ni comas.");
						}

						if (pClient.name.Trim() == "")
						{
								throw new Exception("El nombre del cliente es obligatorio, no se puede insertar a la base de datos sin esta información");
						}
				}

				private static bool ClientExistInDB(string pClient_document) {
						try
						{
								bool rta = false;
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var clientDB = db.Client.Where(cl => cl.cli_document == pClient_document).FirstOrDefault();
										if (clientDB != null)
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


				private static Client SetDataToClient(ClientViewModel pClient) {
						
						ClientViewModel.ClientHasErros(pClient);
						
						Client clientToInsert = new Client();
						clientToInsert.cli_document = pClient.document;
						clientToInsert.cli_name = pClient.name.ToUpper();
						clientToInsert.cli_phone = pClient.phone;
						clientToInsert.cli_cellphone = pClient.cellphone;
						clientToInsert.cli_adress = pClient.address.ToUpper();
						clientToInsert.cli_website = (pClient.website != null)? pClient.website.ToUpper():null;
						clientToInsert.cty_id = (pClient.city != null)? pClient.city.id : null;
						clientToInsert.cli_state = true;
						clientToInsert.cli_registrationDate = DateTime.Now;

						return clientToInsert;	
				}


				public static bool InsertIntoDB(Client client) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										db.Client.Add(client);
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