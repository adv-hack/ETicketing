<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ActivitiesList.aspx.vb" Inherits="PagesAgent_CRM_ActivitiesList" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <div class="alert-box alert" id="clientside-errors-wrapper" style="display:none;">
        <ul id="clientside-errors"></ul>
    </div>
    <div class="alert-box success" id="clientside-success-wrapper" style="display:none;">
        <asp:Literal ID="ltlSuccessfullyDeletedActivity" runat="server" />
    </div>
    <asp:PlaceHolder ID="plhErrorList" runat="server">
	    <div class="alert-box alert">
		    <asp:BulletedList ID="blErrorMessages" runat="server" />
	    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSuccessList" runat="server">
	    <div class="alert-box success">
		    <asp:BulletedList ID="blSuccessMessages" runat="server" />
	    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhCreateActivityWarning" runat="server">
        <div class="alert-box warning callout" data-closable id="ebiz-create-activity-warning" style="display:none;">
            <asp:Literal ID="ltlCreateActivityWarning" runat="server" />
            <button class="close-button" aria-label="Dismiss alert" type="button" data-close>
                <i class="fa fa-times" aria-hidden="true"></i>
            </button>
        </div>
    </asp:PlaceHolder>

    <asp:HiddenField ID="hdfCustomerNumber" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfTalentAPIUrl" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesLengthMenu" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesZeroRecords" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesInfo" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesInfoEmpty" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesInfoFiltered" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesPreviousPage" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDataTablesNextPage" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfDatePickerClearDateText" runat="server" ClientIDMode="Static" />

    <asp:HiddenField ID="hdfAlertifyDeleteActivityTitle" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyDeleteActivityMessage" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyOK" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyCancel" runat="server" ClientIDMode="Static" />

    <asp:ValidationSummary ID="vlsAdditionalInfoErrors" runat="server" ShowSummary="true" EnableClientScript="true" DisplayMode="BulletList" CssClass="alert-box alert" ValidationGroup="Activities" />
    <div class="panel ebiz-activities-list-wrap">
        <div class="row">
            <div class="column ebiz-activities-filter-item ebiz-subject">
                <asp:Label ID="lblSubject" runat="server" AssociatedControlID="txtSubject" />
                <asp:TextBox ID="txtSubject" runat="server" ClientIDMode="Static" />
            </div>
            <div class="column ebiz-activities-filter-item ebiz-activity">
                <asp:Label ID="lblActivity" runat="server"  AssociatedControlID="ddlActivityTemplate" />
                <asp:DropDownList ID="ddlActivityTemplate" runat="server" ClientIDMode="Static" />
            </div>
            <div class="column ebiz-activities-filter-item ebiz-user">
                <asp:Label ID="lblUser" runat="server" AssociatedControlID="ddlUser" />
                <asp:DropDownList ID="ddlUser" runat="server" ClientIDMode="Static" />
            </div>
            <div class="column ebiz-activities-filter-item ebiz-status">
                <asp:Label ID="lblStatus" runat="server" AssociatedControlID="ddlStatus" />
                <asp:DropDownList ID="ddlStatus" runat="server" ClientIDMode="Static" />
            </div>
            <div class="column ebiz-activities-filter-item ebiz-date">
                <asp:Label ID="lblDate" runat="server" AssociatedControlID="txtDate" />
                <asp:TextBox ID="txtDate" runat="server" ClientIDMode="Static" CssClass="datepicker" />
                <asp:RegularExpressionValidator ID="revDate" runat="server" ControlToValidate="txtDate" CssClass="error" ValidationGroup="Activities" Display="Static"></asp:RegularExpressionValidator>
            </div>
            <div class="column ebiz-activities-filter-item ebiz-actions">
                <div class="button-group ebiz-button-group-icons">
                    <a href="#" onclick="retrieveActivities();" id="retrieve-activities" data-tooltip aria-haspopup="true" class="button has-tip" data-disable-hover="false" title="<%=SearchActivityText %>" data-hover-delay="600" data-v-offset="7"><i class="fa fa-search fa-fw" aria-hidden="true"></i></a>
                    <asp:LinkButton ID="lbtnCreate" runat="server" data-tooltip aria-haspopup="true" class="button has-tip" data-disable-hover="false" data-hover-delay="600" data-v-offset="7" ValidationGroup="Activities" CausesValidation="true"><i class="fa fa-plus fa-fw" aria-hidden="true"></i></asp:LinkButton>
                </div>
            </div>
        </div>
        <table class="ebiz-responsive-table ebiz-activities-list">
            <thead>
                <tr>
                    <th scope="col" class="ebiz-unique-id" data-title=""></th>
                    <th scope="col" class="ebiz-customer" data-title="<%=CustomerHeaderText %>"><%=CustomerHeaderText %></th>
                    <th scope="col" class="ebiz-activity-template-id" data-title=""></th>
                    <th scope="col" class="ebiz-activity-user" data-title="<%=UserHeaderText%>"><%=UserHeaderText%></th>
                    <th scope="col" class="ebiz-activity-status" data-title="<%=StatusHeaderText%>"><%=StatusHeaderText%></th>
                    <th scope="col" class="ebiz-activity-date" data-title="<%=DateHeaderText%>"><%=DateHeaderText%></th>
                    <th scope="col" class="ebiz-subject" data-title="<%=SubjectHeaderText%>"><%=SubjectHeaderText%></th>
                    <th scope="col" class="ebiz-actions" data-title="<%=ActionsHeaderText %>"><%=ActionsHeaderText %></th>
                </tr>
            </thead>
        </table>
    </div>

</asp:content>
