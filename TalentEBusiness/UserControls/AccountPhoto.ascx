<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AccountPhoto.ascx.vb"
    Inherits="UserControls_AccountPhoto" %>
    <asp:PlaceHolder ID="pHValidationSummary" runat="server">
            <div id="divValidationSummary" class="alert-box alert">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="UploadPhoto" />
        </div>
    </asp:PlaceHolder>


    <div class="panel ebiz-account-photo-wrap">
        <asp:Panel ID="pnlProfilePhoto" runat="server" CssClass="ebiz-profile-photo-wrap">
            <h2><asp:Literal ID="lblProfilePhoto" runat="server"></asp:Literal></h2>
            <asp:Image ID="imgProfilePhoto" runat="server" />
        </asp:Panel>
        <asp:Panel ID="pnlUpload" runat="server" CssClass="uploadphoto">
            <div class="alert-box info">
                <asp:Label ID="lblUploadInformation" runat="server" CssClass="ebiz-upload-instructions"></asp:Label>
            </div>
            <div class="row ebiz-upload-control-wrap">
                <div class="medium-3 columns">
                    <asp:Label ID="lblUpload" AssociatedControlID="Upload" runat="server"></asp:Label>
                </div>
                <div class="medium-9 columns">
                    <asp:FileUpload ID="Upload" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" ControlToValidate="Upload"
                        OnLoad="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="UploadPhoto"
                        Display="Static" Enabled="true" CssClass="error"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revUpload" ControlToValidate="Upload" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="UploadPhoto"
                        Display="Static" Enabled="true" CssClass="error"></asp:RegularExpressionValidator>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlCrop" runat="server" Visible="false">
            <asp:Image ID="imgCrop" runat="server" />
            <asp:HiddenField ID="X" runat="server" />
            <asp:HiddenField ID="Y" runat="server" />
            <asp:HiddenField ID="W" runat="server" />
            <asp:HiddenField ID="H" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlTandCs" runat="server" CssClass="ebiz-upload-TandCs-wrap">
            <asp:CheckBox ID="chkTermsAndCond" runat="server" />
            <asp:CustomValidator ID="cvTermsAndCond" ControlToValidate="Upload" runat="server"
                SetFocusOnError="true" Visible="true" ValidationGroup="UploadPhoto" Display="Static"
                Enabled="true" ClientValidationFunction="isTermsAndCondAccepted" CssClass="error"></asp:CustomValidator>
        </asp:Panel>

        <asp:Panel ID="pnlUploadButton" runat="server" CssClass="ebiz-upload-button-wrap">
            <asp:Button ID="btnUpload" runat="server" ValidationGroup="UploadPhoto" Text="Upload"
                CssClass="button ebiz-primary-action ebiz-upload" />
        </asp:Panel>

        <asp:Panel ID="pnlCropButton" runat="server" Visible="false" CssClass="ebiz-crop-button-wrap">
            <asp:Button ID="btnCrop" runat="server" CssClass="button ebiz-primary-action ebiz-crop" />
        </asp:Panel>

    </div>