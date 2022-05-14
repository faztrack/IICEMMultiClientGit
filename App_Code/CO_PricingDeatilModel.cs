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
/// Summary description for CO_PricingDeatilModel
/// </summary>
public class CO_PricingMaster
{
    public int colId { get; set; }
    public string colName { get; set; }
    public int sort_id { get; set; }
}
public class CO_PricingDeatilModel
{
	public CO_PricingDeatilModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int co_pricing_list_id { get; set; }
    public int item_status_id { get; set; }
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
    public decimal prev_total_price { get; set; }
    public decimal minimum_qty { get; set; }
    public decimal quantity { get; set; }
    public decimal retail_multiplier { get; set; }
    public decimal labor_rate { get; set; }
    public decimal section_serial { get; set; }
    public int item_cnt { get; set; }
    public int other_item_cnt { get; set; }
    public int is_direct { get; set; }
    public string pricing_type { get; set; }
    public string short_notes { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_update_date { get; set; }
    public string tmpCol { get; set; }
    public decimal execution_unit { get; set; }
    public int week_id { get; set; }
    public bool is_complete { get; set; }
    public string schedule_note { get; set; }
    public int sort_id { get; set; }
    public int CalEventId { get; set; }
    public bool is_Scheduled { get; set; }

    public decimal unit_cost { get; set; }
    public decimal total_unit_cost { get; set; }
    public decimal total_labor_cost { get; set; }

    public string short_notes_new { get; set; }
    public bool is_mandatory { get; set; }
    public string IsItemFlag { get; set; }
    public  int co_pricing_item_id { get; set; }
    
    
}
