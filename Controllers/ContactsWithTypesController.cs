using API_FleetService.ViewModels;
using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_FleetService.Controllers
{
    public class ContactsWithTypesController : ApiController
    {
        // GET: api/ContactsWithTypes
        public IHttpActionResult Get()
        {
			try
			{
				using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
				{
					var oContactWithTypes = db.ContactsWithTypes.Select(cwt => new ContactsWithTypesViewModel
																{
																	id = cwt.id,
																	contact = (cwt.cnt_id != null) ? new ContactViewModel { 
																			id = (int) cwt.cnt_id,
																			name = cwt.Contact.cnt_name,
																			lastname = cwt.Contact.cnt_lastName,
																			phone = cwt.Contact.cnt_phone,
																			cellphone = cwt.Contact.cnt_cellPhone,
																			email = cwt.Contact.cnt_email,
																			address = cwt.Contact.cnt_adress,
																			city = (cwt.Contact.cty_id != null) ? new CityViewModel { id = cwt.Contact.cty_id, name = cwt.Contact.Cities.cty_name, departmentId = cwt.Contact.Cities.dpt_id } : null,
																			jobTitle = (cwt.Contact.jtcl_id != null) ? new JobTitleViewModel { id = cwt.Contact.jtcl_id, description = cwt.Contact.JobTitlesClient.jtcl_description } : null,
																			mustNotify = cwt.Contact.cnt_mustNotify,
																			registrationDate = cwt.Contact.cnt_registrationDate,
																			updateDate= cwt.Contact.cnt_updateDate

																	} : null,
																	contactType = null //(cwt.cnttp_id != null) ? new ViewModels.ContactType { id = cwt.cnttp_id, name = cwt.ContactType.cnttp_name } : null


					}).ToList();

					return Ok(oContactWithTypes);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

        // GET: api/ContactsWithTypes/5
        public IHttpActionResult GetContactstypesbyId(int pId)
        {
			try
			{
				using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
				{

					var oContactWithTypes = (from cwt in db.ContactsWithTypes
											join cnt in db.Contact on cwt.cnt_id equals cnt.cnt_id
											join cnttp in db.ContactType on cwt.cnttp_id equals cnttp.cnttp_id
											where cnt.cnt_id == pId
								  select new
								  {
									  idContact = cnt.cnt_id,
									  name = cnttp.cnttp_name
								  }).ToList();
					return Ok(oContactWithTypes);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IHttpActionResult Insert( ContactsWithTypesViewModel contactsWithTypesViewModel )
		{
			try
			{
				using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
				{
					//var contactSaved = ContactController.InsertContact(contactsWithTypesViewModel.contact);


					ContactsWithTypes oContactsWithTypesDB = new ContactsWithTypes();
					oContactsWithTypesDB.cnt_id = contactsWithTypesViewModel.contact.id;
					List<ViewModels.ContactType> listcontactWithTypes = contactsWithTypesViewModel.contactType.ToList();
					

					for (int i = 0; i < listcontactWithTypes.Count(); i++ )
					{
						oContactsWithTypesDB.cnttp_id = listcontactWithTypes[i].id;
						db.ContactsWithTypes.Add(oContactsWithTypesDB);
						db.SaveChanges();
					}
					//prueba
					//oContactsWithTypesDB.cnt_id = 18;
					//oContactsWithTypesDB.cnttp_id = 1;
					
					//db.ContactsWithTypes.Add(oContactsWithTypesDB);
					
					return Ok();
				}

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		//public static ContactViewModel InsertContactWithType(ContactsWithTypesViewModel contactsWithTypes)
		//{
		//	try
		//	{
		//		using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
		//		{
		//			ContactsWithTypes oContactWithTypeDB = new ContactsWithTypes();
		//			ContactsWithTypesController.setDatatoContactWithType(contactsWithTypes, ref oContactWithTypeDB, true);
		//			db.ContactsWithTypes.Add(oContactWithTypeDB);
		//			db.SaveChanges();
		//			var contactSaved = ContactController.getLastContactSaved();
		//			return contactSaved;
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}
		//}



		

		// PUT: api/ContactsWithTypes/5
		public void Put(int id, [FromBody]string value)
        {
        }


    }
}
