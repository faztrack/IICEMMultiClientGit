<%@ WebHandler Language="C#" Class="EmailHandler" %>

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq;
public class EmailHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        try
        {
            string strJson = new StreamReader(context.Request.InputStream).ReadToEnd();

            //deserialize the object
            csEmailTemplate objET = Deserialize<csEmailTemplate>(strJson);
            if (objET != null)
            {
                string Message = objET.Message;
              string Name = objET.Name;

                DataClassesDataContext _db = new DataClassesDataContext();
                EmailTemplateInfo emTemplate = new EmailTemplateInfo();

                emTemplate.Name =Name;
                //emTemplate.ToEmailAddress = txtTo.Text;
                //emTemplate.FromAddress = txtFrom.Text;
                //emTemplate.CcAddress = txtCc.Text;
                //emTemplate.BccAddress = txtBcc.Text;
                //emTemplate.Subject = txtSubject.Text;
                //emTemplate.Message = txtBody.Content;

                emTemplate.LastUpdatedDate = DateTime.Now;
                emTemplate.LastUpdatedUserId = 1;

                emTemplate.LastUpdatedDate = DateTime.Now;

                emTemplate.Message =Message;
                emTemplate.CreatedDate = DateTime.Now;
                emTemplate.CreatedUserId = 1;
                _db.EmailTemplateInfos.InsertOnSubmit(emTemplate);




                _db.SubmitChanges();

                context.Response.Write(string.Format("Message :{0}, Name :{1}", Message,Name));

        
              
            }
            else
            {
                context.Response.Write("No Data");
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("Error :" + ex.Message);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public class csEmailTemplate
    {
        public string Message { get; set; }
        public string Name { get; set; }

    }


    public T Deserialize<T>(string context)
    {
        string jsonData = context;

        //cast to specified objectType
        var obj = (T)new JavaScriptSerializer().Deserialize<T>(jsonData);
        return obj;
    }
}



