using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GoogleCalendarRefreshToken : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSalesPerson();
            var code = "";
            //if (Request.QueryString.Get("code") == null)
            //{
            //    Authenticate();
            //}
            if (Request.QueryString.Get("code") != null)
            {
                code = Request.QueryString.Get("code").ToString();
                GoogleAuthorization(code);
            }

        }
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnTest.ID, btnTest.GetType().Name, "Click"); 
        //Authenticate();
        create();
    }

    private void BindSalesPerson()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE google_calendar_account is not null and google_calendar_account != '' and sales_person.is_active = 1 and sales_person.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sales_person_id asc";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSalesPerson.DataSource = mList;
        ddlSalesPerson.DataTextField = "sales_person_name";
        ddlSalesPerson.DataValueField = "sales_person_id";
        ddlSalesPerson.DataBind();
    }

    public void Authenticate()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string calendarId = string.Empty;
            // string strUserName = "tislam";
            //calendarId = _db.UserProfiles.FirstOrDefault(c => c.UserName == strUserName).Email;
            //calendarId = "6i3ieredsih1japqbknrj31d9o@group.calendar.google.com";

            Session.Add("UserID", Convert.ToInt32(ddlSalesPerson.SelectedValue));
            sales_person objSP = new sales_person();
            if (_db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
            {
                objSP = _db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                calendarId = objSP.google_calendar_id;
            }

            string url = GoogleAuthorizationHelper.GetAuthorizationUrl(calendarId);

            Response.Redirect(url);

            lblMessage.Text = csCommonUtility.GetSystemMessage("Authenticate OK");

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemMessage("Authenticate Error: " + ex.Message); ;
        }

    }

    public void GoogleAuthorization(string code)
    {
        try
        {
            // Retrieve the authenticator and save it in session for future use
            var authenticator = GoogleAuthorizationHelper.GetAuthenticator(code);
            //  Session["authenticator"] = authenticator;

            // Save the refresh token locally
            DataClassesDataContext _db = new DataClassesDataContext();
            GoogleRefreshToken objGRT = new GoogleRefreshToken();
            int nUserID = Convert.ToInt32(Session["UserID"]);

            if (_db.GoogleRefreshTokens.Where(c => c.UserID == nUserID).Count() == 0)
            {
                objGRT.UserID = nUserID;
                objGRT.RefreshToken = authenticator.RefreshToken;
                _db.GoogleRefreshTokens.InsertOnSubmit(objGRT);
                _db.SubmitChanges();

            }
            else
            {
                objGRT = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserID == nUserID);
                objGRT.RefreshToken = authenticator.RefreshToken;
                _db.SubmitChanges();
            }

            lblMessage.Text = csCommonUtility.GetSystemMessage("GoogleAuthorization OK");

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemMessage("GoogleAuthorization Error: " + ex.Message); ;
        }
    }

    private void create()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nUserID = Convert.ToInt32(Session["UserID"]);
            string calendarId = string.Empty;
            sales_person objSP = new sales_person();
            if (_db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
            {
                objSP = _db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                calendarId = objSP.google_calendar_id;
            }
            //calendarId = _db.GoogleUser.FirstOrDefault(c => c.UserName == "tislam").Email;

            var calendarEvent = new gCalendarEvent()
            {
                CalendarId = calendarId,
                Title = "II Test (John Smith)",
                Location = "Arizona Test",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMinutes(60),
                Description = "Let's start this day",
                ColorId = 1
            };

            var authenticator = GetAuthenticator(nUserID);
            var service = new GoogleCalendarServiceProxy(authenticator);

            service.CreateEvent(calendarEvent);

            lblMessage.Text = csCommonUtility.GetSystemMessage("CalendarEvent Saved");

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("CalendarEvent Error: " + ex.StackTrace + "<br/><br/>" + ex.Message); ;
        }
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

    protected void btnAuthenticate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAuthenticate.ID, btnAuthenticate.GetType().Name, "Click"); 
        Authenticate();
    }

    protected void btnAuthenticateByID_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAuthenticateByID.ID, btnAuthenticateByID.GetType().Name, "Click"); 
        try
        {
            if (txtCalendarID.Text == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Calendar ID is required");
                return;
            }
            if (txtUserID.Text == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("User ID is required");
                return;
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            string calendarId = string.Empty;

            //--------- Save GoogleRefreshToken for specific calendar by calendarId & UserID (Example: A Opeartion Calendar ("Faztimate (Operation)") in Google) --------- 
            calendarId = txtCalendarID.Text.Trim(); // Holtzman (Operation)
            Session.Add("CalendarID", calendarId);
            Session.Add("UserID", txtUserID.Text.Trim());

            string url = GoogleAuthorizationHelper.GetAuthorizationUrl(calendarId);

            Response.Redirect(url);

            lblMessage.Text = csCommonUtility.GetSystemMessage("Authenticate OK");

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Authenticate Error: " + ex.Message);
        }
    }

    protected void btnTestByID_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnTestByID.ID, btnTestByID.GetType().Name, "Click"); 

        try
        {
            if (Session["CalendarID"] == null || Session["UserID"] == null)
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please Authenticate first");
                return;
            }

            DataClassesDataContext _db = new DataClassesDataContext();

            string calendarId = Session["CalendarID"].ToString();// Holtzman (Operation)
            int nUserID = Convert.ToInt32(Session["UserID"]);

            var calendarEvent = new gCalendarEvent()
            {
                CalendarId = calendarId,
                Title = "II Test (John Smith)",
                Location = "Arizona Test",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMinutes(60),
                Description = "Let's start this day",
                ColorId = 1
            };

            var authenticator = GetAuthenticator(nUserID);
            var service = new GoogleCalendarServiceProxy(authenticator);

            service.CreateEvent(calendarEvent);

            lblMessage.Text = csCommonUtility.GetSystemMessage("CalendarEvent Saved");

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemMessage("CalendarEvent Error: " + ex.Message); ;
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnReset.ID, btnReset.GetType().Name, "Click"); 
        lblMessage.Text = "";
        txtCalendarID.Text = "";
        txtUserID.Text = "";
        Session.Add("CalendarID", null);
        Session.Add("UserID", null);
        Response.Redirect("GoogleCalendarRefreshToken.aspx");
    }



    //protected void btnText_Click(object sender, EventArgs e)
    //{

    //    //Authenticate();
    //    create();
    //}
    //private void BindSalesPerson()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE google_calendar_account is not null and google_calendar_account != '' and sales_person.is_active = 1 and sales_person.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sales_person_id asc";
    //    List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
    //    ddlSalesPerson.DataSource = mList;
    //    ddlSalesPerson.DataTextField = "sales_person_name";
    //    ddlSalesPerson.DataValueField = "sales_person_id";
    //    ddlSalesPerson.DataBind();
    //}
    //public void Authenticate()
    //{
    //    try
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        string calendarId = string.Empty;
    //        // string strUserName = "tislam";
    //        //calendarId = _db.UserProfiles.FirstOrDefault(c => c.UserName == strUserName).Email;
    //        //calendarId = "6i3ieredsih1japqbknrj31d9o@group.calendar.google.com";
    //        Session.Add("UserID", Convert.ToInt32(ddlSalesPerson.SelectedValue));

    //        sales_person objSP = new sales_person();
    //        if (_db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
    //        {
    //            objSP = _db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
    //            calendarId = objSP.google_calendar_id;
    //        }

    //        string url = GoogleAuthorizationHelper.GetAuthorizationUrl(calendarId);

    //        Response.Redirect(url);

    //        lblMessage.Text = csCommonUtility.GetSystemMessage("Authenticate OK");

    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Authenticate Error: " + ex.Message); ;
    //    }

    //}

    //public void GoogleAuthorization(string code)
    //{
    //    try
    //    {
    //        // Retrieve the authenticator and save it in session for future use
    //        var authenticator = GoogleAuthorizationHelper.GetAuthenticator(code);
    //      //  Session["authenticator"] = authenticator;

    //        // Save the refresh token locally
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        GoogleRefreshToken objGRT = new GoogleRefreshToken();
    //        int nUserID = Convert.ToInt32(Session["UserID"]);

    //        if (_db.GoogleRefreshTokens.Where(c => c.UserID == nUserID).Count() == 0)
    //        {
    //            objGRT.UserID = nUserID;
    //            objGRT.RefreshToken = authenticator.RefreshToken;
    //            _db.GoogleRefreshTokens.InsertOnSubmit(objGRT);
    //            _db.SubmitChanges();

    //        }
    //        else
    //        {
    //            objGRT = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserID == nUserID);
    //            objGRT.RefreshToken = authenticator.RefreshToken;
    //            _db.SubmitChanges();
    //        }

    //        lblMessage.Text = csCommonUtility.GetSystemMessage("GoogleAuthorization OK");

    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessage.Text = csCommonUtility.GetSystemErrorMessage("GoogleAuthorization Error: " + ex.Message); ;
    //    }
    //}


    //private void create()
    //{
    //    try
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        int nUserID = Convert.ToInt32(Session["UserID"]);
    //        string calendarId = string.Empty;
    //        sales_person objSP = new sales_person();
    //        if (_db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
    //        {
    //            objSP = _db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(ddlSalesPerson.SelectedValue) && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
    //            calendarId = objSP.google_calendar_id;
    //        }
    //        //calendarId = _db.GoogleUser.FirstOrDefault(c => c.UserName == "tislam").Email;

    //        var calendarEvent = new gCalendarEvent()
    //        {
    //            CalendarId = calendarId,
    //            Title = "Holtzman Test (" + objSP.first_name + " " + objSP.last_name + ")",
    //            Location = "Arizona Test",
    //            StartDate = DateTime.Today,
    //            EndDate = DateTime.Today.AddMinutes(60),
    //            Description = "Let's start this day",
    //            ColorId = 1
    //        };

    //        var authenticator = GetAuthenticator(nUserID);
    //        var service = new GoogleCalendarServiceProxy(authenticator);

    //        service.CreateEvent(calendarEvent);

    //        lblMessage.Text = csCommonUtility.GetSystemMessage("CalendarEvent Saved");

    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessage.Text = csCommonUtility.GetSystemErrorMessage("CalendarEvent Error: " + ex.Message); ;
    //    }
    //}
    //private GoogleAuthenticator GetAuthenticator(int salespersonid)
    //{
    //    //var authenticator = (GoogleAuthenticator)Session["authenticator"];

    //    //if (authenticator == null || !authenticator.IsValid)
    //    //{
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        // Get a new Authenticator using the Refresh Token
    //        int nUserID = salespersonid;
    //        var refreshToken = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserID == salespersonid).RefreshToken;
    //        var authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
    //    //    System.Web.HttpContext.Current.Session["authenticator"] = authenticator;
    //    //}

    //    return authenticator;
    //}
    //protected void btnAuthenticate_Click(object sender, EventArgs e)
    //{
    //    Authenticate();
    //}
}