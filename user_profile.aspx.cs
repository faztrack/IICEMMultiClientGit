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
using System.Collections.Generic;
using System.Drawing;
using Prabhu;

public partial class user_profile : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin041") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            userinfo obj = (userinfo)Session["oUser"];
            int nuid = obj.user_id;
            hdnUserId.Value = nuid.ToString();

            BindStates();
          
            BindQuestions();

            if (Convert.ToInt32(hdnUserId.Value) > 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                user_info uinfo = new user_info();
                uinfo = _db.user_infos.Single(c => c.user_id == Convert.ToInt32(hdnUserId.Value));
                lblChangePassword.Visible = true;
                lblPasswordRequ.Visible = false;
                lblReTypePasswordReq.Visible = false;
                txtFirstName.Text = uinfo.first_name;
                txtLastName.Text = uinfo.last_name;
                txtAddress.Text = uinfo.address;
                txtCity.Text = uinfo.city;
                ddlState.SelectedItem.Text = uinfo.state;
                txtZip.Text = uinfo.zip;
                txtPhone.Text = uinfo.phone;
                txtFax.Text = uinfo.fax;
                txtEmail.Text = uinfo.email;
                txtGoogleCalendarAccount.Text = uinfo.google_calendar_account;
                txtUsername.Text = uinfo.username;
                ddlQuestion.SelectedValue = uinfo.QuestionID.ToString();
                txtAnswer.Text = uinfo.Answer.Trim();
                hdnEmailPassword.Value = uinfo.email_password;

                txtEMailSignature.Text = uinfo.EmailSignature.Replace("<br/>", "\n");

                hdnSalesPersonId.Value = uinfo.sales_person_id.ToString();

                BindLandingPage(Convert.ToInt32(uinfo.role_id));

                if (uinfo.menu_id != 0)
                    ddLandingPage.SelectedValue = uinfo.menu_id.ToString();
            }
            else
            {
                hdnUserId.Value = "0";
                hdnSalesPersonId.Value = "0";
            }
            this.Validate();
        }
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

    private void BindQuestions()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            var item = from q in _db.Questions
                       select q;
            ddlQuestion.DataSource = item.ToList();
            ddlQuestion.DataTextField = "QuestionName";
            ddlQuestion.DataValueField = "QuestionID";
            ddlQuestion.DataBind();
            ddlQuestion.SelectedValue = "10";
        }
        catch (Exception ex)
        {

        }
    }
    private void BindLandingPage(int RoleId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var query = from mi in _db.menu_items
                    join rr in _db.role_rights on mi.menu_id equals rr.menu_id
                    where rr.role_id == RoleId && mi.parent_id != 0
                    orderby mi.menu_name
                    select new { MenuName = mi.menu_name, MenuId = mi.menu_id };
        ddLandingPage.DataSource = query;
        ddLandingPage.DataTextField = "MenuName";
        ddLandingPage.DataValueField = "MenuId";
        ddLandingPage.DataBind();
        ddLandingPage.Items.Insert(0, "Select landing page");
        ddLandingPage.SelectedIndex = 0;


    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (txtFirstName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: First Name.");
            return;
        }

        if (txtLastName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Last Name.");
            return;
        }

        if (txtUsername.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: User Name.");
            return;
        }
        if (txtEmail.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Email.");
            return;
        }

        if (txtPassword.Text.Trim() != "")
        {
            if (txtPassword.Text.Trim().Length < 6)
            {
                lblResult.Visible = true;
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Password length should be minimum 6");
                return;
            }

            if (txtConfirmPass.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Confirm Password.");

                return;
            }
            if (txtPassword.Text.Trim() != txtConfirmPass.Text.Trim())
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please confirm password");
                return;
            }
        }

        if (ddlQuestion.SelectedValue == "10")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please select a Password Verification Question.");
            return;
        }
        

        DataClassesDataContext _db = new DataClassesDataContext();
        user_info uinfo = new user_info();

        sales_person obj = new sales_person();


        if (Convert.ToInt32(hdnUserId.Value) > 0)
            uinfo = _db.user_infos.Single(c => c.user_id == Convert.ToInt32(hdnUserId.Value));
        else
        {
            if (_db.user_infos.Where(sp => sp.username == txtUsername.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Username already exist. Please try another Username.");
                return;
            }
            if (txtGoogleCalendarAccount.Text != "")
            {
                if (_db.user_infos.Where(sp => sp.google_calendar_account == txtGoogleCalendarAccount.Text).SingleOrDefault() != null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Calendar Email already exist. Please try another Calendar Email.");

                    return;
                }
            }
        }
        if (txtGoogleCalendarAccount.Text != "")
        {
            if (_db.user_infos.Where(sp => sp.google_calendar_account == txtGoogleCalendarAccount.Text && sp.user_id != Convert.ToInt32(hdnUserId.Value)).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Calendar Email already exist. Please try another Calendar Email.");

                return;
            }
        }


        uinfo.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        uinfo.user_id = Convert.ToInt32(hdnUserId.Value);

        uinfo.first_name = txtFirstName.Text;
        uinfo.last_name = txtLastName.Text;
        uinfo.address = txtAddress.Text;
        uinfo.city = txtCity.Text;
        uinfo.state = ddlState.SelectedItem.Text;
        uinfo.zip = txtZip.Text;
        uinfo.phone = txtPhone.Text;
        uinfo.fax = txtFax.Text;
        uinfo.email = txtEmail.Text;  //txtCompanyEmail.Text;
       
        uinfo.username = txtUsername.Text;
        uinfo.company_email = txtEmail.Text; // txtCompanyEmail.Text;

        uinfo.google_calendar_account = txtGoogleCalendarAccount.Text;       

        uinfo.QuestionID = Convert.ToInt32(ddlQuestion.SelectedValue);
        uinfo.Answer = txtAnswer.Text.Trim();

        uinfo.EmailSignature = txtEMailSignature.Text.Replace("\n", "<br/>");

        if (ddLandingPage.SelectedValue == "Select landing page")
        {
            uinfo.menu_id = 0;
        }
        else
        {
            uinfo.menu_id = Convert.ToInt32(ddLandingPage.SelectedValue);
        }

        // Sales person Info
        obj.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        obj.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);

        obj.first_name = txtFirstName.Text;
        obj.last_name = txtLastName.Text;
        obj.address = txtAddress.Text;
        obj.city = txtCity.Text;
        obj.state = ddlState.SelectedItem.Text;
        obj.zip = txtZip.Text;
        obj.phone = txtPhone.Text;
        obj.fax = txtFax.Text;
        obj.email = txtEmail.Text;  //txtCompanyEmail.Text;
        obj.google_calendar_account = txtGoogleCalendarAccount.Text;       


        if (Convert.ToInt32(hdnUserId.Value) == 0)
        {
            if (txtPassword.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Password.");
                return;
            }
            else
            {
                if (txtPassword.Text.Trim().Length < 6)
                {
                    lblResult.Visible = true;
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Password length should be minimum 6");
                    return;
                }
            }
            if (txtConfirmPass.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Confirm Password.");
                return;
            }
            if (txtPassword.Text.Trim() != txtConfirmPass.Text.Trim())
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please confirm password");
                return;
            }
            
            uinfo.email_password = txtPassword.Text; //txtEmailPassword.Text;
            uinfo.is_verify = false;
            uinfo.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            uinfo.create_date = Convert.ToDateTime(DateTime.Now);
            uinfo.last_login_time = Convert.ToDateTime(DateTime.Now);
            uinfo.password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text.Trim(), "SHA1");
            
            _db.user_infos.InsertOnSubmit(uinfo);
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");

        }
        else
        {
            
            uinfo.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            if (txtPassword.Text.Trim() != "" && txtConfirmPass.Text.Trim() != "" && txtPassword.Text.Trim() == txtConfirmPass.Text.Trim())
            {
                uinfo.password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text.Trim(), "SHA1");
            }
            if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            {
                string strQ = "UPDATE sales_person SET google_calendar_account='" + txtEmail.Text + "', is_active='" + obj.is_active + "' WHERE sales_person_id =" + obj.sales_person_id + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
            }
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");

        }

        _db.SubmitChanges();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("user_management.aspx");
    }
    
}
