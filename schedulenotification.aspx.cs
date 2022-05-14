using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class schedulenotification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("calllog004") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            DateTime date = DateTime.Now;

            string start = "";
            string end = "";

            if (Session["ssScheduleSMS"] != null)
            {
                List<csScheduleSMS> list = (List<csScheduleSMS>)(Session["ssScheduleSMS"]);
                if (list.Count > 0)
                {
                    date = list.FirstOrDefault().schedule_date;

                    start = date.ToShortDateString();
                    end = date.AddDays(1).ToShortDateString();
                }
                else
                {
                    start = date.AddDays(1).ToShortDateString();
                    end = date.AddDays(2).ToShortDateString();
                }
            }
            else
            {
                start = date.AddDays(1).ToShortDateString();
                end = date.AddDays(2).ToShortDateString();
            }

            //  lblToday.Text = "Today: " + DateTime.Now.ToShortDateString();
            // lblTomorrow.Text = "Tomorrow: " + DateTime.Now.AddDays(1).ToShortDateString();

            txtScheduleDate.Text = start;

            GetSchedule(start, end);

            Session["ssScheduleSMS"] = null;

        }
    }

    protected void GetSchedule(string start, string end)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();


            string sql = "select sc.event_id, c.customer_id, c.first_name1, c.last_name1, c.first_name1 +' '+ c.last_name1 AS customer_name, '('+c.last_name1+') '+sc.title as title, sc.employee_name, sc.estimate_id, " +
                        " CASE WHEN c.Phone = '' THEN c.mobile ELSE c.Phone END AS mobile, sc.event_start, sc.event_end, sc.section_name, sc.location_name, sc.IsScheduleDayException," +
                        " ISNULL(s.first_name,'') as superfirstname, ISNULL(s.last_name,'') as superlastname, ISNULL(s.phone,'') as supermobile, " +
                        " c.address + ', ' + c.city + ', ' + c.state + ', ' + c.zip_code as CustAddress " +
                        " from [ScheduleCalendar] AS sc " +
                        " Inner join customers AS c on sc.customer_id = c.customer_id " +
                        " LEFT OUTER JOIN user_info as s on c.SuperintendentId = s.user_id " +
                        " where   (event_start>= '" + start + "'  and event_start<'" + end + "') or ( event_start< '" + start + "'  and event_end>='" + start + "') " +
                        " AND type_id in (1,11) AND sc.IsEstimateActive = 1 AND c.is_active = 1" +
                        " order by  c.first_name1";

            List<csScheduleNotification> item = _db.ExecuteQuery<csScheduleNotification>(sql, string.Empty).ToList();


            grdScheduleNotification.DataSource = item.ToList();
            grdScheduleNotification.DataKeyNames = new string[] { "event_id", "employee_name", "customer_name", "mobile", "customer_id", "estimate_id", "section_name", "location_name", "superfirstname", "superlastname", "supermobile", "CustAddress" };
            grdScheduleNotification.DataBind();

            //lblCount.Text = "Total: " + item.Count().ToString();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }


    protected void grdScheduleNotification_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nEvent_id = Convert.ToInt32(grdScheduleNotification.DataKeys[e.Row.RowIndex].Values[0]);
            string strEmployee_name = grdScheduleNotification.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string strCustomer_name = grdScheduleNotification.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strMobile = grdScheduleNotification.DataKeys[e.Row.RowIndex].Values[3].ToString();
            int nCustomer_id = Convert.ToInt32(grdScheduleNotification.DataKeys[e.Row.RowIndex].Values[4]);

            Label lblCustomerName = e.Row.FindControl("lblCustomerName") as Label;
            //  CheckBox chkSendAll = e.Row.FindControl("chkSendAll") as CheckBox;
            // CheckBox chkSendToCustomer = e.Row.FindControl("chkSendToCustomer") as CheckBox;
            LinkButton lnkCustomer = e.Row.FindControl("lnkCustomer") as LinkButton;

            //  CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");

            Label lblSubmittedBy = e.Row.FindControl("lblSubmittedBy") as Label;
            Label lblMessageStatus = e.Row.FindControl("lblMessageStatus") as Label;

            //// ----Check Crew Phone Number
            if (strMobile.Length == 0)
            {
                lblCustomerName.Visible = false;
                lnkCustomer.Visible = true;
                lnkCustomer.ForeColor = Color.Red;
                lnkCustomer.CommandArgument = nCustomer_id.ToString();
                //  chkSendToCustomer.Visible = false;
            }
            else
            {
                lblCustomerName.Text = strCustomer_name + " <br/>" + strMobile;
            }
            /////---------------------


            ////------- 

            DateTime dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);

            var objSMS = _db.ScheduleSMs.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCustomer_id && s.reference_type == 1 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date);

            if (objSMS != null)
            {
                lblSubmittedBy.Text = "Submitted By: " + objSMS.created_by + "<br/>" + Convert.ToDateTime(objSMS.create_date).ToString("MM/dd/yyyy hh:mm tt");
                // chkSendToCustomer.Checked = true;
                if ((bool)objSMS.is_success)
                {
                    lblMessageStatus.Text = "Message Sent: " + Convert.ToDateTime(objSMS.send_date).ToString("MM/dd/yyyy hh:mm tt");
                    //  chkSendToCustomer.Enabled = false;
                    //  chkSendAll.Enabled = false;
                }
            }



            ///----------
            if (Session["ssScheduleSMS"] != null)
            {
                List<csScheduleSMS> list = (List<csScheduleSMS>)(Session["ssScheduleSMS"]);

                if (list.Count > 0)
                {
                    txtScheduleDate.Text = list.FirstOrDefault().schedule_date.ToShortDateString();

                    var ssObjSMS = list.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCustomer_id && s.reference_type == 1);

                    if (ssObjSMS != null)
                    {
                        //  chkSendToCustomer.Checked = true;

                    }
                }
            }


            //chkSendAll.Checked = chkSendToCustomer.Checked;
            //---- chkboxSelectAll.Checked = chkSendToCustomer.Checked;



            GridView gv = e.Row.FindControl("grdCrewList") as GridView;
            GetCrewInfo(gv, strEmployee_name);

            GridView gvd = e.Row.FindControl("grdDeliveryStatus") as GridView;
            GetDeliveryStatus(gvd, nEvent_id);
        }
    }

    private void GetCrewInfo(GridView grd, string strEmployee_name)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string[] ary = strEmployee_name.Split(',').Select(p => p.Trim()).ToArray();

        var item = _db.Crew_Details.Where(c => ary.Contains(c.full_name)).ToList();

        grd.DataSource = item;
        grd.DataKeyNames = new string[] { "crew_id", "full_name", "phone" };
        grd.DataBind();
    }

    private void GetDeliveryStatus(GridView grd, int eventid)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DateTime dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);

        var item = (from s in _db.ScheduleSMs.AsEnumerable()
                    where s.event_id == eventid && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date
                    select new
                    {
                        ScheduleSMSId = s.ScheduleSMSId,
                        mobile = (s.Status ?? "") == "" ? "" : s.mobile.Replace("1(", "("),
                        Status = (s.Status ?? "") == "" ? ""
                        : (s.Status ?? "") == "delivered" ? "Delivered on " + ((DateTime)s.delivered_date).ToString("MM/dd/yyyy hh:mm tt")
                        : (s.Status ?? "") == "message(s) queued" ? "Message(s) queued"
                        : (s.Status ?? "") == "sent" ? "Sent on " + ((DateTime)s.delivered_date).ToString("MM/dd/yyyy hh:mm tt")
                        : "Undelivered: " + (s.error ?? ""),
                        cssStatus = s.Status == "delivered" ? "cssStatusGreen" : s.Status == "sent" ? "cssStatusGreen" : s.Status == "message(s) queued" ? "cssStatusGreen" : "cssStatusRed"
                    }
                    ).ToList();

        DataTable testdt = csCommonUtility.LINQToDataTable(item);

        grd.DataSource = item;
        grd.DataKeyNames = new string[] { "ScheduleSMSId" };
        grd.DataBind();
    }

    string strEventId = "";
    int nUncheckedCrewCount = 0;
    int nUncheckedAll = 0;
    protected void grdCrewList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            GridView grdCrewList = (GridView)sender;
            int nCrew_id = Convert.ToInt32(grdCrewList.DataKeys[e.Row.RowIndex].Values[0]);
            string strCrew_name = grdCrewList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string strPhone = grdCrewList.DataKeys[e.Row.RowIndex].Values[2].ToString();

            Label lblCrewName = e.Row.FindControl("lblCrewName") as Label;
            //  CheckBox chkSendToCrew = e.Row.FindControl("chkSendToCrew") as CheckBox;
            LinkButton lnkCrew = e.Row.FindControl("lnkCrew") as LinkButton;

            ////------get Parent grid items
            int i = 0;
            GridViewRow gvMasterRow = (GridViewRow)e.Row.Parent.Parent.Parent.Parent;
            i = gvMasterRow.RowIndex;
            Label lblSubmittedBy = (Label)e.Row.Parent.Parent.Parent.FindControl("lblSubmittedBy");
            Label lblMessageStatus = (Label)e.Row.Parent.Parent.Parent.FindControl("lblMessageStatus");
            int nEvent_id = Convert.ToInt32(grdScheduleNotification.DataKeys[i].Values[0].ToString());

            //// -----Code for Set Checkbox All Check Status
            if (!strEventId.Contains(nEvent_id.ToString()))
            {
                nUncheckedCrewCount = 0;
            }
            strEventId += ", " + nEvent_id.ToString();
            //////-------------

            //   CheckBox chkSendAll = (CheckBox)e.Row.Parent.Parent.Parent.FindControl("chkSendAll");
            //  CheckBox chkSendToCustomer = (CheckBox)e.Row.Parent.Parent.Parent.FindControl("chkSendToCustomer");
            //   CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");
            ///------///


            //// Check Crew Phone Number
            if (strPhone.Length == 0)
            {
                lblCrewName.Visible = false;
                lnkCrew.Visible = true;
                lnkCrew.ForeColor = Color.Red;
                lnkCrew.CommandArgument = nCrew_id.ToString();
                //  chkSendToCrew.Enabled = false;
            }
            else
            {
                lblCrewName.Text = strCrew_name + "<br/>" + strPhone;
            }
            ////------------


            ///-----------

            DateTime dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);


            var objSMS = _db.ScheduleSMs.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCrew_id && s.reference_type == 2 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date);

            if (objSMS != null)
            {
                lblSubmittedBy.Text = "Submitted By: " + objSMS.created_by + "<br/>" + Convert.ToDateTime(objSMS.create_date).ToString("MM/dd/yyyy hh:mm tt");
                //   chkSendToCrew.Checked = true;
                if ((bool)objSMS.is_success)
                {
                    lblMessageStatus.Text = "Message Sent: " + Convert.ToDateTime(objSMS.send_date).ToString("MM/dd/yyyy hh:mm tt");
                    //    chkSendToCrew.Enabled = false;
                    //    chkSendAll.Enabled = false;
                }

            }
            else
            {
                nUncheckedCrewCount++;
                nUncheckedAll++;
            }

            if (Session["ssScheduleSMS"] != null)
            {
                List<csScheduleSMS> list = (List<csScheduleSMS>)(Session["ssScheduleSMS"]);
                if (list.Count > 0)
                {
                    txtScheduleDate.Text = list.FirstOrDefault().schedule_date.ToShortDateString();

                    var ssObjSMS = list.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCrew_id && s.reference_type == 2);

                    if (ssObjSMS != null)
                    {
                        //   chkSendToCrew.Checked = true;

                    }
                }
            }

            //if (nUncheckedCrewCount == 0 && chkSendToCustomer.Checked)
            //{
            //    chkSendAll.Checked = true;

            //}
            //else
            //{
            //    chkSendAll.Checked = false;
            //}

            //if (nUncheckedAll == 0 && chkSendToCustomer.Checked)
            //{
            //    chkboxSelectAll.Checked = true;
            //}
            //else
            //{
            //    chkboxSelectAll.Checked = false;
            //}

        }
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "View Click");
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        string start = dtScheduleDate.ToShortDateString();
        string end = dtScheduleDate.AddDays(1).ToShortDateString();

        GetSchedule(start, end);

    }

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        dtScheduleDate = dtScheduleDate.AddDays(-1);

        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void btnToday_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnToday.ID, btnToday.GetType().Name, "Today Click");
        DateTime dtScheduleDate = DateTime.Now;


        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        dtScheduleDate = dtScheduleDate.AddDays(1);

        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Submit Click");
        try
        {
            InsertScheduleSMS();
            DateTime date = DateTime.Now;

            string start = date.AddDays(1).ToShortDateString();
            string end = date.AddDays(2).ToShortDateString();

            DateTime dtScheduleDate = Convert.ToDateTime(start);




            GetSchedule(start, end);
            lblResult.Text = csCommonUtility.GetSystemMessage("Schedule Submited Successfully");
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    //protected void btnSubmit_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        DateTime dtScheduleDate = DateTime.Now;
    //        if (txtScheduleDate.Text != "")
    //        {
    //            try
    //            {
    //                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
    //            }
    //            catch
    //            {
    //                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
    //                return;
    //            }
    //        }

    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        company_profile objComp = _db.company_profiles.FirstOrDefault();

    //        string strCompanyPhone = objComp.phone ?? "";

    //        List<ScheduleSMS> list = new List<ScheduleSMS>();

    //        foreach (GridViewRow mRow in grdScheduleNotification.Rows)
    //        {
    //            ScheduleSMS obj = new ScheduleSMS();
    //            int index = Convert.ToInt32(mRow.RowIndex);
    //            int nEvent_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[0]);
    //            string strEmployee_name = grdScheduleNotification.DataKeys[index].Values[1].ToString();
    //            string strCustomer_name = grdScheduleNotification.DataKeys[index].Values[2].ToString();
    //            string strMobile = grdScheduleNotification.DataKeys[index].Values[3].ToString();
    //            int nCustomer_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[4]);
    //            int nEstimate_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[5]);
    //            string strSection_name = grdScheduleNotification.DataKeys[index].Values[6].ToString();
    //            string strLocation_name = grdScheduleNotification.DataKeys[index].Values[7].ToString();
    //            string strSuperFirstName = grdScheduleNotification.DataKeys[index].Values[8].ToString();
    //            string strSuperLastName = grdScheduleNotification.DataKeys[index].Values[9].ToString();
    //            string strSuperMobile = grdScheduleNotification.DataKeys[index].Values[10].ToString();
    //            string strCustAddress = grdScheduleNotification.DataKeys[index].Values[11].ToString();


    //            Label lblCustomerName = mRow.FindControl("lblCustomerName") as Label;
    //            // CheckBox chkSendAll = mRow.FindControl("chkSendAll") as CheckBox;
    //            //  CheckBox chkSendToCustomer = mRow.FindControl("chkSendToCustomer") as CheckBox;
    //            LinkButton lnkCustomer = mRow.FindControl("lnkCustomer") as LinkButton;

    //            GridView grdCrewList = mRow.FindControl("grdCrewList") as GridView;

    //            string CrewName = "";
    //            string[] ary = strEmployee_name.Split(',').Select(p => p.Trim()).ToArray();

    //            foreach (var ar in ary)
    //            {
    //                if (_db.Crew_Details.Any(c => c.full_name.Trim() == ar.Trim()))
    //                {
    //                    var objCrew = _db.Crew_Details.FirstOrDefault(c => c.full_name.Trim() == ar.Trim());
    //                    CrewName += objCrew.first_name.Trim() + ", ";
    //                }
    //            }

    //            // Customer Msg:
    //            var objSMSCust = _db.ScheduleSMS.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCustomer_id && s.reference_type == 1 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date);

    //            if (objSMSCust == null)
    //            {
    //                //if (chkSendToCustomer.Checked && strMobile.Trim() != "")
    //                {
    //                    string NoFormat = csCommonUtility.ExtractNumber(strMobile.Trim());

    //                    if (NoFormat.Length == 10)
    //                    {
    //                        strMobile = "1" + strMobile;
    //                    }

    //                    obj.title = "HHI Schedule: " + strSection_name + " (" + strLocation_name + ") is scheduled for " + dtScheduleDate.ToShortDateString() + ". Crew Names: " + CrewName.Trim().TrimEnd(',') + ". Check your calendar to see any changes. Call " + strSuperFirstName + " at " + strSuperMobile + " for more info.";
    //                    obj.reference_id = nCustomer_id;
    //                    obj.reference_type = 1; // Customer
    //                    obj.estimate_id = nEstimate_id;
    //                    obj.mobile = strMobile;
    //                    //obj.reponse = "";
    //                    obj.event_id = nEvent_id;
    //                    obj.is_success = false;
    //                    // obj.send_date = DateTime.Now;
    //                    obj.schedule_date = dtScheduleDate;
    //                    obj.create_date = DateTime.Now;
    //                    obj.created_by = User.Identity.Name;
    //                    list.Add(obj);
    //                }
    //            }
    //            else
    //            {
    //                // if (!chkSendToCustomer.Checked)
    //                {
    //                    if (!(bool)objSMSCust.is_success)
    //                        _db.ScheduleSMS.DeleteOnSubmit(objSMSCust);
    //                }
    //            }

    //            // Crew Msg:
    //            foreach (GridViewRow cRow in grdCrewList.Rows)
    //            {
    //                obj = new ScheduleSMS();
    //                int nCrew_id = Convert.ToInt32(grdCrewList.DataKeys[cRow.RowIndex].Values[0]);
    //                string strCrew_name = grdCrewList.DataKeys[cRow.RowIndex].Values[1].ToString();
    //                string strPhone = grdCrewList.DataKeys[cRow.RowIndex].Values[2].ToString();

    //                Label lblCrewName = cRow.FindControl("lblCrewName") as Label;
    //                CheckBox chkSendToCrew = cRow.FindControl("chkSendToCrew") as CheckBox;
    //                LinkButton lnkCrew = cRow.FindControl("lnkCrew") as LinkButton;

    //                var objSMSCrew = _db.ScheduleSMS.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCrew_id && s.reference_type == 2 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate);

    //                if (objSMSCrew == null)
    //                {
    //                    if (chkSendToCrew.Checked && strPhone.Trim() != "")
    //                    {
    //                        string NoFormat = csCommonUtility.ExtractNumber(strPhone.Trim());

    //                        if (NoFormat.Length == 10)
    //                        {
    //                            strPhone = "1" + strPhone;
    //                        }

    //                        obj.title = "HHI Schedule: " + strCustomer_name + " - " + strCustAddress + " is scheduled for " + dtScheduleDate.ToShortDateString() + ". Crew Names: " + CrewName.Trim().TrimEnd(',') + ". Call " + strSuperFirstName + " at " + strSuperMobile + " for more info.";
    //                        obj.reference_id = nCrew_id;
    //                        obj.reference_type = 2; // Crew
    //                        obj.estimate_id = nEstimate_id;
    //                        obj.mobile = strPhone;
    //                        //obj.reponse = "";
    //                        obj.event_id = nEvent_id;
    //                        obj.is_success = false;
    //                        // obj.send_date = DateTime.Now;
    //                        obj.schedule_date = dtScheduleDate;
    //                        obj.create_date = DateTime.Now;
    //                        obj.created_by = User.Identity.Name;
    //                        list.Add(obj);
    //                    }
    //                }
    //                else
    //                {
    //                    if (!chkSendToCrew.Checked)
    //                    {
    //                        if (!(bool)objSMSCrew.is_success)
    //                            _db.ScheduleSMS.DeleteOnSubmit(objSMSCrew);
    //                    }
    //                }
    //            }
    //        }

    //        // DataTable dtTest = csCommonUtility.LINQToDataTable(list);

    //        if (list.Count() > 0)
    //        {
    //            _db.ScheduleSMS.InsertAllOnSubmit(list);
    //        }
    //        _db.SubmitChanges();


    //        string start = dtScheduleDate.ToShortDateString();
    //        string end = dtScheduleDate.AddDays(1).ToShortDateString();

    //        GetSchedule(start, end);
    //    }
    //    catch (Exception ex)
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    protected void InsertScheduleSMS()
    {
        try
        {



            DateTime date = DateTime.Now;


            try
            {
                date = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }


            string start = date.AddDays(1).ToShortDateString();
            string end = date.AddDays(2).ToShortDateString();

            DateTime dtScheduleDate = Convert.ToDateTime(start);

            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile objComp = _db.company_profiles.FirstOrDefault();

            string strCompanyPhone = objComp.phone ?? "";

            string sql = "select sc.event_id, c.customer_id, c.first_name1, c.last_name1, c.first_name1 +' '+ c.last_name1 AS customer_name, '('+c.last_name1+') '+sc.title as title, sc.employee_name, sc.estimate_id, " +
                        " CASE WHEN c.Phone = '' THEN c.mobile ELSE c.Phone END AS mobile, sc.event_start, sc.event_end, sc.section_name, sc.location_name, sc.IsScheduleDayException," +
                        " ISNULL(s.first_name,'') as superfirstname, ISNULL(s.last_name,'') as superlastname, ISNULL(s.phone,'') as supermobile, " +
                        " c.address + ', ' + c.city + ', ' + c.state + ', ' + c.zip_code as CustAddress " +
                        " from [ScheduleCalendar] AS sc " +
                        " Inner join customers AS c on sc.customer_id = c.customer_id " +
                        " LEFT OUTER JOIN user_info as s on c.SuperintendentId = s.user_id " +
                        " where (event_start>= '" + start + "'  and event_start<'" + end + "') or ( event_start< '" + start + "'  and event_end>='" + start + "') " +
                        " AND type_id in (1,11) AND sc.IsEstimateActive = 1 AND c.is_active = 1 " +
                        " order by  c.first_name1";
            List<csScheduleNotification> sclist = _db.ExecuteQuery<csScheduleNotification>(sql, string.Empty).ToList();

            List<ScheduleSM> list = new List<ScheduleSM>();

            foreach (var l in sclist)
            {
                ScheduleSM obj = new ScheduleSM();

                int nEvent_id = l.event_id;
                string strEmployee_name = l.employee_name;
                string strCustomer_name = l.customer_name;
                string strMobile = l.mobile;
                int nCustomer_id = l.customer_id;
                int nEstimate_id = l.estimate_id;
                string strSection_name = l.section_name;
                string strLocation_name = l.location_name;
                string strSuperFirstName = l.superfirstname;
                string strSuperLastName = l.superlastname;
                string strSuperMobile = l.supermobile;
                string strCustAddress = l.CustAddress;
                bool IsScheduleDayException = l.IsScheduleDayException;

                string CrewName = "";
                string[] ary = strEmployee_name.Split(',').Select(p => p.Trim()).ToArray();

                var crewList = _db.Crew_Details.Where(c => ary.Contains(c.full_name)).ToList();

                foreach (var ar in ary)
                {
                    if (_db.Crew_Details.Any(c => c.full_name.Trim() == ar.Trim()))
                    {
                        var objCrew = _db.Crew_Details.FirstOrDefault(c => c.full_name.Trim() == ar.Trim());
                        CrewName += objCrew.first_name.Trim() + ", ";
                    }
                }

                // Customer Msg:
                var objSMSCust = _db.ScheduleSMs.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCustomer_id && s.reference_type == 1 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date);

                if (objSMSCust == null)
                {
                    if (strMobile.Trim() != "" && CrewName.Length > 0 && !CheckWeekDay(dtScheduleDate, IsScheduleDayException))
                    {
                        string NoFormat = csCommonUtility.ExtractNumber(strMobile.Trim());

                        if (NoFormat.Length == 10)
                        {
                            strMobile = "1" + strMobile;
                        }

                        obj.title = "HHI Schedule: " + strSection_name + " (" + strLocation_name + ") is scheduled for " + dtScheduleDate.ToShortDateString() + ". Crew Names: " + CrewName.Trim().TrimEnd(',') + ". Check your calendar to see any changes. Call " + strSuperFirstName + " at " + strSuperMobile + " for more info.";
                        obj.reference_id = nCustomer_id;
                        obj.reference_type = 1; // Customer
                        obj.estimate_id = nEstimate_id;
                        obj.mobile = strMobile;
                        //obj.reponse = "";
                        obj.event_id = nEvent_id;
                        obj.is_success = false;
                        // obj.send_date = DateTime.Now;
                        obj.schedule_date = dtScheduleDate;
                        obj.create_date = DateTime.Now;
                        obj.created_by = User.Identity.Name;
                        list.Add(obj);
                    }
                }

                // Crew Msg:
                foreach (var cl in crewList)
                {
                    obj = new ScheduleSM();
                    int nCrew_id = cl.crew_id;
                    string strCrew_name = cl.full_name;
                    string strPhone = cl.phone;


                    var objSMSCrew = _db.ScheduleSMs.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCrew_id && s.reference_type == 2 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate);

                    if (objSMSCrew == null)
                    {
                        if (strPhone.Trim() != "" && !CheckWeekDay(dtScheduleDate, IsScheduleDayException))
                        {
                            string NoFormat = csCommonUtility.ExtractNumber(strPhone.Trim());

                            if (NoFormat.Length == 10)
                            {
                                strPhone = "1" + strPhone;
                            }

                            obj.title = "HHI Schedule: " + strCustomer_name + " - " + strCustAddress + " is scheduled for " + dtScheduleDate.ToShortDateString() + ". Crew Names: " + CrewName.Trim().TrimEnd(',') + ". Call " + strSuperFirstName + " at " + strSuperMobile + " for more info.";
                            obj.reference_id = nCrew_id;
                            obj.reference_type = 2; // Crew
                            obj.estimate_id = nEstimate_id;
                            obj.mobile = strPhone;
                            //obj.reponse = "";
                            obj.event_id = nEvent_id;
                            obj.is_success = false;
                            // obj.send_date = DateTime.Now;
                            obj.schedule_date = dtScheduleDate;
                            obj.create_date = DateTime.Now;
                            obj.created_by = User.Identity.Name;
                            list.Add(obj);
                        }
                    }
                }
            }

            DataTable dtTest = csCommonUtility.LINQToDataTable(list);

            if (list.Count() > 0)
            {
                _db.ScheduleSMs.InsertAllOnSubmit(list);
            }
            _db.SubmitChanges();

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected bool CheckWeekDay(DateTime Date, bool IsException)
    {
        bool isWeekday = false;
        if (!IsException)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    isWeekday = true;
                    break;
                case DayOfWeek.Sunday:
                    isWeekday = true;
                    break;
            }
        }
        else
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    isWeekday = true;
                    break;
            }
        }

        if (IsHoliday(Date))
            isWeekday = true;

        return isWeekday;
    }

    protected void lnkCustomer_Click(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {

            SetScheduleSMSSession();

            LinkButton lnkCustomer = (LinkButton)sender;
            int nCustomerId = Convert.ToInt32(lnkCustomer.CommandArgument);

            Response.Redirect("customer_details.aspx?cid=" + nCustomerId, false);

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkCrew_Click(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            SetScheduleSMSSession();

            LinkButton lnkCrew = (LinkButton)sender;
            int nCrewId = Convert.ToInt32(lnkCrew.CommandArgument);

            Response.Redirect("crewdetails.aspx?crid=" + nCrewId, false);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void SetScheduleSMSSession()
    {
        List<csScheduleSMS> list = new List<csScheduleSMS>();

        foreach (GridViewRow mRow in grdScheduleNotification.Rows)
        {
            csScheduleSMS obj = new csScheduleSMS();
            int index = Convert.ToInt32(mRow.RowIndex);
            int nEvent_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[0]);
            string strEmployee_name = grdScheduleNotification.DataKeys[index].Values[1].ToString();
            string strCustomer_name = grdScheduleNotification.DataKeys[index].Values[2].ToString();
            string strMobile = grdScheduleNotification.DataKeys[index].Values[3].ToString();
            int nCustomer_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[4]);
            int nEstimate_id = Convert.ToInt32(grdScheduleNotification.DataKeys[index].Values[5]);

            Label lblCustomerName = mRow.FindControl("lblCustomerName") as Label;
            //  CheckBox chkSendAll = mRow.FindControl("chkSendAll") as CheckBox;
            //  CheckBox chkSendToCustomer = mRow.FindControl("chkSendToCustomer") as CheckBox;
            LinkButton lnkCustomer = mRow.FindControl("lnkCustomer") as LinkButton;

            GridView grdCrewList = mRow.FindControl("grdCrewList") as GridView;

            DateTime dtScheduleDate = DateTime.Now;
            if (txtScheduleDate.Text != "")
            {
                try
                {
                    dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
                }
                catch
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                    return;
                }
            }

            //  if (chkSendToCustomer.Checked)
            {
                obj.title = mRow.Cells[1].Text.ToString();
                obj.reference_id = nCustomer_id;
                obj.reference_type = 1; // Customer
                obj.estimate_id = nEstimate_id;
                obj.mobile = strMobile;
                //obj.reponse = "";
                obj.event_id = nEvent_id;
                obj.is_success = false;
                // obj.send_date = DateTime.Now;
                obj.schedule_date = dtScheduleDate;
                obj.create_date = DateTime.Now;
                obj.created_by = User.Identity.Name;
                list.Add(obj);
            }



            foreach (GridViewRow cRow in grdCrewList.Rows)
            {
                obj = new csScheduleSMS();
                int nCrew_id = Convert.ToInt32(grdCrewList.DataKeys[cRow.RowIndex].Values[0]);
                string strCrew_name = grdCrewList.DataKeys[cRow.RowIndex].Values[1].ToString();
                string strPhone = grdCrewList.DataKeys[cRow.RowIndex].Values[2].ToString();

                Label lblCrewName = cRow.FindControl("lblCrewName") as Label;
                //   CheckBox chkSendToCrew = cRow.FindControl("chkSendToCrew") as CheckBox;
                LinkButton lnkCrew = cRow.FindControl("lnkCrew") as LinkButton;

                // if (chkSendToCrew.Checked)
                {
                    obj.title = mRow.Cells[1].Text.ToString();
                    obj.reference_id = nCrew_id;
                    obj.reference_type = 2; // Crew
                    obj.estimate_id = nEstimate_id;
                    obj.mobile = strPhone;
                    //obj.reponse = "";
                    obj.event_id = nEvent_id;
                    obj.is_success = false;
                    // obj.send_date = DateTime.Now;
                    obj.schedule_date = dtScheduleDate;
                    obj.create_date = DateTime.Now;
                    obj.created_by = User.Identity.Name;
                    list.Add(obj);
                }
            }
        }

        Session.Add("ssScheduleSMS", list);
    }

    //protected void chkboxSelectAll_CheckedChanged(object sender, EventArgs e)
    //{
    //    DateTime dtScheduleDate = DateTime.Now;
    //    if (txtScheduleDate.Text != "")
    //    {
    //        try
    //        {
    //            dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
    //        }
    //        catch
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
    //            return;
    //        }
    //    }
    //    DataClassesDataContext _db = new DataClassesDataContext();


    //    foreach (GridViewRow item in grdScheduleNotification.Rows)
    //    {
    //        GridView grdCrewList = grdScheduleNotification.Rows[item.RowIndex].FindControl("grdCrewList") as GridView;

    //        CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");
    //        CheckBox chkSendAll = (CheckBox)item.FindControl("chkSendAll");
    //        CheckBox chkSendToCustomer = (CheckBox)item.FindControl("chkSendToCustomer");

    //        int nEvent_id = Convert.ToInt32(grdScheduleNotification.DataKeys[item.RowIndex].Values[0]);
    //        int nCustomer_id = Convert.ToInt32(grdScheduleNotification.DataKeys[item.RowIndex].Values[4]);

    //        var objSMSCust = _db.ScheduleSMS.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCustomer_id && s.reference_type == 1 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate.Date);

    //        if (objSMSCust == null)
    //        {
    //            chkSendAll.Checked = chkboxSelectAll.Checked;
    //            chkSendToCustomer.Checked = chkboxSelectAll.Checked;
    //        }



    //        foreach (GridViewRow cRow in grdCrewList.Rows)
    //        {
    //            int nCrew_id = Convert.ToInt32(grdCrewList.DataKeys[cRow.RowIndex].Values[0]);
    //            string strPhone = grdCrewList.DataKeys[cRow.RowIndex].Values[2].ToString();

    //            CheckBox chkSendToCrew = cRow.FindControl("chkSendToCrew") as CheckBox;

    //            var objSMSCrew = _db.ScheduleSMS.FirstOrDefault(s => s.event_id == nEvent_id && s.reference_id == nCrew_id && s.reference_type == 2 && Convert.ToDateTime(s.schedule_date).Date == dtScheduleDate);

    //            if (objSMSCrew == null)
    //            {
    //                if (strPhone.Length > 0)
    //                    chkSendToCrew.Checked = chkSendAll.Checked;
    //            }
    //        }
    //    }
    //}
    //bool bFlagAll = true;
    //protected void chkSendAll_CheckedChanged(object sender, EventArgs e)
    //{
    //    lblResult.Text = "";

    //    int i = 0;
    //    CheckBox chkSendAll1 = (CheckBox)grdScheduleNotification.FindControl("chkSendAll");
    //    chkSendAll1 = (CheckBox)sender;
    //    GridViewRow gvr = (GridViewRow)chkSendAll1.NamingContainer;
    //    i = gvr.RowIndex;

    //    CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");
    //    CheckBox chkSendToCustomer = (CheckBox)grdScheduleNotification.Rows[i].FindControl("chkSendToCustomer");

    //    chkSendToCustomer.Checked = chkSendAll1.Checked;

    //    foreach (GridViewRow row in grdScheduleNotification.Rows)
    //    {

    //        CheckBox chkSendAll = (CheckBox)row.FindControl("chkSendAll");

    //        if (chkSendAll.Checked == false)
    //        {
    //            bFlagAll = false;
    //            break;
    //        }

    //    }

    //    chkboxSelectAll.Checked = bFlagAll;

    //    GridView grdCrewList = (GridView)grdScheduleNotification.Rows[i].FindControl("grdCrewList");

    //    foreach (GridViewRow cRow in grdCrewList.Rows)
    //    {
    //        string strPhone = grdCrewList.DataKeys[cRow.RowIndex].Values[2].ToString();
    //        CheckBox chkSendToCrew = cRow.FindControl("chkSendToCrew") as CheckBox;
    //        if (strPhone.Length > 0)
    //            chkSendToCrew.Checked = chkSendAll1.Checked;

    //    }

    //}

    //bool bFlagCust = true;
    //protected void chkSendToCustomer_CheckedChanged(object sender, EventArgs e)
    //{
    //    lblResult.Text = "";

    //    GridViewRow grdGroupingRow = (GridViewRow)((CheckBox)sender).NamingContainer;

    //    int Index = grdGroupingRow.RowIndex;

    //    GridView grdCrewList = grdScheduleNotification.Rows[Index].FindControl("grdCrewList") as GridView;
    //    CheckBox chkSendAll = (CheckBox)grdScheduleNotification.Rows[Index].FindControl("chkSendAll");
    //    CheckBox chkSendToCustomer = (CheckBox)grdScheduleNotification.Rows[Index].FindControl("chkSendToCustomer");
    //    CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");

    //    foreach (GridViewRow row in grdCrewList.Rows)
    //    {
    //        CheckBox chkSendToCrew = (CheckBox)row.FindControl("chkSendToCrew");

    //        if (chkSendToCrew.Checked == false)
    //        {
    //            bFlagCust = false;
    //            break;
    //        }
    //    }

    //    if (chkSendToCustomer.Checked)
    //    {
    //        chkSendAll.Checked = bFlagCust;
    //        chkboxSelectAll.Checked = bFlagCust;
    //    }
    //    else
    //    {
    //        chkSendAll.Checked = false;
    //        chkboxSelectAll.Checked = false;
    //    }

    //}

    //bool bFlagCrew = true;
    //protected void chkSendToCrew_CheckedChanged(object sender, EventArgs e)
    //{
    //    lblResult.Text = "";

    //    GridViewRow grdGroupingRow = (GridViewRow)((CheckBox)sender).Parent.Parent.Parent.Parent.NamingContainer;

    //    int Index = grdGroupingRow.RowIndex;

    //    GridView grdCrewList = grdScheduleNotification.Rows[Index].FindControl("grdCrewList") as GridView;
    //    CheckBox chkSendAll = (CheckBox)grdScheduleNotification.Rows[Index].FindControl("chkSendAll");
    //    CheckBox chkboxSelectAll = (CheckBox)grdScheduleNotification.HeaderRow.FindControl("chkboxSelectAll");

    //    foreach (GridViewRow row in grdScheduleNotification.Rows)
    //    {

    //        CheckBox chkSendToCustomer = (CheckBox)row.FindControl("chkSendToCustomer");

    //        if (chkSendToCustomer.Checked == false)
    //        {
    //            bFlagAll = false;
    //            break;
    //        }

    //    }

    //    foreach (GridViewRow row in grdCrewList.Rows)
    //    {
    //        CheckBox chkSendToCrew = (CheckBox)row.FindControl("chkSendToCrew");

    //        if (chkSendToCrew.Checked == false)
    //        {
    //            bFlagCrew = false;
    //            break;
    //        }
    //    }

    //    chkSendAll.Checked = bFlagCrew;
    //    chkboxSelectAll.Checked = bFlagCrew;
    //}

    protected bool IsHoliday(DateTime dt)
    {
        bool IsHoliday = false;
        DateTime date = DateTime.Parse(dt.ToShortDateString());
        HolidayCalculator hc;
        if (HttpContext.Current.Session["hc"] == null)
        {
            hc = new HolidayCalculator(date, "Holidays.xml");
        }
        else
        {
            hc = (HolidayCalculator)HttpContext.Current.Session["hc"];
        }


        foreach (HolidayCalculator.Holiday h in hc.OrderedHolidays)
        {
            if (h.Date.ToShortDateString() == date.ToShortDateString())
            {
                IsHoliday = true;
            }
        }
        return IsHoliday;
    }

  
}