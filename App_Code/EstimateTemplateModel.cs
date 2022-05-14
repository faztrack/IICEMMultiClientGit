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
/// Summary description for EstimateTemplateModel
/// </summary>
public class EstimateTemplateModel
{
	public EstimateTemplateModel(){	}
    public int template_id { get; set; }
    public int model_estimate_id { get; set; }
    public int sales_person_id { get; set; }
    public int client_id { get; set; }
    public int status_id { get; set; }
    public string model_estimate_name { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_update_date { get; set; }
    public string estimate_comments { get; set; }
    public string sales_person_name { get; set; }
}
