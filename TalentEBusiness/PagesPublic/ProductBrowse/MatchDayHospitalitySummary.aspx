<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="MatchDayHospitalitySummary.aspx.vb" Inherits="PagesPublic_ProductBrowse_MatchDayHospitalitySummary" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/MatchDayHospitalitySummary.ascx" TagName="MatchDayHospitalitySummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:MatchDayHospitalitySummary ID="MatchDayHospitalitySummary1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>
