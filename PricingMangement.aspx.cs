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
using System.Data.OleDb;
using System.IO;

public partial class PricingMangement : System.Web.UI.Page
{
    string strDetails = "";
    string strTest = String.Empty;
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        LoadEvent();
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin004") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile objCp = _db.company_profiles.SingleOrDefault(cp => cp.client_id == 1);
            hdnMultiplier.Value = objCp.markup.ToString();
            LoadTree();
            // test.SaveText(strTest);
            LoadMainSectionInfo();
            lblTree.Visible = false;
            btnAddnewRow.Visible = true;
            if (Convert.ToInt32(hdnParentId.Value) > 0)
            {
                lblTree.Visible = true;
                lblSubSection.Visible = true;
                lblItemList.Visible = true;
                grdSubSection.Visible = true;
                grdItem_Price.Visible = true;
                LoadSubSectionInfo();
                LoadItemInfo();

            }

        }
    }


    private void LoadEvent()
    {
        string s2 = @"var elem = document.getElementById('{0}_SelectedNode');
                          if(elem != null )
                          {
                                var node = document.getElementById(elem.value);
                                if(node != null)
                                {
                                     node.scrollIntoView(true);
                                }
                          }
                        ";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "myscript", s2.Replace("{0}", trvSection.ClientID), true);
    }

    private void LoadTree()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = " SELECT * FROM sectioninfo WHERE  client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ")";
        //IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);

        List<sectioninfo> list = (from s in _db.sectioninfos
                                  where s.client_id == 1 && s.is_disable == false
                                  && !(from i in _db.item_prices
                                       where i.client_id == 1
                                       select i.item_id).Contains(s.section_id)
                                  select s).OrderBy(od => od.section_name).ToList();

        trvSection.Nodes.Clear();

        foreach (sectioninfo sec in list)
        {
            string name = sec.section_name;
            if (sec.parent_id == 0)
            {
                TreeNode node = new TreeNode(sec.section_name, sec.section_id.ToString());
                trvSection.Nodes.Add(node);
                AddChildMenu(node, sec);

                //Test
                //if (!_db.sectioninfos.Any(s => s.parent_id == sec.section_id && s.parent_id != 0))
                //{
                //    strTest += sec.section_id.ToString() + ", ";
                //}
            }
        }

    }
    //bool nBlock = false;
    //public void LoadNewTreeNode()
    //{
    //    LoadTree();
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    //after new Node Add----------------------------------------------------------------
    //    var sMaxID = _db.sectioninfos.Select(s => s.section_auto_id).Max();
    //    var sName = _db.sectioninfos.Where(s => s.section_auto_id == sMaxID).SingleOrDefault();

    //    if (nBlock == true)  // stop recursive re-entrancy, For Expand
    //        return;
    //    nBlock = true;
    //    trvSection.CollapseAll();        

    //    TreeNode nNode = FindNode(trvSection.Nodes, sName.section_name);

    //    while (nNode != null) // For Expand
    //    {
    //        nNode.Expand();
    //        nNode = nNode.Parent;
    //    }
    //    nBlock = false;
    //}

    //TreeNode n_found_node = null;
    //bool b_node_found = false;
    //bool b_cNode_found = false;
    //public TreeNode FindNode(TreeNodeCollection nCollection, string sectionName)
    //{        

    //    foreach (TreeNode node in nCollection)
    //    {
    //        if (node.Text.ToString() == sectionName)
    //        {
    //            b_node_found = true;
    //            n_found_node = node;                
    //            break;
    //        }
    //        if (!b_node_found)
    //        {
    //            //n_found_node = FindNode(node.ChildNodes, sectionName);
    //            foreach (TreeNode cNode in node.ChildNodes)
    //            {
    //                if (cNode.Text.ToString() == sectionName)
    //                {
    //                    b_cNode_found = true;
    //                    n_found_node = node;                        
    //                    break;
    //                }
    //                if (!b_cNode_found)
    //                {
    //                    n_found_node = FindNode(cNode.ChildNodes, sectionName);
    //                }
    //            }
    //        }
    //    }
    //    return n_found_node;
    //}

    private void AddChildMenu(TreeNode parentNode, sectioninfo sec)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = " SELECT * FROM sectioninfo WHERE is_disable = 0 AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) +
                " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ")  ORDER BY section_name";
            IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
            foreach (sectioninfo subsec in list)
            {
                if (subsec.parent_id.ToString() == parentNode.Value)
                {
                    TreeNode node = new TreeNode(subsec.section_name, subsec.section_id.ToString());
                    parentNode.ChildNodes.Add(node);
                    AddChildMenu(node, subsec);

                    ////Test
                    //if (!_db.sectioninfos.Any(s => s.parent_id == subsec.section_id))
                    //{
                    //    strTest += subsec.section_id.ToString() + ", ";
                    //}
                }
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }



    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }


    protected void trvSection_SelectedNodeChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, trvSection.ID, trvSection.GetType().Name, "SelectedNodeChanged"); 
        hdnSectionId.Value = trvSection.SelectedValue;
        hdnTrvSelectedValue.Value = trvSection.SelectedValue;
        hdnSubItemParentId.Value = trvSection.SelectedValue;
        lblResult.Text = "";
        lblItemResult.Text = "";
        lblMainSecResult.Text = "";
        lblSubSecResult.Text = "";

        btnAddSubnewRow.Visible = true;
        btnAddItem.Visible = true;
        btnDeleteItem.Visible = true;
        lblTree.Visible = true;
        lblMainSection.Visible = false;
        btnAddnewRow.Visible = false;
        grdMainSection.Visible = false;
        BindGrid_SubSection_Item();
    }
    public string GetItemDetialsForUpdateItem(int SectionId)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetails = sec1.section_name + " >> " + strDetails;
            //------txtMainSectionName.Text = sec1.section_name.Trim();// test Code
            GetItemDetialsForUpdateItem(Convert.ToInt32(sec1.parent_id));
        }
        return strDetails;
    }
    public void BindGrid_SubSection_Item()
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo sinfo = new sectioninfo();

       

        sinfo = _db.sectioninfos.SingleOrDefault(c => c.section_id == Convert.ToInt32(hdnTrvSelectedValue.Value) 
            && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        lblParent.Text = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnTrvSelectedValue.Value));

        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        hdnSubItemParentId.Value = Convert.ToInt32(hdnTrvSelectedValue.Value).ToString();
        lblParent.ForeColor = Color.Blue;
        if (Convert.ToInt32(hdnSubItemParentId.Value) > 0)
        {
            lblSubSection.Visible = true;
            lblItemList.Visible = true;
            grdSubSection.Visible = true;
            grdItem_Price.Visible = true;
            LoadSubSectionInfo();
            LoadItemInfo();
        }


    }


    bool Block = false;
    protected void trvSection_TreeNodeExpanded(object sender, TreeNodeEventArgs e)
    {
        if (Block == true)  // stop recursive re-entrancy
            return;
        Block = true;

        trvSection.CollapseAll();

        // expand current node and all parent nodes
        TreeNode Node = e.Node;
        while (Node != null)
        {
            Node.Expand();
            Node = Node.Parent;
        }
        Block = false;

    }


    #region  add new Sectiom

    private void LoadMainSectionInfo()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadSectionTable();

        var item = from sin in _db.sectioninfos
                   where sin.client_id == 1 && sin.parent_id == 0
                   orderby sin.section_name
                   select new SectionInfo()
                   {
                       section_id = (int)sin.section_id,
                       client_id = (int)sin.client_id,
                       section_name = sin.section_name,
                       parent_id = (int)sin.parent_id,
                       section_notes = sin.section_notes,
                       section_level = (int)sin.section_level,
                       section_serial = (decimal)sin.section_serial,
                       is_active = (bool)sin.is_active,
                       create_date = (DateTime)sin.create_date,
                       cssClassName = sin.cssClassName,
                       is_CommissionExclude = (bool)sin.is_CommissionExclude
                   };
        foreach (SectionInfo Sinfo in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["section_id"] = Sinfo.section_id;
            drNew["client_id"] = Sinfo.client_id;
            drNew["section_name"] = Sinfo.section_name;
            drNew["parent_id"] = Sinfo.parent_id;
            drNew["section_notes"] = Sinfo.section_notes;
            drNew["section_level"] = Sinfo.section_level;
            drNew["section_serial"] = Sinfo.section_serial;
            drNew["is_active"] = Sinfo.is_active;
            drNew["create_date"] = Sinfo.create_date;
            drNew["cssClassName"] = Sinfo.cssClassName;
            drNew["is_CommissionExclude"] = Sinfo.is_CommissionExclude;

            tmpTable.Rows.Add(drNew);
        }


        var result = (from sin in _db.sectioninfos
                      where sin.parent_id == 0 && sin.client_id == 1
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();

        nsectionId = nsectionId + 1000;
        hdnSectionId.Value = nsectionId.ToString();
        hdnSectionLevel.Value = nsectionId.ToString();
        hdnSectionSerial.Value = nsectionId.ToString();

        if (item.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();
            drNew1["section_id"] = Convert.ToInt32(hdnSectionId.Value);
            drNew1["client_id"] = 1;
            drNew1["section_name"] = "";
            drNew1["parent_id"] = 0;
            drNew1["section_notes"] = "";
            drNew1["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
            drNew1["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
            drNew1["is_active"] = true;
            drNew1["create_date"] = DateTime.Now;
            drNew1["cssClassName"] = "";
            drNew1["is_CommissionExclude"] = false;

            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("MainSection", tmpTable);
        grdMainSection.DataSource = tmpTable;
        grdMainSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "cssClassName", "client_id" };
        grdMainSection.DataBind();

    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("parent_id", typeof(int));
        table.Columns.Add("section_notes", typeof(string));
        table.Columns.Add("section_level", typeof(int));
        table.Columns.Add("section_serial", typeof(decimal));
        table.Columns.Add("is_active", typeof(bool));
        table.Columns.Add("create_date", typeof(DateTime));
        table.Columns.Add("cssClassName", typeof(string));
        table.Columns.Add("is_CommissionExclude", typeof(bool));



        return table;
    }

    protected void grdMainSection_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            DataTable table = (DataTable)Session["MainSection"];

            foreach (GridViewRow di in grdMainSection.Rows)
            {
                
                    CheckBox chkIsActive = (CheckBox)di.FindControl("chkIsActive");
                    TextBox txtSectionName = (TextBox)di.FindControl("txtSectionName");
                    Label lblSectionName = (Label)di.FindControl("lblSectionName");
                    DataRow dr = table.Rows[di.RowIndex];
                    DropDownList ddlcssClassName = (DropDownList)di.FindControl("ddlcssClassName");

                    DropDownList ddlDivision = (DropDownList)di.FindControl("ddlDivision");

                CheckBox chkIsExcludeCom0 = (CheckBox)di.FindControl("chkIsExcludeCom0");
                    Label lblAExcludeCom0 = (Label)di.FindControl("lblAExcludeCom0");

                    dr["section_id"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = ddlDivision.SelectedValue;
                    dr["section_name"] = txtSectionName.Text;
                    dr["parent_id"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[1]);
                    dr["section_notes"] = "";
                    dr["section_level"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[2]);
                    dr["section_serial"] = Convert.ToDecimal(grdMainSection.DataKeys[di.RowIndex].Values[3]);
                    dr["is_active"] = Convert.ToBoolean(chkIsActive.Checked);
                    dr["create_date"] = DateTime.Now;
                    dr["cssClassName"] = ddlcssClassName.SelectedValue;// grdMainSection.DataKeys[di.RowIndex].Values[4].ToString();
                    dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom0.Checked);
                

            }
            foreach (DataRow dr in table.Rows)
            {
                bool bFlagNew = false;

                sectioninfo SecInfo = _db.sectioninfos.SingleOrDefault(l => l.section_id == Convert.ToInt32(dr["section_id"]));
                if (SecInfo == null)
                {
                    SecInfo = new sectioninfo();
                    bFlagNew = true;
                    if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == 0 && l.is_disable == false && l.section_name == dr["section_name"].ToString()).SingleOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        lblMainSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        return;
                    }
                }

                string str = dr["section_name"].ToString().Trim();
                if (str.Length > 0)
                {
                    SecInfo.section_id = Convert.ToInt32(dr["section_id"]);
                    SecInfo.client_id = Convert.ToInt32(dr["client_id"]);
                    SecInfo.section_name = dr["section_name"].ToString();
                    SecInfo.parent_id = Convert.ToInt32(dr["parent_id"]);
                    SecInfo.section_notes = dr["section_notes"].ToString();
                    SecInfo.section_level = Convert.ToInt32(dr["section_level"]);
                    SecInfo.section_serial = Convert.ToDecimal(dr["section_serial"]);
                    SecInfo.is_active = Convert.ToBoolean(dr["is_active"]);
                    SecInfo.is_disable = false;
                    SecInfo.create_date = DateTime.Now;
                    SecInfo.is_mandatory = false;
                    SecInfo.is_CommissionExclude = Convert.ToBoolean(dr["is_CommissionExclude"]);
                    SecInfo.cssClassName = dr["cssClassName"].ToString();
                    if (bFlagNew)
                    {
                        SecInfo.serial = 100;
                        _db.sectioninfos.InsertOnSubmit(SecInfo);
                    }

                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section Name is a required field");
                    lblMainSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section Name is a required field");
                    return;
                }
               
            }


            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            lblMainSecResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            _db.SubmitChanges();
            LoadMainSectionInfo();
            LoadTree();


        }
    }

    protected void grdMainSection_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo objSI = new sectioninfo();

        int nSectionId = Convert.ToInt32(grdMainSection.DataKeys[Convert.ToInt32(e.NewEditIndex)].Values[0]);


        CheckBox chkIsActive = (CheckBox)grdMainSection.Rows[e.NewEditIndex].FindControl("chkIsActive");
        TextBox txtSectionName = (TextBox)grdMainSection.Rows[e.NewEditIndex].FindControl("txtSectionName");
        Label lblSectionName = (Label)grdMainSection.Rows[e.NewEditIndex].FindControl("lblSectionName");

        Label lblcssClassName = (Label)grdMainSection.Rows[e.NewEditIndex].FindControl("lblcssClassName");
        DropDownList ddlcssClassName = (DropDownList)grdMainSection.Rows[e.NewEditIndex].FindControl("ddlcssClassName");

        Label lblDivision = (Label)grdMainSection.Rows[e.NewEditIndex].FindControl("lblDivision");
        DropDownList ddlDivision = (DropDownList)grdMainSection.Rows[e.NewEditIndex].FindControl("ddlDivision");

        CheckBox chkIsExcludeCom0 = (CheckBox)grdMainSection.Rows[e.NewEditIndex].FindControl("chkIsExcludeCom0");
        Label lblAExcludeCom0 = (Label)grdMainSection.Rows[e.NewEditIndex].FindControl("lblAExcludeCom0");
        Label lblIsActive = (Label)grdMainSection.Rows[e.NewEditIndex].FindControl("lblIsActive");

        if (_db.sectioninfos.Where(si => si.section_id == nSectionId).Count() > 0)
        {
            objSI = _db.sectioninfos.Single(si => si.section_id == nSectionId);
            ddlcssClassName.SelectedValue = objSI.cssClassName.ToString();
            ddlcssClassName.CssClass = ddlcssClassName.SelectedValue;
        }

        lblcssClassName.Visible = false;
        ddlcssClassName.Visible = true;

        lblDivision.Visible = false;
        ddlDivision.Visible = true;

        lblAExcludeCom0.Visible = false;
        chkIsExcludeCom0.Visible = true;

        txtSectionName.Visible = true;
        lblSectionName.Visible = false;
        chkIsActive.Visible = true;
        lblIsActive.Visible = false;
        LinkButton btn = (LinkButton)grdMainSection.Rows[e.NewEditIndex].Cells[5].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }

    protected void GetCssClassName(object sender, EventArgs e)
    {

        string strSender = sender.ToString();
        int i = 0;

        DropDownList ddlcssClassName1 = (DropDownList)grdMainSection.FindControl("ddlcssClassName");
        ddlcssClassName1 = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddlcssClassName1.NamingContainer;
        i = gvr.RowIndex;

        DropDownList ddlcssClassName = (DropDownList)grdMainSection.Rows[i].FindControl("ddlcssClassName");

        ddlcssClassName.CssClass = ddlcssClassName.SelectedValue;

        ddlcssClassName.Items.FindByValue(ddlcssClassName.SelectedValue).Selected = true;
    }

    protected void grdMainSection_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBox chkIsActive = (CheckBox)grdMainSection.Rows[e.RowIndex].FindControl("chkIsActive");
        TextBox txtSectionName = (TextBox)grdMainSection.Rows[e.RowIndex].FindControl("txtSectionName");
        Label lblSectionName = (Label)grdMainSection.Rows[e.RowIndex].FindControl("lblSectionName");

        DropDownList ddlcssClassName = (DropDownList)grdMainSection.Rows[e.RowIndex].FindControl("ddlcssClassName");
        string strClassName = ddlcssClassName.SelectedItem.Value;

        DropDownList ddlDivision = (DropDownList)grdMainSection.Rows[e.RowIndex].FindControl("ddlDivision");

        CheckBox chkIsExcludeCom0 = (CheckBox)grdMainSection.Rows[e.RowIndex].FindControl("chkIsExcludeCom0");
        Label lblAExcludeCom0 = (Label)grdMainSection.Rows[e.RowIndex].FindControl("lblAExcludeCom0");

        int nSectionId = Convert.ToInt32(grdMainSection.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strMainName = txtSectionName.Text.Replace("'", "''");
        if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == 0 && l.is_disable == false && l.section_name == strMainName).SingleOrDefault() != null)
        {
            List<sectioninfo> sList = _db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == 0 && l.is_disable == false && l.section_name == strMainName).ToList();
            foreach (sectioninfo objsec in sList)
            {
                if (objsec.section_id != nSectionId)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name is already exist. Please try another name to update");
                    lblMainSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name is already exist. Please try another name to update");
                    return;
                }
            }

        }
        string strQ = "UPDATE sectioninfo SET section_name='" + txtSectionName.Text.Replace("'", "''") + "' , is_active='" + Convert.ToBoolean(chkIsActive.Checked) + "',is_CommissionExclude='" + Convert.ToBoolean(chkIsExcludeCom0.Checked) + "', cssClassName='" + strClassName + "'  WHERE section_id=" + nSectionId ;
        _db.ExecuteCommand(strQ, string.Empty);

        string strItemQ = "UPDATE sectioninfo SET  client_id = "+ ddlDivision.SelectedValue + ", is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom0.Checked) + "'  WHERE section_level  =" + nSectionId;
        _db.ExecuteCommand(strItemQ, string.Empty);

        LoadMainSectionInfo();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        lblMainSecResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        LoadTree();

    }

    protected void grdMainSection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            CheckBox chkIsExcludeCom0 = (CheckBox)e.Row.FindControl("chkIsExcludeCom0");
            Label lblAExcludeCom0 = (Label)e.Row.FindControl("lblAExcludeCom0");
            CheckBox chkIsActive = (CheckBox)e.Row.FindControl("chkIsActive");
            Label lblIsActive0 = (Label)e.Row.FindControl("lblIsActive");
            TextBox txtSectionName = (TextBox)e.Row.FindControl("txtSectionName");
            Label lblSectionName = (Label)e.Row.FindControl("lblSectionName");
            DropDownList ddlcssClassName = (DropDownList)e.Row.FindControl("ddlcssClassName");
            string strClassName = grdMainSection.DataKeys[e.Row.RowIndex].Values[4].ToString();
            Label lblcssClassName = (Label)e.Row.FindControl("lblcssClassName");
            int clientId = Convert.ToInt32(grdMainSection.DataKeys[e.Row.RowIndex].Values[5]);
            
            Label lblDivision = (Label)e.Row.FindControl("lblDivision");
            DropDownList ddlDivision = (DropDownList)e.Row.FindControl("ddlDivision");

            ddlcssClassName.SelectedValue = strClassName;

            lblcssClassName.CssClass = strClassName;
            lblcssClassName.Text = strClassName.Replace("fc-", "");

            if (chkIsExcludeCom0.Checked)
            {
                lblAExcludeCom0.Text = "Yes";
            }
            else
            {
                lblAExcludeCom0.Text = "No";
            }
            if (chkIsActive.Checked)
            {
                lblIsActive0.Text = "Yes";
            }
            else
            {
                lblIsActive0.Text = "No";
            }


            BindDivition(ddlDivision);


            DataClassesDataContext _db = new DataClassesDataContext();
            division dv = _db.divisions.FirstOrDefault(x => x.Id == clientId);
            if(dv != null)
            {
                lblDivision.Text = dv.division_name;
            }

            



            string str = txtSectionName.Text.Replace("&nbsp;", "");
            if (str == "" || Convert.ToInt32(grdMainSection.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]) == 0)
            {
                txtSectionName.Visible = true;
                lblSectionName.Visible = false;
                ddlcssClassName.Visible = true;
                lblcssClassName.Visible = false;

                chkIsExcludeCom0.Visible = true;
                lblAExcludeCom0.Visible = false;


                lblDivision.Visible = false;
                ddlDivision.Visible = true;

                if (strClassName == "")
                {
                    ddlcssClassName.SelectedValue = "fc-RoyalBlue";
                    ddlcssClassName.CssClass = "fc-RoyalBlue";
                }

                LinkButton btn = (LinkButton)e.Row.Cells[5].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }
        }

    }

    private void BindDivition(DropDownList ddlDivision)
    {
        DataTable dt = csCommonUtility.GetDataTable("select Id, division_name from division order by division_name");
        ddlDivision.DataSource = dt;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
    }

    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from sin in _db.sectioninfos
                      where sin.parent_id == 0 && sin.client_id == 1
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();

        nsectionId = nsectionId + 1000;
        hdnSectionId.Value = nsectionId.ToString();
        hdnSectionLevel.Value = nsectionId.ToString();
        hdnSectionSerial.Value = nsectionId.ToString();

        DataTable table = (DataTable)Session["MainSection"];

        int nSecId = Convert.ToInt32(hdnSectionId.Value);
        bool contains = table.AsEnumerable().Any(row => nSecId == row.Field<int>("section_id"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            lblMainSecResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow di in grdMainSection.Rows)
        {
           
                CheckBox chkIsActive = (CheckBox)di.FindControl("chkIsActive");
                TextBox txtSectionName = (TextBox)di.FindControl("txtSectionName");
                Label lblSectionName = (Label)di.FindControl("lblSectionName");
                DropDownList ddlcssClassName = (DropDownList)di.FindControl("ddlcssClassName");
                DataRow dr = table.Rows[di.RowIndex];

                Label lblDivision = (Label)di.FindControl("lblDivision");
                DropDownList ddlDivision = (DropDownList)di.FindControl("ddlDivision");

                CheckBox chkIsExcludeCom0 = (CheckBox)di.FindControl("chkIsExcludeCom0");
                Label lblAExcludeCom0 = (Label)di.FindControl("lblAExcludeCom0");
           
                 //  ddlcssClassName.Visible = true;


                dr["section_id"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = 1;
                dr["section_name"] = txtSectionName.Text;
                dr["parent_id"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[1]);
                dr["section_notes"] = "";
                dr["section_level"] = Convert.ToInt32(grdMainSection.DataKeys[di.RowIndex].Values[2]);
                dr["section_serial"] = Convert.ToDecimal(grdMainSection.DataKeys[di.RowIndex].Values[3]);
                dr["is_active"] = Convert.ToBoolean(chkIsActive.Checked);
                dr["create_date"] = DateTime.Now;
                dr["cssClassName"] = grdMainSection.DataKeys[di.RowIndex].Values[4].ToString();
                dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom0.Checked);


            

        }



        DataRow drNew = table.NewRow();
        drNew["section_id"] = Convert.ToInt32(hdnSectionId.Value);
        drNew["client_id"] = 1;
        drNew["section_name"] = "";
        drNew["parent_id"] = 0;
        drNew["section_notes"] = "";
        drNew["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
        drNew["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
        drNew["is_active"] = true;
        drNew["create_date"] = DateTime.Now;
        drNew["cssClassName"] = "";
        drNew["is_CommissionExclude"] = false;
        //table.Rows.Add(drNew);
        table.Rows.InsertAt(drNew, 0);

        Session.Add("MainSection", table);
        grdMainSection.DataSource = table;
        grdMainSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "cssClassName", "client_id" };
        grdMainSection.DataBind();
        lblResult.Text = "";
        lblMainSecResult.Text = "";
        lblItemResult.Text = "";
        lblSubSecResult.Text = "";

    }
    #endregion

    #region  add new Sub Section

    private void LoadSubSectionInfo()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadSectionTable();
        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.SingleOrDefault(c => c.section_id == Convert.ToInt32(hdnSubItemParentId.Value) && c.client_id == 1);
        hdnSectionLevel.Value = sinfo.section_level.ToString();
        string strQ = "";
        if (Convert.ToInt32(hdnSectionLevel.Value) == 47000)
        {
            strQ = " SELECT * FROM sectioninfo WHERE is_disable = 0 AND parent_id = " + Convert.ToInt32(hdnSubItemParentId.Value) + " AND section_level = " + Convert.ToInt32(hdnSectionLevel.Value) +
                " AND client_id = 1 AND section_id < " + Convert.ToInt32(hdnSectionLevel.Value) + " + 100 AND " +
                " section_id NOT IN ( SELECT item_id FROM item_price where item_id IN(Select section_id from sectioninfo WHERE section_level = 47000)) " +
                " ORDER BY section_name";
        }
        else
        {
            strQ = " SELECT * FROM sectioninfo WHERE is_disable = 0 AND parent_id = " + Convert.ToInt32(hdnSubItemParentId.Value) + " AND " +
                " section_level = " + Convert.ToInt32(hdnSectionLevel.Value) + " AND client_id = 1 AND " +
                " section_id  NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ") " +
                " AND section_id NOT IN (9139, 9140, 9151, 9152, 9155, 9156, 9145, 9137, 9138, 9163) ORDER BY section_name";
        }
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        //var item = from sin in _db.sectioninfos
        //           where sin.parent_id == Convert.ToInt32(hdnSubItemParentId.Value) && sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1 && sin.section_id < Convert.ToInt32(hdnSectionLevel.Value) + 100
        //           select new SectionInfo()
        //           {
        //               section_id = (int)sin.section_id,
        //               client_id = (int)sin.client_id,
        //               section_name = sin.section_name,
        //               parent_id = (int)sin.parent_id,
        //               section_notes = sin.section_notes,
        //               section_level = (int)sin.section_level,
        //               section_serial = (decimal)sin.section_serial,
        //               is_active = (bool)sin.is_active,
        //               create_date = (DateTime)sin.create_date
        //           };
        foreach (sectioninfo Sinfo in list)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["section_id"] = Sinfo.section_id;
            drNew["client_id"] = Sinfo.client_id;
            drNew["section_name"] = Sinfo.section_name;
            drNew["parent_id"] = Sinfo.parent_id;
            drNew["section_notes"] = Sinfo.section_notes;
            drNew["section_level"] = Sinfo.section_level;
            drNew["section_serial"] = Sinfo.section_serial;
            drNew["is_active"] = Sinfo.is_active;
            drNew["create_date"] = Sinfo.create_date;
            drNew["is_CommissionExclude"] = Sinfo.is_CommissionExclude;
            tmpTable.Rows.Add(drNew);
        }



        var result = (from sin in _db.sectioninfos
                      where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1 && sin.section_id < Convert.ToInt32(hdnSectionLevel.Value) + 100
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();

        if (nsectionId == 0)
        {
            nsectionId = Convert.ToInt32(hdnSectionLevel.Value) + 1;
        }
        else
        {
            nsectionId = nsectionId + 1;
        }
        hdnSectionId.Value = nsectionId.ToString();
        hdnSectionSerial.Value = nsectionId.ToString();
        lblSerial.Text = hdnSectionSerial.Value;


        if (tmpTable.Rows.Count == 0)//if (list.Count() == 0)
        {

            DataRow drNew1 = tmpTable.NewRow();
            drNew1["section_id"] = Convert.ToInt32(hdnSectionId.Value);
            drNew1["client_id"] = 1;
            drNew1["section_name"] = "";
            drNew1["parent_id"] = Convert.ToInt32(hdnSubItemParentId.Value);
            drNew1["section_notes"] = "";
            drNew1["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
            drNew1["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
            drNew1["is_active"] = true;
            drNew1["create_date"] = DateTime.Now;
            drNew1["is_CommissionExclude"] = true;

            //tmpTable.Rows.Add(drNew);
            tmpTable.Rows.InsertAt(drNew1, 0);
        }
        Session.Add("SubSection", tmpTable);
        grdSubSection.DataSource = tmpTable;
        grdSubSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "client_id" };
        grdSubSection.DataBind();

    }


    protected void grdSubSection_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            DataTable table = (DataTable)Session["SubSection"];

            foreach (GridViewRow di in grdSubSection.Rows)
            {
                
                    CheckBox chkIsActive1 = (CheckBox)di.FindControl("chkIsActive1");
                    TextBox txtSubSectionName = (TextBox)di.FindControl("txtSubSectionName");
                    Label lblSubSectionName = (Label)di.FindControl("lblSubSectionName");
                    CheckBox chkIsExcludeCom1 = (CheckBox)di.FindControl("chkIsExcludeCom1");
                    Label lblAExcludeCom1 = (Label)di.FindControl("lblAExcludeCom1");
                    DataRow dr = table.Rows[di.RowIndex];
                    DropDownList ddlSubDivision = (DropDownList)di.FindControl("ddlSubDivision");

                    dr["section_id"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = ddlSubDivision.SelectedValue;
                    dr["section_name"] = txtSubSectionName.Text;
                    dr["parent_id"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[1]);
                    dr["section_notes"] = "";
                    dr["section_level"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[2]);
                    dr["section_serial"] = Convert.ToDecimal(grdSubSection.DataKeys[di.RowIndex].Values[3]);
                    dr["is_active"] = Convert.ToBoolean(chkIsActive1.Checked);
                    dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom1.Checked);
                    dr["create_date"] = DateTime.Now;

                

            }
            foreach (DataRow dr in table.Rows)
            {
                bool bFlagNew = false;

                sectioninfo SecInfo = _db.sectioninfos.SingleOrDefault(l => l.section_id == Convert.ToInt32(dr["section_id"]));
                if (SecInfo == null)
                {
                    //sectioninfo sinfo = new sectioninfo();
                    //sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnParentId.Value) && c.client_id == 1);
                    //hdnSectionLevel.Value = sinfo.section_level.ToString();
                    SecInfo = new sectioninfo();
                    bFlagNew = true;
                    if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == Convert.ToInt32(hdnSubItemParentId.Value) && l.is_disable == false && l.section_name == dr["section_name"].ToString()).SingleOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        lblSubSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        return;
                    }
                }

                string str = dr["section_name"].ToString().Trim();
                if (str.Length > 0)
                {
                    SecInfo.section_id = Convert.ToInt32(dr["section_id"]);
                    SecInfo.client_id = Convert.ToInt32(dr["client_id"]);
                    SecInfo.section_name = dr["section_name"].ToString();
                    SecInfo.parent_id = Convert.ToInt32(dr["parent_id"]);
                    SecInfo.section_notes = dr["section_notes"].ToString();
                    SecInfo.section_level = Convert.ToInt32(dr["section_level"]);
                    SecInfo.section_serial = Convert.ToDecimal(dr["section_serial"]);
                    SecInfo.is_active = Convert.ToBoolean(dr["is_active"]);
                    SecInfo.create_date = DateTime.Now;
                    SecInfo.is_mandatory = false;
                    SecInfo.is_CommissionExclude = Convert.ToBoolean(dr["is_CommissionExclude"]); //false;
                    SecInfo.is_disable = false;
                    if (bFlagNew)
                    {
                        SecInfo.serial = 100;
                        _db.sectioninfos.InsertOnSubmit(SecInfo);
                    }

                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section Name is a required field");
                    lblSubSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section Name is a required field");
                    return;
                }
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            lblSubSecResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            _db.SubmitChanges();
            LoadSubSectionInfo();
            LoadTree();

        }
    }

    protected void grdSubSection_RowEditing(object sender, GridViewEditEventArgs e)
    {

        TextBox txtSubSectionName = (TextBox)grdSubSection.Rows[e.NewEditIndex].FindControl("txtSubSectionName");
        Label lblSubSectionName = (Label)grdSubSection.Rows[e.NewEditIndex].FindControl("lblSubSectionName");

        CheckBox chkIsExcludeCom1 = (CheckBox)grdSubSection.Rows[e.NewEditIndex].FindControl("chkIsExcludeCom1");
        Label lblAExcludeCom1 = (Label)grdSubSection.Rows[e.NewEditIndex].FindControl("lblAExcludeCom1");
        CheckBox chkIsActive = (CheckBox)grdSubSection.Rows[e.NewEditIndex].FindControl("chkIsActive1");
        Label lblIsActive = (Label)grdSubSection.Rows[e.NewEditIndex].FindControl("lblIsActive1");

        Label lblSubDivision = (Label)grdSubSection.Rows[e.NewEditIndex].FindControl("lblSubDivision");
        DropDownList ddlSubDivision = (DropDownList)grdSubSection.Rows[e.NewEditIndex].FindControl("ddlSubDivision");

        lblSubDivision.Visible = false;
        ddlSubDivision.Visible = true;

        chkIsExcludeCom1.Visible = true;
        lblAExcludeCom1.Visible = false;

        txtSubSectionName.Visible = true;
        lblSubSectionName.Visible = false;
        chkIsActive.Visible = true;
        lblIsActive.Visible = false;
        LinkButton btn = (LinkButton)grdSubSection.Rows[e.NewEditIndex].Cells[4].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdSubSection_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBox chkIsActive1 = (CheckBox)grdSubSection.Rows[e.RowIndex].FindControl("chkIsActive1");
        TextBox txtSubSectionName = (TextBox)grdSubSection.Rows[e.RowIndex].FindControl("txtSubSectionName");
        Label lblSubSectionName = (Label)grdSubSection.Rows[e.RowIndex].FindControl("lblSubSectionName");

        CheckBox chkIsExcludeCom1 = (CheckBox)grdSubSection.Rows[e.RowIndex].FindControl("chkIsExcludeCom1");
        Label lblAExcludeCom1 = (Label)grdSubSection.Rows[e.RowIndex].FindControl("lblAExcludeCom1");

        DropDownList ddlSubDivision = (DropDownList)grdSubSection.Rows[e.RowIndex].FindControl("ddlSubDivision");

        int nSectionId = Convert.ToInt32(grdSubSection.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        int nParentId = Convert.ToInt32(grdSubSection.DataKeys[Convert.ToInt32(e.RowIndex)].Values[1]);
        string strSubName = txtSubSectionName.Text.Replace("'", "''");
        if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == nParentId && l.is_disable == false && l.section_name == strSubName).SingleOrDefault() != null)
        {
            List<sectioninfo> sList = _db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == nParentId && l.is_disable == false && l.section_name == strSubName).ToList();
            foreach (sectioninfo objsec in sList)
            {
                if (objsec.section_id != nSectionId)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name is already exist. Please try another name to update");
                    lblSubSecResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name is already exist. Please try another name to update");
                    return;
                }
            }

        }


        //imran
        //string strQ = "UPDATE sectioninfo SET section_name='" + txtSubSectionName.Text.Replace("'", "''") + "' , is_active='" + Convert.ToBoolean(chkIsActive1.Checked) + "',is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom1.Checked) + "'  WHERE section_id=" + nSectionId + "  AND client_id=1";
        //_db.ExecuteCommand(strQ, string.Empty);
        //string strItemQ = "UPDATE sectioninfo SET is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom1.Checked) + "'  WHERE parent_id =" + nSectionId + "  AND client_id=1";

        string strQ = "UPDATE sectioninfo SET section_name='" + txtSubSectionName.Text.Replace("'", "''") + "' , is_active='" + Convert.ToBoolean(chkIsActive1.Checked) + "',is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom1.Checked) + "'  WHERE section_id=" + nSectionId;
        _db.ExecuteCommand(strQ, string.Empty);

        string strItemQ = "UPDATE sectioninfo SET client_id = " + ddlSubDivision.SelectedValue + ", is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom1.Checked) + "'  WHERE parent_id =" + nSectionId ;
        _db.ExecuteCommand(strItemQ, string.Empty);

        LoadTree();
        LoadMainSectionInfo();
        LoadSubSectionInfo();
        LoadItemInfo();

        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        lblSubSecResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");

    }

    protected void grdSubSection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {



            TextBox txtSubSectionName = (TextBox)e.Row.FindControl("txtSubSectionName");
            Label lblSubSectionName = (Label)e.Row.FindControl("lblSubSectionName");

            CheckBox chkIsExcludeCom1 = (CheckBox)e.Row.FindControl("chkIsExcludeCom1");
            Label lblAExcludeCom1 = (Label)e.Row.FindControl("lblAExcludeCom1");
            CheckBox chkIsActive = (CheckBox)e.Row.FindControl("chkIsActive1");
            Label lblIsActive0 = (Label)e.Row.FindControl("lblIsActive1");

            int clientId = Convert.ToInt32(grdSubSection.DataKeys[e.Row.RowIndex].Values[4]);
            Label lblSubDivision = (Label)e.Row.FindControl("lblSubDivision");
            DropDownList ddlSubDivision = (DropDownList)e.Row.FindControl("ddlSubDivision");

            BindDivition(ddlSubDivision);

            DataClassesDataContext _db = new DataClassesDataContext();
            division dv = _db.divisions.FirstOrDefault(x => x.Id == clientId);
            if (dv != null)
            {
                lblSubDivision.Text = dv.division_name;
            }


            if (chkIsExcludeCom1.Checked)
            {
                lblAExcludeCom1.Text = "Yes";
            }
            else
            {
                lblAExcludeCom1.Text = "No";
            }
            if (chkIsActive.Checked)
            {
                lblIsActive0.Text = "Yes";
            }
            else
            {
                lblIsActive0.Text = "No";
            }

            string str = txtSubSectionName.Text.Replace("&nbsp;", "");
            if (str == "" || Convert.ToInt32(grdSubSection.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]) == 0)
            {
                txtSubSectionName.Visible = true;
                lblSubSectionName.Visible = false;

                chkIsExcludeCom1.Visible = true;
                lblAExcludeCom1.Visible = false;

                lblSubDivision.Visible = false;
                ddlSubDivision.Visible = true;

                LinkButton btn = (LinkButton)e.Row.Cells[4].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }


        }

    }
    protected void btnAddSubnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddSubnewRow.ID, btnAddSubnewRow.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.SingleOrDefault(c => c.section_id == Convert.ToInt32(hdnSubItemParentId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        hdnSectionLevel.Value = sinfo.section_level.ToString();
        var result = (from sin in _db.sectioninfos
                      where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1 && sin.section_id < Convert.ToInt32(hdnSectionLevel.Value) + 100
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();


        if (nsectionId == 0)
        {
            nsectionId = Convert.ToInt32(hdnSectionLevel.Value) + 1;
        }
        else
        {
            nsectionId = nsectionId + 1;
        }

        hdnSectionId.Value = nsectionId.ToString();
        hdnSectionSerial.Value = nsectionId.ToString();
        lblSerial.Text = hdnSectionSerial.Value;

        DataTable table = (DataTable)Session["SubSection"];
        int nSecId = Convert.ToInt32(hdnSectionId.Value);
        bool contains = table.AsEnumerable().Any(row => nSecId == row.Field<int>("section_id"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            lblSubSecResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow di in grdSubSection.Rows)
        {
            {
                CheckBox chkIsActive1 = (CheckBox)di.FindControl("chkIsActive1");
                CheckBox chkIsExcludeCom1 = (CheckBox)di.FindControl("chkIsExcludeCom1");
                Label lblAExcludeCom1 = (Label)di.FindControl("lblAExcludeCom1");
                TextBox txtSubSectionName = (TextBox)di.FindControl("txtSubSectionName");
                Label lblSubSectionName = (Label)di.FindControl("lblSubSectionName");
                DataRow dr = table.Rows[di.RowIndex];

                dr["section_id"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = 1;
                dr["section_name"] = txtSubSectionName.Text;
                dr["parent_id"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[1]);
                dr["section_notes"] = "";
                dr["section_level"] = Convert.ToInt32(grdSubSection.DataKeys[di.RowIndex].Values[2]);
                dr["section_serial"] = Convert.ToDecimal(grdSubSection.DataKeys[di.RowIndex].Values[3]);
                dr["is_active"] = Convert.ToBoolean(chkIsActive1.Checked);
                dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom1.Checked);
                dr["create_date"] = DateTime.Now;


            }

        }

        DataRow drNew = table.NewRow();
        drNew["section_id"] = Convert.ToInt32(hdnSectionId.Value);
        drNew["client_id"] = 1;
        drNew["section_name"] = "";
        drNew["parent_id"] = Convert.ToInt32(hdnSubItemParentId.Value);
        drNew["section_notes"] = "";
        drNew["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
        drNew["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
        drNew["is_active"] = true;
        drNew["is_CommissionExclude"] = false;
        drNew["create_date"] = DateTime.Now;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("SubSection", table);
        grdSubSection.DataSource = table;
        grdSubSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "client_id" };
        grdSubSection.DataBind();
        lblResult.Text = "";
        lblMainSecResult.Text = "";
        lblItemResult.Text = "";
        lblSubSecResult.Text = "";
    }
    #endregion

    #region  add new Items
    private DataTable LoadItemPriceTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("parent_id", typeof(int));
        table.Columns.Add("section_notes", typeof(string));
        table.Columns.Add("section_level", typeof(int));
        table.Columns.Add("section_serial", typeof(decimal));
        table.Columns.Add("is_active", typeof(bool));
        table.Columns.Add("create_date", typeof(DateTime));

        table.Columns.Add("item_id", typeof(int));
        table.Columns.Add("measure_unit", typeof(string));
        table.Columns.Add("item_cost", typeof(decimal));
        table.Columns.Add("minimum_qty", typeof(decimal));
        table.Columns.Add("retail_multiplier", typeof(decimal));
        table.Columns.Add("labor_rate", typeof(decimal));
        table.Columns.Add("update_time", typeof(DateTime));
        table.Columns.Add("labor_id", typeof(int));
        table.Columns.Add("is_mandatory", typeof(bool));
        table.Columns.Add("is_CommissionExclude", typeof(bool));

        return table;
    }


    private void LoadItemInfo()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadItemPriceTable();
        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.SingleOrDefault(c => c.section_id == Convert.ToInt32(hdnSubItemParentId.Value) && c.client_id == 1);
        hdnSectionLevel.Value = sinfo.section_level.ToString();
        var item = from it in _db.item_prices
                   join si in _db.sectioninfos on it.item_id equals si.section_id
                   where si.parent_id == Convert.ToInt32(hdnSubItemParentId.Value) && si.section_level == Convert.ToInt32(hdnSectionLevel.Value)
                   && si.client_id == 1 && si.is_disable == false
                   orderby si.serial
                   //var item = from sin in _db.sectioninfos
                   //           where sin.parent_id == Convert.ToInt32(hdnSubItemParentId.Value) && sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1 && sin.section_id > Convert.ToInt32(hdnSectionLevel.Value) + 100
                   select new SectionItemPriceModel()
                  {
                      section_id = (int)si.section_id,
                      client_id = (int)si.client_id,
                      section_name = si.section_name,
                      parent_id = (int)si.parent_id,
                      section_notes = si.section_notes,
                      section_level = (int)si.section_level,
                      section_serial = (decimal)si.section_serial,
                      is_active = (bool)si.is_active,
                      create_date = (DateTime)si.create_date,

                      item_id = (int)it.item_id,
                      measure_unit = it.measure_unit,
                      item_cost = (decimal)it.item_cost,
                      minimum_qty = (decimal)it.minimum_qty,
                      retail_multiplier = (decimal)it.retail_multiplier,
                      labor_rate = (decimal)it.labor_rate,
                      labor_id = (int)it.labor_id,
                      is_mandatory = (bool)si.is_mandatory,
                      is_CommissionExclude = (bool)si.is_CommissionExclude
                  };
        foreach (SectionItemPriceModel Sinfo in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["section_id"] = Sinfo.section_id;
            drNew["client_id"] = Sinfo.client_id;
            drNew["section_name"] = Sinfo.section_name;
            drNew["parent_id"] = Sinfo.parent_id;
            drNew["section_notes"] = Sinfo.section_notes;
            drNew["section_level"] = Sinfo.section_level;
            drNew["section_serial"] = Sinfo.section_serial;
            drNew["is_active"] = Sinfo.is_active;
            drNew["create_date"] = Sinfo.create_date;

            drNew["item_id"] = Sinfo.item_id;
            drNew["measure_unit"] = Sinfo.measure_unit;
            drNew["item_cost"] = Sinfo.item_cost;
            drNew["minimum_qty"] = Sinfo.minimum_qty;
            drNew["retail_multiplier"] = Sinfo.retail_multiplier;
            drNew["labor_rate"] = Sinfo.labor_rate;
            drNew["labor_id"] = Sinfo.labor_id;
            drNew["is_mandatory"] = Sinfo.is_mandatory;
            drNew["is_CommissionExclude"] = Sinfo.is_CommissionExclude;
            tmpTable.Rows.Add(drNew);


        }



        var result = (from sin in _db.sectioninfos
                      where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1
                      && sin.section_id > Convert.ToInt32(hdnSectionLevel.Value) + 100
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();

        if (nsectionId == 0)
        {
            nsectionId = Convert.ToInt32(hdnSectionLevel.Value) + 100 + 1;
        }
        else
        {
            nsectionId = nsectionId + 1;
        }
        hdnSectionId.Value = nsectionId.ToString();
        string strSerial = nsectionId.ToString();
        string str = "";
        if (strSerial.Length < 5)
        {
            str = strSerial.Substring(2);
        }
        hdnSectionSerial.Value = hdnSectionLevel.Value + "." + str;
        lblSerial.Text = hdnSectionSerial.Value;

        //if (tmpTable.Rows.Count() == 0)
        if (item.Count() == 0)
        {

            DataRow drNew1 = tmpTable.NewRow();
            drNew1["section_id"] = Convert.ToInt32(hdnSectionId.Value);
            drNew1["client_id"] = 1;
            drNew1["section_name"] = "";
            drNew1["parent_id"] = Convert.ToInt32(hdnSubItemParentId.Value);
            drNew1["section_notes"] = "";
            drNew1["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
            drNew1["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
            drNew1["is_active"] = true;
            drNew1["create_date"] = DateTime.Now;

            drNew1["item_id"] = Convert.ToInt32(hdnSectionId.Value);
            drNew1["measure_unit"] = "";
            drNew1["item_cost"] = 0;
            drNew1["minimum_qty"] = 0;
            drNew1["retail_multiplier"] = Convert.ToDecimal(hdnMultiplier.Value);
            drNew1["labor_rate"] = 0;
            drNew1["labor_id"] = 2;
            drNew1["is_mandatory"] = false;
            drNew1["is_CommissionExclude"] = false;

            //tmpTable.Rows.Add(drNew);
            tmpTable.Rows.InsertAt(drNew1, 0);
        }
        Session.Add("NewItem", tmpTable);
        grdItem_Price.DataSource = tmpTable;
        grdItem_Price.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "labor_id", "is_mandatory", "is_CommissionExclude", "client_id" };
        grdItem_Price.DataBind();

    }


    protected void grdItem_Price_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            DataTable table = (DataTable)Session["NewItem"];

            foreach (GridViewRow di in grdItem_Price.Rows)
            {
                {
                    CheckBox chkIsActiveItem = (CheckBox)di.FindControl("chkIsActiveItem");
                    CheckBox chkIsMandatory = (CheckBox)di.FindControl("chkIsMandatory");
                    CheckBox chkIsExcludeCom = (CheckBox)di.FindControl("chkIsExcludeCom");

                    TextBox txtItemName = (TextBox)di.FindControl("txtItemName");
                    Label lblItemnName = (Label)di.FindControl("lblItemnName");

                    TextBox txtMeasureUnit = (TextBox)di.FindControl("txtMeasureUnit");
                    Label lblMeasureUnit = (Label)di.FindControl("lblMeasureUnit");

                    TextBox txtCost = (TextBox)di.FindControl("txtCost");
                    Label lblCost = (Label)di.FindControl("lblCost");

                    TextBox txtMinQty = (TextBox)di.FindControl("txtMinQty");
                    Label lblMinQty = (Label)di.FindControl("lblMinQty");

                    TextBox txtRetailMulti = (TextBox)di.FindControl("txtRetailMulti");
                    Label lblRetailMulti = (Label)di.FindControl("lblRetailMulti");

                    TextBox txtLabor = (TextBox)di.FindControl("txtLabor");
                    Label lblLabor = (Label)di.FindControl("lblLabor");

                    DropDownList ddlItemPriceDivision = (DropDownList)di.FindControl("ddlItemPriceDivision");

                    DataRow dr = table.Rows[di.RowIndex];

                    dr["section_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = ddlItemPriceDivision.SelectedValue;
                    dr["section_name"] = txtItemName.Text;
                    dr["parent_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[1]);
                    dr["section_notes"] = "";
                    dr["section_level"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[2]);
                    dr["section_serial"] = Convert.ToDecimal(grdItem_Price.DataKeys[di.RowIndex].Values[3]);
                    dr["is_active"] = Convert.ToBoolean(chkIsActiveItem.Checked);
                    dr["is_mandatory"] = Convert.ToBoolean(chkIsMandatory.Checked);
                    dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom.Checked);
                    dr["create_date"] = DateTime.Now;

                    dr["item_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                    dr["measure_unit"] = txtMeasureUnit.Text;
                    dr["item_cost"] = Convert.ToDecimal(txtCost.Text);
                    dr["minimum_qty"] = Convert.ToDecimal(txtMinQty.Text);
                    dr["retail_multiplier"] = Convert.ToDecimal(txtRetailMulti.Text);
                    dr["labor_rate"] = Convert.ToDecimal(txtLabor.Text);
                    if (Convert.ToDecimal(txtLabor.Text) > 0)
                        dr["labor_id"] = 2;
                    else
                        dr["labor_id"] = 1;

                }

            }
            foreach (DataRow dr in table.Rows)
            {
                bool bFlagNew = false;
                bool bFlagNewIt = false;

                sectioninfo SecInfo = _db.sectioninfos.SingleOrDefault(l => l.section_id == Convert.ToInt32(dr["section_id"]));
                item_price itm = _db.item_prices.SingleOrDefault(l => l.item_id == Convert.ToInt32(dr["item_id"]));

                if (SecInfo == null)
                {
                    SecInfo = new sectioninfo();
                    bFlagNew = true;
                    if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == Convert.ToInt32(hdnSubItemParentId.Value) && l.is_disable == false && l.section_name == dr["section_name"].ToString()).SingleOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        lblItemResult.Text = csCommonUtility.GetSystemRequiredMessage("Section name already exist. Please try another name.");
                        return;
                    }
                }
                if (itm == null)
                {
                    itm = new item_price();
                    bFlagNewIt = true;
                }

                string str = dr["section_name"].ToString().Trim();
                if (str.Length > 0)
                {
                    SecInfo.section_id = Convert.ToInt32(dr["section_id"]);
                    SecInfo.client_id = Convert.ToInt32(dr["client_id"]);
                    SecInfo.section_name = dr["section_name"].ToString();
                    SecInfo.parent_id = Convert.ToInt32(dr["parent_id"]);
                    SecInfo.section_notes = dr["section_notes"].ToString();
                    SecInfo.section_level = Convert.ToInt32(dr["section_level"]);
                    SecInfo.section_serial = Convert.ToDecimal(dr["section_serial"]);
                    SecInfo.is_active = Convert.ToBoolean(dr["is_active"]);
                    SecInfo.is_mandatory = Convert.ToBoolean(dr["is_mandatory"]);
                    SecInfo.is_CommissionExclude = Convert.ToBoolean(dr["is_CommissionExclude"]);
                    SecInfo.is_disable = false;
                    SecInfo.create_date = DateTime.Now;
                    itm.item_id = Convert.ToInt32(dr["item_id"]);
                    itm.measure_unit = dr["measure_unit"].ToString();
                    itm.client_id = Convert.ToInt32(dr["client_id"]);
                    itm.item_cost = Convert.ToDecimal(dr["item_cost"]);
                    itm.minimum_qty = Convert.ToDecimal(dr["minimum_qty"]);
                    itm.retail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                    itm.labor_rate = Convert.ToDecimal(dr["labor_rate"]);
                    itm.labor_id = Convert.ToInt32(dr["labor_id"]);
                    itm.update_time = DateTime.Now;

                    if (bFlagNew)
                    {
                        SecInfo.serial = 100;
                        _db.sectioninfos.InsertOnSubmit(SecInfo);
                    }
                    if (bFlagNewIt)
                    {
                        _db.item_prices.InsertOnSubmit(itm);
                    }

                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Item Name is a required field");
                    lblItemResult.Text = csCommonUtility.GetSystemRequiredMessage("Item Name is a required field");
                    return;
                }
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            lblItemResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            _db.SubmitChanges();
            LoadItemInfo();


        }
    }

    protected void grdItem_Price_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Label lblActive = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblActive");
        CheckBox chkIsActiveItem = (CheckBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("chkIsActiveItem");

        Label lblAMandatory = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblAMandatory");
        CheckBox chkIsMandatory = (CheckBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("chkIsMandatory");

        Label lblAExcludeCom = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblAExcludeCom");
        CheckBox chkIsExcludeCom = (CheckBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("chkIsExcludeCom");



        TextBox txtItemName = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtItemName");
        Label lblItemnName = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblItemnName");
        TextBox txtMeasureUnit = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtMeasureUnit");
        Label lblMeasureUnit = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblMeasureUnit");

        TextBox txtCost = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtCost");
        Label lblCost = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblCost");

        TextBox txtMinQty = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtMinQty");
        Label lblMinQty = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblMinQty");

        TextBox txtRetailMulti = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtRetailMulti");
        Label lblRetailMulti = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblRetailMulti");

        TextBox txtLabor = (TextBox)grdItem_Price.Rows[e.NewEditIndex].FindControl("txtLabor");
        Label lblLabor = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblLabor");

        Label lblItemPriceDivision = (Label)grdItem_Price.Rows[e.NewEditIndex].FindControl("lblItemPriceDivision");
        DropDownList ddlItemPriceDivision = (DropDownList)grdItem_Price.Rows[e.NewEditIndex].FindControl("ddlItemPriceDivision");

        lblItemPriceDivision.Visible = false;
        ddlItemPriceDivision.Visible = true;


        chkIsActiveItem.Visible = true;
        lblActive.Visible = false;

        chkIsMandatory.Visible = true;
        lblAMandatory.Visible = false;

        chkIsExcludeCom.Visible = true;
        lblAExcludeCom.Visible = false;




        txtItemName.Visible = true;
        lblItemnName.Visible = false;


        txtItemName.Visible = true;
        lblItemnName.Visible = false;

        txtMeasureUnit.Visible = true;
        lblMeasureUnit.Visible = false;

        txtCost.Visible = true;
        lblCost.Visible = false;

        txtMinQty.Visible = true;
        lblMinQty.Visible = false;

        txtRetailMulti.Visible = true;
        lblRetailMulti.Visible = false;

        txtLabor.Visible = true;
        lblLabor.Visible = false;

        LinkButton btn = (LinkButton)grdItem_Price.Rows[e.NewEditIndex].Cells[10].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdItem_Price_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBox chkIsActiveItem = (CheckBox)grdItem_Price.Rows[e.RowIndex].FindControl("chkIsActiveItem");
        CheckBox chkIsMandatory = (CheckBox)grdItem_Price.Rows[e.RowIndex].FindControl("chkIsMandatory");
        CheckBox chkIsExcludeCom = (CheckBox)grdItem_Price.Rows[e.RowIndex].FindControl("chkIsExcludeCom");



        TextBox txtItemName = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtItemName");
        Label lblItemnName = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblItemnName");

        TextBox txtMeasureUnit = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtMeasureUnit");
        Label lblMeasureUnit = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblMeasureUnit");

        TextBox txtCost = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtCost");
        Label lblCost = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblCost");

        TextBox txtMinQty = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtMinQty");
        Label lblMinQty = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblMinQty");

        TextBox txtRetailMulti = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtRetailMulti");
        Label lblRetailMulti = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblRetailMulti");

        DropDownList ddlItemPriceDivision = (DropDownList)grdItem_Price.Rows[e.RowIndex].FindControl("ddlItemPriceDivision");

        TextBox txtLabor = (TextBox)grdItem_Price.Rows[e.RowIndex].FindControl("txtLabor");
        Label lblLabor = (Label)grdItem_Price.Rows[e.RowIndex].FindControl("lblLabor");
        int nLaborId = 1;
        if (Convert.ToDecimal(txtLabor.Text) > 0)
            nLaborId = 2;



        int nSectionId = Convert.ToInt32(grdItem_Price.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        int nParentId = Convert.ToInt32(grdItem_Price.DataKeys[Convert.ToInt32(e.RowIndex)].Values[1]);
        string strItemName = txtItemName.Text.Replace("'", "''");
        if (_db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == nParentId && l.is_disable == false && l.section_name == strItemName).SingleOrDefault() != null)
        {
            List<sectioninfo> sList = _db.sectioninfos.Where(l => l.client_id == 1 && l.parent_id == nParentId && l.is_disable == false && l.section_name == strItemName).ToList();
            foreach (sectioninfo objsec in sList)
            {
                if (objsec.section_id != nSectionId)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Item name is already exist. Please try another name to update");
                    lblItemResult.Text = csCommonUtility.GetSystemRequiredMessage("Item name is already exist. Please try another name to update");
                    return;
                }
            }

        }

        //string strQ = "UPDATE sectioninfo SET section_name='" + txtItemName.Text.Replace("'", "''") + "' , is_mandatory ='" + Convert.ToBoolean(chkIsMandatory.Checked) + "',is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom.Checked) + "',  is_active='" + Convert.ToBoolean(chkIsActiveItem.Checked) + "'  WHERE section_id=" + nSectionId + "  AND client_id=1";
        //_db.ExecuteCommand(strQ, string.Empty);
        //string strQItem = "UPDATE item_price SET measure_unit='" + txtMeasureUnit.Text + "', item_cost=" + Convert.ToDecimal(txtCost.Text) + ", minimum_qty=" + Convert.ToDecimal(txtMinQty.Text) + ", retail_multiplier=" + Convert.ToDecimal(txtRetailMulti.Text) + ", labor_rate=" + Convert.ToDecimal(txtLabor.Text) + ", update_time='" + DateTime.Now + "',labor_id=" + nLaborId + " WHERE item_id =" + nSectionId + " AND client_id=1";


        string strQ = "UPDATE sectioninfo SET section_name='" + txtItemName.Text.Replace("'", "''") + "' , is_mandatory ='" + Convert.ToBoolean(chkIsMandatory.Checked) + "',is_CommissionExclude ='" + Convert.ToBoolean(chkIsExcludeCom.Checked) + "',  is_active='" + Convert.ToBoolean(chkIsActiveItem.Checked) + "'  WHERE section_id=" + nSectionId;
        _db.ExecuteCommand(strQ, string.Empty);

        string strQItem = "UPDATE item_price SET client_id = " + ddlItemPriceDivision.SelectedValue + ", measure_unit='" + txtMeasureUnit.Text + "', item_cost=" + Convert.ToDecimal(txtCost.Text) + ", minimum_qty=" + Convert.ToDecimal(txtMinQty.Text) + ", retail_multiplier=" + Convert.ToDecimal(txtRetailMulti.Text) + ", labor_rate=" + Convert.ToDecimal(txtLabor.Text) + ", update_time='" + DateTime.Now + "',labor_id=" + nLaborId + " WHERE item_id =" + nSectionId;

        _db.ExecuteCommand(strQItem, string.Empty);

        LoadItemInfo();

        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        lblItemResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");


    }
   
    protected void grdItem_Price_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            CheckBox chkIsActiveItem = (CheckBox)e.Row.FindControl("chkIsActiveItem");
            CheckBox chkIsMandatory = (CheckBox)e.Row.FindControl("chkIsMandatory");
            CheckBox chkIsExcludeCom = (CheckBox)e.Row.FindControl("chkIsExcludeCom");

            Label lblAMandatory = (Label)e.Row.FindControl("lblAMandatory");
            Label lblActive = (Label)e.Row.FindControl("lblActive");

            Label lblAExcludeCom = (Label)e.Row.FindControl("lblAExcludeCom");

            
            TextBox txtItemName = (TextBox)e.Row.FindControl("txtItemName");
            Label lblItemnName = (Label)e.Row.FindControl("lblItemnName");

            TextBox txtMeasureUnit = (TextBox)e.Row.FindControl("txtMeasureUnit");
            Label lblMeasureUnit = (Label)e.Row.FindControl("lblMeasureUnit");

            TextBox txtCost = (TextBox)e.Row.FindControl("txtCost");
            Label lblCost = (Label)e.Row.FindControl("lblCost");

            TextBox txtMinQty = (TextBox)e.Row.FindControl("txtMinQty");
            Label lblMinQty = (Label)e.Row.FindControl("lblMinQty");

            TextBox txtRetailMulti = (TextBox)e.Row.FindControl("txtRetailMulti");
            Label lblRetailMulti = (Label)e.Row.FindControl("lblRetailMulti");

            TextBox txtLabor = (TextBox)e.Row.FindControl("txtLabor");
            Label lblLabor = (Label)e.Row.FindControl("lblLabor");

            int clientId = Convert.ToInt32(grdItem_Price.DataKeys[e.Row.RowIndex].Values[7]);

            Label lblItemPriceDivision = (Label)e.Row.FindControl("lblItemPriceDivision");
            DropDownList ddlItemPriceDivision = (DropDownList)e.Row.FindControl("ddlItemPriceDivision");

            BindDivition(ddlItemPriceDivision);

            DataClassesDataContext _db = new DataClassesDataContext();
            division dv = _db.divisions.FirstOrDefault(x => x.Id == clientId);
            if (dv != null)
            {
                lblItemPriceDivision.Text = dv.division_name;
            }


            if (chkIsMandatory.Checked)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");
                chkIsMandatory.Attributes.CssStyle.Add("color", "Violet");
                lblAMandatory.Text = "Yes";

            }
            else
            {
                lblAMandatory.Text = "No";
            }

            if (chkIsActiveItem.Checked)
            {
                lblActive.Text = "Yes";
            }
            else
            {
                lblActive.Text = "No";
            }

            if (chkIsExcludeCom.Checked)
            {
                lblAExcludeCom.Text = "Yes";
            }
            else
            {
                lblAExcludeCom.Text = "No";
            }

            string str = txtItemName.Text.Replace("&nbsp;", "");
            if (str == "" || Convert.ToInt32(grdItem_Price.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]) == 0)
            {
                chkIsActiveItem.Visible = true;
                lblActive.Visible = false;

                chkIsMandatory.Visible = true;
                lblAMandatory.Visible = false;

                chkIsExcludeCom.Visible = true;
                lblAExcludeCom.Visible = false;

                txtItemName.Visible = true;
                lblItemnName.Visible = false;

                txtMeasureUnit.Visible = true;
                lblMeasureUnit.Visible = false;

                lblItemPriceDivision.Visible = false;
                ddlItemPriceDivision.Visible = true;

                txtCost.Visible = true;
                lblCost.Visible = false;

                txtMinQty.Visible = true;
                lblMinQty.Visible = false;

                txtRetailMulti.Visible = true;
                lblRetailMulti.Visible = false;

                txtLabor.Visible = true;
                lblLabor.Visible = false;

                LinkButton btn = (LinkButton)e.Row.Cells[10].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }


        }
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Attributes.Add("title", "Unit of Measure");
            e.Row.Cells[2].Attributes.Add("title", "Your Cost");
            e.Row.Cells[3].Attributes.Add("title", "Minimum quantity to be sold");
            e.Row.Cells[4].Attributes.Add("title", "This determines the margin for your item");
            e.Row.Cells[5].Attributes.Add("title", "Hourly labor cost");
            e.Row.Cells[6].Attributes.Add("title", "");
            e.Row.Cells[7].Attributes.Add("title", "Mark 'Yes' if you want an item to be selectable during an estimate");
            e.Row.Cells[8].Attributes.Add("title", "Mark 'Yes' if you want an item to be a mandatory part of the estimate for this section");
            e.Row.Cells[9].Attributes.Add("title", "Mark 'Yes' if you want the sales commission to be excluded for an item");            
        }

    }
    protected void btnAddItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddItem.ID, btnAddItem.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.SingleOrDefault(c => c.section_id == Convert.ToInt32(hdnSubItemParentId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        hdnSectionLevel.Value = sinfo.section_level.ToString();
        var result = (from sin in _db.sectioninfos
                      where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == 1 && sin.section_id > Convert.ToInt32(hdnSectionLevel.Value) + 100
                      select sin.section_id);
        int nsectionId = 0;
        int n = result.Count();
        if (result != null && n > 0)
            nsectionId = result.Max();

        if (nsectionId == 0)
        {
            nsectionId = Convert.ToInt32(hdnSectionLevel.Value) + 100 + 1;
        }
        else
        {
            nsectionId = nsectionId + 1;
        }
        hdnSectionId.Value = nsectionId.ToString();
        string strSerial = nsectionId.ToString();
        string str = "";
        if (strSerial.Length < 5)
        {
            str = strSerial.Substring(2);
        }
        hdnSectionSerial.Value = hdnSectionLevel.Value + "." + str;
        lblSerial.Text = hdnSectionSerial.Value;

        DataTable table = (DataTable)Session["NewItem"];

        int nSecId = Convert.ToInt32(hdnSectionId.Value);
        bool contains = table.AsEnumerable().Any(row => nSecId == row.Field<int>("section_id"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            lblItemResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }
        foreach (GridViewRow di in grdItem_Price.Rows)
        {
            {
                CheckBox chkIsActiveItem = (CheckBox)di.FindControl("chkIsActiveItem");
                CheckBox chkIsMandatory = (CheckBox)di.FindControl("chkIsMandatory");
                CheckBox chkIsExcludeCom = (CheckBox)di.FindControl("chkIsExcludeCom");

                TextBox txtItemName = (TextBox)di.FindControl("txtItemName");
                Label lblItemnName = (Label)di.FindControl("lblItemnName");
                TextBox txtMeasureUnit = (TextBox)di.FindControl("txtMeasureUnit");
                Label lblMeasureUnit = (Label)di.FindControl("lblMeasureUnit");

                TextBox txtCost = (TextBox)di.FindControl("txtCost");
                Label lblCost = (Label)di.FindControl("lblCost");

                TextBox txtMinQty = (TextBox)di.FindControl("txtMinQty");
                Label lblMinQty = (Label)di.FindControl("lblMinQty");

                TextBox txtRetailMulti = (TextBox)di.FindControl("txtRetailMulti");
                Label lblRetailMulti = (Label)di.FindControl("lblRetailMulti");

                TextBox txtLabor = (TextBox)di.FindControl("txtLabor");
                Label lblLabor = (Label)di.FindControl("lblLabor");
                DataRow dr = table.Rows[di.RowIndex];

                dr["section_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = 1;
                dr["section_name"] = txtItemName.Text;
                dr["parent_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[1]);
                dr["section_notes"] = "";
                dr["section_level"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[2]);
                dr["section_serial"] = Convert.ToDecimal(grdItem_Price.DataKeys[di.RowIndex].Values[3]);
                dr["is_active"] = Convert.ToBoolean(chkIsActiveItem.Checked);
                dr["is_mandatory"] = Convert.ToBoolean(chkIsMandatory.Checked);
                dr["is_CommissionExclude"] = Convert.ToBoolean(chkIsExcludeCom.Checked);
                dr["create_date"] = DateTime.Now;

                dr["item_id"] = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                dr["measure_unit"] = txtMeasureUnit.Text;
                dr["item_cost"] = Convert.ToDecimal(txtCost.Text);
                dr["minimum_qty"] = Convert.ToDecimal(txtMinQty.Text);
                dr["retail_multiplier"] = Convert.ToDecimal(txtRetailMulti.Text);
                dr["labor_rate"] = Convert.ToDecimal(txtLabor.Text);
                if (Convert.ToDecimal(txtLabor.Text) > 0)
                    dr["labor_id"] = 2;
                else
                    dr["labor_id"] = 1;



            }

        }


        DataRow drNew = table.NewRow();
        drNew["section_id"] = Convert.ToInt32(hdnSectionId.Value);
        drNew["client_id"] = 1;
        drNew["section_name"] = "";
        drNew["parent_id"] = Convert.ToInt32(hdnSubItemParentId.Value);
        drNew["section_notes"] = "";
        drNew["section_level"] = Convert.ToInt32(hdnSectionLevel.Value);
        drNew["section_serial"] = Convert.ToDecimal(hdnSectionSerial.Value);
        drNew["is_active"] = true;
        drNew["create_date"] = DateTime.Now;

        drNew["item_id"] = Convert.ToInt32(hdnSectionId.Value);
        drNew["measure_unit"] = "";
        drNew["item_cost"] = 0;
        drNew["minimum_qty"] = 0;
        drNew["retail_multiplier"] = Convert.ToDecimal(hdnMultiplier.Value);
        drNew["labor_rate"] = 0;
        drNew["labor_id"] = 2;
        drNew["is_mandatory"] = false;
        drNew["is_CommissionExclude"] = false;


        //table.Rows.Add(drNew);
        table.Rows.InsertAt(drNew, 0);

        Session.Add("NewItem", table);
        grdItem_Price.DataSource = table;
        grdItem_Price.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "labor_id", "is_mandatory", "is_CommissionExclude", "client_id" }; ;
        grdItem_Price.DataBind();
        lblResult.Text = "";
        lblMainSecResult.Text = "";
        lblItemResult.Text = "";
        lblSubSecResult.Text = "";
    }
    #endregion

    //protected void btnDisable_Click(object sender, EventArgs e)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    bool ischecked = false;
    //    try
    //    {
    //        foreach (GridViewRow di in grdItem_Price.Rows)
    //        {
    //            {
    //                CheckBox chkIsDisable = (CheckBox)di.FindControl("chkIsDisable");

    //                int nSectionId = Convert.ToInt32(grdItem_Price.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
    //                if (chkIsDisable.Checked)
    //                {
    //                    ischecked = true;
    //                    //string strQ = "UPDATE sectioninfo SET is_disable='" + Convert.ToBoolean(chkIsDisable.Checked) + "'  WHERE section_id=" + nSectionId + "  AND client_id=1";
    //                    //_db.ExecuteCommand(strQ, string.Empty);


    //                    string strQ = "DELETE FROM sectioninfo WHERE section_id=" + nSectionId + "  AND client_id=1";
    //                    _db.ExecuteCommand(strQ, string.Empty);

    //                    string strQi = "DELETE FROM item_price WHERE item_id=" + nSectionId + "  AND client_id=1";
    //                    _db.ExecuteCommand(strQi, string.Empty);

    //                }
    //            }
    //        }
    //        if (ischecked)
    //        {
    //            LoadItemInfo();
    //            lblResult.Text = csCommonUtility.GetSystemMessage("Items has been Disabled Successfully");
    //        }
    //        else
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please select Item(s)");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    //protected void btnUpdateSerial_Click(object sender, EventArgs e)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    try
    //    {
    //        foreach (GridViewRow di in grdSubSection.Rows)
    //        {
    //            {
    //                TextBox txtItemSerial = (TextBox)di.FindControl("txtItemSerial");

    //                int nSectionId = Convert.ToInt32(grdSubSection.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);

    //                string strQ = "UPDATE sectioninfo SET serial='" + Convert.ToInt32(txtItemSerial.Text.Trim()) + "'  WHERE section_id=" + nSectionId + "  AND client_id=1";
    //                _db.ExecuteCommand(strQ, string.Empty);

    //            }
    //        }
    //        LoadSubSectionInfo();
    //        LoadTree();
    //        lblResult.Text = csCommonUtility.GetSystemMessage("Items Serial has been Updated Successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    //protected void btnUpdateItemSerial_Click(object sender, EventArgs e)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    try
    //    {
    //        foreach (GridViewRow di in grdItem_Price.Rows)
    //        {
    //            {
    //                TextBox txtItemSerial = (TextBox)di.FindControl("txtItemSerial");

    //                int nSectionId = Convert.ToInt32(grdItem_Price.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);

    //                string strQ = "UPDATE sectioninfo SET serial='" + Convert.ToInt32(txtItemSerial.Text.Trim()) + "'  WHERE section_id=" + nSectionId + "  AND client_id=1";
    //                _db.ExecuteCommand(strQ, string.Empty);

    //            }
    //        }
    //        LoadItemInfo();

    //        lblResult.Text = csCommonUtility.GetSystemMessage("Items Serial has been Updated Successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    protected void btnHome_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnHome.ID, btnHome.GetType().Name, "Click"); 
        lblMainSection.Visible = true;
        btnAddnewRow.Visible = true;
        grdMainSection.Visible = true;
        btnAddnewRow.Visible = true;
        hdnParentId.Value = "0";
        hdnSectionId.Value = "0";
        hdnSectionSerial.Value = "0";
        lblParent.Text = "";
        btnAddSubnewRow.Visible = false;
        btnAddItem.Visible = false;
        btnDeleteItem.Visible = false;
        lblTree.Visible = false;
        lblTree.Visible = false;
        lblSubSection.Visible = false;
        lblItemList.Visible = false;
        grdSubSection.Visible = false;
        grdItem_Price.Visible = false;

        LoadTree();



    }
    protected void grdItem_Price_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtItems = (DataTable)Session["NewItem"];

        string strShort = e.SortExpression + " " + hdnOrder.Value;

        if (hdnOrder.Value == "DESC")
        {
            hdnOrder.Value = "ASC";
        }
        else
        {
            hdnOrder.Value = "DESC";
        }
        strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtItems.DefaultView;
        dv.Sort = strShort;
        Session["NewItem"] = dv.ToTable();
        dtItems = (DataTable)Session["NewItem"];
        grdItem_Price.DataSource = dtItems;
        grdItem_Price.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "labor_id", "is_mandatory", "is_CommissionExclude", "client_id" };
        grdItem_Price.DataBind();

    }
    protected void grdSubSection_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSubSection = (DataTable)Session["SubSection"];

        string strShort = e.SortExpression + " " + hdnOrder.Value;

        if (hdnOrder.Value == "DESC")
        {
            hdnOrder.Value = "ASC";
        }
        else
        {
            hdnOrder.Value = "DESC";
        }
        strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtSubSection.DefaultView;
        dv.Sort = strShort;
        Session["SubSection"] = dv.ToTable();
        dtSubSection = (DataTable)Session["SubSection"];
        grdSubSection.DataSource = dtSubSection;
        grdSubSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "client_id" };
        grdSubSection.DataBind();

    }
    protected void grdMainSection_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtMainSection = (DataTable)Session["MainSection"];

        string strShort = e.SortExpression + " " + hdnOrder.Value;

        if (hdnOrder.Value == "DESC")
        {
            hdnOrder.Value = "ASC";
        }
        else
        {
            hdnOrder.Value = "DESC";
        }
        strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtMainSection.DefaultView;
        dv.Sort = strShort;
        Session["MainSection"] = dv.ToTable();
        dtMainSection = (DataTable)Session["MainSection"];

        grdMainSection.DataSource = dtMainSection;
        
        grdMainSection.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "cssClassName", "client_id" };
        grdMainSection.DataBind();

    }

    protected void btnDeleteItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDeleteItem.ID, btnDeleteItem.GetType().Name, "Click"); 
        try
        {
            bool IsChecked = false;
            DataClassesDataContext _db = new DataClassesDataContext();
            List<sectioninfo> listSecInfo = new List<sectioninfo>();
            List<item_price> listitm = new List<item_price>();

            List<sectioninfoDeleted> listSecInfoDeleted = new List<sectioninfoDeleted>();
            List<item_priceDeleted> listitmDeleted = new List<item_priceDeleted>();

            foreach (GridViewRow di in grdItem_Price.Rows)
            {

                CheckBox chkDelete = (CheckBox)di.FindControl("chkDelete");

                int nSectionId = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                if (chkDelete.Checked)
                {
                    IsChecked = true;
                    if (_db.sectioninfos.Any(l => l.section_id == nSectionId))
                    {
                        sectioninfo SecInfo = _db.sectioninfos.SingleOrDefault(l => l.section_id == nSectionId);
                        listSecInfo.Add(SecInfo);


                        _db.sectioninfos.DeleteOnSubmit(SecInfo);

                    }
                    if (_db.item_prices.Any(l => l.item_id == nSectionId))
                    {
                        item_price itm = _db.item_prices.SingleOrDefault(l => l.item_id == nSectionId);
                        listitm.Add(itm);
                        _db.item_prices.DeleteOnSubmit(itm);
                    }

                }
            }
            if (IsChecked)
            {
                listSecInfoDeleted = listSecInfo
                                    .Select(x => new sectioninfoDeleted()
                                    {
                                        client_id = x.client_id,
                                        section_id = x.section_id,
                                        section_name = x.section_name,
                                        parent_id = x.parent_id,
                                        section_notes = x.section_notes,
                                        section_level = x.section_level,
                                        section_serial = x.section_serial,
                                        is_active = x.is_active,
                                        create_date = x.create_date,
                                        cssClassName = x.cssClassName,
                                        is_mandatory = x.is_mandatory,
                                        is_disable = x.is_disable,
                                        is_CommissionExclude = x.is_CommissionExclude,
                                        serial = x.serial,
                                        Deleted_Date = DateTime.Now,
                                        Deleted_By = User.Identity.Name
                                    })
                                    .ToList();


                if (listSecInfoDeleted.Count() > 0)
                {
                    _db.sectioninfoDeleteds.InsertAllOnSubmit(listSecInfoDeleted);
                }


                listitmDeleted = listitm
                                .Select(x => new item_priceDeleted()
                                {
                                    item_id = x.item_id,
                                    client_id = x.client_id,
                                    measure_unit = x.measure_unit,
                                    item_cost = x.item_cost,
                                    minimum_qty = x.minimum_qty,
                                    retail_multiplier = x.retail_multiplier,
                                    labor_rate = x.labor_rate,
                                    update_time = x.update_time,
                                    labor_id = x.labor_id,
                                    Deleted_Date = DateTime.Now,
                                    Deleted_By = User.Identity.Name
                                })
                                .ToList();

                if (listitmDeleted.Count() > 0)
                {
                    _db.item_priceDeleteds.InsertAllOnSubmit(listitmDeleted);
                }

                _db.SubmitChanges();

                lblResult.Text = csCommonUtility.GetSystemMessage("Item(s) deleted successfully");
                lblItemResult.Text = csCommonUtility.GetSystemMessage("Item(s) deleted successfully");

                LoadItemInfo();
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please select Item(s)");
                lblItemResult.Text = csCommonUtility.GetSystemErrorMessage("Please select Item(s)");
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblItemResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    //protected void btnUpload_Click(object sender, EventArgs e)
    //{
    //    OleDbConnection oledbConn = null;
    //    string strTest = "";
    //    try
    //    {
    //        if (ExcelUploader.FileName.Length == 0)
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid file name.");
    //            return;
    //        }

    //        if (!(ExcelUploader.FileName.ToLower().Contains(".xlsx") || ExcelUploader.FileName.ToLower().Contains(".xls")))
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid file format.");
    //            return;


    //        }


    //        string sFileName = "Section_Upload_" + DateTime.Now.ToString("yyyyMMddhhmmss");
    //        if (ExcelUploader.FileName.ToLower().Contains(".xlsx"))
    //        {
    //            sFileName += ".xlsx";
    //        }
    //        else
    //        {
    //            sFileName += ".xls";
    //        }

    //        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"];
    //        if (Directory.Exists(sFilePath) == false)
    //        {
    //            Directory.CreateDirectory(sFilePath);

    //        }
    //        sFilePath = sFilePath + "\\" + sFileName;
    //        ExcelUploader.PostedFile.SaveAs(sFilePath);

    //        string sourceFile = sFilePath;

    //        string destinationFile = System.Configuration.ConfigurationManager.AppSettings["TempDir"];

    //        if (Directory.Exists(destinationFile) == false)
    //        {
    //            Directory.CreateDirectory(destinationFile);

    //        }
    //        destinationFile = destinationFile + "\\" + sFileName;


    //        lblResult.Text = "";

    //        //OleDbConnection oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" +
    //        //                   "Data Source=" + sFilePath + ";" +
    //        //                   "Extended Properties=Excel 8.0;");

    //        oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sFilePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
    //        oledbConn.Open();
    //        OleDbCommand cmd = new OleDbCommand(); ;
    //        OleDbDataAdapter oleda = new OleDbDataAdapter();
    //        DataSet ds = new DataSet();

    //        // selecting distict list of Slno 
    //        cmd.Connection = oledbConn;

    //        string sSheet = GetSheetNameInExcel(sFilePath, txtMainSectionName.Text.Trim()); // txtSectionName.Text.Trim(); //



    //        cmd.CommandType = CommandType.Text;
    //        cmd.CommandText = "SELECT * FROM [" + sSheet + "]";
    //        oleda = new OleDbDataAdapter(cmd);
    //        oleda.Fill(ds, "Section");
    //        DataTable dtProduct = ds.Tables["Section"];

    //        DataClassesDataContext _db = new DataClassesDataContext();

    //        foreach (DataRow dr in dtProduct.Rows)
    //        {
    //            //if (dr["item_id"].ToString() == "")
    //            //    strTest = dr["item_id"].ToString();

    //            //if(dr["item_cost"].ToString() == "")
    //            //    strTest = dr["item_id"].ToString();

    //            //if (dr["minimum_qty"].ToString() == "")
    //            //    strTest = dr["minimum_qty"].ToString();

    //            //if (dr["retail_multiplier"].ToString() == "")
    //            //    strTest = dr["retail_multiplier"].ToString();

    //            //if (dr["labor_rate"].ToString() == "")
    //            //    strTest = dr["labor_rate"].ToString();


    //            //string strQ = "UPDATE item_price SET item_cost=" + dr["item_cost"] + ", minimum_qty = " + dr["minimum_qty"] + ", " +
    //            //    " retail_multiplier = " + dr["retail_multiplier"] + ", labor_rate = " + dr["labor_rate"] +
    //            //    " WHERE item_id=" + dr["item_id"] + "  AND client_id=1";
    //            //_db.ExecuteCommand(strQ, string.Empty);
    //            //strTest = dr["item_id"].ToString();

    //        }

    //        //btnSave.Visible = true;
    //        //btnUpload.Visible = false;
    //        LoadItemInfo();


    //    }
    //    catch (Exception ex)
    //    {

    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace) +", "+ strTest;

    //    }
    //    finally
    //    {
    //        // Clean up.
    //        if (oledbConn != null)
    //        {
    //            oledbConn.Close();
    //            oledbConn.Dispose();
    //        }

    //    }
    //    //System.IO.File.Move(sourceFile, destinationFile);
    //}
    //public string GetSheetNameInExcel(string filePath, string strSheetName)
    //{
    //    string sSheetName = "";
    //    OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
    //    String strExtendedProperties = String.Empty;
    //    sbConnection.DataSource = filePath;
    //    sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
    //    strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
    //    sbConnection.Add("Extended Properties", strExtendedProperties);
    //    using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
    //    {
    //        conn.Open();
    //        DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

    //        foreach (DataRow drSheet in dtSheet.Rows)
    //        {
    //            if (drSheet["TABLE_NAME"].ToString().Trim().Contains(strSheetName + "$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
    //            {
    //                sSheetName = drSheet["TABLE_NAME"].ToString();
    //                break;
    //            }
    //        }

    //        conn.Close();
    //        conn.Dispose();


    //    }
    //    return sSheetName;
    //}
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        try
        {
            lblResult.Text = "";
            lblMessage.Text = "";
            lblItemResult.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();

            if (txtMultiplier.Text.Trim() == "")
            {
                modUpdateMultiplier.Show();
                lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Missing Retail Multiplier.");
                return;
            }
            foreach (GridViewRow di in grdItem_Price.Rows)
            {
                CheckBox chkDelete = (CheckBox)di.FindControl("chkDelete");
                int nSectionId = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                if (chkDelete.Checked)
                {
                    if (_db.item_prices.Any(l => l.item_id == nSectionId))
                    {
                        item_price itm = _db.item_prices.SingleOrDefault(l => l.item_id == nSectionId);
                        itm.retail_multiplier = Convert.ToDecimal(txtMultiplier.Text.Trim());
                       
                    }
                }
            }
             _db.SubmitChanges();
             lblResult.Text = csCommonUtility.GetSystemMessage("Item(s) updated successfully");
             lblItemResult.Text = csCommonUtility.GetSystemMessage("Item(s) updated successfully");
               LoadItemInfo();
               lblMessage.Text = "";
               txtMultiplier.Text = "";
           
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblItemResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnClosePopUp_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClosePopUp.ID, btnClosePopUp.GetType().Name, "Click"); 
        modUpdateMultiplier.Hide();
    }

    protected void InkPriceUpdate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, InkPriceUpdate.ID, InkPriceUpdate.GetType().Name, "Click"); 
        try
        {
            string Ids = string.Empty;
            foreach (GridViewRow di in grdItem_Price.Rows)
            {
                CheckBox chkDelete = (CheckBox)di.FindControl("chkDelete");
                if (chkDelete.Checked)
                {
                    int id = Convert.ToInt32(grdItem_Price.DataKeys[di.RowIndex].Values[0]);
                    Ids += id + ",";
                }
            }
             Ids = Ids.Trim().TrimEnd(',');
             if (Ids != "")
             {
                DataTable dtItem = new DataTable();

                 if (Session["NewItem"] != null)
                     dtItem = (DataTable)Session["NewItem"];
                 else
                     dtItem = LoadItemPriceTable();

                 DataTable dtEditList2 =  LoadItemPriceTable();
                 DataView dv = dtItem.DefaultView;
                 dv.RowFilter = "section_id IN (" + Ids + ")";
                 DataRow drNew = null;
                 for (int i = 0; i < dv.Count; i++)
                 {
                     drNew = dtEditList2.NewRow();
                     drNew["section_id"] = dv[i]["section_id"];
                     drNew["client_id"] = dv[i]["client_id"];
                     drNew["section_name"] = dv[i]["section_name"];
                     drNew["parent_id"] = dv[i]["parent_id"];
                     drNew["section_notes"] = dv[i]["section_notes"];
                     drNew["section_level"] = dv[i]["section_level"];
                     drNew["section_serial"] = dv[i]["section_serial"];
                     drNew["is_active"] = dv[i]["is_active"];
                     drNew["create_date"] = dv[i]["create_date"];
                     drNew["item_id"] = dv[i]["item_id"];
                     drNew["measure_unit"] = dv[i]["measure_unit"];
                     drNew["item_cost"] = dv[i]["item_cost"];
                     drNew["minimum_qty"] = dv[i]["minimum_qty"];
                     drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                     drNew["labor_rate"] = dv[i]["labor_rate"];
                     drNew["labor_id"] = dv[i]["labor_id"];
                     drNew["is_mandatory"] = dv[i]["is_mandatory"];
                     drNew["is_CommissionExclude"] = dv[i]["is_CommissionExclude"];
                     dtEditList2.Rows.Add(drNew);

                 }
                 grdPriceUpdate.DataSource = dtEditList2;
                 grdPriceUpdate.DataKeyNames = new string[] { "section_id", "parent_id", "section_level", "section_serial", "labor_id", "is_mandatory", "is_CommissionExclude" };
                 grdPriceUpdate.DataBind();
                 modPriceUpdate.Show();
                 return;
             }
               
             
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblItemResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnUpdatePrice_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdatePrice.ID, btnUpdatePrice.GetType().Name, "Click"); 
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();

            lblResult.Text = "";
            lblMessage.Text = "";
            lblItemResult.Text = "";
            foreach (GridViewRow di in grdPriceUpdate.Rows)
            {
                       CheckBox chkIsActiveItem = (CheckBox)di.FindControl("chkIsActiveItem");
                       CheckBox chkIsMandatory = (CheckBox)di.FindControl("chkIsMandatory");
                       CheckBox chkIsExcludeCom = (CheckBox)di.FindControl("chkIsExcludeCom");
                       TextBox txtItemName = (TextBox)di.FindControl("txtItemName");

                       TextBox txtMinQty = (TextBox)di.FindControl("txtMinQty");
                       TextBox txtCost = (TextBox)di.FindControl("txtCost");
                       TextBox txtMeasureUnit = (TextBox)di.FindControl("txtMeasureUnit");
                       TextBox txtMultiplier = (TextBox)di.FindControl("txtRetailMulti");
                       TextBox txtLabor = (TextBox)di.FindControl("txtLabor");
                      int nSectionId = Convert.ToInt32(grdPriceUpdate.DataKeys[di.RowIndex].Values[0]);
                      if (_db.sectioninfos.Any(l => l.section_id == nSectionId))
                      {
                          sectioninfo SecInfo = _db.sectioninfos.SingleOrDefault(l => l.section_id == nSectionId);
                          if(txtItemName.Text!="")
                           SecInfo.section_name = txtItemName.Text;
                          SecInfo.is_active = chkIsActiveItem.Checked;
                          SecInfo.is_mandatory = chkIsMandatory.Checked;
                          SecInfo.is_CommissionExclude = chkIsExcludeCom.Checked;
                      }

                    if (_db.item_prices.Any(l => l.item_id == nSectionId))
                    {
                        item_price itm = _db.item_prices.SingleOrDefault(l => l.item_id == nSectionId);
                        if (txtCost.Text != "")
                            itm.item_cost = Convert.ToDecimal(txtCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        else
                            itm.item_cost = 0;
                        if (txtMultiplier.Text != "")
                            itm.retail_multiplier = Convert.ToDecimal(txtMultiplier.Text.Trim());
                        else
                            itm.retail_multiplier = 0;
                         if (txtMinQty.Text != "")
                             itm.minimum_qty = Convert.ToDecimal(txtMinQty.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                         else
                             itm.minimum_qty = 1;
                        if(txtMeasureUnit.Text!="")  
                           itm.measure_unit = txtMeasureUnit.Text;
                        if (txtLabor.Text != "")
                            itm.labor_rate = Convert.ToDecimal(txtLabor.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        else
                            itm.labor_rate = 0;

                    }
                
            }
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Item(s) updated successfully");
            lblItemResult.Text = csCommonUtility.GetSystemMessage("Item(s) updated successfully");
            LoadItemInfo();
            lblMessage.Text = "";
         

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblItemResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdPriceUpdate_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblPriceUpdateDivision = (Label)e.Row.FindControl("lblPriceUpdateDivision");
            DropDownList ddlPriceUpdateDivision = (DropDownList)e.Row.FindControl("ddlPriceUpdateDivision");
            BindDivition(ddlPriceUpdateDivision);

            lblPriceUpdateDivision.Visible = false;
            ddlPriceUpdateDivision.Visible = true;
        }
    }
}