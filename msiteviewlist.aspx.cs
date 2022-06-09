using System;
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
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;

public partial class msiteviewlist : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
     
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
            {
                hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("nestid")).ToString();
                hdnCID.Value = Convert.ToInt32(Request.QueryString.Get("nbackId")).ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if (objCust != null)
                {
                    sales_person sap = new sales_person();
                    sap = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(objCust.sales_person_id));
                    if (sap != null)
                    {
                        hdnSalesEmail.Value = sap.email;
                    }
                    string strSuperintendent = string.Empty;
                    user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId);
                    if (uinfo != null)
                    {
                        strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                        hdnSuperandentEmail.Value = uinfo.email;
                    }
                    hdnLastName.Value = objCust.last_name1;
                    hdnClientId.Value = objCust.client_id.ToString();
                }

                txtStartDate.Text = DateTime.Now.AddDays(-7).ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                DeleteTemporaryFiles();
            }
            
            BindSiteReviewDetails(0);
           
          
            btnSiteViewDetailDelete.OnClientClick = "return confirm('Are you sure you want to delere site review?');";
            lblCustomerLastName.Text = "(" + hdnLastName.Value + ", " + GetJobNumber(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value)) + ")";
           
        }
    }

    private string GetJobNumber(int nCustId, int nEstId)
    {
        string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number??"";
        if (strJobNumber=="")
        {
            strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).job_number ?? "";
        }

        return strJobNumber;
    }
    private void BindImageUploadedDetails(int nSiteReviewId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
       if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
        {
            
           //int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            strQ = " select [SiteReview_attach_id], [SiteReviewsId] ,[customer_id],[estimate_id],[SiteReview_file_name] " +
                   " from [SiteReview_upolad_info] where SiteReviewsId=" + nSiteReviewId;
       }
       IEnumerable<csSiteReviewUpload> mList = _db.ExecuteQuery<csSiteReviewUpload>(strQ, string.Empty).ToList();
       if (mList.Count() > 0)
       {
           pnlDetailImageUpload.Visible = true;
       }
       else
       {
           pnlDetailImageUpload.Visible = false;
       }
        grdImageDetails.DataSource = mList;
        grdImageDetails.DataKeyNames = new string[] { "SiteReview_attach_id", "SiteReviewsId", "customer_id", "estimate_id", "SiteReview_file_name" };
        grdImageDetails.DataBind();
    }

    protected void DeleteViewDetailImage(object sender, EventArgs e)
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";
            lblMsg.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            string nSiteReview_attach_id = (sender as ImageButton).CommandArgument;
            SiteReview_upolad_info objSRU = _db.SiteReview_upolad_infos.SingleOrDefault(sr => sr.SiteReview_attach_id == Convert.ToInt32(nSiteReview_attach_id));

            if (objSRU.SiteReview_file_name.Contains(".jpg") || objSRU.SiteReview_file_name.Contains(".jpeg") || objSRU.SiteReview_file_name.Contains(".png") || objSRU.SiteReview_file_name.Contains(".gif"))
            {
                string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//" + objSRU.SiteReview_file_name;
                File.Delete(sIMAGESFolderPath);
            }
            else
            {
                string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//" + objSRU.SiteReview_file_name;
                File.Delete(sFolderPath);
            }
            strQ = "delete from SiteReview_upolad_info where SiteReview_attach_id=" + Convert.ToInt32(nSiteReview_attach_id);
            _db.ExecuteCommand(strQ, string.Empty);
            BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));
           
            BindSiteReviewDetails(0);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "#myModal", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').hide();", true);
            hdnSiteReviewId.Value = "0";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void grdImageDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {

                int SiteReview_attach_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int SiteReviewsId = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[1].ToString());
                int customer_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[2].ToString());
                int estimate_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string SiteReview_file_name = grdImageDetails.DataKeys[e.Row.RowIndex].Values[4].ToString();


                string fileName = SiteReview_file_name.Substring(0, 10);
                Label lblFileName = (Label)e.Row.FindControl("lblFileName");
                lblFileName.Text = fileName;


               
                ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
                imgDelete.CommandArgument = SiteReview_attach_id.ToString();


                if (SiteReview_file_name.Contains(".jpg") || SiteReview_file_name.Contains(".jpeg") || SiteReview_file_name.Contains(".png") || SiteReview_file_name.Contains(".gif"))
                {
                    HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                    hypImg.Visible = true;

                    hypImg.NavigateUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name; 
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

                    img.ImageUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name; 
                    //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);
                }


                if (SiteReview_file_name.Contains(".pdf"))
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
                    hypPDF.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                    hypPDF.Target = "_blank";
                }
                if (SiteReview_file_name.Contains(".doc") || SiteReview_file_name.Contains(".docx"))
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
                    hypDoc.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                    hypDoc.Target = "_blank";

                }
                if (SiteReview_file_name.Contains(".xls") || SiteReview_file_name.Contains(".xlsx") || SiteReview_file_name.Contains(".csv"))
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
                    hypExcel.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                    hypExcel.Target = "_blank";
                }
                if (SiteReview_file_name.Contains(".txt") || SiteReview_file_name.Contains(".TXT"))
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
                    hypTXT.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                    hypTXT.Target = "_blank";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    protected void BindSiteReviewDetails(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdSiteViewList.PageIndex = nPageNo;
        string strQ = string.Empty;
        string strCondition = "";

        if (Session["oUser"] != null)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND SiteReviewsDate>='" + strStartDate + "' AND  SiteReviewsDate<'" + strEndDate.AddDays(1).ToString() + "' ";
            }

        }

        if (Session["oCrew"] != null)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition = "WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND SiteReviewsDate>='" + strStartDate + "' AND  SiteReviewsDate<'" + strEndDate.AddDays(1).ToString() + "' ";
            }
        }

        strQ = " select [SiteReviewsId] ,[customer_id],[estimate_id],[SiteReviewsNotes],[SiteReviewsDate],[StateOfMindID], " +
           " [IsUserView],[IsCustomerView] ,[IsVendorView],[HasAttachments] ,[AttachmentList] ,[CreatedBy] ,[CreateDate] ,[LastUpdatedBy],[LastUpdateDate] " +
           " from [SiteReviewNotes] " + strCondition + " order by CreateDate DESC";

        IEnumerable<csSiteReview> mList = _db.ExecuteQuery<csSiteReview>(strQ, string.Empty).ToList();

        grdSiteViewList.DataSource = mList;
        grdSiteViewList.DataKeyNames = new string[] { "SiteReviewsId", "customer_id", "estimate_id" };
        grdSiteViewList.DataBind();

        hdnCurrentPageNo.Value = Convert.ToString(nPageNo + 1);
    }
    protected void ViewSiteViewDetails(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

            lblCustomerName.Text = hdnLastName.Value;
            //Delete Temporary files

            DeleteTemporaryFiles();
            lblResult.Text = "";
            lblMsg.Text = "";
            pnlDetailImageUpload.Visible = true;
            LinkButton lnkViewDetails = (LinkButton)sender;
            int nSiteReviewId = Convert.ToInt32(lnkViewDetails.CommandArgument);
            hdnSiteReviewId.Value = nSiteReviewId.ToString();
            if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
            {
                btnAttachment.Visible = false;
                btnSiteViewDetailDelete.Visible = true;
               // btnSubmit.Text = "Update";
            }
            else
            {
                btnAttachment.Visible = true;
                btnSubmit.Text = "Save";
            }
            SiteReviewNote objSRN = new SiteReviewNote();
            objSRN = _db.SiteReviewNotes.SingleOrDefault(c => c.SiteReviewsId == nSiteReviewId );
            if (objSRN != null)
            {
                txtSiteReviewDate.Text = Convert.ToDateTime(objSRN.SiteReviewsDate).ToString("MM-dd-yyyy");
                txtSiteReviewNote.Text = objSRN.SiteReviewsNotes;
                if (objSRN.StateOfMindID> 0)
                {
                    chkCustomerMind.Checked = true;
                    pnlCustomerStateofMind.Visible = true;
                    if (objSRN.StateOfMindID == 1)
                    {
                        hdnCustomerMind.Value = "1";
                        imgbtnAngry.Attributes.Add("class", "opacityimage");
                        imgbtnFrustrated.Attributes.Add("class", "");
                        imgbtnConfused.Attributes.Add("class", "");
                        imgbtnIndifferent.Attributes.Add("class", "");
                        imgbtnHappy.Attributes.Add("class", "");
                    }
                    else if (objSRN.StateOfMindID == 2)
                    {
                        hdnCustomerMind.Value = "2";
                        imgbtnAngry.Attributes.Add("class", "");
                        imgbtnFrustrated.Attributes.Add("class", "opacityimage");
                        imgbtnConfused.Attributes.Add("class", "");
                        imgbtnIndifferent.Attributes.Add("class", "");
                        imgbtnHappy.Attributes.Add("class", "");
                    }
                    else if (objSRN.StateOfMindID == 3)
                    {
                        hdnCustomerMind.Value = "3";
                        imgbtnAngry.Attributes.Add("class", "");
                        imgbtnFrustrated.Attributes.Add("class", "");
                        imgbtnConfused.Attributes.Add("class", "opacityimage");
                        imgbtnIndifferent.Attributes.Add("class", "");
                        imgbtnHappy.Attributes.Add("class", "");
                    }
                    else if (objSRN.StateOfMindID == 4)
                    {
                        hdnCustomerMind.Value = "4";
                        imgbtnAngry.Attributes.Add("class", "");
                        imgbtnFrustrated.Attributes.Add("class", "");
                        imgbtnConfused.Attributes.Add("class", "");
                        imgbtnIndifferent.Attributes.Add("class", "opacityimage");
                        imgbtnHappy.Attributes.Add("class", "");
                    }
                    else if (objSRN.StateOfMindID == 5)
                    {
                        hdnCustomerMind.Value = "5";
                        imgbtnAngry.Attributes.Add("class", "");
                        imgbtnFrustrated.Attributes.Add("class", "");
                        imgbtnConfused.Attributes.Add("class", "");
                        imgbtnIndifferent.Attributes.Add("class", "");
                        imgbtnHappy.Attributes.Add("class", "opacityimage");
                    }
                }
                else
                {

                    pnlCustomerStateofMind.Visible = false;
                    chkCustomerMind.Checked = false;
                    imgbtnAngry.Attributes.Add("class", "");
                    imgbtnFrustrated.Attributes.Add("class", "");
                    imgbtnConfused.Attributes.Add("class", "");
                    imgbtnIndifferent.Attributes.Add("class", "");
                    imgbtnHappy.Attributes.Add("class", "");
                }
                if (objSRN.IsUserView==true)
                    chkUser.Checked = true;
                else
                    chkUser.Checked = false;

                if (objSRN.IsCustomerView == true)
                    chkCustomer.Checked = true;
                else
                    chkCustomer.Checked = false;
                if (objSRN.IsVendorView == true)
                    chkVendor.Checked = true;
                else
                    chkVendor.Checked = false;
                if (objSRN.IsUserNotify == true)
                    chkUserNotify.Checked = true;
                else
                    chkUserNotify.Checked = false;

                if (objSRN.IsCustomerNotify == true)
                    chkCustomerNotify.Checked = true;
                else
                    chkCustomerNotify.Checked = false;

                if (objSRN.IsVendorNotify == true)
                    chkVendorNotify.Checked = true;
                else
                    chkVendorNotify.Checked = false;

                BindImageUploadedDetails(objSRN.SiteReviewsId);
               
            }
           
        }
        catch(Exception ex)
        {
            throw ex;
        }
 }

    private void DeleteTemporaryFiles()
    {
        string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
        if (Directory.Exists(DestinationPath))
        {
            string[] fileEntries = Directory.GetFiles(DestinationPath);
            foreach (string file in fileEntries)
            {
                File.Delete(file);
            }

        }
    }


    protected void ViewAllAttachment(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {

            LinkButton InkAttachmentView = (LinkButton)sender;
            int nSiteReviewId = Convert.ToInt32(InkAttachmentView.CommandArgument);
            //hdnSiteReviewId.Value = nSiteReviewId.ToString();
            SiteReview_upolad_info objSRU = new SiteReview_upolad_info();

            SiteReviewNote objSRN = _db.SiteReviewNotes.SingleOrDefault(sr => sr.SiteReviewsId == nSiteReviewId);
            if (objSRN.HasAttachments == true)
            {
                var attchmentList = from su in _db.SiteReview_upolad_infos
                                    where su.SiteReviewsId == nSiteReviewId
                                    orderby su.SiteReview_attach_id ascending
                                    select su;
                grdAttchmentViewImage.DataSource = attchmentList;
                grdAttchmentViewImage.DataKeyNames = new string[] { "SiteReview_attach_id", "SiteReviewsId", "customer_id", "estimate_id", "SiteReview_file_name" };
                grdAttchmentViewImage.DataBind();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalAttachmentView();", true);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
  }


  


    protected void grdAttchmentViewImage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                 string fileName=string.Empty;
                int SiteReview_attach_id = Convert.ToInt32(grdAttchmentViewImage.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int SiteReviewsId = Convert.ToInt32(grdAttchmentViewImage.DataKeys[e.Row.RowIndex].Values[1].ToString());
                int customer_id = Convert.ToInt32(grdAttchmentViewImage.DataKeys[e.Row.RowIndex].Values[2].ToString());
                int estimate_id = Convert.ToInt32(grdAttchmentViewImage.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string SiteReview_file_name = grdAttchmentViewImage.DataKeys[e.Row.RowIndex].Values[4].ToString();

                Label lblFileName = (Label)e.Row.FindControl("lblFileName");
                if (SiteReview_file_name.Length > 10)
                {
                    fileName = SiteReview_file_name.Substring(0, 10);
                    lblFileName.Text = fileName;
                }
                else
                {
                    lblFileName.Text = SiteReview_file_name;
                }


                if (SiteReview_file_name.Contains(".jpg") || SiteReview_file_name.Contains(".jpeg") || SiteReview_file_name.Contains(".png") || SiteReview_file_name.Contains(".gif"))
                {
                    HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                    hypImg.Visible = true;

                    hypImg.NavigateUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name; 
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

                     img.ImageUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name; 
                    //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);
                }

              
               if (SiteReview_file_name.Contains(".pdf"))
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
                   hypPDF.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                   hypPDF.Target = "_blank";
               }
               if (SiteReview_file_name.Contains(".doc") || SiteReview_file_name.Contains(".docx"))
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
                   hypDoc.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                   hypDoc.Target = "_blank";

               }
               if (SiteReview_file_name.Contains(".xls") || SiteReview_file_name.Contains(".xlsx") || SiteReview_file_name.Contains(".csv"))
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
                   hypExcel.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                   hypExcel.Target = "_blank";
               }
               if (SiteReview_file_name.Contains(".txt") || SiteReview_file_name.Contains(".TXT"))
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
                   hypTXT.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name; 
                   hypTXT.Target = "_blank";
               }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    protected void grdSiteViewList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int SiteReviewsId = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[0]);
            int customer_id = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[1]);
            int estimate_id = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[2]);

            SiteReviewNote objSRN = new SiteReviewNote();
            objSRN = _db.SiteReviewNotes.SingleOrDefault(c => c.SiteReviewsId ==SiteReviewsId&&c.customer_id==customer_id && c.estimate_id==estimate_id);
            if (objSRN != null)
            {
                LinkButton InkViewDetails = (LinkButton)e.Row.FindControl("InkViewDetails");
                InkViewDetails.Text = Convert.ToDateTime(objSRN.SiteReviewsDate).ToString("MM-dd-yyyy");
                InkViewDetails.CommandArgument = objSRN.SiteReviewsId.ToString();
            

                Label lblSiteReviewsNotes = (Label)e.Row.FindControl("lblSiteReviewsNotes");
                lblSiteReviewsNotes.Text = objSRN.SiteReviewsNotes;

                int attachmentCount = (from src in _db.SiteReview_upolad_infos where src.SiteReviewsId == SiteReviewsId select src).ToList().Count();

                Label lblAttachmentCount = (Label)e.Row.FindControl("lblAttachmentCount");
                Panel pnlAttachmentView = (Panel)e.Row.FindControl("pnlAttachmentView");

                LinkButton InkAttachmentView = (LinkButton)e.Row.FindControl("InkAttachmentView");
                 if(attachmentCount>0)
                 {
                     lblAttachmentCount.Text = "("+attachmentCount.ToString()+")";
                     InkAttachmentView.Text = "View All";
                     InkAttachmentView.CommandArgument = objSRN.SiteReviewsId.ToString();
                     pnlAttachmentView.Visible = true; 
                 }
                 else
                 {
                     pnlAttachmentView.Visible = false; 
                 }
                Label lblCreateBy = (Label)e.Row.FindControl("lblCreateBy");
                lblCreateBy.Text = objSRN.CreatedBy;
              
             }
        }
    }

    protected void grdSiteViewList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdSiteViewList.ID, grdSiteViewList.GetType().Name, "PageIndexChanging"); 
        BindSiteReviewDetails(e.NewPageIndex);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
    }
   

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMsg.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        SiteReviewNote objSRN = new SiteReviewNote();

        if (txtSiteReviewDate.Text.Trim() != "")
        {
            try
            {
                Convert.ToDateTime(txtSiteReviewDate.Text.Trim());
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalide Date.<br />");
                return;
            }
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Date.<br />");
            return;
        }
        string DestinationPath = null;
        DestinationPath = Server.MapPath("~/Uploads//Temp//") + hdnCustomerId.Value + "//";
        if (!System.IO.Directory.Exists(DestinationPath))
        {
            System.IO.Directory.CreateDirectory(DestinationPath);
        }
        string sourceFile = Server.MapPath("Uploads\\Temp\\") + hdnCustomerId.Value + @"\";
        string[] fileEntries = Directory.GetFiles(sourceFile);
        if (fileEntries.Length == 0 && txtSiteReviewNote.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Notes or Attached files .<br />");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            return;
        }

        if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
            objSRN = _db.SiteReviewNotes.Single(c => c.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value));

        objSRN.customer_id =Convert.ToInt32(hdnCustomerId.Value);
        objSRN.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        objSRN.SiteReviewsDate = Convert.ToDateTime(txtSiteReviewDate.Text);
        objSRN.SiteReviewsNotes = txtSiteReviewNote.Text;
        objSRN.client_id = Convert.ToInt32(hdnClientId.Value);

        if (Session["oUser"] != null)
        {
            objSRN.IsCrew = false;
        }

        if (Session["oCrew"] != null)
        {
            objSRN.IsCrew = true;
        }
        // Customer State of Mind

        if (chkCustomerMind.Checked)
        {

            objSRN.StateOfMindID = Convert.ToInt32(hdnCustomerMind.Value);
        }
        else
        {
            objSRN.StateOfMindID = 0;
        }
        if (chkUser.Checked)
            objSRN.IsUserView = true;
        else
            objSRN.IsUserView = false;
        if (chkCustomer.Checked)
            objSRN.IsCustomerView = true;
        else
            objSRN.IsCustomerView = false;
        if (chkVendor.Checked)
            objSRN.IsVendorView = true;
        else
            objSRN.IsVendorView = false;

        if (chkUserNotify.Checked)
            objSRN.IsUserNotify = true;
        else
            objSRN.IsUserNotify = false;

        if (chkCustomerNotify.Checked)
            objSRN.IsCustomerNotify = true;
        else
            objSRN.IsCustomerNotify = false;

        if (chkVendorNotify.Checked)
            objSRN.IsVendorNotify = true;
        else
            objSRN.IsVendorNotify = false;


            if (Convert.ToInt32(hdnSiteReviewId.Value) == 0)
            {
                objSRN.HasAttachments = false;
                objSRN.LastUpdateDate = DateTime.Now;
                objSRN.CreateDate = DateTime.Now;
                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    objSRN.CreatedBy = objUser.first_name+" "+objUser.last_name;
                }
                if (Session["oCrew"] != null)
                {
                    Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                    objSRN.CreatedBy = objC.first_name + " " + objC.last_name;
                }
                
                _db.SiteReviewNotes.InsertOnSubmit(objSRN);
                _db.SubmitChanges();
                hdnSiteReviewId.Value = objSRN.SiteReviewsId.ToString();
              }
            else
            {

                objSRN.LastUpdateDate = DateTime.Now;
                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    objSRN.CreatedBy = objUser.first_name + " " + objUser.last_name;
                }
                if (Session["oCrew"] != null)
                {
                    Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                    objSRN.CreatedBy = objC.first_name + " " + objC.last_name;
                }
                
                _db.SubmitChanges();

            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully.");
            if (fileEntries.Length>0)
                MoveFile();
            BindSiteReviewDetails(0);
            if (chkEmail.Checked)
            {
                SendSiteReviewEmail();

            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
    }

    private void SendSiteReviewEmail()
    {
            DataClassesDataContext _db = new DataClassesDataContext();   
           string strBody = CreateHtml();
            string strFrom = string.Empty;
            string strTO = string.Empty;
            ProjectNotesEmailInfo ObjPei = _db.ProjectNotesEmailInfos.FirstOrDefault(p => p.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            string AddtionalEmail = string.Empty;

            if (ObjPei != null)
            {
                AddtionalEmail = ObjPei.AddtionalEmail;
            }

            strTO = hdnSalesEmail.Value.ToString();

            if (strTO.Length == 0)
            {
                strTO = hdnSuperandentEmail.Value.ToString();
            }
            else
            {
                if (hdnSuperandentEmail.Value.ToString() != "")
                {
                    strTO = hdnSalesEmail.Value.ToString() + ", " + hdnSuperandentEmail.Value.ToString();
                }

            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (AddtionalEmail.Length > 3)
            {
                string[] strIds = AddtionalEmail.Split(',');
                foreach (string strId in strIds)
                {
                    Match match1 = regex.Match(strId.Trim());
                    if (match1.Success)
                    {
                        if (strTO.Length == 0)
                        {
                            strTO = strId;
                        }
                        else
                        {
                            strTO = ", " + strId;
                        }

                    }
                }

            }

            string strBCCEmail = "avijit019@gmail.com";

            if (strTO.Length > 4)
            {


                try
                {
                    userinfo obj = new userinfo();
                    if ((userinfo)Session["oUser"] != null)
                    {
                        obj = (userinfo)Session["oUser"];
                        hdnEmailType.Value = obj.EmailIntegrationType.ToString();
                        strFrom = obj.company_email;

                    }
                    if (Convert.ToInt32(hdnEmailType.Value) == 1) // outlook email
                    {
                        if (strFrom.Trim().Length > 4)
                        {
                            ExchangeService service = EWSAPI.GetEWSService(strFrom.Trim());

                            EmailMessage message = new EmailMessage(service);
                            message.Subject = "Site Review for " + "(" + hdnLastName.Value + ")";
                            message.Body = strBody;

                            if (strTO.Length > 4)
                            {
                                string[] strIds = strTO.Split(',');
                                foreach (string strId in strIds)
                                {
                                    message.ToRecipients.Add(strId.Trim());
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

                        }
                    }
                    else
                    {
                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("info@interiorinnovations.biz");

                        if (strTO != "")
                            msg.To.Add(strTO);
                        else
                        {
                            return;
                        }
                        if (strBCCEmail.Length > 3)
                        {
                            msg.Bcc.Add(strBCCEmail);
                        }

                        msg.Subject = "Site Review for " + "(" + hdnLastName.Value + ")";

                        msg.IsBodyHtml = true;

                        msg.Body = strBody;
                        msg.Priority = MailPriority.High;

                        csCommonUtility.SendByLocalhost(msg);

                        //SmtpClient smtp = new SmtpClient();
                        //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                        //smtp.Send(msg);

                        msg.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                }

            }

        
    }

    private void ResetValue()
    {
        //Delete Temporary Files
        lblUpload.Visible = false;
        chkCustomerMind.Checked = false;
        pnlCustomerStateofMind.Visible = false;
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
        btnSiteViewDetailDelete.Visible = false;
        lblResult.Text = "";
        lblMsg.Text = "";
        btnSubmit.Text = "Save";
        txtSiteReviewDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
         chkUser.Checked = false;
        chkCustomer.Checked = false;
        chkVendor.Checked = false;
        chkUserNotify.Checked = false;
        chkCustomerNotify.Checked = false;
        chkVendorNotify.Checked = false;
        grdTemp.DataSource = null;
        grdTemp.DataBind();
        grdImageDetails.DataSource = null;
        grdImageDetails.DataBind();
        pnlTemporaryImageUpload.Visible = false;
        pnlDetailImageUpload.Visible = false;
    }


    private void MoveFile()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            SiteReview_upolad_info objSRU = null;
            SiteReviewNote objSRN = new SiteReviewNote();
            string sFileName = "";

            string DestinationPath = null;
            DestinationPath = Server.MapPath("~/Uploads//Temp//") + hdnCustomerId.Value + "//";
            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            string sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\";
            string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//";
            string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//";

            if (!System.IO.Directory.Exists(sIMAGESFolderPath))
            {
                System.IO.Directory.CreateDirectory(sIMAGESFolderPath);
            }
            if (!System.IO.Directory.Exists(sFolderPath))
            {
                System.IO.Directory.CreateDirectory(sFolderPath);
            }
            string[] fileEntries1 = Directory.GetFiles(sourceFile);
            foreach (string file in fileEntries1)
            {
                sFileName = Path.GetFileName(file);
                objSRU = new SiteReview_upolad_info();
                objSRU.SiteReviewsId = Convert.ToInt32(hdnSiteReviewId.Value);
                objSRU.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                objSRU.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                objSRU.SiteReview_file_name = sFileName;
                objSRU.create_date = DateTime.Now;
                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    objSRU.CreatedBy = objUser.first_name + " " + objUser.last_name;
                }
                if (Session["oCrew"] != null)
                {
                    Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                    objSRU.CreatedBy = objC.first_name + " " + objC.last_name;
                }
                _db.SiteReview_upolad_infos.InsertOnSubmit(objSRU);
                _db.SubmitChanges();


            }
        

            // Move Temp File to Destinition
            string[] fileEntries = Directory.GetFiles(sourceFile);
            if (fileEntries.Count() > 0)
            {
                if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
                {
                    objSRN = _db.SiteReviewNotes.Single(c => c.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value));
                    objSRN.HasAttachments = true;
                    _db.SubmitChanges();
                }

                foreach (string file in fileEntries)
                {
                    sFileName = Path.GetFileName(file);
                    sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\" + sFileName;
                    if (sFileName.Contains(".jpg") || sFileName.Contains(".jpeg") || sFileName.Contains(".png") || sFileName.Contains(".gif"))
                    {
                        File.Move(sourceFile, sIMAGESFolderPath + sFileName);
                    }
                    else
                    {
                        File.Move(sourceFile, sFolderPath + sFileName);
                    }
                }
            }

            // grdTemp.Visible = false;
            grdTemp.DataSource = null;
            grdTemp.DataBind();
            BindTempGrid();
            lblUpload.Visible = false;
            BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));
        }
        catch (Exception ex)
        {
            string s = ex.Message;
        }
    }

    

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
       // ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ImageopenModal();", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
       
        HttpFileCollection fileCollection = Request.Files;
        string DestinationPath = null;
      
        DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }

            for (int i = 0; i < fileCollection.Count; i++)
            {
               HttpPostedFile uploadfile = fileCollection[i];
                string fileName = "";
                string fileExt = Path.GetExtension(uploadfile.FileName);
                if (uploadfile.ContentLength > 0)
                {
                    int iFileSize = uploadfile.ContentLength;
                    if ((iFileSize / (1024*2014)) > 2)  // 2MB approx (actually less though)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("File size maximum allowed 2MB");
                        return;
                    }
                    if (uploadfile.FileName.Contains(".jpg") || uploadfile.FileName.Contains(".jpeg") || uploadfile.FileName.Contains(".png") || uploadfile.FileName.Contains(".PNG") || uploadfile.FileName.Contains(".gif") || uploadfile.FileName.Contains(".GIF"))
                    {


                        fileName = uploadfile.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                        string outputFileName = Path.Combine(DestinationPath, fileName);
                        System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(uploadfile.InputStream);
                        System.Drawing.Imaging.ImageFormat format = bmpImage.RawFormat;
                        int newWidth = 1000;
                        int newHeight = 800;
                        System.Drawing.Bitmap bmpOut = null;
                        bmpOut = new System.Drawing.Bitmap(newWidth, newHeight);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpOut);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(System.Drawing.Brushes.White, 0, 0, newWidth, newHeight);
                        g.DrawImage(bmpImage, 0, 0, newWidth, newHeight);
                        bmpImage.Dispose();
                        bmpOut.Save(outputFileName, format);
                        bmpOut.Dispose();
                        
                    }
                    else
                    {
                        fileName = uploadfile.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                        uploadfile.SaveAs(DestinationPath + fileName);
                    }
                   
                }

            }
            BindTempGrid();
          

        
    }

    protected void btnAttachment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAttachment.ID, btnAttachment.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        BindTempGrid();
    }

    protected void btnSiteViewDetailDelete_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSiteViewDetailDelete.ID, btnSiteViewDetailDelete.GetType().Name, "Click"); 
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";
            lblMsg.Text = "";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

            strQ = "delete from SiteReviewNotes where SiteReviewsId=" + Convert.ToInt32(hdnSiteReviewId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//";

            string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//";


            var upList = (from upl in _db.SiteReview_upolad_infos
                          where upl.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value)
                          select new
                             {
                                 SiteReview_file_name = upl.SiteReview_file_name,
                             }).ToList();

            if (Directory.Exists(sFolderPath) || Directory.Exists(sIMAGESFolderPath))
            {
                // string[] fileEntries = Directory.GetFiles(DestinationPath);
                foreach (var file in upList)
                {
                    if (file.SiteReview_file_name.Contains(".jpg") || file.SiteReview_file_name.Contains(".jpeg") || file.SiteReview_file_name.Contains(".png") || file.SiteReview_file_name.Contains(".gif"))
                    {
                        sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//" + file.SiteReview_file_name;
                        File.Delete(sIMAGESFolderPath);
                    }
                    else
                    {
                        sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//" + file.SiteReview_file_name;
                        File.Delete(sFolderPath);
                    }

                    //File.Delete(file.SiteReview_file_name);
                }
            }


            strQ = "delete from SiteReview_upolad_info where SiteReviewsId=" + Convert.ToInt32(hdnSiteReviewId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            ResetValue();
            BindSiteReviewDetails(0);
            hdnSiteReviewId.Value = "0";
        }
        catch(Exception ex)
        {
            throw ex;
        }
       
      
    }

    protected void btnhdnValueReset_Click(object sender, EventArgs e)
    {
        lblCustomerName.Text = hdnLastName.Value;
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        ResetValue();
    }

    private void BindTempGrid()
    {
        int count = 0;
        DataTable tmpSTable = LoadtmpTable();
        DataRow dr = tmpSTable.NewRow();

        string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
        string[] fileEntries = Directory.GetFiles(DestinationPath);

        foreach (string file in fileEntries)
        {
            string FileName = Path.GetFileName(file);

            DataRow drNew = tmpSTable.NewRow();
            drNew["file_name"] = FileName;
          
            tmpSTable.Rows.Add(drNew);
            count++;
        }
        
        grdTemp.DataSource = tmpSTable;
        grdTemp.DataKeyNames = new string[] { "file_name" };
        grdTemp.DataBind();
        if (grdTemp.Rows.Count > 0)
        {
            lblUpload.Visible = true;
            pnlTemporaryImageUpload.Visible = true;
            // btnAttachment.Visible = true;
            lblAttachmentCount.Visible = false;
            lblAttachmentCount.Text = "Attach " + count.ToString();
        }
        else
        {
            pnlTemporaryImageUpload.Visible = false;
            // btnAttachment.Visible = false;
            count = 0;
            lblAttachmentCount.Visible = false;
            lblAttachmentCount.Text = "Attach " + count.ToString();
        }

    }
    private DataTable LoadtmpTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("file_name", typeof(string));
        return table;
    }


   // Temporary Image Load 
    protected void grdTemp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
          
            
            string file = grdTemp.DataKeys[e.Row.RowIndex].Value.ToString();
            string fileName =file.Substring(0, 10);
            Label lblFileName = (Label)e.Row.FindControl("lblFileName");
            lblFileName.Text = fileName;

            string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//" +file;
            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            imgDelete.CommandArgument = DestinationPath;

            if (file.Contains(".jpg") || file.Contains(".jpeg") || file.Contains(".png") || file.Contains(".gif"))
            {
                Image img = (Image)e.Row.FindControl("img");
                img.Visible = true;
                HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                hypPDF.Visible = false;
                HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                hypExcel.Visible = false;
                HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                hypDoc.Visible = false;
                HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                hypTXT.Visible = false;

                img.ImageUrl = "Uploads/Temp/"+hdnCustomerId.Value+"/" + file;
                
            }


            if (file.Contains(".pdf"))
            {
                Image img = (Image)e.Row.FindControl("img");
                img.Visible = false;
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
                hypPDF.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                hypPDF.Target = "_blank";
            }
            if (file.Contains(".doc") || file.Contains(".docx"))
            {
                Image img = (Image)e.Row.FindControl("img");
                img.Visible = false;

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
                hypDoc.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                hypDoc.Target = "_blank";

            }
            if (file.Contains(".xls") || file.Contains(".xlsx") || file.Contains(".csv"))
            {
                Image img = (Image)e.Row.FindControl("img");
                img.Visible = false;

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
                hypExcel.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                hypExcel.Target = "_blank";
            }
            if (file.Contains(".txt") || file.Contains(".TXT"))
            {
                Image img = (Image)e.Row.FindControl("img");
                img.Visible = false;

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
                hypTXT.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                hypTXT.Target = "_blank";
            }
        }
    }
    protected void DeleteFile(object sender, EventArgs e)
    {
        lblResult.Text = "";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        string filePath = (sender as ImageButton).CommandArgument;
        File.Delete(filePath);
        BindTempGrid();
      
        
    }

  

    protected void btnCaptureImage_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCaptureImage.ID, btnCaptureImage.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        string strRequired = string.Empty;
        lblResult.Text = "";
        lblMsg.Text = "";
        string originalImageName = string.Empty;
        string imageExtName = string.Empty;
        string savingImageName = string.Empty;
        HttpPostedFile captureImage = imgCapture.PostedFile;
        if (!(captureImage.ContentLength > 0))
        {
            captureImage = imgFileUpload.PostedFile;
        }

        if (captureImage.ContentLength == 0)
        {
            lblResult.Text = "Please select image.<br/>";
            lblResult.ForeColor = System.Drawing.Color.Red;

            return;
        }

        if (captureImage.ContentLength > 0)
        {
            originalImageName = captureImage.FileName;
            imageExtName = Path.GetExtension(originalImageName);
            if (originalImageName != "")
            {

                imageExtName = imageExtName.ToLower();
                string[] acceptedFileTypes = new string[4];
                acceptedFileTypes[0] = ".jpg";
                acceptedFileTypes[1] = ".jpeg";
                acceptedFileTypes[2] = ".gif";
                acceptedFileTypes[3] = ".png";
                bool acceptFile = false;
                for (int i = 0; i <= 3; i++)
                {
                    if (imageExtName == acceptedFileTypes[i])
                    {
                        acceptFile = true;
                    }
                }
                if (!acceptFile)
                {
                    strRequired += "Photo should be in jpg, jpeg, gif, png format.<br/>";
                }
            }
        }
        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            return;
        }



        HttpFileCollection fileCollection = Request.Files;
        string DestinationPath = null;
       
        DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
        if (!System.IO.Directory.Exists(DestinationPath))
        {
            System.IO.Directory.CreateDirectory(DestinationPath);
        }

        for (int i = 0; i < fileCollection.Count; i++)
        {

            HttpPostedFile imgCapture1 = fileCollection[i];
            
            string fileName = "";
            string fileExt = Path.GetExtension(imgCapture1.FileName);
            if (imgCapture1.ContentLength > 0)
            {
                fileName = imgCapture1.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                imgCapture1.SaveAs(DestinationPath + fileName);
           }
        }
        BindTempGrid();

    }

    protected void chkCustomerMind_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCustomerMind.ID, chkCustomerMind.GetType().Name, "CheckedChanged"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        if (chkCustomerMind.Checked)
        {
            pnlCustomerStateofMind.Visible = true;
        }
        else
        {
            pnlCustomerStateofMind.Visible = false;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCancel.ID, btnCancel.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        lblImageResult.Text = "";
        imgPreview.ImageUrl = "icons/default.png";
       
    }
    protected void imgbtnAngry_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnAngry.ID, imgbtnAngry.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        hdnCustomerMind.Value = "1";
        imgbtnAngry.Attributes.Add("class", "opacityimage");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnFrustrate_Click(object sender, ImageClickEventArgs e)
    {
        
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        hdnCustomerMind.Value = "2";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "opacityimage");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnConfused_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnConfused.ID, imgbtnConfused.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        hdnCustomerMind.Value = "3";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "opacityimage");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnIndifferent_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnIndifferent.ID, imgbtnIndifferent.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        hdnCustomerMind.Value = "4";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "opacityimage");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnHappay_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnHappy.ID, imgbtnHappy.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        hdnCustomerMind.Value = "5";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "opacityimage");
    }



    protected void btnSubmit_Convert_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        lblResult.Text = "";
        lblMsg.Text = "";
        string originalImageName = string.Empty;
        string imageExtName = string.Empty;
        string savingImageName = string.Empty;
        string DestinationPath = null;
        DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
        if (!System.IO.Directory.Exists(DestinationPath))
        {
            System.IO.Directory.CreateDirectory(DestinationPath);
        }
        savingImageName = "MobileCaptured Image _" + DateTime.Now.ToString("ddMMyyhhmmss") + ".jpg";

        string sBase64 = hdnDataURL.Value;
        hdnDataURL.Value = "";
        string strRequired = string.Empty;

    

        try
        {


            if (sBase64.Length == 0)
            {
                lblResult.Text = "Please select image.<br/>";
                lblResult.ForeColor = System.Drawing.Color.Red;
                
                return;
            }


            sBase64 = sBase64.Split(',')[1];
           string outputFileName = Path.Combine(DestinationPath, savingImageName);
           
            System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(new MemoryStream(Convert.FromBase64String(sBase64)));
            System.Drawing.Imaging.ImageFormat format = bmpImage.RawFormat;
            bmpImage.Save(outputFileName, format);
            bmpImage.Dispose();
            BindTempGrid();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
       if(Convert.ToInt32(hdnCID.Value)>0)
           Response.Redirect("mcustomerlist.aspx");
       else
           Response.Redirect("mlandingpage.aspx");

    }


    string CreateHtml()
    {
        string strHead = "Site Review for " + "(" + hdnLastName.Value + ")";
        string strQ = "SELECT * FROM SiteReviewNotes WHERE SiteReviewsId =" + Convert.ToInt32(hdnSiteReviewId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        DataTable dtFinal = csCommonUtility.GetDataTable(strQ);

        DataView dvFinal = dtFinal.DefaultView;
        string strFileHTML = "";
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='left'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + strHead + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='10%'>DATE</td><td width='54%'>NOTES</td><td width='18%'>ADDED BY</td> <td width='18%'>STATE OF MIND</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string str = string.Empty;

            string strF = "SELECT * FROM SiteReview_upolad_info WHERE SiteReviewsId = " + Convert.ToInt32(dr["SiteReviewsId"]) + " AND  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            DataTable dtFile = csCommonUtility.GetDataTable(strF);
            if (dtFile.Rows.Count > 0)
            {
                strFileHTML = "";
                strFileHTML = "<table> <tr> ";
                foreach (DataRow drf in dtFile.Rows)
                {
                    string imgUrl = "https://ii.faztrack.com/";
                    string strFileName = drf["SiteReview_file_name"].ToString().Replace(" ", "%20");
                    if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                    {
                        imgUrl += "UploadedFiles/" + hdnCustomerId.Value + "/IMAGES/" + strFileName;
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

            string imgStateUrl = "https://ii.faztrack.com/";
            string strColor = "";

            string StateOfMindID = "";

            if (Convert.ToInt32(dr["StateOfMindID"]) > 0)
            {
                if (Convert.ToInt32(dr["StateOfMindID"]) == 1)
                {
                    imgStateUrl += "/assets/customerstatemind/angry.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";
                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 2)
                {
                    imgStateUrl += "/assets/customerstatemind/frustrated.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 3)
                {
                    imgStateUrl += "/assets/customerstatemind/confused.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 4)
                {
                    imgStateUrl += "/assets/customerstatemind/indifferent.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 5)
                {
                    imgStateUrl += "/assets/customerstatemind/happy.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else
                {
                    StateOfMindID = "";

                }


            }

            if (i % 2 == 0)
                strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            if (dr["SiteReviewsDate"].ToString().Length > 0)
            {
                strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["SiteReviewsDate"]).ToShortDateString() + "</td><td>" + dr["SiteReviewsNotes"].ToString() + "</td><td>" + dr["CreatedBy"].ToString() + "</td> <td>" + StateOfMindID + "</td></tr>";
                if (strFileHTML.Length > 4)
                {
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'> FILE(S) </td></tr>";
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'>" + strFileHTML + "</td></tr>";
                }
                else
                {
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'>No file attached </td></tr>";

                }
            }

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMsg.Text = "";
        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (txtStartDate.Text == "")
        {
            lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("Start Date is a required field");

            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtStartDate.Text);
            }
            catch (Exception ex)
            {
                lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Invalid Start Date");

                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("End Date is a required field");

            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtEndDate.Text);
            }
            catch (Exception ex)
            {
                lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Invalid End Date");

                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");

            return;
        }

        BindSiteReviewDetails(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {

        txtStartDate.Text = DateTime.Now.AddDays(-7).ToShortDateString();
        txtEndDate.Text = DateTime.Now.ToShortDateString();
        lblResult.Text = "";
        lblMsg.Text = "";
        BindSiteReviewDetails(0);
    }
    protected void btnResetCId_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnResetCId.ID, btnResetCId.GetType().Name, "Click"); 
        hdnSiteReviewId.Value = "0";
        txtSiteReviewNote.Text = "";
        ResetValue();
    }
 
}