using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GeneralSelectionReview : System.Web.UI.Page
{
    int nPageNo = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            string divisionName = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            if (Page.User.IsInRole("Gls01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            lblHeaderTitle.Text = "General Selection Review";

            BindDivision();

            if (Session["SelectionFilter"] != null)
            {
                Hashtable ht = (Hashtable)Session["SelectionFilter"];
                ddlItemPerPage.SelectedIndex = Convert.ToInt32(ht["sItemPerPage"].ToString());
                nPageNo = Convert.ToInt32(ht["sPageNo"].ToString());
                txtStartDate.Text = ht["sStartDate"].ToString();
                txtEndDate.Text = ht["sEndDate"].ToString();
                BindSectionSelectionListDetails(nPageNo);
            }
            else
            {

                txtStartDate.Text = DateTime.Now.AddDays(-2).ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                GetSectionSelections();
                //BindSectionSelectionListDetails(0);
            }


            if (divisionName != "" && divisionName.Contains(","))
            {
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.Enabled = false;
            }




        }
    }

    private void BindDivision()
    {
        string sql = "select id, division_name from division order by division_name ";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        ddlDivision.DataSource = dt;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
        ddlDivision.Items.Insert(0, "All");
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

    }

    private void GetSectionSelections()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = "";
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition = "WHERE  Section_Selection.CreateDate>='" + strStartDate + "' AND  Section_Selection.CreateDate<'" + strEndDate.AddDays(1).ToString() + "' ";
            }

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 2)
                    strCondition += " AND Section_Selection.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue);
                else
                    strCondition = " WHERE Section_Selection.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue);
            }

            string strQ = string.Empty;
            strQ = " select customers.last_name1+', '+first_name1 as customer_name,SectionSelectionID, Section_Selection.client_id as clientID, customers.customer_id, estimate_id, section_id, section_name, location_id, location_name, Title, Price, Section_Selection.Notes, LastUpdateDate, UpdateBy, isSelected, Section_Selection.customer_signature, customer_siignatureDate, customer_signedBy, Section_Selection.CreateDate, ValidTillDate, Section_Selection.CreatedBy, UserEmail" +
                " from Section_Selection "+
                    " INNER JOIN customers on customers.customer_id = Section_Selection.customer_id " + strCondition + " order by Section_Selection.CreateDate DESC";

            DataTable dt = csCommonUtility.GetDataTable(strQ);
            if (dt.Rows.Count > 0)
                Session.Add("nGSectionSelectionList", dt);
            else
                Session.Remove("nGSectionSelectionList");

            if (Session["SelectionFilter"] != null)
            {
                Hashtable ht = (Hashtable)Session["SelectionFilter"];
                nPageNo = Convert.ToInt32(ht["sPageNo"].ToString());
                BindSectionSelectionListDetails(nPageNo);
            }
            else
            {
                BindSectionSelectionListDetails(0);
            }


        }
        catch (Exception ex)
        {
        }
    }

    protected void BindSectionSelectionListDetails(int nPageNo)
    {
        try
        {
            if (Session["nGSectionSelectionList"] != null)
            {
                DataTable dtSiteReview = (DataTable)Session["nGSectionSelectionList"];

                grdSelection.DataSource = dtSiteReview;
                grdSelection.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                grdSelection.PageIndex = nPageNo;
                grdSelection.DataKeyNames = new string[] { "SectionSelectionID", "customer_id", "section_id", "estimate_id", "location_id", "isSelected", "customer_signature", "customer_siignatureDate", "CreateDate", "ValidTillDate", "clientID" };
                grdSelection.DataBind();

                Hashtable ht = new Hashtable();
                ht.Add("sItemPerPage", ddlItemPerPage.SelectedIndex);
                ht.Add("sPageNo", nPageNo);
                ht.Add("sStartDate", txtStartDate.Text);
                ht.Add("sEndDate", txtEndDate.Text);

                Session["SelectionFilter"] = ht;
            }
            else
            {
                grdSelection.DataSource = null;
                grdSelection.DataBind();
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

            if (grdSelection.PageCount == nPageNo + 1)
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
        catch (Exception ex)
        {
        }

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
            string nClientId = grdSelection.DataKeys[e.Row.RowIndex].Values[10].ToString();
            Label lblDivisionName = e.Row.FindControl("lblDivisionName") as Label;
            lblDivisionName.Text = csCommonUtility.GetDivisionName(nClientId);

            TimeSpan ts = dtValidTill - dtCreateDate;
            int nDay = ts.Days;
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");

            CheckBox chkSelected = e.Row.FindControl("chkSelected") as CheckBox;
            Label lblSelected = e.Row.FindControl("lblSelected") as Label;
            Image imgCustomerSign = e.Row.FindControl("imgCustomerSign") as Image;
            Label lblSelectionDate = (Label)e.Row.FindControl("lblSelectionDate");
            //Label lblDayLeft = e.Row.FindControl("lblDayLeft") as Label;

            HyperLink hyp_Custd = (HyperLink)e.Row.FindControl("hyp_Custd");
            hyp_Custd.NavigateUrl = "selectionofsection.aspx?eid=" + estimate_id + "&nbackid=0&cid=" + customer_id;

          //  lblDayLeft.Text = nDay.ToString() + " Day(s)";

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
                //lblDayLeft.Visible = false;
                lblSelected.Visible = true;
                chkSelected.Visible = false;
                lblSelected.Text = "Selected";
                lblSelectionDate.Text = "Date: " + strSignatureDate;
            }
            else
            {

                lblSelected.Visible = false;
                customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == customer_id);

                if (objCust == null || objCust.customer_signature == null || objCust.customer_signature == "")
                {
                    chkSelected.Visible = false;
                }
                else
                {
                    chkSelected.Visible = true;
                }
                lblSelected.Text = "";
                lblSelected.Visible = true;
                chkSelected.Visible = false;
            }
            //if (nDay < 0)
            //{
            //    //chkSelected.Checked = false;
            //    chkSelected.Visible = false;

            //}

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
            grd.DataKeyNames = new string[] { "upload_fileId", "vendor_cost_id", "estimate_id", "ImageName", "CustomerId" };
            grd.DataBind();
        }
    }

    protected void grdUploadedFileList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdUploadedFileList = (GridView)sender;



            int nUploadFileId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            int customer_id = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[4]);


            string file_name = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString();


            string fileName = file_name.Substring(0, 10);
            Label lblFileName = (Label)e.Row.FindControl("lblFileName");
            lblFileName.Text = fileName;




            if (file_name.Contains(".jpg") || file_name.Contains(".jpeg") || file_name.Contains(".png") || file_name.Contains(".gif"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = true;

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
                hypTXT.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypTXT.Target = "_blank";

            }
        }
    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[5].Controls[0].FindControl("lblNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[5].Controls[1].FindControl("lblNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[5].Controls[3].FindControl("lnkOpen") as LinkButton;

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



    protected void btnBack_Click(object sender, EventArgs e)
    {

    }
   

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        BindSectionSelectionListDetails(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {

        if (Session["SelectionFilter"] != null)
        {
            Session.Remove("SelectionFilter");


        }
        ddlItemPerPage.SelectedIndex = 0;
        txtStartDate.Text = DateTime.Now.AddDays(-2).ToShortDateString();
        txtEndDate.Text = DateTime.Now.ToShortDateString();
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        lblResult.Text = "";
        GetSectionSelections();
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindSectionSelectionListDetails(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindSectionSelectionListDetails(nCurrentPage - 2);
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

        GetSectionSelections();
    }
   
    protected void grdSelection_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdSelection.ID, grdSelection.GetType().Name, "PageIndexChanging"); 
        BindSectionSelectionListDetails(e.NewPageIndex);
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetSectionSelections();
    }
}