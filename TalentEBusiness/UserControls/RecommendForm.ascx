<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RecommendForm.ascx.vb"
    Inherits="UserControls_RecommendForm" %>
<div id="RecommendBox" class="RecommendBox" runat="server">
    <div>
        <p><asp:Label ID="RecommendTitleLabel" runat="server"></asp:Label></p>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
            ValidationGroup="Recommendation" />
    </div>
    <div>
        <asp:Label ID="YourNameLabel" runat="server" AssociatedControlID="YourNameTextBox"></asp:Label>
        <asp:TextBox ID="YourNameTextBox" runat="server" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="YourNameRFV" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="YourNameTextBox" Display="Static" Text="*"></asp:RequiredFieldValidator>
    </div>
    <div>
        <asp:Label ID="YourEmailLabel" runat="server" AssociatedControlID="YourEmailTextBox"></asp:Label>
        <asp:TextBox ID="YourEmailTextBox" runat="server" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="YourEmailRFV" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="YourEmailTextBox" Display="Static" Text="*"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="YourEmailRegEx" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="YourEmailTextBox" Display="Static" Text="*"></asp:RegularExpressionValidator>
    </div>
    <div>
        <asp:Label ID="FriendsNameLabel" runat="server" AssociatedControlID="FriendsNameTextBox"></asp:Label>
        <asp:TextBox ID="FriendsNameTextBox" runat="server" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="FriendsNameRFV" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="FriendsNameTextBox" Display="Static" Text="*"></asp:RequiredFieldValidator>
    </div>
    <div>
        <asp:Label ID="FriendsEmailLabel" runat="server" AssociatedControlID="FriendsEmailTextBox"></asp:Label>
        <asp:TextBox ID="FriendsEmailTextBox" runat="server" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="FriendsEmailRFV" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="FriendsEmailTextBox" Display="Static" Text="*"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="FriendsEmailRegEx" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="FriendsEmailTextBox" Display="Static" Text="*"></asp:RegularExpressionValidator>
    </div>
    <div>
        <asp:Label ID="MessageLabel" runat="server" AssociatedControlID="MessageTextBox"></asp:Label>
        <asp:TextBox ID="MessageTextBox" runat="server" Rows="5" TextMode="MultiLine" CssClass="textarea"></asp:TextBox>
        <asp:RequiredFieldValidator ID="MessageTextBoxRFV" runat="server" ValidationGroup="Recommendation"
            ControlToValidate="MessageTextBox" Display="Static" Text="*"></asp:RequiredFieldValidator>
    </div>
    <div>
        <label>
            &nbsp;</label>
        <asp:Button ID="RecommendBtn" runat="server" ValidationGroup="Recommendation" CausesValidation="true"
            CssClass="button" />
    </div>
</div>
