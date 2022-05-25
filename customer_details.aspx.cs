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

public partial class customer_details : System.Web.UI.Page
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
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
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
        if (Page.User.IsInRole("cus002") == false)
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

            DataClassesDataContext _db = new DataClassesDataContext();


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

            int nCallID = 0;
            if (Request.QueryString.Get("callid") != null)
            {
                if (Convert.ToInt32(Request.QueryString.Get("callid")) != 0)
                {
                    nCallID = Convert.ToInt32(Request.QueryString.Get("callid"));
                    hdnCallLogId.Value = Request.QueryString.Get("callid").ToString();
                }
                else
                {
                    if (_db.CustomerCallLogs.Any(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3))
                    {
                        hdnCallLogId.Value = _db.CustomerCallLogs.Where(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.CallTypeId == 3).Max(c => c.CallLogID).ToString();
                        nCallID = Convert.ToInt32(hdnCallLogId.Value);
                    }
                }

                CollapsiblePanelExtender3.Collapsed = false;
                CollapsiblePanelExtender3.ClientState = "false";
                GetCallLogDeatils(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnCallLogId.Value));
                btnSaveCall.Focus();
            }

            //  imgA.Attributes.Add("onClick", "DisplayWindow1();");

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

            if (hdnEmailType.Value == "1")
            {
                HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
                HyperLink2.Attributes.Add("onClick", "DisplayWindow();");
                //  HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
                // HyperLink2.Attributes.Add("onClick", "DisplayWindow();");
            }
            else
            {
                HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
                HyperLink2.Attributes.Add("onClick", "DisplayWindow();");
            }

            BindStates();
            BindSalesPerson();
            BindLeadSource();

            //int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            //hdnCustomerId.Value = ncid.ToString();
            custId = hdnCustomerId.Value;

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                ScheduleCalendar objsc = new ScheduleCalendar();
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                custEmail = cust.email ?? "";
                hdnCustEmail.Value = custEmail;
                if (Session["call"] != null)
                    Response.Redirect("CallLogInfo.aspx?cid=" + Convert.ToInt32(hdnCustomerId.Value) + "&callid=" + nCallID, false);

                else if (Convert.ToBoolean(cust.islead))
                    Response.Redirect("lead_details.aspx?cid=" + Convert.ToInt32(hdnCustomerId.Value) + "&callid=" + nCallID, false);



                lblHeaderTitle.Text = "Customer Details";


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

                hdnClientId.Value = cust.client_id.ToString();

                txtCompany.Text = cust.company;
                txtCrossStreet.Text = cust.cross_street;
                txtCity.Text = cust.city;
                ddlState.SelectedValue = cust.state;
                txtZipCode.Text = cust.zip_code;
                txtPhone.Text = cust.phone;
                txtFax.Text = cust.fax;
                txtMobile.Text = cust.mobile;
                txtEmail.Text = cust.email;
                txtEmail2.Text = cust.email2;
                txtWebsite.Text = cust.website;
                chkOptIn.Checked = (bool)cust.IsEnableSMS;
                lblCreatedBy.Text = "Created by: <font style='font-weight:bold'>" + cust.CreatedBy + "</font>";
                ListItem item = ddlStatus.Items.FindByValue(cust.status_id.ToString());
                if (item != null)
                    ddlStatus.Items.FindByValue(cust.status_id.ToString()).Selected = true;
                // ddlStatus.SelectedValue = cust.status_id.ToString();

                ListItem itemsp = ddlSalesPerson.Items.FindByValue(cust.sales_person_id.ToString());
                if (itemsp != null)
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

                GetAppointmentDate(cust);

                ddlLeadSource.SelectedValue = cust.lead_source_id.ToString();
                txtStatusNote.Text = cust.status_note;

                Session.Add("CustomerId", hdnCustomerId.Value);

                GetCustomerSoldEstimate(Convert.ToInt32(hdnCustomerId.Value));
                GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

                if (_db.customeruserinfos.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                {
                    customeruserinfo objCU = new customeruserinfo();
                    objCU = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));
                    txtUserName.Text = objCU.customerusername;
                    if (Convert.ToInt32(objCU.isactive) == 1)
                    {
                        rdoCustomerLogin.SelectedValue = "1";
                        pnlCustomerPass.Visible = true;
                        lblChangePassword.Text = "Please input password and re-type password to reset password.";

                    }
                    else
                    {
                        rdoCustomerLogin.SelectedValue = "0";
                        pnlCustomerPass.Visible = false;
                        lblChangePassword.Text = "Please input password and re-type password to reset password.";

                    }

                }
                else
                {
                    lblChangePassword.Text = "Please input password & re-type password to activate customer login.";

                }

                txtCallStartDate.Text = DateTime.Today.ToShortDateString();
                ddlCallHour.SelectedItem.Text = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
                ddlCallMinutes.SelectedItem.Text = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
                ddlCallAMPM.SelectedValue = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

                GetCallLogInfo(Convert.ToInt32(hdnCustomerId.Value));
                GetCustomerContactInfo(Convert.ToInt32(hdnCustomerId.Value));
            }
            else
            {

                lblHeaderTitle.Text = "Add New Customer";
                hypMap.Visible = false;

                hdnCustomerId.Value = "0";
                lblRegDate.Visible = false;
                lblRegDateData.Visible = false;
                lblChangePassword.Text = "Please input password & re-type password to activate customer login.";

                Session.Remove("CustomerId");

                pnlEstimate.Visible = false;
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "btnSalesCalendar", "hypMap", "rdoCustomerLogin","btnImageGallery", "btnSaveContact", "HyperLink1", "btnUpload", "HyperLink2", "btnSaveCall", "chkFollowup", "pnlLeadListPopup" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "hypMessageDetails", "Edit", "Update", "Delete" });

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
            //Array.Sort(tempEndTimeListText);
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
    private void BindSalesPerson()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1  and is_sales=1 and sales_person.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by sales_person_id asc";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSalesPerson.DataSource = mList;
        ddlSalesPerson.DataTextField = "sales_person_name";
        ddlSalesPerson.DataValueField = "sales_person_id";
        ddlSalesPerson.DataBind();
    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;
        ddlLeadSource.DataSource = item;
        ddlLeadSource.DataTextField = "lead_name";
        ddlLeadSource.DataValueField = "lead_source_id";

        ddlLeadSource.DataBind();
        ddlLeadSource.Items.Insert(0, "Select Lead Source");
        ddlLeadSource.SelectedIndex = 0;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    public void SaveCustomerData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        customer cust = new customer();
        int nCount = GetCountCustomer();
        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

        txtPhone.Text = csCommonUtility.GetPhoneFormat(txtPhone.Text.Trim());
        txtFax.Text = csCommonUtility.GetPhoneFormat(txtFax.Text.Trim());
        txtMobile.Text = csCommonUtility.GetPhoneFormat(txtMobile.Text.Trim());

        cust.address = txtAddress.Text;
        cust.city = txtCity.Text;
        cust.client_id = Convert.ToInt32(hdnClientId.Value);
        cust.company = txtCompany.Text;
        cust.cross_street = txtCrossStreet.Text;
        cust.email = txtEmail.Text;
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
        cust.status_id = Convert.ToInt32(ddlStatus.SelectedValue);
        cust.website = txtWebsite.Text.Trim();
        cust.IsEnableSMS = chkOptIn.Checked;
        //cust.appointment_date = Convert.ToDateTime(txtAppointmentDate.Text);
        DateTime dt = Convert.ToDateTime("1900-01-01");

        cust.notes = txtNotes.Text;
        cust.lead_source_id = Convert.ToInt32(ddlLeadSource.SelectedValue);
        int nStatusLength = Convert.ToInt32(txtStatusNote.Text.Trim().Length);

        cust.status_note = txtStatusNote.Text;


        if (Convert.ToInt32(hdnCustomerId.Value) == 0)
        {
            cust.CustomerCalendarWeeklyView = 1;
            cust.isCalendarOnline = true;
            cust.isJobSatusViewable = true;
            cust.appointment_date = dt;
            cust.email = txtEmail.Text;
            cust.registration_date = Convert.ToDateTime(DateTime.Now);
            cust.SuperintendentId = 0;
            _db.customers.InsertOnSubmit(cust);
            lblResult.Text = csCommonUtility.GetSystemMessage("Customer '" + txtLastName1.Text + "' has been saved successfully.");

            _db.SubmitChanges();
            hdnCustomerId.Value = cust.customer_id.ToString();

            // New Customer User Insert
            if (rdoCustomerLogin.SelectedValue == "1")
            {
                if (!SaveCustomerUser())
                {
                    return;
                }
            }

            Session.Add("CustomerId", cust.customer_id);

            pnlEstimate.Visible = false;
        }
        else
        {

            if (_db.customeruserinfos.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
            {
                customeruserinfo objcu = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));
                if (txtEmail.Text.Trim() != cust.email)
                {
                    txtUserName.Text = txtEmail.Text;
                    objcu.customerusername = txtEmail.Text;

                }

                if (rdoCustomerLogin.SelectedValue == "1")
                {
                    if (txtUserName.Text.Length > 1)
                        objcu.customerusername = txtUserName.Text;
                    objcu.isactive = 1;
                    if (txtPassword.Text.Trim() != "")
                    {
                        if (Convert.ToInt32(txtPassword.Text.Length) < 6)
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Password length should be minimum 6");
                            return;
                        }
                        if (txtConfirmPassword.Text.Trim() == "")
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: Confirm Password.");
                            return;
                        }
                        if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please confirm password.");
                            return;
                        }
                        objcu.customerpassword = txtPassword.Text;
                    }
                }
                else
                {
                    objcu.isactive = 0;
                }
                _db.SubmitChanges();
            }
            else
            {
                if (rdoCustomerLogin.SelectedValue == "1")
                {
                    if (!SaveCustomerUser())
                    {
                        return;
                    }
                }
            }




            cust.update_date = DateTime.Now;
            lblResult.Text = csCommonUtility.GetSystemMessage("Customer '" + txtLastName1.Text + "' has been updated successfully.");
            Session.Add("CustomerId", hdnCustomerId.Value);
            _db.SubmitChanges();
            updateCustomersLatLng(Convert.ToInt32(hdnCustomerId.Value));

        }
        /* userinfo objUName = (userinfo)Session["oUser"];
         string strUName = objUName.first_name;
         string strClassName = "fc-sales";

         //Google Calendar
         sales_person objSP = new sales_person();
         string calendarId = string.Empty;
         ////calendarId = ConfigurationManager.AppSettings["GoogleOperationCalendarID"];

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
                                 var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                                 var service = new GoogleCalendarServiceProxy(authenticator);
                                 service.DeleteEvent(calendarId, sc.google_event_id); // Delete
                             }
                         }
                     }

                     //Google Calendar Insert----------------------------------------------------------
                     if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                     {
                         var calendarEvent = new gCalendarEvent()
                         {
                             CalendarId = calendarId,
                             Title = (txtLastName1.Text.Trim() + " (" + ddlSalesPerson.SelectedItem.ToString() + ")").Trim(),
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
                     //Google Calendar Insert End Code----------------------------------------------------------
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
                 lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
             }
         }*/
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

                string address = Cuustomer.address.Replace(".", "").Replace("#", "");
                // string address = "2401 E Rio Salado PKWY  UNIT 1085";
                string fulladdress = address + "," + Cuustomer.city.Trim() + "," + Cuustomer.state.Trim() + " " + Cuustomer.zip_code;
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
        DataClassesDataContext _db = new DataClassesDataContext();
        bool bflag = true;

        string strRequired = string.Empty;

        if (txtFirstName1.Text.Trim() == "")
        {
            strRequired += "Missing required field: First Name 1.";


        }
        if (txtLastName1.Text.Trim() == "")
        {
            strRequired += "Missing required field: Last Name 1.";


        }
        if (txtAddress.Text.Trim() == "")
        {
            strRequired += "Missing required field: Address.";


        }
        if (txtCity.Text.Trim() == "")
        {
            strRequired += "Missing required field: City.";


        }

        if (txtZipCode.Text.Trim() == "")
        {
            strRequired += "Missing required field: Zip Code.";


        }
        if (txtEmail.Text.Trim() == "")
        {
            strRequired += "Missing required field: Email.";


        }

        int nStatusLength = Convert.ToInt32(txtStatusNote.Text.Trim().Length);
        if (nStatusLength > 50)
        {
            strRequired += "Status note should less than 50 characters.";
        }



        if (ddlLeadSource.SelectedItem.Text == "Select Lead Source")
        {
            strRequired += "Missing required field: Lead Source.";
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

    //private void SendEmailToCustomer(int nCustomerId)
    //{
    //    // Email To Group

    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    customer objCust = new customer();
    //    objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
    //    customeruserinfo objcu = new customeruserinfo();
    //    objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

    //    company_profile oCom = new company_profile();
    //    oCom = _db.company_profiles.Single(c => c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

    //    string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
    //            "<tr><td align='left'>Dear " + objCust.first_name1 + " " + objCust.last_name1 + ",</td></tr>" + Environment.NewLine +
    //            "<tr><td align='left'>We have created a user logon on your behalf.</td></tr>" + Environment.NewLine +
    //            "<tr><td align='left'>Your username: " + objcu.customerusername + " & password: " + objcu.customerpassword + "</td></tr>" + Environment.NewLine +
    //            "<tr><td align='left'>Security Question: " + objcu.securityquestion + " & Answer: " + objcu.answer + "</td></tr>" + Environment.NewLine +
    //            "<tr><td align='left'>You can manage your profile and users from your portal.</td></tr>";

    //    strTable += "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'>Customer Portal</td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'>Please click <a target='_blank' href='lighting2.faztrack.com/interiorinnovations/customerlogin.aspx'> here </a> to logon to your portal.</td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'></td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'>Should you have any question please do not hesitate to contact us.</td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'>" + oCom.company_name + "</td></tr>" + Environment.NewLine +
    //   "<tr><td align='left'>" + oCom.website + "</td></tr>" + Environment.NewLine +
    //    "<tr><td align='left'></td></tr></table>";

    //    MailMessage msg = new MailMessage();
    //    msg.From = new MailAddress(oCom.email);
    //    msg.To.Add(txtEmail.Text.Trim());
    //    msg.Bcc.Add(oCom.contract_email);
    //    msg.Subject = "Customer Registration";
    //    msg.IsBodyHtml = true;
    //    msg.Body = strTable;
    //    msg.Priority = MailPriority.High;

    //    try
    //    {
    //        SmtpClient smtp = new SmtpClient();
    //        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
    //        smtp.Send(msg);
    //    }
    //    catch(Exception ex)
    //    {
    //        lblResult.Text = ex.Message;
    //        
    //        return;
    //    }

    //    //MailMessage msgMail = new MailMessage();
    //    //msgMail.From = oCom.email;
    //    //msgMail.To = txtEmail.Text.Trim();
    //    //msgMail.Bcc = oCom.contract_email;
    //    //msgMail.Subject = "Customer Registration";
    //    //msgMail.BodyFormat = MailFormat.Html;
    //    //msgMail.Body = strTable;

    //    //try
    //    //{
    //    //    SmtpMail.SmtpServer.Insert(0, "localhost");
    //    //    SmtpMail.Send(msgMail);
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    lblResult.Text = ex.Message;
    //    //    
    //    //    return;
    //    //}
    //}

    private void Reset()
    {
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
        ddlStatus.Enabled = true;
        ddlStatus.SelectedValue = "2";
        ddlSalesPerson.SelectedValue = "0";
        txtNotes.Text = "";
        hdnCustomerId.Value = "0";
        lblRegDate.Visible = false;
        lblRegDateData.Visible = false;
        hypMap.Visible = false;

        txtStatusNote.Text = "";

        rdoCustomerLogin.SelectedValue = "0";
        pnlCustomerPass.Visible = false;

        grdCustomerEstimate.Visible = false;
        grdCustomersFile.Visible = false;
        grdCustomersMessage.Visible = false;
        HyperLink1.Visible = false;

        BindLeadSource();
    }

    private void GetCustomerSoldEstimate(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        //int nSalePersonId = Convert.ToInt32(obj.sales_person_id);

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var SoldEstimate = from cus_est in _db.customer_estimates
                               where cus_est.customer_id == nCustId && cus_est.client_id == Convert.ToInt32(hdnClientId.Value) && cus_est.status_id == 3
                               orderby cus_est.estimate_name ascending
                               select cus_est;
            grdCustomerEstimate.DataSource = SoldEstimate;
            grdCustomerEstimate.DataBind();
        }
        if (grdCustomerEstimate.Rows.Count > 0)
            pnlEstimate.Visible = true;
        else
            pnlEstimate.Visible = false;
    }



    protected void grdCustomerEstimate_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nEstId = Convert.ToInt32(grdCustomerEstimate.DataKeys[e.Row.RowIndex].Value.ToString());
            e.Row.Cells[4].ToolTip = nEstId.ToString();

            string strEstimateName = e.Row.Cells[0].Text.Trim();


            //HyperLink hypContract = (HyperLink)e.Row.FindControl("hypContract");
            //hypContract.NavigateUrl = "htmlcontract.aspx?cid=" + hdnCustomerId.Value + "&eid=" + nEstId;

            LinkButton lnkContractDoc = (LinkButton)e.Row.FindControl("lnkContractDoc");
            lnkContractDoc.CommandArgument = nEstId.ToString();

            LinkButton lnkHTML = (LinkButton)e.Row.FindControl("lnkHTML");
            lnkHTML.CommandArgument = nEstId.ToString();


            decimal totalwithtax = 0;
            if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
            {
                totalwithtax = 0;
            }
            else
            {
                estimate_payment esp = new estimate_payment();
                esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
                totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            }


            //decimal grandtotal = 0;
            //grandtotal=GetRetailTotal(nEstId) + GetDirctTotal(nEstId);

            e.Row.Cells[2].Text = totalwithtax.ToString("c"); //grandtotal.ToString("c");

            LinkButton lnkView = (LinkButton)e.Row.FindControl("lnkView");
            lnkView.Text = "View " + strEstimateName;
            lnkView.PostBackUrl = "sold_estimate.aspx?eid=" + nEstId + "&cid=" + Convert.ToInt32(hdnCustomerId.Value);

            if (DoesCOExist(Convert.ToInt32(hdnCustomerId.Value), nEstId))
            {
                LinkButton lnkViewCO = (LinkButton)e.Row.FindControl("lnkViewCO");
                lnkViewCO.Text = "Go to C/Os";
                lnkViewCO.PostBackUrl = "changeorderlist.aspx?eid=" + nEstId + "&cid=" + Convert.ToInt32(hdnCustomerId.Value);

            }
        }
    }

    private decimal GetRetailTotal(int nEstId)
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where pd.estimate_id == nEstId && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A"
                      select pd.total_retail_price);
        int n = result.Count();
        if (result != null && n > 0)
            dRetail = result.Sum();

        return dRetail;
    }
    private decimal GetDirctTotal(int nEstId)
    {
        decimal dDirect = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where pd.estimate_id == nEstId && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A"
                      select pd.total_direct_price);
        int n = result.Count();
        if (result != null && n > 0)
            dDirect = result.Sum();

        return dDirect;
    }

    private bool DoesCOExist(int nCustId, int nEstId)
    {
        bool bExist = false;
        DataClassesDataContext _db = new DataClassesDataContext();

        //int nSalePersonId = Convert.ToInt32(obj.sales_person_id);

        var item = from co_est in _db.changeorder_estimates
                   where co_est.customer_id == nCustId && co_est.client_id == Convert.ToInt32(hdnClientId.Value) && co_est.estimate_id == nEstId
                   select co_est;
        string strQ = "select * from changeorder_estimate where customer_id=" + nCustId + " and estimate_id =" + nEstId + " and client_id=" + Convert.ToInt32(hdnClientId.Value);
        IEnumerable<changeorder_estimate> list = _db.ExecuteQuery<changeorder_estimate>(strQ, string.Empty);

        if (list.Count() > 0)
            bExist = true;

        return bExist;
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
        decimal total_incentives = 0;
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
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";
        int IsQty = 1;
        int IsSubtotal = 2;


        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;


        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax); //new_total_with_tax

            if (esp.is_KithenSheet != null)
            {
                is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            }
            if (esp.is_BathSheet != null)
            {
                is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            }
            if (esp.is_ShowerSheet != null)
            {
                is_ShowerSheet = Convert.ToBoolean(esp.is_ShowerSheet);
            }
            if (esp.is_TubSheet != null)
            {
                is_TubSheet = Convert.ToBoolean(esp.is_TubSheet);
            }
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
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

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

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
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
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
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();
        // DataTable dtCabinet = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " )  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value) + " " +
                        "  Union " +
                          " SELECT  * from disclaimers WHERE disclaimers.section_level IN (410001,420001)";
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(hdnClientId.Value);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();

        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }

        //string strQCabinet = "select * from CabinetSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        //DataTable dtCabi = csCommonUtility.GetDataTable(strQCabinet);
        //if (dtCabi.Rows.Count > 0)
        //{
        //    dtCabinet = dtCabi;
        //}
        //else
        //{
        //    DataTable tmpTable = LoadSectionTable();
        //    DataRow drNew1 = tmpTable.NewRow();

        //    drNew1["CabinetSheetID"] = 0;
        //    drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        //    drNew1["estimate_id"] = nEstId;
        //    drNew1["UpperWallDoor"] = "";
        //    drNew1["UpperWallWood"] = "";
        //    drNew1["UpperWallStain"] = "";
        //    drNew1["UpperWallExterior"] = "";
        //    drNew1["UpperWallInterior"] = "";
        //    drNew1["UpperWallOther"] = "";
        //    drNew1["BaseDoor"] = "";
        //    drNew1["BaseWood"] = "";
        //    drNew1["BaseStain"] = "";
        //    drNew1["BaseExterior"] = "";
        //    drNew1["BaseInterior"] = "";
        //    drNew1["BaseOther"] = "";
        //    drNew1["MiscDoor"] = "";
        //    drNew1["MiscWood"] = "";
        //    drNew1["MiscStain"] = "";
        //    drNew1["MiscExterior"] = "";
        //    drNew1["MiscInterior"] = "";
        //    drNew1["MiscOther"] = "";
        //    drNew1["LastUpdateDate"] = DateTime.Now;
        //    drNew1["UpdateBy"] = User.Identity.Name;

        //    tmpTable.Rows.InsertAt(drNew1, 0);
        //    dtCabinet = tmpTable;

        //}


        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");

        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(hdnClientId.Value));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"] == "true")
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
        catch(Exception ex)
        {
            throw ex;
        }



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
        string CustPortalUrl = ConfigurationManager.AppSettings["CustPortalUrl"];
        CustPortalUrl = CustPortalUrl + "?cid=" + hdnCustomerId.Value;

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
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
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
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);
        ht.Add("p_CustPortalUrl", CustPortalUrl);


     

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }

    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("CabinetSheetID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("UpperWallDoor", typeof(string));
        table.Columns.Add("UpperWallWood", typeof(string));
        table.Columns.Add("UpperWallStain", typeof(string));
        table.Columns.Add("UpperWallExterior", typeof(string));
        table.Columns.Add("UpperWallInterior", typeof(string));
        table.Columns.Add("UpperWallOther", typeof(string));
        table.Columns.Add("BaseDoor", typeof(string));
        table.Columns.Add("BaseWood", typeof(string));
        table.Columns.Add("BaseStain", typeof(string));
        table.Columns.Add("BaseExterior", typeof(string));
        table.Columns.Add("BaseInterior", typeof(string));
        table.Columns.Add("BaseOther", typeof(string));
        table.Columns.Add("MiscDoor", typeof(string));
        table.Columns.Add("MiscWood", typeof(string));
        table.Columns.Add("MiscStain", typeof(string));
        table.Columns.Add("MiscExterior", typeof(string));
        table.Columns.Add("MiscInterior", typeof(string));
        table.Columns.Add("MiscOther", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }
    private DataTable LoadBathRoomTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("BathroomID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("BathroomSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Valve_Qty", typeof(string));
        table.Columns.Add("Sink_Valve_Style", typeof(string));
        table.Columns.Add("Sink_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Bathtub_Qty", typeof(string));
        table.Columns.Add("Bathtub_Style", typeof(string));
        table.Columns.Add("Bathtub_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Faucet_Qty", typeof(string));
        table.Columns.Add("Tub_Faucet_Style", typeof(string));
        table.Columns.Add("Tub_Faucet_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Valve_Qty", typeof(string));
        table.Columns.Add("Tub_Valve_Style", typeof(string));
        table.Columns.Add("Tub_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Drain_Qty", typeof(string));
        table.Columns.Add("Tub_Drain_Style", typeof(string));
        table.Columns.Add("Tub_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Tollet_Qty", typeof(string));
        table.Columns.Add("Tollet_Style", typeof(string));
        table.Columns.Add("Tollet_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_TubSystem_Qty", typeof(string));
        table.Columns.Add("Shower_TubSystem_Style", typeof(string));
        table.Columns.Add("Shower_TubSystem_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Value_Qty", typeof(string));
        table.Columns.Add("Shower_Value_Style", typeof(string));
        table.Columns.Add("Shower_Value_WhereToOrder", typeof(string));
        table.Columns.Add("Handheld_Shower_Qty", typeof(string));
        table.Columns.Add("Handheld_Shower_Style", typeof(string));
        table.Columns.Add("Handheld_Shower_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Style", typeof(string));
        table.Columns.Add("Body_Spray_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Style", typeof(string));
        table.Columns.Add("Body_Spray_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Style", typeof(string));
        table.Columns.Add("Shower_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Qty", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Style", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Qty", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Style", typeof(string));
        table.Columns.Add("Tub_skirt_tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Qty", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Style", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_Style", typeof(string));
        table.Columns.Add("Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Style", typeof(string));
        table.Columns.Add("Floor_Tile_layout_WhereToOrder", typeof(string));
        table.Columns.Add("BullnoseTile_Qty", typeof(string));
        table.Columns.Add("BullnoseTile_Style", typeof(string));
        table.Columns.Add("BullnoseTile_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Style", typeof(string));
        table.Columns.Add("Deco_Band_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Height_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Height_Style", typeof(string));
        table.Columns.Add("Deco_Band_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Tile_Baseboard_Qty", typeof(string));
        table.Columns.Add("Tile_Baseboard_Style", typeof(string));
        table.Columns.Add("Tile_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Grout_Selection_Qty", typeof(string));
        table.Columns.Add("Grout_Selection_Style", typeof(string));
        table.Columns.Add("Grout_Selection_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Location_Qty", typeof(string));
        table.Columns.Add("Niche_Location_Style", typeof(string));
        table.Columns.Add("Niche_Location_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Size_Qty", typeof(string));
        table.Columns.Add("Niche_Size_Style", typeof(string));
        table.Columns.Add("Niche_Size_WhereToOrder", typeof(string));
        table.Columns.Add("Glass_Qty", typeof(string));
        table.Columns.Add("Glass_Style", typeof(string));
        table.Columns.Add("Glass_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Grab_Bar_Qty", typeof(string));
        table.Columns.Add("Grab_Bar_Style", typeof(string));
        table.Columns.Add("Grab_Bar_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Qty", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Style", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Qty", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Style", typeof(string));
        table.Columns.Add("Medicine_Cabinet_WhereToOrder", typeof(string));
        table.Columns.Add("Mirror_Qty", typeof(string));
        table.Columns.Add("Mirror_Style", typeof(string));
        table.Columns.Add("Mirror_WhereToOrder", typeof(string));
        table.Columns.Add("Wood_Baseboard_Qty", typeof(string));
        table.Columns.Add("Wood_Baseboard_Style", typeof(string));
        table.Columns.Add("Wood_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Paint_Color_Qty", typeof(string));
        table.Columns.Add("Paint_Color_Style", typeof(string));
        table.Columns.Add("Paint_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("TowelRing_Qty", typeof(string));
        table.Columns.Add("TowelRing_Style", typeof(string));
        table.Columns.Add("TowelRing_WhereToOrder", typeof(string));
        table.Columns.Add("TowelBar_Qty", typeof(string));
        table.Columns.Add("TowelBar_Style", typeof(string));
        table.Columns.Add("TowelBar_WhereToOrder", typeof(string));
        table.Columns.Add("TissueHolder_Qty", typeof(string));
        table.Columns.Add("TissueHolder_Style", typeof(string));
        table.Columns.Add("TissueHolder_WhereToOrder", typeof(string));
        table.Columns.Add("ClosetDoorSeries", typeof(string));
        table.Columns.Add("ClosetDoorOpeningSize", typeof(string));
        table.Columns.Add("ClosetDoorNumberOfPanels", typeof(string));
        table.Columns.Add("ClosetDoorFinish", typeof(string));
        table.Columns.Add("ClosetDoorInsert", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("KitchenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinets_Qty", typeof(string));
        table.Columns.Add("Cabinets_Style", typeof(string));
        table.Columns.Add("Cabinets_WhereToOrder", typeof(string));
        table.Columns.Add("Disposal_Qty", typeof(string));
        table.Columns.Add("Disposal_Style", typeof(string));
        table.Columns.Add("Disposal_WhereToOrder", typeof(string));
        table.Columns.Add("Baseboard_Qty", typeof(string));
        table.Columns.Add("Baseboard_Style", typeof(string));
        table.Columns.Add("Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("AutoKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenTileSheetName", typeof(string));
        table.Columns.Add("BacksplashQTY", typeof(string));
        table.Columns.Add("BacksplashMOU", typeof(string));
        table.Columns.Add("BacksplashStyle", typeof(string));
        table.Columns.Add("BacksplashColor", typeof(string));
        table.Columns.Add("BacksplashSize", typeof(string));
        table.Columns.Add("BacksplashVendor", typeof(string));
        table.Columns.Add("BacksplashPattern", typeof(string));
        table.Columns.Add("BacksplashGroutColor", typeof(string));
        table.Columns.Add("BBullnoseQTY", typeof(string));
        table.Columns.Add("BBullnoseMOU", typeof(string));
        table.Columns.Add("BBullnoseStyle", typeof(string));
        table.Columns.Add("BBullnoseColor", typeof(string));
        table.Columns.Add("BBullnoseSize", typeof(string));
        table.Columns.Add("BBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));


        return table;
    }

    private DataTable LoadTubTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("TubID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("TubTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("ShelfLocation", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }

    private DataTable LoadShowerTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ShowerKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("ShowerTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("ShowerPanQTY", typeof(string));
        table.Columns.Add("ShowerPanMOU", typeof(string));
        table.Columns.Add("ShowerPanStyle", typeof(string));
        table.Columns.Add("ShowerPanColor", typeof(string));
        table.Columns.Add("ShowerPanSize", typeof(string));
        table.Columns.Add("ShowerPanVendor", typeof(string));
        table.Columns.Add("ShowerPanPattern", typeof(string));
        table.Columns.Add("ShowerPanGroutColor", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("BenchTileQTY", typeof(string));
        table.Columns.Add("BenchTileMOU", typeof(string));
        table.Columns.Add("BenchTileStyle", typeof(string));
        table.Columns.Add("BenchTileColor", typeof(string));
        table.Columns.Add("BenchTileSize", typeof(string));
        table.Columns.Add("BenchTileVendor", typeof(string));
        table.Columns.Add("BenchLocation", typeof(string));
        table.Columns.Add("BenchSize", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
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
        decimal total_incentives = 0;
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
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";
        int IsQty = 1;
        int IsSubtotal = 2;


        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;


        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax); //edited

            if (esp.is_KithenSheet != null)
            {
                is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            }
            if (esp.is_BathSheet != null)
            {
                is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            }
            if (esp.is_ShowerSheet != null)
            {
                is_ShowerSheet = Convert.ToBoolean(esp.is_ShowerSheet);
            }
            if (esp.is_TubSheet != null)
            {
                is_TubSheet = Convert.ToBoolean(esp.is_TubSheet);
            }
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
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

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

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
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
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
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();

        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptShortContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

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
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);

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
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
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



    protected void rdoCustomerLogin_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(rdoCustomerLogin.SelectedValue) == 1)
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            if (_db.customeruserinfos.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
            {
                customeruserinfo objCU = new customeruserinfo();
                objCU = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));
                txtUserName.Text = objCU.customerusername;
            }
            else
            {
                int nCount = GetCountCustomer();
                if (Convert.ToInt32(hdnCustomerId.Value) > 0)
                {
                    if (nCount == 1)
                        txtUserName.Text = txtEmail.Text;
                    else if (nCount == 2)
                        txtUserName.Text = txtLastName1.Text;
                    else if (nCount == 3)
                        txtUserName.Text = txtFirstName1.Text;
                    else if (nCount == 4)
                        txtUserName.Text = txtFirstName1.Text + "" + txtLastName1.Text;
                    else
                        txtUserName.Text = txtEmail.Text + "" + nCount.ToString();
                }
                else
                {
                    if (nCount == 0)
                        txtUserName.Text = txtEmail.Text;
                    else if (nCount == 1)
                        txtUserName.Text = txtLastName1.Text;
                    else if (nCount == 2)
                        txtUserName.Text = txtFirstName1.Text;
                    else if (nCount == 3)
                        txtUserName.Text = txtFirstName1.Text + "" + txtLastName1.Text;
                    else
                        txtUserName.Text = txtEmail.Text + "" + nCount.ToString();
                }

            }
            pnlCustomerPass.Visible = true;

        }
        else
        {
            pnlCustomerPass.Visible = false;
        }
    }
    //protected void btnSavePassword_Click(object sender, EventArgs e)
    //{
    //    if (!SaveCustomerUser())
    //    {
    //        return;
    //    }

    //}

    private void GetCustomerFileInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        //int nSalePersonId = Convert.ToInt32(obj.sales_person_id);

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var file = from file_info in _db.file_upload_infos
                       where file_info.CustomerId == nCustId && file_info.client_id == Convert.ToInt32(hdnClientId.Value) && file_info.type != 1 && file_info.type != 5
                       orderby file_info.upload_fileId ascending
                       select file_info;
            grdCustomersFile.DataSource = file;
            grdCustomersFile.DataKeyNames = new string[] { "upload_fileId", "is_design", "ImageName", "dms_fileid", "dms_dirid" };
            grdCustomersFile.DataBind();
        }

    }
    protected void grdCustomersFile_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.Row.RowIndex].Values[0].ToString());
            bool is_design = Convert.ToBoolean(grdCustomersFile.DataKeys[e.Row.RowIndex].Values[1].ToString());
            string strFile = grdCustomersFile.DataKeys[e.Row.RowIndex].Values[2].ToString();
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
    protected void grdCustomersFile_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        string ImageName = grdCustomersFile.DataKeys[e.RowIndex].Values[2].ToString();
        int dms_fileid = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[3].ToString());
        int dms_dirid = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[4].ToString());

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
    protected void grdCustomersFile_RowEditing(object sender, GridViewEditEventArgs e)
    {
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.NewEditIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.NewEditIndex].FindControl("lblDescription");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        LinkButton btn = (LinkButton)grdCustomersFile.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdCustomersFile_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int upload_fileId = Convert.ToInt32(grdCustomersFile.DataKeys[e.RowIndex].Values[0].ToString());
        TextBox txtDescription = (TextBox)grdCustomersFile.Rows[e.RowIndex].FindControl("txtDescription");
        Label lblDescription = (Label)grdCustomersFile.Rows[e.RowIndex].FindControl("lblDescription");
        string StrQ = "UPDATE file_upload_info SET Desccription='" + txtDescription.Text.Replace("'", "''") + "' WHERE upload_fileId=" + Convert.ToInt32(upload_fileId) + " AND client_id =" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(StrQ, string.Empty);
        GetCustomerFileInfo(Convert.ToInt32(hdnCustomerId.Value));
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        Response.Redirect("DocumentManagement.aspx?cid=" + hdnCustomerId.Value);
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
        lblMessage.Text = "";
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

                if (!System.IO.Directory.Exists(NewDir + "\\Original"))
                {
                    System.IO.Directory.CreateDirectory(NewDir + "\\Original");
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
        // Response.Redirect("schedulecalendar.aspx?cid=" + hdnCustomerId.Value + "&TypeID=2");
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
            lblResultCallLog.Text = "";
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
            ResetCustomerContact();
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
            custContact.Fax = "";
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
    private bool SaveCustomerUser()
    {
        bool bFalg = true;
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        customeruserinfo objcu = new customeruserinfo();
        customer objCust = new customer();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

            if (rdoCustomerLogin.SelectedValue == "1")
            {
                if (_db.customeruserinfos.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                {
                    objcu = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));

                    if (txtPassword.Text.Trim() == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Password.");
                        bFalg = false;
                    }
                    else
                    {
                        if (Convert.ToInt32(txtPassword.Text.Length) < 6)
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Password length should be minimum 6");
                            bFalg = false;
                        }
                        if (txtConfirmPassword.Text.Trim() == "")
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: Confirm Password.");
                            bFalg = false;
                        }
                        if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please confirm password.");
                            bFalg = false;
                        }
                    }
                    objcu.customerpassword = txtPassword.Text.Trim();
                    if (bFalg)
                    {
                        _db.SubmitChanges();
                        // lblResult.Text = csCommonUtility.GetSystemMessage("Password reset successfully.");
                    }

                }
                else
                {
                    if (txtPassword.Text.Trim() == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Password.");
                        bFalg = false;
                    }
                    else
                    {
                        if (Convert.ToInt32(txtPassword.Text.Length) < 6)
                        {
                            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Password length should be minimum 6");
                            bFalg = false;
                        }
                        if (txtConfirmPassword.Text.Trim() == "")
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: Confirm Password.");
                            bFalg = false;
                        }
                        if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
                        {
                            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please confirm password.");
                            bFalg = false;
                        }
                    }

                    objcu.customerid = objCust.customer_id;
                    objcu.customerusername = txtUserName.Text;
                    objcu.customerpassword = txtPassword.Text.Trim();
                    objcu.isactive = 1;
                    if (_db.customeruserinfos.Where(c => c.customerusername == txtUserName.Text.Trim()).SingleOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("User already exist. Please try another user name.");
                        bFalg = false;
                    }
                    if (bFalg)
                    {
                        _db.customeruserinfos.InsertOnSubmit(objcu);
                        _db.SubmitChanges();

                        //lblResult1.Text = csCommonUtility.GetSystemMessage("Password saved successfully.");
                        //btnSavePassword.Visible = false;
                    }



                }
            }
        }
        return bFalg;
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

}