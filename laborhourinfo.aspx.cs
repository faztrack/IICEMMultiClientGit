using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class laborhourinfo : System.Web.UI.Page
{


    public DataTable dtSection;
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        return (from c in _db.customers
                where c.last_name1.StartsWith(prefixText)
                join ce in _db.customer_estimates on c.customer_id equals ce.customer_id
                where ce.status_id == 3
                select c.first_name1 + " " + c.last_name1 + " (" + ce.job_number+")").Take<String>(count).ToArray();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("t03") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSuperintendent();

        }

    }
    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS crew_name,crew_id from Crew_Details WHERE is_active = 1";
        List<CrewDe> mList = _db.ExecuteQuery<CrewDe>(strQ, string.Empty).ToList();
        ddlInstaller.DataSource = mList;
        ddlInstaller.DataTextField = "crew_name";
        ddlInstaller.DataValueField = "crew_id";
        ddlInstaller.DataBind();
        ddlInstaller.Items.Insert(0, "Select");
    }
    private void LoadSection(int custID, int estID)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpSTable = LoadSectionTable();
        DataRow dr = tmpSTable.NewRow();
        dr["section_id"] = -1;
        dr["section_name"] = "Select";
        tmpSTable.Rows.Add(dr);

        var item = from it in _db.customer_sections
                   join si in _db.sectioninfos on it.section_id equals si.section_id
                   where it.customer_id == custID && it.estimate_id == estID 
                   select new SectionInfo()
                   {
                       section_id = (int)it.section_id,
                       section_name = si.section_name

                   };
        foreach (SectionInfo sinfo in item)
        {

            DataRow drNew = tmpSTable.NewRow();
            drNew["section_id"] = sinfo.section_id;
            drNew["section_name"] = sinfo.section_name;
            tmpSTable.Rows.Add(drNew);
        }

        dr = tmpSTable.NewRow();
        dr["section_id"] = 1;
        dr["section_name"] = "Travel";
        tmpSTable.Rows.Add(dr);
        dr = tmpSTable.NewRow();
        dr["section_id"] = 2;
        dr["section_name"] = "Meeting";
        tmpSTable.Rows.Add(dr);

        dr = tmpSTable.NewRow();
        dr["section_id"] = 3;
        dr["section_name"] = "Other";
        tmpSTable.Rows.Add(dr);




        Session.Add("Section", tmpSTable);
        dtSection = (DataTable)Session["Section"];

    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        return table;
    }
    protected void txtStartDate_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtStartDate.ID, txtStartDate.GetType().Name, "TextChanged"); 
        lblResult.Text = "";
        if (txtStartDate.Text != "")
        {
            try
            {
                DateTime DtDate = Convert.ToDateTime(txtStartDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please enter date in correct format : mm/dd/yyyy");
                lblResult.Visible = true;
                return;

            }

            if (Convert.ToDateTime(txtStartDate.Text).DayOfWeek != DayOfWeek.Monday)
            {

                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Work Weeks starts on Monday");
                lblResult.Visible = true;
                return;
            }

           
           lblDate1.Text = Convert.ToDateTime(txtStartDate.Text).ToLongDateString();
               DateTime dt = Convert.ToDateTime(lblDate1.Text);
               LoadLaborEntryByDate(dt,grdLaborDate1);
           lblDate2.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(1).ToLongDateString();
               dt = Convert.ToDateTime(lblDate2.Text);
               LoadLaborEntryByDate(dt, grdLaborDate2);
           lblDate3.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(2).ToLongDateString();
               dt = Convert.ToDateTime(lblDate3.Text);
               LoadLaborEntryByDate(dt, grdLaborDate3);
           lblDate4.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(3).ToLongDateString();
               dt = Convert.ToDateTime(lblDate4.Text);
               LoadLaborEntryByDate(dt, grdLaborDate4);
           lblDate5.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(4).ToLongDateString();
           dt = Convert.ToDateTime(lblDate5.Text);
           LoadLaborEntryByDate(dt, grdLaborDate5);
           lblDate6.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(5).ToLongDateString();
               dt = Convert.ToDateTime(lblDate6.Text);
               LoadLaborEntryByDate(dt, grdLaborDate6);
           lbldate7.Text = Convert.ToDateTime(txtStartDate.Text).AddDays(6).ToLongDateString();
               dt = Convert.ToDateTime(lbldate7.Text);
               LoadLaborEntryByDate(dt, grdLaborDate7);
            
        }

    }
    private void LoadLaborEntryByDate(DateTime dt, GridView grd)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();
        int nInstallerId = 0;
        if (ddlInstaller.SelectedItem.Text != "Select")
        {
            nInstallerId = Convert.ToInt32(ddlInstaller.SelectedValue);
 
        }

        string strQ = "select * from labor_hour_entry where installer_id = "+nInstallerId+" AND  labor_date between '" + dt + "' AND '" + dt.AddHours(23).AddMinutes(59) + "'  order by labor_date desc ";
        IEnumerable<LaborHour> clist = _db.ExecuteQuery<LaborHour>(strQ, string.Empty);
        foreach (LaborHour jsc in clist)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["labor_entry_id"] = jsc.labor_entry_id;
            drNew["customer_id"] = jsc.customer_id;
            drNew["estimate_id"] = jsc.estimate_id;
            drNew["section_id"] = jsc.section_id;
            drNew["labor_hour"] = jsc.labor_hour;
            drNew["labor_date"] = jsc.labor_date;
            drNew["installer_id"] = jsc.installer_id;
            drNew["serial"] = jsc.serial;
            drNew["last_name"] = jsc.last_name;
            tmpTable.Rows.Add(drNew);
        }
       
            DataRow drNew1 = tmpTable.NewRow();
            drNew1["labor_entry_id"] = 0;
            drNew1["customer_id"] = 0;
            drNew1["estimate_id"] = -1;
            drNew1["section_id"] = -1;
            drNew1["labor_hour"] = 0;
            drNew1["labor_date"] = dt;
            drNew1["installer_id"] = 0;
            drNew1["serial"] = 0;
            drNew1["last_name"] = "";
            tmpTable.Rows.Add(drNew1);
        
        Session.Add("labor_hour_entr", tmpTable);
        DataView dv = tmpTable.DefaultView;
        dv.Sort = "labor_date ASC";
        grd.DataSource = dv;
        grd.DataKeyNames = new string[] { "customer_id", "estimate_id", "labor_date", "installer_id", "labor_entry_id", "section_id" };
        grd.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("labor_entry_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("labor_hour", typeof(decimal));
        table.Columns.Add("labor_date", typeof(DateTime));
        table.Columns.Add("installer_id", typeof(int));
        table.Columns.Add("serial", typeof(int));
        table.Columns.Add("last_name", typeof(string));
        return table;
    }

    #region Date1

        protected void grdLaborDate1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int cid = Convert.ToInt32(grdLaborDate1.DataKeys[e.Row.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate1.DataKeys[e.Row.RowIndex].Values[1]);
                int sid = Convert.ToInt32(grdLaborDate1.DataKeys[e.Row.RowIndex].Values[5]);
                DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


                    if (eid < 1)
                        eid = Convert.ToInt32(hdnEstimateId.Value);
                    if (cid == 0)
                        cid = Convert.ToInt32(hdnCustomerId.Value);
                  
                    LoadSection(cid, eid);
                    ddlSection.DataSource = dtSection;
                    ddlSection.DataBind();
                    ddlSection.SelectedValue = sid.ToString();


                Label lblLastName = (Label)e.Row.FindControl("lblLastName");
                Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


                if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
                {
                    txtlast_name.Visible = true;
                    lblLastName.Visible = false;

                    lbllabor_hour.Visible = false;
                    txtlabor_hour.Visible = true;

                    ddlSection.Enabled = true;
                    LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                    btn.Text = "Save";
                    btn.CommandName = "Save";

                }

            }

        }
        protected void grdLaborDate1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            DropDownList ddlSection = (DropDownList)grdLaborDate1.Rows[e.NewEditIndex].FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)grdLaborDate1.Rows[e.NewEditIndex].FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)grdLaborDate1.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

            Label lblLastName = (Label)grdLaborDate1.Rows[e.NewEditIndex].FindControl("lblLastName");
            Label lbllabor_hour = (Label)grdLaborDate1.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

            lbllabor_hour.Visible = false;
            txtlabor_hour.Visible = true;
       
            ddlSection.Enabled = true;
            LinkButton btn = (LinkButton)grdLaborDate1.Rows[e.NewEditIndex].Cells[3].Controls[0];
            btn.Text = "Update";
            btn.CommandName = "Update";

        }
        protected void grdLaborDate1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblResult.Text = "";

            DataClassesDataContext _db = new DataClassesDataContext();
        
            DropDownList ddlSection = (DropDownList)grdLaborDate1.Rows[e.RowIndex].FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)grdLaborDate1.Rows[e.RowIndex].FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)grdLaborDate1.Rows[e.RowIndex].FindControl("txtlabor_hour");

            Label lblLastName = (Label)grdLaborDate1.Rows[e.RowIndex].FindControl("lblLastName");
            Label lbllabor_hour = (Label)grdLaborDate1.Rows[e.RowIndex].FindControl("lbllabor_hour");

            int nlabor_entry_id = Convert.ToInt32(grdLaborDate1.DataKeys[e.RowIndex].Values[4]);
            decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

            string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
            _db.ExecuteCommand(strQ, string.Empty);


            lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
       

            DateTime dt = Convert.ToDateTime(lblDate1.Text);
            LoadLaborEntryByDate(dt, grdLaborDate1);

        }
        private DataTable LoadLaborEntryByWeek(DateTime dt)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable tmpTable = LoadDataTable();
             int nInstallerId = 0;
            if (ddlInstaller.SelectedItem.Text != "Select")
            {
                nInstallerId = Convert.ToInt32(ddlInstaller.SelectedValue);
 
            }

            string strQ = "select * from labor_hour_entry where installer_id = "+nInstallerId+" AND labor_date between '" + dt + "' AND '" + dt.AddHours(23).AddMinutes(59) + "'  order by labor_date desc ";
            IEnumerable<LaborHour> clist = _db.ExecuteQuery<LaborHour>(strQ, string.Empty);
            foreach (LaborHour jsc in clist)
            {

                DataRow drNew = tmpTable.NewRow();
                drNew["labor_entry_id"] = jsc.labor_entry_id;
                drNew["customer_id"] = jsc.customer_id;
                drNew["estimate_id"] = jsc.estimate_id;
                drNew["section_id"] = jsc.section_id;
                drNew["labor_hour"] = jsc.labor_hour;
                drNew["labor_date"] = jsc.labor_date;
                drNew["installer_id"] = jsc.installer_id;
                drNew["serial"] = jsc.serial;
                drNew["last_name"] = jsc.last_name;

                tmpTable.Rows.Add(drNew);
            }
           
                DataRow drNew1 = tmpTable.NewRow();
                drNew1["labor_entry_id"] = 0;
                drNew1["customer_id"] = 0;
                drNew1["estimate_id"] = -1;
                drNew1["section_id"] = -1;
                drNew1["labor_hour"] = 0;
                drNew1["labor_date"] = dt;
                drNew1["installer_id"] = 0;
                drNew1["serial"] = 0;
                drNew1["last_name"] = "";
                tmpTable.Rows.Add(drNew1);
            return tmpTable;

        }
        protected void grdLaborDate1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Save")
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate1.Text));
                if (ddlInstaller.SelectedItem.Text == "Select")
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                    return;
                }
                    foreach (GridViewRow di in grdLaborDate1.Rows)
                    {
                        int cid = Convert.ToInt32(grdLaborDate1.DataKeys[di.RowIndex].Values[0]);
                        int eid = Convert.ToInt32(grdLaborDate1.DataKeys[di.RowIndex].Values[1]);


                        if (eid < 1)
                            eid = Convert.ToInt32(hdnEstimateId.Value);
                        if (cid == 0)
                            cid = Convert.ToInt32(hdnCustomerId.Value);
                            DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                            TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                            TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                            Label lblLastName = (Label)di.FindControl("lblLastName");
                            Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                            DataRow dr = table.Rows[di.RowIndex];

                            dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate1.DataKeys[di.RowIndex].Values[4]);
                            dr["customer_id"] = cid;
                            dr["estimate_id"] = eid;
                            dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                            dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                            dr["labor_date"] = Convert.ToDateTime(lblDate1.Text);
                            dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                            dr["serial"] = Convert.ToInt32(di.RowIndex);
                            dr["last_name"] = txtlast_name.Text;


                    }
                    foreach (DataRow dr in table.Rows)
                    {
                        labor_hour_entry lbe = new labor_hour_entry();
                        if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                            lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                        if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                        {
                            lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                            lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                            lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                            lbe.section_id = Convert.ToInt32(dr["section_id"]);
                            lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                            lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                            lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                            lbe.serial = Convert.ToInt32(dr["serial"]);
                            lbe.last_name = dr["last_name"].ToString();
                            if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                            {
                                _db.labor_hour_entries.InsertOnSubmit(lbe);
                            }

                        }
                        
                    }
                    _db.SubmitChanges();
                     lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
                    DateTime dt = Convert.ToDateTime(lblDate1.Text);
                    LoadLaborEntryByDate(dt, grdLaborDate1);
               

                }

        }

    #endregion

    #region Date2

    protected void grdLaborDate2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate2.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate2.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate2.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid < 1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();


            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate2.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate2.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate2.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate2.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate2.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate2.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdLaborDate2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate2.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate2.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate2.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate2.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate2.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate2.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);

      
        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        
        DateTime dt = Convert.ToDateTime(lblDate2.Text);
        LoadLaborEntryByDate(dt, grdLaborDate2);

    }
        
    protected void grdLaborDate2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate2.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate2.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate2.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate2.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate2.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lblDate2.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
              
            }
            _db.SubmitChanges();

          
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");

            DateTime dt = Convert.ToDateTime(lblDate2.Text);
            LoadLaborEntryByDate(dt, grdLaborDate2);


        }

    }

    #endregion

    #region Date3

    protected void grdLaborDate3_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate3.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate3.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate3.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid < 1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();


            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate3_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate3.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate3.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate3.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate3.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate3.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate3.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdLaborDate3_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate3.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate3.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate3.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate3.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate3.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate3.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);


    

        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        
        DateTime dt = Convert.ToDateTime(lblDate3.Text);
        LoadLaborEntryByDate(dt, grdLaborDate3);

    }

    protected void grdLaborDate3_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate3.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate3.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate3.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate3.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate3.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lblDate3.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
               
            }
            _db.SubmitChanges();
         
           
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            DateTime dt = Convert.ToDateTime(lblDate3.Text);
            LoadLaborEntryByDate(dt, grdLaborDate3);


        }

    }

    #endregion

    #region Date4

    protected void grdLaborDate4_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate4.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate4.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate4.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid < 1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();

            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate4_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate4.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate4.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate4.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate4.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate4.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate4.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

       

    }
    protected void grdLaborDate4_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate4.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate4.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate4.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate4.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate4.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate4.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);

        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
     
        DateTime dt = Convert.ToDateTime(lblDate4.Text);
        LoadLaborEntryByDate(dt, grdLaborDate4);

    }

    protected void grdLaborDate4_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate4.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate4.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate4.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate4.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate4.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lblDate4.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
                
            }
            _db.SubmitChanges();
          
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            DateTime dt = Convert.ToDateTime(lblDate4.Text);
            LoadLaborEntryByDate(dt, grdLaborDate4);


        }

    }

    #endregion

    #region Date5

    protected void grdLaborDate5_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate5.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate5.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate5.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid < 1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();


            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate5_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate5.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate5.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate5.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate5.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate5.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate5.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdLaborDate5_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate5.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate5.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate5.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate5.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate5.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate5.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);


        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
      

        DateTime dt = Convert.ToDateTime(lblDate5.Text);
        LoadLaborEntryByDate(dt, grdLaborDate5);

    }

    protected void grdLaborDate5_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate5.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate5.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate5.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate5.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate5.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lblDate5.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
              
            }
            _db.SubmitChanges();
          
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            DateTime dt = Convert.ToDateTime(lblDate5.Text);
            LoadLaborEntryByDate(dt, grdLaborDate5);


        }

    }

    #endregion

    #region Date6

    protected void grdLaborDate6_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate6.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate6.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate6.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid == -1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();

            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate6_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate6.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate6.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate6.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate6.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate6.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate6.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdLaborDate6_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate6.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate6.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate6.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate6.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate6.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate6.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);


        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
      

        DateTime dt = Convert.ToDateTime(lblDate6.Text);
        LoadLaborEntryByDate(dt, grdLaborDate6);

    }

    protected void grdLaborDate6_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lblDate6.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate6.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate6.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate6.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate6.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lblDate6.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
               
            }
            _db.SubmitChanges();
           
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            DateTime dt = Convert.ToDateTime(lblDate6.Text);
            LoadLaborEntryByDate(dt, grdLaborDate6);


        }

    }

    #endregion

    #region Date7

    protected void grdLaborDate7_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int cid = Convert.ToInt32(grdLaborDate7.DataKeys[e.Row.RowIndex].Values[0]);
            int eid = Convert.ToInt32(grdLaborDate7.DataKeys[e.Row.RowIndex].Values[1]);
            int sid = Convert.ToInt32(grdLaborDate7.DataKeys[e.Row.RowIndex].Values[5]);
            DropDownList ddlSection = (DropDownList)e.Row.FindControl("ddlSection");
            TextBox txtlast_name = (TextBox)e.Row.FindControl("txtlast_name");
            TextBox txtlabor_hour = (TextBox)e.Row.FindControl("txtlabor_hour");


            if (eid == -1)
                eid = Convert.ToInt32(hdnEstimateId.Value);
            if (cid == 0)
                cid = Convert.ToInt32(hdnCustomerId.Value);

            LoadSection(cid, eid);
            ddlSection.DataSource = dtSection;
            ddlSection.DataBind();
            ddlSection.SelectedValue = sid.ToString();


            Label lblLastName = (Label)e.Row.FindControl("lblLastName");
            Label lbllabor_hour = (Label)e.Row.FindControl("lbllabor_hour");


            if (Convert.ToDecimal(lbllabor_hour.Text.Replace("(", "-").Replace(")", "")) == 0)
            {
                txtlast_name.Visible = true;
                lblLastName.Visible = false;

                lbllabor_hour.Visible = false;
                txtlabor_hour.Visible = true;

                ddlSection.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }

        }

    }
    protected void grdLaborDate7_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlSection = (DropDownList)grdLaborDate7.Rows[e.NewEditIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate7.Rows[e.NewEditIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate7.Rows[e.NewEditIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate7.Rows[e.NewEditIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate7.Rows[e.NewEditIndex].FindControl("lbllabor_hour");

        lbllabor_hour.Visible = false;
        txtlabor_hour.Visible = true;

        ddlSection.Enabled = true;
        LinkButton btn = (LinkButton)grdLaborDate7.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdLaborDate7_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlSection = (DropDownList)grdLaborDate7.Rows[e.RowIndex].FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)grdLaborDate7.Rows[e.RowIndex].FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)grdLaborDate7.Rows[e.RowIndex].FindControl("txtlabor_hour");

        Label lblLastName = (Label)grdLaborDate7.Rows[e.RowIndex].FindControl("lblLastName");
        Label lbllabor_hour = (Label)grdLaborDate7.Rows[e.RowIndex].FindControl("lbllabor_hour");

        int nlabor_entry_id = Convert.ToInt32(grdLaborDate7.DataKeys[e.RowIndex].Values[4]);
        decimal dTotalHour = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));

        string strQ = "UPDATE labor_hour_entry SET section_id=" + Convert.ToInt32(ddlSection.SelectedValue) + " ,labor_hour=" + dTotalHour + " WHERE labor_entry_id =" + nlabor_entry_id;
        _db.ExecuteCommand(strQ, string.Empty);


        lblResult.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
    

        DateTime dt = Convert.ToDateTime(lbldate7.Text);
        LoadLaborEntryByDate(dt, grdLaborDate7);

    }

    protected void grdLaborDate7_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = LoadLaborEntryByWeek(Convert.ToDateTime(lbldate7.Text));
            if (ddlInstaller.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Installer is required");
                return;
            }
            foreach (GridViewRow di in grdLaborDate7.Rows)
            {
                int cid = Convert.ToInt32(grdLaborDate7.DataKeys[di.RowIndex].Values[0]);
                int eid = Convert.ToInt32(grdLaborDate7.DataKeys[di.RowIndex].Values[1]);


                if (eid < 1)
                    eid = Convert.ToInt32(hdnEstimateId.Value);
                if (cid == 0)
                    cid = Convert.ToInt32(hdnCustomerId.Value);
                DropDownList ddlSection = (DropDownList)di.FindControl("ddlSection");
                TextBox txtlast_name = (TextBox)di.FindControl("txtlast_name");
                TextBox txtlabor_hour = (TextBox)di.FindControl("txtlabor_hour");

                Label lblLastName = (Label)di.FindControl("lblLastName");
                Label lbllabor_hour = (Label)di.FindControl("lbllabor_hour");


                DataRow dr = table.Rows[di.RowIndex];

                dr["labor_entry_id"] = Convert.ToInt32(grdLaborDate7.DataKeys[di.RowIndex].Values[4]);
                dr["customer_id"] = cid;
                dr["estimate_id"] = eid;
                dr["section_id"] = Convert.ToInt32(ddlSection.SelectedValue);
                dr["labor_hour"] = Convert.ToDecimal(txtlabor_hour.Text.Replace("(", "-").Replace(")", ""));
                dr["labor_date"] = Convert.ToDateTime(lbldate7.Text);
                dr["installer_id"] = Convert.ToInt32(ddlInstaller.SelectedValue);
                dr["serial"] = Convert.ToInt32(di.RowIndex);
                dr["last_name"] = txtlast_name.Text;


            }
            foreach (DataRow dr in table.Rows)
            {
                labor_hour_entry lbe = new labor_hour_entry();
                if (Convert.ToInt32(dr["labor_entry_id"]) > 0)
                    lbe = _db.labor_hour_entries.Single(l => l.labor_entry_id == Convert.ToInt32(dr["labor_entry_id"]));

                if (Convert.ToDecimal(dr["labor_hour"]) != 0)
                {
                    lbe.labor_entry_id = Convert.ToInt32(dr["labor_entry_id"]);
                    lbe.customer_id = Convert.ToInt32(dr["customer_id"]);
                    lbe.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    lbe.section_id = Convert.ToInt32(dr["section_id"]);
                    lbe.labor_hour = Convert.ToDecimal(dr["labor_hour"]);
                    lbe.labor_date = Convert.ToDateTime(dr["labor_date"]);
                    lbe.installer_id = Convert.ToInt32(dr["installer_id"]);
                    lbe.serial = Convert.ToInt32(dr["serial"]);
                    lbe.last_name = dr["last_name"].ToString();
                    if (Convert.ToInt32(dr["labor_entry_id"]) == 0)
                    {
                        _db.labor_hour_entries.InsertOnSubmit(lbe);
                    }

                }
               
            }
            _db.SubmitChanges();
        
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            DateTime dt = Convert.ToDateTime(lbldate7.Text);
            LoadLaborEntryByDate(dt, grdLaborDate7);


        }

    }

    #endregion

    protected void Load_Product_Info(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        GridViewRow diitem = ((GridViewRow)((TextBox)sender).NamingContainer);
        DropDownList ddlSection = (DropDownList)diitem.FindControl("ddlSection");
        TextBox txtlast_name = (TextBox)diitem.FindControl("txtlast_name");
        TextBox txtlabor_hour = (TextBox)diitem.FindControl("txtlabor_hour");

        Label lblLastName = (Label)diitem.FindControl("lblLastName");
        Label lbllabor_hour = (Label)diitem.FindControl("lbllabor_hour");

        lblResult.Text = "";

        if (txtlast_name.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Customer Name.");

            return;
        }
      

        string fullName = txtlast_name.Text;
        string job = string.Empty;
         var JNum = fullName.Split('(');
        if (fullName.IndexOf("(") != -1)
        {
            job = JNum[1];
 
        }
        job = job.Replace(")","");
        //string firstName = "";
        //string lastName = "";
        //string middleName = "";
       // var names = fullName.Split(' ');
      
       //if(names.Length == 1)
       //{
       //     lastName = names[0];
       //}
       //else if(names.Length == 2)
       //{
       //      firstName = names[0];
       //      lastName = names[1];
       //}
       //else if (names.Length > 2)
       //{
       //    firstName = names[0];
       //    lastName = names[2];
       //    middleName = names[1];
       //}

       // int customer_id = 0;

       // if (_db.customers.Where(ep => ep.last_name1 == lastName && ep.first_name1 == firstName).FirstOrDefault() != null)
       // {
       //     customer cus = _db.customers.First(p => p.last_name1 == lastName && p.first_name1==firstName);

       //     customer_id = cus.customer_id;
       // }
       // // grdSelectedItem.DataKeys[diitem.RowIndex].Values[0] = customer_id;
       // hdnCustomerId.Value = customer_id.ToString();
       // if (_db.customer_estimates.Where(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
       // {
       //     int nEstId = 0;
       //     var result = (from ce in _db.customer_estimates
       //                   where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == 1 && ce.status_id == 3
       //                   select ce.estimate_id);

       //     int n = result.Count();
       //     if (result != null && n > 0)
       //         nEstId = result.Max();
       //     hdnEstimateId.Value = nEstId.ToString();
       // }
        int customer_id = 0;
        int Estimate_id = 0;

        
        if (_db.customer_estimates.Where(ce => ce.job_number == job.Trim() && ce.status_id == 3).ToList().Count > 0)
        {
            customer_estimate obj = _db.customer_estimates.FirstOrDefault(ce => ce.job_number == job.Trim() && ce.status_id == 3);

            customer_id = Convert.ToInt32(obj.customer_id);
            Estimate_id = Convert.ToInt32(obj.estimate_id);
            hdnEstimateId.Value = Estimate_id.ToString();
            hdnCustomerId.Value = customer_id.ToString();

        }


        LoadSection(customer_id, Convert.ToInt32(hdnEstimateId.Value));
        ddlSection.DataSource = dtSection;
        ddlSection.DataBind();



    }
    protected void ddlInstaller_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlInstaller.ID, ddlInstaller.GetType().Name, "SelectedIndexChanged"); 
        //if (ddlInstaller.SelectedItem.Text != "Select")
        //{
        //    txtStartDate_TextChanged(sender, e);
 
        //}
        lblResult.Text = "";
        txtStartDate_TextChanged(sender, e);

    }
}