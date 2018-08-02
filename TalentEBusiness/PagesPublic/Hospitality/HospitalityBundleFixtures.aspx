<%@ Page Language="VB" AutoEventWireup="false" CodeFile="HospitalityBundleFixtures.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityBundleFixtures" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBundlePackageHeader.ascx" TagName="HospitalityBundlePackageHeader" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackageHTMLInclude.ascx" TagName="HospitalityPackageHTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixtures.ascx" TagName="HospitalityFixtures" TagPrefix="Talent" %>

<asp:content ID="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">

    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div class="row">
        <div class="column">
            <Talent:HospitalityBundlePackageHeader ID="uscHospitalityBundlePackageHeader" runat="server" />

            <div class="row">
                <div class="columns o-hosp-fix-pak__cont">
                    <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude1" runat="server" Sequence="1"/>
                    <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude2" runat="server" Sequence="2"/>
                </div>

                <div class="columns o-hosp-fix-pak__sb c-hosp-pak--sb">
                    <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude3" runat="server" Sequence="3"/>
                    <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude4" runat="server" Sequence="4"/>
                </div>
            </div>

            <Talent:HospitalityFixtures ID="uscHospitalityFixtures" runat="server" />
        </div>
    </div>

    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" /> 

</asp:content>
