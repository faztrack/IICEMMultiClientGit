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

public partial class companyspecialnote : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        hdnClientId.Value = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]).ToString();

        if (!IsPostBack)
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin012") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            BindDivision();    
            GetSpecialNote();
            
            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit" });
        }
    }


    private void GetSpecialNote()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            Company_special_note objCSN = new Company_special_note();

            if (_db.Company_special_notes.Any(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue)))
            {
                objCSN = _db.Company_special_notes.FirstOrDefault(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
                if (objCSN != null)
                {
                    txtSpecialNote.Text = objCSN.special_note;
                }
            }
            else
            {
                txtSpecialNote.Text = "";
            }
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

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        if (Convert.ToInt32(txtSpecialNote.Text.Trim().Length) == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing special note content.");            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        Company_special_note objCSN = new Company_special_note();

        //update
        if(_db.Company_special_notes.Any(x=>x.client_id == Convert.ToInt32(ddlDivision.SelectedValue)))
        {
            objCSN = _db.Company_special_notes.FirstOrDefault(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
            if(objCSN != null)
            {
                objCSN.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
                objCSN.special_note = txtSpecialNote.Text;

                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
            }
        }
        else  //new 
        {
            objCSN.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
            objCSN.special_note = txtSpecialNote.Text;
            _db.Company_special_notes.InsertOnSubmit(objCSN);

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        }       

        _db.SubmitChanges();
        
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
        GetSpecialNote();
    }
}
