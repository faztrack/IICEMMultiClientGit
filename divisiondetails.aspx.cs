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

public partial class divisiondetails : System.Web.UI.Page
{
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        //DateTime end = (DateTime)Session["loadstarttime"];
        //TimeSpan loadtime = DateTime.Now - end;
        //lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("loc002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }



            BindDivisonGridView();

            //csCommonUtility.SetPagePermission(this.Page, new string[] { "chkActive", "btnSubmit" });

        }

    }

    private void BindDivisonGridView()
    {
        try
        {
            string sql = "select id, division_name, case when status = 0 then 'Inactive' else 'Active' end as status from division ORDER BY division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            grddivision.DataKeyNames = new string[] { "id", "division_name", "status" };
            grddivision.DataSource = dt;
            grddivision.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
       
    }


    
    protected void imgEdit_Click(object sender, EventArgs e)
    {
        try
        {
            ImageButton btnEdit = (ImageButton)sender;
            int id = Convert.ToInt32(btnEdit.CommandArgument);

            DataClassesDataContext _db = new DataClassesDataContext();
            division dv = _db.divisions.FirstOrDefault(x => x.Id == id);
            if(dv != null)
            {
                txtDivisionName.Text = dv.division_name.Trim();

                if (Convert.ToInt32(dv.status) == 0)
                    chkActive.Checked = false;
                else
                    chkActive.Checked = true;

                btnSubmit.Text = "Update";
                hdnDivisionId.Value = dv.Id.ToString();
            }

        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click");
            DataClassesDataContext _db = new DataClassesDataContext();

            if (txtDivisionName.Text == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Division name is required.");
                txtDivisionName.Focus();
                return;
            }


            division dv = new division();

            //for update division 
            if (Convert.ToInt32(hdnDivisionId.Value) > 0)
            {
                dv = _db.divisions.FirstOrDefault(x => x.Id == Convert.ToInt32(hdnDivisionId.Value));

                if (dv.division_name.ToLower().Trim() != txtDivisionName.Text.ToLower().Trim())
                {
                    if (_db.divisions.Any(x => x.division_name.ToLower().Trim() == txtDivisionName.Text.ToLower().Trim()))
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Division name is already exists.");
                        txtDivisionName.Focus();
                        return;
                    }
                }


                dv.division_name = txtDivisionName.Text.ToLower().Trim();
                if (chkActive.Checked)
                    dv.status = true;
                else
                    dv.status = false;


                _db.SubmitChanges();
                lblResult.Text = csCommonUtility.GetSystemMessage("Division has been updated successfully.");

                btnSubmit.Text = "Submit";
                hdnDivisionId.Value = "0";
            }

            //for new division 
            else
            {
                if (_db.divisions.Any(x => x.division_name.ToLower().Trim() == txtDivisionName.Text.ToLower().Trim()))
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Division name is already exists.");
                    txtDivisionName.Focus();
                    return;
                }

                dv.division_name = txtDivisionName.Text.Trim();
                if (chkActive.Checked)
                    dv.status = true;
                else
                    dv.status = false;
                _db.divisions.InsertOnSubmit(dv);
                _db.SubmitChanges();
                lblResult.Text = csCommonUtility.GetSystemMessage("Division has been saved successfully.");

                
            }

            Reset();
            BindDivisonGridView();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
       
    }
    private void Reset()
    {
        txtDivisionName.Text = "";
        chkActive.Checked = false;
    }

    

    
}
