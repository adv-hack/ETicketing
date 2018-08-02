<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ChangeBU.ascx.vb" Inherits="UserControls_ChangeBU" %>
<asp:PlaceHolder ID="plhChangeBU" runat="server" Visible="false">
    <%--
    <asp:Button runat="server" ID="btnEN" Text="Change to EN" />
    <asp:Button runat="server" ID="btnTR" Text="Change to TR" />
    <asp:Button runat="server" ID="btnUK" Text="Change to UK" />
    --%>
    <asp:PlaceHolder ID="plhErrlist" runat="server">
        <asp:BulletedList ID="blErrList" runat="server" CssClass="alert-box alert"></asp:BulletedList>
    </asp:PlaceHolder>
    <div class="panel ebiz-change-bu">
        <ul class="inline-list">
            <asp:Repeater ID="rptBusinessUnits" runat="server">
                <ItemTemplate>
                    <asp:PlaceHolder ID="plhBusinessUnit" runat="server" Visible="true">
                        <li><asp:ImageButton ID="btnBusinessUnit" runat="server" /></li>
                    </asp:PlaceHolder>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</asp:PlaceHolder>