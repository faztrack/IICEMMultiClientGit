using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.IO;

using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Data.SqlClient;
using System.Reflection;
using System.Net.Mail;
using System.Text.RegularExpressions;



using System.Threading;


using TheArtOfDev.HtmlRenderer.WinForms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Plivo;
using System.Net;
/// <summary>
/// Summary description for csCommonUtility
/// </summary>
public class csCommonUtility
{
	public csCommonUtility()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    private static string SHORTING = "ASC";

    public static string FormatPhoneNo(string sNumber)
    {


        if (sNumber.Trim().Length == 0)
            return sNumber;

        string sReturn = "";

        try
        {

            sReturn = sNumber.Trim().Replace("-", "");

            sReturn = sReturn.Insert(3, "-");

            sReturn = sReturn.Insert(7, "-");
        }
        catch
        {
            sReturn = sNumber;
        }

        return sReturn;

    }
    public static void GridItemSorting(object sender, GridViewSortEventArgs e, DataTable table)
    {
        GridView grd = (GridView)sender;

        string strShort = e.SortExpression + " " + SHORTING;

        DataView dv = table.DefaultView;
        dv.Sort = strShort;
        grd.DataSource = dv.Table;
        grd.DataBind();
        if (SHORTING == "ASC")
            SHORTING = SHORTING.Replace("ASC", "DESC");
        else
            SHORTING = SHORTING.Replace("DESC", "ASC");
    }

    private static string[] DayArray = { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
    public static string GetSystemMessage(string str)
    {
        return "<span style='color:blue;'><b>System MSG: </b></span><span style='color:green;'>" + str + "</span>";

    }
    public static string GetSystemErrorMessage(string str)
    {
        return "<span style='color:blue;'><b>System MSG: </b></span><span style='color:red;'>" + str + "</span>";
    }

    public static string GetSystemRequiredMessage(string str)
    {
        return "<span style='color:blue;'><b>System MSG: </b></span><br/><span style='color:red;'>" + str + "</span>";
    }
    public static string GetSystemRequiredMessage2(string str)
    {
        return "<span style='color:red;'>" + str + "</span>";
    }

    public static int GetDateDifference(string StartDayName, string EndDayName)
    {

        int nDif = Array.IndexOf(DayArray, EndDayName) - Array.IndexOf(DayArray, StartDayName);
        if (nDif <= 0) nDif += 7;

        return nDif;
    }
    private static int GetDateDifference_Neg(string StartDayName, string EndDayName)
    {

        int nDif = Array.IndexOf(DayArray, EndDayName) - Array.IndexOf(DayArray, StartDayName);


        return nDif;
    }
    public static DateTime GetDateByDayName_Next(string DayName)
    {
        DateTime dToday = DateTime.Today;
        if (dToday.ToString("dddd") != DayName)
        {
            int nDif = GetDateDifference_Neg(dToday.ToString("dddd"), DayName);
            if (nDif <= 0) nDif += 7;
            dToday = dToday.AddDays(nDif);
        }

        return dToday;
    }

    public static DateTime GetDateByDayName_Prev(string DayName)
    {
        DateTime dToday = DateTime.Today;
        if (dToday.ToString("dddd") != DayName)
        {
            dToday = dToday.AddDays(GetDateDifference_Neg(dToday.ToString("dddd"), DayName));
        }

        return dToday;
    }

    public static DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
    {
        DataTable dtReturn = new DataTable();

        // column names
        PropertyInfo[] oProps = null;

        if (varlist == null) return dtReturn;

        foreach (T rec in varlist)
        {
            // Use reflection to get property names, to create table, Only first time, others will follow
            if (oProps == null)
            {
                oProps = ((System.Type)rec.GetType()).GetProperties();
                foreach (PropertyInfo pi in oProps)
                {
                    System.Type colType = pi.PropertyType;

                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                    == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }

                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                }
            }

            DataRow dr = dtReturn.NewRow();

            foreach (PropertyInfo pi in oProps)
            {
                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                (rec, null);
            }

            dtReturn.Rows.Add(dr);
        }
        return dtReturn;
    }

    public static void WriteLog(string msg)
    {
        try
        {

            string currentPath = ConfigurationManager.AppSettings["UploadDir"];//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(currentPath + "\\Log\\"))
                Directory.CreateDirectory(currentPath + "\\Log\\");
            currentPath = currentPath + "\\Log\\" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            msg = DateTime.Now.ToString() + " --> " + msg;
            FileStream fs = new FileStream(currentPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(msg);
            sw.Close();
            fs.Close();

        }
        catch (Exception ex)
        {
            // string ss = ex.Message;
        }
    }


    public static DataTable GetDataTable(string strQ)
    {
        DataTable table = new DataTable();
        SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CRMDBConnectionString"].ConnectionString);
        try
        {
            SqlCommand cmd = new SqlCommand(strQ, con);
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            table = ds.Tables[0];
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

            con.Close();
            con.Dispose();
        }
        return table;
    }

    public static company_profile GetCompanyProfile()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        company_profile objCom = _db.company_profiles.Single(c => c.client_id == 1);

        return objCom;
    }

    public static customer_estimate GetCustomerEstimateInfo(int nCusId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customer_estimate cus_est = new customer_estimate();

        if (_db.customer_estimates.Any(ce => ce.customer_id == nCusId && ce.client_id == 1 && ce.estimate_id == nEstId))
        {
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == nCusId && ce.client_id == 1 && ce.estimate_id == nEstId);
        }
        return cus_est;
    }

    public static void SendMail(string From, string To, string Cc, string Bcc, string Subject, string Body)
    {

        MailMessage msg = new MailMessage();
        msg.From = new MailAddress(From);
        if (Cc.Length > 0)
            msg.CC.Add(Cc);
        if (Bcc.Length > 0)
            msg.Bcc.Add(Bcc);
        msg.To.Add(To);
        msg.Subject = Subject;
        msg.IsBodyHtml = true;
        msg.Body = Body;
        msg.Priority = MailPriority.High;


        SendByLocalhost(msg);

    }

    public static void SendByLocalhost(MailMessage msg)
    {

        try
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            string strSendEmail = System.Configuration.ConfigurationManager.AppSettings["SendEmail"];
            if (strSendEmail == "Yes")
            {
                smtp.Send(msg);
            }

        }
        catch
        {

        }

    }

    public static string ExtractNumber(string original) { return new string(original.Where(c => Char.IsDigit(c)).ToArray()); }

    public static string GetPhoneFormat(string str)
    {
        string NoFormat = string.Empty;
        string Phone = string.Empty;

        //NoFormat = str.Replace(" ", "").Replace( "-", "").Replace("(", "").Replace(")", "").Replace("_", "").Replace("+", "").Replace(".","");

        if (str.Trim().Length > 0)
        {
            NoFormat = ExtractNumber(str);
            if (NoFormat.Length > 9)
            {
                string countrycode = NoFormat.Substring(0, 3);
                string Areacode = NoFormat.Substring(3, 3);
                string number = NoFormat.Substring(6);
                Phone = "(" + countrycode + ") " + Areacode + "-" + number;
            }
            else
                Phone = NoFormat;
        }

        return Phone;

    }

    public class setDMUserData
    {
        public int CustomerId { get; set; }
        public string Role { get; set; }
    }

    public static bool ValidMultipleEmail(string email)
    {
        bool IsValid = true;
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        string[] strIds = email.Split(',');
        foreach (string strId in strIds)
        {
            Match match1 = regex.Match(strId.Trim());
            if (!match1.Success)
            {
                IsValid = false;
                break;
            }
        }
        return IsValid;
    }

    private static string sImageName = "";
    private static int nWidth;
    private static int nHeight;


    public static void CreateContactAddressImage(string Value, string sFilePath)
    {
        try
        {
            nWidth = 620;
            nHeight = 550;
            sImageName = sFilePath;
            //    Save_HTML_as_Image(Value);
            if (Value.Length == 0)
                Value = " ";
            System.Drawing.Image m_Bitmap = HtmlRender.RenderToImage(Value, new Size(nWidth, nHeight));
            m_Bitmap.Save(sImageName, ImageFormat.Png);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public static string GetProjectUrl()
    {
        string ProjectUrl = System.Configuration.ConfigurationManager.AppSettings["ProjectUrl"].ToString();

        return ProjectUrl;
    }


    public static string ReplaceTemplateParameters(string template, string body)
    {
        template = template.Replace("#MESSAGE BODY#", body);

        return template;
    }




    // FaztrackPagePermission
    public static void SetPagePermission(Page page, string[] controlClientID)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            userinfo objUser = (userinfo)HttpContext.Current.Session["oUser"];

            string PageName = page.AppRelativeVirtualPath;

            int slashpos = PageName.LastIndexOf('/');
            if (slashpos >= 0)
                PageName = PageName.Substring(slashpos + 1);


            if (_db.PagePermissions.Where(x => x.user_id == objUser.user_id && x.IsWrite == false && x.PageName.Trim().ToLower() == PageName.Trim().ToLower()).Any())
            {

                var allButtons = page.FindDescendants<Button>();
                var allLnkButtons = page.FindDescendants<LinkButton>();
                var allHyperLink = page.FindDescendants<HyperLink>();
                var allImageButtons = page.FindDescendants<ImageButton>();
                var allDropdowns = page.FindDescendants<DropDownList>();
                var allCheckBoxs = page.FindDescendants<CheckBox>();
                var allCheckBoxLists = page.FindDescendants<CheckBoxList>();
                var allRadioButtons = page.FindDescendants<RadioButtonList>();
                var allPanels = page.FindDescendants<Panel>();
                var allListBox = page.FindDescendants<ListBox>();
                var allUploadFiles = page.FindDescendants<FileUpload>();
                var alltreeViews = page.FindDescendants<TreeView>();



                if (controlClientID.Length != 0)
                {
                    foreach (var id in controlClientID)
                    {
                        var button = allButtons.FirstOrDefault(bt => bt.ID == id);
                        if (button != null)
                            button.Visible = false;

                        var lnkbutton = allLnkButtons.FirstOrDefault(bt => bt.ID == id);
                        if (lnkbutton != null)
                            lnkbutton.Visible = false;

                        var hyperLink = allHyperLink.FirstOrDefault(bt => bt.ID == id);
                        if (hyperLink != null)
                            hyperLink.Visible = false;

                        var imgButton = allImageButtons.FirstOrDefault(bt => bt.ID == id);
                        if (imgButton != null)
                            imgButton.Visible = false;

                        var dropdown = allDropdowns.FirstOrDefault(bt => bt.ID == id);
                        if (dropdown != null)
                            dropdown.Enabled = false;

                        var checkBox = allCheckBoxs.FirstOrDefault(bt => bt.ID == id);
                        if (checkBox != null)
                            checkBox.Enabled = false;

                        var checkBoxList = allCheckBoxLists.FirstOrDefault(bt => bt.ID == id);
                        if (checkBoxList != null)
                            checkBoxList.Enabled = false;

                        var radioButton = allRadioButtons.FirstOrDefault(bt => bt.ID == id);
                        if (radioButton != null)
                            radioButton.Enabled = false;

                        var panel = allPanels.FirstOrDefault(bt => bt.ID == id);
                        if (panel != null)
                            panel.Visible = false;

                        var listBox = allCheckBoxLists.FirstOrDefault(bt => bt.ID == id);
                        if (listBox != null)
                            listBox.Enabled = false;

                        var fileUpload = allUploadFiles.FirstOrDefault(bt => bt.ID == id);
                        if (fileUpload != null)
                            fileUpload.Enabled = false;

                        var treeView = alltreeViews.FirstOrDefault(bt => bt.ID == id);
                        if (treeView != null)
                            treeView.Enabled = false;

                    }
                }
                else
                {
                    foreach (var b in allButtons)
                    {
                        b.Visible = false;
                    }

                    foreach (var b in allLnkButtons)
                    {
                        b.Visible = false;
                    }
                    foreach (var b in allLnkButtons)
                    {
                        b.Visible = false;
                    }

                    foreach (var b in allImageButtons)
                    {
                        b.Visible = false;
                    }

                    foreach (var b in allDropdowns)
                    {
                        b.Enabled = false;
                    }

                    foreach (var b in allCheckBoxs)
                    {
                        b.Enabled = false;
                    }

                    foreach (var b in allCheckBoxLists)
                    {
                        b.Enabled = false;
                    }

                    foreach (var b in allRadioButtons)
                    {
                        b.Enabled = false;
                    }
                    foreach (var b in allPanels)
                    {
                        b.Visible = false;
                    }
                    foreach (var b in allListBox)
                    {
                        b.Enabled = false;
                    }
                    foreach (var b in allUploadFiles)
                    {
                        b.Enabled = false;
                    }
                    foreach (var b in alltreeViews)
                    {
                        b.Enabled = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }




    public static string GetDivisionName(string nClientId)
    {

        string Ssql = " SELECT distinct  STUFF((SELECT ', ' + CAST(division_name AS VARCHAR(100))[text()] " +
                     " FROM  division "+
                     " where status = 1 AND  Id in ( " + nClientId + " ) order by division_name " +
                     " FOR XML PATH(''), TYPE) " +
                     " .value('.', 'NVARCHAR(MAX)'),1,2,' ') division  " +
                     " FROM division t ";
        DataTable dt = csCommonUtility.GetDataTable(Ssql);
        string divisionName = "";
        if (dt.Rows.Count > 0)
        {
            try
            {
                divisionName = dt.Rows[0].Field<string>("division");
            }
            catch
            {
                divisionName = "";
            }
        }

        return divisionName;

    }




    public static void SetPagePermissionForGrid(Page page, string[] controlName)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            userinfo objUser = (userinfo)HttpContext.Current.Session["oUser"];

            string PageName = page.AppRelativeVirtualPath;

            int slashpos = PageName.LastIndexOf('/');
            if (slashpos >= 0)
                PageName = PageName.Substring(slashpos + 1);

            if (_db.PagePermissions.Where(x => x.user_id == objUser.user_id && x.IsWrite == false && x.PageName.Trim().ToLower() == PageName.Trim().ToLower()).Any())
            {

                var allButtons = page.FindDescendants<Button>();
                var allLnkButtons = page.FindDescendants<LinkButton>();
                var allImage = page.FindDescendants<System.Web.UI.WebControls.Image>();
                var allIFileUpload = page.FindDescendants<FileUpload>();
                var allCheckBoxs = page.FindDescendants<CheckBox>();
                var allradioButtonLists = page.FindDescendants<RadioButtonList>();
                var allDropdownLists = page.FindDescendants<DropDownList>();
                var allHyperLink = page.FindDescendants<HyperLink>();


                if (controlName.Length != 0)
                {
                    foreach (var name in controlName)
                    {
                        var buttonList = allButtons.Where(bt => bt.Text.ToLower() == name.ToLower());
                        if (buttonList.Any())
                        {
                            foreach (var b in buttonList.ToList())
                            {
                                b.Visible = false;
                            }
                        }

                        //for LinkButton Text  //LinkButton
                        var lnkbuttonList = allLnkButtons.Where(bt => bt.Text.ToLower() == name.ToLower());
                        if (lnkbuttonList.Any())
                        {
                            foreach (var b in lnkbuttonList.ToList())
                            {
                                b.Visible = false;
                            }
                        }
                        //for LinkButton ClintID   //for image LinkButton
                        var lnkbutton = allLnkButtons.Where(bt => bt.ClientID.ToLower().Contains(name.ToLower()));
                        if (lnkbutton.Any())
                        {
                            foreach (var b in lnkbutton.ToList())
                            {
                                b.Visible = false;
                            }
                        }




                        var imgBtn = allImage.Where(x => x.ClientID.ToLower().Contains(name.ToLower()));
                        if (imgBtn.Any())
                        {
                            foreach (var b in imgBtn.ToList())
                            {
                                b.Visible = false;
                            }
                        }

                        var fileUpload = allIFileUpload.Where(x => x.ClientID.ToLower().Contains(name.ToLower()));
                        if (fileUpload.Any())
                        {
                            foreach (var b in fileUpload.ToList())
                            {
                                b.Visible = false;
                            }
                        }

                        var checkbox = allCheckBoxs.Where(x => x.ClientID.ToLower().Contains(name.ToLower()));
                        if (checkbox.Any())
                        {
                            foreach (var b in checkbox.ToList())
                            {
                                b.Enabled = false;
                            }
                        }

                        var radioButton = allradioButtonLists.Where(x => x.ClientID.ToLower().Contains(name.ToLower()));
                        if (radioButton.Any())
                        {
                            foreach (var b in radioButton.ToList())
                            {
                                b.Enabled = false;
                            }
                        }

                        var dropdown = allDropdownLists.Where(x => x.ClientID.ToLower().Contains(name.ToLower()));
                        if (dropdown.Any())
                        {
                            foreach (var b in dropdown.ToList())
                            {
                                b.Enabled = false;
                            }
                        }

                        //for HyperLink Text  //HyperLink
                        var hyperlink = allHyperLink.Where(bt => bt.ClientID.ToLower().Contains(name.ToLower()));
                        if (hyperlink.Any())
                        {
                            foreach (var b in hyperlink.ToList())
                            {
                                b.Visible = false;
                            }
                        }

                    }
                }
                else
                {
                    foreach (var b in allButtons)
                    {
                        b.Visible = false;
                    }

                    foreach (var b in allLnkButtons)
                    {
                        b.Visible = false;
                    }

                    foreach (var b in allImage)
                    {
                        b.Visible = false;
                    }
                    foreach (var b in allIFileUpload)
                    {
                        b.Visible = false;
                    }
                    foreach (var b in allCheckBoxs)
                    {
                        b.Enabled = false;
                    }
                    foreach (var b in allDropdownLists)
                    {
                        b.Enabled = false;
                    }
                    foreach (var b in allHyperLink)
                    {
                        b.Visible = false;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

public class csIsSuccess
{
    public csIsSuccess()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string result { get; set; }
    public string value { get; set; }
    public int signup_status { get; set; }

}



