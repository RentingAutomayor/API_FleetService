using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_FleetService.Controllers
{
    public class BillStateController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                var bills = db.billState.Select(billst => new ViewModels.BillStateviewModel
                {
                    id = billst.billst_id,
                    name = billst.billst_name,
                    description = billst.billst_description,
                    registrationDate = billst.billst_registrationDate,
                    state = billst.billst_state
                }).ToList();


                return Ok(bills);
            }
        }

        [HttpGet]
        public IHttpActionResult Getbyid( int Tid)
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                var billst = db.billState
                    .Where( bilst => bilst.billst_id == Tid )
                    .Select(bilst => new ViewModels.BillStateviewModel
                {
                    id = bilst.billst_id,
                    name = bilst.billst_name,
                    description = bilst.billst_description,
                    registrationDate = bilst.billst_registrationDate,
                    state = bilst.billst_state
                }).ToList();


                return Ok(billst);
            }
        }



    }
}
