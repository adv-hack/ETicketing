<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductOptions.ascx.vb" Inherits="UserControls_ProductOptions" %>

<div class="row ebiz-product-options1">
    <div class="large-4 columns">
        <asp:Label ID="ddlLabelLevel1" runat="server" AssociatedControlID="ddlOptionLevel1" CssClass="middle" />
    </div>
    <div class="large-8 columns">
        <asp:DropDownList ID="ddlOptionLevel1" runat="server" AutoPostBack="true" />
    </div>
</div>

<asp:PlaceHolder ID="plhSizeChart" runat="server">
    <p class="ebiz-size-chart">
        <asp:HyperLink ID="hplSizeChart" data-open="size-chart-modal" runat="server" CssClass="ebiz-open-modal"  />
        <div id="size-chart-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
    </p>
</asp:PlaceHolder>


<asp:Repeater ID="rptOptionLevel1" runat="server" OnItemCommand="rptOptionLevel1_OnItemCommand">
    <HeaderTemplate>
        <table>
            <thead>
            <tr>
                <th scope="col" class="ebiz-option">
                    <asp:Literal ID="Option1Label" runat="server" OnPreRender="GetText" />
                </th>
                <th scope="col" class="ebiz-price" id="priceColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                    <asp:Literal ID="PriceLabel" runat="server" OnPreRender="GetText" />
                </th>
                <th scope="col" class="ebiz-quantity" id="quantityColumn" runat="server" onprerender="CheckVisibilityForFreeItem" visible="true">
                    <asp:Literal ID="QuantityLabel" runat="server" OnPreRender="GetText" visible="true"/>
                </th>
                <th scope="col" class="ebiz-option2" id="option2DDLcolumn" runat="server" onprerender="checkLevel2Visibility">
                    <asp:Literal ID="Option2Label" runat="server" OnPreRender="GetText" />
                </th>
                <th scope="col" class="ebiz-stock">
                    <asp:Literal ID="StockLabel" runat="server" OnPreRender="GetText" />
                </th>
                <th scope="col" class="ebiz-option3" id="option3DDLcolumn" runat="server" onprerender="checkLevel3Visibility">
                </th>
                <asp:PlaceHolder ID="plhPersonalise" runat="server">
                <th scope="col" class="show-for-large-up ebiz-personalise" id="personaliseColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                    <asp:Literal ID="PersonaliseLabel" runat="server" OnPreRender="GetText" />
                </th>
                </asp:PlaceHolder>
                <th scope="col" class="ebiz-promotion" id="promotionColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                    <asp:Literal ID="PromotionSelectLabel" runat="server" OnPreRender="GetText" />
                </th>
            </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="ebiz-option">
                <asp:Literal ID="option1Value" runat="server" />
                <asp:Literal ID="newProductCode" runat="server" />
                <asp:HiddenField ID="hdfMasterProductCode" runat="server" />
                <asp:HiddenField ID="hdfProductWeight" runat="server" />
                <asp:Literal ID="optionLabel" runat="server" />
            </td>
            <td class="ebiz-price" id="priceColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                <asp:Literal ID="priceLabel" runat="server" />
            </td>
            <td class="ebiz-quantity" id="quantityColumn" runat="server" onprerender="CheckVisibilityForFreeItem" visible="true">
                <asp:TextBox ID="quantityBox" runat="server" Columns="4" type="number" visible="true" min="0"/>
            </td>
            <td class="ebiz-option2" id="option2DDLcolumn" runat="server" onprerender="checkLevel2Visibility">
                <asp:DropDownList ID="option2DDL" runat="server" AutoPostBack="true" OnSelectedIndexChanged="option2DDL_IndexChanged" />
            </td>
            <td class="ebiz-stock">
                <strong><asp:Literal ID="stockLabel" runat="server" /></strong>
            </td>
            <td class="ebiz-option3" id="option3DDLcolumn" runat="server" onprerender="checkLevel3Visibility">
                <asp:DropDownList ID="option3DDL" runat="server" AutoPostBack="true" OnSelectedIndexChanged="option3DDL_IndexChanged" />
            </td>
            <asp:PlaceHolder ID="plhPersonalise" runat="server">
                <td class="show-for-large-up ebiz-personalise" id="personaliseColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                    <asp:Button ID="btnPersonalise" runat="server" CssClass="button fa-input" CommandName="Personalise" Text="" />
                </td>
            </asp:PlaceHolder>
            <td class="ebiz-promotion" id="promotionColumn" runat="server" onprerender="CheckVisibilityForFreeItem">
                <asp:Button ID="btnPromotionOptionSelect" runat="server" CssClass="button" CommandName="PromotionOptionSelect" />
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</asp:Repeater>

<div id="Level2Options" runat="server" class="row ebiz-product-options2">
    <div class="large-4 columns">
        <asp:Label ID="ddlLabelLevel2" runat="server" AssociatedControlID="ddlOptionLevel2" CssClass="middle" />
    </div>
    <div class="large-8 columns">
        <asp:DropDownList ID="ddlOptionLevel2" runat="server" AutoPostBack="true" />
    </div>
</div>

<div id="Level3Options" runat="server" class="row ebiz-product-options3">
    <div class="large-4 columns">
        <asp:Label ID="ddlLabelLevel3" runat="server" AssociatedControlID="ddlOptionLevel3" CssClass="middle" />
    </div>
    <div class="large-8 columns">
        <asp:DropDownList ID="ddlOptionLevel3" runat="server" AutoPostBack="true" />
    </div>
</div>

<div id="DDL_BuyOptions" runat="server">
    <div class="row">
        <div class="medium-6 columns">
        <asp:Label ID="QuantityLabel" runat="server" CssClass="quantity-label" visible="true"/>
        </div>
        <div class="medium-6 columns">
        <asp:TextBox ID="Quantity" runat="server" CssClass="input-s" MaxLength="4" Visible="true" min="0"></asp:TextBox>
        </div>
    </div>
    <asp:Button ID="DDL_BuyButton" runat="server" CssClass="button ebiz-add-to-basket" />
    <asp:Button ID="btnDDLPromotionOptionSelect" runat="server" CssClass="button ebiz-promotion-option-select" />
</div>

<div id="TABLE_BuyOptions" runat="server">
    <asp:Button ID="TABLE_BuyButton" runat="server" CssClass="button ebiz-add-to-basket" />
</div>