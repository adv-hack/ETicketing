<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductAssociations.ascx.vb" Inherits="UserControls_ProductAssociations" ViewStateMode="Disabled" %>
<%@ Reference Control="~/usercontrols/productrelationsgraphical.ascx" %>
<%@ Reference Control="~/usercontrols/productrelationsgraphical2.ascx" %>
<%@ Register Src="ProductRelationsGraphical.ascx" TagName="ProductRelationsGraphical" TagPrefix="Talent" %>
<%@ Register Src="ProductRelationsGraphical2.ascx" TagName="ProductRelationsGraphical2" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductRelationsGraphical3.ascx" TagName="ProductRelationsGraphical3" TagPrefix="Talent" %>
<asp:Repeater ID="AssociatedProductsRepeater" runat="server">
    <ItemTemplate>
        <Talent:ProductRelationsGraphical ID="prg" runat="server" />
        <Talent:ProductRelationsGraphical2 ID="prg2" runat="server" />
        <Talent:ProductRelationsGraphical3 ID="uscProductRelationsGraphical3" runat="server" />
    </ItemTemplate>
</asp:Repeater>
