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

public partial class mobile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            Session.RemoveAll();
            // Remember Password
            if (Request.Cookies["username"] != null)

                txtUserName.Text = Request.Cookies["username"].Value;

            if (Request.Cookies["pwd"] != null)

                txtPassword.Attributes.Add("value", Request.Cookies["pwd"].Value);
            if (Request.Cookies["username"] != null && Request.Cookies["pwd"] != null)
                rememberme.Checked = true;

            //string strUseHTTPS = System.Configuration.ConfigurationManager.AppSettings["UseHTTPS"];
            //if (strUseHTTPS == "Yes")
            //{


            //    if (Request.Url.Scheme == "http")
            //    {
            //        string strQuery = Request.Url.AbsoluteUri;

            //        Response.Redirect(strQuery.Replace("http:", "https:").Replace("ii.faztrack.com", "ii.faztrack.com"));
            //    }
            //}
            //else if (strUseHTTPS == "No")
            //{
            //    if (Request.Url.Scheme == "https")
            //    {
            //        string strQuery = Request.Url.AbsoluteUri;
            //        Response.Redirect(strQuery.Replace("https:", "http:"));
            //    }
            //}
          

            if (Request.QueryString.Get("appuid") != null)
            {

                int nUserId = Convert.ToInt32(Request.QueryString.Get("appuid"));
                string Type = Request.QueryString.Get("type");
                Session.Add("Type", Type);
                LoginFromApp(nUserId, Type);

            }
        }
    }

    private void LoginFromApp(int UserId, string Type)
    {
        int ClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        DataClassesDataContext _db = new DataClassesDataContext();

        if (UserId == 0 && Type == "admin")
        {
            userinfo obj = new userinfo();
            obj.first_name = "FazTrack";
            obj.username = "Faztrack";
            obj.role_id = 1;
            obj.user_id = 0;
            obj.email = "tislam@faztrack.com";
            obj.sales_person_id = 0;
            obj.IsTimeClock = true;
            Session.Add("oUser", obj);

            Session.Add("sRole", "admin"); //for Context menu

            Session.Add("sPermissionRole", "admin"); // for table Column

            string sPrevliage = this.GetPrevliage((int)obj.role_id);

            if (sPrevliage.Length > 1)  // means authetication
            {
                // Create the authentication ticket
                FormsAuthenticationTicket authTicket = new
                FormsAuthenticationTicket(1,  // version
                obj.first_name,               // user name
                DateTime.Now,                 // creation
                DateTime.Now.AddMinutes(720),  // Expiration
                false,                        // Persistent
                sPrevliage);                     // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie and add the encrypted ticket to the 
                // cookie as data.
                HttpCookie authCookie =
                  new HttpCookie(FormsAuthentication.FormsCookieName,
                  encryptedTicket);
                // Add the cookie to the outgoing cookies collection. 
                Response.Cookies.Add(authCookie);


                Response.Redirect("mlandingpage.aspx");
            }
        }




        //Session.Clear();
        string role = GetUserRoles(UserId, ClientId);

        if (Type == "crew")
        {
            Crew_Detail objCrew = new Crew_Detail();
            objCrew = _db.Crew_Details.Single(u => u.client_id == ClientId && u.crew_id == UserId);
            Crew_Detail objC = new Crew_Detail();

            objC.first_name = objCrew.first_name;
            objC.last_name = objCrew.last_name;
            objC.Address = objCrew.Address;
            objC.city = objCrew.city;
            objC.state = objCrew.state;
            objC.zip_code = objCrew.zip_code;
            objC.phone = objCrew.phone;
            objC.fax = objCrew.fax;
            objC.email = objCrew.email;
            objC.is_active = Convert.ToBoolean(objCrew.is_active);
            objC.client_id = Convert.ToInt32(objCrew.client_id);
            objC.CreatedDate = Convert.ToDateTime(objCrew.CreatedDate);
            objC.username = objCrew.username;
            objC.crew_id = Convert.ToInt32(objCrew.crew_id);
            objC.MaxCrewId = Convert.ToInt32(objCrew.MaxCrewId);
            objC.full_name = objCrew.full_name;
            objC.MaxCrewId = objCrew.MaxCrewId;

            Session.Add("oCrew", objC);

            // Create the authentication ticket
            FormsAuthenticationTicket authTicket = new
              FormsAuthenticationTicket(1,  // version
              objCrew.username,               // user name
              DateTime.Now,                 // creation
              DateTime.Now.AddMinutes(720),  // Expiration
              false,                        // Persistent
              role);                     // User data

            // Now encrypt the ticket.
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            // Create a cookie and add the encrypted ticket to the 
            // cookie as data.
            HttpCookie authCookie =
              new HttpCookie(FormsAuthentication.FormsCookieName,
              encryptedTicket);
            // Add the cookie to the outgoing cookies collection. 
            Response.Cookies.Add(authCookie);
            //UpdateLoginDateTime(userName);

            //Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));
            Session.Add("sRole", "admin"); //for Context menu

            Session.Add("sPermissionRole", "admin"); // for table Column



            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "initializeMap", "initializeMap();", true);
            Response.Redirect("mlandingpage.aspx");

        }
        else if (Type == "user")
        {
            user_info uinfo = new user_info();

            uinfo = _db.user_infos.Single(u =>  u.user_id == UserId);

            userinfo obj = new userinfo();
            obj.first_name = uinfo.first_name;
            obj.last_name = uinfo.last_name;
            obj.address = uinfo.address;
            obj.city = uinfo.city;
            obj.state = uinfo.state;
            obj.zip = uinfo.zip;
            obj.phone = uinfo.phone;
            obj.fax = uinfo.fax;
            obj.email = uinfo.email;
            obj.role_id = Convert.ToInt32(uinfo.role_id);
            obj.is_active = Convert.ToBoolean(uinfo.is_active);
            obj.client_id = uinfo.client_id;
            obj.create_date = Convert.ToDateTime(uinfo.create_date);
            obj.username = uinfo.username;
            obj.sales_person_id = Convert.ToInt32(uinfo.sales_person_id);
            obj.is_verify = Convert.ToBoolean(uinfo.is_verify);
            obj.company_email = uinfo.company_email;
            obj.email_password = uinfo.email_password;
            obj.user_id = Convert.ToInt32(uinfo.user_id);
            obj.IsTimeClock = Convert.ToBoolean(uinfo.IsTimeClock);
            Session.Add("oUser", obj);

            Session.Add("sRole", "admin"); //for Context menu

            Session.Add("sPermissionRole", "admin"); // for table Column

            // Update Last Login by User
            uinfo.last_login_time = Convert.ToDateTime(DateTime.Now);
            _db.SubmitChanges();
            string strQ = "UPDATE sales_person SET last_login_time='" + uinfo.last_login_time + "' WHERE sales_person_id =" + obj.sales_person_id + " AND client_id=" + ClientId;
            _db.ExecuteCommand(strQ, string.Empty);

            // Create the authentication ticket
            FormsAuthenticationTicket authTicket = new
              FormsAuthenticationTicket(1,  // version
              uinfo.username,               // user name
              DateTime.Now,                 // creation
              DateTime.Now.AddMinutes(720),  // Expiration
              false,                        // Persistent
              role);                     // User data

            // Now encrypt the ticket.
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            // Create a cookie and add the encrypted ticket to the 
            // cookie as data.
            HttpCookie authCookie =
              new HttpCookie(FormsAuthentication.FormsCookieName,
              encryptedTicket);
            // Add the cookie to the outgoing cookies collection. 
            Response.Cookies.Add(authCookie);
            //UpdateLoginDateTime(userName);

            //Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));

            Response.Redirect("mlandingpage.aspx");
        }


    }

    public string GetUserRoles(int nUserID, int nClientID)
    {
        string sRoles = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select m.* from Menu_item m " +
                    " right join role_right r on r.menu_id = m.menu_id " +
                    " right outer join user_info u on u.role_id = r.role_id " +
                    " WHERE u.is_active = 1 AND u.user_id =" + nUserID + " AND r.client_id = " + nClientID + " AND m.client_id = " + nClientID;

        List<menu_item> list = _db.ExecuteQuery<menu_item>(strQ, string.Empty).ToList();

        if (list.Count() > 0)
        {
            foreach (menu_item mi in list)
            {
                sRoles += "|" + mi.menu_code;
            }
        }
        return sRoles;
    }

    protected void btnLogIn_Click(object sender, EventArgs e)
    {

        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnLogIn.ID, btnLogIn.GetType().Name, "Click");
            lblResult.Text = "";

            DataClassesDataContext _db = new DataClassesDataContext();

            int nClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

            client_info objClient = new client_info();
            objClient = _db.client_infos.Single(ci => ci.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            Session.Add("MyCompany", objClient);

            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string crewPassword = txtPassword.Text.Trim();


            # region Super User Login

            if (userName.ToLower() == "Faztrack".ToLower() && password == "F@ztrack")
            {
                password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
                userinfo obj = new userinfo();
                obj.first_name = "FazTrack";
                obj.username = "Faztrack";
                obj.role_id = 1;
                obj.user_id = 0;
                obj.email = "tislam@faztrack.com";
                obj.sales_person_id = 0;
                obj.IsTimeClock = true;
                obj.client_id = "1,2";
                Session.Add("oUser", obj);

                Session.Add("sRole", "admin"); //for Context menu

                Session.Add("sPermissionRole", "admin"); // for table Column

                string sPrevliage = this.GetPrevliage((int)obj.role_id);

                if (sPrevliage.Length > 1)  // means authetication
                {
                    // Create the authentication ticket
                    FormsAuthenticationTicket authTicket = new
                    FormsAuthenticationTicket(1,  // version
                    obj.first_name,               // user name
                    DateTime.Now,                 // creation
                    DateTime.Now.AddMinutes(720),  // Expiration
                    false,                        // Persistent
                    sPrevliage);                     // User data

                    // Now encrypt the ticket.
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    // Create a cookie and add the encrypted ticket to the 
                    // cookie as data.
                    HttpCookie authCookie =
                      new HttpCookie(FormsAuthentication.FormsCookieName,
                      encryptedTicket);
                    // Add the cookie to the outgoing cookies collection. 
                    Response.Cookies.Add(authCookie);

                    // Redirect the user to the originally requested page
                    // Response.Redirect(FormsAuthentication.GetRedirectUrl(obj.username, false));

                    if (rememberme.Checked == true)
                    {
                        Response.Cookies["username"].Value = userName;
                        Response.Cookies["pwd"].Value = txtPassword.Text.Trim();
                        Response.Cookies["username"].Expires = DateTime.Now.AddYears(20);
                        Response.Cookies["pwd"].Expires = DateTime.Now.AddYears(20);
                    }
                    else
                    {

                        Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);

                        Response.Cookies["pwd"].Expires = DateTime.Now.AddDays(-1);

                    }
                    Response.Redirect("mlandingpage.aspx");
                }
            }

            # endregion

            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");

            //Session.Clear();

            string role = ""; //GetUserRoles(userName, password, nClientId);

            if (_db.user_infos.Where(sp =>  sp.username == userName && sp.password == password && sp.is_active == Convert.ToBoolean(1)).SingleOrDefault() == null)
            {
                if (_db.Crew_Details.Where(sp =>  sp.username.ToLower() == userName.ToLower() && sp.password == crewPassword && sp.is_active == Convert.ToBoolean(1)).SingleOrDefault() == null)
                {
                    lblResult.Text = "Invalid username and password.";
                    lblResult.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    Crew_Detail objCrew = new Crew_Detail();
                    objCrew = _db.Crew_Details.Single(u =>  u.username == userName && u.password == crewPassword);
                  
                    
                   
                    Crew_Detail objC = new Crew_Detail();
                    try
                    {
                        objC.first_name = objCrew.first_name;
                        objC.last_name = objCrew.last_name;
                        objC.Address = objCrew.Address;
                        objC.city = objCrew.city;
                        objC.state = objCrew.state;
                        objC.zip_code = objCrew.zip_code;
                        objC.phone = objCrew.phone;
                        objC.fax = objCrew.fax;
                        objC.email = objCrew.email;
                        objC.is_active = Convert.ToBoolean(objCrew.is_active);
                        objC.client_id = Convert.ToInt32(objCrew.client_id);
                        objC.CreatedDate = Convert.ToDateTime(objCrew.CreatedDate);
                        objC.username = objCrew.username;
                        objC.crew_id = Convert.ToInt32(objCrew.crew_id);
                        objC.MaxCrewId = Convert.ToInt32(objCrew.MaxCrewId);
                        objC.full_name = objCrew.full_name;
                        objC.MaxCrewId = objCrew.MaxCrewId;
                        Session.Add("oCrew", objC);
                        // csCommonUtility.WriteLog("Crew Session added");
                        Session.Add("sRole", "admin"); //for Context menu

                        Session.Add("sPermissionRole", "admin"); // for table Column
                    }
                   catch( Exception ex)
                    {
                       // csCommonUtility.WriteLog(" Crew login error in Catch " + ex.Message);
                    }

                   
                    
                    // Create the authentication ticket

                    try
                    {

                        csCommonUtility.WriteLog("Crew Authenticate");

                        FormsAuthenticationTicket authTicket = new
                          FormsAuthenticationTicket(1,  // version
                          userName,               // user name
                          DateTime.Now,                 // creation
                          DateTime.Now.AddMinutes(720),  // Expiration
                          false,                        // Persistent
                          role);                     // User data

                        // Now encrypt the ticket.
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        // Create a cookie and add the encrypted ticket to the 
                        // cookie as data.
                        HttpCookie authCookie =
                          new HttpCookie(FormsAuthentication.FormsCookieName,
                          encryptedTicket);
                        // Add the cookie to the outgoing cookies collection. 
                        Response.Cookies.Add(authCookie);
                      //  UpdateLoginDateTime(userName);

                      //  Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));
                     //   Session.Add("sRole", "admin"); //for Context menu

                      ///  Session.Add("sPermissionRole", "admin"); // for table Column


                       // csCommonUtility.WriteLog("Crew Authenticate end");

                    }
                    catch (Exception ex)
                    {
                        //csCommonUtility.WriteLog(" Crew FormsAuthenticationTicket error in Catch " + ex.Message);
                    }


                    if (rememberme.Checked == true)
                    {
                        Response.Cookies["username"].Value = userName;
                        Response.Cookies["pwd"].Value = txtPassword.Text.Trim();
                        Response.Cookies["username"].Expires = DateTime.Now.AddYears(20);
                        Response.Cookies["pwd"].Expires = DateTime.Now.AddYears(20);
                    }
                    else
                    {
                        Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["pwd"].Expires = DateTime.Now.AddDays(-1);

                    }

                    Response.Redirect("mlandingpage.aspx",false);
                }
            }
            else
            {
                user_info uinfo = new user_info();

                uinfo = _db.user_infos.Single(u => u.username == userName && u.password == password);

                userinfo obj = new userinfo();
                obj.first_name = uinfo.first_name;
                obj.last_name = uinfo.last_name;
                obj.address = uinfo.address;
                obj.city = uinfo.city;
                obj.state = uinfo.state;
                obj.zip = uinfo.zip;
                obj.phone = uinfo.phone;
                obj.fax = uinfo.fax;
                obj.email = uinfo.email;
                obj.role_id = Convert.ToInt32(uinfo.role_id);
                obj.is_active = Convert.ToBoolean(uinfo.is_active);
                obj.client_id = uinfo.client_id;
                obj.create_date = Convert.ToDateTime(uinfo.create_date);
                obj.username = uinfo.username;
                obj.sales_person_id = Convert.ToInt32(uinfo.sales_person_id);
                obj.is_verify = Convert.ToBoolean(uinfo.is_verify);
                obj.company_email = uinfo.company_email;
                obj.email_password = uinfo.email_password;
                obj.user_id = Convert.ToInt32(uinfo.user_id);
                obj.IsTimeClock = Convert.ToBoolean(uinfo.IsTimeClock);
                Session.Add("oUser", obj);

                Session.Add("sRole", "admin"); //for Context menu

                Session.Add("sPermissionRole", "admin"); // for table Column

                // Update Last Login by User
                uinfo.last_login_time = Convert.ToDateTime(DateTime.Now);
                _db.SubmitChanges();
                string strQ = "UPDATE sales_person SET last_login_time='" + uinfo.last_login_time + "' WHERE sales_person_id =" + obj.sales_person_id + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);

                // Create the authentication ticket
                FormsAuthenticationTicket authTicket = new
                  FormsAuthenticationTicket(1,  // version
                  userName,               // user name
                  DateTime.Now,                 // creation
                  DateTime.Now.AddMinutes(720),  // Expiration
                  false,                        // Persistent
                  role);                     // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie and add the encrypted ticket to the 
                // cookie as data.
                HttpCookie authCookie =
                  new HttpCookie(FormsAuthentication.FormsCookieName,
                  encryptedTicket);
                // Add the cookie to the outgoing cookies collection. 
                Response.Cookies.Add(authCookie);
                //UpdateLoginDateTime(userName);

                //Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));

                if (rememberme.Checked == true)
                {
                    Response.Cookies["username"].Value = userName;
                    Response.Cookies["pwd"].Value = txtPassword.Text.Trim();
                    Response.Cookies["username"].Expires = DateTime.Now.AddYears(20);
                    Response.Cookies["pwd"].Expires = DateTime.Now.AddYears(20);
                }

                else
                {

                    Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1);

                    Response.Cookies["pwd"].Expires = DateTime.Now.AddDays(-1);

                }

                Response.Redirect("mlandingpage.aspx");
            }
        }
        catch (Exception ex)
        {
            csCommonUtility.WriteLog( " Error in Catch "+ex.Message);
            throw ex;
        }

    }

    public string GetUserRoles(string sName, string sPassword, int nClientID)
    {
        string sRoles = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select m.* from Menu_item m " +
                    " right join role_right r on r.menu_id = m.menu_id " +
                    " right outer join user_info u on u.role_id = r.role_id " +
                    " WHERE u.is_active = 1 AND u.username ='" + sName + "' AND u.password ='" + sPassword + "' AND u.client_id =" + nClientID + " AND r.client_id = " + nClientID + " AND m.client_id = " + nClientID;

        List<menu_item> list = _db.ExecuteQuery<menu_item>(strQ, string.Empty).ToList();

        if (list.Count() > 0)
        {
            foreach (menu_item mi in list)
            {
                sRoles += "|" + mi.menu_code;
            }
        }
        return sRoles;
    }


    private string GetPrevliage(int nRoleId)
    {
        string sPrev = String.Empty;
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            var objMenus = (from mi in _db.menu_items
                            join rr in _db.role_rights on mi.menu_id equals rr.menu_id
                            where rr.role_id == nRoleId
                            select mi).ToList();


            foreach (menu_item mi in objMenus)
            {
                sPrev += mi.menu_code.ToString() + "|";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return sPrev;
    }
    protected void lnkForgotPassword_Click(object sender, EventArgs e)
    {
        lblResult.Text = "";

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            user_info objUser = new user_info();
            string userName = txtUserName.Text.Trim().ToLower();
            if (txtUserName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please enter the Username above and then click ''Forgot Password?''");
                return;
            }
            else
            {
                if (_db.user_infos.Where(u => u.is_active == Convert.ToBoolean(1) && u.username.ToLower() == userName).SingleOrDefault() == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid username.");
                    return;
                }
                else
                {
                    objUser = _db.user_infos.Single(u => u.is_active == Convert.ToBoolean(1) && u.username.ToLower() == userName);
                    Session.Add("mForgotPasword", objUser.user_id);
                    Response.Redirect("mforgotpassword.aspx");
                    //Response.Redirect("forgotpassword.aspx?eid=" + Convert.ToInt32(objUser.EmployeeID));
                }
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
}
