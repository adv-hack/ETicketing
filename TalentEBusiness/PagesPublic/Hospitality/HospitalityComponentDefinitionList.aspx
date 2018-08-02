<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityComponentDefinitionList.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityComponentDefinitionList"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityComponentDefinitionList.ascx" TagName="HospitalityComponentDefinitionList" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server"></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    
    <Talent:HospitalityComponentDefinitionList ID="HospitalityComponentDefinitionList" runat="server" />
    
<Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" /></asp:Content>