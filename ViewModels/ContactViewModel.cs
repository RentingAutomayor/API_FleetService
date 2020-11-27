using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class ContactViewModel:PersonViewModel
		{
				public string email;
				public JobTitleViewModel jobTitle;
				public CityViewModel city;
				public BranchViewModel branch;
				public int Client_id;
				public int Dealer_id;

				public static Contact setDataContact(ContactViewModel pContact) {
						Contact oContact = new Contact();					
						oContact.cnt_name = pContact.name;
						oContact.cnt_lastName = pContact.lastname;
						oContact.cnt_phone = pContact.phone;
						oContact.cnt_cellPhone = pContact.cellphone;
						oContact.cnt_email = pContact.email;
						oContact.cnt_adress = pContact.address;
						if (pContact.jobTitle != null) {
								var jobId = ContactViewModel.validateJobTitleId(pContact.jobTitle.description);
								if (jobId != 0)
								{
										oContact.jtcl_id = jobId;
								}
						}						
						oContact.cty_id = (pContact.city != null) ? pContact.city.id : null;
						oContact.bra_id = (pContact.branch != null) ? pContact.branch.id : null;
						if (pContact.Client_id != 0) {
								oContact.cli_id =  pContact.Client_id ;
						}

						if (pContact.Dealer_id != 0) {
								oContact.deal_id =  pContact.Dealer_id;
						}					
						oContact.cnt_state = true;
						oContact.cnt_registrationDate = DateTime.Now;
						return oContact;
				}


				public static int validateJobTitleId(string pJobTitleDescription) {
						try
						{
								using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
										if (pJobTitleDescription.Trim() != "")
										{
												var jobTitle = db.JobTitlesClient.Where(jt => jt.jtcl_description.ToUpper() == pJobTitleDescription.ToUpper()).FirstOrDefault();

												if (jobTitle == null) {
														JobTitlesClient job = new JobTitlesClient();
														job.jtcl_description = pJobTitleDescription.ToUpper();
														job.jtcl_state = true;
														db.JobTitlesClient.Add(job);
														db.SaveChanges();

														jobTitle = db.JobTitlesClient.Where(jt => jt.jtcl_description.ToUpper() == pJobTitleDescription.ToUpper()).FirstOrDefault();
												}
												return jobTitle.jtcl_id;
										}
										else {
												return 0;
										}
								}
									
						}
						catch (Exception ex)
						{
								return 0;
						}
						
				}
		}
}