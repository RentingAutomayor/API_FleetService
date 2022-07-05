using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class OrderWorkViewModel
    {
        public Nullable<int> trx_id { get; set; }
        public string trx_code { get; set; }
        public string cli_name { get; set; }
        public string veh_licensePlate { get; set; }
        public string deal_name { get; set; }
        public Nullable<DateTime> trx_registrationDate { get; set; }
        public Nullable<Double> trx_value { get; set; }
        public Nullable<int> trxst_id { get; set; }
        public string trxst_name { get; set; }
        public Nullable<int> bill_id { get; set; }
        public double iva { get; set; }
        public double beforeiva { get; set; }

        public Nullable<Boolean> trx_bill_status { get; set; }

        public BillStateviewModel order_state { get; set; }

        public Nullable<DateTime> trx_updateDate { get; set; }

    }
}
