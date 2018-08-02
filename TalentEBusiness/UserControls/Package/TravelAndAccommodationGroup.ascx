<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TravelAndAccommodationGroup.ascx.vb"
    Inherits="UserControls_Package_TravelAndAccommodationGroup" ViewStateMode="Disabled" %>
<asp:PlaceHolder ID="plhTravelAndAccommodation" Visible="false" runat="server">
    <asp:HiddenField ID="hdnSelectedAreas" runat="server" />
    <asp:HiddenField ID="hdnSelectedComponentId" runat="server" />
    <asp:HiddenField ID="hdnComponentGroupId" runat="server" />
    <asp:HiddenField ID="hdnProductCode" runat="server" />
    <asp:HiddenField ID="hdnTicketingProductCode" runat="server" />
    <asp:HiddenField ID="hdnAvailableComponentQuantity" runat="server" />
    <asp:HiddenField ID="hdnPackageId" runat="server" />
    <asp:HiddenField ID="hdnStage" runat="server" />
    <asp:HiddenField ID="hdnLastStage" runat="server" />
    
    <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
        <asp:BulletedList ID="blErrorList" runat="server" CssClass="alert-box alert">
        </asp:BulletedList>
    </asp:PlaceHolder>
    <ul class="no-bullet ebiz-components-wrap" runat="server" id="ulComponents">
    </ul>
    <asp:PlaceHolder ID="plhComponentDetails" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <asp:Literal ID="ltlComponentText" runat="server"></asp:Literal>
                <asp:PlaceHolder ID="plhMoreInfo" runat="server">
                    <p class="ebiz-more-info-hpl">
                        <asp:HyperLink ID="hplMoreInfo" runat="server" data-open="more-info-modal" CssClass="ebiz-open-modal">
                            <asp:Literal ID="ltlMoreInfo" runat="server" />
                            <i class="fa fa-info-circle"></i>
                        </asp:HyperLink>                        
                    </p>
                    <div id="more-info-modal" class="reveal ebiz-reveal-ajaxs" data-reveal></div>     
                </asp:PlaceHolder>
            </div>
        </div>
        <div class="row">
            <div class="large-6 columns">
                <div class="panel">
                    <div class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblDestination" runat="server" AssociatedControlID="ddlDestination"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:Literal id="ltlDestinationValue" runat="server" Visible="false"></asp:Literal>
                            <asp:DropDownList ID="ddlDestination" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdnFromProductDate" runat="server" />
                    <asp:HiddenField ID="hdnToProductdate" runat="server" />
                    <asp:PlaceHolder ID="plhDates" Visible="true" runat="server">
                        <div class="row">
                            <div class="large-6 columns">
                                <asp:Label ID="lblFromDate" runat="server" AssociatedControlID="ddlFromDate" ></asp:Label>
                            </div>
                            <div class="large-6 columns">
                                <asp:DropDownList ID="ddlFromDate" runat="server">
                                </asp:DropDownList>
                                <asp:Label ID="lblFromDateValue" runat="server" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 columns">
                                <asp:Label ID="lblToDate" AssociatedControlID="ddlToDate" runat="server"></asp:Label>
                            </div>
                            <div class="large-6 columns">
                                <asp:DropDownList ID="ddlToDate" runat="server">
                                </asp:DropDownList>
                                <asp:Label ID="lblToDateValue" runat="server" Visible="false"></asp:Label>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblCars" runat="server" AssociatedControlID="ddlCars" CssClass="middle"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_Car" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_Car">
                                    <asp:Literal ID="ltlRNM_Brief_Car" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_Car" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_Car" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlCars" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbCars" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="carsRegEx" runat="server" ControlToValidate="tbCars" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblRoomType1" runat="server" AssociatedControlID="ddlRoomType1"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_RoomType1" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_RoomType1">
                                    <asp:Literal ID="ltlRNM_Brief_RoomType1" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_RoomType1" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_RoomType1" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlRoomType1" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbRoomType1" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="roomType1RegEx" runat="server" ControlToValidate="tbRoomType1" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblRoomType2" runat="server" AssociatedControlID="ddlRoomType2"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_RoomType2" runat="server" Visible="false">
                               <a href="#" class="alert-box alert" data-toggle="RNM_RoomType2">
                                    <asp:Literal ID="ltlRNM_Brief_RoomType2" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_RoomType2" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_RoomType2" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlRoomType2" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbRoomType2" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="roomType2RegEx" runat="server" ControlToValidate="tbRoomType2" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div id="divLblExtra1" runat="server" class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblExtra1" runat="server" AssociatedControlID="ddlExtra1"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_Extra1" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_Extra1">
                                    <asp:Literal ID="ltlRNM_Brief_Extra1" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_Extra1" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_Extra1" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlExtra1" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbExtra1" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="extra1RegEx" runat="server" ControlToValidate="tbExtra1" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div id="divLblExtra2" runat="server" class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblExtra2" runat="server" AssociatedControlID="ddlExtra2"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_Extra2" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_Extra2">
                                    <asp:Literal ID="ltlRNM_Brief_Extra2" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_Extra2" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_Extra2" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlExtra2" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbExtra2" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="extra2RegEx" runat="server" ControlToValidate="tbExtra2" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div id="divLblExtra3" runat="server" class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblExtra3" runat="server" AssociatedControlID="ddlExtra3"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_Extra3" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_Extra3">
                                    <asp:Literal ID="ltlRNM_Brief_Extra3" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_Extra3" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_Extra3" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlExtra3" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbExtra3" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="extra3RegEx" runat="server" ControlToValidate="tbExtra3" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                    <div id="divLblExtra4" runat="server" class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="lblExtra4" runat="server" AssociatedControlID="ddlExtra4"></asp:Label>
                        </div>
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhRNM_Extra4" runat="server" Visible="false">
                                <a href="#" class="alert-box alert" data-toggle="RNM_Extra4">
                                    <asp:Literal ID="ltlRNM_Brief_Extra4" runat="server"></asp:Literal>
                                    <i class="fa fa-info-circle"></i>
                                </a>
                                <div id="RNM_Extra4" data-dropdown class="dropdown-pane">
                                    <asp:Literal ID="ltlRNM_Detail_Extra4" runat="server"></asp:Literal>
                                </div>
                            </asp:PlaceHolder>
                            <asp:DropDownList ID="ddlExtra4" runat="server">
                            </asp:DropDownList>
                            <asp:TextBox ID="tbExtra4" runat="server" Visible="False"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="extra4RegEx" runat="server" ControlToValidate="tbExtra4" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                        </div>
                    </div>
                </div>
            </div>
            <asp:PlaceHolder ID="plhPriceBands" Visible="true" runat="server">
                <div class="large-6 columns">
                    <div class="panel">
                        <asp:Repeater ID="rptPriceBands" runat="server">
                            <ItemTemplate>
                                <div class="row">
                                    <div class="large-6 columns">
                                        <asp:HiddenField ID="hdfPriceBand" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PriceBand").ToString().Trim() %>' />
                                        <asp:Label ID="lblPriceBand" AssociatedControlID="ddlPriceBand"
                                            Text='<%# DataBinder.Eval(Container.DataItem, "PriceBandText").ToString().Trim() %>'
                                            runat="server"></asp:Label>
                                    </div>
                                    <div class="large-6 columns">
                                        <asp:DropDownList ID="ddlPriceBand" runat="server">
                                        </asp:DropDownList>
                                        <asp:TextBox ID="tbPriceBand" runat="server" Visible="False"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="priceBandRegEx" runat="server" ControlToValidate="tbPriceBand" CssClass="error"
                                                    ErrorMessage="Invalid Quantity" OnLoad="SetRegEx" Enabled="True" Display="Static" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhButtons" runat="server" Visible="true">
        <div class="button-group ebiz-travel-and-accommodation-group-buttons-wrap">
            <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-muted-action ebiz-back" />
            <asp:Button ID="btnStartAgain" runat="server" CssClass="button ebiz-start-again" />
            <asp:Button ID="btnAdd" runat="server" CssClass="button ebiz-add" />
	        <asp:Button ID="btnProceed" runat="server" CssClass="button ebiz-proceed" />
        </div>
    </asp:PlaceHolder>
    <asp:Literal ID="ltlComponentChildControlValues" runat="server"></asp:Literal>
    <script language="javascript" type="text/javascript">
        function assignChildControlValues(parentControl) {
            var selectedkeyCode = "";
            if (document.getElementById(parentControl) != null) {
                selectedkeyCode = document.getElementById(parentControl).value;
            }
            if (componentChilds != null) {
                if (componentChilds.length > 0) {
                    if (selectedkeyCode != null) {
                        if (selectedkeyCode.length > 0) {
                            for (compChildControlIndex = 0; compChildControlIndex < componentChilds.length; compChildControlIndex++) {
                                if (componentChilds[compChildControlIndex].keyCode == selectedkeyCode) {
                                    if (componentChilds[compChildControlIndex].keyType == "DDL") {
                                        setChildControlValues(componentChilds[compChildControlIndex].keyValue.toString(), document.getElementById(componentChilds[compChildControlIndex].controlID));
                                    }
                                    if (componentChilds[compChildControlIndex].keyType == "DIV") {
                                        setChildControlVisibility(componentChilds[compChildControlIndex].keyValue.toString(), document.getElementById(componentChilds[compChildControlIndex].controlID));
                                    }
                                }

                            }

                        }
                    }
                }
            }

        }
        function setChildControlValues(keyValue, childControl) {
            if (childControl != null) {
                for (var i = 0; i < childControl.length; i++) {
                    if (childControl.options[i].value.toString() == keyValue) {
                        childControl.selectedIndex = i;
                    }
                }
            }
        }
        function setChildControlVisibility(keyValue, childControl) {
            if (childControl != null) {
                if (keyValue.toString().toLowerCase() == "true") {
                    childControl.style.display = "block";
                }
                else {
                    childControl.style.display = "none";
                }
            }
        }
    </script>
</asp:PlaceHolder>
