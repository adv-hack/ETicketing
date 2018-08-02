<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderEnquiry.ascx.vb"
    Inherits="UserControls_OrderEnquiry" %>



<div id="order-enquiry">
    <p class="instructions">
        <asp:Label ID="InstructionsLabel" runat="server"></asp:Label></p>
    <div>
        <asp:Label ID="OrderNoLabel" runat="server" AssociatedControlID="OrderNo"></asp:Label>
        <asp:TextBox ID="OrderNo" CssClass="input-l" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="FromDateLabel" runat="server" AssociatedControlID="FromDate"></asp:Label>
        <asp:TextBox ID="FromDate" CssClass="input-l datepicker" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="ToDateLabel" runat="server" AssociatedControlID="ToDate"></asp:Label>
        <asp:TextBox ID="ToDate" CssClass="input-l datepicker" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="StatusLabel" runat="server" AssociatedControlID="Status"></asp:Label>
        <asp:DropDownList ID="Status" CssClass="select" runat="server">
        </asp:DropDownList>
    </div>
    <div>
        <label>
            &nbsp;</label>
        <asp:Button ID="filterButton" CssClass="button" runat="server" />
    </div>
</div>
<div class="enquiry-results">
    <p class="pager-display">
        <asp:Label ID="CurrentResultsDisplaying" runat="server"></asp:Label></p>
    <p class="pager-nav">
        <asp:LinkButton ID="FirstTop" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav1Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav2Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav3Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav4Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav5Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav6Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav7Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav8Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav9Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav10Top" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="LastTop" runat="server" OnClick="ChangePage"></asp:LinkButton></p>
</div>
<div id="order-enquiry-matrix" class="default-table">
    <asp:Repeater ID="OrderHistoryView" runat="Server">
        <HeaderTemplate>
            <table cellspacing="0" summary="Order history">
                <tr>
                    <th class="order-no" scope="col">
                        <asp:Label ID="OrderIDHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="web-order-no" scope="col">
                        <asp:Label ID="WebOrderIDHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="order-date" scope="col">
                        <asp:Label ID="OrderDateHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="customer-ref" scope="col">
                        <asp:Label ID="CustomerRefHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="lines" scope="col">
                        <asp:Label ID="LinesHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <%--<th class="order-value" scope="col">
                        <asp:Label ID="OrderValueHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    --%>
                    <asp:PlaceHolder ID="plhShowOrderHeader1" runat="server" Visible="false" OnPreRender="CanDisplayThisColumn"><th class="order-value" scope="col">
                        <asp:Label ID="OrderValueHeader" runat="server" OnPreRender="GetText"></asp:Label></th></asp:PlaceHolder>                    
                    <th class="status" scope="col">
                        <asp:Label ID="OrderStatusHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="action" scope="col">
                        <asp:Label ID="ViewDetailsLinkHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="order-no">
                    <asp:Label ID="OrderID" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("BACK_OFFICE_ORDER_ID")) %>'></asp:Label></td>
                <td class="web-order-no">
                    <asp:Label ID="WebOrderID" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("PROCESSED_ORDER_ID")) %>'></asp:Label></td>
                <td class="order-date">
                    <asp:Label ID="OrderDate" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("CREATED_DATE")) %>'></asp:Label></td>
                <td class="customer-ref">
                    <asp:Label ID="CustomerRef" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem(GetCustomerRefField())) %>'></asp:Label></td>
                <td class="lines">
                    <asp:Label ID="Lines" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("LINES")) %>'></asp:Label></td>
                <%--<td class="order-value">
                    <asp:Label ID="OrderValue" runat="server" Text='<%# GetOrderValue(Container.DataItem("TOTAL_AMOUNT_CHARGED"))%>'></asp:Label></td>
                --%>
                <asp:PlaceHolder ID="plhShowOrderValue" runat="server" Visible="false" OnPreRender="CanDisplayThisColumn"><td class="order-value">
                    <asp:Label ID="OrderValue" runat="server" Text='<%# TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Talent.eCommerce.Utilities.CheckForDBNull_Decimal(Container.DataItem("TOTAL_AMOUNT_CHARGED")), 0.01, False), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("CURRENCY")), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("BUSINESS_UNIT")))%>'></asp:Label></td></asp:PlaceHolder>
                
                <td class="status">
                    <asp:Label ID="OrderStatus" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("STATUSTEXT")) %>'></asp:Label></td>
                <td class="action">
                    <asp:HyperLink ID="ViewDetailsLink" runat="server" OnLoad="GetText" OnPreRender="OpenOrderDetails"></asp:HyperLink></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div class="enquiry-results">
    <p class="pager-nav">
        <asp:LinkButton ID="FirstBottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav1Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav2Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav3Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav4Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav5Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav6Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav7Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav8Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav9Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="Nav10Bottom" runat="server" OnClick="ChangePage"></asp:LinkButton>
        <asp:LinkButton ID="LastBottom" runat="server" OnClick="ChangePage"></asp:LinkButton></p>
</div>
