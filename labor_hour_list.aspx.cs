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


using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;


public partial class labor_hour_list : System.Web.UI.Page
{
    public DataTable dtSection;
    int nPageNo = 0;
    int nPageNoEmp = 0;
    string selectedvalue = "";
    string selectedCustomerJobvalue = "";
    string selectedValueEmp = "";
    int View = 0;
    int Activecrew = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string divisionName = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            if (Page.User.IsInRole("t04") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }


            BindDivision();

            if (divisionName != "" && divisionName.Contains(","))
            {
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.Enabled = false;
            }

            BindCrew();
          
            BindCustomer();


            if (Session["CPUFilter"] != null)
            {
                Hashtable ht = (Hashtable)Session["CPUFilter"];
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
                txtStartDate.Text = ht["StartDate"].ToString();
                txtEndDate.Text = ht["EndDate"].ToString();


            }

           
            BindLaborHourTracking();

            //txtStartDate.Text = Convert.ToDateTime("04/01/2022").ToString("dd/MM/yy");
            //txtEndDate.Text = Convert.ToDateTime("05/29/2022").ToString("dd/MM/yy");


            csCommonUtility.SetPagePermission(this.Page, new string[] {  "btnExpCustList", "btnGMap1" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdLaberTrack_imgDelete" });
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
            ddlDivision.Items.Insert(0, "All");
            ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        

    }

    private void BindLaborHourTracking()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = string.Empty;
            //strCondition = " where IsCrew=1 ";
            selectedvalue = "";
            selectedCustomerJobvalue = "";

            foreach (ListItem item in lstCrew.Items)
            {
                if (item.Selected)
                {
                    selectedvalue += item.Value + ",";
                }
            }

            foreach (ListItem item1 in lstJobName.Items)
            {
                if (item1.Selected)
                {
                    selectedCustomerJobvalue += "'" + item1.Value + "',";
                }
            }

            if(radEmployeeType.SelectedValue!="3")
            {
                strCondition = " where IsCrew=" + radEmployeeType.SelectedValue;
            }

            if (selectedvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";
                else
                    strCondition= " where UserID in (" + selectedvalue.TrimEnd(',') + " )";
            }
                


            if (selectedCustomerJobvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
                else
                    strCondition = " where gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
            }
                

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                if (strCondition.Length > 0)
                {
                    strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                else
                {
                    strCondition= " where labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                    
               
            }

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 2)
                    strCondition += " AND gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%'";
                else
                    strCondition = " WHERE gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%'";
            }


            //imran ch
            string strQ= " SELECT GPSTrackID, gps.client_id, StartPlace, MakeStopPlace, gps.division_name, EndPlace, Distance, Time, UserID, IsCrew," +                      
                     // " c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                     " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                      " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " user_info.first_name + ' ' + user_info.last_name as full_name, section_id, SectionName, StartTime, EndTime," +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress," +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  user_info ON gps.UserID = user_info.user_id " +
                       " left join customers as c on c.customer_id = gps.customer_id" +
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id " +
                       " " + strCondition + 
                       " UNION " +
                       " SELECT   GPSTrackID, gps.client_id, StartPlace, MakeStopPlace, gps.division_name, EndPlace, Distance, Time, UserID, IsCrew," +
                       //" c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                       " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                      " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " Crew_Details.full_name AS full_name, section_id, SectionName, StartTime, EndTime, " +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress, " +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  Crew_Details ON gps.UserID = Crew_Details.MaxCrewId " +
                       " left join customers as c on c.customer_id = gps.customer_id"+
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id "+
                       " " + strCondition + " order by labor_date desc ";

            DataTable clist = csCommonUtility.GetDataTable(strQ);

            DateTime EndTimedt = new DateTime(2000, 01, 01);
           
            


            Session.Add("nLoaborHour", clist);

            // var nList = clist;
            var nCount = from myRow in clist.AsEnumerable()
                         where myRow.Field<DateTime>("EndTime") == EndTimedt
                         select myRow;

            Activecrew = nCount.Count();


            if (Session["CPUFilter"] != null)
            {
                Hashtable ht = (Hashtable)Session["CPUFilter"];
                nPageNo = Convert.ToInt32(ht["PageNo"].ToString());
                GetLaberTracking(nPageNo);
            }
            else
            {
                GetLaberTracking(0);
            }

           
            lblActiveCrew.Text = Activecrew.ToString();

            string strCondition2 = string.Empty;
            if (selectedvalue.Length > 0)
                strCondition2 += " AND gps.UserID in (" + selectedvalue.TrimEnd(',') + " )";

            if (selectedCustomerJobvalue.Length > 0)
                strCondition2 += " AND gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition2 += " AND gps.labor_date >= '" + dt1 + "' and gps.labor_date <'" + dt2.AddDays(1) + "'";
            }

            if (ddlDivision.SelectedItem.Text != "All")
            {
                strCondition2 += " AND gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%'";
            }

            String aSql = "SELECT  distinct  ce.customer_estimate_id "+
                            "FROM GPSTracking AS gps inner JOIN "+
                            " customer_estimate AS ce ON gps.Estimate_id = ce.estimate_id  and gps.customer_id = ce.customer_id "+
                            " WHERE (ce.status_id = 3) AND(ce.IsEstimateActive = 1)  " + strCondition2;

            DataTable dts = csCommonUtility.GetDataTable(aSql);
            View = dts.Rows.Count;
            lblView.Text = View.ToString();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void BindCrew()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
          
            string strQ = " SELECT first_name +' ' + last_name AS full_name, MaxCrewId AS UserId " +
                          " FROM Crew_Details " +
                          " WHERE is_active =1 " +
                          " UNION " +
                          " SELECT first_name + ' ' + last_name+' (Emp)' AS full_name, user_id AS UserId " +
                          " FROM user_info " +
                          " WHERE is_active =1 and IsTimeClock=1 ORDER BY full_name ";

            DataTable dt = csCommonUtility.GetDataTable(strQ);
            lstCrew.DataSource = dt;
            lstCrew.DataTextField = "full_name";
            lstCrew.DataValueField = "UserId";
            lstCrew.DataBind();
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void BindCustomer()
    {
        try
        {
            string strQ = " select customer_estimate_id,c.customer_id,ce.customer_estimate_id,c.last_name1, " +
                          //"   c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as customername "+
                          " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                          " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as customername " +
                          " from customers as c " +
                          " inner join customer_estimate as ce on c.customer_id=ce.customer_id " +
                          " where ce.status_id = 3  and ce.IsEstimateActive=1 " +
                          " order by customername asc ";
           

            DataTable dt = csCommonUtility.GetDataTable(strQ);
            lstJobName.DataSource = dt;
            lstJobName.DataTextField = "customername";
            lstJobName.DataValueField = "customer_estimate_id";
            lstJobName.DataBind();
           
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void GetLaberTracking(int nPageNo)
    {
        try
        {
            if (Session["nLoaborHour"] != null)
            {
                DataTable dtLaborHour = (DataTable)Session["nLoaborHour"];
                grdLaberTrack.DataSource = dtLaborHour;
                grdLaberTrack.PageIndex = nPageNo;
                grdLaberTrack.AllowPaging = true;
                grdLaberTrack.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress", "labor_date", "WorkingDayName", "CustomerName", "client_id" };
                grdLaberTrack.DataBind();


                Hashtable ht = new Hashtable();
                ht.Add("ItemPerPage", ddlItemPerPage.SelectedIndex);
                if (selectedvalue.Length > 0)
                    ht.Add("Installer", selectedvalue);
                else
                    ht.Add("Installer", 0);
                ht.Add("PageNo", nPageNo);
                ht.Add("StartDate", txtStartDate.Text);
                ht.Add("EndDate", txtEndDate.Text);

                Session["CPUFilter"] = ht;
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

        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

        
    }

    protected void grdLaberTrack_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                int nGPSId = Convert.ToInt32(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[0].ToString());
                string SectionName = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[1].ToString();
                string EndPlace = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[2].ToString();
                int nUserId = Convert.ToInt32(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string StartPlace = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[4].ToString();
                DateTime StartTime = Convert.ToDateTime(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[5]);
                DateTime EndTime = Convert.ToDateTime(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[6]);
                string customerStartPlace = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[7].ToString();
                string customerEndPlace = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[8].ToString();
                DateTime LaborDate = Convert.ToDateTime(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[9].ToString());
                string dayName = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[10].ToString();
                string customername = grdLaberTrack.DataKeys[e.Row.RowIndex].Values[11].ToString();

                Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");
                Label lblLunch = (Label)e.Row.FindControl("lblLunch");
                Label lblSection = (Label)e.Row.FindControl("lblSection");
                Label lblOTHours = (Label)e.Row.FindControl("lblOTHours");
                HyperLink InkLaborDate = (HyperLink)e.Row.FindControl("InkLaborDate");
                Label lblLaborDayName = (Label)e.Row.FindControl("lblLaborDayName");
                Label lblRegular = (Label)e.Row.FindControl("lblRegular");
                Label lblCustomerName = (Label)e.Row.FindControl("lblCustomerName");

                lblLaborDayName.Text= dayName;
                InkLaborDate.Text = LaborDate.ToString("MM/dd/yyyy");
                InkLaborDate.NavigateUrl = "laborhourdetails.aspx?gpid=" + nGPSId;

                


                if (SectionName != "Select")
                    lblSection.Text = SectionName;
                lblLunch.Text =".50";

                double OtH = 0;
                double TotalRegularHour = 0;
                if (Convert.ToDateTime(EndTime).Year != 2000)
                {
                    StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
                    StartTime = StartTime.AddSeconds(-StartTime.Second);
                    EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
                    EndTime = EndTime.AddSeconds(-EndTime.Second);
                    TimeSpan span = EndTime.Subtract(StartTime);
                    if (span.Days > 0)
                    {
                        lblTotalHours.Text = span.Days + ":" + span.Hours + ":" + span.Minutes;
                    }
                    else
                    {
                        lblTotalHours.Text = span.Hours.ToString().PadLeft(2, '0') + ":" + span.Minutes.ToString().PadLeft(2, '0'); ///span.ToString(@"hh\:mm");//span.Hours + ":" + span.Minutes;
                    }
                    if (dayName.ToLower() == "saturday" || dayName.ToLower() == "sunday")
                    {
                        OtH = span.TotalHours;
                        int nHour = 0;
                        double nMin = 0;
                        nHour = (int)(OtH * 60) / 60;
                        nMin = (OtH * 60) % 60;
                        lblOTHours.Text = nHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nMin, 6).ToString().PadLeft(2, '0');
                        lblRegular.Text = "00:00";
                    }
                    else
                    {
                        if (span.TotalHours > 8)
                        {
                            OtH = span.TotalHours - 8;//8*60
                            TotalRegularHour = span.TotalHours - OtH;
                            int nHour = 0;
                            double nMin = 0;
                            nHour = (int)(OtH * 60) / 60;
                            nMin = (OtH * 60) % 60;
                            lblOTHours.Text = nHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nMin, 6).ToString().PadLeft(2, '0');

                            //Regular Hour
                            nHour = 0;
                            nMin = 0;
                            nHour = (int)(TotalRegularHour * 60) / 60;
                            nMin = (TotalRegularHour * 60) % 60;
                            lblRegular.Text = nHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nMin, 6).ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            lblOTHours.Text = "00:00";
                            lblRegular.Text = span.Hours.ToString().PadLeft(2, '0') + ":" + span.Minutes.ToString().PadLeft(2, '0');
                        }
                        if (span.TotalHours >= 12)
                        {
                            e.Row.ForeColor = System.Drawing.Color.Red;
                            InkLaborDate.ForeColor = System.Drawing.Color.Red;
                        }
                    }


                }
                else
                {
                    e.Row.Cells[8].Text = "";  //imran
                    lblTotalHours.Text = "";
                    lblOTHours.Text = "";
                    lblRegular.Text = "";
                }
                if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
                {
                    e.Row.Cells[5].Text = StartPlace.Remove(StartPlace.Length - 5, 5);  //imran
                }
                else
                {
                    if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
                    {
                        e.Row.Cells[5].Text = "";  //imran
                    }
                    else
                    {
                        e.Row.Cells[5].Text = StartPlace;  //imran

                    }

                }
                if (EndPlace == "0")
                {
                    e.Row.Cells[6].Text = "";  //imran
                }
                else
                {
                    if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
                    {
                        e.Row.Cells[7].Text = EndPlace.Remove(EndPlace.Length - 5, 5);  //imran

                    }
                    else
                    {
                        e.Row.Cells[6].Text = EndPlace;  //imran

                    }
                }
                ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
                imgDelete.OnClientClick = "return confirm('This will permanently delete this time entry.');";
                imgDelete.CommandArgument = nGPSId.ToString();


            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }
        }
    }

    protected void DeleteFile(object sender, EventArgs e)
    {
        try
        {
            userinfo obj = (userinfo)Session["oUser"];
            ImageButton imgDelete = (ImageButton)sender;
            int nGPSId = Convert.ToInt32(imgDelete.CommandArgument);
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";
         

            strQ = "insert into DeleteGPSTracking " +
                          " SELECT [GPSTrackID] ,[StartPlace] ,[StartLatitude],[StartLogitude] ,[EndLatitude] ,[EndLogitude],[MakeStopPlace], " +
                          " [EndPlace],[Distance],[Time],[CreatedDate] ,[UserID] ,[CustomerName] ,[section_id],[SectionName] ,[StartTime], " +
                          " [EndTime],[customer_id],[Estimate_id],[labor_date],[deviceName],[IsCrew]," + "'" + obj.username + "'" + ",getdate() " +
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
            lblResult.Text = csCommonUtility.GetSystemMessage("Labor tracking has been deleted successfully.");
            BindLaborHourTracking();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click");
        string strRequired = string.Empty;
        lblResult.Text = "";
        
        try
        {
            Convert.ToDateTime(txtStartDate.Text.Trim());
        }
        catch
        {
            strRequired += "Start Date is required.<br/>";

        }

        try
        {
            Convert.ToDateTime(txtEndDate.Text.Trim());
            
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
        if (chkTotalhours.Checked)
        {
            BindSubTotalHours();
        }
        else
        {
            BindLaborHourTracking();
        }

    }

    private void BindSubTotalHours()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = string.Empty;
           // strCondition = " where IsCrew=1 ";
            selectedvalue = "";

            foreach (ListItem item in lstCrew.Items)
            {
                if (item.Selected)
                {
                    selectedvalue += item.Value + ",";
                }
            }

            foreach (ListItem item1 in lstJobName.Items)
            {
                if (item1.Selected)
                {
                    selectedCustomerJobvalue += "'" + item1.Value + "',";
                }
            }
            if (radEmployeeType.SelectedValue != "3")
            {
                strCondition = " where IsCrew=" + radEmployeeType.SelectedValue;
            }
            if (selectedvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";
                else
                    strCondition = " where UserID in (" + selectedvalue.TrimEnd(',') + " )";
            }



            if (selectedCustomerJobvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
                else
                    strCondition = " where gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
            }


            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                if (strCondition.Length > 0)
                {
                    strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                else
                {
                    strCondition = " where labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }


            }

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 2)
                    strCondition += " AND gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%' ";
                else
                    strCondition = " WHERE  gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%' ";
            }

            string strQ = " SELECT  GPSTrackID, gps.division_name, StartPlace, MakeStopPlace, EndPlace, Distance, Time, UserID, IsCrew," +
                      //" c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                      " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                     " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " user_info.first_name + ' ' + user_info.last_name as full_name, section_id, SectionName, StartTime, EndTime," +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress," +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  user_info ON gps.UserID = user_info.user_id " +
                       " left join customers as c on c.customer_id = gps.customer_id" +
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id " +
                       " " + strCondition +
                       " UNION " +
                       " SELECT   GPSTrackID, StartPlace, gps.division_name, MakeStopPlace, EndPlace, Distance, Time, UserID, IsCrew," +
                       //" c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                       " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                     " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " Crew_Details.full_name AS full_name, section_id, SectionName, StartTime, EndTime, " +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress, " +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  Crew_Details ON gps.UserID = Crew_Details.MaxCrewId " +
                       " left join customers as c on c.customer_id = gps.customer_id" +
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id " +
                       " " + strCondition + " order by UserID,labor_date desc ";
            

            DataTable clist = csCommonUtility.GetDataTable(strQ);



            //DataTable dataTable = csCommonUtility.LINQToDataTable(clist);

            if (clist.Rows.Count > 0)
            {
                grdLaberTrack.DataSource = clist;
                grdLaberTrack.PageIndex = 0;
                grdLaberTrack.AllowPaging = false;
                grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress", "labor_date", "WorkingDayName", "CustomerName" };
                grdLaberTrack.DataBind();
            }
            else
            {
                grdLaberTrack.DataSource = null;
                grdLaberTrack.PageIndex = 0;
                grdLaberTrack.AllowPaging = false;
                grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "StartCustomerAddress", "EndCustomerAddress", "labor_date", "WorkingDayName", "CustomerName" };
                grdLaberTrack.DataBind();
            }


        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdLaberTrack_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (chkTotalhours.Checked)
            {
                for (int i = subTotalRowIndex; i < grdLaberTrack.Rows.Count; i++)
                {

                    DateTime StartTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[17].Text); //imran
                    string eTime = (grdLaberTrack.Rows[i].Cells[18].Text).ToString();  //imran


                    if (eTime != null && eTime != "")
                    {
                        DateTime EndTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[18].Text);   //imran
                        if (EndTime.Year != 2000)
                        {
                            string dayName = grdLaberTrack.Rows[i].Cells[19].Text;   //imran
                            StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond); 
                            StartTime = StartTime.AddSeconds(-StartTime.Second);
                            EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
                            EndTime = EndTime.AddSeconds(-EndTime.Second);
                            TimeSpan span = EndTime.Subtract(StartTime);
                            decimal totalMinutes = 0;
                            double totalHours = 0;
                            double totalRegularHours = 0;

                            totalMinutes = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                            subTotal += totalMinutes;
                            if (dayName.ToLower() == "saturday" || dayName.ToLower() == "sunday")
                            {
                                 totalHours = span.TotalHours;
                                  nCRewOtH += totalHours;
                            }
                            else
                            {
                               
                                if (span.TotalHours > 8)
                                {
                                    totalHours = span.TotalHours-8;
                                    totalRegularHours = span.TotalHours - totalHours;
                                    nRegularTotalMinute += (totalRegularHours * 60);
                                    nCRewOtH += totalHours;
                                   
                                }
                                else
                                {
                                    nRegularTotalMinute += (double)totalMinutes;
                                }
                            }
                            
                        }
                    }
                }


                //Ot

                if (nCRewOtH > 0)
                {

                    nCHour = (int)(nCRewOtH * 60) / 60;
                    nCMin = (int)(Math.Round(nCRewOtH * 60,4)) % 60;
                    CrewOTTotal = nCHour.ToString().PadLeft(2, '0') + ":" + nCMin.ToString().PadLeft(2, '0');
                }
                else
                {
                    CrewOTTotal = nCHour.ToString().PadLeft(2, '0') + ":" + nCMin.ToString().PadLeft(2, '0');
                }

                hour = (int)subTotal / 60;
                subTotal = (int)subTotal % 60;
                TotalM = hour.ToString().PadLeft(2, '0') + ":" + subTotal.ToString().PadLeft(2, '0');
                //Regular
                hour = 0;
                hour = (int)nRegularTotalMinute / 60;
                nRegularTotalMinute = (int)nRegularTotalMinute % 60;
                TotalRegular = hour.ToString().PadLeft(2, '0') + ":" + nRegularTotalMinute.ToString().PadLeft(2, '0');

                if (grdLaberTrack.Rows.Count > 0)
                    this.AddTotalRow("Total", TotalRegular, CrewOTTotal, TotalM);

            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    int currentId = 0;
    decimal subTotal = 0;
    int subTotalRowIndex = 0;
    int hour = 0;
    string TotalM = "";
    string TotalRegular = "";
    int nCHour = 0;
    int nCMin = 0;
    double nCRewOtH = 0;
    double nRegularTotalMinute= 0;
    string CrewOTTotal = "";
    protected void grdLaberTrack_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (chkTotalhours.Checked == true)
            {

                subTotal = 0;
                nCRewOtH = 0;
                nCHour = 0;
                nCMin = 0;
                nRegularTotalMinute = 0;
                hour = 0;
                if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItem is DataRowView && (e.Row.DataItem as DataRowView).DataView.Table != null)
                    {
                        DataTable dt = (e.Row.DataItem as DataRowView).DataView.ToTable(); //(e.Row.DataItem as DataRowView).DataView.Table;
                        int UserID = Convert.ToInt32(dt.Rows[e.Row.RowIndex]["UserID"]);

                        if (UserID != currentId)
                        {
                            if (e.Row.RowIndex > 0)
                            {
                                for (int i = subTotalRowIndex; i < e.Row.RowIndex; i++)
                                {
                                    DateTime StartTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[17].Text);  //imran
                                    string eTime = (grdLaberTrack.Rows[i].Cells[18].Text).ToString();    //imran
                                    if (eTime != null && eTime != "")
                                    {
                                        DateTime EndTime = Convert.ToDateTime(grdLaberTrack.Rows[i].Cells[18].Text);    //imran

                                        if (EndTime.Year != 2000)
                                        {
                                            string dayName = grdLaberTrack.Rows[i].Cells[19].Text;    //imran
                                            StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
                                            StartTime = StartTime.AddSeconds(-StartTime.Second);
                                            EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
                                            EndTime = EndTime.AddSeconds(-EndTime.Second);
                                            TimeSpan span = EndTime.Subtract(StartTime);
                                            decimal totalMinutes = 0;
                                            double totalHours = 0;
                                            double totalRegularHours = 0;
                                            totalMinutes = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                                            subTotal += totalMinutes;
                                            if (dayName.ToLower() == "saturday" || dayName.ToLower() == "sunday")
                                            {
                                                totalHours = span.TotalHours;
                                                nCRewOtH += totalHours;
                                            }
                                            else
                                            {
                                               
                                                if (span.TotalHours > 8)
                                                {
                                                    totalHours = span.TotalHours - 8;
                                                    totalRegularHours = span.TotalHours - totalHours;
                                                    nRegularTotalMinute += (totalRegularHours * 60);
                                                    nCRewOtH += totalHours;

                                                }
                                                else
                                                {
                                                    nRegularTotalMinute += (double)totalMinutes;
                                                }
                                            }
                                        }
                                    }
                                }

                                //ot
                                if (nCRewOtH > 0)
                                {

                                    nCHour = (int)(nCRewOtH * 60) / 60;
                                    nCMin = (int)(Math.Round(nCRewOtH * 60,4)) % 60;
                                    CrewOTTotal = nCHour.ToString().PadLeft(2, '0') + ":" + nCMin.ToString().PadLeft(2, '0');
                                }
                                else
                                {
                                    CrewOTTotal = nCHour.ToString().PadLeft(2, '0') + ":" + nCMin.ToString().PadLeft(2, '0');
                                }

                                //total
                                hour = (int)subTotal / 60;
                                subTotal = (int)subTotal % 60;
                                TotalM = hour.ToString().PadLeft(2, '0')+ ":" + subTotal.ToString().PadLeft(2, '0');
                                //Regular
                                hour = 0;
                                hour = (int)nRegularTotalMinute / 60;
                                nRegularTotalMinute = (int)nRegularTotalMinute % 60;
                                TotalRegular = hour.ToString().PadLeft(2, '0') + ":" + nRegularTotalMinute.ToString().PadLeft(2, '0');
                                if (grdLaberTrack.Rows.Count > 0)
                                    this.AddTotalRow("Total", TotalRegular, CrewOTTotal, TotalM);

                                subTotalRowIndex = e.Row.RowIndex;
                            }
                            currentId = UserID;
                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void AddTotalRow(string labelText, string RegularHour,  string OTValue, string TotalHour)
    {
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        row.BackColor = ColorTranslator.FromHtml("#F9F9F9");

        row.Cells.AddRange(new TableCell[17] {new TableCell() ,new TableCell (),new TableCell (), new TableCell(),  new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),new TableCell (),//Empty Cell
                                        new TableCell { Text = labelText, HorizontalAlign = HorizontalAlign.Right,CssClass="cellColor"},
                                        new TableCell { Text = RegularHour, HorizontalAlign = HorizontalAlign.Center,CssClass="cellColor" },new TableCell {Text = OTValue, HorizontalAlign = HorizontalAlign.Center,CssClass="cellColor" },new TableCell {Text = TotalHour, HorizontalAlign = HorizontalAlign.Center,CssClass="cellColor" },new TableCell () });

        grdLaberTrack.Controls[0].Controls.Add(row);
    }
    protected void ddlInstaller_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
       
        if (chkTotalhours.Checked == true)
        {
            BindSubTotalHours();
          
        }
        else
        {
            chkTotalhours.Checked = false;
            BindLaborHourTracking();
        }
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        lblResult.Text = "";
        // ddlInstaller.SelectedIndex = -1;
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        chkTotalhours.Checked = false;
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        Session.Remove("Installer");
        Session.Remove("");
        radEmployeeType.SelectedValue = "3";
        if (Session["CPUFilter"] != null)
        {
            Session.Remove("CPUFilter");

        }
        Session.Remove("Installer");
       
        selectedvalue = "";
       
        BindCrew();
       
        BindCustomer();
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
        chkTotalhours.Checked = false;
        GetLaberTracking(0);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("laborhourdetails.aspx?isCrew=1");
    }

    protected void btnExpCustList_Click(object sender, EventArgs e)
    {

        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click");
          
            DataClassesDataContext _db = new DataClassesDataContext();

            string strRequired = string.Empty;
            lblResult.Text = "";
            string strCondition = string.Empty;
           // strCondition = " where IsCrew=1 ";
            selectedvalue = "";
            foreach (ListItem item in lstCrew.Items)
            {
                if (item.Selected)
                {
                    selectedvalue += item.Value + ",";
                }
            }
            foreach (ListItem item1 in lstJobName.Items)
            {
                if (item1.Selected)
                {
                    selectedCustomerJobvalue += "'" + item1.Value + "',";
                }
            }
            if (radEmployeeType.SelectedValue != "3")
            {
                strCondition = " where IsCrew=" + radEmployeeType.SelectedValue;
            }
            if (selectedvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";
                else
                    strCondition = " where UserID in (" + selectedvalue.TrimEnd(',') + " )";
            }



            if (selectedCustomerJobvalue.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " AND gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
                else
                    strCondition = " where gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
            }

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                try
                {
                    Convert.ToDateTime(txtStartDate.Text.Trim());
                }
                catch
                {
                    strRequired += "Start Date is required.<br/>";

                }

                try
                {
                    Convert.ToDateTime(txtEndDate.Text.Trim());

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


                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                if (strCondition.Length > 0)
                {
                    strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                else
                {
                    strCondition = " where labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                              


            }

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if(strCondition.Length > 2)
                    strCondition += " AND gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%' ";
                else
                    strCondition += " where  gps.division_name like '%" + ddlDivision.SelectedItem.Text + "%' ";
            }


            //string strMaster = " SELECT DISTINCT UserID, Full_Name FROM GPSTracking " +
            //                   " INNER JOIN Crew_Details ON GPSTracking.UserID = Crew_Details.crew_id " +
            //                   " " + strCondition + " order by Full_Name, UserID";

            string strMaster = " SELECT DISTINCT gps.UserID, user_info.first_name + ' ' + user_info.last_name AS full_name " +
                            " FROM GPSTracking as gps  INNER JOIN " +
                            " user_info ON gps.UserID = user_info.user_id " +
                           " " + strCondition + 
                           " UNION " +
                           " SELECT DISTINCT gps.UserID, Crew_Details.full_name AS full_name " +
                           " FROM GPSTracking as gps INNER JOIN " +
                           " Crew_Details ON gps.UserID = Crew_Details.MaxCrewId " +
                            " " + strCondition + " ORDER BY full_name, gps.UserID ";
            DataTable dtMaster = csCommonUtility.GetDataTable(strMaster);

         
            string strQ = " SELECT GPSTrackID, StartPlace, gps.division_name, MakeStopPlace, EndPlace, Distance, Time, UserID, IsCrew," +
                     // " c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                     " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                      " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " user_info.first_name + ' ' + user_info.last_name as full_name, section_id, SectionName, StartTime, EndTime," +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress," +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  user_info ON gps.UserID = user_info.user_id " +
                       " left join customers as c on c.customer_id = gps.customer_id" +
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id " +
                       " " + strCondition +
                       " UNION " +
                       " SELECT   GPSTrackID, StartPlace, gps.division_name, MakeStopPlace, EndPlace, Distance, Time, UserID, IsCrew," +
                       //" c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as CustomerName, " +
                       " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                      " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as CustomerName, " +
                       " Crew_Details.full_name AS full_name, section_id, SectionName, StartTime, EndTime, " +
                       " gps.customer_id, gps.Estimate_id, labor_date, deviceName, StartCustomerAddress, " +
                       " EndCustomerAddress, gps.Notes, DATENAME(DW, labor_date) AS WorkingDayName " +
                       " FROM GPSTracking as gps " +
                       " INNER JOIN  Crew_Details ON gps.UserID = Crew_Details.MaxCrewId " +
                       " left join customers as c on c.customer_id = gps.customer_id" +
                       " left join customer_estimate as ce on ce.customer_id = gps.customer_id and ce.estimate_id = gps.Estimate_id " +
                       " " + strCondition + " order by labor_date desc ";

            DataTable dtReport = csCommonUtility.GetDataTable(strQ);
            DataView dv = dtReport.DefaultView;

            string SourceFilePath = Server.MapPath(@"Reports\Common") + @"\";
            string FilePath = Server.MapPath(@"Reports\excel_report") + @"\";
            string sFileName = "LaborTrackingList" + DateTime.Now.Ticks.ToString() + ".xlsx";

            FileInfo tempFile = new FileInfo(SourceFilePath + "LaborTrackingList.xlsx");
            tempFile.CopyTo(FilePath + sFileName);
            FileInfo newFile = new FileInfo(FilePath + sFileName);
            int nCount = 2;

            double grandTotalMin = 0;
            double grandOTMin = 0;
            double grandRegularMin = 0;
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["LaborTrackingList"];
                for (int i = 0; i < dtMaster.Rows.Count; i++)
                {
                    int nUserID = Convert.ToInt32(dtMaster.Rows[i]["UserID"]);
                    string strFullName = dtMaster.Rows[i]["full_name"].ToString();
                    dv.RowFilter = "UserID = " + nUserID;


                    if (dv.Count > 0)
                    {
                        double dTotalMin = 0;
                        double dOTTotalMin = 0;
                        double dRegularGTotalMin = 0;


                        for (int q = 0; q < dv.Count; q++)
                        {
                            DataRowView dr = dv[q];
                            string StartPlace = "";
                            string EndPlace = "";
                            StartPlace = dr["StartPlace"].ToString();
                            EndPlace = dr["EndPlace"].ToString();

                            worksheet.Cells[nCount, 1].Value = dr["WorkingDayName"].ToString().Substring(0, 3) + "(" + Convert.ToDateTime(dr["labor_date"]).ToString("MM/dd/yyyy") + ")";
                            worksheet.Cells[nCount, 2].Value = strFullName;


                            if (dr["CustomerName"].ToString().Length > 0)
                            {
                                worksheet.Cells[nCount, 3].Value = dr["CustomerName"].ToString();
                            }


                            worksheet.Cells[nCount, 4].Value = dr["division_name"].ToString();



                            if (dr["SectionName"].ToString() != "Select")
                                worksheet.Cells[nCount, 5].Value = dr["SectionName"].ToString();

                            worksheet.Cells[nCount, 6].Value = dr["Notes"].ToString();

                            if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
                            {

                                worksheet.Cells[nCount, 7].Value = StartPlace.Remove(StartPlace.Length - 5, 5);
                            }
                            else
                            {
                                if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
                                {

                                    worksheet.Cells[nCount, 7].Value = "";
                                }
                                else
                                {

                                    worksheet.Cells[nCount, 7].Value = StartPlace;
                                }
                            }

                            if (EndPlace == "0")
                            {

                                worksheet.Cells[nCount, 8].Value = "";
                            }
                            else
                            {
                                if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
                                {
                                    worksheet.Cells[nCount, 8].Value = EndPlace.Remove(EndPlace.Length - 5, 5); ;

                                }
                                else
                                {

                                    worksheet.Cells[nCount, 8].Value = EndPlace;
                                }
                            }

                            worksheet.Cells[nCount, 9].Value = Convert.ToDateTime(dr["StartTime"]).ToShortTimeString();

                            if (Convert.ToDateTime(dr["EndTime"]).Year != 2000)
                            {
                                worksheet.Cells[nCount, 10].Value = Convert.ToDateTime(dr["EndTime"]).ToShortTimeString();
                                DateTime StartTime = Convert.ToDateTime(dr["StartTime"].ToString());
                                DateTime EndTime = Convert.ToDateTime(dr["EndTime"].ToString());
                                StartTime = StartTime.AddMilliseconds(-StartTime.Millisecond);
                                StartTime = StartTime.AddSeconds(-StartTime.Second);
                                EndTime = EndTime.AddMilliseconds(-EndTime.Millisecond);
                                EndTime = EndTime.AddSeconds(-EndTime.Second);
                                TimeSpan span = EndTime.Subtract(StartTime);
                                double OtH = 0;
                                double regularHr = 0;
                                dTotalMin += span.TotalMinutes;// (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                                grandTotalMin += span.TotalMinutes;
                                if (span.Days > 0)
                                    worksheet.Cells[nCount, 13].Value = span.Days + ":" + span.Hours + ":" + span.Minutes;
                                else
                                    worksheet.Cells[nCount, 13].Value = span.ToString(@"hh\:mm");
                                if (dr["WorkingDayName"].ToString().ToLower() == "saturday" || dr["WorkingDayName"].ToString().ToLower() == "sunday")
                                {
                                    OtH = span.TotalHours;
                                    dOTTotalMin += (OtH * 60);
                                    grandOTMin += (OtH * 60);
                                    int nOTHour = 0;
                                    double nOTMin = 0;
                                    nOTHour = (int)(OtH * 60) / 60;
                                    nOTMin = (OtH * 60) % 60;
                                    worksheet.Cells[nCount, 11].Value = nOTHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nOTMin, 6).ToString().PadLeft(2, '0');
                                    worksheet.Cells[nCount, 10].Value = "00:00";
                                }
                                else
                                {
                                   
                                    if (span.TotalHours > 8)
                                    {
                                        OtH = span.TotalHours - 8;
                                        regularHr = span.TotalHours - OtH;
                                        dRegularGTotalMin += regularHr * 60;
                                        grandRegularMin += regularHr * 60;
                                        dOTTotalMin += (OtH * 60);
                                        grandOTMin += (OtH * 60);
                                        int nOTHour = 0;
                                        double nOTMin = 0;
                                        nOTHour = (int)(OtH * 60) / 60;
                                        nOTMin = (OtH * 60) % 60;
                                        worksheet.Cells[nCount, 11].Value = nOTHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nOTMin, 6).ToString().PadLeft(2, '0');

                                        //Regular 
                                        nOTHour = 0;
                                        nOTMin = 0;
                                        nOTHour = (int)(regularHr * 60) / 60;
                                        nOTMin = (regularHr * 60) % 60;
                                        worksheet.Cells[nCount, 10].Value = nOTHour.ToString().PadLeft(2, '0') + ":" + Math.Round(nOTMin, 6).ToString().PadLeft(2, '0');

                                    }
                                    else
                                    {

                                        dRegularGTotalMin += span.TotalMinutes;
                                        grandRegularMin += span.TotalMinutes;
                                        worksheet.Cells[nCount, 11].Value = span.ToString(@"hh\:mm");
                                        worksheet.Cells[nCount, 12].Value = "00:00";

                                    }
                                }
                            }
                            else
                            {
                                worksheet.Cells[nCount, 11].Value = "00:00";
                                worksheet.Cells[nCount, 12].Value = "00:00";
                                worksheet.Cells[nCount, 13].Value = "00:00";
                                worksheet.Cells[nCount, 10].Value = "";

                            }

                            nCount++;
                        }

                        int nHour = 0;
                        int nMin = 0;


                        worksheet.Cells[nCount, 10].Value = "Total:";
                        worksheet.Cells[nCount, 10].Style.Font.Bold = true;
                        worksheet.Cells[nCount, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ///Regular
                        nHour = (int)dRegularGTotalMin / 60;
                        nMin = (int)Math.Round(dRegularGTotalMin % 60, 6);
                        worksheet.Cells[nCount, 11].Value = nHour.ToString().PadLeft(2, '0') + ":" + nMin.ToString().PadLeft(2, '0');
                        worksheet.Cells[nCount, 11].Style.Font.Bold = true;

                        //OT

                        nHour = 0;
                        nMin = 0;
                        nHour = (int)dOTTotalMin / 60;
                        nMin = (int)Math.Round(dOTTotalMin % 60, 6);
                        worksheet.Cells[nCount, 12].Value = nHour.ToString().PadLeft(2, '0') + ":" + nMin.ToString().PadLeft(2, '0'); ;
                        worksheet.Cells[nCount, 12].Style.Font.Bold = true;


                        //Total

                        nHour = 0;
                        nMin = 0;
                        nHour = (int)dTotalMin / 60;
                        nMin = (int)Math.Round(dTotalMin % 60, 6);
                        worksheet.Cells[nCount, 13].Value = nHour.ToString().PadLeft(2, '0') + ":" + nMin.ToString().PadLeft(2, '0');
                        worksheet.Cells[nCount, 13].Style.Font.Bold = true;
                        nCount++;
                    }

                }

                if (dtReport.Rows.Count > 0)
                {
                    nCount++;

                    worksheet.Cells[nCount, 10].Value = "Grand Total:";
                    worksheet.Cells[nCount, 10].Style.Font.Bold = true;
                    worksheet.Cells[nCount, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    //Regular 
                    int nHour2 = (int)grandRegularMin / 60;
                    int nMin2 = (int)Math.Round(grandRegularMin % 60, 6);
                    worksheet.Cells[nCount, 11].Value = nHour2.ToString().PadLeft(2, '0') + ":" + nMin2.ToString().PadLeft(2, '0');
                    worksheet.Cells[nCount, 11].Style.Font.Bold = true;


                    //OT
                    nHour2 = 0;
                    nMin2 = 0;
                    nHour2 = (int)grandOTMin / 60;
                    nMin2 = (int)Math.Round(grandOTMin % 60, 6);
                    worksheet.Cells[nCount, 12].Value = nHour2.ToString().PadLeft(2, '0') + ":" + nMin2.ToString().PadLeft(2, '0'); ;
                    worksheet.Cells[nCount, 12].Style.Font.Bold = true;

                    //Total
                    nHour2 = 0;
                    nMin2 = 0;
                    nHour2 = (int)grandTotalMin / 60;
                    nMin2 = (int)Math.Round(grandTotalMin % 60, 6);
                    worksheet.Cells[nCount, 13].Value = nHour2.ToString().PadLeft(2, '0') + ":" + nMin2.ToString().PadLeft(2, '0'); ;
                    worksheet.Cells[nCount, 13].Style.Font.Bold = true;

                    worksheet.Cells["A:I"].AutoFitColumns();
                    package.Save();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/excel_report/" + sFileName + "');", true);
                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("No records found.");
                }
            }
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        }
   
    protected void chkTotalhours_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkTotalhours.ID, chkTotalhours.GetType().Name, "Changed");
        if (chkTotalhours.Checked == true)
        {
            BindSubTotalHours();
            
        }
        else
        {
            chkTotalhours.Checked = false;
            BindLaborHourTracking();
        }
    }


    

    

    //arefin 09-14-2019 -- Formate StartCustomerAddress (Customer Start Address) & EndCustomerAddress (Customer End Address)
    protected void btnUpdateData_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateData.ID, btnUpdateData.GetType().Name, "Click");
        try
        {
            UpdateData();
            BindLaborHourTracking();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data Updated Successfully");
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    //arefin 09-14-2019 -- Formate StartCustomerAddress (Customer Start Address) & EndCustomerAddress (Customer End Address)
    private void UpdateData()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
                          " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress,Notes from GPSTracking  WHERE     (CrewLastLangitude IS NOT NULL) ";
            IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);

            foreach (var c in clist)
            {
                GPSTracking objGT = _db.GPSTrackings.Where(g => g.GPSTrackID == c.GPSTrackID).FirstOrDefault();

                objGT.StartCustomerAddress = c.StartCustomerAddress.Replace("Customer Name: ", "").Replace("Address: ", Environment.NewLine).Replace(",", ", ").Replace("AZ, ", "AZ ");
                objGT.EndCustomerAddress = c.EndCustomerAddress.Replace("Customer Name: ", "").Replace("Address: ", Environment.NewLine).Replace(",", ", ").Replace("AZ, ", "AZ ");
                _db.SubmitChanges();

            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            lblResult.Text = "";
            if (chkTotalhours.Checked == true)
            {
                BindSubTotalHours();

            }
            else
            {
                chkTotalhours.Checked = false;
                BindLaborHourTracking();
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

        
    }
    protected void btnGMap_Click(object sender, EventArgs e)
    {
        Response.Redirect("gCusCrewTrackermap.aspx");
    }


    protected void radEmployeeType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblResult.Text = "";

            if (chkTotalhours.Checked == true)
            {
                BindSubTotalHours();

            }
            else
            {
                chkTotalhours.Checked = false;
                BindLaborHourTracking();
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
       
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            Session.Remove("nLoaborHour");
            BindLaborHourTracking();
            GetLaberTracking(0);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
        
    }
}