using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for EWSAPI
/// </summary>
public class EWSAPI
{
    public EWSAPI()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public static ExchangeService GetEWSService_()
    {
        ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
        if (HttpContext.Current.Session["EWS"] == null)
        {
            // Get the information of the account.
            userinfo objUser = (userinfo)HttpContext.Current.Session["oUser"];
            if (objUser != null)
            {
                service.Credentials = new WebCredentials(objUser.company_email, objUser.email_password);
                service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
                //  service.AutodiscoverUrl(objUser.company_email, CallbackMethods.RedirectionUrlValidationCallback);

                HttpContext.Current.Session.Add("EWS", service);
            }
        }
        else
        {

            return (ExchangeService)HttpContext.Current.Session["EWS"];
        }

        return service;
    }
    public static ExchangeService GetEWSService(string Email, string Password)
    {

        try
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(Email, Password);
           service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
           // service.Url = new Uri("https://west.exch092.serverdata.net/EWS/Exchange.asmx");
           //service.AutodiscoverUrl(Email, CallbackMethods.RedirectionUrlValidationCallback);
            

            return service;
        }
        catch( Exception ex)
        {
            throw ex;
        }


    }
    public static ExchangeService GetEWSService(string Email)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var user = _db.user_infos.SingleOrDefault(u => u.company_email == Email);

        if (user != null)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(Email, user.email_password);
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            // service.AutodiscoverUrl(Email, CallbackMethods.RedirectionUrlValidationCallback);
            return service;
        }

        throw new Exception("From Email address does not exist in the Outlook / Exchange server");
    }
    public static string GetEmailId(ExtendedPropertyDefinition fazExtendedPropertyDefinition,  ExchangeService service )
    {
      

        // Now, find the saved copy of the message by using the custom extended property.
        ItemView view = new ItemView(10);
        SearchFilter searchFilter = new SearchFilter.IsEqualTo(fazExtendedPropertyDefinition, "FaztrackPropertyName");
        view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, fazExtendedPropertyDefinition);
        FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.SentItems, searchFilter, view);

        // Process results.
        foreach (Item myItem in findResults.Items)
        {
            if (myItem is EmailMessage)
            {
                EmailMessage em = myItem as EmailMessage;
               
               return em.Id.UniqueId;
            }
        }


        return "";
    }

   

    public static ExchangeService GetEWSServiceByCustomer(int CustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var user = (from u in _db.user_infos
                    join s in _db.sales_persons on u.sales_person_id equals s.sales_person_id
                    join c in _db.customers on s.sales_person_id equals c.sales_person_id
                    where c.customer_id == CustomerId && c.sales_person_id > 0
                    select u).SingleOrDefault();

        ExchangeService service = EWSAPI.GetEWSService(user.company_email, user.email_password);


        return service;
    }


    public static bool DoesUserExistInOutlookServer(string Email, string Password)
    {


        try
        {
            ExchangeService service = GetEWSService(Email, Password);
            ItemView view = new ItemView(50);
            view.PropertySet = new PropertySet(ItemSchema.Subject, ItemSchema.Id);

            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            view.Traversal = ItemTraversal.Shallow;

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, view);

            int nCount = findResults.Items.Count();

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }
    public static EmailMessage GetEmailDetails(string MessID, ExchangeService service)
    {
        // ExchangeService service = GetEWSService();

        EmailMessage message = new EmailMessage(service);

        try
        {
            message = EmailMessage.Bind(service,
                           MessID,
                            new PropertySet(BasePropertySet.FirstClassProperties,
                            new ExtendedPropertyDefinition(0x1013, MapiPropertyType.Binary)));


            if (message.Body.BodyType.Equals(BodyType.Text))
            {

                message.Body = ToHtml(message.Body.ToString(), false);
            }

            foreach (Attachment attachment in message.Attachments)
            {
                if (attachment is FileAttachment)
                {
                    FileAttachment fileAttachment = attachment as FileAttachment;

                    // Load the file attachment into memory and print out its file name.
                    fileAttachment.Load();
                    // Console.WriteLine("Attachment name: " + fileAttachment.Name);

                    // Load attachment contents into a file.
                    // fileAttachment.Load("C:\\temp\\" + fileAttachment.Name);
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return message;
    }

    public static string ToHtml(string s, bool nofollow)
    {
        s = HttpUtility.HtmlEncode(s);
        string[] paragraphs = s.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
        StringBuilder sb = new StringBuilder();
        foreach (string par in paragraphs)
        {
            sb.AppendLine("<p>");
            string p = par.Replace(Environment.NewLine, "<br />\r\n");
            if (nofollow)
            {
                p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\" rel=\"nofollow\">$1</a>");
                p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\" rel=\"nofollow\">$1</a>");
            }
            else
            {
                p = Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\">$1</a>");
                p = Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\">$1</a>");
                sb.AppendLine(p);
            }
            sb.AppendLine("</p>");
        }
        return sb.ToString();
    }

    public static DSMessage GetEmailList(string CustomerEmail, int CustomerId, WellKnownFolderName FolderName, ExchangeService service)
    {
        DSMessage dsMessage = new DSMessage();
        try
        {


            // Add a search filter that searches on the body or subject.
            //List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            //searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, "Test"));
            //searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Body, "homecoming"));

            //// Create the search filter.
            //SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.Or, searchFilterCollection.ToArray());

            // Create a view with a page size of 50.
            ItemView view = new ItemView(50);

            // Identify the Subject and DateTimeReceived properties to return.
            // Indicate that the base property will be the item identifier
            view.PropertySet = new PropertySet(ItemSchema.Subject, ItemSchema.Id);

            // Order the search results by the DateTimeReceived in descending order.
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Set the traversal to shallow. (Shallow is the default option; other options are Associated and SoftDeleted.)
            view.Traversal = ItemTraversal.Shallow;

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(FolderName, view);


            if (findResults.Items.Count > 0)
            {
                PropertySet PropSet = new PropertySet();
                PropSet.Add(EmailMessageSchema.Attachments);
                PropSet.Add(EmailMessageSchema.ToRecipients);
                PropSet.Add(EmailMessageSchema.From);
                PropSet.Add(EmailMessageSchema.HasAttachments);
                PropSet.Add(EmailMessageSchema.IsRead);
                PropSet.Add(EmailMessageSchema.Id);
                PropSet.Add(EmailMessageSchema.DateTimeCreated);
                PropSet.Add(EmailMessageSchema.Subject);
                PropSet.Add(EmailMessageSchema.DateTimeReceived);

                service.LoadPropertiesForItems(findResults.Items, PropSet);





                if (FolderName.Equals(WellKnownFolderName.SentItems))
                {


                    foreach (EmailMessage msg in findResults)
                    {



                        if (msg.ToRecipients.Where(r => r.Address.Equals(CustomerEmail)).ToList().Count() > 0)
                        {

                            DSMessage.MessageRow mes = dsMessage.Message.NewMessageRow();
                            mes.AttachmentList = "";
                            mes.HasAttachments = msg.HasAttachments;
                            if (mes.HasAttachments)
                            {
                                foreach (var at in msg.Attachments)
                                {
                                    mes.AttachmentList += at.Name + ", ";

                                }

                                mes.AttachmentList = mes.AttachmentList.Trim().TrimEnd(',');

                            }
                            mes.To = GetToAddressList(msg.ToRecipients);
                            mes.From = msg.From.Address;
                            mes.IsRead = msg.IsRead;
                            mes.customer_id = CustomerId.ToString();
                            mes.message_id = msg.Id.ToString();
                            mes.create_date = msg.DateTimeCreated;
                            mes.mess_subject = msg.Subject.ToString();
                            mes.last_view = msg.DateTimeReceived;
                            mes.Protocol = "Outlook";
                            mes.Type = "Sent";
                            dsMessage.Message.AddMessageRow(mes);
                        }
                    }
                }
                else if (FolderName.Equals(WellKnownFolderName.Inbox))
                {

                    foreach (EmailMessage msg in findResults)
                    {



                        if (msg.From.Address.Equals(CustomerEmail))
                        {

                            DSMessage.MessageRow mes = dsMessage.Message.NewMessageRow();
                            mes.AttachmentList = "";
                            mes.HasAttachments = msg.HasAttachments;
                            if (mes.HasAttachments)
                            {
                                foreach (var at in msg.Attachments)
                                {
                                    mes.AttachmentList += at.Name + ", ";

                                }

                                mes.AttachmentList = mes.AttachmentList.Trim().TrimEnd(',');

                            }
                            mes.To = GetToAddressList(msg.ToRecipients);
                            mes.From = msg.From.Address;
                            mes.IsRead = msg.IsRead;
                            mes.customer_id = CustomerId.ToString();
                            mes.message_id = msg.Id.ToString();
                            mes.create_date = msg.DateTimeCreated;
                            mes.mess_subject = msg.Subject.ToString();
                            mes.last_view = msg.DateTimeReceived;
                            mes.Protocol = "Outlook";
                            mes.Type = "Received";
                            dsMessage.Message.AddMessageRow(mes);
                        }
                    }

                }
            }

            dsMessage.AcceptChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return dsMessage;

    }

    private static string GetToAddressList(EmailAddressCollection list)
    {
        string sToList = "";

        foreach (EmailAddress add in list)
        {

            sToList += add.Address + ", ";
        }

        sToList = sToList.Trim().TrimEnd(',');
        return sToList;

    }

    public static DSMessage GetEmailList(string CustomerEmail, int CustomerId, ExchangeService service)
    {
        DSMessage dsMessage = new DSMessage();
        try
        {



            // Create a view with a page size of 50.
            ItemView view = new ItemView(500);

            view.PropertySet = new PropertySet(ItemSchema.Subject, ItemSchema.Id);

            // Order the search results by the DateTimeReceived in descending order.
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Set the traversal to shallow. (Shallow is the default option; other options are Associated and SoftDeleted.)
            view.Traversal = ItemTraversal.Shallow;



            PropertySet PropSet = new PropertySet();
            PropSet.Add(EmailMessageSchema.Attachments);
            PropSet.Add(EmailMessageSchema.ToRecipients);
            PropSet.Add(EmailMessageSchema.From);
            PropSet.Add(EmailMessageSchema.HasAttachments);
            PropSet.Add(EmailMessageSchema.IsRead);
            PropSet.Add(EmailMessageSchema.Id);
            PropSet.Add(EmailMessageSchema.DateTimeCreated);
            PropSet.Add(EmailMessageSchema.Subject);
            PropSet.Add(EmailMessageSchema.DateTimeReceived);



            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResultsSent = service.FindItems(WellKnownFolderName.SentItems, view);
            if (findResultsSent.Items.Count > 0)
            {


                service.LoadPropertiesForItems(findResultsSent.Items, PropSet);

                foreach (EmailMessage msg in findResultsSent)
                {



                    if (msg.ToRecipients.Where(r => r.Address.Equals(CustomerEmail)).ToList().Count() > 0)
                    {

                        DSMessage.MessageRow mes = dsMessage.Message.NewMessageRow();
                        mes.AttachmentList = "";
                        mes.HasAttachments = msg.HasAttachments;
                        if (mes.HasAttachments)
                        {
                            foreach (var at in msg.Attachments)
                            {
                                mes.AttachmentList += at.Name + ", ";

                            }

                            mes.AttachmentList = mes.AttachmentList.Trim().TrimEnd(',');

                        }
                        mes.To = GetToAddressList(msg.ToRecipients);
                        mes.From = msg.From.Address;
                        mes.IsRead = msg.IsRead;
                        mes.customer_id = CustomerId.ToString();
                        mes.message_id = msg.Id.ToString();
                        mes.create_date = msg.DateTimeCreated;
                        mes.mess_subject = msg.Subject.ToString();
                        mes.last_view = msg.DateTimeReceived;
                        mes.Protocol = "Outlook";
                        mes.Type = "Sent";
                        mes.sent_by = msg.From.Name;
                        dsMessage.Message.AddMessageRow(mes);
                    }
                }


            }


            FindItemsResults<Item> findResultsInbox = service.FindItems(WellKnownFolderName.Inbox, view);


            if (findResultsInbox.Items.Count > 0)
            {
                service.LoadPropertiesForItems(findResultsInbox.Items, PropSet);



                foreach (EmailMessage msg in findResultsInbox)
                {



                    if (msg.From.Address.Equals(CustomerEmail))
                    {

                        DSMessage.MessageRow mes = dsMessage.Message.NewMessageRow();
                        mes.AttachmentList = "";
                        mes.HasAttachments = msg.HasAttachments;
                        if (mes.HasAttachments)
                        {
                            foreach (var at in msg.Attachments)
                            {
                                mes.AttachmentList += at.Name + ", ";

                            }

                            mes.AttachmentList = mes.AttachmentList.Trim().TrimEnd(',');

                        }
                        mes.To = GetToAddressList(msg.ToRecipients);
                        mes.From = msg.From.Address;
                        mes.IsRead = msg.IsRead;
                        mes.customer_id = CustomerId.ToString();
                        mes.message_id = msg.Id.ToString();
                        mes.create_date = msg.DateTimeCreated;
                        mes.mess_subject = msg.Subject.ToString();
                        mes.last_view = msg.DateTimeReceived;
                        mes.Protocol = "Outlook";
                        mes.Type = "Received";
                        mes.sent_by = msg.From.Name;
                        dsMessage.Message.AddMessageRow(mes);
                    }
                }

            }


            dsMessage.AcceptChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return dsMessage;

    }

}