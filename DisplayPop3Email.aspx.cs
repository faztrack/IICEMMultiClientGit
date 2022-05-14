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
using System.Collections.Generic;
using Prabhu;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Net.Mail;


public partial class DisplayPop3Email : System.Web.UI.Page
{
    public const string Host = "pop.gmail.com";
    public const int Port = 995;
    public string Email ;
    public string Password;
    protected static Regex CharsetRegex = new Regex("charset=\"?(?<charset>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex QuotedPrintableRegex = new Regex("=(?<hexchars>[0-9a-fA-F]{2,2})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex UrlRegex = new Regex("(?<url>https?://[^\\s\"]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex FilenameRegex = new Regex("filename=\"?(?<filename>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex NameRegex = new Regex("name=\"?(?<filename>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected void Page_Load(object sender, EventArgs e)
    {
        int emailId = -1;
		if (Request.QueryString["emailId"] == null)
		{
			Response.Redirect("Pop3Client.aspx");
			Response.Flush();
			Response.End();
		}
		else
            Email = Session["email"].ToString();
            Password = Session["pwd"].ToString();
			emailId = Convert.ToInt32(Request.QueryString["emailId"]);
		Email email = null;
		List<MessagePart> msgParts = null;
		using (Prabhu.Pop3Client client = new Prabhu.Pop3Client (Host, Port,Email, Password, true))
		{
            
			client.Connect();
			email = client.FetchEmail(emailId);
			msgParts = client.FetchMessageParts(emailId);
		}
		if (email == null || msgParts == null)
		{
			Response.Redirect("Pop3Client.aspx");
			Response.Flush();
			Response.End();
		}
		MessagePart preferredMsgPart = FindMessagePart(msgParts, "text/html");
		if (preferredMsgPart == null)
			preferredMsgPart = FindMessagePart(msgParts, "text/plain");
		else if (preferredMsgPart == null && msgParts.Count > 0)
			preferredMsgPart = msgParts[0];
		string contentType, charset, contentTransferEncoding, body = null;
		if (preferredMsgPart != null)
		{
			contentType = preferredMsgPart.Headers["Content-Type"];
			charset = "us-ascii";
			contentTransferEncoding =preferredMsgPart.Headers["Content-Transfer-Encoding"];
			Match m = CharsetRegex.Match(contentType);
			if (m.Success)
				charset = m.Groups["charset"].Value;
			HeadersLiteral.Text = contentType != null ? "Content-Type: " +contentType + "<br />" : string.Empty;
			HeadersLiteral.Text += contentTransferEncoding != null ?"Content-Transfer-Encoding: " +contentTransferEncoding : string.Empty;
			if (contentTransferEncoding != null)
			{
				if (contentTransferEncoding.ToLower() == "base64")
					body = DecodeBase64String(charset,preferredMsgPart.MessageText);
				else if (contentTransferEncoding.ToLower() =="quoted-printable")
					body = DecodeQuotedPrintableString(preferredMsgPart.MessageText);
				else
					body = preferredMsgPart.MessageText;
			}
			else
				body = preferredMsgPart.MessageText;
		}
		EmailIdLiteral.Text = Convert.ToString(emailId);
		DateLiteral.Text = email.UtcDateTime.ToString(); ;
		FromLiteral.Text = email.From;
		SubjectLiteral.Text = email.Subject;
		BodyLiteral.Text = preferredMsgPart != null ? (preferredMsgPart.Headers["Content-Type"].IndexOf("text/plain") != -1 ?"<pre>" + FormatUrls(body) + "</pre>" : body) : null;
        //string str = HtmlToPlainText(BodyLiteral.Text);

        //TextBox1.Text = str;
        //StreamReader rdr = new StreamReader(sFileName);
        //string strHtml = preferredMsgPart != null ? (preferredMsgPart.Headers["Content-Type"].IndexOf("text/plain") != -1 ? "<pre>" + FormatUrls(body) + "</pre>" : body) : null;
        //Editor1.Content = "</br></br></br></br></br>" + "--------------Original message--------------" + "</br>" + "Date & Time:" + email.UtcDateTime.ToString() + "</br>" + "From:"+email.From + "</br>" + "</br>" + BodyLiteral.Text;
		ListAttachments(msgParts);
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
        text =HttpUtility.HtmlDecode(text);
        //Remove tag whitespace/line breaks
        text = tagWhiteSpaceRegex.Replace(text, "><");
        //Replace <br /> with line breaks
        text = lineBreakRegex.Replace(text, Environment.NewLine);
        //Strip formatting
        text = stripFormattingRegex.Replace(text, string.Empty);

        return text;
    }

    protected Decoder GetDecoder(string charset)
    {
        Decoder decoder;
        switch (charset.ToLower())
        {
            case "utf-7":
                decoder = Encoding.UTF7.GetDecoder();
                break;
            case "utf-8":
                decoder = Encoding.UTF8.GetDecoder();
                break;
            case "us-ascii":
                decoder = Encoding.ASCII.GetDecoder();
                break;
            case "iso-8859-1":
                decoder = Encoding.ASCII.GetDecoder();
                break;
            default:
                decoder = Encoding.ASCII.GetDecoder();
                break;
        }
        return decoder;
    }
    protected string DecodeBase64String(string charset, string encodedString)
    {
        Decoder decoder = GetDecoder(charset);
        byte[] buffer = Convert.FromBase64String(encodedString);
        char[] chararr = new char[decoder.GetCharCount(buffer,0, buffer.Length)];
        decoder.GetChars(buffer, 0, buffer.Length, chararr, 0);
        return new string(chararr);
    }
    protected string DecodeQuotedPrintableString(string encodedString)
    {
        StringBuilder b = new StringBuilder();
        int startIndx = 0;
        MatchCollection matches = QuotedPrintableRegex.Matches(encodedString);
        for (int i = 0; i < matches.Count; i++)
        {
            Match m = matches[i];
            string hexchars = m.Groups["hexchars"].Value;
            int charcode = Convert.ToInt32(hexchars, 16);
            char c = (char)charcode;
            if (m.Index > 0)
                b.Append(encodedString.Substring(startIndx, (m.Index - startIndx)));
            b.Append(c);
            startIndx = m.Index + 3;
        }
        if (startIndx < encodedString.Length)
            b.Append(encodedString.Substring(startIndx));
        return Regex.Replace(b.ToString(), "=\r\n", "");
    }
    protected void ListAttachments(List<MessagePart> msgParts)
    {
        bool attachmentsFound = false;
        StringBuilder b = new StringBuilder();
        b.Append("<ol>");
        foreach (MessagePart p in msgParts)
        {
            string contentType = p.Headers["Content-Type"];
            string contentDisposition = p.Headers["Content-Disposition"];
            Match m;
            if (contentDisposition != null)
            {
                m = FilenameRegex.Match(contentDisposition);
                if (m.Success)
                {
                    attachmentsFound = true;
                    b.Append("<li>").Append(m.Groups["filename"].Value).Append("</li>");
                    
                }
            }
            else if (contentType != null)
            {
                m = NameRegex.Match(contentType);

                if (m.Success)
                {
                    attachmentsFound = true;
                    b.Append("<li>").Append(m.Groups["filename"].Value).Append("</li>");
                }
            }
        }
        b.Append("</ol>");
        if (attachmentsFound)
            AttachmentsLiteral.Text = b.ToString();
        else
            AttachementsRow.Visible = false;
    }
    protected MessagePart FindMessagePart(List<MessagePart> msgParts,string contentType)
    {
        foreach (MessagePart p in msgParts)
            if (p.ContentType != null && p.ContentType.IndexOf(contentType) != -1)
                return p;
        return null;
    }
    protected string FormatUrls(string plainText)
    {
        string replacementLink = "<a href=\"${url}\">${url}</a>";
        return UrlRegex.Replace(plainText, replacementLink);
    }
   
}
