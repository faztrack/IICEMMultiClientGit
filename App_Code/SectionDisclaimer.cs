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
/// Summary description for SectionDisclaimer
/// </summary>
public class SectionDisclaimer
{
	public SectionDisclaimer()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int disclaimer_id { get; set; }
    public int section_level { get; set; }
    public string section_name { get; set; }
    public string section_heading { get; set; }
    public string disclaimer_name { get; set; }
    public int client_id { get; set; }
    public bool IsInitilal { get; set; }
}
