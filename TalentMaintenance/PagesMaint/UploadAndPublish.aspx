<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="UploadAndPublish.aspx.vb" Inherits="PagesMaint_UploadAndPublish" MasterPageFile="~/MasterPages/MasterPage.master"  EnableEventValidation="True" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>



<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">

    <p class="title"><asp:Label ID="titleLabel" runat="server" Text="Upload and Publish Content" /></p>
    <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" Text="Use this page to upload and then publish web site content and configuration." /></p>

    <div id="publish" class="upload-and-publish-content ajax-accordion">

        <asp:Repeater ID="rptType" runat="server">
            <ItemTemplate>

                <asp:panel id="pnlImageTypeWrapper" runat="server" Visible="false">
                    <ajaxToolkit:CollapsiblePanelExtender 
                        ID="ajaxImageType" 
                        runat="server"
                        TargetControlID="pnlImageType" 
                        ExpandControlID="pnlImageTypeTitle" 
                        CollapseControlID="pnlImageTypeTitle"
                        TextLabelID="lblImageTypeTitle"
                        CollapsedText="Show Image Upload and Publish" 
                        ExpandedText="Hide Image Upload and Publish"
                        Collapsed="False" 
                        SuppressPostBack="true">
                    </ajaxToolkit:CollapsiblePanelExtender>
                    <asp:Panel ID="pnlImageTypeTitle" runat="server" CssClass="ajax-accordion-header" Visible="true">
                        <asp:Label ID="lblImageTypeTitle" runat="server" Visible="true" text="Image Type" ></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="pnlImageType" runat="server" CssClass="ajax-accordion-content">
                        <fieldset>
                            <legend>Image Type</legend>
                            <div class="error">
                                <asp:BulletedList ID="blImageErrList" runat="server"></asp:BulletedList>
                            </div>
                            <p class="instructions"><asp:Label ID="lblImageTypeInstructions" runat="server" Text="Use the Choose File button to select a file to be uploaded. The Upload Image button will transfer the file to the web server ready for use.  The Publish button will copy all the latest content to all production web servers."></asp:Label></p>
                            <ul>            
                                <li class="images-upload button"><asp:Button ID="ImageUploadLink" runat="server" CssClass="edit" Visible="true" Text="Upload Image" /></li>
                                <li class="images-choose-file button"><asp:FileUpload ID="ImageUpload1" runat="server"  Visible="true" /></li>
                                <li class="images-overwrite checkbox"><asp:CheckBox ID="ImageOverwriteCheck" runat="server" Text="Overwrite?" /></li>
                                <li class="images-publish button"><asp:Button ID="btnFtpImages" runat="server" Text="Publish Images" /><asp:Label ID="imagesDate" runat="server" /></li>
                                <li class="images-process-child-directories checkbox"><asp:CheckBox ID="ImageProcessChildDirectories" runat="server" Text="Process Child Directories?" /></li>
                            </ul>
                            <asp:HiddenField ID="hidImagesName" runat="server" />
                        </fieldset>
                    </asp:panel>
                </asp:panel>

                <asp:panel id="pnlFileTypeWrapper" runat="server" Visible="false">
                    <ajaxToolkit:CollapsiblePanelExtender 
                        ID="ajaxFileType" 
                        runat="server"
                        TargetControlID="pnlFileType" 
                        ExpandControlID="pnlFileTypeTitle" 
                        CollapseControlID="pnlFileTypeTitle"
                        TextLabelID="lblFileTypeTitle"
                        CollapsedText="Show File Upload and Publish"
                        ExpandedText="Hide File Upload and Publish"
                        Collapsed="False" 
                        SuppressPostBack="true">
                    </ajaxToolkit:CollapsiblePanelExtender>
                    <asp:Panel ID="pnlFileTypeTitle" runat="server" CssClass="ajax-accordion-header" Visible="true">
                        <asp:Label ID="lblFileTypeTitle" runat="server" Visible="true" text="File Type" ></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="pnlFileType" runat="server" CssClass="ajax-accordion-content">
                        <fieldset>
                            <legend>File Type</legend>
                            <div class="error">
                                <asp:BulletedList ID="blFileErrList" runat="server"></asp:BulletedList>
                            </div>
                            <p class="instructions"><asp:Label ID="lblFileTypeInstructions" runat="server" Text="Use the Choose File button to select a file to be uploaded.  The Upload Image button will transfer the file to teh web server ready for use.  The Publish button will copy all the latest content to all production web servers."></asp:Label></p>
                            <ul>            
                                <li class="files-upload button"><asp:Button ID="FileUploadLink" runat="server" CssClass="edit" Visible="true" Text="Upload Html File" /></li>
                                <li class="files-choose-file button"><asp:FileUpload ID="FileUpload1" runat="server"  Visible="true" /></li>
                                <li class="files-overwrite checkbox"><asp:CheckBox ID="FileOverwriteCheck" runat="server" Text="Overwrite?" /></li>
                                <li class="files-publish button"><asp:Button ID="btnFtpHTML" runat="server" Text="Publish HTML" /><asp:Label ID="fileDate" runat="server" /></li>
                                <li class="files-process-child-directories checkbox"><asp:CheckBox ID="FileProcessChildDirectories" runat="server" Text="Process Child Directories?" /></li>
                           </ul>
                            <asp:HiddenField ID="hidFilesName" runat="server" />
                        </fieldset>
                    </asp:panel>
                </asp:panel>

                <asp:panel id="pnlCMSTypeWrapper" runat="server" Visible="false">
                    <ajaxToolkit:CollapsiblePanelExtender 
                        ID="ajaxCMSType" 
                        runat="server"
                        TargetControlID="pnlCMSType" 
                        ExpandControlID="pnlCMSTypeTitle" 
                        CollapseControlID="pnlCMSTypeTitle"
                        TextLabelID="lblCMSTypeTitle"
                        CollapsedText="Show CMS Publish"
                        ExpandedText="Hide CMS Publish"
                        Collapsed="False" 
                        SuppressPostBack="true">
                    </ajaxToolkit:CollapsiblePanelExtender>
                    <asp:Panel ID="pnlCMSTypeTitle" runat="server" CssClass="ajax-accordion-header" Visible="true">
                        <asp:Label ID="lblCMSTypeTitle" runat="server" Visible="true" text="CMS Type" ></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="pnlCMSType" runat="server" CssClass="ajax-accordion-content">
                        <fieldset>
                            <legend>CMS Type</legend>
                            <div class="error">
                                <asp:BulletedList ID="blCMSErrList" runat="server"></asp:BulletedList>
                            </div>
                            <p class="instructions"><asp:Label ID="lblCMSTypeInstructions" runat="server" Text="Use the Publish button to ensure all latest CMS changes are replicated across all production web servers. Select any or all of the checkboxes to replicate specific areas of CMS configuration. The Clear Cache button can be used to force production web servers to use newly published content/configuration without the need to restart each web server."></asp:Label></p>
                            <ul>            
                                <asp:Placeholder runat="server" id="plhChkSystemDefaults" visible="false"><li class="cms-system-defaults checkbox"><asp:CheckBox ID="chkSystemDefaults" runat="server" Text="System Defaults" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkRetailProducts" visible="false"><li class="cms-retail-products checkbox"><asp:CheckBox ID="chkRetailProducts" runat="server" Text="Retail Products" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkRetailNavigation" visible="false"><li class="cms-retail-navigation checkbox"><asp:CheckBox ID="chkRetailNavigation" runat="server" Text="Retail Navigation" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkProductRelations" visible="false"><li class="cms-product-relations checkbox"><asp:CheckBox ID="chkProductRelations" runat="server" Text="Product Relations" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkProductPersonalisation" visible="false"><li class="cms-product-personalisation checkbox"><asp:CheckBox ID="chkProductPersonalisation" runat="server" Text="Product Personalisation" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkPromotions" visible="false"><li class="cms-promotions checkbox"><asp:CheckBox ID="chkPromotions" runat="server" Text="Promotions" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkPageContent" visible="false"><li class="cms-page-content checkbox"><asp:CheckBox ID="chkPageContent" runat="server" Text="Page Content" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkControlContent" visible="false"><li class="cms-control-content checkbox"><asp:CheckBox ID="chkControlContent" runat="server" Text="Control Content" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkFlashContent" visible="false"><li class="cms-flash-content checkbox"><asp:CheckBox ID="chkFlashContent" runat="server" Text="Flash Settings" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkTicketingMenus" visible="false"><li class="cms-ticketing-menus checkbox"><asp:CheckBox ID="chkTicketingMenus" runat="server" Text="Ticketing Menus" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkPageTracking" visible="false"><li class="cms-page-tracking checkbox"><asp:CheckBox ID="chkPageTracking" runat="server" Text="Page Tracking Settings" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkEmailTemplates" visible="false"><li class="cms-email_templates checkbox"><asp:CheckBox ID="chkEmailTemplates" runat="server" Text="Email Templates" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhChkAlerts" visible="false"><li class="cms-alerts checkbox"><asp:CheckBox ID="chkAlerts" runat="server" Text="Alerts" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhVouchers" visible="false"><li class="cms-vouchers checkbox"><asp:CheckBox ID="chkVouchers" runat="server" Text="Vouchers" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhQueryStrings" visible="false"><li class="cms-querystrings checkbox"><asp:CheckBox ID="chkQueryStrings" runat="server" Text="Query Strings" /></li></asp:Placeholder>
                                <asp:Placeholder runat="server" id="plhActivities" visible="false"><li class="cms-activities checkbox"><asp:CheckBox ID="chkActivities" runat="server" Text="Activities/Transactional Related Question" /></li></asp:Placeholder>
                                <li class="cms-publish button"><asp:Button ID="btnUploadCMS" runat="server" Text="Publish CMS" /><asp:Label ID="cmsDate" runat="server" /></li>
                                <li class="cms-clear-cache button"><asp:Button ID="btnClearCacheCMS" runat="server" Text="Clear Cache" /></li>
                            </ul>
                            <asp:HiddenField ID="hidCMSName" runat="server" />
                        </fieldset>
                    </asp:panel>
                </asp:panel>

            </ItemTemplate>        
        </asp:Repeater>
    </div>
</asp:Content>
