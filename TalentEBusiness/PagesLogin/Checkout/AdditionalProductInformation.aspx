<%@ Page Title="" Language="VB"  AutoEventWireup="false" CodeFile="AdditionalProductInformation.aspx.vb" Inherits="PagesLogin_Checkout_AdditionalProductInformation" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/AdditionalProductInformation.ascx" TagName="AdditionalInformation" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script type="text/javascript">
        function OpenNextItem(item) {
            $(".additional-information").accordion("option", "active", item);
        }

        $(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' });
        });
    </script>
</asp:Content>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:AdditionalInformation ID="AdditionalInformation" runat="server" />
</asp:Content>