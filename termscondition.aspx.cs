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
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpTable = LoadDataTable();

            var item = from it in _db.company_terms_conditions
                       where it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                       select new TermsAndCondition()
                       {
                           terms_condition_id = (int)it.terms_condition_id,
                           client_id = (int)it.client_id,
                           terms_condition = it.terms_condition,
                           terms_header = it.terms_header,
                           IsInitilal = (bool)it.IsInitilal
                       };
            foreach (TermsAndCondition tc in item)
            {

                DataRow drNew = tmpTable.NewRow();
                drNew["terms_condition_id"] = tc.terms_condition_id;
                drNew["client_id"] = tc.client_id;
                drNew["terms_condition"] = tc.terms_condition;
                drNew["terms_header"] = tc.terms_header;
                drNew["IsInitilal"] = tc.IsInitilal;
                tmpTable.Rows.Add(drNew);
            }
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
            Session.Add("Terms", tmpTable);
            grdTermCon.DataSource = tmpTable;
            grdTermCon.DataKeyNames = new string[] { "terms_condition_id" };
            grdTermCon.DataBind();



            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSave" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdTermCon_chkInitial", "Add" });
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
        string strQ_Delete = "Delete company_terms_condition WHERE  client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ_Delete, string.Empty);



        DataTable table = (DataTable)Session["Terms"];

        foreach (GridViewRow di in grdTermCon.Rows)
        {
            {
                DataRow dr = table.Rows[di.RowIndex];
                CheckBox chkInitial = (CheckBox)di.FindControl("chkInitial");
                dr["terms_condition_id"] = Convert.ToInt32(grdTermCon.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                dr["terms_condition"] = ((TextBox)di.FindControl("txtTermsDetails")).Text;
                dr["terms_header"] = ((TextBox)di.FindControl("txtTermsHeader")).Text;
                dr["IsInitilal"] = Convert.ToBoolean(chkInitial.Checked);

            }

        }
        foreach (DataRow dr in table.Rows)
        {
            company_terms_condition term_con = new company_terms_condition();
            string str = dr["terms_condition"].ToString().Trim() + dr["terms_header"].ToString().Trim();
            if (str.Length > 0)
            {
                term_con.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                term_con.terms_condition = dr["terms_condition"].ToString();
                term_con.terms_header = dr["terms_header"].ToString();
                term_con.IsInitilal = Convert.ToBoolean(dr["IsInitilal"]);
                _db.company_terms_conditions.InsertOnSubmit(term_con);


            }
        }

        lblResult.Text = "Data saved successfully";
        lblResult.ForeColor = System.Drawing.Color.Green;
        _db.SubmitChanges();

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
            foreach (GridViewRow di in grdTermCon.Rows)
            {
                {
                    DataRow dr = table.Rows[di.RowIndex];
                    CheckBox chkInitial = (CheckBox)di.FindControl("chkInitial");
                    dr["terms_condition_id"] = Convert.ToInt32(grdTermCon.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    dr["terms_condition"] = ((TextBox)di.FindControl("txtTermsDetails")).Text;
                    dr["terms_header"] = ((TextBox)di.FindControl("txtTermsHeader")).Text;
                    dr["IsInitilal"] = Convert.ToBoolean(chkInitial.Checked);

                }

            }

            DataRow drNew = table.NewRow();
            drNew["terms_condition_id"] = 0;
            drNew["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            drNew["terms_condition"] = "";
            drNew["terms_header"] = "";
            drNew["IsInitilal"] = false;
            table.Rows.Add(drNew);

            Session.Add("Terms", table);
            grdTermCon.DataSource = table;
            grdTermCon.DataKeyNames = new string[] { "terms_condition_id" };
            grdTermCon.DataBind();

        }
    }
}
