<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CourseDetails.aspx.vb" Inherits="PagesLogin_Profile_CourseDetails" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhSuccessMessage" runat="server">
        <p class="alert-box success"><asp:Literal ID="ltlSuccessMessage" runat="server" /></p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <p class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsCourseDetails" runat="server" DisplayMode="BulletList" CssClass="error" ShowSummary="true" />

    <div class="panel ebiz-course-details-wrap">
        <fieldset>
            <legend><asp:Literal ID="ltlCourseDetailsLegend" runat="server" /></legend>
            <div class="row ebiz-checkbox-row-wrap ebiz-fan-flag">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCourseDetailsFanFlag" runat="server" AssociatedControlID="chkCourseDetailsFanFlag" />
                </div>
                <div class="medium-9 columns">
                    <asp:CheckBox ID="chkCourseDetailsFanFlag" runat="server" />
                </div>
            </div>
            <div class="row ebiz-contact-name">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCourseDetailsContactName" runat="server" AssociatedControlID="txtCourseDetailsContactName" />
                 </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtCourseDetailsContactName" runat="server" MaxLength="30" />
                </div>
            </div>
            <div class="row ebiz-contact-number">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCourseDetailsContactNumber" runat="server" AssociatedControlID="txtCourseDetailsContactNumber" />
                 </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtCourseDetailsContactNumber" runat="server" MaxLength="30" />
                </div>
            </div>
            <div class="row ebiz-medical-information">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCourseDetailsMedicalInfo" runat="server" AssociatedControlID="txtCourseDetailsMedicalInfo" />
                 </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtCourseDetailsMedicalInfo" runat="server" TextMode="MultiLine" MaxLength="750" />
                    <asp:RegularExpressionValidator ID="regCourseDetailsMedicalInfo" runat="server" Display="None" 
                        ControlToValidate="txtCourseDetailsMedicalInfo" ValidationExpression="(\s|.){0,750}$" CssClass="error" />
                </div>
            </div>
            <div class="ebiz-confirm-wrap">
                <asp:Button ID="btnConfirm" runat="server" CausesValidation="true" CssClass="button ebiz-primary-action" />
            </div>
        </fieldset>
    </div>

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>