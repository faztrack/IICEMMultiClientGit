using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Material_Tracking_List : System.Web.UI.Page
{
    int nPageNo = 0;
    public DataTable dtVendor;
    int countRow = 0;
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
                userinfo oUser = Session["oUser"] as userinfo;
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            if (Page.User.IsInRole("gem01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            lblHeaderTitle.Text = "Material Tracking List";
            countRow = 0;

            BindDivision();
            LoadVendor();

            if (Session["htMaterialTrackingList"] != null)
            {
                Hashtable ht = (Hashtable)Session["htMaterialTrackingList"];
                ddlItemPerPage.SelectedIndex = Convert.ToInt32(ht["sItemPerPage"].ToString());
                nPageNo = Convert.ToInt32(ht["sPageNo"].ToString());
                txtStartDate.Text = ht["sStartDate"].ToString();
                txtEndDate.Text = ht["sEndDate"].ToString();
              // BindOrderList(nPageNo);
                GetOrderList();

            }
            else
            {

                txtStartDate.Text = DateTime.Now.AddDays(-360).ToShortDateString();
                txtEndDate.Text = DateTime.Now.AddDays(1).ToShortDateString();

                GetOrderList();
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
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindOrderList(nCurrentPage - 2);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetOrderList();

        #region comment
        //lblResult.Text = "";
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

        //string strCondition = "";





        //if (txtSearch.Text.Trim() != "")
        //{



        //    string str = txtSearch.Text.Trim();

        //    if (ddlSearchBy.SelectedValue == "1")
        //    {
        //        strCondition = " And b.first_name1 LIKE '%" + str.Replace("'", "''") + "%'";
        //    }
        //    else if (ddlSearchBy.SelectedValue == "2")
        //    {
        //        strCondition = " And  b.last_name1 LIKE '%" + str.Replace("'", "''") + "%'";
        //    }
        //    else if (ddlSearchBy.SelectedValue == "3")
        //    {

        //        strCondition = " And  b.email LIKE '%" + str + "%'";
        //    }
        //    else if (ddlSearchBy.SelectedValue == "4")
        //    {
        //        strCondition = " And  b.address LIKE '%" + str.Replace("'", "''") + "%'";
        //    }
        //}
        //if (ddlVendor.SelectedValue != "0")
        //{
        //    strCondition += " And  Vendor_id=" + Convert.ToInt32(ddlVendor.SelectedValue);
        //}
        //if (chkShipped.Checked == true)
        //{
        //    strCondition += " And Is_Shipped=1 ";
        //}
        //if (chkReceived.Checked == true)
        //{
        //    strCondition += " And Is_Received=1 ";
        //}
        //if (chkPicked.Checked == true)
        //{
        //    strCondition += " And Is_Picked=1 ";
        //}
        //if (chkConfirmed.Checked == true)
        //{
        //    strCondition += " And Is_Confirmed=1 ";
        //}
        //strCondition += " AND  a.Order_date>='" + strStartDate + "' AND  a.Order_date<'" + strEndDate + "' ";


        //// GetOrderList();
        //String strQ =
        //        " Select a.Order_id,a.Order_date,a.Customer_id,a.Estimate_id,a.Section_id,a.Section_name,a.Vendor_id,a.Vendor_name,a.Item_text,a.Item_note,Shipped_date,Shipped_by,Shipped_note," +
        //        " a.Received_date,a.Received_by,a.Received_note,a.Picked_date,a.Picked_by,a.Picked_note,a.Confirmed_date,a.Confirmed_by,a.Confirmed_note,a.Is_Shipped," +
        //        " a.Is_Received,a.Is_Picked,a.Is_Confirmed,b.first_name1,b.last_name1" +
        //        " from Material_Traking_Order AS a Inner join customers AS b ON a.Customer_id = b.customer_id where a.Is_Active=1 " + strCondition + " order by a.Order_date DESC";

        //DataTable dt = csCommonUtility.GetDataTable(strQ);
        //countRow = dt.Rows.Count;
        //lblcount.Text = countRow.ToString();
        //grdSelection.DataSource = dt;
        //grdSelection.DataKeyNames = new string[] { "Order_id", "Customer_id", "Section_id", "Estimate_id", "Vendor_id", "Is_Shipped", "Is_Received", "Is_Picked", "Is_Confirmed" };
        //grdSelection.DataBind();

        #endregion 

    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {

        if (Session["htMaterialTrackingList"] != null)
        {
            Session.Remove("htMaterialTrackingList");
            Session.Remove("mCustomerSerch");
            Session.Remove("nMaterilaList");

        }
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        ddlItemPerPage.SelectedIndex = 0;
        txtStartDate.Text = DateTime.Now.AddDays(-360).ToShortDateString();
        txtEndDate.Text = DateTime.Now.AddDays(1).ToShortDateString();
        lblResult.Text = "";
        txtSearch.Text = "";
        ddlVendor.SelectedValue = "0";
        chkConfirmed.Checked = false;
        chkPicked.Checked = false;
        chkReceived.Checked = false;
        chkShipped.Checked = false;
        GetOrderList();
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        BindOrderList(0);
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        BindOrderList(nCurrentPage);
    }

    protected void grdSelection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nOrderID = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[1].ToString());

            int estimate_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[3].ToString());

            string nClientId = grdSelection.DataKeys[e.Row.RowIndex].Values[9].ToString();
            Label lblDivisionName = e.Row.FindControl("lblDivisionName") as Label;
            lblDivisionName.Text = csCommonUtility.GetDivisionName(nClientId);

            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");



            HyperLink hyp_Custd = (HyperLink)e.Row.FindControl("hyp_Custd");
            hyp_Custd.NavigateUrl = "material_traknig.aspx?eid=" + estimate_id + "&cid=" + customer_id;

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

            //chkSelected.Checked = isSelected;


            if (nOrderID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nOrderID);
            }


        }

    }

    protected void grdSelection_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdSelection.ID, grdSelection.GetType().Name, "PageIndexChanging"); 
        BindOrderList(e.NewPageIndex);
    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[5].Controls[0].FindControl("lblNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[5].Controls[1].FindControl("lblNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[5].Controls[2].FindControl("lnkOpen") as LinkButton;

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

                hypImg.NavigateUrl = "File/" + customer_id + "/MATERIAL/" + file_name;
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

                img.ImageUrl = "File/" + customer_id + "/MATERIAL/Thumbnail/" + file_name;
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
                hypPDF.NavigateUrl = "File/" + customer_id + "/MATERIAL/" + file_name;
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
                hypDoc.NavigateUrl = "File/" + customer_id + "/MATERIAL/" + file_name;
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
                hypExcel.NavigateUrl = "File/" + customer_id + "/MATERIAL/" + file_name;
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
                hypTXT.NavigateUrl = "File/" + customer_id + "/MATERIAL/" + file_name;
                hypTXT.Target = "_blank";

            }
        }
    }

    protected void BindOrderList(int nPageNo)
    {
        try
        {
            if (Session["nMaterilaList"] != null)
            {
                DataTable dtSiteReview = (DataTable)Session["nMaterilaList"];

                countRow = dtSiteReview.Rows.Count;
                lblcount.Text = countRow.ToString();

                grdSelection.DataSource = dtSiteReview;
                grdSelection.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                grdSelection.PageIndex = nPageNo;
                grdSelection.DataKeyNames = new string[] { "Order_id", "Customer_id", "Section_id", "Estimate_id", "Vendor_id", "Is_Shipped", "Is_Received", "Is_Picked", "Is_Confirmed", "clientID" };
                grdSelection.DataBind();

                Hashtable ht = new Hashtable();
                ht.Add("sItemPerPage", ddlItemPerPage.SelectedIndex);
                ht.Add("sPageNo", nPageNo);
                ht.Add("sStartDate", txtStartDate.Text);
                ht.Add("sEndDate", txtEndDate.Text);

                Session["htMaterialTrackingList"] = ht;
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

    private void GetOrderList()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            lblResult.Text = "";
            string strQ = string.Empty;
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

            string strCondition = "";





            if (txtSearch.Text.Trim() != "")
            {



                string str = txtSearch.Text.Trim();

                if (ddlSearchBy.SelectedValue == "1")
                {
                    strCondition = " And b.first_name1 LIKE '%" + str.Replace("'", "''") + "%'";
                }
                else if (ddlSearchBy.SelectedValue == "2")
                {
                    strCondition = " And  b.last_name1 LIKE '%" + str.Replace("'", "''") + "%'";
                }
                else if (ddlSearchBy.SelectedValue == "3")
                {

                    strCondition = " And  b.email LIKE '%" + str + "%'";
                }
                else if (ddlSearchBy.SelectedValue == "4")
                {
                    strCondition = " And  b.address LIKE '%" + str.Replace("'", "''") + "%'";
                }
            }
            if (ddlVendor.SelectedValue != "0")
            {
                strCondition += " And  Vendor_id=" + Convert.ToInt32(ddlVendor.SelectedValue);
            }
            if (chkShipped.Checked == true)
            {
                strCondition += " And Is_Shipped=1 ";
            }
            if (chkReceived.Checked == true)
            {
                strCondition += " And Is_Received=1 ";
            }
            if (chkPicked.Checked == true)
            {
                strCondition += " And Is_Picked=1 ";
            }
            if (chkConfirmed.Checked == true)
            {
                strCondition += " And Is_Confirmed=1 ";
            }
            else
            {
                strCondition += " And Is_Confirmed<>1 ";
            }
            strCondition += " AND  a.Order_date>='" + strStartDate + "' AND  a.Order_date<'" + strEndDate + "' ";

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 2)
                    strCondition += " AND a.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue);
                else
                    strCondition = " WHERE a.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue);
            }

            strQ =
                " Select a.Order_id,a.Order_date,a.Customer_id,a.Estimate_id,a.Section_id, a.client_id as clientID,  a.Section_name,a.Vendor_id,a.Vendor_name,a.Item_text,a.Item_note,Shipped_date,Shipped_by,Shipped_note," +
                " a.Received_date,a.Received_by,a.Received_note,a.Picked_date,a.Picked_by,a.Picked_note,a.Confirmed_date,a.Confirmed_by,a.Confirmed_note,a.Is_Shipped," +
                " a.Is_Received,a.Is_Picked,a.Is_Confirmed,b.first_name1,b.last_name1" +
                " from Material_Traking_Order AS a Inner join customers AS b ON a.Customer_id = b.customer_id where a.Is_Active=1 " + strCondition + " order by a.Order_date DESC";

            //List<MetarialTrackingListViewModel> mList = _db.ExecuteQuery<MetarialTrackingListViewModel>(strQ, string.Empty).ToList();
            //DataTable dt = DataReader.Complex_Read_DataTable(strQ);

            DataTable dt = csCommonUtility.GetDataTable(strQ);

            if (dt.Rows.Count > 0)
                Session.Add("mCustomerSerch", dt);

            if (dt.Rows.Count > 0)
            {
                Session.Add("nMaterilaList", dt);
                countRow = dt.Rows.Count;
                lblcount.Text = countRow.ToString();
            }
            else
            {
                Session.Remove("nMaterilaList");
            }

            if (Session["htMaterialTrackingList"] != null)
            {
                Hashtable ht = (Hashtable)Session["htMaterialTrackingList"];
                nPageNo = Convert.ToInt32(ht["sPageNo"].ToString());
                BindOrderList(nPageNo);
            }
            else
            {
                BindOrderList(0);
            }


        }
        catch (Exception ex)
        {
        }
    }

    private void GetUploadedFileListData(GridView grd, int nSectionSelectionID)
    {
        DataClassesDataContext _db = new DataClassesDataContext();



        var objfui = from fui in _db.file_upload_infos
                     where fui.vendor_cost_id == nSectionSelectionID && fui.type == 2
                     orderby fui.upload_fileId ascending
                     select fui;
        if (objfui != null)
        {
            grd.DataSource = objfui;
            grd.DataKeyNames = new string[] { "upload_fileId", "vendor_cost_id", "estimate_id", "ImageName", "CustomerId" };
            grd.DataBind();
        }
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetAddress";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }
        //GetCustomersNew(0);
    }

    private void LoadVendor()
    {

        string sSQL = string.Empty;



        sSQL = "select distinct Vendor_id,Vendor_name from Material_Traking_Order where Is_Active = 1";



        dtVendor = DataReader.Complex_Read_DataTable(sSQL);
        DataRow newRow = dtVendor.NewRow();
        newRow[0] = 0;
        newRow[1] = "Select one";
        dtVendor.Rows.InsertAt(newRow, 0);

        ddlVendor.DataSource = dtVendor; //tmpSTable;
        ddlVendor.DataValueField = "Vendor_id";
        ddlVendor.DataTextField = "Vendor_name";
        ddlVendor.DataBind();


    }

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["mCustomerSerch"] != null)
        {
            List<MetarialTrackingListViewModel> cList = (List<MetarialTrackingListViewModel>)HttpContext.Current.Session["mCustomerSerch"];
            return (from c in cList
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ =
                  " Select a.Order_id,a.Order_date,a.Customer_id,a.Estimate_id,a.Section_id,a.Section_name,a.Vendor_id,a.Vendor_name,a.Item_text,a.Item_note,Shipped_date,Shipped_by,Shipped_note," +
                  " a.Received_date,a.Received_by,a.Received_note,a.Picked_date,a.Picked_by,a.Picked_note,a.Confirmed_date,a.Confirmed_by,a.Confirmed_note,a.Is_Shipped," +
                  " a.Is_Received,a.Is_Picked,a.Is_Confirmed,b.first_name1,b.last_name1" +
                  " from Material_Traking_Order AS a Inner join customers AS b ON a.Customer_id = b.customer_id where a.Is_Active=1  order by b.first_name1 DESC";

            List<MetarialTrackingListViewModel> cList = _db.ExecuteQuery<MetarialTrackingListViewModel>(strQ, string.Empty).ToList();

            return (from c in cList
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }

    protected void chkShipped_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkShipped.ID, chkShipped.GetType().Name, "CheckedChanged"); 
        GetOrderList();
    }
    protected void chkReceived_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkReceived.ID, chkReceived.GetType().Name, "CheckedChanged"); 
        GetOrderList();
    }
    protected void chkPicked_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkPicked.ID, chkPicked.GetType().Name, "CheckedChanged");
        GetOrderList();
    }
    protected void chkConfirmed_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkConfirmed.ID, chkConfirmed.GetType().Name, "CheckedChanged");
        GetOrderList();
    }
    protected void ddlVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlVendor.ID, ddlVendor.GetType().Name, "CheckedChanged");
        GetOrderList();
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetOrderList();
        BindOrderList(0);
    }
}