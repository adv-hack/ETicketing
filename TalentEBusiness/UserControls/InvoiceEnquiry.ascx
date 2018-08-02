<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InvoiceEnquiry.ascx.vb"
    Inherits="UserControls_InvoiceEnquiry" %>

<script language="javascript" type="text/javascript">
    /// <summary>
    /// Launches the DatePicker page in a popup window,
    /// passing a JavaScript reference to the field that we want to set.
    /// </summary>
    /// <param name="strField">String. The JavaScript reference to the field that we want to set,
    /// in the format: FormName.FieldName
    /// Please note that JavaScript is case-sensitive.</param>
    function calendarPicker(strField)
    {
        window.open('../../PagesPublic/DatePicker.aspx?field=' + strField, 'calendarPopup', 'width=250,height=190,resizable=yes');
    }
</script>

<div id="order-enquiry">
    <p class="instructions">
        <asp:Label ID="InstructionsLabel" runat="server"></asp:Label></p>
    <div>
        <asp:Label ID="InvoiceNoLabel" runat="server" AssociatedControlID="InvoiceNo"></asp:Label>
        <asp:TextBox ID="InvoiceNo" CssClass="input-l" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="OrderNoLabel" runat="server" AssociatedControlID="OrderNo"></asp:Label>
        <asp:TextBox ID="OrderNo" CssClass="input-l" runat="server"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="FromDateLabel" runat="server" AssociatedControlID="FromDate"></asp:Label>
        <asp:TextBox ID="FromDate" CssClass="input-l" runat="server"></asp:TextBox>
        <asp:Button ID="FromDateLinkButton" runat="server" CssClass="button selectDate"></asp:Button>
    </div>
    <div>
        <asp:Label ID="ToDateLabel" runat="server" AssociatedControlID="ToDate"></asp:Label>
        <asp:TextBox ID="ToDate" CssClass="input-l" runat="server"></asp:TextBox>
        <asp:Button ID="ToDateLinkButton" runat="server" CssClass="button selectDate"></asp:Button>
    </div>
    <div id="statusDiv" runat="server">
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
<div id="invoice-enquiry-matrix" class="default-table">
    <asp:Repeater ID="InvoiceHistoryView" runat="Server">
        <HeaderTemplate>
            <table cellspacing="0" summary="Invoice history">
                <tr>
                    <th class="invoice-no" scope="col">
                        <asp:Label ID="InvoiceIDHeader" runat="server" OnPreRender="GetText" ></asp:Label></th>
                    <th class="order-no" scope="col">
                        <asp:Label ID="OrderIDHeader" runat="server" OnPreRender="GetText"> </asp:Label></th>
                    <th class="invoice-date" scope="col">
                        <asp:Label ID="InvoiceDateHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="customer-ref" scope="col">
                        <asp:Label ID="CustomerRefHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="invoice-value" scope="col">
                        <asp:Label ID="InvoiceValueHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="outstanding-value" scope="col">
                        <asp:Label ID="OutstandingValueHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                    <th class="action" scope="col">
                        <asp:Label ID="ViewDetailsLinkHeader" runat="server" OnPreRender="GetText"></asp:Label></th>
                
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="invoice-no">
                    <asp:Label ID="InvoiceID" runat="server" Text='<%# Container.DataItem("InvoiceNumber") %>'></asp:Label></td>
                <td class="order-no">
                    <asp:Label ID="OrderID" runat="server" Text='<%# Container.DataItem("OrderNumber") %>'></asp:Label></td>
                <td class="invoice-date">
                    <asp:Label ID="InvoiceDate" runat="server" Text='<%# Container.DataItem("InvoiceDateTime") %>'></asp:Label></td>
                <td class="customer-ref">
                    <asp:Label ID="CustomerRef" runat="server" Text='<%# Container.DataItem("LoginID") %>'></asp:Label></td>
                <td class="invoice-value">
                    <asp:Label ID="InvoiceValue" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("InvoiceAmount"), 0.01, False))%>'></asp:Label></td>
                <td class="outstanding-value">
                    <asp:Label ID="OutstandingValue" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("OutstandingAmount"), 0.01, False))%>'></asp:Label></td>
                <td class="action">
                    <asp:HyperLink ID="ViewDetailsLink" runat="server" OnLoad="GetText" OnPreRender="OpenInvoiceDetails"></asp:HyperLink></td>                
                
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
