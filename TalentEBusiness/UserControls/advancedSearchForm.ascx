<%@ Control Language="VB" AutoEventWireup="false" CodeFile="advancedSearchForm.ascx.vb"
    Inherits="UserControls_advancedSearchForm" %>
<%@ Register Src="advancedSearchType2.ascx" TagName="advancedSearchType2" TagPrefix="Talent" %>
<%@ Register Src="advancedSearchType3.ascx" TagName="advancedSearchType3" TagPrefix="Talent" %>
<%@ Register Src="advancedSearchType1.ascx" TagName="advancedSearchType1" TagPrefix="Talent" %>
<%@ Register Src="advancedSearchType4.ascx" TagName="advancedSearchType4" TagPrefix="Talent" %>
&nbsp;<div id="advanced-search" class="box">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="AdvancedSearch" />
    <div class="section">
        <asp:Label ID="sectionLabel" runat="server" AssociatedControlID="section"></asp:Label>
        <asp:DropDownList ID="section" CssClass="select" runat="server" AutoPostBack="true">
        </asp:DropDownList>
    </div>
    <Talent:advancedSearchType1 ID="AdvancedSearchType1" runat="server" />
    <Talent:advancedSearchType2 ID="AdvancedSearchType2" runat="server" />
    <Talent:advancedSearchType3 ID="AdvancedSearchType3" runat="server" />
    <Talent:advancedSearchType4 ID="AdvancedSearchType4" runat="server" />
    <div class="submit">
        <label>
            &nbsp;</label><asp:Button ID="searchButton" CssClass="button" runat="server" CausesValidation="true"
                ValidationGroup="AdvancedSearch" />
    </div>
</div>
