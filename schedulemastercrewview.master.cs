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

public partial class schedulemastercrewview : System.Web.UI.MasterPage
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

            //lnkProfile.Text = "Welcome " + strName + "";

            //GetMenuData();

            //string thispage = this.Page.AppRelativeVirtualPath;
            //int slashpos = thispage.LastIndexOf('/');
            //string pagename = thispage.Substring(slashpos + 1);

            //foreach (MenuItem mi in menuBar.Items)
            //{
            //    if (mi.ChildItems.Count > 0)
            //        for (int i = 0; i < mi.ChildItems.Count; i++)
            //        {
            //            var ttt = mi.ChildItems[i].NavigateUrl;
            //            if (mi.ChildItems[i].NavigateUrl.Contains(pagename))
            //            {
            //                mi.ChildItems[i].Selected = true;
            //                mi.ChildItems[i].Parent.Selected = true;
            //                break;
            //            }

            //        }
            //    else if (mi.NavigateUrl.Contains(pagename) && mi.Text != "Home")
            //    {
            //        mi.Selected = true;
            //        break;
            //    }
            //}


            //foreach (MenuItem mi in menuSettings.Items)
            //{
            //    if (mi.ChildItems.Count > 0)
            //        for (int i = 0; i < mi.ChildItems.Count; i++)
            //        {
            //            var ttt = mi.ChildItems[i].NavigateUrl;
            //            var mcName = mi.ChildItems[i].Text;
            //            if (mi.ChildItems[i].NavigateUrl.Contains(pagename))
            //            {
            //                var tttt = mi.ChildItems[i].Text;
            //                mi.ChildItems[i].Selectable = true;
            //                // mi.ChildItems[i].Text = "<span class='StaticSelectedStyle' >" + mi.ChildItems[i].Text + "</span>"; 
            //                mi.ChildItems[i].Parent.ChildItems[i].Selected = true;
            //                mi.ChildItems[i].Parent.Selected = true;
            //                break;
            //            }
            //        }
            //    else if (mi.NavigateUrl.Contains(pagename))
            //    {
            //        mi.Selected = true;
            //        break;
            //    }

            //}
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



    //protected void lnkProfile_Click(object sender, EventArgs e)
    //{

    //    Response.Redirect("user_profile.aspx");
    //}



    //private void GetMenuData()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    List<menu_item> menulist = new List<menu_item>();

    //    menulist = _db.menu_items.Where(p => p.parent_id == 0).OrderBy(s => s.serial).ToList();

    //    foreach (menu_item m in menulist.Where(p => p.menu_id != 1))
    //    {
    //        MenuItem menuItem = new MenuItem(m.menu_name.ToString(), m.menu_id.ToString());
    //        menuItem.NavigateUrl = m.menu_url ?? "".ToString();
    //        menuBar.Items.Add(menuItem);
    //        AddChildItems(menulist, menuItem);
    //    }

    //    foreach (menu_item m in menulist.Where(p => p.menu_id == 1).OrderBy(s => s.serial).ToList())
    //    {
    //        MenuItem menuItem = new MenuItem(m.menu_name.ToString(), m.menu_id.ToString());
    //        menuItem.NavigateUrl = m.menu_url ?? "".ToString();
    //        menuItem.Text = "";
    //        menuSettings.Items.Add(menuItem);
    //        AddSettingsChildItems(menulist.Where(p => p.menu_id == 1).ToList(), menuItem);
    //    }
    //}

    //private void AddChildItems(List<menu_item> menulist, MenuItem menuItem)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    List<menu_item> mlist = new List<menu_item>(menulist);

    //    mlist = _db.menu_items.Where(m => m.parent_id == Convert.ToInt32(menuItem.Value) && m.isShow == 1).OrderBy(s => s.serial).ToList();

    //    foreach (menu_item c in mlist)
    //    {
    //        MenuItem childItem = new MenuItem(c.menu_name.ToString(), c.menu_id.ToString());
    //        childItem.NavigateUrl = c.menu_url ?? "".ToString();
    //        menuItem.ChildItems.Add(childItem);
    //        AddChildItems(mlist, childItem);
    //    }
    //}

    //private void AddSettingsChildItems(List<menu_item> menulist, MenuItem menuItem)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    List<menu_item> mlist = new List<menu_item>(menulist);

    //    mlist = _db.menu_items.Where(m => m.parent_id == Convert.ToInt32(menuItem.Value) && m.isShow == 1).OrderBy(s => s.serial).ToList();

    //    foreach (menu_item c in mlist)
    //    {
    //        MenuItem childItem = new MenuItem(c.menu_name.ToString(), c.menu_id.ToString());
    //        childItem.NavigateUrl = c.menu_url ?? "".ToString();
    //        menuItem.ChildItems.Add(childItem);
    //        AddSettingsChildItems(mlist, childItem);
    //    }
    //}

    //protected void btnOperationCalendar_Click(object sender, ImageClickEventArgs e)
    //{
    //    Response.Redirect("schedulecalendar.aspx?TypeID=1");
    //}
}
