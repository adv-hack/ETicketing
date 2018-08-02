<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PackageSeatDetails.aspx.vb" Inherits="PagesPublic_ProductBrowse_PackageSeatDetails" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
 <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhNoSeatDetails" runat="server">
    <div class="alert-box info ebiz-no-seat-details">
        <asp:Literal ID="ltlNoSeatDetails" runat="server" />
    </div>
    </asp:PlaceHolder>

     <asp:PlaceHolder ID="plhRowSeats" runat="server">
         <asp:Repeater ID ="rptRowSeats" runat="server">
             <HeaderTemplate>
                 <table class="ebiz-row-seats-header ebiz-responsive-table">
                     <thead>
                         <tr>
                             <th scope="col" class="ebiz-stand-number"><%# GetText("StandColumn")%></th>
                             <th scope="col" class="ebiz-area-number"><%# GetText("AreaColumn")%></th>
                             <th scope="col" class="ebiz-row-number"><%# GetText("RowColumn")%></th>
                             <th scope="col" class="ebiz-seat-number"><%# GetText("SeatColumn")%></th>
                         </tr>
                     </thead>
                     <tbody>
             </HeaderTemplate>
             <ItemTemplate>
                        <tr>
                            <td id="StandData" class="ebiz-stand-number" data-title="<%# GetText("StandColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Stand").ToString()%></td>
                            <td id="AreaData" class="ebiz-area-number" data-title="<%# GetText("AreaColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Area").ToString()%></td>
                            <td id="RowData" class="ebiz-row-number" data-title="<%# GetText("RowColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Row_Num").ToString()%></td>
                            <td id="SeatData" class="ebiz-seat-number" data-title="<%# GetText("SeatColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Seats_Formatted").ToString()%></td>
                        </tr>
             </ItemTemplate>
             <FooterTemplate>
                    </tbody>
                 </table>
             </FooterTemplate>
         </asp:Repeater>
     </asp:PlaceHolder>

     <asp:PlaceHolder ID="plhComponentBulkSeats" runat="server">
         <asp:Repeater ID ="rptComponentBulkSeats" runat="server">
             <HeaderTemplate>
                 <table class ="ebiz-comp-bulk-seats-header ebiz-responsive-table">
                     <thead>
                         <tr>
                             <th scope="col" class="ebiz-product-desc"><%# GetText("ProductDescColumn")%></th>
                             <th scope="col" class="ebiz-customer"><%# GetText("CustomerColumn")%></th>
                             <th scope="col" class="ebiz-price-band"><%# GetText("PriceBandColumn")%></th>
                             <th scope="col" class="ebiz-quantity"><%# GetText("QuantityColumn")%></th>
                             <th scope="col" class="ebiz-stand-number"><%# GetText("StandColumn")%></th>
                             <th scope="col" class="ebiz-area-number"><%# GetText("AreaColumn")%></th>
                             <th scope="col" class="ebiz-row-number"><%# GetText("RowColumn")%></th>
                             <th scope="col" class="ebiz-seat-number"><%# GetText("SeatColumn")%></th>
                             <th scope="col" class="ebiz-price"><%# GetText("PriceColumn")%></th>
                         </tr>
                     </thead>
                     <tbody>
             </HeaderTemplate>
             <ItemTemplate>
                        <tr>
                            <td id="ProdDescData" class="ebiz-product-desc" data-title="<%# GetText("ProductDescColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Product_Desc").ToString()%></td>
                            <td id="CustomerData" class="ebiz-customer" data-title="<%# GetText("CustomerColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Customer").ToString()%></td>
                            <td id="PriceBandData" class="ebiz-price-band" data-title="<%# GetText("PriceBandColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Price_Band").ToString()%></td>
                            <td id="QuantityData" class="ebiz-quantity" data-title="<%# GetText("QuantityColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Seat_Count").ToString()%></td>
                            <td id="StandData" class="ebiz-stand-number" data-title="<%# GetText("StandColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Stand").ToString()%></td>
                            <td id="AreaData" class="ebiz-area-number" data-title="<%# GetText("AreaColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Area").ToString()%></td>
                            <td id="RowData" class="ebiz-row-number" data-title="<%# GetText("RowColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Row_Num").ToString()%></td>
                            <td id="SeatData" class="ebiz-seat-number" data-title="<%# GetText("SeatColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Seats_Formatted").ToString()%></td>
                            <td id="PriceData" class="ebiz-price" data-title="<%# GetText("PriceColumn")%>"><%# DataBinder.Eval(Container.DataItem, "Price").ToString()%></td>
                        </tr>
             </ItemTemplate>
             <FooterTemplate>
                    </tbody>
                 </table>
             </FooterTemplate>
         </asp:Repeater>
     </asp:PlaceHolder>

    <button class="close-button" data-close aria-label="Close modal" type="button">
      <span aria-hidden="true"><i class="fa fa-times"></i></span>
    </button>
     <script>
         $(document).ready(function () {
             setTimeout(function () {
                 $(".ebiz-close").focus();
             }, 1);
         });
    </script>
 </asp:Content>
