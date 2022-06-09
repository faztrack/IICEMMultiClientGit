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

public partial class material_traknig : System.Web.UI.Page
{
    public DataTable dtSection;
    //public int CustomerId = 0;
    //public int EstimateId=0;
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

            if (Page.User.IsInRole("mat01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            //------------------------------------------------------
            DataClassesDataContext _db = new DataClassesDataContext();
            int nCustomerID = Convert.ToInt32(Request.QueryString.Get("cid"));

            customer objCust = new customer();
            string strCustName = "";
            if (_db.customers.Where(c => c.customer_id == nCustomerID).Count() > 0)
            {
                objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerID);
                strCustName = objCust.first_name1 + " " + objCust.last_name1;
                hdnClientId.Value = objCust.client_id.ToString();
            }

            lblHeaderTitle.Text = "Material Tracking (" + strCustName + ")";
            //---------------------------------------------

            if (Request.QueryString.Get("cid") != null)
            {
                hdnCustomerID.Value = Request.QueryString.Get("cid");
            }
            if (Request.QueryString.Get("eid") != null)
            {
                hdnEstimateID.Value = Request.QueryString.Get("eid");
            }
            BindEstimate();
            LoadSectionSec();
            LoadVendor();
            txtSelectionDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtShippedTrackingInfoDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtRecReceivedByDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtPickedReceivedByDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtConfimedByDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            hdnOrderId.Value = "";
            GetSectionSelectionListData(1); // row bind for order by, order date(1=orderdate )

            csCommonUtility.SetPagePermission(this.Page, new string[] { "ddlSection","ddlEst", "ddlVendor", "chkShipped", "chkReceived", "chkPicked", "chkConfirmed", "file_upload", "btnSave" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdMaterial_imgbtngrdUpload", "grdMaterial_imgEdit", "grdMaterial_imgDelete","grdfile_upload", "Delete" });
        }


    }

    private void BindEstimate()
    {
        try
        {

            int nCustId = Convert.ToInt32(hdnCustomerID.Value);
            string strQ = "select  estimate_id, customer_id,estimate_name,estimate_name " +
                          " from customer_estimate where customer_id=" + nCustId + " and client_id=" + Convert.ToInt32(hdnClientId.Value) +
                          " Order by convert(datetime,sale_date) desc";
            DataTable dt = csCommonUtility.GetDataTable(strQ);
            
            ddlEst.DataSource = dt;
            ddlEst.DataTextField = "estimate_name";
            ddlEst.DataValueField = "estimate_id";
            ddlEst.DataBind();
            ddlEst.SelectedValue = hdnEstimateID.Value;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void ddlEst_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hdnEstimateID.Value = ddlEst.SelectedValue;
            GetSectionSelectionListData(1);
            LoadSectionSec();
            

        }
        catch (Exception ex)
        {
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SaveData();
    }

    protected void btnCancel_click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCancel.ID, btnCancel.GetType().Name, "Click"); 
        resetsave();
        hdnOrderId.Value = "";
        lblResult2.Text = "";
        lblResult.Text = "";
        btnSave.Text = "save";
    }

    private void LoadSectionSec()
    {
        try
        {
            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
            string sSQL = string.Empty;
            sSQL = " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from " +
                   " pricing_details " +
                    " inner join sectioninfo on pricing_details.section_level=sectioninfo.section_id  and pricing_details.customer_id=" + CustomerId + " and pricing_details.estimate_id=" + EstimateId + "" +
                    " UNION " +
                    " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from " +
                   "  co_pricing_master" +
                   " inner join sectioninfo on co_pricing_master.section_level=sectioninfo.section_id " +
                   " where co_pricing_master.customer_id=" + CustomerId + " and co_pricing_master.estimate_id=" + EstimateId + " and parent_id=0 order by section_name ";


            dtSection = DataReader.Complex_Read_DataTable(sSQL);
            DataRow newRow = dtSection.NewRow();
            newRow[0] = "Select one";
            newRow[1] = 0;
            dtSection.Rows.InsertAt(newRow, 0);

            ddlSection.DataSource = dtSection; //tmpSTable;
            ddlSection.DataValueField = "section_id";
            ddlSection.DataTextField = "section_name";
            ddlSection.DataBind();

            Session.Add("matSection", dtSection);
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    private void LoadVendor()
    {
        try
        {
            string sSQL = "Select distinct a.Vendor_Id,b.vendor_name from vendor_section as a inner join Vendor AS b ON a.Vendor_Id=b.vendor_id  where b.is_active=1";
            ddlVendor.DataSource = DataReader.Complex_Read_DataTable(sSQL);
            ddlVendor.DataTextField = "vendor_name";
            ddlVendor.DataValueField = "Vendor_Id";
            ddlVendor.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void LoadVendorEdit(string selectionId, string vendorId)
    {
        try
        {
            string sSQL = "Select distinct a.Vendor_Id,b.vendor_name from vendor_section as a inner join Vendor AS b ON a.Vendor_Id=b.vendor_id  where b.is_active=1";
            ddlVendor.DataSource = DataReader.Complex_Read_DataTable(sSQL);
            ddlVendor.DataTextField = "vendor_name";
            ddlVendor.DataValueField = "Vendor_Id";
            ddlVendor.DataBind();
            ddlVendor.SelectedValue = vendorId;
            lblVendor.Text = ddlVendor.SelectedItem.Text;
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

      
    }
    protected void ddlSection_IndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSection.ID, ddlSection.GetType().Name, "IndexChanged"); 
        LoadVendor();
        

    }
    protected void btnSave_click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        SaveData();

    }
    protected void SaveData()
    {
        try
        {
            lblResult2.Text = "";
            lblResult.Text = "";
            string strFileExt = "";
            string strRequired = string.Empty;


            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
            DataClassesDataContext _db = new DataClassesDataContext();

            DateTime dtCreateDate = new DateTime();
            try
            {
                dtCreateDate = Convert.ToDateTime(txtSelectionDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                return;
            }

            if (txtTitle.Text.Trim().Length != 0)
            {
                if (txtTitle.Text.Trim().Length > 499)
                {
                    strRequired += "Item Name is Up to 500 Characters<br/>";
                }

            }
            else
            {
                strRequired += "Title is required<br/>";
            }
            if (txtNotes.Text.Trim().Length != 0)
            {

                if (txtNotes.Text.Trim().Length > 499)
                {
                    strRequired += "Item Notes is Up to 500 Characters<br/>";
                }

            }
            else
            {
                strRequired += "Notes is required<br/>";
            }
            if (Convert.ToInt32(ddlSection.SelectedItem.Value) == 0)
            {
                strRequired += "Please Select Section is required<br/>";
            }
            if (chkShipped.Checked)
            {
                if (txtShippedTrackingInfo.Text.Trim() == "")
                    strRequired += "Missing Shipped Tracking Info.<br/>";

            }
            if (chkReceived.Checked)
            {
                if (txtRecReceivedBy.Text.Trim() == "")
                    strRequired += "Missing ReceivedBy Info.<br/>";
            }
            if (chkPicked.Checked)
            {
                if (txtPickedReceivedBy.Text.Trim() == "")
                    strRequired += "Missing Picked Info.<br/>";
            }
            if (chkConfirmed.Checked)
            {
                if (txtConfimedBy.Text.Trim() == "")
                    strRequired += "Missing Confimed By Info.<br/>";
            }

            if (strRequired.Length > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                return;
            }
            string strUserName = string.Empty;
            string strUseEmail = string.Empty;

            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                strUserName = objUser.first_name + " " + objUser.last_name;
                strUseEmail = objUser.email;
            }

            Material_Traking_Order materialTrakingOrder = new Material_Traking_Order();

            if (hdnOrderId.Value != "")
                materialTrakingOrder = _db.Material_Traking_Orders.Single(c => c.Order_id == Convert.ToInt32(hdnOrderId.Value));


            materialTrakingOrder.Order_date = Convert.ToDateTime(txtSelectionDate.Text);
            materialTrakingOrder.Section_id = Convert.ToInt32(ddlSection.SelectedItem.Value);
            materialTrakingOrder.Section_name = ddlSection.SelectedItem.Text;
            materialTrakingOrder.Vendor_id = Convert.ToInt32(ddlVendor.SelectedItem.Value);
            materialTrakingOrder.Vendor_name = ddlVendor.SelectedItem.Text;
            materialTrakingOrder.Item_text = txtTitle.Text;
            materialTrakingOrder.Item_note = txtNotes.Text;
            materialTrakingOrder.client_id = Convert.ToInt32(hdnClientId.Value);

            materialTrakingOrder.Shipped_by = txtShippedTrackingInfo.Text;
            materialTrakingOrder.Shipped_note = txtShippedNotes.Text;
            materialTrakingOrder.Received_by = txtRecReceivedBy.Text;
            materialTrakingOrder.Received_note = txtRecNotes.Text;
            materialTrakingOrder.Picked_by = txtPickedReceivedBy.Text;
            materialTrakingOrder.Picked_note = txtPickedNotes.Text;
            materialTrakingOrder.Confirmed_by = txtConfimedBy.Text;
            materialTrakingOrder.Confirmed_note = txtConfirmedNotes.Text;

            materialTrakingOrder.Is_Confirmed = chkConfirmed.Checked;
            materialTrakingOrder.Is_Picked = chkPicked.Checked;
            materialTrakingOrder.Is_Received = chkReceived.Checked;
            materialTrakingOrder.Is_Shipped = chkShipped.Checked;
            materialTrakingOrder.Shipped_date = Convert.ToDateTime(txtShippedTrackingInfoDate.Text);
            materialTrakingOrder.Received_date = Convert.ToDateTime(txtRecReceivedByDate.Text);
            materialTrakingOrder.Picked_date = Convert.ToDateTime(txtPickedReceivedByDate.Text);
            materialTrakingOrder.Confirmed_date = Convert.ToDateTime(txtConfimedByDate.Text);
            materialTrakingOrder.Estimate_id = Convert.ToInt32(ddlEst.SelectedValue);
            if (hdnOrderId.Value == "")
            {
                materialTrakingOrder.Order_date = Convert.ToDateTime(txtSelectionDate.Text);
                materialTrakingOrder.Section_id = Convert.ToInt32(ddlSection.SelectedItem.Value);
                materialTrakingOrder.Section_name = ddlSection.SelectedItem.Text;
                materialTrakingOrder.Vendor_id = Convert.ToInt32(ddlVendor.SelectedItem.Value);
                materialTrakingOrder.Vendor_name = ddlVendor.SelectedItem.Text;
                materialTrakingOrder.Item_text = txtTitle.Text;
                materialTrakingOrder.Item_note = txtNotes.Text;

                materialTrakingOrder.Last_update_date = DateTime.Now;
                materialTrakingOrder.Customer_id = CustomerId;
           
                materialTrakingOrder.Is_Active = true;

                materialTrakingOrder.CreateBy = strUserName;
                materialTrakingOrder.Create_date = DateTime.Now;

                _db.Material_Traking_Orders.InsertOnSubmit(materialTrakingOrder);
                _db.SubmitChanges();
            }
            else
            {
                _db.SubmitChanges();
            }
            //int lastRowId = materialTrakingOrder.Order_id;
            foreach (var file in file_upload.PostedFiles)
            {
                file_upload_info objfui = new file_upload_info();
                //if (hdnOrderId.Value != "")
                //{
                //    objfui = _db.file_upload_infos.Single(c => c.vendor_cost_id == Convert.ToInt32(hdnOrderId.Value) && c.CustomerId == Convert.ToInt32(hdnCustomerID.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateID.Value));
                //}
                strFileExt = Path.GetExtension(file.FileName).ToLower();
                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                if (originalFileName != "")
                {
                    if (strFileExt != ".pdf" && strFileExt != ".doc" && strFileExt != ".docx" && strFileExt != ".xls" && strFileExt != ".xlsx" && strFileExt != ".csv" && strFileExt != ".txt" && strFileExt != ".jpg" && strFileExt != ".jpeg" && strFileExt != ".png" && strFileExt != ".gif")
                    {
                        strRequired = "Invalid file type, This (" + strFileExt + ")  file type is not allowed to upload.";

                    }

                    if (file.FileName.Length == 0)
                    {
                        strRequired = "Invalid file name.";
                    }

                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);

                        return;
                    }

                    if (strRequired.Length == 0)
                    {

                        string trim_originalFileName = originalFileName.Replace(" ", "");

                        string sFileName = trim_originalFileName.Trim() + "_" + DateTime.Now.Ticks.ToString() + strFileExt;

                        string sFileThumbnailPath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + CustomerId + "\\MATERIAL\\Thumbnail";
                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + CustomerId + "\\MATERIAL";

                        if (Directory.Exists(sFilePath) == false)
                        {
                            Directory.CreateDirectory(sFilePath);

                        }
                        if (Directory.Exists(sFileThumbnailPath) == false)
                        {
                            Directory.CreateDirectory(sFileThumbnailPath);

                        }

                        sFilePath = sFilePath + "\\" + sFileName;
                        file.SaveAs(sFilePath);

                        // Thumbnail Image Save
                        if (strFileExt == ".jpg" || strFileExt == ".png" || strFileExt == ".jpeg")
                        {
                            ImageUtility.SaveThumbnailImage(sFilePath, sFileThumbnailPath);

                        }

                        objfui.client_id = Convert.ToInt32(hdnClientId.Value);
                        objfui.CustomerId = CustomerId;
                        objfui.estimate_id = EstimateId;
                        objfui.Desccription = "";
                        objfui.ImageName = sFileName;
                        objfui.is_design = false;
                        objfui.IsSiteProgress = false;
                        objfui.type = 2; // (item=2,pickup=3,confirmed=4)
                        objfui.vendor_cost_id = materialTrakingOrder.Order_id;
                        _db.file_upload_infos.InsertOnSubmit(objfui);
                        _db.SubmitChanges();
                    }
                }

            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            GetSectionSelectionListData(1);
            resetsave();
            btnSave.Text = "Save";
            hdnOrderId.Value = "";
           
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }

    }

    private void resetsave()
    {

        txtSelectionDate.Text = DateTime.Now.ToShortDateString();
        txtShippedTrackingInfoDate.Text = DateTime.Now.ToShortDateString();
        txtRecReceivedByDate.Text = DateTime.Now.ToShortDateString();
        txtPickedNotes.Text = DateTime.Now.ToShortDateString();
        txtConfimedByDate.Text = DateTime.Now.ToShortDateString();
        txtTitle.Text = "";
        txtNotes.Text = "";
        txtShippedTrackingInfo.Text = "";
        txtShippedNotes.Text = "";
        txtRecReceivedBy.Text = "";
        txtRecNotes.Text = "";
        txtPickedReceivedBy.Text = "";
        txtPickedNotes.Text = "";
        txtConfimedBy.Text = "";
        txtConfirmedNotes.Text = "";
        chkShipped.Checked = false;
        chkReceived.Checked = false;
        chkPicked.Checked = false;
        chkConfirmed.Checked = false;

        CollapsiblePanelExtenderShipped.Collapsed = true;
        CollapsiblePanelExtenderShipped.ClientState = "true";
        CollapsiblePanelExtenderReceived.Collapsed = true;
        CollapsiblePanelExtenderReceived.ClientState = "true";
        CollapsiblePanelExtenderPicked.Collapsed = true;
        CollapsiblePanelExtenderPicked.ClientState = "true";
        CollapsiblePanelExtenderConfirmed.Collapsed = true;
        CollapsiblePanelExtenderConfirmed.ClientState = "true";

       

        txtSelectionDate.Visible = true;
        imgSelectionDate.Visible = true;
        txtTitle.Visible = true;
        txtNotes.Visible = true;
        ddlSection.Visible = true;
        ddlVendor.Visible = true;



        lblSelectionDate.Visible = false;
        lblTitle.Visible = false;
        lblNotes.Visible = false;
        lblSection.Visible = false;
        lblVendor.Visible = false;

    }

    protected void grdMaterial_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //userinfo objUser = (userinfo)Session["oUser"];

            int nOrderID = Convert.ToInt32(grdMaterial.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int Section_id = Convert.ToInt32(grdMaterial.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int Vendor_id = Convert.ToInt32(grdMaterial.DataKeys[e.Row.RowIndex].Values[4].ToString());

            LinkButton lnkOpen_ConfirmedNotes = (LinkButton)e.Row.FindControl("lnkOpen_ConfirmedNotes");

            LinkButton lnkOpen_PickedNotes = (LinkButton)e.Row.FindControl("lnkOpen_PickedNotes");

            LinkButton lnkOpen_ReceivedNotes = (LinkButton)e.Row.FindControl("lnkOpen_ReceivedNotes");

            LinkButton lnkOpen_Shipped_Notes = (LinkButton)e.Row.FindControl("lnkOpen_Shipped_Notes");

            LinkButton lnkOpen_Item_note = (LinkButton)e.Row.FindControl("lnkOpen_Item_note");

            if (nOrderID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nOrderID);
            }

            Label lblItemnote = (Label)e.Row.FindControl("lblItemnote");
            string str = lblItemnote.Text.Replace("&nbsp;", "");
            if (str.Length > 20)
            {
                // txtNotes.Text = str;
                lblItemnote.Text = str.Substring(0, 20) + "...";
                lblItemnote.ToolTip = str;
                lnkOpen_Item_note.Visible = true;

            }
            else
            {
                //txtNotes.Text = str;
                lblItemnote.Text = str;
                lblItemnote.ToolTip = str;
                lnkOpen_Item_note.Visible = false;

            }

            Label lblShippedNotes = (Label)e.Row.FindControl("lblgShippedTrackingDate");
            string str1 = lblShippedNotes.Text.Replace("&nbsp;", "");
            if (str1.Length > 20)
            {
                //txtNotes.Text = str;
                lblShippedNotes.Text = str1.Substring(0, 20) + "...";
                lblShippedNotes.ToolTip = str1;
                //lnkOpen_Shipped_Notes.Visible = true;

            }
            else
            {
                //txtNotes.Text = str;
                lblShippedNotes.Text = str1;
                lblShippedNotes.ToolTip = str1;
                //lnkOpen_Shipped_Notes.Visible = false;

            }

            Label lblReceivedNotes = (Label)e.Row.FindControl("lblgRecevedDate");
            string str2 = lblReceivedNotes.Text.Replace("&nbsp;", "");

            if (str2.Length > 20)
            {
                //txtNotes.Text = str;
                lblReceivedNotes.Text = str2.Substring(0, 20) + "...";
                lblReceivedNotes.ToolTip = str2;
                //lnkOpen_ReceivedNotes.Visible = true;

            }
            else
            {
                //txtNotes.Text = str;
                lblReceivedNotes.Text = str2;
                lblReceivedNotes.ToolTip = str2;
                //lnkOpen_ReceivedNotes.Visible = false;

            }
            Label lblPickedNotes = (Label)e.Row.FindControl("lblgPickedDate");
            string str3 = lblPickedNotes.Text.Replace("&nbsp;", "");
            if (str3.Length > 20)
            {
                //txtNotes.Text = str;
                lblPickedNotes.Text = str3.Substring(0, 20) + "...";
                lblPickedNotes.ToolTip = str3;
                //lnkOpen_PickedNotes.Visible = true;

            }
            else
            {
                //txtNotes.Text = str;
                lblPickedNotes.Text = str3;
                lblPickedNotes.ToolTip = str3;
                //lnkOpen_PickedNotes.Visible = false;

            }
            Label lblConfirmedNotes = (Label)e.Row.FindControl("lblgConfirmedDate");
            string str4 = lblConfirmedNotes.Text.Replace("&nbsp;", "");
            if (str4.Length > 20)
            {
                //txtNotes.Text = str;
                lblConfirmedNotes.Text = str4.Substring(0, 20) + "...";
                lblConfirmedNotes.ToolTip = str4;
                //lnkOpen_ConfirmedNotes.Visible = true;

            }
            else
            {
                //txtNotes.Text = str;
                lblConfirmedNotes.Text = str4;
                lblConfirmedNotes.ToolTip = str4;
                // lnkOpen_ConfirmedNotes.Visible = false;

            }

            DropDownList ddlgSectiong = (DropDownList)e.Row.FindControl("ddlgSectiong");
            ddlgSectiong.DataSource = (DataTable)Session["matSection"];
            ddlgSectiong.DataValueField = "section_id";
            ddlgSectiong.DataTextField = "section_name";
            ddlgSectiong.DataBind();

            ddlgSectiong.SelectedValue = Section_id.ToString();

        }

    }
    private void GetUploadedFileListData(GridView grd, int nOrderID)
    {
        DataClassesDataContext _db = new DataClassesDataContext();



        var objfui = from fui in _db.file_upload_infos
                     where fui.vendor_cost_id == nOrderID && fui.type == 2
                     orderby fui.upload_fileId ascending
                     select fui;
        if (objfui != null)
        {
            grd.DataSource = objfui;
            grd.DataKeyNames = new string[] { "upload_fileId", "vendor_cost_id", "estimate_id", "ImageName" };
            grd.DataBind();
        }
    }

    private void GetSectionSelectionListData(int orderbyType)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
           // object sslist = null;
            DataTable dt = null;


            if (orderbyType == 1)
            {
               // sslist =
               //_db.Material_Traking_Orders.Where(ss => ss.Customer_id == CustomerId && ss.Estimate_id == EstimateId)
               //    .ToList()
               //    .OrderByDescending(c => c.Order_date);
                string sSql=" SELECT Order_id, m.Customer_id, m.Estimate_id, Section_id, Section_name, Vendor_id, Vendor_name, Item_text, Item_note, Shipped_by, Shipped_note, Received_by, "+
                            " Received_note, Picked_by, Picked_note, Confirmed_by, "+
                           " Confirmed_note, m.Order_date, m.Create_date, m.Last_update_date, CreateBy, Is_Shipped, Is_Received, Is_Picked, Is_Confirmed, Is_Active, Shipped_date,"+
                           " Received_date, Picked_date, Confirmed_date,estimate_name " +
                           " FROM  Material_Traking_Order as m "+
                           " inner join customer_estimate as  ce on ce.customer_id=m.customer_id and ce.estimate_id=m.estimate_id " +
                           " where m.customer_id=" + CustomerId + " and m.estimate_id=" + EstimateId +
                           " order by m.Order_date DESC ";
                 dt = csCommonUtility.GetDataTable(sSql);

            }
            else
            {
               // sslist =
               //_db.Material_Traking_Orders.Where(ss => ss.Customer_id == CustomerId && ss.Estimate_id == EstimateId)
               //    .ToList()
               //    .OrderByDescending(c => c.Last_update_date);
                string sSql = " SELECT Order_id, m.Customer_id, m.Estimate_id, Section_id, Section_name, Vendor_id, Vendor_name, Item_text, Item_note, Shipped_by, Shipped_note, Received_by, " +
                           " Received_note, Picked_by, Picked_note, Confirmed_by, " +
                          " Confirmed_note, m.Order_date, m.Create_date, m.Last_update_date, CreateBy, Is_Shipped, Is_Received, Is_Picked, Is_Confirmed, Is_Active, Shipped_date," +
                          " Received_date, Picked_date, Confirmed_date,estimate_name " +
                          " FROM  Material_Traking_Order as m " +
                          " inner join customer_estimate as  ce on ce.customer_id=m.customer_id and ce.estimate_id=m.estimate_id " +
                          " where m.customer_id=" + CustomerId + " and m.estimate_id=" + EstimateId +
                          " order by m.Last_update_date DESC ";
                 dt = csCommonUtility.GetDataTable(sSql);
            }

            grdMaterial.DataSource = dt;
            grdMaterial.DataKeyNames = new string[] { "Order_id", "Customer_id", "Estimate_id", "Section_id", "Vendor_id" };
            grdMaterial.DataBind();

            if (grdMaterial.Rows.Count == 0)
            {
                CollapsiblePanelExtender1.Collapsed = false;
                CollapsiblePanelExtender1.ClientState = "false";
            }
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkOpen_Click_Item_note(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[5].Controls[0].FindControl("lblItemnote") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[5].Controls[1].FindControl("lblItemnote_r") as Label;
        LinkButton lnkOpen = gRow.Cells[5].Controls[3].FindControl("lnkOpen_Item_note") as LinkButton;

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
    protected void lnkOpen_Shipped_Notes(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[6].Controls[0].FindControl("lblShippedNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[6].Controls[1].FindControl("lblShippedNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[6].Controls[3].FindControl("lnkOpen_Shipped_Notes") as LinkButton;

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
    protected void lnkOpen_ReceivedNotes(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[8].Controls[0].FindControl("lblReceivedNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[8].Controls[1].FindControl("lblReceivedNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[8].Controls[3].FindControl("lnkOpen_ReceivedNotes") as LinkButton;

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
    protected void lnkOpen_PickedNotes(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[10].Controls[0].FindControl("lblPickedNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[10].Controls[1].FindControl("lblPickedNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[10].Controls[3].FindControl("lnkOpen_PickedNotes") as LinkButton;

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
    protected void lnkOpen_ConfirmedNotes(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[10].Controls[0].FindControl("lblConfirmedNotes") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[10].Controls[1].FindControl("lblConfirmedNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[10].Controls[3].FindControl("lnkOpen_ConfirmedNotes") as LinkButton;

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
            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
            GridView grdUploadedFileList = (GridView)sender;
            userinfo objUser = (userinfo)Session["oUser"];
            GridViewRow grdSelectionRow = (GridViewRow)(grdUploadedFileList.NamingContainer);
            int Index = grdSelectionRow.RowIndex;

            GridView grdSelection = (GridView)(grdSelectionRow.Parent.Parent);
            //bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);



            int nUploadFileId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();


            string file_name = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString();


            string fileName = file_name.Substring(0, 10);
            Label lblFileName = (Label)e.Row.FindControl("lblFileName");
            lblFileName.Text = fileName;

            Button btnDeleteUploadedFile = (Button)e.Row.FindControl("btnDeleteUploadedFile");
            btnDeleteUploadedFile.CommandArgument = nUploadFileId.ToString();





            //if (objUser.role_id == 1 || objUser.role_id == 2)
            //{
            //    btnDeleteUploadedFile.Visible = true;

            //}
            //else
            //{
            //    btnDeleteUploadedFile.Visible = false;
            //}



            if (file_name.Contains(".jpg") || file_name.Contains(".jpeg") || file_name.Contains(".png") || file_name.Contains(".gif"))
            {
                HyperLink hypImg = (HyperLink)e.Row.FindControl("hypImg");
                hypImg.Visible = true;

                hypImg.NavigateUrl = "File/" + CustomerId + "/MATERIAL/" + file_name;
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

                img.ImageUrl = "File/" + CustomerId + "/MATERIAL/Thumbnail/" + file_name;
                //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this Image?');";
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

                hypPDF.NavigateUrl = "File/" + CustomerId + "/MATERIAL/" + file_name;

                hypPDF.Target = "_blank";
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this PDF File?');";
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

                hypDoc.NavigateUrl = "File/" + CustomerId + "/MATERIAL/" + file_name;

                hypDoc.Target = "_blank";
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this Doc. File?');";

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

                hypExcel.NavigateUrl = "File/" + CustomerId + "/MATERIAL/" + file_name;

                hypExcel.Target = "_blank";
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this Excel File?');";
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

                hypTXT.NavigateUrl = "File/" + CustomerId + "/MATERIAL/" + file_name;

                hypTXT.Target = "_blank";
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this Text File?');";
            }






        }
    }
    protected void DeleteUploadedFile(object sender, EventArgs e)
    {
        Label lblResult = new Label();
        try
        {

            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
            DataClassesDataContext _db = new DataClassesDataContext();

            Button btnDeleteUploadedFile = (Button)sender;
            int nUploadFileId = Convert.ToInt32(btnDeleteUploadedFile.CommandArgument);

            GridViewRow grdUploadedFileListRow = (GridViewRow)((Button)sender).NamingContainer;
            GridView grdUploadedFileList = (GridView)(grdUploadedFileListRow.Parent.Parent);

            GridViewRow grdSelectionRow = (GridViewRow)(grdUploadedFileList.NamingContainer);
            int Index = grdSelectionRow.RowIndex;

            GridView grdSelection = (GridView)(grdSelectionRow.Parent.Parent);
            //bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);

            string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + CustomerId + "//MATERIAL//";
            string sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "\\" + CustomerId + "\\MATERIAL\\";
            string sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//Thumbnail//";


            var upList = (from upl in _db.file_upload_infos
                          where upl.upload_fileId == nUploadFileId
                          select new
                          {
                              ImageName = upl.ImageName,
                          }).ToList();

            if (Directory.Exists(sFolderPath))
            {

                foreach (var file in upList)
                {

                    sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//" + file.ImageName;
                    File.Delete(sFolderPath);


                    sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//Thumbnail//" + file.ImageName;
                    File.Delete(sFileThumbnailPath);
                }

            }
            string strQ = "Delete file_upload_info WHERE upload_fileId=" + nUploadFileId + "  AND type = 2 AND client_id =" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            //Set Focus
            int j = 0;
            int nIndx = Index + 1;
            if (nIndx < grdSelection.Rows.Count)
            {
                j = nIndx;
                grdSelection.Rows[j].Cells[9].Focus();
            }

            //End Set Focus
            GetSectionSelectionListData(2);

            lblResult.Text = csCommonUtility.GetSystemMessage("Data Updated successfully");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);


        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

        }


    }
    protected void imgbtngrdUpload_Click(object sender, EventArgs e)
    {
        Label lblResult = new Label();
        try
        {
            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);


            DataClassesDataContext _db = new DataClassesDataContext();

            ImageButton imgbtngrdUpload = (ImageButton)sender;
            int nOrderID = Convert.ToInt32(imgbtngrdUpload.CommandArgument);


            GridViewRow grdSelectionRow = (GridViewRow)((ImageButton)sender).NamingContainer;
            int Index = grdSelectionRow.RowIndex;
            //bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);

            string strFileExt = "";
            string strRequired = string.Empty;


            FileUpload grdfile_upload = (FileUpload)grdMaterial.Rows[Index].FindControl("grdfile_upload");
            //int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[Index].Values[0].ToString());



            if (grdfile_upload.HasFiles)
            {

                foreach (var file in grdfile_upload.PostedFiles)
                {
                    file_upload_info objfui = new file_upload_info();
                    strFileExt = Path.GetExtension(file.FileName).ToLower();
                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (strFileExt != ".pdf" && strFileExt != ".doc" && strFileExt != ".docx" && strFileExt != ".xls" && strFileExt != ".xlsx" && strFileExt != ".csv" && strFileExt != ".txt" && strFileExt != ".jpg" && strFileExt != ".jpeg" && strFileExt != ".png" && strFileExt != ".gif")
                    {
                        strRequired = "Invalid file type, This (" + strFileExt + ")  file type is not allowed to upload.";

                    }

                    if (file.FileName.Length == 0)
                    {
                        strRequired = "Invalid file name.";
                    }

                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        return;
                    }

                    if (strRequired.Length == 0)
                    {
                        string trim_originalFileName = originalFileName.Replace(" ", "");

                        string sFileName = trim_originalFileName.Trim() + "_" + DateTime.Now.Ticks.ToString() + strFileExt;

                        string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "\\" + CustomerId + "\\MATERIAL";
                        string sFileThumbnailPath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + CustomerId + "\\MATERIAL\\Thumbnail";
                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + CustomerId + "\\MATERIAL";

                        if (Directory.Exists(selectionFolderPath) == false)
                        {
                            Directory.CreateDirectory(selectionFolderPath);

                        }

                        if (Directory.Exists(sFilePath) == false)
                        {
                            Directory.CreateDirectory(sFilePath);

                        }

                        if (Directory.Exists(sFileThumbnailPath) == false)
                        {
                            Directory.CreateDirectory(sFileThumbnailPath);

                        }


                        sFilePath = sFilePath + "\\" + sFileName;
                        file.SaveAs(sFilePath);

                        // Thumbnail Image Save
                        if (strFileExt == ".jpg" || strFileExt == ".png" || strFileExt == ".jpeg")
                        {
                            ImageUtility.SaveThumbnailImage(sFilePath, sFileThumbnailPath);
                        }


                        objfui.client_id = Convert.ToInt32(hdnClientId.Value);
                        objfui.CustomerId = CustomerId;
                        objfui.estimate_id = EstimateId;
                        objfui.Desccription = "";
                        objfui.ImageName = sFileName;
                        objfui.is_design = false;
                        objfui.IsSiteProgress = false;
                        objfui.type = 2; // Section Selection
                        objfui.vendor_cost_id = nOrderID;
                        _db.file_upload_infos.InsertOnSubmit(objfui);
                        _db.SubmitChanges();
                    }

                }


                string strMessage = csCommonUtility.GetSystemMessage("Data saved and file uploaded successfully");
                lblResult.Text = strMessage;
                GetSectionSelectionListData(2);

            }
            else
            {
                strRequired = csCommonUtility.GetSystemErrorMessage("Please Select File");
                lblResult.Text = strRequired;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
                return;
            }

            if (strRequired.Length > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
                return;
            }

            //Set Focus           
            int j = 0;
            int nIndx = Index + 1;
            if (nIndx < grdMaterial.Rows.Count)
            {
                j = nIndx;
                grdMaterial.Rows[j].Cells[9].Focus();
            }

            //End Set Focus

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }
    protected void DeleteSelection(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateID.Value);

            ImageButton imgDelete = (ImageButton)sender;
            int nOrderID = Convert.ToInt32(imgDelete.CommandArgument);
            Material_Traking_Order objS = _db.Material_Traking_Orders.SingleOrDefault(ss => ss.Order_id == nOrderID);

            strQ = "delete from Material_Traking_Order where Order_id=" + nOrderID + " AND customer_id=" + CustomerId + "  AND  estimate_id=" + EstimateId;
            _db.ExecuteCommand(strQ, string.Empty);

            //string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + hdnCustomerID + "//SELECTIONS//";
            string sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//";
            string sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//Thumbnail//";

            var upList = (from upl in _db.file_upload_infos
                          where upl.vendor_cost_id == nOrderID && upl.CustomerId == CustomerId && upl.estimate_id == EstimateId && upl.type == 2
                          select new
                          {
                              ImageName = upl.ImageName,
                          }).ToList();

            if (Directory.Exists(sFolderPath))
            {

                foreach (var file in upList)
                {
                    sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//" + file.ImageName;
                    File.Delete(sFolderPath);
                    sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + CustomerId + "//MATERIAL//Thumbnail//" + file.ImageName;
                    File.Delete(sFileThumbnailPath);
                }
            }

            strQ = "delete from file_upload_info where vendor_cost_id=" + nOrderID + " AND  estimate_id=" + EstimateId + " AND type = 2 AND CustomerId=" + CustomerId + "";
            _db.ExecuteCommand(strQ, string.Empty);

            GetSectionSelectionListData(2);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemMessage("Data deleted successfully");
        }
        catch (Exception ex)
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void EditSelection(object sender, EventArgs e)
    {
        try
        {
            lblResult2.Text = "";
            lblResult.Text = "";
            resetsave();
            DataClassesDataContext _db = new DataClassesDataContext();
            BindEstimate();

            ImageButton imgEdit = (ImageButton)sender;
            int nOrderID = Convert.ToInt32(imgEdit.CommandArgument);
            Material_Traking_Order objS = _db.Material_Traking_Orders.SingleOrDefault(ss => ss.Order_id == nOrderID);

            txtSelectionDate.Text = Convert.ToDateTime(objS.Order_date).ToString("MM/dd/yyyy");
            txtTitle.Text = objS.Item_text;
            txtNotes.Text = objS.Item_note;
            ddlSection.SelectedValue = objS.Section_id.ToString();
            LoadVendorEdit(objS.Section_id.ToString(), objS.Vendor_id.ToString());

            hdnEstimateID.Value = objS.Estimate_id.ToString();
            ddlEst.SelectedValue = objS.Estimate_id.ToString();


            txtSelectionDate.Visible = true;
            imgSelectionDate.Visible = true;
            txtTitle.Visible = true;
            txtNotes.Visible = true;
            ddlSection.Visible = true;
            ddlVendor.Visible = true;

            hdnOrderId.Value = nOrderID.ToString();
            lblSelectionDate.Text = Convert.ToDateTime(objS.Order_date).ToString("MM/dd/yyyy");
            lblTitle.Text = objS.Item_text;
            lblNotes.Text = objS.Item_note;
            lblSection.Text = ddlSection.SelectedItem.Text;



            CollapsiblePanelExtender1.Collapsed = false;
            CollapsiblePanelExtender1.ClientState = "false";

            if (objS.Is_Shipped == true)
            {
                CollapsiblePanelExtenderShipped.Collapsed = false;
                CollapsiblePanelExtenderShipped.ClientState = "false";
                chkShipped.Checked = true;
                txtShippedTrackingInfo.Text = objS.Shipped_by;
                txtShippedTrackingInfoDate.Text = Convert.ToDateTime(objS.Shipped_date).ToString("MM/dd/yyyy"); ;
                txtShippedNotes.Text = objS.Shipped_note;

                PaneTrackingInfo.Visible = true;

            }
            if (objS.Is_Received == true)
            {
                chkReceived.Checked = true;
                CollapsiblePanelExtenderReceived.Collapsed = false;
                CollapsiblePanelExtenderReceived.ClientState = "false";
                txtRecReceivedBy.Text = objS.Received_by;
                txtRecReceivedByDate.Text = Convert.ToDateTime(objS.Received_date).ToString("MM/dd/yyyy");
                txtRecNotes.Text = objS.Received_note;

                PanelReceived.Visible = true;

            }
            if (objS.Is_Picked == true)
            {
                chkPicked.Checked = true;
                CollapsiblePanelExtenderPicked.Collapsed = false;
                CollapsiblePanelExtenderPicked.ClientState = "false";
                txtPickedReceivedBy.Text = objS.Picked_by;
                txtPickedReceivedByDate.Text = Convert.ToDateTime(objS.Picked_date).ToString("MM/dd/yyyy");
                txtPickedNotes.Text = objS.Picked_note;


                PanelPicked.Visible = true;

            }
            if (objS.Is_Confirmed == true)
            {
                chkConfirmed.Checked = true;
                CollapsiblePanelExtenderConfirmed.Collapsed = false;
                CollapsiblePanelExtenderConfirmed.ClientState = "false";
                txtConfimedBy.Text = objS.Confirmed_by;
                txtConfimedByDate.Text = Convert.ToDateTime(objS.Confirmed_date).ToString("MM/dd/yyyy");
                txtConfirmedNotes.Text = objS.Confirmed_note;

                PanelConfirmed.Visible = true;
            }
            btnSave.Text = "Update";
            btnSave.CommandName = "Update";
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdSelection_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = "";


        DropDownList ddlgSectiong = (DropDownList)grdMaterial.Rows[e.RowIndex].FindControl("ddlgSectiong");
        DropDownList ddlgVendor = (DropDownList)grdMaterial.Rows[e.RowIndex].FindControl("ddlgVendor");
        TextBox txtOrderDate = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtOrderDate");
        TextBox txtItem = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtItem");
        TextBox txtItemnote = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtItemnote");
        TextBox txtShippedTrackingInfo = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtShippedTrackingInfo");
        TextBox txtShippedNotes = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtShippedNotes");
        TextBox txtReceived = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtReceived");
        TextBox txtReceivedNotes = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtReceivedNotes");
        TextBox txtPicked = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtPicked");
        TextBox txtPickedNotes = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtPickedNotes");
        TextBox txtConfirmed = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtConfirmed");
        TextBox txtConfirmedNotes = (TextBox)grdMaterial.Rows[e.RowIndex].FindControl("txtConfirmedNotes");


        DateTime dtOrderDate = new DateTime();
        try
        {
            dtOrderDate = Convert.ToDateTime(txtOrderDate.Text);
        }
        catch
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }
        if (Convert.ToInt32(ddlgSectiong.SelectedItem.Value) == 0)
        {
            strRequired += "Please Select Section is required<br/>";
        }

        if (txtItem.Text.Trim().Length != 0)
        {
            if (txtItem.Text.Trim().Length > 499)
            {
                strRequired += "Item Name is Up to 500 Characters<br/>";
            }

        }
        else
        {
            strRequired += "Item Name is required<br/>";
        }

        if (txtItemnote.Text.Trim().Length != 0)
        {

            if (txtItemnote.Text.Trim().Length > 499)
            {
                strRequired += "Item Notes is Up to 500 Characters<br/>";
            }

        }


        if (txtShippedTrackingInfo.Text.Trim().Length != 0)
        {
            if (txtShippedTrackingInfo.Text.Trim().Length > 499)
            {
                strRequired += "Shipped Tracking Info is Up to 500 Characters<br/>";
            }

        }


        if (txtShippedNotes.Text.Trim().Length != 0)
        {

            if (txtShippedNotes.Text.Trim().Length > 499)
            {
                strRequired += "Shipped  Notes is Up to 500 Characters<br/>";
            }

        }

        if (txtReceived.Text.Trim().Length != 0)
        {
            if (txtReceived.Text.Trim().Length > 499)
            {
                strRequired += "Received Info is Up to 500 Characters<br/>";
            }

        }


        if (txtReceivedNotes.Text.Trim().Length != 0)
        {

            if (txtReceivedNotes.Text.Trim().Length > 499)
            {
                strRequired += "Received  Notes is Up to 500 Characters<br/>";
            }

        }
        if (txtPicked.Text.Trim().Length != 0)
        {

            if (txtPicked.Text.Trim().Length > 499)
            {
                strRequired += "Picked is Up to 500 Characters<br/>";
            }

        }
        if (txtPickedNotes.Text.Trim().Length != 0)
        {

            if (txtPickedNotes.Text.Trim().Length > 499)
            {
                strRequired += "Picked Notes is Up to 500 Characters<br/>";
            }

        }
        if (txtConfirmed.Text.Trim().Length != 0)
        {

            if (txtConfirmed.Text.Trim().Length > 499)
            {
                strRequired += "Confirmed is Up to 500 Characters<br/>";
            }

        }
        if (txtConfirmedNotes.Text.Trim().Length != 0)
        {

            if (txtConfirmedNotes.Text.Trim().Length > 499)
            {
                strRequired += "Confirmed Notes is Up to 500 Characters<br/>";
            }

        }
        int CustomerId = Convert.ToInt32(hdnCustomerID.Value);
        int EstimateId = Convert.ToInt32(hdnEstimateID.Value);
        if (strRequired.Length > 0)
        {
            lblResult2.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
        }
        else
        {
            int nOderID = Convert.ToInt32(grdMaterial.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

            Material_Traking_Order objSS = _db.Material_Traking_Orders.SingleOrDefault(ss => ss.Order_id == nOderID && ss.Customer_id == CustomerId && ss.Estimate_id == EstimateId);

            objSS.Order_date = dtOrderDate;
            objSS.Section_id = Convert.ToInt32(ddlgSectiong.SelectedValue);
            //  objSS.Vendor_id = Convert.ToInt32(ddlgVendor.SelectedValue);
            objSS.Item_text = txtItem.Text.Trim();
            objSS.Item_note = txtItemnote.Text.Trim();
            objSS.Shipped_by = txtShippedTrackingInfo.Text.Trim();
            objSS.Shipped_note = txtShippedNotes.Text.Trim();
            objSS.Received_by = txtReceived.Text.Trim();
            objSS.Received_note = txtReceivedNotes.Text.Trim();
            objSS.Picked_by = txtPicked.Text.Trim();
            objSS.Picked_note = txtPickedNotes.Text.Trim();
            objSS.Confirmed_by = txtConfirmed.Text.Trim();
            objSS.Confirmed_note = txtConfirmedNotes.Text.Trim();
            objSS.Last_update_date = DateTime.Now;

            _db.SubmitChanges();

            lblResult2.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
            GetSectionSelectionListData(2);  //order by updatedate so use 2 or upper number
        }

    }

    protected void ddlgVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlMake = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddlMake.NamingContainer;
        if (row != null)
        {

            DropDownList ddlModel = (DropDownList)row.FindControl("ddlgVendor");
            ddlModel.DataSource = GetvendorList(Convert.ToInt32(ddlMake.SelectedValue));
            ddlModel.DataValueField = "Vendor_Id";
            ddlModel.DataTextField = "vendor_name";
            ddlModel.DataBind();

        }
    }

    private DataTable GetvendorList(int sectionID)
    {
        string sSQL = "Select a.Vendor_Id,b.vendor_name from vendor_section as a inner join Vendor AS b ON a.Vendor_Id=b.vendor_id  where section_id=" + sectionID + " and b.is_active=1";

        DataTable table = DataReader.Complex_Read_DataTable(sSQL);

        return table;
    }
    

    protected void chkReceived_OnChanged(object sender, EventArgs e)
    {


    }
}
