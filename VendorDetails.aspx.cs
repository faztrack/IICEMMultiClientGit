using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class VendorDetails : System.Web.UI.Page
{
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
          

            int nvid = Convert.ToInt32(Request.QueryString.Get("vid"));
            hdnVendorId.Value = nvid.ToString();

            BindSections();
            BindVendorSection();
            GetVendorDetails(Convert.ToInt32(hdnVendorId.Value));
            if (Convert.ToInt32(hdnVendorId.Value) > 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                Vendor objVendor = new Vendor();
                objVendor = _db.Vendors.Single(c => c.vendor_id == Convert.ToInt32(hdnVendorId.Value));

                txtVendorName.Text = objVendor.vendor_name;
                txtAddress.Text = objVendor.Address;
                txtCity.Text = objVendor.city;
                ddlState.SelectedItem.Text = objVendor.state;
                txtZip.Text = objVendor.zip_code;
                txtPhone.Text = objVendor.phone;
                txtFax.Text = objVendor.fax;
                txtEmailAddress.Text = objVendor.email;

                if (_db.vendor_sections.Any(v => v.Vendor_Id == Convert.ToInt32(hdnVendorId.Value)))
                {
                    var item = _db.vendor_sections.Where(v => v.Vendor_Id == Convert.ToInt32(hdnVendorId.Value)).ToList();
                    foreach (ListItem li in chkSections.Items)
                    {
                        foreach (vendor_section vSec in item)
                        {
                            if (vSec.section_id == Convert.ToInt32(li.Value.ToString()))
                                li.Selected = true;
                        }
                    }
                }
            }

            this.Validate();


            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkIsActive", "chkSections", "btnSubmit", "btnSaveVD" });
        }

    }
    private void BindSections()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        chkSections.DataSource = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.parent_id == 0).OrderBy(s => s.section_name).ToList();
        chkSections.DataTextField = "section_name";
        chkSections.DataValueField = "section_id";
        chkSections.DataBind();
    }
    private void BindVendorSection()
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
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblResultContact.Text = "";
        if (txtVendorName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Vendor Name.");

            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        Vendor objVendor = new Vendor();

        if (Convert.ToInt32(hdnVendorId.Value) > 0)
            objVendor = _db.Vendors.Single(c => c.vendor_id == Convert.ToInt32(hdnVendorId.Value));
        else
            if (_db.Vendors.Where(v => v.vendor_name == txtVendorName.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Vendor Name already exist. Please try another Vendor Name.");

                return;
            }

        objVendor.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        objVendor.vendor_id = Convert.ToInt32(hdnVendorId.Value);
        objVendor.vendor_name = txtVendorName.Text;
        objVendor.Address = txtAddress.Text;
        objVendor.city = txtCity.Text;
        objVendor.state = ddlState.SelectedItem.Text;
        objVendor.zip_code = txtZip.Text;
        objVendor.phone = txtPhone.Text;
        objVendor.fax = txtFax.Text;
        objVendor.email = txtEmailAddress.Text;
        objVendor.is_active = chkIsActive.Checked;

        if (Convert.ToInt32(hdnVendorId.Value) == 0)
        {
            if (txtVendorName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Vendor Name.");

                return;
            }

            _db.Vendors.InsertOnSubmit(objVendor);
            _db.SubmitChanges();
            hdnVendorId.Value = objVendor.vendor_id.ToString();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");

        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");

        }

        int selectedCount = chkSections.Items.Cast<ListItem>().Count(li => li.Selected);
        if (selectedCount > 0)
        {
            foreach (ListItem li in chkSections.Items)
            {
                vendor_section objVendSec = new vendor_section();
                if (li.Selected)
                {
                    objVendSec.section_id = Convert.ToInt32(li.Value);
                    objVendSec.SectionName = li.Text;
                    objVendSec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    objVendSec.Vendor_Id = Convert.ToInt32(hdnVendorId.Value);
                    objVendSec.LastUpdateDate = DateTime.Now;
                    objVendSec.UpdateBy = User.Identity.Name;

                    _db.vendor_sections.InsertOnSubmit(objVendSec);
                }
            }          
        }
        string strQ = "DELETE vendor_section WHERE Vendor_Id =" + Convert.ToInt32(hdnVendorId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);

        _db.SubmitChanges();
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("VendorList.aspx");

    }

    protected void btnSaveVD_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveVD.ID, btnSaveVD.GetType().Name, "Click"); 
        try
        {
            lblResult.Text = "";
            lblResultContact.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            vendorDetail objVD = new vendorDetail();
            objVD.vendorDetailId = 0;
            objVD.vendor_id = Convert.ToInt32(hdnVendorId.Value);
            objVD.FirstName = txtFirstName.Text.Trim();
            objVD.LastName = txtLastName.Text.Trim();
            objVD.Email = txtEmail.Text.Trim();
            objVD.Phone = txtVdPhone.Text.Trim();
            if (Convert.ToInt32(objVD.vendorDetailId) == 0)
            {
                _db.vendorDetails.InsertOnSubmit(objVD);
                lblResultContact.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully.");
                _db.SubmitChanges();
                GetVendorDetails(Convert.ToInt32(hdnVendorId.Value));

                objVD.FirstName = "";
                objVD.LastName = "";
                objVD.Email = "";
                objVD.Phone = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtEmail.Text = "";
                txtVdPhone.Text = "";

            }
        }
        catch (Exception ex)
        {
            lblResultContact.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdVendorDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Label lblFirstNameG = (Label)grdVendorDetails.Rows[e.NewEditIndex].FindControl("lblFirstNameG");
        TextBox txtFirstNameG = (TextBox)grdVendorDetails.Rows[e.NewEditIndex].FindControl("txtFirstNameG");

        Label lblLastNameG = (Label)grdVendorDetails.Rows[e.NewEditIndex].FindControl("lblLastNameG");
        TextBox txtLastNameG = (TextBox)grdVendorDetails.Rows[e.NewEditIndex].FindControl("txtLastNameG");

        Label lblEmailG = (Label)grdVendorDetails.Rows[e.NewEditIndex].FindControl("lblEmailG");
        TextBox txtEmailG = (TextBox)grdVendorDetails.Rows[e.NewEditIndex].FindControl("txtEmailG");
        Label lblPhoneG = (Label)grdVendorDetails.Rows[e.NewEditIndex].FindControl("lblPhoneG");
        TextBox txtPhoneG = (TextBox)grdVendorDetails.Rows[e.NewEditIndex].FindControl("txtPhoneG");
        
        lblFirstNameG.Visible = false;
        txtFirstNameG.Visible = true;
        lblLastNameG.Visible = false;
        txtLastNameG.Visible = true;
       
        lblEmailG.Visible = false;
        txtEmailG.Visible = true;
        lblPhoneG.Visible = false;
        txtPhoneG.Visible = true;
        LinkButton btn = (LinkButton)grdVendorDetails.Rows[e.NewEditIndex].Cells[4].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdVendorDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        Label lblFirstNameG = (Label)grdVendorDetails.Rows[e.RowIndex].FindControl("lblFirstNameG");
        TextBox txtFirstNameG = (TextBox)grdVendorDetails.Rows[e.RowIndex].FindControl("txtFirstNameG");
        Label lblLastNameG = (Label)grdVendorDetails.Rows[e.RowIndex].FindControl("lblLastNameG");
        TextBox txtLastNameG = (TextBox)grdVendorDetails.Rows[e.RowIndex].FindControl("txtLastNameG");
        Label lblEmailG = (Label)grdVendorDetails.Rows[e.RowIndex].FindControl("lblEmailG");
        TextBox txtEmailG = (TextBox)grdVendorDetails.Rows[e.RowIndex].FindControl("txtEmailG");
        Label lblPhoneG = (Label)grdVendorDetails.Rows[e.RowIndex].FindControl("lblPhoneG");
        TextBox txtPhoneG = (TextBox)grdVendorDetails.Rows[e.RowIndex].FindControl("txtPhoneG");

        int nVedorDetailsId = Convert.ToInt32(grdVendorDetails.DataKeys[Convert.ToInt32(e.RowIndex)].Values[1]);


        vendorDetail objVD = new vendorDetail();
        if (nVedorDetailsId > 0)
        {
            objVD = _db.vendorDetails.Single(c => c.vendorDetailId == nVedorDetailsId);
            objVD.vendor_id = Convert.ToInt32(hdnVendorId.Value);
            objVD.FirstName = txtFirstNameG.Text.Trim();
            objVD.LastName = txtLastNameG.Text.Trim();
            objVD.Email = txtEmailG.Text.Trim();
            objVD.Phone = txtPhoneG.Text.Trim();
           _db.SubmitChanges();
            GetVendorDetails(Convert.ToInt32(hdnVendorId.Value));
            lblResultContact.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        }


    }
    private void GetVendorDetails(int nVendorId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnVendorId.Value) > 0)
        {
            var vendorList = from cc in _db.vendorDetails
                       where cc.vendor_id == nVendorId
                       orderby cc.vendorDetailId descending
                       select cc;
            grdVendorDetails.DataSource = vendorList;
            grdVendorDetails.DataKeyNames = new string[] { "vendor_id", "vendorDetailId" };
            grdVendorDetails.DataBind();
        }

    }
}
