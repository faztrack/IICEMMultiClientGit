using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mselectionlist : System.Web.UI.Page
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
                hdnCustomerID.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
                hdnEstimateID.Value = Convert.ToInt32(Request.QueryString.Get("nestid")).ToString();
               
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerID.Value));
                lblCustomerLastName.Text = "(" + objCust.last_name1 + ", " + GetJobNumber(Convert.ToInt32(hdnCustomerID.Value), Convert.ToInt32(hdnEstimateID.Value)) + ")";
                GetSectionSelectionListData();
            }
        }
    
    }

    private string GetJobNumber(int nCustId, int nEstId)
    {
        string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number ?? "";
        if (strJobNumber == "")
        {
            strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).job_number ?? "";
        }
        return strJobNumber;
    }
    private void GetSectionSelectionListData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nCustId = Convert.ToInt32(hdnCustomerID.Value);
        int nEstId = Convert.ToInt32(hdnEstimateID.Value);
       
       var sslist = _db.Section_Selections.Where(ss => ss.customer_id == nCustId && ss.estimate_id == nEstId && ss.isSelected == true).ToList().OrderByDescending(c => c.CreateDate);
       grdSelection.DataSource = sslist;
       grdSelection.DataKeyNames = new string[] { "SectionSelectionID", "customer_id", "estimate_id", "CreateDate", "section_name", "location_name", "Title", "Price", "Notes" };
       grdSelection.DataBind();
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

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
       Response.Redirect("mlandingpage.aspx");

    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton btnsubmit = sender as LinkButton;

            GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
            Label lblCallDescriptionG = gRow.Cells[0].Controls[0].FindControl("lblNotes_r") as Label;
            Label lblCallDescriptionG_r = gRow.Cells[0].Controls[7].FindControl("lblNotes_r") as Label;
            LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpen") as LinkButton;

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
        catch(Exception ex)
        {
        }
    }

    protected void grdSelection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[1].ToString());
            DateTime date = Convert.ToDateTime(grdSelection.DataKeys[e.Row.RowIndex].Values[3]);
            string section_name = grdSelection.DataKeys[e.Row.RowIndex].Values[4].ToString();
            string location_name = grdSelection.DataKeys[e.Row.RowIndex].Values[5].ToString();
            string Title = grdSelection.DataKeys[e.Row.RowIndex].Values[6].ToString();
            decimal Price = Convert.ToDecimal(grdSelection.DataKeys[e.Row.RowIndex].Values[7]); ;
            string Notes = grdSelection.DataKeys[e.Row.RowIndex].Values[8].ToString();
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            lblDate.Text = "<font style='font-weight:bold'>Date: </font>" + date.ToString("MM-dd-yyyy");
            Label lblSection = (Label)e.Row.FindControl("lblSection");
            lblSection.Text = "<font style='font-weight:bold'>Section: </font>" + section_name;
            Label lblLocation = (Label)e.Row.FindControl("lblLocation");
            lblLocation.Text = "<font style='font-weight:bold'>Location: </font>" + location_name;
            Label lblTitle = (Label)e.Row.FindControl("lblTitle");
            lblTitle.Text = "<font style='font-weight:bold'>Title: </font>" + Title;
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            if (Notes != "" && Notes.Length > 60)
            {

                lblNotes.Text = "<font style='font-weight:bold'>Notes: </font>" + Notes.Substring(0, 60) + "...";
                lblNotes.ToolTip = Notes;
                lnkOpen.Visible = true;

            }
            else
            {

                lblNotes.Text = "<font style='font-weight:bold'>Notes: </font>" + Notes;
                lblNotes.ToolTip = Notes;
                lnkOpen.Visible = false;

            }


            Label lblPrice = (Label)e.Row.FindControl("lblPrice");
            lblPrice.Text = "<font style='font-weight:bold'>Price: </font>" + Price.ToString("c");

           if (nSectionSelectionID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nSectionSelectionID);
            }

         
           

           
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
            int nUploadFileId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            int customer_id = Convert.ToInt32(hdnCustomerID.Value);


            string file_name = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString();
           

            if (file_name.Contains(".jpg") || file_name.Contains(".jpeg") || file_name.Contains(".png") || file_name.Contains(".gif"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = true;

                hypImg.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypPDF.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypDoc.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypExcel.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypTXT.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                hypTXT.Target = "_blank";
                
            }
        }
    }
}