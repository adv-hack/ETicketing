<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DespatchNoteGeneration.aspx.vb" Inherits="PagesAgent_DespatchNoteGeneration" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>


<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="1" />
    <script language="javascript" type="text/javascript">
        $(document).ready(function () { $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', maxDate: 0 }); });

        function openWindow(openPDFScript) {
            window.open(openPDFScript, '_blank');
        }

        function enableDatesRFV() {
            var payRef = document.getElementById('<%= txtPaymentRef.ClientID %>');
            var enable = false;
            if (payRef == null || payRef.value == '') {
                enable = true;
            }

            ValidatorEnable(document.getElementById('<%= rfvFromDate.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%= rfvToDate.ClientID%>'), enable);
        }     
    </script>


    <asp:PlaceHolder ID="plhDespatchNoteGeneration" runat="server" >
        <div class="row ebiz-despatch-note-generation-wrap">
             <asp:UpdateProgress ID="updProgressDespatchNoteGeneration" runat="server">
                    <ProgressTemplate>
                        <div class="ebiz-loading-default ebiz-checkout-accordion-loading">
                            <%-- <% = LoadingText %>  --%>  
                            <i class="fa fa-spinner fa-pulse"></i>                     
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            <div class="large-3 columns ebiz-despatch-note-generation-search">
                <div class="panel">
                    <div class="button-group">
                     <asp:Button ID="btnClearFilterTop" runat="server" CssClass="button ebiz-muted-action" Click="btnClearFilter_Click"/>               
                     <asp:Button ID="btnSearchTop" runat="server" CssClass="button ebiz-primary-action" Click="btnSearch_Click" OnClientClick="enableDatesRFV()" UseSubmitBehavior="False" ValidationGroup="Search"/>
                     
                    </div>
                  
                     <asp:PlaceHolder ID="plhType" runat="server">
                    <div class="row ebiz-type">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblType" runat="server" AssociatedControlID="ddlType" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlType" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged"/>
                            <asp:RequiredFieldValidator ID="rfvType" runat="server" ValidationGroup="Search" ControlToValidate="ddlType" InitialValue="-1" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />

                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDateFrom" runat="server">
                    <div class="row ebiz-date-from">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblDateFrom" runat="server" AssociatedControlID="txtDateFrom" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:TextBox ID="txtDateFrom" runat="server" class="datepicker"/>
                            <asp:RequiredFieldValidator ID="rfvFromDate" runat="server" ControlToValidate="txtDateFrom" ValidationGroup="Search" CssClass="error" Display="Static"/>
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDateTo" runat="server">
                    <div class="row ebiz-date-to">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblDateTo" runat="server" AssociatedControlID="txtDateTo" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:TextBox ID="txtDateTo" runat="server" class="datepicker"/>
                            <asp:RequiredFieldValidator ID="rfvToDate" runat="server" ControlToValidate="txtDateTo"  ValidationGroup="Search"  CssClass="error" Display="Static" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhCategory" runat="server">
                    <div class="row ebiz-category">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblCategory" runat="server" AssociatedControlID="ddlCategory" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlCategory" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"/>
                             <asp:RequiredFieldValidator ID="rfvCategory" runat="server" enabled ="true" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlCategory" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSubCategory" runat="server">
                        <div class="row ebiz-sub-category">
                            <div class="large-12 medium-4 columns">
                                <asp:Label ID="lblSubCategory" runat="server" AssociatedControlID="ddlSubCategory" />
                            </div>
                            <div class="large-12 medium-8 columns">
                                <asp:DropDownList ID="ddlSubCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" />
                                 <asp:RequiredFieldValidator ID="rfvSubCategory" runat="server"  InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlSubCategory" Display="Static"  SetFocusOnError="true" />
                            </div>
                        </div>
                    </asp:PlaceHolder>              
                    <asp:PlaceHolder ID="plhEvent" runat="server">
                    <div class="row ebiz-event">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblEvent" runat="server" AssociatedControlID="ddlEvent" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlEvent" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEvent_SelectedIndexChanged"/>
                            <asp:RequiredFieldValidator ID="rfvEvent" runat="server" InitialValue="" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlEvent" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhStand" runat="server">
                    <div class="row ebiz-stand">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblStand" runat="server" AssociatedControlID="ddlStand" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlStand" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStand_SelectedIndexChanged"/>
                            <asp:RequiredFieldValidator ID="rfvStand" runat="server" InitialValue="" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlStand" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhArea" runat="server">
                    <div class="row ebiz-area">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblArea" runat="server" AssociatedControlID="ddlArea" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlArea" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvArea" runat="server" InitialValue="" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlArea" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPayRef" runat="server">
                    <div class="row ebiz-pay-ref">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblPaymentRef" runat="server" AssociatedControlID="txtPaymentRef" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:TextBox ID="txtPaymentRef" runat="server"/>
                             <asp:RequiredFieldValidator ID="rfvPayRef" runat="server" ValidationGroup="Search" CssClass="error"  ControlToValidate="txtPaymentRef" Display="Static"  SetFocusOnError="true" />
                            <asp:RegularExpressionValidator ID="regExPaymentRef" runat="server" ControlToValidate="txtPaymentRef" ValidationGroup="Search" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDeliveryMethod" runat="server">
                    <div class="row ebiz-delivery-method">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblDeliveryMethod" runat="server" AssociatedControlID="ddlDeliveryMethod" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlDeliveryMethod" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvDeliveryMethod" runat="server" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlDeliveryMethod" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhCountry" runat="server">
                    <div class="row ebiz-country">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblCountry" runat="server" AssociatedControlID="ddlCountry" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlCountry" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlCountry" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPostcode" runat="server">
                    <div class="row ebiz-postcode">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblPostcode" runat="server" AssociatedControlID="ddlPostcode" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlPostcode" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvPostcode" runat="server" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlPostcode" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSaleAgent" runat="server">
                    <div class="row ebiz-saleAgent">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblSaleAgent" runat="server" AssociatedControlID="ddlSaleAgent" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlSaleAgent" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvSaleAgent" runat="server" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlSaleAgent" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPayMeth" runat="server">
                    <div class="row ebiz-payMeth">
                        <div class="large-12 medium-4 columns">
                            <asp:Label ID="lblPayMeth" runat="server" AssociatedControlID="ddlPayMeth" />
                        </div>
                        <div class="large-12 medium-8 columns">
                            <asp:DropDownList ID="ddlPayMeth" runat="server"/>
                            <asp:RequiredFieldValidator ID="rfvPayMeth" runat="server" InitialValue="-1" ValidationGroup="Search" CssClass="error"  ControlToValidate="ddlPayMeth" Display="Static"  SetFocusOnError="true" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhIncompleteTransactionsOnly" runat="server">
                    <div class="row ebiz-gift-wrap">
                        <div class="large-12 columns">
                            <asp:CheckBox ID="cbIncompleteTransactionsOnly" runat="server" CssClass="ebiz-checkbox-label-wrap" />
                            <asp:Label ID="lblIncompleteTransactionsOnly" runat="server" AssociatedControlID="cbIncompleteTransactionsOnly" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhGiftWrap" runat="server">
                    <div class="row ebiz-gift-wrap">
                        <div class="large-12 columns">
                            <asp:CheckBox ID="cbGiftWrap" runat="server" CssClass="ebiz-checkbox-label-wrap" />
                            <asp:Label ID="lblGiftWrap" runat="server" AssociatedControlID="cbGiftWrap" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                      <asp:PlaceHolder ID="plhIncludeAdditionalTickets" runat="server">
                    <div class="row ebiz-gift-wrap">
                        <div class="large-12 columns">
                            <asp:CheckBox ID="cbIncludeAdditionalTickets" runat="server" CssClass="ebiz-checkbox-label-wrap" />
                            <asp:Label ID="lblIncludeAdditionalTickets" runat="server" AssociatedControlID="cbIncludeAdditionalTickets" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <div class="button-group">
                     <asp:Button ID="btnClearFilterBottom" runat="server" CssClass="button ebiz-muted-action" Click="btnClearFilter_Click" />
                     <asp:Button ID="btnSearchBottom" runat="server" CssClass="button ebiz-primary-action" Click="btnSearch_Click" OnClientClick="enableDatesRFV()" UseSubmitBehavior="False" ValidationGroup="Search"/>
                     </div>
                </div>
            </div>

             
          
            <div class="large-9 columns ebiz-despatch-note-generation-results">
                <div class="panel">
                    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true" UpdateMode="conditional">
                        <ContentTemplate>             
                             <asp:PlaceHolder ID="plhErrorList" runat="server">
                                <div class="alert-box error">
                                    <asp:BulletedList ID="blErrorMessages" runat="server" />
                                </div>
                            </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhSuccessList" runat="server">
                                <div class="alert-box success">
                                    <asp:BulletedList ID="blSuccessMessages" runat="server" />
                                </div>
                            </asp:PlaceHolder>
                            <div class="button-group despatch-buttons-top mb3">
                            <asp:PlaceHolder ID="plhCreatePDFTop" runat="server">  
                                <asp:Button ID="btnCreatePDFTop" runat="server" CssClass="button" Click="btnCreatePDF_Click"  />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PlhCreateCSVTop" runat="server">  
                                <asp:Button ID="btnCreateCSVTop" runat="server" CssClass="button" Click="btnCreateCSV_Click"  />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhDespatchButtonsTop" runat="server">                              
                                    <asp:Button ID="btnPrintAllTop" runat="server" CssClass="button"/>
                                    <asp:Button ID="btnPrintUnprintedTop" runat="server" CssClass="button"/>   
                            </asp:PlaceHolder>
                            </div>

<asp:Repeater ID="rptPagingTop" runat="server" OnItemCommand="rptPaging_ItemCommand">
    <HeaderTemplate>
        <div class="ebiz-pagination">
            <ul class="pagination">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <asp:LinkButton ID="btnPageTop" CommandName="Page" CommandArgument="<%# Container.DataItem %>" runat="server">
                <%# Container.DataItem %>
            </asp:LinkButton>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
        </div>
    </FooterTemplate>
</asp:Repeater>           

                            <asp:Repeater ID="rptTransaction" runat="server">
                                <HeaderTemplate>
                                <table class="ebiz-responsive-table">
                                        <thead>
                                            <tr>
                                                <asp:PlaceHolder ID="plhchkSelectAll" runat="server">
                                                    <th scope="col" class="ebiz-select"> 
                                                        <asp:CheckBox ID="chkSelectAll" runat="server" ClientIDMode="Static" OnClick="selectAll(this.checked, '.ebiz-item-select');" /> 
                                                    </th>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhPaymentRefVal" runat="server">
                                                    <th scope="col" class="ebiz-payref"> <%=ColumnHeaderText_Ref%></th>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhStatusVal" runat="server">
                                                    <th scope="col" class="ebiz-status"> <%=ColumnHeaderText_Status%></th>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhCustomerVal" runat="server">
                                                    <th scope="col" class="ebiz-customer"> <%=ColumnHeaderText_Customer%>
                                                    </th>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhQuantityVal" runat="server">
                                                    <th scope="col" class="ebiz-quantity"> <%=ColumnHeaderText_Quantity%></th>
                                                </asp:PlaceHolder>
                                                
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        
                                            <tr>
                                                <asp:PlaceHolder ID="plhchkSelectedItem" runat="server">
                                                    <td class="ebiz-item-select" data-title="<%=ColumnHeaderText_Select%>">
                                                        <asp:CheckBox ID="chkSelectedItem" runat="server" OnClick="validateSelAllChkBox(this.checked, '#chkSelectAll', '.ebiz-item-select');"></asp:Checkbox>
                                                    </td>
                                                        <asp:HiddenField ID="hdfTransactionRef" runat="server" Value='<%# Container.DataItem("TransactionRef")%>' />
                                                        <asp:HiddenField ID="hdfBatchID" runat="server" Value='<%# Container.DataItem("TransactionId")%>' />
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhPaymentRef" runat="server">
                                                    <td class="ebiz-payref" data-title="<%=ColumnHeaderText_Ref%>"><%# Container.DataItem("TransactionRef").ToString().Trim()%></td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhStatus" runat="server">
                                                    <td class="ebiz-status" data-title="<%=ColumnHeaderText_Status%>"><%# TransactionStatus(Container.DataItem("TransactionStatus").ToString().Trim())%></td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhCustomer" runat="server">
                                                    <td class="ebiz-customer" data-title="<%=ColumnHeaderText_Customer%>"><%# Container.DataItem("TransactionCustomerName").ToString().Trim()%>&nbsp;<%# FormatCustomerNumber(Container.DataItem("TransactionCustomer").ToString().Trim())%></td>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhQuantity" runat="server">
                                                    <td class="ebiz-quantity" data-title="<%=ColumnHeaderText_Quantity%>"><%# Container.DataItem("TransactionQuantity").ToString().Trim()%></td>
                                                </asp:PlaceHolder>
                                                
                                            </tr>
                                        
                                </ItemTemplate>
                                <FooterTemplate>
                                </tbody>
                                </table>                             
                            </FooterTemplate>
                            </asp:Repeater>

<asp:Repeater ID="rptPagingBottom" runat="server" OnItemCommand="rptPaging_ItemCommand">
    <HeaderTemplate>
        <div class="ebiz-pagination">
            <ul class="pagination">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <asp:LinkButton ID="btnPageBottom" CommandName="Page" CommandArgument="<%# Container.DataItem %>" runat="server">
                <%# Container.DataItem %>
            </asp:LinkButton>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
        </div>
    </FooterTemplate>
</asp:Repeater>

                            <div class="button-group despatch-buttons-bottom">
                            <asp:PlaceHolder ID="plhCreatePDFBottom" runat="server"> 
                                <asp:Button ID="btnCreatePDFBottom" runat="server" CssClass="button" Click="btnCreatePDF_Click" /> 
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhCreateCSVBottom" runat="server"> 
                                <asp:Button ID="btnCreateCSVBottom" runat="server" CssClass="button" Click="btnCreateCSV_Click" /> 
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhDespatchButtonsBottom" runat="server">                          
                                    <asp:Button ID="btnPrintAllBottom" runat="server" CssClass="button"/>
                                    <asp:Button ID="btnPrintUnprintedBottom" runat="server" CssClass="button"/>
                            </asp:PlaceHolder>
                            </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearchTop" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSearchBottom" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCreatePDFTop" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCreatePDFBottom" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div> 
    </asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
</asp:content>