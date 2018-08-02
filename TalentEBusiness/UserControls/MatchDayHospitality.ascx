<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MatchDayHospitality.ascx.vb" Inherits="UserControls_MatchDayHospitality" ViewStateMode="Disabled" %>
<asp:PlaceHolder ID="MDHPackageError" runat="server">
    <div id="alert-box alert">
        <asp:Label ID="lblMDHPackageError" runat="server"></asp:Label>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="MDHPackageList" runat="server">
    <asp:Repeater ID="rptMDHPackages" runat="server">
        <HeaderTemplate>
            <table>
                <thead>
                    <tr>
                        <th class="ebiz-description">
                            <asp:Literal ID="HospitalityHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                        <th id="colNetPrice" class="ebiz-net-price" runat="server" onprerender="netPricePreRender">
                            <asp:Literal ID="NetPriceHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="ebiz-price">
                            <asp:Literal ID="PriceHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="ebiz-quantity">
                            <asp:Literal ID="QuantityHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="ebiz-add-to-basket">
                            <asp:Literal ID="AddToBasketHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                        <th id="hlkViewCol" runat="server" class="ebiz-view">
                            <asp:Literal ID="ViewHeaderLabel" runat="server" OnPreRender="GetText" />
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="ebiz-description">
                    <asp:HiddenField ID="hfProductCode" runat="server" />
                    <asp:HiddenField ID="hfPackageID" runat="server" />
                    <asp:HiddenField ID="hfPackageCode" runat="server" />
                    <asp:HiddenField ID="hfSeatComponentID" runat="server" />
                    <asp:PlaceHolder ID="plhDescription" runat="server">
                         <asp:HyperLink ID="hlkDescription" runat="server" />
                    </asp:PlaceHolder>                    
                    <asp:Label ID="lblDescription" CssClass="ebiz-description" runat="server" />
                    <asp:PlaceHolder ID="plhComments" runat="server">
                        <span class="ebiz-comments"><asp:Literal ID="ltlComments" runat="server" /></span>
                    </asp:PlaceHolder>
                </td>
                <td id="colNetPrice" runat="server" class="ebiz-net-price">
                    <asp:Literal ID="ltlNetPrice" runat="server" />
                </td>
                <td class="price">
                    <asp:Literal ID="ltlPrice" runat="server" />
                </td>
                <td id="soldOutCol" colspan="2" runat="server" class="ebiz-sold-out">
                    <div class="alert-box alert"><asp:Literal ID="ltlSoldOut" runat="server" /></div>
                </td>
                <td id="quantityCol" runat="server" class="ebiz-quantity">
                    <asp:DropDownList ID="ddlQuantity" runat="server" />
                </td>
                <td id="btnAddToBasketCol" runat="server" class="ebiz-add-to-basket">
                    <asp:Button ID="btnAddToBasket" CssClass="button" runat="server" OnClick="AddPackageItemsToBasket" />
                </td>
                <td id="hlkViewCol" runat="server" class="ebiz-view">
                    <asp:HyperLink ID="hlkView" runat="server" />
                    <div class="reveal" data-reveal id='view-area-<%# Container.ItemIndex%>'>
                        <asp:Image ID="imgViewArea" runat="server" />
                        <button class="close-button" data-close aria-label="Close modal" type="button">
                            <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
                        </button>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>
