<%@ Page Title="" Language="VB"
    AutoEventWireup="false" CodeFile="ExternalGatewayError.aspx.vb" Inherits="PagesPublic_Error_ExternalGatewayError" %>

<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhExternalGatewayError" runat="server" Visible="False">
        <div class="externalgatewayerror">
            <asp:Literal ID="ltlExtGatewayErrMess" runat="server" />
        </div>
    </asp:PlaceHolder>
</asp:Content>
