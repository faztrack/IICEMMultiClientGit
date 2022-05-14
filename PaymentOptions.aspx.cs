using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaymentOptions : System.Web.UI.Page
{
    public DataTable dtState;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();
            BindStates();
            if (Session["oCustomerUser"] != null)
            {
                customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
                int nCustomerId = Convert.ToInt32(obj.customerid);

                hdnCustomerID.Value = nCustomerId.ToString();
                if (Convert.ToInt32(hdnCustomerID.Value) > 0)
                {
                    customer cust = new customer();
                    cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerID.Value));

                    hdnCustomerFirstName.Value = cust.first_name1.ToString();
                    hdnCustomerLastName.Value = cust.last_name1.ToString();
                    hdnCustomerEmail.Value = cust.email.ToString();
                    txtAddress.Text = cust.address;
                    txtCity.Text = cust.city;
                    ddlState.SelectedValue = cust.state;
                    txtZip.Text = cust.zip_code;
                }

            }
            else
            {
                Response.Redirect("customerlogin.aspx");
            }

            LoadExpirationYear();
            GetCardLists();

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
        DataTable dt = csCommonUtility.LINQToDataTable(states);
        Session.Add("State", dt);
        dtState = dt;

    }

    public void LoadExpirationYear()
    {
        int nYear = System.DateTime.Now.Year;
        for (int i = 0; i <= 74; i++)
        {
            ddlExpirationYear.Items.Add(new ListItem((nYear + i).ToString(), (nYear + i).ToString().Substring(2)));
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerdashboard.aspx");
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        SaveCardInfo();
        GetCardLists();
    }

    private void SaveCardInfo()
    {
        lblResult.Text = "";

        if (!IsCustomerDataValid())
            return;

        try
        {
            AuthorizeAPI api = new AuthorizeAPI();
            DataClassesDataContext _db = new DataClassesDataContext();

            PaymentProfile objPP = new PaymentProfile();

            int nCustomerId = Convert.ToInt32(hdnCustomerID.Value);
            string nCardNumber = txtCardNumber.Text.Trim().Substring(txtCardNumber.Text.Trim().Length - 4);
            string strNameOnCard = txtNameOnCard.Text.Trim();
            string strCardType = api.GetCardType(txtCardNumber.Text.Trim()).ToString();
            string strMonth = ddlExpirationMonth.SelectedItem.Text;
            string strYear = ddlExpirationYear.SelectedItem.Text;

            if (_db.PaymentProfiles.Where(pp => pp.CustomerId == nCustomerId && pp.NameOnCard == strNameOnCard && pp.CardNumber == nCardNumber && pp.CardType == strCardType).Count() > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Card already exist");
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
            objPP.LastUpdatedBy = hdnCustomerLastName.Value.ToString();
            objPP.BillAddress = txtAddress.Text;
            objPP.BillCity = txtCity.Text;
            objPP.BillState = ddlState.SelectedValue;
            objPP.BillZip = txtZip.Text;


            if (_db.PaymentProfiles.Where(pp => pp.CustomerId == nCustomerId).Count() > 0)
            {
                PaymentProfile objProfile = _db.PaymentProfiles.First(pp => pp.CustomerId == nCustomerId);

                string sPaymentId = api.CreatePaymentProfile(txtCardNumber.Text, new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1).ToString("MMyy"), objProfile.AuthorisedCustomerId.ToString(), strNameOnCard, hdnCustomerEmail.Value.ToString(), txtAddress.Text.Trim(), txtCity.Text.Trim(), ddlState.SelectedItem.Text, txtZip.Text.Trim());
                if (sPaymentId.Length > 0)
                {
                    objPP.AuthorisedCustomerId = Convert.ToInt32(objProfile.AuthorisedCustomerId);
                    objPP.AuthorisedPaymentId =sPaymentId;
                    _db.PaymentProfiles.InsertOnSubmit(objPP);
                    _db.SubmitChanges();
                }
            }
            else
            {

                List<string> sArray = api.CreateCustomerProfile(txtCardNumber.Text, new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1).ToString("MMyy"), hdnCustomerFirstName.Value + " " + hdnCustomerLastName.Value, txtNameOnCard.Text, hdnCustomerEmail.Value.ToString(), txtAddress.Text.Trim(), txtCity.Text.Trim(), ddlState.SelectedItem.Text, txtZip.Text.Trim());

                if (sArray.Count > 1)
                {
                    objPP.AuthorisedCustomerId = Convert.ToInt32(sArray[0]);
                    objPP.AuthorisedPaymentId =sArray[1];
                    _db.PaymentProfiles.InsertOnSubmit(objPP);
                    _db.SubmitChanges();
                }
            }


            lblResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully.");
            Reset();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void GetCardLists()
    {
        dtState = (DataTable)Session["State"];
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();

        try
        {
            var item = from pp in _db.PaymentProfiles
                       where pp.CustomerId == Convert.ToInt32(hdnCustomerID.Value)
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
                           LastUpdatedBy = (string)pp.LastUpdatedBy,
                           BillAddress = (string)pp.BillAddress,
                           BillCity = (string)pp.BillCity,
                           BillState = (string)pp.BillState,
                           BillZip = (string)pp.BillZip

                       };

            if (item.Count() > 0)
            {
                grdCardList.DataSource = item;
                grdCardList.DataKeyNames = new string[] { "PaymentProfileId", "CustomerId", "CardNumber", "CardType", "BillAddress", "BillCity", "BillState", "BillZip" };
                grdCardList.DataBind();

                DataTable dt = csCommonUtility.LINQToDataTable(item);
                Session.Add("sCardList", dt);
            }
            else
            {
                grdCardList.DataSource = null;
                grdCardList.DataBind();


                Session.Remove("sCardList");
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdCardList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCardList.ID, grdCardList.GetType().Name, "Sorting"); 
        lblResult.Text = "";
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
            string strbCity = string.Empty;
            string strbState = string.Empty;
            string strbZip = string.Empty;
            string strbAddress = string.Empty;
            int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[0]);
            int nCustomerId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[1]);
            string nCardNumber = grdCardList.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strCardType = grdCardList.DataKeys[e.Row.RowIndex].Values[3].ToString();
            if (grdCardList.DataKeys[e.Row.RowIndex].Values[4] != null)
                strbAddress = grdCardList.DataKeys[e.Row.RowIndex].Values[4].ToString();
            if (grdCardList.DataKeys[e.Row.RowIndex].Values[5] != null)
                strbCity = grdCardList.DataKeys[e.Row.RowIndex].Values[5].ToString();
            if (grdCardList.DataKeys[e.Row.RowIndex].Values[6] != null)
                strbState = grdCardList.DataKeys[e.Row.RowIndex].Values[6].ToString();
            if (grdCardList.DataKeys[e.Row.RowIndex].Values[7] != null)
                strbZip = grdCardList.DataKeys[e.Row.RowIndex].Values[7].ToString();

            Image imggrdCardType = (Image)e.Row.FindControl("imggrdCardType");
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



            Label lblbAddress = (Label)e.Row.FindControl("lblbAddress");
            //Label lblbCitySateZip = (Label)e.Row.FindControl("lblbCitySateZip");
            TextBox txtbAddress = (TextBox)e.Row.FindControl("txtbAddress");
            TextBox txtbCity = (TextBox)e.Row.FindControl("txtbCity");
            TextBox txtbZip = (TextBox)e.Row.FindControl("txtbZip");
            DropDownList ddlbState = (DropDownList)e.Row.FindControl("ddlbState");

            lblbAddress.Text = strbAddress + ", " + strbCity + ", " + strbState + " " + strbZip;
            if (strbState.Length > 0)
            {
                ddlbState.SelectedValue = strbState;
            }

            LinkButton btn = (LinkButton)e.Row.Cells[6].Controls[0];// Delete Button
            btn.Attributes.Add("onclick", "if (confirm('Are you sure to delete this data?')){return true} else {return false}");
        }
    }

    protected void grdCardList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        dtState = (DataTable)Session["State"];
        lblResult.Text = "";
        if (e.CommandName == "Edit")
        {
            grdCardList.Columns[5].Visible = true; // Cancel Button
            grdCardList.Columns[6].Visible = false; // Delete Button
        }

        if (e.CommandName == "CancelRecord")
        {
            grdCardList.EditIndex = -1;
            GetCardLists();
            UpdatePanel1.Update();
            grdCardList.Columns[4].Visible = true; // Edit Button
            grdCardList.Columns[5].Visible = false; // Cancel Button
            grdCardList.Columns[6].Visible = true; // Delete Button
        }

        if (e.CommandName == "Delete")
        {
            grdCardList.EditIndex = -1;
            GetCardLists();
            UpdatePanel1.Update();
            grdCardList.Columns[5].Visible = false;// Cancel Button
            grdCardList.Columns[6].Visible = true; // Delete Button
        }
    }

    protected void grdCardList_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //dtState = (DataTable)Session["State"];
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();

        int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[Convert.ToInt32(e.NewEditIndex)].Values[0].ToString());

        //  Image imggrdCardType = (Image)grdCardList.Rows[e.NewEditIndex].FindControl("imggrdCardType");
        Label lblgrdCreditCard = (Label)grdCardList.Rows[e.NewEditIndex].FindControl("lblgrdCreditCard");
        TextBox txtgrdCreditCard = (TextBox)grdCardList.Rows[e.NewEditIndex].FindControl("txtgrdCreditCard");

        Label lblgrdNameOnCard = (Label)grdCardList.Rows[e.NewEditIndex].FindControl("lblgrdNameOnCard");
        TextBox txtgrdNameOnCard = (TextBox)grdCardList.Rows[e.NewEditIndex].FindControl("txtgrdNameOnCard");

        DropDownList ddlgrdExpirationMonth = (DropDownList)grdCardList.Rows[e.NewEditIndex].FindControl("ddlgrdExpirationMonth");
        DropDownList ddlgrdExpirationYear = (DropDownList)grdCardList.Rows[e.NewEditIndex].FindControl("ddlgrdExpirationYear");
        Label lblgrdExpirationDatee = (Label)grdCardList.Rows[e.NewEditIndex].FindControl("lblgrdExpirationDatee");

        //    Label lblbAddress = (Label)grdCardList.Rows[e.NewEditIndex].FindControl("lblbAddress");
        //Label lblbCitySateZip = (Label)grdCardList.Rows[e.NewEditIndex].FindControl("lblbCitySateZip");
        //    TextBox txtbAddress = (TextBox)grdCardList.Rows[e.NewEditIndex].FindControl("txtbAddress");
        //   TextBox txtbCity = (TextBox)grdCardList.Rows[e.NewEditIndex].FindControl("txtbCity");
        //  TextBox txtbZip = (TextBox)grdCardList.Rows[e.NewEditIndex].FindControl("txtbZip");
        //   DropDownList ddlbState = (DropDownList)grdCardList.Rows[e.NewEditIndex].FindControl("ddlbState");

        //imggrdCardType.Visible = false;
        lblgrdCreditCard.Visible = false;
        txtgrdCreditCard.Visible = true;

        lblgrdNameOnCard.Visible = false;
        txtgrdNameOnCard.Visible = true;

        ddlgrdExpirationMonth.Visible = true;
        ddlgrdExpirationYear.Visible = true;
        lblgrdExpirationDatee.Visible = false;

        //lblbAddress.Visible = false;
        ////lblbCitySateZip.Visible = false;
        //txtbAddress.Visible = true;
        //txtbCity.Visible = true;
        //txtbZip.Visible = true;
        //ddlbState.Visible = true;


        int nYear = System.DateTime.Now.Year;
        for (int i = 0; i <= 74; i++)
        {
            ddlgrdExpirationYear.Items.Add(new ListItem((nYear + i).ToString(), (nYear + i).ToString().Substring(2)));
        }


        if (_db.PaymentProfiles.Where(pp => pp.PaymentProfileId == cPaymentProfileId).Count() > 0)
            objPP = _db.PaymentProfiles.Single(pp => pp.PaymentProfileId == cPaymentProfileId);

        ddlgrdExpirationMonth.SelectedValue = Convert.ToDateTime(objPP.ExpirationDate).Month.ToString();
        ddlgrdExpirationYear.SelectedItem.Text = Convert.ToDateTime(objPP.ExpirationDate).Year.ToString();

        LinkButton btnUpdate = (LinkButton)grdCardList.Rows[e.NewEditIndex].Cells[4].Controls[0]; // Edit Button
        btnUpdate.Text = "Update";
        btnUpdate.CommandName = "Update";

        LinkButton btnCancel = (LinkButton)grdCardList.Rows[e.NewEditIndex].Cells[5].Controls[0]; // Cancel Button
        btnCancel.Visible = true;
    }

    protected void grdCardList_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCardList.ID, grdCardList.GetType().Name, "RowUpdating");
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();
        AuthorizeAPI api = new AuthorizeAPI();
        TextBox txtgrdCreditCard = (TextBox)grdCardList.Rows[e.RowIndex].FindControl("txtgrdCreditCard");
        TextBox txtgrdNameOnCard = (TextBox)grdCardList.Rows[e.RowIndex].FindControl("txtgrdNameOnCard");
        Label lblCardMessage = (Label)grdCardList.Rows[e.RowIndex].FindControl("lblCardMessage");
        Label lblNameMessage = (Label)grdCardList.Rows[e.RowIndex].FindControl("lblNameMessage");

        bool IsEmpty = false;
        if (txtgrdCreditCard.Text.Trim() == "")
        {
            IsEmpty = true;
            lblCardMessage.Text = "Card Number is required.";
            lblCardMessage.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            lblCardMessage.Text = "";
        }
        if (txtgrdNameOnCard.Text.Trim() == "")
        {
            IsEmpty = true;
            lblNameMessage.Text = "Name On Card is required.";
            lblNameMessage.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            lblNameMessage.Text = "";
        }

        if (IsEmpty)
        {
            txtgrdCreditCard.Focus();
            return;
        }

        int nCardNumber = Convert.ToInt32(txtgrdCreditCard.Text.Trim().Substring(txtgrdCreditCard.Text.Trim().Length - 4));
        string strCardType = api.GetCardType(txtgrdCreditCard.Text.Trim()).ToString();
        string strNameOnCard = txtgrdNameOnCard.Text.Trim();


        DropDownList ddlgrdExpirationMonth = (DropDownList)grdCardList.Rows[e.RowIndex].FindControl("ddlgrdExpirationMonth");
        DropDownList ddlgrdExpirationYear = (DropDownList)grdCardList.Rows[e.RowIndex].FindControl("ddlgrdExpirationYear");
        Label lblgrdExpirationDatee = (Label)grdCardList.Rows[e.RowIndex].FindControl("lblgrdExpirationDatee");

        Label lblbAddress = (Label)grdCardList.Rows[e.RowIndex].FindControl("lblbAddress");
        //Label lblbCitySateZip = (Label)grdCardList.Rows[e.RowIndex].FindControl("lblbCitySateZip");
        TextBox txtbAddress = (TextBox)grdCardList.Rows[e.RowIndex].FindControl("txtbAddress");
        TextBox txtbCity = (TextBox)grdCardList.Rows[e.RowIndex].FindControl("txtbCity");
        TextBox txtbZip = (TextBox)grdCardList.Rows[e.RowIndex].FindControl("txtbZip");
        DropDownList ddlbState = (DropDownList)grdCardList.Rows[e.RowIndex].FindControl("ddlbState");

        int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0].ToString());
        string strMonth = ddlgrdExpirationMonth.SelectedItem.Text;
        string strYear = ddlgrdExpirationYear.SelectedItem.Text;

        if (_db.PaymentProfiles.Where(pp => pp.PaymentProfileId == cPaymentProfileId).Count() > 0)
            objPP = _db.PaymentProfiles.Single(pp => pp.PaymentProfileId == cPaymentProfileId);
        try
        {


            if (api.UpdatePaymentProfile(txtgrdCreditCard.Text.Trim(), new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1).ToString("MMyy"), objPP.AuthorisedCustomerId.ToString(), objPP.AuthorisedPaymentId.ToString(), strNameOnCard, hdnCustomerEmail.Value, txtbAddress.Text, txtbCity.Text, ddlbState.SelectedItem.Value, txtbZip.Text))
            {
                objPP.CardNumber = nCardNumber.ToString();
                objPP.NameOnCard = strNameOnCard;
                objPP.CardType = strCardType;

                objPP.BillAddress = txtbAddress.Text;
                objPP.BillCity = txtbCity.Text;
                objPP.BillState = ddlbState.SelectedValue;
                objPP.BillZip = txtbZip.Text;

                objPP.ExpirationDate = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1);
                objPP.LastUpdatedDate = DateTime.Now;
                objPP.LastUpdatedBy = hdnCustomerLastName.Value.ToString();

                _db.SubmitChanges();

                GetCardLists();
                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");

                grdCardList.Columns[5].Visible = false; // Cancel Button
                grdCardList.Columns[6].Visible = true; // Delete Button
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }
    }

    protected void grdCardList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCardList.ID, grdCardList.GetType().Name, "RowDeleting");
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();
        try
        {
            int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0].ToString());
            if (_db.PaymentProfiles.Where(pp => pp.PaymentProfileId == cPaymentProfileId).Count() > 0)
            {
                objPP = _db.PaymentProfiles.Single(pp => pp.PaymentProfileId == cPaymentProfileId);

                AuthorizeAPI api = new AuthorizeAPI();

                if (api.DeletePaymentProfile(objPP.AuthorisedCustomerId.ToString(), objPP.AuthorisedPaymentId.ToString()))
                {
                    _db.PaymentProfiles.DeleteOnSubmit(objPP);
                    _db.SubmitChanges();

                    GetCardLists();
                    lblResult.Text = csCommonUtility.GetSystemMessage("Data deleted successfully");
                }

            }

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private bool IsCustomerDataValid()
    {
        lblResult.Text = "";

        bool bflag = true;

        string strRequired = string.Empty;

        if (txtCardNumber.Text.Trim() == "")
        {
            strRequired += "Missing required field: Card Number.<br/>";
        }

        if (txtNameOnCard.Text.Trim() == "")
        {
            strRequired += "Missing required field: Name On Card.<br/>";
        }

        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
            bflag = false;
        }

        return bflag;
    }

    private void Reset()
    {
        txtCardNumber.Text = "";
        txtNameOnCard.Text = "";
    }


}