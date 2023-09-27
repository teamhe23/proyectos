using Facade.Models.In_Asn;
using Facade.Models.In_Comex;
using IntegracionBbook.Api.Models;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Interfaces
{
    public interface ICommonRepository
    {
        public string CambiarFormatoFecha(string fecha);
        public PruebaInterna DeleteTable(string nameTable);
        public List<PruebaInterna> DeleteTableIn_po(string nameTable);
        public List<PruebaInterna> DeleteTableIn_Codes(string nameTable);
        public List<PruebaInterna> UpdateDateTable(string nameTable,string tipo,List<string> codigos);
        public List<PruebaInterna> UpdateDateTableSize(string nameTable,string tipo,List<string> codigos);
        public List<PruebaInterna> UpdateDateTableMaster_po(string nameTable, string tipo, List<string> codigos);
        public List<PruebaInterna> UpdateDateTableProduct(string nameTable,string tipo,List<string> codigos);
        public List<PruebaInterna> UpdateDateTableHierarchy(string nameTable,string tipo,List<string> codigos);
        public List<PruebaInterna> UpdateDateTableIn_po(string nameTable, string tipo,string codigo);
        public List<PruebaInterna> UpdateDateTableIn_Codes(string nameTable, string tipo,string codigo);
        public List<PruebaInterna> AddDataTableHierarchy(string nameTable,string tipo, IEnumerable<IEnumerable<string>> codigos);
        public List<PruebaInterna> AddDataTableIn_po(string nameTable,string tipo, DTOUnitario<In_Po>.Request in_PoRequest);
        public List<PruebaInterna> AddDataTableIn_Asn(string nameTable, string tipo, DTOUnitario<In_Asn>.Request in_AsnRequest); 
        public List<PruebaInterna> AddDataTableProduct_Modification(string nameTable, string tipo, DTO<Product_modification>.Request product_ModificationRequest);
        public List<PruebaInterna> AddDataTableIn_Codes(string nameTable,DTOUnitario<In_Codes>.Request in_codesRequest, int tipo);
        public bool IsPrepack(string sku,ref string ErrorMessage);
        public bool DataOKIn_po(ref string ErrorMessage, DTOUnitario<In_Po>.Request in_PoRequest);
        public bool DataOKProduct_Modification(ref string ErrorMessage, ref string id,DTO<Product_modification>.Request product_ModificationRequest);
        public bool DataOKIn_Codes(ref string ErrorMessage, DTOUnitario<In_Codes>.Request in_codesRequest, int tipo);
        public Task<HttpResponseMessage> ApiBbook(string json, string tipo,string endpoint);
        public void Bbook_history(string tablename,string request, string response, string internaldata);
        public bool DataOKIn_Asn(ref string ErrorMessage, DTOUnitario<In_Asn>.Request in_PoRequest);
        public bool DataOKIn_Comex(ref string ErrorMessage, DTOUnitario<In_Comex>.Request in_ComexSRequest);
        public List<PruebaInterna> AddDataTableIn_Comex(string nameTable, string tipo, DTOUnitario<In_Comex>.Request in_ComexRequest);
    }
}
