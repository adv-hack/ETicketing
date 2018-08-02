<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PackageSummary.ascx.vb"
    Inherits="UserControls_Package_PackageSummary" ViewStateMode="Disabled" %>
    <asp:PlaceHolder ID="plhPackageSummary" Visible="false" runat="server">

        <div class="ebiz-package-status">
            <h2>
                <asp:Literal ID="ltlPackageStatus" runat="server" />
                <asp:Label runat="server" ID="lblStatus"></asp:Label>
            </h2>

            <asp:Repeater ID="rptComponentGroups" runat="server">
                <HeaderTemplate>
                    <table class="stack mb3">
                        <thead>
                            <tr>
                                <th class="ebiz-tickets" scope="col">
                                    <%# TicketsText%>
                                </th>
                                <asp:PlaceHolder ID="plhDiscountPricingHeader" runat="server" Visible="<%# IsDiscountDropDownVisible()%>">
                                    <th class="ebiz-price" scope="col">
                                        <%# PriceText%>
                                    </th>
                                    <th class="ebiz-discount" scope="col">
                                        <asp:Label runat="server" ID="lblDiscount" Text='<%# DiscountText%>'></asp:Label>
                                    </th>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhTotalPricingHeader" runat="server" Visible="<%# IsTotalValueVisible()%>">
                                    <th class="ebiz-total large-text-right" scope="col">
                                        <%# TotalText%>
                                    </th>
                                </asp:PlaceHolder>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:PlaceHolder ID="plhComponentGroupTitle" Visible="false" runat="server">
                        <tr>
                            <td class="ebiz-component-group" colspan="4">
                                <h3>
                                    <asp:Label ID="lblComponentGroup" runat="server"></asp:Label>
                                </h3>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:Repeater ID="rptComponents" runat="server" EnableViewState="false" OnItemDataBound="rptComponents_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td data-title="<%# TicketsText%>" class="ebiz-tickets">
                                    <asp:Label ID="lblComponent" runat="server" Text='<%# GetComponentText(DataBinder.Eval(Container.DataItem, "Quantity").ToString(), DataBinder.Eval(Container.DataItem, "ComponentDescription").ToString())%>'></asp:Label>
                                </td>
                                <asp:PlaceHolder ID="plhDiscountPricing" runat="server" Visible="<%# IsDiscountDropDownVisible()%>">
                                    <td data-title="<%# PriceText%>" class="ebiz-price">
                                        <asp:Label ID="lblPrice" runat="server"></asp:Label>
                                    </td>
                                    <td data-title="<%# DiscountText%>" class="ebiz-discount">
                                        <div class="ebiz-ui-slider-wrap">
                                            <span id="compDiscSlider" class="compDiscSlider"></span>
                                            <asp:TextBox ID="compDiscTB" runat="server" CssClass="compDiscTB"></asp:TextBox>
                                        </div>
                                        <asp:RegularExpressionValidator ID="CompDiscRegEx" runat="server" ControlToValidate="compDiscTB" CssClass="error" ErrorMessage="Invalid Discount" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                                    </td>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhTotalPricing" runat="server" Visible="<%# IsTotalValueVisible()%>">
                                    <td data-title="<%# TotalText%>" class="ebiz-total large-text-right">
                                        <asp:Literal ID="ltlVATPrice" runat="server" Text='<%# GetFormattedPrice(DataBinder.Eval(Container.DataItem, "PriceIncludingVAT").ToString())%>' />
                                        <asp:HiddenField ID="hdnComponentId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ComponentId").ToString()%>' />
                                        <asp:HiddenField ID="hdnComponentGroupId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "ComponentGroupId").ToString()%>' />
                                        <asp:HiddenField ID="hdnQuantity" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Quantity").ToString()%>' />
                                    </td>
                                </asp:PlaceHolder>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="ebiz-summary-total">
            <h2>

                <asp:Literal ID="ltlSummaryTotals" runat="server" />
            </h2>
            <asp:PlaceHolder ID="plhPackageDiscount" runat="server" Visible="True">
                <div class="row ebiz-discount">
                    <div class="large-6 columns">
                        <asp:Label runat="server" ID="lblPackageDiscount" AssociatedControlID="packageDiscTB"></asp:Label>
                    </div>
                    <div class="large-6 columns">
                        <div class="ebiz-ui-slider-wrap">
                            <span id="packageDiscSlider" class="packageDiscSlider"></span>
                            <asp:TextBox ID="packageDiscTB" runat="server" CssClass="packageDiscTB"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <asp:RegularExpressionValidator ID="PackDiscRegEx" runat="server" ControlToValidate="packageDiscTB" CssClass="error" ErrorMessage="Invalid Discount" OnLoad="SetRegEx" Enabled="True" Display="Static" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhNetAndVAT" runat="server">
                <div class="row ebiz-net-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlNetTotal" runat="server"></asp:Literal>
                    </div>
                    <div class="small-6 columns text-right">
                      <asp:Label ID="lblPackageNetTotal" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row ebiz-vat-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlVatTotal" runat="server" />
                    </div>
                    <div class="small-6 columns text-right">
                      <asp:Label ID="lblPackageVATTotal" runat="server"></asp:Label>
                    </div>
                </div>
                <asp:PlaceHolder ID="plhPackageLevelDiscount" runat="server">
                    <div class="row ebiz-package-level-discount">
                        <div class="small-6 columns">
                            <asp:Literal ID="ltlPackageLevelDiscount" runat="server"></asp:Literal>
                        </div>
                        <div class="small-6 columns text-right">
                              <asp:Label ID="lblPackageLevelDiscount" runat="server"></asp:Label>
                        </div>
                    </div>

                </asp:PlaceHolder>
            </asp:PlaceHolder>
            <div class="row">
                <div class="small-6 columns">
                    <span class="f2"><asp:Literal ID="ltlTotal" runat="server" /></span>
                </div>
                <div class="small-6 columns text-right">
                  <span class="f2"><asp:Label ID="lblPackageTotal" runat="server"></asp:Label></span>
                </div>
            </div>
            <asp:PlaceHolder ID="plhPackageOutstandTotal" runat="server">
                <div class="row outstand total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlOutstandTotal" runat="server" />
                    </div>
                    <div class="small-6 columns">
                        <asp:Label ID="lblPackageOutstandTotal" runat="server"></asp:Label>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="button-group ebiz-package-summary-buttons-wrap">
                <asp:Button ID="btnUpdatePackage" runat="server" CssClass="button ebiz-update-package" />
                <asp:Button ID="btnShowBasket" runat="server" CssClass="button ebiz-show-basket" />
                <asp:Button ID="btnAmendBasket" runat="server" CssClass="button ebiz-amend-basket" />
                <asp:Button ID="btnDeletePackage" runat="server" CssClass="button ebiz-muted-action ebiz-delete-package" />
                <a id="hplComments" runat="server" class="button fa-input ebiz-add-comment" data-reveal-id="comments-modal" data-reveal-ajax="true">
                    <asp:Literal ID="ltlCommentsContent" runat="server" />
                </a>
            </div>
            <div class="reveal ebiz-reveal-ajax" id="comments-modal" data-reveal></div>
        </div>

        <asp:HiddenField ID="hdnOriginalFulfilmentCode" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="hdnOriginalFulfilment" runat="server"></asp:HiddenField>
    </asp:PlaceHolder>