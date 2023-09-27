using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Api.Repositories.Utils;
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
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class ProductRepository:IProductRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;
        public ProductRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public DTO<Product>.Request GetAllProducts()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new DTO<Product>.Request()
                {
                    data = GetProducts("sp_product_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new DTO<Product>.Request()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<DTO<Product>> LoadDataProduct()
        {
            List<DTO<Product>.Request> productRequests = new List<DTO<Product>.Request>();
            List<DTO<Product>.Response> productResponses = new List<DTO<Product>.Response>();
            List<List<Product>> productRequestTemporales;
            List<Product> productosAEnviar = new List<Product>();
            List<Product> productosEnviados = new List<Product>();
            List<Product> productosNoEnviados = new List<Product>();
            IEnumerable<Product> productRequestTemporal;
            DTO<Product>.Request productRequest = new DTO<Product>.Request();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                string[] palabras = {"A","B","C","D","E","F","G","H","I","J",
                    "K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
                productRequest.data = GetProducts("sp_product_create","-A");
                if (productRequest.data.Count != 0)
                {
                    productRequestTemporales = FraccionamientorequestProducto(productRequest.data);
                    foreach (var temporal in productRequestTemporales)
                    {
                        productosAEnviar = new List<Product>();
                        productosEnviados = new List<Product>();
                        productosNoEnviados = new List<Product>();
                        foreach (var product in temporal)
                        {
                            productosAEnviar.Add(product);
                        }
                        foreach (var item in palabras)
                        {
                            productRequestTemporal = LinqExtensions.SplitWithCharacter<Product>(temporal, item);
                            if (productRequestTemporal.ToList().Count != 0)
                            {
                                foreach (var product in productRequestTemporal)
                                {
                                    productosEnviados.Add(product);
                                }
                                //HACER PROCESO POST HACIA EL BBOOK
                                productRequests.Add(new DTO<Product>.Request() { data = productRequestTemporal.ToList() });

                                productResponses.Add(await GetResponseAndUpdateTable(
                                    new DTO<Product>.Request() { data = productRequestTemporal.ToList() }, "A","A"));
                            }
                        }
                        foreach (var item in productosAEnviar)
                        {
                            if (!productosEnviados.Exists(x => x.product_id == item.product_id)) productosNoEnviados.Add(item);
                        }
                        if(productosNoEnviados.Count > 0)
                        {
                            //HACER PROCESO POST HACIA EL BBOOK
                            productRequests.Add(new DTO<Product>.Request() { data = productosNoEnviados });

                            productResponses.Add(await GetResponseAndUpdateTable(
                                new DTO<Product>.Request() { data = productosNoEnviados }, "A","A"));
                        }

                    }   
                }
                productRequest.data = GetProducts("sp_product_update","-C");
                if (productRequest.data.Count != 0)
                {
                    productRequestTemporales = FraccionamientorequestProducto(productRequest.data);
                    foreach (var temporal in productRequestTemporales)
                    {
                        productosAEnviar = new List<Product>();
                        productosEnviados = new List<Product>();
                        productosNoEnviados = new List<Product>();
                        foreach (var product in temporal)
                        {
                            productosAEnviar.Add(product);
                        }
                        foreach (var item in palabras)
                        {
                            productRequestTemporal = LinqExtensions.SplitWithCharacter<Product>(temporal, item);
                            if (productRequestTemporal.ToList().Count != 0)
                            {
                                foreach (var product in productRequestTemporal)
                                {
                                    productosEnviados.Add(product);
                                }
                                //HACER PROCESO POST HACIA EL BBOOK
                                productRequests.Add(new DTO<Product>.Request() { data = productRequestTemporal.ToList() });

                                //productResponses.Add(await GetResponseAndUpdateTable( new DTO<Product>.Request() { data = productRequestTemporal.ToList() }, "C"));
                                productResponses.Add(await GetResponseAndUpdateTable(new DTO<Product>.Request() { data = productRequestTemporal.ToList() }, "S","C"));
                            }
                        }
                        foreach (var item in productosAEnviar)
                        {
                            if (!productosEnviados.Exists(x => x.product_id == item.product_id)) productosNoEnviados.Add(item);
                        }
                        if (productosNoEnviados.Count > 0)
                        {
                            //HACER PROCESO POST HACIA EL BBOOK
                            productRequests.Add(new DTO<Product>.Request() { data = productosNoEnviados });

                            //productResponses.Add(await GetResponseAndUpdateTable( new DTO<Product>.Request() { data = productosNoEnviados }, "C"));
                            productResponses.Add(await GetResponseAndUpdateTable(new DTO<Product>.Request() { data = productosNoEnviados }, "S","C"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataProduct",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Product"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Product", JsonConvert.SerializeObject(productRequests),
                    JsonConvert.SerializeObject(productResponses), JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new DTO<Product>()
            {
                requests = productRequests,
                responses = productResponses,
                pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
            };
        }

        private List<List<Product>> FraccionamientorequestProducto(List<Product> data)
        {
            List<List<Product>> li_products = new List<List<Product>>();
            List<Product> i_products;
            int skuInicial = int.Parse(data[0].parent_sku.Trim());
            int skuFinal = int.Parse(data[data.Count - 1].parent_sku.Trim());

            for (int i = skuInicial; i <= skuFinal; i+=50000)
            {
                i_products = data.Select(x => x).Where(x => int.Parse(x.parent_sku) >= i && int.Parse(x.parent_sku) <= i + 50000).ToList();
                if(i_products != null) li_products.Add(i_products);
            }

            return li_products;
        }
        #endregion
        #region Metodos Extras
        public List<Product> GetProducts(string storedProcedure,string tipoproceso)
        {
            //Busca y carga marcas nuevas a la tabla Book_Product //Consulta todas las marcas en la tabla Book_Product
            List<Product> Products = new List<Product>();
            Product ProductTemporal = new Product();
            Product.StylePrePack stylePrePack = new Product.StylePrePack();
            List<Product.Product_Size> sizes = new List<Product.Product_Size>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            double master_price = 0;
            List<string> curves = new List<string>();
            string colorchange = string.Empty;
            string parent_sku = string.Empty;
            string product_key = string.Empty;
            bool devuelvedatos = false;
            try
            {
                oracleCommand.Parameters.Add("Products_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand, ref ErrorMessage, storedProcedure, true))
                {
                    if (ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            //if ((int.Parse(oracleDataReader["is_unique_size"].ToString().Trim()) == 1 && oracleDataReader["style_code"].ToString().Trim() == "") ||
                            //    (int.Parse(oracleDataReader["is_unique_size"].ToString().Trim()) == 0 && oracleDataReader["style_code"].ToString().Trim() != ""))
                            //{
                            devuelvedatos = true;
                            if (colorchange != string.Empty && (colorchange != oracleDataReader["dimension_id"].ToString().Trim()
                                || parent_sku != oracleDataReader["parent_sku"].ToString().Trim()))
                            {
                                sizes = sizes.GroupBy(x => x.product_key).Select(group => group.First()).ToList();
                                if (curves.Count != 0)
                                {
                                    ProductTemporal.style_pre_pack = new Product.StylePrePack()
                                    {
                                        code = stylePrePack.code,
                                        curve = curves
                                    };
                                }
                                ProductTemporal.sizes = sizes;
                                sizes = new List<Product.Product_Size>();
                                curves = new List<string>();
                                Products.Add(ProductTemporal);
                            }
                            if (oracleDataReader["style_curve"].ToString().Trim() != "") curves.Add(oracleDataReader["style_curve"].ToString().Trim());
                            stylePrePack = new Product.StylePrePack()
                            {
                                code = oracleDataReader["style_code"].ToString().Trim()
                            };
                            if (int.Parse(oracleDataReader["is_unique_size"].ToString().Trim()) != 1)
                            {
                                Product.Product_Size size = new Product.Product_Size()
                                {
                                    size = oracleDataReader["sizes_size"].ToString().Trim(),
                                    product_key = oracleDataReader["parent_sku"].ToString().Trim() + "-" + oracleDataReader["dimension_id"].ToString().Trim() + "-" + oracleDataReader["sizes_size"].ToString().Trim(),
                                    sku = oracleDataReader["sizes_sku"].ToString().Trim() == "" ? oracleDataReader["parent_sku"].ToString().Trim() : oracleDataReader["sizes_sku"].ToString().Trim(),
                                    ean_upc = oracleDataReader["sizes_ean_upc"].ToString().Trim(),
                                    description = oracleDataReader["sizes_description"].ToString().Trim()
                                };
                                sizes.Add(size);
                            }
                            else
                            {
                                Product.Product_Size size = new Product.Product_Size()
                                {
                                    product_key = oracleDataReader["parent_sku"].ToString().Trim() + "-" + oracleDataReader["dimension_id"].ToString().Trim() + oracleDataReader["sizes_size"].ToString().Trim(),
                                    ean_upc = oracleDataReader["sizes_ean_upc"].ToString().Trim(),
                                    sku = oracleDataReader["parent_sku"].ToString().Trim()
                                };
                                sizes.Add(size);
                            }

                            if (oracleDataReader["master_price"].ToString().Trim() != "") master_price = double.Parse(oracleDataReader["master_price"].ToString().Trim());
                            ProductTemporal = new Product()
                            {
                                product_id = oracleDataReader["product_id"].ToString().Trim(),
                                brand_id = oracleDataReader["brand_id"].ToString().Trim(),
                                hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                is_unique_size = int.Parse(oracleDataReader["is_unique_size"].ToString().Trim()) == 1 ? true : false,
                                dimension_id = oracleDataReader["dimension_id"].ToString().Trim(),
                                vendor_id = oracleDataReader["codigo_proveedor"].ToString().Trim(),
                                dimension_type = oracleDataReader["dimension_type"].ToString().Trim(),
                                season_id = oracleDataReader["season_id"].ToString().Trim(),
                                master_price = master_price,
                                size_type_id = oracleDataReader["size_type_id"].ToString().Trim(),
                                style_desc = oracleDataReader["style_desc"].ToString().Trim(),
                                parent_sku = oracleDataReader["parent_sku"].ToString().Trim()
                            };

                            colorchange = ProductTemporal.dimension_id;
                            parent_sku = ProductTemporal.parent_sku;
                            //}
                        }
                        if (ProductTemporal != null && devuelvedatos)
                        {

                            sizes = sizes.GroupBy(x => x.product_key).Select(group => group.First()).ToList();
                            if (curves.Count != 0)
                            {
                                ProductTemporal.style_pre_pack = new Product.StylePrePack()
                                {
                                    code = stylePrePack.code,
                                    curve = curves
                                };
                            }
                            ProductTemporal.sizes = sizes;
                            sizes = new List<Product.Product_Size>();
                            curves = new List<string>();
                            Products.Add(ProductTemporal);
                            devuelvedatos = false;
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetProducts" + tipoproceso,
                        quantity = Products.Count,
                        message = ErrorMessage,
                        name = "Product",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetProducts" + tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Product",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetProducts"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Product",
                    table = false
                });
            }

            return Products;
        }
        public async Task<DTO<Product>.Response> GetResponseAndUpdateTable(DTO<Product>.Request productRequest, string tipo_api, string tipo_table)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            DTO<Product>.Response productResponse = new DTO<Product>.Response();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<Product> exceptionalError;
             
            //////////////////
            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(productRequest), tipo_api, "products");
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:  

                    try
                    {
                        productResponse = JsonConvert.DeserializeObject<DTO<Product>.Response>(res.Content.ReadAsStringAsync().Result);

                        try
                        {
                            codigos = (from c in productRequest.data select c.parent_sku.Trim()).ToList();
                            foreach (var item in _commonRepository.UpdateDateTableProduct("product", tipo_table, codigos))
                            {
                                pruebaInternas.Add(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Product>>(res.Content.ReadAsStringAsync().Result);
                            return new DTO<Product>.Response()
                            {
                                internalCode = exceptionalError.internalCode,
                                message = exceptionalError.message,
                                statusCode = exceptionalError.statusCode,
                                status = exceptionalError.status
                            };
                        }
                        catch
                        {
                            productResponse = new DTO<Product>.Response()
                            {
                                internalCode = "JsonError",
                                message = "Error al deserializar Json" + "\n" + ex.Message,
                                statusCode = 00,
                                status = "error"
                            };
                        }
                    }
                    break;
                 case HttpStatusCode.Conflict:

                    try
                    {
                       productResponse = JsonConvert.DeserializeObject<DTO<Product>.Response>(res.Content.ReadAsStringAsync().Result); 
                         
                        try
                        {
                            
                            List<ErrorDTO<Product>> l_errores = productResponse.errors;
                            ErrorDTO<Product> error1 = l_errores.FirstOrDefault();
                            //foreach(ErrorDTO<Product> error1 in l_errores)
                            //{
                                if (error1.code == "IF500")
                                {
                                    var prod_data_error = productRequest.data.Where(a => a.product_id == error1.record.product_id).ToList().FirstOrDefault();

                                    //res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(prod_data_error), "PA", "products"); //<12/05/2022>
                                    res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(prod_data_error), tipo_api, "products");

                               if (res.StatusCode == HttpStatusCode.OK)
                                {
                                    codigos.Clear();
                                    codigos.Add(error1.record.parent_sku);
                                    foreach (var item in _commonRepository.UpdateDateTableProduct("product", tipo_table, codigos))//se mantiene con el tipo que viene para q lo pueda encontrar en BD
                                    {
                                        pruebaInternas.Add(item);
                                    }
                                }  

                                }
                            //}                             
                             
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<Product>>(res.Content.ReadAsStringAsync().Result);
                            return new DTO<Product>.Response()
                            {
                                internalCode = exceptionalError.internalCode,
                                message = exceptionalError.message,
                                statusCode = exceptionalError.statusCode,
                                status = exceptionalError.status
                            };
                        }
                        catch
                        {
                            productResponse = new DTO<Product>.Response()
                            {
                                internalCode = "JsonError",
                                message = "Error al deserializar Json" + "\n" + ex.Message,
                                statusCode = 00,
                                status = "error"
                            };
                        }
                    }
                    break;
                case HttpStatusCode.Unauthorized: 
                case HttpStatusCode.RequestEntityTooLarge: 
                case HttpStatusCode.GatewayTimeout:
                    productResponse = new DTO<Product>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = int.Parse( res.StatusCode.ToString()), // 401,
                        status = "error"
                    }; 
                    break;
                default:
                    productResponse = new DTO<Product>.Response()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = int.Parse(res.StatusCode.ToString()), 
                        status = ""
                    };
                    break;
            }

            ///////////////////
            return productResponse;
        }
        public PruebaInterna DeleteBbook_Product()
        {
            return _commonRepository.DeleteTable("Bbook_product");
        }
        #endregion
    }
}
