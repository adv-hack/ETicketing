<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SeasonTicketWaitListSummary.ascx.vb" Inherits="UserControls_SeasonTicketWaitListSummary" %>

<div class="wait-list-summary default-form" >
    <h2><asp:Label ID="titleLabel" runat="server"></asp:Label></h2>
        
    
    <p class="content">
        <asp:Label ID="contentLabel" runat="server"></asp:Label>
    </p>
    
        <asp:HiddenField ID="hfWaitListID" runat="server" />
        <div class="status-row">
            <asp:Label ID="statusLabel" runat="server" AssociatedControlID="statusVal"></asp:Label>
            <asp:Label ID="statusVal" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <div class="date-row">
            <asp:Label ID="dateLabel" runat="server" AssociatedControlID="dateVal"></asp:Label>
            <asp:Label ID="dateVal" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <div class="added-row">
            <asp:Label ID="addedLabel" runat="server" AssociatedControlID="addedVal"></asp:Label>
            <asp:Label ID="addedVal" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <div class="preferred-stand-row">
            <asp:Label ID="prefStand1Label" runat="server" AssociatedControlID="prefStand1Val"></asp:Label>
            <asp:Label ID="prefStand1Val" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <asp:Panel ID="pnlPreferredAreaRow1" runat="server">
            <div class="preferred-area-row">
                <asp:Label ID="prefArea1Label" runat="server" AssociatedControlID="prefArea1Val"></asp:Label>
                <asp:Label ID="prefArea1Val" runat="server" CssClass="wait-list-label"></asp:Label>
            </div>
        </asp:Panel>
        <div class="preferred-stand-row">
            <asp:Label ID="prefStand2Label" runat="server" AssociatedControlID="prefStand2Val"></asp:Label>
            <asp:Label ID="prefStand2Val" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <asp:Panel ID="pnlPreferredAreaRow2" runat="server">
         <div class="preferred-area-row">
            <asp:Label ID="prefArea2Label" runat="server" AssociatedControlID="prefArea2Val"></asp:Label>
            <asp:Label ID="prefArea2Val" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        </asp:Panel>
        <div class="preferred-stand-row">
            <asp:Label ID="prefStand3Label" runat="server" AssociatedControlID="prefStand3Val"></asp:Label>
            <asp:Label ID="prefStand3Val" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        <asp:Panel ID="pnlPreferredAreaRow3" runat="server">
        <div class="preferred-area-row">
            <asp:Label ID="prefArea3Label" runat="server" AssociatedControlID="prefArea3Val"></asp:Label>
            <asp:Label ID="prefArea3Val" runat="server" CssClass="wait-list-label"></asp:Label>
        </div>
        </asp:Panel>
       <div class="wait-list-repeater">
            <asp:Repeater ID="WaitListRepeater" runat="server">
                <HeaderTemplate>
                    <table cellspacing="0">
                        <tr>
                            <th><asp:Label ID="statusHeader" runat="server" OnLoad="GetText"></asp:Label></th>
                            <th><asp:Label ID="custNameHeader" runat="server" OnLoad="GetText"></asp:Label></th>
                            <th><asp:Label ID="custNumberHeader" runat="server" OnLoad="GetText"></asp:Label></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr>
                            <td><asp:Label ID="statusValLabel" runat="server"></asp:Label></td>
                            <td><asp:Label ID="custNameValLabel" runat="server"></asp:Label></td>
                            <td><asp:Label ID="custNumberValLabel" runat="server"></asp:Label></td>
                        </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="button-wrap">
            <asp:Button ID="withdrawButton" runat="server" CssClass="button" />
        </div>
</div>