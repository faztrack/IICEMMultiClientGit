using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pendingpayment : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        return (from c in _db.customers
                where c.isCustomer == 1 && c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();

    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("cus002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            
            BindDueCustomers();


            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdDueCustomers_lnkReminderEmail", "grdDueCustomers_lnkSendSMS" });
        }
        if (Session["FromPendingPaymentPage"] != null)
        {
            GetDueCustomers(0);
            Session.Remove("FromPendingPaymentPage");
        }

    }




   private void BindDueCustomers()
    {
        try
        {
          
            string strCondition = "";
            DateTime todays = DateTime.Now;
            if (ddlEstimateStatus.SelectedItem.Text != "All")
            {
                strCondition += " AND ce.IsEstimateActive =" + Convert.ToInt32(ddlEstimateStatus.SelectedValue);
            }
            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {

                    strCondition += " AND c.status_id NOT IN(4,5,7)  ";
                }
                else
                {
                    strCondition += " AND c.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                }

            }

            if (txtSearch.Text != "")
            {
                strCondition += " and c.last_name1 LIKE '%" + txtSearch.Text.ToString().Replace("'", "''") + "%'";
            }
            if (Convert.ToInt32(rabDays.SelectedValue)==4)
            {
               
                strCondition+= "  AND sc.event_start < '" + todays.AddDays(-90).ToString("MM/dd/yyyy") + "'";
            }
            else
            {
                int days = Convert.ToInt32(rabDays.SelectedValue);
                strCondition+= " and sc.event_start >= '" + todays.AddDays(-days).ToString("MM/dd/yyyy") + "' AND sc.event_start < '" + todays.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }
       
            string Sql = " SELECT sc.event_id, c.customer_id, c.first_name1, c.last_name1,  c.first_name1+' '+c.last_name1 AS customer_name, '(' + c.last_name1 + ') ' + sc.title AS title, sc.employee_name, sc.estimate_id, " +
                              " CASE WHEN c.Phone = '' THEN c.mobile ELSE c.Phone END AS mobile, sc.event_start, sc.event_end, sc.section_name, sc.location_name, sc.IsScheduleDayException, ISNULL(u.first_name, '') AS superfirstname," +
                              " ISNULL(u.last_name, '') AS superlastname, ISNULL(u.phone, '') AS supermobile, c.address + ', ' + c.city + ', ' + c.state + ', ' + c.zip_code AS CustAddress, ce.estimate_name,s.email as salespersonemail, " +
                              " u.email as useremail,c.email as customeremail,isnull(ce.sale_date,'01-01-1900') as sale_date," +
                              " case when ce.alter_job_number != '' then ce.alter_job_number  " +
                              " else   ce.job_number end as job_number " +
                              " FROM ScheduleCalendar AS sc INNER JOIN " +
                              " customers AS c ON sc.customer_id = c.customer_id INNER JOIN " +
                              " customer_estimate AS ce ON ce.customer_id = sc.customer_id AND ce.estimate_id = sc.estimate_id " +
                              " inner join sales_person as s on s.sales_person_id = c.sales_person_id " +
                              
                              " LEFT OUTER JOIN " +
                              " user_info AS u ON c.SuperintendentId = u.user_id " +
                              " WHERE ce.status_id=3 and  sc.cssClassName ='fc-white fc-PaymentTerms'" + strCondition+
                              " ORDER BY c.first_name1 ";
            DataTable table = DataReader.Complex_Read_DataTable(Sql);
        

          
  
            CreateTable(table);
           
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

   

    
    protected void rabDays_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rabScheduleType.SelectedValue == "1")
        {
            BindDueCustomers();

        }
        else
        {
            GetUnSchedulPaymentTerm();

        }
    }
    private  void CreateTable(DataTable table)
    {
        try
        {
            DataView dv = table.DefaultView;
            DataTable dtSelectionMaster = LoadSelection();
          
            DataRowView dr = null;
            for (int i = 0; i < dv.Count; i++)
            {
                dr = dv[i];


                 decimal dProjectTotal = GetProjectTotal(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                 decimal dChangeOrderTotal = GetChangeOrderTotal(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                 decimal dTotalRecived = GetAmountReceived(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                 decimal dBalanceDue = (dProjectTotal + dChangeOrderTotal) - dTotalRecived;

                if (dBalanceDue > 0)
                {
                  
                    decimal PaymentTermsAmount = 0;
                    

                   

                    string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["section_name"].ToString().Replace("Payment", "").ToLower() + "' and customer_id=" + dr["customer_id"] + " and estimate_id=" + dr["estimate_id"];
                    DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                    if (dt.Rows.Count == 0)
                    {

                        DataRow drNew = dtSelectionMaster.NewRow();

                        if (dr["section_name"].ToString().ToLower().Trim() == "Balance Due at Completion Payment".ToLower().Trim())
                        {
                            PaymentTermsAmount = GetPaymentTermAmount(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]), dr["section_name"].ToString().Replace("Payment", ""));

                            drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                            drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                            drNew["eventDate"] = Convert.ToDateTime(dr["event_start"]);
                            drNew["section_name"] = dr["section_name"].ToString().Replace("Payment","");
                            drNew["estimatename"] = dr["estimate_name"].ToString();
                            drNew["customername"] = dr["customer_name"].ToString();
                           

                            if (dBalanceDue > PaymentTermsAmount)
                                drNew["DueAmount"] = PaymentTermsAmount;
                            else
                                drNew["DueAmount"] = dBalanceDue;

                            drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                            drNew["job_number"] = dr["job_number"].ToString();

                        }
                        else
                        {

                            PaymentTermsAmount = GetPaymentTermAmount(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]), dr["section_name"].ToString().Replace("Payment", ""));
                            drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                            drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                            drNew["eventDate"] = Convert.ToDateTime(dr["event_start"]);
                            drNew["section_name"] = dr["section_name"].ToString().Replace("Payment","");
                            drNew["estimatename"] = dr["estimate_name"].ToString();
                            drNew["customername"] = dr["customer_name"].ToString();
                         
                            if (dBalanceDue > PaymentTermsAmount)
                                drNew["DueAmount"] = PaymentTermsAmount;
                            else
                                drNew["DueAmount"] = dBalanceDue;
                            drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                            drNew["job_number"] = dr["job_number"].ToString();
                        }
                        if (PaymentTermsAmount > 0)
                            dtSelectionMaster.Rows.Add(drNew);



                    }
                }

            }

            Session.Add("sCustomerDue", dtSelectionMaster);
          
            GetDueCustomers(0);


        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    private DataTable LoadSelection()
    {
        DataTable table = new DataTable();
       
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("customername", typeof(string));
        table.Columns.Add("estimatename", typeof(string));
        table.Columns.Add("eventDate", typeof(DateTime));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("payment_terms", typeof(string));
        table.Columns.Add("DueAmount", typeof(decimal));
       
        table.Columns.Add("sale_date", typeof(DateTime));
        table.Columns.Add("job_number", typeof(string));
        return table;
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "Item Per page");
        GetDueCustomers(0);
    }
    protected void GetDueCustomers(int nPageNo)
    {
        try
        {
            if (rabScheduleType.SelectedValue == "1")
            {
                if (Session["sCustomerDue"] != null)
                {
                    DataTable dtCrews = (DataTable)Session["sCustomerDue"];

                    grdDueCustomers.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                    grdDueCustomers.PageIndex = nPageNo;
                    grdDueCustomers.DataSource = dtCrews;
                    grdDueCustomers.DataKeyNames = new string[] { "customer_id", "estimate_id", "customername", "estimatename", "eventDate", "section_name", "DueAmount" };
                    grdDueCustomers.DataBind();

                }
            }
            else
            {
                if (Session["schedulePaymentTerms"] != null)
                {
                    DataTable dt = new DataTable();

                   dt = (DataTable)Session["schedulePaymentTerms"];

                    grdDueCustomers.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                    grdDueCustomers.PageIndex = nPageNo;
                    grdDueCustomers.DataSource = dt;
                    grdDueCustomers.DataKeyNames = new string[] { "customer_id", "estimate_id", "customername", "estimatename", "eventDate", "section_name", "DueAmount" };
                    grdDueCustomers.DataBind();

                }

            }
           

        }

        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
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

        if (grdDueCustomers.PageCount == nPageNo + 1)
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


  
    protected void grdDueCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            try
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                int ncid = Convert.ToInt32(grdDueCustomers.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int neid = Convert.ToInt32(grdDueCustomers.DataKeys[e.Row.RowIndex].Values[1].ToString());
                DateTime eventDate = Convert.ToDateTime(grdDueCustomers.DataKeys[e.Row.RowIndex].Values[4].ToString());
                string section = grdDueCustomers.DataKeys[e.Row.RowIndex].Values[5].ToString();
                DateTime currentDate = DateTime.Now;



                estimate_payment objEstPay = new estimate_payment();
                HyperLink hyp_section = (HyperLink)e.Row.FindControl("hyp_section");
                if (_db.estimate_payments.Where(est_p => est_p.estimate_id == neid && est_p.customer_id == ncid && est_p.client_id == 1).SingleOrDefault() == null)
                {
                    hyp_section.NavigateUrl = "payment_info.aspx?eid=" + neid + "&cid=" + ncid;

                }
                else
                {

                    objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == neid && pay.customer_id == ncid && pay.client_id == 1);
                    hyp_section.NavigateUrl = "payment_recieved.aspx?cid=" + ncid + "&epid=" + objEstPay.est_payment_id + "&eid=" + neid;

                }

                if (rabScheduleType.SelectedValue == "1")
                {
                    grdDueCustomers.Columns[6].Visible = true;
                    grdDueCustomers.Columns[7].Visible = true;
                    grdDueCustomers.Columns[8].Visible = true;
                    grdDueCustomers.Columns[9].Visible = true;
                }
                else
                {

                    grdDueCustomers.Columns[6].Visible = false;
                    grdDueCustomers.Columns[7].Visible = false;
                    grdDueCustomers.Columns[8].Visible = false;
                    grdDueCustomers.Columns[9].Visible = false;



                }


                Label lblPastDue = (Label)e.Row.FindControl("lblPastDueTwo");
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                LinkButton lnkReminderEmail = (LinkButton)e.Row.FindControl("lnkReminderEmail");
                LinkButton lnkSendSMS = (LinkButton)e.Row.FindControl("lnkSendSMS");
                lnkReminderEmail.CommandArgument = (ncid.ToString() + ',' + neid.ToString());
                lnkSendSMS.CommandArgument = (ncid.ToString() + ',' + neid.ToString());

                TimeSpan leadAgeDifference = (currentDate - Convert.ToDateTime(eventDate));
                if (Convert.ToInt32(leadAgeDifference.TotalDays) > 0)
                    lblPastDue.Text =  Convert.ToInt32(leadAgeDifference.TotalDays).ToString() + " days";

              
         
                if (_db.PendingPaymentHistories.Any(p => p.customer_id == ncid && p.estimate_id == neid && p.section.ToLower().Trim() == section.ToLower().Trim()))
                {
                    PendingPaymentHistory objPP = _db.PendingPaymentHistories.FirstOrDefault(p => p.customer_id == ncid && p.estimate_id == neid && p.section.ToLower().Trim() == section.ToLower().Trim());
                    if (objPP != null)
                    {
                        lblStatus.Text ="Sent on "+Convert.ToDateTime(objPP.createddate).ToString("MM/dd/yyyy");
                    }
                }

            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }


        }
    }
    protected void grdDueCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GetDueCustomers(e.NewPageIndex);
    }
   
  

    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetDueCustomers(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetDueCustomers(nCurrentPage - 2);
    }

    protected void lnkSendSMS_Click(object sender, EventArgs e)
    {
        try
        {



            LinkButton btnsubmit = sender as LinkButton;
            int ncid = 0;
            int eid = 0;

            string cideid = btnsubmit.CommandArgument;
            if (cideid.Contains(','))
            {
                var cid = cideid.Split(',');

                ncid = Convert.ToInt32(cid[0]);
                eid = Convert.ToInt32(cid[1]);



            }

            GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
            string customerName = gRow.Cells[0].Text;
            string estimateName = gRow.Cells[1].Text;
            string section = gRow.Cells[2].Text;
            Session.Add("pSection", section);
            string url = "window.open('sendsms.aspx?custId=" + ncid + "&sms=sms&eid=" + eid + "', 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnPendingEmail_Click(object sender, EventArgs e)
    {
        try
        {



            LinkButton btnsubmit = sender as LinkButton;
            int ncid = 0;
            int eid = 0;

            string cideid = btnsubmit.CommandArgument;
            if (cideid.Contains(','))
            {
                var cid = cideid.Split(',');

                ncid = Convert.ToInt32(cid[0]);
                eid = Convert.ToInt32(cid[1]);



            }

            GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
            string customerName = gRow.Cells[2].Text;
            string estimateName = gRow.Cells[3].Text;

            HyperLink hyp_section =(HyperLink)gRow.FindControl("hyp_section");

            string section = hyp_section.Text;




            string CCEmail = string.Empty;
            string strHTML = string.Empty;
            string footer = string.Empty;
            string footer2 = string.Empty;
            footer = string.Empty;
            footer2 = string.Empty;

            string Subject = "You have a pending payment ";
            Subject += "(" + customerName + ", " + estimateName + "-" + section + ")";

            strHTML = "<p>Hi " + customerName + ",</p>" + Environment.NewLine;
            footer = "<p> Thank you,</p>" + Environment.NewLine;
            footer += "<p>Arizona's Interior Innovations</p>" + Environment.NewLine;

            footer2 = "<p>Warmest Regards,</p>" + Environment.NewLine;
            footer2 += "<p>Ann Lyons</p>" + Environment.NewLine;


            if (section.ToLower().Trim() == "Balance Due at Completion Payment".ToLower().Trim())
            {
                strHTML += "<p>Thank you so much for trusting the Interior Innovations Team with your remodeling project. It has been our pleasure to work with you. Please login to your client portal to complete payment at your earliest convenience.<p/> " + Environment.NewLine;
                strHTML += "<p><a target='_blank' href='https://azinteriorinnovations.com/" + "'> www.azinteriorinnovations.com </a> </p>" + Environment.NewLine;
                strHTML += "<p>If you have any questions don't hesitate to reach out.<p/> " + Environment.NewLine;
                strHTML += "<p>We hope you and your family enjoy your new space!<p/> " + Environment.NewLine;
                strHTML += footer2;
            }
            else
            {
                strHTML += "<p>Your <b>" + section + "</b> progress payment is due. Please login to your client portal via <a target='_blank' href='https://ii.faztrack.com/customerlogin.aspx" + "'> www.azinteriorinnovations.com </a> to process your payment.</p>" + Environment.NewLine;
                strHTML += "<p>If you have any questions, please reach out to your sales associate for assistance.<p/> " + Environment.NewLine;
                strHTML += footer;
            }

            Session.Add("PMessBody", strHTML);
            Session.Add("penSubject", Subject);
            Session.Add("pSection", section);
            string url = "window.open('sendemailoutlook.aspx?custId=" + ncid + "&penemail=pemail&eid=" + eid + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }




    private static decimal GetDueAmount(int custId, int estId)
    {
        decimal dueAmount = 0;
     
        string sSql = "SELECT DueAmount from PBI_Estimate " +
            " WHERE customer_id = " + custId + " AND estimate_id = " + estId;

        DataTable table = DataReader.Complex_Read_DataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
        {
            dueAmount = Convert.ToDecimal(table.Rows[0]["DueAmount"]);
           
            return dueAmount;
        }

    }

    private static decimal GetPaymentTermAmount(int custId, int estId, string paymentTerm)
    {
        decimal pamentTermAmount = 0;

        string sSql = "SELECT * FROM estimate_payments " +
            " WHERE customer_id = " + custId + " AND estimate_id = " + estId;

        DataTable table = DataReader.Complex_Read_DataTable(sSql);




        if (table.Rows.Count == 0)
            return 0;
        else
        {
            if (table.Rows[0]["deposit_value"].ToString() != null && table.Rows[0]["deposit_value"].ToString() != "")
            {
                if (table.Rows[0]["deposit_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["deposit_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["deposit_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["countertop_value"].ToString() != null && table.Rows[0]["countertop_value"].ToString() != "")
            {
                if (table.Rows[0]["countertop_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["countertop_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["countertop_amount"]);
                }

            }
            if (table.Rows[0]["start_job_value"].ToString() != null && table.Rows[0]["start_job_value"].ToString() != "")
            {
                if (table.Rows[0]["start_job_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["start_job_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["start_job_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["due_completion_value"].ToString() != null && table.Rows[0]["due_completion_value"].ToString() != "")
            {
                if (table.Rows[0]["due_completion_value"].ToString().ToLower().Trim() == paymentTerm.ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["due_completion_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["due_completion_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["final_measure_value"].ToString() != null && table.Rows[0]["final_measure_value"].ToString() != "")
            {
                if (table.Rows[0]["final_measure_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["final_measure_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["final_measure_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["deliver_caninet_value"].ToString() != null && table.Rows[0]["deliver_caninet_value"].ToString() != "")
            {


                if (table.Rows[0]["deliver_caninet_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["deliver_cabinet_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["deliver_cabinet_amount"]);
                    return pamentTermAmount;
                }

            }

            if (table.Rows[0]["substantial_value"].ToString().Trim() != null && table.Rows[0]["substantial_value"].ToString().Trim() != "")
            {
                if (table.Rows[0]["substantial_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["substantial_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["substantial_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["drywall_value"].ToString() != null && table.Rows[0]["drywall_value"].ToString() != "")
            {
                if (table.Rows[0]["drywall_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["drywall_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["drywall_amount"]);
                    return pamentTermAmount;
                }

            }

            if (table.Rows[0]["flooring_value"].ToString() != null && table.Rows[0]["flooring_value"].ToString() != "")
            {
                if (table.Rows[0]["flooring_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["flooring_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["flooring_amount"]);
                    return pamentTermAmount;
                }

            }
            if (table.Rows[0]["other_value"].ToString() != null && table.Rows[0]["other_value"].ToString() != "")
            {
                if (table.Rows[0]["other_value"].ToString().Replace("  ", " ").ToLower().Trim() == paymentTerm.Replace("  ", " ").ToLower().Trim())
                {
                    if (Convert.ToDecimal(table.Rows[0]["other_amount"]) > 0)
                        pamentTermAmount = Convert.ToDecimal(table.Rows[0]["other_amount"]);
                    return pamentTermAmount;
                }

            }



            return pamentTermAmount;
        }
    }


    private static decimal GetProjectTotal(int custId, int estId)
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string sSql = "SELECT new_total_with_tax,adjusted_price,project_subtotal,adjusted_tax_amount,tax_amount FROM estimate_payments " +
            " WHERE customer_id = " + custId + " AND estimate_id = " + estId;

        DataTable table = DataReader.Complex_Read_DataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
        {
            totalwithtax = Convert.ToDecimal(table.Rows[0]["new_total_with_tax"]);
            if (Convert.ToDecimal(table.Rows[0]["adjusted_price"]) > 0)
                project_subtotal = Convert.ToDecimal(table.Rows[0]["adjusted_price"]);
            else
                project_subtotal = Convert.ToDecimal(table.Rows[0]["project_subtotal"]);
            if (Convert.ToDecimal(table.Rows[0]["adjusted_tax_amount"]) > 0)
                tax_amount = Convert.ToDecimal(table.Rows[0]["adjusted_tax_amount"]);
            else
                tax_amount = Convert.ToDecimal(table.Rows[0]["tax_amount"]);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            return totalwithtax;
        }

    }
    private static decimal GetChangeOrderTotal(int custId, int estId)
    {
        decimal TotalCOAmount = 0;
        string sSql = "SELECT  chage_order_id, tax FROM changeorder_estimate " +
        " WHERE customer_id = " + custId + " AND estimate_id = " + estId + " AND change_order_status_id = 3 ";

        DataTable table = DataReader.Complex_Read_DataTable(sSql);
        if (table.Rows.Count > 0)
        {
            int chage_order_id = 0;
            decimal taxRate = 0;
            foreach (DataRow dr in table.Rows)
            {
                chage_order_id = Convert.ToInt32(dr["chage_order_id"]);
                taxRate = Convert.ToDecimal(dr["tax"]);
                decimal COAmount = GetCOAmount(custId, estId, chage_order_id);
                decimal CoTax = COAmount * (taxRate / 100);
                decimal CoPrice = 0;

                if (CoTax > 0)
                {
                    CoPrice = COAmount + CoTax;

                }
                else
                {
                    CoPrice = COAmount;
                }
                TotalCOAmount += CoPrice;
            }

        }
        return TotalCOAmount;
    }
    private static decimal GetCOAmount(int custId, int estId, int coId)
    {
        string sSql = "SELECT Isnull(sum(EconomicsCost),0) as CoTotal  FROM change_order_pricing_list " +
                      " WHERE customer_id = " + custId + " AND estimate_id = " + estId + " AND chage_order_id = " + coId;

        DataTable table = DataReader.Complex_Read_DataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
            return Convert.ToDecimal(table.Rows[0]["CoTotal"]);
    }
    private static decimal GetAmountReceived(int custId, int estId)
    {
        string sSql = "SELECT Isnull(sum(pay_amount),0) as RecivedAmount  FROM New_partial_payment " +
                       " WHERE customer_id = " + custId + " AND estimate_id = " + estId;
        DataTable table = DataReader.Complex_Read_DataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
            return Convert.ToDecimal(table.Rows[0]["RecivedAmount"]);
    }






    protected void rabScheduleType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rabScheduleType.SelectedValue == "1")
        {
          
            lblPastDue.Text = "Past Due:";
            BindDueCustomers();
           
        }
        else
        {
            GetUnSchedulPaymentTerm();
      
            lblPastDue.Text = "Sold Date Based";
        }
       
    }

    private void GetUnSchedulPaymentTerm()
    {
        try
        {


            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dtSelectionMaster = LoadSelection();
            string strCondition = "";
            DateTime todays = DateTime.Now;
            if (ddlEstimateStatus.SelectedItem.Text != "All")
            {
                strCondition += " AND ce.IsEstimateActive =" + Convert.ToInt32(ddlEstimateStatus.SelectedValue);
            }
            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {

                    strCondition += " AND c.status_id NOT IN(4,5,7)  ";
                }
                else
                {
                    strCondition += " AND c.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                }

            }

            if (txtSearch.Text != "")
            {
                strCondition += " and c.last_name1 LIKE '%" + txtSearch.Text.ToString().Replace("'", "''") + "%'";
            }
            if (Convert.ToInt32(rabDays.SelectedValue) == 4)
            {

                strCondition += "  AND CONVERT(DATETIME,ce.sale_date) < '" + todays.AddDays(-90).ToString("MM/dd/yyyy") + "'";
            }
            else
            {
                int days = Convert.ToInt32(rabDays.SelectedValue);
                strCondition += " and  CONVERT(DATETIME,ce.sale_date) >= '" + todays.AddDays(-days).ToString("MM/dd/yyyy") + "' AND CONVERT(DATETIME,ce.sale_date) < '" + todays.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }


            string sSql = @"SELECT c.first_name1+' '+c.last_name1 as customername,ce.estimate_name, ce.customer_id, ce.estimate_id,  
                          deposit_percent, deposit_amount, countertop_percent, countertop_amount, start_job_percent, start_job_amount, due_completion_percent, 
                         due_completion_amount, final_measure_percent, 
                         final_measure_amount, deliver_caninet_percent, deliver_cabinet_amount, substantial_percent, substantial_amount, 
                          other_value, other_percent, other_amount, based_on_percent, based_on_dollar,
                         incentive_ids, deposit_date, countertop_date, startof_job_date, due_completion_date, measure_date, delivery_date, 
                         substantial_date, other_date, lead_time, contract_date, deposit_value, countertop_value, start_job_value, 
                         due_completion_value, final_measure_value, deliver_caninet_value, substantial_value, special_note, start_date,
                         completion_date, is_KithenSheet, is_BathSheet, is_ShowerSheet, is_TubSheet, drywall_percent, 
                         drywall_amount, drywall_date, drywall_value, flooring_percent, flooring_amount, flooring_date, flooring_value,ce.sale_date,
                         case when ce.alter_job_number!='' then  ce.alter_job_number 
                         else   ce.job_number end as job_number
                       
                        FROM  estimate_payments
                        inner join
                        customer_estimate as ce on ce.customer_id = estimate_payments.customer_id and ce.estimate_id = estimate_payments.estimate_id
                        inner join customers as c on c.customer_id = ce.customer_id 
                        
                         WHERE  ce.status_id=3  " + strCondition  + " order by CONVERT(DATETIME,ce.sale_date) desc";
                     DataTable table = DataReader.Complex_Read_DataTable(sSql);
            DataView dv = table.DefaultView;
            DataRowView dr = null;
            for (int i = 0; i < dv.Count; i++)
            {
                dr = dv[i];

                decimal dProjectTotal = GetProjectTotal(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                decimal dChangeOrderTotal = GetChangeOrderTotal(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                decimal dTotalRecived = GetAmountReceived(Convert.ToInt32(dr["customer_id"]), Convert.ToInt32(dr["estimate_id"]));
                decimal dBalanceDue = (dProjectTotal + dChangeOrderTotal) - dTotalRecived;

                if (dBalanceDue > 0)
                {
                   

                    if (dr["deposit_value"].ToString() != null && dr["deposit_value"].ToString() != "")
                    {




                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["deposit_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["deposit_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["deposit_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["deposit_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["deposit_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                  
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }



                    }
                   if (dr["countertop_value"].ToString() != null && dr["countertop_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["countertop_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["countertop_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["countertop_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["countertop_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["countertop_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                  
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);

                                }
                            }
                        }
                    }


                  if (dr["start_job_value"].ToString() != null && dr["start_job_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["start_job_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["start_job_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["start_job_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["start_job_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["start_job_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                  
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);

                                }
                            }
                        }





                    }
                    if (dr["due_completion_value"].ToString() != null && dr["due_completion_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["due_completion_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["due_completion_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["due_completion_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["due_completion_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["due_completion_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                    
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);

                                }
                            }
                        }





                    }
                    if (dr["final_measure_value"].ToString() != null && dr["final_measure_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["final_measure_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["final_measure_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["final_measure_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["final_measure_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["final_measure_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                   
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }

                            }
                        }




                    }
                    if (dr["deliver_caninet_value"].ToString() != null && dr["deliver_caninet_value"].ToString() != "")
                    {

                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["deliver_caninet_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["deliver_caninet_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["deliver_cabinet_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["deliver_caninet_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["deliver_cabinet_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                   
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }





                    }

                    if (dr["substantial_value"].ToString().Trim() != null && dr["substantial_value"].ToString().Trim() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["substantial_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["substantial_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["substantial_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["substantial_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["substantial_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                    
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }





                    }
                    if (dr["drywall_value"].ToString() != null && dr["drywall_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id ==Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id==Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() ==dr["drywall_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["drywall_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["drywall_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["drywall_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["drywall_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                   
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }





                    }

                    if (dr["flooring_value"].ToString() != null && dr["flooring_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id==Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["flooring_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["flooring_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {
                                if (Convert.ToDecimal(dr["flooring_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["flooring_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["flooring_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                    
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }





                    }
                    if (dr["other_value"].ToString() != null && dr["other_value"].ToString() != "")
                    {
                        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(dr["customer_id"].ToString()) && sc.estimate_id == Convert.ToInt32(dr["estimate_id"].ToString()) && sc.section_name.Replace("Payment", "").ToLower().Trim() == dr["other_value"].ToString().ToLower().Trim()))
                        {
                            continue;
                        }
                        else
                        {
                            string Sqls = " SELECT * from New_partial_payment where LOWER(pay_term_desc)='" + dr["other_value"].ToString().ToLower() + "' and customer_id=" + Convert.ToInt32(dr["customer_id"].ToString()) + " and estimate_id=" + Convert.ToInt32(dr["estimate_id"].ToString());
                            DataTable dt = DataReader.Complex_Read_DataTable(Sqls);
                            if (dt.Rows.Count == 0)
                            {

                                if (Convert.ToDecimal(dr["other_amount"]) > 0)
                                {
                                    DataRow drNew = dtSelectionMaster.NewRow();
                                    drNew["customer_id"] = Convert.ToInt32(dr["customer_id"]);
                                    drNew["estimate_id"] = Convert.ToInt32(dr["estimate_id"]);
                                    drNew["section_name"] = dr["other_value"].ToString();

                                    drNew["DueAmount"] = Convert.ToDecimal(dr["other_amount"]).ToString();
                                    drNew["eventDate"] = Convert.ToDateTime("01-01-2020");
                                    drNew["estimatename"] = dr["estimate_name"].ToString();
                                    drNew["customername"] = dr["customername"].ToString();
                                    
                                    drNew["sale_date"] = Convert.ToDateTime(dr["sale_date"]);
                                    drNew["job_number"] = dr["job_number"].ToString();
                                    dtSelectionMaster.Rows.Add(drNew);
                                }
                            }
                        }
                    }
                    }

               



                
            }
                

              


                Session.Add("schedulePaymentTerms", dtSelectionMaster);
               GetDueCustomers(0);
            
           

        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }



    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {

            if (rabScheduleType.SelectedValue == "1")
            {
                BindDueCustomers();

            }
            else
            {
                GetUnSchedulPaymentTerm();

            }



        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {


        try
        {
            lblResult.Text = "";
            txtSearch.Text = "";
            ddlEstimateStatus.SelectedValue = "1";
            ddlStatus.SelectedValue = "2";
        


            if (rabScheduleType.SelectedValue == "1")
            {
                BindDueCustomers();

            }
            else
            {
                GetUnSchedulPaymentTerm();

            }



        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {

            if (rabScheduleType.SelectedValue == "1")
            {
                BindDueCustomers();

            }
            else
            {
                GetUnSchedulPaymentTerm();

            }



        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void ddlEstimateStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {

            if (rabScheduleType.SelectedValue == "1")
            {
                BindDueCustomers();

            }
            else
            {
                GetUnSchedulPaymentTerm();

            }



        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
}