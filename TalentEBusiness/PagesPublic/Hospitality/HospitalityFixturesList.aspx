<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityFixturesList.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityFixturesList"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFilter.ascx" TagName="HospitalityFilter" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixtures.ascx" TagName="HospitalityFixtures" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <div class="c-hosp-fltr--c">
        <Talent:HospitalityFilter ID="HospitalityFilter" runat="server" />
    </div>
    <div class="c-hosp-fixs--no-h">
    	<Talent:HospitalityFixtures ID="HospitalityFixtures" runat="server" />
    </div>
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />





    <!-- A TEMPORARY SCRIPT TO FUDGE FUNCTIONALITY -- FOR MOCKUP PURPOSES ONLY -->
    <script>
        $(function(){
            $(".c-hosp-fixs__bck > a").attr("href", "../../PagesPublic/Hospitality/HospitalityFixturePackages.aspx?fixture=PVFC")
        });
    </script>
</asp:Content>


