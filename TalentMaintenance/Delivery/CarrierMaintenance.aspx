<%@ Page Language="VB"MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="CarrierMaintenance.aspx.vb" Inherits="Delivery_CarrierMaintenance" %>

<asp:Content ID="ContentArea1" runat="server" ContentPlaceHolderID="Content1"></asp:Content>

<asp:Content ID="ContentArea2" runat="server" ContentPlaceHolderID="Content2">
    
    <div class="unavailable-days-wrapper">
        <div class="unavailable-days-detail">
            <p class="title"><asp:Literal ID="ltlPageTitle" runat="server" /></p>
            <p class="instructions"><asp:Literal ID="ltlPageInstructions" runat="server" /></p>
        </div>

        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <p class="error"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
        </asp:PlaceHolder>

        <div class="carrier-table">
            <asp:Repeater ID="rptCarrier" runat="server">
                <HeaderTemplate>
                <table cellspacing="0" class="defaultTable">
                    <thead>
                        <tr>
                            <th scope="col"><%= CarrierCodeHeaderText%></th>
                            <th scope="col"><%= InstallationAvailableHeaderText%></th>
                            <th scope="col"><%= CollectOldAvailableHeaderText%></th>
                            <th scope="col"><%= DeliverMondayHeaderText%></th>
                            <th scope="col"><%= DeliverTuesdayHeaderText%></th>
                            <th scope="col"><%= DeliverWednesdayHeaderText%></th>
                            <th scope="col"><%= DeliverThursdayHeaderText%></th>
                            <th scope="col"><%= DeliverFridayHeaderText%></th>
                            <th scope="col"><%= DeliverSaturdayHeaderText%></th>
                            <th scope="col"><%= DeliverSundayHeaderText%></th>
                            <th scope="col">&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                       <tr>
                            <td><asp:TextBox ID="txtCarrierCode" runat="server" MaxLength="200" Text='<%# DataBinder.Eval(Container.DataItem, "CARRIER_CODE").ToString() %>' /></td>
                            <td><asp:CheckBox ID="chkInstallationAvailable" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "INSTALLATION_AVAILABLE")) %>' /></td>
                            <td><asp:CheckBox ID="chkCollectOldAvailable" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "COLLECT_OLD_AVAILABLE")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverMonday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_MONDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverTuesday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_TUESDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverWednesday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_WEDNESDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverThursday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_THURSDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverFriday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_FRIDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverSaturday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_SATURDAY")) %>' /></td>
                            <td><asp:CheckBox ID="chkDeliverSunday" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "DELIVER_SUNDAY")) %>' /></td>
                            <td>
                                <asp:Button ID="btnSave" runat="server" CssClass="button save" Text='<%# SaveButtonText %>' CommandName="Save" 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID").ToString() %>' />
                                <asp:Button ID="btnDelete" runat="server" CssClass="button delete" Text='<%# DeleteButtonText %>' CommandName="Delete" 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID").ToString() %>' />
                            </td>
                       </tr> 
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="plhNoCarriers" runat="server">
                <p><asp:Literal ID="ltlNoCarriers" runat="server" /></p>
            </asp:PlaceHolder>

            <fieldset class="add-carrier">
                <legend><asp:Literal ID="ltlAddCarrierLegend" runat="server" /></legend>
                <ul>
                    <li class="carrier-code">
                        <asp:Label ID="lblAddCarrierCode" runat="server" CssClass="label" AssociatedControlID="txtAddCarrierCode" />
                        <span class="value"><asp:TextBox ID="txtAddCarrierCode" runat="server" MaxLength="200" /></span>
                    </li>
                    <li class="installation-available">
                        <asp:Label ID="lblAddInstallationAvailable" runat="server" CssClass="label" AssociatedControlID="chkAddInstallationAvailable" />
                        <span class="value"><asp:CheckBox ID="chkAddInstallationAvailable" runat="server" /></span>
                    </li>
                    <li class="collect-old-available">
                        <asp:Label ID="lblAddCollectOldAvailable" runat="server" CssClass="label" AssociatedControlID="chkAddCollectOldAvailable" />
                        <span class="value"><asp:CheckBox ID="chkAddCollectOldAvailable" runat="server" /></span>
                    </li>
                    <li class="deliver-monday">
                        <asp:Label ID="lblAddDeliverMonday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverMonday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverMonday" runat="server" /></span>
                    </li>
                    <li class="deliver-tuesday">
                        <asp:Label ID="lblAddDeliverTuesday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverTuesday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverTuesday" runat="server" /></span>
                    </li>
                    <li class="deliver-wednesday">
                        <asp:Label ID="lblAddDeliverWednesday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverWednesday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverWednesday" runat="server" /></span>
                    </li>
                    <li class="deliver-thursday">
                        <asp:Label ID="lblAddDeliverThursday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverThursday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverThursday" runat="server" /></span>
                    </li>
                    <li class="deliver-friday">
                        <asp:Label ID="lblAddDeliverFriday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverFriday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverFriday" runat="server" /></span>
                    </li>
                    <li class="deliver-saturday">
                        <asp:Label ID="lblAddDeliverSaturday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverSaturday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverSaturday" runat="server" /></span>
                    </li>
                    <li class="deliver-sunday">
                        <asp:Label ID="lblAddDeliverSunday" runat="server" CssClass="label" AssociatedControlID="chkAddDeliverSunday" />
                        <span class="value"><asp:CheckBox ID="chkAddDeliverSunday" runat="server" /></span>
                    </li>
                    <li class="add-carrier-submit-button"><asp:Button ID="btnAddCarrier" runat="server" CssClass="button" /></li>
                </ul>
            </fieldset>
        </div>
    </div>
</asp:Content>