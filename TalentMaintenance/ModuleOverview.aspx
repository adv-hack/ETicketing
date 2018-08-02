<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ModuleOverview.aspx.vb" Inherits="ModuleOverview" %>
<%@ Register Src="UserControls/PromotionsOverview.ascx" TagName="PromotionsOverview" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="pageMaintenanceTopNavigation">
       <asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
            <asp:listItem Text="Maintenance Portal" Value="MaintenancePortal.aspx" />
       </asp:BulletedList>
    </div>
    <div id="moduleOverview1">
        <p class="title"><asp:Label ID="titleLabel" runat="server" Text="PageLabel" /></p>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" Text="PageLabel" /></p>
        <div class="moduleOverview2">
            <table class="defaultTable">
                <tr>
                    <th>
                        <asp:Label ID="businessunitLabel" runat="server" />
                    </th>
                    <td>
                        <asp:DropDownList ID="businessunitDropDownList" runat="server" AutoPostBack="True" CssClass="select" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Label ID="partnerGroupLabel" runat="server" />
                    </th>
                    <td>
                        <asp:DropDownList ID="partnerGroupDropDownList" runat="server" AutoPostBack="True" CssClass="select" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Label ID="partnerLabel" runat="server" />
                    </th>
                    <td>
                        <asp:DropDownList ID="partnerDropDownList" runat="server" AutoPostBack="True" CssClass="select" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="promotionsOverviewDropDown">
            <Talent:PromotionsOverview ID="PromotionsOverview1" BusinessUnit='<%# businessunitDropDownList.SelectedValue %>'
                Partner='<%# businessunitDropDownList.SelectedValue %>' PartnerGroup='<%# partnerGroupDropDownList.SelectedValue %>' runat="server" />
        </div>
    </div>
</asp:Content>