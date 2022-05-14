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

public partial class disclaimerui : System.Web.UI.Page
{
    public DataTable dtSection;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Page.User.IsInRole("di1") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            LoadSectionSec();
            BindDisclaimer();


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSave", "btnUp", "btnDown" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdDisclaimer_chkInitial", "Add", "Select", "grdDisclaimer_imgDelete" });
        }

    }


    private void LoadSectionSec()
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpSTable = LoadDataTable();
            DataRow dr = tmpSTable.NewRow();


            string strQ = " select distinct section_level, section_name from disclaimers order by section_name asc  ";
            IEnumerable<csDisclaimer> mList = _db.ExecuteQuery<csDisclaimer>(strQ, string.Empty).ToList();
            foreach (var vi in mList)
            {

                DataRow drNew = tmpSTable.NewRow();
                drNew["section_level"] = vi.section_level;
                drNew["section_name"] = vi.section_name;
                tmpSTable.Rows.Add(drNew);
            }

            Session.Add("gSection", tmpSTable);
            dtSection = (DataTable)Session["gSection"];
        }
        catch (Exception ex)
        {
            throw ex;
        }
      

    }

    private void BindDisclaimer()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpTable = LoadDataTable();


            string strQ = " select* from disclaimers order by section_level asc ";
            IEnumerable<csDisclaimer> mList = _db.ExecuteQuery<csDisclaimer>(strQ, string.Empty).ToList();

            foreach (csDisclaimer dc in mList)
            {

                DataRow drNew = tmpTable.NewRow();
                drNew["disclaimer_id"] = dc.disclaimer_id;
                drNew["client_id"] = dc.client_id;
                drNew["section_level"] = dc.section_level;
                drNew["section_name"] = dc.section_name;
                drNew["section_heading"] = dc.section_heading;
                drNew["disclaimer_name"] = dc.disclaimer_name;
                drNew["IsInitilal"] = dc.IsInitilal;
                tmpTable.Rows.Add(drNew);
            }
            if (tmpTable.Rows.Count == 0)
            {
                DataRow drNew = tmpTable.NewRow();
                drNew["disclaimer_id"] = 0;
                drNew["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                drNew["section_level"] = 0;
                drNew["section_name"] = "";
                drNew["section_heading"] = "";
                drNew["disclaimer_name"] = "";
                drNew["IsInitilal"] = false;

                tmpTable.Rows.Add(drNew);
            }
            Session.Add("Disclaimer", tmpTable);
            grdDisclaimer.DataSource = tmpTable;
            grdDisclaimer.DataKeyNames = new string[] { "disclaimer_id", "section_level", "section_name" };
            grdDisclaimer.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("disclaimer_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("section_level", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("section_heading", typeof(string));
        table.Columns.Add("disclaimer_name", typeof(string));
        table.Columns.Add("IsInitilal", typeof(bool));
        return table;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ_Delete = "Delete disclaimers WHERE  client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ_Delete, string.Empty);

            DataTable table = (DataTable)Session["Disclaimer"];

            foreach (GridViewRow di in grdDisclaimer.Rows)
            {
                disclaimer objDis = new disclaimer();

                DataRow dr = table.Rows[di.RowIndex];
                CheckBox chkInitial = (CheckBox)di.FindControl("chkInitial");
                int disclaimerId = Convert.ToInt32(grdDisclaimer.DataKeys[di.RowIndex].Values[0]);
                DropDownList ddlSectiong = (DropDownList)di.FindControl("ddlSectiong");
                dr["disclaimer_id"] = Convert.ToInt32(grdDisclaimer.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                dr["section_heading"] = ((TextBox)di.FindControl("txtHeader")).Text;
                dr["disclaimer_name"] = ((TextBox)di.FindControl("txtDetails")).Text;
                dr["IsInitilal"] = Convert.ToBoolean(chkInitial.Checked);
                objDis.section_level = Convert.ToInt32(ddlSectiong.SelectedValue);
                objDis.section_name = ddlSectiong.SelectedItem.Text;
                objDis.section_heading = ((TextBox)di.FindControl("txtHeader")).Text;
                objDis.disclaimer_name = ((TextBox)di.FindControl("txtDetails")).Text;
                objDis.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                objDis.IsInitilal = Convert.ToBoolean(chkInitial.Checked);
                //if (disclaimerId == 0)
                    _db.disclaimers.InsertOnSubmit(objDis);
                _db.SubmitChanges();
            }
           
            BindDisclaimer();
            lblResult.Text = "Data saved successfully";
            lblResult.ForeColor = System.Drawing.Color.Green;
            btnSave.Focus();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void grdDisclaimer_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            DataTable table = (DataTable)Session["Disclaimer"];
            dtSection = (DataTable)Session["gSection"];
            if (e.CommandName == "Add")
            {
                foreach (GridViewRow di in grdDisclaimer.Rows)
                {

                    DataRow dr = table.Rows[di.RowIndex];
                    CheckBox chkInitial = (CheckBox)di.FindControl("chkInitial");
                    DropDownList ddlSectiong = (DropDownList)di.FindControl("ddlSectiong");

                    dr["disclaimer_id"] = Convert.ToInt32(grdDisclaimer.DataKeys[di.RowIndex].Values[0]);
                    dr["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    dr["section_heading"] = ((TextBox)di.FindControl("txtHeader")).Text;
                    dr["disclaimer_name"] = ((TextBox)di.FindControl("txtDetails")).Text;
                    dr["IsInitilal"] = Convert.ToBoolean(chkInitial.Checked);



                }
                DataRow drNew = table.NewRow();
                drNew["disclaimer_id"] = 0;
                drNew["client_id"] = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                drNew["section_level"] = 0;
                drNew["section_name"] = "";
                drNew["section_heading"] = "";
                drNew["disclaimer_name"] = "";
                drNew["IsInitilal"] = false;

                table.Rows.Add(drNew);
                Session.Add("Disclaimer", table);
                grdDisclaimer.DataSource = table;
                grdDisclaimer.DataKeyNames = new string[] { "disclaimer_id", "section_level", "section_name" };
                grdDisclaimer.DataBind();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }
    protected void grdDisclaimer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                dtSection = (DataTable)Session["gSection"];
                DataClassesDataContext _db = new DataClassesDataContext();
                int disclaimer_id = Convert.ToInt32(grdDisclaimer.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int section_level = Convert.ToInt32(grdDisclaimer.DataKeys[e.Row.RowIndex].Values[1].ToString());
                string section_name = grdDisclaimer.DataKeys[e.Row.RowIndex].Values[2].ToString();
                DropDownList ddlSectiong = (DropDownList)e.Row.FindControl("ddlSectiong");

                ddlSectiong.DataSource = dtSection;
                ddlSectiong.DataTextField = "section_name";
                ddlSectiong.DataValueField = "section_level";
                ddlSectiong.DataBind();

                ListItem items = ddlSectiong.Items.FindByValue(section_level.ToString());
                if (items != null)
                    ddlSectiong.Items.FindByValue(Convert.ToString(section_level)).Selected = true;

                ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
                imgDelete.OnClientClick = "return confirm('Are you sure you want to delete ?');";
                imgDelete.CommandArgument = disclaimer_id.ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    protected void DeleteFile(object sender, EventArgs e)
    {
        try
        {
             DataClassesDataContext _db = new DataClassesDataContext();
             ImageButton imgDelete = (ImageButton)sender;
             int disclaimer_id = Convert.ToInt32(imgDelete.CommandArgument);
             string strQ_Delete = " Delete disclaimers where disclaimer_id=" + disclaimer_id;
              _db.ExecuteCommand(strQ_Delete, string.Empty);
              lblResult.Text = "Data deleted successfully";
             lblResult.ForeColor = System.Drawing.Color.Green;
             BindDisclaimer();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}