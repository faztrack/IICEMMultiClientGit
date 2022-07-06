﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SiteProgress_graphics : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            lnkClose.Attributes.Add("onClick", "CloseWindow();");
            lnkPrint.Attributes.Add("onClick", "PrintWindow();");

            if (Request.QueryString.Get("jsid") != null)
            {
                int nCustomerId = 0;
                int nEstimateId = 0;
                if (Request.QueryString.Get("cid") != null)
                    nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                if (Request.QueryString.Get("eid") != null)
                    nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
                int nJobStatusId = Convert.ToInt32(Request.QueryString.Get("jsid"));
                hdnCustomerId.Value = nCustomerId.ToString();
                hdnEstimateId.Value = nEstimateId.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
              
                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == nCustomerId);

                lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1;

                var item = from jd in _db.job_status_descs
                           where jd.jobstatusid == nJobStatusId && jd.customer_id == nCustomerId && jd.estimate_id == nEstimateId
                           orderby jd.status_serial ascending
                           select jd;
                grdDescriptions.DataSource = item;
                grdDescriptions.DataBind();

                if (nJobStatusId == 1)
                    lblJobStatusFor.Text = "Design";
                else if (nJobStatusId == 2)
                    lblJobStatusFor.Text = "Selection";
                else if (nJobStatusId == 3)
                    lblJobStatusFor.Text = "Site Progress & Photos";
                else if (nJobStatusId == 4)
                    lblJobStatusFor.Text = "Schedule";
                else if (nJobStatusId == 5)
                    lblJobStatusFor.Text = "Final Project Review";
                else if (nJobStatusId == 6)
                    lblJobStatusFor.Text = "Completion Certificate";
                else if (nJobStatusId == 7)
                    lblJobStatusFor.Text = "Warranty";

               

                GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
                GetCustomerImageInfo(Convert.ToInt32(hdnCustomerId.Value));

                if (Request.QueryString.Get("type") != null)
                {
                    file_upload.Visible = false;
                    btnUpload.Visible = false;
                    grdCustomersFile.Columns[3].Visible = false;
                    grdCustomersFile.Columns[4].Visible = false;


                }


            }
        }


    }

    protected void grdDescriptions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nJobStatusid = Convert.ToInt32(e.Row.Cells[0].Text);
            if (nJobStatusid == 1)
                e.Row.Cells[0].Text = "Design";
            else if (nJobStatusid == 2)
                e.Row.Cells[0].Text = "Selection";
            else if (nJobStatusid == 3)
                e.Row.Cells[0].Text = "Site Progress & Photos";
            else if (nJobStatusid == 4)
                e.Row.Cells[0].Text = "Schedule";
            else if (nJobStatusid == 5)
                e.Row.Cells[0].Text = "Final Project Review";
            else if (nJobStatusid == 6)
                e.Row.Cells[0].Text = "Completion Certificate";
            else if (nJobStatusid == 7)
                e.Row.Cells[0].Text = "Warranty";
        }
    }

    protected void grdCustomersImage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {

                int upload_ImageId = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[0].ToString());
                string strImage = grdCustomersImage.DataKeys[e.Row.RowIndex].Values[2].ToString();
                int CustomerId = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string Desccription = grdCustomersImage.DataKeys[e.Row.RowIndex].Values[4].ToString();

                Label lblDescription = (Label)e.Row.FindControl("lblDescription");
                HyperLink hypImage = (HyperLink)e.Row.FindControl("hypImage");
                Image img = (Image)e.Row.FindControl("img");



                hypImage.NavigateUrl = "UploadedFiles/" + CustomerId + "/" + "SITE PROGRESS/" + strImage;
                hypImage.Attributes.Add("data-ilb2-caption", Desccription);
                hypImage.Attributes.Add("data-ilb3-gellarytitle", "Site Photos for: " + lblCustomerName.Text);
                img.ImageUrl = "UploadedFiles/" + CustomerId + "/SITE PROGRESS" + "/Thumbnail/" + strImage;

                if (Desccription != "" && Desccription.Length > 10)
                {
                    lblDescription.Text = Desccription.Substring(0, 10) + " <u style='color:#992a4f; font-weight:bold;'>...</u>";
                    lblDescription.ToolTip = Desccription;
                 
                }
                else
                {
                    lblDescription.Text = Desccription;                  
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    protected void grdCustomersImage_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int upload_ImageId = Convert.ToInt32(grdCustomersImage.DataKeys[e.RowIndex].Values[0].ToString());
        string strQ = "Delete file_upload_info WHERE upload_fileId=" + Convert.ToInt32(upload_ImageId) ;
        _db.ExecuteCommand(strQ, string.Empty);
        GetCustomerImageInfo(Convert.ToInt32(hdnCustomerId.Value));

    }

    protected void grdCustomersImage_RowEditing(object sender, GridViewEditEventArgs e)
    {
        TextBox txtDescription = (TextBox)grdCustomersImage.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersImage.Rows[e.NewEditIndex].FindControl("lblDescription");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        ImageButton imgEdit = (ImageButton)grdCustomersImage.Rows[e.NewEditIndex].FindControl("imgEdit");
        ImageButton imgDelete = (ImageButton)grdCustomersImage.Rows[e.NewEditIndex].FindControl("imgDelete");

        imgEdit.ImageUrl = "~/images/icon_save_16x16.png";
        imgEdit.CommandName = "Update";

        imgDelete.Visible = false;
    }

    protected void grdCustomersImage_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int upload_ImageId = Convert.ToInt32(grdCustomersImage.DataKeys[e.RowIndex].Values[0].ToString());
        TextBox txtDescription = (TextBox)grdCustomersImage.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersImage.Rows[e.RowIndex].FindControl("lblDescription");

        ImageButton imgEdit = (ImageButton)grdCustomersImage.Rows[e.RowIndex].FindControl("imgEdit");
        ImageButton imgDelete = (ImageButton)grdCustomersImage.Rows[e.RowIndex].FindControl("imgDelete");

        string StrQ = "UPDATE file_upload_info SET Desccription='" + txtDescription.Text.Replace("'", "''") + "' WHERE upload_fileId=" + Convert.ToInt32(upload_ImageId);
        _db.ExecuteCommand(StrQ, string.Empty);
        GetCustomerImageInfo(Convert.ToInt32(hdnCustomerId.Value));

        imgEdit.ImageUrl = "~/images/icon_edit_16x16.png";
        imgEdit.CommandName = "Edit";

        imgDelete.Visible = true;
    }    
   
    private void GetCustomerImageInfo(int nCustId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string[] aryImageType = new string[] { "png", "jpg" };

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                var itemImage = from f in _db.file_upload_infos
                               where f.CustomerId == nCustId
                               && f.IsSiteProgress == true
                            
                               && f.type != 1
                               && (f.ImageName.ToString().ToLower().Contains("jpg") || f.ImageName.ToLower().ToString().Contains("png") || f.ImageName.ToLower().ToString().Contains("jpeg"))
                               orderby f.upload_fileId ascending
                               select f;

              //  DataTable dt = csCommonUtility.LINQToDataTable(itemImage);

                if (itemImage.Count() > 0)
                {
                    lblImageGalleryTitle.Visible = true;
                }

                grdCustomersImage.DataSource = itemImage;
                grdCustomersImage.DataKeyNames = new string[] { "upload_fileId", "is_design", "ImageName", "CustomerId", "Desccription" };
                grdCustomersImage.DataBind();
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
            int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.Row.RowIndex].Values[0].ToString());
            string strFile = grdCustomersFile.DataKeys[e.Row.RowIndex].Values[2].ToString();
            HyperLink hypView = (HyperLink)e.Row.FindControl("hypView");
            string sFileName = strFile;
            if (strFile.IndexOf('_') != -1)
            {
                sFileName = strFile.Substring(0, strFile.IndexOf('_')).Trim() + Path.GetExtension(strFile); ;
            }

            hypView.Text = "View " + sFileName;
            hypView.NavigateUrl = "UploadedFiles/" + hdnCustomerId.Value + "/" + "SITE PROGRESS/" + strFile;
            hypView.Target = "_blank";
            e.Row.Cells[1].Text = sFileName;

        }
    }

    protected void grdCustomersFile_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        string strQ = "Delete file_upload_info WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) ;
        _db.ExecuteCommand(strQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));

    }

    protected void grdCustomersFile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.NewEditIndex].FindControl("lblDescription");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        LinkButton btn = (LinkButton)grdCustomersFile.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }

    protected void grdCustomersFile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.RowIndex].FindControl("lblDescription");
        string StrQ = "UPDATE file_upload_info SET Desccription='" + txtDescription.Text.Replace("'", "''") + "' WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) ;
        _db.ExecuteCommand(StrQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
    }

    private void GetCustomerFileInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var file = from file_info in _db.file_upload_infos
                       where file_info.CustomerId == nCustId && file_info.IsSiteProgress == true
                       && file_info.type != 1
                        && (!file_info.ImageName.ToString().ToLower().Contains("jpg") && !file_info.ImageName.ToLower().ToString().Contains("png") && !file_info.ImageName.ToLower().ToString().Contains("jpeg"))
                       orderby file_info.upload_fileId ascending
                       select file_info;

            if (file.Count() > 0)
            {
                lblDocumentsTitle.Visible = true;
            }

            grdCustomersFile.DataSource = file;
            grdCustomersFile.DataKeyNames = new string[] { "upload_fileId", "IsSiteProgress", "ImageName" };
            grdCustomersFile.DataBind();
        }

    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        lblMessage.Text = string.Empty;
        HttpFileCollection fileCollection = Request.Files;
        for (int i = 0; i < fileCollection.Count; i++)
        {
            string DestinationPath = Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value + "//SITE PROGRESS" + "//Test");

            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            HttpPostedFile uploadfile = fileCollection[i];
            string fileName = "";
            string fileExt = Path.GetExtension(uploadfile.FileName);
            string originalFileName = Path.GetFileNameWithoutExtension(uploadfile.FileName);
            if (uploadfile.ContentLength > 0)
            {
                fileName = originalFileName + "_" + DateTime.Now.Ticks.ToString() + fileExt;

                uploadfile.SaveAs(Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value + "//SITE PROGRESS" + "//Test//") + fileName);
                lblMessage.Text += csCommonUtility.GetSystemMessage(originalFileName + " uploaded successfully<br>");
            }
            BindTempGrid();
            if (grdTemp.Rows.Count > 0)
                btnSave.Visible = true;
            else
                btnSave.Visible = false;

        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    private void BindTempGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpSTable = LoadtmpTable();
        DataRow dr = tmpSTable.NewRow();

        string DestinationPath = Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value + "//SITE PROGRESS" + "//Test");
        string[] fileEntries = Directory.GetFiles(DestinationPath);

        foreach (string file in fileEntries)
        {
            string FileName = Path.GetFileName(file);

            DataRow drNew = tmpSTable.NewRow();
            drNew["file_name"] = FileName;
            drNew["file_description"] = "";
            tmpSTable.Rows.Add(drNew);
        }
        grdTemp.DataSource = tmpSTable;
        grdTemp.DataKeyNames = new string[] { "file_name" };
        grdTemp.DataBind();

    }

    private DataTable LoadtmpTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("file_name", typeof(string));
        table.Columns.Add("file_description", typeof(string));

        return table;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow di in grdTemp.Rows)
        {
            HyperLink hyp = (HyperLink)di.FindControl("hyp");
            TextBox txtDes = (TextBox)di.FindControl("txtDes");
            string sfileName = grdTemp.DataKeys[di.RowIndex].Value.ToString();
            sfileName = sfileName.Replace("amp;", "").Trim();
            file_upload_info fui = new file_upload_info();
            if (_db.file_upload_infos.Where(l => l.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && l.ImageName == sfileName).SingleOrDefault() == null)
            {
                fui.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                fui.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
                fui.Desccription = txtDes.Text;
                fui.ImageName = sfileName;
                fui.is_design = false;
                fui.estimate_id = 0;
                fui.type = 0;
                fui.vendor_cost_id = 0;
                fui.IsSiteProgress = true;
                _db.file_upload_infos.InsertOnSubmit(fui);
                _db.SubmitChanges();
            }

        }

        string strpath = Request.PhysicalApplicationPath + "UploadedFiles\\";
        strpath = strpath + "\\" + hdnCustomerId.Value.ToString() + "\\SITE PROGRESS" + "\\Test";
        string NewDir = Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value + "//SITE PROGRESS");
        if (!System.IO.Directory.Exists(NewDir))
        {
            System.IO.Directory.CreateDirectory(NewDir);
        }
        string[] fileEntries = Directory.GetFiles(strpath);
        foreach (string file in fileEntries)
        {

            string FileName = Path.GetFileName(file);
            string Ext = Path.GetExtension(file).ToLower();
            if (Ext == ".jpg" || Ext == ".png" || Ext == ".jpeg")
            {
                ImageUtility.SaveSlideImage(file, Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value + "//SITE PROGRESS"));
                ImageUtility.SaveThumbnailImage(file, Server.MapPath("~/UploadedFiles//" + hdnCustomerId.Value) + "\\SITE PROGRESS" + "\\Thumbnail");

                if (!System.IO.Directory.Exists(NewDir + "\\Original"))
                {
                    System.IO.Directory.CreateDirectory(NewDir + "\\Original");
                }
                File.Move(file, Path.Combine(NewDir + "\\Original", FileName));
            }
            else
                File.Move(file, Path.Combine(NewDir, FileName));
        }

        lblMessage.Text = csCommonUtility.GetSystemMessage("File for Design saved successfully");
        BindTempGrid();
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
        GetCustomerImageInfo(Convert.ToInt32(hdnCustomerId.Value));
        if (grdTemp.Rows.Count > 0)
            btnSave.Visible = true;
        else
            btnSave.Visible = false;


    }

    protected void grdTemp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string file = grdTemp.DataKeys[e.Row.RowIndex].Value.ToString();
            file = file.Replace("amp;", "").Trim();
            HyperLink hypView = (HyperLink)e.Row.FindControl("hyp");
            string strFileName = file;
            if (file.IndexOf('_') != -1)
            {
                strFileName = file.Substring(0, file.IndexOf('_')).Trim() + Path.GetExtension(file);
            }
            hypView.Text = strFileName;
            hypView.NavigateUrl = "UploadedFiles/" + hdnCustomerId.Value + "/SITE PROGRESS" + "/" + file;
            hypView.Target = "_blank";

        }
    }
}