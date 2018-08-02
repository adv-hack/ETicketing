<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AlternativeProducts.ascx.vb" Inherits="UserControls_AlternativeProducts" %>
<asp:ValidationSummary ID="QuantitySummary" runat="server" ValidationGroup="Quantity" />
<p class="error">
    <asp:BulletedList ID="ErrorList" runat="server" CssClass="error"></asp:BulletedList></p>
<div id="ecommerceAlternativeProductsWrapper" runat="server">
   <div id="alternativeProducts">
        <asp:Repeater ID="AlternativeProductsRepeater" runat="server">
             <HeaderTemplate>
                <table summary="alternativeProducts" cellspacing="0">
             </HeaderTemplate>
            <ItemTemplate>
                <tr class="row">
                   <td scope="col" class="product-code" id="ProductCodeColumn" runat="server" >
                        <asp:Label  ID="ProductCodeLabel" runat="server" visible="True" Text='<%# Container.DataItem("AltProductCode") %>'></asp:Label>
                        <asp:Label  ID="hdProductCode" runat="server" visible="False" Text='<%# Container.DataItem("ProductCode") %>'></asp:Label>
                    </td>
                    <td class="name" id="ProductNameColumn" runat="server" >
                          <asp:Label ID="ProductDescription" runat="server" Text=''></asp:Label>
                    </td>
                    <td id="UpdateColumn" class="action" runat="server" >
                        <asp:Button ID="AddButton" runat="server" CssClass="button" CommandArgument='<%# Container.DataItem("AltProductCode") %>'></asp:Button>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="alternativeRow">
                    <td scope="col" class="product-code" id="ProductCodeColumn" runat="server" >
                        <asp:Label ID="ProductCodeLabel" runat="server" visible="True" Text='<%# Container.DataItem("AltProductCode") %>'></asp:Label>
                          <asp:Label  ID="hdProductCode" runat="server" visible="False" Text='<%# Container.DataItem("ProductCode") %>'></asp:Label>
                    </td>
                    <td class="name" id="ProductNameColumn" runat="server" >
                          <asp:Label ID="ProductDescription" runat="server" Text=''></asp:Label>
                    </td>
                    <td id="UpdateColumn" class="action" runat="server" >
                        <asp:Button ID="AddButton" runat="server" CssClass="button"  CommandArgument='<%# Container.DataItem("AltProductCode") %>'></asp:Button>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
   </div>
</div>



