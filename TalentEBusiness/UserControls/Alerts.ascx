<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Alerts.ascx.vb" Inherits="UserControls_Alerts" ViewStateMode="Enabled" %>
    <asp:Repeater ID="rptUserAlerts" runat="server">
        <HeaderTemplate>
            <asp:Literal ID="ltlHeaderString" runat="server" />
        </HeaderTemplate>
        <ItemTemplate>
            <asp:PlaceHolder ID="plhAlertString" runat="server" />
            <asp:LinkButton ID="lbtnRemoveThisAlert" runat="server" CommandName="RemoveThisAlert" />
            <asp:HiddenField ID="hdfAlertID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ID").ToString %>' />
        </ItemTemplate>
        <FooterTemplate>
            <asp:Literal ID="ltlFooterString" runat="server" />
        </FooterTemplate>
    </asp:Repeater>
    <asp:Label ID="lblRedirectMessage" runat="server" Visible="false"></asp:Label>