using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class mProjectNotes : System.Web.UI.Page
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
                hdnCID.Value = Convert.ToInt32(Request.QueryString.Get("nbackId")).ToString();
            }
            if (Request.QueryString.Get("nestid") != null)
            {
                int neid = Convert.ToInt32(Request.QueryString.Get("nestid"));
                hdnEstimateId.Value = neid.ToString();
            }
            string ProjectNotesEmail = "";
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == 1).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == 1);

                    hdnProjectNotesEmail.Value = com.ProjectNotesEmail ?? "";
                    ProjectNotesEmail = hdnProjectNotesEmail.Value;
                }

                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                hdnLastName.Value = cust.last_name1;
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
                sap = _db.sales_persons.SingleOrDefault(c => c.sales_person_id == Convert.ToInt32(cust.sales_person_id) && c.is_active == true);
                if (sap != null)
                {
                    lblSalesPerson.Text = sap.first_name + " " + sap.last_name;
                    hdnSalesEmail.Value = sap.email;
                }
                string strSuperintendent = string.Empty;
                user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == cust.SuperintendentId && u.is_active==true);
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
            txtProjectDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
            LoadSectionSec();
            LoadProjectNoteInfo();
        }

    }

    private void LoadSectionSec()
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpSTable = LoadSectionTable();
        string sSQL = string.Empty;
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

        sSQL = " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from " +
                  " pricing_details " +
                   " inner join sectioninfo on pricing_details.section_level=sectioninfo.section_id  and pricing_details.customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and pricing_details.estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + "" +
                   " UNION " +
                   " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id  from " +
                  "  co_pricing_master" +
                  " inner join sectioninfo on co_pricing_master.section_level=sectioninfo.section_id " +
                 " UNION " +
                " select distinct sectioninfo.section_name as section_name,sectioninfo.section_id as section_id" +
                " from ProjectNoteInfo inner join sectioninfo  on ProjectNoteInfo.section_id=sectioninfo.section_id and ProjectNoteInfo.customer_id='" + Convert.ToInt32(hdnCustomerId.Value) + "'" +
                  " where ProjectNoteInfo.customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + "and parent_id=0 order by section_name ";
        DataTable td = csCommonUtility.GetDataTable(sSQL);
        foreach (DataRow drt in td.Rows)
        {
            DataRow drNew = tmpSTable.NewRow();
            drNew["section_id"] = drt["section_id"];
            drNew["section_name"] = drt["section_name"];
            tmpSTable.Rows.Add(drNew);
        }
        ddlSection.DataSource = tmpSTable;
        ddlSection.DataTextField = "section_name";
        ddlSection.DataValueField = "section_id";
        ddlSection.DataBind();
       
       


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
        //var item = from it in _db.ProjectNoteInfos
        //           where it.customer_id == Convert.ToInt32(hdnCustomerId.Value)
        //           orderby it.ProjectDate descending
        //           select new PrjectNoteInfrmation()
        //           {
        //               ProjectNoteId = (int)it.ProjectNoteId,
        //               customer_id = (int)it.customer_id,
        //               MaterialTrack=it.MaterialTrack,
        //               DesignUpdates=it.DesignUpdates,
        //               SuperintendentNotes=it.SuperintendentNotes,
        //               section_id=(int)it.section_id,
        //               SectionName=it.SectionName,
        //               isSOW=(bool)it.isSOW,
        //               NoteDetails = it.NoteDetails,
        //               is_complete = (bool)it.is_complete,
        //               ProjectDate = (DateTime)it.ProjectDate,
        //               CreateDate = (DateTime)it.CreateDate,

        //           };
        string sql = "select * from ProjectNoteInfo where  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " order by ProjectNoteId DESC ";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        Session.Add("ProjectNote", dt);
        grdProjectNote.PageSize = 200;
        grdProjectNote.DataSource = dt;
        grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "customer_id", "MaterialTrack", "DesignUpdates", "SuperintendentNotes", "section_id", "SectionName", "NoteDetails", "is_complete", "ProjectDate", "isSOW" };
        grdProjectNote.DataBind();

    }

    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        ResetValue();

       
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

    


    protected void imgSentEmail_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSentEmail.ID, imgSentEmail.GetType().Name, "Click"); 
        try
        {
            string strBody = CreateHtml();

            string strTO = string.Empty;
            string strCC = string.Empty;
            string strFrom = string.Empty;

            string strAddEmail = txtAdditionalEmail.Text;

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
                strTO = hdnSalesEmail.Value.ToString() + ", " + strAddEmail;
            }
            else
            {
                strTO = hdnSalesEmail.Value.ToString();
            }

            //string ProjectNotesEmail = hdnProjectNotesEmail.Value + "," + hdnSuperandentEmail.Value.ToString();

            //if (ProjectNotesEmail.Length > 4)
            //{
            //    string[] strCCIds = ProjectNotesEmail.Split(',');
            //    foreach (string strCCId in strCCIds)
            //    {
            //        Match match1 = regex.Match(strCCId.Trim());
            //        if (match1.Success)
            //        {
            //            strCC += strCCId + ",";

            //        }
            //    }
            //}
            //strCC = strCC.TrimEnd(',');

            strCC = hdnSuperandentEmail.Value.ToString();

            Match match = regex.Match(strCC);
            if (!match.Success)
                strCC = "";
            try
            {
                userinfo obj = new userinfo();
                if ((userinfo)Session["oUser"] != null)
                {
                    obj = (userinfo)Session["oUser"];
                    hdnEmailType.Value = "1";
                    strFrom = obj.company_email;

                }
                
                if (Convert.ToInt32(hdnEmailType.Value) == 1) // outlook email
                {

                    EmailAPI email = new EmailAPI();

                    string strUser = "";
                    strFrom = "alyons@azinteriorinnovations.com";
                    int ProtocolType = 1;
                    email.From = strFrom;

                    email.To = strTO;
                    email.CC = strCC;
                    email.BCC = "iisupport@faztrack.com";


                    email.Subject = "Project Notes for (" + lblCustomerName.Text + ") ";

                    email.Body = strBody;

                    email.UserName = strUser;

                    email.IsSaveEmailInDB = true;

                    email.ProtocolType = ProtocolType;

                    email.SendEmail();

                    lnkUpdateAddEmail_Click(sender, e);
                       
                }
                else
                {
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("alyons@azinteriorinnovations.com");

                    if (strTO != "")
                        msg.To.Add(strTO);
                    else
                    {
                        return;
                    }
                    if (strCC.Length > 3)
                    {
                        msg.CC.Add(strCC);
                    }
                    msg.Bcc.Add("iisupport@faztrack.com");

                    msg.Subject = "Project Notes for (" + lblCustomerName.Text + ") ";

                    msg.IsBodyHtml = true;

                    msg.Body = strBody;
                    msg.Priority = MailPriority.High;

                    csCommonUtility.SendByLocalhost(msg);

                    //SmtpClient smtp = new SmtpClient();
                    //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                    //smtp.Send(msg);

                    lnkUpdateAddEmail_Click(sender, e);
                    lblResult.Text = csCommonUtility.GetSystemMessage("Project Notes email sent successfully.");

                    msg.Dispose();
                }
            }
            catch { }

        }
        catch (Exception ex)
        {
            throw ex;
        }

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

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }


    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        if (Convert.ToInt32(hdnCID.Value) > 0)
            Response.Redirect("mcustomerlist.aspx");
        else
            Response.Redirect("mlandingpage.aspx");
    }


   
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMSG.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        ProjectNoteInfo objP = new ProjectNoteInfo();

        if (txtProjectDate.Text.Trim() != "")
        {
            try
            {
                Convert.ToDateTime(txtProjectDate.Text.Trim());
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
                lblMSG.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date.<br />");
                return;
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            lblMSG.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Date.<br />");
            return;
        }
        if (ddlSection.SelectedIndex == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            lblMSG.Text = csCommonUtility.GetSystemErrorMessage("Please Select Section.<br />");
            return;
        }
        if(txtMaterialTrack.Text.Trim()==""&&txtDesignUpdates.Text.Trim()==""&& txtSuperintendentNotes.Text.Trim()=="" &&txtGeneralNotes.Text.Trim()=="")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            lblMSG.Text = csCommonUtility.GetSystemErrorMessage("Missing Notes.<br />");
            return;
        }
        if (Convert.ToInt32(hdnProjectId.Value) > 0)
            objP = _db.ProjectNoteInfos.Single(c => c.ProjectNoteId == Convert.ToInt32(hdnProjectId.Value));

        objP.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        objP.ProjectDate = Convert.ToDateTime(txtProjectDate.Text);
        objP.MaterialTrack = txtMaterialTrack.Text.Trim();
        objP.DesignUpdates = txtDesignUpdates.Text.Trim();
        objP.SuperintendentNotes = txtSuperintendentNotes.Text.Trim();
        objP.section_id = Convert.ToInt32(ddlSection.SelectedValue);
        objP.SectionName = ddlSection.SelectedItem.Text;
        objP.NoteDetails = txtGeneralNotes.Text.Trim();
        //if (ddlSection.SelectedIndex == -1)
        //    objP.isSOW = false;
        //else
            objP.isSOW= Convert.ToBoolean(chkSOW.Checked);
        objP.is_complete = Convert.ToBoolean(chkComplete.Checked);
        
        if (Convert.ToInt32(hdnProjectId.Value) == 0)
        {
            objP.CreateDate = DateTime.Now;
            _db.ProjectNoteInfos.InsertOnSubmit(objP);
            _db.SubmitChanges();
            lblMSG.Text = csCommonUtility.GetSystemMessage("Project Notes has been saved successfully.");
        }
        else
        {
            _db.SubmitChanges();
            lblMSG.Text = csCommonUtility.GetSystemMessage("Project Notes has been updated successfully.");
        }
        btnSubmit.Focus();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
       
        LoadProjectNoteInfo();
    }

    protected void btnResetCId_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnResetCId.ID, btnResetCId.GetType().Name, "Click"); 
        hdnProjectId.Value = "0";
        ResetValue();
    }
    private void ResetValue()
    {
        txtMaterialTrack.Text = "";
        txtDesignUpdates.Text = "";
        txtSuperintendentNotes.Text = "";
        txtGeneralNotes.Text = "";
        chkComplete.Checked = false;
        chkSOW.Checked = true;
        txtProjectDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
        btnSubmit.Text = "Save";
        lblResult.Text = "";
        lblMSG.Text = "";
        ddlSection.SelectedIndex = 0;
       // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "#myModal", "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$('#myModal').hide();", true);

    }
    protected void grdProjectNote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
             int ProjectNoteId = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[0]);
             int customer_id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[1]);
            string MaterialTrack = grdProjectNote.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string DesignUpdates = grdProjectNote.DataKeys[e.Row.RowIndex].Values[3].ToString();
            string SuperintendentNotes = grdProjectNote.DataKeys[e.Row.RowIndex].Values[4].ToString();
            int section_id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[5]);
            string SectionName = grdProjectNote.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string NoteDetails = grdProjectNote.DataKeys[e.Row.RowIndex].Values[7].ToString();
            bool is_complete =Convert.ToBoolean(grdProjectNote.DataKeys[e.Row.RowIndex].Values[8]);
            DateTime ProjectDate = Convert.ToDateTime(grdProjectNote.DataKeys[e.Row.RowIndex].Values[9]);
            bool isSOW =Convert.ToBoolean(grdProjectNote.DataKeys[e.Row.RowIndex].Values[10]);



            Label lblDate = (Label)e.Row.FindControl("lblDate");
            LinkButton inkDate = (LinkButton)e.Row.FindControl("inkDate");
            inkDate.CommandArgument = ProjectNoteId.ToString();

            Label lblSection = (Label)e.Row.FindControl("lblSection");
            Label lblMaterialTrackNotes = (Label)e.Row.FindControl("lblMaterialTrack");
            Label lblUpdateDesingNotes = (Label)e.Row.FindControl("lblDesignUpdates");
            Label lblSuperNotes = (Label)e.Row.FindControl("lblSuperintendentNotes");
            Label lblGeneralNotes = (Label)e.Row.FindControl("lblDescription");
            Label lblSow = (Label)e.Row.FindControl("lblSow");
            Label lblCompleted = (Label)e.Row.FindControl("lblCompleted");


            Label lblMNotes = (Label)e.Row.FindControl("lblMNotes");
            Label lblDUNotes = (Label)e.Row.FindControl("lblDUNotes");
            Label lblSNotes = (Label)e.Row.FindControl("lblSNotes");
            Label lblGNotes = (Label)e.Row.FindControl("lblGNotes");


            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpenDescription");
            LinkButton lnkOpenMaterialTrack = (LinkButton)e.Row.FindControl("lnkOpenMaterialTrack");
            LinkButton lnkOpenDesignUpdates = (LinkButton)e.Row.FindControl("lnkOpenDesignUpdates");
            LinkButton lnkOpenSuperintendentNotes = (LinkButton)e.Row.FindControl("lnkOpenSuperintendentNotes");

            lblSection.Text = "<font style='font-weight:bold'>Section: </font>" + SectionName;

        

            if (MaterialTrack != "" && MaterialTrack.Length > 60)
            {
                lblMaterialTrackNotes.Text = MaterialTrack.Substring(0, 60) + "...";
                lblMaterialTrackNotes.ToolTip = MaterialTrack;
                lnkOpenMaterialTrack.Visible = true;

            }
            else
            {

                lblMaterialTrackNotes.Text = MaterialTrack;
                lblMaterialTrackNotes.ToolTip = MaterialTrack;
                lnkOpenMaterialTrack.Visible = false;

            }

            if (DesignUpdates != "" && DesignUpdates.Length > 60)
            {
                lblUpdateDesingNotes.Text =DesignUpdates.Substring(0, 60) + "...";
                lblUpdateDesingNotes.ToolTip = DesignUpdates;
                lnkOpenDesignUpdates.Visible = true;

            }
            else
            {

                lblUpdateDesingNotes.Text =  DesignUpdates;
                lblUpdateDesingNotes.ToolTip = DesignUpdates;
                lnkOpenDesignUpdates.Visible = false;

            }

            if (SuperintendentNotes != "" && SuperintendentNotes.Length > 60)
            {
                lblSuperNotes.Text = SuperintendentNotes.Substring(0, 60) + "...";
                lblSuperNotes.ToolTip = SuperintendentNotes;
                lnkOpenSuperintendentNotes.Visible = true;

            }
            else
            {

               // lblSuperNotes.Text = "<font style='font-weight:bold'>Superintendent Notes: </font>" + SuperintendentNotes;
                lblSuperNotes.ToolTip = SuperintendentNotes;
                lnkOpenSuperintendentNotes.Visible = false;

            }

            if (NoteDetails != "" && NoteDetails.Length > 60)
            {
                lblGeneralNotes.Text =  NoteDetails.Substring(0, 60) + "...";
                lblGeneralNotes.ToolTip = NoteDetails;
                lnkOpen.Visible = true;

            }
            else
            {

                lblGeneralNotes.Text =  NoteDetails;
                lblGeneralNotes.ToolTip = NoteDetails;
                lnkOpen.Visible = false;

            }

            if (isSOW)
            {
                lblSow.Text = "<font style='font-weight:bold'>Include in SOW: </font>" + "Yes";
            }
            else
            {
                lblSow.Text = "<font style='font-weight:bold'>Include in SOW: </font>" + "No";
            }

            if (is_complete)
            {
                

                inkDate.Visible = false;
                lblDate.Visible = true;
                lblCompleted.Text = "<font style='font-weight:bold'>Completed: </font>"+"Yes";
              
                lblDate.Text ="<font style='font-weight:bold;'>Date: </font>" +ProjectDate.ToString("MM-dd-yyyy");
                lblDate.Style.Add("color", "green");
                lblSection.Style.Add("color", "green");
                lblMaterialTrackNotes.Style.Add("color", "green");
                lblUpdateDesingNotes.Style.Add("color", "green");
                lblSuperNotes.Style.Add("color", "green");
                lblGeneralNotes.Style.Add("color", "green");
                lblCompleted.Style.Add("color", "green");
                lblSow.Style.Add("color", "green");

                lblMNotes.Style.Add("color", "green");
                lblDUNotes.Style.Add("color", "green");
                lblSNotes.Style.Add("color", "green");
                lblGNotes.Style.Add("color", "green");
            }
            else
            {
                inkDate.Visible = true;
                lblDate.Visible = false;
                inkDate.CommandArgument = ProjectNoteId.ToString();
                inkDate.Text = "<font style='font-weight:bold'>Date: </font>" + ProjectDate.ToString("MM-dd-yyyy");
                lblCompleted.Text ="<font style='font-weight:bold'>Completed: </font>" +"No";   
            }


            if (MaterialTrack.Length > 0)
            {
                lblMaterialTrackNotes.Visible = true;
                lblMNotes.Visible = true;
            }
            else
            {
                lblMaterialTrackNotes.Visible = false;
                lblMNotes.Visible = false;
            }

            if (DesignUpdates.Length > 0)
            {
                lblUpdateDesingNotes.Visible = true;
                lblDUNotes.Visible = true;
            }
            else
            {
                lblUpdateDesingNotes.Visible = false;
                lblDUNotes.Visible = false;
            }

            if (SuperintendentNotes.Length > 0)
            {
                lblSuperNotes.Visible = true;
                lblSNotes.Visible = true;
            }
            else
            {
                lblSuperNotes.Visible = false;
                lblSNotes.Visible = false;
            }

            if (NoteDetails.Length > 0)
            {
                lblGeneralNotes.Visible = true;
                lblGNotes.Visible = true;
            }
            else
            {
                lblGeneralNotes.Visible = false;
                lblGNotes.Visible = false;
            }


        }
    }
    protected void viewDetails(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            LoadSectionSec();
            LinkButton InkView = (LinkButton)sender;
            int nProjectId = Convert.ToInt32(InkView.CommandArgument);
            
            ProjectNoteInfo objP = _db.ProjectNoteInfos.SingleOrDefault(p => p.ProjectNoteId == nProjectId);
            if (objP!= null)
            {
                hdnProjectId.Value = objP.ProjectNoteId.ToString();
                txtMaterialTrack.Text = objP.MaterialTrack;
                txtDesignUpdates.Text = objP.DesignUpdates;
                txtSuperintendentNotes.Text = objP.SuperintendentNotes;
                txtGeneralNotes.Text = objP.NoteDetails;
                chkSOW.Checked =Convert.ToBoolean(objP.isSOW);
                chkComplete.Checked = Convert.ToBoolean(objP.is_complete);
                ddlSection.SelectedValue = objP.section_id.ToString();
                txtProjectDate.Text = objP.ProjectDate.ToString("MM-dd-yyyy");
                btnSubmit.Text = "Update";
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void grdProjectNote_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //LoadProjectNoteInfo(e.NewPageIndex);
    }

    protected void lnklnkOpenDescription_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
      
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDescription = gRow.Cells[0].Controls[0].FindControl("lblDescription") as Label;
        Label lblDescription_r = gRow.Cells[0].Controls[1].FindControl("lblDescription_r") as Label;
        LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpenDescription") as LinkButton;
        bool is_complete = Convert.ToBoolean(grdProjectNote.DataKeys[gRow.RowIndex].Values[8]);
        if (lnkOpen.Text == "More")
        {
            lblDescription.Visible = false;
            lblDescription_r.Visible = true;
            if (is_complete == true)
                lblDescription_r.Style.Add("color", "green");

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
        Label lblMaterialTrack = gRow.Cells[0].Controls[0].FindControl("lblMaterialTrack") as Label;
        Label lblMaterialTrack_r = gRow.Cells[0].Controls[1].FindControl("lblMaterialTrack_r") as Label;
        LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpenMaterialTrack") as LinkButton;

        bool is_complete = Convert.ToBoolean(grdProjectNote.DataKeys[gRow.RowIndex].Values[8]);

        if (lnkOpen.Text == "More")
        {
            lblMaterialTrack.Visible = false;
            lblMaterialTrack_r.Visible = true;

            if(is_complete==true)
                lblMaterialTrack_r.Style.Add("color", "green");


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
        Label lblDesignUpdates = gRow.Cells[0].Controls[0].FindControl("lblDesignUpdates") as Label;
        Label lblDesignUpdates_r = gRow.Cells[0].Controls[1].FindControl("lblDesignUpdates_r") as Label;
        LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpenDesignUpdates") as LinkButton;
        bool is_complete = Convert.ToBoolean(grdProjectNote.DataKeys[gRow.RowIndex].Values[8]);
        if (lnkOpen.Text == "More")
        {
            lblDesignUpdates.Visible = false;
            lblDesignUpdates_r.Visible = true;
            if (is_complete == true)
                lblDesignUpdates_r.Style.Add("color", "green");
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
        Label lblSuperintendentNotes = gRow.Cells[0].Controls[0].FindControl("lblSuperintendentNotes") as Label;
        Label lblSuperintendentNotes_r = gRow.Cells[0].Controls[1].FindControl("lblSuperintendentNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpenSuperintendentNotes") as LinkButton;

        bool is_complete = Convert.ToBoolean(grdProjectNote.DataKeys[gRow.RowIndex].Values[8]);
        if (lnkOpen.Text == "More")
        {
            lblSuperintendentNotes.Visible = false;
            lblSuperintendentNotes_r.Visible = true;
            if (is_complete == true)
                lblSuperintendentNotes_r.Style.Add("color", "green");
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

