using System;
using System.Collections.Generic;
using System.Data;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mtimeclock : System.Web.UI.Page
{
    public DataTable dtSection;
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        //return (from c in _db.customers
        //        where c.last_name1.StartsWith(prefixText)
        //        join ce in _db.customer_estimates on c.customer_id equals ce.customer_id
        //        where ce.status_id == 3
        //        select c.first_name1 + " " + c.last_name1 + " (" + ce.job_number + ")").Take<String>(count).ToArray();
        List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["tSearch"];
        return (from c in cList
                where c.jobNumber.ToLower().Contains(prefixText.ToLower())
                select c.jobNumber).Take<String>(count).ToArray();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
              

                Response.Redirect("mobile.aspx");
               // lblResult.Text = csCommonUtility.GetSystemErrorMessage("Session Empty");

               
            }
            else
            {
                csCommonUtility.WriteLog("M time clock Crew Session found.");

                btnStartTrip.Attributes.Add("class", "btn btn-success");
                btnEndTrip.Attributes.Add("class", "btn btn-danger endbuttonwidth125");
                CheckEndTime();
            }
            BindJob();
            lblDateTime.Text = DateTime.Now.ToShortDateString();
            BindLaborHourTracking();
        }


    }
    private void BindJob()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = string.Empty;
            strQ = " select c.last_name1+'('+ce.estimate_name +')' as customer_name,c.customer_id,ce.customer_estimate_id,c.last_name1," +
                       " case when ce.alter_job_number!='' then c.last_name1 + ' ' + c.first_name1 + '(' + ce.alter_job_number + ')' " +
                       " else  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')' end as jobNumber " +
                       " from customers as c " +
                        " inner join customer_estimate as ce on c.customer_id=ce.customer_id " +
                        " where ce.status_id = 3 and ce.IsEstimateActive=1 " +
                        " order by c.last_name1";


            List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
            Session.Add("tSearch", mList);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void CheckEndTime()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            // Crew 
            if (Session["oCrew"] != null)
            {

                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                GPSTracking objCrew = _db.GPSTrackings.FirstOrDefault(gps => gps.UserID == objC.MaxCrewId && gps.IsCrew == true && (Convert.ToDateTime(gps.EndTime).Year == 2000));
                if (objCrew != null)
                {
                    if (Convert.ToDateTime(objCrew.EndTime).Year == 2000)
                    {
                        timeClockNote.Visible = true;
                        txtNotes.Text = objCrew.Notes;
                        btnStartTrip.Visible = false;  // Start Button
                        pnlEndButton.Visible = true;
                        lblStartTime.Text = Convert.ToDateTime(objCrew.StartTime).ToShortTimeString();
                        lblSearchCustomerName.Text = objCrew.CustomerName;
                        txtSearch.Text = objCrew.CustomerName;
                        hdnGpsId.Value = objCrew.GPSTrackID.ToString();
                        if (objCrew.customer_id != 0 && objCrew.Estimate_id != 0)
                        {
                            hdnCustomerId.Value = objCrew.customer_id.ToString();
                            hdnEstimateId.Value = objCrew.Estimate_id.ToString();
                            hdnCustomerEstimateId.Value = objCrew.customer_estimate_id.ToString();
                            LoadSection(Convert.ToInt32(objCrew.customer_id), Convert.ToInt32(objCrew.Estimate_id));
                            ddlSection.SelectedValue = objCrew.section_id.ToString();
                        }
                        else
                        {
                            hdnCustomerId.Value = objCrew.customer_id.ToString();
                            hdnEstimateId.Value = objCrew.Estimate_id.ToString();
                            hdnCustomerEstimateId.Value = objCrew.customer_estimate_id.ToString();
                            LoadSection(0, 0);
                        }
                    }
                    else
                    {
                        LoadSection(0, 0);
                    }

                }
                else
                {
                    LoadSection(0, 0);
                }

            }

            // User 
            if (Session["oUser"] != null)
            {


                userinfo objUser = (userinfo)Session["oUser"];
                GPSTracking objCrew = _db.GPSTrackings.FirstOrDefault(gps => gps.UserID == objUser.user_id && gps.IsCrew == false && (Convert.ToDateTime(gps.EndTime).Year == 2000));
                if (objCrew != null)
                {
                    if (Convert.ToDateTime(objCrew.EndTime).Year == 2000)
                    {
                        timeClockNote.Visible = true;
                        txtNotes.Text = objCrew.Notes;
                        btnStartTrip.Visible = false;  // Start Button
                        pnlEndButton.Visible = true;
                        lblStartTime.Text = Convert.ToDateTime(objCrew.StartTime).ToShortTimeString();
                        lblSearchCustomerName.Text = objCrew.CustomerName;
                        txtSearch.Text = objCrew.CustomerName;
                        hdnGpsId.Value = objCrew.GPSTrackID.ToString();
                        if (objCrew.customer_id != 0 && objCrew.Estimate_id != 0)
                        {
                            hdnCustomerId.Value = objCrew.customer_id.ToString();
                            hdnEstimateId.Value = objCrew.Estimate_id.ToString();
                            hdnCustomerEstimateId.Value = objCrew.customer_estimate_id.ToString();
                            LoadSection(Convert.ToInt32(objCrew.customer_id), Convert.ToInt32(objCrew.Estimate_id));
                            ddlSection.SelectedValue = objCrew.section_id.ToString();
                        }
                        else
                        {
                            hdnCustomerId.Value = objCrew.customer_id.ToString();
                            hdnEstimateId.Value = objCrew.Estimate_id.ToString();
                            hdnCustomerEstimateId.Value = objCrew.customer_estimate_id.ToString();
                            LoadSection(0, 0);
                        }
                    }
                    else
                    {
                        LoadSection(0, 0);
                    }

                }
                else
                {
                    LoadSection(0, 0);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void btnStartTrip_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnStartTrip.ID, btnStartTrip.GetType().Name, "Click"); 
        try
        {
            lblMSG.Text = "";
            lblResult.Text = "";
            lblEndTime.Text = "";
            lblNote.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            if (btnStartTrip.Text == "START CLOCK")
            {
                if (Session["oCrew"] != null)
                {
                   
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "getCurrentLocation", "getCurrentLocation('" + 1 + "');", true);
                        Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                        btnEndTrip.Attributes.Add("class", "btn btn-danger endbuttonwidth135");
                        lblStartTime.Text = DateTime.Now.ToShortTimeString();
                        pnlEndButton.Visible = true;
                        btnEndTrip.Visible = true;
                        btnStartTrip.Visible = false;  // Start Button
                        timeClockNote.Visible = true;
                        GPSTracking objCrew = new GPSTracking();
                        objCrew.deviceName = hdnDeviceName.Value;
                        objCrew.StartLatitude = hdnLatitude.Value;
                        objCrew.StartLogitude = hdnLongitude.Value;
                        objCrew.CreatedDate = DateTime.Now;
                        objCrew.labor_date = DateTime.Now;
                        objCrew.UserID = objC.MaxCrewId;
                        objCrew.IsCrew = true;
                        objCrew.Distance = "";// hdnDistance.Value;
                        objCrew.CustomerName = lblSearchCustomerName.Text;
                        objCrew.SectionName = ddlSection.SelectedItem.Text;
                        objCrew.Notes = txtNotes.Text;
                        if (ddlSection.SelectedItem.Text == "Select")
                        {
                            objCrew.section_id = 0;
                        }
                        else
                        {
                            objCrew.section_id = Convert.ToInt32(ddlSection.SelectedValue);
                        }
                        objCrew.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        objCrew.Estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                        objCrew.customer_estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        objCrew.StartTime = DateTime.Now; //lblStartTime.Text;
                        lblStartTime.Text = DateTime.Now.ToShortTimeString();
                        objCrew.EndTime = Convert.ToDateTime("01/01/2000");
                        objCrew.StartPlace = hdnStartLocation.Value;
                        objCrew.EndPlace = "0";
                        objCrew.Time = "0:0:0:0";
                        calculateDistanceFromCrewtoCustomer(objCrew);
                        _db.GPSTrackings.InsertOnSubmit(objCrew);
                        _db.SubmitChanges();
                        hdnGpsId.Value = objCrew.GPSTrackID.ToString();
                        GPSTrackingDetail objGPSTrackingDetail = new GPSTrackingDetail();
                        objGPSTrackingDetail.Latitude = hdnLatitude.Value;
                        objGPSTrackingDetail.Longitude = hdnLongitude.Value;
                        objGPSTrackingDetail.InputType = 0;
                        objGPSTrackingDetail.CreateDate = DateTime.Now;
                        objGPSTrackingDetail.GPSTrackID = objCrew.GPSTrackID;
                        _db.GPSTrackingDetails.InsertOnSubmit(objGPSTrackingDetail);
                        _db.SubmitChanges();
                    
                }

                // User 
                if (Session["oUser"] != null)
                {
                    
                        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "getCurrentLocation", "getCurrentLocation('" + 1 + "');", true);
                        userinfo objUser = (userinfo)Session["oUser"];
                        btnEndTrip.Attributes.Add("class", "btn btn-danger endbuttonwidth135");
                        lblStartTime.Text = DateTime.Now.ToShortTimeString();
                        pnlEndButton.Visible = true;
                        btnEndTrip.Visible = true;
                        btnStartTrip.Visible = false;  // Start Button
                        timeClockNote.Visible = true;
                       
                        GPSTracking objCrew = new GPSTracking();
                        objCrew.deviceName = hdnDeviceName.Value;
                        objCrew.StartLatitude = hdnLatitude.Value;
                        objCrew.StartLogitude = hdnLongitude.Value;
                        objCrew.CreatedDate = DateTime.Now;
                        objCrew.labor_date = DateTime.Now;
                        objCrew.UserID = objUser.user_id;
                        objCrew.IsCrew = false;
                        objCrew.Distance = "";// hdnDistance.Value;
                        objCrew.CustomerName = lblSearchCustomerName.Text;
                        objCrew.SectionName = ddlSection.SelectedItem.Text;
                        objCrew.Notes = txtNotes.Text;
                        if (ddlSection.SelectedItem.Text == "Select")
                        {
                            objCrew.section_id = 0;
                        }
                        else
                        {
                            objCrew.section_id = Convert.ToInt32(ddlSection.SelectedValue);
                        }
                        objCrew.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        objCrew.Estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                        objCrew.customer_estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        objCrew.StartTime = DateTime.Now; //lblStartTime.Text;
                        lblStartTime.Text = DateTime.Now.ToShortTimeString();
                        objCrew.EndTime = Convert.ToDateTime("01/01/2000");
                        objCrew.StartPlace = hdnStartLocation.Value;
                        objCrew.EndPlace = "0";
                        objCrew.Time = "0:0:0:0";
                        _db.GPSTrackings.InsertOnSubmit(objCrew);
                        _db.SubmitChanges();

                        hdnGpsId.Value = objCrew.GPSTrackID.ToString();
                        GPSTrackingDetail objGPSTrackingDetail = new GPSTrackingDetail();
                        objGPSTrackingDetail.Latitude = hdnLatitude.Value;
                        objGPSTrackingDetail.Longitude = hdnLongitude.Value;
                        objGPSTrackingDetail.InputType = 0;
                        objGPSTrackingDetail.CreateDate = DateTime.Now;
                        objGPSTrackingDetail.GPSTrackID = objCrew.GPSTrackID;
                        _db.GPSTrackingDetails.InsertOnSubmit(objGPSTrackingDetail);
                        _db.SubmitChanges();
                    
                }
            }
        }
        catch (Exception ex)
        {
            lblMSG.Text = csCommonUtility.GetSystemErrorMessage(ex.Message + ".---<br/>---" + ex.StackTrace);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message + ".---<br/>---" + ex.StackTrace);
        }

    }

    private void calculateDistanceFromCrewtoCustomer(GPSTracking objCustStartAddressinGpsTracking)
    {


        Crew_Detail crwDetailsobj = new Crew_Detail();
        csCrewLocation objLowestDistanceCustomer = new csCrewLocation();
        if (System.Web.HttpContext.Current.Session["oCrew"] != null)
        {
            crwDetailsobj = (Crew_Detail)System.Web.HttpContext.Current.Session["oCrew"];
        }
        try
        {
            DateTime date = DateTime.Now;
            double distance = 0.0;
            string start = date.ToShortDateString();
            string end = date.AddDays(1).ToShortDateString();

            List<csCrewLocation> NewitemList = new List<csCrewLocation>();
            DataClassesDataContext _db = new DataClassesDataContext();

            double lowest_distance = 100000000.00;
            Crew_Location objCLoaction = new Crew_Location();
            string sql = " SELECT sc.event_id, sc.employee_name, c.address+', '+c.city+', '+c.state+' '+c.zip_code As CustomerAddress," +
                         " c.first_name1+' '+c.last_name1 AS CfullName, sc.event_start, sc.event_end, " +
                         " c.customer_id, c.Latitude AS CustLatitude, c.Longitude AS CustLongitude  FROM ScheduleCalendar AS sc" +
                         " INNER JOIN customers AS c ON sc.customer_id = c.customer_id" +
                         " AND ( (event_start<= '" + start + "' and event_end>='" + start + "') or(event_start>= '" + start + "'  and  event_start<'" + end + "') )" +
                         " AND (sc.type_id IN (1, 11)) AND (sc.IsEstimateActive = 1) AND (c.is_active = 1) AND sc.employee_name like '%" + crwDetailsobj.full_name.Trim() + "%'";

            List<csCrewLocation> item = _db.ExecuteQuery<csCrewLocation>(sql, string.Empty).ToList();

            foreach (var i in item)
            {
                Crew_Detail obj = new Crew_Detail();
                string strEmployee_name = i.employee_name;

                string[] crAry = strEmployee_name.Split(',').Select(p => p.Trim()).ToArray();

                var crewList = _db.Crew_Details.Where(c => crAry.Contains(c.full_name)).ToList();
                var crew = crewList.FirstOrDefault(c => c.crew_id == crwDetailsobj.crew_id);

                //foreach (var cl in crewList)
                //{
                obj = new Crew_Detail();
                int nCrew_id = crew.crew_id;

                string s = " select crl.CrewId,  crl.Latitude AS CrewLatitude, crl.Longitude AS CrewLongitude from " +
                      " Crew_Location AS crl " +
                      " where  crl.CrewLocationId =(SELECT MAX(CrewLocationId) AS Expr1 FROM Crew_Location where Crew_Location.CrewId='" + nCrew_id + "')" +
                      " AND  CONVERT(varchar, crl.CreatedDate, 23)=CONVERT(varchar, getdate(), 23)";


                csCrewLocation objCrl = _db.ExecuteQuery<csCrewLocation>(s, string.Empty).FirstOrDefault();
                if (objCrl != null && i.CustLatitude != null && i.CustLongitude != null)
                {
                    double latForCrew = Convert.ToDouble(objCrl.CrewLatitude),
                  longForCrew = Convert.ToDouble(objCrl.CrewLongitude),
                  latForCustomer = Convert.ToDouble(i.CustLatitude),
                  longForCustomer = Convert.ToDouble(i.CustLongitude);

                    //if (latForCrew != null && longForCrew != null && latForCustomer != null && longForCustomer != null)
                    //{
                    var sCoord = new GeoCoordinate(latForCrew, longForCrew);
                    var eCoord = new GeoCoordinate(latForCustomer, longForCustomer);
                    distance = sCoord.GetDistanceTo(eCoord);
                    //  }
                    if (lowest_distance > distance)
                    {
                        objLowestDistanceCustomer = i;
                        lowest_distance = distance;

                    }
                    //}

                }

            }



            if (item.Count > 0)
            {

                if (objCustStartAddressinGpsTracking.StartCustomerAddress != null)
                {
                    objCustStartAddressinGpsTracking.EndCustomerAddress = objLowestDistanceCustomer.CfullName + " " + Environment.NewLine + "" + objLowestDistanceCustomer.CustomerAddress;
                    //objCustStartAddressinGpsTracking.CrewLastLangitude = objLowestDistanceCustomer.CrewLatitude;
                    //objCustStartAddressinGpsTracking.CrewLastLongitude = objLowestDistanceCustomer.CrewLongitude;
                }
                else
                {
                    objCustStartAddressinGpsTracking.StartCustomerAddress = objLowestDistanceCustomer.CfullName + " " + Environment.NewLine + "" + objLowestDistanceCustomer.CustomerAddress;
                    objCustStartAddressinGpsTracking.CrewLastLangitude = objLowestDistanceCustomer.CrewLatitude;
                    objCustStartAddressinGpsTracking.CrewLastLongitude = objLowestDistanceCustomer.CrewLongitude;
                }
            }



        }
        catch (Exception ex)
        {
            lblMSG.Text = csCommonUtility.GetSystemErrorMessage(ex.Message + ".---<br/>---" + ex.StackTrace);
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message + ".---<br/>---" + ex.StackTrace);
        }


    }

    protected void btnNoteUpdate_Click(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        lblNote.Text = "";
        GPSTracking objGPS = new GPSTracking();
        objGPS = _db.GPSTrackings.FirstOrDefault(c => c.GPSTrackID == Convert.ToInt32(hdnGpsId.Value));
        if (objGPS != null)
        {
            objGPS.Notes = txtNotes.Text;
            _db.SubmitChanges();
            lblNote.Text = csCommonUtility.GetSystemMessage("Note has been updated successfully.");
        }
        BindLaborHourTracking();
    }
    protected void btnEndTrip_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnEndTrip.ID, btnEndTrip.GetType().Name, "Click"); 
        btnStartTrip.Attributes.Add("class", "btn btn-success");
        btnEndTrip.Attributes.Add("class", "btn btn-danger endbuttonwidth125");
        btnStartTrip.Text = "START CLOCK";
        btnStartTrip.Visible = false;
        lblResult.Text = "";
        lblNote.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        if (Session["oCrew"] != null)
        {
            
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "getCurrentLocation", "getCurrentLocation('" + 3 + "');", true);
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                GPSTracking objCrew = _db.GPSTrackings.FirstOrDefault(gps => gps.UserID == objC.MaxCrewId && gps.IsCrew == true && (Convert.ToDateTime(gps.EndTime).Year == 2000));
                if (objCrew != null)
                {
                    objCrew.EndTime = DateTime.Now;
                    lblEndTime.Text = DateTime.Now.ToShortTimeString();
                    objCrew.EndPlace = hdnEndLcation.Value;
                    if (hdnDeviceName.Value == "0")
                        objCrew.deviceName = "Others";
                    else
                        objCrew.deviceName = hdnDeviceName.Value;
                    objCrew.EndLatitude = hdnLatitude.Value;
                    objCrew.EndLogitude = hdnLongitude.Value;
                    objCrew.Distance = "";// hdnDistance.Value;
                    objCrew.Notes = txtNotes.Text;
                    calculateDistanceFromCrewtoCustomer(objCrew);
                    _db.SubmitChanges();
                    btnStartTrip.Text = "START CLOCK";
                    btnStartTrip.Visible = true;
                    btnEndTrip.Visible = false;
                    timeClockNote.Visible = false;
                    txtNotes.Text = "";
                    hdnGpsId.Value = "0";
                    BindLaborHourTracking();
                

            }
        }

        // User 
        if (Session["oUser"] != null)
        {

            
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "getCurrentLocation", "getCurrentLocation('" + 3 + "');", true);
                userinfo objUser = (userinfo)Session["oUser"];
                GPSTracking objCrew = _db.GPSTrackings.FirstOrDefault(gps => gps.UserID == objUser.user_id && gps.IsCrew == false && (Convert.ToDateTime(gps.EndTime).Year == 2000));
                if (objCrew != null)
                {
                    objCrew.EndTime = DateTime.Now;
                    lblEndTime.Text = DateTime.Now.ToShortTimeString();
                    objCrew.EndPlace = hdnEndLcation.Value;
                    if (hdnDeviceName.Value == "0")
                        objCrew.deviceName = "Others";
                    else
                        objCrew.deviceName = hdnDeviceName.Value;
                    objCrew.EndLatitude = hdnLatitude.Value;
                    objCrew.EndLogitude = hdnLongitude.Value;

                    objCrew.Distance = "";// hdnDistance.Value;
                    objCrew.Notes = txtNotes.Text;
                    _db.SubmitChanges();
                    btnStartTrip.Text = "START CLOCK";
                    btnStartTrip.Visible = true;
                    btnEndTrip.Visible = false;
                    timeClockNote.Visible = false;
                    txtNotes.Text = "";
                    hdnGpsId.Value = "0";
                    BindLaborHourTracking();

                
            }
        }
    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {

        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click");
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = string.Empty;

            if (txtSearch.Text.Trim() != "")
            {

                string fullName = txtSearch.Text;
                string job = string.Empty;
                var JNum = fullName.Split('(');
                if (fullName.IndexOf("(") != -1)
                {
                    job = JNum[1];

                }
                job = job.Replace(")", "");

                int customer_id = 0;
                int Estimate_id = 0;


                if (_db.customer_estimates.Where(ce => ce.job_number == job.Trim() && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
                {
                    customer_estimate obj = _db.customer_estimates.FirstOrDefault(ce => ce.job_number == job.Trim() && ce.client_id == 1 && ce.status_id == 3);

                    customer_id = Convert.ToInt32(obj.customer_id);
                    Estimate_id = Convert.ToInt32(obj.estimate_id);
                    hdnEstimateId.Value = Estimate_id.ToString();
                    hdnCustomerId.Value = customer_id.ToString();
                    hdnCustomerEstimateId.Value = obj.customer_estimate_id.ToString();

                    lblSearchCustomerName.Text = fullName;
                }
                else
                {
                    customer_estimate obj = _db.customer_estimates.FirstOrDefault(ce => ce.alter_job_number == job.Trim() && ce.client_id == 1 && ce.status_id == 3);
                    if (obj != null)
                    {
                        customer_id = Convert.ToInt32(obj.customer_id);
                        Estimate_id = Convert.ToInt32(obj.estimate_id);
                        hdnEstimateId.Value = Estimate_id.ToString();
                        hdnCustomerId.Value = customer_id.ToString();
                        hdnCustomerEstimateId.Value = obj.customer_estimate_id.ToString();
                        lblSearchCustomerName.Text = fullName;
                    }

                }

                LoadSection(customer_id, Convert.ToInt32(hdnEstimateId.Value));
            }
            else
            {
                hdnEstimateId.Value = "0";
                hdnCustomerId.Value = "0";
                hdnCustomerEstimateId.Value = "0";
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    private void LoadSection(int custID, int estID)
    {

        try
        {

            DataTable tmpSTable = LoadSectionTable();
            DataRow dr = tmpSTable.NewRow();
            dr["section_id"] = -1;
            dr["section_name"] = "Select";
            tmpSTable.Rows.Add(dr);

            DataClassesDataContext _db = new DataClassesDataContext();


            var item = (from it in _db.customer_sections
                        join si in _db.sectioninfos on it.section_id equals si.section_id
                        where it.customer_id == custID && it.estimate_id == estID && it.client_id == 1
                        orderby si.section_name ascending
                        select new SectionInfo()
                        {
                            section_id = (int)it.section_id,
                            section_name = si.section_name

                        }).ToList();

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
            ddlSection.DataSource = tmpSTable;
            ddlSection.DataTextField = "section_name";
            ddlSection.DataValueField = "section_id";
            ddlSection.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        return table;
    }


    private void BindLaborHourTracking()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strCondition = string.Empty;
            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                strCondition = " where IsCrew=1 AND UserID = " + objC.MaxCrewId;
            }

            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                strCondition = " where  IsCrew=0 AND UserID = " + objUser.user_id;
            }


            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());

                if (strCondition.Length == 0)
                {

                    strCondition = " WHERE labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }
                else
                {
                    strCondition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                }


            }
            string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
                         " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName from GPSTracking  " + strCondition + " order by labor_date desc ";
            IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);
            Session.Add("nLoaborHour", csCommonUtility.LINQToDataTable(clist));

            GetLaberTracking(0);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void GetLaberTracking(int nPageNo)
    {

        if (Session["nLoaborHour"] != null)
        {
            DataTable dtLaborHour = (DataTable)Session["nLoaborHour"];
            grdLaberTrack.DataSource = dtLaborHour;
            grdLaberTrack.PageIndex = nPageNo;
            grdLaberTrack.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime" };
            grdLaberTrack.DataBind();
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
    float GrandHours = 0;
    int hour = 0;
    string TotalM = "";
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

                Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");

                if (Convert.ToDateTime(EndTime).Year != 2000)
                {
                    TimeSpan span = EndTime.Subtract(StartTime);
                    float totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                    // totalHours = totalHours / 60;
                    GrandHours += totalHours;
                    if (span.Days > 0)
                        lblTotalHours.Text = span.Days + ":" + span.Hours + ":" + span.Minutes;
                    else
                        lblTotalHours.Text = span.ToString(@"hh\:mm");//span.Hours + ":" + span.Minutes;
                }
                else
                {
                    lblTotalHours.Text = "";
                    e.Row.Cells[4].Text = "";
                }
                if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
                {
                    e.Row.Cells[1].Text = StartPlace.Remove(StartPlace.Length - 5, 5);
                }
                else
                {
                    if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
                    {
                        e.Row.Cells[1].Text = "";
                    }
                    else
                    {
                        e.Row.Cells[1].Text = StartPlace;
                    }

                }




                if (EndPlace == "0")
                {
                    e.Row.Cells[2].Text = "";
                }
                else
                {
                    if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
                    {
                        e.Row.Cells[2].Text = EndPlace.Remove(EndPlace.Length - 5, 5);
                    }
                    else
                    {
                        e.Row.Cells[2].Text = EndPlace;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }



        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            if (GrandHours >= 60)
            {
                hour = (int)GrandHours / 60;
                GrandHours = GrandHours % 60;
            }
            else
            {
                hour = 0;
                GrandHours = GrandHours % 60;
            }
            if (GrandHours < 10)
                TotalM = hour.ToString() + ":0" + GrandHours;
            else
                TotalM = hour.ToString() + ":" + GrandHours;

            e.Row.Cells[4].Text = "Total";
            e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
            e.Row.Cells[4].CssClass = "cellColor";
            e.Row.Cells[5].Text = TotalM.ToString();
            e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[5].CssClass = "cellColor";
        }

    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click"); 
        string strRequired = string.Empty;
        lblMSG.Text = "";
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
            if (Convert.ToDateTime(txtStartDate.Text.Trim()) >= Convert.ToDateTime(txtEndDate.Text.Trim()))
            {
                strRequired += "End Date must be greater than Start Date.<br/>";

            }
        }
        catch
        {
            strRequired += "End Date is required.<br/>";

        }
        if (strRequired.Length > 0)
        {
            lblMSG.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            return;
        }
        BindLaborHourTracking();
    }


    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        lblMSG.Text = "";
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        BindLaborHourTracking();

    }

    protected void grdLaberTrack_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLaberTrack.ID, grdLaberTrack.GetType().Name, "PageIndexChanging"); 
        GetLaberTracking(e.NewPageIndex);
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLaberTracking(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLaberTracking(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "Click"); 
        GetLaberTracking(0);
    }


}