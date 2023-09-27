using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models
{
    public class Vendor
    {
            public string vendor_id{get;set;}
            public string vendor_name{get;set;} 
            public bool inactive{get;set;} 
            public string origin{get;set;}
            public string country{get;set;} 
            public string address_1{get;set;} 
            public string address_2{get;set;} 
            public string city{get;set;} 
            public int area_code{get;set;} 
            public string phone{get;set;} 
            public string email{get;set;} 
            public string contact_name{get;set;} 
            public string payment_terms{get;set;} 
            public int effective_days{get;set;} 
            public string currency{get;set;} 
    }
}
