using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class In_Po
    {
        public int id_document { get; set; }
        public string proforma_invoice { get; set; }
        public string po_type { get; set; }
        public string distribution { get; set; }
        public bool is_style_prepack { get; set; }
        public In_po_Vendor vendor { get; set; }
        public In_po_Buyer buyer { get; set; }
        public Dates dates { get; set; }
        public double import_factor { get; set; }
        public double change_type { get; set; }
        public PortOfLoading port_of_loading { get; set; }
        public PortOfDischarge port_of_discharge { get; set; }
        public Payment_terms payment_terms { get; set; }
        public string incoterm { get; set; }
        public string currency { get; set; }
        public GeneralDestination destination { get; set; }
        public List<In_po_Product> products { get; set; }

        public class In_po_Vendor
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class In_po_Buyer
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class Dates
        {
            public string delivery { get; set; }
            public string shipment { get; set; }
            public string reception { get; set; }
            public string cancellation { get; set; }             
        }
        public class PortOfLoading: GeneralDestination { }
        public class PortOfDischarge: GeneralDestination { }
        public class Destination:GeneralDestination
        {
            public int units { get; set; }
        }
        public class In_po_Product
        {
            public string sku { get; set; }
            public double unit_cost { get; set; }
            public double total_cost_local { get; set; }
            public string description { get; set; }
            public string dim_value { get; set; }
            public string size { get; set; }
            public int total_units { get; set; }
            public List<Destination> destination { get; set; }
        }
        public class GeneralDestination
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class Payment_terms
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}
