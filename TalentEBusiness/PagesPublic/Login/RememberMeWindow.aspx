<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RememberMeWindow.aspx.vb" Inherits="PagesPublic_Login_RememberMeWindow" MasterPageFile="~/MasterPages/Shared/Blank.master" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="row">
        <div class="large-12 columns">
            <Talent:PageHeaderText ID="uscPageHeaderText1" runat="server" />
            <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" usage="2" sequence="1" />
        </div>
    </div>
</asp:Content>