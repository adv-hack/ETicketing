<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SmartcardCardMaintenance.ascx.vb"
    Inherits="UserControls_SmartcardCardMaintenance" %>

<div id="PageHeaderText">
  <asp:Label ID="PageHeaderTextLabel" runat="server"></asp:Label>
</div>
<asp:Label ID="CustomerLabel" runat="server" CssClass="SCCustLbl"></asp:Label>
<div class="error">
  <asp:BulletedList ID="errorlist" runat="server"> </asp:BulletedList>
</div>
<asp:Label ID="SCFunctionHeaderLabel" runat="server" CssClass="SCMaintHdrLbl"></asp:Label>
<asp:Panel ID="functionPanel" runat="server" CssClass="SCFunctionPanel">
  <div id="function-panel">
    <div class="fp-buttons">
      <p><asp:Button ID="ProduceCardButton" CssClass="button ProduceCardButton" runat="server" /></p>
      <p><asp:Button ID="ReprintCardButton" CssClass="button ReprintCardButton" runat="server" /></p>
      <p><asp:Button ID="CancelCardButton" CssClass="button CancelCardButton" runat="server" /></p>
    </div>
    <div class="fp-select">
      <asp:Label ID="PrinterLabel" runat="server" AssociatedControlID="PrinterListDD"></asp:Label>
      <asp:DropDownList ID="PrinterListDD" CssClass="select" runat="server"> </asp:DropDownList>
    </div>
  </div>
  </asp:Panel>
<asp:Label ID="SaleDataHeaderLabel" runat="server" CssClass="SaleDataHeaderLabel"></asp:Label>
<div id="sale-matrix" class="default-table">
  <table cellspacing="0" summary="Sale Data">
    <asp:Repeater ID="SaleRepeater" runat="server">
      <HeaderTemplate>
        <tr>
          <th class="selection" scope="col"> </th>
          <th class="saleProductHdr" scope="col"> <%#GetText("SaleProductHdrLabel")%> </th>
          <th class="saleSoldDateHdr" scope="col"> <%#GetText("SaleSoldDateHdrLabel")%> </th>
          <th class="saleSoldByHdr" scope="col"> <%#GetText("SaleSoldByHdrLabel")%> </th>
          <th class="saleCancDateHdr" scope="col"> <%#GetText("SaleCancDateHdrLabel")%> </th>
          <th class="saleCancByHdr" scope="col"> <%#GetText("SaleCancByHdrLabel")%> </th>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <asp:Panel ID="rowStyle" CssClass="rowActive" runat="server">
          <tr class="row">
            <td class="saleSelectionCBox"><asp:CheckBox ID="saleSelectionCBox" runat="server"></asp:CheckBox></td>
            <td class="saleProduct"><asp:Label ID="saleProductLabel" runat="server"></asp:Label></td>
            <td class="saleSoldDate"><asp:Label ID="saleSoldDateLabel" runat="server"></asp:Label></td>
            <td class="saleSoldBy"><asp:Label ID="saleSoldByLabel" runat="server"></asp:Label></td>
            <td class="saleCancDate"><asp:Label ID="saleCancDateLabel" runat="server"></asp:Label></td>
            <td class="saleCancBy"><asp:Label ID="saleCancByLabel" runat="server"></asp:Label></td>
            <asp:HiddenField ID="hfSaleRecID" runat="server" />
          </tr>
          </asp:Panel>
      </ItemTemplate>
    </asp:Repeater>
  </table>
</div>
<asp:Label ID="CardDataHeaderLabel" runat="server" CssClass="CardDataHeaderLabel"></asp:Label>
<div id="card-matrix" class="default-table">
  <table cellspacing="0" summary="Card Data">
    <asp:Repeater ID="CardRepeater" runat="server">
      <HeaderTemplate>
        <tr>
          <th class="selection" scope="col"> </th>
          <th class="cardProductHdr" scope="col"> <%#GetText("CardProductHdrLabel")%> </th>
          <th class="cardNumberHdr" scope="col"> <%#GetText("CardNumberHdrLabel")%> </th>
          <th class="cardStatusHdr" scope="col"> <%#GetText("CardStatusHdrLabel")%> </th>
          <th class="cardDateActivatedHdr" scope="col"> <%#GetText("CardDateActivatedHdrLabel")%> </th>
          <th class="cardDateDeactivatedHdr" scope="col"> <%#GetText("CardDateDeactivatedHdrLabel")%> </th>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <asp:Panel ID="rowStyle" CssClass="rowActive" runat="server">
          <tr class="row">
            <td class="cardSelectionCBox"><asp:CheckBox ID="cardSelectionCBox" runat="server"></asp:CheckBox></td>
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
