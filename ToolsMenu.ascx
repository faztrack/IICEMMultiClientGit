<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ToolsMenu.ascx.cs" Inherits="ToolsMenu" %>


<div class="divToolsLeft">
    <ul style="list-style-type: none; margin: 0; padding: 0;">
        <li>
            <ul style="float: left;">
                <li style="float: left;">
                    <asp:HyperLink ID="hypMessage" runat="server"><img width="26" height="26" src="images/system_icons/01_icon.png" style="color:green;" alt="Message Center" title="Message Center" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hypCostLoc" runat="server"><img width="26" height="26" src="images/system_icons/16_icon.png" alt="Project Summary Report" title="Project Summary Report" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_CallLog" runat="server"><img width="26" height="26" src="images/system_icons/09_icon.png" alt="Activity Log" title="Activity Log" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_SMS" runat="server"><img width="26" height="26" src="images/system_icons/17_icon.png"  alt="SMS" title="SMS" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_Schedule" runat="server"><img width="26" height="26" src="images/system_icons/05_icon.png" alt="Schedule" title="Schedule" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_Sow" runat="server"><img width="26" height="26" src="images/system_icons/06_icon.png" alt="Composite SOW" title="Composite SOW" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_ProjectNotes" runat="server"><img width="26" height="26" src="images/system_icons/14_icon.png" alt="Project Notes" title="Project Notes" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_Allowance" runat="server"><img width="26" height="26" src="images/system_icons/08_icon.png" alt="Allowance Report" title="Allowance Report" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_PreCon" runat="server"><img width="26" height="26" src="images/system_icons/10_icon.png" alt="Pre-Con Check List" title="Pre-Con Check List" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_SiteReview" runat="server"><img width="26" height="26" src="images/system_icons/11_icon.png" alt="Site Review" title="Site Review" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_DocumentManagement" runat="server"><img width="26" height="26" src="images/system_icons/12_icon.png" alt="Document Management" title="Document Management" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_Section_Selection" runat="server"><img width="26" height="26" src="images/system_icons/15_icon.png" alt="Selection" title="Selection" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_MaterialTracking" runat="server"><img width="26" height="26" src="images/system_icons/19_icon.png" alt="Material Tracking" title="Material Tracking" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_vendor" runat="server"><img width="26" height="26" src="images/system_icons/02_icon.png" alt="Vendor Cost" title="Vendor Cost" /></asp:HyperLink></li>
                <li style="float: left;">
                    <asp:HyperLink ID="hyp_Payment" runat="server"><img width="26" height="26" src="images/system_icons/03_icon.png" alt="Payment Info" title="Payment Info" /></asp:HyperLink></li>

                <li style="float: left;">
                    <asp:HyperLink ID="hypWarrenty" runat="server"><img  width="26" height="26" src="images/system_icons/18_icon.png" alt="Project completion & warranty certificate" title="Project Completion & Warranty Certificate" /></asp:HyperLink></li>

                <li style="float: left;">
                    <asp:HyperLink ID="hypChangeOrderList" runat="server"><img  width="26" height="26" src="images/system_icons/20_icon.png" alt="Change Order List" title="Change Order List" /></asp:HyperLink></li>
        </li>

        <li style="float: left;">
            <asp:HyperLink ID="hyp_survey" runat="server"><img width="26" height="26" src="images/system_icons/07_icon.png" alt="Exit Questionnaire" title="Exit Questionnaire" /></asp:HyperLink></li>
        <%--<asp:HyperLink ID="hyp_Selection" style="display:none;" runat="server"><img width="26" height="26" src="images/section_sheet.png" alt="Selection Sheet" title="Selection Sheet" /></asp:HyperLink>--%>
        <li style="float: left;"></li>
        <li style="float: left;">
            <asp:HyperLink ID="hyp_jstatus" runat="server"><img width="26" height="26" src="images/system_icons/04_icon.png" alt="Job Status Graphics" title="Job Status Graphics" /></asp:HyperLink></li>
    </ul>
    </li>
                                                        </ul>
</div>

<div>
    <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
    <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />
</div>
