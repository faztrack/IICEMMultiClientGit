using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Device.Location;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public partial class joblocation : System.Web.UI.Page
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }

            if (Session["oCrew"] != null)
            {
               CrewMapData();

            }

        }


    }
    public void CrewMapData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        Crew_Detail crwDetailsobj = new Crew_Detail();
        csCrewLocation objLowestDistanceCustomer = new csCrewLocation();
        if (System.Web.HttpContext.Current.Session["oCrew"] != null)
        {
            crwDetailsobj = (Crew_Detail)System.Web.HttpContext.Current.Session["oCrew"];
        }

        DateTime date = DateTime.Now;
        List<csCrewLocation> eCoord = new List<csCrewLocation>();
        string start = date.ToShortDateString();
        string end = date.AddDays(1).ToShortDateString();
        List<csCrewLocation> NewitemList = new List<csCrewLocation>();
        Crew_Location objCLoaction = new Crew_Location();
        string sql = " SELECT Distinct sc.employee_name, c.address+', '+c.city+', '+c.state+' '+c.zip_code As CustomerAddress," +
                    " c.first_name1+' '+c.last_name1 AS CfullName, sc.event_start, sc.event_end, " +
                    " c.customer_id, c.Latitude AS CustLatitude, c.Longitude AS CustLongitude  FROM ScheduleCalendar AS sc" +
                    " INNER JOIN customers AS c ON sc.customer_id = c.customer_id" +
                    " AND ( (event_start<= '" + start + "' and event_end>='" + start + "') or(event_start>= '" + start + "'  and  event_start<'" + end + "') )" +
                    " AND (sc.type_id IN (1, 11)) AND (sc.IsEstimateActive = 1) AND (c.is_active = 1) AND sc.employee_name like '%" + crwDetailsobj.full_name.Trim() + "%'";

        List<csCrewLocation> item = _db.ExecuteQuery<csCrewLocation>(sql, string.Empty).ToList();
        if (item.Count > 0)
        {
            Regex regex_newline = new Regex("(\r\n|\r|\n|)");
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
                    i.CustomerAddress = regex_newline.Replace(i.CustomerAddress, "");
                    i.CfullName = i.CfullName.Replace("\"", "");
                    eCoord.Add(i);

                }

                // }


            }
            string jsonCustomer = "";

            jsonCustomer = JsonConvert.SerializeObject(eCoord);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "loadMarkerForCrew", "jobLocationMap('" + jsonCustomer + "');", true);
          
        }
        else {
            double DefaultrCustomerLat = 33.4122784,
                       DefaultrCustomerlong = -112.0836409;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "DefaultCustomerinitializeMap", "DefaultCustomerinitializeMap('" + DefaultrCustomerLat + "','" + DefaultrCustomerlong + "');", true);
        }


    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }

}