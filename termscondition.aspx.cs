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

public partial class termscondition : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Page.User.IsInRole("admin011") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            if (Session["oUser"] != null)
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
            }

            BindDivision();      
            BindGrid();


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSave" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdTermCon_chkInitial", "Add" });
        }

    }

    private void BindGrid()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpTable = LoadDataTable();

            string condition = " ";
            if (ddlDivision.SelectedItem.Text != "All")
            {
                condition += " where client_id = " + Convert.ToInt32(ddlDivision.SelectedValue);
            }

            string strQ = "select * from company_terms_condition " + condition;

            DataTable dt = csCommonUtility.GetDataTable(strQ);


            if (tmpTable.Rows.Count == 0)
            {
                DataRow drNew = tmpTable.NewRow();
                drNew["terms_condition_id"] = 0;
                drNew["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                drNew["terms_condition"] = "";
                drNew["terms_header"] = "";
                drNew["IsInitilal"] = false;

                tmpTable.Rows.Add(drNew);
            }

            Session.Add("Terms", dt);
            grdTermCon.DataSource = dt;
            grdTermCon.DataKeyNames = new string[] { "terms_condition_id", "client_id" };
            grdTermCon.DataBind();
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
            ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }

    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("terms_condition_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("terms_condition", typeof(string));
        table.Columns.Add("terms_header", typeof(string));
        table.Columns.Add("IsInitilal", typeof(bool));
        return table;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
      

        foreach (GridViewRow di in grdTermCon.Rows)
        {

            int id = Convert.ToInt32(grdTermCon.DataKeys[di.RowIndex].Values[0]);

            DropDownList ddlDivision = (DropDownList)di.FindControl("ddlDivision");
            TextBox txtTermsDetails = (TextBox)di.FindControl("txtTermsDetails");
            TextBox txtTermsHeader = (TextBox)di.FindControl("txtTermsHeader");
            CheckBox chkInitial = (CheckBox)di.FindControl("chkInitial");


            company_terms_condition trmCond = new company_terms_condition();

            if(_db.company_terms_conditions.Any(x=>x.terms_condition_id == id))
            {
                trmCond = _db.company_terms_conditions.FirstOrDefault(x => x.terms_condition_id == id);
                if (txtTermsDetails.Text.Length > 0 && trmCond != null)
                {
                    trmCond.terms_header = txtTermsHeader.Text.Trim();
                    trmCond.terms_condition = txtTermsDetails.Text.Trim();

                    if (chkInitial.Checked)
                        trmCond.IsInitilal = true;
                    else
                        trmCond.IsInitilal = false;

                    trmCond.client_id = Convert.ToInt32(ddlDivision.SelectedValue);


                }
            }
            else
            {
                if (txtTermsDetails.Text.Length > 0)
                {
                    trmCond.terms_header = txtTermsHeader.Text.Trim();
                    trmCond.terms_condition = txtTermsDetails.Text.Trim();
                    trmCond.client_id = Convert.ToInt32(ddlDivision.SelectedValue);

                    if (chkInitial.Checked)
                        trmCond.IsInitilal = true;
                    else
                        trmCond.IsInitilal = false;

                    _db.company_terms_conditions.InsertOnSubmit(trmCond);
                }
               
            }

        }
       
        _db.SubmitChanges();

        lblResult.Text = "Data saved successfully";
        lblResult.ForeColor = System.Drawing.Color.Green;
        BindGrid();



    }

    

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void grdTermCon_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        DataTable table = (DataTable)Session["Terms"];
        

        if (e.CommandName == "Add")
        {
          
            DataRow drNew = table.NewRow();
            drNew["terms_condition_id"] = 0;
            drNew["client_id"] = "1";
            drNew["terms_condition"] = "";
            drNew["terms_header"] = "";
            drNew["IsInitilal"] = false;
            table.Rows.Add(drNew);

            Session.Add("Terms", table);
            grdTermCon.DataSource = table;
            grdTermCon.DataKeyNames = new string[] { "terms_condition_id", "client_id" };
            grdTermCon.DataBind();

        }
    }

    protected void grdTermCon_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string clientId = grdTermCon.DataKeys[e.Row.RowIndex].Values[1].ToString();
            DropDownList ddlDivision = e.Row.FindControl("ddlDivision") as DropDownList;

            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            ddlDivision.DataSource = dt;
            ddlDivision.DataTextField = "division_name";
            ddlDivision.DataValueField = "id";            
            ddlDivision.DataBind();
            ddlDivision.SelectedValue = clientId;
        }
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrid();
    }
}
