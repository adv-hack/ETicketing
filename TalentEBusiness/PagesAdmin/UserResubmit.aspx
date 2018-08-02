<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserResubmit.aspx.vb" Inherits="PagesAdmin_UserResubmit" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>Simon Jersey User Resubmit</h1>
    <asp:BulletedList ID="ErrorList" runat="server">
    </asp:BulletedList>
    Registration Type:
    <asp:DropDownList ID="RegTypeList" runat="server">
        <asp:ListItem Text="Standard (Type 1)" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Partner & User (Type 2)" Value="2" Selected="false"></asp:ListItem>
    </asp:DropDownList>
    Usage:<br />
    <asp:RadioButtonList ID="IterativeOnOff" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Single User Mode" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Iteritive Mode" Value="2" Selected="false"></asp:ListItem>
    </asp:RadioButtonList>
    Partner Prefix:<asp:TextBox ID="PartnerPrefixBox" runat="server"></asp:TextBox><br />
    Start Partner:<asp:TextBox ID="PartnerBox" runat="server"></asp:TextBox><br />
    Start LoginID:<asp:TextBox ID="LoginIdBox" runat="server"></asp:TextBox><br />
    <asp:Label ID="ItDirectionLabel" runat="server" >Increase Count Up or Down:</asp:Label><br />
    <asp:RadioButtonList ID="UpOrDownList" runat="server">
        <asp:ListItem Text="Up" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Down" Value="2" Selected="false"></asp:ListItem>
    </asp:RadioButtonList>
    <br />
    <asp:Label ID="NoOfItsLabel" runat="server">No. Of Iterations:</asp:Label><asp:TextBox ID="iterations" runat="server">0</asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="Go" />
</asp:Content>

