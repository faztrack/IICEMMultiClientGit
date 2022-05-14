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

public partial class gmap : System.Web.UI.Page
{
    List<csCrewLocation> eCoordForCrew = new List<csCrewLocation>();

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
                userinfo objUser = (userinfo)Session["oUser"];


                DateTime date = DateTime.Now;

                string start = "";
                string end = "";

                start = date.ToShortDateString();
                end = date.AddDays(1).ToShortDateString();

                txtScheduleDate.Text = start;

                CustomerLocation(start, end);
            }





        }

    }

    private void CustomerLocation(string start, string end)
    {
        try
        {
            lblResult.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            List<csCrewLocation> eCoord = new List<csCrewLocation>();


            string custListCurrentDate = " SELECT   Distinct sc.employee_name,  c.address+', '+c.city+', '+c.state+' '+c.zip_code As CustomerAddress," +
                                         " c.first_name1+' '+c.last_name1 AS  CfullName,  sc.event_start, sc.event_end," +
                                         " c.customer_id, c.Latitude AS CustLatitude, c.Longitude AS CustLongitude,s.first_name + ' ' + s.last_name AS superName FROM ScheduleCalendar AS sc" +
                                         " INNER JOIN customers AS c ON sc.customer_id = c.customer_id " +
                                         " AND ( (event_start<= '" + start + "' and event_end>='" + start + "') or(event_start>= '" + start + "'  and  event_start<'" + end + "') )" +
                                         " AND (sc.type_id IN (1, 11)) AND (sc.IsEstimateActive = 1) AND (c.is_active = 1) AND sc.employee_name not like '%TBD TBD%'" +
                                         " AND sc.employee_name !=''"+
                                         " LEFT OUTER JOIN user_info as s on c.SuperintendentId = s.user_id";
            List<csCrewLocation> item = _db.ExecuteQuery<csCrewLocation>(custListCurrentDate, string.Empty).ToList();

            if (item.Count == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No Data found");
            }
            Regex regex_newline = new Regex("(\r\n|\r|\n|)");
            foreach (var i in item)
            {
                double latForCustomer = Convert.ToDouble(i.CustLatitude),
                       longForCustomer = Convert.ToDouble(i.CustLongitude);
                       i.SuperintendentLatitude = i.CustLatitude;
                      i.SuperintendentLongitude = i.CustLongitude;
                if (i.superName == null) {
                    i.superName = "TBD";
                }

                if (i.CustLatitude != null && i.CustLongitude != null)
                {
                    i.CustomerAddress = regex_newline.Replace(i.CustomerAddress, "");
                    i.CfullName = i.CfullName.Replace("\"", "");
                    //  .Replace(@"""", "")
                    eCoord.Add(i);
                }

                ScheduleCrewList(i, start, end);

            }
            if (eCoord.Count > 0 && (chkCustomer.Checked || chkCrew.Checked||chkSuper.Checked))
            {
                string json = "";
                string Crewjson = "";
                string SuperJson = "";

                //if (chkCustomer.Checked && !chkCrew.Checked) {
                //    json = JsonConvert.SerializeObject(eCoord);
                //    Crewjson = JsonConvert.SerializeObject("");
                //}
                //else if (!chkCustomer.Checked && chkCrew.Checked)
                //{

                //    json = JsonConvert.SerializeObject("");
                //    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                //}
                //else {
                //    json = JsonConvert.SerializeObject(eCoord);
                //    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                //}
                if (!chkCustomer.Checked && chkCrew.Checked && chkSuper.Checked)
                {
                    json = JsonConvert.SerializeObject("");
                    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                    SuperJson = JsonConvert.SerializeObject(eCoord);
                }
                else if (!chkCustomer.Checked && !chkCrew.Checked && chkSuper.Checked) {
                    json = JsonConvert.SerializeObject("");
                    Crewjson = JsonConvert.SerializeObject("");
                    SuperJson = JsonConvert.SerializeObject(eCoord);
                }
                else if (chkCustomer.Checked && !chkCrew.Checked && !chkSuper.Checked) {
                    json = JsonConvert.SerializeObject(eCoord);
                    Crewjson = JsonConvert.SerializeObject("");
                    SuperJson = JsonConvert.SerializeObject("");
                }
                else if (chkCustomer.Checked && chkCrew.Checked && !chkSuper.Checked)
                {
                    json = JsonConvert.SerializeObject(eCoord);
                    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                    SuperJson = JsonConvert.SerializeObject("");
                }
                else if (chkCustomer.Checked && !chkCrew.Checked && chkSuper.Checked)
                {
                    json = JsonConvert.SerializeObject(eCoord);
                    Crewjson = JsonConvert.SerializeObject("");
                    SuperJson = JsonConvert.SerializeObject(eCoord);
                }
                else if (!chkCustomer.Checked && chkCrew.Checked && !chkSuper.Checked)
                {
                    json = JsonConvert.SerializeObject("");
                    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                    SuperJson = JsonConvert.SerializeObject("");
                }
                else
                {
                    json = JsonConvert.SerializeObject(eCoord);
                    Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                    SuperJson = JsonConvert.SerializeObject(eCoord);
                }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "loadMarkerFromAddress", "loadMarkerFromAddress('" + json + "','" + Crewjson + "','" + SuperJson + "');", true);
            }
            else
            {
                double DefaultrCustomerLat = 33.4122784,
                       DefaultrCustomerlong = -112.0836409;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "DefaultrCustomerinitializeMap", "DefaultrCustomerinitializeMap('" + DefaultrCustomerLat + "','" + DefaultrCustomerlong + "');", true);
            }

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    private void ScheduleCrewList(csCrewLocation i, string start, string end)
    {
        try
        {
            DateTime dateCrw = DateTime.Now;

            string startCrw = "";
            string endCrw = "";

            startCrw = dateCrw.ToShortDateString();
            endCrw = dateCrw.AddDays(1).ToShortDateString();

            if (startCrw == start && endCrw == end)
            {
                

                DataClassesDataContext _db = new DataClassesDataContext();

                Crew_Detail obj = new Crew_Detail();
                string strEmployee_name = i.employee_name;

                string[] crAry = strEmployee_name.Split(',').Select(p => p.Trim()).ToArray();
                //make crew list for same customer
                var crewList = _db.Crew_Details.Where(c => crAry.Contains(c.full_name.Trim())).ToList();

                foreach (var cl in crewList)
                {
                    obj = new Crew_Detail();
                    int nCrew_id = cl.crew_id;

                    string s = " select crl.CrewId,  crl.Latitude AS CrewLatitude, crl.Longitude AS CrewLongitude from " +
                          " Crew_Location AS crl " +
                          " where  crl.CrewLocationId =(SELECT MAX(CrewLocationId) AS Expr1 FROM Crew_Location where Crew_Location.CrewId='" + nCrew_id + "')" +
                          " AND (crl.CreatedDate>= '" + start + "' and crl.CreatedDate< '" + end + "')";

                    csCrewLocation objCrl = _db.ExecuteQuery<csCrewLocation>(s, string.Empty).FirstOrDefault();
                    if (objCrl != null)
                    {

                        //check same crew
                        objCrl.CfullName = cl.full_name.Replace("Crew", "").Trim();
                        var match = eCoordForCrew.Where(stringToCheck => stringToCheck.CfullName.Contains(cl.full_name.Trim())).FirstOrDefault();
                        if (match == null)
                        {
                            //add the crews for today who has schdule with customer
                            eCoordForCrew.Add(objCrl);
                        }
                    }

                }

            }
            else {
               // chkCrew.Checked = false;
            }

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click"); 
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        string start = dtScheduleDate.ToShortDateString();
        string end = dtScheduleDate.AddDays(1).ToShortDateString();

        CustomerLocation(start, end);

    }

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        dtScheduleDate = dtScheduleDate.AddDays(-1);

        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void btnToday_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnToday.ID, btnToday.GetType().Name, "Click"); 
        DateTime dtScheduleDate = DateTime.Now;


        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        DateTime dtScheduleDate = DateTime.Now;
        if (txtScheduleDate.Text != "")
        {
            try
            {
                dtScheduleDate = Convert.ToDateTime(txtScheduleDate.Text);
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Schedule Date is required");
                return;
            }
        }

        dtScheduleDate = dtScheduleDate.AddDays(1);

        txtScheduleDate.Text = dtScheduleDate.ToShortDateString();

        btnView_Click(sender, e);

    }

    protected void chkCustomer_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCustomer.ID, chkCustomer.GetType().Name, "CheckedChanged"); 
        btnView_Click(sender, e);
    }

    protected void chkCrew_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCrew.ID, chkCrew.GetType().Name, "CheckedChanged"); 
        btnView_Click(sender, e);
    }
    protected void chkSuperintendent_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkSuper.ID, chkSuper.GetType().Name, "CheckedChanged"); 
        btnView_Click(sender, e);
    }
}