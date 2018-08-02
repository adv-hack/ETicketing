<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductSubTypesFilter.ascx.vb" Inherits="UserControls_ProductSubTypesFilter" %>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="Search" CssClass="alert-box alert" />
<asp:Panel ID="plhSearchOptions" runat="server" CssClass="panel ebiz-product-subtypes-options">
    <div class="row">
        <asp:PlaceHolder ID="plhSearchType" runat="server">
            <div class="column ebiz-subtype-option-item ebiz-search-type">
                <asp:Label ID="lblSearchType" runat="server" AssociatedControlID="ddlSearchType" />
                <asp:DropDownList ID="ddlSearchType" runat="server" ViewStateMode="Enabled" AutoPostBack="true" OnSelectedIndexChanged="ddlSearchType_SelectedIndexChanged"/>
            </div>
        </asp:PlaceHolder> 
        <asp:PlaceHolder ID="plhSortBy" runat="server">
            <div class="column ebiz-subtype-option-item ebiz-sort-by">
                <asp:Label ID="lblSortBy" runat="server" AssociatedControlID="ddlSortBy" />
                <asp:DropDownList ID="ddlSortBy" runat="server" ViewStateMode="Enabled" AutoPostBack="true" OnSelectedIndexChanged="ddlSortBy_SelectedIndexChanged"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhViewType" runat="server">
            <div class="column ebiz-subtype-option-item ebiz-view-type">
                <asp:Label ID="lblViewType" runat="server" AssociatedControlID="ddlViewType" />
                <asp:DropDownList ID="ddlViewType" runat="server" ViewStateMode="Enabled" AutoPostBack="true" OnSelectedIndexChanged="ddlViewType_SelectedIndexChanged"/>
            </div>
        </asp:PlaceHolder>
    </div>      
</asp:Panel>
<asp:Panel ID="plhSearchFilter" runat="server" DefaultButton="btnSearch" CssClass="panel ebiz-product-subtypes-filter">       
    <div class="row">
        <asp:PlaceHolder ID="plhKeyword" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-keywords">
                <asp:Label ID="lblKeyword" runat="server" AssociatedControlID="txtKeyword" />
                <asp:TextBox ID="txtKeyword" runat="server" ViewStateMode="Enabled"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhCategory" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-category">
                <asp:Label ID="lblCategory" runat="server" AssociatedControlID="ddlCategory" />
                <asp:DropDownList ID="ddlCategory" runat="server" ViewStateMode="Enabled" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhLocation" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-location">
                <asp:Label ID="lblLocation" runat="server" AssociatedControlID="ddlLocation" />
                <asp:DropDownList ID="ddlLocation" runat="server" ViewStateMode="Enabled" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhProductType" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-product-type">
                <asp:Label ID="lblProductType" runat="server" AssociatedControlID="ddlProductType" />
                <asp:DropDownList ID="ddlProductType" runat="server" ViewStateMode="Enabled" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhStadium" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-stadium">
                <asp:Label ID="lblStadium" runat="server" AssociatedControlID="ddlStadium" />
                <asp:DropDownList ID="ddlStadium" runat="server" ViewStateMode="Enabled" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhDate" runat="server">
            <div class="column ebiz-subtype-filter-item ebiz-date">
                <asp:Label ID="lblDates" runat="server" AssociatedControlID="txtDate" />
                <asp:TextBox ID="txtDate" runat="server" CssClass="datepicker" ViewStateMode="Enabled" />
                <asp:RegularExpressionValidator ControlToValidate="txtDate" ID="txtDateRegEx" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="Search" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </asp:PlaceHolder>
    </div>
    <div class="button-group ebiz-subtype-filter-buttons">
        <asp:PlaceHolder ID="plhSearchButton" runat="server">
            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="button ebiz-primary-action ebiz-search" ValidationGroup="Search" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhClearButton" runat="server" Visible="False">
            <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" CssClass="button ebiz-clear" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhSaveAndSearchButton" runat="server">
            <asp:Button ID="btnSaveAndSearch" runat="server" OnClick="btnSaveAndSearch_Click" CssClass="button ebiz-save-and-search" />
        </asp:PlaceHolder>
    </div>
</asp:Panel>