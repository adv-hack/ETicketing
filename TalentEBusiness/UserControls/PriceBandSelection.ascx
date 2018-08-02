<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PriceBandSelection.ascx.vb" Inherits="UserControls_PriceBandSelection" ViewStateMode="Enabled" %>
<asp:HiddenField ID="hdfProductCode" runat="server" />
<asp:HiddenField ID="hdfProductType" runat="server" />
<asp:HiddenField ID="hdfProductDetailCode" runat="server" />
<asp:HiddenField ID="hdfQuantityAvailable" runat="server" />
<asp:HiddenField ID="hdfProductStadium" runat="server" />
<asp:HiddenField ID="hdfProductSubType" runat="server" />

<asp:Panel ID="plhProductAvailable" runat="server" DefaultButton="btnAddToBasket">
    <asp:PlaceHolder ID="plhExtendedText" runat="server">
        <div class="alert-box info ebiz-event-extended-text">
            <asp:Literal ID="lblExtendedText1" runat="server" />
            <asp:Literal ID="lblExtendedText2" runat="server" />
            <asp:Literal ID="lblExtendedText3" runat="server" />
            <asp:Literal ID="lblExtendedText4" runat="server" />
            <asp:Literal ID="lblExtendedText5" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhTravelDropdown" runat="server" Visible="False">
        <div class="row ebiz-product-dropdown-wrap">
            <div class="medium-3 columns">
                <asp:Label ID="ltlTravelDropDownDesciption" runat="server" AssociatedControlID="ddlTravelOptions" />
            </div>
            <div class="medium-9 columns">
                <asp:DropDownList ID="ddlTravelOptions" runat="server" ViewStateMode="Enabled"></asp:DropDownList>
            </div>
        </div>
     </asp:PlaceHolder>
    <asp:Repeater ID="rptPriceBandSelection" runat="server">
        <HeaderTemplate>
            <table class="ebiz-responsive-table">
            <thead>
                <tr>
                    <th scope="col" class="ebiz-description"><%= DescriptionHeaderText%></th>
                    <th scope="col" class="ebiz-price"><%= PriceHeaderText%></th>
                    <th scope="col" class="ebiz-quantity"><%= QuantityHeaderText%></th>
                </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="odd <%# GetLoggedInCustomerPriceBandCss(Eval("PriceBand").ToString())%> ">
                    <td class="ebiz-description" data-title="<%= DescriptionHeaderText%>">
                        <asp:Literal ID="ltlDescriptionText" runat="server" />
                        <asp:HiddenField ID="hdfPriceBand" runat="server" />
                    </td>
                    <td class="ebiz-price" data-title="<%= PriceHeaderText%>"><asp:Literal ID="ltlPriceText" runat="server" /></td>
                    <td class="ebiz-quantity" data-title="<%= QuantityHeaderText%>">
                        <asp:DropDownList ID="ddlQuantity" runat="server"/>
                        <asp:TextBox ID="txtBulkSalesQuantity" runat="server" />
                        <asp:HiddenField ID="hdfQuantityMultiplier" runat="server" />
                        <asp:RegularExpressionValidator ID="rgxBulkSalesQuantity" runat="server" ValidationGroup="Quantity" ControlToValidate="txtBulkSalesQuantity" Display="None" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                    </td>
                </tr>
        </ItemTemplate>
        <FooterTemplate>
        </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:PlaceHolder ID="plhTravelProduct" runat="server" Visible="False">
        <p class="ebiz-include-travel">
            <asp:CheckBox ID="chkIncludeTravel" runat="server" /> <asp:Label ID="lblIncludeTravel" runat="server" AssociatedControlID="chkIncludeTravel"></asp:Label>
        </p>
    </asp:PlaceHolder>
    <div class="ebiz-actions"><asp:Button ID="btnAddToBasket" runat="server" CssClass="button ebiz-primary-action ebiz-add-to-basket" CausesValidation="true" ValidationGroup="Quantity" /></div>
    
</asp:Panel>

<asp:PlaceHolder ID="plhProductSoldOut" runat="server" Visible="False">
    <div class="alert-box alert ebiz-sold-out">
        <asp:Literal ID="ltlSoldOutText" runat="server" />
    </div>
</asp:PlaceHolder>