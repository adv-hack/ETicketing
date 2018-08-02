<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CreditNoteEnquiryDetail.ascx.vb"
    Inherits="UserControls_CreditNoteEnquiryDetail" %>

<div>
    <div>
        <asp:Label ID="Label1" runat="server"></asp:Label><br />
        <asp:Label ID="Label2" runat="server"></asp:Label><br />
        <asp:Label ID="Label3" runat="server"></asp:Label><br />
        <asp:Label ID="Label4" runat="server"></asp:Label><br />
        <asp:Label ID="Label5" runat="server"></asp:Label><br />
        <asp:Label ID="Label6" runat="server"></asp:Label><br />
        <asp:Label ID="Label7" runat="server"></asp:Label><br />
        <asp:Label ID="Label8" runat="server"></asp:Label><br />
        <asp:Label ID="Label9" runat="server"></asp:Label>
    </div>
    <div>
        <asp:Label ID="invAddressLabel" runat="server"></asp:Label><br />
        <asp:Label ID="invName" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd1" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd2" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd3" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd4" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd5" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd6" runat="server"></asp:Label><br />
        <asp:Label ID="invAdd7" runat="server"></asp:Label>
    </div>
    <br />
    <div>
        <asp:Label ID="delAddressLabel" runat="server"></asp:Label><br />
        <asp:Label ID="delName" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd1" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd2" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd3" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd4" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd5" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd6" runat="server"></asp:Label><br />
        <asp:Label ID="delAdd7" runat="server"></asp:Label>
    </div>
    <div>
        <div>
            <asp:Label ID="invNoLabel" runat="server"></asp:Label>
            <asp:Label ID="invNo" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="invDateLabel" runat="server"></asp:Label>
            <asp:Label ID="invDate" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="custOrderNoLabel" runat="server"></asp:Label>
            <asp:Label ID="custOrderNo" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="custAccLabel" runat="server"></asp:Label>
            <asp:Label ID="custAccNo" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="buRefLabel" runat="server"></asp:Label>
            <asp:Label ID="buRef" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="despDateLabel" runat="server"></asp:Label>
            <asp:Label ID="despDate" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="salesAreaLabel" runat="server"></asp:Label>
            <asp:Label ID="salesArea" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="shipNoLabel" runat="server"></asp:Label>
            <asp:Label ID="shipNo" runat="server"></asp:Label>
        </div>
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
    <asp:Repeater ID="CreditNoteHistoryView" runat="Server">
        <HeaderTemplate>
            <table cellspacing="0" summary="CreditNote history">
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
                    <asp:Label ID="InvoiceID" runat="server" Text='<%# Container.DataItem("CreditNoteNumber") %>'></asp:Label></td>
                <td class="prod-code">
                    <asp:Label ID="ProductCode" runat="server" Text='<%# Container.DataItem("ProductCode") %>'></asp:Label></td>
                <td class="prod-desc">
                    <asp:Label ID="Description" runat="server" Text='<%# Container.DataItem("Description1") %>'></asp:Label></td>
                <td class="prod-price">
                    <asp:Label ID="Price" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("ProductPrice"), 0.01, False))%>'></asp:Label></td>
                <td class="quantity">
                    <asp:Label ID="Quantity" runat="server" Text='<%# Container.DataItem("QuantityInvoiced")%>'></asp:Label></td>
                 <td class="value">
                    <asp:Label ID="Value" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("InvoiceLineNetAmount"), 0.01, False))%>'></asp:Label></td>
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
<div>
    <asp:Label ID="subTotalLabel" runat="server"></asp:Label>
    <asp:Label ID="subTotal" runat="server"></asp:Label>
    <br />
    <asp:Label ID="vatLabel" runat="server"></asp:Label>
    <asp:Label ID="vat" runat="server"></asp:Label>
    <br />
    <asp:Label ID="totalLabel" runat="server"></asp:Label>
    <asp:Label ID="total" runat="server"></asp:Label>
    <br />
</div>