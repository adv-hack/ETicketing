<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AdditionalProductInformation.ascx.vb" Inherits="UserControls_AdditionalProductInformation" EnableViewState="False" %>

<div id="additional-information-accodion" class="accordion ebiz-additional-information">
    <asp:Repeater ID="rptTemplateIDs" runat="server" >
        <ItemTemplate>
                <div id="accordionheader" class="panel ebiz-header" runat="server">
                    <h2>
                        
                            <asp:Label runat="server" ID="lblProfileIDLabel" CssClass="ebiz-profile-id-label" />
                            <div style="display: inline;"><asp:Label runat="server" ID="lblGameDetailsLabel" CssClass="ebiz-game-details-label" /></div>
                            <div style="display: inline;"><asp:Label runat="server" ID="lblGameTimeLabel" CssClass="ebiz-game-time-label" /></div>
                            <asp:HiddenField runat="server" ID="hdfDataTemplateID" />
                            <asp:HiddenField runat="server" ID="hdfDataLoginID" />
                            <asp:HiddenField runat="server" ID="hdfDataUserName" />
                            <asp:HiddenField runat="server" ID="hdfDataProductDescription1" />
                            <asp:HiddenField runat="server" ID="hdfDataProductDescription2" />
                            <asp:HiddenField runat="server" ID="hdfDataProductDescription3" />
                            <asp:HiddenField runat="server" ID="hdfDataProductDescription4" />
                            <asp:HiddenField runat="server" ID="hdfDataProductDescription5" />
                            <asp:HiddenField runat="server" ID="hdfDataProduct" />
                            <asp:HiddenField runat="server" ID="hdfDataSeat" />
                            <asp:HiddenField runat="server" ID="hdfDataPriceBand" />
                            <asp:HiddenField runat="server" ID="hdfDataBasketHeaderID" />
                            <asp:HiddenField runat="server" ID="hdfDataFullName" />
                            <asp:HiddenField runat="server" ID="hdfDataIsTemplatePerProduct" />
                            
                        
                    </h2>
                </div>

                <div id="accordioncontent" class="panel ebiz-content" runat="server">
                    <asp:ValidationSummary ID="vlsAdditionalInfoErrors" runat="server" ShowSummary="false" EnableClientScript="true" DisplayMode="BulletList" CssClass="alert-box alert" />
                    <asp:ValidationSummary ID="vlxRgxAdditionalInfoErrors" runat="server" ShowSummary="false" EnableClientScript="true" DisplayMode="BulletList" CssClass="alert-box alert" />
                    <asp:Repeater ID="rptProductQuestions" runat="server" Visible="false" OnItemDataBound="rptProductQuestions_ItemDatabound">
                        <ItemTemplate>
                            <asp:PlaceHolder id="plhFreeTextField" runat="server" Visible="False" ViewStateMode="Enabled">
                                <div class="row">
                                    <div class="large-6 columns">
                                        <asp:Label runat="server" ID="lblQuestionText" AssociatedControlID="txtQuestionText" CssClass="middle" />
                                    </div>
                                    <div class="large-6 columns">
                                        <asp:TextBox runat="server" ID="txtQuestionText" TextMode="MultiLine" />
                                        <asp:RequiredFieldValidator ID="rfvQuestionText" runat="server" ControlToValidate="txtQuestionText" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled= "false" ClientIDMode="Predictable"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revQuestionText" runat="server" ControlToValidate="txtQuestionText" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled="false" ClientIDMode="Predictable"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <asp:HyperLink runat="server" ID="hplFreeTextFieldExternalLink" Visible="false" target="_blank" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder id="plhCheckbox" runat="server" Visible="False"  ViewStateMode="Enabled">
                                <div class="row">
                                    <div class="large-6 columns">
                                        <asp:Label runat="server" ID="lblQuestionCheckText" CssClass="middle" />
                                    </div>
                                    <div class="large-6 columns">
                                        <asp:CheckBox runat="server" ID="chkQuestionCheckText" ClientIDMode="Static" />
                                        <asp:CustomValidator ID="cvCheckbox" runat="server" Visible="true" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled="false" CssClass="error" ClientValidationFunction="validateCheckbox"></asp:CustomValidator>
                                    </div>
                                </div>
                                <asp:HyperLink runat="server" ID="hplCheckBoxExternalLink" Visible="false" target="_blank" CssClass="button" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder id="plhDate" runat="server" Visible="False"  ViewStateMode="Enabled">
                                <div class="row">
                                    <div class="large-6 columns">
                                        <asp:Label runat="server" ID="lblDate" AssociatedControlID="txtDate" CssClass="middle" />
                                    </div>
                                    <div class="large-6 columns">
                                        <asp:TextBox runat="server" ID="txtDate" CssClass="datepicker"/>
                                        <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled="false"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revDate" runat="server" ControlToValidate="txtDate" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <asp:HyperLink runat="server" ID="hplDateExternalLink" Visible="false" target="_blank" CssClass="button" />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder id="plhListOfAnswers" runat="server" Visible="False">
                            <div class="row">
                                <div class="large-6 columns">
                                    <asp:Label runat="server" ID="lblListOfAnswers" AssociatedControlID="ddlAnswers" CssClass="middle" />
                                </div>
                                <div class="large-6 columns">
                                    <asp:DropDownList ID="ddlAnswers" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 columns">
                                    <asp:Label runat="server" ID="lblSpecify" Visible="false" ClientIDMode="Static" AssociatedControlID="txtSpecify" CssClass="middle" />
                                </div>
                                <div class="large-6 columns">
                                    <asp:TextBox runat="server" ID="txtSpecify" Visible="false" TextMode="MultiLine" />
                                    <asp:RequiredFieldValidator ID="rfvSpecify" runat="server" ControlToValidate="txtSpecify" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled="false" ClientIDMode="Predictable"></asp:RequiredFieldValidator>
                                    <%--<asp:RegularExpressionValidator ID="revSpecify" runat="server" ControlToValidate="txtSpecify" CssClass="error" ValidationGroup="AdditionalInfoValidation" Display="Static" Enabled="false"></asp:RegularExpressionValidator>--%>
                                </div>
                            </div>
                            <asp:HyperLink runat="server" ID="hplListOfAnswersExternalLink" Visible="false"  target="_blank" CssClass="button" />
                        </asp:PlaceHolder>
                        <asp:HiddenField runat="server" ID="hdfRememberedAnswer" />
                        <asp:HiddenField runat="server" ID="answerType" />
                        <asp:HiddenField runat="server" ID="hdfQuestionID" />
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Button ID="btnNext" runat="server" Visible="False" CssClass="button" />
             </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:Button ID="btnSave" runat="server" OnClientClick="return multiValidator()" CssClass="button" />
<input id="hdfValidators" type="hidden" value="<%= Validators %>"/>
