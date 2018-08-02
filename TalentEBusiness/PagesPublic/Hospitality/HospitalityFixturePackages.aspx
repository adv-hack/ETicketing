<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityFixturePackages.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityFixturePackages"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixturePackageHeader.ascx" TagName="HospitalityFixturePackageHeader" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackages.ascx" TagName="HospitalityPackages" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <Talent:HospitalityFixturePackageHeader ID="uscHospitalityFixturePackageHeader" runat="server" />
    <Talent:HospitalityPackages ID="uscHospitalityPackages" Usage="FullPackageList" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>