using System;
using System.Collections.Generic;

namespace Facade.Models.In_Asn
{
    public class In_Asn
    {
        public Int32 asn { get; set; }
        public String folder_comex { get; set; }
        public String container { get; set; }
        public String container_type { get; set; }
        public List<DataLPN> lpns { get; set; }

        public class DataLPN
        {
            public String lpn { get; set; }
            public String package_type { get; set; }
            public Int32 cod_store { get; set; }
            public List<productLPN> products { get; set; }
        }
        public class productLPN
        {
            public String sku { get; set; }
            public String packing_type_po { get; set; }
            public Int32 purchase_order { get; set; }
            public Int32 units { get; set; }
        }
    }
}