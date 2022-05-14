<%@ Page Title="Error" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="error404.aspx.cs" Inherits="error404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <div class="ministyle">
        <div class="panel panel-inverse" style="text-align:center;padding:100px;">
            <p style="font-weight:bold;font-size:42px;color:black;"><span style="font-weight:bold;font-size:42px;color:black;">Sorry</span></p>
            <br />
            <br />
            <p style="font-size:26px;">The page you are looking for could not be found</p>
            <br />
            <br />
            <a id="A1" style="padding:10px 20px;background-color:#df0031;font-weight:bold;color:#fff;border-radius:3px;font-size:16px;" runat="server" href="~\">Go to Homepage</a>
        </div>
    </div>
</asp:Content>

