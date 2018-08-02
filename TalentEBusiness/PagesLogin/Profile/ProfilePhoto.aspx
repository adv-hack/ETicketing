<%@ Page Title="" Language="VB" AutoEventWireup="false" CodeFile="ProfilePhoto.aspx.vb" Inherits="PagesLogin_Profile_ProfilePhoto" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/AccountPhoto.ascx" TagName="AccountPhoto" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <link href="../../JavaScript/jquery.Jcrop.css" rel="stylesheet" type="text/css" />
    <script src="../../JavaScript/jquery.Jcrop.min.js" type="text/javascript"></script>
    <script src="../../JavaScript/jquery.Jcrop.js" type="text/javascript"></script>
    <script src="../../JavaScript/jquery.color.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="1" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
    <Talent:AccountPhoto ID="AccountPhoto1" runat="server" />
</asp:Content>