using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PopUpService : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public csIsSuccess SetUpPortal()
    {
        csIsSuccess objIsSuccess = new csIsSuccess();
        try
        {


            objIsSuccess.result = "error";
            objIsSuccess.value = "okk";
            objIsSuccess.signup_status = 0;
        }
        catch (Exception ex)
        {
            objIsSuccess.result = "error";
            objIsSuccess.value = ex.Message;
            objIsSuccess.signup_status = 0;
        }

        return objIsSuccess;
    }
}