<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AddressFormat1Form.ascx.vb"
    Inherits="UserControls_AddressFormat1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Label ID="RegistrationHeaderLabel" runat="server" CssClass="titleLabel"></asp:Label>
<p class="instructions">
    <asp:Label ID="RegistrationInstructionsLabel" runat="server" CssClass="instructionsLabel"></asp:Label></p>
<script type="text/javascript">
    function GetAreas(sender, args) {

        document.getElementById('<%=btnGetAreas.ClientID%>').click();
    }
    function resetPosition(object, args) {
        var tb = object._element;
        var tbposition = findPositionWithScrolling(tb);
        var xposition = tbposition[0];
        var yposition = tbposition[1] + 20; // 22 textbox height 
        var ex = object._completionListElement;
        if (ex)
            $common.setLocation(ex, new Sys.UI.Point(xposition, yposition));
    }
    function findPositionWithScrolling(oElement) {
        if (typeof (oElement.offsetParent) != 'undefined') {
            var originalElement = oElement;
            for (var posX = 0, posY = 0; oElement; oElement = oElement.offsetParent) {
                posX += oElement.offsetLeft;
                posY += oElement.offsetTop;
                if (oElement != originalElement && oElement != document.body && oElement != document.documentElement) {
                    posX -= oElement.scrollLeft;
                    posY -= oElement.scrollTop;
                }
            }
            return [posX, posY];
        } else {
            return [oElement.x, oElement.y];
        }
    }
</script>
<div id="addressformat1">
    <div id="AddressBox" class="AddressBox box default-form" runat="server">
        <div class="addressBoxGroup">
            <div id="AddressTitleRow" runat="server">
                <h2>
                    <asp:Label ID="AddressInfoLabel" runat="server"></asp:Label>
                </h2>
            </div>
            <div id="AddressLine3Row" class="AddressLine3Row" runat="server">
                <asp:Label ID="townLabel" runat="server" AssociatedControlID="town"></asp:Label>
                <asp:TextBox ID="town" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="town" ID="townRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceAddressSearch1" TargetControlID="town"
                    FirstRowSelected="false" CompletionListCssClass="ui-autocomplete" ServiceMethod="GetCityTownList"
                    MinimumPrefixLength="3" CompletionInterval="100" EnableCaching="true" OnClientItemSelected="GetAreas" OnClientShown="resetPosition" >
                </ajaxToolkit:AutoCompleteExtender>
            </div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <fieldset>
                        <asp:Button ID="btnGetAreas" runat="server" Text="" class="getareas" />
                        <div id="AddressLine4Row" class="AddressLine4Row" runat="server">
                            <asp:Label ID="cityLabel" runat="server" AssociatedControlID="city"></asp:Label>
                            <asp:DropDownList ID="cityDDL" CssClass="input-l" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                            <asp:TextBox ID="city" runat="server" Visible="false"></asp:TextBox>
                        </div>
                        <div id="AddressLine5Row" class="AddressLine5Row" runat="server">
                            <asp:Label ID="countyLabel" runat="server" AssociatedControlID="county"></asp:Label>
                            <asp:DropDownList ID="countyDDL" CssClass="input-l" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                            <asp:TextBox ID="county" CssClass="input-l" runat="server" Visible="false"></asp:TextBox>
                        </div>
                        <div id="AddressLine2Row" class="AddressLine2Row" runat="server">
                            <asp:Label ID="streetLabel" runat="server" AssociatedControlID="street"></asp:Label>
                            <asp:TextBox ID="street" CssClass="input-l" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="street" ID="streetRFV" runat="server"
                                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        </div>
                        <div id="AddressPostcodeRow" class="AddressPostcodeRow" runat="server">
                            <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
                            <asp:TextBox ID="postcode" CssClass="input-m" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="postcode" ID="postcodeRFV" runat="server"
                                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="postcode" ID="postcodeRegEx" runat="server"
                                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                        </div>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="AddressCountryRow" class="AddressCountryRow" runat="server">
                <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country"></asp:Label>
                <asp:DropDownList ID="country" CssClass="select" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <asp:Button runat="server" ID="btnValidate" Text="Validate" Visible="false" />
        </div>
    </div>
</div>
