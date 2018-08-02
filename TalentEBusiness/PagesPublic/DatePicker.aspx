<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DatePicker.aspx.vb" Inherits="PagesPublic_DatePicker" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:Calendar ID="Calendar1" runat="server" CssClass="ebiz-date-picker"></asp:Calendar>
</asp:Content>
