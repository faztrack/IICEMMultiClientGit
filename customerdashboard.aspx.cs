using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using com.paypal.sdk.core;
using com.paypal.sdk.util;
using com.paypal.sdk.services;
using com.paypal.sdk.profiles;
using System.Net;
using System.IO;
using System.Net.Mail;




public partial class customerdashboard : System.Web.UI.Page
{
    public DataTable dtTerms;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            if (Session["oCustomerUser"] == null)
            {
                Response.Redirect("customerlogin.aspx");
            }

            BindStates();
            imgA.Attributes.Add("onClick", "DisplayWindow1();");
            imgB.Attributes.Add("onClick", "DisplayWindow2();");
            imgC.Attributes.Add("onClick", "DisplayWindow3();");

            imgD.Attributes.Add("onClick", "DisplayWindow4();");

            imgE.Attributes.Add("onClick", "DisplayWindow5();");


            // btnFinalizePayment.Attributes.Add("onClick", "return confirmOperation();");	

            lblDisMessage.Text = string.Empty;

            if (Session["oCustomerUser"] != null)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
                int nCustomerId = Convert.ToInt32(obj.customerid);



                hdnCustomerId.Value = nCustomerId.ToString();
                if (Convert.ToInt32(hdnCustomerId.Value) > 0)
                {
                    customer cust = new customer();
                    cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    string strSecondName = cust.first_name2 + " " + cust.last_name2;
                    if (strSecondName.Trim() == "")
                        lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                    else
                        lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1 + " & " + strSecondName;

                    string strAddress = "";
                    strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                    lblAddress.Text = strAddress;
                    lblPhone.Text = cust.phone;
                    lblEmail.Text = cust.email;

                    hdnClientId.Value = cust.client_id.ToString();


                    txtCardHolderName.Text = cust.first_name1 + " " + cust.last_name1;
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();
                    txtAddress.Text = cust.address;
                    txtCity.Text = cust.city;
                    ddlState.SelectedValue = cust.state;
                    txtZip.Text = cust.zip_code;

                    trJobStatus.Visible = (bool)cust.isJobSatusViewable;
                }
                string strQ = "select * from customer_estimate where customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and client_id= "+ hdnClientId.Value + " and status_id = 3 order by estimate_id desc ";
                IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);
                grdEstimates.DataSource = clist;
                grdEstimates.DataKeyNames = new string[] { "customer_id", "estimate_id", "status_id", "sale_date", "estimate_name" };
                grdEstimates.DataBind();


                if (_db.customer_estimates.Where(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.status_id == 3).ToList().Count > 0)
                {
                    int nEstId = 0;
                    var result = (from ce in _db.customer_estimates
                                  where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.status_id == 3
                                  select ce.estimate_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nEstId = result.Max();

                    if (_db.pricing_details.Any(p => p.estimate_id == nEstId && p.customer_id == Convert.ToInt32(hdnCustomerId.Value)))
                        hdnEstimateId.Value = nEstId.ToString();
                    else
                    {
                        hdnEstimateId.Value = (nEstId - 1).ToString();
                        hypComposite.Visible = false;
                    }
                }
                else
                {
                    int nEstId = 0;
                    var result = (from ce in _db.customer_estimates
                                  where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value)
                                  select ce.estimate_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nEstId = result.Max();

                    if (_db.pricing_details.Any(p => p.estimate_id == nEstId && p.customer_id == Convert.ToInt32(hdnCustomerId.Value)))
                        hdnEstimateId.Value = nEstId.ToString();
                    else
                    {
                        hdnEstimateId.Value = (nEstId - 1).ToString();
                        hypComposite.Visible = false;
                    }
                }
                hypComposite.NavigateUrl = "customer_sow.aspx?eid=" + Convert.ToInt32(hdnEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value);

                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == nCustomerId && sc.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).Count() == 0)
                {
                    imgD.Attributes.Add("onClick", "DisplayWindow6();"); // alert message
                }

                GetChangeOrders(nCustomerId, 0);
                GetCustomerMessageInfo(nCustomerId, 0);
                GetJobStatusInfo(nCustomerId);
                Calculate();
                LoadDscription();
                LoadTerms();

                GetCardLists();

                if (grdCardList.Rows.Count > 0)
                {
                    pnlExistCard.Visible = true;
                    pnlNewCard.Visible = false;
                    chkNewCard.Visible = true;
                    chkNewCard.Checked = false;
                }
                else
                {
                    pnlExistCard.Visible = false;
                    pnlNewCard.Visible = true;
                    chkNewCard.Checked = true;
                    chkNewCard.Visible = false;
                }


            }

        }



    }
    private void BindStates()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var states = from st in _db.states
                     orderby st.abbreviation
                     select st;
        ddlState.DataSource = states;
        ddlState.DataTextField = "abbreviation";
        ddlState.DataValueField = "abbreviation";
        ddlState.DataBind();
    }

    private void Calculate()
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes

        }
        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == Convert.ToInt32(hdnClientId.Value)
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecievedAmount.Text = payAmount.ToString("c");

     

        decimal TotalCOAmount = 0;
        var COitem = from co in _db.changeorder_estimates
                     where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                     orderby co.changeorder_name ascending
                     select co;
        foreach (changeorder_estimate cho in COitem)
        {
            int ncoeid = cho.chage_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            CoTaxRate = Convert.ToDecimal(cho.tax);
            decimal dEconCost = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int cn = Coresult.Count();
            if (Coresult != null && cn > 0)
                dEconCost = Coresult.Sum();

            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;

            }
            else
            {
                CoPrice = dEconCost;
            }
            TotalCOAmount += CoPrice;

        }
        lblTotalCOAmount.Text = TotalCOAmount.ToString("c");

        decimal TotalCostAmount = 0;
        TotalCostAmount = totalwithtax + TotalCOAmount;
        lblTotalAmount.Text = TotalCostAmount.ToString("c");

        decimal TotalBalanceAmount = 0;
        TotalBalanceAmount = TotalCostAmount - payAmount;
        lblTotalBalanceAmount.Text = TotalBalanceAmount.ToString("c");

    }
    private void GetChangeOrders(int nCustId, int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from co in _db.changeorder_estimates
                   where co.customer_id == nCustId && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.is_cutomer_viewable == 1
                   orderby co.chage_order_id ascending
                   select co;
        grdChangeOrders.PageIndex = nPageNo;

        if (ddlShowChangesOrders.SelectedValue != "4")
        {
            grdChangeOrders.PageSize = Convert.ToInt32(ddlShowChangesOrders.SelectedValue);
        }
        else
        {
            grdChangeOrders.PageSize = 200;
        }

        grdChangeOrders.DataSource = item;
        grdChangeOrders.DataKeyNames = new string[] { "chage_order_id", "sales_person_id", "estimate_id", "change_order_estimate_id" };
        grdChangeOrders.DataBind();

        if (item.Count() == 0)
            tblChangeOrders.Visible = false;
        else
            tblChangeOrders.Visible = true;

    }
    protected void grdChangeOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nCOId = Convert.ToInt32(grdChangeOrders.DataKeys[e.Row.RowIndex].Values[0]);
            int nSalesPersonId = Convert.ToInt32(hdnSalesPersonId.Value);
            int nEstimateId = Convert.ToInt32(grdChangeOrders.DataKeys[e.Row.RowIndex].Values[2]);
            int ncoestId = Convert.ToInt32(grdChangeOrders.DataKeys[e.Row.RowIndex].Values[3]);
            sales_person objsp = new sales_person();
            objsp = _db.sales_persons.Single(sp => sp.sales_person_id == nSalesPersonId);

            e.Row.Cells[5].Text = objsp.first_name + " " + objsp.last_name;

            HyperLink hypViewCO = (HyperLink)e.Row.FindControl("hypViewCO");
            hypViewCO.NavigateUrl = "customerchangeorder.aspx?coid=" + nCOId + "&eid=" + nEstimateId + "&cid=" + hdnCustomerId.Value + "&coestid=" + ncoestId;

            if (e.Row.Cells[4].Text.Trim() != "")
            {
                int nStatusId = Convert.ToInt32(e.Row.Cells[4].Text.Trim());
                if (nStatusId == 1)
                    e.Row.Cells[4].Text = "Draft";
                else if (nStatusId == 2)
                    e.Row.Cells[4].Text = "Pending";
                else if (nStatusId == 3)
                    e.Row.Cells[4].Text = "Executed";
                else if (nStatusId == 4)
                    e.Row.Cells[4].Text = "Declined";
            }
        }
    }
    private void GetJobStatusInfo(int nCustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
        {
            customer_estimate cus_est = new customer_estimate();
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            lblEstimateName.Text = cus_est.estimate_name;
        }
        customer_jobstatus objCJS = new customer_jobstatus();
        int nJobStatusId = 0;
        if (_db.customer_jobstatus.Where(c => c.customerid == nCustomerId && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            objCJS = _db.customer_jobstatus.Single(c => c.customerid == nCustomerId && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            nJobStatusId = Convert.ToInt32(objCJS.jobstatusid);
        }
        BindImages(Convert.ToInt32(objCJS.jobstatusid));

        if (_db.customersurveys.Where(cs => cs.customerid == nCustomerId && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customersurvey csv = new customersurvey();
            csv = _db.customersurveys.Single(cs => cs.customerid == nCustomerId && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            rdoProjectComplete.Enabled = false;
            rdoProjectComplete.SelectedValue = "1";
            lblProjectCompletion.Visible = true;
            lblProjectCompletionDate.Visible = true;
            lblProjectCompletionDate.Text = Convert.ToDateTime(csv.date).ToShortDateString();

        }
        else
        {
            rdoProjectComplete.Enabled = true;
            rdoProjectComplete.SelectedValue = "2";
            lblProjectCompletion.Visible = false;
            lblProjectCompletionDate.Visible = false;
        }

    }
    protected void ddlShowChangesOrders_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlShowChangesOrders.ID, ddlShowChangesOrders.GetType().Name, "SelectedIndexChanged"); 
        GetChangeOrders(Convert.ToInt32(hdnCustomerId.Value), 0);
    }
    protected void grdChangeOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdChangeOrders.ID, grdChangeOrders.GetType().Name, "PageIndexChanging"); 
        GetChangeOrders(Convert.ToInt32(hdnCustomerId.Value), e.NewPageIndex);
    }
   
    private void GetCustomerMessageInfo(int nCustId, int nPageNo)
    {
        if (nCustId > 0)
        {

            try
            {

                DSMessage dsMessageSent = new DSMessage();

                DataClassesDataContext _db = new DataClassesDataContext();
                string custEmail = lblEmail.Text;
                var messList = (from mess_info in _db.customer_messages
                                where mess_info.customer_id == nCustId && (mess_info.mess_to.Contains(custEmail.Trim()) || mess_info.mess_from.Contains(custEmail.Trim())) && mess_info.client_id == Convert.ToInt32(hdnClientId.Value)
                                orderby mess_info.cust_message_id descending
                                select mess_info).ToList();

                foreach (customer_message msg in messList)
                {
                    DSMessage.MessageRow mes = dsMessageSent.Message.NewMessageRow();

                    if (msg.HasAttachments == null)
                    {
                        string strQ = "select * from message_upolad_info where customer_id=" + nCustId + " and message_id=" + msg.message_id + " and client_id=" + Convert.ToInt32(hdnClientId.Value);
                        IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                        string mess_file = "";
                        foreach (message_upolad_info message_upolad in list)
                        {
                            mess_file += message_upolad.mess_file_name.Replace("amp;", "").Trim() + ", "; ;
                        }
                        mess_file = mess_file.Trim().TrimEnd(',');

                        if (mess_file.Length > 0)
                        {
                            mes.HasAttachments = true;
                            mes.AttachmentList = mess_file.Trim().TrimEnd(',');


                        }
                        else
                        {
                            mes.AttachmentList = "";
                            mes.HasAttachments = false;// msg.HasAttachments;
                        }

                        msg.HasAttachments = mes.HasAttachments;
                        msg.AttachmentList = mes.AttachmentList;

                    }
                    else if (Convert.ToBoolean(msg.HasAttachments))
                    {

                        mes.HasAttachments = true;
                        mes.AttachmentList = msg.AttachmentList;


                    }
                    else
                    {
                        mes.HasAttachments = false;
                        mes.AttachmentList = "";
                    }

                    mes.From = msg.mess_from;
                    mes.To = msg.mess_to;
                    mes.IsRead = (bool)(msg.IsView ?? false);
                    mes.customer_id = nCustId.ToString();
                    mes.message_id = msg.message_id.ToString();
                    mes.create_date = (DateTime)msg.create_date;
                    if (msg.mess_subject != null)
                        mes.mess_subject = msg.mess_subject.ToString();
                    else
                        mes.mess_subject = "";
                    mes.last_view = (DateTime)msg.last_view;
                    mes.Protocol = msg.Protocol;
                    mes.Type = msg.Type;
                    mes.sent_by = msg.sent_by;
                    dsMessageSent.Message.AddMessageRow(mes);

                }

                _db.SubmitChanges();

                dsMessageSent.AcceptChanges();

                DataView dv = dsMessageSent.Tables[0].DefaultView;
                dv.Sort = "create_date DESC";
                grdCustomersMessage.PageIndex = nPageNo;
                grdCustomersMessage.DataSource = dv;
                grdCustomersMessage.DataKeyNames = new string[] { "customer_id", "message_id", "AttachmentList", "From", "To", "Type" };
                grdCustomersMessage.DataBind();

                if (grdCustomersMessage.Rows.Count == 0)
                    tblMessage.Visible = false;
                else
                    tblMessage.Visible = true;




            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

            }

        }

    }


    protected void grdCustomersMessage_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                string script = "";

                string Attacheent = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[2].ToString();
                string MessId = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[1].ToString();
                int CustId = Convert.ToInt32(grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[0].ToString());

                string From = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[3].ToString();
                string To = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[4].ToString();
                string Type = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[5].ToString();

                HyperLink hypMessageDetails = (HyperLink)e.Row.FindControl("hypMessageDetails");
                hypMessageDetails.ToolTip = "Click on Message Details to view specific Message Details .";
                hypMessageDetails.Target = "MyWindow";


                script = String.Format("GetdatakeyValue1Old('{0}')", MessId.ToString());

                hypMessageDetails.Attributes.Add("onclick", script);
                if (Attacheent.Length > 0)
                {
                    HyperLink hypAttachment = (HyperLink)e.Row.FindControl("hypAttachment");
                    hypAttachment.Text = Attacheent;
                    hypAttachment.ToolTip = "Click on Message Details to view specific Message Details .";
                    hypAttachment.Target = "MyWindow";

                    hypAttachment.Attributes.Add("onclick", script);
                }





            }
            catch { }

        }
    }


    protected void grdCustomersMessage_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustomersMessage.ID, grdCustomersMessage.GetType().Name, "PageIndexChanging"); 
        GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value), e.NewPageIndex);
    }
    private void BindImages(int nJobStatusId)
    {

        if (nJobStatusId == 0)
        {
            imgA.ImageUrl = "JobImages/OrangeA.jpg";
            imgB.ImageUrl = "JobImages/WhiteB.jpg";
            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";
        }
        else if (nJobStatusId == 1)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/OrangeB.jpg";
            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";
        }
        else if (nJobStatusId == 2)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/OrangeC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";


        }
        else if (nJobStatusId == 3)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/OrangeD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";
        }
        else if (nJobStatusId == 4)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/OrangeE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";


        }
        else if (nJobStatusId == 5)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/OrangeF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";
        }
        else if (nJobStatusId == 6)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/GreenF.jpg";
            imgButtonG.ImageUrl = "JobImages/OrangeG.jpg";
        }
        else
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/GreenF.jpg";
            imgButtonG.ImageUrl = "JobImages/GreenG.jpg";
        }
    }
    protected void rdoProjectComplete_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoProjectComplete.ID, rdoProjectComplete.GetType().Name, "SelectedIndexChanged"); 
        lblResult.Text = "";
        lblMessage.Text = "";

        if (rdoProjectComplete.SelectedValue == "1")
            pnlSurvey.Visible = true;
        else
            pnlSurvey.Visible = false;
    }
    protected void btnSaveAnswers_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveAnswers.ID, btnSaveAnswers.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMessage.Text = "";

        #region comment
        //if (txtAnswer1.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question1.";

        //    return;
        //}
        //if (txtAnswer2.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question2.";

        //    return;
        //}
        //if (txtAnswer3.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question3.";

        //    return;
        //}
        //if (txtAnswer4.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question4.";

        //    return;
        //}
        //if (txtAnswer5.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question5.";

        //    return;
        //}
        //if (txtAnswer6.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question6.";

        //    return;
        //}
        //if (txtAnswer7.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question7.";

        //    return;
        //}
        //if (txtAnswer8.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question8.";

        //    return;
        //}
        //if (txtAnswer9.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question9.";

        //    return;
        //}
        //if (txtAnswer10.Text.Trim() == "")
        //{
        //    lblResult.Text = "Please answer question10.";

        //    return;
        //}
        #endregion

        if (Convert.ToInt32(hdnCustomerId.Value) == 0)
        {
            if (Convert.ToInt32(hdnEstimateId.Value) == 0)
                return;
        }


        DataClassesDataContext _db = new DataClassesDataContext();

        customer_jobstatus objCJS = new customer_jobstatus();
        objCJS.customerid = Convert.ToInt32(hdnCustomerId.Value);
        objCJS.jobstatusid = 7;
        objCJS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        objCJS.client_id = Convert.ToInt32(hdnClientId.Value);
        if (_db.customer_jobstatus.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() == null)
        {
            _db.customer_jobstatus.InsertOnSubmit(objCJS);
        }
        else
        {
            string strQ = "UPDATE customer_jobstatus SET jobstatusid = 7 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
        }
        _db.SubmitChanges();

        customersurvey objcs = new customersurvey();
        objcs.customerid = Convert.ToInt32(hdnCustomerId.Value);
        objcs.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        objcs.answer1 = txtAnswer1.Text;
        objcs.answer2 = txtAnswer2.Text;
        objcs.answer3 = txtAnswer3.Text;
        objcs.answer4 = txtAnswer4.Text;
        objcs.answer5 = txtAnswer5.Text;
        objcs.answer6 = txtAnswer6.Text;
        objcs.answer7 = txtAnswer7.Text;
        objcs.answer8 = txtAnswer8.Text;
        objcs.answer9 = "";
        objcs.answer10 = "";
        objcs.date = DateTime.Now;
        objcs.client_id = Convert.ToInt32(hdnClientId.Value);

        _db.customersurveys.InsertOnSubmit(objcs);
        _db.SubmitChanges();

        lblMessage.Text = csCommonUtility.GetSystemMessage("Survey saved successfully.");



        BindImages(7);

        if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customersurvey csv = new customersurvey();
            csv = _db.customersurveys.Single(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            rdoProjectComplete.Enabled = false;
            rdoProjectComplete.SelectedValue = "1";
            lblProjectCompletion.Visible = true;
            lblProjectCompletionDate.Visible = true;
            lblProjectCompletionDate.Text = Convert.ToDateTime(csv.date).ToShortDateString();

        }
        pnlSurvey.Visible = false;
        rdoProjectComplete.Enabled = false;

    }
    protected void imgButtonF_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgButtonF.ID, imgButtonF.GetType().Name, "Click"); 
        lblDisMessage.Text = string.Empty;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            string strCustName2 = "";
            string strCross = "";
            string strCustName = objCust.first_name1 + " " + objCust.last_name1;
            strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
            string strAddress = objCust.address;
            strCross = objCust.cross_street;
            string strCityStaeZip = objCust.city + " " + objCust.state + " " + objCust.zip_code;
            ReportDocument rptFile = new ReportDocument();
            string strReportPath = Server.MapPath(@"Reports\rpt\rptCompletionCertificate.rpt");
            rptFile.Load(strReportPath);


            Hashtable ht = new Hashtable();
            ht.Add("p_CustomerName", strCustName);
            ht.Add("p_CustomerName2", strCustName2);
            ht.Add("p_address", strAddress);
            ht.Add("p_crossstreet", strCross);
            ht.Add("p_CityStaeZip", strCityStaeZip);

            Session.Add(SessionInfo.Report_File, rptFile);
            Session.Add(SessionInfo.Report_Param, ht);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        }
        else
        {
            lblDisMessage.Text = csCommonUtility.GetSystemRequiredMessage("Can't display completion certificate without completion of project");
        }
    }
    protected void imgButtonG_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgButtonG.ID, imgButtonG.GetType().Name, "Click"); 
        lblDisMessage.Text = string.Empty;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

            string strCompletionDate = "";
            string strContractDate = "";
            string strMainCustName = "";
            string MainAddress = "";
            string strCustName2 = "";
            string strCross = "";
            string strCustName = objCust.first_name1 + " " + objCust.last_name1;
            strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
            string strAddress = objCust.address;
            strCross = objCust.cross_street;
            string strCityStaeZip = objCust.city + ", " + objCust.state + ", " + objCust.zip_code;
            if (strCustName2.Length > 2)
                strMainCustName = strCustName + "&" + strCustName2;
            else
                strMainCustName = strCustName;
            if (strCross.Length > 2)
                MainAddress = strAddress + ", " + strCross;
            else
                MainAddress = strAddress;
            MainAddress += ", " + strCityStaeZip;
            if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
            {
                customersurvey csv = new customersurvey();
                csv = _db.customersurveys.Single(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                strCompletionDate = Convert.ToDateTime(csv.date).ToShortDateString();

            }
           
            if (_db.estimate_payments.Where(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                estimate_payment objEstPay = new estimate_payment();
                objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value));
                strContractDate = objEstPay.contract_date;
            }


            ReportDocument rptFile = new ReportDocument();
            string strReportPath = Server.MapPath(@"Reports\rpt\rptWarranty.rpt");
            rptFile.Load(strReportPath);


            Hashtable ht = new Hashtable();
            ht.Add("p_CustomerName", strMainCustName);
            ht.Add("p_address", MainAddress);
            ht.Add("p_ContractDate", strContractDate);
            ht.Add("p_CompletionDate", strCompletionDate);

            Session.Add(SessionInfo.Report_File, rptFile);
            Session.Add(SessionInfo.Report_Param, ht);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        }
        else
        {
            lblDisMessage.Text = csCommonUtility.GetSystemRequiredMessage("Can't display warranty document without completion of project");
        }
    }
    protected void grdEstimates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int neid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nsid = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[2].ToString());

            if (nsid == 3)
                e.Row.Cells[1].Text = "Sold on " + Convert.ToDateTime(grdEstimates.DataKeys[e.Row.RowIndex].Values[3].ToString()).ToShortDateString();


        }


    }

    protected void grdEstimates_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView grdEstimates = (GridView)sender;
        if (e.CommandName == "view")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int index = Convert.ToInt32(e.CommandArgument);
            int nCustomerId = Convert.ToInt32(grdEstimates.DataKeys[index].Values[0].ToString());
            int neid = Convert.ToInt32(grdEstimates.DataKeys[index].Values[1].ToString());
            int nsid = Convert.ToInt32(grdEstimates.DataKeys[index].Values[2].ToString());
            hdnEstimateId.Value = neid.ToString();

            if (_db.ScheduleCalendars.Where(sc => sc.customer_id == nCustomerId && sc.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).Count() == 0)
            {
                imgD.Attributes.Add("onClick", "DisplayWindow6();"); // alert message
            }
            else
            {
                imgD.Attributes.Add("onClick", "DisplayWindow4();");
            }

            hypComposite.NavigateUrl = "customer_sow.aspx?eid=" + neid + "&cid=" + Convert.ToInt32(hdnCustomerId.Value);
            GetChangeOrders(nCustomerId, 0);
            GetCustomerMessageInfo(nCustomerId, 0);
            GetJobStatusInfo(nCustomerId);
            Calculate();
            LoadDscription();
            LoadTerms();

            GetCardLists();

            if (grdCardList.Rows.Count > 0)
            {
                pnlExistCard.Visible = true;
                pnlNewCard.Visible = false;
                chkNewCard.Visible = true;
                chkNewCard.Checked = false;
            }
            else
            {
                pnlExistCard.Visible = false;
                pnlNewCard.Visible = true;
                chkNewCard.Checked = true;
                chkNewCard.Visible = false;
            }
            lblReason.Text = "";
            lblCardResult.Text = "";

        }
    }
    private static string HtmlToPlainText(string html)
    {
        const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
        const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
        const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
        var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
        var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

        var text = html;
        //Decode html specific characters
        //text = System.Net.WebUtility.HtmlDecode(text);
        text = HttpUtility.HtmlDecode(text);
        //Remove tag whitespace/line breaks
        text = tagWhiteSpaceRegex.Replace(text, "><");
        //Replace <br /> with line breaks
        text = lineBreakRegex.Replace(text, Environment.NewLine);
        //Strip formatting
        text = stripFormattingRegex.Replace(text, Environment.NewLine);

        return text;
    }
    private void LoadTerms()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from co in _db.changeorder_estimates
                   where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                   select new COName_Tax()
                   {
                       change_order_id = (int)co.chage_order_id,
                       changeorder_name = (string)co.changeorder_name,
                       tax = (decimal)co.tax
                   };

        estimate_payment objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value)); //pay.est_payment_id == nPaymentId &&

        DataTable dtPayment = (DataTable)Session["part_payment"];
        DataView dv = dtPayment.DefaultView;

        DataTable tmpTTable = LoadTermTable();
        DataRow dr = tmpTTable.NewRow();
        if (objEstPay.deposit_amount > 0)
        {

            dr = tmpTTable.NewRow();
            string strTermId = "1";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }

            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 1;
            dr["term_name"] = objEstPay.deposit_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.deposit_amount;
            dr["pay_term_date"] = objEstPay.deposit_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.countertop_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "2";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 2;
            dr["term_name"] = objEstPay.countertop_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.countertop_amount;
            dr["pay_term_date"] = objEstPay.countertop_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.start_job_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "3";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 3;
            dr["term_name"] = objEstPay.start_job_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.start_job_amount;
            dr["pay_term_date"] = objEstPay.startof_job_date;
            tmpTTable.Rows.Add(dr);
        }

        if (objEstPay.final_measure_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "5";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 5;
            dr["term_name"] = objEstPay.final_measure_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.final_measure_amount;
            dr["pay_term_date"] = objEstPay.measure_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.deliver_cabinet_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "6";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 6;
            dr["term_name"] = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.deliver_cabinet_amount;
            dr["pay_term_date"] = objEstPay.delivery_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.substantial_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "7";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 7;
            dr["term_name"] = objEstPay.substantial_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.substantial_amount;
            dr["pay_term_date"] = objEstPay.substantial_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.drywall_amount > 0) //At Start of Drywall 
        {
            dr = tmpTTable.NewRow();
            string strTermId = "8";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 8;
            dr["term_name"] = objEstPay.drywall_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.drywall_amount;
            dr["pay_term_date"] = objEstPay.drywall_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.flooring_amount > 0) //At Start of Flooring
        {
            dr = tmpTTable.NewRow();
            string strTermId = "9";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 9;
            dr["term_name"] = objEstPay.flooring_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = objEstPay.flooring_amount;
            dr["pay_term_date"] = objEstPay.flooring_date;
            tmpTTable.Rows.Add(dr);
        }
        if (objEstPay.other_amount > 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "10";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dAmount = 0;
            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 10;
            dr["term_name"] = "Other: " + objEstPay.other_value.ToString();
            dr["pay_term_amount"] = objEstPay.other_amount;
            dr["pay_term_date"] = objEstPay.other_date;
            tmpTTable.Rows.Add(dr);
        }
        decimal dCreditedCO = 0;
        foreach (COName_Tax cn in item)
        {
            int ncoeid = cn.change_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            CoTaxRate = Convert.ToDecimal(cn.tax);
            decimal dEconCost = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int nCount = Coresult.Count();
            if (Coresult != null && nCount > 0)
                dEconCost = Coresult.Sum();

            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;

            }
            else
            {
                CoPrice = dEconCost;
            }

            if (CoPrice != 0)
            {
                if (CoPrice <= 0)
                {
                    dCreditedCO += CoPrice;
                }

                // CO Payment Term

                string strUponSignValue = string.Empty;
                string strUponCompletionValue = string.Empty;
                string strBalanceDueValue = string.Empty;
                string strOtherValue = string.Empty;

                string strUponSignDate = string.Empty;
                string strUponCompletionDate = string.Empty;
                string strBalanceDueDate = string.Empty;
                string strOtherDate = string.Empty;

                decimal dUponSignAmount = 0;
                decimal dUponCompletionAmount = 0;
                decimal dBalanceDueAmount = 0;
                decimal dOtherAmount = 0;


                if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(cn.change_order_id) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
                {
                    Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(cn.change_order_id) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));

                    if (objPayTerm.UponSign_value != null)
                    {
                        strUponSignValue = objPayTerm.UponSign_value.ToString().Replace("^", "'");
                    }
                    if (objPayTerm.UponCompletion_value != null)
                    {
                        strUponCompletionValue = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
                    }
                    if (objPayTerm.BalanceDue_value != null)
                    {
                        strBalanceDueValue = objPayTerm.BalanceDue_value.Replace("^", "'");
                    }
                    if (objPayTerm.other_value != null)
                    {
                        strOtherValue = objPayTerm.other_value.Replace("^", "'");
                    }

                    dUponSignAmount = Convert.ToDecimal(objPayTerm.UponSign_amount);

                    dUponCompletionAmount = Convert.ToDecimal(objPayTerm.UponCompletion_amount);

                    dBalanceDueAmount = Convert.ToDecimal(objPayTerm.BalanceDue_amount);

                    dOtherAmount = Convert.ToDecimal(objPayTerm.other_amount);

                    strUponSignDate = objPayTerm.UponSign_date;
                    strUponCompletionDate = objPayTerm.UponCompletion_date;
                    strBalanceDueDate = objPayTerm.BalanceDue_date;
                    strOtherDate = objPayTerm.other_date;

                    if (dUponSignAmount != 0)
                    {
                        dr = tmpTTable.NewRow();
                        string strTermId = string.Empty;
                        int nTermId = 1100 + cn.change_order_id;
                        strTermId = nTermId.ToString();
                        string strDate = string.Empty;
                        string TransactionId = string.Empty;
                        string CreditCardNum = string.Empty;
                        string CreditCardType = string.Empty;
                        decimal dAmount = 0;
                        dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
                      

                        if (dv.Count > 0)
                        {
                            for (int i = 0; i < dv.Count; i++)
                            {
                                if (strDate.Length == 0)
                                {
                                    strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = dv[i]["TransactionId"].ToString();
                                    CreditCardNum = dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = dv[i]["CreditCardType"].ToString();
                                }
                                else
                                {
                                    strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                                    CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                                }
                                dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                            }
                        }
                        dr["TransactionId"] = TransactionId;
                        dr["CreditCardNum"] = CreditCardNum;
                        dr["CreditCardType"] = CreditCardType;
                        dr["pay_date"] = strDate;
                        dr["pay_amount"] = dAmount;
                        dr["pay_term_id"] = nTermId;
                        dr["term_name"] = cn.changeorder_name + " (" + strUponSignValue + ": " + dUponSignAmount.ToString("c") + ")";
                        dr["pay_term_amount"] = dUponSignAmount;
                        dr["pay_term_date"] = "";
                        tmpTTable.Rows.Add(dr);

                    }
                    if (dUponCompletionAmount != 0)
                    {

                        dr = tmpTTable.NewRow();
                        string strTermId = string.Empty;
                        int nTermId = 2100 + cn.change_order_id;
                        strTermId = nTermId.ToString();
                        string strDate = string.Empty;
                        string TransactionId = string.Empty;
                        string CreditCardNum = string.Empty;
                        string CreditCardType = string.Empty;
                        decimal dAmount = 0;
                        dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
                       

                        if (dv.Count > 0)
                        {
                            for (int i = 0; i < dv.Count; i++)
                            {
                                if (strDate.Length == 0)
                                {
                                    strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = dv[i]["TransactionId"].ToString();
                                    CreditCardNum = dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = dv[i]["CreditCardType"].ToString();
                                }
                                else
                                {
                                    strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                                    CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                                }
                                dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                            }
                        }
                        dr["TransactionId"] = TransactionId;
                        dr["CreditCardNum"] = CreditCardNum;
                        dr["CreditCardType"] = CreditCardType;
                        dr["pay_date"] = strDate;
                        dr["pay_amount"] = dAmount;
                        dr["pay_term_id"] = nTermId;
                        dr["term_name"] = cn.changeorder_name + " (" + strUponCompletionValue + ": " + dUponCompletionAmount.ToString("c") + ")";
                        dr["pay_term_amount"] = dUponCompletionAmount;
                        dr["pay_term_date"] = "";
                        tmpTTable.Rows.Add(dr);

                    }
                    if (dBalanceDueAmount != 0)
                    {
                        dr = tmpTTable.NewRow();
                        string strTermId = string.Empty;
                        int nTermId = 3100 + cn.change_order_id;
                        strTermId = nTermId.ToString();
                        string strDate = string.Empty;
                        string TransactionId = string.Empty;
                        string CreditCardNum = string.Empty;
                        string CreditCardType = string.Empty;
                        decimal dAmount = 0;
                        dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
                        //if (dv.Count == 0)
                        //{
                        //    nTermId = 100 + cn.change_order_id;
                        //    strTermId = nTermId.ToString();
                        //    dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);

                        //}
                        if (dv.Count > 0)
                        {
                            for (int i = 0; i < dv.Count; i++)
                            {
                                if (strDate.Length == 0)
                                {
                                    strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = dv[i]["TransactionId"].ToString();
                                    CreditCardNum = dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = dv[i]["CreditCardType"].ToString();
                                }
                                else
                                {
                                    strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                                    CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                                }
                                dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                            }
                        }
                        dr["TransactionId"] = TransactionId;
                        dr["CreditCardNum"] = CreditCardNum;
                        dr["CreditCardType"] = CreditCardType;
                        dr["pay_date"] = strDate;
                        dr["pay_amount"] = dAmount;
                        dr["pay_term_id"] = nTermId;
                        dr["term_name"] = cn.changeorder_name + " (" + strBalanceDueValue + ": " + dBalanceDueAmount.ToString("c") + ")";
                        dr["pay_term_amount"] = dBalanceDueAmount;
                        dr["pay_term_date"] = "";
                        tmpTTable.Rows.Add(dr);

                    }
                    if (dOtherAmount != 0)
                    {
                        dr = tmpTTable.NewRow();
                        string strTermId = string.Empty;
                        int nTermId = 4100 + cn.change_order_id;
                        strTermId = nTermId.ToString();
                        string strDate = string.Empty;
                        string TransactionId = string.Empty;
                        string CreditCardNum = string.Empty;
                        string CreditCardType = string.Empty;
                        decimal dAmount = 0;
                        dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
                        //if (dv.Count == 0)
                        //{
                        //    nTermId = 100 + cn.change_order_id;
                        //    strTermId = nTermId.ToString();
                        //    dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);

                        //}
                        if (dv.Count > 0)
                        {
                            for (int i = 0; i < dv.Count; i++)
                            {
                                if (strDate.Length == 0)
                                {
                                    strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = dv[i]["TransactionId"].ToString();
                                    CreditCardNum = dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = dv[i]["CreditCardType"].ToString();
                                }
                                else
                                {
                                    strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                    TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                                    CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                                    CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                                }
                                dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                            }
                        }
                        dr["TransactionId"] = TransactionId;
                        dr["CreditCardNum"] = CreditCardNum;
                        dr["CreditCardType"] = CreditCardType;
                        dr["pay_date"] = strDate;
                        dr["pay_amount"] = dAmount;
                        dr["pay_term_id"] = nTermId;
                        dr["term_name"] = cn.changeorder_name + " (" + strOtherValue + ": " + dOtherAmount.ToString("c") + ")";
                        dr["pay_term_amount"] = dOtherAmount;
                        dr["pay_term_date"] = "";
                        tmpTTable.Rows.Add(dr);

                    }

                }
                else
                {

                    dr = tmpTTable.NewRow();
                    string strTermId = string.Empty;
                    int nTermId = 100 + cn.change_order_id;
                    strTermId = nTermId.ToString();
                    string strDate = string.Empty;
                    string TransactionId = string.Empty;
                    string CreditCardNum = string.Empty;
                    string CreditCardType = string.Empty;
                    decimal dAmount = 0;
                    dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);
                    if (dv.Count == 0)
                    {
                        nTermId = 100 + cn.change_order_id;
                        strTermId = nTermId.ToString();
                        dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);

                    }
                    if (dv.Count > 0)
                    {
                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (strDate.Length == 0)
                            {
                                strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                TransactionId = dv[i]["TransactionId"].ToString();
                                CreditCardNum = dv[i]["CreditCardNum"].ToString();
                                CreditCardType = dv[i]["CreditCardType"].ToString();
                            }
                            else
                            {
                                strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                                TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                                CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                                CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                            }
                            dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                        }
                    }
                    dr["TransactionId"] = TransactionId;
                    dr["CreditCardNum"] = CreditCardNum;
                    dr["CreditCardType"] = CreditCardType;
                    dr["pay_date"] = strDate;
                    dr["pay_amount"] = dAmount;
                    dr["pay_term_id"] = 100 + cn.change_order_id;
                    dr["term_name"] = cn.changeorder_name;
                    dr["pay_term_amount"] = CoPrice;
                    dr["pay_term_date"] = "";
                    tmpTTable.Rows.Add(dr);
                }



            }
        }
        if (objEstPay.due_completion_amount > 0 || dCreditedCO < 0)
        {
            dr = tmpTTable.NewRow();
            string strTermId = "4";
            string strDate = string.Empty;
            string TransactionId = string.Empty;
            string CreditCardNum = string.Empty;
            string CreditCardType = string.Empty;
            decimal dPaytermAmount = Convert.ToDecimal(objEstPay.due_completion_amount) + dCreditedCO;
            decimal dAmount = 0;

            dv.RowFilter = "pay_term_ids='" + strTermId + "' AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value);

            if (dv.Count > 0)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if (strDate.Length == 0)
                    {
                        strDate = Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = dv[i]["TransactionId"].ToString();
                        CreditCardNum = dv[i]["CreditCardNum"].ToString();
                        CreditCardType = dv[i]["CreditCardType"].ToString();
                    }
                    else
                    {
                        strDate = strDate + ", " + Convert.ToDateTime(dv[i]["pay_date"]).ToShortDateString();
                        TransactionId = TransactionId + ", " + dv[i]["TransactionId"].ToString();
                        CreditCardNum = CreditCardNum + ", " + dv[i]["CreditCardNum"].ToString();
                        CreditCardType = CreditCardType + ", " + dv[i]["CreditCardType"].ToString();
                    }
                    dAmount += Convert.ToDecimal(dv[i]["pay_amount"]);
                }
            }
            dr["TransactionId"] = TransactionId;
            dr["CreditCardNum"] = CreditCardNum;
            dr["CreditCardType"] = CreditCardType;
            dr["pay_date"] = strDate;
            dr["pay_amount"] = dAmount;
            dr["pay_term_id"] = 4;
            dr["term_name"] = objEstPay.due_completion_value.Replace("^", "'").ToString();
            dr["pay_term_amount"] = dPaytermAmount;
            dr["pay_term_date"] = objEstPay.due_completion_date;
            tmpTTable.Rows.Add(dr);
        }

        DataView dvSort = tmpTTable.DefaultView;
        dvSort.Sort = "pay_term_id ASC";
        Session["Terms"] = dvSort.ToTable();
        // Session.Add("Terms", tmpTTable);
        dtTerms = (DataTable)Session["Terms"];

        grdPaymentTerm.DataSource = dtTerms;
        grdPaymentTerm.DataKeyNames = new string[] { "pay_term_id", "term_name", "pay_term_amount", "pay_date", "pay_amount", "TransactionId", "CreditCardNum", "CreditCardType" };
        grdPaymentTerm.DataBind();
        //chkPayterm.DataSource = dtTerms;
        //chkPayterm.DataTextField = "term_name";
        //chkPayterm.DataValueField = "pay_term_id";
        //chkPayterm.DataBind();

    }
    private DataTable LoadTermTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("pay_term_id", typeof(int));
        table.Columns.Add("term_name", typeof(string));
        table.Columns.Add("pay_term_amount", typeof(decimal));
        table.Columns.Add("pay_term_date", typeof(string));
        table.Columns.Add("pay_date", typeof(string));
        table.Columns.Add("pay_amount", typeof(decimal));
        table.Columns.Add("TransactionId", typeof(string));
        table.Columns.Add("CreditCardNum", typeof(string));
        table.Columns.Add("CreditCardType", typeof(string));

        return table;
    }
    private void LoadDscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.New_partial_payments
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == Convert.ToInt32(hdnClientId.Value)
                   select new PartialPayment_new()
                   {
                       payment_id = (int)it.payment_id,
                       pay_term_ids = it.pay_term_ids,
                       pay_term_desc = it.pay_term_desc,
                       pay_type_id = (int)it.pay_type_id,
                       client_id = (int)it.client_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       reference = it.reference,
                       pay_amount = (decimal)it.pay_amount,
                       pay_date = (DateTime)it.pay_date,
                       create_date = (DateTime)it.create_date,
                       TransactionId = it.TransactionId,
                       CreditCardNum = it.CreditCardNum,
                       CreditCardType = it.CreditCardType
                   };
        foreach (PartialPayment_new pp in item)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["payment_id"] = pp.payment_id;
            drNew["pay_term_ids"] = pp.pay_term_ids;
            drNew["pay_term_desc"] = pp.pay_term_desc;
            drNew["pay_type_id"] = pp.pay_type_id;
            drNew["client_id"] = pp.client_id;
            drNew["customer_id"] = pp.customer_id;
            drNew["estimate_id"] = pp.estimate_id;
            drNew["reference"] = pp.reference;
            drNew["pay_amount"] = pp.pay_amount;
            drNew["pay_date"] = pp.pay_date;
            drNew["create_date"] = pp.create_date;
            drNew["TransactionId"] = pp.TransactionId;
            drNew["CreditCardNum"] = pp.CreditCardNum;
            drNew["CreditCardType"] = pp.CreditCardType;

            tmpTable.Rows.Add(drNew);
        }

        Session.Add("part_payment", tmpTable);
        //grdPyement.DataSource = tmpTable;
        //grdPyement.DataKeyNames = new string[] { "payment_id", "pay_term_ids", "pay_term_desc", "pay_type_id","TransactionId","CreditCardNum","CreditCardType" };
        //grdPyement.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("payment_id", typeof(int));
        table.Columns.Add("pay_term_ids", typeof(string));
        table.Columns.Add("pay_term_desc", typeof(string));
        table.Columns.Add("pay_type_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("reference", typeof(string));
        table.Columns.Add("pay_amount", typeof(decimal));
        table.Columns.Add("pay_date", typeof(DateTime));
        table.Columns.Add("create_date", typeof(DateTime));
        table.Columns.Add("TransactionId", typeof(string));
        table.Columns.Add("CreditCardNum", typeof(string));
        table.Columns.Add("CreditCardType", typeof(string));

        return table;
    }

    private bool ValidateSecurityDigits()
    {
        // default to true, since user might accidentally turn on the security digits box, when the selected credit card type
        // does not require security digits
        bool isValid = true;

        //string ctype = rdCreditCardType.SelectedValue;
        AuthorizeAPI api = new AuthorizeAPI();
        string ctype = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString(); // credit card type
        int securityDigitLength = txtCVV.Text.Length;

        switch (ctype.ToUpper())
        {
            case "VI":
            case "MC":
            case "DS":
                if (securityDigitLength != 3)
                    isValid = false;
                break;

            case "AX":
                if (securityDigitLength != 4)
                    isValid = false;
                break;
            default:
                isValid = true;
                break;
        }
        return isValid;
    }


    protected void valCheckOut_ServerValidate(object source, ServerValidateEventArgs args)
    {
        valCheckOut.ErrorMessage = "";

        args.IsValid = true;

        if (args.IsValid)
        {
            args.IsValid = false;

            //credit card validation
            AuthorizeAPI api = new AuthorizeAPI();
            string cType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString(); // credit card type
            //string cType = rdCreditCardType.SelectedValue;	// credit card type
            string cNumber = this.txtCreditCardNumber.Text;
            int cLength = cNumber.Length;	// credit card number length

            switch (cType)
            {
                case "VISA":	//visa
                    if (cNumber.StartsWith("4") && (cLength == 13 || cLength == 16))
                        args.IsValid = true;
                    break;

                case "MasterCard":	//master card
                    if (cNumber.StartsWith("5") && cLength == 16)
                        args.IsValid = true;
                    break;

                case "Amex":
                    if ((cNumber.StartsWith("34") || cNumber.StartsWith("37")) && cLength == 15)
                        args.IsValid = true;
                    break;
               

                case "Discover":	//discover
                    if (cNumber.StartsWith("6011") && cLength == 16)
                        args.IsValid = true;
                    break;

                case "DinersClub":	//diner's club
                    if ((cNumber.StartsWith("30") || cNumber.StartsWith("36") || cNumber.StartsWith("38")) &&
                        (cLength == 14 || cLength == 10))
                    {
                        args.IsValid = true;
                    }
                    break;

                case "CB":	//carte blanc
                    if ((cNumber.StartsWith("30") || cNumber.StartsWith("38")) && cLength == 10)
                        args.IsValid = true;
                    break;

                case "TP":	//uatp
                    if (cNumber.StartsWith("1") && cLength == 15)
                        args.IsValid = true;
                    break;

                case "JCB":
                    if ((cNumber.StartsWith("3088") || cNumber.StartsWith("3096") || cNumber.StartsWith("3112") ||
                        cNumber.StartsWith("3158") || cNumber.StartsWith("3337") || cNumber.StartsWith("3528")) && cLength == 16)
                    {
                        args.IsValid = true;
                    }
                    break;

                default:
                    //if (rdCreditCardType.SelectedIndex > 0) args.IsValid = true;
                    break;
            }


            if (args.IsValid == false) valCheckOut.ErrorMessage = "Invalid credit card number for selected card type.";
        }

        // check security digit ONLY when credit card number is valid
        if (args.IsValid)
        {
            args.IsValid = ValidateSecurityDigits();
            if (args.IsValid == false) valCheckOut.ErrorMessage += "<br><li>Invalid security digits.</li>";
        }
        if (ddlMonth.SelectedValue == "0")
        {
            args.IsValid = false;
            valCheckOut.ErrorMessage += "<br><li>Exp. Month is a required field.</li>";
        }
        if ((ddlYear.SelectedIndex == 0) || (Convert.ToInt32(ddlYear.SelectedItem.Text.ToString()) < System.DateTime.Now.Year))
        {
            args.IsValid = false;
            valCheckOut.ErrorMessage += "<br><li>Exp. Year is a required field.</li>";
        }
    }


    private bool _checkCreditCardInformation()
    {
        lblCardResult.Text = "";
        lblCardResult.ForeColor = System.Drawing.Color.Red;



        if (txtCreditCardNumber.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Credit Card Number is a required field");

            return false;
        }
      
        if (ddlMonth.SelectedIndex == 0)
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry month");
            return false;
        }
        if (ddlYear.SelectedIndex == 0)
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry year");
            return false;
        }
        if (txtCVV.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("CVV is a required field");
            return false;
        }
        return true;
    }
    private bool _checkECheckInformation()
    {
        lblCardResult.Text = "";
        lblCardResult.ForeColor = System.Drawing.Color.Red;


        if (txtBank_acct_name.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Bank Account Name is a required field");
            return false;
        }
        if (txtBank_acct_num.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Bank account number is a required field");
            return false;
        }
        if (txtbank_name.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Bank Name is a required field");
            return false;
        }
        if (txtBank_aba_code.Text == "")
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Bank Routing Number is a required field");

            return false;
        }




        return true;
    }


    private string GetttingErrorReason(int nReasonCode)
    {
        string strReson = null;
        if (nReasonCode == 1)
            strReson = "This transaction has been approved.";
        else if (nReasonCode == 2 || nReasonCode == 2 || nReasonCode == 3 || nReasonCode == 4 || nReasonCode == 41 ||
            nReasonCode == 44 || nReasonCode == 45 || nReasonCode == 65 || nReasonCode == 141 || nReasonCode == 145 ||
            nReasonCode == 165 || nReasonCode == 200 || nReasonCode == 201 || nReasonCode == 202 || nReasonCode == 203 ||
            nReasonCode == 204 || nReasonCode == 205 || nReasonCode == 206 || nReasonCode == 206 || nReasonCode == 207 ||
            nReasonCode == 208 || nReasonCode == 209 || nReasonCode == 210 || nReasonCode == 211 || nReasonCode == 212 ||
            nReasonCode == 213 || nReasonCode == 214 || nReasonCode == 215 || nReasonCode == 216 || nReasonCode == 217 ||
            nReasonCode == 218 || nReasonCode == 219 || nReasonCode == 220 || nReasonCode == 221 || nReasonCode == 222 ||
            nReasonCode == 223 || nReasonCode == 224 || nReasonCode == 250 || nReasonCode == 251 || nReasonCode == 254)
            strReson = "This transaction has been declined.";
        else if (nReasonCode == 5)
            strReson = "A valid amount is required.";
        else if (nReasonCode == 6)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 7)
            strReson = "The credit card expiration date is invalid.";
        else if (nReasonCode == 8)
            strReson = "The credit card has expired.";
        else if (nReasonCode == 9)
            strReson = "The ABA code is invalid.";
        else if (nReasonCode == 10)
            strReson = "The account number is invalid.";
        else if (nReasonCode == 11)
            strReson = "A duplicate transaction has been submitted.";
        else if (nReasonCode == 12)
            strReson = "An authorization code is required but not present.";
        else if (nReasonCode == 13)
            strReson = "The merchant API Login ID is invalid or the account is inactive.";
        else if (nReasonCode == 14)
            strReson = "The Referrer or Relay Response URL is invalid.";
        else if (nReasonCode == 15)
            strReson = "The transaction ID is invalid.";
        else if (nReasonCode == 16)
            strReson = "The transaction was not found.";
        else if (nReasonCode == 17)
            strReson = "The merchant does not accept this type of credit card.";
        else if (nReasonCode == 18)
            strReson = "ACH transactions are not accepted by this merchant.";
        else if (nReasonCode == 19 || nReasonCode == 20 || nReasonCode == 21 || nReasonCode == 22 || nReasonCode == 23)
            strReson = "An error occurred during processing. Please try again in 5 minutes.";
        else if (nReasonCode == 24)
            strReson = "The Nova Bank Number or Terminal ID is incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 25 || nReasonCode == 26 || nReasonCode == 57 || nReasonCode == 58 ||
            nReasonCode == 59 || nReasonCode == 60 || nReasonCode == 61 || nReasonCode == 62 || nReasonCode == 63)
            strReson = "An error occurred during processing. Please try again in 5 minutes.";
        else if (nReasonCode == 27)
            strReson = "The transaction resulted in an AVS mismatch. The address provided does not match billing address of cardholder.";
        else if (nReasonCode == 28)
            strReson = "The merchant does not accept this type of credit card.";
        else if (nReasonCode == 29)
            strReson = "The Paymentech identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 30)
            strReson = "The configuration with the processor is invalid. Call Merchant Service Provider.";
        else if (nReasonCode == 31)
            strReson = "The FDC Merchant ID or Terminal ID is incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 32)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 33)
            strReson = "FIELD cannot be left blank.";
        else if (nReasonCode == 34)
            strReson = "The VITAL identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 35)
            strReson = "An error occurred during processing. Call Merchant Service Provider.";
        else if (nReasonCode == 36)
            strReson = "The authorization was approved, but settlement failed.";
        else if (nReasonCode == 33)
            strReson = "FIELD cannot be left blank.";
        else if (nReasonCode == 34)
            strReson = "The VITAL identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 35)
            strReson = "An error occurred during processing. Call Merchant Service Provider.";
        else if (nReasonCode == 37)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 38)
            strReson = "The Global Payment System identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 40)
            strReson = "This transaction must be encrypted.";
        else if (nReasonCode == 43)
            strReson = "The merchant was incorrectly set up at the processor. Call your Merchant Service Provider.";
        else if (nReasonCode == 46)
            strReson = "Your session has expired or does not exist. You must log in to continue working.";
        else if (nReasonCode == 47)
            strReson = "The amount requested for settlement may not be greater than the original amount authorized.";
        else if (nReasonCode == 48)
            strReson = "This processor does not accept partial reversals.";
        else if (nReasonCode == 49)
            strReson = "A transaction amount greater than $[amount] will not be accepted.";
        else if (nReasonCode == 50)
            strReson = "This transaction is awaiting settlement and cannot be refunded.";
        else if (nReasonCode == 51)
            strReson = "The sum of all credits against this transaction is greater than the original transaction amount.";
        else if (nReasonCode == 52)
            strReson = "The transaction was authorized, but the client could not be notified; the transaction will not be settled.";
        else if (nReasonCode == 53)
            strReson = "The transaction type was invalid for ACH transactions.";
        else if (nReasonCode == 54)
            strReson = "The referenced transaction does not meet the criteria for issuing a credit.";
        else if (nReasonCode == 55)
            strReson = "The sum of credits against the referenced transaction would exceed the original debit amount.";
        else if (nReasonCode == 56)
            strReson = "This merchant accepts ACH transactions only; no credit card transactions are accepted.";
        else if (nReasonCode == 66)
            strReson = "This transaction cannot be accepted for processing.";
        else if (nReasonCode == 68)
            strReson = "The version parameter is invalid.";
        else if (nReasonCode == 69)
            strReson = "The transaction type is invalid.";
        else if (nReasonCode == 70)
            strReson = "The transaction method is invalid.";
        else if (nReasonCode == 71)
            strReson = "The bank account type is invalid.";
        else if (nReasonCode == 72)
            strReson = "The authorization code is invalid.";
        else if (nReasonCode == 73)
            strReson = "The drivers license date of birth is invalid.";
        else if (nReasonCode == 74)
            strReson = "The duty amount is invalid.";
        else if (nReasonCode == 75)
            strReson = "The freight amount is invalid.";
        else if (nReasonCode == 76)
            strReson = "The tax amount is invalid.";
        else if (nReasonCode == 77)
            strReson = "The SSN or tax ID is invalid.";
        else if (nReasonCode == 78)
            strReson = "The Card Code (CVV2/CVC2/CID) is invalid.";
        else if (nReasonCode == 79)
            strReson = "The drivers license number is invalid.";
        else if (nReasonCode == 80)
            strReson = "The drivers license state is invalid.";
        else if (nReasonCode == 81)
            strReson = "The requested form type is invalid.";
        else if (nReasonCode == 82)
            strReson = "Scripts are only supported in version 2.5.";
        else if (nReasonCode == 83)
            strReson = "The requested script is either invalid or no longer supported.";
        else if (nReasonCode == 84 || nReasonCode == 85 || nReasonCode == 86 || nReasonCode == 87 || nReasonCode == 88 || nReasonCode == 89 || nReasonCode == 90)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 91)
            strReson = "Version 2.5 is no longer supported.";
        else if (nReasonCode == 92)
            strReson = "The gateway no longer supports the requested method of integration.";
        else if (nReasonCode == 97 || nReasonCode == 98 || nReasonCode == 99)
            strReson = "This transaction cannot be accepted.";
        else if (nReasonCode == 100)
            strReson = "The eCheck.Net type is invalid.";
        else if (nReasonCode == 101)
            strReson = "The given name on the account and/or the account type does not match the actual account.";
        else if (nReasonCode == 102)
            strReson = "This request cannot be accepted.";
        else if (nReasonCode == 103)
            strReson = "This transaction cannot be accepted.";
        else if (nReasonCode == 104 || nReasonCode == 105 || nReasonCode == 106 || nReasonCode == 107 || nReasonCode == 108 || nReasonCode == 109 || nReasonCode == 110)
            strReson = "This transaction is currently under review.";
        else if (nReasonCode == 116)
            strReson = "The authentication indicator is invalid.";
        else if (nReasonCode == 117)
            strReson = "The cardholder authentication value is invalid.";
        else if (nReasonCode == 118)
            strReson = "The combination of authentication indicator and cardholder authentication value is invalid.";
        else if (nReasonCode == 119)
            strReson = "Transactions having cardholder authentication values cannot be marked as recurring.";
        else if (nReasonCode == 120 || nReasonCode == 121 || nReasonCode == 122)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 123)
            strReson = "This account has not been given the permission(s) required for this request.";
        else if (nReasonCode == 127)
            strReson = "The transaction resulted in an AVS mismatch. The address provided does not match billing address of cardholder.";
        else if (nReasonCode == 128)
            strReson = "This transaction cannot be processed.";
        else if (nReasonCode == 130)
            strReson = "This payment gateway account has been closed.";
        else if (nReasonCode == 131 || nReasonCode == 132)
            strReson = "This transaction cannot be accepted at this time.";
        else if (nReasonCode == 152)
            strReson = "The transaction was authorized, but the client could not be notified; the transaction will not be settled.";
        else if (nReasonCode == 170 || nReasonCode == 171 || nReasonCode == 172 || nReasonCode == 173)
            strReson = "An error occurred during processing. Please contact the merchant.";
        else if (nReasonCode == 174)
            strReson = "The transaction type is invalid. Please contact the merchant.";
        else if (nReasonCode == 175)
            strReson = "The processor does not allow voiding of credits.";
        else if (nReasonCode == 180 || nReasonCode == 181)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 185)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 193)
            strReson = "The transaction is currently under review.";
        else if (nReasonCode == 243)
            strReson = "Recurring billing is not allowed for this eCheck.Net type.";
        else if (nReasonCode == 244)
            strReson = "This eCheck.Net type is not allowed for this Bank Account Type.";
        else if (nReasonCode == 245)
            strReson = "This eCheck.Net type is not allowed when using the payment gateway hosted payment form.";
        else if (nReasonCode == 247 || nReasonCode == 246)
            strReson = "This eCheck.Net type is not allowed.";
        else if (nReasonCode == 252 || nReasonCode == 253)
            strReson = "Your order has been received. Thank you for your business!";
        else if (nReasonCode == 261)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 270)
            strReson = "The line item [item number] is invalid.";
        else if (nReasonCode == 271)
            strReson = "The number of line items submitted is not allowed. A maximum of 30 line items can be submitted.";
        else if (nReasonCode == 315)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 316)
            strReson = "The credit card expiration date is invalid.";
        else if (nReasonCode == 317)
            strReson = "The credit card has expired.";
        else if (nReasonCode == 318)
            strReson = "A duplicate transaction has been submitted.";
        else if (nReasonCode == 318)
            strReson = "The transaction cannot be found.";
        return strReson;

    }

    protected void btnFinalizePayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnFinalizePayment.ID, btnFinalizePayment.GetType().Name, "Click"); 
        PaymentProfile payPro = new PaymentProfile();

        AuthorizeAPI api = new AuthorizeAPI();

        lblCardResult.Text = "";
        lblReason.Text = "";
        bool cCheck = false;
        bool bFound = false;
        decimal nAmount = 0;
        string strPayTermId = hdnPayTermId.Value;
        string strPayTermDesc = hdnPayTerm.Value;
        string sTranId = "";
        if (txtCardHolderName.Text=="")
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is a required field.");
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is a required field.");
            txtCardHolderName.Focus();
            return;
        }
        if (txtCardHolderName.Text.Trim().Length>20)
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is allowed maximum 20 characters.");
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is allowed maximum 20 characters.");
            txtCardHolderName.Focus();
            return;
        }
        if (txtAmount.Text.Trim().Length == 0)
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount is a required field.");

            return;
        }
        else
        {

            try
            {
                nAmount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount must be number.");
                return;

            }
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        valCheckOut.Visible = true;

        foreach (GridViewRow item in grdCardList.Rows)
        {
            RadioButton rdo = (RadioButton)item.FindControl("rdoSelect");
            if (rdo.Checked)
            {
                bFound = true;
                int nPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[item.RowIndex].Values[0]);
                int nCustomerId = Convert.ToInt32(grdCardList.DataKeys[item.RowIndex].Values[1]);

                payPro = _db.PaymentProfiles.SingleOrDefault(p => p.PaymentProfileId == nPaymentProfileId && p.CustomerId == nCustomerId);

                if (payPro != null)
                {
                    try
                    {

                        sTranId = api.ChargeCustomerProfile(payPro.AuthorisedCustomerId.ToString(), payPro.AuthorisedPaymentId.ToString(), nAmount);
                        if (sTranId.Length > 0)
                        {
                            cCheck = true;

                        }
                    }
                    catch (Exception ex)
                    {
                        lblCardResult.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                        lblReason.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                        return;

                    }
                }
            }
        }


        if (bFound == false)
        {
            if (!_checkCreditCardInformation())
                return;

            if (valCheckOut.IsValid == true)
            {
                try
                {
                    customer objcust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    string sOrderNo = DateTime.Now.ToString("yyyyMMddHHmmssff"); //CommonUtility.GetOrderNumber();


                    decimal total_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", string.Empty));


                   

                    sTranId = api.ChargeCreditCard(txtCreditCardNumber.Text.Trim(), ddlMonth.SelectedItem.Text.Trim() + ddlYear.SelectedValue.Trim(), txtCVV.Text, total_amount, txtCardHolderName.Text.Trim(), objcust.email, txtAddress.Text, txtCity.Text, ddlState.SelectedItem.Text, txtZip.Text);

                    if (sTranId.Length > 0)
                    {
                        cCheck = true;

                    }

                }
                catch (Exception ex)
                {
                    lblCardResult.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                    lblReason.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                    return;

                }
            }
            else
            {
                //valSummary.Visible = true;
                valCheckOut.Text = "";
            }

        }


        if (cCheck)
        {
            try
            {

                // Insert Customer

                string strOrderNote = string.Empty;

                string strServiceName = string.Empty;

                string strCCN = txtCreditCardNumber.Text.Trim().ToString();

                string BillAddress = string.Empty;
                string BillCity = string.Empty;
                string BillState = string.Empty;
                string BillZip = string.Empty;

                string strCardType = string.Empty;

                if (strCCN.Length > 3)
                {
                    strCCN = strCCN.Substring((strCCN.Length - 4), 4);
                    strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
                    BillAddress = txtAddress.Text;
                    BillCity = txtCity.Text;
                    BillState = ddlState.SelectedValue;
                    BillZip = txtZip.Text;

                }
                else
                {
                    strCCN = payPro.CardNumber.ToString();
                    BillAddress = payPro.BillAddress;
                    BillCity = payPro.BillCity;
                    BillState = payPro.BillState;
                    BillZip = payPro.BillZip;
                    strCardType = payPro.CardType;
                }
                New_partial_payment pay_cost = new New_partial_payment();

                // pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);

                pay_cost.pay_term_ids = strPayTermId;
                pay_cost.pay_term_desc = strPayTermDesc;
                pay_cost.client_id = Convert.ToInt32(hdnClientId.Value);
                pay_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                pay_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                pay_cost.pay_type_id = 3;
                pay_cost.reference = strCCN;
                pay_cost.pay_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")); ;
                pay_cost.pay_date = DateTime.Now;
                pay_cost.create_date = DateTime.Now;
                pay_cost.TransactionId = sTranId;
                pay_cost.CreditCardType = strCardType;
                pay_cost.CreditCardNum = strCCN;
                _db.New_partial_payments.InsertOnSubmit(pay_cost);
                _db.SubmitChanges();
                int payId = pay_cost.payment_id;
                hdnPayId.Value = payId.ToString();
                // Send Email to New Customer
                //Email.SendMailtoCustomer( Convert.ToInt32(hdnCustomerId.Value));



                // Insert Payment
                Payments_card_Info objPay = new Payments_card_Info();

                objPay.PaymentMethodID = 0; //rdCreditCardType.SelectedIndex + 1;
                objPay.payment_id = payId;
                objPay.Date = DateTime.Now;
                objPay.CreditCardNum = strCCN;

                if (chkNewCard.Checked)
                {
                    objPay.CardHoldersName = txtCardHolderName.Text;

                    objPay.CreditCardExpDate = Convert.ToDateTime(ddlMonth.SelectedValue + "/1/" + ddlYear.SelectedItem.Text);
                }
                else
                {
                    objPay.CardHoldersName = payPro.NameOnCard;

                    objPay.CreditCardExpDate = payPro.ExpirationDate;

                }

                objPay.CreditCardAuthNum = "";
                objPay.Amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                objPay.Notes = "";
                objPay.Message = "";
                objPay.Approval = "";
                objPay.IsEmailSent = false;
                objPay.TransactionId = sTranId;
                objPay.BillAddress = BillAddress;
                objPay.BillCity = BillCity;
                objPay.BillState = BillState;
                objPay.BillZip = BillZip;
                objPay.client_id = Convert.ToInt32(hdnClientId.Value);
                _db.Payments_card_Infos.InsertOnSubmit(objPay);
                _db.SubmitChanges();

                //Save card info after transaction

                if (_db.PaymentProfiles.Where(pp => pp.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && pp.NameOnCard == objPay.CardHoldersName && pp.CardNumber == objPay.CreditCardNum && pp.CardType == strCardType).Count() == 0)
                {
                    if (chkSaveCardInfo.Checked)
                    {
                        SaveCustomerProfile(Convert.ToInt32(hdnPayId.Value));

                    }
                }
                 SendEmailToCustomer(Convert.ToInt32(hdnCustomerId.Value), Convert.ToDecimal(txtAmount.Text.Replace("$", "")), strPayTermDesc, strCardType, strCCN);

                Calculate();
                LoadDscription();
                LoadTerms();

                GetCardLists();

                if (grdCardList.Rows.Count > 0)
                {
                    pnlExistCard.Visible = true;
                    pnlNewCard.Visible = false;
                    chkNewCard.Visible = true;
                    chkNewCard.Checked = false;
                }
                else
                {
                    pnlExistCard.Visible = false;
                    pnlNewCard.Visible = true;
                    chkNewCard.Checked = true;
                    chkNewCard.Visible = false;
                }
                txtAmount.Text = "";
                lblPayterm.Text = "";
                txtCreditCardNumber.Text = "";
                txtCVV.Text = "";
                ddlMonth.SelectedIndex = 0;
                ddlYear.SelectedIndex = 0;
                tblNewPayment.Visible = false;
                lblReason.Text = csCommonUtility.GetSystemMessage("This payment has been submitted successfully");
                lblCardResult.Text = csCommonUtility.GetSystemMessage("This payment has been submitted successfully");
            }
            catch (Exception ex)
            {
                Session.RemoveAll();
                Session.Clear();
                lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }

        }
        else
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage("Transaction fail...." + Environment.NewLine + lblReason.Text + "");

        }
    }


    private void SaveCustomerProfile(int nPaymentId)
    {

        try
        {
            AuthorizeAPI api = new AuthorizeAPI();
            DataClassesDataContext _db = new DataClassesDataContext();

            PaymentProfile objPP = new PaymentProfile();


            Payments_card_Info cardInfo = _db.Payments_card_Infos.Single(p => p.payment_id == nPaymentId);

            int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            string nCardNumber = cardInfo.CreditCardNum;
            string strNameOnCard = cardInfo.CardHoldersName;
            string strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
            string strMonth = ddlMonth.SelectedItem.Text.Trim();
            string strYear = ddlYear.SelectedItem.Text.Trim();

            if (_db.PaymentProfiles.Where(pp => pp.CustomerId == nCustomerId && pp.NameOnCard == strNameOnCard && pp.CardNumber == nCardNumber && pp.CardType == strCardType).Count() > 0)
            {
                return;
            }

            objPP.CustomerId = nCustomerId;
            objPP.AuthorisedPaymentId = "0";
            objPP.AuthorisedCustomerId = 0;
            objPP.CardNumber = nCardNumber;
            objPP.NameOnCard = strNameOnCard;
            objPP.CardType = strCardType;
            objPP.ExpirationDate = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1);
            objPP.CreateDate = DateTime.Now;
            objPP.LastUpdatedDate = DateTime.Now;
            objPP.LastUpdatedBy = "";// hdnCustomerLastName.Value.ToString();
            objPP.client_id = Convert.ToInt32(hdnClientId.Value);
            objPP.BillAddress = txtAddress.Text;
            objPP.BillCity = txtCity.Text;
            objPP.BillState = ddlState.SelectedValue;
            objPP.BillZip = txtZip.Text;

            List<string> sArray = api.CreateCustomerProfileFromTransaction(cardInfo.TransactionId);

            if (sArray.Count > 1)
            {
                objPP.AuthorisedCustomerId = Convert.ToInt32(sArray[0]);
                objPP.AuthorisedPaymentId = sArray[1];
                _db.PaymentProfiles.InsertOnSubmit(objPP);
                _db.SubmitChanges();
            }
            txtCreditCardNumber.Text = "";
            ddlYear.SelectedIndex = -1;
            ddlMonth.SelectedIndex = -1;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }





    private void GetCardLists()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();

        try
        {
            var item = from pp in _db.PaymentProfiles
                       where pp.CustomerId == Convert.ToInt32(hdnCustomerId.Value)
                       select new csCreditCard
                       {
                           PaymentProfileId = (int)pp.PaymentProfileId,
                           CustomerId = (int)pp.CustomerId,
                           AuthorisedPaymentId = pp.AuthorisedPaymentId,
                           AuthorisedCustomerId = (int)pp.AuthorisedCustomerId,
                           CardNumber = pp.CardNumber,
                           NameOnCard = pp.NameOnCard,
                           CardType = pp.CardType,
                           ExpirationDate = (DateTime)pp.ExpirationDate,
                           CreateDate = (DateTime)pp.CreateDate,
                           LastUpdatedDate = (DateTime)pp.LastUpdatedDate,
                           LastUpdatedBy = (string)pp.LastUpdatedBy
                       };

            if (item.Count() > 0)
            {
                grdCardList.DataSource = item;
                grdCardList.DataKeyNames = new string[] { "PaymentProfileId", "CustomerId", "CardNumber", "CardType" };
                grdCardList.DataBind();

                DataTable dt = csCommonUtility.LINQToDataTable(item);
                Session.Add("sCardList", dt);
            }
        }
        catch (Exception ex)
        {
            lblCardResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdCardList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCardList.ID, grdCardList.GetType().Name, "Sorting"); 
        lblCardResult.Text = "";
        DataTable dtCardList = (DataTable)Session["sCardList"];

        string strShort = e.SortExpression + " " + hdnOrder.Value;

        DataView dv = dtCardList.DefaultView;
        dv.Sort = strShort;
        Session["sCardList"] = dv.ToTable();

        if (hdnOrder.Value == "ASC")
            hdnOrder.Value = "DESC";
        else
            hdnOrder.Value = "ASC";
        GetCardLists();
    }
    protected void grdCardList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[0]);
            int nCustomerId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[1]);
            string nCardNumber = grdCardList.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strCardType = grdCardList.DataKeys[e.Row.RowIndex].Values[3].ToString();

            System.Web.UI.WebControls.Image imggrdCardType = (System.Web.UI.WebControls.Image)e.Row.FindControl("imggrdCardType");
            imggrdCardType.ImageUrl = "~/Images/" + strCardType.ToUpper() + ".png";
            imggrdCardType.AlternateText = strCardType;

            Label lblgrdCreditCard = (Label)e.Row.FindControl("lblgrdCreditCard");
            lblgrdCreditCard.Text = strCardType + " ending in " + nCardNumber;

            Label lblgrdExpirationDatee = (Label)e.Row.FindControl("lblgrdExpirationDatee");
            if (DateTime.Now > Convert.ToDateTime(lblgrdExpirationDatee.Text))
            {
                lblgrdExpirationDatee.Text = "Expired <br/>" + Convert.ToDateTime(lblgrdExpirationDatee.Text).ToString("MM/yyyy");
                lblgrdExpirationDatee.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblgrdExpirationDatee.Text = Convert.ToDateTime(lblgrdExpirationDatee.Text).ToString("MM/yyyy");
                lblgrdExpirationDatee.ForeColor = System.Drawing.Color.Black;
            }

        }
    }

    protected void chkNewCard_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkNewCard.ID, chkNewCard.GetType().Name, "CheckedChanged"); 
        if (chkNewCard.Checked)
        {
            btnEcheckPayment.Visible = false;
            btnFinalizePayment.Visible = true;

            //   chkPaymentECheck.Checked = false;
            pnlECheckPayment.Visible = false;
            pnlNewCard.Visible = true;
            if (grdCardList.Rows.Count > 0)
            {
                foreach (GridViewRow di in grdCardList.Rows)
                {
                    RadioButton rdoSelect = (RadioButton)di.FindControl("rdoSelect");
                    if (rdoSelect.Checked)
                    {
                        rdoSelect.Checked = false;
                    }

                }


            }
        }
        else
        {
            pnlNewCard.Visible = false;
            
        }

    }
   
    protected void grdPaymentTerm_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string strId = string.Empty;
            int npid = Convert.ToInt32(grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[0].ToString());
            string termName = grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[1].ToString();
            decimal pay_term_amount = Convert.ToDecimal(grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[2].ToString());

            string PayDate = grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[3].ToString();
            decimal PayAmount = Convert.ToDecimal(grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[4].ToString());
            string TransactionId = grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[5].ToString();
            string CreditCardNum = grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string CreditCardType = grdPaymentTerm.DataKeys[e.Row.RowIndex].Values[7].ToString();


            decimal dueBalance = pay_term_amount - PayAmount;

            Label lblPayInfo = (Label)e.Row.FindControl("lblPayInfo");
            Label lblCardInfo = (Label)e.Row.FindControl("lblCardInfo");

            if (pay_term_amount < 0)
            {
                lblPayInfo.Text = "Credited/ adjusted";
                if (PayAmount > 0)
                {
                    lblPayInfo.Text = "Paid: " + PayAmount.ToString("c") + " on " + PayDate;
                }
            }
            else
            {
                lblPayInfo.Text = "Paid: " + PayAmount.ToString("c") + " on " + PayDate;
            }

            if (TransactionId.Length > 5)
            {
                lblCardInfo.Text = CreditCardType + " " + CreditCardNum + " ID: " + TransactionId;
            }
            else
            {
                lblCardInfo.Text = "";
            }



            LinkButton lnkPayment = (LinkButton)e.Row.FindControl("lnkPayment");
            lnkPayment.CausesValidation = false;

            strId = npid.ToString() + "_" + termName + "_" + pay_term_amount;
            lnkPayment.Text = "Make a payment";
            lnkPayment.CommandArgument = strId;

            if (PayAmount >= pay_term_amount)
            {
                lblPayInfo.Visible = true;
                lnkPayment.Visible = false;
                lblPayInfo.ForeColor = Color.Green;
            }
            else if (PayAmount > 0 && dueBalance > 0)
            {
                if (dueBalance < 1)
                {
                    lblPayInfo.Text = "Paid: " + PayAmount.ToString("c") + " on " + PayDate;
                    lnkPayment.Visible = false;
                    lblPayInfo.ForeColor = Color.Green;

                }
                else
                {
                    lblPayInfo.Visible = true;
                    lnkPayment.Visible = true;

                    lblPayInfo.ForeColor = Color.Red;
                    strId = npid.ToString() + "_" + termName + "_" + dueBalance;
                    lnkPayment.Text = "Pay " + dueBalance.ToString("c");
                    lnkPayment.CommandArgument = strId;
                }
            }
            else
            {
                lblPayInfo.Visible = false;
                lnkPayment.Visible = true;
                decimal ndue = Convert.ToDecimal(lblTotalBalanceAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                if (ndue < 1)
                {
                    if (lnkPayment.Text == "Make a payment")
                    {
                        lblPayInfo.Text = "Paid/ adjusted";
                        lblPayInfo.ForeColor = Color.Green;
                        lblPayInfo.Visible = true;
                        lnkPayment.Visible = false;
                    }

                }
            }

        }
    }
    protected void CreatePayment(object sender, EventArgs e)
    {

        LinkButton lnkPayment = (LinkButton)sender;
        string str = lnkPayment.CommandArgument.ToString();
        string[] strIds = str.Split('_');

        int nPayTermId = 0;
        string PayTerm = string.Empty;
        decimal pay_term_amount = 0;

        foreach (string strId in strIds)
        {
            if (nPayTermId == 0)
                nPayTermId = Convert.ToInt32(strId);
            else if (PayTerm.Length == 0)
                PayTerm = strId;
            else if (pay_term_amount == 0)
                pay_term_amount = Convert.ToDecimal(strId);

        }
        hdnPayTermId.Value = nPayTermId.ToString();
        hdnPayTerm.Value = PayTerm;
        tblNewPayment.Visible = true;
        lblPayterm.Text = PayTerm;
        txtAmount.Text = pay_term_amount.ToString("c");
        lblamount.Text = pay_term_amount.ToString("c");
        lblamount1.Text = pay_term_amount.ToString("c");
        lblCardResult.Text = "";
        lblReason.Text = "";


    }

    string body(string fullName, string amount, string strTerms, string strPMName, string strCCNum, string companyName, string website)
    {




        string body = @"<div marginwidth='0' marginheight='0' style='font:14px/20px 'Helvetica',Arial,sans-serif;margin:0;padding:75px 0 0 0;text-align:center;background-color:#330f02'>
    <center>
       <table border='0' cellpadding='20' cellspacing='0' height='100%' width='100%' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624bodyTable' style='background-color:#330f02'>
          <tbody>
             <tr>
                <td align='center' valign='top'>
                   <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;border-radius:6px;background-color:none' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624templateContainer'>
                      <tbody>
                         <tr>
                            <td align='center' valign='top'>
                               <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px'>
                                  <tbody>
                                     <tr>
                                        <td style='margin: 0 auto; text-align: center'>
                                           <h1 style='font-size:24px;line-height:100%;margin-bottom:30px;margin-top:0;padding:0'>
                                             
                                                <img src='https://ii.faztrack.com/assets/II_EMAIL_LOGO.png' alt='' border='0' width='50%'  ></a>
                                           </h1>
                                        </td>
                                     </tr>
                                  </tbody>
                               </table>
                            </td>
                         </tr>
                         <tr>
                            <td align='center' valign='top'>
                               <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;border-radius:6px;background-color:#d6dfdb' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624templateBody'>
                                  <tbody>
                                     <tr>
                                        <td align='left' valign='top' style='line-height:110%;font-family:Verdana;font-size:14px;color:#333333;padding:20px'>
                                           <div>Dear " + fullName + ",</div> " + Environment.NewLine + Environment.NewLine +
                                           "<p style='line-height:20px'>Your payment of <b>" + amount + "</b> for <b>" + strTerms + "</b> using your <b>" + strPMName + "</b> ending in <b>" + strCCNum + "</b> was processed.</p>" + Environment.NewLine + Environment.NewLine +
                                           "<p>Please click <a target='_blank' href='" + csCommonUtility.GetProjectUrl() + "/customerlogin.aspx'> here </a> to view your current project status.</p>" + Environment.NewLine +
                                           "<p>Should you have any question please do not hesitate to contact us.</p>" + Environment.NewLine + Environment.NewLine;


        body += @"<p>Sincerely,</p>" + Environment.NewLine +
                "<p>" + companyName + "</p>" + Environment.NewLine +
                "<p>" + website + "</p>";

        body += @"</td>
                                         </tr>
                                      </tbody>
                                   </table>
                                </td>
                             </tr>
                             <tr>
                                <td align='center' valign='top'>
                                   <table border='0' cellpadding='20' cellspacing='0' width='100%' style='max-width:600px'>
                                      <tbody>
                                         <tr>
                                            <td align='center' valign='top'>
                                            </td>
                                         </tr>
                                      </tbody>
                                   </table>
                                </td>
                             </tr>
                          </tbody>
                       </table>
                    </td>
                 </tr>
              </tbody>
           </table>
        </center>
    
     </div>";

        return body;


    }
    private void SendEmailToCustomer(int nCustomerId, decimal amount, string strTerms, string strPMName, string strCCNum)
    {
        // Email To Group
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
            customeruserinfo objcu = new customeruserinfo();
            objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

            company_profile oCom = new company_profile();
            oCom = _db.company_profiles.Single(c => c.client_id == Convert.ToInt32(hdnClientId.Value));

            string strTable = "";
            strTable = body(objCust.first_name1 + " " + objCust.last_name1, Convert.ToDecimal(amount).ToString("c"), strTerms, strPMName, strCCNum, oCom.company_name, oCom.website);

            

            string strToEmail = objCust.email;
            string FromEmail = oCom.email;

            string TransactionEmail = oCom.co_email; // using as Transaction related Emails

            string strCCEmail = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (TransactionEmail.Length > 4)
            {
                string[] strCCIds = TransactionEmail.Split(',');
                foreach (string strCCId in strCCIds)
                {
                    Match match1 = regex.Match(strCCId.Trim());
                    if (match1.Success)
                    {
                        strCCEmail += strCCId + ",";
                    }
                }
            }
            strCCEmail = strCCEmail.TrimEnd(',');

           

            Match match = regex.Match(strToEmail);
            if (!match.Success)
                strToEmail = "";

            if (strToEmail.Length > 4)
            {
                EmailAPI email = new EmailAPI();
                customeruserinfo obj = new customeruserinfo();
                string strUser = "";
                string strFrom = "alyons@azinteriorinnovations.com";
                int ProtocolType = 1;

                if ((customeruserinfo)Session["oCustomerUser"] != null)
                {
                    obj = (customeruserinfo)Session["oCustomerUser"];
                    strUser = obj.customerusername;
                }

                email.From = strFrom;

                email.To = strToEmail.Trim();

                email.BCC = "";
                email.CC = strCCEmail;

                email.Subject = "Your payment with Arizona's Interior Innovations";

                email.Body = strTable;

                email.UserName = strUser;

                email.IsSaveEmailInDB = false;

                email.ProtocolType = ProtocolType;

                email.SendEmail();

            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            return;
        }
    }
    private void SendEcheckEmailToCustomer(int nCustomerId, decimal amount, string strTerms, string strAccountNum, string strBankNamae)
    {
        // Email To Group
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
            customeruserinfo objcu = new customeruserinfo();
            objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

            company_profile oCom = new company_profile();
            oCom = _db.company_profiles.Single(c => c.client_id == Convert.ToInt32(hdnClientId.Value));

            string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                    "<tr><td align='left'>Dear " + objCust.first_name1 + " " + objCust.last_name1 + ",</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>Your payment of  <b>" + Convert.ToDecimal(amount).ToString("c") + "</b> for <b>" + strTerms + "</b> using your bank account of <b>" + strBankNamae + "</b> bank, account number ending in <b>" + strAccountNum + "</b> was processed.</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>Please click <a target='_blank' href='https://ii.faztrack.com/customerlogin.aspx'> here </a> to view your current project status.</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>Should you have any question please do not hesitate to contact us.</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>" + oCom.company_name + "</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'>" + oCom.website + "</td></tr>" + Environment.NewLine +
                    "<tr><td align='left'></td></tr></table>";

            string strToEmail = objCust.email;
            string FromEmail = oCom.email;

            string TransactionEmail = oCom.co_email; // using as Transaction related Emails

            string strCCEmail = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (TransactionEmail.Length > 4)
            {
                string[] strCCIds = TransactionEmail.Split(',');
                foreach (string strCCId in strCCIds)
                {
                    Match match1 = regex.Match(strCCId.Trim());
                    if (match1.Success)
                    {
                        strCCEmail += strCCId + ",";

                    }
                }
            }
            strCCEmail = strCCEmail.TrimEnd(',');

            //if (strCCEmail.Length > 4)
            //{
            //    string[] strCCIds = strCCEmail.Split(',');
            //    foreach (string strCCId in strCCIds)
            //    {
            //        Match match1 = regex.Match(strCCId.Trim());
            //        if (!match1.Success)
            //        {
            //            strCCEmail = "alyons@azinteriorinnovations.com, sroman@interiorinnovations.biz, trohlik@interiorinnovations.biz";

            //        }
            //    }
            //}
            //else
            //{
            //    strCCEmail = "alyons@azinteriorinnovations.com, sroman@interiorinnovations.biz, trohlik@interiorinnovations.biz";

            //}

            Match match = regex.Match(strToEmail);
            if (!match.Success)
                strToEmail = "";

            if (strToEmail.Length > 4)
            {

                EmailAPI email = new EmailAPI();
                customeruserinfo obj = new customeruserinfo();
                string strUser = "";
                string strFrom = "alyons@azinteriorinnovations.com";
                int ProtocolType = 1;

                if ((customeruserinfo)Session["oCustomerUser"] != null)
                {
                    obj = (customeruserinfo)Session["oCustomerUser"];
                    strUser = obj.customerusername;
                }

                email.From = strFrom;

                email.To = strToEmail.Trim();

                email.BCC = strCCEmail;

                email.Subject = "Your payment with Arizona's Interior Innovations";

                email.Body = strTable;

                email.UserName = strUser;

                email.IsSaveEmailInDB = false;

                email.ProtocolType = ProtocolType;

                email.SendEmail();

            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            return;
        }
    }
    protected void txtCreditCardNumber_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtCreditCardNumber.ID, txtCreditCardNumber.GetType().Name, "TextChanged"); 
        lblCardResult.Text = "";
        if (txtCreditCardNumber.Text.Trim().Length > 5)
        {
            AuthorizeAPI api = new AuthorizeAPI();
            string strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
            imgCardType.ImageUrl = "~/Images/" + strCardType.ToUpper() + ".png";
            imgCardType.AlternateText = strCardType;

           
            chkSaveCardInfo.Visible = true;
        }
        else
        {
            chkSaveCardInfo.Visible = false;
        }
    }
    protected void txtAmount_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtAmount.ID, txtAmount.GetType().Name, "TextChanged"); 
        string str = txtAmount.Text.Replace("$", "");
        lblamount.Text = "$" + str;
        lblamount1.Text = "$" + str;
    }

    protected void btnEcheckPayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnEcheckPayment.ID, btnEcheckPayment.GetType().Name, "Click"); 
        PaymentProfile payPro = new PaymentProfile();

        lblCardResult.Text = "";
        lblReason.Text = "";
        bool cCheck = false;
        decimal nAmount = 0;
        string strPayTermId = hdnPayTermId.Value;
        string strPayTermDesc = hdnPayTerm.Value;
        string sTranId = "";


        if (txtAmount.Text.Trim().Length == 0)
        {
            lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount is a required field.");

            return;
        }
        else
        {

            try
            {
                nAmount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                lblCardResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount must be number.");
                return;

            }
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        //  if (chkPaymentECheck.Checked == true)
        if (rdbPaymentOption.SelectedItem.Value == "2")
        {
            if (!_checkECheckInformation())
                return;


            try
            {
                customer objcust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                string sOrderNo = DateTime.Now.ToString("yyyyMMddHHmmssff"); //CommonUtility.GetOrderNumber();


                decimal total_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", string.Empty));

                AuthorizeAPI api = new AuthorizeAPI();

                sTranId = api.ChargeECheck(txtBank_acct_num.Text.Trim(), txtBank_aba_code.Text.Trim(), txtBank_acct_name.Text.Trim(), txtbank_name.Text.Trim(), nAmount);
                if (sTranId.Length > 0)
                {
                    cCheck = true;

                }


            }
            catch (Exception ex)
            {
                lblCardResult.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                lblReason.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                return;

            }
        }


        if (cCheck)
        {
            try
            {

                New_partial_payment pay_cost = new New_partial_payment();

                // pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);

                pay_cost.pay_term_ids = strPayTermId;
                pay_cost.pay_term_desc = strPayTermDesc;
                pay_cost.client_id = Convert.ToInt32(hdnClientId.Value);
                pay_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                pay_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                pay_cost.pay_type_id = 2;
                pay_cost.reference = "Check";
                pay_cost.pay_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")); ;
                pay_cost.pay_date = DateTime.Now;
                pay_cost.create_date = DateTime.Now;
                pay_cost.TransactionId = sTranId;
                pay_cost.CreditCardType = "";
                pay_cost.CreditCardNum = "";
                _db.New_partial_payments.InsertOnSubmit(pay_cost);
                _db.SubmitChanges();
                int payId = pay_cost.payment_id;
                hdnPayId.Value = payId.ToString();


                // Insert Payment
                PaymentECheckInfo objEPay = new PaymentECheckInfo();

                objEPay.payment_id = payId;
                objEPay.Date = DateTime.Now;
                objEPay.AccountNumber = txtBank_acct_num.Text.Trim();
                objEPay.RoutingNumber = txtBank_aba_code.Text.Trim();
                objEPay.Amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                objEPay.AccountType = "Checking";
                objEPay.EcheckType = "WEB";
                objEPay.NameOnAccount = txtBank_acct_name.Text.Trim();
                objEPay.BankName = txtbank_name.Text.Trim();
                objEPay.client_id = Convert.ToInt32(hdnClientId.Value);

                _db.PaymentECheckInfos.InsertOnSubmit(objEPay);
                _db.SubmitChanges();
                string strAccountNum = txtBank_acct_num.Text.Trim().Substring((txtBank_acct_num.Text.Trim().Length - 3), 3);

                SendEcheckEmailToCustomer(Convert.ToInt32(hdnCustomerId.Value), Convert.ToDecimal(txtAmount.Text.Replace("$", "")), strPayTermDesc, strAccountNum, txtbank_name.Text.Trim());

                Calculate();
                LoadDscription();
                LoadTerms();

                txtAmount.Text = "";
                lblPayterm.Text = "";
                txtBank_acct_num.Text = "";
                txtBank_aba_code.Text = "";
                txtBank_acct_name.Text = "";
                txtbank_name.Text = "";

                pnlECheckPayment.Visible = false;
                lblReason.Text = csCommonUtility.GetSystemMessage("This payment has been submitted successfully");
                lblCardResult.Text = csCommonUtility.GetSystemMessage("This payment has been submitted successfully");
            }
            catch (Exception ex)
            {
                Session.RemoveAll();
                Session.Clear();
                lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }

        }
        else
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage("Transaction fail...." + Environment.NewLine + lblReason.Text + "");

        }



    }
    protected void rdbPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdbPaymentOption.ID, rdbPaymentOption.GetType().Name, "SelectedIndexChanged"); 
        if (rdbPaymentOption.SelectedItem.Value == "2")// E-Check
        {
            btnEcheckPayment.Visible = true;
            btnFinalizePayment.Visible = false;

            pnlECheckPayment.Visible = true;

            pnlExistCard.Visible = false;
            grdCardList.Visible = false;
            chkNewCard.Visible = false;
            chkNewCard.Checked = false;
            pnlNewCard.Visible = false;

            if (grdCardList.Rows.Count > 0)
            {
                foreach (GridViewRow di in grdCardList.Rows)
                {
                    RadioButton rdoSelect = (RadioButton)di.FindControl("rdoSelect");
                    if (rdoSelect.Checked)
                    {
                        rdoSelect.Checked = false;
                    }

                }


            }
        }
        else
        {
            if (grdCardList.Rows.Count > 0)
            {
                pnlExistCard.Visible = true;
                grdCardList.Visible = true;
                chkNewCard.Visible = true;
                btnEcheckPayment.Visible = false;
                btnFinalizePayment.Visible = true;

                pnlECheckPayment.Visible = false;

                tblNewPayment.Visible = true;

                lblCardResult.Text = "";
                lblReason.Text = "";
                if (chkNewCard.Checked)
                {
                    pnlNewCard.Visible = true;
                    if (grdCardList.Rows.Count > 0)
                    {
                        foreach (GridViewRow di in grdCardList.Rows)
                        {
                            RadioButton rdoSelect = (RadioButton)di.FindControl("rdoSelect");
                            if (rdoSelect.Checked)
                            {
                                rdoSelect.Checked = false;
                            }

                        }


                    }
                }

            }
            else
            {
                pnlExistCard.Visible = false;
                grdCardList.Visible = false;
                chkNewCard.Visible = false;
                btnEcheckPayment.Visible = false;
                btnFinalizePayment.Visible = true;

                pnlECheckPayment.Visible = false;

                pnlNewCard.Visible = true;
                tblNewPayment.Visible = true;

            }

        }
    }



}

