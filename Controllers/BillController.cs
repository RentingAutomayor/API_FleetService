using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_FleetService.Controllers
{
    public class BillController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get()
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                var billd = db.bills.Select(bill => new ViewModels.BillWiewModel
                {
                    bill_id = bill.bill_id,
                    bill_consecutive = (int)bill.bill_consecutive,
                    bill_code = bill.bill_code,
                    bill_value = (Double)bill.bill_value,
                    bill_status = (bool)bill.bill_status,
                    bill_registrationDate = (DateTime)bill.bill_registrationDate,
                    bill_deleteDate = (DateTime)bill.bill_deleteDate,
                    bill_updateDate = (DateTime)bill.bill_updateDate,
                    bill_state =  new ViewModels.BillStateviewModel 
                        { 
                            id = bill.billst_id,
                            name = bill.billState.billst_name
                        },
                    Order_work = (from tr in db.transactions
                                  join cl in db.Client on tr.cli_id equals cl.cli_id
                                  join vh in db.Vehicle on cl.cli_id equals vh.cli_id
                                  join td in db.transactionDetail on tr.trx_id equals td.trx_id
                                  join dl in db.Dealer on td.deal_id equals dl.deal_id
                                  join trxst in db.transactionState on tr.trxst_id equals trxst.trxst_id
                                  where tr.trxst_id == 4 && tr.trx_id == bill.trx_id
                                                    // 4 = transaction state 'FINALIZADA'
                                  select new ViewModels.OrderWorkViewModel
                                  {
                                      trx_id = (int)tr.trx_id,
                                      trx_code = tr.trx_code,
                                      cli_name = cl.cli_name,
                                      veh_licensePlate = vh.veh_licensePlate,
                                      deal_name = dl.deal_name,
                                      trx_registrationDate = (DateTime)tr.trx_registrationDate,
                                      trx_value = (Double)tr.trx_value,
                                      beforeiva = (Double)tr.trx_valueWithoutDiscount,
                                      iva = (Double)tr.trx_taxesValue,
                                      trxst_id = (int)tr.trxst_id,
                                      trxst_name = trxst.trxst_name,
                                      trx_bill_status = tr.trx_bill_status,
                                      order_state = new ViewModels.BillStateviewModel { id = trxst.trxst_id, name = trxst.trxst_name },
                                      trx_updateDate = (DateTime)tr.trx_updateDate
                                  }).FirstOrDefault()
                     }).ToList();

                return Ok(billd);
            }
        }

        [HttpDelete]
        public IHttpActionResult delete(int bill_id)
        {
            
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {

                var databill = db.bills.Single(bill => bill.bill_id == bill_id);

                databill.billState = (from bst in db.billState where bst.billst_id == 1 select bst).FirstOrDefault();

                var dataTransaction = db.transactions.Single(tr => tr.trx_id == databill.trx_id);
                
                db.bills.Remove(databill);

                dataTransaction.trx_bill_status = null;

                db.SaveChanges();

                return Ok();
            }
        }



        /*
        [HttpPost]
        public IHttpActionResult InsertBill( ViewModels.BillDTO bill)
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {


                //OPTIMIZAR ESTO EN RENDIMIENTO NO ES BUENA PRACTICA CONSULTAR LA MISMA DOS VECES 
                // INICIO BLOQUE A OPTIMIZAR ------------
                Double value = 0;
                foreach(var i in bill.idUpdates)
                {
                    var query = (from a in db.transactions where (a.m_id == 4 && a.trxst_id == 4 && a.trx_id == i) select a).ToList();
                    value = value + (Double) query[0].trx_value;
                }
                // FIN BLOQUE A OPTIMIZAR  ------------
                
                var res = db.STRPRC_PROCESS_BILL(
                    "asd",
                    (decimal?) value,
                    bill.bill_state.id
                );
                

                foreach( var i in bill.idUpdates)
                {
                    var query = (from a in db.transactions where (a.m_id == 4 && a.trxst_id == 4 && a.trx_id == i) select a).ToList();
                    foreach (var item in query)
                    {
                      //item.bill_id = res;
                    }
                }

                db.SaveChanges();

                return Ok();
            }

        }
        */

        [HttpPost]
        public IHttpActionResult payBill( ViewModels.BillSPDTO paybill )
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {

                foreach (var i in paybill.BILL_ID)
                {
                    var result = (from fc in db.bills where fc.bill_id == i select fc).FirstOrDefault();
                    result.billState = (from bst in db.billState where bst.billst_id == 2 select bst).FirstOrDefault(); 

                }

                db.SaveChanges();

                return Ok();
            }
        }
        

        [HttpPost]
        public IHttpActionResult Insert(ViewModels.BillSPDTO bill)
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                
            var query = (from a in db.transactions where (a.m_id == 4 && a.trxst_id == 4 && a.trx_id == bill.trx_id) select a).ToList();
            Double value = (Double) query[0].trx_value;

                // FIN BLOQUE A OPTIMIZAR  ------------

                var res = db.STRPRC_PROCESS_BILL(
                    bill.CODE,
                    (decimal?)bill.VALUE,
                    bill.STATE.id,
                    bill.trx_id
                );          

                return Ok();
            }

        }




        [HttpGet]
        public IHttpActionResult getAllOrderWorksToBill()
        {
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                // SE REALIZA LA CONSULTA QUE ME TRAE LAS TRANSACCIONES APROBADAS
                var result = (from tr in db.transactions
                              join cl in db.Client on tr.cli_id equals cl.cli_id
                              join vh in db.Vehicle on cl.cli_id equals vh.cli_id
                              join td in db.transactionDetail on tr.trx_id equals td.trx_id
                              join dl in db.Dealer on td.deal_id equals dl.deal_id
                              join trxst in db.transactionState on tr.trxst_id equals trxst.trxst_id
                              where tr.trxst_id == 4 // 4 = transaction state 'FINALIZADA' 

                              select new ViewModels.OrderWorkViewModel
                              {
                                  trx_id = (int) tr.trx_id,
                                  trx_code = tr.trx_code,
                                  cli_name = cl.cli_name,
                                  veh_licensePlate = vh.veh_licensePlate,
                                  deal_name = dl.deal_name,
                                  trx_registrationDate = (DateTime)tr.trx_registrationDate,
                                  trx_value = (Double)tr.trx_value,
                                  beforeiva = (Double) tr.trx_valueWithoutDiscount,
                                  iva = (Double)tr.trx_taxesValue,
                                  trxst_id = (int) tr.trxst_id,
                                  trxst_name =    trxst.trxst_name,
                                  trx_bill_status = tr.trx_bill_status,
                                  order_state = new ViewModels.BillStateviewModel { id = trxst.trxst_id , name = trxst.trxst_name },
                                  trx_updateDate = (DateTime) tr.trx_updateDate
                              }).ToList();
                return Ok(result);
            }
        }



    }
}
