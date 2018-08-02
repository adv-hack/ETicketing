<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master" CodeFile="SelectPartner.aspx.vb" Inherits="Modules_SelectPartner" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="selectPartner">
        <asp:GridView ID="PartnersView" CssClass="defaultTable" runat="server" AllowSorting="True"
            DataKeyNames="PARTNER_ID" AllowPaging="True" PagerSettings-FirstPageText="First"
            PagerSettings-LastPageText="Last" PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous"
            PageSize="20" PagerSettings-Mode="NumericFirstLast">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="PARTNER_ID" DataNavigateUrlFormatString="~/Modules/AddModulesToPartner.aspx?PartnerID={0}"
                    Text="Select" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
