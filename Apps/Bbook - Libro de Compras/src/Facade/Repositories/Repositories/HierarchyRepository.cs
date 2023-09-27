using IntegracionBbook.Api.Models.Utils;
using IntegracionBbook.Data.Interfaces;
using IntegracionBbook.Models;
using IntegracionBbook.Models.Hierarchy;
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
using static IntegracionBbook.Models.Hierarchy.HierarchyAreaLineaPatch;
using static IntegracionBbook.Models.Hierarchy.HierarchyPatch;
using static IntegracionBbook.Models.Hierarchy.HierarchyPatch.HierarchyPatchResponse;
using static IntegracionBbook.Models.Hierarchy.HierarchyPost;
using static IntegracionBbook.Models.Hierarchy.HierarchyPut;
using static IntegracionBbook.Models.PruebaInternaDTO;

namespace IntegracionBbook.Repositories.Repositories
{
    public class HierarchyRepository:IHierarchyRepository
    {
        string ErrorMessage;
        private readonly IDBOracleRepository _iDBOracleRepository; //Interface de Conexion a Oracle
        private readonly ICommonRepository _commonRepository; //Interface de metodos comunes
        private List<PruebaInterna> pruebaInternas;

        public class FatherSon
        {
            public string fatherId { get; set; }
            public string SonId { get; set; }
            public string SonDescrption { get; set; }
        }
        public HierarchyRepository(IDBOracleRepository iDBOracleRepository, ICommonRepository commonRepository)
        {
            _iDBOracleRepository = iDBOracleRepository;
            _commonRepository = commonRepository;
        }
        #region Metodos Principales
        public HierarchyPostRequest GetAllHierarchies()
        {
            pruebaInternas = new List<PruebaInterna>();
            try
            {
                return new HierarchyPostRequest()
                {
                    data = GetHierarchiesPost("sp_Hierarchy_read",""),
                    MessageError = pruebaInternas[0].message
                };
            }
            catch (Exception ex)
            {
                return new HierarchyPostRequest()
                {
                    MessageError = ex.Message
                };
            }
            finally
            {
                _iDBOracleRepository.Dispose();
            }
        }
        public async Task<HierarchyDTO> LoadDataHierarchy()
        {
            HierarchyPost hierarchyPost = new HierarchyPost();
            HierarchyPatch hierarchyPatch = new HierarchyPatch();
            HierarchyPut hierarchyPut = new HierarchyPut();

            List<HierarchyPostRequest> hierarchyPostRequests = new List<HierarchyPostRequest>();
            List<HierarchyPatchRequest> hierarchyPatchRequests = new List<HierarchyPatchRequest>();
            List<HierarchyPutRequest> hierarchyPutRequests = new List<HierarchyPutRequest>();

            List<HierarchyPostResponse> hierarchyPostResponses = new List<HierarchyPostResponse>();
            List<HierarchyPatchResponse> hierarchyPatchResponses = new List<HierarchyPatchResponse>();
            List<HierarchyPutResponse> hierarchyPutResponses = new List<HierarchyPutResponse>();

            HierarchyPostRequest hierarchyPostRequest = new HierarchyPostRequest();

            HierarchyPatchRequest hierarchyPatchRequest = new HierarchyPatchRequest();
            List<HierarchyAreaLineaPatch> hierarchyAreaLineaPatchs = new List<HierarchyAreaLineaPatch>();
            List<HierarchyDepartamentoLineaPatch> hierarchyDepartamentoLineaPatchs = new List<HierarchyDepartamentoLineaPatch>();
            List<HierarchyLineaPatch> hierarchyLineaPatchs = new List<HierarchyLineaPatch>();
            HierarchyAreaLineaPatch hierarchyAreaLineaPatch = null;
            HierarchyDepartamentoLineaPatch hierarchyDepartamentoLineaPatch = null;
            HierarchyLineaPatch hierarchyLineaPatch = null;
            HierarchyPutRequest hierarchyPutRequest = new HierarchyPutRequest();
            IEnumerable<IEnumerable<string>> codigos;
            string json;
            int estructura=0;
            pruebaInternas = new List<PruebaInterna>();

            try
            {
                hierarchyPostRequest.data = GetHierarchiesPost("sp_hierarchy_create","-A");
                if (hierarchyPostRequest.data.Count != 0)
                {
                    //HACER PROCESO POST HACIA EL BBOOK
                    hierarchyPostRequests.Add(new HierarchyPostRequest() { data = hierarchyPostRequest.data });
                    hierarchyPostResponses.Add(await GetResponsePostAndUpdateTable(hierarchyPostRequest, "A"));
                    hierarchyPost = new HierarchyPost()
                    {
                        hierarchyPostRequests= hierarchyPostRequests,
                        hierarchyPostResponses = hierarchyPostResponses,
                        pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
                    };
                }
                hierarchyPutRequest.data = GetHierarchiesPut("sp_hierarchy_update_put","-C");
                if (hierarchyPutRequest.data.Count != 0)
                {
                    //HACER PROCESO PUT HACIA EL BBOOK
                    hierarchyPutRequests.Add(hierarchyPutRequest);
                    hierarchyPutResponses.Add(await GetResponsePutAndUpdateTable(hierarchyPutRequest, "C"));
                    hierarchyPut = new HierarchyPut()
                    {
                        hierarchyPatchRequests = hierarchyPutRequests,
                        hierarchyPutResponses = hierarchyPutResponses,
                        pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
                    };
                }
                GetHierarchiesPatch("sp_hierarchy_update_patch",ref hierarchyAreaLineaPatch,ref hierarchyDepartamentoLineaPatch, ref hierarchyLineaPatch,ref estructura);
                if(hierarchyAreaLineaPatch != null || hierarchyDepartamentoLineaPatch != null || hierarchyLineaPatch != null)
                {
                    //HACER PROCESO PATCH HACIA EL BBOOK
                    if (hierarchyAreaLineaPatch != null)
                    {
                        hierarchyAreaLineaPatchs.Add(hierarchyAreaLineaPatch);
                        hierarchyPatchRequest.hierarchyAreaLineaPatchRequest = new HierarchyPatchRequest.HierarchyAreaLineaPatchRequest()
                        {
                            data = hierarchyAreaLineaPatchs
                        };
                        hierarchyPatchRequests.Add(hierarchyPatchRequest);
                        codigos = (IEnumerable<IEnumerable<string>>)hierarchyPatchRequest.hierarchyAreaLineaPatchRequest.data.Select(x => x.sublevel.Select(a => a.sublevel.Select(d => d.sublevel.Select(l=> l.level_code)))).ToList();
                        json = JsonConvert.SerializeObject(hierarchyPatchRequest.hierarchyAreaLineaPatchRequest);
                    }
                    else if (hierarchyDepartamentoLineaPatch != null)
                    {
                        hierarchyDepartamentoLineaPatchs.Add(hierarchyDepartamentoLineaPatch);
                        hierarchyPatchRequest.hierarchyDepartamentoLineaPatchRequest = new HierarchyPatchRequest.HierarchyDepartamentoLineaPatchRequest()
                        {
                            data = hierarchyDepartamentoLineaPatchs
                        };
                        hierarchyPatchRequests.Add(hierarchyPatchRequest);
                        codigos = (IEnumerable<IEnumerable<string>>)hierarchyPatchRequest.hierarchyDepartamentoLineaPatchRequest.data.Select(a => a.sublevel.Select(d => d.sublevel.Select(l => l.level_code))).ToList();
                        json = JsonConvert.SerializeObject(hierarchyPatchRequest.hierarchyDepartamentoLineaPatchRequest);
                    }
                    else
                    {
                        hierarchyLineaPatchs.Add(hierarchyLineaPatch);
                        hierarchyPatchRequest.hierarchyLineaPatchRequest = new HierarchyPatchRequest.HierarchyLineaPatchRequest()
                        {
                            data = hierarchyLineaPatchs
                        };
                        hierarchyPatchRequests.Add(hierarchyPatchRequest);
                        codigos = (IEnumerable<IEnumerable<string>>)hierarchyPatchRequest.hierarchyLineaPatchRequest.data.Select(d => d.sublevel.Select(l => l.level_code)).ToList();
                        json = JsonConvert.SerializeObject(hierarchyPatchRequest.hierarchyLineaPatchRequest);
                    }
                    hierarchyPatchResponses.Add(await GetResponsePatchAndUpdateTable(codigos,json, "M",estructura));
                    hierarchyPatch = new HierarchyPatch()
                    {
                        hierarchyPatchRequests = hierarchyPatchRequests,
                        hierarchyPatchResponses = hierarchyPatchResponses,
                        pruebaInterna = new PruebaInternaDTO() { data = pruebaInternas }
                    };
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInterna()
                {
                    id = "01",
                    method = "LoadDataHierarchy",
                    quantity = 0,
                    message = ex.Message,
                    table = false,
                    name = "Hierarchy"
                });
            }
            finally
            {
                _commonRepository.Bbook_history("Bbook_Hierarchy",
                    "[" + JsonConvert.SerializeObject(hierarchyPostRequests) +","+ 
                    JsonConvert.SerializeObject(hierarchyPutRequests) +","+ 
                    JsonConvert.SerializeObject(hierarchyPatchRequests)+"]",
                    "[" + JsonConvert.SerializeObject(hierarchyPostResponses) +","+ 
                    JsonConvert.SerializeObject(hierarchyPutResponses) +","+
                    JsonConvert.SerializeObject(hierarchyPatchResponses) + "]", 
                    JsonConvert.SerializeObject(pruebaInternas));
                _iDBOracleRepository.Dispose();
            }
            return new HierarchyDTO
            {
                HierarchyPost = hierarchyPost,
                HierarchyPatch = hierarchyPatch,
                HierarchyPut = hierarchyPut
            };
        }
        #endregion MetodosPrincipales
        #region Metodos Extras
        public List<DivisionPost> GetHierarchiesPost(string HierarchydProcedure,string tipoproceso)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Hierarchy //Consulta todas las sucursales en la tabla Book_Hierarchy
            List<DivisionPost> Divisions = new List<DivisionPost>();
            List<AreaPost> Areas = new List<AreaPost>();
            List<Departamento> Departments = new List<Departamento>();
            List<FatherSon> TAreas = new List<FatherSon>();
            List<FatherSon> TDepartments = new List<FatherSon>();
            List<FatherSon> TLines = new List<FatherSon>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Hierarchys_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, HierarchydProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            Divisions.Add(new DivisionPost()
                            {
                                hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                level_code = oracleDataReader["division_id"].ToString().Trim(),
                                level_name = oracleDataReader["division_name"].ToString().Trim()
                            });
                            TAreas.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["division_id"].ToString().Trim(),
                                SonId = oracleDataReader["area_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["area_name"].ToString().Trim()
                            });
                            TDepartments.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["area_id"].ToString().Trim(),
                                SonId = oracleDataReader["department_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["department_name"].ToString().Trim()
                            });
                            TLines.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["department_id"].ToString().Trim(),
                                SonId = oracleDataReader["line_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["line_name"].ToString().Trim()
                            });
                        }
                        Divisions = Divisions.GroupBy(x => x.level_code).Select(group => group.First()).ToList();
                        TAreas = TAreas.GroupBy(x => x.SonId).Select(group => group.First()).ToList();
                        TDepartments = TDepartments.GroupBy(x => x.SonId).Select(group => group.First()).ToList();
                        TLines = TLines.GroupBy(x => x.SonId).Select(group => group.First()).ToList();

                        for (int i = 0; i < Divisions.Count; i++)
                        {
                            foreach (var item in Areas)
                            {
                                Areas = (from a in TAreas
                                         where a.fatherId == item.level_code
                                        select new AreaPost
                                        {
                                            level_code = a.SonId,
                                            level_name = a.SonDescrption
                                        }).ToList();
                            }
                            Divisions[i].sublevel = (from a in TAreas
                                                     where a.fatherId == Divisions[i].level_code
                                                     select new AreaPost
                                                     {
                                                         level_code = a.SonId,
                                                         level_name = a.SonDescrption,
                                                         sublevel = (from d in TDepartments
                                                                     where d.fatherId == a.SonId
                                                                     select new Departamento
                                                                     {
                                                                         level_code = d.SonId,
                                                                         level_name = d.SonDescrption,
                                                                         sublevel = (from l in TLines
                                                                                     where l.fatherId == d.SonId
                                                                                     select new Linea
                                                                                     {
                                                                                         level_code = l.SonId,
                                                                                         level_name = l.SonDescrption
                                                                                     }).ToList()
                                                                     }).ToList()
                                                     }).ToList();
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPost"+ tipoproceso,
                        quantity = Divisions.Count,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPost"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetHierarchiesPost"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Hierarchy",
                    table = false
                });
            }
            return Divisions;
        }
        public void GetHierarchiesPatch(string HierarchydProcedure,ref HierarchyAreaLineaPatch hierarchyAreaLineaPatch, 
            ref HierarchyDepartamentoLineaPatch hierarchyDepartamentoLineaPatch, ref HierarchyLineaPatch hierarchyLineaPatch,ref int estructura)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Hierarchy //Consulta todas las sucursales en la tabla Book_Hierarchy
            List<string> json = new List<string>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;
            OracleDataReader oracleDataReaderPatch = null;
            List<DivisionPost> Divisions = new List<DivisionPost>();
            List<FatherSon> TAreas = new List<FatherSon>();
            List<FatherSon> TDepartments = new List<FatherSon>();
            List<FatherSon> TLines = new List<FatherSon>();
            try
            {
                oracleCommand.Parameters.Add("Hierarchys_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, HierarchydProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            string codigounico = oracleDataReader["hierarchy_id"].ToString().Trim();
                            oracleCommand = new OracleCommand();
                            _iDBOracleRepository.EjecutaSQL(ref oracleDataReaderPatch, ref ErrorMessage,"select * from Bbook_Hierarchy where Area_id = '" + codigounico.Split("-")[1]+ "'");
                            if (!oracleDataReaderPatch.Read())
                            {
                                estructura = 1;
                            }
                            _iDBOracleRepository.EjecutaSQL(ref oracleDataReaderPatch,ref ErrorMessage,"select * from Bbook_Hierarchy where Department_id = '" + codigounico.Split("-")[2] + "'");
                            if (!oracleDataReaderPatch.Read())
                            {
                                estructura = 2;
                            }
                            _iDBOracleRepository.EjecutaSQL(ref oracleDataReaderPatch, ref ErrorMessage, "select * from Bbook_Hierarchy where Line_id = '" + codigounico.Split("-")[3] + "'");
                            if (!oracleDataReaderPatch.Read())
                            {
                                estructura = 3;
                            }
                            Divisions.Add(new DivisionPost()
                            {
                                level_code = oracleDataReader["division_id"].ToString().Trim(),
                                level_name = oracleDataReader["division_name"].ToString().Trim()
                            });
                            TAreas.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["division_id"].ToString().Trim(),
                                SonId = oracleDataReader["area_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["area_name"].ToString().Trim()
                            });
                            TDepartments.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["area_id"].ToString().Trim(),
                                SonId = oracleDataReader["department_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["department_name"].ToString().Trim()
                            });
                            TLines.Add(new FatherSon()
                            {
                                fatherId = oracleDataReader["department_id"].ToString().Trim(),
                                SonId = oracleDataReader["line_id"].ToString().Trim(),
                                SonDescrption = oracleDataReader["line_name"].ToString().Trim()
                            });
                        }
                        Divisions = Divisions.GroupBy(x => x.level_code).Select(group => group.First()).ToList();
                        TAreas = TAreas.GroupBy(x => x.SonId).Select(group => group.First()).ToList();
                        TDepartments = TDepartments.GroupBy(x => x.SonId).Select(group => group.First()).ToList();
                        TLines = TLines.GroupBy(x => x.SonId).Select(group => group.First()).ToList();
                        if (estructura == 1)
                        {
                            for (int i = 0; i < Divisions.Count; i++)
                            {
                                //hierarchyAreaLineaPatch
                                hierarchyAreaLineaPatch = new HierarchyAreaLineaPatch()
                                {
                                    level_code = Divisions[i].level_code,
                                    sublevel = (from a in TAreas where a.fatherId == Divisions[i].level_code select new AreaPatch()
                                    {
                                        level_code = a.SonId,
                                        level_name = a.SonDescrption,
                                        sublevel = (from d in TAreas where d.fatherId == a.SonId select new Departamento()
                                        {
                                            level_code = d.SonId,
                                            level_name = d.SonDescrption,
                                            sublevel = (from l in TLines where l.fatherId == d.SonId 
                                                        select new Linea()
                                            {
                                                level_code = l.SonId,
                                                level_name = l.SonDescrption
                                            }).ToList()
                                        }).ToList()
                                    }).ToList()
                                };
                            }
                        }
                        else if(estructura == 2)
                        {
                            //hierarchyDepartamentoPatch
                            for (int i = 0; i < TAreas.Count; i++)
                            {
                                hierarchyDepartamentoLineaPatch = new HierarchyDepartamentoLineaPatch()
                                {
                                    level_code = TAreas[i].SonId,
                                    sublevel = (from d in TDepartments where d.fatherId == TAreas[i].SonId
                                                select new Departamento()
                                    {
                                        level_code = d.SonId,
                                        level_name = d.SonDescrption,
                                        sublevel = (from l in TLines where l.fatherId == d.SonId
                                                    select new Linea()
                                        {
                                            level_code = l.SonId,
                                            level_name = l.SonDescrption
                                        }).ToList()
                                    }).ToList()
                                };
                            }
                        }
                        else if (estructura == 3)
                        {
                            //hierarchyLineaPatch
                            for (int i = 0; i < TDepartments.Count; i++)
                            {
                                hierarchyLineaPatch = new HierarchyLineaPatch()
                                {
                                    level_code = TDepartments[i].SonId,
                                    sublevel = (from l in TLines where l.fatherId == TDepartments[i].SonId
                                                select new HierarchyLineaPatch.LineaPatch()
                                    {
                                        level_code = l.SonId,
                                        level_name = l.SonDescrption
                                    }).ToList()
                                };
                            }
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPatch",
                        quantity = Divisions.Count,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPatch",
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetHierarchiesPatch",
                    quantity = 0,
                    message = ex.Message,
                    name = "Hierarchy",
                    table = false
                });
            }
        }
        public List<TreePut> GetHierarchiesPut(string HierarchydProcedure,string tipoproceso)
        {
            //Busca y carga sucursales nuevas a la tabla Book_Hierarchy //Consulta todas las sucursales en la tabla Book_Hierarchy
            List<TreePut> treePuts = new List<TreePut>();
            OracleCommand oracleCommand = new OracleCommand();
            OracleDataReader oracleDataReader = null;

            try
            {
                oracleCommand.Parameters.Add("Hierarchys_C", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                if (_iDBOracleRepository.EjecutaSPBbook(ref oracleDataReader, ref oracleCommand,ref ErrorMessage, HierarchydProcedure, true))
                {
                    if(ErrorMessage == "OK")
                    {
                        while (oracleDataReader.Read())
                        {
                            string codigounico = oracleDataReader["hierarchy_id"].ToString().Trim();
                            string nombreunico = oracleDataReader["hierarchy_name"].ToString().Trim();
                            if (oracleDataReader["division_id"].ToString().Trim() != codigounico.Split("-")[0]
                                || oracleDataReader["division_name"].ToString().Trim() != nombreunico.Split("-")[0])
                            {
                                treePuts.Add(new TreePut()
                                {
                                    hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                    level_code = oracleDataReader["division_id"].ToString().Trim(),
                                    level_name = oracleDataReader["division_name"].ToString().Trim()
                                });
                            }
                            else if (oracleDataReader["area_id"].ToString().Trim() != codigounico.Split("-")[1]
                                || oracleDataReader["area_name"].ToString().Trim() != nombreunico.Split("-")[1])
                            {
                                if(oracleDataReader["area_id"].ToString().Trim() != codigounico.Split("-")[1])
                                {
                                    treePuts.Add(new TreePut()
                                    {
                                        hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                        level_code = oracleDataReader["department_id"].ToString().Trim(),
                                        level_name = oracleDataReader["department_name"].ToString().Trim(),
                                        level_parent = oracleDataReader["area_id"].ToString().Trim()
                                    });
                                }
                                else
                                {
                                    treePuts.Add(new TreePut()
                                    {
                                        hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                        level_code = oracleDataReader["area_id"].ToString().Trim(),
                                        level_name = oracleDataReader["area_name"].ToString().Trim()
                                    });
                                }
                            }
                            else if (oracleDataReader["department_id"].ToString().Trim() != codigounico.Split("-")[2]
                                || oracleDataReader["department_name"].ToString().Trim() != nombreunico.Split("-")[2])
                            {
                                if(oracleDataReader["department_id"].ToString().Trim() != codigounico.Split("-")[2])
                                {
                                    treePuts.Add(new TreePut()
                                    {
                                        hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                        level_code = oracleDataReader["line_id"].ToString().Trim(),
                                        level_name = oracleDataReader["line_name"].ToString().Trim(),
                                        level_parent = oracleDataReader["department_id"].ToString().Trim()
                                    });
                                }
                                else
                                {
                                    treePuts.Add(new TreePut()
                                    {
                                        hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                        level_code = oracleDataReader["department_id"].ToString().Trim(),
                                        level_name = oracleDataReader["department_name"].ToString().Trim()
                                    });
                                }
                            }
                            else if (oracleDataReader["line_name"].ToString().Trim() !=
                                oracleDataReader["hierarchy_name"].ToString().Trim().Split("-")[3])
                            {
                                treePuts.Add(new TreePut()
                                {
                                    hierarchy_id = oracleDataReader["hierarchy_id"].ToString().Trim(),
                                    level_code = oracleDataReader["line_id"].ToString().Trim(),
                                    level_name = oracleDataReader["line_name"].ToString().Trim()
                                });
                            }
                        }
                    }
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPut"+ tipoproceso,
                        quantity = treePuts.Count,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
                else
                {
                    pruebaInternas.Add(new PruebaInterna()
                    {
                        id = "00",
                        method = "GetHierarchiesPut"+ tipoproceso,
                        quantity = 0,
                        message = ErrorMessage,
                        name = "Hierarchy",
                        table = false
                    });
                }
            }
            catch (Exception ex)
            {
                pruebaInternas.Add(new PruebaInternaDTO.PruebaInterna()
                {
                    id = "01",
                    method = "GetHierarchiesPut"+ tipoproceso,
                    quantity = 0,
                    message = ex.Message,
                    name = "Hierarchy",
                    table = false
                });
            }
            return treePuts;
        }
        public async Task<HierarchyPostResponse> GetResponsePostAndUpdateTable(HierarchyPostRequest hierarchyPostRequest, string tipo)
        {
            HttpResponseMessage res;
            HierarchyPostResponse hierarchyResponse = new HierarchyPostResponse();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<HierarchyPost> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(hierarchyPostRequest), tipo, "hierarchies");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    hierarchyResponse = JsonConvert.DeserializeObject<HierarchyPostResponse>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in hierarchyPostRequest.data select c.level_code).ToList();
                        foreach (var item in _commonRepository.UpdateDateTableHierarchy("hierarchy", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<HierarchyPost>>(res.Content.ReadAsStringAsync().Result);
                        return new HierarchyPostResponse()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        hierarchyResponse = new HierarchyPostResponse()
                        {
                            internalCode = "JsonError",
                            message = "Error al deserializar Json" + "\n" + ex.Message,
                            statusCode = 00,
                            status = "error"
                        };
                    }
                }
            }
            else
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    hierarchyResponse = new HierarchyPostResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    hierarchyResponse = new HierarchyPostResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    hierarchyResponse = new HierarchyPostResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return hierarchyResponse;
        }
        public async Task<HierarchyPutResponse> GetResponsePutAndUpdateTable(HierarchyPutRequest hierarchyPutRequest, string tipo)
        {
            HttpResponseMessage res;
            HierarchyPutResponse hierarchyResponse = new HierarchyPutResponse();
            List<string> codigos = new List<string>();
            ExceptionalErrorDTO<HierarchyPut> exceptionalError;

            res = await _commonRepository.ApiBbook(JsonConvert.SerializeObject(hierarchyPutRequest), tipo, "hierarchies");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    hierarchyResponse = JsonConvert.DeserializeObject<HierarchyPutResponse>(res.Content.ReadAsStringAsync().Result);
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        codigos = (from c in hierarchyPutRequest.data select c.level_code).ToList();
                        foreach (var item in _commonRepository.UpdateDateTableHierarchy("hierarchy", tipo, codigos))
                        {
                            pruebaInternas.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<HierarchyPut>>(res.Content.ReadAsStringAsync().Result);
                        return new HierarchyPutResponse()
                        {
                            internalCode = exceptionalError.internalCode,
                            message = exceptionalError.message,
                            statusCode = exceptionalError.statusCode,
                            status = exceptionalError.status
                        };
                    }
                    catch
                    {
                        hierarchyResponse = new HierarchyPutResponse()
                        {
                            internalCode = "JsonError",
                            message = "Error al deserializar Json" + "\n" + ex.Message,
                            statusCode = 00,
                            status = "error"
                        };
                    }
                }
            }
            else
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    hierarchyResponse = new HierarchyPutResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 401,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    hierarchyResponse = new HierarchyPutResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 413,
                        status = "error"
                    };
                }
                else if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    hierarchyResponse = new HierarchyPutResponse()
                    {
                        internalCode = res.StatusCode.ToString(),
                        message = res.Content.ReadAsStringAsync().Result,
                        statusCode = 504,
                        status = "error"
                    };
                }
            }
            return hierarchyResponse;
        }
        public async Task<HierarchyPatchResponse> GetResponsePatchAndUpdateTable(IEnumerable<IEnumerable<string>> codigos, string json, string tipo,int estructura)
        {
            HttpResponseMessage res;
            HierarchyPatchResponse hierarchyResponse;
            HierarchyPatchResponse.HierarchyAreaLineaPatchResponse hierarchyAreaLineaResponse = null;
            HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse hierarchyDepartamentoLineaResponse = null;
            HierarchyPatchResponse.HierarchyLineaPatchResponse hierarchyLineaResponse = null;
            ExceptionalErrorDTO<HierarchyPatch> exceptionalError;

            res = await _commonRepository.ApiBbook(json, tipo, "hierarchies");
            if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Conflict)
            {
                if(estructura == 1)
                {
                    try
                    {
                        hierarchyAreaLineaResponse = 
                            JsonConvert.DeserializeObject<HierarchyPatchResponse.HierarchyAreaLineaPatchResponse>(res.Content.ReadAsStringAsync().Result);
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            pruebaInternas = _commonRepository.AddDataTableHierarchy("hierarchy", tipo, codigos);
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<HierarchyPatch>>(res.Content.ReadAsStringAsync().Result);
                            hierarchyAreaLineaResponse = new HierarchyAreaLineaPatchResponse()
                            {
                                internalCode = exceptionalError.internalCode,
                                message = exceptionalError.message,
                                statusCode = exceptionalError.statusCode,
                                status = exceptionalError.status
                            };
                        }
                        catch
                        {
                            hierarchyAreaLineaResponse = new HierarchyAreaLineaPatchResponse()
                            {
                                internalCode = "JsonError",
                                message = "Error al deserializar Json" + "\n" + ex.Message,
                                statusCode = 00,
                                status = "error"
                            };
                        }
                    }
                }
                else if (estructura == 2)
                {
                    try
                    {
                        hierarchyDepartamentoLineaResponse =
                            JsonConvert.DeserializeObject<HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse>(res.Content.ReadAsStringAsync().Result);
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            pruebaInternas = _commonRepository.AddDataTableHierarchy("hierarchy", tipo, codigos);
                        }
                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<HierarchyPatch>>(res.Content.ReadAsStringAsync().Result);
                            hierarchyDepartamentoLineaResponse = new HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse()
                            {
                                internalCode = exceptionalError.internalCode,
                                message = exceptionalError.message,
                                statusCode = exceptionalError.statusCode,
                                status = exceptionalError.status
                            };
                        }
                        catch
                        {
                            hierarchyDepartamentoLineaResponse = new HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse()
                            {
                                internalCode = "JsonError",
                                message = "Error al deserializar Json" + "\n" + ex.Message,
                                statusCode = 00,
                                status = "error"
                            };
                        }
                    }
                }
                else
                {
                    try
                    {
                        hierarchyLineaResponse =
                            JsonConvert.DeserializeObject<HierarchyPatchResponse.HierarchyLineaPatchResponse>(res.Content.ReadAsStringAsync().Result);
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            pruebaInternas = _commonRepository.AddDataTableHierarchy("hierarchy", tipo, codigos);
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            exceptionalError = JsonConvert.DeserializeObject<ExceptionalErrorDTO<HierarchyPatch>>(res.Content.ReadAsStringAsync().Result);
                            hierarchyLineaResponse = new HierarchyPatchResponse.HierarchyLineaPatchResponse()
                            {
                                internalCode = exceptionalError.internalCode,
                                message = exceptionalError.message,
                                statusCode = exceptionalError.statusCode,
                                status = exceptionalError.status
                            };
                        }
                        catch
                        {
                            hierarchyLineaResponse = new HierarchyPatchResponse.HierarchyLineaPatchResponse()
                            {
                                internalCode = "JsonError",
                                message = "Error al deserializar Json" + "\n" + ex.Message,
                                statusCode = 00,
                                status = "error"
                            };
                        }
                    }
                }
            }
            else
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (estructura == 1)
                    {
                        hierarchyAreaLineaResponse = new HierarchyPatchResponse.HierarchyAreaLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 401,
                            status = "error"
                        };
                    }
                    else if(estructura == 2)
                    {
                        hierarchyDepartamentoLineaResponse = new HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 401,
                            status = "error"
                        };
                    }
                    else
                    {
                        hierarchyLineaResponse = new HierarchyPatchResponse.HierarchyLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 401,
                            status = "error"
                        };
                    }
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    if (estructura == 1)
                    {
                        hierarchyAreaLineaResponse = new HierarchyPatchResponse.HierarchyAreaLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 413,
                            status = "error"
                        };
                    }
                    else if(estructura == 2)
                    {
                        hierarchyDepartamentoLineaResponse = new HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 413,
                            status = "error"
                        };
                    }
                    else
                    {
                        hierarchyLineaResponse = new HierarchyPatchResponse.HierarchyLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 413,
                            status = "error"
                        };
                    }
                }
                else if (res.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                {
                    if (estructura == 1)
                    {
                        hierarchyAreaLineaResponse = new HierarchyPatchResponse.HierarchyAreaLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 504,
                            status = "error"
                        };
                    }
                    else if (estructura == 2)
                    {
                        hierarchyDepartamentoLineaResponse = new HierarchyPatchResponse.HierarchyDepartamentoLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 504,
                            status = "error"
                        };
                    }
                    else
                    {
                        hierarchyLineaResponse = new HierarchyPatchResponse.HierarchyLineaPatchResponse()
                        {
                            internalCode = res.StatusCode.ToString(),
                            message = res.Content.ReadAsStringAsync().Result,
                            statusCode = 504,
                            status = "error"
                        };
                    }
                }
            }
            hierarchyResponse = new HierarchyPatchResponse()
            {
                hierarchyAreaLineaPatchResponse = hierarchyAreaLineaResponse,
                hierarchyDepartamentoLineaPatchResponse = hierarchyDepartamentoLineaResponse,
                hierarchyLineaPatchResponse = hierarchyLineaResponse
            };
            return hierarchyResponse;
        }
        public PruebaInterna DeleteBbook_Hierarchy()
        {
            return _commonRepository.DeleteTable("Bbook_Hierarchy");
        }
        #endregion Metodos Extras
    }
}
