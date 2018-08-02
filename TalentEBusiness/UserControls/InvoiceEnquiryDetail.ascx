<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InvoiceEnquiryDetail.ascx.vb" Inherits="UserControls_InvoiceEnquiryDetail" %>

<div class="InvoiceEnquiryDetailWrap1">
    <div class="InvoiceEnquiryDetailInvDate">
        <asp:Label ID="invDateLabel" CssClass="invDateLabel" runat="server"></asp:Label>
        <asp:Label ID="invDate" CssClass="invDate" runat="server"></asp:Label>
    </div>
    <div class="InvoiceEnquiryDetailCustAcc">
        <asp:Label ID="custAccLabel" CssClass="custAccLabel" runat="server"></asp:Label>
        <asp:Label ID="custAccNo" CssClass="custAccNo" runat="server"></asp:Label>
    </div>
    <div class="InvoiceEnquiryDetailCustRef">
        <asp:Label ID="custRefLabel" CssClass="custRefLabel" runat="server"></asp:Label>
        <asp:Label ID="custRef" CssClass="custRef" runat="server"></asp:Label>
    </div>
    <div class="InvoiceEnquiryDetailInvAddress">
        <asp:Label ID="invAddressLabel" CssClass="invAddressLabel" runat="server"></asp:Label><br />
        <asp:Label ID="invName" CssClass="invName" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd1" CssClass="invAdd1" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd2" CssClass="invAdd2" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd3" CssClass="invAdd3" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd4" CssClass="invAdd4" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd5" CssClass="invAdd5" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd6" CssClass="invAdd6" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd7" CssClass="invAdd7" runat="server"></asp:Label>
    </div>
    <div class="InvoiceEnquiryDetailDelAddress">
        <asp:Label ID="delAddressLabel" CssClass="delAddressLabel" runat="server"></asp:Label><br />
        <asp:Label ID="delName" CssClass="delName" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd1" CssClass="delAdd1" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd2" CssClass="delAdd2" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd3" CssClass="delAdd3" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd4" CssClass="delAdd4" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd5" CssClass="delAdd5" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd6" CssClass="delAdd6" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd7" CssClass="delAdd7" runat="server"></asp:Label>
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
<div id="invoice-enquiry-matrix">
    <asp:Repeater ID="InvoiceHistoryView" runat="Server">
        <HeaderTemplate>
            <table cellspacing="0" summary="Invoice history">
                <tr>
                    <th class="invoice-no" scope="col">
                        <asp:Label ID="InvoiceIDHeader" runat="server" OnPreRender="GetText" ></asp:Label></th>
                    <th class="prod-code" scope="col">
                        <asp:Label ID="ProductCodeHeader" runat="server" OnPreRender="GetText"> </asp:Label></th>
                    <th class="prod-desc" scope="col">
                        <asp:Label ID="DescriptionHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="prod-price" scope="col">
                        <asp:Label ID="PriceHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="quantity" scope="col">
                        <asp:Label ID="QuantityHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="value" scope="col">
                        <asp:Label ID="ValueHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="invoice-no">
                    <asp:Label ID="InvoiceID" runat="server" Text='<%# Container.DataItem("InvoiceNumber") %>'></asp:Label></td>
                <td class="prod-code">
                    <asp:Label ID="ProductCode" runat="server" Text='<%# Container.DataItem("ProductCode") %>'></asp:Label></td>
                <td class="prod-desc">
                    <asp:Label ID="Description" runat="server" Text='<%# Container.DataItem("Description1") %>'></asp:Label></td>
                <td class="prod-price">
                    <asp:Label ID="Price" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("ProductPrice"), 0.01, False))%>'></asp:Label></td>
                <td class="quantity">
                    <asp:Label ID="Quantity" runat="server" Text='<%# Container.DataItem("QuantityInvoiced")%>'></asp:Label></td>
                 <td class="value">
                    <asp:Label ID="Value" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(CalcLineValue(Container.DataItem("ProductPrice") * Container.DataItem("QuantityInvoiced"), Container.DataItem("InvoiceLineNetAmount")), 0.01, False))%>'></asp:Label>
                </td>
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
<div class="InvoiceEnquiryDetailSummary">
    <asp:Label ID="subTotalLabel" CssClass="subTotalLabel" runat="server"></asp:Label>
    <asp:Label ID="subTotal" CssClass="subTotal" runat="server"></asp:Label>
    <br />
    <asp:Label ID="vatLabel" CssClass="vatLabel" runat="server"></asp:Label>
    <asp:Label ID="vat" CssClass="vat" runat="server"></asp:Label>
    <br />
    <asp:Label ID="totalLabel" CssClass="totalLabel" runat="server"></asp:Label>
    <asp:Label ID="total" CssClass="total" runat="server"></asp:Label><br />
    <br />
    <asp:Button ID="btnCreatePDF" CssClass="button emailPDF" runat="server" Text="Email PDF" />
    <asp:Label ID="FeedbackLabel" CssClass="FeedbackLabel" runat="server" />
</div>