using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace API_FleetService.ViewModels
{
		interface ICRUDElement
		{
				[System.Web.Http.HttpGet]
				IHttpActionResult Get();

				[System.Web.Http.HttpGet]
				IHttpActionResult GetById(int pId);


		}
}
