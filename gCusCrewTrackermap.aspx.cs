using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Device.Location;
using Newtonsoft.Json;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;


public partial class gCusCrewTrackermap : System.Web.UI.Page
{
    string selectedvalue = "";
    string selectedCustomerJobvalue = "";
    int View = 0;
    int Activecrew = 0;
    List<csCrewLocation> eCoordForCrew = new List<csCrewLocation>();
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
         
            string Condition = string.Empty;
          
            txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            //ddlStartTime.SelectedValue = "5:00 AM";
            //ddlEndTime.SelectedValue = "7:00 PM";
            DataTable data = new DataTable();
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
        
            userinfo objUser = (userinfo)Session["oUser"];
            BindCrew();
            BindCustomer();
            LoadData1();

        }


    }



   
    private void LoadData1()
    {
        try
        {

            lblResult.Text ="";
            string Condition = "";
            DataClassesDataContext _db = new DataClassesDataContext();

            CustomerInfoCrewLocationTracker customerInfoCrewLocationTracker = new CustomerInfoCrewLocationTracker();

            string CrewJson = "", CustomerJson = "";
            List<CrewLocationTracker> gpsLocationItemClone = new List<CrewLocationTracker>();

             
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
                Condition = " where IsCrew=" + radEmployeeType.SelectedValue;
            }

            if (selectedvalue.Length > 0)
            {
                if (Condition.Length > 0)
                    Condition += " AND UserID in (" + selectedvalue.TrimEnd(',') + " )";
                else
                    Condition = " where UserID in (" + selectedvalue.TrimEnd(',') + " )";
            }



            if (selectedCustomerJobvalue.Length > 0)
            {
                if (Condition.Length > 0)
                    Condition += " AND gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
                else
                    Condition = " where gps.customer_estimate_id in (" + selectedCustomerJobvalue.TrimEnd(',') + " )";
            }

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {


                //if (ddlStartTime.SelectedItem.Text != "Start Time" && ddlEndTime.SelectedItem.Text != "End Time")
                //{
                //    DateTime StartTime = Convert.ToDateTime(ddlStartTime.SelectedValue.Trim());
                //    DateTime Endtime = Convert.ToDateTime(ddlEndTime.SelectedValue.Trim());

                //    DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                //    DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                //    dt1 = dt1.Add(StartTime.TimeOfDay);
                //    dt2 = dt2.Add(Endtime.TimeOfDay);

                //    Condition += "AND (gps.StartTime >= '" + dt1 + "' and gps.EndTime <='" + dt2.AddSeconds(59).AddMilliseconds(100) + "')";
                //}
                //else if (ddlStartTime.SelectedItem.Text != "Start Time")
                //{
                //    DateTime StartTime = Convert.ToDateTime(ddlStartTime.SelectedValue.Trim());
                //    DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                //    dt1 = dt1.Add(StartTime.TimeOfDay);
                //    Condition += "AND (gps.StartTime >= '" + dt1 + "' and gps.StartTime <='" + dt1.AddSeconds(59).AddMilliseconds(100) + "')";
                //    // Condition += "AND gps.StartTime >= '" + dt1 + "'";
                //}
                //else if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
                if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
                {
                    DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                    DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                    if (Condition.Length > 0)
                    {
                        Condition += " AND labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                    }
                    else
                    {
                        Condition = " where labor_date >= '" + dt1 + "' and labor_date <'" + dt2.AddDays(1) + "'";
                    }
                }


            }


            string gpsLocationQuery = " select gps.StartLatitude,gps.StartLogitude,gps.EndLatitude,ltrim(right(convert(varchar(25), gps.StartTime, 100), 7)) as StartTime,gps.CustomerName,gps.StartPlace,gps.EndPlace, " +
                                      " ltrim(right(convert(varchar(25), gps.EndTime, 100), 7)) as EndTime,gps.EndLogitude,cd.full_name as CrewfullName,labor_date,gps.EndTime as endtime2 from GPSTracking as gps" +
                                      " INNER JOIN Crew_Details AS cd ON gps.UserID = cd.MaxCrewId " +
                                      " " + Condition + "  and (( gps.StartLatitude is not null and gps.StartLatitude <> '0') or(gps.EndLatitude <> '0' and gps.EndLatitude is not null) ) " +
                                      " union "+
                                      " select gps.StartLatitude,gps.StartLogitude,gps.EndLatitude,ltrim(right(convert(varchar(25), gps.StartTime, 100), 7)) as StartTime,gps.CustomerName,gps.StartPlace,gps.EndPlace, " +
                                      " ltrim(right(convert(varchar(25), gps.EndTime, 100), 7)) as EndTime,gps.EndLogitude,u.first_name+' '+u.last_name AS CrewfullName,labor_date,gps.EndTime as endtime2 from GPSTracking as gps" +
                                      " INNER JOIN user_info AS u ON gps.UserID =u.user_id " +
                                      " " + Condition + "  and (( gps.StartLatitude is not null and gps.StartLatitude <> '0') or(gps.EndLatitude <> '0' and gps.EndLatitude is not null) ) ";

            List<CrewLocationTracker> gpsLocationItem = _db.ExecuteQuery<CrewLocationTracker>(gpsLocationQuery, string.Empty).ToList();

            gpsLocationItemClone.AddRange(gpsLocationItem);
            if (gpsLocationItemClone.Count > 0)
            {
                CrewJson = JsonConvert.SerializeObject(gpsLocationItemClone);
                CustomerJson = JsonConvert.SerializeObject(customerInfoCrewLocationTracker);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CustomerCrewTrackerMap", "CustomerCrewTrackerMap(" + CrewJson + "," + CustomerJson + ");", true);
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No records found.");
                double DefaultrCustomerLat = 33.4122784,//usa
                DefaultrCustomerlong = -112.0836409;//usa
                //double DefaultrCustomerLat = 23.6850,//bangladesh
                //DefaultrCustomerlong = 90.3563;//bangladesh



                Page.ClientScript.RegisterStartupScript(this.GetType(), "DefaultrCustomerCrewTrackerMap", "DefaultrCustomerCrewTrackerMap('" + DefaultrCustomerLat + "','" + DefaultrCustomerlong + "');", true);
           
             }
            string strQq = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[IsCrew],[CustomerName],[section_id], " +
                    " [SectionName],StartTime, EndTime,[customer_id],[Estimate_id],[labor_date],DeviceName,StartCustomerAddress,EndCustomerAddress,Notes from GPSTracking as gps " + Condition + " order by labor_date desc ";
            IEnumerable<CrewTrack> clistt = _db.ExecuteQuery<CrewTrack>(strQq, string.Empty);
            DataTable DTLoaborHour2 = csCommonUtility.LINQToDataTable(clistt);
            DateTime EndTimedt = new DateTime(2000, 01, 01);

       
            var nCount = from myRow in DTLoaborHour2.AsEnumerable()
                         where myRow.Field<DateTime>("EndTime") == EndTimedt
                         select myRow;

            Activecrew = nCount.Count();
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

           
            String aSql = "SELECT  distinct  ce.customer_estimate_id " +
                            "FROM GPSTracking AS gps inner JOIN " +
                            " customer_estimate AS ce ON gps.Estimate_id = ce.estimate_id  and gps.customer_id = ce.customer_id " +
                            " WHERE(ce.status_id = 3) AND(ce.IsEstimateActive = 1)  " + strCondition2;

            DataTable dts = csCommonUtility.GetDataTable(aSql);
            View = dts.Rows.Count;
            lblView.Text = View.ToString();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        lblResult.Text = "";
        LoadData1();
    }

    protected void radEmployeeType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadData1();
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        //Session.Remove("gCusCrewTrackerData");
        selectedvalue = "";
        BindCrew();
        BindCustomer();
        lblResult.Text = "";
        //ddlStartTime.SelectedValue = "5:00 AM";
        //ddlEndTime.SelectedValue = "7:00 PM";
        txtStartDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
        txtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
      
        Session.Remove("Installer");
        Session.Remove("");
        LoadData1();
      
       



    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click");
        string strRequired = string.Empty;
        lblResult.Text = "";
        Session.Remove("gCusCrewTrackerData");
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
            if (Convert.ToDateTime(txtStartDate.Text.Trim()) > Convert.ToDateTime(txtEndDate.Text.Trim()))
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
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            LoadData1();
            return;
        }
        LoadData1();

    }
    private void BindCrew()
    {
        try
        {
            string strQ = " SELECT first_name +' ' + last_name AS full_name, MaxCrewId AS UserId " +
                          " FROM Crew_Details " +
                          " WHERE is_active =1 " +
                          " UNION " +
                          " SELECT first_name + ' ' + last_name +' (Emp)' AS full_name, user_id AS UserId " +
                          " FROM user_info " +
                          " WHERE is_active =1 and IsTimeClock=1 ORDER BY full_name ";

            DataTable dt = csCommonUtility.GetDataTable(strQ);
            lstCrew.DataSource = dt;
            lstCrew.DataTextField = "full_name";
            lstCrew.DataValueField = "UserId";
            lstCrew.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void BindCustomer()
    {
        try
        {
            string strQ = " select customer_estimate_id,c.customer_id,ce.customer_estimate_id,c.last_name1, " +
                           //"  c.last_name1 + ' ' + c.first_name1 + '(' + ce.job_number + ')'  as customername " +
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

   
}