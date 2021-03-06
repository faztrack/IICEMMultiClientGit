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

public partial class schedulemaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            // lnkSignOut.Text = "Welcome " + this.Context.User.Identity.Name + ", (Logout)";

            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                divTestSystem.Visible = true;
            }

            string strName = string.Empty;
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo objUser = (userinfo)Session["oUser"];
                strName = objUser.first_name;
            }

            lnkProfile.Text = "Welcome " + strName + "";

            GetMenuData();
            GetTools();
            string thispage = this.Page.AppRelativeVirtualPath;
            int slashpos = thispage.LastIndexOf('/');
            string pagename = thispage.Substring(slashpos + 1);

            foreach (MenuItem mi in menuBar.Items)
            {
                if (mi.ChildItems.Count > 0)
                    for (int i = 0; i < mi.ChildItems.Count; i++)
                    {
                        var ttt = mi.ChildItems[i].NavigateUrl;
                        if (mi.ChildItems[i].NavigateUrl.Contains(pagename))
                        {
                            mi.ChildItems[i].Selected = true;
                            mi.ChildItems[i].Parent.Selected = true;
                            break;
                        }

                    }
                else if (mi.NavigateUrl.Contains(pagename) && mi.Text != "Home")
                {
                    mi.Selected = true;
                    break;
                }
            }


            foreach (MenuItem mi in menuSettings.Items)
            {
                if (mi.ChildItems.Count > 0)
                    for (int i = 0; i < mi.ChildItems.Count; i++)
                    {
                        var ttt = mi.ChildItems[i].NavigateUrl;
                        var mcName = mi.ChildItems[i].Text;
                        if (mi.ChildItems[i].NavigateUrl.Contains(pagename))
                        {
                            var tttt = mi.ChildItems[i].Text;
                            mi.ChildItems[i].Selectable = true;
                            // mi.ChildItems[i].Text = "<span class='StaticSelectedStyle' >" + mi.ChildItems[i].Text + "</span>"; 
                            mi.ChildItems[i].Parent.ChildItems[i].Selected = true;
                            mi.ChildItems[i].Parent.Selected = true;
                            break;
                        }
                    }
                else if (mi.NavigateUrl.Contains(pagename))
                {
                    mi.Selected = true;
                    break;
                }

            }
        }
        //if (!IsPostBack)
        //{
        //    LoadMenu();

        //}
        // if (Request.Url.OriginalString.ToLower().IndexOf("customer_details.aspx") > 0 || Request.Url.OriginalString.ToLower().IndexOf("payment_recieved.aspx") > 0 || Request.Url.OriginalString.ToLower().IndexOf("vendor_cost_details.aspx") > 0)
        //{
        //     LoadMenu();
        //}
        //if (Session["oUser"] != null)
        //{
        //    userinfo obj = (userinfo)Session["oUser"];

        //    lnkSignIn.Text = "Sign Out: " + obj.first_name + " " + obj.last_name;
        //}
    }

    protected void btnMap_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("gmap.aspx");
    }

    protected void lnkProfile_Click(object sender, EventArgs e)
    {

        Response.Redirect("user_profile.aspx");
    }



    private void GetMenuData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<menu_item> menulist = new List<menu_item>();

        menulist = _db.menu_items.Where(p => p.parent_id == 0).OrderBy(s => s.serial).ToList();

        foreach (menu_item m in menulist.Where(p => p.menu_id != 1))
        {
            MenuItem menuItem = new MenuItem(m.menu_name.ToString(), m.menu_id.ToString());
            menuItem.NavigateUrl = m.menu_url ?? "".ToString();
            menuBar.Items.Add(menuItem);
            AddChildItems(menulist, menuItem);
        }

        foreach (menu_item m in menulist.Where(p => p.menu_id == 1).OrderBy(s => s.serial).ToList())
        {
            MenuItem menuItem = new MenuItem(m.menu_name.ToString(), m.menu_id.ToString());
            menuItem.NavigateUrl = m.menu_url ?? "".ToString();
            menuItem.Text = "";
            menuSettings.Items.Add(menuItem);
            AddSettingsChildItems(menulist.Where(p => p.menu_id == 1).ToList(), menuItem);
        }
    }

    private void AddChildItems(List<menu_item> menulist, MenuItem menuItem)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<menu_item> mlist = new List<menu_item>(menulist);

        mlist = _db.menu_items.Where(m => m.parent_id == Convert.ToInt32(menuItem.Value) && m.isShow == 1).OrderBy(s => s.serial).ToList();

        foreach (menu_item c in mlist)
        {
            MenuItem childItem = new MenuItem(c.menu_name.ToString(), c.menu_id.ToString());
            childItem.NavigateUrl = c.menu_url ?? "".ToString();
            menuItem.ChildItems.Add(childItem);
            AddChildItems(mlist, childItem);
        }
    }

    private void AddSettingsChildItems(List<menu_item> menulist, MenuItem menuItem)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<menu_item> mlist = new List<menu_item>(menulist);

        mlist = _db.menu_items.Where(m => m.parent_id == Convert.ToInt32(menuItem.Value) && m.isShow == 1).OrderBy(s => s.serial).ToList();

        foreach (menu_item c in mlist)
        {
            MenuItem childItem = new MenuItem(c.menu_name.ToString(), c.menu_id.ToString());
            childItem.NavigateUrl = c.menu_url ?? "".ToString();
            menuItem.ChildItems.Add(childItem);
            AddSettingsChildItems(mlist, childItem);
        }
    }

    protected void btnOperationCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=1");
    }

    private void GetTools()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string tools = string.Empty;
            userinfo obj = (userinfo)Session["oUser"];
            user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == obj.user_id);
            if (objUser != null)
            {
                if (objUser.tools.Contains("Message"))
                    chkMessage.Checked = true;
                else
                    chkMessage.Checked = false;
                if (objUser.tools.Contains("Vendor"))
                    chkVendor.Checked = true;
                else
                    chkVendor.Checked = false;

                if (objUser.tools.Contains("Payment"))
                    chkPayment.Checked = true;
                else
                    chkPayment.Checked = false;
                if (objUser.tools.Contains("JobStatus"))
                    chkJobStatus.Checked = true;
                else
                    chkJobStatus.Checked = false;
                if (objUser.tools.Contains("Schedule"))
                    chkSchedule.Checked = true;
                else
                    chkSchedule.Checked = false;
                if (objUser.tools.Contains("CompositeSow"))
                    chkCompositeSow.Checked = true;
                else
                    chkCompositeSow.Checked = false;

                if (objUser.tools.Contains("ProjectNotes"))
                    chkProjectNotes.Checked = true;
                else
                    chkProjectNotes.Checked = false;
                if (objUser.tools.Contains("AllowanceReport"))
                    chkAllowanceReport.Checked = true;
                else
                    chkAllowanceReport.Checked = false;
                if (objUser.tools.Contains("ActivityLog"))
                    chkActivityLog.Checked = true;
                else
                    chkActivityLog.Checked = false;
                if (objUser.tools.Contains("PreConCheckList"))
                    chkPreConCheckList.Checked = true;
                else
                    chkPreConCheckList.Checked = false;
                if (objUser.tools.Contains("SiteReview"))
                    chkSiteReview.Checked = true;
                else
                    chkSiteReview.Checked = false;
                if (objUser.tools.Contains("DocumentManagement"))
                    chkDocumentManagement.Checked = true;
                else
                    chkDocumentManagement.Checked = false;
                if (objUser.tools.Contains("Selection"))
                    chkSelection.Checked = true;
                else
                    chkSelection.Checked = false;
                if (objUser.tools.Contains("ProjectSummary"))
                    chkProjectSummary.Checked = true;
                else
                    chkProjectSummary.Checked = false;


            }
        }
        catch (Exception ex)
        {
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string tools = string.Empty;
            userinfo obj = (userinfo)Session["oUser"];
            user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == obj.user_id);
            if (objUser != null)
            {
                if (chkMessage.Checked)
                    tools = "Message,";
                if (chkVendor.Checked)
                    tools += "Vendor,";
                if (chkPayment.Checked)
                    tools += "Payment,";
                if (chkJobStatus.Checked)
                    tools += "JobStatus,";
                if (chkSchedule.Checked)
                    tools += "Schedule,";
                if (chkCompositeSow.Checked)
                    tools += "CompositeSow,";
                if (chkProjectNotes.Checked)
                    tools += "ProjectNotes,";
                if (chkAllowanceReport.Checked)
                    tools += "AllowanceReport,";
                if (chkActivityLog.Checked)
                    tools += "ActivityLog,";
                if (chkPreConCheckList.Checked)
                    tools += "PreConCheckList,";
                if (chkSiteReview.Checked)
                    tools += "SiteReview,";
                if (chkDocumentManagement.Checked)
                    tools += "DocumentManagement,";
                if (chkSelection.Checked)
                    tools += "Selection,";
                if (chkProjectSummary.Checked)
                    tools += "ProjectSummary,";

                objUser.tools = tools.TrimEnd(',');
                _db.SubmitChanges();
                Response.Redirect(Request.RawUrl);
            }
        }
        catch (Exception ex)
        {
        }

    }
}
