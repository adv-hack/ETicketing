<%@ Page Language="VB" EnableEventValidation="false"  ValidateRequest="false"  AutoEventWireup="false" CodeFile="FriendsAndFamily.aspx.vb" Inherits="PagesLogin_FriendsAndFamily" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/FriendsAndFamilyDetails.ascx" TagName="FriendsAndFamilyDetails" TagPrefix="uc2" %>
<%@ Register Src="~/UserControls/FriendsAndFamilyOptions.ascx" TagName="FriendsAndFamilyOptions" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <p class="alert-box alert"><asp:Label ID="ErrorLabel" runat="server" /></p>
    </asp:PlaceHolder>

    <uc1:FriendsAndFamilyOptions id="FriendsAndFamilyOptions1" runat="server" />
    <uc2:FriendsAndFamilyDetails id="FriendsAndFamilyDetails1" runat="server" />
</asp:Content >