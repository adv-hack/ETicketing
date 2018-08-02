<%@ Page Language="VB" AutoEventWireup="false" CodeFile="myAccount.aspx.vb" Inherits="PagesLogin_myAccount" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/AccountWindow.ascx" TagName="AccountWindow" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    
    <asp:PlaceHolder ID="plhErrorList" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="errorList" runat="server" />
    </div>
    </asp:PlaceHolder>

    <asp:Literal ID="bodyLabel1" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="1" />
    <Talent:AccountWindow ID="AccountWindow1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
</asp:Content>