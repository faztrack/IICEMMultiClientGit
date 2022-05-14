using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class customerlogin : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            string strUseHTTPS = System.Configuration.ConfigurationManager.AppSettings["UseHTTPS"];
            if (strUseHTTPS == "Yes")
            {
                if (Request.Url.Scheme == "http")
                {
                    string strQuery = Request.Url.AbsoluteUri;
                    Response.Redirect(strQuery.Replace("http:", "https:"));
                }
            }
            
        }
    }
}
