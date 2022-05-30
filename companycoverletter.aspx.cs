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

public partial class companycoverletter : System.Web.UI.Page
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
           
            if (Page.User.IsInRole("admin010") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            BindDivision();
            CoverLetterBind();







            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit" });
        }
    }


    private void CoverLetterBind()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (_db.company_cover_letters.Any(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue)))
        {
            company_cover_letter objComcl = _db.company_cover_letters.FirstOrDefault(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
            txtCoverLetter.Text = objComcl.cover_letter;
        }
        else
        {
            txtCoverLetter.Text = "";
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
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
        if (Convert.ToInt32(txtCoverLetter.Text.Trim().Length) == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing cover letter content.");            
            return;
        }


        DataClassesDataContext _db = new DataClassesDataContext();
        company_cover_letter objComcl = new company_cover_letter();


        objComcl = _db.company_cover_letters.FirstOrDefault(x => x.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
        
        //update
        if(objComcl != null)
        {
            objComcl.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
            objComcl.cover_letter = txtCoverLetter.Text;
        }
        //new 
        else
        {
            company_cover_letter objC = new company_cover_letter();
            objC.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
            objC.cover_letter = txtCoverLetter.Text;
            _db.company_cover_letters.InsertOnSubmit(objC);
        }
          
        _db.SubmitChanges();

        lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        CoverLetterBind();
    }
}
