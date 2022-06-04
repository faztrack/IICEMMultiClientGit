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

public partial class mcustomer_details : System.Web.UI.Page
{
    protected string custId = "0";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["oUser"] == null)
        {
            Response.Redirect("mobile.aspx");
        }
        if (Page.User.IsInRole("cus002") == false)
        {
            // No Permission Page.
           // Response.Redirect("mobile.aspx");
        }
        //if (!Request.Browser.IsMobileDevice)
        //{
        //    Response.Redirect("customer_details.aspx");
        //}
        if (Request.QueryString.Get("cid") != null)
        {
            hdnCustomerId.Value = Request.QueryString.Get("cid").ToString();
            lblHearder.Text = "Lead Details";
        }

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            DataClassesDataContext _db = new DataClassesDataContext();

            BindDivision();
            BindStates();
            BindSalesPerson();
            BindLeadSource();
            

            //int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            //hdnCustomerId.Value = ncid.ToString();
            custId = hdnCustomerId.Value;

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {


                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                

                txtFirstName1.Text = cust.first_name1;
                txtLastName1.Text = cust.last_name1;

                txtAddress.Text = cust.address;

                ddlDivision.SelectedValue = cust.client_id.ToString();

                

                string strAddress = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;

                txtCity.Text = cust.city;
              //  ddlState.SelectedValue = cust.state;
                txtZipCode.Text = cust.zip_code;
                txtPhone.Text = cust.phone;

                txtEmail.Text = cust.email;



              //  ddlStatus.SelectedValue = cust.status_id.ToString();
                //ddlSalesPerson.SelectedValue = cust.sales_person_id.ToString();

                if (cust.sales_person_id != null && cust.sales_person_id != 0)
                {
                    string strsale_person = "";
                    ListItem dditem = ddlSalesPerson.Items.FindByValue(cust.sales_person_id.ToString());
                    if (dditem != null)
                        this.ddlSalesPerson.Items.FindByValue(cust.sales_person_id.ToString()).Selected = true;
                    else
                    {
                        sales_person spinfo = _db.sales_persons.Single(u => u.sales_person_id == cust.sales_person_id);
                        if (spinfo != null)
                        {
                            strsale_person = spinfo.first_name + " " + spinfo.last_name;
                            ddlSalesPerson.Items.Insert(0, new ListItem(strsale_person, cust.sales_person_id.ToString()));
                        }
                    }
                }

                txtNotes.Text = cust.notes;

                //lblRegDate.Visible = true;
                //lblRegDateData.Visible = true;
                //lblRegDateData.Text = Convert.ToDateTime(cust.registration_date).ToShortDateString();
                ddlLeadSource.SelectedValue = cust.lead_source_id.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
              
            }
        }

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
    private void BindStates()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var states = from st in _db.states
                     orderby st.abbreviation
                     select st;
        //ddlState.DataSource = states;
        //ddlState.DataTextField = "abbreviation";
        //ddlState.DataValueField = "abbreviation";
        //ddlState.DataBind();
    }
    private void BindSalesPerson()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1  and is_sales=1 and sales_person.client_id in ('" + Convert.ToInt32(ddlDivision.SelectedValue) + "') order by sales_person_id asc";
        //List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
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
        Response.Redirect("mcustomerlist.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        if (txtFirstName1.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: First Name 1.";
            lblResult.ForeColor = Color.Red;
            return;
        }
        if (txtLastName1.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: Last Name 1.";
            lblResult.ForeColor = Color.Red;
            return;
        }
        if (txtAddress.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: Address.";
            lblResult.ForeColor = Color.Red;
            return;
        }
        if (txtCity.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: City.";
            lblResult.ForeColor = Color.Red;
            return;
        }

        if (txtZipCode.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: Zip Code.";
            lblResult.ForeColor = Color.Red;
            return;
        }
        if (txtEmail.Text.Trim() == "")
        {
            lblResult.Text = "Missing required field: Email.";
            lblResult.ForeColor = Color.Red;
            return;
        }
        

        if (ddlLeadSource.SelectedItem.Text == "Select Lead Source")
        {
            lblResult.Text = "Missing required field: Lead Source.";
            lblResult.ForeColor = Color.Red;
            ddlLeadSource.Focus();
            return;
        }

        customer cust = new customer();
        int nCount = GetCountCustomer();
        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

        cust.address = txtAddress.Text;
        cust.city = txtCity.Text;
        cust.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
        cust.company = "";
        cust.cross_street = "";
        cust.email2 = "";
        cust.fax ="";
        cust.islead = 1;
        cust.isCustomer = 0;
        cust.first_name1 = txtFirstName1.Text;
        cust.first_name2 = "";
        cust.is_active = Convert.ToBoolean(1);
        cust.last_name1 = txtLastName1.Text;
        cust.last_name2 = "";
        cust.phone = txtPhone.Text;
        cust.state = "AZ";//ddlState.SelectedItem.Text;
        cust.sales_person_id = Convert.ToInt32(ddlSalesPerson.SelectedValue);
        cust.zip_code = txtZipCode.Text;
        cust.update_date = Convert.ToDateTime(DateTime.Now);
        cust.status_id = 1;// Convert.ToInt32(ddlStatus.SelectedValue);
        //cust.appointment_date = Convert.ToDateTime(txtAppointmentDate.Text);
        DateTime dt = Convert.ToDateTime("1900-01-01");
        
        cust.appointment_date = dt;
        
        cust.notes = txtNotes.Text;
        cust.lead_source_id = Convert.ToInt32(ddlLeadSource.SelectedValue);
        cust.status_note = "";

        if (Convert.ToInt32(hdnCustomerId.Value) == 0)
        {
            cust.email = txtEmail.Text;
            cust.registration_date = Convert.ToDateTime(DateTime.Now);
            cust.SuperintendentId = 0;
            _db.customers.InsertOnSubmit(cust);
            _db.SubmitChanges();
            lblResult.Text = "Customer '" + txtLastName1.Text + "' has been saved successfully.";
            lblResult.ForeColor = System.Drawing.Color.Green;
            hdnCustomerId.Value = cust.customer_id.ToString();
            Session.Add("CustomerId", cust.customer_id);
        }
        else
        {
            cust.email = txtEmail.Text;
            cust.update_date = DateTime.Now;
            _db.SubmitChanges();
            lblResult.Text = "Customer '" + txtLastName1.Text + "' has been updated successfully.";
            lblResult.ForeColor = System.Drawing.Color.Green;
            Session.Add("CustomerId", hdnCustomerId.Value);
        }
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
        txtFirstName1.Text = "";
        txtLastName1.Text = "";
    
        txtAddress.Text = "";
      
        txtCity.Text = "";
        BindStates();
        txtZipCode.Text = "";
        txtPhone.Text = "";
       
        txtEmail.Text = "";
      
        //ddlStatus.Enabled = true;
        //ddlStatus.SelectedValue = "1";
        ddlSalesPerson.SelectedValue = "0";
        txtNotes.Text = "";
        hdnCustomerId.Value = "0";
        //lblRegDate.Visible = false;
        //lblRegDateData.Visible = false;
      

        BindLeadSource();
    }
    
    private DataTable LoadtmpTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("file_name", typeof(string));
        table.Columns.Add("file_description", typeof(string));

        return table;
    }



    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mcustomerlist.aspx");
    }

  
    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSalesPerson();
    }
}