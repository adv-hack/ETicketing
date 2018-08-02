<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityPackagesList.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityPackagesList"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFilter.ascx" TagName="HospitalityFilter" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackages.ascx" TagName="HospitalityPackages" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <div class="c-hosp-fltr--c">
        <Talent:HospitalityFilter ID="HospitalityFilter" runat="server" />
    </div>
    <div class="c-hosp-pak--no-h">
    	<Talent:HospitalityPackages ID="HospitalityPackages" runat="server" />
	</div>
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>