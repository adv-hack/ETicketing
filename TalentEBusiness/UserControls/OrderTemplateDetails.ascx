<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderTemplateDetails.ascx.vb" Inherits="UserControls_OrderTemplateDetails" %>
<%@ Register Src="SummaryTotals.ascx" TagName="SummaryTotals" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhReplaceBasketQuestion" runat="server" Visible="false">
<div id="ReplaceBasketQuestion">
    <div><asp:Label ID="lblReplaceBasketQuestion" runat="server" /></div>
    <div class="btn keep-and-replace">
        <asp:Button ID="btnReplaceItems" runat="server" CssClass="button" />
        <asp:Button ID="btnKeepItems" runat="server" CssClass="button" />
    </div>
    <input id="HiddenTemplateHeaderID" runat="server" type="hidden" />
</div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhErrorMessage" runat="server">
    <p class="error"><asp:Label ID="errlabel" runat="server"/></p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhResults" runat="server">

<div class="enquiry-results">
    <p class="pager-display"><asp:Label ID="CurrentResultsDisplaying" runat="server" /></p>
    <div class="show-all-button"><asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>" CssClass="button" /></div>
    <span class="pager-nav">
        <asp:LinkButton ID="FirstTop" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav1Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav2Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav3Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav4Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav5Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav6Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav7Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav8Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav9Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav10Top" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="LastTop" runat="server" OnClick="ChangePage" />
    </span>
</div>

<div id="order-enquiry-matrix">
    <asp:PlaceHolder ID="Repeater1PlaceHolder" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table cellspacing="0" summary="Order Templates">
                    <tr>
                        <th class="column1" scope="col"><asp:Label ID="column1Header" runat="server" OnPreRender="GetText" /></th>
                        <th class="column2" scope="col"><asp:Label ID="column2Header" runat="server" OnPreRender="GetText" /></th>
                        <th class="column3" scope="col"><asp:Label ID="column3Header" runat="server" OnPreRender="GetText" /></th>
                        <th class="column4" scope="col"><asp:Label ID="column4Header" runat="server" OnPreRender="GetText" /></th>
                        <th class="column5" scope="col"><asp:Label ID="column5Header" runat="server" OnPreRender="GetText" /></th>
                        <th class="buttons" scope="col"><asp:Label ID="buttonsHeader" runat="server" OnPreRender="GetText" /></th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="column1">
                        <asp:CheckBox ID="column1CheckBox" runat="server" />
                        <asp:Label ID="column1Label" runat="server" />
                    </td>
                    <td class="column2">
                        <asp:HyperLink ID="column2HyperLink" runat="server" />
                        <asp:Label ID="column2Label" runat="server" />
                    </td>
                    <td class="column3">
                        <asp:Label ID="column3Label" runat="server" />
                    </td>
                    <td class="column4">
                        <asp:TextBox ID="column4TextBox" runat="server" Width="40px" />
                        <asp:Label ID="lblMasterProduct" Visible="false" runat="server" />
                    </td>
                    <td class="column5">
                        <asp:Label ID="column5Label" runat="server" />
                    </td>
                    <asp:PlaceHolder ID="plhActions" runat="server" Visible='<%# ShowOptions %>'>
                    <td class="buttons">
                        <asp:Button ID="ViewDetailsLink" runat="server" OnPreRender="GetText" CssClass="button" />
                        <asp:Button ID="AddLink" runat="server" OnPreRender="GetText" CssClass="button" />
                        <asp:Button ID="DeleteLink" runat="server" OnPreRender="GetText" OnClick="DeleteItem" CssClass="button" />
                    </td>
                    </asp:PlaceHolder>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                    <td class="column1">
                        <asp:CheckBox ID="column1CheckBox" runat="server" />
                        <asp:Label ID="column1Label" runat="server" />
                    </td>
                    <td class="column2">
                        <asp:HyperLink ID="column2HyperLink" runat="server" />
                        <asp:Label ID="column2Label" runat="server" />
                    </td>
                    <td class="column3">
                        <asp:Label ID="column3Label" runat="server" />
                    </td>
                    <td class="column4">
                        <asp:TextBox ID="column4TextBox" runat="server" Width="40px" />
                        <asp:Label ID="lblMasterProduct" Visible="false" runat="server" />
                    </td>
                    <td class="column5">
                        <asp:Label ID="column5Label" runat="server" />
                    </td>
                    <asp:PlaceHolder ID="plhActions" runat="server" Visible='<%# ShowOptions %>'>
                    <td class="buttons">
                        <asp:Button ID="ViewDetailsLink" runat="server" OnPreRender="GetText" CssClass="button" />
                        <asp:Button ID="AddLink" runat="server" OnPreRender="GetText" CssClass="button" />
                        <asp:Button ID="DeleteLink" runat="server" OnPreRender="GetText" OnClick="DeleteItem" CssClass="button" />
                    </td>
                    </asp:PlaceHolder>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                <tr>
                    <td colspan="5">
                        <asp:Button ID="SelectAll" runat="server" OnPreRender="GetText" OnClick="SelectAll_Click" CssClass="button selectAll" />
                        <asp:Button ID="DeSelectAll" runat="server" OnPreRender="GetText" OnClick="DeSelectAll_Click" CssClass="button clearAll" />
                    </td>
                </tr>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="Repeater2PlaceHolder" runat="server">
        <asp:Repeater ID="Repeater2" runat="server">
            <HeaderTemplate>
                <table cellspacing="0" summary="Order Templates">
                    <tr>
                        <th class="column1" scope="col" id="layout2_col1" onprerender="SetLayout2_Visibility" runat="server">
                            <asp:Label ID="column1Header" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column2" scope="col">
                            <asp:Label ID="productImageColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column3" scope="col">
                            <asp:Label ID="productCodeColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column4" scope="col">
                            <asp:Label ID="descriptionColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column5" scope="col">
                            <asp:Label ID="packsizeColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column6" scope="col">
                            <asp:Label ID="quantityColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                        <th class="column7" scope="col">
                            <asp:Label ID="ValueColumnHeader" runat="server" OnPreRender="GetText" />
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="column1" runat="server" id="layout2_col1" onprerender="SetLayout2_Visibility"><asp:CheckBox ID="column1CheckBox" runat="server" /></td>
                    <td class="column2"><asp:Image ID="img" runat="server" /></td>
                    <td class="column3">
                        <asp:Label ID="productCodeLabel" runat="server" OnPreRender="GetText" />
                        <asp:HyperLink ID="productCodeLink" runat="server" />
                        <asp:Label ID="ProductDescription" runat="server" />
                        <asp:HiddenField ID="hdfProductCode" runat="server" />
                        <asp:HiddenField ID="hdfMasterProduct" runat="server" />
                    </td>
                    <td class="column4">
                        <asp:Label ID="productBrandLabel" runat="server" />
                        <asp:Label ID="html1" runat="server" />
                        <asp:Label ID="html2" runat="server" />
                    </td>
                    <td class="column5"><asp:Label ID="packSizeLabel" runat="server" /></td>
                    <td class="column6"><asp:TextBox ID="qtyBox" runat="server" /></td>
                    <td class="column7"><asp:Label ID="priceLabel" runat="server" /></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>
</div>

<div class="enquiry-results">
    <span class="pager-nav">
        <asp:LinkButton ID="FirstBottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav1Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav2Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav3Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav4Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav5Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav6Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav7Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav8Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav9Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="Nav10Bottom" runat="server" OnClick="ChangePage" />
        <asp:LinkButton ID="LastBottom" runat="server" OnClick="ChangePage" />
    </span>
</div>

<Talent:SummaryTotals ID="SummaryTotals1" runat="server" Usage="OrderTemplate" />

<div class="btn update-buttons">
    <asp:Button ID="AddToBasketButton" runat="server" OnPreRender="GetText" CssClass="button" />
    <asp:Button ID="UpdateButton" runat="server" OnPreRender="GetText" CssClass="button" />
</div>

</asp:PlaceHolder>