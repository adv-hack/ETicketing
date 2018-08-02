<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="ApplyCashback.aspx.vb" Inherits="PagesAdmin_Checkout_ApplyCashback" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ApplyCashback.ascx" TagName="ApplyCashback" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:ApplyCashback ID="ApplyCashback" runat="server" />
</asp:Content>