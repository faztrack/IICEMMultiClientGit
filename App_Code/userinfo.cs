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
/// Summary description for userinfo
/// </summary>
public class userinfo
{
	public userinfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string client_id { get; set; }
    public int user_id { get; set; }
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
    public string username { get; set; }
    public string password { get; set; }
    public int role_id { get; set; }
    public bool is_sales { get; set; }
    public bool is_service { get; set; }
    public bool is_install { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_login_time { get; set; }
    public string sales_person_name { get; set; }
    public string company_email { get; set; }
    public string email_password { get; set; }
    public bool is_verify { get; set; }
    public string Superintendent_name { get; set; }
    public int EmailIntegrationType { get; set; }
    public string EmailIntegration { get; set; }
    public bool IsTimeClock { get; set; }
    public bool IsPriceChange { get; set; }
}
// FaztrackPagePermission
public class csUserPagePermission
{
    public csUserPagePermission()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int menu_id { get; set; }
    public string menu_name { get; set; }
    public string menu_url { get; set; }
    public bool IsWrite { get; set; }
}

