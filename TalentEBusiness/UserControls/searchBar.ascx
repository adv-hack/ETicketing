<%@ Control Language="VB" AutoEventWireup="false" CodeFile="searchBar.ascx.vb" Inherits="UserControls_searchBar" ViewStateMode="Disabled" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">
    function pageLoad() {
    var myDelegate = Function.createDelegate(this, OnAcShowing);
    $find('aceProductSearch').add_showing(myDelegate);
    }

    function OnAcShowing(sender, args){
        sender._popupBehaviour.setpositioningMode(AjaxControlToolkit.PositioningMode.TopLeft);
    }

</script>

<div class="input-group ebiz-product-search-wrap">
	<asp:TextBox ID="txbSearch" runat="server" placeholder='<%# Search %>' CssClass="input-group-field"></asp:TextBox>
    <ajaxToolkit:AutoCompleteExtender ID="aceProductSearch" runat="server" TargetControlID="txbSearch" FirstRowSelected="false"
            ServiceMethod="GetProductCodeList" MinimumPrefixLength="2" CompletionInterval="100" EnableCaching="true">
    </ajaxToolkit:AutoCompleteExtender>
	<div class="input-group-button">
		<asp:Button ID="btnSubmit" CssClass="button expand fa-input" runat="server" />
	</div>
</div>
<asp:PlaceHolder ID="plhAddToBasket" runat="server" Visible="false">
    <asp:Button ID="btnAddToBasket" CssClass="button ebiz-add-to-basket" runat="server" />
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhAdvancedSearch" runat="server" Visible="false">
    <span class="ebiz-advanced-search">
        <asp:HyperLink ID="hypAdvancedSearch" runat="server" NavigateUrl="~/PagesPublic/Search/advancedSearch.aspx">[hypAdvancedSearch]</asp:HyperLink>
    </span>
</asp:PlaceHolder>


