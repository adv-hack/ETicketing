<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderEnquiry.aspx.vb" Inherits="PagesLogin_orderEnquiry" Title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/OrderHistoryLinks.ascx" TagName="OrderHistoryLinks" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/OrderEnquiry.ascx" TagName="OrderEnquiry" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/OrderEnquiry2.ascx" TagName="OrderEnquiry2" TagPrefix="uc2" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () { $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' }); });
    </script>
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:OrderHistoryLinks ID="OrderHistoryLinks1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhOrderEnquiry1" runat="server">
        <uc1:OrderEnquiry ID="OrderEnquiry1" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhOrderEnquiry2" runat="server">
        <uc2:OrderEnquiry2 ID="OrderEnquiry2" runat="server" />
    </asp:PlaceHolder>
</asp:Content>