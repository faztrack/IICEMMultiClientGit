using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CustomerScheduleMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                divTestSystem.Visible = true;
            }

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

        int nCustomerId = Convert.ToInt32(objCustUser.customerid);
        int nEstimateId = 0;
        bool isAddSOWItem = false;

        if (objCustUser != null)
        {


            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == nCustomerId);
            string strName = cust.last_name1;

            MenuItem menuBarItem1 = new MenuItem("Dashboard", "1");
            menuBarItem1.NavigateUrl = "customerdashboard.aspx";
            menuBar.Items.Add(menuBarItem1);

            MenuItem menuBarItem2 = new MenuItem("Selections", "2");
            menuBarItem2.NavigateUrl = "CustomerSelectionInfoSec.aspx";
            menuBar.Items.Add(menuBarItem2);

            MenuItem menuBarItem3 = new MenuItem("Schedule", "3");
            menuBarItem3.NavigateUrl = "customerschedulecalendar.aspx";
            menuBar.Items.Add(menuBarItem3);

            MenuItem menuBarItem4 = new MenuItem("SOW", "4");
            if (_db.customer_estimates.Where(ce => ce.customer_id == nCustomerId && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
            {
                int nEstId = 0;
                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == nCustomerId && ce.client_id == 1 && ce.status_id == 3
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();

                if (_db.pricing_details.Any(p => p.estimate_id == nEstId && p.customer_id == nCustomerId))
                {
                    nEstimateId = nEstId;

                    isAddSOWItem = true;
                }
                else
                {
                    nEstimateId = (nEstId - 1);
                }
            }
            else
            {
                int nEstId = 0;
                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == nCustomerId && ce.client_id == 1
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();

                if (_db.pricing_details.Any(p => p.estimate_id == nEstId && p.customer_id == nCustomerId))
                {
                    nEstimateId = nEstId;

                    isAddSOWItem = true;
                }
                else
                {
                    nEstimateId = (nEstId - 1);

                }
            }


            menuBarItem4.NavigateUrl = "customer_sow.aspx?eid=" + nEstimateId + "&cid=" + nCustomerId;


            if (isAddSOWItem)
            {
                menuBar.Items.Add(menuBarItem4);
            }

            MenuItem menuBarItem5 = new MenuItem("Your Profile", "5");
            menuBarItem5.NavigateUrl = "customerprofile.aspx";
            menuBar.Items.Add(menuBarItem5);

            MenuItem menuBarItem6 = new MenuItem("Card Settings", "6");
            menuBarItem6.NavigateUrl = "PaymentOptions.aspx";
            menuBar.Items.Add(menuBarItem6);

            MenuItem menuBarItem7 = new MenuItem("Site Review", "7");
            menuBarItem7.NavigateUrl = "customersitereview.aspx";
            menuBar.Items.Add(menuBarItem7);

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
