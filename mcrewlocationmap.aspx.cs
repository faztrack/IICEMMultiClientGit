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

public partial class mcrewlocationmap : System.Web.UI.Page
{
    List<csCrewLocation> eCoordForCrew = new List<csCrewLocation>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                DateTime date = DateTime.Now;
                string start = "";
                string end = "";
                start = date.ToShortDateString();
                end = date.AddDays(1).ToShortDateString();
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                //Crew_Detail objCD = _db.Crew_Details.Single(c => c.crew_id == objC.crew_id);
                //if (objCD.Image != null && objCD.Image != "")
                    ///ImgCrew.ImageUrl = "~/Uploads/CrewProfileImage/" + objCD.crew_id + "/" + objCD.Image;
                CustomerLocation(start, end, Convert.ToInt32(Request.QueryString.Get("cid")), objC.crew_id);
            }
            
        }

    }

    private void CustomerLocation(string start, string end, int nCustomerId, int nCrewId)
    {
        try

        {
           // lblResult.Text = "";
           
            DataClassesDataContext _db = new DataClassesDataContext();
            List<csCrewLocation> eCoord = new List<csCrewLocation>();
           // company_profile com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
          ////  lblOfficePhone.Text = com.phone;//"<a href=tel:'" + com.phone + "'"+ com.phone+ "</a>";
          //  hrfOfficePhone.HRef = "tel:"+com.phone;

            customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
           
           lblCrewName.Text =objCust.first_name1 + " " + objCust.last_name1;

            string address = objCust.address + ",+" + objCust.city + ",+" + objCust.state + ",+" + objCust.zip_code;
            hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            hypGoogleMap.Text =  objCust.address + " " + objCust.city + "," + objCust.state + " " + objCust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            hypAddress.Text= objCust.address + " " + objCust.city + "," + objCust.state + " " + objCust.zip_code;
            lblOfficePhone.Text = objCust.phone;//"<a href=tel:'" + com.phone + "'"+ com.phone+ "</a>";
            hrfOfficePhone.HRef = "tel:" + objCust.phone;


            string custListCurrentDate = " SELECT  DISTINCT TOP 1 c.address + ', ' + c.city + ', ' + c.state + ' ' + c.zip_code AS CustomerAddress, c.first_name1 + ' ' + c.last_name1 AS CfullName, c.customer_id, c.Latitude AS CustLatitude, " +
                                       "  c.Longitude AS CustLongitude, cd.full_name as employee_name, cd.crew_id  as CrewId " +
                                       " FROM  customers AS c INNER JOIN " +
                                       " CrewCustomerRoute AS cc ON cc.customer_id = c.customer_id " +
                                       " INNER JOIN   Crew_Details as cd on cd.crew_id = cc.CrewId " +
                                       " WHERE  cc.customer_id ="+ nCustomerId + " AND cc.CrewId="+ nCrewId + "  AND  (cc.CreatedDate >= '" + start + "' and cc.CreatedDate < '" + end + "')" ;
            List<csCrewLocation> item = _db.ExecuteQuery<csCrewLocation>(custListCurrentDate, string.Empty).ToList();

            if (item.Count == 0)
            {
               // lblResult.Text = csCommonUtility.GetSystemErrorMessage("No Data found");
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

                MapCrewInGoogleMap(i, start, end, latForCustomer, longForCustomer);

            }
            if (eCoord.Count > 0)
            {
                string json = "";
                string Crewjson = "";
                string SuperJson = "";
                json = JsonConvert.SerializeObject(eCoord);
                Crewjson = JsonConvert.SerializeObject(eCoordForCrew);
                SuperJson = JsonConvert.SerializeObject(eCoord);
                

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
            //lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    private void MapCrewInGoogleMap(csCrewLocation i, string start, string end, double latForCustomer, double longForCustomer)
    {
        try
        {
            DateTime dateCrw = DateTime.Now;

            string startCrw = "";
            string endCrw = "";
            double distance = 0.0;
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

                   
                    string s = " select crl.CrewId,  crl.Latitude AS CrewLatitude, crl.Longitude AS CrewLongitude " +
                               " from  Crew_Location AS crl" +
                               " where crl.CrewId ="+ nCrew_id+" AND  (crl.CreatedDate >= '" + start + "' and crl.CreatedDate < '" + end + "')" +
                               " order by crl.CrewLocationId desc ";

                    csCrewLocation objCrl = _db.ExecuteQuery<csCrewLocation>(s, string.Empty).FirstOrDefault();
                    if (objCrl != null)
                    {
                        if(objCrl.CrewLatitude != null  && objCrl.CrewLatitude != "" && objCrl.CrewLongitude != null && objCrl.CrewLongitude != "")
                        {
                            var sCoord = new GeoCoordinate(Convert.ToDouble(objCrl.CrewLatitude), Convert.ToDouble(objCrl.CrewLongitude));
                            var eCoord = new GeoCoordinate(latForCustomer, longForCustomer);
                            distance = sCoord.GetDistanceTo(eCoord);
                            lblDistanceInMile.Text = (distance * 0.000621371).ToString("0.00") + " miles";
                        }


                        //check same crew
                        objCrl.CfullName = cl.full_name.Replace("Crew", "").Trim();
                       // lblCrewName.Text= cl.full_name.Replace("Crew", "").Trim();

                        var match = eCoordForCrew.Where(stringToCheck => stringToCheck.CfullName.Contains(cl.full_name.Trim())).FirstOrDefault();
                        if (match == null)
                        {
                            //add the crews for today who has schdule with customer
                            eCoordForCrew.Add(objCrl);
                        }
                    }

                }

            }
           

        }
        catch (Exception ex)
        {
           // lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }




}