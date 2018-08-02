<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ComponentSeats.ascx.vb" Inherits="UserControls_Package_ComponentSeats" ViewStateMode="Disabled"%>
<asp:PlaceHolder ID="plhComponentSeats" Visible="false" runat="server">
    <asp:placeholder ID="plhErrorList" runat="server"> 
        <div class="alert-box alert">
	        <asp:BulletedList ID="blErrorList" runat="server" />
        </div>
	</asp:placeholder>
    <script>
        function RegFriendsAndFamily(ddlClientID, hidFFURLClientID) {
            var custDDL = document.getElementById(ddlClientID);
            var hiddenField = document.getElementById(hidFFURLClientID);
            if (custDDL != null && hiddenField != null) {
                if (custDDL.options[custDDL.selectedIndex].value == document.getElementById(hidFFURLClientID).value) {
                    location = document.getElementById(hidFFURLClientID).value;
                }
            }
        }
    </script>

      <asp:Repeater ID="rptSeats" runat="server">
        <HeaderTemplate>
            <table class="stack ebiz-component-seats">
                <thead>
                    <tr>
                        <th id="customerHeaderCol" class="ebiz-customer" scope="col" runat="server">
                            <%# GetText("lblCustomerHeader") %>
                        </th>
                        <asp:PlaceHolder ID="plhSeatHeader" runat="server">
                            <th runat="server" class="ebiz-seat" scope="col">
                                <%# GetText("lblSeatHeader")%>
                            </th>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhQuantityHeader" runat="server">
                            <th runat="server" class="ebiz-quantity" scope="col">
                                <%# GetText("lblQuantityHeader")%>
                            </th>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhSeatDetailsHeader" runat="server">
                            <th runat="server" class="ebiz-seat-details" scope="col">
                                <%# GetText("lblSeatDetailsHeader")%>
                            </th>
                        </asp:PlaceHolder>                        
                        <th id="priceCodeHeaderCol" class="ebiz-price-code" scope="col" runat="server">
                            <%# GetText("lblPriceCodeHeader")%>
                        </th>
                        <th class="ebiz-price-band" scope="col">
                            <%# GetText("lblPriceBandHeader")%>
                        </th>
                        <th class="ebiz-price" scope="col">
                            <%# GetText("lblPrice")%>
                        </th>
                        <th id="removeButtonHeaderCol" class="ebiz-remove" runat="server">
                            <%# GetText("lblRemove")%>
                        </th>
                    </tr>
                </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:PlaceHolder ID="plhStandAreaDescriptions" runat="server">
                    <tr>
                        <td class="ebiz-ebiz-component-seats-container">
                            <div class="row ebiz-stand-area-descriptions">
                                <div class="columns large-6 ebiz-stand-description">
                                    <span><asp:Literal ID="ltlStandDescriptionLabel" runat="server" /></span><asp:Literal ID="ltlStandDescriptionValue" runat="server" />
                                </div>
                                <div class="columns large-6 ebiz-area-description">
                                    <span><asp:Literal ID="ltlAreaDescriptionLabel" runat="server" /></span><asp:Literal ID="ltlAreaDescriptionValue" runat="server" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </asp:PlaceHolder>
        
                <tr>
                    <td data-title='<%# GetText("lblCustomerHeader") %>' id="customerItemCol" class="ebiz-customer" runat="server">
                        <asp:DropDownList ID="ddlcustomer" runat="server"></asp:DropDownList>
                        <asp:HiddenField ID="hdfProductCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ProductCode").ToString().Trim() %>' />
                        <asp:HiddenField ID="hdfPriceCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PriceCode").ToString().Trim() %>' />
                        <asp:HiddenField ID="hidFFRegURL" runat="server" />
                        <asp:HiddenField ID="hdfBulkID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "BulkID").ToString().Trim()%>' />
                    </td>
                    <asp:PlaceHolder ID="plhSeatRow" runat="server">
                         <td data-title='<%# GetText("lblSeatHeader")%>' class="ebiz-seat">
                            <asp:Literal ID="litSeat" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SeatText").ToString().Trim()%>'></asp:Literal>
                            <asp:Literal ID="ltlAlpha" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AlphaSuffix").ToString().Trim() %>'></asp:Literal>
                            <asp:HiddenField ID="hdfSeat" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "SeatDetails").ToString().Trim() %>' />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhQuantityRow" runat="server">
                        <td data-title='<%# GetText("lblQuantityHeader")%>' class="ebiz-quantity">
                            <asp:Literal ID="litQuantity" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "BulkQuantity").ToString().Trim()%>'></asp:Literal>
                            <asp:HiddenField ID="hdfQuantity" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "BulkQuantity").ToString().Trim()%>' />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSeatDetailsRow" runat="server">
                        <td data-title='<%# GetText("lblSeatDetailsButton")%>' class ="ebiz-seat-details">
                            <asp:HyperLink ID="hlkSeatDetails" runat="server" data-open="seat-details-modal" CssClass="button ebiz-open-modal ebiz-seat-details-history" NavigateUrl='<%# GetSeatDetailsURL(Profile.Basket.Basket_Header_ID, DataBinder.Eval(Container.DataItem, "BulkID")).ToString%>'><%# GetText("lblSeatDetailsButton")%></asp:HyperLink>
                            <div class="reveal ebiz-reveal-ajax" id="seat-details-modal" data-reveal></div> 
                        </td>
                    </asp:PlaceHolder>
                    <td data-title='<%# GetText("lblPriceCodeHeader")%>' id="priceCodeItemCol" class="ebiz-price-code" runat="server">
                        <asp:DropDownList ID="ddlPriceCode" runat="server"></asp:DropDownList>
                    </td>
                    <td data-title='<%# GetText("lblPriceBandHeader")%>' class="ebiz-price-band">
                        <asp:DropDownList ID="ddlPriceBand" runat="server"></asp:DropDownList>
                    </td>
                    <td data-title='<%# GetText("lblPrice")%>' class="ebiz-price">
                        <asp:Label ID="lblPrice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Price").ToString().Trim() %>'></asp:Label>
                    </td>
                    <td data-title='<%# GetText("lblRemove")%>' id="removeButtonItemCol" class="ebiz-remove" runat="server">
                        <asp:Button ID="btnRemove" runat="server" class="fa-input ebiz-fa-input ebiz-remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "SeatDetails").ToString().Trim() & "," & DataBinder.Eval(Container.DataItem, "AlphaSuffix").ToString().Trim() %>' CommandName="DELETE" Text="&#xf00d;" />
                    </td> 
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
            </table>
            </FooterTemplate>
        </asp:Repeater>
        <div class="button-group ebiz-component-seats-buttons-wrap">
            <asp:Button id="btnChangeSeats" runat="server" CssClass="button ebiz-change-seats" />
            <asp:Button id="btnAddMoreSeats" runat="server" CssClass="button ebiz-change-seats" />
            <asp:Button id="btnUpdateSeats" runat="server" CssClass="button ebiz-update-seats" />
            <asp:Button id="btnProceed" runat="server" CssClass="button button ebiz-primary-action ebiz-proceed" />
        </div>
</asp:PlaceHolder>
