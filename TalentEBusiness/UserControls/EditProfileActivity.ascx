<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EditProfileActivity.ascx.vb" Inherits="UserControls_EditProfileActivity" %>

<script type="text/template" id="qq-template">
    <div class="qq-uploader-selector qq-uploader qq-gallery" qq-drop-area-text="Drop files here">
        <div class="qq-total-progress-bar-container-selector qq-total-progress-bar-container">
            <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-total-progress-bar-selector qq-progress-bar qq-total-progress-bar"></div>
        </div>
        <div class="qq-upload-drop-area-selector qq-upload-drop-area" qq-hide-dropzone>
            <span class="qq-upload-drop-area-text-selector">Drop files here to upload</span>
        </div>
        <div class="qq-upload-button-selector button">
            <div>Upload a file</div>
        </div>
        <span class="qq-drop-processing-selector qq-drop-processing">
            <span>Processing dropped files...</span>
            <span class="qq-drop-processing-spinner-selector qq-drop-processing-spinner"></span>
        </span>
        <ul class="qq-upload-list-selector qq-upload-list" role="region" aria-live="polite" aria-relevant="additions removals">
            <li>
                <span role="status" class="qq-upload-status-text-selector qq-upload-status-text"></span>
                <div class="qq-progress-bar-container-selector qq-progress-bar-container">
                    <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-progress-bar-selector qq-progress-bar"></div>
                </div>
                <span class="qq-upload-spinner-selector qq-upload-spinner"></span>
                <div class="qq-thumbnail-wrapper">
                    <img class="qq-thumbnail-selector" qq-max-size="120" qq-server-scale>
                </div>
                <button type="button" class="qq-upload-cancel-selector qq-upload-cancel"><i class="fa fa-times" aria-hidden="true"></i></button>
                <button type="button" class="qq-upload-retry-selector qq-upload-retry">
                    <span class="qq-btn qq-retry-icon" aria-label="Retry"></span>
                    Retry
                </button>

                <div class="qq-file-info">
                    <div class="qq-file-name">
                        <span class="qq-upload-file-selector qq-upload-file"></span>
                        <span class="qq-edit-filename-icon-selector qq-btn qq-edit-filename-icon" aria-label="Edit filename"></span>
                    </div>
                    <input class="qq-edit-filename-selector qq-edit-filename" tabindex="0" type="text">
                    <span class="qq-upload-size-selector qq-upload-size"></span>
                    <button type="button" class="qq-btn qq-upload-delete-selector qq-upload-delete">
                        <span class="qq-btn qq-delete-icon" aria-label="Delete"></span>
                    </button>
                    <button type="button" class="qq-btn qq-upload-pause-selector qq-upload-pause">
                        <span class="qq-btn qq-pause-icon" aria-label="Pause"></span>
                    </button>
                    <button type="button" class="qq-btn qq-upload-continue-selector qq-upload-continue">
                        <span class="qq-btn qq-continue-icon" aria-label="Continue"></span>
                    </button>
                </div>
            </li>
        </ul>

        <dialog class="qq-alert-dialog-selector">
            <div class="qq-dialog-message-selector"></div>
            <div class="qq-dialog-buttons">
                <button type="button" class="qq-cancel-button-selector">Close</button>
            </div>
        </dialog>

        <dialog class="qq-confirm-dialog-selector">
            <div class="qq-dialog-message-selector"></div>
            <div class="qq-dialog-buttons">
                <button type="button" class="qq-cancel-button-selector">No</button>
                <button type="button" class="qq-ok-button-selector">Yes</button>
            </div>
        </dialog>

        <dialog class="qq-prompt-dialog-selector">
            <div class="qq-dialog-message-selector"></div>
            <input type="text">
            <div class="qq-dialog-buttons">
                <button type="button" class="qq-cancel-button-selector">Cancel</button>
                <button type="button" class="qq-ok-button-selector">Ok</button>
            </div>
        </dialog>
    </div>

</script>

<asp:ValidationSummary ID="vlsAdditionalInfoErrors" runat="server" ShowSummary="true" EnableClientScript="true" DisplayMode="BulletList" CssClass="alert-box alert" ValidationGroup="Activities" />
<div class="alert-box alert" id="clientside-errors-wrapper" style="display: none;">
    <ul id="clientside-errors"></ul>
</div>
<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
    <div class="alert-box alert">
        <asp:BulletedList ID="blErrorMessages" runat="server" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhSuccessList" runat="server" Visible="false">
    <div class="alert-box success">
        <asp:BulletedList ID="blSuccessMessages" runat="server" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhCustomerPromptMessage" runat="server" Visible="False">
        <div class="panel activity-user-prompt">
            <asp:Literal ID="ltlCustomerPromptMessage" runat="server" />
        </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhActivityHeader" runat="server" Visible="true">
    <div class="panel ebiz-activities-edit-filter-wrap">
        <div class="row">
            <div class="column ebiz-activities-edit-filter-item ebiz-activity-wrap">
                <div class="ebiz-activity">
                    <asp:Label ID="lblActivity" runat="server" />
                </div>
                <div class="ebiz-activity-value">
                    <asp:Label ID="lblActivityValue" runat="server" />
                </div>
            </div>
            <asp:PlaceHolder ID="plhActivityDate" runat="server">
                <div class="column ebiz-activities-edit-filter-item ebiz-activity-date">
                    <asp:Label ID="lblActivityDate" runat="server" AssociatedControlID="txtActivityDate" />
                    <asp:TextBox ID="txtActivityDate" runat="server" CssClass="datepicker" />
                    <asp:RegularExpressionValidator ID="revActivityDate" runat="server" ControlToValidate="txtActivityDate" CssClass="error" ValidationGroup="Activities"  OnLoad="SetupRegExValidator" Display="Static"></asp:RegularExpressionValidator>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhActivitySubject" runat="server">
                <div class="column ebiz-activities-edit-filter-item ebiz-activity-subject">
                    <asp:Label ID="lblActivitySubject" runat="server" AssociatedControlID="txtActivitySubject" />
                    <asp:TextBox ID="txtActivitySubject" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvActivitySubject" runat="server" ControlToValidate="txtActivitySubject" ValidationGroup="Activities"
                        SetFocusOnError="true" OnLoad="SetupRequiredValidator" Visible="true" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhActivityStatus" runat="server">
                <div class="column ebiz-activities-edit-filter-item ebiz-activity-status">
                    <asp:Label ID="lblActivityStatus" runat="server" AssociatedControlID="ddlActivityStatus" />
                    <asp:DropDownList ID="ddlActivityStatus" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAcivityUser" runat="server">
                <div class="column ebiz-activities-edit-filter-item ebiz-activity-user">
                    <asp:Label ID="lblUserDropdown" runat="server" AssociatedControlID="ddlUser" />
                    <asp:DropDownList ID="ddlUser" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhTemplateQuestions" runat="server" Visible="true">
<asp:Repeater ID="rptActivityQuestions" runat="server" Visible="true" OnItemDataBound="rptActivityQuestions_ItemDataBound">
    <HeaderTemplate>
        <div class="panel ebiz-new-activity-wrap">
    </HeaderTemplate>
    <ItemTemplate>
        <asp:PlaceHolder ID="plhFreeTextField" runat="server" Visible="False" ViewStateMode="Enabled">
            <div class="row ebiz-question-text">
                <div class="medium-3 columns">
                    <asp:Label runat="server" ID="lblQuestionText" AssociatedControlID="txtQuestionText" />
                </div>
                <div class="medium-6 columns">
                    <asp:TextBox runat="server" ID="txtQuestionText" TextMode="MultiLine" />
                    <asp:RequiredFieldValidator ID="rfvQuestionText" runat="server" ControlToValidate="txtQuestionText" CssClass="error" Display="Static" Enabled="false" SetFocusOnError="true" ValidationGroup="Activities" />
                    <asp:RegularExpressionValidator ID="revQuestionText" runat="server" ControlToValidate="txtQuestionText" CssClass="error" Display="Static" Enabled="false" SetFocusOnError="true" ValidationGroup="Activities" />
                </div>
                <div class="medium-3 columns">
                    <asp:HyperLink runat="server" ID="hplFreeTextFieldExternalLink" Visible="false" Target="_blank" CssClass="button" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhCheckbox" runat="server" Visible="False" ViewStateMode="Enabled">
            <div class="row ebiz-question-check-box">
                <div class="medium-3 columns">
                    <asp:Label runat="server" ID="lblQuestionCheckText" AssociatedControlID="chkQuestionCheck" />
                </div>
                <div class="medium-6 columns">
                    <asp:CheckBox runat="server" ID="chkQuestionCheck" />
                    <asp:CustomValidator ID="csvQuestionCheck" runat="server" CssClass="error" Display="Static" Enabled="false"
                        OnServerValidate="ValidateCheckboxAnswer" ClientValidationFunction="ValidateCheckboxAnswer" EnableClientScript="true" SetFocusOnError="true" ValidationGroup="Activities" />
                </div>
                <div class="medium-3 columns">
                    <asp:HyperLink runat="server" ID="hplCheckBoxExternalLink" Visible="false" Target="_blank" CssClass="button" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhDate" runat="server" Visible="False" ViewStateMode="Enabled">
            <div class="row ebiz-question-date">
                <div class="medium-3 columns">
                    <asp:Label runat="server" ID="lblDate" AssociatedControlID="txtDate" />
                </div>
                <div class="medium-6 columns">
                    <asp:TextBox runat="server" ID="txtDate" CssClass="datepicker" />
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" CssClass="error" Display="Static" Enabled="false" SetFocusOnError="true" ValidationGroup="Activities" />
                    <asp:RegularExpressionValidator ID="revDate" runat="server" ControlToValidate="txtDate" CssClass="error" ValidationGroup="Activities" Display="Static"></asp:RegularExpressionValidator>
                </div>
                <div class="medium-3 columns">
                    <asp:HyperLink runat="server" ID="hplDateExternalLink" Visible="false" Target="_blank" CssClass="button" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhListOfAnswers" runat="server" Visible="False" ViewStateMode="Enabled">
            <div class="row ebiz-question-select">
                <div class="medium-3 columns">
                    <asp:Label runat="server" ID="lblListOfAnswers" AssociatedControlID="ddlAnswers" />
                </div>
                <div class="medium-6 columns">
                    <asp:DropDownList ID="ddlAnswers" runat="server" />
                </div>
                <div class="medium-3 columns">
                    <asp:HyperLink runat="server" ID="hplListOfAnswersExternalLink" Visible="false" Target="_blank" CssClass="button" />
                </div>
            </div>
            <div class="row ebiz-list-of-answers" id="specifyanswer" runat="server">
                <div class="medium-3 columns">
                    <asp:Label runat="server" ID="lblSpecify" ClientIDMode="Static" AssociatedControlID="txtSpecify" />
                </div>
                <div class="medium-6 medium-pull-3 columns">
                    <asp:TextBox runat="server" ID="txtSpecify" TextMode="MultiLine" />
                    <asp:RegularExpressionValidator ID="revSpecify" runat="server" ControlToValidate="txtSpecify" CssClass="error" Display="Static" Enabled="false" SetFocusOnError="true" ValidationGroup="Activities" />
                    <asp:CustomValidator ID="csvSpecify" runat="server" CssClass="error" Display="Static" Enabled="false"
                        OnServerValidate="ValidateListAnswer" ClientValidationFunction="ValidateListAnswer" EnableClientScript="true" SetFocusOnError="true" ValidationGroup="Activities" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:HiddenField runat="server" ID="answerType" />
        <asp:HiddenField runat="server" ID="hdfQuestionID" Value='<%# Container.DataItem("QUESTION_ID")%>' />
    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhActivityFileAttachments" runat="server" Visible="true">
    <div class="panel ebiz-activity-file-attachments">
        <h2>
            <asp:Literal ID="ltlAttachmentHeader" runat="server" /></h2>
        <div class="ebiz-activity-file-details-outer-wrapper">
            <asp:Repeater ID="rptActivityFiles" runat="server">
                <ItemTemplate>
                    <div class="ebiz-activity-file-details-wrap" id="ebiz-activity-file-details-wrapper-<%# Container.ItemIndex%>">
                        <div class="row">
                            <div class="medium-6 columns ebiz-activity-file-details">
                                <span class="ebiz-activity-file-agent">
                                    <asp:Literal ID="ltlFileAgentName" runat="server" /></span>
                                <span class="ebiz-activity-file-blurb">
                                    <asp:Literal ID="ltlFileAttachmentBlurb" runat="server" /></span>
                                <span class="ebiz-activity-file-date">
                                    <asp:Literal ID="ltlFileDateUploaded" runat="server" /></span>
                                <span class="ebiz-activity-file-time">
                                    <asp:Literal ID="ltlFileTimeUploaded" runat="server" /></span>
                            </div>
                            <div class="medium-6 columns ebiz-activity-file-attachment">
                                <span class="ebiz-activity-file-link">
                                    <asp:HyperLink ID="hplFileAttachment" runat="server" Target="_blank" /></span>
                                <input type="button" id="btnDeleteFile" runat="server" class="button ebiz-activity-file-delete-button" />
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plhFileDescription" runat="server">
                            <div class="ebiz-activity-file-description">
                                <span>
                                    <asp:Literal ID="ltlFileDescription" runat="server" /></span>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="row ebiz-add-activity-file-description">
            <div class="medium-3 columns">
                <asp:Label ID="lblFileDescription" runat="server" AssociatedControlID="txtFileDescription" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtFileDescription" runat="server" TextMode="MultiLine" ValidationGroup="AddFile" ClientIDMode="Static" />
            </div>
        </div>
        <div class="ebiz-add-activity-file-upload">
            <div id="fine-uploader"></div>
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhComments" runat="server" Visible="true">
<div class="ebiz-edit-activity-comments">
    <div class="ebiz-edit-activity-comment-wrap">
        <asp:Repeater ID="rptActivityComments" runat="server">
            <ItemTemplate>
                <div class="panel" id="ebiz-edit-activity-comment-wrapper-<%# Container.ItemIndex%>">
                    <div class="ebiz-edit-activity-comment-header-outer-wrap">
                        <div class="ebiz-edit-activity-comment-header-inner-wrap">
                            <span class="ebiz-edit-activity-comment-agent">
                                <asp:Literal ID="ltlCommentAgentName" runat="server" /></span>
                            <span class="ebiz-edit-activity-comment-blurb">
                                <asp:Literal ID="ltlCommentBlurb" runat="server" /></span>
                            <span class="ebiz-edit-activity-comment-date">
                                <asp:Literal ID="ltlCommentDate" runat="server" /></span>
                            <span class="ebiz-edit-activity-comment-time">
                                <asp:Literal ID="ltlCommentTime" runat="server" /></span>
                        </div>
                        <div class="ebiz-edit-delete-activity-wrap" id="ebiz-read-comment-<%# Container.ItemIndex%>">
                            <i class="fa fa-times" id="deleteIcon" runat="server"></i>
                        </div>
                    </div>
                    <div class="ebiz-edit-activity-comment-text-wrap">
                        <div contenteditable="true" id="editComment" runat="server" title="Click to edit">
                            <asp:Literal ID="ltlCommentText" runat="server" />
                        </div>
                        <div class="ebiz-comment-edit-wrap" style="display: none;">
                            <i class="fa fa-pencil" aria-hidden="true"></i>
                        </div>
                        <div class="ebiz-comment-submit-cancel-wrap" style="display: none;">
                            <i class="fa fa-check" aria-hidden="true" title="Commit" runat="server" id="commitIcon"></i>
                            <i class="fa fa-undo" aria-hidden="true" title="Undo" onmousedown="undoActivityCommentChanges(<%# Container.ItemIndex%>);"></i>
                        </div>
                        <div class="ebiz-edit-activity-comment-hidden-text" style="display: none;">
                            <asp:Literal ID="ltlCommentTextHidden" runat="server" />
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="panel ebiz-add-activity-comment-wrap">
        <h2>
            <asp:Literal ID="ltlCommentHeader" runat="server" /></h2>
        <asp:TextBox ID="txtAddComment" runat="server" TextMode="MultiLine" ClientIDMode="Static" />
        <div class="ebiz-add-activity-comment-button-wrap">
            <input type="button" id="btnAddComment" runat="server" class="button ebiz-edit-activity-add-button" />
        </div>
    </div>
</div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhButtons" runat="server">
    <div class="stacked-for-small button-group ebiz-edit-activity-buttons-wrap">
        <asp:HyperLink ID="hplBack" runat="server" CssClass="button ebiz-activities-back" NavigateUrl="~/PagesAgent/CRM/ActivitiesList.aspx" />
        <asp:Button ID="btnCreateActivity" runat="server" CssClass="button ebiz-primary-action ebiz-activities-save" CausesValidation="true" ValidationGroup="Activities" />
        <asp:Button ID="btnUpdateActivity" runat="server" CssClass="button ebiz-primary-action ebiz-activities-save" CausesValidation="true" ValidationGroup="Activities" />
    </div>
</asp:PlaceHolder>

<asp:HiddenField ID="hdfTalentAPIUrl" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfMaxFileSize" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAllowedFileTypes" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfInvalidFileErrorMessage" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfTemplateID" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfCustomerActivityUniqueID" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfActivityCommentItemIndex" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfActivityFileItemIndex" runat="server" ClientIDMode="Static" />

<asp:HiddenField ID="hdfAlertifyDeleteFileTitle" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAlertifyDeleteFileMessage" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAlertifyDeleteCommentTitle" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAlertifyDeleteCommentMessage" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAlertifyOK" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdfAlertifyCancel" runat="server" ClientIDMode="Static" />
