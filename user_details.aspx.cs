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
using System.Net;

public partial class user_details : System.Web.UI.Page
{
    string selectedDivisionValue = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin007") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            userinfo obj = (userinfo)Session["oUser"];
            int nuid = Convert.ToInt32(Request.QueryString.Get("uid"));
            hdnUserId.Value = nuid.ToString();

            BindStates();
            BindRoles();
            BindQuestions();
            BindDivision();
            

            lblEmailIntegration.Text = "Outlook/Exchange Email: ";
            lblEmailPassword.Text = "Outlook/Exchange Password: ";
            lblEmailPasswordCon.Text = "Outlook/Exchange Confirm Password: ";

            if (Convert.ToInt32(hdnUserId.Value) > 0)
            {
                lblHeaderTitle.Text = "User Details";

                DataClassesDataContext _db = new DataClassesDataContext();
                user_info uinfo = new user_info();
                uinfo = _db.user_infos.SingleOrDefault(c => c.user_id == Convert.ToInt32(hdnUserId.Value));
                if(uinfo != null)
                {
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
                    txtAnswer.Text = uinfo.Answer;
                    //Company Email 
                    //txtCompanyEmail.Text = uinfo.company_email;
                    txtEmailIntegration.Text = uinfo.company_email;
                    hdnEmailPassword.Value = uinfo.email_password;
                    // txtEmailPassword.Text = uinfo.email_password;

                    selectedDivisionValue = uinfo.client_id;

                    if (uinfo.client_id.Contains(','))
                    {
                        string[] ary = uinfo.client_id.Split(',');
                        foreach (ListItem item in lstDivision.Items)
                        {
                            foreach (var a in ary)
                            {
                                if (a == item.Value)
                                {
                                    item.Selected = true;                                    
                                }
                            }

                        }

                        BindDivisionRadioButton();
                        BindPirmaryDivision();
                    }
                    else
                    {
                        lstDivision.SelectedValue = uinfo.client_id;
                    }




                    


                    rdoPrimaryButton.SelectedValue = uinfo.primary_division.ToString();


                    if (uinfo.ViewPassword!=null && uinfo.ViewPassword!="")
                    {
                        txtPassword.Attributes.Add("value", uinfo.ViewPassword);
                        txtConfirmPass.Attributes.Add("value", uinfo.ViewPassword);
                    }
                    if (uinfo.email_password != null && uinfo.email_password != "")
                    {
                        txtEmailPassword.Attributes.Add("value", uinfo.email_password);
                        txtEmailPasswordCon.Attributes.Add("value", uinfo.email_password);
                    }

                    txtEMailSignature.Text = uinfo.EmailSignature.Replace("<br/>", "\n");


                    BindLandingPage(Convert.ToInt32(uinfo.role_id));

                    if (uinfo.menu_id != 0)
                        ddLandingPage.SelectedValue = uinfo.menu_id.ToString();

                    ddlRole.SelectedValue = uinfo.role_id.ToString();
                    chkIsActive.Checked = Convert.ToBoolean(uinfo.is_active);
                    chkIsSales.Checked = Convert.ToBoolean(uinfo.is_sales);
                    chkIsService.Checked = Convert.ToBoolean(uinfo.is_service);
                    chkIsInstall.Checked = Convert.ToBoolean(uinfo.is_install);
                    hdnSalesPersonId.Value = uinfo.sales_person_id.ToString();

                    if (Convert.ToInt32(uinfo.EmailIntegrationType) == 1)
                    {
                        chkEmailIntegrationType.Checked = true;

                        lblEmailIntegrationRequred.Visible = true;
                        lblEmailPasswordRequred.Visible = true;
                        lblEmailPasswordConRequred.Visible = true;
                    }
                    else
                    {
                        chkEmailIntegrationType.Checked = false;

                        lblEmailIntegrationRequred.Visible = false;
                        lblEmailPasswordRequred.Visible = false;
                        lblEmailPasswordConRequred.Visible = false;
                    }

                    if (uinfo.IsTimeClock == true)
                    {
                        chkIsTimeClock.Checked = true;
                    }
                    else
                    {
                        chkIsTimeClock.Checked = false;
                    }
                    if (uinfo.IsPriceChange == true)
                    {
                        IsPriceChange.Checked = true;
                    }
                    else
                    {
                        IsPriceChange.Checked = false;
                    }
                    chkIsSMS.Checked = Convert.ToBoolean(uinfo.IsEnableSMS);
                }

                if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    if (sp_info.com_per != null)
                        txtCom.Text = Convert.ToDecimal(sp_info.com_per).ToString();
                    if (sp_info.co_com_per != null)
                        txtCOCom.Text = Convert.ToDecimal(sp_info.co_com_per).ToString();



                }

                // FaztrackPagePermission
                SetUserPagePermission(Convert.ToInt32(hdnUserId.Value));
            }
            else
            {
                hdnUserId.Value = "0";
                hdnSalesPersonId.Value = "0";
                BindLandingPage(Convert.ToInt32(ddlRole.SelectedValue));

            }
            this.Validate();

            //BindDivisionRadioButton();

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "ddlRole", "chkIsActive", "chkIsSales", "chkIsTimeClock", "IsPriceChange", "chkIsSMS", "rdbEmailIntegrationType", "ddLandingPage" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdPagePermission_chkIsWrite" });
        }
    }

    private void BindRadioDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            rdoPrimaryButton.DataSource = dt;
            rdoPrimaryButton.DataTextField = "division_name";
            rdoPrimaryButton.DataValueField = "id";
            rdoPrimaryButton.DataBind();

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }


    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            lstDivision.DataSource = dt;
            lstDivision.DataTextField = "division_name";
            lstDivision.DataValueField = "id";
            lstDivision.DataBind();

        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }

    public void SetUserPagePermission(int uid)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = @"SELECT m.menu_id, m.menu_name, menu_url, CAST(ISNULL(p.IsWrite, 1) AS BIT) AS IsWrite 
                    FROM user_info AS u
                    INNER JOIN role_right AS r ON u.role_id = r.role_id
                    INNER JOIN menu_item AS m ON r.menu_id = m.menu_id
                    LEFT OUTER JOIN PagePermission AS p ON m.menu_id = p.menu_id AND u.user_id = p.user_id
                    WHERE m.parent_id != 0 AND u.user_id = " + uid + "";


        List<csUserPagePermission> itemList = _db.ExecuteQuery<csUserPagePermission>(strQ, string.Empty).ToList();


        grdPagePermission.DataSource = itemList;
        grdPagePermission.DataKeyNames = new string[] { "menu_id", "menu_url" };
        grdPagePermission.DataBind();

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
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }
    private void BindRoles()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var roles = from ro in _db.roles
                    select ro;
        ddlRole.DataSource = roles;
        ddlRole.DataTextField = "role_name";
        ddlRole.DataValueField = "role_id";
        ddlRole.DataBind();
    }

    private void BindLandingPage(int RoleId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var query = from mi in _db.menu_items
                    join rr in _db.role_rights on mi.menu_id equals rr.menu_id
                    where rr.role_id == RoleId && mi.parent_id != 0 && mi.menu_url != "#" && mi.menu_id != 67
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

        MasterDataClassesDataContext _mdb = new MasterDataClassesDataContext();
        MasterUserInfo muinfo = new MasterUserInfo();

        DataClassesDataContext _db = new DataClassesDataContext();
        user_info uinfo = new user_info();
        sales_person obj = new sales_person();

        int divisionCount = 0;
        string selectedvalue = "";
        string selectDivisionName = "";
        foreach (ListItem item in lstDivision.Items)
        {
            if (item.Selected)
            {
                selectedvalue += item.Value + ",";
                selectDivisionName += item.Text + ", ";
                divisionCount++;
            }
        }
        if(selectedvalue == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Division.");
            lblResult.Focus();
            return;
        }

        if (selectedDivisionValue.Contains(","))
        {
            if(rdoPrimaryButton.SelectedValue == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Primary Division.");
                return;
            }             
        }
        if(divisionCount > 1)
        {
            if (rdoPrimaryButton.SelectedValue == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Primary Division.");
                return;
            }
        }

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

        if (txtEmail.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: " + lblEmail.Text.Replace(":", ".") + "");
            return;
        }

        if (txtUsername.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: User Name.");
            return;
        }

        if (ddlQuestion.SelectedValue == "10")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please select a Password Verification Question.");
            return;
        }

        if (txtAnswer.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Answer.");
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

        if (Convert.ToInt32(hdnUserId.Value) > 0)
        {
            uinfo = _db.user_infos.Single(c => c.user_id == Convert.ToInt32(hdnUserId.Value));

            try
            {
                muinfo = _mdb.MasterUserInfos.SingleOrDefault(m => m.client_id == 2 && (m.email == uinfo.email && m.password == uinfo.password));

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        else
        {///////////////////////
            if (_db.user_infos.Where(sp => sp.email == txtEmail.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Email already exist. Please try another Email.");
                return;
            }

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
        //////////////////
        if (_db.user_infos.Where(sp => sp.email == txtEmail.Text && sp.user_id != Convert.ToInt32(hdnUserId.Value)).SingleOrDefault() != null)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Email already exist. Please try another Email.");
            return;
        }

        if (txtGoogleCalendarAccount.Text != "")
        {
            if (_db.user_infos.Where(sp => sp.google_calendar_account == txtGoogleCalendarAccount.Text && sp.user_id != Convert.ToInt32(hdnUserId.Value) && sp.is_active == true).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Calendar Email already exist. Please try another Calendar Email.");
                return;
            }
        }



        if (chkEmailIntegrationType.Checked)
        {
            if (txtEmailIntegration.Text != "")
            {
               if (Convert.ToInt32(hdnUserId.Value) == 0)
                {
                    if (_db.user_infos.Where(sp => sp.company_email == txtEmailIntegration.Text && sp.user_id != Convert.ToInt32(hdnUserId.Value)).FirstOrDefault() != null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage(lblEmailIntegration.Text.Replace(":", "").Trim() + " Email already exist. Please try another " + lblEmailIntegration.Text.Replace(":", "").Trim() + " Email.");
                        return;
                    }
                }

                if (_db.user_infos.Where(sp => sp.company_email == txtEmailIntegration.Text && sp.user_id == Convert.ToInt32(hdnUserId.Value)).SingleOrDefault() != null)
                {
                    if (txtEmailPassword.Text != "")
                    {
                        if (txtEmailPassword.Text != hdnEmailPassword.Value)
                        {
                            if (txtEmailPasswordCon.Text.Trim() == "")
                            {
                                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Outlook/Exchange Confirm Password.");
                                return;
                            }
                            if (txtEmailPassword.Text.Trim() != txtEmailPasswordCon.Text.Trim())
                            {
                                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please confirm Outlook/Exchange Password");
                                return;
                            }

                            if (EWSAPI.DoesUserExistInOutlookServer(txtEmailIntegration.Text, txtEmailPassword.Text.Trim()) == false)
                            {
                                lblResult.Text = csCommonUtility.GetSystemErrorMessage(lblEmailIntegration.Text.Replace(":", "").Trim() + " or Password does not match in the Outlook / Exchange server.");
                                return;
                            }

                        }
                    }
                }
                else // New outook entry
                {
                    if (txtEmailPassword.Text == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: " + lblEmailIntegration.Text.Replace(":", "").Trim() + " Password.");
                        return;
                    }
                    if (txtEmailPasswordCon.Text.Trim() == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Outlook/Exchange Confirm Password.");
                        return;
                    }
                    if (txtEmailPassword.Text.Trim() != txtEmailPasswordCon.Text.Trim())
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please confirm Outlook/Exchange Password");
                        return;
                    }

                    if (EWSAPI.DoesUserExistInOutlookServer(txtEmailIntegration.Text, txtEmailPassword.Text.Trim()) == false)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage(lblEmailIntegration.Text.Replace(":", "").Trim() + " does not exist in the Outlook / Exchange server.");
                        return;
                    }
                }


            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: " + lblEmailIntegration.Text.Replace(":", "").Trim() + ".");
                return;
            }
        }
        if (chkIsTimeClock.Checked == true)
        {
            uinfo.IsTimeClock = true;
        }
        else
        {
            uinfo.IsTimeClock = false;
        }
        if (IsPriceChange.Checked)
        {
            uinfo.IsPriceChange = true;
        }
        else
        {
            uinfo.IsPriceChange = false;
        }
        uinfo.IsEnableSMS = chkIsSMS.Checked;
        txtPhone.Text = csCommonUtility.GetPhoneFormat(txtPhone.Text.Trim());
        txtFax.Text = csCommonUtility.GetPhoneFormat(txtFax.Text.Trim());


        uinfo.division_name = selectDivisionName.Trim().TrimEnd(',');
        uinfo.client_id = selectedvalue.Trim().TrimEnd(',');
        
        

        if(divisionCount > 1)
        {
            uinfo.primary_division = Convert.ToInt32(rdoPrimaryButton.SelectedValue);
        }
        else
        {
            uinfo.primary_division = Convert.ToInt32(lstDivision.SelectedValue);
        }



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

        if (chkEmailIntegrationType.Checked)
            uinfo.EmailIntegrationType = 1; // Outlook = 1
        else
            uinfo.EmailIntegrationType = 2; // Others = 2

        uinfo.is_active = chkIsActive.Checked;
        uinfo.is_sales = chkIsSales.Checked;
        uinfo.is_service = chkIsService.Checked;
        uinfo.is_install = chkIsInstall.Checked;
        uinfo.username = txtUsername.Text;

        uinfo.company_email = txtEmailIntegration.Text.Trim(); // Intregretion Email
        if (txtEmailPassword.Text != "")
            uinfo.email_password = txtEmailPassword.Text; // Intregretion Email Password

        uinfo.google_calendar_account = txtGoogleCalendarAccount.Text;


        uinfo.role_id = Convert.ToInt32(ddlRole.SelectedValue);

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
        //obj.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        obj.client_id = selectedvalue.TrimEnd(',');
        obj.division_name = selectDivisionName.Trim().TrimEnd(',');


        if (divisionCount > 1)
        {
            obj.primary_division = Convert.ToInt32(rdoPrimaryButton.SelectedValue);
        }
        else
        {
            obj.primary_division = Convert.ToInt32(lstDivision.SelectedValue);
        }

        //if (selectedDivisionValue.Contains(","))
        //{
        //    obj.primary_division = Convert.ToInt32(rdoPrimaryButton.SelectedValue);
        //}
        //else
        //{
        //    obj.primary_division = Convert.ToInt32(lstDivision.SelectedValue);
        //}

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
        obj.is_active = chkIsActive.Checked;
        obj.is_sales = chkIsSales.Checked;
        obj.is_service = chkIsService.Checked;
        obj.is_install = chkIsInstall.Checked;
        obj.role_id = Convert.ToInt32(ddlRole.SelectedValue);
        obj.com_per = Convert.ToDecimal(txtCom.Text.Replace("%", "").Replace("$", ""));
        obj.co_com_per = Convert.ToDecimal(txtCOCom.Text.Replace("%", "").Replace("$", ""));
        //string strEmpC1 = string.Empty;
        //string strEmpC2 = string.Empty;
        //string strEmpC3 = string.Empty;
        //try
        //{
        //     strEmpC1 = txtFirstName.Text.Trim().Substring(0, 1) + "" + txtLastName.Text.Trim().Substring(0, 1);
        //     strEmpC2 = txtFirstName.Text.Trim().Substring(0, 2) + "" + txtLastName.Text.Trim().Substring(0, 1);
        //     strEmpC3 = txtFirstName.Text.Trim().Substring(0, 2) + "" + txtLastName.Text.Trim().Substring(0, 2);
        //}
        //catch
        //{

        //}


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
            if (chkIsSales.Checked || ddlRole.SelectedValue == "3")
            {
                if (Convert.ToInt32(hdnSalesPersonId.Value) == 0)
                {

                    int nSalePersonId = 0;
                    var result = (from sp in _db.sales_persons
                                  where sp.client_id == selectedvalue.TrimEnd(',')
                    select sp.sales_person_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nSalePersonId = result.Max();
                    nSalePersonId = nSalePersonId + 1;
                    hdnSalesPersonId.Value = nSalePersonId.ToString();
                    obj.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    obj.create_date = Convert.ToDateTime(DateTime.Now);
                    obj.last_login_time = Convert.ToDateTime(DateTime.Now);
                    obj.com_per = Convert.ToDecimal(txtCom.Text.Replace("%", "").Replace("$", ""));
                    obj.co_com_per = Convert.ToDecimal(txtCOCom.Text.Replace("%", "").Replace("$", ""));
                    _db.sales_persons.InsertOnSubmit(obj);
                    _db.SubmitChanges();
                    hdnSalesPersonId.Value = obj.sales_person_id.ToString();
                }
                else
                {
                    string strQ = "UPDATE sales_person SET first_name='" + obj.first_name + "',last_name='" + obj.last_name + "',address='" + obj.address + "',division_name='" + selectDivisionName.Trim().TrimEnd(',') + "',city='" + obj.city + "', state='" + obj.state + "', zip='" + obj.zip + "',phone='" + obj.phone + "',fax='" + obj.fax + "',email='" + obj.email + "', role_id=" + obj.role_id + ", is_active='" + obj.is_active + "',is_sales='" + obj.is_sales + "',is_service='" + obj.is_service + "',is_install='" + obj.is_install + "',client_id=" + selectedvalue.Trim().TrimEnd(',') + ", com_per =" + Convert.ToDecimal(txtCom.Text.Replace("%", "").Replace("$", "")) + ", co_com_per =" + Convert.ToDecimal(txtCOCom.Text.Replace("%", "").Replace("$", "")) + "  WHERE sales_person_id =" + obj.sales_person_id;
                    _db.ExecuteCommand(strQ, string.Empty);

                }
            }

            uinfo.tools = "Message,Vendor,Payment,JobStatus,Schedule,CompositeSow,ProjectNotes,AllowanceReport,ActivityLog,PreConCheckList,SiteReview,DocumentManagement,Selection,ProjectSummary";
            uinfo.google_calendar_id = "";
            uinfo.is_verify = false;
            uinfo.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            uinfo.create_date = Convert.ToDateTime(DateTime.Now);
            uinfo.last_login_time = Convert.ToDateTime(DateTime.Now);
            uinfo.ViewPassword = txtPassword.Text.Trim();
            uinfo.password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text.Trim(), "SHA1");
            // uinfo.sales_head_id = 0;
            _db.user_infos.InsertOnSubmit(uinfo);

          



            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");

        }
        else
        {
            if (chkIsSales.Checked || ddlRole.SelectedValue == "3")
            {
                if (Convert.ToInt32(hdnSalesPersonId.Value) == 0)
                {

                    int nSalePersonId = 0;
                    var result = (from sp in _db.sales_persons
                                  where sp.client_id == selectedvalue.TrimEnd(',')
                                  select sp.sales_person_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nSalePersonId = result.Max();
                    nSalePersonId = nSalePersonId + 1;
                    hdnSalesPersonId.Value = nSalePersonId.ToString();
                    obj.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    obj.create_date = Convert.ToDateTime(DateTime.Now);
                    obj.last_login_time = Convert.ToDateTime(DateTime.Now);
                    obj.com_per = Convert.ToDecimal(txtCom.Text.Replace("%", "").Replace("$", ""));
                    obj.co_com_per = Convert.ToDecimal(txtCOCom.Text.Replace("%", "").Replace("$", ""));
                    _db.sales_persons.InsertOnSubmit(obj);
                    _db.SubmitChanges();
                    hdnSalesPersonId.Value = obj.sales_person_id.ToString();
                }
                else
                {
                    string strQ = "UPDATE sales_person SET first_name='" + obj.first_name + "',last_name='" + obj.last_name + "',address='" + obj.address + "',division_name='" + selectDivisionName.Trim().TrimEnd(',') + "',city='" + obj.city + "', state='" + obj.state + "', zip='" + obj.zip + "',phone='" + obj.phone + "',fax='" + obj.fax + "',email='" + obj.email + "', role_id=" + obj.role_id + ", is_active='" + obj.is_active + "',is_sales='" + obj.is_sales + "',is_service='" + obj.is_service + "',is_install='" + obj.is_install + "',client_id= '" + selectedvalue.Trim().TrimEnd(',') + "' , com_per =" + Convert.ToDecimal(txtCom.Text.Replace("%", "").Replace("$", "")) + ",  co_com_per =" + Convert.ToDecimal(txtCOCom.Text.Replace("%", "").Replace("$", "")) + "   WHERE sales_person_id =" + obj.sales_person_id;
                    _db.ExecuteCommand(strQ, string.Empty);

                }
            }
            uinfo.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            if (txtPassword.Text.Trim() != "" && txtConfirmPass.Text.Trim() != "" && txtPassword.Text.Trim() == txtConfirmPass.Text.Trim())
            {
                uinfo.ViewPassword = txtPassword.Text.Trim();
                uinfo.password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text.Trim(), "SHA1");
            }
            if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            {
                string strQ = "UPDATE sales_person SET google_calendar_account='" + txtEmail.Text + "', is_active='" + obj.is_active + "' WHERE sales_person_id =" + obj.sales_person_id + " AND client_id= '" + selectedvalue.TrimEnd(',')+"'";
                _db.ExecuteCommand(strQ, string.Empty);
            }



            



            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");

        }

        _db.SubmitChanges();

        foreach (GridViewRow row in grdPagePermission.Rows)
        {
            PagePermission pp = new PagePermission();

            CheckBox chkIsWrite = (CheckBox)row.FindControl("chkIsWrite");
            int menuId = Convert.ToInt32(grdPagePermission.DataKeys[row.RowIndex].Values[0]);
            string menuUrl = grdPagePermission.DataKeys[row.RowIndex].Values[1].ToString();

            var item = _db.PagePermissions.Where(p => p.user_id == uinfo.user_id && p.menu_id == menuId);

            if (item.Any())
                pp = item.FirstOrDefault();

            pp.IsWrite = chkIsWrite.Checked;

            if (!item.Any())
            {
                pp.menu_id = menuId;
                pp.user_id = uinfo.user_id;
                //pp.client_id = (int)uinfo.client_id;
                pp.client_id = 1;
                pp.PageName = menuUrl;

                _db.PagePermissions.InsertOnSubmit(pp);
            }

            _db.SubmitChanges();
        }

        SetUserPagePermission(uinfo.user_id);


        try
        {
            // Master User Info Insert
            var objMUInfo = _mdb.MasterUserInfos.Where(m => m.client_id == 2 && m.email == uinfo.email);

            if (objMUInfo.Count() == 0 && Convert.ToInt32(hdnUserId.Value) == 0)
            {
                muinfo.client_id = 2; // Client 2 for IICEM
                muinfo.user_type = 1; // user type 1 for user
                muinfo.first_name = uinfo.first_name;
                muinfo.last_name = uinfo.last_name;
                muinfo.email = uinfo.email;
                muinfo.phone = csCommonUtility.ExtractNumber(uinfo.phone);
                muinfo.password = uinfo.password;
                muinfo.is_active = uinfo.is_active;
                muinfo.create_date = uinfo.create_date;
                muinfo.user_id = uinfo.user_id;
                _mdb.MasterUserInfos.InsertOnSubmit(muinfo);
                _mdb.SubmitChanges();
            }
            //----------------------------------   
            // Master User Info Update // Client 2 for IICEM
            if (muinfo != null && Convert.ToInt32(hdnUserId.Value) > 0)
            {
                muinfo.first_name = uinfo.first_name;
                muinfo.last_name = uinfo.last_name;
                muinfo.user_id = uinfo.user_id;
                muinfo.email = uinfo.email;
                muinfo.phone = csCommonUtility.ExtractNumber(uinfo.phone);
                muinfo.password = uinfo.password;
                muinfo.is_active = uinfo.is_active;
                _mdb.SubmitChanges();

            }
        }
        catch (Exception ex)
        {

             lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

      
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("user_management.aspx");
    }



    protected void chkEmailIntegrationType_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkEmailIntegrationType.ID, chkEmailIntegrationType.GetType().Name, "CheckedChanged"); 
        if (chkEmailIntegrationType.Checked)
        {
            lblEmailIntegrationRequred.Visible = true;
            lblEmailPasswordRequred.Visible = true;
            lblEmailPasswordConRequred.Visible = true;
        }
        else
        {
            lblEmailIntegrationRequred.Visible = false;
            lblEmailPasswordRequred.Visible = false;
            lblEmailPasswordConRequred.Visible = false;
        }
    }
    protected void lnkEmail_Click(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        user_info uinfo = new user_info();
        string strRequired = "";
        lblResult.Text = "";
        if (chkEmailIntegrationType.Checked)
        {
            if (txtEmailIntegration.Text != "")
            {
                //if (_db.user_infos.Where(sp => sp.company_email == txtEmailIntegration.Text && sp.user_id != Convert.ToInt32(hdnUserId.Value)).FirstOrDefault() != null)
                //{
                //    strRequired += lblEmailIntegration.Text.Replace(":", "").Trim() + " Email already exist. Please try another " + lblEmailIntegration.Text.Replace(":", "").Trim() + " Email.<br/>";

                //}

                if (_db.user_infos.Where(sp => sp.company_email == txtEmailIntegration.Text && sp.user_id == Convert.ToInt32(hdnUserId.Value)).SingleOrDefault() != null)
                {
                    if (txtEmailPassword.Text != "")
                    {
                        if (txtEmailPassword.Text != hdnEmailPassword.Value)
                        {
                            if (txtEmailPasswordCon.Text.Trim() == "")
                            {
                                strRequired += "Missing required field: Outlook/Exchange Confirm Password.<br/>";

                            }
                            if (txtEmailPassword.Text.Trim() != txtEmailPasswordCon.Text.Trim())
                            {
                                strRequired += "Please confirm Outlook/Exchange Password<br/>";

                            }

                            if (EWSAPI.DoesUserExistInOutlookServer(txtEmailIntegration.Text, txtEmailPassword.Text.Trim()) == false)
                            {
                                strRequired += lblEmailIntegration.Text.Replace(":", "").Trim() + " or Password does not match in the Outlook / Exchange server.<br/>";

                            }

                        }
                        else
                        {

                            if (EWSAPI.DoesUserExistInOutlookServer(txtEmailIntegration.Text, txtEmailPassword.Text.Trim()) == false)
                            {
                                strRequired += lblEmailIntegration.Text.Replace(":", "").Trim() + " or Password does not match in the Outlook / Exchange server.<br/>";

                            }
                            else
                            {
                                lblResult.Text = csCommonUtility.GetSystemMessage(" Outlook / Exchange  information is valide.");

                            }

                        }
                    }
                    else
                    {
                        strRequired += "Missing required field: Outlook/Exchange Password.<br/>";
                    }

                }
                else // New outook entry
                {
                    if (txtEmailPassword.Text == "")
                    {
                        strRequired += "Missing required field: " + lblEmailIntegration.Text.Replace(":", "").Trim() + " Password.<br/>";

                    }
                    if (txtEmailPasswordCon.Text.Trim() == "")
                    {
                        strRequired += "Missing required field: Outlook/Exchange Confirm Password.<br/>";

                    }
                    if (txtEmailPassword.Text.Trim() != txtEmailPasswordCon.Text.Trim())
                    {
                        strRequired += "Please confirm Outlook/Exchange Password<br/>";

                    }

                    if (EWSAPI.DoesUserExistInOutlookServer(txtEmailIntegration.Text, txtEmailPassword.Text.Trim()) == false)
                    {
                        strRequired += lblEmailIntegration.Text.Replace(":", "").Trim() + " does not exist in the Outlook / Exchange server.<br/>";

                    }
                }


            }
            else
            {
                strRequired += "Missing required field: " + lblEmailIntegration.Text.Replace(":", "").Trim() + ".<br/>";

            }
        }
        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
        }

    }

    protected void BindDivisionRadioButton()
    {

        try
        {
            string strQ = @"SELECT Id, division_name, status FROM division WHERE status = 1 AND Id in (" + selectedDivisionValue.TrimEnd(',') + ")" +                         
                         " ORDER BY id ";

            DataTable dt = csCommonUtility.GetDataTable(strQ);
            rdoPrimaryButton.DataSource = dt;
            rdoPrimaryButton.DataTextField = "division_name";
            rdoPrimaryButton.DataValueField = "Id";
            rdoPrimaryButton.DataBind();

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }


    }


    protected void lstDivision_SelectedIndexChanged(object sender, EventArgs e)
    {

        BindPirmaryDivision();

    }

    private void BindPirmaryDivision()
    {
        int selectedDivisionToCount = 0;

        string selectedvalue = "";
        foreach (ListItem item in lstDivision.Items)
        {
            if (item.Selected)
            {
                selectedDivisionToCount++;
                selectedvalue += item.Value + ",";
            }
        }

        if (selectedDivisionToCount > 1)
        {
            if (selectedvalue != "")
            {
                selectedDivisionValue = selectedvalue;
                BindDivisionRadioButton();
                pnlDivision.Visible = true;
            }
            else
            {
                pnlDivision.Visible = false;
                selectedDivisionValue = "";
            }
        }
        else
        {
            pnlDivision.Visible = false;
        }
    }



    protected void imgPasswordShow_Click(object sender, ImageClickEventArgs e)
    {
        imgPasswordShow.Visible = false;
        imgPasswordHide.Visible = true;


        DataClassesDataContext _db = new DataClassesDataContext();

        user_info objU = _db.user_infos.SingleOrDefault(c => c.user_id == Convert.ToInt32(hdnUserId.Value));
        if (objU != null)
        {
            txtEmailPassword.TextMode = TextBoxMode.SingleLine;
            txtEmailPasswordCon.TextMode = TextBoxMode.SingleLine;


        }
        else
        {
            txtEmailPassword.TextMode = TextBoxMode.SingleLine;
            txtEmailPasswordCon.TextMode = TextBoxMode.SingleLine;
        }
        lblResult.Text = "";
    }

    protected void imgPasswordHide_Click(object sender, ImageClickEventArgs e)
    {
        imgPasswordShow.Visible = true;
        imgPasswordHide.Visible = false;

        DataClassesDataContext _db = new DataClassesDataContext();
        user_info objU = _db.user_infos.SingleOrDefault(c => c.user_id == Convert.ToInt32(hdnUserId.Value));
        if (objU != null)
        {
            txtEmailPassword.TextMode = TextBoxMode.Password;
            txtEmailPasswordCon.TextMode = TextBoxMode.Password;
        }
        else
        {
            txtEmailPassword.TextMode = TextBoxMode.Password;
            txtEmailPasswordCon.TextMode = TextBoxMode.Password;

        }
        lblResult.Text = "";
    }
}
