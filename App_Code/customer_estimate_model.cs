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
/// Summary description for customer_estimate_model
/// </summary>
public class customer_estimate_model
{
	public customer_estimate_model()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int customer_estimate_id { get; set; }
    public int estimate_id { get; set; }
    public int customer_id { get; set; }
    public int client_id { get; set; }
    public int sales_person_id { get; set; }
    public int status_id { get; set; }
    public string estimate_name { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_update_date { get; set; }
    public string sale_date { get; set; }
    public string estimate_comments { get; set; }
    public string job_number { get; set; }
    public bool IsEstimateActive { get; set; }
    public string alter_job_number { get; set; }

}

public class customer_CallNotes
{
    public customer_CallNotes()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string CallActivity { get; set; }

}
