using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class replayemail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            #region Original send eamil
            if (Request.QueryString.Get("custId") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
                hdnCustomerId.Value = ncid.ToString();
            }
            string sMessId = Request.QueryString.Get("MessId");

            imgCencel.Attributes.Add("onClick", "CloseWindow();");

        }
            #endregion

    }




    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        HttpFileCollection fileCollection = Request.Files;
        for (int i = 0; i < fileCollection.Count; i++)
        {
            string DestinationPath = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Temp//");

            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            HttpPostedFile uploadfile = fileCollection[i];
            // string fileName = Path.GetFileNameWithoutExtension(uploadfile.FileName);
            string fileName = "";
            string fileExt = Path.GetExtension(uploadfile.FileName);
            if (uploadfile.ContentLength > 0)
            {
                fileName = uploadfile.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                uploadfile.SaveAs(DestinationPath + fileName);
                lblMessage.Text += csCommonUtility.GetSystemMessage(uploadfile.FileName + "  Attachment(s) uploaded successfully<br>");
            }
            string[] fileEntries = Directory.GetFiles(DestinationPath);

            tdLink.Rows.Clear();
            foreach (string file in fileEntries)
            {
                string FileName = Path.GetFileName(file);
                //File.Delete(Path.Combine(NewDir, FileName));
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink hyp = new HyperLink();

                cell.BorderWidth = 0;
                hyp.Text = FileName.Substring(0, FileName.IndexOf('_')).Trim() + Path.GetExtension(file);
                hyp.NavigateUrl = "Uploads/" + hdnCustomerId.Value + "/" + "Temp" + "/" + FileName;
                hyp.Target = "_blank";
                cell.Controls.Add(hyp);
                cell.HorizontalAlign = HorizontalAlign.Left;
                row.Cells.Add(cell);
                tdLink.Rows.Add(row);

            }
        }
    }

    protected void imgCencel_Click(object sender, ImageClickEventArgs e)
    {

    }


    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        string strpath = Request.PhysicalApplicationPath + "Uploads\\";
        strpath = strpath + "\\" + hdnCustomerId.Value + "\\Temp\\";


        string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnMessageId.Value);
        try
        {

            if (Directory.Exists(strpath))
            {
                // string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnMessageId.Value);
                if (!System.IO.Directory.Exists(NewDir))
                {
                    System.IO.Directory.CreateDirectory(NewDir);
                }
                string[] fileEntries = Directory.GetFiles(strpath);
                foreach (string file in fileEntries)
                {
                    string FileName = Path.GetFileName(file);
                    File.Move(file, Path.Combine(NewDir, FileName));

                }

            }
        }
        catch (Exception ex)
        {

        }

        string strBody = txtBody.Content;

        //try
        //{
        //    ExchangeService service = EWSAPI.GetEWSService();

        //    string sMessId = Request.QueryString.Get("MessId");
        //    sMessId = sMessId.Replace(" ", "+");


        //    EmailMessage message = EWSAPI.GetEmailDetails(sMessId);

        //    ResponseMessage responseMessage = message.CreateReply(false);

        //    responseMessage.BodyPrefix = strBody;
        //    EmailMessage reply = responseMessage.Save();
        //    try
        //    {
        //        if (Directory.Exists(NewDir))
        //        {
        //            string[] fileEntries = Directory.GetFiles(NewDir);
        //            foreach (string fileName in fileEntries)
        //            {
        //                string FName = Path.GetFileName(fileName);
        //                string extFName = FName.Substring(0, FName.IndexOf('_')).Trim() + Path.GetExtension(fileName);
        //                reply.Attachments.AddFileAttachment(extFName, fileName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }



        //    reply.Update(ConflictResolutionMode.AutoResolve);
        //    reply.SendAndSaveCopy();




        //}
        //catch (Exception ex)
        //{
        //    string ss = ex.Message;

        //}




        Session.Add("FromEmailPage", "Yes");
        string url = "customer_details.aspx?cid=" + hdnCustomerId.Value;
        string Script = @"<script language=JavaScript>window.close('" + url + "'); opener.document.forms[0].submit(); </script>";
        if (!IsClientScriptBlockRegistered("OpenFile"))
            this.RegisterClientScriptBlock("OpenFile", Script);


    }

}