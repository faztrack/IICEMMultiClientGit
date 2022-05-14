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

public partial class muserprofile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (HttpContext.Current.User != null)
        //{
        //    if (HttpContext.Current.User.Identity.IsAuthenticated == false)
        //    {
        //        Server.Transfer(ConfigurationManager.AppSettings["LoginPage"].ToString());
        //    }
        //}
        //else
        //{
        //    Server.Transfer(ConfigurationManager.AppSettings["LoginPage"].ToString());
        //}

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
           
            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }

            if (Session["oCrew"] != null)
            {
                Response.Redirect("mlandingpage.aspx");
            }
            BindStates();
            BindQuestions();
            GetUserDetails();
        }
    }

    private void GetUserDetails()
    {

        userinfo obj = (userinfo)Session["oUser"];

        if (obj.user_id != 0)
        {
            hdnUserId.Value = obj.user_id.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            user_info uinfo = new user_info();
            uinfo = _db.user_infos.Single(c => c.user_id == obj.user_id);

            txtFirstName.Text = uinfo.first_name;
            txtLastName.Text = uinfo.last_name;
            txtAddress.Text = uinfo.address;
            txtCity.Text = uinfo.city;
            ddlState.SelectedItem.Text = uinfo.state;
            txtZip.Text = uinfo.zip;
            txtPhone.Text = uinfo.phone;
            // txtFax.Text = uinfo.fax;
            txtEmail.Text = uinfo.email;

            txtUsername.Text = uinfo.username;
            ddlQuestion.SelectedValue = uinfo.QuestionID.ToString();
            txtAnswer.Text = uinfo.Answer;
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
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        string strRequired = string.Empty;
        DataClassesDataContext _db = new DataClassesDataContext();
        user_info uinfo = new user_info();
        try
        {
            if (Convert.ToInt32(hdnUserId.Value) > 0)
            {
                if (txtFirstName.Text.Trim() == "")
                {
                    strRequired += "Missing required field: First Name.<br/>";

                }

                if (txtLastName.Text.Trim() == "")
                {
                    strRequired += "Missing required field: Last Name.<br/>";

                }

                if (txtEmail.Text.Trim() == "")
                {
                    strRequired += "Missing required field:  Email<br/>";

                }

                if (txtUsername.Text.Trim() == "")
                {
                    strRequired += "Missing required field: User Name.<br/>";

                }

                if (ddlQuestion.SelectedValue == "10")
                {
                    strRequired += "Please select a Password Verification Question.<br/>";

                }

                if (txtAnswer.Text.Trim() == "")
                {
                    strRequired += "Missing Answer.<br/>";

                }

                if (txtPassword.Text.Trim() != "")
                {
                    if (txtPassword.Text.Trim().Length < 6)
                    {

                        strRequired += "Password length should be minimum 6.<br/>";

                    }

                    if (txtConfirmPass.Text.Trim() == "")
                    {
                        strRequired += "Missing required field: Confirm Password.<br/>";


                    }
                    if (txtPassword.Text.Trim() != txtConfirmPass.Text.Trim())
                    {
                        strRequired += "Please confirm password<br/>";

                    }
                }

                if (strRequired.Length > 0)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                    return;
                }

                if (Convert.ToInt32(hdnUserId.Value) > 0)
                    uinfo = _db.user_infos.Single(c => c.user_id == Convert.ToInt32(hdnUserId.Value));

                txtPhone.Text = csCommonUtility.GetPhoneFormat(txtPhone.Text.Trim());
                uinfo.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                uinfo.user_id = Convert.ToInt32(hdnUserId.Value);

                uinfo.first_name = txtFirstName.Text;
                uinfo.last_name = txtLastName.Text;
                uinfo.address = txtAddress.Text;
                uinfo.city = txtCity.Text;
                uinfo.state = ddlState.SelectedItem.Text;
                uinfo.zip = txtZip.Text;
                uinfo.phone = txtPhone.Text;
                uinfo.email = txtEmail.Text;  //txtCompanyEmail.Text;
                uinfo.username = txtUsername.Text;
                if (txtPassword.Text.Trim() != "")
                {
                    uinfo.password = txtPassword.Text.Trim();
                }
                uinfo.QuestionID = Convert.ToInt32(ddlQuestion.SelectedValue);
                uinfo.Answer = txtAnswer.Text.Trim();
                _db.SubmitChanges();
                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
            }
        }

        catch (Exception ex)
        {

        }
        
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
}
