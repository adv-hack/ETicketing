<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="Alerts.aspx.vb" Inherits="PagesLogin_Profile_Alerts" %>
<%@ Register Src="~/UserControls/Alerts.ascx" TagName="Alerts" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<Talent:Alerts ID="Alerts1" runat="server" />
</asp:Content>