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
using System.Drawing;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Net.Mail;
using Prabhu;
using System.Globalization;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;
using System.Net;

public partial class lead_details : System.Web.UI.Page
{
    protected string custId = "0";

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        string custEmail = "";
        if (Session["oUser"] == null)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
        else
        {
            userinfo oUser = (userinfo)Session["oUser"];
            hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

        }
        string test = hdnCustomerId.Value;
        if (Page.User.IsInRole("le02") == false)
        {
            // No Permission Page.
            Response.Redirect("nopermission.aspx");
        }
        if (Request.QueryString.Get("cid") != null)
        {
            hdnCustomerId.Value = Request.QueryString.Get("cid").ToString();
        }
        if (Request.QueryString.Get("eid") != null)
        {
            hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();

        }

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();

            BindDivision();

            hdnClientId.Value = ddlDivision.SelectedValue;

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {


                userinfo objUser = (userinfo)HttpContext.Current.Session["oUser"];

                if (objUser.role_id == 1 || objUser.role_id == 2)
                {

                    var user = (from u in _db.user_infos
                                join s in _db.sales_persons on u.sales_person_id equals s.sales_person_id
                                join c in _db.customers on s.sales_person_id equals c.sales_person_id
                                where c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.sales_person_id > 0
                                select u).SingleOrDefault();

                    if (user != null)
                    {

                        Session["EWS"] = EWSAPI.GetEWSService(user.company_email, user.email_password);
                    }

                }
            }



            if (Request.QueryString.Get("callid") != null)
            {

                if (Convert.ToInt32(Request.QueryString.Get("callid")) != 0)
                {
                    hdnCallLogId.Value = Request.QueryString.Get("callid").ToString();
                }
                else
                {
                    if (_db.CustomerCallLogs.Any(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3))
                    {
                        hdnCallLogId.Value = _db.CustomerCallLogs.Where(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3).Max(c => c.CallLogID).ToString();
                    }
                }

                CollapsiblePanelExtender3.Collapsed = false;
                CollapsiblePanelExtender3.ClientState = "false";
                GetCallLogDeatils(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnCallLogId.Value));
                btnSaveCall.Focus();
            }


            //cmbStartTime.DataSource = GetStartTimeList();
            //cmbStartTime.SelectedValue = "6:00 AM";
            //cmbStartTime.DataBind();

            //cmbEndTime.DataSource = GetEndTimeList();
            //cmbEndTime.SelectedValue = "7:00 AM";
            //cmbEndTime.DataBind();

            cmbStartTimec.DataSource = GetStartTimeList();
            cmbStartTimec.SelectedValue = "6:00 AM";
            cmbStartTimec.DataBind();

            cmbEndTimec.DataSource = GetEndTimeList();
            cmbEndTimec.SelectedValue = "7:00 AM";
            cmbEndTimec.DataBind();

            //HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
            //HyperLink2.Attributes.Add("onClick", "DisplayWindow();");

            if (hdnEmailType.Value == "1")
            {
                HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
                HyperLink2.Attributes.Add("onClick", "DisplayWindow();");
            }
            else
            {
                HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
                HyperLink2.Attributes.Add("onClick", "DisplayWindow();");
            }

            txtCallStartDate.Text = DateTime.Today.ToShortDateString();
            ddlCallHour.SelectedItem.Text = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
            ddlCallMinutes.SelectedItem.Text = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
            ddlCallAMPM.SelectedValue = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

            


            BindStates();
            BindLeadStatus();
            BindSalesPerson();
            BindLeadSource();
            

            //int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            //hdnCustomerId.Value = ncid.ToString();
            custId = hdnCustomerId.Value;

            

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {

                lblHeaderTitle.Text = "Lead Details";

                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                custEmail = cust.email ?? "";
                hdnCustEmail.Value = custEmail;

                txtFirstName1.Text = cust.first_name1;
                txtLastName1.Text = cust.last_name1;
                txtFirstName2.Text = cust.first_name2;
                txtLastName2.Text = cust.last_name2;
                txtAddress.Text = cust.address;

                hypMap.Visible = true;
                hypMap.ToolTip = cust.address + ", " + cust.city + ", " + cust.state + " " + cust.zip_code;
                //hypMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + hypMap.ToolTip;
                string strAddress = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + strAddress;

                txtCompany.Text = cust.company;
                txtCrossStreet.Text = cust.cross_street;
                txtCity.Text = cust.city;
                ddlState.SelectedValue = cust.state;
                txtZipCode.Text = cust.zip_code;
                txtPhone.Text = cust.phone;
                txtMobile.Text = cust.mobile;


                //for (int i = 1; i <= ddlDivision.Items.Count; i++)
                //{
                //    if(cust.client_id == i)
                //    {
                //        ddlDivision.SelectedValue = i.ToString();
                //    }
                //}
                ddlDivision.SelectedValue = cust.client_id.ToString();
                hdnClientId.Value = cust.client_id.ToString();                

                txtFax.Text = cust.fax;
                txtEmail.Text = cust.email;
                txtEmail2.Text = cust.email2;
                txtWebsite.Text = cust.website;
                lblCreatedBy.Text = "Created by: <font style='font-weight:bold'>" + cust.CreatedBy + "</font>";
                //ddlStatus.SelectedValue = cust.status_id.ToString();
                if (Convert.ToInt32(cust.status_id) == 5)
                {
                    rdbEstimateIsActive.SelectedValue = "0";
                }
                else
                {
                    rdbEstimateIsActive.SelectedValue = "1";
                }
                if (cust.lead_status_id > 7)
                {
                    ddlLeadStatus.SelectedValue = cust.lead_status_id.ToString();
                }
                ListItem item = ddlSalesPerson.Items.FindByValue(cust.sales_person_id.ToString());
                if (item != null)
                    this.ddlSalesPerson.Items.FindByValue(cust.sales_person_id.ToString()).Selected = true;
                else
                {
                    string str1 = cust.sales_person_id.ToString();
                    sales_person sep = _db.sales_persons.FirstOrDefault(w => w.sales_person_id == cust.sales_person_id);
                    string strSalesPerson = sep.first_name + " " + sep.last_name;
                    ddlSalesPerson.Items.Insert(0, new ListItem(strSalesPerson, str1));
                }
               // ddlSalesPerson.SelectedValue = cust.sales_person_id.ToString();
                txtNotes.Text = cust.notes;

                lblRegDate.Visible = true;
                lblRegDateData.Visible = true;
                lblRegDateData.Text = Convert.ToDateTime(cust.registration_date).ToShortDateString();
                //if (Convert.ToDateTime(cust.appointment_date).ToShortDateString() != "1/1/1900" )
                GetAppointmentDate(cust);

                ddlLeadSource.SelectedValue = cust.lead_source_id.ToString();

                Session.Add("LeadId", hdnCustomerId.Value);

                txtCallStartDate.Text = DateTime.Today.ToShortDateString();
                ddlCallHour.SelectedItem.Text = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
                ddlCallMinutes.SelectedItem.Text = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
                ddlCallAMPM.SelectedValue = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);


                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

                GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));
                GetCustomerContactInfo(Convert.ToInt32(hdnCustomerId.Value));

                GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));




            }
            else
            {
                hypMap.Visible = false;
                lblHeaderTitle.Text = "Add New Lead";
                hdnCustomerId.Value = "0";
                lblRegDate.Visible = false;
                lblRegDateData.Visible = false;
                Session.Remove("LeadId");
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "rdbEstimateIsActive", "btnUpload", "HyperLink1", "HyperLink2", "btnSaveCall", "btnSalesCalendar", "btnSaveContact", "ddlLeadSource", "ddlSalesPerson", "ddlLeadStatus", "hypMap", "btnImageGallery" });

        }
        else
        {
            if (Session["FromEmailPage"] != null)
            {
                Session.Remove("FromEmailPage");
                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));
                GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));

            }
        }

    }

    public void GetAppointmentDate(customer cust)
    {
        if (Convert.ToDateTime(cust.appointment_date).Year != 1900)
        {
            int nCallID = 0;
            DataClassesDataContext _db = new DataClassesDataContext();
            ScheduleCalendar objsc = new ScheduleCalendar();
            btnSalesCalendar.Visible = true;
            // txtAppointmentDate.Text = Convert.ToDateTime(cust.appointment_date).ToShortDateString();
            lblAppointmentDate.Text = Convert.ToDateTime(cust.appointment_date).ToShortDateString();
            string startTime = Convert.ToDateTime(cust.appointment_date).ToShortTimeString();
            string endTime = "";
            if (startTime == "12:00 AM")
            {
                //startTime = "6:00 AM";
                //endTime = "7:00 AM";

                startTime = "";
                endTime = "";
            }
            else
            {
                if (Convert.ToInt32(hdnCallLogId.Value) != 0)
                    nCallID = Convert.ToInt32(hdnCallLogId.Value);
                else
                {
                    if (_db.CustomerCallLogs.Any(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3))
                        nCallID = _db.CustomerCallLogs.Where(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3).Max(c => c.CallLogID);
                }

                if (_db.ScheduleCalendars.Any(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == nCallID))
                {
                    objsc = _db.ScheduleCalendars.Single(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == nCallID);
                    endTime = Convert.ToDateTime(objsc.event_end).ToShortTimeString();
                }
                else
                {
                    endTime = startTime;
                }
            }

            // Appointment Start Time
            lblStartTime.Text = startTime;
            //ListItem item = cmbStartTime.Items.FindByText(startTime);
            //if (item != null)
            //    cmbStartTime.SelectedValue = startTime;
            //else
            //{
            //    cmbStartTime.Items.Insert(0, new ListItem(startTime));
            //    cmbStartTime.SelectedValue = startTime;
            //}

            //Appointment End Time
            lblEndTime.Text = endTime;
            //ListItem eitem = cmbEndTime.Items.FindByText(endTime);
            //if (eitem != null)
            //    cmbEndTime.SelectedValue = endTime;
            //else
            //{
            //    cmbEndTime.Items.Insert(0, new ListItem(endTime));
            //    cmbEndTime.SelectedValue = endTime;
            //}
        }
        else
        {
            btnSalesCalendar.Visible = false;
        }
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


    private void BindStates()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var states = from st in _db.states
                     orderby st.abbreviation
                     select st;
        ddlState.DataSource = states;
        ddlState.DataTextField = "abbreviation";
        ddlState.DataValueField = "abbreviation";
        ddlState.DataBind();
    }
    private void BindLeadStatus()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var LeadStatus = from st in _db.lead_status
                         where st.lead_status_id > 7
                         orderby st.lead_status_name
                         select st;
        ddlLeadStatus.DataSource = LeadStatus;
        ddlLeadStatus.DataTextField = "lead_status_name";
        ddlLeadStatus.DataValueField = "lead_status_id";
        ddlLeadStatus.DataBind();
    }
    private void BindSalesPerson()
    {
       
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1  and is_sales=1 and sales_person.client_id in ('" + Convert.ToInt32(hdnClientId.Value) + "') order by sales_person_id asc";
        DataTable mList = csCommonUtility.GetDataTable(strQ);
        ddlSalesPerson.DataSource = mList;
        ddlSalesPerson.DataTextField = "sales_person_name";
        ddlSalesPerson.DataValueField = "sales_person_id";
        ddlSalesPerson.DataBind();

    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(hdnClientId.Value) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;
        ddlLeadSource.DataSource = item;
        ddlLeadSource.DataTextField = "lead_name";
        ddlLeadSource.DataValueField = "lead_source_id";

        ddlLeadSource.DataBind();
        ddlLeadSource.Items.Insert(0, "Select Lead Source");
        ddlLeadSource.SelectedIndex = 0;
    }

    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            ddlDivision.DataSource = dt;
            ddlDivision.DataTextField = "division_name";
            ddlDivision.DataValueField = "id";
            ddlDivision.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("leadlist.aspx");
    }

    public void SaveCustomerData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (Request.QueryString.Get("cid") == null)
        {
            customer custcheck = new customer();
            if (_db.customers.Any(c => c.first_name1 == txtFirstName1.Text && c.last_name1 == txtLastName1.Text && c.email == txtEmail.Text))
            {
                custcheck = _db.customers.SingleOrDefault(c => c.first_name1 == txtFirstName1.Text && c.last_name1 == txtLastName1.Text && c.email == txtEmail.Text);
                if (custcheck != null)
                    hdnCustomerId.Value = custcheck.customer_id.ToString();
            }
        }
       
        customer cust = new customer();
        int nCount = GetCountCustomer();
        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

        txtPhone.Text = csCommonUtility.GetPhoneFormat(txtPhone.Text.Trim());
        txtFax.Text = csCommonUtility.GetPhoneFormat(txtFax.Text.Trim());
        txtMobile.Text = csCommonUtility.GetPhoneFormat(txtMobile.Text.Trim());

        cust.address = txtAddress.Text;
        cust.city = txtCity.Text;
        
        cust.company = txtCompany.Text;
        cust.cross_street = txtCrossStreet.Text;
        cust.email2 = txtEmail2.Text;
        cust.fax = txtFax.Text;
        cust.first_name1 = txtFirstName1.Text;
        cust.first_name2 = txtFirstName2.Text;
        cust.is_active = Convert.ToBoolean(1);
        cust.last_name1 = txtLastName1.Text;
        cust.last_name2 = txtLastName2.Text;
        cust.phone = txtPhone.Text;
        cust.mobile = txtMobile.Text;
        cust.state = ddlState.SelectedItem.Text;
        cust.sales_person_id = Convert.ToInt32(ddlSalesPerson.SelectedValue);
        cust.zip_code = txtZipCode.Text;
        cust.update_date = Convert.ToDateTime(DateTime.Now);
        cust.website = txtWebsite.Text.Trim();

        //cust.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        
        cust.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
        

        DateTime dt = Convert.ToDateTime("1900-01-01");

        cust.notes = txtNotes.Text;
        cust.lead_source_id = Convert.ToInt32(ddlLeadSource.SelectedValue);
        cust.lead_status_id = Convert.ToInt32(ddlLeadStatus.SelectedValue);
        cust.islead = 1;
        

        cust.status_note = "";

        if (Convert.ToInt32(hdnCustomerId.Value) == 0)
        {
            cust.appointment_date = dt;

            if (rdbEstimateIsActive.SelectedValue == "0")
            {
                cust.status_id = 5; // InActive status
            }
            else
            {
                cust.status_id = 2;
                //if (Convert.ToInt32(ddlSalesPerson.SelectedValue) == 0)
                //    cust.status_id = 1;//
                //else
                //    cust.status_id = 2;
            }
            cust.IsEnableSMS = true;
            cust.CustomerCalendarWeeklyView = 1;
            cust.isJobSatusViewable = true;
            cust.isCalendarOnline = true;
            cust.isCustomer = 0;
            cust.email = txtEmail.Text;
            cust.registration_date = Convert.ToDateTime(DateTime.Now);
            if (Session["oUser"] != null)
            {
                userinfo oUser = (userinfo)Session["oUser"];
                cust.CreatedBy = oUser.first_name + " " + oUser.last_name;
            }
            cust.SuperintendentId = 0;
            
            _db.customers.InsertOnSubmit(cust);
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead '" + txtLastName1.Text + "' has been saved successfully.");

            _db.SubmitChanges();
            hdnCustomerId.Value = cust.customer_id.ToString();
            Session.Add("LeadId", cust.customer_id);
            updateCustomersLatLng(cust.customer_id);
        }
        else
        {
            if (rdbEstimateIsActive.SelectedValue == "0")
            {
                cust.status_id = 5; // InActive status
            }
            else
            {
                cust.status_id = 2;
                //if (Convert.ToInt32(ddlSalesPerson.SelectedValue) == 0)
                //    cust.status_id = 1;// Convert.ToInt32(ddlStatus.SelectedValue);
                //else
                //    cust.status_id = 2;
            }
            cust.email = txtEmail.Text;
            cust.update_date = DateTime.Now;

            _db.SubmitChanges();
            updateCustomersLatLng(cust.customer_id);

            lblResult.Text = csCommonUtility.GetSystemMessage("Lead '" + txtLastName1.Text + "' has been updated successfully.");

            Session.Add("LeadId", hdnCustomerId.Value);

        }
        #region Schedule Calendar Block Code
        /* userinfo objUName = (userinfo)Session["oUser"];
        string strUName = objUName.first_name;
        string strClassName = "fc-sales";
        //Google Calendar
        sales_person objSP = new sales_person();
        string calendarId = string.Empty;
        //calendarId = ConfigurationManager.AppSettings["GoogleSalesCalendarID"];

        //Get Sales Person ID
        int nSalesPersonID = Convert.ToInt32(ddlSalesPerson.SelectedValue);

        if (txtAppointmentDate.Text != "")
        {
            try
            {
                DateTime ndt = Convert.ToDateTime(GetDayOfWeek(txtAppointmentDate.Text));
                var GoogleEventID = "";

                //Get calendarId by Sales Person ID
                if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
                {
                    objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                    calendarId = objSP.google_calendar_id;
                }

                if (calendarId != "")
                {
                    // Google Calendar DELETE------------
                    if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                    {
                        List<ScheduleCalendar> sclist = _db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2).ToList();
                        foreach (ScheduleCalendar sc in sclist)
                        {
                            if (sc.google_event_id != "")
                            {
                                var authenticator = GetAuthenticator(nSalesPersonID); //  Sales Persion ID
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
                            Title = (txtLastName1.Text.Trim() + " " + txtPhone.Text.Trim() + " (" + ddlSalesPerson.SelectedItem.ToString() + ")").Trim(),
                            Location = txtAddress.Text.Trim() + " " + txtCity.Text.Trim() + ", " + ddlState.SelectedItem.Text.Trim() + " " + txtZipCode.Text.Trim() + ", USA",
                            StartDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString()),
                            EndDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTime.SelectedValue.ToString()),
                            Description = txtNotes.Text.Trim(),
                            ColorId = 1
                        };

                        var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                        var service = new GoogleCalendarServiceProxy(authenticator);
                        GoogleEventID = service.CreateEvent(calendarEvent);
                    }
                    ////Google Calendar Insert End Code----------------------------------------------------------
                }

                // Calendar DELETE------------
                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2).Count() > 0)
                {

                    string sql2 = "DELETE ScheduleCalendar WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND type_id = 2";
                    _db.ExecuteCommand(sql2, string.Empty);

                }

                ScheduleCalendar objsc = new ScheduleCalendar();
                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2).Count() == 0)
                {
                    objsc.title = (txtLastName1.Text.Trim() + " (" + ddlSalesPerson.SelectedItem.ToString() + ")").Trim();
                    objsc.description = txtNotes.Text.Trim();
                    objsc.event_start = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString());
                    objsc.event_end = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTime.SelectedValue.ToString());
                    objsc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objsc.estimate_id = 0;
                    objsc.employee_id = 0;
                    objsc.section_name = "";
                    objsc.location_name = "";
                    objsc.type_id = 2;
                    objsc.create_date = DateTime.Now;
                    objsc.last_updated_date = DateTime.Now;
                    objsc.last_updated_by = strUName;
                    objsc.week_id = 0;
                    objsc.job_start_date = DateTime.Now;
                    objsc.co_pricing_list_id = 0;
                    objsc.cssClassName = strClassName;
                    objsc.google_event_id = GoogleEventID;
                    objsc.operation_notes = "";
                    objsc.trade_partner = "";
                    objsc.is_complete = false;
                    objsc.IsEstimateActive = true;

                    _db.ScheduleCalendars.InsertOnSubmit(objsc);
                    _db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace);
            }
        }*/
        #endregion
    }
    protected void btnUpdateLatlong_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateLatlong.ID, btnUpdateLatlong.GetType().Name, "Click"); 
        if (Request.QueryString.Get("cid") != null)
        {
            int cid = Convert.ToInt32(Request.QueryString.Get("cid"));

            updateCustomersLatLng(cid);
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead '" + txtLastName1.Text + "' has been updated successfully.");
        }

    }

    private void updateCustomersLatLng(int cid)
    {
        XDocument xdoc;
        WebResponse response = null;
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {

            if (_db.customers.Any(x => x.customer_id == cid))
            {
                customer Cuustomer = _db.customers.SingleOrDefault(x => x.customer_id == cid);

                string address = Cuustomer.address.Replace(".", "").Replace("Rd #", "Road ").Replace("Rd#", "Road ").Replace("#", " ");
                // string address = "2401 E Rio Salado PKWY  UNIT 1085";
                string fulladdress = address + ", " + Cuustomer.city.Trim() + ", " + Cuustomer.state.Trim() + " " + Cuustomer.zip_code;
                //  string fulladdress = address;

                string url = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?address=" +
                    fulladdress + "&key=AIzaSyAg9x2CEaBTyFmwXm75gvfmQVuOGcSND0Y");

                WebRequest request = WebRequest.Create(url);
                request.UseDefaultCredentials = true;
                try
                {
                    response = request.GetResponse();

                    xdoc = XDocument.Load(response.GetResponseStream());

                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement locationElement = result.Element("geometry").Element("location");
                    XElement Latitude = locationElement.Element("lat");
                    XElement Longitude = locationElement.Element("lng");

                    string lat = (string)Latitude.Value;
                    string lang = (string)Longitude.Value;
                    Cuustomer.Latitude = lat;
                    Cuustomer.Longitude = lang;

                }
                catch
                {
                    response = null;
                }

                if (response != null)
                {
                    _db.SubmitChanges();
                }
            }
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("There is an error . Because " + ex);

        }

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (!IsCustomerDataValid())
            return;

        SaveCustomerData();
        //Reset();
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
    }

    private bool IsCustomerDataValid()
    {
        lblResult.Text = "";

        bool bflag = true;

        string strRequired = string.Empty;

        if (txtFirstName1.Text.Trim() == "")
        {
            strRequired += "Missing required field: First Name 1.<br/>";
        }
        if (txtLastName1.Text.Trim() == "")
        {
            strRequired += "Missing required field: Last Name 1.<br/>";
        }
        if (txtAddress.Text.Trim() == "")
        {
            strRequired += "Missing required field: Address.<br/>";
        }
        if (txtCity.Text.Trim() == "")
        {
            strRequired += "Missing required field: City.<br/>";
        }

        if (txtZipCode.Text.Trim() == "")
        {
            strRequired += "Missing required field: Zip Code.<br/>";
        }
        if (txtEmail.Text.Trim() == "")
        {
            strRequired += "Missing required field: Email.<br/>";
        }

        //if (txtAppointmentDate.Text.Trim() == "")
        //{
        //    strRequired += "Missing required field: Appointment Date.<br/>";
        //}
        //else
        //{
        //    try
        //    {
        //        Convert.ToDateTime(txtAppointmentDate.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        strRequired += "Invalid Appointment Date.<br/>";
        //    }
        //}

        if (ddlLeadSource.SelectedItem.Text == "Select Lead Source")
        {
            strRequired += "Missing required field: Lead Source.<br/>";
            ddlLeadSource.Focus();
        }

        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            bflag = false;
        }

        return bflag;
    }

    private int GetCountCustomer()
    {
        int nCount = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from c in _db.customers
                      where c.email == txtEmail.Text.Trim()
                      select c.customer_id);
        int n = result.Count();
        if (result != null && n > 0)
            nCount = result.Count();
        return nCount;
    }



    private void Reset()
    {
        txtMobile.Text = "";
        txtFirstName1.Text = "";
        txtLastName1.Text = "";
        txtFirstName2.Text = "";
        txtLastName2.Text = "";
        txtAddress.Text = "";
        txtCompany.Text = "";
        txtCrossStreet.Text = "";
        txtCity.Text = "";
        BindStates();
        txtZipCode.Text = "";
        txtPhone.Text = "";
        txtFax.Text = "";
        txtEmail.Text = "";
        txtEmail2.Text = "";
        //ddlStatus.Enabled = true;
        //ddlStatus.SelectedValue = "1";
        ddlLeadStatus.Enabled = true;
        //ddlLeadStatus.SelectedValue = "1";
        ddlSalesPerson.SelectedValue = "0";
        txtNotes.Text = "";
        hdnCustomerId.Value = "0";
        lblRegDate.Visible = false;
        lblRegDateData.Visible = false;
        hypMap.Visible = false;


        BindLeadSource();
    }
   



    protected void GetPDF(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        int nEstId = Convert.ToInt32(lnk.CommandArgument);

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";


        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        int IsQty = 1;
        int IsSubtotal = 2;


        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                }
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();
            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = " " + DepositValue + ":           $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }

        }


        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == nEstId && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == nEstId && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }
        string strCoverLetter = "";
        company_cover_letter objComcl = new company_cover_letter();
        if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value));

            strCoverLetter = objComcl.cover_letter;
        }
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(hdnClientId.Value);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);
        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_CoverLettter", strCoverLetter);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }
    protected void GetHTML(object sender, EventArgs e)
    {

        LinkButton lnk = (LinkButton)sender;
        int nEstId = Convert.ToInt32(lnk.CommandArgument);

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";

        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        int IsQty = 1;
        int IsSubtotal = 2;


        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                }
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();

            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = " " + DepositValue + ":           $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }

        }


        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == nEstId && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == nEstId && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }

        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptShortContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(hdnClientId.Value));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"]=="true")
        {
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "D:/Faztimate/testiicrm/logouploads/");
            }
            else
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "D:/Faztimate/iicrm/logouploads/");
            }
        }
        else
        {
            ContactAddress = ContactAddress.Replace("/IICEM", "http://localhost:7854/IICEM");
        }
        //Cover Page Shohid
        try
        {
            string sImagePath = Server.MapPath("Reports\\Common\\pdf_report") + @"\" + DateTime.Now.Ticks.ToString() + ".png";
            csCommonUtility.CreateContactAddressImage(ContactAddress, sImagePath);

            rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sImagePath + "'";
        }
        catch (Exception ex)
        {
            throw ex;
        }

        //ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        //subReport.SetDataSource(des_List);
        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }



    private void GetCustomerFileInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var file = from file_info in _db.file_upload_infos
                       where file_info.CustomerId == nCustId && file_info.client_id == Convert.ToInt32(hdnClientId.Value) && file_info.type != 1 && file_info.type != 5
                       orderby file_info.upload_fileId ascending
                       select file_info;
            grdLeadsFile.DataSource = file;
            grdLeadsFile.DataKeyNames = new string[] { "upload_fileId", "is_design", "ImageName", "dms_fileid", "dms_dirid" };
            grdLeadsFile.DataBind();
        }

    }
    protected void grdLeadsFile_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int upload_fileId = Convert.ToInt32(grdLeadsFile.DataKeys[e.Row.RowIndex].Values[0].ToString());
            bool is_design = Convert.ToBoolean(grdLeadsFile.DataKeys[e.Row.RowIndex].Values[1].ToString());
            string strFile = grdLeadsFile.DataKeys[e.Row.RowIndex].Values[2].ToString();
            HyperLink hypView = (HyperLink)e.Row.FindControl("hypView");
            string sFileName = strFile;
            string sFilePath = Path.GetExtension(strFile);
            if (strFile.IndexOf('_') != -1)
            {
                sFileName = strFile.Substring(0, strFile.IndexOf('_')).Trim() + sFilePath;
            }

            hypView.Text = "View " + sFileName;

            if (sFilePath.ToString().ToLower().Contains("jpg") || sFilePath.ToString().ToLower().Contains("png") || sFilePath.ToString().ToLower().Contains("jpeg"))
            {
                hypView.Attributes.Add("onclick", "DisplayWindow1();");
            }
            else
            {
                hypView.NavigateUrl = "Document/" + hdnCustomerId.Value + "/" + strFile;
                hypView.Target = "_blank";
            }
            e.Row.Cells[1].Text = sFileName;
            if (is_design)
            {
                e.Row.Attributes.CssStyle.Add("color", "maroon");
            }
        }
    }
    protected void grdLeadsFile_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int upload_fileId = Convert.ToInt32(grdLeadsFile.DataKeys[e.RowIndex].Values[0].ToString());
        string ImageName = grdLeadsFile.DataKeys[e.RowIndex].Values[2].ToString();
        int dms_fileid = Convert.ToInt32(grdLeadsFile.DataKeys[e.RowIndex].Values[3].ToString());
        int dms_dirid = Convert.ToInt32(grdLeadsFile.DataKeys[e.RowIndex].Values[4].ToString());

        string strQ = "Delete file_upload_info WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ, string.Empty);

        string strQ2 = "Delete FilesTable WHERE FileId=" + Convert.ToInt32(dms_fileid) + " AND CustomerId =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ2, string.Empty);

        string slideImgPath = ConfigurationManager.AppSettings["Document_path"].ToString() + "\\" + Convert.ToInt32(hdnCustomerId.Value);
        string thumbnailImage = slideImgPath + "\\Thumbnail";

        if (File.Exists(slideImgPath + "\\" + ImageName))
        {
            File.Delete(slideImgPath + "\\" + ImageName);
        }

        if (File.Exists(thumbnailImage + "\\" + ImageName))
        {
            File.Delete(thumbnailImage + "\\" + ImageName);
        }

        string DMSImgPath = ConfigurationManager.AppSettings["DocumentManager_Path"].ToString() + "\\" + Convert.ToInt32(hdnCustomerId.Value) + "\\IMAGES";

        if (File.Exists(DMSImgPath + "\\" + ImageName))
        {
            File.Delete(DMSImgPath + "\\" + ImageName);
        }

        lblMessage.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));

    }
    protected void grdLeadsFile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        TextBox txtDescription = (TextBox)grdLeadsFile.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdLeadsFile.Rows[e.NewEditIndex].FindControl("lblDescription");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        LinkButton btn = (LinkButton)grdLeadsFile.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdLeadsFile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int upload_fileId = Convert.ToInt32(grdLeadsFile.DataKeys[e.RowIndex].Values[0].ToString());
        TextBox txtDescription = (TextBox)grdLeadsFile.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdLeadsFile.Rows[e.RowIndex].FindControl("lblDescription");
        string StrQ = "UPDATE file_upload_info SET Desccription='" + txtDescription.Text.Replace("'", "''") + "' WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(StrQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
    }

    
    private void BindTempGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpSTable = LoadtmpTable();
        DataRow dr = tmpSTable.NewRow();

        string DestinationPath = Server.MapPath("~/Document//" + hdnCustomerId.Value + "//Test");
        string[] fileEntries = Directory.GetFiles(DestinationPath);

        foreach (string file in fileEntries)
        {
            string FileName = Path.GetFileName(file);

            DataRow drNew = tmpSTable.NewRow();
            drNew["file_name"] = FileName;
            drNew["file_description"] = "";
            tmpSTable.Rows.Add(drNew);
        }
        grdTemp.DataSource = tmpSTable;
        grdTemp.DataKeyNames = new string[] { "file_name" };
        grdTemp.DataBind();

    }
    private DataTable LoadtmpTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("file_name", typeof(string));
        table.Columns.Add("file_description", typeof(string));

        return table;
    }
    protected void btnSaveFiles_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveFiles.ID, btnSaveFiles.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow di in grdTemp.Rows)
        {
            HyperLink hyp = (HyperLink)di.FindControl("hyp");
            TextBox txtDes = (TextBox)di.FindControl("txtDes");
            string sfileName = grdTemp.DataKeys[di.RowIndex].Value.ToString();
            sfileName = sfileName.Replace("amp;", "").Trim();
            file_upload_info fui = new file_upload_info();
            if (_db.file_upload_infos.Where(l => l.client_id == Convert.ToInt32(hdnClientId.Value) && l.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && l.ImageName == sfileName.ToString()).SingleOrDefault() == null)
            {
                fui.client_id = Convert.ToInt32(hdnClientId.Value);
                fui.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
                fui.Desccription = txtDes.Text;
                fui.ImageName = sfileName;
                fui.is_design = false;
                fui.estimate_id = 0;
                fui.type = 0;
                fui.vendor_cost_id = 0;
                fui.IsSiteProgress = false;
                fui.dms_dirid = 0;
                fui.dms_fileid = 0;
                _db.file_upload_infos.InsertOnSubmit(fui);
                _db.SubmitChanges();
            }

        }

        string strpath = Request.PhysicalApplicationPath + "Document\\";
        strpath = strpath + "\\" + hdnCustomerId.Value.ToString() + "\\Test";
        string NewDir = Server.MapPath("~/Document//" + hdnCustomerId.Value);
        if (!System.IO.Directory.Exists(NewDir))
        {
            System.IO.Directory.CreateDirectory(NewDir);
        }
        string[] fileEntries = Directory.GetFiles(strpath);
        foreach (string file in fileEntries)
        {
            string FileName = Path.GetFileName(file);
            string Ext = Path.GetExtension(file).ToLower();
            if (Ext == ".jpg" || Ext == ".png" || Ext == ".jpeg")
            {
                ImageUtility.SaveSlideImage(file, Server.MapPath("~/Document//" + hdnCustomerId.Value));
                ImageUtility.SaveThumbnailImage(file, Server.MapPath("~/Document//" + hdnCustomerId.Value) + "\\Thumbnail");

                if (!System.IO.Directory.Exists(NewDir+ "\\Original"))
                {
                    System.IO.Directory.CreateDirectory(NewDir+ "\\Original");
                }
                File.Move(file, Path.Combine(NewDir + "\\Original", FileName));
            }
            else
                File.Move(file, Path.Combine(NewDir, FileName));
        }

        lblMessage.Text = csCommonUtility.GetSystemMessage("Files saved successfully");

        BindTempGrid();
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
        if (grdTemp.Rows.Count > 0)
            btnSaveFiles.Visible = true;
        else
            btnSaveFiles.Visible = false;

    }
    protected void grdTemp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string file = grdTemp.DataKeys[e.Row.RowIndex].Value.ToString();
            file = file.Replace("amp;", "").Trim();
            HyperLink hypView = (HyperLink)e.Row.FindControl("hyp");
            string strFileName = file;
            if (file.IndexOf('_') != -1)
            {
                strFileName = file.Substring(0, file.IndexOf('_')).Trim() + Path.GetExtension(file);
            }
            hypView.Text = strFileName;
            hypView.NavigateUrl = "Document/" + hdnCustomerId.Value + "/" + file;
            hypView.Target = "_blank";

        }
    }

    protected void btnSalesCalendar_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSalesCalendar.ID, btnSalesCalendar.GetType().Name, "Click"); 
        //DataClassesDataContext _db = new DataClassesDataContext();
        //if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2).Count() == 0)
        //{
        //    lblResult1.Text = "Appointment must be saved before the Calendar can be used";
        //    lblResult1.ForeColor = System.Drawing.Color.Red;            
        //}
        //else
        //{
        //Response.Redirect("schedulecalendar.aspx?cid=" + hdnCustomerId.Value + "&TypeID=2");
        // }

        if (IsCustomerDataValid())
        {
            SaveCustomerData();

            Response.Redirect("schedulecalendar.aspx?cid=" + hdnCustomerId.Value + "&TypeID=2");
        }
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

    //private GoogleAuthenticator GetAuthenticator()
    //{
    //    var authenticator = (GoogleAuthenticator)Session["authenticator"];

    //    if (authenticator == null || !authenticator.IsValid)
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        // Get a new Authenticator using the Refresh Token
    //        string strUserName = "tislam";
    //        var refreshToken = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserName == strUserName).RefreshToken;
    //        authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
    //        Session["authenticator"] = authenticator;
    //    }

    //    return authenticator;
    //}


    protected void btnSaveCall_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveCall.ID, btnSaveCall.GetType().Name, "Click"); 
        lblMessage.Text = "";
        lblResult.Text = "";
        lblResultCallLog.Text = "";
        lblResultContact.Text = "";

        if (IsCallDataValid())
        {
            SaveCallData();
            GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));
            ResetCallLog();
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

            GetCallLogDeatils(nCustomerId, nCallId);
        }

    }
    private void GetCallLogDeatils(int nCustId, int nCallId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (nCallId > 0)
        {
            hdnCallLogId.Value = nCallId.ToString();
            ScheduleCalendar objsc = new ScheduleCalendar();
            CustomerCallLog custCall = new CustomerCallLog();
            custCall = _db.CustomerCallLogs.Single(c => c.CallLogID == nCallId && c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

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
                txtAppointmentDateC.Text = Convert.ToDateTime(custCall.AppointmentDateTime).ToShortDateString();
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
                ListItem item = cmbStartTimec.Items.FindByText(startTime);
                if (item != null)
                    cmbStartTimec.SelectedValue = startTime;
                else
                {
                    cmbStartTimec.Items.Insert(0, new ListItem(startTime));
                    cmbStartTimec.SelectedValue = startTime;
                }

                //Appointment End Time
                ListItem eitem = cmbEndTimec.Items.FindByText(endTime);
                if (eitem != null)
                    cmbEndTimec.SelectedValue = endTime;
                else
                {
                    cmbEndTimec.Items.Insert(0, new ListItem(endTime));
                    cmbEndTimec.SelectedValue = endTime;
                }
            }
        }

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
    protected void lnkAddNewCall_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkAddNewCall.ID, lnkAddNewCall.GetType().Name, "Click"); 
        lblResultCallLog.Text = string.Empty;
        ResetCallLog();
        //hdnCallLogId.Value = "0";
        //Response.Redirect("CallLogInfo.aspx?cid="+hdnCustomerId.Value);
    }

    private void ResetCallLog()
    {
        // lblResultCallLog.Text = string.Empty;
        txtCallSubject.Text = "";
        hdnCallLogId.Value = "0";
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
        txtAppointmentDateC.Text = "";
        tblApptDate.Visible = false;
        tblApptTime.Visible = false;
        tblFollowUp.Visible = false;

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

    protected void btnSaveContact_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveContact.ID, btnSaveContact.GetType().Name, "Click"); 
        try
        {
            string strRequired = "";
            if (txtContactFirstName.Text == "")
                strRequired = "Missing required field: First Name.<br/>";
            if (txtContactLastName.Text == "")
                strRequired += "Missing required field: Last Name.<br/>";


            if (strRequired.Length > 0)
            {
                lblResultCallLog.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                return;
            }
            DataClassesDataContext _db = new DataClassesDataContext();



            CustomerContact custContact = new CustomerContact();

            custContact.ContactId = 0;
            custContact.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            custContact.FirstName = txtContactFirstName.Text.Trim();
            custContact.LastName = txtContactLastName.Text.Trim();
            custContact.Title = txtContactTitle.Text.Trim();
            custContact.Email = txtContactEmail.Text.Trim();
            custContact.Phone = txtContactPhone.Text.Trim();

            custContact.Fax = "";
            custContact.Mobile = txtContactMobile.Text.Trim();
            custContact.CreatedBy = User.Identity.Name;
            custContact.LastUpdateDate = Convert.ToDateTime(DateTime.Now);


            if (Convert.ToInt32(custContact.ContactId) == 0)
            {
                _db.CustomerContacts.InsertOnSubmit(custContact);
                lblResultContact.Text = csCommonUtility.GetSystemMessage("Contact has been saved successfully.");
                _db.SubmitChanges();
                GetCustomerContactInfo(Convert.ToInt32(hdnCustomerId.Value));

                custContact.FirstName = "";
                custContact.LastName = "";
                custContact.Title = "";
                custContact.Email = "";
                custContact.Phone = "";

                custContact.Fax = "";
                custContact.Mobile = "";
            }
        }
        catch (Exception ex)
        {
            lblResultContact.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdContact_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Label lblFirstNameG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblFirstNameG");
        TextBox txtFirstNameG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtFirstNameG");

        Label lblLastNameG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblLastNameG");
        TextBox txtLastNameG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtLastNameG");

        Label lblTitleG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblTitleG");
        TextBox txtTitleG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtTitleG");
        Label lblEmailG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblEmailG");
        TextBox txtEmailG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtEmailG");
        Label lblPhoneG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblPhoneG");
        TextBox txtPhoneG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtPhoneG");


        Label lblMobileG = (Label)grdContact.Rows[e.NewEditIndex].FindControl("lblMobileG");
        TextBox txtMobileG = (TextBox)grdContact.Rows[e.NewEditIndex].FindControl("txtMobileG");


        lblFirstNameG.Visible = false;
        txtFirstNameG.Visible = true;
        lblLastNameG.Visible = false;
        txtLastNameG.Visible = true;
        lblTitleG.Visible = false;
        txtTitleG.Visible = true;
        lblEmailG.Visible = false;
        txtEmailG.Visible = true;
        lblPhoneG.Visible = false;
        txtPhoneG.Visible = true;

        lblMobileG.Visible = false;
        txtMobileG.Visible = true;



        LinkButton btn = (LinkButton)grdContact.Rows[e.NewEditIndex].Cells[7].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdContact_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        Label lblFirstNameG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblFirstNameG");
        TextBox txtFirstNameG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtFirstNameG");
        Label lblLastNameG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblLastNameG");
        TextBox txtLastNameG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtLastNameG");
        Label lblTitleG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblTitleG");
        TextBox txtTitleG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtTitleG");
        Label lblEmailG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblEmailG");
        TextBox txtEmailG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtEmailG");
        Label lblPhoneG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblPhoneG");
        TextBox txtPhoneG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtPhoneG");

        Label lblMobileG = (Label)grdContact.Rows[e.RowIndex].FindControl("lblMobileG");
        TextBox txtMobileG = (TextBox)grdContact.Rows[e.RowIndex].FindControl("txtMobileG");

        int nContactId = Convert.ToInt32(grdContact.DataKeys[Convert.ToInt32(e.RowIndex)].Values[1]);


        CustomerContact custContact = new CustomerContact();
        if (nContactId > 0)
        {
            custContact = _db.CustomerContacts.Single(c => c.ContactId == nContactId);


            custContact.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            custContact.FirstName = txtFirstNameG.Text.Trim();
            custContact.LastName = txtLastNameG.Text.Trim();
            custContact.Title = txtTitleG.Text.Trim();
            custContact.Email = txtEmailG.Text.Trim();
            custContact.Phone = txtPhoneG.Text.Trim();
            custContact.Mobile = txtMobileG.Text.Trim();
            custContact.CreatedBy = User.Identity.Name;
            custContact.LastUpdateDate = Convert.ToDateTime(DateTime.Now);

            _db.SubmitChanges();

            GetCustomerContactInfo(Convert.ToInt32(hdnCustomerId.Value));
            lblMessageGrdContact.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        }


    }
    private void GetCustomerContactInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var Call = from cc in _db.CustomerContacts
                       where cc.customer_id == nCustId
                       orderby cc.ContactId descending
                       select cc;
            grdContact.DataSource = Call;
            grdContact.DataKeyNames = new string[] { "customer_id", "ContactId" };
            grdContact.DataBind();
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
    protected void cmbStartTimec_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, cmbStartTimec.ID, cmbStartTimec.GetType().Name, "SelectedIndexChanged"); 
        DateTime dateTime = DateTime.ParseExact(cmbStartTimec.SelectedValue.ToString(), "h:mm tt", CultureInfo.InvariantCulture).AddHours(1);
        string endTime = dateTime.ToShortTimeString();
        //Appointment End Time
        ListItem eitem = cmbEndTimec.Items.FindByText(endTime);
        if (eitem != null)
            cmbEndTimec.SelectedValue = endTime;
        else
        {
            cmbEndTimec.Items.Insert(0, new ListItem(endTime));
            cmbEndTimec.SelectedValue = endTime;
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
  
       private void GetCustomerMessageInfo(int nCustId)
    {
        if (nCustId > 0)
        {

            try
            {
              //  ExchangeService service = EWSAPI.GetEWSService("alyons@interiorinnovations.biz", "");

                // ExchangeService service = EWSAPI.GetEWSServiceByCustomer(Convert.ToInt32(hdnCustomerId.Value));
                //  DSMessage dsMessageSent = EWSAPI.GetEmailList(lblEmail.Text, nCustId, service);

                DSMessage dsMessageSent = new DSMessage();

                DataClassesDataContext _db = new DataClassesDataContext();
                var messList = (from mess_info in _db.customer_messages
                                where mess_info.customer_id == nCustId && mess_info.client_id == Convert.ToInt32(hdnClientId.Value)
                                orderby mess_info.cust_message_id descending
                                select mess_info).ToList();

                foreach (customer_message msg in messList)
                {
                    DSMessage.MessageRow mes = dsMessageSent.Message.NewMessageRow();

                    //string strQ = "select * from message_upolad_info where customer_id=" + nCustId + " and message_id=" + msg.message_id + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    //IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                    //string mess_file = "";
                    //foreach (message_upolad_info message_upolad in list)
                    //{
                    //    mess_file += message_upolad.mess_file_name.Replace("amp;", "").Trim() + ", "; ;
                    //}
                    //mess_file = mess_file.Trim().TrimEnd(',');

                    //if (mess_file.Length > 0)
                    //{
                    //    mes.HasAttachments = true;
                    //    mes.AttachmentList = mess_file.Trim().TrimEnd(',');
                    //}
                    //else
                    //{
                    //    mes.AttachmentList = "";
                    //    mes.HasAttachments = false;// msg.HasAttachments;
                    //}

                    if (msg.HasAttachments == null)
                    {
                        string strQ = "select * from message_upolad_info where customer_id=" + nCustId + " and message_id=" + msg.message_id + " and client_id=" + Convert.ToInt32(hdnClientId.Value);
                        IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                        string mess_file = "";
                        foreach (message_upolad_info message_upolad in list)
                        {
                            mess_file += message_upolad.mess_file_name.Replace("amp;", "").Trim() + ", "; ;
                        }
                        mess_file = mess_file.Trim().TrimEnd(',');

                        if (mess_file.Length > 0)
                        {
                            mes.HasAttachments = true;
                            mes.AttachmentList = mess_file.Trim().TrimEnd(',');


                        }
                        else
                        {
                            mes.AttachmentList = "";
                            mes.HasAttachments = false;// msg.HasAttachments;
                        }

                        msg.HasAttachments = mes.HasAttachments;
                        msg.AttachmentList = mes.AttachmentList;

                    }
                    else if (Convert.ToBoolean(msg.HasAttachments))
                    {

                        mes.HasAttachments = true;
                        mes.AttachmentList = msg.AttachmentList;


                    }
                    else
                    {
                        mes.HasAttachments = false;
                        mes.AttachmentList = "";
                    }

                    mes.From = msg.mess_from;
                    mes.To = msg.mess_to;
                    mes.IsRead = (bool)(msg.IsView ?? false);
                    mes.customer_id = nCustId.ToString();
                    mes.message_id = msg.message_id.ToString();
                    mes.create_date = (DateTime)msg.create_date;
                    mes.create_date = (DateTime)msg.create_date;
                    if (msg.mess_subject != null)
                        mes.mess_subject = msg.mess_subject.ToString();
                    else
                        mes.mess_subject = "";
                    mes.last_view = (DateTime)msg.last_view;
                    mes.Protocol = msg.Protocol;
                    mes.Type = msg.Type;
                    mes.sent_by = msg.sent_by;
                    dsMessageSent.Message.AddMessageRow(mes);

                }

                _db.SubmitChanges();

                dsMessageSent.AcceptChanges();

                DataView dv = dsMessageSent.Tables[0].DefaultView;
                dv.Sort = "create_date DESC";
                grdCustomersMessage.DataSource = dv;
                grdCustomersMessage.DataKeyNames = new string[] { "customer_id", "message_id", "AttachmentList", "From", "To", "Type" };
                grdCustomersMessage.DataBind();




            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

            }

        }

    }


    protected void grdCustomersMessage_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                string script = "";

                string Attacheent = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[2].ToString();
                string MessId = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[1].ToString();
                int CustId = Convert.ToInt32(grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[0].ToString());

                string From = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[3].ToString();
                string To = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[4].ToString();
                string Type = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[5].ToString();




                if (e.Row.Cells[7].Text.Equals("True"))
                    e.Row.Cells[7].Text = "Yes";
                else
                    e.Row.Cells[7].Text = "";


                HyperLink hypMessageDetails = (HyperLink)e.Row.FindControl("hypMessageDetails");
                hypMessageDetails.ToolTip = "Click on Message Details to view specific Message Details .";
                hypMessageDetails.Target = "MyWindow";

                //if (e.Row.Cells[8].Text.Equals("Outlook"))
                //{
                //    script = String.Format("GetdatakeyValue1('{0}','{1}','{2}','{3}')", MessId.ToString(), From, To, Type);
                //}
                //else
                //{
                //    script = String.Format("GetdatakeyValue1Old('{0}')", MessId.ToString());
                //}

                script = String.Format("GetdatakeyValue1Old('{0}')", MessId.ToString());

                hypMessageDetails.Attributes.Add("onclick", script);
                if (Attacheent.Length > 0)
                {
                    HyperLink hypAttachment = (HyperLink)e.Row.FindControl("hypAttachment");
                    hypAttachment.Text = Attacheent;
                    hypAttachment.ToolTip = "Click on Message Details to view specific Message Details .";
                    hypAttachment.Target = "MyWindow";

                    hypAttachment.Attributes.Add("onclick", script);
                }





            }
            catch { }

        }
    }


    private bool IsCallDataValid()
    {

        bool bflag = true;
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
            bflag = false;
        }

        return bflag;
    }

    public void SaveCallData()
    {

        try
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
        
            if (txtDurationH.Text == "" && txtDurationH.Text == "0")
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
            custCall.sales_person_id = Convert.ToInt32(ddlSalesPerson.SelectedValue);

            DateTime dt = Convert.ToDateTime("1900-01-01");
            if (txtAppointmentDateC.Text.Trim() != "")
            {
                try
                {
                    txtAppointmentDateC.Text = GetDayOfWeek(txtAppointmentDateC.Text);
                    dt = Convert.ToDateTime(txtAppointmentDateC.Text);
                    custCall.AppointmentDateTime = DateTime.Parse(dt.ToShortDateString() + " " + cmbStartTimec.SelectedValue.ToString());
                    // cust.appointment_date = DateTime.Parse(dt.ToShortDateString() + " " + cmbStartTime.SelectedValue.ToString());
                }
                catch
                {
                    txtAppointmentDateC.Text = GetDayOfWeek(txtAppointmentDateC.Text);
                    dt = Convert.ToDateTime(txtAppointmentDateC.Text);
                    custCall.AppointmentDateTime = dt;
                }
            }
            else
            {
                custCall.AppointmentDateTime = dt;
                // cust.appointment_date = dt;
            }

            #region Save Appointment

            userinfo objUName = (userinfo)Session["oUser"];
            string strUName = objUName.first_name;
            string strClassName = "fc-sales";
            //Google Calendar
            sales_person objSP = new sales_person();
            string calendarId = string.Empty;
            //calendarId = ConfigurationManager.AppSettings["GoogleSalesCalendarID"];

            //Get Sales Person ID
            int nSalesPersonID = Convert.ToInt32(ddlSalesPerson.SelectedValue);

            if (txtAppointmentDateC.Text != "")
            {

                DateTime ndt = Convert.ToDateTime(GetDayOfWeek(txtAppointmentDateC.Text));
                var GoogleEventID = "";

                //Get calendarId by Sales Person ID
                if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
                {
                    objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                    calendarId = objSP.google_calendar_id ?? "";
                }

                if (calendarId != "")
                {
                    // Google Calendar DELETE------------
                    if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                    {
                        List<ScheduleCalendar> sclist = _db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == Convert.ToInt32(hdnCallLogId.Value)).ToList();
                        foreach (ScheduleCalendar sc in sclist)
                        {
                            if (sc.google_event_id != "")
                            {
                                var authenticator = GetAuthenticator(nSalesPersonID); //  Sales Persion ID
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
                            Title = (txtLastName1.Text.Trim() + " " + txtPhone.Text.Trim() + " (" + ddlSalesPerson.SelectedItem.ToString() + ")").Trim(),
                            Location = txtAddress.Text.Trim() + " " + txtCity.Text.Trim() + ", " + ddlState.SelectedItem.Text.Trim() + " " + txtZipCode.Text.Trim() + ", USA",
                            StartDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTimec.SelectedValue.ToString()),
                            EndDate = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTimec.SelectedValue.ToString()),
                            Description = txtNotes.Text.Trim(),
                            ColorId = 1
                        };

                        var authenticator = GetAuthenticator(nSalesPersonID); //  Sales Persion ID
                        var service = new GoogleCalendarServiceProxy(authenticator);
                        GoogleEventID = service.CreateEvent(calendarEvent);
                    }
                    ////Google Calendar Insert End Code----------------------------------------------------------
                }

                // Calendar DELETE------------
                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == Convert.ToInt32(hdnCallLogId.Value)).Count() > 0)
                {

                    string sql2 = "DELETE ScheduleCalendar WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND type_id = 2 AND estimate_id =" + Convert.ToInt32(hdnCallLogId.Value);
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
                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && sc.type_id == 2 && sc.estimate_id == Convert.ToInt32(hdnCallLogId.Value)).Count() == 0)
                {
                    objsc.event_id = nEventId;
                    objsc.title = txtCallSubject.Text.Trim();
                    objsc.description = txtCallDescription.Text.Trim();
                    objsc.event_start = DateTime.Parse(ndt.ToShortDateString() + " " + cmbStartTimec.SelectedValue.ToString());
                    objsc.event_end = DateTime.Parse(ndt.ToShortDateString() + " " + cmbEndTimec.SelectedValue.ToString());
                    objsc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objsc.estimate_id = Convert.ToInt32(hdnCallLogId.Value);
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
                    objsc.IsEstimateActive = true;
                    objsc.duration = 1;
                    _db.ScheduleCalendars.InsertOnSubmit(objsc);
                    _db.SubmitChanges();

                }

                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                cust.appointment_date = objsc.event_start;
                _db.SubmitChanges();
                GetAppointmentDate(cust);


            }
            #endregion

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
        }
        catch (Exception ex)
        {
            lblResultCallLog.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace);
        }
    }

    protected void btnSalesCalendarC_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSalesCalendarC.ID, btnSalesCalendarC.GetType().Name, "Click"); 
        try
        {
            if (IsCallDataValid())
            {
                SaveCallData();
            }
            Response.Redirect("schedulecalendar.aspx?cid=" + hdnCustomerId.Value + "&TypeID=2&CallLogID=" + Convert.ToInt32(hdnCallLogId.Value));
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public void ResetCustomerContact()
    {
        txtContactFirstName.Text = "";
        txtContactLastName.Text = "";
        txtContactTitle.Text = "";
        txtContactEmail.Text = "";
        txtContactPhone.Text = "";
        txtContactMobile.Text = "";
    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
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

    protected void btnUpload_Click(object sender, EventArgs e)
    {


        Response.Redirect("DocumentManagement.aspx?cid=" + hdnCustomerId.Value);

    }


    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSalesPerson();
    }
}