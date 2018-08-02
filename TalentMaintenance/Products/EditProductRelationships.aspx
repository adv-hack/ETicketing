<%@ Page Language="VB"  MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="EditProductRelationships.aspx.vb" 
    Inherits="Products_EditProductRelationships" EnableEventValidation="false" ValidateRequest="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <asp:HyperLink Id="hplMenuLink" runat="server">Product Relationships Main Menu</asp:HyperLink>
    <asp:ScriptManager ID="scmProductRelationships" runat="server" />
    <asp:UpdatePanel ID="updProductRelationships" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlProductType1" />
            <asp:AsyncPostBackTrigger ControlID="ddlProductType2" />
            <asp:PostBackTrigger ControlID="rptProductRelationships" />
            <asp:PostBackTrigger ControlID="btnCreateNewLink" />
            <asp:PostBackTrigger ControlID="btnUpdateLink" />
            <asp:PostBackTrigger ControlID="btnGoBack" />
        </Triggers>
        <ContentTemplate>
            <script type="text/javascript">
                function productCheckLinkingFrom(productCode) {
                    if (productCode != $(".product-code-linking-from").val()) {
                        return confirm('<%= ValidateProductLinksMessage %>');
                    } else {
                        return true;
                    }
                }
                function productCheckLinkingTo(productCode) {
                    if (productCode != $(".product-code-linking-to").val()) {
                        return confirm('<%= ValidateProductLinksMessage %>');
                    } else {
                        return true;
                    }
                }
            </script>
            <div id="edit-product-relationships-wrapper">
                <div class="product-relationships-detail">
                    <p class="title"><asp:Label ID="pagetitleLabel" runat="server"></asp:Label></p>
                    <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server"></asp:Label></p>
                    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
                        <p class="error"><asp:Label ID="lblErrorMessage" runat="server" /></p>
                    </asp:PlaceHolder>
                    <asp:ValidationSummary ID="vlsErrorMessage" runat="server" CssClass="error" />
                    <asp:Button ID="btnAddNewRelationship" runat="server"  CausesValidation="false" Visible="false" />
                </div>

                <asp:PlaceHolder ID="plhRelationshipEditingFields" runat="server">
                    <fieldset>
                        <p class="instructions"><asp:Literal ID="ltlEditingInstructions1" runat="server" /></p>
                        <ul>
                            <li class="product-link-type">
                                <asp:Label ID="lblTicketingLinkType" runat="server" AssociatedControlID="ddlTicketingLinkType" />
                                <asp:DropDownList ID="ddlTicketingLinkType" runat="server" AutoPostBack="true" /><asp:Literal ID="ltlTicketingLinkTypeInstructions" runat="server" />
                            </li>
                            <li class="ticketing-or-retail">
                                <asp:Label ID="lblProductType1" runat="server" AssociatedControlID="ddlProductType1" />
                                <asp:DropDownList ID="ddlProductType1" runat="server" AutoPostBack="true" />
                            </li>
                            <li class="product-code">
                                <asp:Label ID="lblProductCode1" runat="server" AssociatedControlID="txtProductCode1" />
                                <asp:TextBox ID="txtProductCode1" runat="server" CssClass="product-code-textbox product-code-linking-from" />
                                <asp:Button ID="btnValidateProductCode1" runat="server" Visible="false" CausesValidation="false" />
                                <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceCodeSearch1" TargetControlID="txtProductCode1" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                                    ServiceMethod="GetProductCodeList" MinimumPrefixLength="2" CompletionInterval="100" EnableCaching="true">
                                </ajaxToolkit:AutoCompleteExtender>
                            </li>
                            <li class="product-description">
                                <asp:Label ID="lblProductDescription1" runat="server" AssociatedControlID="txtProductDescription1" />
                                <asp:TextBox ID="txtProductDescription1" runat="server" CssClass="product-description-textbox" />
                                <asp:Button ID="btnValidateProductDescription1" runat="server" Visible="false" CausesValidation="false" />
                                <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceSearch1" TargetControlID="txtProductDescription1" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                                    ServiceMethod="GetProductDescList" MinimumPrefixLength="2" CompletionInterval="100" EnableCaching="true">
                                </ajaxToolkit:AutoCompleteExtender>
                            </li>
                            <asp:PlaceHolder ID="plhTicketingOptions1" runat="server" Visible="false">
                                <li class="product-type">
                                    <asp:Label ID="lblTicketingProductType1" runat="server" AssociatedControlID="ddlTicketingProductType1" />
                                    <asp:DropDownList ID="ddlTicketingProductType1" runat="server" AutoPostBack="true" />
                                </li>
                                <li class="product-sub-type">
                                    <asp:Label ID="lblTicketingProductSubType1" runat="server" AssociatedControlID="ddlTicketingProductSubType1" />
                                    <asp:DropDownList ID="ddlTicketingProductSubType1" runat="server" />
                                </li>
                                <asp:PlaceHolder ID="plhTicketingPriceCode1" runat="server" Visible="false">
                                <li class="price-code">
                                    <asp:Label ID="lblTicketingPriceCode1" runat="server" AssociatedControlID="ddlTicketingPriceCode1" />
                                    <asp:DropDownList ID="ddlTicketingPriceCode1" runat="server" />
                                </li>
                                </asp:PlaceHolder>
                            </asp:PlaceHolder>
                        </ul>
                    </fieldset>

                    <asp:PlaceHolder ID="plhEditRelationship" runat="server" Visible="true">
                        <fieldset>
                            <p class="instructions"><asp:Literal ID="ltlEditingInstructions2" runat="server" /></p>
                            <ul>
                                <li class="ticketing-or-retail">
                                    <asp:Label ID="lblProductType2" runat="server" AssociatedControlID="ddlProductType2" />
                                    <asp:DropDownList ID="ddlProductType2" runat="server" AutoPostBack="true" />
                                </li>
                                <li class="product-code">
                                    <asp:Label ID="lblProductCode2" runat="server" AssociatedControlID="txtProductCode2" />
                                    <asp:TextBox ID="txtProductCode2" runat="server" CssClass="product-code-textbox product-code-linking-to" />
                                    <asp:Button ID="btnValidateProductCode2" runat="server" Visible="false" CausesValidation="false" />
                                    <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceCodeSearch2" TargetControlID="txtProductCode2" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                                        ServiceMethod="GetProductCodeList" MinimumPrefixLength="2" CompletionInterval="100" EnableCaching="true" CompletionSetCount="25">
                                    </ajaxToolkit:AutoCompleteExtender>
                                    <asp:RequiredFieldValidator ID="rfvProductCode2" runat="server" ControlToValidate="txtProductCode2" Display="None" Enabled="false" />
                                </li>
                                <li class="product-description">
                                    <asp:Label ID="lblProductDescription2" runat="server" AssociatedControlID="txtProductDescription2" />
                                    <asp:TextBox ID="txtProductDescription2" runat="server" CssClass="product-description-textbox" />
                                    <asp:Button ID="btnValidateProductDescription2" runat="server" Visible="false" CausesValidation="false" />
                                    <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceSearch2" TargetControlID="txtProductDescription2" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                                        ServiceMethod="GetProductDescList" MinimumPrefixLength="2" CompletionInterval="100" EnableCaching="true" CompletionSetCount="25">
                                    </ajaxToolkit:AutoCompleteExtender>
                                    <asp:RequiredFieldValidator ID="rfvProductDescription2" runat="server" ControlToValidate="txtProductDescription2" Display="None" Enabled="false" />
                                </li>

                                <asp:PlaceHolder ID="plhTicketingOptions2" runat="server" Visible="false">
                                    <li class="product-type">
                                        <asp:Label ID="lblTicketingProductType2" runat="server" AssociatedControlID="ddlTicketingProductType2" />
                                        <asp:DropDownList ID="ddlTicketingProductType2" runat="server" AutoPostBack="true" />
                                    </li>
                                    <li class="product-sub-type">
                                        <asp:Label ID="lblTicketingProductSubType2" runat="server" AssociatedControlID="ddlTicketingProductSubType2" />
                                        <asp:DropDownList ID="ddlTicketingProductSubType2" runat="server" />
                                    </li>
                                    <asp:PlaceHolder ID="plhTicketingPriceCode2" runat="server" Visible="false">
                                    <li class="price-code">
                                        <asp:Label ID="lblTicketingPriceCode2" runat="server" AssociatedControlID="ddlTicketingPriceCode2" />
                                        <asp:DropDownList ID="ddlTicketingPriceCode2" runat="server" />
                                    </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhRelatedTicketingCampaignCode" runat="server" Visible="false">
                                    <li class="campaign-code">
                                        <asp:Label ID="lblRelatedTicketingCampaignCode" runat="server" AssociatedControlID="ddlRelatedTicketingCampaignCode" />
                                        <asp:DropDownList ID="ddlRelatedTicketingCampaignCode" runat="server" />
                                    </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhDefaultTicketingStadiumOptions" runat="server" Visible="false">
                                    <li class="stand">
                                        <asp:HiddenField ID="hdfStadiumCode" runat="server" />
                                        <asp:Label ID="lblDefaultTicketingStand" runat="server" AssociatedControlID="ddlDefaultTicketingStand" />
                                        <asp:DropDownList ID="ddlDefaultTicketingStand" runat="server" AutoPostBack="true" />
                                        <asp:CheckBox ID="chkDefaultTicketingStandReadonlyOption" runat="server" />
                                        <asp:Label ID="lblDefaultTicketingStandReadonlyOption" runat="server" AssociatedControlID="chkDefaultTicketingStandReadonlyOption" CssClass="right-label" />
                                    </li>
                                    <li class="area">
                                        <asp:Label ID="lblDefaultTicketingArea" runat="server" AssociatedControlID="ddlDefaultTicketingArea" />
                                        <asp:DropDownList ID="ddlDefaultTicketingArea" runat="server" />
                                        <asp:CheckBox ID="chkDefaultTicketingAreaReadonlyOption" runat="server" />
                                        <asp:Label ID="lblDefaultTicketingAreaReadonlyOption" runat="server" AssociatedControlID="chkDefaultTicketingAreaReadonlyOption" CssClass="right-label" />
                                    </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhDefaultTicketingExtraTravelOptions" runat="server" Visible="false">
                                    <li class="product-detail-code">
                                        <asp:Label ID="lblDefaultTicketingExtraTravelOptions" runat="server" AssociatedControlID="ddlDefaultTicketingExtraTravelOptions" />
                                        <asp:DropDownList ID="ddlDefaultTicketingExtraTravelOptions" runat="server" CssClass="product-detail-code-textbox" />
                                    </li>
                                    </asp:PlaceHolder>

                                    <asp:PlaceHolder ID="plhAllQuantitySettings" runat="server">
                                    <li class="quantity-settings">
                                        <asp:Label ID="lblQuantitySettings" runat="server" AssociatedControlID="chkQuantitySettings" />
                                        <asp:CheckBox ID="chkQuantitySettings" runat="server" AutoPostBack="true" />
                                        <asp:PlaceHolder ID="plhQuantitySettings" runat="server" Visible="false">
                                        <ul>
                                            <asp:PlaceHolder ID="plhQuantityDefinition" runat="server">
                                            <li class="quantity-definition">
                                                <asp:Label ID="lblQuantityDefinition" runat="server" AssociatedControlID="chkQuantityDefintion" />
                                                <asp:CheckBox ID="chkQuantityDefintion" runat="server" AutoPostBack="true" /><asp:Literal ID="ltlQuantityDefinitionInstructions" runat="server" />
                                            </li>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhQuantityRoundUp" runat="server" Visible="false">
                                            <li class="quantity-round-up">
                                                <asp:Label ID="lblQuantityRoundUp" runat="server" AssociatedControlID="chkQuantityRoundUp" />
                                                <asp:CheckBox ID="chkQuantityRoundUp" runat="server" /><asp:Literal ID="ltlQuantityRoundUpInstructions" runat="server" />
                                            </li>
                                            </asp:PlaceHolder>
                                            <li class="default-quantity">
                                                <asp:Label ID="lblDefaultQuantity" runat="server" AssociatedControlID="txtDefaultQuantity" />
                                                <asp:TextBox ID="txtDefaultQuantity" runat="server" CssClass="quantity-textbox" /><asp:Label ID="lblQuantityPercentage" runat="server" Text="%" Visible="false" />
                                                <asp:CheckBox ID="chkDefaultQuantityReadonlyOption" runat="server" AutoPostBack="true" />
                                                <asp:Label ID="lblDefaultQuantityReadonlyOption" runat="server" AssociatedControlID="chkDefaultQuantityReadonlyOption" CssClass="right-label" />
                                            </li>
                                            <asp:PlaceHolder ID="plhDefaultQuantityMinMax" runat="server">
                                            <li class="minimum-quantity">
                                                <asp:Label ID="ltlDefaultQuantityMin" runat="server" AssociatedControlID="txtDefaultQuantityMin" />
                                                <asp:TextBox ID="txtDefaultQuantityMin" runat="server" CssClass="quantity-textbox" /><asp:Label ID="lblMinPercentage" runat="server" Text="%" Visible="false" />
                                            </li>
                                            <li class="maximum-quantity">
                                                <asp:Label ID="ltlDefaultQuantityMax" runat="server" AssociatedControlID="txtDefaultQuantityMax" />
                                                <asp:TextBox ID="txtDefaultQuantityMax" runat="server" CssClass="quantity-textbox" /><asp:Label ID="lblMaxPercentage" runat="server" Text="%" Visible="false" />
                                            </li>
                                            </asp:PlaceHolder>
                                        </ul>
                                        </asp:PlaceHolder>
                                    </li>
                                    </asp:PlaceHolder>

                                    <li class="instructions">
                                        <asp:Label ID="lblInstructions" runat="server" AssociatedControlID="txtInstructions" />
                                        <asp:TextBox ID="txtInstructions" runat="server" MaxLength="500" />
                                    </li>
                                    <li class="css-class">
                                        <asp:Label ID="lblCssClass" runat="server" AssociatedControlID="txtCssClass" />
                                        <asp:TextBox ID="txtCssClass" runat="server" MaxLength="20" />
                                    </li>

                                    <asp:PlaceHolder ID="plhMandatoryProductOption" runat="server">
                                    <li class="mandatory-product">
                                        <asp:Label ID="lblMandatoryProduct" runat="server" AssociatedControlID="chkMandatoryProduct" />
                                        <asp:CheckBox ID="chkMandatoryProduct" runat="server" /><asp:Literal ID="ltlMandatoryProductInstructions" runat="server" />
                                    </li>
                                    </asp:PlaceHolder>
                                    <li class="copy-all-bu">
                                        <asp:Label ID="lblCopyToAllBU" runat="server" AssociatedControlID="chkCopyToAllBU" />
                                        <asp:CheckBox ID="chkCopyToAllBU" runat="server" />
                                    </li>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plhReverseLink" runat="server">
                                <li class="reverse-link">
                                    <asp:Label ID="lblReverseLink" runat="server" AssociatedControlID="chkReverseLink" />
                                    <asp:CheckBox ID="chkReverseLink" runat="server" /><asp:Literal ID="ltlReverseLinkInstructions" runat="server" />
                                </li>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plhComponentValue" runat="server">
                                <li class="component-value">
                                    <asp:Label ID="lblComponentValue" runat="server" AssociatedControlID="txtComponentValue1" />
                                    <li class="component-value-item">    
                                        <asp:TextBox runat="server" ID="txtComponentPriceBand1" style="width: 10px" />   
                                        <asp:TextBox ID="txtComponentValue1" runat="server" CssClass="input-s" />
                                        <asp:CompareValidator id="CheckFormat1" runat="server" ControlToValidate="txtComponentValue1" Operator="DataTypeCheck"
                                                 Type="Currency"  Display="Dynamic" ErrorMessage="Invalid - must be in n.nn format." />
                                    </li>
                                    
                                    <li class="component-value-item">
                                        <asp:TextBox runat="server" ID="txtComponentPriceBand2" style="width: 10px" />        
                                        <asp:TextBox ID="txtComponentValue2" runat="server" CssClass="input-s" />
                                        <asp:CompareValidator id="CheckFormat2" runat="server" ControlToValidate="txtComponentValue2" Operator="DataTypeCheck"
                                                 Type="Currency"  Display="Dynamic" ErrorMessage="Invalid - must be in n.nn format." />
                                    </li>
                                    <li class="component-value-item"> 
                                        <asp:TextBox runat="server" ID="txtComponentPriceBand3" style="width: 10px" />   
                                        <asp:TextBox ID="txtComponentValue3" runat="server" CssClass="input-s" />
                                        <asp:CompareValidator id="CheckFormat3" runat="server" ControlToValidate="txtComponentValue3" Operator="DataTypeCheck"
                                                 Type="Currency"  Display="Dynamic" ErrorMessage="Invalid - must be in n.nn format." />
                                    </li>
                                    <li class="component-value-item">
                                        <asp:TextBox runat="server" ID="txtComponentPriceBand4" style="width: 10px" />
                                        <asp:TextBox ID="txtComponentValue4" runat="server" CssClass="input-s" />
                                        <asp:CompareValidator id="CheckFormat4" runat="server" ControlToValidate="txtComponentValue4" Operator="DataTypeCheck"
                                                 Type="Currency"  Display="Dynamic" ErrorMessage="Invalid - must be in n.nn format." />
                                    </li>
                                    <li class="component-value-item">
                                        <asp:TextBox runat="server" ID="txtComponentPriceBand5" style="width: 10px" />
                                        <asp:TextBox ID="txtComponentValue5" runat="server" CssClass="input-s" />
                                        <asp:CompareValidator id="CheckFormat5" runat="server" ControlToValidate="txtComponentValue5" Operator="DataTypeCheck"
                                                 Type="Currency"  Display="Dynamic" ErrorMessage="Invalid - must be in n.nn format." />
                                    </li>
                                </li>
                                </asp:PlaceHolder>

                            </ul>
                        </fieldset>
                    </asp:PlaceHolder>
                    
                    <div class="product-relationships-link-button">
                        <asp:Button ID="btnCreateNewLink" runat="server" CausesValidation="true" />
                        <asp:Button ID="btnUpdateLink" runat="server" CausesValidation="true" />
                        <asp:Button ID="btnGoBack" runat="server" CausesValidation="false" />
                        <asp:HiddenField ID="hdfProductCodeLinkingFrom" runat="server" />
                        <asp:HiddenField ID="hdfProductCodeLinkingTo" runat="server" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhRelationships" runat="server" Visible="false">
                    <div class="product-relationships-list">
                        <p class="product-relationships-table-title"><asp:Literal ID="ltlProductsRelatingTo" runat="server" /></p>
                        <asp:Repeater ID="rptProductRelationships" runat="server">
                            <HeaderTemplate>
                                <table class="defaultTable">
                                    <tr class="header">
                                        <th class="product" scope="col"><%# ProductColumnHeader%></th>
                                        <th class="product-type" scope="col"><%#TypeSubTypeColumnHeader %></th>
                                        <th class="price-codes1" scope="col"><%#PriceCodesColumnHeader%></th>
                                        <th class="linked-to" scope="col"><%#LinkedToColumnHeader %></th>
                                        <th class="product-type" scope="col"><%#TypeSubTypeColumnHeader %></th>
                                        <th class="price-codes2" scope="col"><%#PriceCodesColumnHeader%></th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <tr class="odd">
                                        <td class="product"><%# DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString(), DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString())%></td>
                                        <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                                        <td class="price-codes1"><%# DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_PRICE_CODE").ToString()%></td>
                                        <td class="linked-to"><%# DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_CODE").ToString())%></td>
                                        <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                                        <td class="price-codes2"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString())%></td>
                                    </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                    <tr class="even">
                                        <td class="product"><%# DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString(), DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString())%></td>
                                        <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                                        <td class="price-codes1"><%# DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_PRICE_CODE").ToString()%></td>
                                        <td class="linked-to"><%# DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_CODE").ToString())%></td>
                                        <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                                        <td class="price-codes2"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString())%></td>
                                    </tr>
                            </AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>