using IntegracionBbook.Models.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Models.Hierarchy
{
    public class HierarchyDTO
    {
        public HierarchyPost HierarchyPost { get; set; }
        public HierarchyPatch HierarchyPatch { get; set; }
        public HierarchyPut HierarchyPut { get; set; }
    }
    public class HierarchyPost
    {
        public List<HierarchyPostRequest> hierarchyPostRequests { get; set; }
        public List<HierarchyPostResponse> hierarchyPostResponses { get; set; }
        public PruebaInternaDTO pruebaInterna { get; set; }
        public class HierarchyPostRequest : RequestGeneral
        {
            [JsonProperty(PropertyName = "data")]
            public List<DivisionPost> data { get; set; }
        }
        public class HierarchyPostResponse : ResponseBbookDTO
        {
            public List<DivisionPost> data { get; set; }
            public List<ErrorHierarchyPost> errors { get; set; }
        }
        public class ErrorHierarchyPost : ErrorDTO<DivisionPost> { }
        public class DivisionPost : Hierarchy
        {
            public bool inactive => false;
            public List<AreaPost> sublevel { get; set; }
        }
        public class AreaPost : Hierarchy
        {
            public List<Departamento> sublevel { get; set; }
        }
    }
    public class HierarchyPatch
    {
        public List<HierarchyPatchRequest> hierarchyPatchRequests { get; set; }
        public List<HierarchyPatchResponse> hierarchyPatchResponses { get; set; }
        public PruebaInternaDTO pruebaInterna { get; set; }
        public class HierarchyPatchRequest
        {
            public HierarchyAreaLineaPatchRequest hierarchyAreaLineaPatchRequest { get; set; }
            public HierarchyDepartamentoLineaPatchRequest hierarchyDepartamentoLineaPatchRequest { get; set; }
            public HierarchyLineaPatchRequest hierarchyLineaPatchRequest { get; set; }
            public class HierarchyAreaLineaPatchRequest
            {
                [JsonProperty(PropertyName = "data")]
                public List<HierarchyAreaLineaPatch> data { get; set; }
            }
            public class HierarchyDepartamentoLineaPatchRequest
            {
                [JsonProperty(PropertyName = "data")]
                public List<HierarchyDepartamentoLineaPatch> data { get; set; }
            }
            public class HierarchyLineaPatchRequest
            {
                [JsonProperty(PropertyName = "data")]
                public List<HierarchyLineaPatch> data { get; set; }
            }
        }
        public class HierarchyPatchResponse
        {
            public HierarchyAreaLineaPatchResponse hierarchyAreaLineaPatchResponse { get; set; }
            public HierarchyDepartamentoLineaPatchResponse hierarchyDepartamentoLineaPatchResponse { get; set; }
            public HierarchyLineaPatchResponse hierarchyLineaPatchResponse { get; set; }
            public class HierarchyAreaLineaPatchResponse : ResponseBbookDTO
            {
                public List<HierarchyAreaLineaPatch> data { get; set; }
                public List<ErrorHierarchyAreaLineaPatch> errors { get; set; }
            }
            public class HierarchyDepartamentoLineaPatchResponse : ResponseBbookDTO
            {
                public List<HierarchyDepartamentoLineaPatch> data { get; set; }
                public List<ErrorHierarchyDepartamentoLineaPatch> errors { get; set; }
            }
            public class HierarchyLineaPatchResponse : ResponseBbookDTO
            {
                public List<HierarchyLineaPatch> data { get; set; }
                public List<ErrorHierarchyLineaPatch> errors { get; set; }
            }
        }
        public class ErrorHierarchyAreaLineaPatch : ErrorDTO<HierarchyAreaLineaPatch> { }
        public class ErrorHierarchyDepartamentoLineaPatch : ErrorDTO<HierarchyDepartamentoLineaPatch> { }
        public class ErrorHierarchyLineaPatch : ErrorDTO<HierarchyLineaPatch> { }
    }
    public class HierarchyPut
    {
        public List<HierarchyPutRequest> hierarchyPatchRequests { get; set; }
        public List<HierarchyPutResponse> hierarchyPutResponses { get; set; }
        public PruebaInternaDTO pruebaInterna { get; set; }
        public class HierarchyPutRequest
        {
            [JsonProperty(PropertyName = "data")]
            public List<TreePut> data { get; set; }
        }
        public class HierarchyPutResponse : ResponseBbookDTO
        {
            public List<TreePut> data { get; set; }
            public List<ErrorHierarchyPut> errors { get; set; }
        }
        public class TreePut
        {
            [JsonIgnore]
            public string hierarchy_id { get; set; }
            public string level_code { get; set; }
            public string level_name { get; set; }
            public string level_parent { get; set; }
            public bool inactive => false;
        }
        public class ErrorHierarchyPut : ErrorDTO<TreePut> { }
    }
    public class Departamento : Hierarchy
    {
        public List<Linea> sublevel { get; set; }
    }
    public class Linea : Hierarchy { }
    public class Hierarchy
    {
        [JsonIgnore]
        public string hierarchy_id { get; set; }
        public string level_code { get; set; }
        public string level_name { get; set; }
    }
    public class HierarchyAreaLineaPatch
    {
        public string level_code { get; set; }
        public List<AreaPatch> sublevel { get; set; }
        public class AreaPatch : Hierarchy
        {
            public List<Departamento> sublevel { get; set; }
        }
    }
    public class HierarchyDepartamentoLineaPatch
    {
        public string level_code { get; set; }
        public List<Departamento> sublevel { get; set; }
    }
    public class HierarchyLineaPatch
    {
        public string level_code { get; set; }
        public List<LineaPatch> sublevel { get; set; }
        public class LineaPatch : Hierarchy { }
    }
}