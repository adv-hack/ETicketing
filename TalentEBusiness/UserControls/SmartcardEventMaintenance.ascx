<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SmartcardEventMaintenance.ascx.vb"
    Inherits="UserControls_SmartcardEventMaintenance" %>

<div id="PageHeaderText">
  <asp:Label ID="PageHeaderTextLabel" runat="server"></asp:Label>
</div>

<asp:Label ID="CustomerLabel" runat="server" CssClass="SCCustLbl"></asp:Label>

<div class="error">
  <asp:BulletedList ID="errorlist" runat="server"> </asp:BulletedList>
</div>

<asp:Label ID="CardDataHeaderLabel" runat="server" CssClass="CardDataHeaderLabel"></asp:Label>
<asp:Label ID="hdnActiveCard" runat="server" Visible="False" CssClass="CardDataHeaderLabel"></asp:Label>
<div id="card-matrix" class="default-table">
  <table cellspacing="0" summary="Card Data">
    <asp:Repeater ID="CardRepeater" runat="server">
      <HeaderTemplate>
        <tr>

          <th class="cardProductHdr" scope="col"> <%#GetText("CardProductHdrLabel")%> </th>
          <th class="cardNumberHdr" scope="col"> <%#GetText("CardNumberHdrLabel")%> </th>
          <th class="cardStatusHdr" scope="col"> <%#GetText("CardStatusHdrLabel")%> </th>
          <th class="cardDateActivatedHdr" scope="col"> <%#GetText("CardDateActivatedHdrLabel")%> </th>
          <th class="cardDateDeactivatedH   dr" scope="col"> <%#GetText("CardDateDeactivatedHdrLabel")%> </th>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <asp:Panel ID="rowStyle" CssClass="rowActive" runat="server">
          <tr class="row">
           
            <td class="cardProduct"><asp:Label ID="cardProductLabel" runat="server"></asp:Label></td>
            <td class="cardNumber"><asp:Label ID="cardNumberLabel" runat="server"></asp:Label></td>
            <td class="cardStatus"><asp:Label ID="cardStatusLabel" runat="server"></asp:Label></td>
            <td class="cardDateActivated"><asp:Label ID="cardDateActivatedLabel" runat="server"></asp:Label></td>
            <td class="cardDateDeactivated"><asp:Label ID="cardDateDeactivatedLabel" runat="server"></asp:Label></td>
            <asp:HiddenField ID="hfCardRecID" runat="server" />
          </tr>
          </asp:Panel>
      </ItemTemplate>
    </asp:Repeater>
  </table>
</div>

<asp:Label ID="SaleDataHeaderLabel" runat="server" CssClass="SaleDataHeaderLabel"></asp:Label>
<div id="sale-matrix" class="default-table">
  <table cellspacing="0" summary="Sale Data">
    <asp:Repeater ID="SaleRepeater" runat="server" OnItemCommand="SaleRepeater_ItemCommand">
      <HeaderTemplate>
        <tr>
        
          <th class="saleProductHdr" scope="col"> <%#GetText("SaleProductHdrLabel")%> </th>
          <th class="saleSoldDateHdr" scope="col"> <%#GetText("SaleSoldDateHdrLabel")%> </th>
          <th class="saleSoldByHdr" scope="col"> <%#GetText("SaleSoldByHdrLabel")%> </th>
          <th class="saleStatusHdr" scope="col"> <%#GetText("SaleStatusHdrLabel")%> </th>
          <th class="saleOptionsHdr" scope="col"> <%#GetText("SaleOptionsHdrLabel")%> </th>
      
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <asp:Panel ID="rowStyle" CssClass="rowActive" runat="server">
          <tr class="row">
            
            <td class="saleProduct"><asp:Label ID="saleProductLabel" runat="server"></asp:Label></td>
            <td class="saleSoldDate"><asp:Label ID="saleSoldDateLabel" runat="server"></asp:Label></td>
            <td class="saleSoldBy"><asp:Label ID="saleSoldByLabel" runat="server"></asp:Label></td>
            <td class="saleStatus"><asp:Label ID="saleStatusLabel" runat="server"></asp:Label></td>
            <td class="saleOptions">
                <asp:Button ID="saleUploadButton" runat="server" CommandArgument="Upload"></asp:Button>
                <asp:Button ID="salePrintTicketButton" runat="server" CommandArgument="Print"></asp:Button>
            </td>
           
            <asp:HiddenField ID="hfSaleRecID" runat="server" />
            <asp:HiddenField ID="hfSCRecordId" runat="server" />
             <asp:HiddenField ID="cardNumber" runat="server" />
          </tr>
          </asp:Panel>
      </ItemTemplate>
    </asp:Repeater>
  </table>
</div>

<asp:Label ID="ErrorLogHeaderLabel" runat="server" CssClass="ErrorLogHeaderLabel"></asp:Label>
<div id="errorLog-matrix" class="default-table">
  <table cellspacing="0" summary="SC Error Log Data">
    <asp:Repeater ID="ErrorLogRepeater" runat="server">
      <HeaderTemplate>
        <tr>
          <th class="errLogDateHdr" scope="col"> <%#GetText("ErrLogDateHdrLabel")%> </th>
          <th class="errLogProductHdr" scope="col"> <%#GetText("ErrLogProductHdrLabel")%> </th>
          <th class="errLogCardNunHdr" scope="col"> <%#GetText("ErrLogCardNumHdrLabel")%> </th>
          <th class="errLogActionHdr" scope="col"> <%#GetText("ErrLogActionHdrLabel")%> </th>
          <th class="errLogNumberHdr" scope="col"> <%#GetText("ErrLogNumberHdrLabel")%> </th>
          <th class="errLogDescHdr" scope="col"> <%#GetText("ErrLogDescriptionHdrLabel")%> </th>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <asp:Panel ID="rowStyle" CssClass="rowInactive" runat="server">
          <tr class="row">
            <td class="errLogDate"><asp:Label ID="errLogDateLabel" runat="server"></asp:Label></td>
            <td class="errLogProduct"><asp:Label ID="errLogProductLabel" runat="server"></asp:Label></td>
            <td class="errLogCardNum"><asp:Label ID="errLogCardNumLabel" runat="server"></asp:Label></td>
            <td class="errLogAction"><asp:Label ID="errLogActionLabel" runat="server"></asp:Label></td>
            <td class="errLogNumber"><asp:Label ID="errLogNumberLabel" runat="server"></asp:Label></td>
            <td class="errLogDesc"><asp:Label ID="errLogDescLabel" runat="server"></asp:Label></td>
          </tr>
          </asp:Panel>
      </ItemTemplate>
    </asp:Repeater>
  </table>
</div>
