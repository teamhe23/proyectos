using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PrecioProgramadoRequest
    {
        public PrecioProgramado             cabecera    { get; set; }
        public List<PrecioProgramadoDet>    productos   { get; set; }

        public PrecioProgramadoRequest()
        {
            this.cabecera       = new PrecioProgramado();
            this.productos      = new List<PrecioProgramadoDet>();
        }
    }

    public class PrecioProgramado
    {
        public Int64    cap_chain_id    { get; set; }
        public Int64    prc_type        { get; set; }
        public string   fecha_inicio    { get; set; } = string.Empty;
        public string   descripcion     { get; set; } = string.Empty;
        public string   usuario         { get; set; } = string.Empty;
        public string   userhost        { get; set; } = string.Empty;
    }

    public class PrecioProgramadoDet
    {
        public Int64    id_sucursal     { get; set; }
        public Int64    prd_lvl_child   { get; set; }
        public decimal  precio          { get; set; }
        public string   cod_umv         { get; set; } = string.Empty;
        public int?     qty_ini_ran1    { get; set; }
        public decimal? pct_ran1        { get; set; }
        public decimal? prc_ran1        { get; set; }
        public int?     qty_ini_ran2    { get; set; }
        public decimal? pct_ran2        { get; set; }
        public decimal? prc_ran2        { get; set; }
        public bool     ind_reca_triprc { get; set; }
        public bool     ind_calc_triprc { get; set; }
    }

    public class PrecioProgramadoResponse
    {
        public Int64                               num_eve_prc { get; set; }
        public List<PrecioProgramadoResponseError> errores     { get; set; }

        public PrecioProgramadoResponse()
        {
            this.errores = new List<PrecioProgramadoResponseError>();
        }
    }

    public class PrecioProgramadoResponseError
    {
        public Int64    org_lvl_number  { get; set; }
        public string   prd_lvl_number  { get; set; } = string.Empty;
        public string   rej_desc        { get; set; } = string.Empty;
    }


    public class CurProducto
    {
        public Int64 aux { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string cod_umv { get; set; } = string.Empty;
    }

    public class CurTri
    {
        public Int64 org_lvl_child { get; set; }
        public Int64 prd_lvl_child { get; set; }
    }

    public class CurPrecio
    {
        public Int64 prd_lvl_child { get; set; }
        public string prd_lvl_number { get; set; } = string.Empty;
        public Int64 org_lvl_child { get; set; }
        public Int64 org_lvl_number { get; set; }
        public string org_name_full { get; set; } = string.Empty;
        public Int64 precio_actual { get; set; }
        public Int64 costo { get; set; }
        public Int64? cant_1 { get; set; }
        public Int64? prc_1 { get; set; }
        public Int64? cant_2 { get; set; }
        public Int64? prc_2 { get; set; }

}



    public class CurLiq
    {      
        public Int64 prd_lvl_number { get; set; }
    }

    public class PrecioProgramadoBuscar
    {
        public Int64        sec_te_prc      { get; set; }
        public string       cap_chain_name  { get; set; } = string.Empty;
        public string       prc_type_name   { get; set; } = string.Empty;
        public DateTime     prd_eff_date    { get; set; }
        public string       prc_hdr_name    { get; set; } = string.Empty;
        public Int64        org_lvl_number  { get; set; }
        public string       org_name_full   { get; set; } = string.Empty;
        public string       prd_lvl_number  { get; set; } = string.Empty;
        public string       prd_full_name   { get; set; } = string.Empty;
        public decimal      prd_prc_price   { get; set; }
        public decimal?     qty_ini_ran1    { get; set; }
        public decimal?     pct_ran1        { get; set; }
        public decimal?     prd_prc_ran1    { get; set; }
        public decimal?     qty_ini_ran2    { get; set; }
        public decimal?     pct_ran2        { get; set; }
        public decimal?     prd_prc_ran2    { get; set; }
        public int?         ind_triprc      { get; set; }
        public int?         ind_calc_triprc { get; set; }
        public int?         rej_code        { get; set; }
        public string       rej_desc        { get; set; } = string.Empty;
        public string       usr_cre         { get; set; } = string.Empty;
        public DateTime     fec_cre         { get; set; }
    }

    
}
