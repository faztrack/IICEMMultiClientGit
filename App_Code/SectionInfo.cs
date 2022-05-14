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
/// Summary description for SectionInfo
/// </summary>
public class SectionInfo
{
    public SectionInfo() { }
    public int client_id { get; set; }
    public int section_id { get; set; }
    public string section_name { get; set; }
    public int parent_id { get; set; }
    public string section_notes { get; set; }
    public int section_level { get; set; }
    public int sorting_order { get; set; }
    public decimal section_serial { get; set; }
    public bool is_active { get; set; }
    public DateTime create_date { get; set; }
    public string cssClassName { get; set; }
    public bool is_CommissionExclude { get; set; }
    
}


