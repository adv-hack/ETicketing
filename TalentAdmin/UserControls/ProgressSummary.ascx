<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProgressSummary.ascx.vb" Inherits="UserControls_ProgressSummary" %>

<div id="ProgressSummary"> 
    <div class="title">
        <asp:Label ID="lblSiteHeader" runat="server"></asp:Label>
    </div>
    <div class="details">
        <asp:Repeater ID="DataList1" runat="server">
            <HeaderTemplate>
                <div class="Header">
                    <asp:Label ID="lblStage" runat="server">Stage</asp:Label>
                    <asp:Label ID="lblStatus" runat="server">Status</asp:Label>
                    <asp:Label ID="lblDetails" runat="server">Details</asp:Label>
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="item">
                    <asp:Label ID="lblStageDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "Stage")%></asp:Label>
                    <asp:Label ID="lblStatusDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "Status")%></asp:Label>
                    <asp:Label ID="lblDetailsDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "Details")%></asp:Label>
                </div>
           </ItemTemplate>
        </asp:Repeater>
    </div>
</div>