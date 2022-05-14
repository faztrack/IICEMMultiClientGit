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

public partial class sitereviewlist : System.Web.UI.Page
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
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            int nCustId = 0;
            int nEstId = 0;
            if (Request.QueryString.Get("cid") != null)
            {
                nCustId = Convert.ToInt32(Request.QueryString.Get("cid"));
                nEstId = Convert.ToInt32(Request.QueryString.Get("nestid"));
                hdnCustomerId.Value = nCustId.ToString();
                hdnEstimateId.Value = nEstId.ToString();
                hdnBackId.Value = Convert.ToInt32(Request.QueryString.Get("nbackId")).ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
            }

            GetCustomerName();
            GetSiteReviews();
            BindSiteReviewDetails(0);
       
            string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).job_number ?? "";

            if (strJobNumber.Length > 0)
            {
                if (csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number == "" || csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number == null)
                    lblTitelJobNumber.Text = " ( Job Number: " + strJobNumber + " )";
                else
                    lblTitelJobNumber.Text = " ( Job Number: " + csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number + " )";
            }
        }
    }

    private void GetSiteReviews()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = "";
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND SiteReviewsDate>='" + strStartDate + "' AND  SiteReviewsDate<'" + strEndDate.AddDays(1).ToString() + "' ";
            }
            else
            {
                strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
            }

            //  strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);

            string strQ = string.Empty;
            strQ = " select [SiteReviewsId] ,[customer_id],[estimate_id],[SiteReviewsNotes],[SiteReviewsDate],[StateOfMindID], " +
                   " [IsUserView],[IsCustomerView] ,[IsVendorView],[HasAttachments] ,[AttachmentList] ,[CreatedBy] ,[CreateDate] ,[LastUpdatedBy],[LastUpdateDate] " +
                   " from [SiteReviewNotes] " + strCondition + " order by CreateDate DESC";

            IEnumerable<csSiteReview> mList = _db.ExecuteQuery<csSiteReview>(strQ, string.Empty).ToList();
            if (mList.Count() > 0)
                Session.Add("nSiteReviewList", csCommonUtility.LINQToDataTable(mList));
            else
                Session.Remove("nSiteReviewList");
            BindSiteReviewDetails(0);
           
        }
        catch (Exception ex)
        {
        }
    }


    private void GetCustomerName()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        lblCustomerName.Text = objCust.last_name1;
    }

  

    protected void BindSiteReviewDetails(int nPageNo)
    {
        try
        {
            if (Session["nSiteReviewList"]!= null)
            {
                DataTable dtSiteReview = (DataTable)Session["nSiteReviewList"];

                grdSiteViewList.DataSource = dtSiteReview;
                grdSiteViewList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                grdSiteViewList.PageIndex = nPageNo;
                grdSiteViewList.DataKeyNames = new string[] { "SiteReviewsId", "customer_id", "estimate_id", "SiteReviewsDate", "IsCustomerView", "SiteReviewsNotes", "StateOfMindID" };
                grdSiteViewList.DataBind();
            }
            else
            {
                grdSiteViewList.DataSource = null;
                grdSiteViewList.DataBind();
            }


            lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
            if (nPageNo == 0)
            {
                btnPrevious.Enabled = false;
                btnPrevious0.Enabled = false;
            }
            else
            {
                btnPrevious.Enabled = true;
                btnPrevious0.Enabled = true;
            }

            if (grdSiteViewList.PageCount == nPageNo + 1)
            {
                btnNext.Enabled = false;
                btnNext0.Enabled = false;
            }
            else
            {
                btnNext.Enabled = true;
                btnNext0.Enabled = true;
            }
        }
        catch( Exception ex)
        {
        }
       
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSiteReviewDetails(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtStartDate.Text = "";
        lblResult.Text = "";
        txtEndDate.Text = "";
        GetSiteReviews();
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindSiteReviewDetails(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindSiteReviewDetails(nCurrentPage - 2);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        lblResult.Text = "";
        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (txtStartDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Start Date is a required field");

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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Start Date");

                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("End Date is a required field");

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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid End Date");

                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");

            return;
        }

        GetSiteReviews();
    }

    private void GetSearchSiteReview(int nPageNo)
    {
        //DataClassesDataContext _db = new DataClassesDataContext();
        //lblResult.Text = "";


        //string strCondition = "";
        //DateTime strStartDate = DateTime.Now;
        //DateTime strEndDate = DateTime.Now;
        //if (txtStartDate.Text == "")
        //{
        //    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Start Date is a required field");

        //    return;
        //}
        //else
        //{
        //    try
        //    {
        //        Convert.ToDateTime(txtStartDate.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Start Date");

        //        return;
        //    }
        //    strStartDate = Convert.ToDateTime(txtStartDate.Text);
        //}

        //if (txtEndDate.Text == "")
        //{
        //    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("End Date is a required field");

        //    return;
        //}
        //else
        //{
        //    try
        //    {
        //        Convert.ToDateTime(txtEndDate.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid End Date");

        //        return;
        //    }
        //    strEndDate = Convert.ToDateTime(txtEndDate.Text);
        //}
        //if (strStartDate > strEndDate)
        //{
        //    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");

        //    return;
        //}

        //if (txtStartDate.Text != "" && txtEndDate.Text != "")
        //{
        //    strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND SiteReviewsDate>='" + strStartDate + "' AND  SiteReviewsDate<'" + strEndDate.AddDays(1).ToString() + "' ";
        //}
        //else
        //{
        //    strCondition = "WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        //}


        //string strQ = string.Empty;
        //strQ = " select [SiteReviewsId] ,[customer_id],[estimate_id],[SiteReviewsNotes],[SiteReviewsDate],[StateOfMindID], " +
        //       " [IsUserView],[IsCustomerView] ,[IsVendorView],[HasAttachments] ,[AttachmentList] ,[CreatedBy] ,[CreateDate] ,[LastUpdatedBy],[LastUpdateDate] " +
        //       " from [SiteReviewNotes] " + strCondition + " order by CreateDate DESC";

        //IEnumerable<csSiteReview> mList = _db.ExecuteQuery<csSiteReview>(strQ, string.Empty).ToList();

        //if (mList.Count() == 0)
        //{
        //    lblResult.Text = csCommonUtility.GetSystemErrorMessage("No data exist.");
        //    grdSiteViewList.DataSource = null;
        //    grdSiteViewList.DataBind();
        //    return;
        //}
        //grdSiteViewList.DataSource = mList;
        //grdSiteViewList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        //grdSiteViewList.PageIndex = nPageNo;
        //grdSiteViewList.DataKeyNames = new string[] { "SiteReviewsId", "customer_id", "estimate_id", "SiteReviewsDate", "IsCustomerView", "StateOfMindID" };
        //grdSiteViewList.DataBind();



    }

    protected void grdSiteViewList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        BindSiteReviewDetails(e.NewPageIndex);
    }
    protected void grdSiteViewList_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int SiteReviewsId = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int estimate_id = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[2].ToString());
            DateTime SiteReviewsDate = Convert.ToDateTime(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[3]);
            Boolean IsCustomer = Convert.ToBoolean(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[4]);
            string SiteReviewNote = grdSiteViewList.DataKeys[e.Row.RowIndex].Values[5].ToString();
            int StatetOfMind = Convert.ToInt32(grdSiteViewList.DataKeys[e.Row.RowIndex].Values[6].ToString());

            Image imgStateOfMind = (Image)e.Row.FindControl("imgStateOfMind");
            if (StatetOfMind == 0)
            {
                imgStateOfMind.Visible = false;
            }
            else
            {
                if (StatetOfMind == 1)
                {
                    imgStateOfMind.ImageUrl = "~/assets/customerstatemind/angry.png";
                    imgStateOfMind.ToolTip = "Angry";
                }
                else if (StatetOfMind == 2)
                {
                    imgStateOfMind.ImageUrl = "~/assets/customerstatemind/frustrated.png";
                    imgStateOfMind.ToolTip = "Frustrated";
                }
                else if (StatetOfMind == 3)
                {
                    imgStateOfMind.ImageUrl = "~/assets/customerstatemind/confused.png";
                    imgStateOfMind.ToolTip = "Confused";
                }
                else if (StatetOfMind == 4)
                {
                    imgStateOfMind.ImageUrl = "~/assets/customerstatemind/indifferent.png";
                    imgStateOfMind.ToolTip = "Indifferent";
                }
                else 
                {
                    imgStateOfMind.ImageUrl = "~/assets/customerstatemind/happy.png";
                    imgStateOfMind.ToolTip = "Happy";
                }
            }


            Label lblSiteReviewNote = (Label)e.Row.FindControl("lblSiteReviewNote");

            Label lblNotesLabel = (Label)e.Row.FindControl("lblNotesLabel");
            if (SiteReviewNote != null&&SiteReviewNote!="")
                lblNotesLabel.Text = "Notes:";
            else
            {
                lblNotesLabel.Visible = false;
                lblNotesLabel.Text = "";
            }
            lblSiteReviewNote.Text = SiteReviewNote;

            HyperLink hypSiteReview = (HyperLink)e.Row.FindControl("hypSiteReview");
            hypSiteReview.Text = SiteReviewsDate.ToString("MM-dd-yyyy");
            hypSiteReview.NavigateUrl = "sitereviewdetails.aspx?sid=" + SiteReviewsId + "&cid=" + customer_id + "&nbackid=" + hdnBackId.Value + "&nestid=" + estimate_id;
           


            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            imgDelete.OnClientClick = "return confirm('Are you sure you want to delete site review?');";
            imgDelete.CommandArgument = SiteReviewsId.ToString();

            if (SiteReviewsId != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, SiteReviewsId);
            }

            if (IsCustomer == true)
            {
                e.Row.Cells[3].Text = "Yes";
            }
            else
            {
                e.Row.Cells[3].Text = "";
            }
        }
    }

    
    private void DeleteTemporaryFiles()
    {
        string DestinationPath = Server.MapPath("~/Uploads//Temp//");
        if (Directory.Exists(DestinationPath))
        {
            string[] fileEntries = Directory.GetFiles(DestinationPath);
            foreach (string file in fileEntries)
            {
                File.Delete(file);
            }

        }
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddNew.ID, btnAddNew.GetType().Name, "Click"); 
        DeleteTemporaryFiles();
        Response.Redirect("sitereviewdetails.aspx?cid=" + hdnCustomerId.Value +"&nbackid="+ hdnBackId.Value+"&nestid=" + hdnEstimateId.Value, false);

    }

    


    protected void DeleteFile(object sender, EventArgs e)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";

            ImageButton imgDelete = (ImageButton)sender;
            int nSiteReviewId = Convert.ToInt32(imgDelete.CommandArgument);


            strQ = "delete from SiteReviewNotes where SiteReviewsId=" + nSiteReviewId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);



            string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//";

            string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//";


            var upList = (from upl in _db.SiteReview_upolad_infos
                          where upl.SiteReviewsId == nSiteReviewId
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

            strQ = "delete from SiteReview_upolad_info where SiteReviewsId=" + Convert.ToInt32(nSiteReviewId) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            GetSiteReviews();
        }
        catch (Exception ex)
        {
        }


    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgBack.ID, imgBack.GetType().Name, "Click"); 
        if (Convert.ToInt32(hdnBackId.Value)==1)
            Response.Redirect("customerlist.aspx");
        else if (Convert.ToInt32(hdnBackId.Value) == 2)
            Response.Redirect("generalsitereview.aspx");
        else
            Response.Redirect("leadlist.aspx");
    }


    protected void grdUploadedFileList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdUploadedFileList = (GridView)sender;
           
                  
                 
                try
                {
                    Label lblImages = (Label)e.Row.Parent.Parent.Parent.FindControl("lblImages");
                   
                    
                    int SiteReview_attach_id = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0].ToString());
                    int SiteReviewsId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString());
                    int customer_id = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[2].ToString());
                    int estimate_id = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString());
                    string SiteReview_file_name = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[4].ToString();

                    if (SiteReview_file_name != null && SiteReview_file_name!="")
                    {
                        lblImages.Text = "Files:";
                    }
                    else
                        lblImages.Visible = false;

                    //string fileName = SiteReview_file_name.Substring(0, 10);
                    //Label lblFileName = (Label)e.Row.FindControl("lblFileName");
                    //lblFileName.Text = fileName;


                    if (SiteReview_file_name.Contains(".jpg") || SiteReview_file_name.Contains(".jpeg") || SiteReview_file_name.Contains(".png") || SiteReview_file_name.Contains(".gif"))
                    {
                        HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                        hypImg.Visible = true;

                        //hypImg.NavigateUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name;
                        //hypImg.Target = "_blank";
                        hypImg.Attributes.Add("onclick", "window.open('generalsitereview_image_gallery.aspx?gsid=" + SiteReviewsId + "&cid=" + customer_id + "', 'MyWindow', 'left=150,top=100,width=900,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');");
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
                       // img.Attributes.Add("data-zoom-image", "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name);
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

  

    private void GetUploadedFileListData(GridView gvUp, int nSiteReviewId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
           
            DeleteTemporaryFiles();
            SiteReview_upolad_info objSRU = new SiteReview_upolad_info();

           // SiteReviewNote objSRN = _db.SiteReviewNotes.SingleOrDefault(sr => sr.SiteReviewsId == nSiteReviewId);
           
                var attchmentList = from su in _db.SiteReview_upolad_infos
                                    where su.SiteReviewsId == nSiteReviewId
                                    orderby su.SiteReview_attach_id ascending
                                    select su;



                gvUp.DataSource = attchmentList;
                gvUp.DataKeyNames = new string[] { "SiteReview_attach_id", "SiteReviewsId", "customer_id", "estimate_id", "SiteReview_file_name" };
                gvUp.DataBind();


                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalAttachmentView();", true);
            
        }
        catch (Exception ex)
        {

        }
    }
     
}