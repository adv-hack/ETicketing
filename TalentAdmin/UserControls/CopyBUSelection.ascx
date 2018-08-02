<%@ Control 
Language="VB" 
AutoEventWireup="False" 
CodeFile="CopyBUSelection.ascx.vb"
Inherits="UserControls_CopyBUSelection" %>

<div id="copyBUSelection">
    <asp:SqlDataSource ID="dsBU" runat="server" ConnectionString="<%$ ConnectionStrings:SqlServer2005 %>" SelectCommand="SELECT [CLIENT_NAME] FROM [tbl_client_backend_servers]"></asp:SqlDataSource>
    <asp:DropDownList ID="ddlBU" runat="server" DataSourceID="dsBU" DataTextField="CLIENT_NAME" DataValueField="CLIENT_NAME"></asp:DropDownList>
    <asp:Button CssClass="button" ID="btnCopy" runat="server" Text="Copy BU" />
    <asp:Button ID="btnHomeLink" runat="server" CssClass="button" PostBackUrl="~/Default.aspx" Text="Return Home" />
</div>