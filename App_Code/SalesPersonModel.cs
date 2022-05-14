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
/// Summary description for SalesPersonModel
/// </summary>
public class SalesPersonModel
{
    public SalesPersonModel() { }
    public int client_id { get; set; }
    public int sales_person_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string address { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
    public string phone { get; set; }
    public string fax { get; set; }
    public string email { get; set; }
    public bool is_active { get; set; }
    public string sales_person_code { get; set; }
    public string user_name { get; set; }
    public string password { get; set; }
    public int role_id { get; set; }
    public bool is_sales { get; set; }
    public bool is_service { get; set; }
    public bool is_install { get; set; }
    public DateTime last_login_time { get; set; }
    public DateTime create_date { get; set; }
    public string SalesPesonName { get; set; }
    public decimal com_per { get; set; }
    public decimal co_com_per { get; set; }
    public string google_calendar_account { get; set; }
    public string google_calendar_id { get; set; }
    public string EmpCode { get; set; }
}
public class CrewDe
{
    public int crew_id { get; set; }
    public string crew_name { get; set; }

}
