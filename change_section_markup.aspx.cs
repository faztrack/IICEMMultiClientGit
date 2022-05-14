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

public partial class change_section_markup : System.Web.UI.Page
{
    string strDetails = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin021") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            LoadTree();
            pnlItem.Visible = false;
        }
    }

    private void LoadTree()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE  client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ")";
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        // List<sectioninfo> list = _db.sectioninfos.Where(c => c.client_id == 1).ToList();
        trvSection.Nodes.Clear();
        foreach (sectioninfo sec in list)
        {
            string name = sec.section_name;
            if (sec.parent_id == 0)
            {
                TreeNode node = new TreeNode(sec.section_name, sec.section_id.ToString());
                trvSection.Nodes.Add(node);
                AddChildMenu(node, sec);
            }
        }



    }
    private void AddChildMenu(TreeNode parentNode, sectioninfo sec)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE  client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ")";
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        //List<sectioninfo> list = _db.sectioninfos.Where(c => c.client_id == 1).ToList();
        foreach (sectioninfo subsec in list)
        {
            if (subsec.parent_id.ToString() == parentNode.Value)
            {
                TreeNode node = new TreeNode(subsec.section_name, subsec.section_id.ToString());
                parentNode.ChildNodes.Add(node);
                AddChildMenu(node, subsec);
            }
        }
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (chkNewSubSection.Checked)
        {
            if (lblParent.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select section from tree.");
                
                return;
            }
           
            if (txtSectionName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sub Section Name is required field");
                
                return;
            }
            //txtSectionName.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            sectioninfo sinfo = new sectioninfo();
            sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnParentId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            hdnSectionLevel.Value = sinfo.section_level.ToString();
            var result = (from sin in _db.sectioninfos
                          where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && sin.section_id < Convert.ToInt32(hdnSectionLevel.Value) + 100
                          select sin.section_id);
            int nsectionId = 0;
            int n = result.Count();
            if (result != null && n > 0)
                nsectionId = result.Max();

            if (hdnParentId.Value == "0")
            {
                nsectionId = nsectionId + 1000;
            }
            else
            {
                nsectionId = nsectionId + 1;
            }
            hdnSectionId.Value = nsectionId.ToString();
            hdnSectionSerial.Value = nsectionId.ToString();
            lblSerial.Text = hdnSectionSerial.Value;
        }
        if (chkNewItem.Checked)
        {
            if (lblParent.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select section from tree.");
                
                return;
            }
            
            if (txtSectionName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Item Name is required field");
                
                return;
            }
            if (txtUom.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing UoM.");
                
                return;
            }
            if (txtMinQty.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Minimun Quantity.");
                
                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtMinQty.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid minimum quantity.");
                    
                    return;
                }
            }
            if (txtCost.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Retail Price.");
                
                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtCost.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Retail Price.");
                    
                    return;
                }
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            sectioninfo sinfo = new sectioninfo();
            sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnParentId.Value) && c.client_id == 1);
            hdnSectionLevel.Value = sinfo.section_level.ToString();

            var result = (from sin in _db.sectioninfos
                          where sin.section_level == Convert.ToInt32(hdnSectionLevel.Value) && sin.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && sin.section_id > Convert.ToInt32(hdnSectionLevel.Value) + 100
                          select sin.section_id);
            int nsectionId = 0;
            int n = result.Count();
            if (result != null && n > 0)
                nsectionId = result.Max();

            if (hdnParentId.Value == "0")
            {
                nsectionId = nsectionId + 1000;
            }
            else if (nsectionId == 0)
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
        }
        if (chkNewSection.Checked)
        {
            if (txtSectionName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Section Name is required field");
                
                return;
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            var result = (from sin in _db.sectioninfos
                          where sin.parent_id == Convert.ToInt32(hdnParentId.Value) && sin.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select sin.section_id);
            int nsectionId = 0;
            int n = result.Count();
            if (result != null && n > 0)
                nsectionId = result.Max();

            nsectionId = nsectionId + 1000;
            hdnSectionId.Value = nsectionId.ToString();
            hdnSectionLevel.Value = nsectionId.ToString();
            hdnSectionSerial.Value = nsectionId.ToString();
        }


        //
        //
        sectioninfo si = new sectioninfo();
        item_price it = new item_price();
        si.section_name = txtSectionName.Text;
        si.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        si.section_id = Convert.ToInt32(hdnSectionId.Value);
        si.parent_id = Convert.ToInt32(hdnParentId.Value);
        si.section_level = Convert.ToInt32(hdnSectionLevel.Value);
        si.section_serial = Convert.ToDecimal(hdnSectionSerial.Value);
        si.section_notes = "";
        si.is_active = true;
        si.create_date = DateTime.Today;
        if (hdnItem.Value == "1")
        {
            it.item_id = Convert.ToInt32(hdnSectionId.Value);
            it.measure_unit = txtUom.Text;
            it.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            it.item_cost = Convert.ToDecimal(txtCost.Text);
            it.minimum_qty = Convert.ToDecimal(txtMinQty.Text);
            it.retail_multiplier = Convert.ToDecimal(txtRetail.Text);
            if (txtLabor.Text == "")
                txtLabor.Text = "0";
            if (rdoLabor.SelectedValue == "2")
                it.labor_rate = Convert.ToDecimal(txtLabor.Text);
            else
                it.labor_rate = 0;
            it.labor_id = Convert.ToInt32(rdoLabor.SelectedValue);
            it.update_time = DateTime.Today;
        }
        if (chkNewItem.Checked)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            _db.sectioninfos.InsertOnSubmit(si);
            _db.item_prices.InsertOnSubmit(it);
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
            
            //LoadTree();
            //pnlItem.Visible = false;
            txtSectionName.Text = "";
            txtUom.Text = "";
            txtCost.Text = "";
            txtMinQty.Text = "";
            txtRetail.Text = "";
            txtLabor.Text = "";
            lblSerial.Text = "";
            //BindGrid();
        }
        else if (chkNewSubSection.Checked)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            _db.sectioninfos.InsertOnSubmit(si);
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
            
            LoadTree();
            pnlItem.Visible = false;
            txtSectionName.Text = "";
            txtUom.Text = "";
            txtCost.Text = "";
            txtMinQty.Text = "";
            txtRetail.Text = "";
            txtLabor.Text = "";
            lblSerial.Text = "";
        }
        else if (chkNewSection.Checked)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            _db.sectioninfos.InsertOnSubmit(si);
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
            
            LoadTree();
            pnlItem.Visible = false;
            txtSectionName.Text = "";
            txtUom.Text = "";
            txtCost.Text = "";
            txtMinQty.Text = "";
            txtRetail.Text = "";
            txtLabor.Text = "";
            lblSerial.Text = "";
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            if (hdnItem.Value == "1")
            {
                strQ = "UPDATE item_price SET measure_unit='" + it.measure_unit + "', item_cost=" + it.item_cost + ", minimum_qty=" + it.minimum_qty + ", retail_multiplier=" + it.retail_multiplier + ", labor_rate=" + it.labor_rate + ", update_time='" + it.update_time + "',labor_id=" + it.labor_id + " WHERE item_id =" + it.item_id + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
                strQ = "UPDATE sectioninfo SET section_name='" + si.section_name.Replace("'", "''") + "', parent_id=" + si.parent_id + ", section_notes='" + si.section_notes + "', section_level=" + si.section_level + ", section_serial=" + si.section_serial + ", is_active='" + si.is_active + "' , create_date='" + si.create_date + "' WHERE section_id =" + si.section_id + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
                chkNewItem.Checked = false;
            }
            else
            {
                strQ = "UPDATE sectioninfo SET section_name='" + si.section_name.Replace("'", "''") + "', parent_id=" + si.parent_id + ", section_notes='" + si.section_notes + "', section_level=" + si.section_level + ", section_serial=" + si.section_serial + ", is_active='" + si.is_active + "' , create_date='" + si.create_date + "' WHERE section_id =" + si.section_id + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
            }
            LoadTree();
            pnlItem.Visible = false;
            txtSectionName.Text = "";
            txtUom.Text = "";
            txtCost.Text = "";
            txtMinQty.Text = "";
            txtRetail.Text = "";
            txtLabor.Text = "";
            lblSerial.Text = "";
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
            
        }
        BindGrid();
        hdnItem.Value = "0";
        hdnSectionId.Value = hdnTrvSelectedValue.Value;

    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    public string GetItemDetialsForUpdateItem(int SectionId)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetails = sec1.section_name + " >> " + strDetails;
            GetItemDetialsForUpdateItem(Convert.ToInt32(sec1.parent_id));
        }
        return strDetails;
    }
    public void BindGrid()
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnTrvSelectedValue.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        lblParent.Text = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnTrvSelectedValue.Value));
        
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        txtSectionName.Text = sinfo.section_name;
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        lblParent.ForeColor = Color.Blue;
       
        var item = from it in _db.item_prices
                   join si in _db.sectioninfos on it.item_id equals si.section_id
                   where si.parent_id == Convert.ToInt32(hdnTrvSelectedValue.Value)
                   select new ItemPriceModel()
                   {
                       item_id = (int)it.item_id,
                       section_name = si.section_name,
                       measure_unit = it.measure_unit,
                       item_cost = (decimal)it.item_cost,
                       minimum_qty = (decimal)it.minimum_qty,
                       retail_multiplier = (decimal)it.retail_multiplier,
                       labor_rate = (decimal)it.labor_rate
                   };
        grdItemPrice.DataSource = item;
        grdItemPrice.DataKeyNames = new string[] { "item_id" };
        grdItemPrice.DataBind();
    }

    protected void trvSection_SelectedNodeChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, trvSection.ID, trvSection.GetType().Name, "SelectedNodeChanged"); 
        hdnSectionId.Value = trvSection.SelectedValue;
        hdnTrvSelectedValue.Value = trvSection.SelectedValue;
        lblResult.Text = "";
        btnSave.Text = "Update";
        chkNewItem.Checked = false;
        chkNewSubSection.Checked = false;
        chkNewSection.Checked = false;
        lblTree.Visible = true;
        grdItemPrice.Visible = true;
        BindGrid();
        pnlItem.Visible = false;
        lblSection.Text = "Section Name";

    }
    protected void grdItemPrice_RowEditing(object sender, GridViewEditEventArgs e)
    {

        hdnSectionId.Value = grdItemPrice.DataKeys[e.NewEditIndex].Values[0].ToString();
        txtSectionName.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[0].Text.Replace("amp;", "");
        txtUom.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[1].Text;
        txtCost.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[2].Text;
        txtMinQty.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[3].Text;
        txtRetail.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[4].Text;
        txtLabor.Text = grdItemPrice.Rows[e.NewEditIndex].Cells[5].Text;
        txtLabor.ToolTip = grdItemPrice.Rows[e.NewEditIndex].Cells[5].Text;
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo si = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        hdnParentId.Value = Convert.ToInt32(si.parent_id).ToString();
        hdnSectionLevel.Value = Convert.ToInt32(si.section_level).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(si.section_serial).ToString();
        if (Convert.ToDecimal(txtLabor.Text) > 0)
        {
            rdoLabor.SelectedValue = "2";
            txtLabor.Text = txtLabor.ToolTip;
            lblLabor0.Visible = true;
            txtLabor.Visible = true;
        }
        else
        {
            rdoLabor.SelectedValue = "1";
            txtLabor.Text = "0";
            lblLabor0.Visible = false;
            txtLabor.Visible = false;
        }
        lblSerial.Text = hdnSectionSerial.Value;
        lblSection.Text = "Item Name";
        btnSave.Text = "Update";
        pnlItem.Visible = true;
        hdnItem.Value = "1";
        lblResult.Text = "";
        chkNewItem.Checked = false;
        chkNewSubSection.Checked = false;
        chkNewSection.Checked = false;

    }

    protected void chkNewSubSection_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkNewSubSection.ID, chkNewSubSection.GetType().Name, "CheckedChanged"); 
        if (chkNewSubSection.Checked)
        {
            chkNewItem.Checked = false;
            chkNewSection.Checked = false;
            pnlItem.Visible = false;
            if (trvSection.SelectedValue == "")
            {
                if (lblParent.Text.Length == 0)
                {
                    hdnSectionId.Value = "0";
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select any parent node");
                    
                    return;
                }
                else
                {
                    hdnParentId.Value = hdnTrvSelectedValue.Value;
                }

            }
            //hdnParentId.Value = hdnSectionId.Value;
            hdnParentId.Value = hdnTrvSelectedValue.Value;
            txtSectionName.Text = "";
            lblSection.Text = "Sub Section Name";
            //DataClassesDataContext _db = new DataClassesDataContext();
            //sectioninfo sinfo = new sectioninfo();
            //sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnParentId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            //hdnSectionLevel.Value = sinfo.section_level.ToString();
            //var result = (from si in _db.sectioninfos
            //              where si.section_level == Convert.ToInt32(hdnSectionLevel.Value) && si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.section_id < Convert.ToInt32(hdnSectionLevel.Value) + 100
            //              select si.section_id);
            //int nsectionId = 0;
            //int n = result.Count();
            //if (result != null && n > 0)
            //    nsectionId = result.Max();

            //if (hdnParentId.Value == "0")
            //{
            //    nsectionId = nsectionId + 1000;
            //}
            //else
            //{
            //    nsectionId = nsectionId + 1;
            //}
            //hdnSectionId.Value = nsectionId.ToString();
            //hdnSectionSerial.Value = nsectionId.ToString();
            //lblSerial.Text = hdnSectionSerial.Value;

            hdnItem.Value = "3";
            btnSave.Text = "Save";
        }
    }
    protected void chkNewItem_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkNewItem.ID, chkNewItem.GetType().Name, "Click"); 
        if (chkNewItem.Checked)
        {
            btnSave.Text = "Save";
            chkNewSubSection.Checked = false;
            chkNewSection.Checked = false;
            lblResult.Text = "";
            txtSectionName.Text = "";
            lblSection.Text = "Item Name";

            if (trvSection.SelectedValue == "")
            {
                if (lblParent.Text.Length == 0)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select any parent node");
                    
                    return;
                }
                else
                {
                    hdnParentId.Value = hdnTrvSelectedValue.Value;
                }
                
            }
            else
            {
                hdnParentId.Value = hdnTrvSelectedValue.Value;
            }
            if (rdoLabor.SelectedValue == "1")
            {
                txtLabor.Text = "0";
                lblLabor0.Visible = false;
                txtLabor.Visible = false;
            }
            else
            {
                lblLabor0.Visible = true;
                txtLabor.Visible = true;
            }

            //hdnParentId.Value = hdnSectionId.Value;

            //DataClassesDataContext _db = new DataClassesDataContext();
            //sectioninfo sinfo = new sectioninfo();
            //sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnParentId.Value) && c.client_id == 1);
            //hdnSectionLevel.Value = sinfo.section_level.ToString();

            //var result = (from si in _db.sectioninfos
            //              where si.section_level == Convert.ToInt32(hdnSectionLevel.Value) &&  si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.section_id > Convert.ToInt32(hdnSectionLevel.Value) + 100
            //              select si.section_id);
            //int nsectionId = 0;
            //int n = result.Count();
            //if (result != null&& n>0)
            //    nsectionId = result.Max();

            //if (hdnParentId.Value == "0")
            //{
            //    nsectionId = nsectionId + 1000;
            //}
            //else if(nsectionId==0)
            //{
            //    nsectionId = Convert.ToInt32(hdnSectionLevel.Value) + 100 + 1;
            //}
            //else 
            //{
            //    nsectionId = nsectionId+1;
            //}
            //hdnSectionId.Value = nsectionId.ToString();
            //string strSerial = nsectionId.ToString();
            //string str = "";
            //if (strSerial.Length <5 )
            //{
            //    str = strSerial.Substring(2);
            //}
            //hdnSectionSerial.Value = hdnSectionLevel.Value + "." + str;
            //lblSerial.Text = hdnSectionSerial.Value;
            pnlItem.Visible = true;
            hdnItem.Value = "1";
        }
        else
        {
            pnlItem.Visible = false;
            hdnItem.Value = "0";
            lblResult.Text = "";
        }
    }
    protected void chkNewSection_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkNewSection.ID, chkNewSection.GetType().Name, "Click"); 
        if (chkNewSection.Checked)
        {
            btnSave.Text = "Save";
            lblResult.Text = "";
            hdnParentId.Value = "0";
            lblParent.Text = "";
            lblTree.Visible = false;
            grdItemPrice.Visible = false;
            txtSectionName.Text = "";
            chkNewItem.Checked = false;
            chkNewSubSection.Checked = false;
            lblSection.Text = "Section Name";
            pnlItem.Visible = false;
            //DataClassesDataContext _db = new DataClassesDataContext();
            //var result = (from si in _db.sectioninfos
            //              where si.parent_id == Convert.ToInt32(hdnParentId.Value) && si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //              select si.section_id);
            //int nsectionId = 0;
            //int n = result.Count();
            //if (result != null && n > 0)
            //    nsectionId = result.Max();

            //nsectionId = nsectionId + 1000;
            //hdnSectionId.Value = nsectionId.ToString();
            //hdnSectionLevel.Value = nsectionId.ToString();
            //hdnSectionSerial.Value = nsectionId.ToString();
            hdnItem.Value = "2";
        }

    }
    protected void rdoLabor_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoLabor.ID, rdoLabor.GetType().Name, "Click"); 
        if (rdoLabor.SelectedValue == "1")
        {
            //txtLabor.Text = "0";
            lblLabor0.Visible = false;
            txtLabor.Visible = false;
        }
        else
        {
            lblLabor0.Visible = true;
            txtLabor.Visible = true;
        }
    }

    }

