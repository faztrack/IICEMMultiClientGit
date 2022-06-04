using System;
using System.Collections;
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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Text.RegularExpressions;

public partial class compse_email : System.Web.UI.Page
{
    private string COName = "";
    private string strCustName = "";
    private string strEstName = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            imgCencel.Attributes.Add("onClick", "CloseWindow();");
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Request.QueryString.Get("custId") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
                hdnCustomerId.Value = ncid.ToString();
            }
            if (Request.QueryString.Get("sid") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("sid"));
                hdnSalesPersonId.Value = ncid.ToString();
            }
            if (Request.QueryString.Get("coid") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("coid"));
                hdnChEstId.Value = ncid.ToString();



            }
            if (Request.QueryString.Get("eid") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("eid"));
                hdnEstimateId.Value = ncid.ToString();
                customer_estimate cust_est = new customer_estimate();
                cust_est = _db.customer_estimates.Single(c => c.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                strEstName = cust_est.estimate_name;

            }            

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                txtTo.Text = cust.email;
                strCustName = cust.first_name1 + "" + cust.last_name1;
                hdnClientId.Value = cust.client_id.ToString();

                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == 1);

                    txtCc.Text = com.email;

                    //txtBcc.Text = "info@interiorinnovations.biz";
                    userinfo obj = (userinfo)Session["oUser"];
                    txtFrom.Text = obj.company_email;
                    CreateReportfor_Mail();
                }

            }

            changeorder_estimate cho = new changeorder_estimate();
            cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

            COName = cho.changeorder_name;
        }
    }
    private void CreateReportfor_Mail()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id AND change_order_pricing_list.client_id=location.client_id " +
                    " WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();

        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(rList);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));

        string strSalesPerson = "( " + sp.first_name + " " + sp.last_name + " )";

        string strpayment_terms = "";
        if (cho.payment_terms == "Other")
        {
            strpayment_terms = cho.other_terms.ToString();
        }
        else
        {
            strpayment_terms = cho.payment_terms.ToString();
        }

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComFax = oCom.fax;
        string strComEmail = oCom.email;
        string strComAddress = oCom.address;
        string strComCity = oCom.city;
        string strComState = oCom.state;
        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;
        COName = cho.changeorder_name;

        Hashtable ht = new Hashtable();

        ht.Add("p_CustomerName", strCustName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_changeorder_name", cho.changeorder_name);
        ht.Add("p_change_order_status_id", cho.change_order_status_id);
        ht.Add("P_change_order_type_id", cho.change_order_type_id);
        ht.Add("p_comments", cho.comments);
        ht.Add("p_payment_terms", strpayment_terms);
        ht.Add("p_is_total", cho.is_total);
        ht.Add("p_is_tax", cho.is_tax);
        ht.Add("p_tax", cho.tax);
        ht.Add("p_total_payment_due", cho.total_payment_due);
        ht.Add("p_changeorder_date", cho.changeorder_date);
        ht.Add("p_notes1", cho.notes1);
        ht.Add("p_ExecuteDate", cho.last_updated_date);
        ht.Add("p_EstimateName", strEstName);

        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyFax", strComFax);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_CompanyWeb", strComWeb);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        try
        {
            rptFile = (ReportDocument)Session[SessionInfo.Report_File];
            bool bParam = false;
            foreach (string strKey in Session.Keys)
            {
                if (strKey == SessionInfo.Report_Param)
                {
                    bParam = true;
                    break;
                }
            }
            if (bParam)
            {
                Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                ParameterValues param = new ParameterValues();
                ParameterDiscreteValue Val = new ParameterDiscreteValue();
                foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                {
                    if (htable.ContainsKey(obj.Name))
                    {
                        Val.Value = htable[obj.Name].ToString();
                        param.Add(Val);
                        obj.ApplyCurrentValues(param);
                    }
                }
            }
            CRViewer.ReportSource = rptFile;

            exportReport(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }
    }

    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            Send_Mail();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        string strQUP = "UPDATE changeorder_estimate SET is_cutomer_viewable=1 WHERE chage_order_id=" + hdnChEstId.Value + " AND estimate_id=" + hdnEstimateId.Value + " AND customer_id=" + hdnCustomerId.Value;
        _db.ExecuteCommand(strQUP, string.Empty);
        string url = "change_order_worksheet.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value;
        string Script = @"<script language=JavaScript>window.close('" + url + "'); opener.document.forms[0].submit(); </script>";
        if (!IsClientScriptBlockRegistered("OpenFile"))
            this.RegisterClientScriptBlock("OpenFile", Script);

    }
    protected void exportReport(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {
        try
        {

            string contentType = "";
            string strFile = "CO_" + DateTime.Now.Ticks;

            // string tempFileName = Request.PhysicalApplicationPath + "tmp\\ChangeOrder\\";
            string tempFileName = Server.MapPath("tmp\\ChangeOrder") + @"\" + strFile;
            switch (eft)
            {
                case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                    tempFileName = tempFileName + ".pdf";
                    contentType = "application/pdf";
                    break;
            }

            CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
            dfo.DiskFileName = tempFileName;
            CrystalDecisions.Shared.ExportOptions eo = selectedReport.ExportOptions;
            eo.DestinationOptions = dfo;
            eo.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
            eo.ExportFormatType = eft;
            selectedReport.Export();
            hdnTempfile.Value = tempFileName;
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            HyperLink hyp = new HyperLink();

            cell.BorderWidth = 0;

            hyp.Text = COName;
            // hyp.NavigateUrl = tempFileName;
            hyp.NavigateUrl = "tmp/ChangeOrder/" + strFile + ".pdf";
            hyp.Target = "_blank";
            cell.Controls.Add(hyp);
            cell.HorizontalAlign = HorizontalAlign.Left;
            row.Cells.Add(cell);
            tdLink.Rows.Add(row);

            //System.IO.File.Delete(tempFileName);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void Send_Mail()
    {
        try
        {
            string strFromEmail = txtFrom.Text;
            string strToEmail = txtTo.Text;
            string strCCEmail = txtCc.Text;

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (strFromEmail.Length > 4)
            {

                Match match1 = regex.Match(strFromEmail.Trim());
                if (!match1.Success)
                {
                    lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("From email address " + strFromEmail + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                    return;

                }
            }
            else
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("From email address is a required field");
                return;

            }

            if (strToEmail.Length > 4)
            {
                string[] strIds = strToEmail.Split(',');
                foreach (string strId in strIds)
                {
                    Match match1 = regex.Match(strId.Trim());
                    if (!match1.Success)
                    {
                        lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Recipient email address " + strId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                        return;

                    }
                }
            }
            else
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Recipient email address is a required field");
                return;

            }

            if (strCCEmail.Length > 4)
            {
                string[] strCCIds = strCCEmail.Split(',');
                foreach (string strCCId in strCCIds)
                {
                    Match match1 = regex.Match(strCCId.Trim());
                    if (!match1.Success)
                    {
                        lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("CC email address " + strCCId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                        return;

                    }
                }
            }
            string NewDir = Server.MapPath("~/tmp//ChangeOrder");

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(txtFrom.Text.ToString());
            msg.To.Add(txtTo.Text.ToString());
            if (txtCc.Text.Length > 4)
                msg.CC.Add(txtCc.Text.ToString());
            // msg.Bcc.Add(txtBcc.Text.ToString());
            msg.Subject = txtSubject.Text.ToString();
            msg.IsBodyHtml = true;

            msg.Body = txtBody.Text.ToString();
            msg.Priority = MailPriority.High;
            try
            {
                if (Directory.Exists(NewDir))
                {
                    string[] fileEntries = Directory.GetFiles(NewDir);
                    foreach (string fileName in fileEntries)
                    {
                        if (fileName == hdnTempfile.Value)
                            msg.Attachments.Add(new Attachment(fileName));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            csCommonUtility.SendByLocalhost(msg);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            //smtp.Send(msg);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
