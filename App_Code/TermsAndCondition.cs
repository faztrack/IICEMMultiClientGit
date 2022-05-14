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
/// Summary description for TermsAndCondition
/// </summary>
public class TermsAndCondition
{
	public TermsAndCondition()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int terms_condition_id{ get; set; }
    public int client_id{ get; set; }
    public string terms_condition{ get; set; }
    public string terms_header { get; set; }
    public bool IsInitilal { get; set; }
    
}
