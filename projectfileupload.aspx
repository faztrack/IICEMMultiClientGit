<%@ Page Title="Customer Information" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="projectfileupload.aspx.cs" Inherits="projectfileupload" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">

    <style>
        div.fileinputs {
            position: relative;
        }

        div.fakefile {
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
        }

        .file {
            position: relative;
            text-align: right;
            -moz-opacity: 0;
            filter: alpha(opacity: 0);
            opacity: 0;
            z-index: 2;
        }

        .ajax__calendar_container {
            z-index: 1 !important;
        }

        a.fancybox img {
            border: none;
            box-shadow: 0 1px 7px rgba(0,0,0,0.6);
            -o-transform: scale(1,1);
            -ms-transform: scale(1,1);
            -moz-transform: scale(1,1);
            -webkit-transform: scale(1,1);
            transform: scale(1,1);
            -o-transition: all 0.2s ease-in-out;
            -ms-transition: all 0.2s ease-in-out;
            -moz-transition: all 0.2s ease-in-out;
            -webkit-transition: all 0.2s ease-in-out;
            transition: all 0.2s ease-in-out;
        }

        a.fancybox:hover img {
            position: relative;
            z-index: 999;
            -o-transform: scale(1.03,1.03);
            -ms-transform: scale(1.03,1.03);
            -moz-transform: scale(1.03,1.03);
            -webkit-transform: scale(1.03,1.03);
            transform: scale(1.03,1.03);
        }
    </style>



    <div class="panel panel-default">
        <div class="panel-heading panel-heading-ext">
            <h3 class="panel-title"><strong>
                <asp:Label ID="lblHearder" runat="server" Text="Add New Customer"></asp:Label></strong></h3>
        </div>
        <div class="panel-body panel-body-ext">
            <div class="form-horizontal">
                <div class="panel-body">


                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="col-md-4"></div>
                            <div class="col-md-5">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12" style="float: left; margin-top: -7px;">

                                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="dropdown-toggle" data-toggle="dropdown">
                                        <asp:Image ID="image1" runat="server" ImageAlign="Middle" ImageUrl="~/icons/ic_photo_camera_black_24px.png" Style="cursor: pointer; width: 48px; height: 48px;" />

                                        <asp:Image ID="imgPreview" runat="server" ImageAlign="Middle" ImageUrl="~/icons/default.png" Style="width: 150px; height: 70px; margin-left: 10px; margin-top: 5px;" />
                                        <ul class="dropdown-menu animated fadeInLeft" style="top: 50px; min-width: 270px; min-height: 200px; border: 1px solid #000;">
                                            <li class="has-sub">
                                                <a href="javascript:;">
                                                    <span>Select Source</span>
                                                </a>
                                                <ul style="list-style-type: none; margin: 0; padding: 0;">
                                                    <li style="display: inline; float: left; padding: 5px 15px;">
                                                        <div class="form-group" runat="server" id="Div1">
                                                            <div class="col-md-3"></div>
                                                            <div class="col-md-2">
                                                                <div class="fileinputs">
                                                                    <asp:FileUpload ID="imgFileUpload" CssClass="btn btn-primary start file" runat="server" onchange="imagePreview()" TabIndex="8" Style="width: 52px; height: 49px" />

                                                                    <div class="fakefile">
                                                                        <p style="text-align: center">
                                                                            <img src="icons/ic_photo_black_24px.png" style="width: 64px; height: 64px" />
                                                                            <b>Photos</b>
                                                                        </p>
                                                                    </div>
                                                                </div>


                                                            </div>
                                                        </div>
                                                    </li>
                                                    <li style="display: inline; float: left; padding: 5px 20px;">
                                                        <div class="form-group" runat="server" id="Div2">
                                                            <div class="col-md-3"></div>
                                                            <div class="col-md-2">
                                                                <div class="fileinputs">
                                                                    <input id="imgCapture" name="imgCapture" type="file" class="file" runat="server" onchange="imagePreview()" style="width: 52px; height: 49px" accept="image/*" capture="camera" />

                                                                    <div class="fakefile">
                                                                        <p style="text-align: center;">
                                                                            <img src="icons/ic_photo_camera_black_24px.png" style="width: 64px; height: 64px; cursor: pointer;" />
                                                                            <b>Camera</b>
                                                                        </p>

                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </asp:HyperLink>
                                </div>

                            </div>
                        </div>


                        <div class="form-group">
                            <div class="col-md-10" style="text-align: center;">
                                <asp:Label ID="lblResult" runat="server" CssClass="control-label"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-11" style="text-align: center;">
                                <asp:Button ID="btnSubmit" runat="server" CssClass="orngButton" OnClick="btnSubmit_Click" TabIndex="23" Text="Save" Width="80px" />
                                &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="brownButton" OnClick="btnCancel_Click" TabIndex="24" Text="Close" Width="80px" />
                            </div>
                        </div>
                        <div>
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        </div>





                    </div>
                    <div class="row">


                        <div id="divImageCapture" class="col-xs-12 col-sm-12 col-md-12 col-lg-12" runat="server">
                        </div>

                    </div>


                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function ($) {
            var addToAll = false;
            var gallery = true;
            var titlePosition = 'inside';
            $(addToAll ? 'img' : 'img.fancybox').each(function () {
                var $this = $(this);
                var title = $this.attr('title');
                var src = $this.attr('data-big') || $this.attr('src');
                var a = $('<a href="#" class="fancybox"></a>').attr('href', src).attr('title', title);
                $this.wrap(a);
            });
            if (gallery)
                $('a.fancybox').attr('rel', 'fancyboxgallery');
            $('a.fancybox').fancybox({
                titlePosition: titlePosition
            });
        });
        $.noConflict();
    </script>
    <script type="text/javascript">

        $(document).ready(function () {








        });


        function imagePreview() {
            try {

                var imgPreview = document.querySelector('#<%=imgPreview.ClientID %>'),
                    file,
                    reader;
                file = document.querySelector('#<%=imgCapture.ClientID %>').files[0] || null;
                if (file == null) {
                    file = document.querySelector('#<%=imgFileUpload.ClientID %>').files[0];
                }
                reader = new FileReader();

                reader.onloadend = function () {
                    imgPreview.src = reader.result;
                }

                if (file) {
                    reader.readAsDataURL(file);
                } else {
                    imgPreview.src = "";

                }
            }
            catch (err) {
                document.getElementById('<%=lblResult.ClientID%>').innerHTML = err.message;
            }
        }
        window.onerror = function () {
            alert("Error caught");
        };
    </script>





</asp:Content>
