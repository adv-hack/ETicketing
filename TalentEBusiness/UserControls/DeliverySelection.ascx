<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DeliverySelection.ascx.vb" Inherits="UserControls_DeliverySelection" %>
<div class="delivery-selection-wrap">
    <asp:PlaceHolder ID="plhTitle" runat="server">
        <h2>
            
                <asp:Label ID="titleLabel" runat="server" />
            
        </h2>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhDeliveryPanel1" runat="server">
        <div class="row ebiz-delivery-selection-ddl1" id="DeliveryPanel1" runat="server">
            <div class="large-3 columns">
                <asp:Label ID="ddl1Label" runat="server" AssociatedControlID="DeliveryDDL1" />
            </div>
            <div class="large-9 columns">
                <asp:DropDownList ID="DeliveryDDL1" runat="server" AutoPostBack="true" />
                <div class="alert-box info ebiz-delivery-selection-description1">
                    <asp:Literal ID="ltlDesc1" runat="server" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhDeliveryPanel2" runat="server">
        <div class="row ebiz-delivery-selection-ddl2" id="DeliveryPanel2" runat="server">
            <div class="large-3 columns">
                <asp:Label ID="ddl2Label" runat="server" AssociatedControlID="DeliveryDDL2" />
            </div>
            <div class="large-9 columns">
                <asp:DropDownList ID="DeliveryDDL2" runat="server" AutoPostBack="true" />
                <div class="alert-box info ebiz-delivery-selection-description2">
                    <asp:Literal ID="ltlDesc2" runat="server" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhDeliverySelectionInfo" runat="server">
    <div class="alert-box info ebiz-delivery-selection-info">
        <asp:Label ID="infoLabel" runat="server" />
    </div>
    </asp:PlaceHolder>
</div>