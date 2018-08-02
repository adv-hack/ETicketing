<%@ Control Language="VB" AutoEventWireup="false" CodeFile="QuickOrder.ascx.vb" Inherits="UserControls_QuickOrder" %>
<div id="QuickOrderHolder">
    <script type="text/javascript">

        function CheckDeleteBasket() {

            if (confirm('Warning: Quick Order will replace your existing basket items. OK to continue?')) {
                var anchor = document.getElementById('ContentPlaceHolder1_QuickOrder1_addBtn');
                anchor.innerText = 'Processing';
                anchor.text = 'Processing';
                anchor.removeAttribute('href');
                anchor.removeAttribute('onClick');
                if (typeof (Page_ClientValidate) == 'function') {
                    var oldPage_IsValid = Page_IsValid; var oldPage_BlockSubmit = Page_BlockSubmit; if (Page_ClientValidate('') == false) { Page_IsValid = oldPage_IsValid; Page_BlockSubmit = oldPage_BlockSubmit; return false; }
                } document.getElementById("ContentPlaceHolder1_QuickOrder1_addBtn").value = 'Processing'; document.getElementById("ctl00_cphBody_btnSubmit").disabled = true; __doPostBack('ctl00$cphBody$ContentPlaceHolder1_QuickOrder1_addBtn', '');
                return true;
            }
            else {
                return false;
            }
        }
   

    </script>

    <asp:Label runat="server" id="lblIntro" Text=""></asp:Label>
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <div class="quickOrderErrorSummary alert-box alert">
            <asp:Label ID="errorSummary" runat="server" CssClass="error" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSuccessMessage" runat="server">
        <div class="alert-box success">
            <asp:Literal ID="ltlSuccessMessage" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhQuickOrderHomeDelivery" runat="server" Visible="false">
        <div class="quick-order-home-delivery">
            <asp:Label ID="lblQuickOrderHomeDelivery" runat="server" AssociatedControlID="chkQuickOrderHomeDelivery" />
            <asp:CheckBox ID="chkQuickOrderHomeDelivery" runat="server" AutoPostBack="true" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhPostCodeOptions" runat="server">
        <div class="quick-order-postcode">
            <p>
                <asp:Literal ID="ltlPostCodeSearch" runat="server" /></p>
            <asp:Label ID="lblPostCodeSearch" runat="server" AssociatedControlID="txtPostCodeSearch" />
            <asp:TextBox ID="txtPostCodeSearch" runat="server" MaxLength="10" />
            <asp:Button ID="btnPostCodeSearch" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhQuickOrderTable" runat="server">
        <div id="quickOrder" class="default-table">
            <asp:Table ID="QuickOrderTable" runat="server" EnableViewState="true" CellSpacing="0">
                <asp:TableHeaderRow runat="server">
                    <asp:TableHeaderCell ID="numberColumn" CssClass="number-col">
                        <asp:Label ID="srNoHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="productCodeColumn" CssClass="product-col">
                        <asp:Label ID="productCodeHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="quantityColumn" CssClass="qty-col">
                        <asp:Label ID="quantityHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="removeItemColumn" CssClass="remove-col">
                        <asp:Label ID="removeItemHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="unitPriceColumn" CssClass="unit-col">
                        <asp:Label ID="unitPriceHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="totalPriceColumn" CssClass="total-col">
                        <asp:Label ID="totalPriceHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="stockLevelColumn" CssClass="stock-col">
                        <asp:Label ID="stockLevelHeader" runat="server" /></asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="productDescriptionColumn" CssClass="desc-col">
                        <asp:Label ID="productDescriptionHeader" runat="server" /></asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhQuickOrderButtons" runat="server">
        <div id="quickOrderButtons">
            <div class="update">
                <asp:Button ID="updateBtn" runat="server" CssClass="button" />
            </div>
            <div class="add">
             <%--<asp:Button ID="addBtn2" runat="server" CssClass="button" OnClientClick="return CheckDeleteBasket();" />--%>
                <asp:Button ID="addBtn" runat="server" CssClass="button"  />
            </div>
            <asp:PlaceHolder ID="plhMoreRows" runat="server">
                <div class="more">
                    <asp:Button ID="moreRowsBtn" runat="server" CssClass="button" />
                </div>
                <div class="more-box">
                    <asp:TextBox ID="noOfRowsBox" runat="server" CssClass="input-s" MaxLength="2" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhDeliveryDate" runat="server" Visible="false">
        <div class="projected-delivery-date">
            <p>
                <asp:Literal ID="ltlProjectedDeliveryDateLabel" runat="server" /><span class="date"><asp:Literal
                    ID="ltlProjectedDeliveryDateValue" runat="server" /></span></p>
        </div>
    </asp:PlaceHolder>
</div>