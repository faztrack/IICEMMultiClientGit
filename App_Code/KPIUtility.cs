using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for KPIUtility
/// </summary>
public class KPIUtility
{
    public KPIUtility()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void PageLoad(string PageName)
    {


        DateTime dtNow = DateTime.Now;

        int slashpos = PageName.LastIndexOf('/');
        if (slashpos >= 0)
            PageName = PageName.Substring(slashpos + 1);


        DataClassesDataContext _db = new DataClassesDataContext();


         if (HttpContext.Current.Session["PageLoadId"] != null)
        {


            KPI kpiPrev = _db.KPIs.SingleOrDefault(k => k.Id == Convert.ToInt32(HttpContext.Current.Session["PageLoadId"]) && k.EventName == "PageLoad");
            if (kpiPrev != null)
            {

                TimeSpan ts = dtNow.Subtract(Convert.ToDateTime(kpiPrev.EventTime));
                kpiPrev.SpentTimeInSec = ts.TotalSeconds;
                _db.SubmitChanges();
            }


        }



        KPI kpi = new KPI();
        kpi.PageName = PageName;
        kpi.ControlName = "";
        kpi.ControlType = "";
        kpi.EventName = "PageLoad";
        kpi.EventTime = dtNow;

        try
        {
            HttpBrowserCapabilities bCaps = HttpContext.Current.Request.Browser;
            kpi.Browser = bCaps.Browser;
            kpi.BrowserVer = bCaps.Version.ToString();
        }
        catch { }

        try
        {
            kpi.UserName = HttpContext.Current.User.Identity.Name;
        }
        catch
        {

            kpi.UserName = "";

        }

        _db.KPIs.InsertOnSubmit(kpi);
        
        _db.SubmitChanges();



       


        HttpContext.Current.Session["PageLoadId"] = kpi.Id;


    }

    public static int PageLoadPopup(string PageName)
    {


        DateTime dtNow = DateTime.Now;

        int slashpos = PageName.LastIndexOf('/');
        if (slashpos >= 0)
            PageName = PageName.Substring(slashpos + 1);


        DataClassesDataContext _db = new DataClassesDataContext();

        KPI kpi = new KPI();
        kpi.PageName = PageName;
        kpi.ControlName = "";
        kpi.ControlType = "";
        kpi.EventName = "Popup";
        kpi.EventTime = dtNow;

        try
        {
            HttpBrowserCapabilities bCaps = HttpContext.Current.Request.Browser;
            kpi.Browser = bCaps.Browser;
            kpi.BrowserVer = bCaps.Version.ToString();
        }
        catch { }

        try
        {
            kpi.UserName = HttpContext.Current.User.Identity.Name;
        }
        catch
        {

            kpi.UserName = "";

        }

        _db.KPIs.InsertOnSubmit(kpi);
        _db.SubmitChanges();





        return kpi.Id;


    }


    public static void PopupClose(int Id)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DateTime dtNow = DateTime.Now;

        KPI kpiPrev = _db.KPIs.SingleOrDefault(k => k.Id == Id && k.EventName == "Popup");
        if (kpiPrev != null)
        {

            TimeSpan ts = dtNow.Subtract(Convert.ToDateTime(kpiPrev.EventTime));
            kpiPrev.SpentTimeInSec = ts.TotalSeconds;
            _db.SubmitChanges();
        }




    }

    public static void SaveEvent(string PageName, string ControlName, string ControlType, string EventName)
    {
        int slashpos = PageName.LastIndexOf('/');
        if (slashpos >= 0)
            PageName = PageName.Substring(slashpos + 1);

        DataClassesDataContext _db = new DataClassesDataContext();

        KPI kpi = new KPI();
        kpi.PageName = PageName;
        kpi.ControlName = ControlName;
        kpi.ControlType = ControlType;
        kpi.EventName = EventName;
        kpi.EventTime = DateTime.Now;
        kpi.SpentTimeInSec = 0;
        try
        {
            kpi.UserName = HttpContext.Current.User.Identity.Name;
        }
        catch
        {

            kpi.UserName = "";

        }
        try
        {
            HttpBrowserCapabilities bCaps = HttpContext.Current.Request.Browser;
            kpi.Browser = bCaps.Browser;
            kpi.BrowserVer = bCaps.Version.ToString();
        }
        catch { }
        _db.KPIs.InsertOnSubmit(kpi);
        _db.SubmitChanges();



    }

    private void postIt(object sender, EventArgs e)
    {
        string type = sender.GetType().ToString();
        switch (type)
        {
            case "System.Web.UI.WebControls.TextBox":
                TextBox tb = (TextBox)sender;

                break;
            case "System.Web.UI.WebControls.RadioButton":
                RadioButton rb = (RadioButton)sender;
                break;
            case "System.Web.UI.WebControls.RadioButtonList":
                RadioButtonList rbl = (RadioButtonList)sender;
                break;
            case "System.Web.UI.WebControls.CheckBox":
                CheckBox cb = (CheckBox)sender;
                break;
            case "System.Web.UI.WebControls.CheckBoxList":
                CheckBoxList cbl = (CheckBoxList)sender;
                break;
            case "System.Web.UI.WebControls.DropDownList":
                DropDownList ddl = (DropDownList)sender;
                break;
            case "System.Web.UI.WebControls.ListBox":
                ListBox lb = (ListBox)sender;
                break;
            default:
                //Label1.Text = type;
                break;
        }
    }
}