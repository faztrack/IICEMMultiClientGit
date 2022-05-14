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

public partial class VendorManger : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            LoadVendorInfo();
        }

    }
    private void LoadVendorInfo()
    {
        if (Session["oUser"] == null)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
        if (Page.User.IsInRole("admin013") == false)
        {
            // No Permission Page.
            Response.Redirect("nopermission.aspx");
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.vendor_infos
                   where it.client_id == 1
                   select new vendorInfrmation()
                   {
                       vendor_id = (int)it.vendor_id,
                       client_id = (int)it.client_id,
                       vendor_name = it.vendor_name,
                       is_active = (bool)it.is_active,
                       create_date = (DateTime)it.create_date
                   };
        foreach (vendorInfrmation vinfo in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["vendor_id"] = vinfo.vendor_id;
            drNew["client_id"] = vinfo.client_id;
            drNew["vendor_name"] = vinfo.vendor_name;
            drNew["is_active"] = vinfo.is_active;
            drNew["create_date"] = vinfo.create_date;
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["vendor_id"] = 0;
            drNew["client_id"] =1;
            drNew["vendor_name"] = "";
            drNew["is_active"] = true;
            drNew["create_date"] = DateTime.Now;
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("vendor", tmpTable);
        grdVendor.DataSource = tmpTable;
        grdVendor.DataKeyNames = new string[] { "vendor_id" };
        grdVendor.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("vendor_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("vendor_name", typeof(string));
        table.Columns.Add("is_active", typeof(bool));
        table.Columns.Add("create_date", typeof(DateTime));
        
        return table;
    }
   
    protected void grdVendor_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            
            DataTable table = (DataTable)Session["vendor"];

            foreach (GridViewRow di in grdVendor.Rows)
            {
                {
                    CheckBox chkIsActive = (CheckBox)di.FindControl("chkIsActive");
                    TextBox txtVendorName = (TextBox)di.FindControl("txtVendorName");
                    Label lblVendorName = (Label)di.FindControl("lblVendorName");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["vendor_id"] = Convert.ToInt32(grdVendor.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = 1;
                    dr["vendor_name"] = txtVendorName.Text;
                    dr["is_active"] = Convert.ToBoolean(chkIsActive.Checked);
                    dr["create_date"] = DateTime.Now;

                }

            }
            foreach (DataRow dr in table.Rows)
            {
                vendor_info VenInfo = new vendor_info();
                if (Convert.ToInt32(dr["vendor_id"]) > 0)
                    VenInfo = _db.vendor_infos.Single(l => l.vendor_id == Convert.ToInt32(dr["vendor_id"]));
                else
                    if (_db.vendor_infos.Where(l => l.client_id == 1 && l.vendor_name == dr["vendor_name"].ToString()).SingleOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Vendor name already exist. Please try another name.");
                        return;
                    }
                string str = dr["vendor_name"].ToString().Trim();
                if (str.Length > 0)
                {
                    

                    VenInfo.vendor_id = Convert.ToInt32(dr["vendor_id"]);
                    VenInfo.client_id = Convert.ToInt32(dr["client_id"]);
                    VenInfo.vendor_name = dr["vendor_name"].ToString();
                    VenInfo.is_active = Convert.ToBoolean(dr["is_active"]);
                    VenInfo.create_date = DateTime.Now;
                    

                }
                if(Convert.ToInt32(dr["vendor_id"])==0)
                {
                    _db.vendor_infos.InsertOnSubmit(VenInfo);
                }
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");            
            _db.SubmitChanges();
            LoadVendorInfo();
           

        }
    }
    
    protected void grdVendor_RowEditing(object sender, GridViewEditEventArgs e)
    {
        CheckBox chkIsActive = (CheckBox)grdVendor.Rows[e.NewEditIndex].FindControl("chkIsActive");
        TextBox txtVendorName = (TextBox)grdVendor.Rows[e.NewEditIndex].FindControl("txtVendorName");
        Label lblVendorName = (Label)grdVendor.Rows[e.NewEditIndex].FindControl("lblVendorName");
        
        txtVendorName.Visible = true;
        lblVendorName.Visible = false;
        chkIsActive.Enabled=true;
        LinkButton btn = (LinkButton)grdVendor.Rows[e.NewEditIndex].Cells[2].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdVendor_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBox chkIsActive = (CheckBox)grdVendor.Rows[e.RowIndex].FindControl("chkIsActive");
        TextBox txtVendorName = (TextBox)grdVendor.Rows[e.RowIndex].FindControl("txtVendorName");
        Label lblVendorName = (Label)grdVendor.Rows[e.RowIndex].FindControl("lblVendorName");

        int nVendorId = Convert.ToInt32(grdVendor.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE vendor_info SET vendor_name='" + txtVendorName.Text.Replace("'", "''") + "' , is_active='" + Convert.ToBoolean(chkIsActive.Checked) + "'  WHERE vendor_id=" + nVendorId + "  AND client_id=1";
        _db.ExecuteCommand(strQ, string.Empty);

        LoadVendorInfo();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
    }
    
    protected void grdVendor_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
           

            CheckBox chkIsActive = (CheckBox)e.Row.FindControl("chkIsActive");
            TextBox txtVendorName = (TextBox)e.Row.FindControl("txtVendorName");
            Label lblVendorName = (Label)e.Row.FindControl("lblVendorName");
            if(chkIsActive.Checked)
            {
                chkIsActive.Text="Yes";
            }
            else
            {
                chkIsActive.Text="No";
            }
            string str = txtVendorName.Text.Replace("&nbsp;", "");
            if (str == ""||Convert.ToInt32(grdVendor.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0])==0)
            {
                txtVendorName.Visible = true;
                lblVendorName.Visible = false;
               
                LinkButton btn = (LinkButton)e.Row.Cells[2].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }
            

        }

    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["vendor"];

            foreach (GridViewRow di in grdVendor.Rows)
            {
                {
                    CheckBox chkIsActive = (CheckBox)di.FindControl("chkIsActive");
                    TextBox txtVendorName = (TextBox)di.FindControl("txtVendorName");
                    Label lblVendorName = (Label)di.FindControl("lblVendorName");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["vendor_id"] = Convert.ToInt32(grdVendor.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = 1;
                    dr["vendor_name"] = txtVendorName.Text;
                    dr["is_active"] = Convert.ToBoolean(chkIsActive.Checked);
                    dr["create_date"] = DateTime.Now;


                }

            }

            DataRow drNew = table.NewRow();
            drNew["vendor_id"] = 0;
            drNew["client_id"] = 1;
            drNew["vendor_name"] = "";
            drNew["is_active"] = true;
            drNew["create_date"] = DateTime.Now;
            table.Rows.Add(drNew);

            Session.Add("vendor", table);
            grdVendor.DataSource = table;
            grdVendor.DataKeyNames = new string[] { "vendor_id" };
            grdVendor.DataBind();
            lblResult.Text = "";

    }
}

