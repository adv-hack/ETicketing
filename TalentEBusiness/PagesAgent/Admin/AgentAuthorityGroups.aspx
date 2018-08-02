<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AgentAuthorityGroups.aspx.vb" Inherits="PagesAgent_Admin_AgentAuthorityGroups" ViewStateMode="Enabled" %>

<asp:Content ID="Content3" contentplaceholderid="ContentPlaceHolder1" runat="Server">
	 
<asp:UpdatePanel ID="updGroupsPermissions" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="true">
	<ContentTemplate>
	   <Fieldset>
			<div class="panel ebiz-groups-and-permissions">  
				<h2><asp:Literal ID="ltlGroupsAndPermissions" runat="server"></asp:Literal></h2>    
				<asp:PlaceHolder ID="plhUpdateGroupErrorMessage" runat="server"><div class="alert alert-box"><asp:Literal ID="ltlUpdateGroupErrorMessage" runat="server" /></div></asp:PlaceHolder>
				<asp:PlaceHolder ID="plhUpdateGroupSuccessMessage" runat="server"><div class="success alert-box"><asp:Literal ID="ltlUpdateGroupSuccessMessage" runat="server" /></div></asp:PlaceHolder>
				<div class="row ebiz-current-group">
				<div class="medium-3 columns">
					<asp:Label ID="lblCurrentGroup" runat="server" AssociatedControlID="ddlGroups" />
				</div>
					<div class="medium-9 columns">
						<asp:DropDownList ID="ddlGroups" runat="server" OnSelectedIndexChanged="ddlGroups_Index_Changed" AutoPostBack="true" />
					</div>
				</div>
				<div class="row ebiz-category">
					<div class="medium-3 columns">
						<asp:Label ID="lblCategory" runat="server" AssociatedControlID="ddlCategories" />
					</div>
					<div class="medium-9 columns">
						<asp:DropDownList ID="ddlCategories" runat="server" OnSelectedIndexChanged="ddlCategories_Index_Changed" AutoPostBack="true" Enabled="false" />
					</div>
				</div>
				<div class="ebiz-list-permissions">
					<asp:CheckboxList ID="chkListPermissions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chkListPermissions_Changed"></asp:CheckboxList>
				</div>
				<div class="ebiz-update-permissions-button-wrap">
					<asp:Button ID="btnUpdatePermissions" runat="server" CausesValidation="false" OnClick="btnUpdatePermissions_Click" Enabled="false" CssClass="button ebiz-cta" />
				</div>
			</div>
		</Fieldset>
	</ContentTemplate>
	<Triggers>
	   <asp:AsyncPostBackTrigger ControlID="chkListPermissions" />
	   <asp:AsyncPostBackTrigger ControlID="ddlCategories" />
	   <asp:AsyncPostBackTrigger ControlID="ddlGroups" />
	</Triggers>
</asp:UpdatePanel>

<asp:UpdatePanel ID="updNewGroup" UpdateMode="Conditional" runat="server">
	<ContentTemplate>
	   <Fieldset>   
		   <asp:ValidationSummary ID="vlsNewGroup" runat="server" DisplayMode="BulletList" CssClass="alert-box alert" ValidationGroup="AddGroup" />
			<div class="panel ebiz-add-new-group">
				<h2><asp:Literal ID="ltlAddNewGroup"  runat="server"></asp:Literal></h2>
				<div class="row ebiz-new-group-name">
					<div class="medium-3 columns">
						<asp:Label ID="lblNewGroupName" runat="server" AssociatedControlID="txtNewGroupName" />
					</div>
					<div class="medium-9 columns">
						<asp:Textbox ID="txtNewGroupName" runat="server" maxlength="100" />
						<asp:RequiredFieldValidator ControlToValidate="txtNewGroupName" ID="rfvGroupName" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="AddGroup"
												Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
					</div>
				</div>
				<div class="row ebiz-based-on-group">
					<div class="medium-3 columns">
						<asp:Label ID="lblBasedOn" runat="server" AssociatedControlID="ddlBasedOnGroups" />
					</div>
					<div class="medium-9 columns">
						<asp:DropDownList ID="ddlBasedOnGroups" runat="server" />
					</div>
				</div>
				<div class="ebiz-save-new-group-button-wrap">
					<asp:Button ID="btnSaveNewGroup" runat="server" CausesValidation="true" OnClick="btnSaveNewGroup_Click" CssClass="button ebiz-cta" ValidationGroup="AddGroup" />
				</div>
			</div>
		 </Fieldset>
	</ContentTemplate>
	<Triggers>
	   <asp:PostBackTrigger ControlID="btnSaveNewGroup" />
	</Triggers>
</asp:UpdatePanel>
			

	<asp:UpdateProgress ID="updUpdateProgress" runat="server">
		<ProgressTemplate>
			<div class="ebiz-loading-option">
				<span>
					<% = LoadingText %>                      
				</span>
			</div>
		</ProgressTemplate>
	</asp:UpdateProgress>
</asp:Content>