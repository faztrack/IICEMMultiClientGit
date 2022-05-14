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
/// Summary description for ItemPriceModel
/// </summary>
public class ItemPriceModel
{
    public ItemPriceModel() { }
    public int client_id { get; set; }
    public int item_id { get; set; }
    public string section_name { get; set; }
    public string measure_unit { get; set; }
    public decimal item_cost { get; set; }
    public decimal minimum_qty { get; set; }
    public decimal retail_multiplier { get; set; }
    public decimal labor_rate { get; set; }
    public DateTime update_time { get; set; }
    public decimal section_serial { get; set; }
    public int labor_id { get; set; }
    public decimal ext_item_cost { get; set; }
    public bool is_mandatory { get; set; }
    public decimal LaborUnitCost { get; set; }

    
}

public class SectionItemPriceModel
{
    public SectionItemPriceModel() { }
    public int client_id { get; set; }
    public int item_id { get; set; }
    public string section_name { get; set; }
    public string measure_unit { get; set; }
    public decimal item_cost { get; set; }
    public decimal minimum_qty { get; set; }
    public decimal retail_multiplier { get; set; }
    public decimal labor_rate { get; set; }
    public DateTime update_time { get; set; }
    public int labor_id { get; set; }
   
    public int section_id { get; set; }
    public int parent_id { get; set; }
    public string section_notes { get; set; }
    public int section_level { get; set; }
    public decimal section_serial { get; set; }
    public bool is_active { get; set; }
    public DateTime create_date { get; set; }
    public bool is_mandatory { get; set; }
    public bool is_CommissionExclude { get; set; }
    
    
}

