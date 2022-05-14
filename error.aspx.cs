using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sURL = "";
        try
        {
            if (!this.IsPostBack)
            {
                KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

                HttpBrowserCapabilities bCaps = Request.Browser;
                string strURL = Server.UrlDecode(Request.QueryString.Get("RequestedURL"));
                HttpException ex = (HttpException)Server.GetLastError();
                int httpCode = ex.GetHttpCode();

                string strError = "";
                if (httpCode != 404)
                {
                    strError = "Server Path: " + strURL + Environment.NewLine + Environment.NewLine +
                       "Borwser: " + bCaps.Browser + Environment.NewLine + Environment.NewLine +
                       "Error: " + Server.GetLastError().InnerException.Message + Environment.NewLine + Environment.NewLine +
                       "Source: " + Server.GetLastError().InnerException.Source + Environment.NewLine + Environment.NewLine +
                       "StackTrace: " + Server.GetLastError().InnerException.StackTrace;

                    string IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (IPAddress == null)
                        IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                    if (IPAddress.IndexOf("66.249") != 0 && Server.GetLastError().InnerException.Message.ToLower().IndexOf("invalid viewstate") == -1)
                    {
                        SendMailContent(strError + Environment.NewLine + IPAddress, strURL);
                    }

                    sURL = "error500.aspx";

                }
                else
                {
                    //string IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    //if (IPAddress == null)
                    //    IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                    //strError = "Server Path: " + strURL + Environment.NewLine + Environment.NewLine +
                    //    "Borwser: " + bCaps.Browser + Environment.NewLine + Environment.NewLine + "Page not found.";
                    //SendMailContent(strError + Environment.NewLine + IPAddress, strURL);
                    sURL = "error404.aspx";
                }

              //  lblError.Text = strError;

            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
            //throw ex;
        }
        finally
        {
            Server.ClearError();
        }

        Response.Redirect(sURL);
    }

    private void SendMailContent(string str, string strUrl)
    {

        try
        {
            csCommonUtility.SendMail("faztrackbd@gmail.com", System.Configuration.ConfigurationManager.AppSettings["ErrorTo"].Trim(), "", "", "Error On " + strUrl, str);
        }
        catch (Exception ex)
        {
            throw ex;
        }


        //System.Net.Mail.MailMessage msgMail = new System.Net.Mail.MailMessage("errors@faztrack.com",
        //    System.Configuration.ConfigurationManager.AppSettings["ErrorTo"].Trim() + "," +
        //    System.Configuration.ConfigurationManager.AppSettings["ErrorCC"].Trim(),
        //    "Error On " + strUrl, str);

        //try
        //{
        //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
        //    smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
        //    smtp.Send(msgMail);
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }
}