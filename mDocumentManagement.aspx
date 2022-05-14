﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mDocumentManagement.aspx.cs" Inherits="DocumentManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">





    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="panel panel-default">
                <div class="panel-heading panel-heading-ext">
                    <h3 class="panel-title">
                        <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                        <strong>Document Management&nbsp;<asp:Label ID="lblCustomer" class="titleNu" runat="server" Text="Label"></asp:Label>
                        </strong>

                    </h3>
                </div>
            </div>
            <%--<table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td> <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_title.png" Style="margin-top:3px;" /></td>
                                <td style="padding:6px 0;" align="left"><span class="headerTitle">Document Management&nbsp;<asp:Label ID="lblCustomer" class="titleNu" runat="server" Text="Label"></asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>--%>
            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
            <asp:HiddenField ID="hdnCID" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="fileManager">finder</div>

    <script type="text/javascript" charset="utf-8">
        //$(function () {
        //    $('.fileManager').elfinder({
        //        url: 'elfinder.connector',
        //        height: 600
        //    });
        //});

        $(function () {
            var navbarContextMenu = "";
            var cwdContextMenu = "";
            var fileContextMenu = "";
            var toolBarOption = "";

            $.ajax({
                type: "POST",
                url: 'DocumentManagement.aspx/GetContextMenuData',
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    // debugger;
                    var data = response;
                    //console.log(data.d[0]);

                    if (data.d[1] == 'Admin') {
                        // navbarfolder menu
                        navbarContextMenu = ['open', 'download', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', '|', 'places', 'info'];

                        // current directory menu
                        cwdContextMenu = ['back', 'up', 'reload', '|', 'upload', 'mkdir', 'paste', '|', 'view', 'sort', 'selectall', 'colwidth', '|', 'places', 'info', '|', 'fullscreen'];

                        // current directory file menu
                        fileContextMenu = ['open', 'download', 'opendir', 'quicklook', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', 'edit', '|', 'selectall', 'selectinvert', '|', 'places', 'SelectEmail', 'info', '|', 'ToggleCustomer', 'ToggleVendor'];

                        // toolbar configuration
                        toolBarOption = [
                            ['home', 'back', 'forward', 'up', 'reload'],
                            ['mkdir', 'upload'],
                            ['open', 'download'],
                            ['copy', 'cut', 'paste', 'rm'],
                            ['duplicate', 'rename'],
                            ['SelectEmail'],
                            ['selectall', 'selectnone', 'selectinvert'],
                            ['quicklook', 'info'],
                            ['search'],
                            ['view', 'sort'],
                            ['fullscreen']

                        ];
                    }

                    if (data.d[1] == 'Customer') {
                        // navbarfolder menu
                        navbarContextMenu = ['open', 'download', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', '|', 'places', 'info'];

                        // current directory menu
                        cwdContextMenu = ['back', 'up', 'reload', '|', 'upload', 'mkdir', 'paste', '|', 'view', 'sort', 'selectall', 'colwidth', '|', 'places', 'info', '|', 'fullscreen'];

                        // current directory file menu
                        fileContextMenu = ['open', 'download', 'opendir', 'quicklook', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', 'edit', '|', 'selectall', 'selectinvert', '|', 'places', 'SelectEmail', 'info', 'ToggleCustomer', 'ToggleVendor'];

                        // toolbar configuration
                        toolBarOption = [
                            ['home', 'back', 'forward', 'up', 'reload'],
                            ['mkdir', 'upload'],
                            ['open', 'download'],
                            ['copy', 'cut', 'paste', 'rm'],
                            ['duplicate', 'rename'],
                             ['SelectEmail'],
                            ['selectall', 'selectnone', 'selectinvert'],
                            ['quicklook', 'info'],
                            ['search'],
                            ['view', 'sort'],
                            ['fullscreen']
                        ];
                    }

                    if (data.d[1] == 'Vendor') {
                        // navbarfolder menu
                        navbarContextMenu = ['open', 'download', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', '|', 'places', 'info'];

                        // current directory menu
                        cwdContextMenu = ['back', 'up', 'reload', '|', 'upload', 'mkdir', 'paste', '|', 'view', 'sort', 'selectall', 'colwidth', '|', 'places', 'info', '|', 'fullscreen'];

                        // current directory file menu
                        fileContextMenu = ['open', 'download', 'opendir', 'quicklook', '|', 'upload', 'mkdir', '|', 'copy', 'cut', 'paste', 'duplicate', 'rm', '|', 'rename', 'edit', '|', 'selectall', 'selectinvert', '|', 'places', 'SelectEmail', 'info', '|', 'ToggleCustomer', 'ToggleVendor'];

                        // toolbar configuration
                        toolBarOption = [
                           ['home', 'back', 'forward', 'up', 'reload'],
                           ['mkdir', 'upload'],
                           ['open', 'download'],
                           ['copy', 'cut', 'paste', 'rm'],
                           ['duplicate', 'rename'],
                            ['SelectEmail'],
                           ['selectall', 'selectnone', 'selectinvert'],
                           ['quicklook', 'info'],
                           ['search'],
                           ['view', 'sort'],
                           ['fullscreen']
                        ];
                    }



                    $('.fileManager').elfinder({
                        url: 'elfinder.connector',
                        height: 800,
                        contextmenu: {
                            // navbarfolder menu
                            navbar: navbarContextMenu,
                            // current directory menu
                            cwd: cwdContextMenu,
                            // current directory file menu
                            files: fileContextMenu
                        },
                        uiOptions: {
                            // toolbar configuration
                            toolbar: toolBarOption
                        }
                    });
                },
                error: function (e) {
                    console.log("error: " + e);
                }
            });



        });
    </script>
</asp:Content>

