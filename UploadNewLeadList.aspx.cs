using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UploadNewLeadList : System.Web.UI.Page
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
            else
            {
                hdnDivisionName.Value = ((userinfo)Session["oUser"]).divisionName;
            }
            if (Page.User.IsInRole("lead003") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            BindSalesPerson();
            BindLeadStatus();
            BindLeadSource();


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnUpload", "lnkSampleExcelFile", "ExcelUploader" });
        }
    }

    private void BindSalesPerson()
    {

        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_id asc";

        DataTable mList = csCommonUtility.GetDataTable(strQ);
        ddlSalesPersonPopUp.DataSource = mList;
        ddlSalesPersonPopUp.DataTextField = "sales_person_name";
        ddlSalesPersonPopUp.DataValueField = "sales_person_id";
        ddlSalesPersonPopUp.DataBind();
        ddlSalesPersonPopUp.Items.Insert(0, "All");

    }

    private void BindLeadStatus()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var LeadStatus = from st in _db.lead_status
                         orderby st.lead_status_id
                         select st;


        ddlLeadStatusPopUp.DataSource = LeadStatus.Where(l => l.lead_status_name != "All").ToList();
        ddlLeadStatusPopUp.DataTextField = "lead_status_name";
        ddlLeadStatusPopUp.DataValueField = "lead_status_id";
        ddlLeadStatusPopUp.DataBind();
        ddlLeadStatusPopUp.Items.Insert(0, "Select");
        ddlLeadStatusPopUp.SelectedIndex = 0;
       
    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;

        int nItemCount = item.Count() + 1;

        ddlLeadSourcePopUp.DataSource = item;
        ddlLeadSourcePopUp.DataTextField = "lead_name";
        ddlLeadSourcePopUp.DataValueField = "lead_source_id";

        ddlLeadSourcePopUp.DataBind();
        ddlLeadSourcePopUp.Items.Insert(0, "Select");
        ddlLeadSourcePopUp.Items.Insert(1, "Create Lead Source");
        ddlLeadSourcePopUp.SelectedIndex = 0;
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        OleDbConnection oledbConn = null;
        string strTest = "";
        try
        {
            if (ExcelUploader.FileName.Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please Click 'Choose File'");
                return;
            }

            if (!(ExcelUploader.FileName.ToLower().Contains(".xlsx") || ExcelUploader.FileName.ToLower().Contains(".xls")))
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid file format.");
                return;
            }


            string sFileName = "Lead_Upload_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            if (ExcelUploader.FileName.ToLower().Contains(".xlsx"))
            {
                sFileName += ".xlsx";
            }
            else
            {
                sFileName += ".xls";
            }

            string sFilePath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"];
            if (Directory.Exists(sFilePath) == false)
            {
                Directory.CreateDirectory(sFilePath);

            }
            sFilePath = sFilePath + "\\" + sFileName;
            ExcelUploader.PostedFile.SaveAs(sFilePath);

            string sourceFile = sFilePath;

            string destinationFile = System.Configuration.ConfigurationManager.AppSettings["TempDir"];

            if (Directory.Exists(destinationFile) == false)
            {
                Directory.CreateDirectory(destinationFile);

            }
            destinationFile = destinationFile + "\\" + sFileName;


            lblResult.Text = "";


            oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sFilePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            oledbConn.Open();
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();

            // selecting distict list of Slno 
            cmd.Connection = oledbConn;

            string sSheet = GetSheetNameInExcel(sFilePath); // txtSectionName.Text.Trim(); //

            if (sSheet == "")
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No Record Found");
                return;
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM [" + sSheet + "]";
            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(ds, "Lead");
            if (ds != null)
            {
                DataTable dtLead = ds.Tables["Lead"];
                Session.Add("sdtLead", dtLead);



                #region Bloack Code
                //lblLeadColumnData0.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("first_name1")).Take(5));
                //lblLeadColumnData1.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("last_name1")).Take(5));
                //lblLeadColumnData2.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("first_name2")).Take(5));
                //lblLeadColumnData3.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("last_name2")).Take(5));
                //lblLeadColumnData4.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("company")).Take(5));
                //lblLeadColumnData5.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("email")).Take(5));
                //lblLeadColumnData6.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("email2")).Take(5));
                //lblLeadColumnData7.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("address")).Take(5));
                //lblLeadColumnData8.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("cross_street")).Take(5));
                //lblLeadColumnData9.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("city")).Take(5));
                //lblLeadColumnData10.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("state")).Take(5));
                //lblLeadColumnData11.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<double>("zip_code").ToString()).Take(5));
                //lblLeadColumnData12.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("phone")).Take(5));
                //lblLeadColumnData13.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("mobile")).Take(5));
                //lblLeadColumnData14.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("fax")).Take(5));
                //lblLeadColumnData15.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>("notes")).Take(5));

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //string col0 = dtLead.Columns[0].ColumnName;
                //lblLeadColumnName0.Text = col0;
                //lblLeadColumnData0.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col0)).Take(5));

                //string col1 = dtLead.Columns[1].ColumnName;
                //lblLeadColumnName1.Text = col1;
                //lblLeadColumnData1.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col1)).Take(5));

                //string col2 = dtLead.Columns[2].ColumnName;
                //lblLeadColumnName2.Text = col2;
                //lblLeadColumnData2.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col2)).Take(5));

                //string col3 = dtLead.Columns[3].ColumnName;
                //lblLeadColumnName3.Text = col3;
                //lblLeadColumnData3.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col3)).Take(5));

                //string col4 = dtLead.Columns[4].ColumnName;
                //lblLeadColumnName4.Text = col4;
                //lblLeadColumnData4.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col4)).Take(5));

                //string col5 = dtLead.Columns[5].ColumnName;
                //lblLeadColumnName5.Text = col5;
                //lblLeadColumnData5.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col5)).Take(5));

                //string col6 = dtLead.Columns[6].ColumnName;
                //lblLeadColumnName6.Text = col6;
                //lblLeadColumnData6.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col6)).Take(5));

                //string col7 = dtLead.Columns[7].ColumnName;
                //lblLeadColumnName7.Text = col7;
                //lblLeadColumnData7.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col7)).Take(5));

                //string col8 = dtLead.Columns[8].ColumnName;
                //lblLeadColumnName8.Text = col8;
                //lblLeadColumnData8.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col8)).Take(5));

                //string col9 = dtLead.Columns[9].ColumnName;
                //lblLeadColumnName9.Text = col9;
                //lblLeadColumnData9.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col9)).Take(5));

                //string col10 = dtLead.Columns[10].ColumnName;
                //lblLeadColumnName10.Text = col10;
                //lblLeadColumnData10.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col10)).Take(5));

                //string col11 = dtLead.Columns[11].ColumnName;
                //lblLeadColumnName11.Text = col11;
                //lblLeadColumnData11.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<double>(col11).ToString()).Take(5));

                //string col12 = dtLead.Columns[12].ColumnName;
                //lblLeadColumnName12.Text = col12;
                //lblLeadColumnData12.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col12)).Take(5));

                //string col13 = dtLead.Columns[13].ColumnName;
                //lblLeadColumnName13.Text = col13;
                //lblLeadColumnData13.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col13)).Take(5));

                //string col14 = dtLead.Columns[14].ColumnName;
                //lblLeadColumnName14.Text = col14;
                //lblLeadColumnData14.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col14)).Take(5));

                //string col15 = dtLead.Columns[15].ColumnName;
                //lblLeadColumnName15.Text = col15;
                //lblLeadColumnData15.Text = string.Join("<br/>", dtLead.AsEnumerable().Select(d => d.Field<string>(col15)).Take(5));
                #endregion
                

                string[] arrCol0 = GetColumnData(dtLead, 0);
                lblLeadColumnName0.Text = arrCol0[0];
                lblLeadColumnData0.Text = arrCol0[1];

                string[] arrCol1 = GetColumnData(dtLead, 1);
                lblLeadColumnName1.Text = arrCol1[0];
                lblLeadColumnData1.Text = arrCol1[1];

                string[] arrCol2 = GetColumnData(dtLead, 2);
                lblLeadColumnName2.Text = arrCol2[0];
                lblLeadColumnData2.Text = arrCol2[1];

                string[] arrCol3 = GetColumnData(dtLead, 3);
                lblLeadColumnName3.Text = arrCol3[0];
                lblLeadColumnData3.Text = arrCol3[1];

                string[] arrCol4 = GetColumnData(dtLead, 4);
                lblLeadColumnName4.Text = arrCol4[0];
                lblLeadColumnData4.Text = arrCol4[1];

                string[] arrCol5 = GetColumnData(dtLead, 5);
                lblLeadColumnName5.Text = arrCol5[0];
                lblLeadColumnData5.Text = arrCol5[1];

                string[] arrCol6 = GetColumnData(dtLead, 6);
                lblLeadColumnName6.Text = arrCol6[0];
                lblLeadColumnData6.Text = arrCol6[1];

                string[] arrCol7 = GetColumnData(dtLead, 7);
                lblLeadColumnName7.Text = arrCol7[0];
                lblLeadColumnData7.Text = arrCol7[1];

                string[] arrCol8 = GetColumnData(dtLead, 8);
                lblLeadColumnName8.Text = arrCol8[0];
                lblLeadColumnData8.Text = arrCol8[1];

                string[] arrCol9 = GetColumnData(dtLead, 9);
                lblLeadColumnName9.Text = arrCol9[0];
                lblLeadColumnData9.Text = arrCol9[1];

                string[] arrCol10 = GetColumnData(dtLead, 10);
                lblLeadColumnName10.Text = arrCol10[0];
                lblLeadColumnData10.Text = arrCol10[1];

                string[] arrCol11 = GetColumnData(dtLead, 11);
                lblLeadColumnName11.Text = arrCol11[0];
                lblLeadColumnData11.Text = arrCol11[1];

                string[] arrCol12 = GetColumnData(dtLead, 12);
                lblLeadColumnName12.Text = arrCol12[0];
                lblLeadColumnData12.Text = arrCol12[1];

                string[] arrCol13 = GetColumnData(dtLead, 13);
                lblLeadColumnName13.Text = arrCol13[0];
                lblLeadColumnData13.Text = arrCol13[1];

                string[] arrCol14 = GetColumnData(dtLead, 14);
                lblLeadColumnName14.Text = arrCol14[0];
                lblLeadColumnData14.Text = arrCol14[1];

                string[] arrCol15 = GetColumnData(dtLead, 15);
                lblLeadColumnName15.Text = arrCol15[0];
                lblLeadColumnData15.Text = arrCol15[0];

                string[] arrCol16 = GetColumnData(dtLead, 16);
                lblLeadColumnName16.Text = arrCol16[0];
                lblLeadColumnData16.Text = arrCol16[0];



                ModalPopupExtender2.Show();
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No Record Found");
            }

        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace) + ", " + strTest;

        }
        finally
        {
            // Clean up.
            if (oledbConn != null)
            {
                oledbConn.Close();
                oledbConn.Dispose();
            }

        }
        //System.IO.File.Move(sourceFile, destinationFile);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    
    public string[] GetColumnData(DataTable dt, int nCol)
    {

        string[] arrData = new string[2]{"",""};
        int nCount = 0;
        if (nCol < dt.Columns.Count)
        {
            foreach (DataRow dr in dt.Rows)
            {
                arrData[0] = dr.Table.Columns[nCol].ColumnName;
                arrData[1] += dr[nCol].ToString() + "<br/>";

                nCount++;
                if (nCount == 5)
                    break;
            }
        }
        return arrData;
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnImport.ID, btnImport.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        lblResultPopUp.Text = "";
        bool isSelected = false;

        string strRequired = string.Empty;

        //if (ddlSalesPersonPopUp.SelectedItem.Text == "Select")
        //{
        //    strRequired += "Missing required field: Sales Person.<br/>";
        //}

        if (ddlLeadSourcePopUp.SelectedItem.Text == "Select")
        {
            strRequired += "Missing required field: Lead Source.<br/>";
        }
        else if (ddlLeadSourcePopUp.SelectedItem.Text == "Create Lead Source")
        {
            if (txtLeadName.Text == "")
                strRequired += "Missing required field: Lead Source Name.<br/>";
        }

        if (ddlLeadStatusPopUp.SelectedItem.Text == "Select")
        {
            strRequired += "Missing required field: Lead Status.<br/>";
        }

        if (Convert.ToInt32(ddlLeadColumn0.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn1.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn2.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn3.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn4.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn5.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn6.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn7.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn8.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn9.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn10.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn11.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn12.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn13.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn14.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn15.SelectedValue) != 111)
            isSelected = true;
        if (Convert.ToInt32(ddlLeadColumn16.SelectedValue) != 111)
            isSelected = true;

        if (isSelected == false)
            strRequired += "Please Select Import Item";

        if (strRequired.Length > 0)
        {
            lblResultPopUp.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            ModalPopupExtender2.Show();
            btnSave.Focus();
            return;
        }
        List<customer> clist = new List<customer>();
        DataTable dtLead = (DataTable)Session["sdtLead"];

        lblLeadCount.Text = "Total Records: " + dtLead.Rows.Count;

        foreach (DataRow dr in dtLead.Rows)
        {
            customer objCust = new customer();

            objCust.client_id = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["client_id"]);


            if (GetIndex("0") != 111) // Do Not Import (111)
                objCust.first_name1 = dr.ItemArray[GetIndex("0")].ToString() == "" ? "TBD" : dr.ItemArray[GetIndex("0")].ToString();
            else
                objCust.first_name1 = "";

            if (GetIndex("1") != 111)
                objCust.last_name1 = dr.ItemArray[GetIndex("1")].ToString() == "" ? "TBD" : dr.ItemArray[GetIndex("1")].ToString();
            else
                objCust.last_name1 = "";

            if (GetIndex("2") != 111)
                objCust.first_name2 = dr.ItemArray[GetIndex("2")].ToString();
            else
                objCust.first_name2 = "";

            if (GetIndex("3") != 111)
                objCust.last_name2 = dr.ItemArray[GetIndex("3")].ToString();
            else
                objCust.last_name2 = "";

            if (GetIndex("4") != 111)
                objCust.company = dr.ItemArray[GetIndex("4")].ToString();
            else
                objCust.company = "";

            if (GetIndex("5") != 111)
                objCust.email = dr.ItemArray[GetIndex("5")].ToString();
            else
                objCust.email = "";

            if (GetIndex("6") != 111)
                objCust.email2 = dr.ItemArray[GetIndex("6")].ToString();
            else
                objCust.email2 = "";

            if (GetIndex("7") != 111)
                objCust.address = dr.ItemArray[GetIndex("7")].ToString();
            else
                objCust.address = "";

            if (GetIndex("8") != 111)
                objCust.cross_street = dr.ItemArray[GetIndex("8")].ToString();
            else
                objCust.cross_street = "";

            if (GetIndex("9") != 111)
                objCust.city = dr.ItemArray[GetIndex("9")].ToString();
            else
                objCust.city = "";

            if (GetIndex("10") != 111)
                objCust.state = dr.ItemArray[GetIndex("10")].ToString();
            else
                objCust.state = "";

            if (GetIndex("11") != 111)
                objCust.zip_code = dr.ItemArray[GetIndex("11")].ToString();
            else
                objCust.zip_code = "";

            if (GetIndex("12") != 111)
                objCust.phone = dr.ItemArray[GetIndex("12")].ToString();
            else
                objCust.phone = "";

            if (GetIndex("13") != 111)
                objCust.mobile = dr.ItemArray[GetIndex("13")].ToString();
            else
                objCust.mobile = "";

            if (GetIndex("14") != 111)
                objCust.fax = dr.ItemArray[GetIndex("14")].ToString();
            else
                objCust.fax = "";

            if (GetIndex("15") != 111)
                objCust.notes = dr.ItemArray[GetIndex("15")].ToString();
            else
                objCust.notes = "";

            if (GetIndex("16") != 111)
                objCust.website = dr.ItemArray[GetIndex("16")].ToString();
            else
                objCust.website = "";

            objCust.registration_date = DateTime.Now;
            objCust.update_date = DateTime.Now;
            objCust.appointment_date = Convert.ToDateTime("1900-01-01");

            objCust.sales_person_id = Convert.ToInt32(ddlSalesPersonPopUp.SelectedValue);
            objCust.lead_source_id = Convert.ToInt32(ddlLeadSourcePopUp.SelectedValue);
            objCust.lead_status_id = Convert.ToInt32(ddlLeadStatusPopUp.SelectedValue);

            objCust.is_active = true;
            objCust.status_id = 2;
            objCust.status_note = "";
            objCust.SuperintendentId = 13;
            objCust.islead = 1;
            objCust.isCustomer = 0;
            objCust.is_active = true;


            clist.Add(objCust);
        }



        DataTable dtTemp = csCommonUtility.LINQToDataTable(clist);
        Session.Add("sLeadList", dtTemp);

        pnlExcelImportMatch.Visible = false;
        pnlExcelImportedDataList.Visible = true;
        gridMatchLeadColumn.DataSource = clist.Take(15);
        gridMatchLeadColumn.DataBind();



        ModalPopupExtender2.Show();
        btnImport.Visible = false;
        btnSave.Visible = true;
        btnBack.Visible = true;
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    public int GetIndex(string sv)
    {
        int index = 111;
        if (ddlLeadColumn0.SelectedItem.Value == sv)
        {
            index = 0;
        }
        else if (ddlLeadColumn1.SelectedItem.Value == sv)
        {
            index = 1;
        }
        else if (ddlLeadColumn2.SelectedItem.Value == sv)
        {
            index = 2;
        }
        else if (ddlLeadColumn3.SelectedItem.Value == sv)
        {
            index = 3;
        }
        else if (ddlLeadColumn4.SelectedItem.Value == sv)
        {
            index = 4;
        }
        else if (ddlLeadColumn5.SelectedItem.Value == sv)
        {
            index = 5;
        }
        else if (ddlLeadColumn6.SelectedItem.Value == sv)
        {
            index = 6;
        }
        else if (ddlLeadColumn7.SelectedItem.Value == sv)
        {
            index = 7;
        }
        else if (ddlLeadColumn8.SelectedItem.Value == sv)
        {
            index = 8;
        }
        else if (ddlLeadColumn9.SelectedItem.Value == sv)
        {
            index = 9;
        }
        else if (ddlLeadColumn10.SelectedItem.Value == sv)
        {
            index = 10;
        }
        else if (ddlLeadColumn11.SelectedItem.Value == sv)
        {
            index = 11;
        }
        else if (ddlLeadColumn12.SelectedItem.Value == sv)
        {
            index = 12;
        }
        else if (ddlLeadColumn13.SelectedItem.Value == sv)
        {
            index = 13;
        }
        else if (ddlLeadColumn14.SelectedItem.Value == sv)
        {
            index = 14;
        }
        else if (ddlLeadColumn15.SelectedItem.Value == sv)
        {
            index = 15;
        }
        else if (ddlLeadColumn16.SelectedItem.Value == sv)
        {
            index = 16;
        }


        return index;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable dtLead = (DataTable)Session["sLeadList"];

        foreach (DataRow dr in dtLead.Rows)
        {
            customer objCust = new customer();

            objCust.client_id = Convert.ToInt32(dr["client_id"]);
            objCust.first_name1 = dr["first_name1"].ToString();
            objCust.last_name1 = dr["last_name1"].ToString();
            objCust.first_name2 = dr["first_name2"].ToString();
            objCust.last_name2 = dr["last_name2"].ToString();
            objCust.company = dr["company"].ToString();
            objCust.email = dr["email"].ToString();
            objCust.email2 = dr["email2"].ToString();
            objCust.address = dr["address"].ToString();
            objCust.cross_street = dr["cross_street"].ToString();
            objCust.city = dr["city"].ToString();
            objCust.state = dr["state"].ToString();
            objCust.zip_code = dr["zip_code"].ToString();
            objCust.phone = dr["phone"].ToString();
            objCust.mobile = dr["mobile"].ToString();
            objCust.fax = dr["fax"].ToString();
            objCust.notes = dr["notes"].ToString();
            objCust.website = dr["website"].ToString();

            objCust.registration_date = Convert.ToDateTime(dr["registration_date"]);
            objCust.update_date = Convert.ToDateTime(dr["update_date"]);
            objCust.appointment_date = Convert.ToDateTime(dr["appointment_date"]);

            objCust.sales_person_id = Convert.ToInt32(dr["sales_person_id"]);
            objCust.lead_source_id = Convert.ToInt32(dr["lead_source_id"]);
            objCust.lead_status_id = Convert.ToInt32(dr["lead_status_id"]);

            objCust.is_active = true;
            objCust.status_id = 2;
            objCust.status_note = "";
            objCust.SuperintendentId = 13;
            objCust.islead = 1;
            objCust.isCustomer = 0;
            objCust.is_active = true;
            objCust.isCalendarOnline = true;
            objCust.isJobSatusViewable = true;
            objCust.CustomerCalendarWeeklyView = 1;

            _db.customers.InsertOnSubmit(objCust);
        }
        _db.SubmitChanges();
        ResetUpload();
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
        Response.Redirect("leadlist.aspx");
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnBack.ID, btnBack.GetType().Name, "Click"); 
        ModalPopupExtender2.Show();
        pnlExcelImportMatch.Visible = true;
        pnlExcelImportedDataList.Visible = false;

        Session.Add("sLeadList", null);

        gridMatchLeadColumn.DataSource = null;
        gridMatchLeadColumn.DataBind();
        btnImport.Visible = true;
        btnSave.Visible = false;
        btnBack.Visible = false;
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        ResetUpload();
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    public void ResetUpload()
    {
        ExcelUploader = new FileUpload();

        Session["sdtLead"] = null;

        btnImport.Visible = true;
        btnSave.Visible = false;

        pnlExcelImportMatch.Visible = true;
        pnlExcelImportedDataList.Visible = false;
        gridMatchLeadColumn.DataSource = null;
        gridMatchLeadColumn.DataBind();



        ddlLeadColumn0.SelectedValue = "111";
        ddlLeadColumn1.SelectedValue = "111";
        ddlLeadColumn2.SelectedValue = "111";
        ddlLeadColumn3.SelectedValue = "111";
        ddlLeadColumn4.SelectedValue = "111";
        ddlLeadColumn5.SelectedValue = "111";
        ddlLeadColumn6.SelectedValue = "111";
        ddlLeadColumn7.SelectedValue = "111";
        ddlLeadColumn8.SelectedValue = "111";
        ddlLeadColumn9.SelectedValue = "111";
        ddlLeadColumn10.SelectedValue = "111";
        ddlLeadColumn11.SelectedValue = "111";
        ddlLeadColumn12.SelectedValue = "111";
        ddlLeadColumn13.SelectedValue = "111";
        ddlLeadColumn14.SelectedValue = "111";
        ddlLeadColumn15.SelectedValue = "111";
        ddlLeadColumn16.SelectedValue = "111";


        lblLeadColumnData0.Text = "";
        lblLeadColumnData1.Text = "";
        lblLeadColumnData2.Text = "";
        lblLeadColumnData3.Text = "";
        lblLeadColumnData4.Text = "";
        lblLeadColumnData5.Text = "";
        lblLeadColumnData6.Text = "";
        lblLeadColumnData7.Text = "";
        lblLeadColumnData8.Text = "";
        lblLeadColumnData9.Text = "";
        lblLeadColumnData10.Text = "";
        lblLeadColumnData11.Text = "";
        lblLeadColumnData12.Text = "";
        lblLeadColumnData13.Text = "";
        lblLeadColumnData14.Text = "";
        lblLeadColumnData15.Text = "";
        lblLeadColumnData16.Text = "";

        BindSalesPerson();
        BindLeadStatus();
        BindLeadSource();
        ModalPopupExtender2.Hide();
    }

    public string GetSheetNameInExcelByName(string filePath, string strSheetName)
    {
        string sSheetName = "";
        OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
        String strExtendedProperties = String.Empty;
        sbConnection.DataSource = filePath;
        sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
        strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
        sbConnection.Add("Extended Properties", strExtendedProperties);
        using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
        {
            conn.Open();
            DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            foreach (DataRow drSheet in dtSheet.Rows)
            {
                if (drSheet["TABLE_NAME"].ToString().Trim().Contains(strSheetName + "$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                {
                    sSheetName = drSheet["TABLE_NAME"].ToString();
                    break;
                }
            }

            conn.Close();
            conn.Dispose();


        }
        return sSheetName;
    }

    public string GetSheetNameInExcel(string filePath)
    {
        string sSheetName = "";
        OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
        String strExtendedProperties = String.Empty;
        sbConnection.DataSource = filePath;
        sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
        strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
        sbConnection.Add("Extended Properties", strExtendedProperties);
        using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
        {
            conn.Open();
            DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            foreach (DataRow drSheet in dtSheet.Rows)
            {
                if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                {
                    sSheetName = drSheet["TABLE_NAME"].ToString();
                    break;
                }
            }

            conn.Close();
            conn.Dispose();


        }
        return sSheetName;
    }

    protected void ddlLeadSourcePopUp_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlLeadSourcePopUp.ID, ddlLeadSourcePopUp.GetType().Name, "SelectedIndexChanged"); 
        lblResult2.Text = "";
        if (ddlLeadSourcePopUp.SelectedItem.Text == "Create Lead Source")
        {
            divLeadSource.Visible = true;
            txtLeadName.Focus();
        }
        else
        {
            divLeadSource.Visible = false;
            ddlLeadSourcePopUp.Focus();
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }


    protected void btnSaveLeadSource_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveLeadSource.ID, btnSaveLeadSource.GetType().Name, "Click"); 
        try
        {
            lblResult.Text = "";
            lblResult2.Text = "";

            if (txtLeadName.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Lead Source Name.");

                return;
            }
            DataClassesDataContext _db = new DataClassesDataContext();

            // Lead Source Name Does Exist
            lead_source ls = new lead_source();

            if (_db.lead_sources.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.lead_name == txtLeadName.Text).SingleOrDefault() != null)
            {
                lblResult2.Text = csCommonUtility.GetSystemErrorMessage("Lead source name '" + txtLeadName.Text.Trim() + "' already exist. Please try another name.");

                txtLeadName.Focus();
                return;
            }
            ls.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            ls.lead_name = txtLeadName.Text.Trim();
            ls.description = "";
            ls.is_active = true;

            _db.lead_sources.InsertOnSubmit(ls);



            _db.SubmitChanges();

            BindLeadSource();

            ListItem ddlList = ddlLeadSourcePopUp.Items.FindByValue(ls.lead_source_id.ToString());
            int nIndex = ddlLeadSourcePopUp.Items.IndexOf(ddlList);
            ddlLeadSourcePopUp.SelectedIndex = nIndex;

            divLeadSource.Visible = false;
            ddlLeadSourcePopUp.Focus();

            lblResult2.Text = csCommonUtility.GetSystemMessage("Lead source '" + txtLeadName.Text.Trim() + "' has been saved successfully.");
            txtLeadName.Text = "";
        }
        catch (Exception ex)
        {
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    protected void lnkSampleExcelFile_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkSampleExcelFile.ID, lnkSampleExcelFile.GetType().Name, "Click"); 
        try
        {
            string fileName = "Lead Sample.xlsx";
            string filePath = "~/File/Sample/" + fileName;
            Response.Clear();
            Response.ContentType = "Application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(Server.MapPath(filePath));

            HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
            HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            HttpContext.Current.ApplicationInstance.CompleteRequest();  // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.


        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    protected void ddlLeadColumn_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        ModalPopupExtender2.Show();




        //if (ddl.ClientID.ToString() == ddlLeadColumn1.ClientID.ToString())
        //{
        //    ddlLeadColumn1.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn1.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn2.ClientID.ToString())
        //{
        //    ddlLeadColumn2.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn2.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn3.ClientID.ToString())
        //{
        //    ddlLeadColumn3.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn3.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn4.ClientID.ToString())
        //{
        //    ddlLeadColumn4.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn4.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn5.ClientID.ToString())
        //{
        //    ddlLeadColumn5.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn5.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn6.ClientID.ToString())
        //{
        //    ddlLeadColumn6.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn6.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn7.ClientID.ToString())
        //{
        //    ddlLeadColumn7.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn7.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn8.ClientID.ToString())
        //{
        //    ddlLeadColumn8.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn8.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn9.ClientID.ToString())
        //{
        //    ddlLeadColumn9.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn9.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        ////
        //if (ddl.ClientID.ToString() == ddlLeadColumn10.ClientID.ToString())
        //{
        //    ddlLeadColumn10.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = true;
        //}
        //else
        //{
        //    ddlLeadColumn10.Items[Convert.ToInt32(ddl.SelectedValue)].Enabled = false;
        //}

        // ModalPopupExtender2.Show();
        // ddlLeadColumn1.Items[i].Enabled = false;
    }
}