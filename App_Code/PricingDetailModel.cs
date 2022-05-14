using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for PricingDetailModel
/// </summary>
public class PricingMaster
{
    public int colId { get; set; }
    public string colName { get; set; }
    public int sort_id { get; set; }
    public string LocationNotes { get; set; }
    public string LocationNotesNew { get; set; }

}

public class PricingDetailModel
{
    public PricingDetailModel() { }

    public int pricing_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int location_id { get; set; }
    public int sales_person_id { get; set; }
    public int section_level { get; set; }
    public int item_id { get; set; }
    public int labor_id { get; set; }
    public string item_name { get; set; }
    public string measure_unit { get; set; }
    public decimal item_cost { get; set; }
    public decimal total_retail_price { get; set; }
    public decimal total_direct_price { get; set; }
    public decimal minimum_qty { get; set; }
    public decimal quantity { get; set; }
    public decimal retail_multiplier { get; set; }
    public decimal labor_rate { get; set; }
    public decimal section_serial { get; set; }
    public int item_cnt { get; set; }
    public string pricing_type { get; set; }
    public string short_notes { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_update_date { get; set; }
    public string tmpCol { get; set; }
    public string upload_file_path { get; set; }
    public decimal execution_unit { get; set; }
    public int week_id { get; set; }
    public bool is_complete { get; set; }
    public string schedule_note { get; set; }
    public int sort_id { get; set; }
    public bool is_mandatory { get; set; }
    public bool is_CommissionExclude { get; set; }

    public decimal unit_cost { get; set; }
    public decimal total_unit_cost { get; set; }
    public decimal total_labor_cost { get; set; }
      

}

public class PMainSummary
{
    public int MainID { get; set; }
    public string colName { get; set; }
    public int SortId { get; set; }
    public string MainName { get; set; }
    public string SummaryName { get; set; }
    public decimal ProjectCost { get; set; }

}


