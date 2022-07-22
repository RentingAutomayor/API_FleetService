using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_FleetService.Models
{
		public interface IBussinessObject
		{
				 bool state { get; set; }
				 Nullable<DateTime> registrationDate { get; set; }
				Nullable<DateTime> updateDate { get; set; }
				Nullable<DateTime> deleteDate { get; set; }
		}
}
