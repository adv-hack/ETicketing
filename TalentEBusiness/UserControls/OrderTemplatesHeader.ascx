<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderTemplatesHeader.ascx.vb" Inherits="UserControls_OrderTemplatesHeader" %>

<asp:PlaceHolder id="plhReplaceBasketQuestion" runat="server" visible="false">
    <div class="replace-basket-items">
        <h2><asp:Label ID="lblReplaceBasketQuestion" runat="server" /></h2>
        <div class="btn keep-and-replace">
            <asp:Button ID="btnReplaceItems" runat="server" CssClass="button replace-items" />
            <asp:Button ID="btnKeepItems" runat="server" CssClass="button keep-items" />
        </div>
        <input id="HiddenTemplateHeaderID" runat="server" type="hidden" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoOrderTemplates" runat="server">
    <div class="no-order-templates">
        <p><asp:Literal ID="ltlNoOrderTemplates" runat="server" /></p>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhOrderTemplates" runat="server">
<div class="enquiry-results">
    <p class="pager-display"><asp:Label ID="lblCurrentResultsDisplaying" runat="server" /></p>
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
    <asp:Repeater ID="rptOrderTemplateHeader" runat="server">
        <HeaderTemplate>
            <table cellspacing="0" summary="Order Templates">
                <tr>
                    <th class="column1" scope="col">
                        <asp:Label ID="column1Header" runat="server" OnPreRender="GetText" />
                    </th>
                    <th class="column2" scope="col">
                        <asp:Label ID="column2Header" runat="server" OnPreRender="GetText" />
                    </th>
                    <asp:PlaceHolder ID="plhDefaultOption" runat="server" Visible='<%# ShowOptions %>'>
                    <th class="column3" scope="col">
                        <asp:Label ID="column3Header" runat="server" OnPreRender="GetText" />
                    </th>
                    </asp:PlaceHolder>
                    <th class="buttons" scope="col">
                        <asp:Label ID="buttonsHeader" runat="server" OnPreRender="GetText" />
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="column1">
                    <asp:HyperLink ID="column1HyperLink" runat="server" Visible="false" />
                    <asp:Label ID="column1Label" runat="server" Visible="false" />
                </td>
                <td class="column2">
                    <asp:HyperLink ID="column2HyperLink" runat="server" Visible="false" />
                    <asp:Label ID="column2Label" runat="server" Visible="false" />
                </td>
                <asp:PlaceHolder ID="plhDefaultOption" runat="server" Visible='<%# ShowOptions %>'>
                <td class="column3">
                    <asp:CheckBox ID="column3CheckBox" runat="server"  Visible="false" OnCheckedChanged="CheckBoxChanged"/>
                    <asp:Label ID="column3Label" runat="server" Visible="false" />
                </td>
                </asp:PlaceHolder>
                <td class="buttons">
                    <asp:Button ID="AddLink" runat="server" OnPreRender="GetText" OnClick="AddToBasket" CssClass="button" />
                    <asp:Button ID="DeleteLink" runat="server" OnPreRender="GetText" OnClick="DeleteTemplate" CssClass="button" Visible='<%# ShowOptions %>'/>
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr>
                <td class="column1">
                    <asp:HyperLink ID="column1HyperLink" runat="server" Visible="false" />
                    <asp:Label ID="column1Label" runat="server" Visible="false" />
                </td>
                <td class="column2">
                    <asp:HyperLink ID="column2HyperLink" runat="server" Visible="false" />
                    <asp:Label ID="column2Label" runat="server" Visible="false" />
                </td>
                <asp:PlaceHolder ID="plhDefaultOption" runat="server" Visible='<%# ShowOptions %>'>
                <td class="column3">
                    <asp:CheckBox ID="column3CheckBox" runat="server"  Visible="false" OnCheckedChanged="CheckBoxChanged"/>
                    <asp:Label ID="column3Label" runat="server" Visible="false" />
                </td>
                </asp:PlaceHolder>
                <td class="buttons">
                    <asp:Button ID="AddLink" runat="server" OnPreRender="GetText" OnClick="AddToBasket" CssClass="button" />
                    <asp:Button ID="DeleteLink" runat="server" OnPreRender="GetText" OnClick="DeleteTemplate" CssClass="button" Visible='<%# ShowOptions %>' />
                </td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
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
</asp:PlaceHolder>