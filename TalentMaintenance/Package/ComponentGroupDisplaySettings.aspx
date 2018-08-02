<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ComponentGroupDisplaySettings.aspx.vb" Inherits="Package_ComponentGroupDisplaySettings" 
MasterPageFile="~/MasterPages/MasterPage.master" EnableEventValidation="false" ValidateRequest="false" %>

<%@ Register Src="../UserControls/BusinessUnitPartnerLanguage.ascx" TagName="BusinessUnitPartnerLanguage" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="ComponentGroupTopNavigation">
       <asp:HyperLink Class="component-settings" ID="hlnkComponent" runat="server"></asp:HyperLink>
    </div>

    <div class="row">
        <asp:Label ID="lblComponentGroupDescription" runat="server"></asp:Label>
    </div>
    <asp:PlaceHolder ID="plhErrorList" runat="server">
        <div class="row error-list">
            <asp:BulletedList ID="blErrorMessages" runat="server" CssClass="error" Visible="false"></asp:BulletedList>
        </div>
    </asp:PlaceHolder>
    <asp:ScriptManager ID="scmMainScriptManager" runat="server" />
    <div class="validation-summary">
        <asp:ValidationSummary ID="vsComponentGroupSettings" runat="server" ValidationGroup="ComponentGroupSettings" />
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:PlaceHolder ID="phlBUPL" runat="server" Visible="true">
                <div class="panel maint-bu-lang">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <div class="row">
                        <div class="large-3 columns">
                        </div>
                        <div class="large-6 columns">
                            <uc1:BusinessUnitPartnerLanguage ID="BUPL" runat="server" />
                            </div>
                            <div class="large-3 columns">
                            </div>
                        </div>
                    </fieldset>
                </div>      
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phlHeader" runat="server" Visible="true">
                <div class="panel maint-comp-group">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <div class="comp-group-settings">
                            <h6>Component Group Settings</h6>
                        </div>
                        <div class="row comp-group-header">
                            <div class="large-3 columns">
                                <asp:Label ID="lblHeader" runat="server" OnPreRender="GetText"></asp:Label>
                                <%--<asp:Label ID="lblHeader" runat="server" Text="Header" ></asp:Label>--%>
                            </div>
                            <div class="large-6 columns">
                                <asp:TextBox ID="txtHeader" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-3 columns">
                                <asp:Button ID="btnUpdateHeader" runat="server" OnPreRender="GetText" />
                                <%--<asp:Button ID="btnUpdateHeader" runat="server" Text="Update" />--%>
                            </div>
                        </div>
                    </fieldset>
                </div>      
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phlComponents" runat="server" Visible="true">
                <div class="panel maint-comp-edit">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <div class="row comp-edit">
                            <h6><asp:Literal ID="litEditComponent" runat="server" OnPreRender="GetText"></asp:Literal></h6>
                        </div>
                        <div class="edit-comp-header ">
                            <div class="large-6 columns ">
                                <asp:Label ID="lblComponent" runat="server"  OnPreRender="GetText"></asp:Label>
                            </div> 
                            <div class="large-6 columns ">
                                <asp:DropDownList ID="ddlComponent" runat="server" AutoPostBack="true"></asp:DropDownList>
                            </div>
                        </div>
                    </fieldset>
                </div>      
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhFields" runat="server" Visible="true">
                <div class="panel maint-comp-fields-edit">
                    <fieldset>
                        <legend>Fieldset</legend>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblComponentHeader" runat="server" Text="Header"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentHeader" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblImage" runat="server" Text="Image"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentImage" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                <asp:Button ID="btnSelectImage" runat="server" Text="Select Image" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblRadioButton" runat="server" Text="Radio Button Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentRadioButtonText" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblText" runat="server" Text="Text"></asp:Label>
                            </div> 
                            <div class="large-8 columns ">
                                <asp:TextBox ID="txtComponentText" runat="server" TextMode="MultiLine" Rows="10" ></asp:TextBox>
                            </div>
                            
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblMoreInfoLink" runat="server" Text="More Info Link"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentMoreInfoLink" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblMoreInfoButton" runat="server" Text="More Info Button Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentMoreInfoButtonText" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblDestination" runat="server" Text="Destination Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentDestination" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblArrivaldate" runat="server" Text="Arrival Date Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentArrivalDate" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblDepartureDate" runat="server" Text="Departure Date Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentDepartureDate" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblCars" runat="server" Text="Cars Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentCarsText" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblRoomType1" runat="server" Text="Room Type1 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentRoomType1" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblRoomType2" runat="server" Text="Room Type2 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentRoomType2" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblExtra1" runat="server" Text="Extra1 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentExtra1" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblExtra2" runat="server" Text="Extra2 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentExtra2" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblExtra3" runat="server" Text="Extra3 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentExtra3" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Label ID="lblExtra4" runat="server" Text="Extra4 Text"></asp:Label>
                            </div> 
                            <div class="large-4 columns ">
                                <asp:TextBox ID="txtComponentExtra4" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-4 columns ">
                                
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-4 columns ">
                                <asp:Button ID="btnUpdateComponent" runat="server" Text="Update" />&nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                            </div> 
                            
                        </div>
                    </fieldset>
                </div>
            </asp:PlaceHolder>

        </div>
    </div>

    </asp:Content>
