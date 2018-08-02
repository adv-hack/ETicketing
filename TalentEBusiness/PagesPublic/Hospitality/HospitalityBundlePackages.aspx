<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityBundlePackages.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityBundlePackages"  ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBundlePackageHeader.ascx" TagName="HospitalityBundlePackageHeader" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackages.ascx" TagName="HospitalityPackages" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:HospitalityBundlePackageHeader ID="uscHospitalityBundlePackageHeader" runat="server" />
    <Talent:HospitalityPackages ID="uscHospitalityPackages" Usage="BundlePackageList" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" /> 
</asp:content>
