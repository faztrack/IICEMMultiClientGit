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

public partial class Customer_survey : System.Web.UI.Page
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
                string strQs1 = "";
                string strQs2= "";
                string strQs3 = "";
                string strQs4 = "";
                string strQs5 = "";
                string strQs6 = "";
                string strQs7 = "";
                string strQs8 = "";
                string strCompletionDate = "";
                string strSalesPerson = "";

               
                string strCustName2 = "";
                string strCross = "";
                string strCustName = objCust.first_name1 + " " + objCust.last_name1;
                strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
                string strAddress = objCust.address;
                strCross = objCust.cross_street;
                string strCityStaeZip = objCust.city + ", " + objCust.state + ", " + objCust.zip_code;
                ReportDocument rptFile = new ReportDocument();
                string strReportPath = Server.MapPath(@"Reports\rpt\rptCustomerSurvey.rpt");
                rptFile.Load(strReportPath);
                customersurvey objcs = new customersurvey();
                if (_db.customersurveys.Where(cl => cl.customerid == CusId && cl.estimate_id == EstId).ToList().Count > 0)
                {
                    objcs = _db.customersurveys.Where(cs => cs.customerid == CusId && cs.estimate_id == EstId).FirstOrDefault();

                    strQs1 = objcs.answer1;
                    strQs2 = objcs.answer2;
                    strQs3 = objcs.answer3;
                    strQs4 = objcs.answer4;
                    strQs5 = objcs.answer5;
                    strQs6 = objcs.answer6;
                    strQs7 = objcs.answer7;
                    strQs8 = objcs.answer8;
                    strCompletionDate = Convert.ToDateTime(objcs.date).ToShortDateString();
                }

                sales_person sal_per = new sales_person();
                if (_db.sales_persons.Where(sp => sp.sales_person_id == objCust.sales_person_id).ToList().Count > 0)
                {
                    sal_per = _db.sales_persons.Where(sp => sp.sales_person_id == objCust.sales_person_id).FirstOrDefault();
                    strSalesPerson = sal_per.first_name + " " + sal_per.last_name;
                }



                Hashtable ht = new Hashtable();
                ht.Add("p_CustomerName", strCustName);
                ht.Add("p_CustomerName2", strCustName2);
                ht.Add("p_address", strAddress);
                ht.Add("p_crossstreet", strCross);
                ht.Add("p_CityStaeZip", strCityStaeZip);
                ht.Add("p_Qs1", strQs1);
                ht.Add("p_Qs2", strQs2);
                ht.Add("p_Qs3", strQs3);
                ht.Add("p_Qs4", strQs4);
                ht.Add("p_Qs5", strQs5);
                ht.Add("p_Qs6", strQs6);
                ht.Add("p_Qs7", strQs7);
                ht.Add("p_Qs8", strQs8);
                ht.Add("p_CompletionDate", strCompletionDate);
                ht.Add("p_SalesPerson", strSalesPerson);

                Session.Add(SessionInfo.Report_File, rptFile);
                Session.Add(SessionInfo.Report_Param, ht);
                Response.Redirect("Reports/Common/ReportViewer.aspx");
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
            }

           
        }
    }
}
