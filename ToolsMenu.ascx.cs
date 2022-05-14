using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ToolsMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerID.Value = nCid.ToString();
            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateID.Value = nEstid.ToString();
            GetTools();
        }
    }

    private void GetTools()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string tools = string.Empty;
            userinfo obj = (userinfo)Session["oUser"];
            user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == obj.user_id);
            int ncid = Convert.ToInt32(hdnCustomerID.Value);
            int neid = Convert.ToInt32(hdnEstimateID.Value);

            customer_estimate cus_est = csCommonUtility.GetCustomerEstimateInfo(ncid, neid);
            bool bIsEstimateActive = Convert.ToBoolean(cus_est.IsEstimateActive);

            bool IsCustomersurveysExist = _db.customersurveys.Any(cl => cl.customerid == ncid && cl.estimate_id == neid);

            if (hdnEmailType.Value == "1")
                hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=" + neid;
            else
                hypMessage.NavigateUrl = "customermessagecenter.aspx?cid=" + ncid + "&eid=" + neid;

            hyp_vendor.NavigateUrl = "Vendor_cost_details.aspx?eid=" + neid + "&cid=" + ncid;

            if (!_db.estimate_payments.Any(est_p => est_p.estimate_id == neid && est_p.customer_id == ncid && est_p.client_id == 1))
            {
                hyp_Payment.NavigateUrl = "payment_info.aspx?eid=" + neid + "&cid=" + ncid;

            }
            else
            {
                estimate_payment objEstPay = new estimate_payment();
                objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == neid && pay.customer_id == ncid && pay.client_id == 1);
                hyp_Payment.NavigateUrl = "payment_recieved.aspx?cid=" + ncid + "&epid=" + objEstPay.est_payment_id + "&eid=" + neid;

            }

            hyp_jstatus.NavigateUrl = "customer_job_status_info.aspx?eid=" + neid + "&cid=" + ncid;


            if (bIsEstimateActive && cus_est.status_id == 3)
            {
                hyp_Schedule.NavigateUrl = "schedulecalendar.aspx?eid=" + neid + "&cid=" + ncid + "&TypeID=1";
            }



            hyp_Sow.NavigateUrl = "composite_sow.aspx?eid=" + neid + "&cid=" + ncid;

            hyp_survey.NavigateUrl = "Customer_survey.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_survey.Target = "_blank";



            if (IsCustomersurveysExist)
            {
                hyp_survey.Visible = true;
            }
            else
            {
                hyp_survey.Visible = false;
            }

            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=2&eid=" + neid;
            hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=2&eid=" + neid;
            hyp_SMS.Attributes.Add("onClick", "DisplayWindow2(" + ncid + ");");
            hyp_PreCon.NavigateUrl = "PreconstructionCheckList.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + neid + "&nbackId=1&cid=" + ncid;
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=1&eid=" + neid;
            hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + neid + "&nbackid=1&cid=" + ncid;
            hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + neid + "&cid=" + ncid;
            hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + neid + "&cid=" + ncid;
            hypCostLoc.Target = "_blank";
            hypWarrenty.NavigateUrl = "warrentycertificate.aspx?eid=" + neid + "&cid=" + ncid;
            hypWarrenty.Target = "_blank";
            hypChangeOrderList.NavigateUrl = "changeorderlist.aspx?eid=" + neid + "&cid=" + ncid;
            if (objUser != null)
            {
                tools = (objUser.tools) ?? "";

                if (tools.Contains("Message"))
                    hypMessage.Visible = true;
                else
                    hypMessage.Visible = false;

                if (tools.Contains("ProjectNotes"))
                    hyp_ProjectNotes.Visible = true;
                else
                    hyp_ProjectNotes.Visible = false;

                if (tools.Contains("ActivityLog"))
                    hyp_CallLog.Visible = true;
                else
                    hyp_CallLog.Visible = false;

                if (tools.Contains("SiteReview"))
                    hyp_SiteReview.Visible = true;
                else
                    hyp_SiteReview.Visible = false;

                if (tools.Contains("DocumentManagement"))
                    hyp_DocumentManagement.Visible = true;
                else
                    hyp_DocumentManagement.Visible = false;



                if (tools.Contains("Payment") && cus_est.status_id == 3)
                    hyp_Payment.Visible = true;
                else
                    hyp_Payment.Visible = false;

                if (tools.Contains("JobStatus") && cus_est.status_id == 3)
                    hyp_jstatus.Visible = true;
                else
                    hyp_jstatus.Visible = false;

                if (tools.Contains("Schedule") && cus_est.status_id == 3)
                    hyp_Schedule.Visible = true;
                else
                    hyp_Schedule.Visible = false;

                if (tools.Contains("CompositeSow") && cus_est.status_id == 3)
                    hyp_Sow.Visible = true;
                else
                    hyp_Sow.Visible = false;

                if (tools.Contains("PreConCheckList") && cus_est.status_id == 3)
                    hyp_PreCon.Visible = true;
                else
                    hyp_PreCon.Visible = false;


                if (tools.Contains("Vendor") && cus_est.estimate_id != 0)
                    hyp_vendor.Visible = true;
                else
                    hyp_vendor.Visible = false;

                if (tools.Contains("AllowanceReport") && cus_est.estimate_id != 0)
                    hyp_Allowance.Visible = true;
                else
                    hyp_Allowance.Visible = false;

                if (tools.Contains("Selection") && cus_est.estimate_id != 0)
                    hyp_Section_Selection.Visible = true;
                else
                    hyp_Section_Selection.Visible = false;

                if (tools.Contains("ProjectSummary") && cus_est.estimate_id != 0)
                    hypCostLoc.Visible = true;
                else
                    hypCostLoc.Visible = false;

                if (tools.Contains("MaterialTracking") && cus_est.estimate_id != 0)
                    hyp_MaterialTracking.Visible = true;
                else
                    hyp_MaterialTracking.Visible = false;
                if (tools.Contains("ProjectComWarrenty"))
                    hypWarrenty.Visible = true;
                else
                    hypWarrenty.Visible = false;
                if (tools.Contains("ChangeOrderList"))
                    hypChangeOrderList.Visible = true;
                else
                    hypChangeOrderList.Visible = false;

            }
        }
        catch (Exception ex)
        {
        }
    }
}