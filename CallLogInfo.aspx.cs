using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class CallLogInfo : System.Web.UI.Page
{
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            Session.Remove("call");
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }


            cmbStartTime.DataSource = GetStartTimeList();
            cmbStartTime.SelectedValue = "6:00 AM";
            cmbStartTime.DataBind();

            cmbEndTime.DataSource = GetEndTimeList();
            cmbEndTime.SelectedValue = "7:00 AM";
            cmbEndTime.DataBind();
            if (Page.User.IsInRole("calllog002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
                hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
            hdnCallLogId.Value = "0";
            //if (Request.QueryString.Get("callid") != null)
            //    hdnCallLogId.Value = Convert.ToInt32(Request.QueryString.Get("callid")).ToString();

            if (Request.QueryString.Get("eid") != null)
            {
                int neid = Convert.ToInt32(Request.QueryString.Get("eid"));
                hdnEstimateId.Value = neid.ToString();
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

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
                lblCompany.Text = cust.company;

                hdnSalespersonId.Value = cust.sales_person_id.ToString();
                hdnAddress.Value = cust.address + " " + cust.city + ", " + cust.state + " " + cust.zip_code;
                hdnLastName.Value = cust.last_name1;

                txtCallStartDate.Text = DateTime.Today.ToShortDateString();
                ddlCallHour.SelectedItem.Text = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
                ddlCallMinutes.SelectedItem.Text = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
                ddlCallAMPM.SelectedValue = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
                GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));

                if (Request.QueryString.Get("callid") != null)
                {

                    if (Convert.ToInt32(Request.QueryString.Get("callid")) != 0)
                        hdnCallLogId.Value = Request.QueryString.Get("callid").ToString();
                    else
                    {
                        if (_db.CustomerCallLogs.Any(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3))
                            hdnCallLogId.Value = _db.CustomerCallLogs.Where(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3).Max(c => c.CallLogID).ToString();
                    }

                    GetCallLogDeatils(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnCallLogId.Value));
                    btnSaveCall.Focus();
                }

            }

            if (hdnEmailType.Value == "1")
            {
                HyperLink1.Attributes.Add("onClick", "NewEmailWindowOutlook();");               
            }
            else
            {
                HyperLink1.Attributes.Add("onClick", "NewEmailWindow();");               
            }

            // HyperLink1.Attributes.Add("onClick", "DisplayWindow();");

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSaveAndReturn", "btnSaveCall", "HyperLink1", "btnCancel", "btnSalesCalendar", "chkFollowup" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Edit" });
        }
        else
        {
            if (Session["FromEmailPage"] != null)
            {
                Session.Remove("FromEmailPage");
                GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));

            }
        }
    }

    private void GetCallLogDeatils(int nCustId, int nCallId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCallLogId.Value) > 0)
        {
            ScheduleCalendar objsc = new ScheduleCalendar();
            CustomerCallLog custCall = new CustomerCallLog();
            custCall = _db.CustomerCallLogs.Single(c => c.CallLogID == Convert.ToInt32(hdnCallLogId.Value) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

            int nCallType = 1;

            DateTime dtCallDate = Convert.ToDateTime(custCall.CallDateTime);



            txtCallSubject.Text = custCall.CallSubject;
            txtCallStartDate.Text = custCall.CallDate;
            //ddlCallHour.SelectedValue = custCall.CallHour;
            //ddlCallMinutes.SelectedValue = custCall.CallMinutes;
            //ddlCallAMPM.SelectedValue = custCall.CallAMPM;

            ddlCallHour.SelectedItem.Text = dtCallDate.ToString("hh", CultureInfo.InvariantCulture);
            ddlCallMinutes.SelectedItem.Text = dtCallDate.ToString("mm", CultureInfo.InvariantCulture);
            ddlCallAMPM.SelectedValue = dtCallDate.ToString("tt", CultureInfo.InvariantCulture);

            txtDurationH.Text = custCall.DurationHour;
            ddlDurationMin.SelectedValue = custCall.DurationMinutes;

            txtCallDescription.Text = custCall.Description;
            if (custCall.CallTypeId != null)
                nCallType = Convert.ToInt32(custCall.CallTypeId);

            ddlCallType.SelectedValue = nCallType.ToString();
            txtFollowupDate.Text = custCall.FollowDate;
            ddlFollowHour.SelectedValue = custCall.FollowHour;
            ddlFollowMin.SelectedValue = custCall.FollowMinutes;
            ddlFollowAMPM.SelectedValue = custCall.FollowAMPM;

            if (Convert.ToInt32(custCall.CallTypeId) == 3)
            {
                tblApptDate.Visible = true;
                tblApptTime.Visible = true;
            }
            else
            {
                tblApptDate.Visible = false;
                tblApptTime.Visible = false;
            }
            if (Convert.ToBoolean(custCall.IsFollowUp))
            {
                chkFollowup.Checked = true;
                tblFollowUp.Visible = true;
            }
            else
            {
                chkFollowup.Checked = false;
                tblFollowUp.Visible = false;
            }
            if (Convert.ToBoolean(custCall.IsDoNotCall))
            {
                ChkDoNotCall.Checked = true;
            }
            else
            {
                ChkDoNotCall.Checked = false;
            }
            if (Convert.ToDateTime(custCall.AppointmentDateTime).Year != 1900 && custCall.AppointmentDateTime != null)
            {
                txtAppointmentDate.Text = Convert.ToDateTime(custCall.AppointmentDateTime).ToShortDateString();
                string startTime = Convert.ToDateTime(custCall.AppointmentDateTime).ToShortTimeString();
                string endTime = "";
                if (startTime == "12:00 AM")
                {
                    startTime = "6:00 AM";
                    endTime = "7:00 AM";
                }
                else
                {
                    if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == nCallId))
                    {
                        objsc = _db.ScheduleCalendars.Single(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == nCallId);
                        endTime = Convert.ToDateTime(objsc.event_end).ToShortTimeString();
                    }
                    else
                    {
                        endTime = startTime;
                    }
                }

                // Appointment Start Time
                ListItem item = cmbStartTime.Items.FindByText(startTime);
                if (item != null)
                    cmbStartTime.SelectedValue = startTime;
                else
                {
                    cmbStartTime.Items.Insert(0, new ListItem(startTime));
                    cmbStartTime.SelectedValue = startTime;
                }

                //Appointment End Time
                ListItem eitem = cmbEndTime.Items.FindByText(endTime);
                if (eitem != null)
                    cmbEndTime.SelectedValue = endTime;
                else
                {
                    cmbEndTime.Items.Insert(0, new ListItem(endTime));
                    cmbEndTime.SelectedValue = endTime;
                }
            }
        }

    }


    public bool IsCallLogDataValid()
    {
        bool IsDataValid = true;

        string strRequired = "";

        if (txtCallSubject.Text == "")
            strRequired = "Missing required field: Subject.<br/>";
        if (txtCallStartDate.Text == "")
            strRequired += "Missing required field: StartDate.<br/>";
        if (Convert.ToBoolean(chkFollowup.Checked) == true)
        {
            if (txtFollowupDate.Text == "")
                strRequired += "Missing required field: Followup Date.<br/>";
        }
        if (txtCallDescription.Text == "")
            strRequired += "Missing required field: Notes.<br/>";

        if (strRequired.Length > 0)
        {
            lblResultCallLog.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            IsDataValid = false;
        }
        return IsDataValid;
    }

    public bool SaveCallLogData()
    {
        bool IsDataSaved = true;

        try
        {
            if (IsCallLogDataValid())
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                string strFollowupDate = "1900-01-01 00:00:00.000";
                if (txtFollowupDate.Text != "")
                {
                    strFollowupDate = txtFollowupDate.Text + " " + ddlFollowHour.SelectedValue + ":" + ddlFollowMin.SelectedValue + " " + ddlFollowAMPM.SelectedValue;

                }



                string strCallDateTime = txtCallStartDate.Text + " " + ddlCallHour.SelectedValue + ":" + ddlCallMinutes.SelectedValue + " " + ddlCallAMPM.SelectedValue;

                CustomerCallLog custCall = new CustomerCallLog();
                if (Convert.ToInt32(hdnCallLogId.Value) > 0)
                    custCall = _db.CustomerCallLogs.Single(c => c.CallLogID == Convert.ToInt32(hdnCallLogId.Value));

                custCall.CallLogID = Convert.ToInt32(hdnCallLogId.Value);
                custCall.CallSubject = txtCallSubject.Text;
                custCall.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                custCall.CallDate = txtCallStartDate.Text;
                custCall.CallHour = ddlCallHour.SelectedValue;
                custCall.CallMinutes = ddlCallMinutes.SelectedValue;
                custCall.CallAMPM = ddlCallAMPM.SelectedValue;
                if (txtDurationH.Text == "" || txtDurationH.Text == "0")
                {
                    custCall.CallDuration = ddlDurationMin.SelectedValue + " Minutes";
                    custCall.DurationHour = "0";
                }
                else
                {
                    custCall.CallDuration = txtDurationH.Text + " Hour " + ddlDurationMin.SelectedValue + " Minutes";
                    custCall.DurationHour = txtDurationH.Text;
                }
                custCall.DurationMinutes = ddlDurationMin.SelectedValue;
                custCall.Description = txtCallDescription.Text;
                custCall.CreatedByUser = User.Identity.Name;
                custCall.CreateDate = Convert.ToDateTime(DateTime.Now);
                custCall.CallDateTime = Convert.ToDateTime(strCallDateTime);

                custCall.CallTypeId = Convert.ToInt32(ddlCallType.SelectedValue);
                custCall.IsFollowUp = Convert.ToBoolean(chkFollowup.Checked);
                custCall.FollowDate = txtFollowupDate.Text;
                custCall.FollowHour = ddlFollowHour.SelectedValue;
                custCall.FollowMinutes = ddlFollowMin.SelectedValue;
                custCall.FollowAMPM = ddlFollowAMPM.SelectedValue;
                custCall.FollowDateTime = Convert.ToDateTime(strFollowupDate);
                custCall.IsDoNotCall = Convert.ToBoolean(ChkDoNotCall.Checked);
                custCall.sales_person_id = Convert.ToInt32(hdnSalespersonId.Value);

                DateTime dt = Convert.ToDateTime("1900-01-01");
                if (txtAppointmentDate.Text.Trim() != "")
                {
                    try
                    {
                        dt = Convert.ToDateTime(txtAppointmentDate.Text);
                        custCall.AppointmentDateTime = DateTime.Parse(dt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString());
                        // cust.appointment_date = DateTime.Parse(dt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString());
                    }
                    catch
                    {
                        dt = Convert.ToDateTime(txtAppointmentDate.Text);
                        custCall.AppointmentDateTime = dt;

                    }

                }
                else
                {
                    custCall.AppointmentDateTime = dt;
                    // cust.appointment_date = dt;
                }

                if (Convert.ToInt32(hdnCallLogId.Value) == 0)
                {
                    _db.CustomerCallLogs.InsertOnSubmit(custCall);
                    lblResultCallLog.Text = csCommonUtility.GetSystemMessage("Activity log has been saved successfully.");
                }
                else
                {
                    lblResultCallLog.Text = csCommonUtility.GetSystemMessage("Activity log has been updated successfully.");
                }

                _db.SubmitChanges();
                hdnCallLogId.Value = custCall.CallLogID.ToString();

                #region Save Appointment

                userinfo objUName = (userinfo)Session["oUser"];
                string strUName = objUName.first_name;
                string strClassName = "fc-sales";
                //Google Calendar
                sales_person objSP = new sales_person();
                string calendarId = string.Empty;
                //calendarId = ConfigurationManager.AppSettings["GoogleSalesCalendarID"];

                //Get Sales Person ID
                int nSalesPersonID = Convert.ToInt32(hdnSalespersonId.Value);

                if (txtAppointmentDate.Text != "")
                {
                    if (custCall.AppointmentDateTime > DateTime.Now)
                    {
                        try
                        {
                            string salesRep = string.Empty;

                            string sqlCustApp = "Update customers set appointment_date = '" + custCall.AppointmentDateTime + "'   WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
                            _db.ExecuteCommand(sqlCustApp, string.Empty);

                            DateTime ndt = Convert.ToDateTime(GetDayOfWeek(txtAppointmentDate.Text));
                            var GoogleEventID = "";

                            //Get calendarId by Sales Person ID
                            //if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
                            //{
                            //    objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                            //    calendarId = objSP.google_calendar_id ?? "";
                            //    salesRep = objSP.first_name + " " + objSP.last_name;
                            //}
                            if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID).Count() > 0)
                            {
                                objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID).SingleOrDefault();

                                if (objSP.google_calendar_account != null && objSP.google_calendar_account != "")
                                    calendarId = objSP.google_calendar_id ?? "";

                                salesRep = objSP.first_name + " " + objSP.last_name;
                            }


                            if (calendarId != "")
                            {
                                // Google Calendar DELETE------------
                                if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                                {
                                    List<ScheduleCalendar> sclist = _db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == custCall.CallLogID).ToList();
                                    foreach (ScheduleCalendar sc in sclist)
                                    {
                                        if (sc.google_event_id != "")
                                        {
                                            var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                                            var service = new GoogleCalendarServiceProxy(authenticator);
                                            service.DeleteEvent(calendarId, sc.google_event_id); // Delete
                                        }
                                    }
                                }

                                ////Google Calendar Insert----------------------------------------------------------
                                if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                                {
                                    var calendarEvent = new gCalendarEvent()
                                    {
                                        CalendarId = calendarId,
                                        Title = (hdnLastName.Value.Trim() + " " + lblPhone.Text.Trim() + " (" + salesRep + ")").Trim(),
                                        Location = lblAddress.Text + ", USA",
                                        StartDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString()),
                                        EndDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTime.SelectedValue.ToString()),
                                        Description = txtCallDescription.Text.Trim(),
                                        ColorId = 1
                                    };

                                    var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                                    var service = new GoogleCalendarServiceProxy(authenticator);
                                    GoogleEventID = service.CreateEvent(calendarEvent);
                                }
                                ////Google Calendar Insert End Code----------------------------------------------------------
                            }

                            // Calendar DELETE------------
                            if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == custCall.CallLogID).Count() > 0)
                            {

                                string sql2 = "DELETE ScheduleCalendar WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND type_id = 2 AND estimate_id =" + custCall.CallLogID;
                                _db.ExecuteCommand(sql2, string.Empty);

                            }
                            int nEventId = 1;

                            if (_db.ScheduleCalendarTemps.Any())
                            {
                                int nMaxSC = Convert.ToInt32(_db.ScheduleCalendars.DefaultIfEmpty().Max(e => e == null ? 0 : e.event_id));
                                int nMaxSCTemp = Convert.ToInt32(_db.ScheduleCalendarTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.event_id));

                                if (nMaxSCTemp > nMaxSC)
                                    nEventId = nMaxSCTemp + 1;
                                else
                                    nEventId = nMaxSC + 1;
                            }
                            ScheduleCalendar objsc = new ScheduleCalendar();
                            if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == custCall.CallLogID).Count() == 0)
                            {
                                objsc.event_id = nEventId;
                                objsc.title = txtCallSubject.Text.Trim();
                                objsc.description = txtCallDescription.Text.Trim();
                                objsc.event_start = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString());
                                objsc.event_end = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTime.SelectedValue.ToString());
                                objsc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                                objsc.estimate_id = custCall.CallLogID;
                                objsc.employee_id = 0;
                                objsc.section_name = "";
                                objsc.location_name = "";
                                objsc.type_id = 2;
                                objsc.create_date = DateTime.Now;
                                objsc.last_updated_date = DateTime.Now;
                                objsc.last_updated_by = strUName;
                                objsc.parent_id = 0;
                                objsc.job_start_date = DateTime.Now;
                                objsc.co_pricing_list_id = 0;
                                objsc.cssClassName = strClassName;
                                objsc.google_event_id = GoogleEventID;
                                objsc.operation_notes = "";                            
                                objsc.is_complete = false;
                                objsc.duration = 1;
                                objsc.IsEstimateActive = true;

                                _db.ScheduleCalendars.InsertOnSubmit(objsc);
                                _db.SubmitChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            lblResultCallLog.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                            IsDataSaved = false;
                        }
                    }
                }
                #endregion



            }
            else
            {
                IsDataSaved = false;
            }

        }
        catch (Exception ex)
        {
            lblResultCallLog.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            IsDataSaved = false;
        }
        return IsDataSaved;

    }

    protected void btnSaveAndReturn_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveAndReturn.ID, btnSaveAndReturn.GetType().Name, "Click"); 
        Session.Add("CallCustId", hdnCustomerId.Value);
        if (SaveCallLogData())
        {
            if (Request.QueryString.Get("TypeId") != null)
            {
                int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
                if (nTypeId == 3)
                    Response.Redirect("leadlist.aspx");
                else if (nTypeId == 2)
                    Response.Redirect("customerlist.aspx");
                else
                    Response.Redirect("CallActivityList.aspx");

            }
            else
                Response.Redirect("CallActivityList.aspx");
        }
    }

    protected void btnSaveCall_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveCall.ID, btnSaveCall.GetType().Name, "Click"); 
        SaveCallLogData();
        GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));
        ResetCallLog();

    }

    private static string[] startTimeListText;
    public string[] GetStartTimeList()
    {
        if (null == startTimeListText)
        {
            string[] tempStartTimeListText = new string[] {
        "6:00 AM",
        "6:30 AM",
        "7:00 AM",
        "7:30 AM",
        "8:00 AM",
        "8:30 AM",
        "9:00 AM",
        "9:30 AM",
        "10:00 AM",
        "10:30 AM",
        "11:00 AM",
        "11:30 AM",
        "12:00 PM",
        "12:30 PM",
        "1:00 PM",
        "1:30 PM",
        "2:00 PM",
        "2:30 PM",
        "3:00 PM",
        "3:30 PM",
        "4:00 PM",
        "4:30 PM",
        "5:00 PM",
        "5:30 PM",
        "6:00 PM"
        };
            // Array.Sort(tempStartTimeListText);
            startTimeListText = tempStartTimeListText;
        }
        return startTimeListText;
    }

    private static string[] endTimeListText;
    public string[] GetEndTimeList()
    {
        if (null == endTimeListText)
        {
            string[] tempEndTimeListText = new string[] {
        "6:00 AM",
        "6:30 AM",
        "7:00 AM",
        "7:30 AM",
        "8:00 AM",
        "8:30 AM",
        "9:00 AM",
        "9:30 AM",
        "10:00 AM",
        "10:30 AM",
        "11:00 AM",
        "11:30 AM",
        "12:00 PM",
        "12:30 PM",
        "1:00 PM",
        "1:30 PM",
        "2:00 PM",
        "2:30 PM",
        "3:00 PM",
        "3:30 PM",
        "4:00 PM",
        "4:30 PM",
        "5:00 PM",
        "5:30 PM",
        "6:00 PM"
        };
            // Array.Sort(tempEndTimeListText);
            endTimeListText = tempEndTimeListText;
        }
        return endTimeListText;
    }
    private GoogleAuthenticator GetAuthenticator(int salespersonid)
    {
        //var authenticator = (GoogleAuthenticator)Session["authenticator"];

        //if (authenticator == null || !authenticator.IsValid)
        //{
        DataClassesDataContext _db = new DataClassesDataContext();
        // Get a new Authenticator using the Refresh Token
        int nUserID = salespersonid;
        var refreshToken = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserID == salespersonid).RefreshToken;
        var authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
        //    System.Web.HttpContext.Current.Session["authenticator"] = authenticator;
        //}

        return authenticator;
    }
    private string GetDayOfWeek(string strdt)
    {
        int cnt = 0;
        DateTime dt = Convert.ToDateTime(strdt);

        if (dt.DayOfWeek == DayOfWeek.Saturday)
            cnt = +2;
        else if (dt.DayOfWeek == DayOfWeek.Sunday)
            cnt++;
        else if (IsHoliday(dt))
        {
            DateTime hdt = dt.AddDays(1);

            if (hdt.DayOfWeek == DayOfWeek.Saturday)
                cnt = +3;
            else if (hdt.DayOfWeek == DayOfWeek.Sunday)
                cnt = +2;
            else
                cnt++;
        }

        return dt.AddDays(cnt).ToShortDateString();
    }

    private bool IsHoliday(DateTime dt)
    {
        bool IsHoliday = false;
        DateTime date = DateTime.Parse(dt.ToShortDateString());
        HolidayCalculator hc = new HolidayCalculator(date, "Holidays.xml");
        foreach (HolidayCalculator.Holiday h in hc.OrderedHolidays)
        {
            if (h.Date.ToShortDateString() == date.ToShortDateString())
            {
                IsHoliday = true;
            }
        }
        return IsHoliday;
    }
    protected void cmbStartTime_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, cmbStartTime.ID, cmbStartTime.GetType().Name, "SelectedIndexChanged"); 
        //cmbStartTime.SelectedValue.ToString()
        DateTime dateTime = DateTime.ParseExact(cmbStartTime.SelectedValue.ToString(), "h:mm tt", CultureInfo.InvariantCulture).AddHours(1);
        string endTime = dateTime.ToShortTimeString();
        //Appointment End Time
        ListItem eitem = cmbEndTime.Items.FindByText(endTime);
        if (eitem != null)
            cmbEndTime.SelectedValue = endTime;
        else
        {
            cmbEndTime.Items.Insert(0, new ListItem(endTime));
            cmbEndTime.SelectedValue = endTime;
        }
    }



    protected void ddlCallType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlCallType.ID, ddlCallType.GetType().Name, "SelectedIndexChanged"); 
        if (Convert.ToInt32(ddlCallType.SelectedValue) == 3)
        {
            tblApptDate.Visible = true;
            tblApptTime.Visible = true;
        }
        else
        {
            tblApptDate.Visible = false;
            tblApptTime.Visible = false;
        }

    }
    private void GetCallLogInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var Call = from Call_info in _db.CustomerCallLogs
                       where Call_info.customer_id == nCustId
                       orderby Call_info.CallLogID descending
                       select Call_info;
            grdCallLog.DataSource = Call;
            grdCallLog.DataKeyNames = new string[] { "customer_id", "CallLogID", "CallTypeId", "IsFollowUp", "AppointmentDateTime", "FollowDateTime" };
            grdCallLog.DataBind();
        }

    }
    protected void chkFollowup_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkFollowup.ID, chkFollowup.GetType().Name, "CheckedChanged"); 
        if (Convert.ToBoolean(chkFollowup.Checked))
        {
            tblFollowUp.Visible = true;
        }
        else
        {
            tblFollowUp.Visible = false;
        }

    }
    protected void lnkAddNewCall_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkAddNewCall.ID, lnkAddNewCall.GetType().Name, "Click"); 
        lblResultCallLog.Text = string.Empty;
        ResetCallLog();
        //hdnCallLogId.Value = "0";
        //Response.Redirect("CallLogInfo.aspx?cid="+hdnCustomerId.Value);
    }
    protected void grdCallLog_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustId = Convert.ToInt32(grdCallLog.DataKeys[e.Row.RowIndex].Values[0]);
            int nCallId = Convert.ToInt32(grdCallLog.DataKeys[e.Row.RowIndex].Values[1]);
            int nCallTypeId = Convert.ToInt32(grdCallLog.DataKeys[e.Row.RowIndex].Values[2]);
            bool nIsFollowUp = Convert.ToBoolean(grdCallLog.DataKeys[e.Row.RowIndex].Values[3]);
            DateTime strAppointmentDateTime = Convert.ToDateTime(grdCallLog.DataKeys[e.Row.RowIndex].Values[4]);
            DateTime strFollowDateTime = Convert.ToDateTime(grdCallLog.DataKeys[e.Row.RowIndex].Values[5]);

            Label lblCallDescriptionG = (Label)e.Row.FindControl("lblCallDescriptionG");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            string str = lblCallDescriptionG.Text.Replace("&nbsp;", "");

            Label lblCallType = (Label)e.Row.FindControl("lblCallType");
            if (nCallTypeId == 1)
                lblCallType.Text = "Called";
            else if (nCallTypeId == 2)
                lblCallType.Text = "Pitched";
            else if (nCallTypeId == 6)
                lblCallType.Text = "Emailed";
            else if (nCallTypeId == 3)
            {

                DataClassesDataContext _db = new DataClassesDataContext();
                ScheduleCalendar objsc = new ScheduleCalendar();
                string apptDate = Convert.ToDateTime(strAppointmentDateTime).ToShortDateString();
                string startTime = Convert.ToDateTime(strAppointmentDateTime).ToShortTimeString();
                string endTime = "";
                if (startTime == "12:00 AM")
                {
                    startTime = "6:00 AM";
                    endTime = "7:00 AM";
                }
                else
                {
                    if (_db.ScheduleCalendars.Any(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallId))
                    {
                        objsc = _db.ScheduleCalendars.Single(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallId);
                        endTime = Convert.ToDateTime(objsc.event_end).ToShortTimeString();
                    }
                    else
                    {
                        endTime = startTime;
                    }
                }
                lblCallType.Text = "Booked Appt:" + apptDate + " </br>" + startTime + " to " + endTime;
            }
            else
            {
                lblCallType.Text = "";
                //lblCallType.Text = "Booked" + " </br>" + strAppointmentDateTime.ToString("MM/dd/yyyy  h:mm:ss tt");
            }
            Label lblFollowup = (Label)e.Row.FindControl("lblFollowup");
            string sFollowDate = string.Empty;
            if (nIsFollowUp)
            {
                sFollowDate = Convert.ToDateTime(strFollowDateTime).ToShortDateString();
                string sFollowTime = Convert.ToDateTime(strFollowDateTime).ToShortTimeString();
                //lblFollowup.Text = "Followup" + " </br>" + strFollowDateTime.ToString("MM/dd/yyyy  h:mm:ss tt");
                lblFollowup.Text = sFollowDate + " </br>" + sFollowTime;
            }
            else
                lblFollowup.Text = "";


            Label lblCallStartDateTime = (Label)e.Row.FindControl("lblCallStartDateTime");

            DateTime dtCallDate = Convert.ToDateTime(lblCallStartDateTime.Text);
            string sCallDate = Convert.ToDateTime(dtCallDate).ToShortDateString();
            if (dtCallDate.Year == 1900)
            {
                lblCallStartDateTime.Text = "";
            }
            else
            {
                string SCallTime = Convert.ToDateTime(dtCallDate).ToShortTimeString();
                lblCallStartDateTime.Text = sCallDate + " </br>" + SCallTime;


            }
            if (str != "" && str.Length > 90)
            {
                lblCallDescriptionG.Text = str.Substring(0, 90) + " ...";
                lblCallDescriptionG.ToolTip = str;
                lnkOpen.Visible = true;
            }
            else
            {
                lblCallDescriptionG.Text = str;
                lnkOpen.Visible = false;
            }


        }
    }

    protected void grdCallLog_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView grdCallLog = (GridView)sender;
        if (e.CommandName == "Select")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int index = Convert.ToInt32(e.CommandArgument);
            int nCustomerId = Convert.ToInt32(grdCallLog.DataKeys[index].Values[0].ToString());
            int nCallId = Convert.ToInt32(grdCallLog.DataKeys[index].Values[1].ToString());

            hdnCallLogId.Value = nCallId.ToString();
            GetCallLogDeatils(nCustomerId, nCallId);


        }

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCancel.ID, btnCancel.GetType().Name, "Click"); 
        Session.Add("CallCustId", hdnCustomerId.Value);
        if (Request.QueryString.Get("TypeId") != null)
        {
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            if (nTypeId == 3)
                Response.Redirect("leadlist.aspx");
            else if (nTypeId == 2)
                Response.Redirect("customerlist.aspx");
            else
                Response.Redirect("CallActivityList.aspx");

        }
        else
            Response.Redirect("CallActivityList.aspx");
    }



    protected void btnSalesCalendarC_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSalesCalendar.ID, btnSalesCalendar.GetType().Name, "Click"); 
        try
        {
            Session.Add("call", "test");
            SaveCallLogData();
            Response.Redirect("schedulecalendar.aspx?cid=" + hdnCustomerId.Value + "&TypeID=2&CallLogID=" + Convert.ToInt32(hdnCallLogId.Value));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void ResetCallLog()
    {
        PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        // make collection editable
        isreadonly.SetValue(this.Request.QueryString, false, null);
        Request.QueryString.Remove("callid");

        hdnCallLogId.Value = "0";
        txtCallSubject.Text = "";

        txtCallStartDate.Text = DateTime.Today.ToShortDateString();
        ddlCallHour.SelectedItem.Text = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
        ddlCallMinutes.SelectedItem.Text = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
        ddlCallAMPM.SelectedValue = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

        txtDurationH.Text = "";
        ddlDurationMin.SelectedIndex = -1;
        txtCallDescription.Text = "";
        ddlCallType.SelectedIndex = -1;
        chkFollowup.Checked = false;
        txtFollowupDate.Text = "";
        ddlFollowHour.SelectedIndex = -1;
        ddlFollowMin.SelectedIndex = -1;
        ddlFollowAMPM.SelectedIndex = -1;
        ChkDoNotCall.Checked = false;
        txtAppointmentDate.Text = "";
        tblApptDate.Visible = false;
        tblApptTime.Visible = false;
        tblFollowUp.Visible = false;

    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        //KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkOpen.ID, lnkOpen.GetType().Name, "Click"); 
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[2].Controls[0].FindControl("lblCallDescriptionG") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[2].Controls[1].FindControl("lblCallDescriptionG_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpen") as LinkButton;

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

    
}