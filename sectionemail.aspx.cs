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
using Prabhu;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;

public partial class sectionemail : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            imgCencel.Attributes.Add("onClick", "CloseWindow();");
            DataClassesDataContext _db = new DataClassesDataContext();
            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();
            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstId.ToString();
            int nsid = Convert.ToInt32(Request.QueryString.Get("sid"));
            hdnCallId.Value = nsid.ToString();
            int ndirectId = Convert.ToInt32(Request.QueryString.Get("directId"));
            hdnTypeId.Value = ndirectId.ToString();

            company_profile com = new company_profile();
            if (_db.company_profiles.Where(cp => cp.client_id == 1).SingleOrDefault() != null)
            {
                com = _db.company_profiles.Single(cp => cp.client_id == 1);

                txtCc.Text = com.email;
                if ((userinfo)Session["oUser"] != null)
                {
                    userinfo obj = (userinfo)Session["oUser"];
                    txtFrom.Text = obj.company_email;
                }
            }
            lblEmail.Visible = false;
            lblFEmail.Visible = false;

            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level)
                                      && p.item_status_id != 3 && p.section_level == nsid && p.is_direct == ndirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                               orderby p.week_id, p.section_level, p.execution_unit, lc.location_name ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = lc.location_name,
                                   section_name = p.section_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   tmpCol = string.Empty,
                                   week_id = (int)p.week_id,
                                   execution_unit = (decimal)p.execution_unit,
                                   is_complete = (bool)p.is_complete,
                                   schedule_note = p.schedule_note
                               };
            if (nsid < 1000)
            {
                price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level)
                                      && p.item_status_id != 3 && p.week_id == nsid && p.is_direct == ndirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                               orderby p.week_id, p.section_level, p.execution_unit, lc.location_name ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = lc.location_name,
                                   section_name = p.section_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   tmpCol = string.Empty,
                                   week_id = (int)p.week_id,
                                   execution_unit = (decimal)p.execution_unit,
                                   is_complete = (bool)p.is_complete,
                                   schedule_note = p.schedule_note
                               };

            }
            DataTable dt = SessionInfo.LINQToDataTable(price_detail);

            string Body = CreateHtml(dt, ncid);
            Editor1.Content = Body;

        }

    }
    string CreateHtml(DataTable dt, int ncid)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        // Customer Address
        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == ncid);
        string strCustomer = cust.first_name1 + " " + cust.last_name1;
        string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
        string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
        string strPO = "";

        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
        {
            customer_estimate cus_est = new customer_estimate();
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            strPO = cus_est.job_number;
        }

        string url = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

        DataView dvFinal = dt.DefaultView;
        dvFinal.Sort = "location_name,week_id";
        string strHTML = "<table width='680' border='0' cellspacing='0'cellpadding='0' align='center'> <tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Customer Name: " + strCustomer + "</p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'><a style='color:#000;' target='_blank' href='" + url + "'>Address: " + strAddress + "</a></p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Job Number: " + strPO + "</p> </td></tr>";
        strHTML += " <tr style='background-color:#330f02; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'><td align='left' valign='top'><table width='100%' border='0' cellspacing='1' cellpadding='5' > <tr style='background-color:#330f02; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='6%'>SL</td><td  width='12%' >Section</td><td width='12%'>Location</td><td width='40%'>Item Name</td><td width='5%'>UoM</td><td align='right'width='5%'>Code</td><td width='20%'>Short Notes</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string strColor = "";

            if (i % 2 == 0)
                strColor = "background-color:#f0eae8; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";

            strHTML += "<tr style='" + strColor + "'><td>" + dr["section_serial"].ToString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["item_name"].ToString() + "</td><td>" + dr["measure_unit"].ToString() + "</td><td align='right'>" + Convert.ToInt32(dr["quantity"]).ToString() + "</td><td>" + dr["short_notes"].ToString() + "</td></tr>";

        }
        strHTML += "</table> </td></tr>";
        strHTML += "</table>";

        return strHTML;
    }
    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        string strFromEmail = txtFrom.Text;
        string strToEmail = txtTo.Text;
        string strCCEmail = txtCc.Text;
        //string strCC2Email = txtCc2.Text;
        //string strBCCEmail = txtBcc.Text;

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
        //if (strCC2Email.Length > 4)
        //{
        //    string[] strCC2Ids = strCC2Email.Split(',');
        //    foreach (string strCC2Id in strCC2Ids)
        //    {
        //        Match match1 = regex.Match(strCC2Id.Trim());
        //        if (!match1.Success)
        //        {
        //            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("CC (Email 2) email address " + strCC2Id + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
        //            return;

        //        }
        //    }
        //}
        //if (strBCCEmail.Length > 4)
        //{
        //    string[] strBCCIds = strBCCEmail.Split(',');
        //    foreach (string strBCCId in strBCCIds)
        //    {
        //        Match match1 = regex.Match(strBCCId.Trim());
        //        if (!match1.Success)
        //        {
        //            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("BCC email address " + strBCCId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
        //            return;

        //        }
        //    }
        //}

        DataClassesDataContext _db = new DataClassesDataContext();
        string strpath = Request.PhysicalApplicationPath + "Uploads\\";
        strpath = strpath + "\\" + hdnCustomerId.Value + "\\Test";

        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) == 0)
        {
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            hdnEstimateId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = Convert.ToInt32(hdnEstimateId.Value);
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = txtTo.Text;
            cus_ms.mess_from = txtFrom.Text;
            cus_ms.mess_cc = txtCc.Text;
            cus_ms.mess_bcc = "";
            cus_ms.mess_description = Editor1.Content;
            cus_ms.mess_subject = txtSubject.Text;
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;

            _db.customer_messages.InsertOnSubmit(cus_ms);

        }
        _db.SubmitChanges();
        string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnEstimateId.Value);
        try
        {

            if (Directory.Exists(strpath))
            {
                // string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnMessageId.Value);
                if (!System.IO.Directory.Exists(NewDir))
                {
                    System.IO.Directory.CreateDirectory(NewDir);
                }
                string[] fileEntries = Directory.GetFiles(strpath);
                foreach (string file in fileEntries)
                {
                    string FileName = Path.GetFileName(file);
                    File.Move(file, Path.Combine(NewDir, FileName));
                    if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0)
                    {
                        message_upolad_info mui = new message_upolad_info();
                        if (_db.message_upolad_infos.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.message_id == Convert.ToInt32(hdnEstimateId.Value) && l.mess_file_name == FileName.ToString()).SingleOrDefault() == null)
                        {
                            mui.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                            mui.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            mui.message_id = Convert.ToInt32(hdnEstimateId.Value); ;
                            mui.mess_file_name = FileName.ToString();
                            mui.create_date = DateTime.Now;
                            _db.message_upolad_infos.InsertOnSubmit(mui);
                        }

                    }
                }
                _db.SubmitChanges();
            }
        }
        catch (Exception ex)
        {

        }



        MailMessage msg = new MailMessage();
        if (strFromEmail.Length > 4)
            msg.From = new MailAddress(strFromEmail);
        if (strToEmail.Length > 4)
            msg.To.Add(strToEmail);
        if (strCCEmail.Length > 4)
            msg.CC.Add(strCCEmail);
        //if (strCC2Email.Length > 4)
        //    msg.CC.Add(strCC2Email);
        //if (strBCCEmail.Length > 4)
        //    msg.Bcc.Add(strBCCEmail);
        msg.Subject = txtSubject.Text.ToString();
        msg.IsBodyHtml = true;
        msg.Body = Editor1.Content;
        msg.Priority = MailPriority.High;
        try
        {
            if (Directory.Exists(NewDir))
            {
                string[] fileEntries = Directory.GetFiles(NewDir);
                foreach (string fileName in fileEntries)
                {
                    msg.Attachments.Add(new Attachment(fileName));
                }
            }
        }
        catch (Exception ex)
        {

        }
        try
        {
            csCommonUtility.SendByLocalhost(msg);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            //smtp.Send(msg);

            string url = "customer_details.aspx?cid=" + hdnCustomerId.Value;
            string Script = @"<script language=JavaScript>window.close('" + url + "'); opener.document.forms[0].submit(); </script>";
            if (!IsClientScriptBlockRegistered("OpenFile"))
                this.RegisterClientScriptBlock("OpenFile", Script);

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        HttpFileCollection fileCollection = Request.Files;
        for (int i = 0; i < fileCollection.Count; i++)
        {
            string DestinationPath = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Test");

            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            HttpPostedFile uploadfile = fileCollection[i];
            // string fileName = Path.GetFileNameWithoutExtension(uploadfile.FileName);
            string fileName = "";
            string fileExt = Path.GetExtension(uploadfile.FileName);
            if (uploadfile.ContentLength > 0)
            {
                fileName = DateTime.Now.Ticks.ToString() + fileExt;
                uploadfile.SaveAs(Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Test//") + fileName);
                lblMessage.Text += fileName + "  Attachment(s) uploaded successfully<br>";
            }
            string[] fileEntries = Directory.GetFiles(DestinationPath);
            tdLink.Rows.Clear();
            foreach (string file in fileEntries)
            {
                string FileName = Path.GetFileName(file);
                //File.Delete(Path.Combine(NewDir, FileName));
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink hyp = new HyperLink();

                cell.BorderWidth = 0;
                hyp.Text = FileName;
                hyp.NavigateUrl = "Uploads/" + hdnCustomerId.Value + "/" + "Test" + "/" + FileName;
                hyp.Target = "_blank";
                cell.Controls.Add(hyp);
                cell.HorizontalAlign = HorizontalAlign.Left;
                row.Cells.Add(cell);
                tdLink.Rows.Add(row);

            }
        }
    }

}
