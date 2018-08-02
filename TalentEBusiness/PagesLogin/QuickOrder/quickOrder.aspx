<%@ Page Language="VB" AutoEventWireup="false" CodeFile="quickOrder.aspx.vb" Inherits="PagesLogin_QuickOrder_QuickOrder" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/QuickOrder.ascx" TagName="QuickOrder" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:QuickOrder ID="QuickOrder1" runat="server" />
</asp:Content>

