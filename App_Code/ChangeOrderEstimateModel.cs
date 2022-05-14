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
/// Summary description for ChangeOrderEstimateModel
/// </summary>
public class ChangeOrderEstimateModel
{
	public ChangeOrderEstimateModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int change_order_estimate_id { get; set; }
    public int chage_order_id { get; set; }
    public int estimate_id { get; set; }
    public int customer_id { get; set; }
    public int client_id { get; set; }
    public int sales_person_id { get; set; }
    public int change_order_status_id { get; set; }
    public string changeorder_name { get; set; }
    public int change_order_type_id { get; set; }
    public string payment_terms { get; set; }
    public string other_terms { get; set; }
    public int is_total { get; set; }
    public bool is_tax { get; set; }
    public decimal tax { get; set; }
    public string total_payment_due { get; set; }
    public string execute_date { get; set; }
    public string changeorder_date { get; set; }
    public bool is_close { get; set; }
    public string notes1 { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_updated_date { get; set; }
    public string comments { get; set; }
    public int is_cutomer_viewable { get; set; }
    
}
