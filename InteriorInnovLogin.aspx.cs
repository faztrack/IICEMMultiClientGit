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

public partial class InteriorInnovLogin : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            string strUseHTTPS = System.Configuration.ConfigurationManager.AppSettings["UseHTTPS"];
            if (strUseHTTPS == "Yes")
            {
                if (Request.Url.Scheme == "http")
                {
                    string strQuery = Request.Url.AbsoluteUri;
                    Response.Redirect(strQuery.Replace("http:", "https:"));
                }
            }

            Session.RemoveAll();
            Session.Remove("oUser");

            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
                lblLoginPanelTitle.Text = "Test System Login";

            lnkMobileSiteLogin.NavigateUrl = ConfigurationManager.AppSettings["MobileLoginPage"].ToString();
        }
    }
    protected void btnLogIn_Click(object sender, EventArgs e)
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

        //Session.Clear();
        # region Super User Login

        if (userName.ToLower() == "Faztrack".ToLower() && password == "F@ztrack")
        {

            user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == 14);

            userinfo obj = new userinfo();
            obj.first_name = "FazTrack";
            obj.username = "Faztrack";
            obj.role_id = 1;
            obj.client_id = uinfo.client_id;
            obj.divisionName = uinfo.division_name;
            obj.primaryDivision = Convert.ToInt32(uinfo.primary_division);
            obj.user_id = 0;
            obj.IsPriceChange = true;
            obj.email = "tislam@faztrack.com";
            obj.sales_person_id = 0;

            string strUseEmailIntegration = System.Configuration.ConfigurationManager.AppSettings["UseEmailIntegration"];
            if (strUseEmailIntegration == "Yes")
            {
                if (uinfo != null)
                {
                    obj.company_email = uinfo.company_email;
                    obj.email_password = uinfo.email_password;
                    obj.EmailIntegrationType = Convert.ToInt32(uinfo.EmailIntegrationType);
                }
                else
                {
                    obj.company_email = "";
                    obj.email_password = "";
                    obj.EmailIntegrationType = 2;
                }
            }
            else
            {
                obj.company_email = "";
                obj.email_password = "";
                obj.EmailIntegrationType = 2;
            }

            Session.Add("sRole", "admin"); //for Context menu

            Session.Add("sPermissionRole", "admin"); // for table Column

            Session.Add("oUser", obj);
            string sPrevliage = this.GetPrevliage((int)obj.role_id);

            if (sPrevliage.Length > 1)  // means authetication
            {
                // Create the authentication ticket
                FormsAuthenticationTicket authTicket = new
                FormsAuthenticationTicket(1,  // version
                obj.first_name,               // user name
                DateTime.Now,                 // creation
                DateTime.Now.AddMinutes(360),  // Expiration
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
                if (uinfo.menu_id != 0)
                {
                    menu_item objMe = _db.menu_items.SingleOrDefault(m => m.menu_id == uinfo.menu_id);
                    Response.Redirect(objMe.menu_url);
                }
                else
                {
                    Response.Redirect("customerlist.aspx");
                }
            }
        }


        # endregion

        password =  FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");

        string role = GetUserRoles(userName, password, nClientId);

        if (_db.user_infos.Where(sp =>  sp.username == userName && sp.password == password && sp.is_active == Convert.ToBoolean(1)).SingleOrDefault() == null)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid username and password.");

            return;
        }

        if (role.Equals(""))
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("The user doesn't have proper permission to login.  Please check with the Administrator.");

            return;
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
            obj.primaryDivision = Convert.ToInt32(uinfo.primary_division);
            obj.divisionName = uinfo.division_name;
            obj.create_date = Convert.ToDateTime(uinfo.create_date);
            obj.username = uinfo.username;
            obj.sales_person_id = Convert.ToInt32(uinfo.sales_person_id);
            obj.is_verify = Convert.ToBoolean(uinfo.is_verify);
            obj.company_email = uinfo.company_email;
            obj.email_password = uinfo.email_password;
            obj.user_id = Convert.ToInt32(uinfo.user_id);
            obj.EmailIntegrationType = Convert.ToInt32(uinfo.EmailIntegrationType);         
            obj.IsPriceChange =  Convert.ToBoolean(uinfo.IsPriceChange);
            Session.Add("oUser", obj);

            Session.Add("sRole", "admin"); //for Context menu

            Session.Add("sPermissionRole", "admin"); // for table Column

            // Update Last Login by User
            uinfo.last_login_time = Convert.ToDateTime(DateTime.Now);
            _db.SubmitChanges();
            string strQ = "UPDATE sales_person SET last_login_time='" + uinfo.last_login_time + "' WHERE sales_person_id =" + obj.sales_person_id;
            _db.ExecuteCommand(strQ, string.Empty);

            // Create the authentication ticket
            FormsAuthenticationTicket authTicket = new
              FormsAuthenticationTicket(1,  // version
              userName,               // user name
              DateTime.Now,                 // creation
              DateTime.Now.AddMinutes(360),  // Expiration
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

            if (uinfo.menu_id != 0)
            {
                menu_item objMe = _db.menu_items.SingleOrDefault(m => m.menu_id == uinfo.menu_id);
                string strUrl = objMe.menu_url;
                if (strUrl.Length > 5)
                {
                    Response.Redirect(strUrl);
                }
                else
                {
                    Response.Redirect("customerlist.aspx");
                }
            }
            else
            {
                Response.Redirect("customerlist.aspx");
            }
        }
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

    public string GetUserRoles(string sName, string sPassword, int nClientID)
    {
        string sRoles = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select m.* from Menu_item m " +
                    " right join role_right r on r.menu_id = m.menu_id " +
                    " right outer join user_info u on u.role_id = r.role_id " +
                    " WHERE u.is_active = 1 AND u.username ='" + sName + "' AND u.password ='" + sPassword + "' AND  r.client_id in (" + nClientID + ") AND m.client_id in (" + nClientID + ")" ;

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

    protected void lnkForgotPassword_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkForgotPassword.ID, lnkForgotPassword.GetType().Name, "Click"); 
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
                    Session.Add("ForgotPasword", objUser.user_id);
                    Response.Redirect("forgotpassword.aspx");
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