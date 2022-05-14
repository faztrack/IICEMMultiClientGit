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
/// Summary description for StatusDescription
/// </summary>
public class StatusDescription
{
	public StatusDescription()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int jobstatus_desc_id { get; set; }
    public int jobstatusid { get; set; }
    public int customer_id { get; set; }
    public string status_description { get; set; }
    public int status_serial { get; set; }
    public int estimate_id { get; set; }
}
