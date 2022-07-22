using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class BillWiewModel 
    {
        public int bill_id { get; set; }
        public Nullable<int> bill_consecutive { get; set; }
        public string bill_code { get; set; }
        public Nullable<Double> bill_value { get; set; }
        public Nullable<Boolean> bill_status { get; set; }
        public Nullable<DateTime> bill_registrationDate { get; set; }
        public Nullable<DateTime> bill_updateDate { get; set; }
        public Nullable<DateTime> bill_deleteDate { get; set; }
        public BillStateviewModel bill_state { get; set; }
        public OrderWorkViewModel Order_work { get; set; }

    }
    
    public class BillDTO
    {
        //ESTA ES PARA REVISAR EL GRUPO DE BILLS
        public BillStateviewModel bill_state { get; set; }

        public List<int> idUpdates { get; set; }

        /*
            export class BillDTO {
              bill_state: BillStateDto;
              idUpdates: number[];
              paybill: number[]
            }
        */
    }

    public class BillSPDTO
    {
        public string CODE { get; set; }
        public double VALUE { get; set; }

        public BillStateviewModel STATE { get; set; }
        public int trx_id { get; set; }
        public List<int> BILL_ID { get; set; }
    }


    public class BillStateDTO
    {
        public Nullable<int> id;
        public string name;
        public string description;
        public Nullable<bool> state;
        public Nullable<DateTime> registrationDate;
        public List<int> paybill { get; set; }
    }


}