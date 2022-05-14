using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gmap2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = " SELECT  a.Zip, ZipMapAZ.City+' ('+ CONVERT(VARCHAR(50),a.SalesNum)+')'  AS SalesDescription, Latitude, Longitude FROM ZipMapAZ " +
                " INNER JOIN (SELECT count(id) AS SalesNum,Zip from ZipCodeBook GROUP BY Zip) a ON a.Zip =  ZipMapAZ.Zip";

            List<csZipMap> list = _db.ExecuteQuery<csZipMap>(strQ, string.Empty).ToList();
            DataTable dt = csCommonUtility.LINQToDataTable(list);
            //DataTable dt = this.GetData(str);
            BuildScript(dt);
        }

    }

    private void BuildScript(DataTable tbl)
    {

        string Locations = "";
        string msg = "";
        string aZip = "";

        foreach (DataRow r in tbl.Rows)
        {
            // bypass empty rows	 	
            if (r["Latitude"].ToString().Trim().Length == 0)
                continue;

            string Latitude = r["Latitude"].ToString();
            string Longitude = r["Longitude"].ToString();
            string Description = r["SalesDescription"].ToString();
            string Zip = r["Zip"].ToString().Trim();
            Description = Zip + " " + Description;

            // create a line of JavaScript for marker on map for this record	               
            Locations += Environment.NewLine + "ltlng.push(new google.maps.LatLng(" + Latitude + "," + Longitude + "));";
            msg += Environment.NewLine + "ltmsg.push('" + Description.Trim() + "');";
            aZip += Environment.NewLine + "ltZip.push('" + Zip + "');";
        }

        js.Text = @"<script type ='text/javascript'>
                        var map; var infowindow;
                        function InitializeMap() {
                            var latlng = new google.maps.LatLng(33.7712,-111.3877);
                            var myOptions =
                            {
                                zoom: 8,
                                center: latlng,
                                mapTypeId: google.maps.MapTypeId.ROADMAP
                            };
                            map = new google.maps.Map(document.getElementById('map'), myOptions);
                        }


                        function markicons() {

                            InitializeMap();

                            var ltlng = [];				
                             " + Locations + @"
							var ltmsg = [];				
                             " + msg + @"
							var ltZip = [];				
                             " + aZip + @"

       
	                     map.setCenter(ltlng[0]);
                            for (var i = 0; i <= ltlng.length; i++) {
								
                                marker = new google.maps.Marker({
                                    map: map,
                                    position: ltlng[i],
									title: ltmsg[i]
                                });

                                (function (i, marker) {

                                    google.maps.event.addListener(marker, 'click', function () {

                                        if (!infowindow) {
                                            infowindow = new google.maps.InfoWindow();
                                        }
										 
                                        infowindow.setContent(ltmsg[i]);

                                        infowindow.open(map, marker);
										load(ltZip[i]);
                                    });

                                })(i, marker);

                            }

                        }

                        window.onload = markicons; 

                    </script>
                    ";

    }

    protected void btnLoad_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnLoad.ID, btnLoad.GetType().Name, "Click"); 
        txtZip.Text = "";
        txtZipKey.Text = "";
        string sZip = hdnZip.Text.ToString().Trim();
        BindGrid(sZip, 0);
    }

    private void btnSearch_Click(object sender, System.EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        hdnZip.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string nZip = txtZip.Text.ToString().Trim();
        string strQ = " SELECT  a.Zip, ZipMapAZ.City+' ('+ CONVERT(VARCHAR(50),a.SalesNum)+')'  AS SalesDescription, Latitude, Longitude FROM ZipMapAZ " +
            " INNER JOIN (SELECT count(id) AS SalesNum,Zip from ZipCodeBook GROUP BY Zip) a ON a.Zip =  ZipMapAZ.Zip where a.Zip='" + nZip + "'";
        List<csZipMap> list = _db.ExecuteQuery<csZipMap>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(list);
        BuildScript(dt);

        BindGrid(nZip, 0);
        txtZipKey.Text = nZip;

    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnReset.ID, btnReset.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  a.Zip, ZipMapAZ.City+' ('+ CONVERT(VARCHAR(50),a.SalesNum)+')'  AS SalesDescription, Latitude, Longitude FROM ZipMapAZ " +
                    " INNER JOIN (SELECT count(id) AS SalesNum,Zip from ZipCodeBook GROUP BY Zip) a ON a.Zip =  ZipMapAZ.Zip";

        List<csZipMap> list = _db.ExecuteQuery<csZipMap>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(list);
        BuildScript(dt);
        grdCusDetails.DataSource = null;
        grdCusDetails.DataBind();

        txtZip.Text = "";
        txtZipKey.Text = "";
        hdnZip.Text = "";
    }

    protected void BindGrid(string sZip, int pageindex)
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            //string strQ = "SELECT FirstName +' '+LastName As Customer, Address +' ' +City +' '+State+' '+ Zip As fullAddress, SalesRep FROM ZipCodeBook where Zip='" + sZip + "' order by FirstName";
            //List<ZipCodeBook> list = _db.ExecuteQuery<ZipCodeBook>(strQ, string.Empty).ToList();

            var items = from z in _db.ZipCodeBooks
                        where z.Zip == sZip
                        orderby z.FirstName
                        select new
                        {
                            Customer = z.FirstName + ' ' + z.LastName,
                            fullAddress = z.Address + ' ' + z.City + ' ' + z.State + ' ' + z.Zip,
                            SalesRep = z.SalesRep
                        };

            grdCusDetails.DataSource = items.ToList();
            grdCusDetails.DataBind();

        }
        catch (Exception ex)
        {

        }
    }

    private void grdCusDetails_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCusDetails.ID, grdCusDetails.GetType().Name, "PageIndexChanged"); 
        //grdCusDetails.CurrentPageIndex=e.NewPageIndex;

        string sZip = txtZipKey.Text.ToString().Trim();

        if (hdnZip.Text != "")
            sZip = hdnZip.Text.ToString().Trim();


        BindGrid(sZip, e.NewPageIndex);
    }
}