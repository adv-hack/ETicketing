<%@ Page Title="" Language="VB" AutoEventWireup="false" CodeFile="ProductSubTypes.aspx.vb" Inherits="PagesPublic_ProductBrowse_ProductSubTypes" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductSubTypes.ascx" TagName="ProductSubTypes" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' });
        });
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <Talent:ProductSubTypes ID="ProductSubTypes" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>
