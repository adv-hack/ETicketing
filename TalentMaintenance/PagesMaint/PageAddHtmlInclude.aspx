<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    CodeFile="PageAddHtmlInclude.aspx.vb" Inherits="PagesMaint_PageAddHtmlInclude" Title="Add HTML Include" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="page-add-html-include">
        <p class="title"><asp:Label ID="TitleLabel" runat="server" Text="Add HTML Include" /></p>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" /></p>
        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <p class="error"><asp:Label ID="ErrLabel" runat="server" /></p>
        </asp:PlaceHolder>

        <div class="page-add-html-include-wrapper">
            <table cellspacing="0" class="page-add-html-include-summary vertical">
                <tbody>
                    <tr class="business-unit">
                        <th scope="row"><asp:Label ID="businessunitLabel" runat="server" Text="Business Unit" /></th>
                        <td><asp:Label ID="businessunit" runat="server" /></td>
                    </tr>
                    <tr class="partner">
                        <th scope="row"><asp:Label ID="partnerLabel" runat="server" Text="Partner" /></th>
                        <td><asp:Label ID="partner" runat="server" /></td>
                    </tr>
                    <tr class="page-name">
                        <th scope="row"><asp:Label ID="PageNameLabel" runat="server" Text="Page Name" /></th>
                        <td><asp:Label ID="PageName" runat="server" /></td>
                    </tr>
                    <tr class="description">
                        <th scope="row"><asp:Label ID="DescriptionLabel" runat="server" Text="Description" /></th>
                        <td><asp:Label ID="Description" runat="server" /></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="page-add-html-data-wrapper">
            <table cellspacing="0" class="page-add-html-data vertical">
                <tbody>
                    <tr>
                        <th scope="row" class="section"><asp:Label ID="SectionLabel" runat="server" Text="Section" /></th>
                        <td class="section">
                            <asp:TextBox ID="txtSection" runat="server" CssClass="input-s" MaxLength="2" />
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="sequence"><asp:Label ID="SequenceLabel" runat="server" Text="Sequence" /></th>
                        <td class="sequence"><asp:TextBox ID="TextSequence" runat="server" CssClass="input-s" MaxLength="3" /></td>
                    </tr>
                    <tr>
                        <th scope="row" class="query"><asp:Label ID="QueryLabel" runat="server" Text="Query String" /></th>
                        <td class="query"><asp:TextBox ID="TextQuery" runat="server" CssClass="input-s input-readonly" MaxLength="200" ReadOnly="True" /></td>
                    </tr>
                    <tr>
                        <th scope="row" class="html1"><asp:Label ID="HTML1Label" runat="server" Text="HTML 1" /></th>
                        <td class="html1 ftbControl">
                            <asp:TextBox ID="TextHTML1" runat="server" CssClass="input-l" />
                            <asp:Button ID="btnEditHtml1" Enabled="false" runat="server" CssClass="button" CommandName='<%#Eval("PAGE_HTML_ID") %>' Text="Edit" />
                        </td>
                    </tr>
                    <tr id="trHTMLEditor1" runat="server" visible="false">
                        <td colspan="2">
                            <CKEditor:CKEditorControl ID="uscCKEditor1" runat="server" BasePath="~/ckeditor" />
                            <textarea id="FreeTextBox1" runat="server" name="elm1" rows="30" cols="120"></textarea>
                            <p>
                                <asp:Button ID="SaveButton" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton" runat="server" Text="Cancel" CssClass="button" />
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="html2"><asp:Label ID="HTML2Label" runat="server" Text="HTML 2" /></th>
                        <td class="html2 ftbControl">
                            <asp:TextBox ID="TextHTML2" runat="server" class="input-l"></asp:TextBox>
                            <asp:Button ID="btnEditHtml2" runat="server" CssClass="button" CommandName='<%#Eval("PAGE_HTML_ID") %>' Text="Edit"></asp:Button>
                        </td>
                    </tr>
                    <tr id="trHTMLEditor2" runat="server" visible="false">
                        <td colspan="2">
                            <CKEditor:CKEditorControl ID="uscCKEditor2" runat="server" BasePath="~/ckeditor" />
                            <p>
                                <asp:Button ID="SaveButton2" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton2" runat="server" Text="Cancel" CssClass="button" />
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="html3"><asp:Label ID="HTML3Label" runat="server" Text="HTML 3" /></th>
                        <td class="html3 ftbControl">
                            <asp:TextBox ID="TextHTML3" runat="server" CssClass="input-l"></asp:TextBox>
                            <asp:Button ID="btnEditHtml3" runat="server" CssClass="button" CommandName='<%#Eval("PAGE_HTML_ID") %>' Text="Edit"></asp:Button>
                        </td>
                    </tr>
                    <tr id="trHTMLEditor3" runat="server" visible="false">
                        <td colspan="2">
                            <CKEditor:CKEditorControl ID="uscCKEditor3" runat="server" BasePath="~/ckeditor" />
                            <p>
                                <asp:Button ID="SaveButton3" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton3" runat="server" Text="Cancel" CssClass="button" />
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="html-include-name"><asp:Label ID="HTMLIncNameLabel" runat="server" Text="HTML Include Name" /></th>
                        <td class="html-include-name ftbControl">
                            <asp:DropDownList ID="ddlAvailableUploadedFiles" runat="server" Width="500" />
                            <asp:TextBox ID="TextHTMLIncName" runat="server" CssClass="input-l" Visible="False"></asp:TextBox>
                            <asp:Button ID="btnEditHtmlInclude" runat="server" CssClass="button" CommandName='<%#Eval("PAGE_HTML_ID") %>' Text="Edit" />
                        </td>
                    </tr>
                    <tr id="trHTMLIncludeEditor" runat="server" visible="false">
                        <td colspan="2">
                            <CKEditor:CKEditorControl ID="uscCKEditor4" runat="server" BasePath="~/ckeditor" />
                            <p>
                                <asp:Button ID="SaveButton4" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton4" runat="server" Text="Cancel" CssClass="button" />
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="file-upload"><asp:Label ID="FileUploadLabel" runat="server" Text="Upload Html" Visible="true" /></th>
                        <td class="file-upload">
                            <div>
                                <asp:FileUpload ID="FileUpload1" runat="server"  Visible="true" />
                                <asp:CheckBox ID="HtmlOverwriteCheck" runat="server" Text="Overwrite File if exists?" /><br />
                                <asp:Button ID="FileUploadLink" runat="server" CssClass="edit" Visible="true" Text="Upload Html File" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row" class="image-upload"><asp:Label ID="ImageUploadLabel" runat="server" Text="Upload Image" Visible="true" /></th>
                        <td class="image-upload">
                            <div>
                                <asp:FileUpload ID="ImageUpload1" runat="server"  Visible="true" />
                                <asp:CheckBox ID="ImageOverwriteCheck" runat="server" Text="Overwrite Image if exists?" /><br />
                                <asp:Button ID="ImageUploadLink" runat="server" CssClass="edit" Visible="true" Text="Upload Image" />
                                <p><asp:Literal ID="ltlImagePath" runat="server" /></p>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="page-add-html-include-buttons button-bar">
            <asp:Button ID="ConfirmButton" runat="server" Text="Confirm" CssClass="confirm button" />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" CssClass="cancel button" />
        </div>
    </div>
</asp:Content>
