<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AgentCopy.aspx.vb" Inherits="PagesAgent_Admin_AgentCopy" ViewStateMode="Enabled" %>

<%@ Register Src="../../UserControls/AgentsList.ascx" TagName="AgentsList" TagPrefix="Talent" %>


<asp:content id="Content3" contentplaceholderid="ContentPlaceHolder1" runat="Server">
<asp:UpdatePanel ID="updNewGroup" UpdateMode="Conditional" runat="server">
	<ContentTemplate>
	   <Fieldset>  
           <asp:ValidationSummary ID="vlsNewGroup" runat="server" DisplayMode="BulletList" CssClass="alert-box alert" ValidationGroup="ValidateAgent" />
            <asp:PlaceHolder ID="plhAgentCopyErrorMessage" runat="server"><div class="alert alert-box"><asp:Literal ID="ltlAgentCopyErrorMessage" runat="server" /></div></asp:PlaceHolder>

            <div class="panel ebiz-agent-preferences-agent">    
    	        <h2>
                    <asp:Label ID="lblAgentCopy" runat="server" />                
                </h2>
                <Talent:AgentsList ID="uscAgentList" runat="server" />            
            </div>
            
			<div class="panel ebiz-add-new-group">
                <div id="agentCopyControls" runat="server">
                <div class="row">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblNewAgentId" text="New agent id" runat="server" AssociatedControlID="txtNewAgentID" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtNewAgentID" runat="server" MaxLength="10" />
                        <asp:RequiredFieldValidator ControlToValidate="txtNewAgentID" ID="rfvNewAgentID" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="ValidateAgent"
											errormessage="Agent Id is required." Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                     </div>
                <div class="row">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblNewPassword" text="Password" runat="server" AssociatedControlID="txtNewAgentPassword" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtNewAgentPassword" runat="server" TextMode="Password" MaxLength="10" />
                        <asp:RequiredFieldValidator ControlToValidate="txtNewAgentPassword" ID="rfvNewAgentPassword" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="ValidateAgent"
											errormessage="Agent password is required."	Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                    </div>
                <div class="row">
                     <div class="medium-3 columns">
                        <asp:Label ID="lblNewAgentDescription" text="Description" runat="server" AssociatedControlID="txtNewAgentDescription" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtNewAgentDescription" runat="server" MaxLength="30" />
                        <asp:RequiredFieldValidator ControlToValidate="txtNewAgentDescription" ID="rfvNewAgentDescription" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="ValidateAgent"
											errormessage="Agent description is required."	Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                 </div>
                 <div class="medium-12 row">
                     <asp:Button ID="btnCopySubmit" text="Submit" runat="server" CausesValidation="true" OnClick="btnCopySubmit_Click" CssClass="button ebiz-cta" ValidationGroup="ValidateAgent" style="float: right;" />
                </div>
                </div>
			</div>
		 </Fieldset>
	</ContentTemplate>
</asp:UpdatePanel>			
</asp:content>

