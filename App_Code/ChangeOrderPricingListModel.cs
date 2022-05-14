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
/// Summary description for ChangeOrderPricingListModel
/// </summary>
public class ChangeOrderPricingListModel
{
	public ChangeOrderPricingListModel()
	{
	}
    public int co_pricing_item_id { get; set; }
    public int co_pricing_list_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int chage_order_id { get; set; }
    public int location_id { get; set; }
    public int sales_person_id { get; set; }
    public int section_level { get; set; }
    public int item_id { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public string item_name { get; set; }
    public decimal total_retail_price { get; set; }
    public decimal total_direct_price { get; set; }
    public int is_direct { get; set; }
    public int item_status_id { get; set; }
    public int EconomicsId { get; set; }
    public decimal EconomicsCost { get; set; }
    public decimal section_serial { get; set; }
    public decimal quantity { get; set; }
    public string measure_unit { get; set; }
    public string short_notes { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_update_date { get; set; }
  
}
