using IntegracionBbook.Api.Models;
using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Api.Repositories.Interfaces;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Api.Repositories.Repositories
{
    public class Product_modificationRepository : IProduct_modificationRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;

        public Product_modificationRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }

        public PruebaInternaDTO DeleteBbook_Product_Modification()
        {
            return new PruebaInternaDTO()
            {
                data = new List<PruebaInterna>()
                {
                    _commonRepository.DeleteTable("bbook_prdmod_master"),
                    _commonRepository.DeleteTable("bbook_prdmod_attrs")
                }
            };
        }

        public DTO<Product_modification>.Response LoadDataProduct_Modification(DTO<Product_modification>.Request product_ModificationRequest)
        {
            DTO<Product_modification>.Response response = new DTO<Product_modification>.Response();
            List<ErrorDTO<Product_modification>> errors = new List<ErrorDTO<Product_modification>>();
            pruebaInternas = new List<PruebaInterna>();
            string id = string.Empty;

            if (_commonRepository.DataOKProduct_Modification(ref ErrorMessage,ref id, product_ModificationRequest))
            {
                try
                {
                    //HACER PROCESO 
                    foreach (var item in _commonRepository.AddDataTableProduct_Modification("Product_modification", "A", product_ModificationRequest))
                    {
                        pruebaInternas.Add(item);
                        if (item.message != "OK")
                        {
                            errors.Add(new ErrorDTO<Product_modification>()
                            {
                                code = "01",
                                message = item.message,
                                record = (from x in product_ModificationRequest.data
                                          where x.parent_sku == item.id
                                         select x).FirstOrDefault()
                            });
                        }
                    }
                    if (errors.Count < 1) response = new DTO<Product_modification>.Response() { status = "OK", statusCode = 200, internalCode = "00", message = "OK" };
                    else response = new DTO<Product_modification>.Response() { status = "ERROR", statusCode = 409, internalCode = "00", message = "ERROR", errors = errors };
                }
                catch (Exception ex)
                {
                    response = new DTO<Product_modification>.Response() { status = "ERROR", statusCode = 500, internalCode = "01", message = ex.Message };
                }
                finally
                {
                    _commonRepository.Bbook_history("Bbook_Product_modification", JsonConvert.SerializeObject(product_ModificationRequest),
                   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
                    _iDBOracleRepository.Dispose();
                }
            }
            else
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = id,
                    table = false,
                    message = ErrorMessage,
                    method = "LoadDataProduct_Modification",
                    name = "Product_modification",
                    quantity = 1
                });
                response = new DTO<Product_modification>.Response()
                {
                    internalCode = "99",
                    message = ErrorMessage,
                    status = "ERROR",
                    statusCode = 406
                };
                _commonRepository.Bbook_history("Bbook_Product_modification", JsonConvert.SerializeObject(product_ModificationRequest),
                   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pruebaInternas));
            }

            return response;
        }
    }
}
