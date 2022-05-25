using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class laborhourdetails : System.Web.UI.Page
{
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


    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
          
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
            }
            BindJob();
            BindCrewMember();
            if (Request.QueryString.Get("gpid") != null)
            {
                int ncrid = Convert.ToInt32(Request.QueryString.Get("gpid"));
                hdnGpsId.Value = ncrid.ToString();
                Session.Add("nGPSTrackingId", hdnGpsId.Value);
                btnDelete.Visible = true;
                GetLaborrackingDetails(ncrid);

            }
           
            if (Convert.ToInt32(hdnGpsId.Value) ==0)
            {
               
                lblHeaderTitle.Text = "Add New Time Entry";
                btnDelete.Visible = false;
                txtLaberDate.Text = DateTime.Now.ToShortDateString();
                ddCrewMember.Focus();
                LoadSection(0, 0);

            }
            this.Validate();
            btnDelete.OnClientClick = "return confirm('This will permanently delete this time entry.');";


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "btnAddNewTimeEntry", "btnDelete" });
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
    private void BindCrewMember()
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
            ddCrewMember.DataSource = dt;
            ddCrewMember.DataTextField = "full_name";
            ddCrewMember.DataValueField = "UserId";
            ddCrewMember.DataBind();
            ddCrewMember.Items.Insert(0, "Select");
            ddCrewMember.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void GetLaborrackingDetails(int ncrid)
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            GPSTracking objGPS = new GPSTracking();
            objGPS = _db.GPSTrackings.Single(c => c.GPSTrackID == Convert.ToInt32(hdnGpsId.Value));

            if (objGPS.section_id == 0)
            {
                LoadSection(0, 0);
            }
            else
            {

                LoadSection(Convert.ToInt32(objGPS.customer_id), Convert.ToInt32(objGPS.Estimate_id));
                ddlSection.SelectedValue = objGPS.section_id.ToString();
                hdnEstimateId.Value = objGPS.Estimate_id.ToString();
                hdnCustomerId.Value = objGPS.customer_id.ToString();
                hdnCustomerEstimateId.Value = objGPS.customer_estimate_id.ToString();
            }

            txtLaberDate.Text = Convert.ToDateTime(objGPS.labor_date).ToShortDateString();
            lblHeaderTitle.Text = "Labor Time Tracking Details";
            ddCrewMember.SelectedValue = objGPS.UserID.ToString();
           
            txtSearch.Text = objGPS.CustomerName;
            if (Convert.ToDateTime(objGPS.StartTime).ToShortTimeString().Contains("AM"))
            {
                txtStartTime.Text = Convert.ToDateTime(objGPS.StartTime).ToShortTimeString().Replace("AM", ""); ;
                ddlStartTime.SelectedValue = "1";
            }
            else
            {
                txtStartTime.Text = Convert.ToDateTime(objGPS.StartTime).ToShortTimeString().Replace("PM", ""); ;
                ddlStartTime.SelectedValue = "2";
            }
            if (Convert.ToDateTime(objGPS.EndTime).Year != 2000)
            {
                if (Convert.ToDateTime(objGPS.EndTime).ToShortTimeString().Contains("AM"))
                {
                    txtEndTime.Text = Convert.ToDateTime(objGPS.EndTime).ToShortTimeString().Replace("AM", ""); ;
                    ddEndTime.SelectedValue = "1";
                }
                else
                {
                    txtEndTime.Text = Convert.ToDateTime(objGPS.EndTime).ToShortTimeString().Replace("PM", "");
                    ddEndTime.SelectedValue = "2";
                }
            }
            if (Convert.ToDateTime(objGPS.EndTime).Year != 2000)
            {
                DateTime StartTime = Convert.ToDateTime(objGPS.StartTime);
                DateTime EndTime = Convert.ToDateTime(objGPS.EndTime);
                TimeSpan span = EndTime.Subtract(StartTime);
                if (span.Days > 0)
                    lblTotalTime.Text = span.Days + ":" + span.Hours + ":" + span.Minutes + " Day:Hr:Min";
                else
                    lblTotalTime.Text = span.ToString(@"hh\:mm") + " Hr:Min";

                
            }
            else
            {
                lblTotalTime.Text = "";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
   
   
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        try
        {
        
            lblResult.Text = "";

            string sTime = "";
            string eTime = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            GPSTracking objGPS = new GPSTracking();
            GPSTrackingDetail objGPSD = new GPSTrackingDetail();

            if (txtLaberDate.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Labor Date.");
                return;
            }
            if (ddCrewMember.SelectedItem.Text == "Select")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Crew/Employee.");
                return;
            }
          
            if (txtStartTime.Text.Trim().Contains("."))
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Start Time Format.");
                return;
            }
            if (txtStartTime.Text.Trim() == "00:00" || txtStartTime.Text.Trim() == "0:00" || txtStartTime.Text.Trim() == "0" || txtStartTime.Text.Trim() == "00" || txtStartTime.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Start Time.");
                return;
            }

            if (txtEndTime.Text.Trim().Contains(".") || txtEndTime.Text.Trim() == "0:00" || txtEndTime.Text.Trim() == "0" || txtEndTime.Text.Trim() == "00")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid End Time Format.");
                return;
            }

            if (Convert.ToInt32(hdnGpsId.Value)==0)
            {
                var crewList = (from c in _db.GPSTrackings where c.UserID == Convert.ToInt32(ddCrewMember.SelectedValue) && c.EndTime.ToString().Contains("2000") select c).ToList();

                if (crewList.Count > 0)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("This crew.employee didn't clock out from the previous time entry. Either ask the crew/employee to clock out or you may edit the previous entry with an end time.");
                    return;

                }
           
          }

            if (Convert.ToInt32(hdnGpsId.Value) > 0)
                objGPS = _db.GPSTrackings.Single(c => c.GPSTrackID == Convert.ToInt32(hdnGpsId.Value));
            if (txtSearch.Text.Trim() != "")
            {
                objGPS.CustomerName = txtSearch.Text.Trim();
                objGPS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                objGPS.Estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                objGPS.customer_estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
            }
            else
            {
                objGPS.CustomerName = "";
                objGPS.customer_id = 0;
                objGPS.Estimate_id = 0;
                objGPS.customer_estimate_id = 0;
            }
            objGPS.SectionName = ddlSection.SelectedItem.Text;
            if (ddlSection.SelectedItem.Text == "Select")
            {
                objGPS.section_id = 0;
            }
            else
            {
                objGPS.section_id = Convert.ToInt32(ddlSection.SelectedValue);
            }

            objGPS.labor_date = Convert.ToDateTime(txtLaberDate.Text);
            objGPS.UserID = Convert.ToInt32(ddCrewMember.SelectedValue);
            if (_db.user_infos.Any(u => u.user_id == Convert.ToInt32(ddCrewMember.SelectedValue)))
            {
                objGPS.IsCrew = false;
            }
            else
            {
                objGPS.IsCrew = true;
            }
          
            if (txtStartTime.Text.Contains(":"))
            {
                objGPS.StartTime = Convert.ToDateTime(txtStartTime.Text.Trim() + " " + ddlStartTime.SelectedItem.Text);
                sTime = txtStartTime.Text.Trim() + " " + ddlStartTime.SelectedItem.Text;
            }
            else
            {
                objGPS.StartTime = Convert.ToDateTime(txtStartTime.Text.Trim() + ":00" + " " + ddlStartTime.SelectedItem.Text);
               sTime = txtStartTime.Text.Trim() + ":00" + " " + ddlStartTime.SelectedItem.Text;
            }


            if (txtEndTime.Text.Trim() != "00:00" &&txtEndTime.Text.Trim() !="")
            {
                if (txtEndTime.Text.Contains(":"))
                {
                    objGPS.EndTime = Convert.ToDateTime(txtEndTime.Text.Trim() + " " + ddEndTime.SelectedItem.Text);
                    eTime = txtEndTime.Text.Trim() + " " + ddEndTime.SelectedItem.Text;
                }
                else
                {
                    objGPS.EndTime = Convert.ToDateTime(txtEndTime.Text.Trim() + ":00" + " " + ddEndTime.SelectedItem.Text);
                    eTime = txtEndTime.Text.Trim() + ":00" + " " + ddEndTime.SelectedItem.Text;
                }
            }
            else
            {
                string endT = Convert.ToDateTime(("01/01/2000")).ToString("dd/MM/yyyy") ;
                objGPS.EndTime = Convert.ToDateTime(endT);
            }
            if (Convert.ToDateTime(objGPS.EndTime).Year!=2000)
            {
                if (Convert.ToDateTime(sTime) >= Convert.ToDateTime(eTime))
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Clock Start time can't be later than Clock Out Time.");
                    return;
                }
                DateTime StartTime = new DateTime();
                DateTime EndTime = new DateTime();
                StartTime = Convert.ToDateTime(sTime);
                EndTime = Convert.ToDateTime(eTime);
               TimeSpan span = EndTime.Subtract(StartTime);
               if (span.Days > 0)
                   lblTotalTime.Text = span.Days + ":" + span.Hours + ":" + span.Minutes + " Day:Hr:Min";
               else
                   lblTotalTime.Text = span.ToString(@"hh\:mm") + " Hr:Min";
                
            }
            else
            {
                lblTotalTime.Text = "";
            }
            if (Convert.ToInt32(hdnGpsId.Value) == 0)
            {
            
                objGPS.deviceName = "Desktop";
                objGPS.CreatedDate = DateTime.Now;
                _db.GPSTrackings.InsertOnSubmit(objGPS);
                _db.SubmitChanges();
                lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");

            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
                _db.SubmitChanges();
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("labor_hour_list.aspx?IsCrew="+hdnIsCrew.Value);

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click");
            userinfo objU = (userinfo)Session["oUser"];
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
                if (_db.customer_estimates.Where(ce => ce.job_number == job.Trim() && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.status_id == 3).ToList().Count > 0)
                {
                    customer_estimate obj = _db.customer_estimates.FirstOrDefault(ce => ce.job_number == job.Trim() && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.status_id == 3);
                    customer_id = Convert.ToInt32(obj.customer_id);
                    Estimate_id = Convert.ToInt32(obj.estimate_id);
                    hdnEstimateId.Value = Estimate_id.ToString();
                    hdnCustomerId.Value = customer_id.ToString();
                    hdnCustomerEstimateId.Value = obj.customer_estimate_id.ToString();

                }
                else
                {
                    customer_estimate obj = _db.customer_estimates.FirstOrDefault(ce => ce.alter_job_number == job.Trim() && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.status_id == 3);
                    if (obj != null)
                    {
                        customer_id = Convert.ToInt32(obj.customer_id);
                        Estimate_id = Convert.ToInt32(obj.estimate_id);
                        hdnEstimateId.Value = Estimate_id.ToString();
                        hdnCustomerId.Value = customer_id.ToString();
                        hdnCustomerEstimateId.Value = obj.customer_estimate_id.ToString();

                    }
                }
                LoadSection(customer_id, Convert.ToInt32(hdnEstimateId.Value));
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
                        where it.customer_id == custID && it.estimate_id == estID && it.client_id.ToString().Contains(hdnClientId.Value)
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
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDelete.ID, btnDelete.GetType().Name, "Click"); 
        try
        {
            userinfo obj = (userinfo)Session["oUser"];
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";

            strQ = "insert into DeleteGPSTracking " +
                          " SELECT [GPSTrackID] ,[StartPlace] ,[StartLatitude],[StartLogitude] ,[EndLatitude] ,[EndLogitude],[MakeStopPlace], "+
                          " [EndPlace],[Distance],[Time]  ,[CreatedDate] ,[UserID] ,[CustomerName] ,[section_id],[SectionName] ,[StartTime], " +
                          " [EndTime],[customer_id],[Estimate_id],[labor_date],[deviceName],[IsCrew], " +"'"+ obj.username + "'"+",getdate() " +
                          " FROM GPSTracking " +
                          " WHERE GPSTrackID ='" + Convert.ToInt32(hdnGpsId.Value) + "'";
                         _db.ExecuteCommand(strQ, string.Empty);

            strQ = "delete from GPSTracking where GPSTrackID=" + Convert.ToInt32(hdnGpsId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            var nList = (from dgps in _db.GPSTrackingDetails where dgps.GPSTrackID == Convert.ToInt32(hdnGpsId.Value) select dgps).ToList();

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
             strQ = "delete from GPSTrackingDetails where GPSTrackID=" + Convert.ToInt32(hdnGpsId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
             Response.Redirect("labor_hour_list.aspx",false);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}