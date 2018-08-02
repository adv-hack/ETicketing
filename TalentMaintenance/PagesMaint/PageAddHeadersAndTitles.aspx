<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    CodeFile="PageAddHeadersAndTitles.aspx.vb" Inherits="PagesMaint_PageAddHeadersAndTitles" Title="Add Page Header And Title" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
<div id="pageAddHeadersAndTitles1">
    <p class="titles"><asp:Label ID="TitleLabel" runat="server" Text="Add Headers And Titles" /></p>
    <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" Text="Label" /></p>
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <p class="error"><asp:Label ID="ErrLabel" runat="server" /></p>
    </asp:PlaceHolder>

    <div class="pageAddHeadersAndTitles2">
        <table cellspacing="0" class="defaultTable">
            <tbody>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="businessunitLabel" runat="server" Text="Business Unit" /></th>
                    <td class="element"><asp:Label ID="businessunit" runat="server" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="partnerLabel" runat="server" Text="Partner" /></th>
                    <td class="element"><asp:Label ID="partner" runat="server" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="PageNameLabel" runat="server" Text="Page Name" /></th>
                    <td class="element"><asp:Label ID="PageName" runat="server" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="DescriptionLabel" runat="server" Text="Description" /></th>
                    <td class="element"><asp:Label ID="Description" runat="server" /></td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="pageAddHeadersAndTitles3">
        <table cellspacing="0" class="defaultTable">
            <tbody>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="LanguageLabel" runat="server" Text="Language" /></th>
                    <td class="element">
                        <asp:DropDownList ID="DropDownLanguage" runat="server" class="select" DataSourceID="SearchDataSource1" DataTextField="LANGUAGE_DESCRIPTION" DataValueField="LANGUAGE_CODE" />
                        <asp:ObjectDataSource ID="SearchDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="GetLanguageNotInData" TypeName="PageAddHeadersAndTitlesDatasetTableAdapters.tbl_languageTableAdapter"
                            DeleteMethod="Delete" InsertMethod="Insert" UpdateMethod="Update">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                                <asp:QueryStringParameter DefaultValue="" Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                                <asp:QueryStringParameter DefaultValue="" Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                            </SelectParameters>
                            <DeleteParameters>
                                <asp:Parameter Name="Original_LANGUAGE_CODE" Type="String" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="LANGUAGE_CODE" Type="String" />
                                <asp:Parameter Name="LANGUAGE_DESCRIPTION" Type="String" />
                                <asp:Parameter Name="Original_LANGUAGE_CODE" Type="String" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Name="LANGUAGE_CODE" Type="String" />
                                <asp:Parameter Name="LANGUAGE_DESCRIPTION" Type="String" />
                            </InsertParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="PageHeaderLabel" runat="server" Text="Page Header" /></th>
                    <td class="element"><asp:TextBox ID="TextPageHeader" runat="server" class="input-l" MaxLength="255" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="PageTitleLabel" runat="server" Text="Page Title" /></th>
                    <td class="element"><asp:TextBox ID="TextPageTitle" runat="server" class="input-l" MaxLength="500" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="MetaDescriptionLabel" runat="server" Text="Meta Description" /></th>
                    <td class="element"><asp:TextBox ID="TextMetaDescription" runat="server" class="textarea" Rows="6" TextMode="MultiLine" /></td>
                </tr>
                <tr>
                    <th class="label" scope="row"><asp:Label ID="MetaKeywordsLabel" runat="server" Text="Meta Keywords" /></th>
                    <td class="element"><asp:TextBox ID="TextMetaKeywords" runat="server" class="textarea" Rows="6" TextMode="MultiLine" /></td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="pageAddHeadersAndTitlesButtons" >
        <asp:Button ID="ConfirmButton" runat="server" Text="Confirm" class="button" />
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" class="button" />
    </div>
</div>
</asp:Content>
