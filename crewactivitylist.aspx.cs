using DataStreams.Csv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class crewactivitylist : System.Web.UI.Page
{
    public DataTable dtSection;
    int nPageNo = 0;
    int nPageNoEmp = 0;
    string selectedvalue = "";
    string selectedValueEmp = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("t04") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            if (Request.QueryString.Get("isCrew") != null)
            {
                int IsCrew = Convert.ToInt32(Request.QueryString.Get("isCrew"));
               
            }
            
            txtStartDate1.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtEndDate1.Text = DateTime.Now.ToString("MM/dd/yyyy");
            BindCrew();
            /// BindEmployee();

            if (Session["CPUFilter1"] != null)
            {
                Hashtable ht = (Hashtable)Session["CPUFilter1"];
                selectedvalue = "";
                ddlItemPerPage.SelectedIndex = Convert.ToInt32(ht["ItemPerPage"].ToString());
                string crewId = ht["Installer"].ToString();
                if (crewId.Contains(','))
                {
                    string[] ary = crewId.Split(',');
                    foreach (ListItem item in lstCrew.Items)
                    {
                        foreach (var a in ary)
                        {
                            if (a == item.Value)
                            {
                                item.Selected = true;
                                selectedvalue += item.Value + ",";
                            }
                        }

                    }
                }


                nPageNo = Convert.ToInt32(ht["PageNo"].ToString());
                //txtStartDate.Text = ht["StartDate"].ToString();
                // txtEndDate.Text = ht["EndDate"].ToString();


            }

            //if (Session["CPUFilter1Epm"] != null)
            //{
            //    Hashtable htemp = (Hashtable)Session["CPUFilter1Epm"];
            //    ddlItemPerPageEmployee.SelectedIndex = Convert.ToInt32(htemp["ItemPerPageEmp"].ToString());
            //    selectedValueEmp = "";
            //    string employeeId = htemp["Employee"].ToString();
            //    if (employeeId.Contains(','))
            //    {
            //        string[] ary = employeeId.Split(',');
            //        foreach (ListItem item in lstEmployee.Items)
            //        {
            //            foreach (var a in ary)
            //            {
            //                if (a == item.Value)
            //                {
            //                    item.Selected = true;
            //                    selectedValueEmp += item.Value + ",";
            //                }
            //            }

            //        }
            //    }
            //    // ddEmployee.SelectedValue = htemp["Employee"].ToString();
            //    nPageNoEmp = Convert.ToInt32(htemp["PageNoEmp"].ToString());
            //    txtEmployeeStartDate.Text = htemp["StartDateEmp"].ToString();
            //    txtEmployeeEndDate.Text = htemp["EndDateEmp"].ToString();


            //}
            BindLaborHourTracking();
            //BindEmployeeLaborHourTracking();  


        }
    }



    private void BindLaborHourTracking()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = string.Empty;
            strCondition = " where 1=1 ";
            selectedvalue = "";

            foreach (ListItem item in lstCrew.Items)
            {
                if (item.Selected)
                {
                    selectedvalue += item.Value + ",";
                }
            }
            if (selectedvalue.Length > 0)
                strCondition += " AND CR.CrewIId in (" + selectedvalue.TrimEnd(',') + " )";

            if (txtStartDate1.Text.Trim() != "" && txtEndDate1.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate1.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate1.Text.Trim());
                strCondition += " AND CR.ScheduleTime >= '" + dt1 + "' and CR.ScheduleTime <'" + dt2.AddDays(1) + "'";
            }

            //string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
            //        " [SectionName],StartTime, EndTime,[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress from GPSTracking  " + strCondition + " order by labor_date desc ";

            string strQ = "Select Distinct CR.CrewIId,CR.EventId,CR.CustomerId,CR.ScheduleTime,CR.ProcessRunTime,CR.CrewLatitude,CR.CrewLongitude,CR.CustLatitude,CR.CustLongitude,(CR.Distance*0.000621371) as Distance,CR.StatusType,CR.Status, " +
                          " c.address + ', ' + c.city + ', ' + c.state + ' ' + c.zip_code As CustomerAddress, CD.first_name + ' ' + CD.last_name AS crew_name " +
                          " from CrewActivity AS CR INNER JOIN customers AS c ON CR.customerid = c.customer_id " +
                          " inner join Crew_Details AS CD ON CR.CrewIId = CD.crew_id " + strCondition + " order by CR.ScheduleTime desc ";
            IEnumerable<csCrewActivity> clist = _db.ExecuteQuery<csCrewActivity>(strQ, string.Empty);

            //DataTable kk = csCommonUtility.LINQToDataTable(clist);

            Session.Add("nCrewActivityList", csCommonUtility.LINQToDataTable(clist));

            if (Session["CPUFilter1"] != null)
            {
                Hashtable ht = (Hashtable)Session["CPUFilter1"];
                nPageNo = Convert.ToInt32(ht["PageNo"].ToString());
                GetLaberTracking(nPageNo);
            }
            else
            {
                GetLaberTracking(0);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void BindCrew()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS crew_name,crew_id from Crew_Details WHERE is_active = 1 order by crew_name asc";
        List<CrewDe> mList = _db.ExecuteQuery<CrewDe>(strQ, string.Empty).ToList();
        //ddlInstaller.DataSource = mList;
        //ddlInstaller.DataTextField = "crew_name";
        //ddlInstaller.DataValueField = "crew_id";
        //ddlInstaller.DataBind();
        //ddlInstaller.Items.Insert(0, "All");

        lstCrew.DataSource = mList;
        lstCrew.DataTextField = "crew_name";
        lstCrew.DataValueField = "crew_id";
        lstCrew.DataBind();
    }


    protected void GetLaberTracking(int nPageNo)
    {

        if (Session["nCrewActivityList"] != null)
        {
            DataTable dtLaborHour = (DataTable)Session["nCrewActivityList"];
            grdLaberTrack.DataSource = dtLaborHour;
            grdLaberTrack.PageIndex = nPageNo;
            grdLaberTrack.AllowPaging = true;
            grdLaberTrack.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            grdLaberTrack.DataKeyNames = new string[] { "CrewIId", "EventId", "CustomerId", "ScheduleTime", "ProcessRunTime", "CrewLatitude", "CrewLongitude", "CustLatitude", "CustLongitude", "Distance", "StatusType", "Status", "CustomerAddress", "crew_name" };
            grdLaberTrack.DataBind();


            Hashtable ht = new Hashtable();
            ht.Add("ItemPerPage", ddlItemPerPage.SelectedIndex);
            if (selectedvalue.Length > 0)
                ht.Add("Installer", selectedvalue);
            else
                ht.Add("Installer", 0);
            ht.Add("PageNo", nPageNo);
            ht.Add("StartDate", txtStartDate1.Text);
            ht.Add("EndDate", txtEndDate1.Text);

            Session["CPUFilter1"] = ht;
        }

        lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;
            btnPrevious0.Enabled = false;
        }
        else
        {
            btnPrevious.Enabled = true;
            btnPrevious0.Enabled = true;
        }

        if (grdLaberTrack.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;
            btnNext0.Enabled = false;
        }
        else
        {
            btnNext.Enabled = true;
            btnNext0.Enabled = true;
        }
    }

    protected void ButtonShowLocation_Click(object sender, System.EventArgs e)
    {
        ImageButton lnkRowSelection = (ImageButton)sender;
        string[] arguments = lnkRowSelection.CommandArgument.Split(';');
        string CrewLatitude = arguments[0];
        string CrewLongitude = arguments[1];
        string CustLatitude = arguments[2];
        string CustLongitude = arguments[3];

        string crew_name = arguments[4];
        string Status = arguments[5];
        string Distance = arguments[6];
        string CustomerAddress = arguments[7];
        string ScheduleTime = arguments[8];
        string CAddress = CustomerAddress.Replace("\n", " ").Replace('#', ' ');
        //System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('gCusCrewTracker.aspx?Condition=" + strCondition + "' ,'_blank');", true);
        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('gCrewActivity.aspx?CrewLatitude=" + CrewLatitude + "&CrewLongitude=" + CrewLongitude + "&CustLatitude=" + CustLatitude + "&CustLongitude=" + CustLongitude + "&crew_name=" + crew_name + "&Status=" + Status + "&Distance=" + Distance + "&CustomerAddress=" + CAddress + "&ScheduleTime=" + ScheduleTime + "' ,'_blank');", true);
        //System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModal", "window.open('gCrewActivity.aspx?CrewLatitude={0}&CrewLongitude={1}&CustLatitude={2}&CustLongitude={3}&crew_name={4}&Status={5}&Distance={6}&CustomerAddress={7}&ScheduleTime={8}' ,'_blank');", true);
        //Response.Redirect(string.Format("gCrewActivity.aspx?CrewLatitude=" + CrewLatitude + "&CrewLongitude=" + CrewLongitude + "&CustLatitude=" + CustLatitude + "&CustLongitude=" + CustLongitude + "&crew_name=" + crew_name + "&Status=" + Status + "&Distance=" + Distance + "&CustomerAddress=" + CustomerAddress + "&ScheduleTime=" + ScheduleTime + "' ,'_blank');", true));
    }


    protected void grdLaberTrack_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                double CrewLatitude = Convert.ToDouble(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[5].ToString());
                double CrewLongitude = Convert.ToDouble(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[6].ToString());
                double CustLatitude = Convert.ToDouble(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[7].ToString());
                double CustLongitude = Convert.ToDouble(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[8].ToString());
                double Distance = Convert.ToDouble(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[9].ToString());


                if (CrewLatitude == 0 || CrewLongitude == 0 || CustLatitude == 0 || CustLongitude == 0)
                {
                    ((ImageButton)e.Row.FindControl("imgDelete")).Visible = false;
               
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("imgDelete")).Visible = true;
                   
                }

                if (Distance == 0)
                {
                    ((Label)e.Row.FindControl("lblDistance")).Text = "";
                }
                else
                {
                    ((Label)e.Row.FindControl("lblDistance")).Text = Distance.ToString("F");
                }
                


            }
            catch (Exception ex)
            { throw ex; }
        }
    }

    //protected void DeleteFile(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        userinfo obj = (userinfo)Session["oUser"];
    //        ImageButton imgDelete = (ImageButton)sender;
    //        int nGPSId = Convert.ToInt32(imgDelete.CommandArgument);
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        string strQ = "";
    //        lblResult.Text = "";
    //       // lblResultEmp.Text = "";

    //        strQ = "insert into DeleteGPSTracking " +
    //                      " SELECT [GPSTrackID] ,[StartPlace] ,[StartLatitude],[StartLogitude] ,[EndLatitude] ,[EndLogitude],[MakeStopPlace], " +
    //                      " [EndPlace],[Distance],[Time],[CreatedDate] ,[UserID] ,[CustomerName] ,[section_id],[SectionName] ,[StartTime], " +
    //                      " [EndTime],[customer_id],[Estimate_id],[labor_date],[deviceName],[IsCrew]," + "'" + obj.username + "'" + ",getdate() " +
    //                      " FROM GPSTracking " +
    //                      " WHERE GPSTrackID ='" + nGPSId + "'";
    //        _db.ExecuteCommand(strQ, string.Empty);

    //         strQ = "delete from GPSTracking where GPSTrackID=" + nGPSId;
    //        _db.ExecuteCommand(strQ, string.Empty);


    //        var nList = (from dgps in _db.GPSTrackingDetails where dgps.GPSTrackID == nGPSId select dgps).ToList();

    //        DeleteGPSTrackingDetail objDGPS = null;

    //        foreach(var li in nList)
    //        {
    //            objDGPS = new DeleteGPSTrackingDetail();
    //            objDGPS.ID = li.ID;
    //            objDGPS.GPSTrackID = li.GPSTrackID;
    //            objDGPS.Latitude = li.Latitude;
    //            objDGPS.Longitude = li.Longitude;
    //            objDGPS.InputType = li.InputType;
    //            objDGPS.CreateDate = li.CreateDate;

    //            _db.DeleteGPSTrackingDetails.InsertOnSubmit(objDGPS);
    //            _db.SubmitChanges();
    //        }
    //        strQ = "delete from GPSTrackingDetails where GPSTrackID=" + nGPSId;
    //        _db.ExecuteCommand(strQ, string.Empty);
    //        lblResult.Text = csCommonUtility.GetSystemMessage("Labor tracking has been deleted successfully.");
    //        BindLaborHourTracking();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}


    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click");
        string strRequired = string.Empty;
        lblResult.Text = "";
        //lblResultEmp.Text = "";
        try
        {
            Convert.ToDateTime(txtStartDate1.Text.Trim());
        }
        catch
        {
            strRequired += "Start Date is required.<br/>";

        }

        try
        {
            Convert.ToDateTime(txtEndDate1.Text.Trim());
            //if (Convert.ToDateTime(txtStartDate.Text.Trim()) >= Convert.ToDateTime(txtEndDate.Text.Trim()))
            //{
            //    strRequired += "End Date must be greater than Start Date.<br/>";

            //}
        }
        catch
        {
            strRequired += "End Date is required.<br/>";

        }
        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            return;
        }
        //  GetLaberTracking(0);
        //if (chkTotalhours.Checked)
        //{
        //    BindSubTotalHours();
        //}
        //else
        //{
        BindLaborHourTracking();
        //}

    }

    //private void BindSubTotalHours()
    //{
    //    try
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        string strCondition = string.Empty;
    //        strCondition = " where IsCrew=1 ";
    //        selectedvalue = "";

    //        foreach (ListItem item in lstCrew.Items)
    //        {
    //            if (item.Selected)
    //            {
    //                selectedvalue += item.Value + ",";
    //            }
    //        }
    //        if (selectedvalue.Length > 0)
    //            strCondition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";

    //        if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
    //        {

    //            DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
    //            DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
    //            strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
    //        }

    //        string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                     " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress from GPSTracking  " + strCondition + " order by UserID,labor_date DESC ";
    //        IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);

    //        DataTable dataTable = csCommonUtility.LINQToDataTable(clist);

    //        if (dataTable.Rows.Count > 0)
    //        {
    //            grdLaberTrack.DataSource = dataTable;
    //            grdLaberTrack.PageIndex = 0;
    //            grdLaberTrack.AllowPaging = false;
    //            grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress" };
    //            grdLaberTrack.DataBind();
    //        }
    //        else
    //        {
    //            grdLaberTrack.DataSource = null;
    //            grdLaberTrack.PageIndex = 0;
    //            grdLaberTrack.AllowPaging = false;
    //            grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress" };
    //            grdLaberTrack.DataBind();
    //        }


    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    //protected void grdLaberTrack_DataBound(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        //if (chkTotalhours.Checked)
    //       // {
    //            for (int i = subTotalRowIndex; i < grdLaberTrack.Rows.Count; i++)
    //            {

    //                DateTime StartTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[12].Text);
    //                string eTime = (grdLaberTrack.Rows[i].Cells[13].Text).ToString();
    //                if (eTime != null && eTime != "")
    //                {
    //                    DateTime EndTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[13].Text);

    //                    if (EndTime.Year != 2000)
    //                    {
    //                        StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                        StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                        EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                        EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                        TimeSpan span = EndTime.Subtract(StartTime);
    //                        decimal totalMinutes = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                        subTotal += totalMinutes;
    //                    }
    //                }
    //            }
    //            if (subTotal >= 60)
    //            {
    //                hour = (int)subTotal / 60;
    //                subTotal = subTotal % 60;
    //            }
    //            else
    //            {
    //                hour = 0;
    //                subTotal = subTotal % 60;
    //            }
    //            if (subTotal < 12)
    //                TotalM = hour.ToString() + ":0" + subTotal;
    //            else
    //                TotalM = hour.ToString() + ":" + subTotal;
    //            if (grdLaberTrack.Rows.Count > 0)
    //                this.AddTotalRow("Total", TotalM);

    //       // }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //}

    int currentId = 0;
    decimal subTotal = 0;
    int subTotalRowIndex = 0;
    int hour = 0;
    string TotalM = "";

    //protected void grdLaberTrack_RowCreated(object sender, GridViewRowEventArgs e)
    //{
    //    try
    //    {
    //       // if (chkTotalhours.Checked == true)
    //       // {

    //            subTotal = 0;
    //            if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
    //            {
    //                if (e.Row.DataItem is DataRowView && (e.Row.DataItem as DataRowView).DataView.Table != null)
    //                {
    //                    DataTable dt = (e.Row.DataItem as DataRowView).DataView.ToTable(); //(e.Row.DataItem as DataRowView).DataView.Table;
    //                    int UserID = Convert.ToInt32(dt.Rows[e.Row.RowIndex]["UserID"]);

    //                    if (UserID != currentId)
    //                    {
    //                        if (e.Row.RowIndex > 0)
    //                        {
    //                            for (int i = subTotalRowIndex; i < e.Row.RowIndex; i++)
    //                            {
    //                                DateTime StartTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[12].Text);
    //                                string eTime = (grdLaberTrack.Rows[i].Cells[13].Text).ToString();
    //                                if (eTime != null && eTime != "")
    //                                {
    //                                    DateTime EndTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[13].Text);
    //                                    if (EndTime.Year != 2000)
    //                                    {
    //                                        StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                                        StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                                        EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                                        EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                                        TimeSpan span = EndTime.Subtract(StartTime);
    //                                        decimal totalMinutes = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                                        subTotal += totalMinutes;
    //                                    }
    //                                }
    //                            }

    //                            if (subTotal >= 60)
    //                            {
    //                                hour = (int)subTotal / 60;
    //                                subTotal = subTotal % 60;
    //                            }
    //                            else
    //                            {
    //                                hour = 0;
    //                                subTotal = subTotal % 60;
    //                            }


    //                            if (subTotal < 12)
    //                                TotalM = hour.ToString() + ":0" + subTotal;
    //                            else
    //                                TotalM = hour.ToString() + ":" + subTotal;

    //                            if (grdLaberTrack.Rows.Count > 0)
    //                                this.AddTotalRow("Total", TotalM);

    //                            subTotalRowIndex = e.Row.RowIndex;
    //                        }
    //                        currentId = UserID;
    //                    }

    //                }
    //            }
    //        //}
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    //private void AddTotalRow(string labelText, string value)
    //{
    //    GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
    //    row.BackColor = ColorTranslator.FromHtml("#F9F9F9");

    //    row.Cells.AddRange(new TableCell[12] { new TableCell (), new TableCell (),new TableCell (), new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),//Empty Cell
    //                                    new TableCell { Text = labelText, HorizontalAlign = HorizontalAlign.Right,CssClass="cellColor"},
    //                                    new TableCell { Text = value, HorizontalAlign = HorizontalAlign.Center,CssClass="cellColor" },new TableCell () });

    //    grdLaberTrack.Controls[0].Controls.Add(row);
    //}
    //protected void ddlInstaller_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //   // KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlInstaller.ID, ddlInstaller.GetType().Name, "Changed");
    //    // chkTotalhours.Checked = false;
    //    lblResult.Text = "";
    //    // lblResultEmp.Text = "";
    //     if (chkTotalhours.Checked == true)
    //     {
    //         BindSubTotalHours();
    //        // chkEmployeeTotalHours.Checked = false;
    //     }
    //     else
    //     {
    //         chkTotalhours.Checked = false;
    //         BindLaborHourTracking();
    //     }
    //}
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        lblResult.Text = "";
        // ddlInstaller.SelectedIndex = -1;
        txtStartDate1.Text = "";
        txtEndDate1.Text = "";
        //chkTotalhours.Checked = false;
        Session.Remove("Installer");
        Session.Remove("");
        if (Session["CPUFilter1"] != null)
        {
            Session.Remove("CPUFilter1");

        }
        Session.Remove("Installer");
        Session.Remove("Employee");
        selectedvalue = "";
        selectedValueEmp = "";
        BindCrew();
        //BindEmployee();
        ddlItemPerPage.SelectedIndex = 0;
        BindLaborHourTracking();

    }

    protected void grdLaberTrack_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLaberTrack.ID, grdLaberTrack.GetType().Name, "Changed");
        GetLaberTracking(e.NewPageIndex);
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLaberTracking(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLaberTracking(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "Changed");
        //chkTotalhours.Checked = false;
        GetLaberTracking(0);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("laborhourdetails.aspx?isCrew=1");
    }

    //protected void btnExpCustList_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click");
    //    DataTable tmpTable = LoadTmpDataTable();
    //     DataClassesDataContext _db = new DataClassesDataContext();

    //     string strRequired = string.Empty;
    //     lblResult.Text = "";
    //    string strCondition = string.Empty;
    //    strCondition = " where IsCrew=1 ";
    //    selectedvalue = "";
    //    foreach (ListItem item in lstCrew.Items)
    //    {
    //        if (item.Selected)
    //        {
    //            selectedvalue += item.Value + ",";
    //        }
    //    }
    //    if (selectedvalue.Length > 0)
    //        strCondition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";

    //    if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
    //    {

    //        try
    //        {
    //            Convert.ToDateTime(txtStartDate.Text.Trim());
    //        }
    //        catch
    //        {
    //            strRequired += "Start Date is required.<br/>";

    //        }

    //        try
    //        {
    //            Convert.ToDateTime(txtEndDate.Text.Trim());
    //            if (Convert.ToDateTime(txtStartDate.Text.Trim()) >= Convert.ToDateTime(txtEndDate.Text.Trim()))
    //            {
    //                strRequired += "End Date must be greater than Start Date.<br/>";

    //            }
    //        }
    //        catch
    //        {
    //            strRequired += "End Date is required.<br/>";

    //        }
    //        if (strRequired.Length > 0)
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
    //            return;
    //        }


    //        DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
    //        DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
    //        strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";



    //    }



    //    string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[CreatedDate],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                 " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],StartCustomerAddress,EndCustomerAddress from GPSTracking  " + strCondition + " order by labor_date desc ";
    //      DataTable dtReport = csCommonUtility.GetDataTable(strQ);
    //    if (dtReport.Rows.Count> 0)
    //    {
    //        foreach (DataRow dr in dtReport.Rows)
    //        {   string StartPlace = "";
    //            string EndPlace = "";
    //            DataRow drNew = tmpTable.NewRow();
    //            StartPlace = dr["StartPlace"].ToString();
    //            EndPlace = dr["EndPlace"].ToString();
    //            Crew_Detail objCrew = new Crew_Detail();
    //            objCrew = _db.Crew_Details.SingleOrDefault(c => c.crew_id == Convert.ToInt32(dr["UserID"]));
    //            drNew["Labor Date"] = Convert.ToDateTime(dr["labor_date"]).ToString("MM-dd-yyyy");
    //            if (objCrew != null)
    //                drNew["Crew Member Name"] = objCrew.full_name;
    //            drNew["Customer Name"] = dr["CustomerName"].ToString();
    //            if (dr["SectionName"].ToString() != "Select")
    //                drNew["Type"] = dr["SectionName"].ToString();
    //            if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
    //            {
    //                drNew["Start Location"] = StartPlace.Remove(StartPlace.Length - 5, 5);
    //            }
    //            else
    //            {
    //                if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
    //                {
    //                    drNew["Start Location"] = "";
    //                }
    //                else
    //                {
    //                    drNew["Start Location"] = StartPlace;
    //                }
    //            }
    //         if (EndPlace == "0")
    //            {
    //                drNew["End Location"] = "";
    //            }
    //            else
    //            {
    //                if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
    //                {
    //                    drNew["End Location"] = EndPlace.Remove(EndPlace.Length - 5, 5);
    //                }
    //                else
    //                {
    //                    drNew["End Location"] = EndPlace;
    //                }
    //            }

    //         drNew["Start Time"] = Convert.ToDateTime(dr["StartTime"]).ToShortTimeString();
    //         if (Convert.ToDateTime(dr["EndTime"]).Year != 2000)
    //             drNew["End Time"] = Convert.ToDateTime(dr["EndTime"]).ToShortTimeString();

    //            drNew["Lunch Start Time"] = "12 PM";
    //            drNew["Lunch End Time"] = "12:30 PM";

    //            drNew["Lunch End Time"] = "12:30 PM";
    //            drNew["Lunch (Hrs)"] = ".50";

    //            if (Convert.ToDateTime(dr["EndTime"]).Year!=2000)
    //            {
    //                DateTime  StartTime = Convert.ToDateTime(dr["StartTime"].ToString());
    //                DateTime EndTime = Convert.ToDateTime(dr["EndTime"].ToString());

    //                StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                TimeSpan span = EndTime.Subtract(StartTime);

    //                if (span.Days > 0)
    //                    drNew["Total (Hr:Min)"] = span.Days + ":" + span.Hours + ":" + span.Minutes;
    //                else
    //                    drNew["Total (Hr:Min)"] = span.ToString(@"hh\:mm");

    //             }
    //            else
    //            {

    //                drNew["Total (Hr:Min)"] = "";
    //            }
    //            drNew["Customer Start Address"] = dr["StartCustomerAddress"].ToString();
    //            drNew["Customer End Address"] = dr["EndCustomerAddress"].ToString();
    //            tmpTable.Rows.Add(drNew);
    //        }

    //        Response.Clear();
    //        Response.ClearHeaders();

    //        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
    //        {
    //            writer.WriteAll(tmpTable, true);
    //        }
    //        Response.ContentType = "application/vnd.ms-excel";
    //        Response.AddHeader("Content-Disposition", "attachment; filename=LaborTrackingList.csv");
    //        Response.End();
    //    }
    //    else
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage("No records found.");
    //    }
    //}
    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Labor Date", typeof(string));
        table.Columns.Add("Crew Member Name", typeof(string));
        table.Columns.Add("Customer Name", typeof(string));
        table.Columns.Add("Type", typeof(string));
        table.Columns.Add("Start Location", typeof(string));
        table.Columns.Add("End Location", typeof(string));
        table.Columns.Add("Start Time", typeof(string));
        table.Columns.Add("End Time", typeof(string));
        table.Columns.Add("Lunch Start Time", typeof(string));
        table.Columns.Add("Lunch End Time", typeof(string));
        table.Columns.Add("Lunch (Hrs)", typeof(string));
        table.Columns.Add("Total (Hr:Min)", typeof(string));
        table.Columns.Add("Customer Start Address", typeof(string));
        table.Columns.Add("Customer End Address", typeof(string));
        return table;
    }
    //protected void chkTotalhours_CheckedChanged(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkTotalhours.ID, chkTotalhours.GetType().Name, "Changed");
    //    if (chkTotalhours.Checked==true)
    //    {
    //        BindSubTotalHours();
    //        //chkEmployeeTotalHours.Checked = false;
    //    }
    //    else
    //    {
    //        chkTotalhours.Checked = false;
    //        BindLaborHourTracking();
    //    }
    //}


    // Employee Information

    //private void BindEmployee()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    string strQ = "select first_name+' '+last_name AS username,user_id from user_info WHERE is_active = 1 and IsTimeClock=1 order by username asc";
    //    List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
    //    lstEmployee.DataSource = mList;
    //    lstEmployee.DataTextField = "username";
    //    lstEmployee.DataValueField = "user_id";
    //    lstEmployee.DataBind();

    //}
    //private void BindEmployeeLaborHourTracking()
    //{
    //    try
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();

    //        string strCondition = string.Empty;

    //        strCondition = " where IsCrew=0 ";
    //        selectedValueEmp = "";

    //        foreach (ListItem item in lstEmployee.Items)
    //        {
    //            if (item.Selected)
    //            {
    //                selectedValueEmp += item.Value + ",";
    //            }
    //        }
    //        if (selectedValueEmp.Length > 0)
    //            strCondition += " AND UserID in (" + selectedValueEmp.TrimEnd(',') + " )";

    //        if (txtEmployeeStartDate.Text.Trim() != "" && txtEmployeeEndDate.Text.Trim() != "")
    //        {

    //            DateTime dt1 = Convert.ToDateTime(txtEmployeeStartDate.Text.Trim());
    //            DateTime dt2 = Convert.ToDateTime(txtEmployeeEndDate.Text.Trim());
    //            strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
    //        }





    //        string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress  from GPSTracking  " + strCondition + " order by labor_date desc ";
    //        IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);
    //        Session.Add("nEmpLoaborHour", csCommonUtility.LINQToDataTable(clist));

    //        if (Session["CPUFilter1Epm"] != null)
    //        {
    //            Hashtable htEmp = (Hashtable)Session["CPUFilter1Epm"];
    //            nPageNoEmp = Convert.ToInt32(htEmp["PageNoEmp"].ToString());
    //            GetEmployeeLaberTracking(nPageNoEmp);
    //        }
    //        else
    //        {
    //            GetEmployeeLaberTracking(0);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //protected void GetEmployeeLaberTracking(int nPageNoEmp)
    //{

    //    if (Session["nEmpLoaborHour"] != null)
    //    {
    //        DataTable dtLaborHour = (DataTable)Session["nEmpLoaborHour"];
    //        gdrEmployeeTimeTracker.DataSource = dtLaborHour;
    //        gdrEmployeeTimeTracker.PageIndex = nPageNoEmp;
    //        gdrEmployeeTimeTracker.AllowPaging = true;
    //        gdrEmployeeTimeTracker.PageSize = Convert.ToInt32(ddlItemPerPageEmployee.SelectedValue);
    //        gdrEmployeeTimeTracker.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress" };
    //        gdrEmployeeTimeTracker.DataBind();


    //        Hashtable htEmp = new Hashtable();
    //        htEmp.Add("ItemPerPageEmp", ddlItemPerPageEmployee.SelectedIndex);
    //        if (selectedValueEmp.Length>0)
    //            htEmp.Add("Employee", selectedValueEmp);
    //        else
    //            htEmp.Add("Employee", 0);
    //        htEmp.Add("PageNoEmp", nPageNoEmp);
    //        htEmp.Add("StartDateEmp", txtEmployeeStartDate.Text);
    //        htEmp.Add("EndDateEmp", txtEmployeeEndDate.Text);

    //        Session["CPUFilter1Epm"] = htEmp;
    //    }

    //    lblCurrentPageNoEmp.Text = Convert.ToString(nPageNoEmp + 1);
    //    if (nPageNoEmp == 0)
    //    {
    //        btnPreviousEmp.Enabled = false;
    //        btnPrevious0Emp.Enabled = false;
    //    }
    //    else
    //    {
    //        btnPreviousEmp.Enabled = true;
    //        btnPrevious0Emp.Enabled = true;
    //    }

    //    if (gdrEmployeeTimeTracker.PageCount == nPageNoEmp + 1)
    //    {
    //        btnNextEmp.Enabled = false;
    //        btnNext0Emp.Enabled = false;
    //    }
    //    else
    //    {
    //        btnNextEmp.Enabled = true;
    //        btnNext0Emp.Enabled = true;
    //    }
    //}



    //private void BindEmployeeSubTotalHours()
    //{
    //    try
    //    {
    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        string strCondition = string.Empty;
    //        strCondition = " where IsCrew=0 ";
    //        selectedValueEmp = "";

    //        foreach (ListItem item in lstEmployee.Items)
    //        {
    //            if (item.Selected)
    //            {
    //                selectedValueEmp += item.Value + ",";
    //            }
    //        }
    //        if (selectedValueEmp.Length > 0)
    //            strCondition += " AND UserID in (" + selectedValueEmp.TrimEnd(',') + " )";

    //        if (txtEmployeeStartDate.Text.Trim() != "" && txtEmployeeEndDate.Text.Trim() != "")
    //        {

    //            DateTime dt1 = Convert.ToDateTime(txtEmployeeStartDate.Text.Trim());
    //            DateTime dt2 = Convert.ToDateTime(txtEmployeeEndDate.Text.Trim());
    //            strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
    //        }
    //        string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                          " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName from GPSTracking  " + strCondition + " order by UserID,labor_date DESC ";
    //        IEnumerable<CrewTrack> clist2 = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);

    //        DataTable dt=csCommonUtility.LINQToDataTable(clist2);
    //        if(dt.Rows.Count>0)
    //        {
    //            gdrEmployeeTimeTracker.DataSource = dt;
    //            gdrEmployeeTimeTracker.PageIndex = 0;
    //            gdrEmployeeTimeTracker.AllowPaging = false;
    //            gdrEmployeeTimeTracker.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime" };
    //            gdrEmployeeTimeTracker.DataBind();
    //        }
    //        else
    //        {
    //            gdrEmployeeTimeTracker.DataSource = null;
    //            gdrEmployeeTimeTracker.PageIndex = 0;
    //            gdrEmployeeTimeTracker.AllowPaging = false;
    //            gdrEmployeeTimeTracker.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime" };
    //            gdrEmployeeTimeTracker.DataBind();
    //        }


    //    }
    //   catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    int currentIdEmp = 0;
    decimal subTotalEmp = 0;
    int subTotalRowIndexEmp = 0;
    string TotalME = "";
    int hourE = 0;
    //protected void gdrEmployeeTimeTracker_DataBound(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (chkEmployeeTotalHours.Checked)
    //        {
    //            for (int i = subTotalRowIndexEmp; i < gdrEmployeeTimeTracker.Rows.Count; i++)
    //            {

    //                DateTime StartTime = Convert.ToDateTime(gdrEmployeeTimeTracker.Rows[i].Cells[10].Text);
    //                string eTime = (gdrEmployeeTimeTracker.Rows[i].Cells[11].Text).ToString();
    //                if (eTime != null && eTime != "")
    //                {
    //                    DateTime EndTime = Convert.ToDateTime(gdrEmployeeTimeTracker.Rows[i].Cells[11].Text);

    //                    if (EndTime.Year != 2000)
    //                    {
    //                        StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                        StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                        EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                        EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                        TimeSpan span = EndTime.Subtract(StartTime);
    //                        decimal totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                        subTotalEmp += totalHours;
    //                    }
    //                }
    //            }

    //            if (subTotalEmp >= 60)
    //            {
    //                hourE = (int)subTotalEmp / 60;
    //                subTotalEmp = subTotalEmp % 60;
    //            }
    //            else
    //            {
    //                hourE = 0;
    //                subTotalEmp = subTotalEmp % 60;
    //            }
    //            if (subTotalEmp < 10)
    //                TotalME = hourE.ToString() + ":0" + subTotalEmp.ToString();
    //            else
    //                TotalME = hourE.ToString() + ":" + subTotalEmp.ToString();

    //            if (gdrEmployeeTimeTracker.Rows.Count > 0)
    //                this.AddTotalRowEmp("Total", TotalME);

    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //}
    //protected void gdrEmployeeTimeTracker_RowCreated(object sender, GridViewRowEventArgs e)
    //{
    //    try
    //    {
    //        if (chkEmployeeTotalHours.Checked == true)
    //        {

    //            subTotalEmp = 0;
    //            if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
    //            {
    //                if (e.Row.DataItem is DataRowView && (e.Row.DataItem as DataRowView).DataView.Table != null)
    //                {
    //                    DataTable dt = (e.Row.DataItem as DataRowView).DataView.ToTable(); //(e.Row.DataItem as DataRowView).DataView.Table;
    //                    int UserID = Convert.ToInt32(dt.Rows[e.Row.RowIndex]["UserID"]);

    //                    if (UserID != currentIdEmp)
    //                    {
    //                        if (e.Row.RowIndex > 0)
    //                        {
    //                            for (int i = subTotalRowIndexEmp; i < e.Row.RowIndex; i++)
    //                            {
    //                                DateTime StartTime = Convert.ToDateTime(gdrEmployeeTimeTracker.Rows[i].Cells[10].Text);
    //                                string eTime = (gdrEmployeeTimeTracker.Rows[i].Cells[11].Text).ToString();
    //                                if (eTime != null && eTime != "")
    //                                {
    //                                    DateTime EndTime = Convert.ToDateTime(gdrEmployeeTimeTracker.Rows[i].Cells[11].Text);
    //                                    if (EndTime.Year != 2000)
    //                                    {
    //                                        StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                                        StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                                        EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                                        EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                                        TimeSpan span = EndTime.Subtract(StartTime);
    //                                        decimal totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                                        subTotalEmp += totalHours;

    //                                    }
    //                                }

    //                            }
    //                            if (subTotalEmp >= 60)
    //                            {
    //                                hourE = (int)subTotalEmp / 60;
    //                                subTotalEmp = subTotalEmp % 60;
    //                            }
    //                            else
    //                            {
    //                                hourE = 0;
    //                                subTotalEmp = subTotalEmp % 60;
    //                            }
    //                            if (subTotalEmp < 10)
    //                                TotalME = hourE.ToString() + ":0" + Math.Round(subTotalEmp, 2).ToString();
    //                            else
    //                                TotalME = hourE.ToString() + ":" + Math.Round(subTotalEmp, 2).ToString();

    //                            if (gdrEmployeeTimeTracker.Rows.Count > 0)
    //                                this.AddTotalRowEmp("Total", TotalME);

    //                            subTotalRowIndexEmp = e.Row.RowIndex;
    //                        }
    //                        currentIdEmp = UserID;
    //                    }

    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    //private void AddTotalRowEmp(string labelText, string value)
    //{
    //    GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
    //    row.BackColor = ColorTranslator.FromHtml("#F9F9F9");

    //    row.Cells.AddRange(new TableCell[10] { new TableCell (), new TableCell (),new TableCell (),new TableCell (), new TableCell (),new TableCell (),new TableCell (),//Empty Cell
    //                                    new TableCell { Text = labelText, HorizontalAlign = HorizontalAlign.Right,CssClass="cellColor"},
    //                                    new TableCell { Text = value, HorizontalAlign = HorizontalAlign.Center,CssClass="cellColor" },new TableCell () });

    //    gdrEmployeeTimeTracker.Controls[0].Controls.Add(row);
    //}
    //protected void gdrEmployeeTimeTracker_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        try
    //        {
    //            DataClassesDataContext _db = new DataClassesDataContext();

    //            int nGPSId = Convert.ToInt32(gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[0].ToString());
    //            string SectionName = gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[1].ToString();
    //            string EndPlace = gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[2].ToString();
    //            int nUserId = Convert.ToInt32(gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[3].ToString());
    //            string StartPlace = gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[4].ToString();
    //            DateTime StartTime = Convert.ToDateTime(gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[5]);
    //            DateTime EndTime = Convert.ToDateTime(gdrEmployeeTimeTracker.DataKeys[e.Row.RowIndex].Values[6]);

    //            Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");
    //            Label lblLunch = (Label)e.Row.FindControl("lblLunch");
    //            Label lblSection = (Label)e.Row.FindControl("lblSection");
    //            if (SectionName != "Select")
    //                lblSection.Text = SectionName;
    //            lblLunch.Text = ".50";

    //            if (Convert.ToDateTime(EndTime).Year != 2000)
    //            {

    //                StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                TimeSpan span = EndTime.Subtract(StartTime);
    //                int totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                if (span.Days > 0)
    //                    lblTotalHours.Text = span.Days + ":" + span.Hours + ":" + span.Minutes;
    //                else
    //                    lblTotalHours.Text = span.ToString(@"hh\:mm");//span.Hours + ":" + span.Minutes;


    //            }
    //            else
    //            {
    //                e.Row.Cells[5].Text = "";
    //                lblTotalHours.Text = "";
    //            }

    //            if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
    //            {
    //                e.Row.Cells[2].Text = StartPlace.Remove(StartPlace.Length - 5, 5);
    //            }
    //            else
    //            {
    //                if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
    //                {
    //                    e.Row.Cells[2].Text = "";
    //                }
    //                else
    //                {
    //                    e.Row.Cells[2].Text = StartPlace;
    //                }

    //            }

    //            if (EndPlace == "0")
    //            {
    //                e.Row.Cells[3].Text = "";
    //            }
    //            else
    //            {
    //                if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
    //                {
    //                    e.Row.Cells[3].Text = EndPlace.Remove(EndPlace.Length - 5, 5);
    //                }
    //                else
    //                {
    //                    e.Row.Cells[3].Text = EndPlace;
    //                }
    //            }
    //            user_info objUser = new user_info();
    //            objUser = _db.user_infos.SingleOrDefault(c => c.user_id == nUserId && c.IsTimeClock==true);
    //            if (objUser != null)
    //                e.Row.Cells[1].Text = objUser.first_name+" "+objUser.last_name;
    //            else
    //                e.Row.Cells[1].Text = "";
    //            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
    //            imgDelete.OnClientClick = "return confirm('This will permanently delete this time entry.');";
    //            imgDelete.CommandArgument = nGPSId.ToString();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }
    //}

    //protected void chkEmployeeTotalHours_CheckedChanged(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkEmployeeTotalHours.ID, chkEmployeeTotalHours.GetType().Name, "Changed");
    //    if (chkEmployeeTotalHours.Checked == true)
    //    {
    //        chkTotalhours.Checked = false;
    //        BindEmployeeSubTotalHours();
    //    }
    //    else
    //    {
    //        chkEmployeeTotalHours.Checked = false;
    //        BindEmployeeLaborHourTracking();
    //    }
    //}
    //protected void ddEmployee_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //  //  KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddEmployee.ID, ddEmployee.GetType().Name, "Changed");
    //    // chkEmployeeTotalHours.Checked = false;
    //    lblResultEmp.Text = "";
    //    lblResult.Text = "";

    //    if (chkEmployeeTotalHours.Checked == true)
    //    {

    //        chkTotalhours.Checked = false;
    //        BindEmployeeSubTotalHours();
    //    }
    //    else
    //    {
    //        chkEmployeeTotalHours.Checked = false;
    //        BindEmployeeLaborHourTracking();
    //    }

    //}
    //protected void lnkViewAllEmp_Click(object sender, EventArgs e)
    //{
    //    lblResult.Text = "";
    //   // ddEmployee.SelectedIndex = -1;
    //    txtEmployeeStartDate.Text = "";
    //    lblResultEmp.Text = "";
    //    txtEmployeeEndDate.Text = "";
    //    chkEmployeeTotalHours.Checked = false;

    //    if (Session["CPUFilter1Emp"] != null)
    //    {
    //        Session.Remove("CPUFilter1Emp");

    //    }
    //    Session.Remove("Installer");
    //    Session.Remove("Employee");
    //    selectedvalue = "";
    //    selectedValueEmp = "";
    //    BindCrew();
    //    BindEmployee();
    //    ddlItemPerPageEmployee.SelectedIndex = 0;
    //    BindEmployeeLaborHourTracking();

    //}

    //protected void gdrEmployeeTimeTracker_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, gdrEmployeeTimeTracker.ID, gdrEmployeeTimeTracker.GetType().Name, "Changed");
    //    GetEmployeeLaberTracking(e.NewPageIndex);
    //}

    //protected void btnNextEmp_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNextEmp.ID, btnNextEmp.GetType().Name, "Click");
    //    int nCurrentPage = 0;
    //    nCurrentPage = Convert.ToInt32(lblCurrentPageNoEmp.Text);
    //    GetEmployeeLaberTracking(nCurrentPage);
    //}
    //protected void btnPreviousEmp_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPreviousEmp.ID, btnPreviousEmp.GetType().Name, "Click");
    //    int nCurrentPage = 0;
    //    nCurrentPage = Convert.ToInt32(lblCurrentPageNoEmp.Text);
    //    GetEmployeeLaberTracking(nCurrentPage - 2);
    //}
    //protected void ddlItemPerPageEmployee_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPageEmployee.ID, ddlItemPerPageEmployee.GetType().Name, "Changed");
    //    chkEmployeeTotalHours.Checked = false;
    //    GetEmployeeLaberTracking(0);
    //}

    //protected void btnViewEmp_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewEmp.ID, btnViewEmp.GetType().Name, "Click");
    //    string strRequired = string.Empty;
    //    lblResult.Text = "";
    //    lblResultEmp.Text = "";
    //    try
    //    {
    //        Convert.ToDateTime(txtEmployeeStartDate.Text.Trim());
    //    }
    //    catch
    //    {
    //        strRequired += "Start Date is required.<br/>";

    //    }

    //    try
    //    {
    //        Convert.ToDateTime(txtEmployeeEndDate.Text.Trim());
    //        if (Convert.ToDateTime(txtEmployeeStartDate.Text.Trim()) >= Convert.ToDateTime(txtEmployeeEndDate.Text.Trim()))
    //        {
    //            strRequired += "End Date must be greater than Start Date.<br/>";

    //        }
    //    }
    //    catch
    //    {
    //        strRequired += "End Date is required.<br/>";

    //    }
    //    if (strRequired.Length > 0)
    //    {
    //        lblResultEmp.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
    //        return;
    //    }

    //    if (chkEmployeeTotalHours.Checked)
    //    {
    //        BindEmployeeSubTotalHours();
    //    }
    //    else
    //    {
    //        BindEmployeeLaborHourTracking();
    //    }

    //}

    protected void btnAddNewEmp_Click(object sender, EventArgs e)
    {
        Response.Redirect("laborhourdetails.aspx?isCrew=0");
    }

    //protected void btnExpCustListEmp_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ImabtnEmployee.ID, ImabtnEmployee.GetType().Name, "Click");
    //    DataTable tmpTable = LoadTmpDataTableEmp();
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    string strRequired = string.Empty;
    //    lblResult.Text = "";
    //    lblResultEmp.Text = "";
    //    string strCondition = string.Empty;
    //    strCondition = " where IsCrew=0 ";
    //    selectedValueEmp = "";

    //    foreach (ListItem item in lstEmployee.Items)
    //    {
    //        if (item.Selected)
    //        {
    //            selectedValueEmp += item.Value + ",";
    //        }
    //    }
    //    if (selectedValueEmp.Length > 0)
    //        strCondition += " AND UserID in (" + selectedValueEmp.TrimEnd(',') + " )";


    //    if (txtEmployeeStartDate.Text.Trim() != "" && txtEmployeeEndDate.Text.Trim() != "")
    //    {

    //        try
    //        {
    //            Convert.ToDateTime(txtEmployeeStartDate.Text.Trim());
    //        }
    //        catch
    //        {
    //            strRequired += "Start Date is required.<br/>";

    //        }

    //        try
    //        {
    //            Convert.ToDateTime(txtEmployeeEndDate.Text.Trim());
    //            if (Convert.ToDateTime(txtEmployeeStartDate.Text.Trim()) >= Convert.ToDateTime(txtEmployeeEndDate.Text.Trim()))
    //            {
    //                strRequired += "End Date must be greater than Start Date.<br/>";

    //            }
    //        }
    //        catch
    //        {
    //            strRequired += "End Date is required.<br/>";

    //        }
    //        if (strRequired.Length > 0)
    //        {
    //            lblResultEmp.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
    //            return;
    //        }
    //        DateTime dt1 = Convert.ToDateTime(txtEmployeeStartDate.Text.Trim());
    //        DateTime dt2 = Convert.ToDateTime(txtEmployeeEndDate.Text.Trim());
    //        strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";

    //    }
    //string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[CreatedDate],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                 " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date] from GPSTracking  " + strCondition + " order by labor_date desc ";
    //    DataTable dtReport = csCommonUtility.GetDataTable(strQ);
    //    if (dtReport.Rows.Count > 0)
    //    {
    //        foreach (DataRow dr in dtReport.Rows)
    //        {
    //            string StartPlace = "";
    //            string EndPlace = "";
    //            DataRow drNew = tmpTable.NewRow();
    //            StartPlace = dr["StartPlace"].ToString();
    //            EndPlace = dr["EndPlace"].ToString();
    //            user_info objUser = new user_info();
    //            objUser = _db.user_infos.SingleOrDefault(c => c.user_id == Convert.ToInt32(dr["UserID"]) && c.IsTimeClock==true);
    //            drNew["Labor Date"] = Convert.ToDateTime(dr["labor_date"]).ToString("MM-dd-yyyy");
    //            if (objUser != null)
    //                drNew["Employee Name"] = objUser.first_name + " " + objUser.last_name;
    //            drNew["Customer Name"] = dr["CustomerName"].ToString();
    //            if (dr["SectionName"].ToString() != "Select")
    //                drNew["Type"] = dr["SectionName"].ToString();
    //            if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
    //            {
    //                drNew["Start Location"] = StartPlace.Remove(StartPlace.Length - 5, 5);
    //            }
    //            else
    //            {
    //                if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
    //                {
    //                    drNew["Start Location"] = "";
    //                }
    //                else
    //                {
    //                    drNew["Start Location"] = StartPlace;
    //                }
    //            }
    //            if (EndPlace == "0")
    //            {
    //                drNew["End Location"] = "";
    //            }
    //            else
    //            {
    //                if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
    //                {
    //                    drNew["End Location"] = EndPlace.Remove(EndPlace.Length - 5, 5);
    //                }
    //                else
    //                {
    //                    drNew["End Location"] = EndPlace;
    //                }
    //            }

    //            drNew["Start Time"] = Convert.ToDateTime(dr["StartTime"]).ToShortTimeString();
    //            if (Convert.ToDateTime(dr["EndTime"]).Year != 2000)
    //                drNew["End Time"] = Convert.ToDateTime(dr["EndTime"]).ToShortTimeString();

    //            drNew["Lunch Start Time"] = "12 PM";
    //            drNew["Lunch End Time"] = "12:30 PM";

    //            drNew["Lunch End Time"] = "12:30 PM";
    //            drNew["Lunch (Hrs)"] = ".50";

    //            if (Convert.ToDateTime(dr["EndTime"]).Year != 2000)
    //            {
    //                DateTime StartTime = Convert.ToDateTime(dr["StartTime"].ToString());
    //                DateTime EndTime = Convert.ToDateTime(dr["EndTime"].ToString());

    //                StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
    //                StartTime = StartTime.AddSeconds(-StartTime.Second);
    //                EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
    //                EndTime = EndTime.AddSeconds(-EndTime.Second);
    //                TimeSpan span = EndTime.Subtract(StartTime);

    //                int totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
    //                if (span.Days > 0)
    //                    drNew["Total (Hr:Min)"] = span.Days + ":" + span.Hours + ":" + span.Minutes;
    //                else
    //                    drNew["Total (Hr:Min)"] = span.ToString(@"hh\:mm");

    //             }
    //            else
    //            {

    //                drNew["Total (Hr:Min)"] = "";
    //            }


    //            tmpTable.Rows.Add(drNew);
    //        }

    //        Response.Clear();
    //        Response.ClearHeaders();

    //        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
    //        {
    //            writer.WriteAll(tmpTable, true);
    //        }
    //        Response.ContentType = "application/vnd.ms-excel";
    //        Response.AddHeader("Content-Disposition", "attachment; filename=LaborTrackingList.csv");
    //        Response.End();
    //    }
    //    else
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage("No records found.");
    //    }
    //}
    private DataTable LoadTmpDataTableEmp()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Labor Date", typeof(string));
        table.Columns.Add("Employee Name", typeof(string));
        table.Columns.Add("Customer Name", typeof(string));
        table.Columns.Add("Type", typeof(string));
        table.Columns.Add("Start Location", typeof(string));
        table.Columns.Add("End Location", typeof(string));
        table.Columns.Add("Start Time", typeof(string));
        table.Columns.Add("End Time", typeof(string));
        table.Columns.Add("Lunch Start Time", typeof(string));
        table.Columns.Add("Lunch End Time", typeof(string));
        table.Columns.Add("Lunch (Hrs)", typeof(string));
        table.Columns.Add("Total (Hr:Min)", typeof(string));


        return table;
    }

    protected void DeleteFileEmp(object sender, EventArgs e)
    {
        try
        {
            userinfo obj = (userinfo)Session["oUser"];
            ImageButton imgDelete = (ImageButton)sender;
            int nGPSId = Convert.ToInt32(imgDelete.CommandArgument);
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";
            // lblResultEmp.Text = "";
            strQ = "insert into DeleteGPSTracking " +
                          " SELECT [GPSTrackID] ,[StartPlace] ,[StartLatitude],[StartLogitude] ,[EndLatitude] ,[EndLogitude],[MakeStopPlace], " +
                          " [EndPlace],[Distance],[Time],[CreatedDate] ,[UserID] ,[CustomerName] ,[section_id],[SectionName] ,[StartTime], " +
                          " [EndTime],[customer_id],[Estimate_id],[labor_date],[deviceName],[IsCrew], " + "'" + obj.username + "'" + ",getdate() " +
                          " FROM GPSTracking " +
                          " WHERE GPSTrackID ='" + nGPSId + "'";
            _db.ExecuteCommand(strQ, string.Empty);

            strQ = "delete from GPSTracking where GPSTrackID=" + nGPSId;
            _db.ExecuteCommand(strQ, string.Empty);


            var nList = (from dgps in _db.GPSTrackingDetails where dgps.GPSTrackID == nGPSId select dgps).ToList();

            DeleteGPSTrackingDetail objDGPS = null;

            foreach (var li in nList)
            {
                objDGPS = new DeleteGPSTrackingDetail();
                objDGPS.ID = li.ID;
                objDGPS.GPSTrackID = li.GPSTrackID;
                objDGPS.Latitude = li.Latitude;
                objDGPS.Longitude = li.Longitude;
                objDGPS.InputType = li.InputType;
                objDGPS.CreateDate = li.CreateDate;

                _db.DeleteGPSTrackingDetails.InsertOnSubmit(objDGPS);
                _db.SubmitChanges();
            }

            strQ = "delete from GPSTrackingDetails where GPSTrackID=" + nGPSId;
            _db.ExecuteCommand(strQ, string.Empty);
            //lblResultEmp.Text = csCommonUtility.GetSystemMessage("Labor tracking has been deleted successfully.");

            //BindEmployeeLaborHourTracking();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //arefin 09-14-2019 -- Formate StartCustomerAddress (Customer Start Address) & EndCustomerAddress (Customer End Address)
    //protected void btnUpdateData_Click(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateData.ID, btnUpdateData.GetType().Name, "Click");
    //    try
    //    {
    //        UpdateData();
    //        //BindLaborHourTracking();
    //        lblResult.Text = csCommonUtility.GetSystemMessage("Data Updated Successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
    //    }
    //}

    //arefin 09-14-2019 -- Formate StartCustomerAddress (Customer Start Address) & EndCustomerAddress (Customer End Address)
    //private void UpdateData()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
    //                  " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress from GPSTracking  WHERE     (CrewLastLangitude IS NOT NULL) ";
    //    IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);

    //    foreach (var c in clist)
    //    {
    //        GPSTracking objGT = _db.GPSTrackings.Where(g => g.GPSTrackID == c.GPSTrackID).FirstOrDefault();

    //        objGT.StartCustomerAddress = c.StartCustomerAddress.Replace("Customer Name: ", "").Replace("Address: ", Environment.NewLine).Replace(",", ", ").Replace("AZ, ", "AZ ");
    //        objGT.EndCustomerAddress = c.EndCustomerAddress.Replace("Customer Name: ", "").Replace("Address: ", Environment.NewLine).Replace(",", ", ").Replace("AZ, ", "AZ ");
    //        _db.SubmitChanges();

    //    }
    //}

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindLaborHourTracking();
    }

    //protected void btnEmployeeSearch_Click(object sender, EventArgs e)
    //{
    //    BindEmployeeLaborHourTracking();
    //}
    //protected void btnGMap_Click(object sender, EventArgs e)
    //{

    //    string strCondition = string.Empty;
    //    strCondition = " where gps.IsCrew=1 ";
    //    selectedvalue = "";

    //    foreach (ListItem item in lstCrew.Items)
    //    {
    //        if (item.Selected)
    //        {
    //            selectedvalue += item.Value + ",";
    //        }
    //    }
    //    if (selectedvalue.Length > 0)
    //        strCondition += " AND gps.UserID in (" + selectedvalue.TrimEnd(',') + " )";

    //    if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
    //    {

    //        DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
    //        DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
    //        strCondition += " AND gps.labor_date >= '" + dt1 + "' and gps.labor_date <'" + dt2.AddDays(1) + "'";
    //    }
    //    Session.Add("gCusCrewTrackerData", strCondition);

    //    Response.Redirect("gCusCrewTrackermap.aspx");

    //}
}