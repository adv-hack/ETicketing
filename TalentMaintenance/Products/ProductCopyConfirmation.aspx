<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ProductCopyConfirmation.aspx.vb" Inherits="Products_ProductCopyConfirmation" %>
<%@ Register Src="../UserControls/ProductMaintenanceTopNavigation.ascx" TagName="ProductMaintenanceTopNavigation"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:ProductMaintenanceTopNavigation ID="ProductMaintenanceTopNavigation1" runat="server" />
    <p class="title">
        <asp:Label ID="titleLabel" runat="server" />
    </p>

    <div id="pageAddHtmlInclude1">
        <p class="error">
            <asp:Label ID="ErrLabel" runat="server" Text="" CssClass="error"></asp:Label>
        </p>
        <div class="pageAddHtmlInclude2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="productLabel" runat="server" Text="Product Code" /></th>
                        <td class="element">
                            <asp:Label ID="product" runat="server" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="pageAddHtmlInclude3">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr runat="server" id="trD1" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="description1Label" runat="server" Text="Product Description #1" /></th>
                        <td class="element">
                            <asp:Label ID="textDescription1" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trD2" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="description2Label" runat="server" Text="Product Description #2" /></th>
                        <td class="element">
                            <asp:Label ID="textDescription2" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trD3" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="description3Label" runat="server" Text="Product Description #3" /></th>
                        <td class="element">
                            <asp:Label ID="textDescription3" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trD4" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="description4Label" runat="server" Text="Product Description #4" /></th>
                        <td class="element">
                            <asp:Label ID="textDescription4" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trD5" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="description5Label" runat="server" Text="Product Description #5" /></th>
                        <td class="element">
                            <asp:Label ID="textDescription5" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trH1" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="HTML1Label" runat="server" Text="Product HTML #1" /></th>
                        <td class="element">
                            <asp:Label ID="textHTML1" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trH2" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="HTML2Label" runat="server" Text="Product HTML #2" /></th>
                        <td class="element">
                            <asp:Label ID="textHTML2" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr runat="server" id="trH3" visible="false">
                        <th class="label" scope="row">
                            <asp:Label ID="HTML3Label" runat="server" Text="Product HTML #3" /></th>
                        <td class="element">
                            <asp:Label ID="textHTML3" runat="server" Width="650" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="pageAddHtmlInclude2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="labelProductsToCopyTo" runat="server" Text="Products to copy to:" /></th>
                        <td class="label">
                            <asp:Label ID="labelProductsToCopy" runat="server"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="pageAddHtmlIncludeButtons">
            <asp:Button ID="ConfirmButton" runat="server" Text="Confirm" CssClass="button" />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False"
                CssClass="button" />
            <asp:HyperLink ID="hypProductList" runat="server" Visible ="false">Product List</asp:HyperLink>
        </div>
    </div>
</asp:Content>

