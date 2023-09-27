using Facade.Models.In_Asn;
using Facade.Models.In_Comex;
using IntegracionBbook.Api.Models.In_po;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static IntegracionBbook.Api.Models.In_po.Out_Po;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class In_ComexRepository : IIn_ComexRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public In_ComexRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository) 
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }


        public PruebaInternaDTO DeleteBbook_In_Comex()
        {
            return null;
            /*
            return new PruebaInternaDTO()
            {
                data = new List<PruebaInterna>()
                {
                    _commonRepository.DeleteTable("Bbook_in_po_cab"),
                    _commonRepository.DeleteTable("Bbook_in_po_det")
                }
            };
            */
        }
        //
        public DTOUnitario<In_Comex>.Response LoadDataIn_Comex(DTOUnitario<In_Comex>.Request in_ComexRequest)
        {
            DTOUnitario<In_Comex>.Response response = new DTOUnitario<In_Comex>.Response();
            List<ErrorDTO<In_Comex>> errors = new List<ErrorDTO<In_Comex>>();
            pruebaInternas = new List<PruebaInterna>();
            if (_commonRepository.DataOKIn_Comex(ref ErrorMessage, in_ComexRequest))
            {
                try
                {
                    foreach (var item in _commonRepository.AddDataTableIn_Comex("bbook_comex_tmp", "A", in_ComexRequest))
                    {
                        pruebaInternas.Add(item);
                        if (item.message != "OK")
                        {
                            errors.Add(new ErrorDTO<In_Comex>()
                            {
                                code = "01",
                                message = item.message,
                                record = in_ComexRequest.data
                            });
                        }
                    }

                    if (errors.Count < 1) response = new DTOUnitario<In_Comex>.Response() { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                    else response = new DTOUnitario<In_Comex>.Response() { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                }
                catch (Exception ex)
                {
                    response = new DTOUnitario<In_Comex>.Response() { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                }
                finally
                {
                    _commonRepository.Bbook_history("Bbook_In_Asn_CAB", JsonConvert.SerializeObject(in_ComexRequest),
                    JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
                    _iDBOracleRepository.Dispose();
                }
                //SP_IN_COMEX_CREATE
            }
            else
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "0",
                    table = false,
                    message = ErrorMessage,
                    method = "LoadDataIn_Comex",
                    name = "In_po",
                    quantity = 1
                });
                response = new DTOUnitario<In_Comex>.Response()
                {
                    internalCode = "99",
                    message = ErrorMessage,
                    status = "ERROR",
                    statusCode = 406
                };
                _commonRepository.Bbook_history("bbook_comex_tmp", JsonConvert.SerializeObject(in_ComexRequest),
                   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
            }

            return response;
        }

    }
}
