using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

public partial class warrentycertificate : System.Web.UI.Page
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
            //int CusId = 226;
            int CusId = Convert.ToInt32(Request.QueryString.Get("cid"));
            int EstId = Convert.ToInt32(Request.QueryString.Get("eid"));
            DataClassesDataContext _db = new DataClassesDataContext();
            if (_db.customersurveys.Where(cs => cs.customerid == CusId && cs.estimate_id == EstId).SingleOrDefault() != null)
            {
                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == CusId);

                //if (objCust.customer_signature == null || objCust.customer_signature == "")
                //{
                //    lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Please signature at first to view completion & warranty certificate.");
                //    return;
                //}

                string imgSign = string.Format("data:image/jpeg;base64,{0}", objCust.customer_signature);
                string sourceFile = "";
                string DestinationPath = null;
                if (imgSign != "data:image/jpeg;base64,")
                {
                    string[] split_1 = imgSign.Split(';');
                    string split_2 = split_1[1].Replace("base64,", "");


                    DestinationPath = Server.MapPath("~/Images//Signature//") + objCust.customer_id + "//";
                    if (!System.IO.Directory.Exists(DestinationPath))
                    {
                        System.IO.Directory.CreateDirectory(DestinationPath);
                    }
                    sourceFile = Server.MapPath("Images\\Signature\\") + objCust.customer_id + @"\";
                    SaveByteArrayAsImage(sourceFile + "signature.png", split_2);

                }


                string strCustName2 = "";
                string strCompletionDate = "";
                string strContractDate = "";
                string strWarrentyValidDate = "";
                string strCross = "";
                string strCustName = objCust.first_name1 + " " + objCust.last_name1;
                strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
                string strAddress = objCust.address;
                strCross = objCust.cross_street;
                string strCityStaeZip = objCust.city + ", " + objCust.state + " " + objCust.zip_code;
                string strAddress2 = strAddress + " " + strCityStaeZip;
                if (_db.estimate_payments.Where(pay => pay.estimate_id == EstId && pay.customer_id == CusId && pay.client_id == Convert.ToInt32(objCust.client_id)).SingleOrDefault() != null)
                {
                    estimate_payment objEstPay = new estimate_payment();
                    objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == EstId && pay.customer_id == CusId && pay.client_id == Convert.ToInt32(objCust.client_id));
                    strContractDate = objEstPay.contract_date;
                }
                customersurvey csv = new customersurvey();
                csv = _db.customersurveys.Single(cs => cs.customerid == CusId && cs.estimate_id == EstId);
                strCompletionDate = Convert.ToDateTime(csv.date).ToShortDateString();
                strWarrentyValidDate = Convert.ToDateTime(csv.date).AddYears(5).ToShortDateString();
                ReportDocument rptFile = new ReportDocument();
                string strReportPath = Server.MapPath(@"Reports\rpt\rptCompletionCertificate.rpt");
                rptFile.Load(strReportPath);


                Hashtable ht = new Hashtable();
                ht.Add("p_CustomerName", strCustName);
                ht.Add("p_CustomerName2", strCustName2);
                ht.Add("p_address", strAddress);
                ht.Add("p_address2", strAddress2);
                ht.Add("p_crossstreet", strCross);
                ht.Add("p_CityStaeZip", strCityStaeZip);
                ht.Add("p_ContractDate", strContractDate);
                ht.Add("p_CompletionDate", strCompletionDate);
                ht.Add("p_WarrentyValidDate", strWarrentyValidDate);
                sourceFile = Server.MapPath("Images\\Signature\\") + objCust.customer_id + @"\";
               // rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sourceFile + "signature.png" + "'";
                Session.Add(SessionInfo.Report_File, rptFile);
                Session.Add(SessionInfo.Report_Param, ht);
                Response.Redirect("Reports/Common/ReportViewer.aspx");
               
            }
            else
            {
                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == CusId);


                string imgSign = string.Format("data:image/jpeg;base64,{0}", objCust.customer_signature);
                string sourceFile = "";
                string DestinationPath = null;
                if (imgSign != "data:image/jpeg;base64,")
                {
                    string[] split_1 = imgSign.Split(';');
                    string split_2 = split_1[1].Replace("base64,", "");


                    DestinationPath = Server.MapPath("~/Images//Signature//") + objCust.customer_id + "//";
                    if (!System.IO.Directory.Exists(DestinationPath))
                    {
                        System.IO.Directory.CreateDirectory(DestinationPath);
                    }
                    sourceFile = Server.MapPath("Images\\Signature\\") + objCust.customer_id + @"\";
                    SaveByteArrayAsImage(sourceFile + "signature.png", split_2);

                }

                string strCustName2 = "";
                string strCompletionDate = "";
                string strContractDate = "";
                string strWarrentyValidDate = "";
                string strCross = "";
                string strCustName = objCust.first_name1 + " " + objCust.last_name1;
                strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
                string strAddress = objCust.address;
                strCross = objCust.cross_street;
                string strCityStaeZip = objCust.city + ", " + objCust.state + " " + objCust.zip_code;
                string strAddress2 = strAddress + " " + strCityStaeZip;
                if (_db.estimate_payments.Where(pay => pay.estimate_id == EstId && pay.customer_id == CusId && pay.client_id == Convert.ToInt32(objCust.client_id)).SingleOrDefault() != null)
                {
                    estimate_payment objEstPay = new estimate_payment();
                    objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == EstId && pay.customer_id == CusId && pay.client_id == Convert.ToInt32(objCust.client_id));
                    strContractDate = objEstPay.contract_date;
                }
               
               
                ReportDocument rptFile = new ReportDocument();
                string strReportPath = Server.MapPath(@"Reports\rpt\rptCompletionCertificate.rpt");
                rptFile.Load(strReportPath);


                Hashtable ht = new Hashtable();
                ht.Add("p_CustomerName", strCustName);
                ht.Add("p_CustomerName2", strCustName2);
                ht.Add("p_address", strAddress);
                ht.Add("p_address2", strAddress2);
                ht.Add("p_crossstreet", strCross);
                ht.Add("p_CityStaeZip", strCityStaeZip);
                ht.Add("p_ContractDate", strContractDate);
                ht.Add("p_CompletionDate", strCompletionDate);
                ht.Add("p_WarrentyValidDate", strWarrentyValidDate);
                sourceFile = Server.MapPath("Images\\Signature\\") + objCust.customer_id + @"\";
                //rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sourceFile + "signature.png" + "'";
                Session.Add(SessionInfo.Report_File, rptFile);
                Session.Add(SessionInfo.Report_Param, ht);
                Response.Redirect("Reports/Common/ReportViewer.aspx");
                // lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Can't display completion certificate without completion of project");
            }


        }


    }

    private void SaveByteArrayAsImage(string fullOutputPath, string base64String)
    {
        base64String = base64String.Replace("data:image/png;base64,", String.Empty);
        if (base64String.Length < 1) return;
        try
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            System.Drawing.Image image;

            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);

            // Convert byte[] to Image
            ms.Write(bytes, 0, bytes.Length);

            image = System.Drawing.Image.FromStream(ms, true);

            // string outputFileName = Path.Combine(fullOutputPath, fileName);
            System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(image);
            System.Drawing.Imaging.ImageFormat format = bmpImage.RawFormat;
            int newWidth = 200;
            int newHeight = 150;
            System.Drawing.Bitmap bmpOut = null;
            bmpOut = new System.Drawing.Bitmap(newWidth, newHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpOut);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, newWidth, newHeight);
            g.DrawImage(bmpImage, 0, 0, newWidth, newHeight);
            bmpImage.Dispose();
            bmpOut.Save(fullOutputPath, format);
            bmpOut.Dispose();

            // image.Save(fullOutputPath, System.Drawing.Imaging.ImageFormat.Png);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
