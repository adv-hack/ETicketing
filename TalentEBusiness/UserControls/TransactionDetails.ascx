<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TransactionDetails.ascx.vb"
    Inherits="UserControls_TransactionDetails" %>
<div id="transaction-payment">
    <table summary="Payment Details" id="payment-details" cellspacing="0">
        <tr>
            <th scope="col">
                <asp:Label ID="PaymentRefLabel" runat="server" OnPreRender="GetText"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentRef" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="col">
                <asp:Label ID="PaymentTypeLabel" runat="server" OnPreRender="GetText"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="col">
                <asp:Label ID="PaymentDateLabel" runat="server" OnPreRender="GetText"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentDate" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="col">
                <asp:Label ID="PaymentValueLabel" runat="server" OnPreRender="GetText"></asp:Label>
            </th>
            <td>
                <asp:Label ID="PaymentValue" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</div>
<div id="transaction-products">
    <asp:Repeater ID="rptProductsList" runat="server">
        <HeaderTemplate>
            <table id="productsList" summary="Products List">
                <tr>
                    <th class="product-name" scope="col">
                        <asp:Label ID="ProductNameHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="product-desc" scope="col">
                        <asp:Label ID="ProductDescHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="seat" scope="col">
                        <asp:Label ID="SeatHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="member" scope="col">
                        <asp:Label ID="MemberNumberHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="price" scope="col">
                        <asp:Label ID="PriceHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="product-date" scope="col">
                        <asp:Label ID="ProductDateHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="product-name">
                    <asp:Label ID="ProductName" runat="server" Text='<%# Container.DataItem("PRODUCTCODE") %>'></asp:Label>
                </td>
                <td class="product-desc">
                    <asp:Label ID="ProductDesc" runat="server" Text='<%# Container.DataItem("PRODUCTDESCRIPTION") %>'></asp:Label>
                </td>
                <td class="seat">
                    <asp:Label ID="Seat" runat="server" Text='<%# Container.DataItem("SEAT") %>'></asp:Label>
                </td>
                <td class="member">
                    <asp:Label ID="Member" runat="server" Text='<%# Container.DataItem("CUSTOMERNO") %>'></asp:Label>
                </td>
                <td class="price">
                    <asp:Label ID="Price" runat="server" Text='<%# Container.DataItem("PRICE") %>'></asp:Label>
                </td>
                <td class="product-date">
                    <asp:Label ID="ProductDate" runat="server" Text='<%# GetFormattedProductDate(Container.DataItem("PRODUCTDATE")) %>'></asp:Label>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr>
                <td class="product-name">
                    <asp:Label ID="ProductName" runat="server" Text='<%# Container.DataItem("PRODUCTCODE") %>'></asp:Label>
                </td>
                <td class="product-desc">
                    <asp:Label ID="ProductDesc" runat="server" Text='<%# Container.DataItem("PRODUCTDESCRIPTION") %>'></asp:Label>
                </td>                
                <td class="seat">
                    <asp:Label ID="Seat" runat="server" Text='<%# Container.DataItem("SEAT") %>'></asp:Label>
                </td>
                <td class="member">
                    <asp:Label ID="Member" runat="server" Text='<%# Container.DataItem("CUSTOMERNO") %>'></asp:Label>
                </td>
                <td class="price">
                    <asp:Label ID="Price" runat="server" Text='<%# Container.DataItem("PRICE") %>'></asp:Label>
                </td>
                <td class="product-date">
                    <asp:Label ID="ProductDate" runat="server" Text='<%# GetFormattedProductDate(Container.DataItem("PRODUCTDATE")) %>'></asp:Label>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
