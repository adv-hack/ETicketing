<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderReturn.ascx.vb" Inherits="UserControls_OrderReturn" %>

<asp:PlaceHolder ID="plhErrorList" runat="server">
<div class="alert-box alert">
    <asp:BulletedList ID="errorlist" runat="server" />
</div>    
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhPageHeaderText" runat="server">
     <p><asp:Literal ID="PageHeaderTextLabel" runat="server" /></p>
</asp:PlaceHolder>
    

<div class="panel ebiz-buybacks-comments">
    <h2><asp:Literal ID="CommentBoxLabel" runat="server" /></h2>
    <div class="row ebiz-buybacks-comment1">
        <div class="medium-3 columns">
            <asp:Label ID="comment1Label" runat="server" AssociatedControlID="commentSelect" />
        </div>
        <div class="medium-9 columns">
            <asp:DropDownList ID="commentSelect" runat="server" />
        </div>
    </div>
    <div class="row ebiz-buybacks-comment2">
        <div class="medium-3 columns">
            <asp:Label ID="comment2Label" runat="server" AssociatedControlID="commentTxtBox" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="commentTxtBox" runat="server" MaxLength="50" />
        </div>
    </div>
</div>    
    
<asp:Repeater ID="CustomerRepeater" runat="server">
    <ItemTemplate>
        <div class="panel ebiz-buyback">
            <div class="row ebiz-buyback-customer">
                <div class="large-3 columns">
                    <asp:Literal ID="ltlCustomerNumberLabel" runat="server" />
                </div>
                <div class="large-9 columns">
                    <asp:Literal ID="ltlCustomerNumberValue" runat="server" />
                </div>
            </div>
            <asp:Repeater ID="OrderRepeater" runat="server" OnItemDataBound="DoOrdersRepeaterItemDatabound">
                <HeaderTemplate>
                    <table>
                        <thead>
                        <tr>
                            <th class="ebiz-buybacks-membership-match" scope="col"><%#GetText("membershipMatchLabel")%></th>
                            <th class="ebiz-buybacks-date" scope="col"><%#GetText("dateLabel")%></th>
                            <th class="ebiz-buybacks-seat" scope="col"><%#GetText("seatLabel")%></th>
                            <th class="ebiz-buybacks-status" scope="col"><%#GetText("statusLabel")%></th>
                        </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr>
                            <td class="ebiz-buyback-membership-match" data-title="<%#GetText("membershipMatchLabel")%>"><asp:Literal ID="matchLabel" runat="server" /></td>
                            <td class="ebiz-buybacks-date" data-title="<%#GetText("dateLabel")%>"><asp:Literal ID="dateLabel" runat="server" /></td>
                            <td class="ebiz-buybacks-seat" data-title="<%#GetText("seatLabel")%>"><asp:Literal ID="seatLabel" runat="server" /></td>
                            <td class="ebiz-buybacks-status" data-title="<%#GetText("statusLabel")%>"><asp:Literal ID="statusLabel" runat="server" /></td>
                        </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>                       
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>


<asp:PlaceHolder ID="ButtonsPanel" runat="server">
    <div class="stacked-for-small button-group ebiz-buyback-button-wrap">
        <asp:Button ID="BackButton" CssClass="button ebiz-back" runat="server" />
        <asp:Button ID="ContinueButton" CssClass="button ebiz-primary-action ebiz-continue" runat="server" />
    </div>
</asp:PlaceHolder>