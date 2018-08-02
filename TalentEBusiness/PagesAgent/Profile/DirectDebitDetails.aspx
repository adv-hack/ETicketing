<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DirectDebitDetails.aspx.vb" Inherits="PagesLogin_Profile_DirectDebitDetails" MaintainScrollPositionOnPostback="true" ViewStateMode="Disabled"  %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebit.ascx" TagName="DirectDebit" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div id="direct-debits" class="panel ebiz-direct-debit-details-wrap">
      <h2><asp:Literal ID="ltlDirectDebitDetailsTitle" runat="server" /></h2>
      <Talent:DirectDebit ID="uscDirectDebit" runat="server" UsePaymentDaysDDL="false" />
      <div id="DirectDebitTreasurer" class="ebiz-direct-debit-treasurer" runat="server">
          <asp:Checkbox ID="cbDirectDebitTreasurer" runat="server" />
          <asp:Label ID="DirectDebitTreasurerlabel" runat="server" AssociatedControlID="cbDirectDebitTreasurer" />
      </div>
      <div class="ebiz-update-direct-debit-details">
        <asp:Button ID="btnUpdateDirectDebitDetails" runat="server" CssClass="button" />
      </div>
    </div>
    <div class="panel ebiz-direct-debit-on-account-wrap">
      <h2><asp:Literal ID="ltlDirectDebitOnAccountTitle" runat="server" /></h2>
      <div id="ShowPaid" class="ebiz-show-paid" runat="server">
          <asp:checkBox ID="cbShowPaid" runat="server" AutoPostBack="true" />
      </div>
      <asp:PlaceHolder ID="plhDirectDebitOnAccountErrorMessages" runat="server">
          <asp:BulletedList ID="DirectDebitOnAccountErrorList" runat="server" CssClass="error" />
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhNoDirectDebitOnAccount" runat="server">
          <div class="alert-box warning ebiz-no-Direct-Debit-onaccount"><asp:Literal ID="noDDOnAccountText" runat="server" /></div>
      </asp:PlaceHolder>
      <asp:Repeater ID="rptDirectDebitOnAccount" runat="server">
          <HeaderTemplate>
            <table class="ebiz-responsive-table ebiz-directdebit-onaccount">
                <thead>
                  <tr>
                      <th scope="col" class="ebiz-date"><%=DateHeaderText%></th>
                      <th scope="col" class="ebiz-ref"><%=RefHeaderText%></th>
                      <th scope="col" class="ebiz-product"><%=ProductHeaderText%></th>
                      <th scope="col" class="ebiz-description"><%=DescriptionHeaderText%></th>
                      <th scope="col" class="ebiz-value"><%=ValueHeaderText%></th>
                      <th scope="col" class="ebiz-status"><%=PaidStatusHeaderText%></th>
                      <th scope="col" class="ebiz-payref"><%=PayrefHeaderText%></th> 
                      <th scope="col" class="ebiz-status"><%=StatusHeaderText%></th>
                      <th scope="col" class="ebiz-change">&nbsp;</th> 
                      <th scope="col" class="ebiz-delete">&nbsp;</th> 
                  </tr>
                </thead>  
              <tbody>
            </HeaderTemplate>
            <ItemTemplate>
              <tr>
                <td class="ebiz-Date" data-title="<%=DateHeaderText%>"><asp:Label ID="lblDate" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Date"))%>' /></td> 
                <td class="ebiz-ref" data-title="<%=RefHeaderText%>"><asp:Label ID="lblRef" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Reference"))%>' /></td> 
                <td class="ebiz-product" data-title="<%=ProductHeaderText%>"><asp:Label ID="lblProduct" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Product"))%>' /></td> 
                <td class="ebiz-description" data-title="<%=DescriptionHeaderText%>"><asp:Label ID="lblDescription"  runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Description"))%>' /></td>
                <td class="ebiz-value" data-title="<%=ValueHeaderText%>"><asp:Label ID="lblValue" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Value"))%>' /></td>
                <td class="ebiz-paid" data-title="<%=PaidStatusHeaderText%>"><asp:Label ID="lblPaid" runat="server" Enabled="false" /></td>
                <td class="ebiz-payref" data-title="<%=PayrefHeaderText%>"><asp:Label ID="lblPayref" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "payref"))%>' /></td>
                <td class="ebiz-status" data-title="<%=StatusHeaderText%>"><asp:Label ID="lblActive" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "status"))%>' /></td>
                <td class="ebiz-change"><asp:Button ID="btnChangeStatus" runat="server" CommandName="ChangeOnAccountStatusButton" CommandArgument='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Reference"))%>' CssClass="fa-input ebiz-fa-input ebiz-upate" /></td>
                <td class="ebiz-delete"><asp:Button ID="btnDelete" runat="server" CommandName="DeleteOnAccountButton" CommandArgument='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Reference"))%>' CssClass="fa-input ebiz-fa-input ebiz-remove" /></td>
              </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
              </table>
            </FooterTemplate>
          </asp:Repeater>
          </div>
          <div class="panel ebiz-direct-debit-balances-wrap">
          <h2><asp:Literal ID="ltlDirectDebitBalancesTitle" runat="server" /></h2>
          <asp:PlaceHolder ID="plhNoDirectDebitBalances" runat="server">
            <div class="alert-box warning ebiz-no-Direct-Debit-balances"><asp:Literal ID="noDDBalancesText" runat="server" /></div>
          </asp:PlaceHolder>
          <asp:Repeater ID="rptDirectDebitBalances" runat="server">
            <HeaderTemplate>
              <table class="ebiz-responsive-table ebiz-directdebit-balances">
                <thead>
                  <tr>
                    <th scope="col" class="ebiz-ref"><%=RefHeaderText%></th>
                    <th scope="col" class="ebiz-batch"><%=BatchHeaderText%></th>
                    <th scope="col" class="ebiz-date"><%=DateCreatedHeaderText%></th>
                    <th scope="col" class="ebiz-balance"><%=BalanceHeaderText%></th>
                    <th scope="col" class="ebiz-user"><%=UserHeaderText%></th>
                  </tr>
                </thead>
              <tbody>
            </HeaderTemplate>
            <ItemTemplate>
              <tr>
                <td class="ebiz-ref" data-title="<%=RefHeaderText%>"><asp:Label ID="lblReference" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Reference"))%>' /></td> 
                <td class="ebiz-batch" data-title="<%=BatchHeaderText%>"><asp:Label ID="lblBatch" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Batch"))%>' /></td> 
                <td class="ebiz-date" data-title="<%=DateCreatedHeaderText%>"><asp:Label ID="lblDateCreated"  runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "DateCreated"))%>' /></td>
                <td class="ebiz-balance" data-title="<%=BalanceHeaderText%>"><asp:Label ID="lblBalance" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Balance"))%>' /></td>
                <td class="ebiz-date" data-title="<%=UserHeaderText%>"><asp:Label ID="lblUser" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "User"))%>' /></td>
              </tr>
            </ItemTemplate>
          <FooterTemplate>
              </tbody>
            </table>
          </FooterTemplate>
      </asp:Repeater>
      <div class="ebiz-direct-debit-details-back">
        <asp:Button ID="btnBack" runat="server" CssClass="button" />
      </div>
    </div>
  </asp:Content>