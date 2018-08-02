<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ComponentGroupDetails.aspx.vb" Inherits="Package_ComponentGroupDetails" 
MasterPageFile="~/MasterPages/MasterPage.master" EnableEventValidation="false" ValidateRequest="false" 
MaintainScrollPositionOnPostback="true"%>

 <%@ Register Src="../UserControls/Package/ComponentGroup.ascx" TagName="ComponentGroup" TagPrefix="uc1" %>
 <%@ Register Src="../UserControls/Package/TravelAndAccommodationGroup.ascx" TagName="TAGroup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="ComponentGroupTopNavigation">
       <asp:HyperLink Class="component-settings" ID="hlnkComponent" runat="server"></asp:HyperLink>
    </div>
    <asp:ScriptManager ID="scmMainScriptManager" runat="server" />

    <div class="validation-summary">
            <asp:ValidationSummary ID="vsVoucherDetail" runat="server" ValidationGroup="VoucherDetail" />
    </div>

    <div class="row">
        <div class="large-24 columns">
            <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
                <div class="alert-box alert">
                    <asp:BulletedList ID="blErrorMessages" runat="server" />
                </div>
            </asp:PlaceHolder>
            
            <div class="panel maint-component-group-details">
                <asp:PlaceHolder ID="plhComponentGroupDetails" runat="server" Visible="true">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <h2 class="subheader"><small><asp:Label ID="pagetitleLabel" runat="server"></asp:Label></small></h2>
                        <p><asp:Label ID="pageinstructionsLabel" runat="server"></asp:Label></p>

                        <div class="row maint-component-group-description">
                            <div class="large-3 columns">   
                                <asp:Label ID="lblSelectedComponentGroupDesc" runat="server" AssociatedControlID="txtComponentGroupDescription"  />
                            </div>
                            <div class="large-9 columns">
                                <asp:TextBox ID="txtComponentGroupDescription" runat="server" MaxLength="30"></asp:TextBox>
                            </div>
                        </div>
        
                        <div class="row maint-component-group-type">
                            <div class="large-3 columns">
                                <asp:Label ID="lblComponentGroupType" runat="server" AssociatedControlID="lblComponentGroupType" />
                            </div>
                            <div class="large-9 columns ">
                                <asp:DropDownList ID="ddlComponentGroupType" runat="server" />
                            </div>
                        </div>

                        <div class="row maint-component-group-code">
                            <div class="large-3 columns">
                                <asp:Label ID="lblComponentGroupCode" runat="server" AssociatedControlID="txtComponentGroupCode" />
                            </div>
                            <div class="large-9 columns ">
                                <asp:TextBox ID="txtComponentGroupCode" runat="server" ReadOnly="true" MaxLength="12"  />
                            </div>
                        </div>

                        <asp:PlaceHolder runat="server" ID="plhValidation" Visible="false">
                            <h2 class="subheader"><small><asp:Label ID="lblValidation" runat="server"></asp:Label></small></h2>
                            <%--<div class="row maint-component-group-no-of-components">
                                <div class="large-3 columns">
                                    <asp:Label ID="lblNumberOfComponentsToSelect" runat="server" AssociatedControlID="lblNumberOfComponentsToSelect" />
                                </div>
                                <div class="large-9 columns ">
                                    <asp:DropDownList ID="ddlNumberOfComponentsToSelect" runat="server" />
                                </div>
                            </div>--%>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="plhValidationCG" Visible="false">
                            <div class="row maint-component-group-CG">
                                <div class="large-3 columns">
                                    <asp:Label ID="lblQuantityToComponentRatio" runat="server" AssociatedControlID="txtQuantityToComponentRatioFrom" />
                                </div>
                                <div class="large-3 columns ">
                                    <asp:DropDownList ID="ddlQuantityToComponentRatio" runat="server"></asp:DropDownList>
                                </div>
                                <div class="large-6 columns ">
                                    <asp:Label ID="lblMin0" runat="server" AssociatedControlID="lblMin0" style="text-align:left"></asp:Label>
                                    <asp:TextBox ID="txtQuantityToComponentRatioFrom" runat="server" />
                                    <asp:Label ID="lblMax0" runat="server" AssociatedControlID="lblMax0" style="text-align:left" ></asp:Label>
                                    <asp:TextBox ID="txtQuantityToComponentRatioTo" runat="server" />
                                </div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder runat="server" ID="plhValidationTA" Visible="false">
                            <div class="row maint-component-group-TA">
                                <div class="row">  
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblQuantityToRoomRatio" runat="server" AssociatedControlID="txtQuantityToRoomRatioFrom" />
                                    </div>
                                    <div class="large-3 columns ">
                                        <asp:DropDownList ID="ddlQuantityToRoomRatio" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="large-6 columns ">
                                        <asp:Label ID="lblMin1" runat="server" AssociatedControlID="lblMin1" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToRoomRatioFrom" runat="server" />
                                        <asp:Label ID="lblMax1" runat="server" AssociatedControlID="lblMax1" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToRoomRatioTo" runat="server" />
                                    </div>
                                </div>
                                <div class="row"> 
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblQuantityToPeopleRatio" runat="server" AssociatedControlID="txtQuantityToPeopleRatioFrom" />
                                    </div>
                                    <div class="large-3 columns ">
                                        <asp:DropDownList ID="ddlQuantityToPeopleRatio" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="large-6 columns ">
                                        <asp:Label ID="lblMin2" runat="server" AssociatedControlID="lblMin2" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToPeopleRatioFrom" runat="server" />
                                        <asp:Label ID="lblMax2" runat="server" AssociatedControlID="lblMax2" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToPeopleRatioTo" runat="server" />
                                    </div>
                                </div>
                                <div class="row"> 
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblQuantityToCarRatio" runat="server" AssociatedControlID="txtQuantityToCarRatioFrom" />
                                    </div>
                                    <div class="large-3 columns ">
                                        <asp:DropDownList ID="ddlQuantityToCarRatio" runat="server"></asp:DropDownList>  
                                    </div>
                                    <div class="large-6 columns ">
                                        <asp:Label ID="lblMin3" runat="server" AssociatedControlID="lblMin3" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToCarRatioFrom" runat="server" />
                                        <asp:Label ID="lblMax3" runat="server" AssociatedControlID="lblMax3" style="text-align:left" ></asp:Label>
                                        <asp:TextBox ID="txtQuantityToCarRatioTo" runat="server" />
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Label ID="lblNoneReqOption" runat="server" AssociatedControlID="ddlNoneReqOpt" />
                                </div>
                                <div class="large-9 columns ">
                                    <asp:DropDownList ID="ddlNoneReqOpt" runat="server" />
                                </div>
                            </div>

                        </asp:PlaceHolder>

                        <div class="row maint-actions">
                            <div class="large-12 columns">
                                <asp:Button ID="btnAddOrUpdateComponentGroup" runat="server"  /> &nbsp;
                                <asp:Button ID="btnCancelComponentGroupAddInsert" runat="server"  />
                            </div>
                        </div>

                    </fieldset>
                </asp:PlaceHolder>
            </div>
             
            <asp:PlaceHolder ID="phlCG" runat="server" Visible="true">
                <div class="panel maint-component-usercontrol-CG">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <uc1:ComponentGroup ID="CG" runat="server" />
                    </fieldset>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phlTA" runat="server" Visible="true">
                <div class="panel maint-component-usercontrol-TA">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <uc1:TAGroup ID="TA" runat="server" />
                    </fieldset>
                </div>
            </asp:PlaceHolder>
       
        </div>
    </div>



    <script language="javascript" type="text/javascript">
        $(document).ready(function () {

            var CGtextboxes = $(".maint-component-group-CG :input");
            CGtextboxes.each(function () {
                $(this).keydown(function () {
                    AllowNumericOnly(event);
                });
            });

            var TAtextboxes = $(".maint-component-group-TA :input");
            TAtextboxes.each(function () {
                $(this).keydown(function () {
                    AllowNumericOnly(event);
                });
            });


            function AllowNumericOnly(event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
            (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            }

            function AllowNumericWithDecimalOnly(event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
            event.keyCode == 190 ||
                // Allow: Ctrl+A
            (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            }


        });
</script>
</asp:Content>
