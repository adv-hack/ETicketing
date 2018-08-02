<%@ Page Title="" Language="VB" AutoEventWireup="false" CodeFile="CustomerReservations.aspx.vb" Inherits="PagesLogin_Profile_CustomerReservations" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
        <div class="alert-box alert ebiz-reservation-error-message">
            <asp:BulletedList id="blErrorDetails" runat="server"></asp:BulletedList>
        </div>
    </asp:PlaceHolder>

    <div class="panel ebiz-customer-reservations-wrap">

        <div class="row ebiz-group-by">
            <div class="medium-3 columns">
                <asp:Label ID="lblGroupBy" AssociatedControlID="ddlGroupBy" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:DropDownList ID="ddlGroupBy" runat="server"  AutoPostBack="True" />
            </div>
        </div>

        <asp:Repeater ID="rptCustomerReservations" runat="server">
            <HeaderTemplate>
                <table>
                    <thead>
                        <tr>
                            <asp:placeholder id ="plhchkSelectAll" runat ="server" OnPreRender="CanDisplayThisColumn">    
                                <th scope="col" class="ebiz-select" >
                                    <asp:Checkbox ID="chkSelectAll"  runat="server" ClientIDMode="Static" OnClick="selectAll(this.checked, '.ebiz-item-select');" /> 
                                </th>   
                            </asp:placeholder> 
                            <asp:placeholder id ="plhReservationReference" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-reservation-reference"><%= ReservationReferenceColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhExpiryDate" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-expiry-date"><%= ExpiryDateColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhCustomer" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-customer"><%= CustomerColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhProduct" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-product"><%= ProductColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhSeat" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-seat"><%= SeatColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhPriceCode" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-price-code"><%= PriceCodeColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhPriceBand" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-price-band"><%= PriceBandColumnHeading%></th>
                            </asp:placeholder> 
                            <asp:placeholder id ="plhQuantity" runat ="server"  OnPreRender="CanDisplayThisColumn">        
                                <th scope="col" class="ebiz-quantity"><%= QuantityColumnHeading%></th>
                            </asp:placeholder> 
                             <asp:placeholder id ="plhTicketNumber" runat ="server"  OnPreRender="CanDisplayThisColumn">
                                <th scope="col" class="ebiz-ticket-no"><%= TicketNumberColumnHeading%></th>
                            </asp:placeholder>
                            <asp:placeholder id ="plhDespatchDate" runat ="server"  OnPreRender="CanDisplayThisColumn">        
                                <th scope="col" class="ebiz-despatchdate"><%= DespatchDateColumnHeading%></th>
                            </asp:placeholder>       
                                  
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <asp:placeholder id="plhchkSelectedItem" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-item-select" data-title="<%=SelectColumnHeading%>">
                            <asp:CheckBox ID="chkSelectedItem" runat="server" OnClick="validateSelAllChkBox(this.checked, '#chkSelectAll', '.ebiz-item-select');"></asp:Checkbox>
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhReservationReference" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-reservation-reference" data-title="<%=ReservationReferenceColumnHeading%>">
                            <asp:Literal ID="ltlReservationReference" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhExpiryDate" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-expiry-date" data-title="<%=ExpiryDateColumnHeading%>">
                            <asp:Literal ID="ltlReservedToTime" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhCustomer" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-customer" data-title="<%=CustomerColumnHeading%>">
                            <asp:Literal ID="ltlReservedTo" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhProduct" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-product" data-title="<%=ProductColumnHeading %>">
                            <asp:Literal ID="ltlProductCode" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhSeat" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-seat" data-title="<%=SeatColumnHeading%>">
                            <asp:Literal ID="ltlSeat" runat="server" />
                        </td>
                    </asp:placeholder>
                     <asp:placeholder id="plhPriceCode" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-price-code" data-title="<%=PriceCodeColumnHeading%>">
                            <asp:Literal ID="ltlPriceCode" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhPriceBand" runat="server"  OnPreRender="CanDisplayThisColumn">
                    <td class="ebiz-price-band" data-title="<%=PriceBandColumnHeading%>">
                        <asp:Literal ID="ltlPriceBand" runat="server" />
                    </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhQuantity" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-quantity" data-title="<%=QuantityColumnHeading%>">
                            <asp:Literal ID="ltlQuantity" runat="server" />
                        </td>
                    </asp:placeholder>
                     <asp:placeholder id="plhTicketNumber" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-seat" data-title="<%=TicketNumberColumnHeading%>">
                               <asp:Literal ID="ltlTicketNo" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:placeholder id="plhDespatchDate" runat="server"  OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-despatchdate" data-title="<%=despatchdateColumnHeading%>">
                            <asp:Literal ID="ltlDespatchDate" runat="server" />
                        </td>
                    </asp:placeholder>
                    <asp:HiddenField ID="hdfSeat" runat="server" />
                    <asp:HiddenField ID="hdfSeatNumber" runat="server" />
                    <asp:HiddenField ID="hdfAlphaSuffix" runat="server" />
                    <asp:HiddenField ID="hdfRowNumber" runat="server" />
                    <asp:HiddenField ID="hdfProductCode" runat="server" />
                    <asp:HiddenField ID="hdfCustomerNumber" runat="server" />
                    <asp:HiddenField ID="hdfLinkedID" runat="server" />
                    <asp:HiddenField ID="hdfLinkMandatory" runat="server" />
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <div class="stacked-for-small button-group">
            <asp:HyperLink ID="hplCommentConfirm" runat="server" CssClass="button ebiz-unreserve" data-open="comment-box" />
            <asp:Button ID="btnAddToBasket" runat="server" CssClass="button ebiz-add-to-basket" />
        </div>
    </div>

    <div id="comment-box" class="reveal ebiz-inline-modal" data-reveal>
        <div class="row">
            <div class="medium-3 columns">
                <asp:Label id="lblAddComment" runat="server" CssClass="middle" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox runat="server" ID="txtCommentConfirm" CssClass="ebiz-unreserve-comment" />
                <asp:RequiredFieldValidator runat="server" ID="rfvComment" ControlToValidate="txtCommentConfirm" CssClass="error" ValidationGroup="CustomerReservation" Display="Static" ClientIDMode="Static" Enabled="false"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="stacked-for-small button-group">
            <asp:HyperLink ID="hlkCloseCommentBox" runat="server" data-close CssClass="button ebiz-unreserve" />
            <asp:Button ID="btnUnreserve" runat="server" CssClass="button ebiz-unreserve" ValidationGroup="CustomerReservation" />
        </div>
    </div>      

    

</asp:Content>

