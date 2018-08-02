<%@ Page Title="" Language="VB" 
    AutoEventWireup="false" CodeFile="PrerequisiteMissing.aspx.vb" Inherits="PagesPublic_Error_PrerequisiteMissing" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhCookiesDisabled" runat="server" Visible="false">
        <div id="divCookiesDisabled">
            <asp:Literal ID="ltlCookiesDisabled" runat="server"></asp:Literal>
        </div>
    </asp:PlaceHolder>
    <noscript>
        <asp:Literal ID="ltlJavascriptDisabled" runat="server"></asp:Literal>
    </noscript>
</asp:Content>
