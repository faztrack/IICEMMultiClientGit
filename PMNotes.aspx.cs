using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PMNotes : System.Web.UI.Page
{
    public DataTable dtSection;
    int neid = 0, ncid = 0;
    DataClassesDataContext _db = new DataClassesDataContext();
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
                ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            }
            if (Request.QueryString.Get("nestid") != null)
            {
                neid = Convert.ToInt32(Request.QueryString.Get("nestid"));
                hdnEstimateId.Value = neid.ToString();
            }
            customer c = _db.customers.Where(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value)).FirstOrDefault();
            lblCustomereHeader.InnerText = " (" + c.last_name1 + ")";
            hdnCustomerName.Value = c.first_name1 + " " + c.last_name1;
            LoadSectionSec();
            BindDatatoGridView(0);

        }
    }


    protected void lnkEditNote_Click(object sender, EventArgs e)
    {
        try
        {

            GridViewRow gvrow = (GridViewRow)(((LinkButton)sender)).NamingContainer;
            HiddenFieldGrd.Value = grdPMnotes.DataKeys[gvrow.RowIndex].Value.ToString();
            int nPMNoteID = Convert.ToInt32(HiddenFieldGrd.Value);
            PMNoteInfo c = _db.PMNoteInfos.Where(pm => pm.PMNoteId == Convert.ToInt32(nPMNoteID)).FirstOrDefault();
            txtNote.Text = c.NoteDetails;
            ddlSection.SelectedValue = Convert.ToInt32(c.section_id).ToString();
            hdnPMnoteID.Value = c.PMNoteId.ToString();
            string strVendor_id = c.vendor_id;

            string[] crAry = strVendor_id.Split(',').Select(p => p.Trim()).ToArray();
            //make crew list for same customer
            // var crewList = _db.vendorDetails.Where(vd =>vd.vendor_id == Convert.ToInt32(crAry.Contains).ToList();
            var crewList = _db.Vendors.Where(g => crAry.Contains(g.vendor_id.ToString())).ToList();

            LoadVendor();
            foreach (ListItem li in chkVendors.Items)
            {
                foreach (Vendor vVen in crewList)
                {
                    if (vVen.vendor_id == Convert.ToInt32(li.Value.ToString()))
                        li.Selected = true;
                }
            }

            btnSubmit.Text = "Update";
            txtNote.Focus();
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void BindDatatoGridView(int nPageNo)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int CustomerId = Convert.ToInt32(hdnCustomerId.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateId.Value);
           
            string dataGridobj =" select distinct si.section_name, pii.customer_id,pii.PMNoteId, c.first_name1 + ' ' + c.last_name1 as CustomerName, pii.estimate_id,pii.section_id, pii.NoteDetails,pii.CreateDate,pii.CreatedBy,pii.vendor_id,pii.vendor_name ,"+
                " case when pii.IsComplete = 1 then 'Yes' else 'No' end as Complete from PMNoteInfo as pii inner join customers as c on c.customer_id = pii.customer_id"+
                " left join sectioninfo as si on si.section_id = pii.section_id where pii.customer_id='" + CustomerId + "' and pii.estimate_id='" + EstimateId + "' order by CreateDate desc";

            DataTable dt = csCommonUtility.GetDataTable(dataGridobj);
            grdPMnotes.DataSource = dt;
            grdPMnotes.DataKeyNames = new string[] { "PMNoteId", "customer_id", "NoteDetails", "estimate_id", "vendor_id", "vendor_name", "CreateDate", "CreatedBy", "section_id", "section_name", "Complete" };
            grdPMnotes.PageIndex = nPageNo;
            grdPMnotes.DataBind();
            Session.Add("PMNote", dt);
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    private void LoadSectionSec()
    {
        try
        {

            int CustomerId = Convert.ToInt32(hdnCustomerId.Value);
            int EstimateId = Convert.ToInt32(hdnEstimateId.Value);
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

            ddlSection.DataSource = dtSection; 
            ddlSection.DataValueField = "section_id";
            ddlSection.DataTextField = "section_name";
            ddlSection.DataBind();

        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void ddlSection_IndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSection.ID, ddlSection.GetType().Name, "IndexChanged"); 
        LoadVendor();

    }

    private void LoadVendor()
    {
        try
        {
            string sSQL = "Select a.Vendor_Id,b.vendor_name from vendor_section as a inner join Vendor AS b ON a.Vendor_Id=b.vendor_id  where section_id=" + ddlSection.SelectedValue + " and b.is_active=1";

            DataTable dt = csCommonUtility.GetDataTable(sSQL);
            if (dt.Rows.Count > 0)
            {
                lblVendorName.Visible = true;
            }
            else
            {
                lblVendorName.Visible = false;
            }
            chkVendors.DataSource = dt;
            chkVendors.DataTextField = "vendor_name";
            chkVendors.DataValueField = "vendor_id";
            chkVendors.DataBind();
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
 
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        try
        {
            int selectedCount = chkVendors.Items.Cast<ListItem>().Count(li => li.Selected);
            lblResultNote.Text = "";
            string vendorId = "";
            string vendorname = "";
            PMNoteInfo pmnoteObj = new PMNoteInfo();

            if (Convert.ToInt32(hdnPMnoteID.Value) != 0)
                pmnoteObj = _db.PMNoteInfos.SingleOrDefault(pmn => pmn.PMNoteId == Convert.ToInt32(hdnPMnoteID.Value));

            if (selectedCount > 0)
            {
                foreach (ListItem li in chkVendors.Items)
                {
                    if (li.Selected)
                    {
                        vendorId += li.Value + ",";
                        vendorname += li.Text + ", ";
                    }
                }
            }

            vendorId = vendorId.TrimEnd(',');
            vendorname = vendorname.TrimEnd(',');
            pmnoteObj.vendor_id = vendorId;
            pmnoteObj.vendor_name = vendorname;
            pmnoteObj.section_id = Convert.ToInt32(ddlSection.SelectedItem.Value);
            pmnoteObj.estimate_id = Convert.ToInt32(Request.QueryString.Get("nestid"));
            pmnoteObj.customer_id = Convert.ToInt32(Request.QueryString.Get("cid"));
            if (Session["oUser"] != null)
            {
                userinfo obj = (userinfo)Session["oUser"];
                pmnoteObj.CreatedBy = obj.first_name + ' ' + obj.last_name;
            }
            if (Session["oCrew"] != null)
            {
                Crew_Detail objCrew = (Crew_Detail)Session["oCrew"];
                pmnoteObj.CreatedBy = objCrew.full_name;
            }


            pmnoteObj.NoteDetails = txtNote.Text;

            if (Convert.ToInt32(hdnPMnoteID.Value) == 0)
            {
                if (txtNote.Text.Trim() == "" && /*Convert.ToInt32(ddlSection.SelectedItem.Value) == 0 &&*/ selectedCount == 0)
                {
                    lblResultNote.Text = csCommonUtility.GetSystemErrorMessage("Vendor Name or Notes needed");

                    return;
                }
                pmnoteObj.CreateDate = DateTime.Now;
                pmnoteObj.IsComplete = false;
                _db.PMNoteInfos.InsertOnSubmit(pmnoteObj);

                _db.SubmitChanges();
                // hdnPMnoteID.Value = pmnoteObj.PMNoteId.ToString();
            }
            else
            {
                if (txtNote.Text.Trim() == "" && /*Convert.ToInt32(ddlSection.SelectedItem.Value) == 0 &&*/ selectedCount == 0)
                {
                    lblResultNote.Text = csCommonUtility.GetSystemErrorMessage("Select atleast one field: Note.");

                    return;
                }
                lblResultNote.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");

                _db.SubmitChanges();
                btnSubmit.Text = "Submit";

            }

            clear();
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    public void clear()
    {
        txtNote.Text = "";
        ddlSection.SelectedIndex = 0;
        BindDatatoGridView(0);
        // chkVendors.Visible = false;
        foreach (ListItem li in chkVendors.Items)
        {
            li.Selected = false;

        }
        int tempval = 0;
        hdnPMnoteID.Value = tempval.ToString();
        LoadVendor();
        btnSubmit.Text = "Submit";
        lblResultNote.Text = "";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCancel.ID, btnCancel.GetType().Name, "Click"); 
        clear();

    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            if (Convert.ToInt32(hdnCID.Value) > 0)
                Response.Redirect("mcustomerlist.aspx");
            else
                Response.Redirect("mlandingpage.aspx");
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdPMnotes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdPMnotes.ID, grdPMnotes.GetType().Name, "PageIndexChanging"); 
        try
        {
            BindDatatoGridView(e.NewPageIndex);
        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void imgSentEmail_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSentEmail.ID, imgSentEmail.GetType().Name, "Click");
        try
        {
            if (txtToEmail.Text == "") {
                lblEmailAddress.Text = csCommonUtility.GetSystemErrorMessage("Email Required");
                ModalPopupExtender1.Show();
                return;
            }

            string strBody = CreateHtml();
            string strTO = txtToEmail.Text.Trim();
           Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            Match match = regex.Match(strTO);
            if (!match.Success)
                strTO = "";

            MailMessage msg = new MailMessage();
            if (Session["oUser"] != null)
            {
                userinfo obj = (userinfo)Session["oUser"];
                msg.From = new MailAddress(obj.email);
            }
            if (strTO != "")
                msg.To.Add(strTO);

        

            msg.Subject = "PM Notes: (" + hdnCustomerName.Value + ") ";
             msg.IsBodyHtml = true;
            msg.Body = strBody;
            msg.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            smtp.Send(msg);

            lblResultNote.Text = csCommonUtility.GetSystemMessage("PM Notes email sent successfully.");

        }
        catch (Exception ex)
        {
            lblResultNote.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        txtToEmail.Text = "";
    }

    string CreateHtml()
    {

        DataTable dtFinal = (DataTable)Session["PMNote"];
        DataView dvFinal = dtFinal.DefaultView;


        string strHTML = "<br/> <br/> <br/> <br/> <br/> <br/>";
        strHTML += "<table width='1200' border='0' cellspacing='0'cellpadding='0' align='center'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Customer Name: " + hdnCustomerName.Value + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='5%'>Date</td><td width='10%'>Section</td><td width='10%'>Vendor Name</td><td width='25%'>PM Notes</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];
            string strColor = "";
           // grdPMnotes.DataKeyNames = new string[] { "PMNoteId", "customer_id", "NoteDetails", "estimate_id", "vendor_id", "vendor_name", "CreateDate", "CreatedBy", "section_id", "section_name" };
            if (i % 2 == 0)
                strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";

            strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["CreateDate"]).ToShortDateString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["vendor_name"].ToString() + "</td><td>" + dr["NoteDetails"].ToString() + "</td></tr>";

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }




}