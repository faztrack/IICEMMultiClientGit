using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

using System.Net.Mail;

public partial class customerlogin : System.Web.UI.Page
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
            Session.Remove("oCustomerUser");

            int nCustomerId = 0;
            if (Request.QueryString.Get("cid") != null)
            {
                nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                if (Request.QueryString.Get("aType") != null)
                {
                    DataClassesDataContext _db = new DataClassesDataContext();
                    int adminType = Convert.ToInt32(Request.QueryString.Get("aType"));
                    if (_db.customer_estimates.Where(ce => ce.customer_id == nCustomerId && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
                    {
                        var customeruserinfo = _db.customeruserinfos.Where(cu => cu.isactive == 1 && cu.customerid == nCustomerId).FirstOrDefault();
                        if (customeruserinfo != null)
                        {
                            Session.Add("oCustomerUser", customeruserinfo);
                            Response.Redirect("customerdashboard.aspx");
                        }
                    }

                }
                else
                {
                    IsCustomerUserExist(nCustomerId);
                }
            }

        }
    }


    public string IsCustomerUserExist(int nCustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strUserStatus = "";
        if (_db.customer_estimates.Where(ce => ce.customer_id == nCustomerId && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
        {
            var customeruserinfo = _db.customeruserinfos.Where(cu => cu.isactive == 1 && cu.customerid == nCustomerId).FirstOrDefault();
            var customer = _db.customers.Where(cu => cu.is_active == true && cu.customer_id == nCustomerId).FirstOrDefault();

            if (customeruserinfo == null)
            {
                txtUserName.Text = customer.email ?? "";
                pReTypePassword.Visible = true;

                strUserStatus = "New";
            }
            else
            {
                txtUserName.Text = customeruserinfo.customerusername ?? "";
                strUserStatus = "Exist";
            }
            hdnCustomerId.Value = nCustomerId.ToString();
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("This Portal is for Sold projects only.<br/>Please contact Arizona's Interior Innovations for more information.");
            strUserStatus = "NotSold";
        }

        return strUserStatus;
    }

    protected void btnLogIn_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnLogIn.ID, btnLogIn.GetType().Name, "Click"); 
        lblResult.Text = "";

        int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);

        DataClassesDataContext _db = new DataClassesDataContext();

        string userName = txtUserName.Text.Trim();
        string password = txtPassword.Text.Trim();
        string retypepassword = txtConfirmPassword.Text.Trim();

        if (nCustomerId != 0) // Redirect From Contract PDF
        {
            if (IsCustomerUserExist(nCustomerId) == "New")
            {
                string strRequired = "";
                customeruserinfo objcu = new customeruserinfo();
                var objCust = _db.customers.Where(cu => cu.is_active == true && cu.customer_id == nCustomerId).SingleOrDefault();

                if (_db.customeruserinfos.Where(c => c.customerusername == userName).SingleOrDefault() != null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("User already exist. Please try another user name.");
                }
                else
                {

                    if (txtUserName.Text.Trim() == "")
                    {
                        strRequired = "Missing required field: User.<br/>";
                    }

                    if (txtPassword.Text.Trim() == "")
                    {
                        strRequired += "Missing required field: Password.<br/>";

                    }
                    else
                    {
                        if (Convert.ToInt32(txtPassword.Text.Length) < 6)
                        {
                            strRequired += "Password length should be minimum 6.<br/>";

                        }
                        if (txtConfirmPassword.Text.Trim() == "")
                        {
                            strRequired += "Missing required file: Confirm Password.<br/>";

                        }
                        if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
                        {
                            strRequired += "Please confirm password.";

                        }
                    }


                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        return;
                    }
                    else
                    {
                        objcu.customerid = objCust.customer_id;
                        objcu.customerusername = userName;
                        objcu.customerpassword = txtPassword.Text.Trim();
                        objcu.isactive = 1;
                        _db.customeruserinfos.InsertOnSubmit(objcu);
                        _db.SubmitChanges();

                        Session.Add("oCustomerUser", objcu);
                        if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("eid") != null && Request.QueryString.Get("coid") != null && Request.QueryString.Get("coestid") != null)
                        {
                            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
                            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
                            int nChEstid = Convert.ToInt32(Request.QueryString.Get("coid"));
                            int nCOEstId = Convert.ToInt32(Request.QueryString.Get("coestid"));

                            Response.Redirect("customerchangeorder.aspx?coid=" + nChEstid + "&eid=" + nEstimateId + "&cid=" + nCid + "&coestid=" + nCOEstId);
                        }
                        else
                        {
                            Response.Redirect("customerdashboard.aspx");
                        }
                    }
                }

            }
            else if (IsCustomerUserExist(nCustomerId) == "Exist")
            {
                var customeruserinfo = _db.customeruserinfos.Where(cu => cu.isactive == 1 && cu.customerusername == userName && cu.customerpassword == password).SingleOrDefault();
                if (customeruserinfo == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid username and password.");

                    return;
                }
                else
                {
                    Session.Add("oCustomerUser", customeruserinfo);
                    if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("eid") != null && Request.QueryString.Get("coid") != null && Request.QueryString.Get("coestid") != null)
                    {
                        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
                        int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
                        int nChEstid = Convert.ToInt32(Request.QueryString.Get("coid"));
                        int nCOEstId = Convert.ToInt32(Request.QueryString.Get("coestid"));

                        Response.Redirect("customerchangeorder.aspx?coid=" + nChEstid + "&eid=" + nEstimateId + "&cid=" + nCid + "&coestid=" + nCOEstId);
                    }
                    else if (Request.QueryString.Get("ssfn") != null)
                    {
                        Response.Redirect("CustomerSelectionInfoSec.aspx");
                    }
                    else
                    {
                        Response.Redirect("customerdashboard.aspx");
                    }
                }
            }
            else if (IsCustomerUserExist(nCustomerId) == "NotSold")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("This Portal is for Sold projects only.<br/>Please contact Arizona's Interior Innovations for more information.");
                return;
            }
        }
        else // Normal Login
        {
            var customeruserinfo = _db.customeruserinfos.Where(cu => cu.isactive == 1 && cu.customerusername == userName && cu.customerpassword == password).SingleOrDefault();
            
            if (customeruserinfo == null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid username and password.");

                return;
            }
            else
            {
                nCustomerId = Convert.ToInt32(customeruserinfo.customerid);

                if (_db.customer_estimates.Where(ce => ce.customer_id == nCustomerId && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
                {
                    Session.Add("oCustomerUser", customeruserinfo);
                    if (Request.QueryString.Get("sc")!=null)
                    {
                        Response.Redirect("customerschedulecalendar.aspx");
                    }
                    else
                    {
                       Response.Redirect("customerdashboard.aspx");
                    }

                    Session.Add("sRole", "customer"); //for Context menu
                    Session.Add("sPermissionRole", "customer"); // for table Column
                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("This Portal is for Sold projects only.<br/>Please contact Arizona's Interior Innovations for more information.");
                }
            }


        }

    }
}
