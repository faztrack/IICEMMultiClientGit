using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class ProjectNotes : System.Web.UI.Page
{
    public DataTable dtSection;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if ((userinfo)Session["oUser"] != null)
            {
                userinfo obj = (userinfo)Session["oUser"];
                hdnEmailType.Value = obj.EmailIntegrationType.ToString();               

            }
            if (Page.User.IsInRole("admin043") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
                hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();

            string ProjectNotesEmail = "";

            if (Request.QueryString.Get("eid") != null)
            {
                int neid = Convert.ToInt32(Request.QueryString.Get("eid"));
                hdnEstimateId.Value = neid.ToString();
            }

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                Session.Add("CustomerId", hdnCustomerId.Value);
                DataClassesDataContext _db = new DataClassesDataContext();
                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));


                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == cust.client_id).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == cust.client_id);

                    hdnProjectNotesEmail.Value = com.ProjectNotesEmail ?? "";
                    ProjectNotesEmail = hdnProjectNotesEmail.Value;
                }

                string strSecondName = cust.first_name2 + " " + cust.last_name2;
                if (strSecondName.Trim() == "")
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                else
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1 + " & " + strSecondName;

                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblPhone.Text = cust.phone;
                lblEmail.Text = cust.email;

                hdnSalesPersonId.Value = cust.sales_person_id.ToString();
                sales_person sap = new sales_person();
                sap = _db.sales_persons.SingleOrDefault(c => c.sales_person_id == Convert.ToInt32(cust.sales_person_id) && c.is_active==true);
                if (sap != null)
                {
                    lblSalesPerson.Text = sap.first_name + " " + sap.last_name;
                    hdnSalesEmail.Value = sap.email;
                }
                string strSuperintendent = string.Empty;
                user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == cust.SuperintendentId && u.is_active == true);
                if (uinfo != null)
                {
                    strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                    hdnSuperandentEmail.Value = uinfo.email;
                }
                lblSuperintendent.Text = strSuperintendent;

                ProjectNotesEmailInfo ObjPei = _db.ProjectNotesEmailInfos.SingleOrDefault(p => p.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                if (ObjPei != null)
                {
                    hdnAddEmailId.Value = ObjPei.ProjectNotesEmailID.ToString();
                    string strAddtionalEmail = ObjPei.AddtionalEmail;
                    string sryEmail = "";
                    if (strAddtionalEmail.Length > 4)
                    {
                        string[] strAdEmail = strAddtionalEmail.Split(',');
                        foreach (string strAdEmId in strAdEmail)
                        {

                            if (!ProjectNotesEmail.Contains(strAdEmId))
                            {
                                sryEmail += strAdEmId + ",";

                            }
                        }
                    }

                    if (sryEmail.Length > 3)
                    {
                        if (ProjectNotesEmail.Length > 3)
                            ProjectNotesEmail += ", " + sryEmail.TrimEnd(',');
                        else
                            ProjectNotesEmail = sryEmail.TrimEnd(',');
                    }

                    if (ProjectNotesEmail.Length > 3)
                    {
                        txtAdditionalEmail.Text = ProjectNotesEmail;
                        lblAdditionalEmail.Text = ProjectNotesEmail;
                        lnkEditAddEmail.Visible = true;
                        lblAdditionalEmail.Visible = true;
                        txtAdditionalEmail.Visible = false;
                        lnkUpdateAddEmail.Visible = false;
                    }
                    else
                    {
                        lnkEditAddEmail.Visible = false;
                        lblAdditionalEmail.Visible = false;
                        txtAdditionalEmail.Visible = true;
                        lnkUpdateAddEmail.Visible = false;

                    }
                }
            }
            LoadSectionSec();
            LoadProjectNoteInfo();



            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnAddnewRow", "imgSentEmail" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Edit", "Delete", "Save", "grdProjectNote_chkIsComplete", "grdProjectNote_grdfile_upload", "grdProjectNote_ddlSectiong", "grdProjectNote_imgbtngrdUpload", "grdProjectNote_chkSOW" });
        }

    }
    private void LoadSectionSec()
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        string sSQL=string.Empty;
        DataTable tmpSTable = LoadSectionTable();
        DataRow dr = tmpSTable.NewRow();
        dr["section_id"] = 0;
        dr["section_name"] = "Select Section";
        tmpSTable.Rows.Add(dr);

         dr = tmpSTable.NewRow();
        dr["section_id"] = -1;
        dr["section_name"] = "GENERAL";
        tmpSTable.Rows.Add(dr);
        dr = tmpSTable.NewRow();
        dr["section_id"] = 1;
        dr["section_name"] = "PROJECT PROGRESS";
        tmpSTable.Rows.Add(dr);
        dr = tmpSTable.NewRow();
        dr["section_id"] = 2;
        dr["section_name"] = "PAYMENT SCHEDULE";
        tmpSTable.Rows.Add(dr);
        //if (_db.changeorder_sections.Any(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])))
        //{
        //    sSQL = "select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from co_pricing_master inner join sectioninfo on co_pricing_master.section_level=sectioninfo.section_id " +
        //         " where customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " and parent_id=0 order by section_name ";
        //    DataTable td = csCommonUtility.GetDataTable(sSQL);
        //    foreach (DataRow drt in td.Rows)
        //    {
        //        DataRow drNew = tmpSTable.NewRow();
        //        drNew["section_id"] = drt["section_id"];
        //        drNew["section_name"] = drt["section_name"];
        //        tmpSTable.Rows.Add(drNew);
        //    }
        //}
        //else
        //{

        //    sSQL = "select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id from pricing_details inner join sectioninfo on pricing_details.section_level=sectioninfo.section_id " +
        //        " where customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " and parent_id=0 order by section_name ";
        //    DataTable td = csCommonUtility.GetDataTable(sSQL);
        //    foreach (DataRow drt in td.Rows)
        //    {
        //           DataRow drNew = tmpSTable.NewRow();
        //           drNew["section_id"] = drt["section_id"];
        //           drNew["section_name"] = drt["section_name"];
        //          tmpSTable.Rows.Add(drNew);
        //    }
           
        //}

            sSQL = " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from " +
                   " pricing_details "+
                    " inner join sectioninfo on pricing_details.section_level=sectioninfo.section_id  and pricing_details.customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and pricing_details.estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + "" +
                    " UNION "+
                    " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from "+
                   "  co_pricing_master"+ 
                   " inner join sectioninfo on co_pricing_master.section_level=sectioninfo.section_id "+
                  " UNION "+
                 " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id"+
                 " from ProjectNoteInfo inner join sectioninfo  on ProjectNoteInfo.section_id=sectioninfo.section_id and ProjectNoteInfo.customer_id='" + Convert.ToInt32(hdnCustomerId.Value) +"'" +
                   " where ProjectNoteInfo.customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + "and parent_id=0 order by section_name ";
                DataTable td = csCommonUtility.GetDataTable(sSQL);
                foreach (DataRow drt in td.Rows)
                {
                       DataRow drNew = tmpSTable.NewRow();
                       drNew["section_id"] = drt["section_id"];
                       drNew["section_name"] = drt["section_name"];
                      tmpSTable.Rows.Add(drNew);
                }

        Session.Add("gSection", tmpSTable);
        dtSection = (DataTable)Session["gSection"];
        

    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));

        return table;
    }
    private void LoadProjectNoteInfo()
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();
        var item = from it in _db.ProjectNoteInfos
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value)
                   orderby it.ProjectNoteId descending
                   select new PrjectNoteInfrmation()
                   {
                       ProjectNoteId = (int)it.ProjectNoteId,
                       customer_id = (int)it.customer_id,
                       section_id = (int)it.section_id,
                       NoteDetails = it.NoteDetails,
                       MaterialTrack = it.MaterialTrack,
                       DesignUpdates = it.DesignUpdates,
                       SuperintendentNotes = it.SuperintendentNotes,
                       isSOW = (bool)it.isSOW,
                       SectionName=it.SectionName,
                       is_complete = (bool)it.is_complete,
                       ProjectDate = (DateTime)it.ProjectDate,
                       CreateDate = (DateTime)it.CreateDate
                   };
         

        foreach (PrjectNoteInfrmation pinfo in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["ProjectNoteId"] = pinfo.ProjectNoteId;
            drNew["customer_id"] = pinfo.customer_id;
            drNew["NoteDetails"] = pinfo.NoteDetails;
            drNew["is_complete"] = pinfo.is_complete;
            drNew["ProjectDate"] = pinfo.ProjectDate;
            drNew["CreateDate"] = pinfo.CreateDate;
            drNew["MaterialTrack"] = pinfo.MaterialTrack;
            drNew["DesignUpdates"] = pinfo.DesignUpdates;
            drNew["SuperintendentNotes"] = pinfo.SuperintendentNotes;
            drNew["section_id"] = pinfo.section_id;
            drNew["isSOW"] = pinfo.isSOW;
            drNew["SectionName"] = pinfo.SectionName;
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["ProjectNoteId"] = 0;
            drNew["customer_id"] = hdnCustomerId.Value;
            drNew["NoteDetails"] = "";
            drNew["is_complete"] = false;
            drNew["ProjectDate"] = DateTime.Now;
            drNew["CreateDate"] = DateTime.Now;
            drNew["MaterialTrack"] = "";
            drNew["DesignUpdates"] = "";
            drNew["SuperintendentNotes"] = "";
            drNew["section_id"] = 0;
            drNew["isSOW"] = true;
            drNew["SectionName"] = "";
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("ProjectNote", tmpTable);
        grdProjectNote.DataSource = tmpTable;
        grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "section_id" };
        grdProjectNote.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("ProjectNoteId", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("NoteDetails", typeof(string));
        table.Columns.Add("is_complete", typeof(bool));
        table.Columns.Add("ProjectDate", typeof(DateTime));
        table.Columns.Add("CreateDate", typeof(DateTime));
        table.Columns.Add("MaterialTrack", typeof(string));
        table.Columns.Add("DesignUpdates", typeof(string));
        table.Columns.Add("SuperintendentNotes", typeof(string));
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("isSOW", typeof(bool));
        table.Columns.Add("SectionName", typeof(string));

        return table;
    }
    protected void grdProjectNote_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = (DataTable)Session["ProjectNote"];
            lblResult.Text = "";
            foreach (GridViewRow di in grdProjectNote.Rows)
            {
                {
                    CheckBox chkIsComplete = (CheckBox)di.FindControl("chkIsComplete");
                    TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                    Label lblDescription = (Label)di.FindControl("lblDescription");

                    TextBox txtDate = (TextBox)di.FindControl("txtDate");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    DataRow dr = table.Rows[di.RowIndex];

                    DateTime dtProjectDate = new DateTime();
                    try
                    {
                        dtProjectDate = Convert.ToDateTime(txtDate.Text);
                    }
                    catch
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
                        return;
                    }
                    TextBox txtMaterialTrack = (TextBox)di.FindControl("txtMaterialTrack");
                    Label lblMaterialTrack = (Label)di.FindControl("lblMaterialTrack");

                    TextBox txtDesignUpdates = (TextBox)di.FindControl("txtDesignUpdates");
                    Label lblDesignUpdates = (Label)di.FindControl("lblDesignUpdates");

                    TextBox txtSuperintendentNotes = (TextBox)di.FindControl("txtSuperintendentNotes");
                    Label lblSuperintendentNotes = (Label)di.FindControl("lblSuperintendentNotes");
                    DropDownList ddlSectiong = (DropDownList)di.FindControl("ddlSectiong");
                    CheckBox chkSOW = (CheckBox)di.FindControl("chkSOW");

                    if (ddlSectiong.SelectedIndex == 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please Select Section");
                        return;
                    }

                    string strDescription = txtDescription.Text.Trim().Replace("'", "''");
                    if (strDescription.Length == 0 && txtMaterialTrack.Text.Trim().Replace("'", "''").Length == 0 && txtDesignUpdates.Text.Trim().Replace("'", "''").Length == 0 && txtSuperintendentNotes.Text.Trim().Replace("'", "''").Length == 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Notes are required field");
                        return;

                    }


                    dr["ProjectNoteId"] = Convert.ToInt32(grdProjectNote.DataKeys[di.RowIndex].Values[0]);
                    dr["customer_id"] = hdnCustomerId.Value;
                    dr["NoteDetails"] = txtDescription.Text;
                    dr["is_complete"] = Convert.ToBoolean(chkIsComplete.Checked);
                    dr["ProjectDate"] = dtProjectDate;
                    dr["CreateDate"] = DateTime.Now;
                    dr["MaterialTrack"] = txtMaterialTrack.Text;
                    dr["DesignUpdates"] = txtDesignUpdates.Text;
                    dr["SuperintendentNotes"] = txtSuperintendentNotes.Text;
                    dr["section_id"] = Convert.ToInt32(ddlSectiong.SelectedValue);
                    dr["SectionName"] = ddlSectiong.SelectedItem.Text;
                    //if (ddlSectiong.SelectedIndex == -1)
                    //    dr["isSOW"] = false;
                    //else
                      dr["isSOW"] = Convert.ToBoolean(chkSOW.Checked);
                   
                }

            }
            int nProjectNoteId = 0;
            foreach (DataRow dr in table.Rows)
            {
                ProjectNoteInfo ProNInfo = new ProjectNoteInfo();
                nProjectNoteId = Convert.ToInt32(dr["ProjectNoteId"]);
                if (nProjectNoteId > 0)
                    ProNInfo = _db.ProjectNoteInfos.Single(l => l.ProjectNoteId == nProjectNoteId);
                //else if (_db.ProjectNoteInfos.Where(l => l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.NoteDetails == dr["NoteDetails"].ToString()).SingleOrDefault() != null)
                //{
                //    lblResult.Text = csCommonUtility.GetSystemErrorMessage("This Note  already exist.");
                //    return;
                //}
                string str = dr["NoteDetails"].ToString().Trim();
                
                if (str.Length > 1499)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Maximum length exceeded (1500 characters Max)");
                    return;

                }
                else if (dr["MaterialTrack"].ToString().Length > 1499)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Material track maximum length exceeded (1500 characters Max)");
                    return;

                }
                else if (dr["DesignUpdates"].ToString().Length > 1499)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Design updates maximum length exceeded (1500 characters Max)");
                    return;

                }
                else if (dr["SuperintendentNotes"].ToString().Length > 1499)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Superintendent notes maximum length exceeded (1500 characters Max)");
                    return;

                }
                ProNInfo.ProjectNoteId = nProjectNoteId;
                ProNInfo.customer_id = Convert.ToInt32(dr["customer_id"]);
                ProNInfo.NoteDetails = dr["NoteDetails"].ToString();
                ProNInfo.is_complete = Convert.ToBoolean(dr["is_complete"]);
                ProNInfo.ProjectDate = Convert.ToDateTime(dr["ProjectDate"]);
                ProNInfo.CreateDate = DateTime.Now;
                ProNInfo.MaterialTrack = dr["MaterialTrack"].ToString();
                ProNInfo.DesignUpdates = dr["DesignUpdates"].ToString();
                ProNInfo.SuperintendentNotes = dr["SuperintendentNotes"].ToString();
                ProNInfo.section_id = Convert.ToInt32(dr["section_id"]);
                ProNInfo.SectionName = dr["SectionName"].ToString();
                //if (ddlSectiong.SelectedIndex == -1)
                //    dr["isSOW"] = false;
                //else
                //    dr["isSOW"] = Convert.ToBoolean(chkSOW.Checked);
                ProNInfo.isSOW = Convert.ToBoolean(dr["isSOW"]);


                if (nProjectNoteId == 0)
                {
                    _db.ProjectNoteInfos.InsertOnSubmit(ProNInfo);
                }
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Project Notes saved successfully");
            _db.SubmitChanges();
            LoadSectionSec();
            LoadProjectNoteInfo();


        }
    }

    protected void grdProjectNote_RowEditing(object sender, GridViewEditEventArgs e)
    {
        HtmlControl div = (HtmlControl)grdProjectNote.Rows[e.NewEditIndex].FindControl("dvCalender");

        CheckBox chkIsComplete = (CheckBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("chkIsComplete");
        TextBox txtDescription = (TextBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblDescription");
        Label lblDescription_r = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblDescription_r");

        TextBox txtDate = (TextBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("txtDate");
        Label lblDate = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblDate");
        LinkButton lnkOpen = (LinkButton)grdProjectNote.Rows[e.NewEditIndex].FindControl("lnkOpen");

        TextBox txtMaterialTrack = (TextBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("txtMaterialTrack");
        Label lblMaterialTrack = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblMaterialTrack");
        Label lblMaterialTrack_r = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblMaterialTrack_r");
        LinkButton lnkOpenMaterialTrack = (LinkButton)grdProjectNote.Rows[e.NewEditIndex].FindControl("lnkOpenMaterialTrack");

        TextBox txtDesignUpdates = (TextBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("txtDesignUpdates");
        Label lblDesignUpdates = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblDesignUpdates");
        Label lblDesignUpdates_r = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblDesignUpdates_r");
        LinkButton lnkOpenDesignUpdates = (LinkButton)grdProjectNote.Rows[e.NewEditIndex].FindControl("lnkOpenDesignUpdates");

        TextBox txtSuperintendentNotes = (TextBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("txtSuperintendentNotes");
        Label lblSuperintendentNotes = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblSuperintendentNotes");
        Label lblSuperintendentNotes_r = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblSuperintendentNotes_r");
        LinkButton lnkOpenSuperintendentNotes = (LinkButton)grdProjectNote.Rows[e.NewEditIndex].FindControl("lnkOpenSuperintendentNotes");

        DropDownList ddlSectiong = (DropDownList)grdProjectNote.Rows[e.NewEditIndex].FindControl("ddlSectiong");
        Label lblSection = (Label)grdProjectNote.Rows[e.NewEditIndex].FindControl("lblSection");
        CheckBox chkSOW = (CheckBox)grdProjectNote.Rows[e.NewEditIndex].FindControl("chkSOW");
        div.Visible = true;
        txtDescription.Focus();
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        lblDescription_r.Visible = false;
        lnkOpen.Visible = false;
        txtDate.Visible = true;
        lblDate.Visible = false;
        chkIsComplete.Enabled = true;

        txtMaterialTrack.Visible = true;
        lblMaterialTrack.Visible = false;
        lblMaterialTrack_r.Visible = false;
        txtDesignUpdates.Visible = true;
        lblDesignUpdates.Visible = false;
        lblDesignUpdates_r.Visible = false;
        txtSuperintendentNotes.Visible = true;
        lblSuperintendentNotes.Visible = false;
        lblSuperintendentNotes_r.Visible = false;

        LinkButton btn = (LinkButton)grdProjectNote.Rows[e.NewEditIndex].Cells[8].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdProjectNote_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBox chkIsComplete = (CheckBox)grdProjectNote.Rows[e.RowIndex].FindControl("chkIsComplete");
        TextBox txtDescription = (TextBox)grdProjectNote.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdProjectNote.Rows[e.RowIndex].FindControl("lblDescription");

        TextBox txtDate = (TextBox)grdProjectNote.Rows[e.RowIndex].FindControl("txtDate");
        Label lblDate = (Label)grdProjectNote.Rows[e.RowIndex].FindControl("lblDate");

        TextBox txtMaterialTrack = (TextBox)grdProjectNote.Rows[e.RowIndex].FindControl("txtMaterialTrack");
        Label lblMaterialTrack = (Label)grdProjectNote.Rows[e.RowIndex].FindControl("lblMaterialTrack");


        TextBox txtDesignUpdates = (TextBox)grdProjectNote.Rows[e.RowIndex].FindControl("txtDesignUpdates");
        Label lblDesignUpdates = (Label)grdProjectNote.Rows[e.RowIndex].FindControl("lblDesignUpdates");


        TextBox txtSuperintendentNotes = (TextBox)grdProjectNote.Rows[e.RowIndex].FindControl("txtSuperintendentNotes");
        Label lblSuperintendentNotes = (Label)grdProjectNote.Rows[e.RowIndex].FindControl("lblSuperintendentNotes");

        DropDownList ddlSectiong = (DropDownList)grdProjectNote.Rows[e.RowIndex].FindControl("ddlSectiong");
        CheckBox chkSOW = (CheckBox)grdProjectNote.Rows[e.RowIndex].FindControl("chkSOW");
        bool isSOW = true;
      
        DateTime dtProjectDate = new DateTime();
        try
        {
            dtProjectDate = Convert.ToDateTime(txtDate.Text);
        }
        catch
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Date is not in correct format, Date format should be MM/DD/YYYY (Ex: " + DateTime.Now.ToShortDateString() + ")");
            return;
        }

        if (ddlSectiong.SelectedIndex == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please Select Section");
            return;
        }
        string strDescription = txtDescription.Text.Trim().Replace("'", "''");
        if (strDescription.Length == 0 && txtMaterialTrack.Text.Trim().Replace("'", "''").Length == 0 && txtDesignUpdates.Text.Trim().Replace("'", "''").Length == 0 && txtSuperintendentNotes.Text.Trim().Replace("'", "''").Length == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Notes is a required field");
            return;

        }
        else if (strDescription.Length > 1499)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("General notes maximum length exceeded (1500 characters Max)");
            return;

        }
        else if (txtMaterialTrack.Text.Length > 1499)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Material track maximum length exceeded (1500 characters Max)");
            return;

        }
        else if (txtDesignUpdates.Text.Length > 1499)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Design updates maximum length exceeded (1500 characters Max)");
            return;

        }
        else if (txtSuperintendentNotes.Text.Length > 1499)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Superintendent notes maximum length exceeded (1500 characters Max)");
            return;

        }

        //if (ddlSectiong.SelectedIndex == -1)
        //    isSOW = false;
        //else
           isSOW = Convert.ToBoolean(chkSOW.Checked);

        int nProjectNoteId = Convert.ToInt32(grdProjectNote.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE ProjectNoteInfo SET NoteDetails='" + strDescription + "',MaterialTrack='" + txtMaterialTrack.Text.Trim().Replace("'", "''") + "', DesignUpdates='" + txtDesignUpdates.Text.Trim().Replace("'", "''") + "', SuperintendentNotes='" + txtSuperintendentNotes.Text.Trim().Replace("'", "''") + "', ProjectDate='" + dtProjectDate + "' , is_complete ='" + Convert.ToBoolean(chkIsComplete.Checked) + "', isSOW ='" + isSOW + "', section_id ='" + Convert.ToInt32(ddlSectiong.SelectedValue) + "' , SectionName ='" + ddlSectiong.SelectedItem.Text + "'  WHERE ProjectNoteId=" + nProjectNoteId + "  AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        LoadSectionSec();
        LoadProjectNoteInfo();
        lblResult.Text = csCommonUtility.GetSystemMessage("Project Notes updated successfully");

    }

    protected void grdProjectNote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                int id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int section_id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[1].ToString());

                HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");
                CheckBox chkIsComplete = (CheckBox)e.Row.FindControl("chkIsComplete");
                TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");
                Label lblDescription = (Label)e.Row.FindControl("lblDescription");
                LinkButton btn = (LinkButton)e.Row.Cells[8].Controls[0];

                LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
                TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");
                Label lblDate = (Label)e.Row.FindControl("lblDate");

                TextBox txtMaterialTrack = (TextBox)e.Row.FindControl("txtMaterialTrack");
                Label lblMaterialTrack = (Label)e.Row.FindControl("lblMaterialTrack");
                LinkButton lnkOpenMaterialTrack = (LinkButton)e.Row.FindControl("lnkOpenMaterialTrack");

                TextBox txtDesignUpdates = (TextBox)e.Row.FindControl("txtDesignUpdates");
                Label lblDesignUpdates = (Label)e.Row.FindControl("lblDesignUpdates");
                LinkButton lnkOpenDesignUpdates = (LinkButton)e.Row.FindControl("lnkOpenDesignUpdates");

                TextBox txtSuperintendentNotes = (TextBox)e.Row.FindControl("txtSuperintendentNotes");
                Label lblSuperintendentNotes = (Label)e.Row.FindControl("lblSuperintendentNotes");
                LinkButton lnkOpenSuperintendentNotes = (LinkButton)e.Row.FindControl("lnkOpenSuperintendentNotes");


                DropDownList ddlSectiong = (DropDownList)e.Row.FindControl("ddlSectiong");
                Label lblSection = (Label)e.Row.FindControl("lblSection");
                CheckBox chkSOW = (CheckBox)e.Row.FindControl("chkSOW");
                if (section_id != 0)
                {
                ListItem items = ddlSectiong.Items.FindByValue(section_id.ToString());
               
                    if (items != null)
                        ddlSectiong.Items.FindByValue(Convert.ToString(section_id)).Selected = true;
                }
                if (chkSOW.Checked)
                {
                    chkSOW.Text = "Yes";
                }
                else
                {
                    chkSOW.Text = "No";
                }

                if (chkIsComplete.Checked)
                {
                    chkIsComplete.Text = "Yes";
                    e.Row.Attributes.CssStyle.Add("color", "green");
                    btn.Visible = false;
                    chkIsComplete.Enabled = false;

                    chkSOW.Enabled = false;
                    ddlSectiong.Visible = false;
                    lblSection.Visible = true;
                }
                else
                {
                    chkIsComplete.Text = "No";
                    btn.Visible = true;
                    chkIsComplete.Enabled = true;

                    chkSOW.Enabled = true;
                    ddlSectiong.Visible = true;
                    lblSection.Visible = false;
                }
                string str = txtDescription.Text.Replace("&nbsp;", "");
                if (str == "" && txtDescription.Text.Replace("&nbsp;", "") == "" && txtMaterialTrack.Text.Replace("&nbsp;", "") == "" && txtDesignUpdates.Text.Replace("&nbsp;", "") == "" && txtSuperintendentNotes.Text.Replace("&nbsp;", "") == "" || Convert.ToInt32(grdProjectNote.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]) == 0)
                {
                    txtDescription.Focus();
                    txtDescription.Visible = true;
                    lblDescription.Visible = false;

                    txtMaterialTrack.Visible = true;
                    lblMaterialTrack.Visible = false;

                    txtDesignUpdates.Visible = true;
                    lblDesignUpdates.Visible = false;

                    txtSuperintendentNotes.Visible = true;
                    lblSuperintendentNotes.Visible = false;

                    txtDate.Visible = true;
                    lblDate.Visible = false;
                    div.Visible = true;



                    btn.Text = "Save";
                    btn.CommandName = "Save";

                }


                chkIsComplete.Attributes["CommandArgument"] = string.Format("{0}", id);

                if (str != "" && str.Length > 90)
                {
                    lblDescription.Text = str.Substring(0, 90) + " ...";
                    lblDescription.ToolTip = str;
                    lnkOpen.Visible = true;
                }
                else
                {
                    lblDescription.Text = str;
                    lnkOpen.Visible = false;
                }

                if (txtMaterialTrack.Text != "" && txtMaterialTrack.Text.Length > 90)
                {
                    lblMaterialTrack.Text = txtMaterialTrack.Text.Substring(0, 90) + " ...";
                    lblMaterialTrack.ToolTip = txtMaterialTrack.Text;
                    lnkOpenMaterialTrack.Visible = true;
                }
                else
                {
                    lblMaterialTrack.Text = txtMaterialTrack.Text;
                    lnkOpenMaterialTrack.Visible = false;
                }

                if (txtDesignUpdates.Text != "" && txtDesignUpdates.Text.Length > 90)
                {
                    lblDesignUpdates.Text = txtDesignUpdates.Text.Substring(0, 90) + " ...";
                    lblDesignUpdates.ToolTip = txtDesignUpdates.Text;
                    lnkOpenDesignUpdates.Visible = true;
                }
                else
                {
                    lblDesignUpdates.Text = txtDesignUpdates.Text;
                    lnkOpenDesignUpdates.Visible = false;
                }

                if (txtSuperintendentNotes.Text != "" && txtSuperintendentNotes.Text.Length > 90)
                {
                    lblSuperintendentNotes.Text = txtSuperintendentNotes.Text.Substring(0, 90) + " ...";
                    lblSuperintendentNotes.ToolTip = txtSuperintendentNotes.Text;
                    lnkOpenSuperintendentNotes.Visible = true;
                }
                else
                {
                    lblSuperintendentNotes.Text = txtSuperintendentNotes.Text;
                    lnkOpenSuperintendentNotes.Visible = false;
                }

                csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdProjectNote_chkIsComplete", "grdProjectNote_chkSOW" });
            }
            catch(Exception ex)
            {
            }

        }

    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["ProjectNote"];

        foreach (GridViewRow di in grdProjectNote.Rows)
        {
            {

                CheckBox chkIsComplete = (CheckBox)di.FindControl("chkIsComplete");
                TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                Label lblDescription = (Label)di.FindControl("lblDescription");

                TextBox txtMaterialTrack = (TextBox)di.FindControl("txtMaterialTrack");
                Label lblMaterialTrack = (Label)di.FindControl("lblMaterialTrack");

                TextBox txtDesignUpdates = (TextBox)di.FindControl("txtDesignUpdates");
                Label lblDesignUpdates = (Label)di.FindControl("lblDesignUpdates");

                TextBox txtSuperintendentNotes = (TextBox)di.FindControl("txtSuperintendentNotes");
                Label lblSuperintendentNotes = (Label)di.FindControl("lblSuperintendentNotes");

                TextBox txtDate = (TextBox)di.FindControl("txtDate");
                Label lblDate = (Label)di.FindControl("lblDate");
                DropDownList ddlSectiong = (DropDownList)di.FindControl("ddlSectiong");
                CheckBox chkSOW = (CheckBox)di.FindControl("chkSOW");

                DataRow dr = table.Rows[di.RowIndex];
                dr["ProjectNoteId"] = Convert.ToInt32(grdProjectNote.DataKeys[di.RowIndex].Values[0]);
                dr["customer_id"] = hdnCustomerId.Value;
                dr["NoteDetails"] = txtDescription.Text;
                dr["is_complete"] = Convert.ToBoolean(chkIsComplete.Checked);
                dr["ProjectDate"] = Convert.ToDateTime(txtDate.Text);
                dr["CreateDate"] = DateTime.Now;
                dr["MaterialTrack"] = txtMaterialTrack.Text;
                dr["DesignUpdates"] = txtDesignUpdates.Text;
                dr["SuperintendentNotes"] = txtSuperintendentNotes.Text;
                dr["section_id"] = Convert.ToInt32(ddlSectiong.SelectedValue);
                dr["isSOW"] = Convert.ToBoolean(chkSOW.Checked);
                dr["SectionName"] = ddlSectiong.SelectedItem.Text;

            }

        }

        DataRow drNew = table.NewRow();
        drNew["ProjectNoteId"] = 0;
        drNew["customer_id"] = hdnCustomerId.Value;
        drNew["NoteDetails"] = "";
        drNew["is_complete"] = false;
        drNew["ProjectDate"] = DateTime.Now;
        drNew["CreateDate"] = DateTime.Now;
        drNew["MaterialTrack"] = "";
        drNew["DesignUpdates"] = "";
        drNew["SuperintendentNotes"] = "";
        drNew["section_id"] = 0;
        drNew["isSOW"] = true;
        drNew["SectionName"] = "";

        table.Rows.Add(drNew);
        DataView dv = table.DefaultView;
        dv.Sort = "ProjectDate desc";
        Session["ProjectNote"] = dv.ToTable();
        table = (DataTable)Session["ProjectNote"];
        //Session.Add("ProjectNote", table);
        LoadSectionSec();
        grdProjectNote.DataSource = table;
        grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "section_id" };
        grdProjectNote.DataBind();
        lblResult.Text = "";

    }

    protected void CheckUnCheckIsProcessed(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        CheckBox chkIsComplete = (CheckBox)sender;
        int nProjectNoteId = Convert.ToInt32(chkIsComplete.Attributes["CommandArgument"]);

        string strQ = string.Empty;

        try
        {
            if (chkIsComplete.Checked)
            {
                strQ = "UPDATE ProjectNoteInfo SET is_complete = 1  WHERE ProjectNoteId=" + nProjectNoteId + "  AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
            }
            else
            {
                strQ = "UPDATE ProjectNoteInfo SET is_complete = 0 WHERE ProjectNoteId=" + nProjectNoteId + "  AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
            }
        }
        catch
        {


        }

        _db.ExecuteCommand(strQ, string.Empty);
        lblResult.Text = csCommonUtility.GetSystemMessage("Project Notes mark as complete successfully");
        LoadSectionSec();
        LoadProjectNoteInfo();
    }

    string CreateHtml()
    {

        DataTable dtFinal = (DataTable)Session["ProjectNote"];
        DataView dvFinal = dtFinal.DefaultView;
        //dvFinal.Sort = "ProjectNoteId,ProjectDate";

        string strHTML = "<br/> <br/> <br/> <br/> <br/> <br/>";
        strHTML += "<table width='1200' border='0' cellspacing='0'cellpadding='0' align='center'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Customer Name: " + lblCustomerName.Text + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='5%'>Date</td><td width='10%'>Section</td><td width='18%'>Material Track</td><td width='18%'>Design Updates</td><td width='18%'>Superintendent Notes</td><td width='18%'>General Notes</td><td width='6%'>Completed?</td><td width='6%'>Include in SOW</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string str = string.Empty;
            if (Convert.ToBoolean(dr["is_complete"]) == true)
                str = "Yes";
            else
                str = "No";
            string strSOW = string.Empty;
            if (Convert.ToBoolean(dr["isSOW"]) == true)
                strSOW = "Yes";
            else
                strSOW = "No";


            string strColor = "";

            if (i % 2 == 0)
                strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            //if (dr["NoteDetails"].ToString().Length > 0)
            //{
            //    strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["ProjectDate"]).ToShortDateString() + "</td><td>" + dr["SectionName"].ToString() + "</td><td>" + dr["MaterialTrack"].ToString() + "</td><td>" + dr["DesignUpdates"].ToString() + "</td><td>" + dr["SuperintendentNotes"].ToString() + "</td><td>" + dr["NoteDetails"].ToString() + "</td><td>" + str + "</td><td>" + strSOW + "</td></tr>";
            //}
            strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["ProjectDate"]).ToShortDateString() + "</td><td>" + dr["SectionName"].ToString() + "</td><td>" + dr["MaterialTrack"].ToString() + "</td><td>" + dr["DesignUpdates"].ToString() + "</td><td>" + dr["SuperintendentNotes"].ToString() + "</td><td>" + dr["NoteDetails"].ToString() + "</td><td>" + str + "</td><td>" + strSOW + "</td></tr>";

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }



    //protected void imgStatement_Click(object sender, ImageClickEventArgs e)
    //{
    //    string strMessage = CreateHtml();
    //    Session.Add("MessBody", strMessage);
    //    string url = "ProjectNoteEmail.aspx?custId=" + hdnCustomerId.Value;
    //    string Script = @"<script language=JavaScript>window.open('" + url + "'); opener.document.forms[0].submit(); </script>";
    //    if (!IsClientScriptBlockRegistered("OpenFile"))
    //        this.RegisterClientScriptBlock("OpenFile", Script);

    //}
    protected void imgSentEmail_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSentEmail.ID, imgSentEmail.GetType().Name, "img Sent Email Click");
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dtFinal = (DataTable)Session["ProjectNote"];
            if (dtFinal.Rows.Count == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Blank data can't be emailed");
                return;
            }
            string strBody = CreateHtml();
            string strTO = string.Empty;
            string strAddEmail = txtAdditionalEmail.Text;
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
            if (strAddEmail.Length > 3)
            {
                string[] strIds = strAddEmail.Split(',');
                foreach (string strId in strIds)
                {
                    Match match1 = regex.Match(strId.Trim());
                    if (!match1.Success)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Additional email " + strId + " is not in correct format.");
                        return;

                    }
                }
                if (strTO.Length == 0)
                {
                    strTO = strAddEmail;
                }
                else
                {
                    strTO += ", " + strAddEmail;
                }
            }
            string strMessage = CreateHtml();
            Session.Add("MessBody", strMessage);
            Session.Add("strTO", strTO);
            string url = "window.open('sendemailoutlook.aspx?custId=" + hdnCustomerId.Value + "&pnote=pnote&eid=" + hdnEstimateId.Value + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);


           



        }
        catch (Exception ex)
        {
            throw ex;
        }

        //KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSentEmail.ID, imgSentEmail.GetType().Name, "Click"); 
        //try
        //{
        //    string strBody = CreateHtml();

        //    string strTO = string.Empty;
        //    string strCC = string.Empty;
        //    string strFrom = string.Empty;

        //    string strAddEmail = txtAdditionalEmail.Text;

        //    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //    if (strAddEmail.Length > 3)
        //    {
        //        string[] strIds = strAddEmail.Split(',');
        //        foreach (string strId in strIds)
        //        {
        //            Match match1 = regex.Match(strId.Trim());
        //            if (!match1.Success)
        //            {
        //                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Additional email " + strId + " is not in correct format.");
        //                return;

        //            }
        //        }
        //        strTO = hdnSalesEmail.Value.ToString() + ", " + strAddEmail;
        //    }
        //    else
        //    {
        //        strTO = hdnSalesEmail.Value.ToString();
        //    }

        //    //string ProjectNotesEmail = hdnProjectNotesEmail.Value + "," + hdnSuperandentEmail.Value.ToString();

        //    //if (ProjectNotesEmail.Length > 4)
        //    //{
        //    //    string[] strCCIds = ProjectNotesEmail.Split(',');
        //    //    foreach (string strCCId in strCCIds)
        //    //    {
        //    //        Match match1 = regex.Match(strCCId.Trim());
        //    //        if (match1.Success)
        //    //        {
        //    //            strCC += strCCId + ",";

        //    //        }
        //    //    }
        //    //}
        //    //strCC = strCC.TrimEnd(',');

        //    strCC = hdnSuperandentEmail.Value.ToString();

        //    Match match = regex.Match(strCC);
        //    if (!match.Success)
        //        strCC = "";
        //    try
        //    {
        //        userinfo obj = new userinfo();
        //        if ((userinfo)Session["oUser"] != null)
        //        {
        //            obj = (userinfo)Session["oUser"];
        //            hdnEmailType.Value = "1";// obj.EmailIntegrationType.ToString();
        //            strFrom = obj.company_email;

        //        }
        //        string sMessageUniqueId = "";
        //        if (Convert.ToInt32(hdnEmailType.Value) == 1) // outlook email
        //        {
                   

                       

        //                EmailAPI email = new EmailAPI();
                      
        //                string strUser = "";
        //                 strFrom = "alyons@azinteriorinnovations.com";
        //                int ProtocolType = 1;
        //                email.From = strFrom;

        //                email.To = strTO;
        //                 email.CC = strCC;
        //                email.BCC = "iisupport@faztrack.com";
                        

        //                email.Subject = "Project Notes for (" + lblCustomerName.Text + ") ";

        //                email.Body = strBody;

        //                email.UserName = strUser;

        //                email.IsSaveEmailInDB = true;

        //                email.ProtocolType = ProtocolType;

        //                email.SendEmail();

        //                lnkUpdateAddEmail_Click(sender, e);




                 
        //        }
        //        else
        //        {
        //            MailMessage msg = new MailMessage();
        //            msg.From = new MailAddress("alyons@azinteriorinnovations.com");

        //            if (strTO != "")
        //                msg.To.Add(strTO);
        //            else
        //            {
        //                return;
        //            }
        //            if (strCC.Length > 3)
        //            {
        //                msg.CC.Add(strCC);
        //            }
        //            msg.Bcc.Add("iisupport@faztrack.com");

        //            msg.Subject = "Project Notes for (" + lblCustomerName.Text + ") ";

        //            msg.IsBodyHtml = true;

        //            msg.Body = strBody;
        //            msg.Priority = MailPriority.High;

        //            csCommonUtility.SendByLocalhost(msg);

        //            //SmtpClient smtp = new SmtpClient();
        //            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
        //            //smtp.Send(msg);

        //            lnkUpdateAddEmail_Click(sender, e);
        //            lblResult.Text = csCommonUtility.GetSystemMessage("Project Notes email sent successfully.");

        //            msg.Dispose();
        //        }
        //    }
        //    catch { }

        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}

    }


    protected void lnkUpdateAddEmail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkUpdateAddEmail.ID, lnkUpdateAddEmail.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        ProjectNotesEmailInfo ObjPei = new ProjectNotesEmailInfo();

        if (Convert.ToInt32(hdnAddEmailId.Value) > 0)
            ObjPei = _db.ProjectNotesEmailInfos.SingleOrDefault(c => c.ProjectNotesEmailID == Convert.ToInt32(hdnAddEmailId.Value));

        ObjPei.ProjectNotesEmailID = Convert.ToInt32(hdnAddEmailId.Value);
        ObjPei.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        ObjPei.SalesPersonEmail = hdnSalesEmail.Value;
        ObjPei.SuperintendentEmail = hdnSalesEmail.Value;
        ObjPei.AddtionalEmail = txtAdditionalEmail.Text;
        ObjPei.LastUpdateDate = DateTime.Now;
        ObjPei.LastUpdateBy = User.Identity.Name;

        if (Convert.ToInt32(hdnAddEmailId.Value) == 0)
        {
            _db.ProjectNotesEmailInfos.InsertOnSubmit(ObjPei);
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
        }

        _db.SubmitChanges();
        hdnAddEmailId.Value = ObjPei.ProjectNotesEmailID.ToString();

        if (txtAdditionalEmail.Text != "")
        {
            lblAdditionalEmail.Text = txtAdditionalEmail.Text;
            lnkEditAddEmail.Visible = true;
            lblAdditionalEmail.Visible = true;
            txtAdditionalEmail.Visible = false;
            lnkUpdateAddEmail.Visible = false;
        }

    }
    protected void lnkEditAddEmail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkEditAddEmail.ID, lnkEditAddEmail.GetType().Name, "Click"); 
        lnkEditAddEmail.Visible = false;
        lblAdditionalEmail.Visible = false;
        txtAdditionalEmail.Visible = true;
        lnkUpdateAddEmail.Visible = true;

    }
    protected void grdProjectNote_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdProjectNote.ID, grdProjectNote.GetType().Name, "Click"); 
        DataTable dtProjectNote = (DataTable)Session["ProjectNote"];


        string strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtProjectNote.DefaultView;

        dv.Sort = strShort;

        if (hdnOrder.Value == "ASC")
            hdnOrder.Value = "DESC";
        else
            hdnOrder.Value = "ASC";

        Session["ProjectNote"] = dv.ToTable();

        dtProjectNote = (DataTable)Session["ProjectNote"];
        LoadSectionSec();
        Session.Add("ProjectNote", dtProjectNote);
        grdProjectNote.DataSource = dtProjectNote;
        grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "section_id" };
        grdProjectNote.DataBind();

    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        if (Request.QueryString.Get("TypeId") != null)
        {
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            if (nTypeId == 3)
                Response.Redirect("leadlist.aspx");
            else if (nTypeId == 2)
                Response.Redirect("customerlist.aspx");
            else if (nTypeId == 4)
                Response.Redirect("ProjectNotesReport.aspx");
            else
                Response.Redirect("leadlist.aspx");
        }
        else
            Response.Redirect("leadlist.aspx");
    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDescription = gRow.Cells[2].Controls[0].FindControl("lblDescription") as Label;
        Label lblDescription_r = gRow.Cells[2].Controls[1].FindControl("lblDescription_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblDescription.Visible = false;
            lblDescription_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDescription.Visible = true;
            lblDescription_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

    protected void lnkOpenMaterialTrack_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblMaterialTrack = gRow.Cells[2].Controls[0].FindControl("lblMaterialTrack") as Label;
        Label lblMaterialTrack_r = gRow.Cells[2].Controls[1].FindControl("lblMaterialTrack_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenMaterialTrack") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblMaterialTrack.Visible = false;
            lblMaterialTrack_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblMaterialTrack.Visible = true;
            lblMaterialTrack_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenDesignUpdates_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDesignUpdates = gRow.Cells[2].Controls[0].FindControl("lblDesignUpdates") as Label;
        Label lblDesignUpdates_r = gRow.Cells[2].Controls[1].FindControl("lblDesignUpdates_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenDesignUpdates") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblDesignUpdates.Visible = false;
            lblDesignUpdates_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDesignUpdates.Visible = true;
            lblDesignUpdates_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenSuperintendentNotes_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblSuperintendentNotes = gRow.Cells[2].Controls[0].FindControl("lblSuperintendentNotes_r") as Label;
        Label lblSuperintendentNotes_r = gRow.Cells[2].Controls[1].FindControl("lblSuperintendentNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenSuperintendentNotes") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblSuperintendentNotes.Visible = false;
            lblSuperintendentNotes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblSuperintendentNotes.Visible = true;
            lblSuperintendentNotes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
}