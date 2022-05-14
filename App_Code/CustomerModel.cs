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
/// Summary description for CustomerModel
/// </summary>
public class CustomerModel
{

    public CustomerModel() { }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public string first_name1 { get; set; }
    public string last_name1 { get; set; }
    public string first_name2 { get; set; }
    public string last_name2 { get; set; }
    public string address { get; set; }
    public string cross_street { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip_code { get; set; }
    public string fax { get; set; }
    public string email { get; set; }
    public string email2 { get; set; }
    public string phone { get; set; }
    public bool is_active { get; set; }
    public DateTime registration_date { get; set; }
    public int sales_person_id { get; set; }
    public DateTime update_date { get; set; }
    public int status_id { get; set; }
    public string notes { get; set; }
    public string customer_name { get; set; }
    public int lead_source_id { get; set; }
    public int SuperintendentId { get; set; }


}
