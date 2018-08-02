<%@ Page Language="VB"  MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ProductRelationships.aspx.vb" 
    Inherits="Products_ProductRelationships" EnableEventValidation="false" ValidateRequest="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <asp:ScriptManager ID="scmMainScriptManager" runat="server" />
    <div id="product-relationships-wrapper">
        <div class="product-relationships-detail">
            <p class="title"><asp:Label ID="pagetitleLabel" runat="server"></asp:Label></p>
            <p class="instructions"><asp:Label ID="pageinstructionsLabel" runat="server"></asp:Label></p>
        </div>

        <div class="product-relationships-refresh">
            <asp:Button ID="btnRefreshProductList" runat="server" />
        </div>

        <asp:PlaceHolder ID="plhProductSearch" runat="server" Visible="false">
        <fieldset>
        <div class="product-relationships-search">
            <ul>
                <li class="search-option1">
                    <asp:Label ID="lblSearchCode" runat="server" AssociatedControlID="txtSearchCode" />
                    <asp:TextBox ID="txtSearchCode" runat="server" />
                    <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceCodeSearch" TargetControlID="txtSearchCode" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                        ServiceMethod="GetProductCodeList" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true">
                    </ajaxToolkit:AutoCompleteExtender>
                </li>
                <li class="search-option2">
                    <asp:Label ID="lblSearchDescription" runat="server" AssociatedControlID="txtSearchDescription" />
                    <asp:TextBox ID="txtSearchDescription" runat="server" />
                    <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceSearch" TargetControlID="txtSearchDescription" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                        ServiceMethod="GetProductDescList" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true">
                    </ajaxToolkit:AutoCompleteExtender>
                </li>
                <li class="search-option3">
                    <asp:Label ID="lblSearchTicketingProductType" runat="server" AssociatedControlID="ddlSearchTicketingProductType" />
                    <asp:DropDownList ID="ddlSearchTicketingProductType" runat="server" />
                </li>
                <li class="search-option4">
                    <asp:Label ID="lblLinkTypeSearch" runat="server" AssociatedControlID="ddlLinkTypeSearch" />
                    <asp:DropDownList ID="ddlLinkTypeSearch" runat="server" />
                </li>
                <li class="submit-search">
                    <asp:Button ID="btnSearchOptions" runat="server" />
                </li>
            </ul>
        </div>
        </fieldset>
        </asp:PlaceHolder>

        <div class="product-relationships-add">
            <asp:Button ID="btnAddNewRelationShip" runat="server" />
        </div>

        <asp:PlaceHolder ID="plhNoProductRelationships" runat="server" Visible="false">
            <p class="error"><asp:Label ID="lblNoProductRelationships" runat="server" /></p>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhRelationshipDeleted" runat="server" Visible="false">
            <p class="error"><asp:Label ID="lblRelationshipDeleted" runat="server" /></p>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhProductRelationships" runat="server">
        <div class="product-relationships-count">
            <span><asp:Literal ID="ltlProductRelationshipsCount" runat="server" /></span>
        </div>
        <div class="product-relationships-list">
            <asp:Repeater ID="rptProductRelationships" runat="server">
                <HeaderTemplate>
                    <table cellpadding="0" class="defaultTable">
                        <tr class="header">
                            <th class="product" scope="col"><%# ProductColumnHeader%></th>
                            <th class="product-type" scope="col"><%#TypeSubTypeColumnHeader %></th>
                            <th class="product-date" scope="col"><%#DateColumnHeader%></th>
                            <th class="price-codes1" scope="col"><%#PriceCodesColumnHeader%></th>
                            <th class="linked-to" scope="col"><%#LinkedToColumnHeader %></th>
                            <th class="product-type" scope="col"><%#TypeSubTypeColumnHeader %></th>
                            <th class="price-codes2" scope="col"><%#PriceCodesColumnHeader%></th>
                            <%--<th class="product-link-type" scope="col"><%#ProductLinkType%></th>--%>
                            <th class="delete-link" scope="col"><%#DeleteColumnHeader %></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr class="odd">
                            <td class="product">
                                <a href="EditProductRelationships.aspx?id=<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString()%>&BU=<%=BusinessUnit %>&Partner=<%= Partner%>">
                                <%# DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString()%> <%# DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString()%></a>
                                <%# GetDescription(DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString(), DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString())%>
                            </td>
                            <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                            <td class="product-date"><%# DataBinder.Eval(Container.DataItem, "PRODUCT_DATE").ToString() %></td>
                            <td class="price-codes1"><%# DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_PRICE_CODE").ToString()%></td>
                            <td class="linked-to"><%# DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_CODE").ToString())%></td>
                            <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                            <td class="price-codes2"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString())%></td>
                            <td class="product-link-type"><%# DataBinder.Eval(Container.DataItem, "LINK_TYPE").ToString()%></td>
							<td class="delete-link">
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString()%>' CommandName="Edit" />
                                <asp:Button ID="btnDeleteLink" runat="server" Text='<%# DeleteButtonText %>' 
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString() %>' CommandName="Delete" />
                            </td>
                        </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                        <tr class="even">
                            <td class="product">
                                <a href="EditProductRelationships.aspx?id=<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString()%>&BU=<%=BusinessUnit %>&Partner=<%= Partner%>">
                                <%# DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString()%> <%# DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString()%></a>
                                <%# GetDescription(DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION").ToString(), DataBinder.Eval(Container.DataItem, "PRODUCT_CODE").ToString())%>
                            </td>
                            <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                            <td class="product-date"><%# DataBinder.Eval(Container.DataItem, "PRODUCT_DATE").ToString() %></td>
                            <td class="price-codes1"><%# DataBinder.Eval(Container.DataItem, "TICKETING_PRODUCT_PRICE_CODE").ToString()%></td>
                            <td class="linked-to"><%# DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString()%><%# GetDescription(DataBinder.Eval(Container.DataItem, "LINKED_TO").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_CODE").ToString())%></td>
                            <td class="product-type"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString())%></td>
                            <td class="price-codes2"><%# Format2Strings(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString(), DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString())%></td>
                            <td class="product-link-type"><%# DataBinder.Eval(Container.DataItem, "LINK_TYPE").ToString()%></td>
                            <td class="delete-link">
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString()%>' CommandName="Edit" />
                                <asp:Button ID="btnDeleteLink" runat="server" Text='<%# DeleteButtonText %>' 
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_RELATIONS_ID").ToString() %>' CommandName="Delete" />
                            </td>
                        </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>