<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderEnquiry2.ascx.vb" Inherits="UserControls_OrderEnquiry2" %>
<script type="text/javascript">
    $(function () { $(".accordion").accordion({ collapsible: true, active: false, heightStyle: "content", icons: false }); });
</script>

<asp:PlaceHolder ID="plhError" runat="server" Visible="false">
    <span class="error"><asp:Literal ID="ltlError" runat="server" Visible="true" /></span>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhFilters" runat="server" Visible="True">
  <div class="panel filter">
    <fieldset>
      <legend></legend>
      <asp:PlaceHolder ID="plhPayRefFilterField" runat="server" Visible="false">
        <div class="row pay-ref">
          <div class="large-4 columns">
            <asp:Label ID="AgentLabel" runat="server" />
            <asp:Label ID="PayRefLabel" runat="server" AssociatedControlID="txtPayRef" />
          </div>
          <div class="large-4 columns">
            <asp:TextBox ID="txtPayRef" runat="server" MaxLength="15" />
          </div>
          <div class="large-4 columns">
            <asp:RegularExpressionValidator ID="rgxPaymentReference" runat="server" ControlToValidate="txtPayRef" Display="Static" CssClass="error" SetFocusOnError="true" ValidationExpression="^[0-9]{1,15}$" />
          </div>
        </div>
      </asp:PlaceHolder>
      <div class="row from-date">
        <div class="large-4 columns">
          <asp:Label ID="FromDateLabel" runat="server" AssociatedControlID="FromDate" />
        </div>
        <div class="large-4 columns">
          <asp:TextBox ID="FromDate" CssClass="datepicker" runat="server" />
        </div>
        <div class="large-4 columns">
          <asp:CompareValidator ID="cvdFromDate" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="FromDate"></asp:CompareValidator>
          <asp:Label ID="lblFromDateValue" runat="server" Visible="false" />
        </div>
      </div>
      <div class="row to-date">
        <div class="large-4 columns">
          <asp:Label ID="ToDateLabel" runat="server" AssociatedControlID="ToDate" />
        </div>
        <div class="large-4 columns">
          <asp:TextBox ID="ToDate" CssClass="datepicker" runat="server" />
        </div>
        <div class="large-4 columns">
          <asp:CompareValidator ID="cvdToDate" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="ToDate"></asp:CompareValidator>
          <asp:Label ID="lblToDateValue" runat="server" Visible="false" />
        </div>
      </div>
      <div class="row status">
        <div class="large-4 columns">
          <asp:Label ID="StatusLabel" runat="server" AssociatedControlID="Status" />
        </div>
        <div class="large-4 pull-4 columns">
          <asp:DropDownList ID="Status" runat="server" />
        </div>
      </div>
      <div class="row product-type">
        <div class="large-4 columns">
            <asp:Label ID="lblProductType" runat="server" AssociatedControlID="ddlProductType" />
        </div>
        <div class="large-4 pull-4 columns">
            <asp:DropDownList ID="ddlProductType" runat="server" />
        </div>
      </div>
      <div class="row corporate-products">
        <div class="large-4 columns">
          <asp:Label ID="lblShowCorporateProducts" runat="server" AssociatedControlID="chkShowCorporateProducts" />
        </div>
        <div class="large-4 pull-4 columns">
          <asp:CheckBox ID="chkShowCorporateProducts" runat="server" />
        </div>
      </div>
      <div class="row actions">
        <div class="large-8 push-4 columns">
          <asp:Button ID="filterButton" CssClass="button filter-btn" runat="server" />
          <asp:Button ID="btnClear" CssClass="button clear-btn" OnClientClick="return ClearFields()" runat="server" Visible="true" />
        </div>
      </div>
    </fieldset>
  </div>
</asp:PlaceHolder>

<asp:Panel ID="orderPaymentView" runat="server" Visible="false" CssClass="payment-order-enquiry">
  <asp:PlaceHolder ID="plhPaymentOwnerDetails" runat="server" Visible="false">
    <div id="divPaymentOwnerDetails">
      <div id="divPaymentOwnerDetailsTitle"><span class="title"><asp:Literal ID="ltlPaymentOwnerDetailsTitle" runat="server" /></span></div>
      <div id="divPaymentOwnerDetailsData">
        <ul class="ulPaymentOwnerDetailsData">
          <li><span class="value"><asp:Literal ID="ltlPayOwnName" runat="server" /></span></li>
          <li><span class="value"><asp:Literal ID="ltlPayOwnAddressLine1" runat="server" /></span></li>
          <li><span class="value"><asp:Literal ID="ltlPayOwnAddressLine2" runat="server" /></span></li>
          <li><span class="value"><asp:Literal ID="ltlPayOwnAddressLine3" runat="server" /></span></li>
          <li><span class="value"><asp:Literal ID="ltlPayOwnAddressLine4" runat="server" /></span></li>
          <li><span class="value"><asp:Literal ID="ltlPayOwnPostCodePart1" runat="server" /><asp:Literal ID="ltlPayOwnPostCodePart2" runat="server" /></span></li>
        </ul>
      </div>
    </div>
  </asp:PlaceHolder>

  <asp:PlaceHolder ID="plhPurchaseDetails" runat="server" Visible="false">
    <div id="divPurchaseDetails">
      <div id="divPurchaseDetailsTitle"><span class="title"><asp:Literal ID="ltlPurchaseDetailsTitle" runat="server" /></span></div>
      <div id="divPurchaseDetailsData">
        <ul class="ulPurchaseDetailsData">
          <li>
            <span class="tag"><asp:Literal ID="ltlPurDetPayReferenceLabel" runat="server" /></span>
            <span class="value"><asp:Literal ID="ltlPurDetPayReferenceValue" runat="server" /></span>
          </li>
          <asp:PlaceHolder ID="plhAgentName" runat="server">
          <li>
            <span class="tag"><asp:Literal ID="ltlPurDetOriginSourceLabel" runat="server" /></span>
            <span class="value"><asp:Literal ID="ltlPurDetOriginSourceValue" runat="server" /></span>
          </li>
          </asp:PlaceHolder>
          <li>
            <span class="tag"><asp:Literal ID="ltlPurDetTranDateLabel" runat="server" /></span>
            <span class="value"><asp:Literal ID="ltlPurDetTranDateValue" runat="server" /></span>
          </li>
          <asp:PlaceHolder ID="plhBatchRef" runat="server">
          <li>
            <span class="tag"><asp:Literal ID="ltlPurDetBatchLabel" runat="server" /></span>
            <span class="value"><asp:Literal ID="ltlPurDetBatchValue" runat="server" /></span>
          </li>
          </asp:PlaceHolder>
        </ul>
      </div>
    </div>
  </asp:PlaceHolder>

  <asp:PlaceHolder ID="plhPaymentDetails" runat="server" Visible="false">
    <div id="divPaymentDetails">
      <div id="divPaymentDetailsTitle"><span class="title"><asp:Literal ID="ltlPaymentDetailsTitle" runat="server" /></span></div>
      <div id="divPaymentDetailsData">
        <asp:Repeater ID="rptPaymentDetails" runat="server">
          <HeaderTemplate>
            <table summary="Payment Details" class="tablepaydetails">
                <tr>
                    <th class="paytype" scope="col"> <asp:Literal ID="ltlPayDetPayTypeLabel" runat="server" /></th>
                    <th class="payamount" scope="col"> <asp:Literal ID="ltlPayDetPayAmountLabel" runat="server" /></th>
                    <th class="paytypedetail" scope="col"> <asp:Literal ID="ltlPayTypeDetailLabel" runat="server" /></th>
                </tr>
          </HeaderTemplate>
          <ItemTemplate>
                <tr class="paydetailrow">
                    <td class="paytype"><asp:Literal ID="ltlPayDetPayTypeValue" runat="server" /></td>
                    <td class="payamount"><asp:Literal ID="ltlPayDetPayAmountValue" runat="server" /></td>
                    <td class="paytypedetail">
                        <asp:PlaceHolder ID="plhPayTypeDetails" runat="server" Visible="true">
                            <ul class="ulPayTypeData"><asp:Literal ID="ltlPayTypeDetail" runat="server" /></ul>
                        </asp:PlaceHolder>
                    </td>
                </tr>
          </ItemTemplate>
          <AlternatingItemTemplate>
                <tr class="paydetailaltrow">
                    <td class="paytype"><asp:Literal ID="ltlPayDetPayTypeValue" runat="server" /></td>
                    <td class="payamount"><asp:Literal ID="ltlPayDetPayAmountValue" runat="server" /></td>
                    <td class="paytypedetail">
                        <asp:PlaceHolder ID="plhPayTypeDetails" runat="server" Visible="true">
                            <ul class="ulPayTypeData"><asp:Literal ID="ltlPayTypeDetail" runat="server" /></ul>
                        </asp:PlaceHolder>
                    </td>
                </tr>
          </AlternatingItemTemplate>
          <FooterTemplate>
            </table>
          </FooterTemplate>
        </asp:Repeater>
      </div>
    </div>
  </asp:PlaceHolder>
</asp:Panel>

<asp:Panel ID="orderPromotionView" runat="server" Visible="true" CssClass="panel promotions">
  <asp:Label ID="promoTitle" runat="server" CssClass="subheader" />
  <div id="promoContentWrap" runat="server" class="promo-content">
    <asp:Label ID="promoIntro" runat="server" CssClass="promo-intro" />
    <div id="promoSelectWrap" runat="server" class="promo-select">
      <asp:Label ID="promoLabel" runat="server" CssClass="label" />
      <asp:DropDownList ID="promoDDL" runat="server" AutoPostBack="true" CssClass="select" />
    </div>
    <asp:PlaceHolder ID="plhPromotionDetails" runat="server" Visible="false">
      <div id="promoDescWrap" runat="server" class="promo-desc">
        <asp:Label ID="descLabel" runat="server" CssClass="label" />
        <asp:Label ID="desc" runat="server" CssClass="value" />
      </div>
      <div id="promoCompWrap" runat="server" class="promo-comp">
        <asp:Label ID="compLabel" runat="server" CssClass="label" />
        <asp:Label ID="comp" runat="server" CssClass="value" />
      </div>
      <div id="promoStartWrap" runat="server" class="promo-start">
        <asp:Label ID="startLabel" runat="server" CssClass="label" />
        <asp:Label ID="startDate" runat="server" CssClass="value" />
      </div>
      <div id="promoEndWrap" runat="server" class="promo-end">
        <asp:Label ID="endLabel" runat="server" CssClass="label" />
        <asp:Label ID="endDate" runat="server" CssClass="value" />
      </div>
      <div id="promoMaxWrap" runat="server" class="promo-max">
        <asp:Label ID="maxLabel" runat="server" CssClass="label" />
        <asp:Label ID="max" runat="server" CssClass="value" />
      </div>
      <div id="promoCurrentWrap" runat="server" class="promo-current">
        <asp:Label ID="currentLabel" runat="server" CssClass="label" />
        <asp:Label ID="current" runat="server" CssClass="value" />
      </div>
    </asp:PlaceHolder>
  </div>
  <div id="noPromoWrap" runat="server" class="no-promotions">
    <asp:Label ID="noPromoLabel" runat="server" />
  </div>
</asp:Panel>

<asp:Panel ID="orderOnAccountView" runat="server" Visible="true" CssClass="panel on-account">
  <asp:Label ID="onaccountTitle" runat="server" CssClass="subheader" />
  <div id="onaccountContentWrap" runat="server" class="content">
    <asp:Label ID="onAccountIntro" runat="server" CssClass="introduction" />
    <asp:Panel ID="cashbackView" runat="server" Visible="true">
      <div class="cashback-info">
        <asp:Label ID="cashbackTitle" runat="server" CssClass="cashback-title" />
        <div id="cashbackContentWrap" runat="server" class="cashback-content-wrap">
          <asp:Label ID="cashbackIntro" runat="server" />
          <div id="cashbackBankedWrap" runat="server">
            <asp:Label ID="cashbackBankedLabel" runat="server" CssClass="cashback-label" />
            <asp:Label ID="cashbackBanked" runat="server" />
          </div>
          <div id="cashbackUnbankedWrap" runat="server">
            <asp:Label ID="cashbackUnbankedLabel" runat="server" CssClass="cashback-label" />
            <asp:Label ID="cashbackUnbanked" runat="server" />
          </div>
          <div id="cashbackSpentWrap" runat="server">
            <asp:Label ID="cashbackSpentLabel" runat="server" CssClass="cashback-label" />
            <asp:Label ID="cashbackSpent" runat="server" />
          </div>
        </div>
        <div id="noCashbackWrap" runat="server">
          <asp:Label ID="noCashbackLabel" runat="server" />
        </div>
      </div>
      </asp:Panel>
  </div>
</asp:Panel>

<asp:Panel ID="orderReturnView" runat="server" Visible="true" CssClass="view">
    <asp:Label ID="orderReturnTitle" runat="server" CssClass="subheader" />
    <div id="orderReturnContentWrap" runat="server" class="order-return">
    <asp:Label ID="orderReturnIntro" runat="server" CssClass="introduction" />
    <div id="orderReturnBankedWrap" runat="server" class="order-return-banked">
        <asp:Label ID="orderReturnBankedLabel" runat="server" CssClass="label" />
        <asp:Label ID="orderReturnBanked" runat="server" CssClass="value" />
    </div>
    <div id="orderReturnRewardWrap" runat="server" class="order-return-reward">
        <asp:Label ID="orderReturnRewardLabel" runat="server" CssClass="label" />
        <asp:Label ID="orderReturnReward" runat="server" CssClass="value" />
    </div>
    </div>
    <div id="noOrderReturnWrap" runat="server" class="no-order-return">
    <asp:Label ID="noOrderReturnLabel" runat="server" />
    </div>
</asp:Panel>

<asp:PlaceHolder ID="plhPrintAll" runat="server">
    <div class="print-all">
        <asp:Button ID="btnPrintAll" runat="server" CssClass="button" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhCancelAll" runat="server" Visible="false">
    <div id="divCancelAll">
        <asp:Label ID="lblCancelAllMsg" CssClass="cancel-all-msg" runat="server" />
        <asp:HyperLink ID="hlnkCancelAll" runat="server"></asp:HyperLink>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhProductSaleDetails" runat="server" Visible="true">
  <asp:PlaceHolder ID="plhTopPager" runat="server">
    <%--Pager Top section--%>
    <div class="graphical-pager">
      <p class="pager-display">
        <span class="display">
            <asp:Label ID="displayingLabelT" runat="server" Text="Displaying" />
            <asp:Label ID="displayingValueLabelT" runat="server" Text="1" />
            <asp:Label ID="toLabelT" runat="server" Text="to" />
            <asp:Label ID="toValueLabelT" runat="server" Text="5" />
            <asp:Label ID="ofLabelT" runat="server" Text="of" />
            <asp:Label ID="ofValueLabelT" runat="server" Text="6" />
        </span>|
        <span class="nav">
            <asp:HyperLink ID="LnkFirstT" runat="server" Text="First"></asp:HyperLink>
            <asp:HyperLink ID="LnkPrevT" runat="server" Text="Prev"></asp:HyperLink>
            <asp:Label ID="LinksLabelT" runat="server" />
            <asp:HyperLink ID="LnkNextT" runat="server" Text="Next"></asp:HyperLink>
            <asp:HyperLink ID="LnkLastT" runat="server" Text="Last"></asp:HyperLink>
        </span>
      </p>
    </div>
  </asp:PlaceHolder>

  
  <asp:Repeater ID="OrderHistoryRepeater" runat="server">
    <HeaderTemplate>
        <div class="order-enquiry accordion">
    </HeaderTemplate>
      <ItemTemplate>
        <div class="header">
          <ul>
            <li class="date"><span class="tag">
                <%#GetText("dateLabel")%></span> <span class="value">
                    <%#showDate(DataBinder.Eval(Container.DataItem, "SaleDate"))%></span></li>
              <li class="membership-match"><span class="tag">
                  <%#GetText("membershipMatchLabel")%></span> <span class="value">
                      <%#DataBinder.Eval(Container.DataItem, "ProductDescription")%></span></li>
              <li class="pay-ref"><span class="tag">
                  <%#GetText("payRefLabel")%></span> <span class="value">
                      <%# DataBinder.Eval(Container.DataItem,"PaymentReference").ToString.TrimStart("0")%></span></li>
              <li class="seat-item"><span class="tag">
                  <%#GetText("seatLabel")%></span><span class="value"><%# CheckSeatDetailsForAttendance(DataBinder.Eval(Container.DataItem, "Seat").ToString, DataBinder.Eval(Container.DataItem, "ProductType").ToString)%></span></li>
          </ul>
        </div>
          <div class="content">
              <ul class="details">
                  <li class="customer-number" id="CustomerNoRow" runat="server" onprerender="CATColumnPreRender">
                      <span class="tag">
                          <%# GetText("customerNumberLabel")%></span><span class="value"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString.TrimStart("0")%></span></li>
                  <li class="customer-name" id="CustomerNameRow" runat="server" onprerender="CATColumnPreRender">
                      <span class="tag">
                          <%# GetText("customerNameLabel")%></span><span class="value"><%# DataBinder.Eval(Container.DataItem, "CustomerName").ToString.TrimStart("0")%></span></li>
                  <li class="fees-item"><span class="tag">
                      <%#GetText("FeesLabel")%></span><span class="value"><%# DataBinder.Eval(Container.DataItem, "FeesCode")%></span>
                  </li>
                  <li class="price-item"><span class="tag">
                      <%# GetText("priceLabel")%></span><span class="value"><%# FormatCurrency(DataBinder.Eval(Container.DataItem, "SalePrice"))%>
                          <asp:HyperLink ID="hlpPromotion" runat="server" CssClass="promotion-link" Visible='<%# ShowPromotion(DataBinder.Eval(Container.DataItem, "PromotionId").ToString()) %>'
                              NavigateUrl='<%# GetNavigateUrl(DataBinder.Eval(Container.DataItem, "PromotionId").ToString(), DataBinder.Eval(Container.DataItem, "OriginalPrice").ToString(), DataBinder.Eval(Container.DataItem, "SalePrice").ToString()) %>'>
                              <asp:Image ID="imgPromotion" runat="server" CssClass="promotion-image" />
                          </asp:HyperLink>
                      </span></li>
                  <li class="price-band-item"><span class="tag">
                      <%# GetText("priceBandLabel")%></span><span class="value"><%# DataBinder.Eval(Container.DataItem, "PriceBandDesc")%></span></li>
                  <li class="status-item"><span class="tag">
                      <%#GetText("statusLabel")%></span><span class="value"><%# GetStatusText(Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "StatusCode")))%></span></li>
                  <li class="loyalty-item"><span class="tag">
                      <%#GetText("loyaltyLabel")%></span><span class="value"><%# DataBinder.Eval(Container.DataItem, "LoyaltyPoints").ToString.TrimStart("0") & CheckIfLoyaltyPointsExpired(Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(DataBinder.Eval(Container.DataItem, "LoyaltyPointsExpired")))%></span></li>
              </ul>
              <ul class="actions">
                  <li class="transfer" id="TransferRow" runat="server" onprerender="TransferColumnPreRender">
                      <asp:HyperLink ID="hlnkTransfer" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                  </li>
                  <li class="amend" id="AmendRow" runat="server" onprerender="AmendColumnPreRender">
                      <asp:HyperLink ID="hlnkAmend" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                  </li>
                  <li class="cancel" id="CancelRow" runat="server" onprerender="CancelColumnPreRender">
                      <asp:HyperLink ID="hlnkCancel" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                  </li>
                  <li class="print" id="PrintRow" runat="server" onprerender="PrintColumnPreRender">
                      <asp:Button ID="btnPrint" runat="server" CssClass="button" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "RRN").ToString %>'
                          CommandName="PrintThisTicket" />
                  </li>
                  <asp:PlaceHolder ID="plhSeatHistory" runat="server">
                      <li class="seat-history">
                          <asp:HyperLink ID="hlkSeatHistory" runat="server" CssClass="button" NavigateUrl='<%# GetSeatHistoryUrl(DataBinder.Eval(Container.DataItem, "ProductCode").ToString(), DataBinder.Eval(Container.DataItem, "Seat").ToString(), DataBinder.Eval(Container.DataItem, "PaymentReference").ToString()) %>'> <%#GetText("SeatHistoryLabel")%> </asp:HyperLink>
                      </li>
                  </asp:PlaceHolder>
                  <asp:PlaceHolder ID="plhSeatPrintHistory" runat="server">
                      <li class="seat-print-history">
                          <asp:HyperLink ID="hlkSeatPrintHistory" runat="server" CssClass="button" NavigateUrl='<%# GetSeatPrintHistoryUrl(DataBinder.Eval(Container.DataItem, "ProductCode").ToString(), DataBinder.Eval(Container.DataItem, "Seat").ToString()) %>'> <%#GetText("SeatPrintHistoryLabel")%> </asp:HyperLink>
                      </li>
                  </asp:PlaceHolder>
                  <asp:PlaceHolder ID="plhPaymentDetails" runat="server">
                      <li class="seat-print-history">
                          <asp:HyperLink ID="hlkPaymentDetails" runat="server" CssClass="button" NavigateUrl='<%# GetPaymenDetailsUrl(DataBinder.Eval(Container.DataItem, "PaymentReference").ToString())%>'> <%#GetText("PaymentDetailsLabel")%> </asp:HyperLink>
                      </li>
                  </asp:PlaceHolder>
              </ul>
          </div>
      </ItemTemplate>
      <FooterTemplate>
          </div>
      </FooterTemplate>
  </asp:Repeater>

  <asp:Repeater ID="TransactionHistoryRepeater" runat="server">
    <HeaderTemplate>
        <div class="transaction-details">
            <table>
                <thead>
                    <tr>
                        <th class="membership-match"><%#GetText("membershipMatchLabel")%></th>
                        <th class="customer-number"><%# GetText("customerNumberLabel")%></th>
                        <th class="seat-item"><%#GetText("seatLabel")%></th>
                        <th class="price-band-item"><%# GetText("priceBandLabel")%></th>
                        <th class="fees-item"><%#GetText("FeesLabel")%></th>
                        <th class="price-item"><%# GetText("priceLabel")%></th>
                        <th class="status-item"><%#GetText("statusLabel")%></th>
                        <th class="actions"></th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
      <ItemTemplate>
                    <tr>
                        <td class="membership-match"><%#DataBinder.Eval(Container.DataItem, "ProductDescription")%></td>
                        <td class="customer-number"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString.TrimStart("0")%></td>
                        <td class="seat-item"><%# CheckSeatDetailsForAttendance(DataBinder.Eval(Container.DataItem, "Seat").ToString, DataBinder.Eval(Container.DataItem, "ProductType").ToString)%></td>
                        <td class="price-band-item"><%# DataBinder.Eval(Container.DataItem, "PriceBandDesc")%></td>
                        <td class="fees-item"><%# DataBinder.Eval(Container.DataItem, "FeesCode")%></td>
                        <td class="price-item"><%# FormatCurrency(DataBinder.Eval(Container.DataItem, "SalePrice"))%>
                            <asp:HyperLink ID="hlpPromotion" runat="server" CssClass="promotion-link" Visible='<%# ShowPromotion(DataBinder.Eval(Container.DataItem, "PromotionId").ToString()) %>'
                              NavigateUrl='<%# GetNavigateUrl(DataBinder.Eval(Container.DataItem, "PromotionId").ToString(), DataBinder.Eval(Container.DataItem, "OriginalPrice").ToString(), DataBinder.Eval(Container.DataItem, "SalePrice").ToString()) %>'>
                              <asp:Image ID="imgPromotion" runat="server" CssClass="promotion-image" />
                            </asp:HyperLink>
                        </td>
                        <td class="status-item"><%# GetStatusText(Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "StatusCode")))%></td>
                        <td class="actions">
                            <asp:HyperLink ID="hlnkTransfer" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                            <asp:HyperLink ID="hlnkAmend" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                            <asp:HyperLink ID="hlnkCancel" runat="server" Visible="false" CssClass="button"></asp:HyperLink>
                            <asp:Button ID="btnPrint" runat="server" CssClass="button" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "RRN").ToString %>' CommandName="PrintThisTicket" />
                        </td>
                    </tr>
      </ItemTemplate>
      <FooterTemplate>
                </tbody>
            </table>
          </div>
      </FooterTemplate>
  </asp:Repeater>

  <asp:PlaceHolder ID="plhBottomPager" runat="server">
    <%--Pager Bottom section--%>
    <div class="graphical-pager">
      <p class="pager-display">
        <span class="display">
            <asp:Label ID="displayingLabelB" runat="server" Text="Displaying" />
            <asp:Label ID="displayingValueLabelB" runat="server" Text="1" />
            <asp:Label ID="toLabelB" runat="server" Text="to" />
            <asp:Label ID="toValueLabelB" runat="server" Text="5" />
            <asp:Label ID="ofLabelB" runat="server" Text="of" />
            <asp:Label ID="ofValueLabelB" runat="server" Text="6" />
        </span>|
        <span class="nav">
            <asp:HyperLink ID="LnkFirstB" runat="server" Text="First"></asp:HyperLink>
            <asp:HyperLink ID="LnkPrevB" runat="server" Text="Prev"></asp:HyperLink>
            <asp:Label ID="LinksLabelB" runat="server" />
            <asp:HyperLink ID="LnkNextB" runat="server" Text="Next"></asp:HyperLink>
            <asp:HyperLink ID="LnkLastB" runat="server" Text="Last"></asp:HyperLink>
        </span>
      </p>
    </div>
  </asp:PlaceHolder>
</asp:PlaceHolder>
