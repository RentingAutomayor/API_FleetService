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
				var oContactWithTypes = new List<ContactsWithTypesViewModel>();
				var temptypes = new List<DAO_FleetService.ContactType>();

				using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
				{
					oContactWithTypes = db.Contact.Select(cwt => new ContactsWithTypesViewModel
									{
										id = cwt.cnt_id,
										contact = new ContactViewModel { 
												id = (int) cwt.cnt_id,
												name = cwt.cnt_name,
												lastname = cwt.cnt_lastName,
												phone = cwt.cnt_phone,
												cellphone = cwt.cnt_cellPhone,
												email = cwt.cnt_email,
												address = cwt.cnt_adress,
												city = (cwt.cty_id != null) ? new CityViewModel { id = cwt.cty_id, name = cwt.Cities.cty_name, departmentId = cwt.Cities.dpt_id } : null,
												jobTitle = (cwt.jtcl_id != null) ? new JobTitleViewModel { id = cwt.jtcl_id, description = cwt.JobTitlesClient.jtcl_description } : null,
												mustNotify = cwt.cnt_mustNotify,
												registrationDate = cwt.cnt_registrationDate,
												updateDate= cwt.cnt_updateDate
										},
										types = (from c in db.Contact 
												 join cwtps in db.ContactsWithTypes on c.cnt_id equals cwtps.cnt_id
												 join ct in db.ContactType on cwtps.cnttp_id equals ct.cnttp_id

												 where c.cnt_id == cwt.cnt_id

												 select new ContactTypeViewModel
												 {
													id =  ct.cnttp_id,
													name = ct.cnttp_name,
													state = ct.cnttp_state
												 }).ToList(),
										
					}).ToList();

					return Ok(oContactWithTypes);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		//trae los tipos de contacto que pertenecen al id de contacto
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

		//[HttpPost]
  //      public IHttpActionResult Insert(ContactsWithTypesViewModel contactsWithTypesViewModel)
  //      {
  //          try
  //          {
  //              using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
  //              {
  //                  //var contactSaved = ContactController.InsertContact(contactsWithTypesViewModel.contact);


  //                  ContactsWithTypes oContactsWithTypesDB = new ContactsWithTypes();
  //                  oContactsWithTypesDB.cnt_id = contactsWithTypesViewModel.contact.id;
  //                  List<ViewModels.ContactTypeViewModel> listcontactWithTypes = contactsWithTypesViewModel.contactType.ToList();


  //                  for (int i = 0; i < listcontactWithTypes.Count(); i++)
  //                  {
  //                      oContactsWithTypesDB.cnttp_id = listcontactWithTypes[i].id;
  //                      db.ContactsWithTypes.Add(oContactsWithTypesDB);
  //                      db.SaveChanges();
  //                  }
  //                  //prueba
  //                  //oContactsWithTypesDB.cnt_id = 18;
  //                  //oContactsWithTypesDB.cnttp_id = 1;

  //                  //db.ContactsWithTypes.Add(oContactsWithTypesDB);

  //                  return Ok();
  //              }

  //          }
  //          catch (Exception ex)
  //          {
  //              return BadRequest(ex.Message);
  //          }
  //      }
  //      public static ContactViewModel InsertContactWithType(ContactsWithTypesViewModel contactsWithTypes)
  //      {
  //          try
  //          {
  //              using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
  //              {
  //                  ContactsWithTypes oContactWithTypeDB = new ContactsWithTypes();
  //                  ContactsWithTypesController.setDatatoContactWithType(contactsWithTypes, ref oContactWithTypeDB, true);
  //                  db.ContactsWithTypes.Add(oContactWithTypeDB);
  //                  db.SaveChanges();
  //                  var contactSaved = ContactController.getLastContactSaved();
  //                  return contactSaved;
  //              }

  //          }
  //          catch (Exception ex)
  //          {
  //              throw ex;
  //          }
  //      }





        // PUT: api/ContactsWithTypes/5
        public void Put(int id, [FromBody]string value)
        {
        }


    }
}
