using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAO_FleetService;

namespace API_FleetService.ViewModels
{
		public class BranchViewModel: PersonViewModel
		{
				public bool isMain;
				public CityViewModel city;
				public int Client_id;
				public int Dealer_id;

				public static branch setDataBranch(BranchViewModel pBranch) {
						branch oBranch = new branch();
						oBranch.bra_name = pBranch.name;
						oBranch.bra_adress = pBranch.address;
						oBranch.bra_phone = pBranch.phone;
						oBranch.bra_cellphone = pBranch.cellphone;
						oBranch.cty_id = (pBranch.city != null) ? pBranch.city.id : null;
						oBranch.bra_state = true;
						oBranch.bra_registrationDate = DateTime.Now;
						if (pBranch.Client_id != 0) {
								oBranch.cli_id = pBranch.Client_id;
						}
						if (pBranch.Dealer_id != 0) {
								oBranch.deal_id = pBranch.Dealer_id;
						}
						return oBranch;
				}
		}
}