using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.User != null)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated == false)
            {
                Server.Transfer(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
        }
        else
        {
            Server.Transfer(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }

        if (!IsPostBack)
        {
            Response.Redirect("customerlist.aspx");
        }
    }
    protected void btnLogIn_Click(object sender, EventArgs e)
    {
          lblResult.Text = "";

        if (txtUserName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please enter your username.");
            
            return;
        }
        if (txtPassword.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please enter password.");
            
            return;
        }
        if (txtUserName.Text == "test" && txtPassword.Text.Trim() == "123456")
        {
            LinkButton lnk = (LinkButton)Master.FindControl("lnkSignIn");
            lnk.Text = "Sign Out: Test";
            string str = "Test";
            Session.Add("oUser", str);
            Response.Redirect("customerlist.aspx");
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session.Remove("oUser");
        Response.Redirect("Default.aspx");
        LinkButton lnk = (LinkButton)Master.FindControl("lnkSignin");
        lnk.Text = "Sign In";
    }
    
   
}
