using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class gMessageList : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetSentBy(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer_message> cList = (List<customer_message>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.sent_by.ToLower().StartsWith(prefixText.ToLower())
                    select c.sent_by).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customer_messages
                    where c.sent_by.StartsWith(prefixText)
                    select c.sent_by).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetEmailTo(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer_message> cList = (List<customer_message>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.mess_to.ToLower().Contains(prefixText.ToLower())
                    select c.mess_to).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customer_messages
                    where c.mess_to.ToLower().Contains(prefixText.ToLower())
                    select c.mess_to).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetEmailFrom(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer_message> cList = (List<customer_message>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.mess_from.ToLower().Contains(prefixText.ToLower())
                    select c.mess_from).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customer_messages
                    where c.mess_from.ToLower().Contains(prefixText.ToLower())
                    select c.mess_from).Distinct().Take<String>(count).ToArray();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("GM01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            // Get Customers
            # region Get Customers
            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer_message> CustomerMessList = _db.customer_messages.ToList();
            Session.Add("cSearch", CustomerMessList);

            # endregion
            lblHeaderMess.Text = "Message List";
            GetCustomerMessageInfo(0);

            //HyperLink1.Attributes.Add("onClick", "DisplayWindow();");

            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkGlobal" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Details" });
        }


    }

    private void GetCustomerMessageInfo(int nPageNo)
    {

            try
            {
                string UserEmail = string.Empty;
                if ((userinfo)Session["oUser"] != null)
                {
                    userinfo obj = (userinfo)Session["oUser"];
                    UserEmail = obj.company_email;
                    if (UserEmail.Length == 0)
                        UserEmail = obj.email;
                  //  hdnEmailType.Value = obj.EmailIntegrationType.ToString();
                }
               
                 var typelist = new int[] { 4, 5 }; 


                DSMessage dsMessageSent = new DSMessage();

                DataClassesDataContext _db = new DataClassesDataContext();
                 var messList = (from mess_info in _db.customer_messages
                                  join c in _db.customers on mess_info.customer_id equals c.customer_id
                                 where typelist.Contains((int)c.status_id) && c.is_active == true && (mess_info.mess_to.Contains(UserEmail) || mess_info.mess_from.Contains(UserEmail)) && mess_info.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                orderby mess_info.cust_message_id descending
                                select mess_info).ToList();
                if (chkGlobal.Checked)
                {
                     messList = (from mess_info in _db.customer_messages
                                 join c in _db.customers on mess_info.customer_id equals c.customer_id
                                 where typelist.Contains((int)c.status_id) && c.is_active == true && mess_info.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                    orderby mess_info.cust_message_id descending
                                    select mess_info).ToList();
 
                }
               

                foreach (customer_message msg in messList)
                {
                    DSMessage.MessageRow mes = dsMessageSent.Message.NewMessageRow();



                    if (msg.HasAttachments == null)
                    {
                        string strQ = "select * from message_upolad_info where customer_id=" + msg.customer_id + " and message_id=" + msg.message_id + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
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
                    mes.customer_id = msg.customer_id.ToString();
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
                if (txtSearch.Text.Trim() != "")
                {
                    string str = txtSearch.Text.Trim();
                    if (ddlSearchBy.SelectedValue == "1")
                    {
                        dv.RowFilter = "sent_by LIKE '%" + str + "%'";
                    }
                    else if (ddlSearchBy.SelectedValue == "2")
                    {

                        dv.RowFilter = "From LIKE '%" + str + "%'";
                    }
                    else if (ddlSearchBy.SelectedValue == "3")
                    {
                        dv.RowFilter = "To LIKE '%" + str + "%'";
                    }
                   
                }
                if (ddlItemPerPage.SelectedValue != "4")
                {
                    grdCustomersMessage.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                }
                else
                {
                    grdCustomersMessage.PageSize = 200;
                }
                grdCustomersMessage.PageIndex = nPageNo;
                grdCustomersMessage.DataSource = dv;
                grdCustomersMessage.DataKeyNames = new string[] { "customer_id", "message_id", "AttachmentList", "From", "To", "Type" };
                grdCustomersMessage.DataBind();
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

                if (grdCustomersMessage.PageCount == nPageNo + 1)
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
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

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




                if (e.Row.Cells[7].Text.Equals("True"))
                    e.Row.Cells[7].Text = "Yes";
                else
                    e.Row.Cells[7].Text = "";


                HyperLink hypMessageDetails = (HyperLink)e.Row.FindControl("hypMessageDetails");
                hypMessageDetails.ToolTip = "Click on Message Details to view specific Message Details .";
                hypMessageDetails.Target = "MyWindow";
                script = String.Format("GetdatakeyValue1Old('{0}','{1}')", CustId.ToString(), MessId.ToString());

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
    protected void chkGlobal_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkGlobal.ID, chkGlobal.GetType().Name, "CheckedChanged"); 
        if (chkGlobal.Checked)
        {
            lblHeaderMess.Text = "Global Message List";
           
        }
        else
        {
            lblHeaderMess.Text = "Message List";
        }
        GetCustomerMessageInfo(0);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetCustomerMessageInfo(0);
    }

    protected void grdCustomersMessage_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustomersMessage.ID, grdCustomersMessage.GetType().Name, "PageIndexChanging"); 
        GetCustomerMessageInfo(e.NewPageIndex);
    }
   
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomerMessageInfo(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomerMessageInfo(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("CustomerId");
        GetCustomerMessageInfo(0);
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetSentBy";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmailFrom";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmailTo";
        }
       
       
        GetCustomerMessageInfo(0);
    }

     protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        GetCustomerMessageInfo(0);

    }
     
    
}