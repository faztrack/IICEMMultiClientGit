using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class undolabor_hour_list : System.Web.UI.Page
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
            //if (Page.User.IsInRole("timetrack004") == false)
            //{
            //    // No Permission Page.
            //    Response.Redirect("nopermission.aspx");
            //}
            BindCrew();
            GetUndoLaberTracking(0);
        }
    }

    private void BindCrew()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS crew_name,crew_id from Crew_Details WHERE is_active = 1 order by crew_name asc";
        List<CrewDe> mList = _db.ExecuteQuery<CrewDe>(strQ, string.Empty).ToList();
        ddlInstaller.DataSource = mList;
        ddlInstaller.DataTextField = "crew_name";
        ddlInstaller.DataValueField = "crew_id";
        ddlInstaller.DataBind();
        ddlInstaller.Items.Insert(0, "All");
    }
    private void GetUndoLaberTracking(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strCondition = string.Empty;
        if (ddlInstaller.SelectedItem.Text != "All")
        {
            strCondition = " where UserID = " + Convert.ToInt32(ddlInstaller.SelectedValue);

        }

     string strQ = "select [GPSTrackID],[StartPlace] ,[MakeStopPlace],[EndPlace],[Distance],[Time],[UserID],[CustomerName],[section_id], " +
                     " [SectionName],[StartTime] ,[EndTime],[customer_id],[Estimate_id],[labor_date],DeviceName,ID,deleteDate,deleteBy from DeleteGPSTracking " + strCondition + " order by deleteDate desc ";
        IEnumerable<CrewTrack> clist = _db.ExecuteQuery<CrewTrack>(strQ, string.Empty);

        grdLaberTrack.DataSource = csCommonUtility.LINQToDataTable(clist);
        grdLaberTrack.PageIndex = nPageNo;
        grdLaberTrack.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        grdLaberTrack.DataKeyNames = new string[] { "GPSTrackID", "SectionName", "EndPlace", "UserID", "StartPlace", "StartTime", "EndTime", "ID" };
        grdLaberTrack.DataBind();

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
                int nID = Convert.ToInt32(grdLaberTrack.DataKeys[e.Row.RowIndex].Values[7].ToString());

                Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");

                Label lblLunch = (Label)e.Row.FindControl("lblLunch");
                lblLunch.Text = ".50";


                Crew_Detail objCrew = new Crew_Detail();
                objCrew = _db.Crew_Details.SingleOrDefault(c => c.crew_id == nUserId);
                if (objCrew != null)
                    e.Row.Cells[1].Text = objCrew.full_name;



                if (Convert.ToDateTime(EndTime).Year != 2000)
                {
                
                   
                    //EndTime = EndTime.AddMinutes(30);
                    TimeSpan span = EndTime.Subtract(StartTime);
                    float totalHours = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                    float totalHours1 = (span.Days * 24 * 60 + span.Hours * 60 + span.Minutes);
                    totalHours = totalHours / 60;
                    if (totalHours > 0)
                    {
                        if (totalHours1 % 60 == 0)
                        {
                            lblTotalHours.Text = Math.Round(totalHours, 2).ToString() + ".00";
                        }
                        else
                            lblTotalHours.Text = Math.Round(totalHours, 2).ToString();
                    }
                    else
                    {
                        if (Convert.ToInt32(Math.Round(totalHours, 2)) != 0)
                        {
                            lblTotalHours.Text = Math.Round(totalHours, 2).ToString();
                            lblTotalHours.ForeColor = Color.Red;
                        }
                        else
                            lblTotalHours.Text = "";
                    }
                }
                else
                {
                    lblTotalHours.Text = "";
                }
              if (StartPlace.Contains("USA") || StartPlace.Contains("usa"))
                {
                    e.Row.Cells[2].Text = StartPlace.Remove(StartPlace.Length - 5, 5);
                }
                else
                {
                    if (StartPlace == "0" || StartPlace == "" || StartPlace == null)
                    {
                        e.Row.Cells[2].Text = "";
                    }
                    else
                    {
                        e.Row.Cells[2].Text = StartPlace;
                    }

                }




                if (EndPlace == "0")
                {
                    e.Row.Cells[3].Text = "";
                }
                else
                {
                    if (EndPlace.Contains("USA") || EndPlace.Contains("usa"))
                    {
                        e.Row.Cells[3].Text = EndPlace.Remove(EndPlace.Length - 5, 5);
                    }
                    else
                    {
                        e.Row.Cells[3].Text = EndPlace;
                    }
                }





                ImageButton imgUndo = (ImageButton)e.Row.FindControl("imgUndo");
                imgUndo.OnClientClick = "return confirm('Are you sure you want to undo this time entry?');";
                imgUndo.CommandArgument = nID.ToString();


            }
            catch (Exception ex)
            {

            }



        }
    }

    protected void UndoFile(object sender, EventArgs e)
    {
        try
        {
            userinfo obj = (userinfo)Session["oUser"];
            string strQ = "";
            ImageButton imgUndo = (ImageButton)sender;
            int nId = Convert.ToInt32(imgUndo.CommandArgument);
            int nGPSTrackId = 0;
            DataClassesDataContext _db = new DataClassesDataContext();
            GPSTracking objGPS = new GPSTracking();
            DeleteGPSTracking objD = _db.DeleteGPSTrackings.SingleOrDefault(gps=>gps.ID==nId);

            nGPSTrackId = Convert.ToInt32(objD.GPSTrackID);

            var nList = (from dgps in _db.DeleteGPSTrackingDetails where dgps.GPSTrackID == objD.GPSTrackID select dgps).ToList();

           


            if (objD != null)
            {
                objGPS.StartPlace = objD.StartPlace;
                objGPS.StartLatitude = objD.StartLatitude;
                objGPS.StartLogitude = objD.StartLogitude;
                objGPS.EndLatitude = objD.EndLatitude;
                objGPS.EndLogitude = objD.EndLogitude;
                objGPS.MakeStopPlace = objD.MakeStopPlace;
                objGPS.EndPlace = objD.EndPlace;
                objGPS.Distance = objD.Distance;
                objGPS.Time = objD.Time;
                objGPS.CreatedDate = objD.CreatedDate;
                objGPS.UserID = objD.UserID;
                objGPS.CustomerName = objD.CustomerName;
                objGPS.section_id = objD.section_id;
                objGPS.SectionName = objD.SectionName;
                objGPS.StartTime =objD.StartTime;
                objGPS.EndTime = objD.EndTime;
                objGPS.customer_id = objD.customer_id;
                objGPS.Estimate_id = objD.Estimate_id;
                objGPS.labor_date = objD.labor_date;
                objGPS.deviceName = objD.deviceName;
                objGPS.IsCrew = objD.IsCrew;
                _db.GPSTrackings.InsertOnSubmit(objGPS);
                _db.SubmitChanges();

                strQ = "delete from DeleteGPSTracking where GPSTrackID=" + objD.GPSTrackID;
                _db.ExecuteCommand(strQ, string.Empty);

               


            }

            GPSTrackingDetail objDGPS = null;

            foreach (var li in nList)
            {
                objDGPS = new GPSTrackingDetail();

                objDGPS.GPSTrackID = objGPS.GPSTrackID;
                objDGPS.Latitude = li.Latitude;
                objDGPS.Longitude = li.Longitude;
                objDGPS.InputType = li.InputType;
                objDGPS.CreateDate = li.CreateDate;

                _db.GPSTrackingDetails.InsertOnSubmit(objDGPS);
                _db.SubmitChanges();
            }


            strQ = "delete from DeleteGPSTrackingDetails where GPSTrackID=" + nGPSTrackId;
            _db.ExecuteCommand(strQ, string.Empty);

            Response.Redirect("labor_hour_list.aspx", false);
               
           
           
        }
        catch (Exception ex)
        {
        }
    }


    protected void grdLaberTrack_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GetUndoLaberTracking(e.NewPageIndex);
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetUndoLaberTracking(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetUndoLaberTracking(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetUndoLaberTracking(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        lblResult.Text = "";
        ddlInstaller.SelectedIndex = -1;
        GetUndoLaberTracking(0);


    }
    protected void ddlInstaller_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
       GetUndoLaberTracking(0);
    }
}