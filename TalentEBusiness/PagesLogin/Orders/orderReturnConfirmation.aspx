<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderReturnConfirmation.aspx.vb" Inherits="PagesLogin_orderReturnConfirmation" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div class="alert-box success">
        <asp:Label ID="PageHeaderTextLabel" runat="server"></asp:Label>
    </div> 
</asp:Content>

