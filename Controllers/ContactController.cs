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
																			registrationDate = cnt.cnt_registrationDate

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
																			registrationDate = cnt.cnt_registrationDate

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
																			registrationDate = cnt.cnt_registrationDate

																	}).FirstOrDefault();

										return Ok(oContact);
								}
						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Insert(ContactViewModel pContact)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										Contact oContactDB = ContactViewModel.setDataContact(pContact);
										db.Contact.Add(oContactDB);
										db.SaveChanges();
										rta.response = true;
										rta.message = "Contacto insertado correctamente a la bd y es asociado al cliente: ["+pContact.Client_id+"] ó al concesionario: ["+pContact.Dealer_id+"]";
										return Ok(rta);
								}

						}
						catch (Exception ex)
						{
								return BadRequest(ex.Message);
						}
				}

				[HttpPost]
				public IHttpActionResult Update(ContactViewModel pContact)
				{
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
								{
										var oContactDB = db.Contact.Where(cnt => cnt.cnt_id == pContact.id).FirstOrDefault();
										oContactDB.cnt_name = pContact.name;
										oContactDB.cnt_lastName = pContact.lastname;
										oContactDB.cnt_phone = pContact.phone;
										oContactDB.cnt_cellPhone = pContact.cellphone;
										oContactDB.cnt_email = pContact.email;
										oContactDB.cnt_adress = pContact.address;
										if (pContact.jobTitle != null)
										{
												var jobId = ContactViewModel.validateJobTitleId(pContact.jobTitle.description);
												if (jobId != 0)
												{
														oContactDB.jtcl_id = jobId;
												}
										}
										oContactDB.cty_id = (pContact.city != null) ? pContact.city.id : null;
										oContactDB.bra_id = (pContact.branch != null) ? pContact.branch.id : null;
										if (pContact.Client_id != 0)
										{
												oContactDB.cli_id = pContact.Client_id;
										}

										if (pContact.Dealer_id != 0)
										{
												oContactDB.deal_id = pContact.Dealer_id;
										}
									

										db.SaveChanges();
										rta.response = true;
										rta.message = "Se ha actualizado el contacto: " + oContactDB.cnt_name + " " + oContactDB.cnt_lastName;
										return Ok(rta);

								}

						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}


				[HttpPost]
				public IHttpActionResult Delete(ContactViewModel pContact) {
						try
						{
								ResponseApiViewModel rta = new ResponseApiViewModel();
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										var oContactDB = db.Contact.Where(cnt => cnt.cnt_id == pContact.id).FirstOrDefault();
										db.Contact.Remove(oContactDB);
										db.SaveChanges();
										rta.response = true;
										rta.message = "Se ha eliminado el contacto: " + oContactDB.cnt_name + " " + oContactDB.cnt_lastName;
										return Ok(rta);

								}
								
						}
						catch (Exception ex)
						{

								return BadRequest(ex.Message);
						}
				}



		}
}
