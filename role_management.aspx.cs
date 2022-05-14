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
using System.Collections.Generic;
using System.Drawing;

public partial class role_management : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin003") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindRoles();
            RefreshData(Convert.ToInt32(ddlRoles.SelectedValue));
        }
    }
    private void BindRoles()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var roles = from ro in _db.roles
                    select ro;
        ddlRoles.DataSource = roles;
        ddlRoles.DataTextField = "role_name";
        ddlRoles.DataValueField = "role_id";
        ddlRoles.DataBind();
    }
    private void RefreshData(int nRoleId)
    {
        LoadTree();
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.role_rights.Where(rr => rr.role_id == nRoleId && rr.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        int cnt = 0;
        foreach (TreeNode node in trvMenu.Nodes)
        {
            node.Checked = false;
            if (node.ChildNodes.Count > 0)
            {
                cnt = 0;
                foreach (TreeNode subNode in node.ChildNodes)
                {
                    foreach (role_right rr in item)
                    {
                        if (Convert.ToInt32(subNode.Value) == rr.menu_id)
                        {
                            subNode.Checked = true;
                            cnt++;
                        }
                    }
                    
                    if (cnt == 0)
                        node.Checked = false;
                    else
                        node.Checked = true;
                }
            }
        }
        trvMenu.ExpandAll();


        csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "trvMenu", "ddlRoles" });
    }
    private void AddChildMenu(TreeNode parentNode)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var items = _db.menu_items.Where(mi => mi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (menu_item objMenu in items)
        {
            if (objMenu.parent_id.ToString() == parentNode.Value)
            {
                TreeNode node = new TreeNode(objMenu.menu_name, objMenu.menu_id.ToString());
                parentNode.ChildNodes.Add(node);
            }
        }
    }
    private void LoadTree()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        menu_item objMenus = new menu_item();
        var items = _db.menu_items.Where(mi => mi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();

        trvMenu.Nodes.Clear();
        foreach (menu_item objMenu in items)
        {
            if (objMenu.parent_id == 0)
            {
                TreeNode node = new TreeNode(objMenu.menu_name, objMenu.menu_id.ToString());
                trvMenu.Nodes.Add(node);
                AddChildMenu(node);
            }
            else
            {
                //    dtMenu.Rows.Add(new object[] { objMenu.MenuID, objMenu.MenuName, objMenu.ParentID });
            }
        }
    }
    protected void ddlRoles_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlRoles.ID, ddlRoles.GetType().Name, "SelectedIndexChanged"); 
        lblResult.Text = "";
        RefreshData(Convert.ToInt32(ddlRoles.SelectedValue));
        //CheckExistingUIs(Convert.ToInt32(ddlRoles.SelectedValue));
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text="";

        DataClassesDataContext _db = new DataClassesDataContext();
        
        int nRoleId = Convert.ToInt32(ddlRoles.SelectedValue);
        int nClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        string strQ = "DELETE role_right WHERE role_id=" + nRoleId + " AND client_id=" + nClientId;
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        try
        {
            bool bFound = false;
            foreach (TreeNode node in trvMenu.Nodes)
            {
                bFound = false;
                if (node.ChildNodes.Count > 0)
                {
                    foreach (TreeNode subNode in node.ChildNodes)
                    {
                        if (subNode.Checked)
                        {
                            bFound = true;
                            role_right obj = new role_right();
                            obj.menu_id = Convert.ToInt32(subNode.Value);
                            obj.client_id = nClientId;
                            obj.role_id = nRoleId;
                            _db.role_rights.InsertOnSubmit(obj);
                        }
                    }
                    if (bFound)
                    {
                        role_right obj = new role_right();
                        obj.menu_id = Convert.ToInt32(node.Value);
                        obj.client_id = nClientId;
                        obj.role_id = nRoleId;
                        _db.role_rights.InsertOnSubmit(obj);
                    }
                }
            }
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
             
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void trvMenu_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, trvMenu.ID, trvMenu.GetType().Name, "TreeNodeCheckChanged"); 
        TreeView tv = (TreeView)sender;
        TreeNode selectedNode = trvMenu.SelectedNode; 
    }
}
