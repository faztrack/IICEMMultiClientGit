
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class UploadedDocs : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin036") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;


                int nSalesPersonId = Convert.ToInt32(cust.sales_person_id);

                if (_db.sales_persons.Any(c => c.sales_person_id == nSalesPersonId && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(nSalesPersonId));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }
            
                GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
            }
        }
       
    }


    protected void grdCustomersFile_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        string strQ = "Delete file_upload_info WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));

    }
    protected void grdCustomersFile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.NewEditIndex].FindControl("lblDescription");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        ImageButton imgEdit = (ImageButton)grdCustomersFile.Rows[e.NewEditIndex].FindControl("imgEdit");
        ImageButton imgDelete = (ImageButton)grdCustomersFile.Rows[e.NewEditIndex].FindControl("imgDelete"); 

        imgEdit.ImageUrl = "~/images/icon_save.png"; 
        imgEdit.CommandName = "Update";

        imgDelete.Visible = false;
    }
    protected void grdCustomersFile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.RowIndex].FindControl("lblDescription");

        ImageButton imgEdit = (ImageButton)grdCustomersFile.Rows[e.RowIndex].FindControl("imgEdit");
        ImageButton imgDelete = (ImageButton)grdCustomersFile.Rows[e.RowIndex].FindControl("imgDelete"); 

        string StrQ = "UPDATE file_upload_info SET Desccription='" + txtDescription.Text.Replace("'", "''") + "' WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(StrQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));

        imgEdit.ImageUrl = "~/images/icon_edit.png";
        imgEdit.CommandName = "Edit";

        imgDelete.Visible = true;
    }
   

    protected void imgDelete_Click(object sender, EventArgs e)
    {
        
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            ImageButton btn = (ImageButton)sender;
            int upload_fileId = Convert.ToInt32(btn.CommandArgument);
            string strQ = "Delete file_upload_info WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ, string.Empty);
            GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));           
        }
        catch (Exception ex)
        {
            Session.Add("sAddCartError", "AddCartError, " + ex.Message);
        }
    }
    private void GetCustomerFileInfo(int nCustId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string[] aryFileType = new string[] { "png", "jpg" };

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                var itemfile = from f in _db.file_upload_infos
                               where f.CustomerId == nCustId
                                   //&& file_info.is_design == true
                               && f.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                   //&& file_info.type != 1

                               && (f.ImageName.ToString().Contains("jpg") || f.ImageName.ToString().Contains("png"))
                               orderby f.upload_fileId ascending
                               select f;

                DataTable dt = csCommonUtility.LINQToDataTable(itemfile);



                grdCustomersFile.DataSource = itemfile;
                grdCustomersFile.DataKeyNames = new string[] { "upload_fileId", "is_design", "ImageName", "CustomerId" };
                grdCustomersFile.DataBind();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void grdCustomersFile_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {

                int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.Row.RowIndex].Values[0].ToString());
                string strFile = grdCustomersFile.DataKeys[e.Row.RowIndex].Values[2].ToString();
                int CustomerId = Convert.ToInt32(grdCustomersFile.DataKeys[e.Row.RowIndex].Values[3].ToString());

                HyperLink hypImage = (HyperLink)e.Row.FindControl("hypImage");
                Image img = (Image)e.Row.FindControl("img");
              
               

                hypImage.NavigateUrl = "Document/" + CustomerId + "/" + strFile;
                img.ImageUrl = "Document/" + CustomerId + "/" + strFile;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}