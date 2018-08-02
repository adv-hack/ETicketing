<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ComponentGroupList.aspx.vb" Inherits="Package_ComponentGroupList"
MasterPageFile="~/MasterPages/MasterPage.master" EnableEventValidation="false" ValidateRequest="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    
    <script type="text/javascript" language="javascript">
        function ClearValues() {
            $('#<%=txtComponentGroupSearch.ClientID%>').val('');
            $('#<%=ddlComponentGroupType.ClientID%>').val('ALL');
            return false;
        }
    </script>

     <div class="row">
        <div class="large-12 columns">

            <h1><asp:Label ID="pagetitleLabel" runat="server"></asp:Label></h1>
            <p><asp:Label ID="pageinstructionsLabel" runat="server"></asp:Label></p>
            
            <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
              <div class="alert-box alert">
                <asp:BulletedList ID="blErrorMessages" runat="server" />
              </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhComponentGroupSearch" runat="server" Visible="false">
                <div class="panel component-group-search">
                    <fieldset>
                    <legend>Fieldset</legend>
                                <div class="row search-component-group">
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblComponentGroupSearch" runat="server" AssociatedControlID="txtComponentGroupSearch" CssClass="inline" />
                                    </div>
                                    <div class="large-9 columns">
                                        <asp:TextBox ID="txtComponentGroupSearch" runat="server" />
                                        <ajaxToolkit:AutoCompleteExtender runat="server" ID="aceComponentGroupSearch" TargetControlID="txtComponentGroupSearch" FirstRowSelected="false" CompletionListCssClass="ui-autocomplete"
                                            ServiceMethod="GetComponentGroupList" MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true">
                                        </ajaxToolkit:AutoCompleteExtender>
                                    </div>
                                </div>

                                <div class="row component-group-type">
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblComponentGroupType" runat="server" AssociatedControlID="lblComponentGroupType" CssClass="inline" />
                                    </div>
                                    <div class="large-9 columns ">
                                        <asp:DropDownList ID="ddlComponentGroupType" runat="server" />
                                    </div>
                                </div>
                                <div class="row maint-actions">
                                    <div class="large-12 columns"> 
                                        <asp:Button ID="btnClearOptions" runat="server" class="button search" OnClientClick="javascript:ClearValues();" />&nbsp;
                                        <asp:Button ID="btnAddComponentGroup" runat="server" class="button add" />
                                    </div>
                                </div>
                    </fieldset> 
                </div>
            </asp:PlaceHolder>    
            

            <div class="row">
                <asp:PlaceHolder ID="plhNoComponentGroups" runat="server" Visible="false">
                    <fieldset>
                        <legend>Fieldset</legend>
                            <div class="large-12 columns">
                                <p><asp:Label ID="lblNoGroups" runat="server" AssociatedControlID="lblNoGroups" /></p>
                            </div>
                    </fieldset>
                </asp:PlaceHolder>
            </div>

            <asp:PlaceHolder ID="plhComponentGroup" runat="server">

                <asp:Repeater ID="rptComponentGroup" runat="server">
                        <HeaderTemplate>
                            <table class="maint-component-group">
                                <thead>
                                        <tr>
                                            <th scope="col" class="maint-comgroup-description"><%#ComponentGroupColumnHeader%></th>
                                            <th scope="col" class="maint-comgroup-type"><%#ComponentGroupTypeColumnHeader%></th>
                                            <th scope="col" class="maint-settings"><%#SettingsColumnHeader%></th>
                                            <th scope="col" class="maint-edit"><%#EditColumnHeader%></th>
                                            <th scope="col" class="maint-delete"><%#DeleteColumnHeader%></th>
                                        </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            
                                <tr>
                                    <td class="maint-voucher-description">
                                        <%# DataBinder.Eval(Container.DataItem, "Description").ToString().Trim()%>
                                    </td>
                                    <td class="maint-voucher-type"><%# DataBinder.Eval(Container.DataItem, "ComponentGroupType").ToString().Trim()%></td>
                                    <td class="maint-setting">
                                        <asp:Button ID="btnSettingLink" runat="server" Text='<%# SettingButtonText %>' 
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ComponentGroupId").ToString().Trim() %>' CommandName="Setting" />
                                    </td>
                                    <td class="maint-edit">
                                        <asp:Button ID="btnEditLink" runat="server" Text='<%# EditButtonText %>' 
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ComponentGroupId").ToString().Trim() %>' CommandName="Edit" />
                                    </td>
                                    <td class="maint-delete">
                                        <asp:Button ID="btnDeleteLink" runat="server" Text='<%# DeleteButtonText %>' 
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ComponentGroupId").ToString().Trim() %>' CommandName="Delete" />
                                            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server" 
                                            TargetControlID="btnDeleteLink"
                                            DisplayModalPopupID="ModalPopupExtender1" />
                                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnDeleteLink" 
                                            PopupControlID="PNL" OkControlID="btnPanelOK" RepositionMode="RepositionOnWindowScroll" DropShadow="True"
                                            CancelControlID="btnPanelCancel" BackgroundCssClass="modalBackground" />
                                            <asp:Panel ID="PNL" runat="server" style="display:none; width:200px; color:Black; background-color:White; border-width:2px; border-color:Black; border-style:solid; padding:20px;">
                                                <asp:Literal Text='<%# DeleteConfirmMessage %>' runat="server" ID="ConfirmBoxText"></asp:Literal>
                                                    <br /><br />
                                                    <div style="text-align:right;">
                                                        <asp:Button ID="btnPanelOK" runat="server" OnPreRender="GetText" />
                                                        <asp:Button ID="btnPanelCancel" runat="server" OnPreRender="GetText" />
                                                    </div>
                                            </asp:Panel>
                                    </td>
                                </tr>
                            
                        </ItemTemplate>
                        
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
            </asp:Repeater>

            </asp:PlaceHolder>

        </div>
    </div>

     <div class="row">
        <ajaxToolkit:ToolkitScriptManager ID="tscmMainScriptManager" runat="server" />
     </div>

     <script type="text/javascript" language="javascript">
         $(document).ready(function () {

             //Extend contains selector to make it case insensitive.
             $.expr[':'].contains = function (a, i, m) {
                 return $(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
             };

             $('#<%=txtComponentGroupSearch.ClientID%>').keyup(function () {
                 FilterComponentGroups();
             });

             $('#<%=txtComponentGroupSearch.ClientID%>').focusout(function () {
                 FilterComponentGroups();
             });

             $('#<%=ddlComponentGroupType.ClientID %>').change(function () {
                 FilterComponentGroups();
             });

             function FilterComponentGroups() {
                 var data = $('#<%=txtComponentGroupSearch.ClientID%>').val();
                 var tableRows = $(".maint-component-group").find("tbody").find("tr");
                 var selectedType = $('#<%=ddlComponentGroupType.ClientID%> option:selected').text();
                 if (data == "" && selectedType == "ALL") {
                     tableRows.show();
                     return;
                 }
                 tableRows.hide(); //initially hide all the rows.
                 var filteredRows = tableRows.filter(function (i, v) {
                     var $trow = $(this);  //Select row from the rows collection object 
                     var $trowFirstTD = $trow.find("td:first-child");
                     var $trowSecondTD = $trow.find("td:nth-child(2)");
                     var componentGroupType = selectedType;
                     if (componentGroupType == "ALL") {
                         componentGroupType = $trowSecondTD.text();
                     }
                     if ($trowFirstTD.is(":contains('" + data + "')") && $trowSecondTD.is(":contains('" + componentGroupType + "')")) {
                         return true;
                     }
                     return false;
                 }).show();

                 if (filteredRows.length == 0) {
                     $('#<%=plhNoComponentGroups.ClientID %>').attr('visible', 'true');
                 }
                 else {
                     $('#<%=plhNoComponentGroups.ClientID %>').attr('visible', 'false');
                 }
             };

         });
     </script>
</asp:Content>
