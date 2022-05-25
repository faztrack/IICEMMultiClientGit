using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Web.UI.HtmlControls;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;

public partial class CustomerSelectionInfoSec : System.Web.UI.Page
{
    public DataTable dtLocation;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oCustomerUser"] == null)
            {
                Response.Redirect("customerlogin.aspx");
            }
            btnSubmitSelection.Attributes.Add("onClick", "return confirmOperation();");
            customer objCust = new customer();
            string strCustName = "";

            DataClassesDataContext _db = new DataClassesDataContext();
            customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
            int nCustomerId = Convert.ToInt32(obj.customerid);

            hdnCustomerID.Value = nCustomerId.ToString();

            if (_db.customers.Where(c => c.customer_id == nCustomerId).Count() > 0)
            {
                objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                strCustName = objCust.first_name1 + " " + objCust.last_name1;
                hdnClientId.Value = objCust.client_id.ToString();
            }

            string strQ = "select * from customer_estimate where customer_id=" + nCustomerId + " and client_id= "+ hdnClientId.Value +" and status_id = 3 order by estimate_id desc ";
            IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);
            grdEstimates.DataSource = clist;
            grdEstimates.DataKeyNames = new string[] { "customer_id", "estimate_id", "status_id", "sale_date", "estimate_name" };
            grdEstimates.DataBind();
            if (_db.customer_estimates.Where(ce => ce.customer_id == Convert.ToInt32(hdnCustomerID.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.status_id == 3 && ce.IsEstimateActive == true).ToList().Count > 0)
            {
                int nEstId = 0;
                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == Convert.ToInt32(hdnCustomerID.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.status_id == 3 && ce.IsEstimateActive == true
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();

                hdnEstimateID.Value = nEstId.ToString();

            }
            

            lblHeaderTitle.Text = "Selection (" + strCustName + ")";

            LoadSignature();
            GetSectionSelectionListData();

        }


    }

    protected void grdEstimates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int neid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nsid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[2].ToString());
            string strEstimateName = grdEstimates.DataKeys[e.Row.RowIndex].Values[4].ToString();

            if (nsid == 3)
            {
                e.Row.Cells[1].Text = "Sold on " + Convert.ToDateTime(grdEstimates.DataKeys[e.Row.RowIndex].Values[3].ToString()).ToShortDateString();

                LinkButton lnkSectionSelection = (LinkButton)e.Row.FindControl("lnkSectionSelection");
                lnkSectionSelection.Visible = true;
                lnkSectionSelection.CommandArgument = neid.ToString() + "-" + strEstimateName.ToString();
            }
        }


    }

    protected void lnkSectionSelection_Onclick(object sender, EventArgs e)
    {
        LinkButton lnkSectionSelection = (LinkButton)sender;

        string[] aryEstimate = lnkSectionSelection.CommandArgument.ToString().Split('-');

        int neid = Convert.ToInt32(aryEstimate[0]);
        string strEstimateName = aryEstimate[1].ToString();

        hdnEstimateID.Value = neid.ToString();

        lblProjectName.Visible = true;
        lblProjectName.Text = "Project: " + strEstimateName;
        GetSectionSelectionListData();

    }


    private void GetSectionSelectionListData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int nCustId = Convert.ToInt32(hdnCustomerID.Value);
        int nEstId = Convert.ToInt32(hdnEstimateID.Value);

        var sslist = _db.Section_Selections.Where(ss => ss.customer_id == nCustId && ss.estimate_id == nEstId && ss.isDeclined == false).ToList().OrderByDescending(c => c.CreateDate);

        grdSelection.DataSource = sslist;
        grdSelection.DataKeyNames = new string[] { "SectionSelectionID", "customer_id", "section_id", "estimate_id", "location_id", "isSelected", "customer_signature", "customer_siignatureDate", "CreateDate", "ValidTillDate", "customer_signedBy" };
        grdSelection.DataBind();
    }

    protected void grdSelection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int section_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[2].ToString());
            int estimate_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int location_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[4].ToString());
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[e.Row.RowIndex].Values[5]);
            string strCustomerSignature = grdSelection.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string strSignatureDate = Convert.ToDateTime(grdSelection.DataKeys[e.Row.RowIndex].Values[7]).ToString("MM/dd/yyyy hh:mm tt");
            DateTime dtCreateDate = Convert.ToDateTime(grdSelection.DataKeys[e.Row.RowIndex].Values[8]);
            DateTime dtValidTill = Convert.ToDateTime(grdSelection.DataKeys[e.Row.RowIndex].Values[9]);
            string SignatureBy = grdSelection.DataKeys[e.Row.RowIndex].Values[10].ToString();

            TimeSpan ts = dtValidTill - dtCreateDate;
            int nDay = ts.Days;
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");

            CheckBox chkSelected = e.Row.FindControl("chkSelected") as CheckBox;
            Label lblSelected = e.Row.FindControl("lblSelected") as Label;
            Image imgCustomerSign = e.Row.FindControl("imgCustomerSign") as Image;
            Label lblSignatureBy = (Label)e.Row.FindControl("lblSignatureBy");
            Label lblSelectionDate = (Label)e.Row.FindControl("lblSelectionDate");
            Label lblDayLeft = e.Row.FindControl("lblDayLeft") as Label;



            lblDayLeft.Text = nDay.ToString() + " Day(s)";

            string str = lblNotes.Text.Replace("&nbsp;", "");

            if (str != "" && str.Length > 60)
            {

                lblNotes.Text = str.Substring(0, 60) + "...";
                lblNotes.ToolTip = str;
                lnkOpen.Visible = true;

            }
            else
            {

                lblNotes.Text = str;
                lblNotes.ToolTip = str;
                lnkOpen.Visible = false;

            }

            chkSelected.Checked = isSelected;


            if (nSectionSelectionID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nSectionSelectionID);
            }

            if (isSelected)
            {
                lblSignatureBy.Visible = true;
                lblDayLeft.Visible = false;
                lblSelected.Visible = true;
                chkSelected.Visible = false;
                lblSelected.Text = "Selected";
                lblSelectionDate.Text = "Date: " + strSignatureDate;
            }
            else
            {
                lblDayLeft.Visible = true;

                lblSelected.Visible = false;
                customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == customer_id);

                if (objCust.customer_signature == null || objCust.customer_signature == "")
                {
                    chkSelected.Visible = false;
                }
                else
                {
                    chkSelected.Visible = true;
                }
                lblSelected.Text = "";
            }
            if (nDay < 0)
            {
                //chkSelected.Checked = false;
                chkSelected.Visible = false;

            }

            if (strCustomerSignature.Length > 0 && strCustomerSignature != null)
            {
                string imgSign = string.Format("data:image/jpeg;base64,{0}", strCustomerSignature);
                if (imgSign != "data:image/jpeg;base64,")
                {
                    imgCustomerSign.ImageUrl = imgSign;
                    imgCustomerSign.Visible = true;
                }
                else
                {
                    imgCustomerSign.Visible = false;
                }
            }
            else
            {
                imgCustomerSign.Visible = false;
                lblSignatureBy.Text ="By "+ SignatureBy;
            }
        }

    }



    private void GetUploadedFileListData(GridView grd, int nSectionSelectionID)
    {
        DataClassesDataContext _db = new DataClassesDataContext();



        var objfui = from fui in _db.file_upload_infos
                     where fui.vendor_cost_id == nSectionSelectionID && fui.type == 5
                     orderby fui.upload_fileId ascending
                     select fui;
        if (objfui != null)
        {
            grd.DataSource = objfui;
            grd.DataKeyNames = new string[] { "upload_fileId", "vendor_cost_id", "estimate_id", "ImageName" };
            grd.DataBind();
        }
    }

    protected void grdUploadedFileList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdUploadedFileList = (GridView)sender;
         
            GridViewRow grdSelectionRow = (GridViewRow)(grdUploadedFileList.NamingContainer);
            int Index = grdSelectionRow.RowIndex;

            GridView grdSelection = (GridView)(grdSelectionRow.Parent.Parent);
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);



            int nUploadFileId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            int customer_id = Convert.ToInt32(hdnCustomerID.Value);


            string file_name = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString();


            string fileName = file_name.Substring(0, 10);
            Label lblFileName = (Label)e.Row.FindControl("lblFileName");
            lblFileName.Text = fileName;




            if (file_name.Contains(".jpg") || file_name.Contains(".jpeg") || file_name.Contains(".png") || file_name.Contains(".gif"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = true;

                if (isSelected)
                    hypImg.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypImg.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypImg.Target = "_blank";

                Image img = (Image)e.Row.FindControl("img");

                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = false;
                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = false;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = false;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = false;

                img.ImageUrl = "File/" + customer_id + "/SELECTIONS/Thumbnail/" + file_name;
                //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);

            }


            if (file_name.Contains(".pdf"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = false;
                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = true;

                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = false;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = false;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = false;

                Image imgPDF = (Image)e.Row.FindControl("imgPDF");
                imgPDF.ImageUrl = "~/images/icon_pdf.png";
                if (isSelected)
                    hypPDF.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypPDF.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypPDF.Target = "_blank";

            }
            if (file_name.Contains(".doc") || file_name.Contains(".docx"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = false;

                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = false;
                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = false;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = true;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = false;

                Image imgDoc = (Image)e.Row.FindControl("imgDoc");
                imgDoc.ImageUrl = "~/images/icon_docs.png";
                if (isSelected)
                    hypDoc.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypDoc.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypDoc.Target = "_blank";


            }
            if (file_name.Contains(".xls") || file_name.Contains(".xlsx") || file_name.Contains(".csv"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = false;
                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = false;
                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = true;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = false;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = false;

                Image imgExcel = (Image)e.Row.FindControl("imgExcel");
                imgExcel.ImageUrl = "~/images/icon_excel.png";
                if (isSelected)
                    hypExcel.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypExcel.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypExcel.Target = "_blank";

            }
            if (file_name.Contains(".txt") || file_name.Contains(".TXT"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = false;
                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = false;
                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = false;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = false;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = true;

                Image imgTXT = (Image)e.Row.FindControl("imgTXT");
                imgTXT.ImageUrl = "~/images/icon_txt.png";
                if (isSelected)
                    hypTXT.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypTXT.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypTXT.Target = "_blank";

            }
        }
    }


    protected void btnClearSignature_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClearSignature.ID, btnClearSignature.GetType().Name, "Click"); 
        try
        {
            int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);

            if (btnClearSignature.Text.Equals("Clear"))
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                btnClearSignature.Text = "Clear";
                btnSaveSignature.Visible = true;
                divSig1.Visible = true;
                imgCustomerSign.Visible = false;

                customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);
                if (objCust != null)
                {
                    // objCust.customer_signature = "";

                    //  _db.SubmitChanges();
                }
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            // lblResult.Text = csCommonUtility.GetSystemErrorMessage("Data deleted successfully");
        }
        catch (Exception ex)
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }



    }

    protected void btnSaveSignature_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveSignature.ID, btnSaveSignature.GetType().Name, "Click"); 

        try
        {
            string signatureCustomer = hdnSignCustomer.Value;
            int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);

            if (signatureCustomer != "")
            {

                string dataURI = hdnSignCustomer.Value;

                string[] split_1 = dataURI.Split(';');
                string split_2 = split_1[1].Replace("base64,", "");

                hdnSignCustomer.Value = "";
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);
                if (objCust != null)
                {
                    objCust.customer_signature = split_2;

                    _db.SubmitChanges();
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

                lblResult.Text = csCommonUtility.GetSystemMessage("Signature has been saved successfully.");
                LoadSignature();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Customer Signature.");
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerdashboard.aspx");
    }

    private void LoadSignature()
    {
        try
        {
            int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
            DataClassesDataContext _db = new DataClassesDataContext();
            customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);
            if (objCust != null)
            {
                string imgSign = string.Format("data:image/jpeg;base64,{0}", objCust.customer_signature);
                if (imgSign != "data:image/jpeg;base64,")
                {
                    imgCustomerSign.ImageUrl = imgSign;

                    divSig1.Visible = false;
                    btnSaveSignature.Visible = false;
                    imgCustomerSign.Visible = true;

                    btnClearSignature.Text = "Clear";
                    btnClearSignature.Visible = true;//change
                }
                else
                {
                    divSig1.Visible = true;
                    btnSaveSignature.Visible = true;
                    imgCustomerSign.Visible = false;

                    btnClearSignature.Text = "Clear";
                    btnClearSignature.Visible = true;
                }


            }
            else
            {
                imgCustomerSign.Visible = false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[4].Controls[0].FindControl("lblNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[4].Controls[1].FindControl("lblNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[4].Controls[3].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblCallDescriptionG.Visible = false;
            lblCallDescriptionG_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblCallDescriptionG.Visible = true;
            lblCallDescriptionG_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

    //protected void btnHdnConfirmSelection_click(object sender, EventArgs e)
    //{
    //    lblResult.Text = "";
    //    try
    //    {
    //        int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        Button chkSelected = (Button)sender;
    //        customeruserinfo objCustInfo = (customeruserinfo)Session["oCustomerUser"];
    //        GridViewRow grdSelectionRow = (GridViewRow)((Button)sender).NamingContainer;
    //        int Index = grdSelectionRow.RowIndex;


    //        int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[Index].Values[0].ToString());

    //        customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);
    //        string imgSign = string.Format("data:image/jpeg;base64,{0}", objCust.customer_signature);

    //        if (imgSign != "data:image/jpeg;base64,")
    //        {
    //            Section_Selection objSS = _db.Section_Selections.SingleOrDefault(s => s.SectionSelectionID == nSectionSelectionID && s.customer_id == Convert.ToInt32(hdnCustomerID.Value) && s.estimate_id == Convert.ToInt32(hdnEstimateID.Value));
    //            if (objSS != null)
    //            {
    //                objSS.customer_signature = objCust.customer_signature;
    //                objSS.customer_siignatureDate = DateTime.Now;
    //                objSS.customer_signedBy = objCustInfo.customerusername;
    //                objSS.isSelected = true;

    //                _db.SubmitChanges();
    //            }
    //            lblReult2.Text = csCommonUtility.GetSystemErrorMessage("Data save successfully");
    //            GetSectionSelectionListData();
    //        }
    //        else
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Your Signature.");
    //            lblReult2.Text = csCommonUtility.GetSystemErrorMessage("Missing Your Signature.");
    //        }
    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    //    }
    //    catch (Exception ex)
    //    {

    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    //        lblReult2.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    protected void btnSubmitSelection_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmitSelection.ID, btnSubmitSelection.GetType().Name, "Click"); 
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowProgress", "ShowProgress();", true);
        int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
        DataClassesDataContext _db = new DataClassesDataContext();
        
        customeruserinfo objCustInfo = (customeruserinfo)Session["oCustomerUser"];
        customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);
        string imgSign = string.Format("data:image/jpeg;base64,{0}", objCust.customer_signature);
        if (imgSign != "data:image/jpeg;base64,")
        {
            bool bFlag = false;
            foreach (GridViewRow di in grdSelection.Rows)
            {
                int index = Convert.ToInt32(di.RowIndex);
                int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[index].Values[0].ToString());
                CheckBox chkSelected = (CheckBox)di.FindControl("chkSelected");
                bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[index].Values[5]);

                if (!isSelected)
                {
                    if (chkSelected.Checked)
                    {
                        bFlag = true;
                        Section_Selection objSS = _db.Section_Selections.SingleOrDefault(s => s.SectionSelectionID == nSectionSelectionID && s.customer_id == Convert.ToInt32(hdnCustomerID.Value) && s.estimate_id == Convert.ToInt32(hdnEstimateID.Value));
                        if (objSS != null)
                        {
                            objSS.customer_signature = objCust.customer_signature;
                            objSS.customer_siignatureDate = DateTime.Now;
                            objSS.customer_signedBy = objCustInfo.customerusername;
                            objSS.isSelected = true;

                            _db.SubmitChanges();


                           
                            try
                            {
                                string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerID.Value + "//SELECTIONS//";
                                if (!System.IO.Directory.Exists(sFolderPath))
                                {
                                    System.IO.Directory.CreateDirectory(sFolderPath);
                                }
                                List<file_upload_info> fList = (from f in _db.file_upload_infos where f.vendor_cost_id == nSectionSelectionID && f.type == 5 && f.estimate_id == objSS.estimate_id && f.CustomerId == objSS.customer_id select f).ToList();
                                foreach (var li in fList)
                                {
                                    string path = Server.MapPath("File\\") + hdnCustomerID.Value + "\\SELECTIONS" + @"\" + li.ImageName;
                                    //sFileName = Path.GetFileName(path);
                                    File.Move(path, sFolderPath + li.ImageName);
                                }
                            }
                           catch
                            {
                            }
                        }

                    }
                }
            }
            if (!bFlag)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");
                lblReult2.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");

            }
            else
            {
                lblReult2.Text = csCommonUtility.GetSystemMessage("Data save successfully");
                GetSectionSelectionListData();
                SendApproveEmail(Convert.ToInt32(hdnCustomerID.Value));
            }

        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Your Signature.");
            lblReult2.Text = csCommonUtility.GetSystemErrorMessage("Missing Your Signature.");
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }

    protected void btnDeclineSelection_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDeclineSelection.ID, btnDeclineSelection.GetType().Name, "Click"); 
        int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
        DataClassesDataContext _db = new DataClassesDataContext();


        bool bFlag = false;
        foreach (GridViewRow di in grdSelection.Rows)
        {
            int index = Convert.ToInt32(di.RowIndex);
            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[index].Values[0].ToString());
            CheckBox chkSelected = (CheckBox)di.FindControl("chkSelected");
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[index].Values[5]);

            if (!isSelected)
            {
                if (chkSelected.Checked)
                {
                    bFlag = true;
                    Section_Selection objSS = _db.Section_Selections.SingleOrDefault(s => s.SectionSelectionID == nSectionSelectionID && s.customer_id == Convert.ToInt32(hdnCustomerID.Value) && s.estimate_id == Convert.ToInt32(hdnEstimateID.Value));
                    if (objSS != null)
                    {
                        objSS.isDeclined = true;
                        objSS.DeclinedDate = DateTime.Now;
                        _db.SubmitChanges();
                    }

                }
            }
        }
        if (!bFlag)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");
            lblReult2.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");

        }
        else
        {
            lblReult2.Text = csCommonUtility.GetSystemMessage("Data save successfully");
            GetSectionSelectionListData();
            SendDeclinedEmail(Convert.ToInt32(hdnCustomerID.Value));
        }


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }

    private void SendApproveEmail(int nCustomerId)
    {
        // Email To Group

        string arySelectionEmail = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = new customer();
        objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
        customeruserinfo objcu = new customeruserinfo();
        objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

        company_profile com = new company_profile();
        if (_db.company_profiles.Where(cp => cp.client_id == 1).SingleOrDefault() != null)
        {
            com = _db.company_profiles.Single(cp => cp.client_id == 1);

            arySelectionEmail = com.SelectionEmail ?? "";
        }

        string strToEmail = string.Empty;
        string strSaleEmail = string.Empty;
        string strSuperEmail = string.Empty;

        sales_person sap = new sales_person();
        sap = _db.sales_persons.SingleOrDefault(c => c.sales_person_id == Convert.ToInt32(objCust.sales_person_id) && c.is_active==true);
        if (sap != null)
        {
            strSaleEmail = sap.email;
        }

        user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId && u.is_active==true);
        if (uinfo != null)
        {
            strSuperEmail = uinfo.email;
        }

        ArrayList arr = new ArrayList();
        if (strSaleEmail.Length > 0)
        {
            if (!arr.Contains(strSaleEmail))
            {
                arr.Add(strSaleEmail);
                strToEmail = strSaleEmail;
            }
        }
        if (strSuperEmail.Length > 0)
        {
            if (!arr.Contains(strSuperEmail))
            {
                arr.Add(strSuperEmail);
                if (strToEmail.Length == 0)
                {
                    strToEmail = strSuperEmail;
                }
                else
                {
                    strToEmail += "," + strSuperEmail;
                }
            }
        }


        string strUserEmail = "SELECT DISTINCT UserEmail FROM Section_Selection  WHERE  customer_id = " + nCustomerId + " AND estimate_id = " + Convert.ToInt32(hdnEstimateID.Value);
        DataTable dtUserEmail = csCommonUtility.GetDataTable(strUserEmail);
        foreach (DataRow drEmail in dtUserEmail.Rows)
        {
            string sEmail = drEmail["UserEmail"].ToString();
            if (sEmail.Length > 0)
            {
                if (!arr.Contains(sEmail))
                {
                    arr.Add(sEmail);
                    if (strToEmail.Length == 0)
                    {
                        strToEmail = sEmail;
                    }
                    else
                    {
                        strToEmail += "," + sEmail;
                    }
                }
            }

        }

        // Selection Email from Company Profile
        //--------------------------------------------------- arefin 07-17-2019
        string[] aryToEmail = strToEmail.Split(',');
        string strSelectionEmail = "";
        foreach (string email in aryToEmail)
        {

            if (!arySelectionEmail.Contains(email))
            {
                strSelectionEmail += email + ",";

            }
        }

        if (strSelectionEmail.Length > 3)
        {
            if (arySelectionEmail.Length > 3)
                arySelectionEmail += ", " + strSelectionEmail.TrimEnd(',');
            else
                arySelectionEmail = strSelectionEmail.TrimEnd(',');
        }

        strToEmail = arySelectionEmail.TrimEnd(',');
        //----------------------------------------------------------

        string strTable = CreateHtml();


        string strCCEmail = string.Empty;
        string strBCCEmail = "faztrackbd@gmail.com";
        if (strToEmail.Length > 4)
        {
            #region Sendemail by out look
            try
            {
                string sMessageUniqueId = "";

                ExchangeService service = EWSAPI.GetEWSService("alyons@azinteriorinnovations.com", "Innovation5");

                EmailMessage message = new EmailMessage(service);
                message.Subject = "Customer " + objCust.last_name1 + " approved selection(s)";
                message.Body = strTable;
                string strTest = "";
                if (strToEmail.Length > 4)
                {
                    string[] strIds = strToEmail.Split(',');
                    foreach (string strId in strIds)
                    {
                       // strTest += strId.Trim()+", ";
                        message.ToRecipients.Add(strId.Trim());
                    }
                }

                if (strCCEmail.Length > 4)
                {
                    string[] strCCIds = strCCEmail.Split(',');
                    foreach (string strCCId in strCCIds)
                    {
                        message.CcRecipients.Add(strCCId.Trim());
                    }
                }

                if (strBCCEmail.Length > 4)
                {
                    string[] strBCCIds = strBCCEmail.Split(',');
                    foreach (string strBCCId in strBCCIds)
                    {
                        message.BccRecipients.Add(strBCCId.Trim());
                    }
                }

                // Create a custom extended property and add it to the message.

                Guid PropertySetId = Guid.NewGuid();
                ExtendedPropertyDefinition fazExtendedPropertyDefinition = new ExtendedPropertyDefinition(PropertySetId, "FaztrackPropertyName", MapiPropertyType.String);
                message.SetExtendedProperty(fazExtendedPropertyDefinition, "FaztrackPropertyName");

                message.SendAndSaveCopy();

                System.Threading.Thread.Sleep(1000);

                // sMessageUniqueId = EWSAPI.GetEmailId(fazExtendedPropertyDefinition, service);



            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

            }
            #endregion
        }

    }

    string CreateHtml()
    {

        string strQ = "SELECT * FROM Section_Selection WHERE isSelected = 1 AND customer_id =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value) + "order by customer_siignatureDate DESC";
        DataTable dtFinal = csCommonUtility.GetDataTable(strQ);

        DataView dvFinal = dtFinal.DefaultView;
        //dvFinal.Sort = "ProjectNoteId,ProjectDate";
        string strFileHTML = "";
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='left'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + lblHeaderTitle.Text + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='10%'>Approve Date</td><td width='10%'>Date</td><td width='10%'>Section</td><td width='10%'>Location</td><td width='15%'>Title</td><td width='15%'>Notes</td><td width='10%'>Price</td><td width='20%'></td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string str = string.Empty;
            if (Convert.ToBoolean(dr["isSelected"]) == true)
            {
              
                string strF = "SELECT * FROM file_upload_info WHERE vendor_cost_id = " + Convert.ToInt32(dr["SectionSelectionID"]) + " AND type = 5 AND  CustomerId =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value);
                DataTable dtFile = csCommonUtility.GetDataTable(strF);
                if (dtFile.Rows.Count > 0)
                {
                    strFileHTML = "";
                    strFileHTML = "<table> <tr> ";
                    foreach (DataRow drf in dtFile.Rows)
                    {
                      
                        string imgUrl = "https://ii.faztrack.com/";
                        //string imgUrl = "" + csCommonUtility.GetProjectUrl() + "/";
                        string strFileName = drf["ImageName"].ToString().Replace(" ", "%20");

                        if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                        {
                            imgUrl += "File/" + hdnCustomerID.Value + "/SELECTIONS/Thumbnail/" + strFileName;
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";
                        }
                        else if (strFileName.Contains(".pdf"))
                        {
                            imgUrl += "images/icon_pdf.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";
                        }
                        else if (strFileName.Contains(".doc") || strFileName.Contains(".docx"))
                        {
                            imgUrl += "images/icon_docs.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";

                        }
                        else if (strFileName.Contains(".xls") || strFileName.Contains(".xlsx") || strFileName.Contains(".csv"))
                        {
                            imgUrl += "images/icon_excel.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";

                        }
                        else if (strFileName.Contains(".txt") || strFileName.Contains(".TXT"))
                        {
                            imgUrl += "images/icon_txt.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";
                        }
                        else if (strFileName.Contains(".plan") || strFileName.Contains(".PLAN"))
                        {
                            imgUrl += "images/icon_plan.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'/></td>";
                        }
                    
                }
                    strFileHTML += "</tr> </table>";

                }
                else
                {
                    strFileHTML = "";
                }

                string strTemp = strFileHTML;

                string strColor = "";

                if (i % 2 == 0)
                    strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
                else
                    strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
                if (dr["section_name"].ToString().Length > 0)
                {
                    strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["customer_siignatureDate"]).ToShortDateString() + "</td><td>" + Convert.ToDateTime(dr["CreateDate"]).ToShortDateString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["Title"].ToString() + "</td><td>" + dr["Notes"].ToString() + "</td><td>" + Convert.ToDecimal(dr["Price"]).ToString("c") + "</td><td>" + strFileHTML + "</td></tr>";
                }
                //}
            }

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }

    private void SendDeclinedEmail(int nCustomerId)
    {
        // Email To Group
        string arySelectionEmail = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = new customer();
        objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
        customeruserinfo objcu = new customeruserinfo();
        objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

        company_profile com = new company_profile();
        if (_db.company_profiles.Where(cp => cp.client_id == 1).SingleOrDefault() != null)
        {
            com = _db.company_profiles.Single(cp => cp.client_id == 1);

            arySelectionEmail = com.SelectionEmail ?? "";
        }

        string strToEmail = string.Empty;
        string strSaleEmail = string.Empty;
        string strSuperEmail = string.Empty;

        sales_person sap = new sales_person();
        sap = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(objCust.sales_person_id));
        if (sap != null)
        {
            strSaleEmail = sap.email;
        }

        user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId);
        if (uinfo != null)
        {
            strSuperEmail = uinfo.email;
        }

        ArrayList arr = new ArrayList();
        if (strSaleEmail.Length > 0)
        {
            if (!arr.Contains(strSaleEmail))
            {
                arr.Add(strSaleEmail);
                strToEmail = strSaleEmail;
            }
        }
        if (strSuperEmail.Length > 0)
        {
            if (!arr.Contains(strSuperEmail))
            {
                arr.Add(strSuperEmail);
                if (strToEmail.Length == 0)
                {
                    strToEmail = strSuperEmail;
                }
                else
                {
                    strToEmail += "," + strSuperEmail;
                }
            }
        }


        string strUserEmail = "SELECT DISTINCT UserEmail FROM Section_Selection  WHERE  customer_id = " + nCustomerId + " AND estimate_id = " + Convert.ToInt32(hdnEstimateID.Value);
        DataTable dtUserEmail = csCommonUtility.GetDataTable(strUserEmail);
        foreach (DataRow drEmail in dtUserEmail.Rows)
        {
            string sEmail = drEmail["UserEmail"].ToString();
            if (sEmail.Length > 0)
            {
                if (!arr.Contains(sEmail))
                {
                    arr.Add(sEmail);
                    if (strToEmail.Length == 0)
                    {
                        strToEmail = sEmail;
                    }
                    else
                    {
                        strToEmail += "," + sEmail;
                    }
                }
            }

        }

        // Selection Email from Company Profile
        //--------------------------------------------------- arefin 07-17-2019
        string[] aryToEmail = strToEmail.Split(',');
        string strSelectionEmail = "";
        foreach (string email in aryToEmail)
        {

            if (!arySelectionEmail.Contains(email))
            {
                strSelectionEmail += email + ",";

            }
        }

        if (strSelectionEmail.Length > 3)
        {
            if (arySelectionEmail.Length > 3)
                arySelectionEmail += ", " + strSelectionEmail.TrimEnd(',');
            else
                arySelectionEmail = strSelectionEmail.TrimEnd(',');
        }

        strToEmail = arySelectionEmail.TrimEnd(',');
        //----------------------------------------------------------

        string strTable = CreateDeclinedHtml();


        string strCCEmail = string.Empty;
        string strBCCEmail = "faztrackbd@gmail.com";
        if (strToEmail.Length > 4)
        {
            //string strUseEmailIntegration = System.Configuration.ConfigurationManager.AppSettings["UseEmailIntegration"];
            //if (strUseEmailIntegration == "Yes")
            //{
                #region Sendemail by out look
                try
                {
                    string sMessageUniqueId = "";

                    ExchangeService service = EWSAPI.GetEWSService("alyons@azinteriorinnovations.com", "Innovation5");

                    EmailMessage message = new EmailMessage(service);
                    message.Subject = "Customer " + objCust.last_name1 + " declined selection(s)";
                    message.Body = strTable;

                    if (strToEmail.Length > 4)
                    {
                        string[] strIds = strToEmail.Split(',');
                        foreach (string strId in strIds)
                        {
                            // string strTest = strId.Trim();
                            message.ToRecipients.Add(strId.Trim());
                        }
                    }

                    if (strCCEmail.Length > 4)
                    {
                        string[] strCCIds = strCCEmail.Split(',');
                        foreach (string strCCId in strCCIds)
                        {
                            message.CcRecipients.Add(strCCId.Trim());
                        }
                    }

                    if (strBCCEmail.Length > 4)
                    {
                        string[] strBCCIds = strBCCEmail.Split(',');
                        foreach (string strBCCId in strBCCIds)
                        {
                            message.BccRecipients.Add(strBCCId.Trim());
                        }
                    }

                    // Create a custom extended property and add it to the message.

                    Guid PropertySetId = Guid.NewGuid();
                    ExtendedPropertyDefinition fazExtendedPropertyDefinition = new ExtendedPropertyDefinition(PropertySetId, "FaztrackPropertyName", MapiPropertyType.String);
                    message.SetExtendedProperty(fazExtendedPropertyDefinition, "FaztrackPropertyName");

                    message.SendAndSaveCopy();

                    System.Threading.Thread.Sleep(1000);

                    // sMessageUniqueId = EWSAPI.GetEmailId(fazExtendedPropertyDefinition, service);



                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

                }
                #endregion
           // }
        }

    }

    string CreateDeclinedHtml()
    {

        string strQ = "SELECT * FROM Section_Selection WHERE isDeclined = 1 AND customer_id =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value) + "order by customer_siignatureDate DESC";
        DataTable dtFinal = csCommonUtility.GetDataTable(strQ);

        DataView dvFinal = dtFinal.DefaultView;
        //dvFinal.Sort = "ProjectNoteId,ProjectDate";
        string strFileHTML = "";
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='left'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + lblHeaderTitle.Text + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='10%'>Approve Date</td><td width='10%'>Date</td><td width='10%'>Section</td><td width='10%'>Location</td><td width='15%'>Title</td><td width='15%'>Notes</td><td width='10%'>Price</td><td width='20%'></td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string str = string.Empty;
            if (Convert.ToBoolean(dr["isSelected"]) == true)
            {
                //DateTime dtCreateDate = Convert.ToDateTime(dr["CreateDate"]);
                //DateTime dtValidTill = Convert.ToDateTime(dr["ValidTillDate"]);
                //TimeSpan ts = dtValidTill - dtCreateDate;
                //int nDay = ts.Days;

                //string strDay = nDay.ToString() + " Day(s)";
                //if (nDay >= 0)
                //{
                string strF = "SELECT * FROM file_upload_info WHERE vendor_cost_id = " + Convert.ToInt32(dr["SectionSelectionID"]) + " AND type = 5 AND  CustomerId =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value);
                DataTable dtFile = csCommonUtility.GetDataTable(strF);
                if (dtFile.Rows.Count > 0)
                {
                    strFileHTML = "";
                    strFileHTML = "<table> <tr> ";
                    foreach (DataRow drf in dtFile.Rows)
                    {
                        string imgUrl = "https://ii.faztrack.com/";
                        string strFileName = drf["ImageName"].ToString().Replace(" ", "%20");

                        if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                        {
                            //imgUrl += "UploadedFiles/" + hdnCustomerID.Value + "/SELECTIONS/" + strFileName;
                            imgUrl += "File/" + hdnCustomerID.Value + "/SELECTIONS/Thumbnail/" + strFileName;
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                        }
                        else if (strFileName.Contains(".pdf"))
                        {
                            imgUrl += "images/icon_pdf.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                        }
                        else if (strFileName.Contains(".doc") || strFileName.Contains(".docx"))
                        {
                            imgUrl += "images/icon_docs.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";

                        }
                        else if (strFileName.Contains(".xls") || strFileName.Contains(".xlsx") || strFileName.Contains(".csv"))
                        {
                            imgUrl += "images/icon_excel.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";

                        }
                        else if (strFileName.Contains(".txt") || strFileName.Contains(".TXT"))
                        {
                            imgUrl += "images/icon_txt.png";
                            strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                        }
                    }
                    strFileHTML += "</tr> </table>";

                }
                else
                {
                    strFileHTML = "";
                }

                string strTemp = strFileHTML;

                string strColor = "";

                if (i % 2 == 0)
                    strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
                else
                    strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
                if (dr["section_name"].ToString().Length > 0)
                {
                    strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["customer_siignatureDate"]).ToShortDateString() + "</td><td>" + Convert.ToDateTime(dr["CreateDate"]).ToShortDateString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["Title"].ToString() + "</td><td>" + dr["Notes"].ToString() + "</td><td>" + Convert.ToDecimal(dr["Price"]).ToString("c") + "</td><td>" + strFileHTML + "</td></tr>";
                }
                //}
            }

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }
}