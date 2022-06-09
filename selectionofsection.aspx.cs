using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class selectionofsection : System.Web.UI.Page
{
    public DataTable dtLocation;
    public DataTable dtSection;
    //public DataTable dtProject;
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

            if (Page.User.IsInRole("admin045") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            if (Session["SelectionFilter"] != null)
            {
                btnBack.Text = "Return to General Selection Review";
            }
            if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("eid") != null) // Customer Schedule
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = new customer();
                string strCustName = "";

                int nCustomerID = Convert.ToInt32(Request.QueryString.Get("cid"));
                int nEstimateID = Convert.ToInt32(Request.QueryString.Get("eid"));

                hdnCustomerID.Value = nCustomerID.ToString();
                hdnEstimateID.Value = nEstimateID.ToString();

                if (_db.customers.Where(c => c.customer_id == nCustomerID).Count() > 0)
                {
                    objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerID);
                    strCustName = objCust.first_name1 + " " + objCust.last_name1;
                    hdnClientId.Value = objCust.client_id.ToString();
                }

                lblHeaderTitle.Text = "Selection (" + strCustName + ")";

                if (_db.Section_Selections.Any(se => se.customer_id == nCustomerID && se.estimate_id == nEstimateID && se.isSelected == true))
                {
                    lblSelectionMSG.Visible = true;
                }
                else
                {
                    lblSelectionMSG.Visible = false;
                }

                SaveSectionCOPricingMaster(nCustomerID, nEstimateID);
                BindEstimate();
                LoadLocation();
                LoadSectionSec();
                GetSectionSelectionListData();
                GetDeclinedSectionSelectionListData();
                Session.Add("CustomerId", nCustomerID);
                resetsave();

                string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustomerID, nEstimateID).job_number ?? "";

                if (strJobNumber.Length > 0)
                {
                    lblTitelJobNumber.Text = " ( Job Number: " + strJobNumber + " )";
                }
        
                btnSelectionApproved.OnClientClick = "return confirm('Are you sure you want to approve the selections?');";
                btnSelectionApproved2.OnClientClick = "return confirm('Are you sure you want to approve the selections?');";
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSelectionApproved", "Button1", "Button2", "ddlLocation", "ddlSection", "file_upload", "imgbtnUpload", "btnSelectionApproved2", "btnEmailSelection", "btnSave" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdSelection_grdfile_upload", "grdSelection_chkAll", "grdSelection_imgbtngrdUpload", "grdSelection_imgDelete", "grdSelection_chkSelected", "Edit", "Delete", "Update", "grdSelection_chkClientText", "grdSelection_chkHhiInternal" });
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
             DataTable dt=csCommonUtility.GetDataTable(strQ);
             //   Session.Add("sProject", dt);
             // dtProject = (DataTable)Session["sProject"];
            ddlEst.DataSource = dt;
            ddlEst.DataTextField = "estimate_name";
            ddlEst.DataValueField = "estimate_id";
            ddlEst.DataBind();
            ddlEst.SelectedValue=hdnEstimateID.Value;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void LoadLocation()
    {
        try
        {
            int nCustId = Convert.ToInt32(hdnCustomerID.Value);
            int nEstId = Convert.ToInt32(hdnEstimateID.Value);

            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpLTable = LoadLocationTable();
            DataRow dr = tmpLTable.NewRow();
            dr["location_id"] = -1;
            dr["location_name"] = "Not Location Specific";
            tmpLTable.Rows.Add(dr);

            var item = from l in _db.locations
                       join c in _db.changeorder_locations on l.location_id equals c.location_id
                       where c.customer_id == nCustId && c.estimate_id == nEstId
                       orderby l.location_name
                       select new
                       {
                           location_id = l.location_id,
                           location_name = l.location_name.Trim(),
                       };

            foreach (var vi in item)
            {

                DataRow drNew = tmpLTable.NewRow();
                drNew["location_id"] = vi.location_id;
                drNew["location_name"] = vi.location_name;
                tmpLTable.Rows.Add(drNew);
            }

            Session.Add("ssLocation", tmpLTable);
            dtLocation = (DataTable)Session["ssLocation"];
            ddlLocation.DataSource = tmpLTable;
            ddlLocation.DataTextField = "location_name";
            ddlLocation.DataValueField = "location_id";
            ddlLocation.DataBind();
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    private void LoadSectionSec()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpSTable = LoadSectionTable();
            DataRow dr = tmpSTable.NewRow();
            dr["section_id"] = -1;
            dr["section_name"] = "Not Section Specific";
            tmpSTable.Rows.Add(dr);

            var section = from sec in _db.tbl_selection_secs
                          orderby sec.section_name ascending
                          select new SectionInfo()
                          {
                              section_id = (int)sec.selection_secid,
                              section_name = sec.section_name
                          };

            foreach (var vi in section)
            {

                DataRow drNew = tmpSTable.NewRow();
                drNew["section_id"] = vi.section_id;
                drNew["section_name"] = vi.section_name;
                tmpSTable.Rows.Add(drNew);
            }

            Session.Add("gSection", tmpSTable);
            dtSection = (DataTable)Session["gSection"];
            ddlSection.DataSource = tmpSTable;
            ddlSection.DataTextField = "section_name";
            ddlSection.DataValueField = "section_id";
            ddlSection.DataBind();
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }



    protected void btnSave_click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        SaveData();

    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnUpload.ID, imgbtnUpload.GetType().Name, "Click"); 
        SaveData();


    }

    protected void SaveData()
    {
        try
        {
            lblResult2.Text = "";
            lblResult.Text = "";
            dtLocation = (DataTable)Session["ssLocation"];
            dtSection = (DataTable)Session["gSection"];
            string strFileExt = "";
            string strRequired = string.Empty;

            int nCustId = Convert.ToInt32(hdnCustomerID.Value);
            int nEstId = Convert.ToInt32(hdnEstimateID.Value);

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

            DateTime dtValidTillDate = new DateTime();
            try
            {
                dtValidTillDate = Convert.ToDateTime(txtValidDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                return;
            }
            if (dtCreateDate >= dtValidTillDate)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Valid till date must be greater than selection date");
                return;
            }

            if (txtPrice.Text.Trim().Length == 0)
            {
                txtPrice.Text = "0";
                //strRequired += "Price is required<br/>";
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtPrice.Text.Replace("$", "").Trim());
                }
                catch
                {
                    txtPrice.Text = "0";
                    // strRequired += "Invalide Price<br/>";
                }
            }
            if (txtTitle.Text.Trim().Length != 0)
            {
                if (txtTitle.Text.Trim().Length > 499)
                {
                    strRequired += "Title is Up to 500 Characters<br/>";
                }

            }
            else
            {
                strRequired += "Title is required<br/>";
            }
            if (txtNotes.Text.Trim().Length != 0)
            {

                if (txtTitle.Text.Trim().Length > 499)
                {
                    strRequired += "Notes is Up to 500 Characters<br/>";
                }

            }
            else
            {
                strRequired += "Notes is required<br/>";
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

            Section_Selection objSS = new Section_Selection();

            objSS.customer_id = nCustId;
            objSS.client_id = Convert.ToInt32(hdnClientId.Value);
            objSS.estimate_id = nEstId;
            objSS.section_id = Convert.ToInt32(ddlSection.SelectedValue);
            objSS.section_name = ddlSection.SelectedItem.Text; ;
            objSS.location_id = Convert.ToInt32(ddlLocation.SelectedValue);
            objSS.location_name = ddlLocation.SelectedItem.Text;
            objSS.Title = txtTitle.Text.Trim();
            objSS.Price = Convert.ToDecimal(txtPrice.Text.Trim().Replace("$", ""));
            objSS.Notes = txtNotes.Text.Trim();
            objSS.isSelected = false;
            objSS.customer_signature = "";
            objSS.customer_siignatureDate = Convert.ToDateTime("1999-01-01");
            objSS.customer_signedBy = "";
            objSS.LastUpdateDate = DateTime.Now;
            objSS.UpdateBy = User.Identity.Name;
            objSS.CreateDate = dtCreateDate;
            objSS.ValidTillDate = dtValidTillDate;
            objSS.CreatedBy = strUserName;
            objSS.UserEmail = strUseEmail;
            objSS.isDeclined = false;

            _db.Section_Selections.InsertOnSubmit(objSS);
            _db.SubmitChanges();

            foreach (var file in file_upload.PostedFiles)
            {
                file_upload_info objfui = new file_upload_info();
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

                        string sFileThumbnailPath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + nCustId + "\\SELECTIONS\\Thumbnail";
                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + nCustId + "\\SELECTIONS";

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
                        objfui.CustomerId = nCustId;
                        objfui.estimate_id = nEstId;
                        objfui.Desccription = "";
                        objfui.ImageName = sFileName;
                        objfui.is_design = false;
                        objfui.IsSiteProgress = false;
                        objfui.type = 5; // Section Selection
                        objfui.vendor_cost_id = objSS.SectionSelectionID;
                        _db.file_upload_infos.InsertOnSubmit(objfui);
                        _db.SubmitChanges();
                    }
                }

            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            GetSectionSelectionListData();
            resetsave();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }

    }

    private void GetSectionSelectionListData()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nCustId = Convert.ToInt32(hdnCustomerID.Value);
            int nEstId = Convert.ToInt32(hdnEstimateID.Value);
            dtLocation = (DataTable)Session["ssLocation"];
            dtSection = (DataTable)Session["gSection"];
           // dtProject = (DataTable)Session["sProject"];

            string sSQL = " select SectionSelectionID,s.customer_id, s.section_id,s.estimate_id,location_id,isSelected,customer_signature,customer_siignatureDate, customer_signedBy,estimate_name,s.CreateDate, " +
                          " s.section_name,s.location_name,s.Title,s.Notes,s.Price,s.ValidTillDate " +
                         " from Section_Selection as s " +
                         " inner join customer_estimate as  ce on ce.customer_id=s.customer_id and ce.estimate_id=s.estimate_id " +
                         " where s.isDeclined=0 and s.customer_id=" + nCustId + " and s.estimate_id=" + nEstId +
                         " order by s.CreateDate DESC ";
            DataTable dt = csCommonUtility.GetDataTable(sSQL);
         
            // var sslist = _db.Section_Selections.Where(ss => ss.customer_id == nCustId && ss.estimate_id == nEstId && ss.isDeclined == false).ToList().OrderByDescending(c => c.CreateDate);

            grdSelection.DataSource = dt;
            grdSelection.DataKeyNames = new string[] { "SectionSelectionID", "customer_id", "section_id", "estimate_id", "location_id", "isSelected", "customer_signature", "customer_siignatureDate", "customer_signedBy" };
            grdSelection.DataBind();
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdSelection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            userinfo objUser = (userinfo)Session["oUser"];

            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int section_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[2].ToString());
            int estimate_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int location_id = Convert.ToInt32(grdSelection.DataKeys[e.Row.RowIndex].Values[4].ToString());
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[e.Row.RowIndex].Values[5]);
            string strCustomerSignature = grdSelection.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string strSignatureDate = Convert.ToDateTime(grdSelection.DataKeys[e.Row.RowIndex].Values[7]).ToString("MM/dd/yyyy hh:mm tt");
            string SignatureBy = grdSelection.DataKeys[e.Row.RowIndex].Values[8].ToString();
           

            dtLocation = (DataTable)Session["ssLocation"];
            dtSection = (DataTable)Session["gSection"];
           // dtProject = (DataTable)Session["sProject"];
            FileUpload grdfile_upload = (FileUpload)e.Row.FindControl("grdfile_upload");

            Image imgCustomerSign = e.Row.FindControl("imgCustomerSign") as Image;
            Label lblSignatureBy = (Label)e.Row.FindControl("lblSignatureBy");
            Label lblLocation = (Label)e.Row.FindControl("lblLocation");
            DropDownList ddlSectiong = (DropDownList)e.Row.FindControl("ddlSectiong");
            Label lblSectiong = (Label)e.Row.FindControl("lblSectiong");

            DropDownList ddlLocation = (DropDownList)e.Row.FindControl("ddlLocation");

           // Label lblProject = (Label)e.Row.FindControl("lblProject");
           // DropDownList ddlProject = (DropDownList)e.Row.FindControl("ddlEst");

        
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");

            TextBox txtTitle = (TextBox)e.Row.FindControl("txtTitle");
            TextBox txtNotes = (TextBox)e.Row.FindControl("txtNotes");
            TextBox txtPrice = (TextBox)e.Row.FindControl("txtPrice");

            Label lblTitle = (Label)e.Row.FindControl("lblTitle");
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");
            Label lblPrice = (Label)e.Row.FindControl("lblPrice");

            Label lblSelected = (Label)e.Row.FindControl("lblSelected");
            Label lblSelectionDate = (Label)e.Row.FindControl("lblSelectionDate");

            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");

            HtmlControl dvValidCalender = (HtmlControl)e.Row.FindControl("dvValidCalender");
            Label lblVDate = (Label)e.Row.FindControl("lblVDate");
            TextBox txtVDate = (TextBox)e.Row.FindControl("txtVDate");

            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            imgDelete.OnClientClick = "return confirm('Are you sure you want to delete this selection?');";
            imgDelete.CommandArgument = nSectionSelectionID.ToString();

            string str = lblNotes.Text.Replace("&nbsp;", "");

            ListItem items = ddlSectiong.Items.FindByValue(section_id.ToString());
            if (items != null)
                ddlSectiong.Items.FindByValue(Convert.ToString(section_id)).Selected = true;
            else
            {
                string strsecId = section_id.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                string strSection = _db.tbl_selection_secs.FirstOrDefault(l => l.selection_secid == section_id).section_name;
                ddlSectiong.Items.Insert(0, new ListItem(strSection, strsecId));
            }

            ddlSectiong.Visible = false;
            lblSectiong.Visible = true;

            ListItem item = ddlLocation.Items.FindByValue(location_id.ToString());
            if (item != null)
                ddlLocation.Items.FindByValue(Convert.ToString(location_id)).Selected = true;
            else
            {
                string strLocId = location_id.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                string strLocation = _db.locations.FirstOrDefault(l => l.location_id == location_id).location_name;
                ddlLocation.Items.Insert(0, new ListItem(strLocation, strLocId));
            }

            ddlLocation.Visible = false;
            lblLocation.Visible = true;

            
            LinkButton btn = (LinkButton)e.Row.Cells[9].Controls[0];
           

            if (str != "" && str.Length > 60)
            {
                txtNotes.Text = str;
                lblNotes.Text = str.Substring(0, 60) + "...";
                lblNotes.ToolTip = str;
                lnkOpen.Visible = true;

            }
            else
            {
                txtNotes.Text = str;
                lblNotes.Text = str;
                lblNotes.ToolTip = str;
                lnkOpen.Visible = false;

            }


            btn.Attributes.Add("onclick", "ShowProgress();");

            if (nSectionSelectionID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nSectionSelectionID);
            }

            ImageButton imgbtngrdUpload = (ImageButton)e.Row.FindControl("imgbtngrdUpload");
            imgbtngrdUpload.CommandArgument = nSectionSelectionID.ToString();
            CheckBox chkSelected = (CheckBox)e.Row.FindControl("chkSelected");

            if (isSelected)
            {
                chkSelected.Visible = false;
                if (objUser.role_id == 1 || objUser.role_id == 2)
                {

                    btn.Visible = true;
                    imgDelete.Visible = true;
                    imgbtngrdUpload.Visible = true;
                    grdfile_upload.Visible = true;
                }
                else
                {
                    btn.Visible = false;
                    imgDelete.Visible = false;
                    imgbtngrdUpload.Visible = false;
                    grdfile_upload.Visible = false;
                }
                lblSignatureBy.Visible = true;
                lblSelected.Visible = true;
                lblSelected.Text = "Selected";
                lblSelectionDate.Text = "Date: " + strSignatureDate;

            }
            else
            {
                chkSelected.Visible = true;
                btn.Visible = true;
                imgDelete.Visible = true;
                lblSelected.Visible = false;
                lblSelected.Text = "";
                lblSelectionDate.Text = "";
                lblSelectionDate.Visible = false;

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
               
                lblSignatureBy.Text ="By "+SignatureBy;
            }
        }

    }

    protected void grdSelection_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            if (e.CommandName == "Save")
            {

                int i = 0;
                lblResult2.Text = "";
                lblResult.Text = "";

                GridViewRow gvrnew = (GridViewRow)grdSelection.NamingContainer;
                i = gvrnew.RowIndex;

                int nSectionId = Convert.ToInt32(ddlSection.SelectedValue);
                string strSectionName = ddlSection.SelectedItem.Text;


                bool IsRequiredPass = true;
                DataClassesDataContext _db = new DataClassesDataContext();
                dtLocation = (DataTable)Session["ssLocation"];
                dtSection = (DataTable)Session["gSection"];
                DataTable table = (DataTable)Session["sSection_Selection"];
                string strRequired = "";

                int nIndex = -1;
                foreach (GridViewRow di in grdSelection.Rows)
                {
                    DropDownList ddlLocation = (DropDownList)di.FindControl("ddlLocation");
                   // DropDownList ddlProject = (DropDownList)e.Row.FindControl("ddlEst");
                    TextBox txtTitle = (TextBox)di.FindControl("txtTitle");
                    TextBox txtNotes = (TextBox)di.FindControl("txtNotes");
                    TextBox txtPrice = (TextBox)di.FindControl("txtPrice");

                    Label lblTitle = (Label)di.FindControl("lblTitle");
                    Label lblNotes = (Label)di.FindControl("lblNotes");
                    Label lblPrice = (Label)di.FindControl("lblPrice");

                    HtmlControl div = (HtmlControl)di.FindControl("dvCalender");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    TextBox txtDate = (TextBox)di.FindControl("txtDate");

                    HtmlControl dvValidCalender = (HtmlControl)di.FindControl("dvValidCalender");
                    Label lblVDate = (Label)di.FindControl("lblVDate");
                    TextBox txtVDate = (TextBox)di.FindControl("txtVDate");

                    DataRow dr = table.Rows[di.RowIndex];

                    DateTime dtCreateDate = new DateTime();
                    try
                    {
                        dtCreateDate = Convert.ToDateTime(txtDate.Text);
                    }
                    catch
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                        return;
                    }

                    DateTime dtValidTillDate = new DateTime();
                    try
                    {
                        dtValidTillDate = Convert.ToDateTime(txtVDate.Text);
                    }
                    catch
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                        return;
                    }
                    if (dtCreateDate >= dtValidTillDate)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Valid till date must be greater than selection date");
                        return;
                    }


                    if (txtTitle.Text.Trim().Length == 0)
                    {
                        strRequired += "Title is required.<br/>";
                        txtTitle.BorderColor = System.Drawing.Color.Red;
                    }

                    if (txtNotes.Text.Trim().Length == 0)
                    {
                        strRequired += "Notes is required.<br/>";
                        txtNotes.BorderColor = System.Drawing.Color.Red;
                    }

                    //if (Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
                    //{
                    //    strRequired += "Price is required.<br/>";
                    //    txtPrice.BorderColor = System.Drawing.Color.Red;
                    //}

                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        IsRequiredPass = false;
                    }

                    else
                    {

                        dr["SectionSelectionID"] = Convert.ToInt32(grdSelection.DataKeys[di.RowIndex].Values[0]);
                        dr["customer_id"] = Convert.ToInt32(hdnCustomerID.Value);
                        dr["estimate_id"] = Convert.ToInt32(hdnEstimateID.Value);
                        dr["section_id"] = nSectionId;
                        dr["section_name"] = strSectionName;
                        dr["location_id"] = Convert.ToInt32(ddlLocation.SelectedValue);
                        dr["location_name"] = ddlLocation.SelectedItem.Text;
                        dr["Title"] = txtTitle.Text;
                        dr["Price"] = Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        dr["Notes"] = txtNotes.Text.Trim();
                        dr["LastUpdateDate"] = DateTime.Now;
                        dr["UpdateBy"] = User.Identity.Name;
                        dr["CreateDate"] = dtCreateDate;
                        dr["ValidTillDate"] = dtValidTillDate;

                        if (Convert.ToInt32(dr["SectionSelectionID"]) == 0)
                        {
                            nIndex = Convert.ToInt32(di.RowIndex);
                        }
                    }
                }
                if (IsRequiredPass)
                {
                    foreach (DataRow dr in table.Rows)
                    {


                        Section_Selection sec_sel = new Section_Selection();
                        if (Convert.ToInt32(dr["SectionSelectionID"]) > 0)
                            sec_sel = _db.Section_Selections.Single(l => l.SectionSelectionID == Convert.ToInt32(dr["SectionSelectionID"]));

                        if (Convert.ToDecimal(dr["cost_amount"]) != 0)
                        {
                            sec_sel.SectionSelectionID = Convert.ToInt32(dr["SectionSelectionID"]);
                            sec_sel.customer_id = Convert.ToInt32(dr["customer_id"]);
                            sec_sel.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                            sec_sel.section_id = Convert.ToInt32(dr["section_id"]);
                            sec_sel.section_name = dr["section_name"].ToString();
                            sec_sel.location_id = Convert.ToInt32(dr["location_id"]);
                            sec_sel.location_name = dr["location_name"].ToString();
                            sec_sel.Title = dr["Title"].ToString();
                            sec_sel.Price = Convert.ToDecimal(dr["Price"]);
                            sec_sel.Notes = dr["Notes"].ToString();
                            sec_sel.LastUpdateDate = DateTime.Now;
                            sec_sel.UpdateBy = User.Identity.Name;
                            sec_sel.CreateDate = Convert.ToDateTime(dr["CreateDate"]);
                            sec_sel.ValidTillDate = Convert.ToDateTime(dr["ValidTillDate"]);
                        }

                    }

                    lblResult2.Text = csCommonUtility.GetSystemMessage("Data saved successfully");

                    _db.SubmitChanges();



                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
        }
    }

    protected void grdSelection_RowEditing(object sender, GridViewEditEventArgs e)
    {
        lblResult2.Text = "";
        lblResult.Text = "";

        DropDownList ddlLocation = (DropDownList)grdSelection.Rows[e.NewEditIndex].FindControl("ddlLocation");
        DropDownList ddlSectiong = (DropDownList)grdSelection.Rows[e.NewEditIndex].FindControl("ddlSectiong");
        //DropDownList ddlEst = (DropDownList)grdSelection.Rows[e.NewEditIndex].FindControl("ddlEst");

        Label lblLocation = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblLocation");
        Label lblSectiong = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblSectiong");
       // Label lblProject = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblProject");

        TextBox txtTitle = (TextBox)grdSelection.Rows[e.NewEditIndex].FindControl("txtTitle");
        TextBox txtNotes = (TextBox)grdSelection.Rows[e.NewEditIndex].FindControl("txtNotes");
        TextBox txtPrice = (TextBox)grdSelection.Rows[e.NewEditIndex].FindControl("txtPrice");

        Label lblTitle = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblTitle");
        Label lblNotes = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblNotes");
        Label lblPrice = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblPrice");

        HtmlControl div = (HtmlControl)grdSelection.Rows[e.NewEditIndex].FindControl("dvCalender");
        Label lblDate = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblDate");
        TextBox txtDate = (TextBox)grdSelection.Rows[e.NewEditIndex].FindControl("txtDate");

        HtmlControl dvValidCalender = (HtmlControl)grdSelection.Rows[e.NewEditIndex].FindControl("dvValidCalender");
        Label lblVDate = (Label)grdSelection.Rows[e.NewEditIndex].FindControl("lblVDate");
        TextBox txtVDate = (TextBox)grdSelection.Rows[e.NewEditIndex].FindControl("txtVDate");



        txtTitle.Visible = true;
        lblTitle.Visible = false;

        string str = lblNotes.Text.Replace("&nbsp;", "");
        if (str.IndexOf(">>") != -1)
        {
            txtNotes.Visible = false;
            lblNotes.Visible = true;
        }
        else
        {
            txtNotes.Visible = true;
            lblNotes.Visible = false;
        }



        txtPrice.Visible = true;
        lblPrice.Visible = false;

        ddlLocation.Enabled = true;
        ddlLocation.Visible = true;
        lblLocation.Visible = false;

       // ddlEst.Enabled = true;
       // ddlEst.Visible = true;
       // lblProject.Visible = false;


        ddlSectiong.Enabled = true;
        ddlSectiong.Visible = true;
        lblSectiong.Visible = false;

        div.Visible = true;
        txtDate.Visible = true;
        lblDate.Visible = false;

        dvValidCalender.Visible = true;
        txtVDate.Visible = true;
        lblVDate.Visible = false;

        LinkButton btn = (LinkButton)grdSelection.Rows[e.NewEditIndex].Cells[9].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

        //Set Focus      
        ddlLocation.Focus();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }

    protected void grdSelection_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = "";
        decimal nPrice = 0;
        DropDownList ddlLocation = (DropDownList)grdSelection.Rows[e.RowIndex].FindControl("ddlLocation");
        DropDownList ddlSectiong = (DropDownList)grdSelection.Rows[e.RowIndex].FindControl("ddlSectiong");

        DropDownList ddlEst = (DropDownList)grdSelection.Rows[e.RowIndex].FindControl("ddlEst");

        TextBox txtTitle = (TextBox)grdSelection.Rows[e.RowIndex].FindControl("txtTitle");
        TextBox txtNotes = (TextBox)grdSelection.Rows[e.RowIndex].FindControl("txtNotes");
        TextBox txtPrice = (TextBox)grdSelection.Rows[e.RowIndex].FindControl("txtPrice");

        Label lblTitle = (Label)grdSelection.Rows[e.RowIndex].FindControl("lblTitle");
        Label lblNotes = (Label)grdSelection.Rows[e.RowIndex].FindControl("lblNotes");
        Label lblPrice = (Label)grdSelection.Rows[e.RowIndex].FindControl("lblPrice");

        TextBox txtDate = (TextBox)grdSelection.Rows[e.RowIndex].FindControl("txtDate");
        Label lblDate = (Label)grdSelection.Rows[e.RowIndex].FindControl("lblDate");

        Label lblVDate = (Label)grdSelection.Rows[e.RowIndex].FindControl("lblVDate");
        TextBox txtVDate = (TextBox)grdSelection.Rows[e.RowIndex].FindControl("txtVDate");

        DateTime dtValidTillDate = new DateTime();
        try
        {
            dtValidTillDate = Convert.ToDateTime(txtVDate.Text);
        }
        catch
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }



        DateTime dtCreateDate = new DateTime();
        try
        {
            dtCreateDate = Convert.ToDateTime(txtDate.Text);
        }
        catch
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }
        if (dtCreateDate >= dtValidTillDate)
        {
            lblResult2.Text = csCommonUtility.GetSystemRequiredMessage("Valid till date must be greater than selection date");
            return;
        }



        if (txtTitle.Text.Trim().Length != 0)
        {
            if (txtTitle.Text.Trim().Length > 499)
            {
                strRequired += "Title is Up to 500 Characters<br/>";
            }

        }
        else
        {
            strRequired += "Title is required<br/>";
        }
        if (txtNotes.Text.Trim().Length != 0)
        {

            if (txtTitle.Text.Trim().Length > 499)
            {
                strRequired += "Notes is Up to 500 Characters<br/>";
            }

        }
        else
        {
            strRequired += "Notes is required<br/>";
        }

        if (txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", "") != "")
        {
            try
            {
                nPrice = Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                nPrice = 0;
                //strRequired += "Price is required.<br/>";
                //txtPrice.BorderColor = System.Drawing.Color.Red;
            }
        }
        else
        {
            nPrice = 0;
            //strRequired += "Price is required.<br/>";
            //txtPrice.BorderColor = System.Drawing.Color.Red;
        }



        if (strRequired.Length > 0)
        {
            lblResult2.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
        }
        else
        {
            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

            Section_Selection objSS = _db.Section_Selections.SingleOrDefault(ss => ss.SectionSelectionID == nSectionSelectionID && ss.customer_id == Convert.ToInt32(hdnCustomerID.Value) && ss.estimate_id == Convert.ToInt32(hdnEstimateID.Value));

          //  objSS.estimate_id = Convert.ToInt32(ddlEst.SelectedValue);
            objSS.location_id = Convert.ToInt32(ddlLocation.SelectedValue);
            objSS.location_name = ddlLocation.SelectedItem.Text;
            objSS.section_id = Convert.ToInt32(ddlSectiong.SelectedValue);
            objSS.section_name = ddlSectiong.SelectedItem.Text;
            objSS.Title = txtTitle.Text.Trim();
            objSS.Price = nPrice;
            objSS.Notes = txtNotes.Text.Trim();
            objSS.LastUpdateDate = DateTime.Now;
            objSS.UpdateBy = User.Identity.Name;
            objSS.CreateDate = dtCreateDate;
            objSS.ValidTillDate = dtValidTillDate;

            _db.SubmitChanges();

            lblResult2.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
            GetSectionSelectionListData();
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
            userinfo objUser = (userinfo)Session["oUser"];
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

            Button btnDeleteUploadedFile = (Button)e.Row.FindControl("btnDeleteUploadedFile");
            btnDeleteUploadedFile.CommandArgument = nUploadFileId.ToString();



            if (isSelected)
            {

                if (objUser.role_id == 1 || objUser.role_id == 2)
                {
                    btnDeleteUploadedFile.Visible = true;

                }
                else
                {
                    btnDeleteUploadedFile.Visible = false;
                }

            }
            else
            {
                btnDeleteUploadedFile.Visible = true;
            }

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
                if (isSelected)
                    hypPDF.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypPDF.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
            
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
                if (isSelected)
                    hypDoc.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypDoc.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
               
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
                if (isSelected)
                    hypExcel.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypExcel.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
               
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
                if (isSelected)
                    hypTXT.NavigateUrl = "UploadedFiles/" + customer_id + "/SELECTIONS/" + file_name;
                else
                    hypTXT.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
           
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

            DataClassesDataContext _db = new DataClassesDataContext();

            Button btnDeleteUploadedFile = (Button)sender;
            int nUploadFileId = Convert.ToInt32(btnDeleteUploadedFile.CommandArgument);

            GridViewRow grdUploadedFileListRow = (GridViewRow)((Button)sender).NamingContainer;
            GridView grdUploadedFileList = (GridView)(grdUploadedFileListRow.Parent.Parent);

            GridViewRow grdSelectionRow = (GridViewRow)(grdUploadedFileList.NamingContainer);
            int Index = grdSelectionRow.RowIndex;

            GridView grdSelection = (GridView)(grdSelectionRow.Parent.Parent);
              bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);

              string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + hdnCustomerID.Value + "//SELECTIONS//";
             string sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//";
             string sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//Thumbnail//";


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
                    if (isSelected)
                    {
                        selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + hdnCustomerID.Value + "//SELECTIONS//" + file.ImageName;
                        File.Delete(selectionFolderPath);
                    }
                    else
                    {
                        sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//" + file.ImageName;
                        File.Delete(sFolderPath);
                    }
                   
                    sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//Thumbnail//" + file.ImageName;
                    File.Delete(sFileThumbnailPath);
                }

            }
            string strQ = "Delete file_upload_info WHERE upload_fileId=" + nUploadFileId + "  AND type = 5 AND client_id =" + Convert.ToInt32(hdnClientId.Value);
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
            GetSectionSelectionListData();

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
            int nCustId = Convert.ToInt32(hdnCustomerID.Value);
            int nEstId = Convert.ToInt32(hdnEstimateID.Value);

            DataClassesDataContext _db = new DataClassesDataContext();

            ImageButton imgbtngrdUpload = (ImageButton)sender;
            int nSectionSelectionID = Convert.ToInt32(imgbtngrdUpload.CommandArgument);


            GridViewRow grdSelectionRow = (GridViewRow)((ImageButton)sender).NamingContainer;
            int Index = grdSelectionRow.RowIndex;
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[Index].Values[5]);

            string strFileExt = "";
            string strRequired = string.Empty;


            FileUpload grdfile_upload = (FileUpload)grdSelection.Rows[Index].FindControl("grdfile_upload");
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

                        string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] +"\\" + nCustId + "\\SELECTIONS";
                        string sFileThumbnailPath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + nCustId + "\\SELECTIONS\\Thumbnail";
                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + nCustId + "\\SELECTIONS";

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

                        if (isSelected)
                        {
                            selectionFolderPath = selectionFolderPath + "\\" + sFileName;
                            file.SaveAs(selectionFolderPath);

                            // Thumbnail Image Save
                            if (strFileExt == ".jpg" || strFileExt == ".png" || strFileExt == ".jpeg")
                            {
                                ImageUtility.SaveThumbnailImage(selectionFolderPath, sFileThumbnailPath);
                            }
                        }
                        else
                        {
                            sFilePath = sFilePath + "\\" + sFileName;
                            file.SaveAs(sFilePath);

                            // Thumbnail Image Save
                            if (strFileExt == ".jpg" || strFileExt == ".png" || strFileExt == ".jpeg")
                            {
                                ImageUtility.SaveThumbnailImage(sFilePath, sFileThumbnailPath);
                            }
                        }

                        objfui.client_id = Convert.ToInt32(hdnClientId.Value);
                        objfui.CustomerId = nCustId;
                        objfui.estimate_id = nEstId;
                        objfui.Desccription = "";
                        objfui.ImageName = sFileName;
                        objfui.is_design = false;
                        objfui.IsSiteProgress = false;
                        objfui.type = 5; // Section Selection
                        objfui.vendor_cost_id = nSectionSelectionID;
                        _db.file_upload_infos.InsertOnSubmit(objfui);
                        _db.SubmitChanges();
                    }

                }


                string strMessage = csCommonUtility.GetSystemMessage("Data saved and file uploaded successfully");
                lblResult.Text = strMessage;
                GetSectionSelectionListData();

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
            if (nIndx < grdSelection.Rows.Count)
            {
                j = nIndx;
                grdSelection.Rows[j].Cells[9].Focus();
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

            ImageButton imgDelete = (ImageButton)sender;
            int nSectionSelectionID = Convert.ToInt32(imgDelete.CommandArgument);
            Section_Selection objS = _db.Section_Selections.SingleOrDefault(ss => ss.SectionSelectionID==nSectionSelectionID);

            strQ = "delete from Section_Selection where SectionSelectionID=" + nSectionSelectionID + " AND customer_id=" + Convert.ToInt32(hdnCustomerID.Value) + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateID.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            string selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + hdnCustomerID.Value + "//SELECTIONS//";
            string sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//";
            string sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//Thumbnail//";

            var upList = (from upl in _db.file_upload_infos
                          where upl.vendor_cost_id == nSectionSelectionID
                          select new
                          {
                              ImageName = upl.ImageName,
                          }).ToList();
            if (objS.isSelected==true)
            {
                if (Directory.Exists(sFolderPath))
                {

                    foreach (var file in upList)
                    {
                        selectionFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + "//" + hdnCustomerID.Value + "//SELECTIONS//" + file.ImageName;
                        File.Delete(selectionFolderPath);
                        sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//Thumbnail//" + file.ImageName;
                        File.Delete(sFileThumbnailPath);
                    }
                }
            }
            else
            {
                if (Directory.Exists(sFolderPath))
                {

                    foreach (var file in upList)
                    {
                        sFolderPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//" + file.ImageName;
                        File.Delete(sFolderPath);
                        sFileThumbnailPath = ConfigurationManager.AppSettings["UploadDir"] + "//" + hdnCustomerID.Value + "//SELECTIONS//Thumbnail//" + file.ImageName;
                        File.Delete(sFileThumbnailPath);
                    }
                }
            }
            strQ = "delete from file_upload_info where vendor_cost_id=" + nSectionSelectionID + " AND  estimate_id=" + Convert.ToInt32(hdnEstimateID.Value) + " AND type = 5";
            _db.ExecuteCommand(strQ, string.Empty);

            GetSectionSelectionListData();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemMessage("Data deleted successfully");
        }
        catch (Exception ex)
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    private DataTable LoadLocationTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("location_id", typeof(int));
        table.Columns.Add("location_name", typeof(string));

        return table;
    }

    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));

        return table;
    }

    //private DataTable LoadDataTable()
    //{
    //    DataTable table = new DataTable();
    //    table.Columns.Add("SectionSelectionID", typeof(int));
    //    table.Columns.Add("customer_id", typeof(int));
    //    table.Columns.Add("estimate_id", typeof(int));
    //    table.Columns.Add("section_id", typeof(int));
    //    table.Columns.Add("section_name", typeof(string));
    //    table.Columns.Add("location_id", typeof(int));
    //    table.Columns.Add("location_name", typeof(string));
    //    table.Columns.Add("Title", typeof(string));
    //    table.Columns.Add("Price", typeof(decimal));
    //    table.Columns.Add("Title", typeof(string));
    //    table.Columns.Add("Notes", typeof(string));
    //    table.Columns.Add("LastUpdateDate", typeof(DateTime));
    //    table.Columns.Add("UpdateBy", typeof(string));
    //    return table;
    //}

    protected void SaveSectionCOPricingMaster(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustId && cl.estimate_id == nEstId && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList().Count == 0)
        {
            List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == nEstId && cl.customer_id == nCustId && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
            List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == nEstId && cs.customer_id == nCustId && cs.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
            List<pricing_detail> Pm_List = _db.pricing_details.Where(pd => pd.estimate_id == nEstId && pd.customer_id == nCustId && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A").ToList();

            foreach (customer_location objcl in Cust_LocList)
            {
                changeorder_location co_loc = new changeorder_location();
                co_loc.client_id = objcl.client_id;
                co_loc.customer_id = objcl.customer_id;
                co_loc.location_id = objcl.location_id;
                co_loc.estimate_id = nEstId;
                _db.changeorder_locations.InsertOnSubmit(co_loc);
            }
            foreach (customer_section objcs in Cust_SecList)
            {
                changeorder_section co_sec = new changeorder_section();
                co_sec.client_id = objcs.client_id;
                co_sec.customer_id = objcs.customer_id;
                co_sec.section_id = objcs.section_id;
                co_sec.estimate_id = nEstId;
                co_sec.sales_person_id = objcs.sales_person_id;
                _db.changeorder_sections.InsertOnSubmit(co_sec);
            }
            foreach (pricing_detail objCpm in Pm_List)
            {
                co_pricing_master cpm = new co_pricing_master();
                cpm.client_id = objCpm.client_id; ;
                cpm.customer_id = objCpm.customer_id;
                cpm.estimate_id = nEstId;
                cpm.location_id = objCpm.location_id; ;
                cpm.sales_person_id = objCpm.sales_person_id;
                cpm.section_level = objCpm.section_level;
                cpm.item_id = objCpm.item_id;
                cpm.section_name = objCpm.section_name;
                cpm.item_name = objCpm.item_name;
                cpm.measure_unit = objCpm.measure_unit;
                cpm.minimum_qty = objCpm.minimum_qty;
                cpm.quantity = objCpm.quantity;
                cpm.retail_multiplier = objCpm.retail_multiplier;
                cpm.labor_id = objCpm.labor_id;
                cpm.is_direct = objCpm.is_direct;
                cpm.item_cost = objCpm.item_cost;
                cpm.total_direct_price = objCpm.total_direct_price;
                cpm.total_retail_price = objCpm.total_retail_price;
                cpm.labor_rate = objCpm.labor_rate;
                cpm.section_serial = objCpm.section_serial;
                cpm.item_cnt = objCpm.item_cnt;
                cpm.item_status_id = 1;
                cpm.short_notes = objCpm.short_notes;
                cpm.create_date = DateTime.Today;
                cpm.last_update_date = DateTime.Today;
                cpm.prev_total_price = 0;
                cpm.execution_unit = 0;
                cpm.week_id = 1;
                cpm.is_complete = false;
                cpm.schedule_note = "";
                cpm.sort_id = objCpm.sort_id;
                cpm.CalEventId = 0;
                cpm.is_CommissionExclude = objCpm.is_CommissionExclude;

                _db.co_pricing_masters.InsertOnSubmit(cpm);
            }
        }

        _db.SubmitChanges();
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Session["SelectionFilter"] != null)
        {
            Response.Redirect("GeneralSelectionReview.aspx");
        }
        else
        {
            Response.Redirect("customerlist.aspx");
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

    private void resetsave()
    {
        txtSelectionDate.Text = DateTime.Now.ToShortDateString();
        txtValidDate.Text = DateTime.Now.AddDays(7).ToShortDateString();
        ddlSection.SelectedIndex = -1;
        ddlLocation.SelectedIndex = -1;
        txtTitle.Text = "";
        txtPrice.Text = "";
        txtNotes.Text = "";
    }

    string CreateHtml()
    {
        string strFileHTML = "";
        DataTable dtFinal = null;
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='left'> " +
        "<tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + lblHeaderTitle.Text + "</p> " +
        "<table width='100%' border='0' cellspacing='3' cellpadding='5' > " +
        "<tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> " +
        "<tr style='background:#171f89;'></tr>" +
        "<tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> " +
        "<td width='10%'>Date</td>" +
        "<td width='10%'>Section</td>" +
        "<td width='10%'>Location</td>" +
        "<td width='15%'>Title</td><td width='15%'>Notes</td>" +
        "<td width='10%'>Price</td><td width='10%'>Days Left</td>" +
        "<td width='20%'></td></tr>";
        foreach (GridViewRow di in grdSelection.Rows)
        {
            int index = Convert.ToInt32(di.RowIndex);
            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[index].Values[0].ToString());
            CheckBox chkSelected = (CheckBox)di.FindControl("chkSelected");
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[index].Values[5]);

            if (chkSelected.Checked)
            {

                string strQ = "SELECT * FROM Section_Selection WHERE   isDeclined = 0 AND SectionSelectionID=" + nSectionSelectionID + " AND customer_id =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value) + "order by CreateDate DESC";
                dtFinal = csCommonUtility.GetDataTable(strQ);

                DataView dvFinal = dtFinal.DefaultView;
                //dvFinal.Sort = "ProjectNoteId,ProjectDate";



                for (int i = 0; i < dvFinal.Count; i++)
                {
                    DataRowView dr = dvFinal[i];

                    string str = string.Empty;
                    if (Convert.ToBoolean(dr["isSelected"]) == false || Convert.ToBoolean(dr["isSelected"]) == true)
                    {
                        DateTime dtCreateDate = Convert.ToDateTime(dr["CreateDate"]);
                        DateTime dtValidTill = Convert.ToDateTime(dr["ValidTillDate"]);
                        TimeSpan ts = dtValidTill - dtCreateDate;
                        int nDay = ts.Days;

                        string strDay = nDay.ToString() + " Day(s)";
                        if (nDay >= 0)
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
                                    string strFileName = drf["ImageName"].ToString().Replace(" ", "%20");
                                    if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                                    {
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
                                strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["CreateDate"]).ToShortDateString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["Title"].ToString() + "</td><td>" + dr["Notes"].ToString() + "</td><td>" + Convert.ToDecimal(dr["Price"]).ToString("c") + "</td><td>" + strDay + "</td><td>" + strFileHTML + "</td></tr>";
                            }
                        }
                    }

                }


            }
        }

        foreach (GridViewRow di in grdDeclinedSelection.Rows)
        {
            
            DataClassesDataContext _db = new DataClassesDataContext();
            int index = Convert.ToInt32(di.RowIndex);
            int nSectionSelectionID = Convert.ToInt32(grdDeclinedSelection.DataKeys[index].Values[0].ToString());
            CheckBox chkSelected = (CheckBox)di.FindControl("chkSelected");
            bool isSelected = Convert.ToBoolean(grdDeclinedSelection.DataKeys[index].Values[5]);
            bool isDeclined = Convert.ToBoolean(grdDeclinedSelection.DataKeys[index].Values[8]);

            if (chkSelected.Checked && isSelected == false && isDeclined == true)
            {

                string strQ = "SELECT * FROM Section_Selection WHERE  isSelected = 0 AND isDeclined = 1 AND SectionSelectionID=" + nSectionSelectionID + " AND customer_id =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value) + "order by CreateDate DESC";
                dtFinal = csCommonUtility.GetDataTable(strQ);

                DataView dvFinal = dtFinal.DefaultView;
                //dvFinal.Sort = "ProjectNoteId,ProjectDate";



                for (int i = 0; i < dvFinal.Count; i++)
                {
                    DataRowView dr = dvFinal[i];

                    string str = string.Empty;
                    if (Convert.ToBoolean(dr["isSelected"]) == false)
                    {
                        DateTime dtCreateDate = Convert.ToDateTime(dr["CreateDate"]);
                        DateTime dtValidTill = Convert.ToDateTime(dr["ValidTillDate"]);
                        TimeSpan ts = dtValidTill - dtCreateDate;
                        int nDay = ts.Days;

                        string strDay = nDay.ToString() + " Day(s)";
                        if (nDay >= 0)
                        {
                            Section_Selection objSS = _db.Section_Selections.SingleOrDefault(s => s.SectionSelectionID == nSectionSelectionID 
                                && s.customer_id == Convert.ToInt32(hdnCustomerID.Value) && s.estimate_id == Convert.ToInt32(hdnEstimateID.Value)
                                && s.isSelected == false && s.isDeclined ==true);

                            if (objSS != null)
                            {
                                objSS.isDeclined = false;                               
                                _db.SubmitChanges();
                            }

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
                                strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["CreateDate"]).ToShortDateString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["Title"].ToString() + "</td><td>" + dr["Notes"].ToString() + "</td><td>" + Convert.ToDecimal(dr["Price"]).ToString("c") + "</td><td>" + strDay + "</td><td>" + strFileHTML + "</td></tr>";
                            }
                        }
                    }

                }


            }

        }

        strHTML += "</table>";
        strHTML += "</table>";
        if (dtFinal != null)
            return strHTML;
        return "";

    }

    protected void btnEmailSelection_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnEmailSelection.ID, btnEmailSelection.GetType().Name, "Click"); 
        string strMessage = CreateHtml();
        Session.Add("MessBody", strMessage);
        string url = "window.open('sendemailoutlook.aspx?custId=" + hdnCustomerID.Value + "&ssfn=a&eid=" + hdnEstimateID.Value + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);

        GetSectionSelectionListData();
        GetDeclinedSectionSelectionListData();

    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, Button2.ID, Button2.GetType().Name, "Click"); 
        DataTable dtSelectionMaster = LoadSelectionMasterTable();
        DataTable dtSelectionImage = LoadSelectionImageTable();
        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow di in grdSelection.Rows)
        {
            int index = Convert.ToInt32(di.RowIndex);
            int nSectionSelectionID = Convert.ToInt32(grdSelection.DataKeys[index].Values[0].ToString());
            CheckBox chkSelected = (CheckBox)di.FindControl("chkSelected");
            bool isSelected = Convert.ToBoolean(grdSelection.DataKeys[index].Values[5]);

            if (isSelected == true)
            {

                string strQ = "SELECT SectionSelectionID, customer_id, estimate_id, " +
                             " CreateDate, section_name,location_name, Title,  Notes, Price,  isSelected, customer_signature, customer_siignatureDate, customer_signedBy,ValidTillDate, CreatedBy, UserEmail" +
                             " FROM Section_Selection " +
                             " WHERE  isSelected = 1 AND SectionSelectionID=" + nSectionSelectionID + " AND customer_id =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value) + "order by CreateDate DESC";
                DataTable dtFinal = csCommonUtility.GetDataTable(strQ);
                DataView dvFinal = dtFinal.DefaultView;
                //dvFinal.Sort = "ProjectNoteId,ProjectDate";

                for (int i = 0; i < dvFinal.Count; i++)
                {
                    DataRowView dr = dvFinal[i];
                    string str = string.Empty;
                    if (Convert.ToBoolean(dr["isSelected"]) == true)
                    {
                        DateTime dtCreateDate = Convert.ToDateTime(dr["CreateDate"]);
                        DateTime dtValidTill = Convert.ToDateTime(dr["ValidTillDate"]);
                        TimeSpan ts = dtValidTill - dtCreateDate;
                        int nDay = ts.Days;
                        string strDay = nDay.ToString() + " Day(s)";

                        DataRow drNew = dtSelectionMaster.NewRow();
                        drNew["SectionSelectionID"] = Convert.ToInt32(dr["SectionSelectionID"]);
                        drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                        drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                        drNew["CreateDate"] = Convert.ToDateTime(dr["CreateDate"]);
                        drNew["section_name"] = dr["section_name"].ToString();
                        drNew["location_name"] = dr["location_name"].ToString();
                        drNew["Title"] = dr["Title"].ToString();
                        drNew["Notes"] = dr["Notes"].ToString();
                        drNew["Price"] = Convert.ToDecimal(dr["Price"]);
                        drNew["DateDiff"] = strDay;





                        if (nDay >= 0)
                        {
                            string strF = "SELECT * FROM file_upload_info WHERE vendor_cost_id = " + Convert.ToInt32(dr["SectionSelectionID"]) + " AND type = 5 AND  CustomerId =" + Convert.ToInt32(hdnCustomerID.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateID.Value);
                            DataTable dtFile = csCommonUtility.GetDataTable(strF);
                            if (dtFile.Rows.Count > 0)
                            {
                                drNew["IsImage"] = 1;
                                foreach (DataRow drf in dtFile.Rows)
                                {
                                    string imgUrl = "";

                                    string strFileName = drf["ImageName"].ToString().Replace(" ", "%20");
                                    if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                                    {
                                        imgUrl = Server.MapPath("File\\" + hdnCustomerID.Value + "\\SELECTIONS\\Thumbnail\\" + strFileName);
                                    }
                                    else if (strFileName.Contains(".pdf"))
                                    {
                                        imgUrl = Server.MapPath("images\\icon_pdf.png");
                                    }
                                    else if (strFileName.Contains(".doc") || strFileName.Contains(".docx"))
                                    {
                                        imgUrl = Server.MapPath("images\\icon_docs.png");

                                    }
                                    else if (strFileName.Contains(".xls") || strFileName.Contains(".xlsx") || strFileName.Contains(".csv"))
                                    {
                                        imgUrl = Server.MapPath("images\\icon_excel.png");

                                    }
                                    else if (strFileName.Contains(".txt") || strFileName.Contains(".TXT"))
                                    {
                                        imgUrl = Server.MapPath("images\\icon_txt.png");
                                    }

                                    DataRow drNewF = dtSelectionImage.NewRow();
                                    drNewF["SectionSelectionID"] = Convert.ToInt32(drf["vendor_cost_id"]);
                                    drNewF["customer_id"] = Convert.ToInt32(drf["CustomerId"]);
                                    drNewF["estimate_id"] = Convert.ToInt32(drf["estimate_id"]);
                                    drNewF["ImageUrl"] = imgUrl.ToString();
                                    dtSelectionImage.Rows.Add(drNewF);

                                }

                            }
                            else
                            {
                                drNew["IsImage"] = 2;
                            }
                            dtSelectionMaster.Rows.Add(drNew);
                        }
                    }

                }


            }
        }

        ReportDocument rptFile = new ReportDocument();

        string strReportPath = Server.MapPath(@"Reports\rpt\rptSelectionSectionReport.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtSelectionMaster);

        ReportDocument subReport = rptFile.OpenSubreport("rptSubSelectionImage.rpt");
        subReport.SetDataSource(dtSelectionImage);
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerID.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateID.Value));


        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerID.Value));
        //string strCustomerName2 = cus_est.job_number ?? "";
        string strCustomerName2 = "";
        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
            strCustomerName2 = cus_est.job_number ?? "";
        else
            strCustomerName2 = cus_est.alter_job_number;
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;
        sales_person objsales = new sales_person();
        objsales = _db.sales_persons.FirstOrDefault(s => s.sales_person_id == Convert.ToInt32(objCust.sales_person_id));

        string strSalesPerson = objsales.first_name + ' ' + objsales.last_name;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_LeadTime", "");
        ht.Add("p_Contractdate", "");
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);
        ht.Add("p_date", "");
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_SalesRep", strSalesPerson);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }

    private DataTable LoadSelectionMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("SectionSelectionID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("CreateDate", typeof(DateTime));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("location_name", typeof(string));
        table.Columns.Add("Title", typeof(string));
        table.Columns.Add("Notes", typeof(string));
        table.Columns.Add("Price", typeof(decimal));
        table.Columns.Add("DateDiff", typeof(string));
        table.Columns.Add("IsImage", typeof(int));

        return table;
    }

    private DataTable LoadSelectionImageTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("SectionSelectionID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("ImageUrl", typeof(string));

        return table;
    }
    
    #region Declined Selections

    private void GetDeclinedSectionSelectionListData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nCustId = Convert.ToInt32(hdnCustomerID.Value);
        int nEstId = Convert.ToInt32(hdnEstimateID.Value);
        dtLocation = (DataTable)Session["ssLocation"];
        dtSection = (DataTable)Session["gSection"];

        string sSQL = " select SectionSelectionID,s.customer_id,s.section_id,s.estimate_id,location_id,isSelected,customer_signature,customer_siignatureDate, customer_signedBy,estimate_name,s.CreateDate,s.DeclinedDate,s.isDeclined, " +
                        " s.section_name,s.location_name,s.Title,s.Notes,s.Price,s.ValidTillDate " +
                       " from Section_Selection as s " +
                       " inner join customer_estimate as  ce on ce.customer_id=s.customer_id and ce.estimate_id=s.estimate_id " +
                       " where s.isDeclined=1 and s.customer_id=" + nCustId + " and s.estimate_id=" + nEstId +
                       " order by s.CreateDate DESC ";
        DataTable dt = csCommonUtility.GetDataTable(sSQL);
        //var sslist = _db.Section_Selections.Where(ss => ss.customer_id == nCustId && ss.estimate_id == nEstId && ss.isDeclined == true).ToList().OrderByDescending(c => c.CreateDate);

        grdDeclinedSelection.DataSource = dt;
        grdDeclinedSelection.DataKeyNames = new string[] { "SectionSelectionID", "customer_id", "section_id", "estimate_id", "location_id", "isSelected", "customer_signature", "DeclinedDate", "isDeclined" };
        grdDeclinedSelection.DataBind();

        if (grdDeclinedSelection.Rows.Count > 0)
        {
            lblDeclinedCount.Text = "(" + grdDeclinedSelection.Rows.Count + ")";
        }
        else
        {
            lblDeclinedCount.Text = "(0)";
        }
    }

    protected void grdDeclinedSelection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            userinfo objUser = (userinfo)Session["oUser"];

            int nSectionSelectionID = Convert.ToInt32(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int customer_id = Convert.ToInt32(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int section_id = Convert.ToInt32(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[2].ToString());
            int estimate_id = Convert.ToInt32(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int location_id = Convert.ToInt32(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[4].ToString());
            bool isSelected = Convert.ToBoolean(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[5]);
            string strCustomerSignature = grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string strDeclinedDate = Convert.ToDateTime(grdDeclinedSelection.DataKeys[e.Row.RowIndex].Values[7]).ToString("MM/dd/yyyy hh:mm tt");

            dtLocation = (DataTable)Session["ssLocation"];
            dtSection = (DataTable)Session["gSection"];
            FileUpload grdfile_upload = (FileUpload)e.Row.FindControl("grdfile_upload");

            // Image imgCustomerSign = e.Row.FindControl("imgCustomerSign") as Image;

            DropDownList ddlSectiong = (DropDownList)e.Row.FindControl("ddlSectiong");
            Label lblSectiong = (Label)e.Row.FindControl("lblSectiong");

            DropDownList ddlLocation = (DropDownList)e.Row.FindControl("ddlLocation");
            Label lblLocation = (Label)e.Row.FindControl("lblLocation");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");

            TextBox txtTitle = (TextBox)e.Row.FindControl("txtTitle");
            TextBox txtNotes = (TextBox)e.Row.FindControl("txtNotes");
            TextBox txtPrice = (TextBox)e.Row.FindControl("txtPrice");

            Label lblTitle = (Label)e.Row.FindControl("lblTitle");
            Label lblNotes = (Label)e.Row.FindControl("lblNotes");
            Label lblPrice = (Label)e.Row.FindControl("lblPrice");

            Label lblDeclined = (Label)e.Row.FindControl("lblDeclined");
            Label lblDeclinedDate = (Label)e.Row.FindControl("lblDeclinedDate");

            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");

            HtmlControl dvValidCalender = (HtmlControl)e.Row.FindControl("dvValidCalender");
            Label lblVDate = (Label)e.Row.FindControl("lblVDate");
            TextBox txtVDate = (TextBox)e.Row.FindControl("txtVDate");

            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            imgDelete.OnClientClick = "return confirm('Are you sure you want to delete this selection?');";
            imgDelete.CommandArgument = nSectionSelectionID.ToString();

            string str = lblNotes.Text.Replace("&nbsp;", "");

            ListItem items = ddlSectiong.Items.FindByValue(section_id.ToString());
            if (items != null)
                ddlSectiong.Items.FindByValue(Convert.ToString(section_id)).Selected = true;
            else
            {
                string strsecId = section_id.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                string strSection = _db.tbl_selection_secs.FirstOrDefault(l => l.selection_secid == section_id).section_name;
                ddlSectiong.Items.Insert(0, new ListItem(strSection, strsecId));
            }

            ddlSectiong.Visible = false;
            lblSectiong.Visible = true;

            ListItem item = ddlLocation.Items.FindByValue(location_id.ToString());
            if (item != null)
                ddlLocation.Items.FindByValue(Convert.ToString(location_id)).Selected = true;
            else
            {
                string strLocId = location_id.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                string strLocation = _db.locations.FirstOrDefault(l => l.location_id == location_id).location_name;
                ddlLocation.Items.Insert(0, new ListItem(strLocation, strLocId));
            }

            ddlLocation.Visible = false;
            lblLocation.Visible = true;

            LinkButton btn = (LinkButton)e.Row.Cells[8].Controls[0];
            //if (Convert.ToDecimal(lblPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
            //{
            //    div.Visible = true;
            //    txtDate.Visible = true;
            //    lblDate.Visible = false;


            //    txtTitle.Visible = true;
            //    lblTitle.Visible = false;

            //    txtNotes.Visible = true;
            //    lblNotes.Visible = false;


            //    txtPrice.Visible = true;
            //    lblPrice.Visible = false;

            //    ddlLocation.Enabled = true;
            //    ddlLocation.Visible = true;
            //    lblLocation.Visible = false;

            //    ddlSectiong.Enabled = true;
            //    ddlSectiong.Visible = true;
            //    lblSectiong.Visible = false;

            //    btn.Text = "Save";
            //    btn.CommandName = "Save";

            //}

            if (str != "" && str.Length > 60)
            {
                txtNotes.Text = str;
                lblNotes.Text = str.Substring(0, 60) + "...";
                lblNotes.ToolTip = str;
                lnkOpen.Visible = true;

            }
            else
            {
                txtNotes.Text = str;
                lblNotes.Text = str;
                lblNotes.ToolTip = str;
                lnkOpen.Visible = false;

            }


            btn.Attributes.Add("onclick", "ShowProgress();");

            if (nSectionSelectionID != 0)
            {
                GridView gvUp = e.Row.FindControl("grdDeclinedUploadedFileList") as GridView;
                GetUploadedFileListData(gvUp, nSectionSelectionID);
            }

            ImageButton imgbtngrdUpload = (ImageButton)e.Row.FindControl("imgbtngrdUpload");
            imgbtngrdUpload.CommandArgument = nSectionSelectionID.ToString();
            CheckBox chkSelected = (CheckBox)e.Row.FindControl("chkSelected");

            lblDeclined.Visible = true;
            lblDeclined.Text = "Declined";
            lblDeclinedDate.Text = "Date: " + strDeclinedDate;

            if (isSelected)
            {
                chkSelected.Visible = false;
                if (objUser.role_id == 1 || objUser.role_id == 2)
                {

                    btn.Visible = true;
                    imgDelete.Visible = true;
                    imgbtngrdUpload.Visible = true;
                    grdfile_upload.Visible = true;
                }
                else
                {
                    btn.Visible = false;
                    imgDelete.Visible = false;
                    imgbtngrdUpload.Visible = false;
                    grdfile_upload.Visible = false;
                }


            }
            else
            {
                chkSelected.Visible = true;
                btn.Visible = true;
                imgDelete.Visible = true;
                //lblSelected.Visible = false;
                //lblSelected.Text = "";
                //lblSelectionDate.Text = "";
                //lblSelectionDate.Visible = false;

            }

            //string imgSign = string.Format("data:image/jpeg;base64,{0}", strCustomerSignature);
            //if (imgSign != "data:image/jpeg;base64,")
            //{
            //    imgCustomerSign.ImageUrl = imgSign;
            //    imgCustomerSign.Visible = true;
            //}
            //else
            //{
            //    imgCustomerSign.Visible = false;
            //}
        }

    }

    protected void grdDeclinedSelection_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            if (e.CommandName == "Save")
            {

                int i = 0;
                lblResult2.Text = "";
                lblResult.Text = "";

                GridViewRow gvrnew = (GridViewRow)grdDeclinedSelection.NamingContainer;
                i = gvrnew.RowIndex;

                int nSectionId = Convert.ToInt32(ddlSection.SelectedValue);
                string strSectionName = ddlSection.SelectedItem.Text;


                bool IsRequiredPass = true;
                DataClassesDataContext _db = new DataClassesDataContext();
                dtLocation = (DataTable)Session["ssLocation"];
                dtSection = (DataTable)Session["gSection"];
                DataTable table = (DataTable)Session["sSection_Selection"];
                string strRequired = "";

                int nIndex = -1;
                foreach (GridViewRow di in grdDeclinedSelection.Rows)
                {
                    DropDownList ddlLocation = (DropDownList)di.FindControl("ddlLocation");

                    TextBox txtTitle = (TextBox)di.FindControl("txtTitle");
                    TextBox txtNotes = (TextBox)di.FindControl("txtNotes");
                    TextBox txtPrice = (TextBox)di.FindControl("txtPrice");

                    Label lblTitle = (Label)di.FindControl("lblTitle");
                    Label lblNotes = (Label)di.FindControl("lblNotes");
                    Label lblPrice = (Label)di.FindControl("lblPrice");

                    HtmlControl div = (HtmlControl)di.FindControl("dvCalender");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    TextBox txtDate = (TextBox)di.FindControl("txtDate");

                    HtmlControl dvValidCalender = (HtmlControl)di.FindControl("dvValidCalender");
                    Label lblVDate = (Label)di.FindControl("lblVDate");
                    TextBox txtVDate = (TextBox)di.FindControl("txtVDate");

                    DataRow dr = table.Rows[di.RowIndex];

                    DateTime dtCreateDate = new DateTime();
                    try
                    {
                        dtCreateDate = Convert.ToDateTime(txtDate.Text);
                    }
                    catch
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                        return;
                    }

                    DateTime dtValidTillDate = new DateTime();
                    try
                    {
                        dtValidTillDate = Convert.ToDateTime(txtVDate.Text);
                    }
                    catch
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                        return;
                    }
                    if (dtCreateDate >= dtValidTillDate)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Valid till date must be greater than selection date");
                        return;
                    }


                    if (txtTitle.Text.Trim().Length == 0)
                    {
                        strRequired += "Title is required.<br/>";
                        txtTitle.BorderColor = System.Drawing.Color.Red;
                    }

                    if (txtNotes.Text.Trim().Length == 0)
                    {
                        strRequired += "Notes is required.<br/>";
                        txtNotes.BorderColor = System.Drawing.Color.Red;
                    }

                    //if (Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
                    //{
                    //    strRequired += "Price is required.<br/>";
                    //    txtPrice.BorderColor = System.Drawing.Color.Red;
                    //}

                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        IsRequiredPass = false;
                    }

                    else
                    {

                        dr["SectionSelectionID"] = Convert.ToInt32(grdDeclinedSelection.DataKeys[di.RowIndex].Values[0]);
                        dr["customer_id"] = Convert.ToInt32(hdnCustomerID.Value);
                        dr["estimate_id"] = Convert.ToInt32(hdnEstimateID.Value);
                        dr["section_id"] = nSectionId;
                        dr["section_name"] = strSectionName;
                        dr["location_id"] = Convert.ToInt32(ddlLocation.SelectedValue);
                        dr["location_name"] = ddlLocation.SelectedItem.Text;
                        dr["Title"] = txtTitle.Text;
                        dr["Price"] = Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        dr["Notes"] = txtNotes.Text.Trim();
                        dr["LastUpdateDate"] = DateTime.Now;
                        dr["UpdateBy"] = User.Identity.Name;
                        dr["CreateDate"] = dtCreateDate;
                        dr["ValidTillDate"] = dtValidTillDate;

                        if (Convert.ToInt32(dr["SectionSelectionID"]) == 0)
                        {
                            nIndex = Convert.ToInt32(di.RowIndex);
                        }
                    }
                }
                if (IsRequiredPass)
                {
                    foreach (DataRow dr in table.Rows)
                    {


                        Section_Selection sec_sel = new Section_Selection();
                        if (Convert.ToInt32(dr["SectionSelectionID"]) > 0)
                            sec_sel = _db.Section_Selections.Single(l => l.SectionSelectionID == Convert.ToInt32(dr["SectionSelectionID"]));

                        if (Convert.ToDecimal(dr["cost_amount"]) != 0)
                        {
                            sec_sel.SectionSelectionID = Convert.ToInt32(dr["SectionSelectionID"]);
                            sec_sel.customer_id = Convert.ToInt32(dr["customer_id"]);
                            sec_sel.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                            sec_sel.section_id = Convert.ToInt32(dr["section_id"]);
                            sec_sel.section_name = dr["section_name"].ToString();
                            sec_sel.location_id = Convert.ToInt32(dr["location_id"]);
                            sec_sel.location_name = dr["location_name"].ToString();
                            sec_sel.Title = dr["Title"].ToString();
                            sec_sel.Price = Convert.ToDecimal(dr["Price"]);
                            sec_sel.Notes = dr["Notes"].ToString();
                            sec_sel.LastUpdateDate = DateTime.Now;
                            sec_sel.UpdateBy = User.Identity.Name;
                            sec_sel.CreateDate = Convert.ToDateTime(dr["CreateDate"]);
                            sec_sel.ValidTillDate = Convert.ToDateTime(dr["ValidTillDate"]);
                        }

                    }

                    lblResult2.Text = csCommonUtility.GetSystemMessage("Data saved successfully");

                    _db.SubmitChanges();



                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
        }
    }

    protected void grdDeclinedSelection_RowEditing(object sender, GridViewEditEventArgs e)
    {
        lblResult2.Text = "";
        lblResult.Text = "";

        DropDownList ddlLocation = (DropDownList)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("ddlLocation");
        DropDownList ddlSectiong = (DropDownList)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("ddlSectiong");

        Label lblLocation = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblLocation");
        Label lblSectiong = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblSectiong");

        TextBox txtTitle = (TextBox)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("txtTitle");
        TextBox txtNotes = (TextBox)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("txtNotes");
        TextBox txtPrice = (TextBox)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("txtPrice");

        Label lblTitle = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblTitle");
        Label lblNotes = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblNotes");
        Label lblPrice = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblPrice");

        HtmlControl div = (HtmlControl)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("dvCalender");
        Label lblDate = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblDate");
        TextBox txtDate = (TextBox)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("txtDate");

        HtmlControl dvValidCalender = (HtmlControl)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("dvValidCalender");
        Label lblVDate = (Label)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("lblVDate");
        TextBox txtVDate = (TextBox)grdDeclinedSelection.Rows[e.NewEditIndex].FindControl("txtVDate");



        txtTitle.Visible = true;
        lblTitle.Visible = false;

        string str = lblNotes.Text.Replace("&nbsp;", "");
        if (str.IndexOf(">>") != -1)
        {
            txtNotes.Visible = false;
            lblNotes.Visible = true;
        }
        else
        {
            txtNotes.Visible = true;
            lblNotes.Visible = false;
        }



        txtPrice.Visible = true;
        lblPrice.Visible = false;

        ddlLocation.Enabled = true;
        ddlLocation.Visible = true;
        lblLocation.Visible = false;

        ddlSectiong.Enabled = true;
        ddlSectiong.Visible = true;
        lblSectiong.Visible = false;

        div.Visible = true;
        txtDate.Visible = true;
        lblDate.Visible = false;

        dvValidCalender.Visible = true;
        txtVDate.Visible = true;
        lblVDate.Visible = false;

        LinkButton btn = (LinkButton)grdDeclinedSelection.Rows[e.NewEditIndex].Cells[8].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

        //Set Focus      
        ddlLocation.Focus();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }

    protected void grdDeclinedSelection_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = "";
        decimal nPrice = 0;
        DropDownList ddlLocation = (DropDownList)grdDeclinedSelection.Rows[e.RowIndex].FindControl("ddlLocation");
        DropDownList ddlSectiong = (DropDownList)grdDeclinedSelection.Rows[e.RowIndex].FindControl("ddlSectiong");

        TextBox txtTitle = (TextBox)grdDeclinedSelection.Rows[e.RowIndex].FindControl("txtTitle");
        TextBox txtNotes = (TextBox)grdDeclinedSelection.Rows[e.RowIndex].FindControl("txtNotes");
        TextBox txtPrice = (TextBox)grdDeclinedSelection.Rows[e.RowIndex].FindControl("txtPrice");

        Label lblTitle = (Label)grdDeclinedSelection.Rows[e.RowIndex].FindControl("lblTitle");
        Label lblNotes = (Label)grdDeclinedSelection.Rows[e.RowIndex].FindControl("lblNotes");
        Label lblPrice = (Label)grdDeclinedSelection.Rows[e.RowIndex].FindControl("lblPrice");

        TextBox txtDate = (TextBox)grdDeclinedSelection.Rows[e.RowIndex].FindControl("txtDate");
        Label lblDate = (Label)grdDeclinedSelection.Rows[e.RowIndex].FindControl("lblDate");

        Label lblVDate = (Label)grdDeclinedSelection.Rows[e.RowIndex].FindControl("lblVDate");
        TextBox txtVDate = (TextBox)grdDeclinedSelection.Rows[e.RowIndex].FindControl("txtVDate");

        DateTime dtValidTillDate = new DateTime();
        try
        {
            dtValidTillDate = Convert.ToDateTime(txtVDate.Text);
        }
        catch
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }



        DateTime dtCreateDate = new DateTime();
        try
        {
            dtCreateDate = Convert.ToDateTime(txtDate.Text);
        }
        catch
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }
        if (dtCreateDate >= dtValidTillDate)
        {
            lblResult2.Text = csCommonUtility.GetSystemRequiredMessage("Valid till date must be greater than selection date");
            return;
        }



        if (txtTitle.Text.Trim().Length != 0)
        {
            if (txtTitle.Text.Trim().Length > 499)
            {
                strRequired += "Title is Up to 500 Characters<br/>";
            }

        }
        else
        {
            strRequired += "Title is required<br/>";
        }
        if (txtNotes.Text.Trim().Length != 0)
        {

            if (txtTitle.Text.Trim().Length > 499)
            {
                strRequired += "Notes is Up to 500 Characters<br/>";
            }

        }
        else
        {
            strRequired += "Notes is required<br/>";
        }

        if (txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", "") != "")
        {
            try
            {
                nPrice = Convert.ToDecimal(txtPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                nPrice = 0;
                //strRequired += "Price is required.<br/>";
                //txtPrice.BorderColor = System.Drawing.Color.Red;
            }
        }
        else
        {
            nPrice = 0;
            //strRequired += "Price is required.<br/>";
            //txtPrice.BorderColor = System.Drawing.Color.Red;
        }



        if (strRequired.Length > 0)
        {
            lblResult2.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
        }
        else
        {
            int nSectionSelectionID = Convert.ToInt32(grdDeclinedSelection.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

            Section_Selection objSS = _db.Section_Selections.SingleOrDefault(ss => ss.SectionSelectionID == nSectionSelectionID && ss.customer_id == Convert.ToInt32(hdnCustomerID.Value) && ss.estimate_id == Convert.ToInt32(hdnEstimateID.Value));

            objSS.location_id = Convert.ToInt32(ddlLocation.SelectedValue);
            objSS.location_name = ddlLocation.SelectedItem.Text;
            objSS.section_id = Convert.ToInt32(ddlSectiong.SelectedValue);
            objSS.section_name = ddlSectiong.SelectedItem.Text;
            objSS.Title = txtTitle.Text.Trim();
            objSS.Price = nPrice;
            objSS.Notes = txtNotes.Text.Trim();
            objSS.LastUpdateDate = DateTime.Now;
            objSS.UpdateBy = User.Identity.Name;
            objSS.CreateDate = dtCreateDate;
            objSS.ValidTillDate = dtValidTillDate;

            _db.SubmitChanges();

            lblResult2.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
            GetDeclinedSectionSelectionListData();
        }

    }

    private void GetDeclinedUploadedFileListData(GridView grd, int nSectionSelectionID)
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

    protected void grdDeclinedUploadedFileList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdDeclinedUploadedFileList = (GridView)sender;
            userinfo objUser = (userinfo)Session["oUser"];
            GridViewRow grdDeclinedSelectionRow = (GridViewRow)(grdDeclinedUploadedFileList.NamingContainer);
            int Index = grdDeclinedSelectionRow.RowIndex;

            GridView grdDeclinedSelection = (GridView)(grdDeclinedSelectionRow.Parent.Parent);
            bool isSelected = Convert.ToBoolean(grdDeclinedSelection.DataKeys[Index].Values[5]);



            int nUploadFileId = Convert.ToInt32(grdDeclinedUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdDeclinedUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            int customer_id = Convert.ToInt32(hdnCustomerID.Value);


            string file_name = grdDeclinedUploadedFileList.DataKeys[e.Row.RowIndex].Values[3].ToString();


            string fileName = file_name.Substring(0, 10);
            Label lblFileName = (Label)e.Row.FindControl("lblFileName");
            lblFileName.Text = fileName;

            Button btnDeleteUploadedFile = (Button)e.Row.FindControl("btnDeleteUploadedFile");
            btnDeleteUploadedFile.CommandArgument = nUploadFileId.ToString();



            if (isSelected)
            {

                if (objUser.role_id == 1 || objUser.role_id == 2)
                {
                    btnDeleteUploadedFile.Visible = true;

                }
                else
                {
                    btnDeleteUploadedFile.Visible = false;
                }

            }
            else
            {
                btnDeleteUploadedFile.Visible = true;
            }

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
                hypPDF.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypDoc.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypExcel.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
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
                hypTXT.NavigateUrl = "File/" + customer_id + "/SELECTIONS/" + file_name;
                hypTXT.Target = "_blank";
                btnDeleteUploadedFile.OnClientClick = "return confirm('Are you sure you want to delete this Text File?');";
            }






        }
    }
    #endregion

    
    protected void btnSubmitSelection_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSelectionApproved.ID, btnSelectionApproved.GetType().Name, "Click");
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowProgress", "ShowProgress();", true);

        try
        {
            int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
            DataClassesDataContext _db = new DataClassesDataContext();

            customeruserinfo objCustInfo = (customeruserinfo)Session["oCustomerUser"];
            customer objCust = _db.customers.SingleOrDefault(s => s.customer_id == nCustomerId);

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
                            objSS.customer_signature = "";
                            objSS.customer_siignatureDate = DateTime.Now;
                            if (Session["oUser"] != null)
                            {
                                userinfo objUser = (userinfo)Session["oUser"];
                                objSS.customer_signedBy = objUser.first_name + " " + objUser.last_name;
                            }

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

                if (!bFlag)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");
                    lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Please check at least one selection.");

                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemMessage("Data save successfully");
                    lblResult2.Text = csCommonUtility.GetSystemMessage("Data save successfully");
                 

                   

                }

            }
            //string strMessage = CreateHtml();
            //Session.Add("MessBody", strMessage);
            //string url = "window.open('sendemailoutlook.aspx?custId=" + hdnCustomerID.Value + "&ssfn=a&eid=" + hdnEstimateID.Value + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
            //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);

            btnEmailSelection_Click(sender, e);
            GetSectionSelectionListData();
        }
        catch( Exception ex)
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
       
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }
    protected void ddlEst_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hdnEstimateID.Value = ddlEst.SelectedValue;
            LoadLocation();
            LoadSectionSec();
            GetSectionSelectionListData();
            GetDeclinedSectionSelectionListData();

        }
        catch(Exception ex)
        {
        }
    }
}