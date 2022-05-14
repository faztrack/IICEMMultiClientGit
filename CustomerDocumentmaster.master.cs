using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Security.Principal;

public partial class CustomerDocumentmaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            GetMenuData();

            string thispage = this.Page.AppRelativeVirtualPath;
            int slashpos = thispage.LastIndexOf('/');
            string pagename = thispage.Substring(slashpos + 1);

            foreach (MenuItem mi in menuBar.Items)
            {
                if (mi.NavigateUrl.Contains(pagename))
                {
                    mi.Selected = true;
                    break;
                }
            }
        }
    }

    protected void lnkSignIn_Click(object sender, EventArgs e)
    {
        Session.RemoveAll();
        Session.Remove("oCustomerUser");
        Response.Redirect("customerlogin.aspx");
    }


    private void GetMenuData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customeruserinfo objCustUser = (customeruserinfo)Session["oCustomerUser"];
        if (objCustUser != null)
        {
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(objCustUser.customerid));
            string strName = cust.last_name1;

            MenuItem menuBarItem1 = new MenuItem("Dashboard", "1");
            menuBarItem1.NavigateUrl = "customerdashboard.aspx";
            menuBar.Items.Add(menuBarItem1);

            MenuItem menuBarItem2 = new MenuItem("Selections", "2");
            menuBarItem2.NavigateUrl = "CustomerSectionSelection.aspx";
            menuBar.Items.Add(menuBarItem2);

            //MenuItem menuBarItem3 = new MenuItem("Document Management", "3");
            //menuBarItem3.NavigateUrl = "CustomerDocumentManagement.aspx";
            //menuBar.Items.Add(menuBarItem3);

            MenuItem menuBarItem4 = new MenuItem("Your Profile", "4");
            menuBarItem4.NavigateUrl = "customerprofile.aspx";
            menuBar.Items.Add(menuBarItem4);

            MenuItem menuBarItem5 = new MenuItem("Card Settings", "5");
            menuBarItem5.NavigateUrl = "PaymentOptions.aspx";
            menuBar.Items.Add(menuBarItem5);


            lnkSignOut.Text = "Welcome " + strName + ", (Logout)";
            //MenuItem menuSettingsItem1 = new MenuItem("Welcome " + strName + ", (Logout)", "4");
            //menuSettingsItem1.NavigateUrl = "customerlogin.aspx";
            //menuSettings.Items.Add(menuSettingsItem1);

            //MenuItem menuSettingsItem1 = new MenuItem("", "");
            //menuSettingsItem1.NavigateUrl = "#";
            //menuSettings.Items.Add(menuSettingsItem1);
            //AddSettingsChildItems(menuSettingsItem1);       
        }

    }

    private void AddSettingsChildItems(MenuItem menuItem)
    {
        MenuItem menuSettingsChildItem1 = new MenuItem("Sign Out", "3");
        menuSettingsChildItem1.NavigateUrl = "customerlogin.aspx";
        menuItem.ChildItems.Add(menuSettingsChildItem1);
    }

    protected void lnkSignOut_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkSignOut.ID, lnkSignOut.GetType().Name, "Click"); 
        Session.RemoveAll();

        if (HttpContext.Current.User.Identity.IsAuthenticated == false)
        {

            Response.Redirect("customerlogin.aspx");
            return;
        }
        FormsAuthentication.SignOut();
        GenericIdentity identity = new GenericIdentity("", "");
        // This principal will flow throughout the request.
        GenericPrincipal principal = new GenericPrincipal(identity, new string[] { });
        // Attach the new principal object to the current HttpContext object
        HttpContext.Current.User = principal;
        Response.Redirect("customerlogin.aspx");
    }
}
