using API_FleetService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAO_FleetService;
namespace API_FleetService.Controllers
{
		public class ContactController : ApiController
		{
				[HttpGet]
				public IHttpActionResult Get(int pOwner_id, string pKindOfOwner)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var lsContact = new List<ContactViewModel>();

										switch (pKindOfOwner.ToUpper())
										{
												case "CLIENT":
														lsContact = db.Contact
																	.Where(cnt => cnt.cli_id == pOwner_id && cnt.cnt_state == true)
																	.Select(cnt => new ContactViewModel
																	{
																			id = cnt.cnt_id,
																			name = cnt.cnt_name,
																			lastname = cnt.cnt_lastName,
																			phone = cnt.cnt_phone,
																			cellphone = cnt.cnt_cellPhone,
																			email = cnt.cnt_email,
																			address = cnt.cnt_adress,
																			city = (cnt.cty_id != null) ? new CityViewModel { id = cnt.cty_id, name = cnt.Cities.cty_name, departmentId = cnt.Cities.dpt_id } : null,
																			jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description } : null,
																			mustNotify = cnt.cnt_mustNotify,
																			//tipo de concacto aqui va
																			registrationDate = cnt.cnt_registrationDate,
																			updateDate= cnt.cnt_updateDate,
																			types = (from c in db.Contact
																					 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
																					 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

																					 where c.cnt_id == cnt.cnt_id

																					 select new ContactTypeViewModel
																					 {
																						 id = ct.cnttp_id,
																						 name = ct.cnttp_name,
																						 state = ct.cnttp_state,
																						 Bchecked = true
																					 }).ToList(),

																	}).ToList();
														break;
												case "DEALER":
														lsContact = db.Contact
																	.Where(cnt => cnt.deal_id == pOwner_id && cnt.cnt_state == true)
																	.Select(cnt => new ContactViewModel
																	{
																			id = cnt.cnt_id,
																			name = cnt.cnt_name,
																			lastname = cnt.cnt_lastName,
																			phone = cnt.cnt_phone,
																			cellphone = cnt.cnt_cellPhone,
																			email = cnt.cnt_email,
																			address = cnt.cnt_adress,
																			city = (cnt.cty_id != null) ? new CityViewModel { id = cnt.cty_id, name = cnt.Cities.cty_name, departmentId = cnt.Cities.dpt_id } : null,
																			jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description } : null,
																			mustNotify = cnt.cnt_mustNotify,
																			registrationDate = cnt.cnt_registrationDate,
																			updateDate = cnt.cnt_updateDate,
																			types = (from c in db.Contact
																					 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
																					 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

																					 where c.cnt_id == cnt.cnt_id

																					 select new ContactTypeViewModel
																					 {
																						 id = ct.cnttp_id,
																						 name = ct.cnttp_name,
																						 state = ct.cnttp_state,
																						 Bchecked = true
																					 }).ToList(),

																	}).ToList();
														break;
												default:
														lsContact = null;
														break;
										}

										return Ok(lsContact);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}


				[HttpGet]
				public IHttpActionResult GetById(int pId)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oContact = db.Contact
																	.Where(cnt => cnt.cnt_id == pId)
																	.Select(cnt => new ContactViewModel
																	{
																			id = cnt.cnt_id,
																			name = cnt.cnt_name,
																			lastname = cnt.cnt_lastName,
																			phone = cnt.cnt_phone,
																			cellphone = cnt.cnt_cellPhone,
																			email = cnt.cnt_email,
																			address = cnt.cnt_adress,
																			city = (cnt.cty_id != null) ? new CityViewModel { id = cnt.cty_id, name = cnt.Cities.cty_name, departmentId = cnt.Cities.dpt_id } : null,
																			jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description } : null,
																			mustNotify = cnt.cnt_mustNotify,
																			registrationDate = cnt.cnt_registrationDate,
																			updateDate = cnt.cnt_updateDate,
																			types = (from c in db.Contact
																					 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
																					 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

																					 where c.cnt_id == cnt.cnt_id

																					 select new ContactTypeViewModel
																					 {
																						 id = ct.cnttp_id,
																						 name = ct.cnttp_name,
																						 state = ct.cnttp_state,
																						 Bchecked = true
																					 }).ToList(), // tipo de contacto

																	}).FirstOrDefault();


									var testdata = (from c in db.Contact
													join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
													join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

													where c.cnt_id == pId

													select new ContactTypeViewModel
													{
														id = ct.cnttp_id,
														name = ct.cnttp_name,
														state = ct.cnttp_state
													}).ToList();

										return Ok(oContact);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}



				[HttpPost]
				public IHttpActionResult Insert(ContactViewModel contact)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{										
										var contactSaved = ContactController.InsertContact(contact);									
										return Ok(contactSaved);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				
				public static ContactViewModel InsertContact(ContactViewModel contact)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										Contact oContactDB = new Contact();
										ContactController.setDataToContact(contact, ref oContactDB, true);
										db.Contact.Add(oContactDB);
										db.SaveChanges();
										var contactSaved = ContactController.getLastContactSaved();
										return contactSaved;
								}

						}
						catch (Exception ex)
						{
								throw ex;
						}
				}

				[HttpPut]
				public IHttpActionResult Update(ContactViewModel pContact)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var contactUpdated = ContactController.updateContact(pContact);										
										return Ok(contactUpdated);
								}
						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				public static ContactViewModel updateContact(ContactViewModel contact) {
						try
						{

								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oContactDB = db.Contact
												.Where(cnt => cnt.cnt_id == contact.id)
												.FirstOrDefault();

										ContactController.setDataToContact(contact,ref oContactDB,false);
										db.SaveChanges();
										var contactUpdated = ContactController.getContactById(contact.id);
										return contactUpdated;
								}

						}
						catch (Exception ex)
						{
								throw ex;
						}
				}


				[HttpDelete]
				public IHttpActionResult Delete(int contactId) {
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										if (ContactController.DeleteById(contactId)) {
												rta.response = true;
												rta.message = "Se ha eliminado el contacto correctamente de la base de datos.";
										}										
										return Ok(rta);
								}
								
						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}

				public static bool DeleteById(int contactId)
				{
						try
						{								
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oContactDB = db.Contact.Where(cnt => cnt.cnt_id == contactId).FirstOrDefault();
										db.Contact.Remove(oContactDB);
										db.SaveChanges();									
										return true;

								}

						}
						catch (Exception ex)
						{
								throw ex;
						}
				}

				public static void setDataToContact(ContactViewModel contact, ref Contact contactDB, bool isToInsert)
				{
						contactDB.cnt_name = contact.name;
						contactDB.cnt_lastName = contact.lastname;
						contactDB.cnt_phone = contact.phone;
						contactDB.cnt_cellPhone = contact.cellphone;
						contactDB.cnt_email = contact.email;
						contactDB.cnt_adress = contact.address;
						if (contact.jobTitle != null)
						{
								var jobId = ContactController.validateJobTitleId(contact.jobTitle.description);
								if (jobId != 0)
								{
										contactDB.jtcl_id = jobId;
								}
						}

						contactDB.cty_id = (contact.city != null) ? contact.city.id : null;
						contactDB.bra_id = (contact.branch != null) ? contact.branch.id : null;						
						//aqui se consultaba el tipo de contacto
						contactDB.cnt_mustNotify = (contact.mustNotify!=null)? contact.mustNotify : false;					
						

						if (contact.Client_id != 0)
						{
								contactDB.cli_id = contact.Client_id;
						}

						if (contact.Dealer_id != 0)
						{
								contactDB.deal_id = contact.Dealer_id;
						}						
						contactDB.cnt_state = true;

						if (isToInsert)
						{
								contactDB.cnt_registrationDate = DateTime.Now;
						}
						else {
								contactDB.cnt_updateDate = DateTime.Now;
						}				
						
				}

				

				public static ContactViewModel getLastContactSaved()
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
								var contactSaved = db.Contact.OrderByDescending(cnt => cnt.cnt_id)
														.Select(cnt => new ContactViewModel
														{
																id = cnt.cnt_id,
																name = cnt.cnt_name,
																lastname = cnt.cnt_lastName,
																phone = cnt.cnt_phone,
																cellphone = cnt.cnt_cellPhone,
																email = cnt.cnt_email,
																address = cnt.cnt_adress,
																city = (cnt.cty_id != null) ? new CityViewModel { id = cnt.cty_id, name = cnt.Cities.cty_name, departmentId = cnt.Cities.dpt_id } : null,
																jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description } : null,
																Client_id = (cnt.cli_id != null) ? cnt.cli_id : null,
																Dealer_id = (cnt.deal_id != null) ? cnt.deal_id : null,
																registrationDate = cnt.cnt_registrationDate

														}).FirstOrDefault();
								return contactSaved;
						}
								
				}


				public static int validateJobTitleId(string pJobTitleDescription)
				{
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										if (pJobTitleDescription.Trim() != "")
										{
												var jobTitle = db.JobTitlesClient.Where(jt => jt.jtcl_description.ToUpper() == pJobTitleDescription.ToUpper()).FirstOrDefault();

												if (jobTitle == null)
												{
														JobTitlesClient job = new JobTitlesClient();
														job.jtcl_description = pJobTitleDescription.ToUpper();
														job.jtcl_state = true;
														db.JobTitlesClient.Add(job);
														db.SaveChanges();

														jobTitle = db.JobTitlesClient.Where(jt => jt.jtcl_description.ToUpper() == pJobTitleDescription.ToUpper()).FirstOrDefault();
												}
												return jobTitle.jtcl_id;
										}
										else
										{
												return 0;
										}
								}

						}
						catch (Exception ex)
						{
								return 0;
						}

				}

				[HttpGet]
				public IHttpActionResult getContactTypes()
				{
					try
					{ // revisar para cambiarlo por la obtencion de tipos de contacto
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
							var lsContactTypes = db.ContactType.Where(ct => ct.cnttp_state == true)
									.Select(ct =>
										 new ViewModels.ContactTypeViewModel
										 {
											 id = ct.cnttp_id,
											 name = ct.cnttp_name
										 }
									 ).ToList<ViewModels.ContactTypeViewModel>();

							return Ok(lsContactTypes);
						}

					}
					catch (Exception ex)
					{
						return BadRequest(ex.Message);
					}

				}


		public static ContactViewModel getContactById(int contactId) {
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var contact = db.Contact.OrderByDescending(cnt => cnt.cnt_id == contactId)
														.Select(cnt => new ContactViewModel
														{
																id = cnt.cnt_id,
																name = cnt.cnt_name,
																lastname = cnt.cnt_lastName,
																phone = cnt.cnt_phone,
																cellphone = cnt.cnt_cellPhone,
																email = cnt.cnt_email,
																address = cnt.cnt_adress,
																city = (cnt.cty_id != null) ? new CityViewModel { id = cnt.cty_id, name = cnt.Cities.cty_name, departmentId = cnt.Cities.dpt_id } : null,
																jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description } : null,
																Client_id = (cnt.cli_id != null) ? cnt.cli_id : null,
																Dealer_id = (cnt.deal_id != null) ? cnt.deal_id : null,
																registrationDate = cnt.cnt_registrationDate,
																mustNotify = cnt.cnt_mustNotify,
																updateDate = cnt.cnt_updateDate,
																types = (from c in db.Contact
																		 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
																		 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

																		 where c.cnt_id == cnt.cnt_id

																		 select new ContactTypeViewModel
																		 {
																			 id = ct.cnttp_id,
																			 name = ct.cnttp_name,
																			 state = ct.cnttp_state,
																			 Bchecked = true
																		 }).ToList(),
																//malisima practica 

														}).FirstOrDefault();
								return contact;
						}
				}

				public static List<ContactViewModel> getListOfContactsByClientId(int clientId)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsContacts = db.Contact.Where(cnt => cnt.cli_id == clientId)
											.Select(cnt => new ContactViewModel
											{
													id = cnt.cnt_id,
													name = cnt.cnt_name,
													lastname = cnt.cnt_lastName,
													phone = cnt.cnt_phone,
													cellphone = cnt.cnt_cellPhone,
													email = cnt.cnt_email,
													address = cnt.cnt_adress,
													jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description, state = cnt.cnt_state } : null,
													registrationDate = cnt.cnt_registrationDate,
													updateDate = cnt.cnt_updateDate,
													mustNotify = cnt.cnt_mustNotify,
													types = (from c in db.Contact
															 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
															 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

															 where c.cnt_id == cnt.cnt_id

															 select new ContactTypeViewModel
															 {
																 id = ct.cnttp_id,
																 name = ct.cnttp_name,
																 state = ct.cnttp_state,
																 Bchecked = true
															 }).ToList(),
												//aqui me iba el tipo de contacto
											}).ToList();
								return lsContacts;
						}
				}

				public static List<ContactViewModel> getListOfContactsByDealerId(int dealerId)
				{
						using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
						{
								var lsContacts = db.Contact.Where(cnt => cnt.deal_id == dealerId)
											.Select(cnt => new ContactViewModel
											{
													id = cnt.cnt_id,
													name = cnt.cnt_name,
													lastname = cnt.cnt_lastName,
													phone = cnt.cnt_phone,
													cellphone = cnt.cnt_cellPhone,
													address = cnt.cnt_adress,
													jobTitle = (cnt.jtcl_id != null) ? new JobTitleViewModel { id = cnt.jtcl_id, description = cnt.JobTitlesClient.jtcl_description, state = cnt.cnt_state } : null,
													registrationDate = cnt.cnt_registrationDate,
													updateDate = cnt.cnt_updateDate,
													mustNotify = cnt.cnt_mustNotify,
													email = cnt.cnt_email,
													types = (from c in db.Contact
															 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
															 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

															 where c.cnt_id == cnt.cnt_id

															 select new ContactTypeViewModel
															 {
																 id = ct.cnttp_id,
																 name = ct.cnttp_name,
																 state = ct.cnttp_state,
																 Bchecked = true
															 }).ToList(),
											}).ToList();
								return lsContacts;
						}


				}
			

		}
}
