<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductRelationsGraphical3.ascx.vb" Inherits="UserControls_ProductRelationsGraphical3" %>
<asp:PlaceHolder ID="plhProductRelationsGraphical" runat="server">
    <div class="panel ebiz-graphical-product-relations">
        <h2>
            
                <asp:Label ID="lblHeaderText" runat="server" />
            
        </h2>
        <asp:ValidationSummary ID="AddToBasketSummary" runat="server" ValidationGroup="AddToBasket" CssClass="error" />
        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <div class="error">
                <asp:Label ID="lblError" runat="server" CssClass="error" />
            </div>
        </asp:PlaceHolder>

        <asp:Repeater ID="rptProductRelationsGraphical" runat="server">
            <HeaderTemplate>

                    <ul class="small-block-grid-1 medium-block-grid-4 large-block-grid-4">
            </HeaderTemplate>
            <ItemTemplate>
                    <li<%# GetClassName(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), Container.ItemIndex) %>>
                            <asp:PlaceHolder ID="plhImage" runat="server" Visible='<%# ShowImage %>'>
                                <a href='<%# GetProductPath(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()) %>'>
                                    <img src='<%# GetImagePath(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()) %>' 
                                        alt='<%# GetProductDescription1ByProductCode(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString())%>' />
                                </a>
                            </asp:PlaceHolder>
                            <div class="ebiz-graphical-product-relation-wrap">
                                <asp:PlaceHolder ID="plhProductLink" runat="server" Visible='<%# ShowLink %>'>
                                <h3>
                                    
                                        <a href='<%# GetProductPath(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString())%>'>
                                            <%# GetProductDescription1ByProductCode(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString())%>
                                        </a>
                                    
                                </h3>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhDescription" runat="server" Visible='<%# ShowText %>'>
                                    <p class="ebiz-product-description">
                                        <%# GetProductDescription2ByProductCode(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString())%>
                                    </p>
                                    </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhRetailOptions" runat="server" Visible='<%# DisplayRetailOptions(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()) %>'>
                                <asp:PlaceHolder ID="plhPriceOptions" runat="server" Visible='<%# ShowPrice %>'>
                                    <div class="row ebiz-product-price">
                                        <div class="large-6 columns">
                                            <%# PriceLabel %>
                                        </div>
                                        <div class="large-6 columns">
                                            <%# GetProductPrice(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString())%>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhBuyOptions" runat="server" Visible='<%# ShowBuy %>'>
                                        <div class="row ebiz-buy-product">
                                            <div class="large-6 columns">
                                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="middle" MaxLength='<%# QuantityBoxMaxLength %>' Text='<%# DefaultQuantity%>' min="0" />
                                                <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity" ErrorMessage='<%# RegularExpressionErrorText %>' 
                                                    ValidationExpression="^[0-9]{0,20}$" ValidationGroup="AddToBasket" Enabled="True" Display="Static" />
                                            </div>
                                            <div class="large-6 columns">
                                                <asp:Button ID="btnBuy" runat="server" CausesValidation="true" ValidationGroup="AddToBasket" CommandName="BUY" CssClass="button" 
                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString() %>' Text='<%# BuyButtonText %>' />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                            </div>
                        </li>
            </ItemTemplate>
            <FooterTemplate>
                    </ul>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>