<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityBookingEnquiry.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityBookingEnquiry" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
   <asp:PlaceHolder ID="plhBookingListErrors" runat="server">
      <div class="alert-box alert"><asp:Literal ID="ltlBookingListErrors" runat="server" /></div>
    </asp:PlaceHolder> 

    <asp:PlaceHolder ID="plhSendQAndAReminderSuccess" runat="server">
      <div class="alert-box success"><asp:Literal ID="ltlSendQAndAReminderSuccessText" runat="server" /></div>
    </asp:PlaceHolder> 

    <asp:PlaceHolder ID="plhPrintAllSuccess" runat="server">
      <div class="alert-box success"><asp:Literal ID="ltlPrintAllSubmitSuccessText" runat="server" /></div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhPrintBookingSuccess" runat="server">
      <div class="alert-box success"><asp:Literal ID="ltlPrintBookingSuccessText" runat="server" /></div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhDocumentGenerationSuccess" runat="server">
      <div class="alert-box success"><asp:Literal ID="ltlDocumentGenerationSuccessText" runat="server" /></div>
    </asp:PlaceHolder>
    
    <div class="row">
        <div class="large-3 columns">
            <asp:Panel ID="plhFilterOptions" runat="server" CssClass="panel" DefaultButton="btnSearch">
                <asp:PlaceHolder ID="plhAgent" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblAgent" runat="server" AssociatedControlID="ddlAgent"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:DropDownList ID="ddlAgent" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                    
                <asp:PlaceHolder ID="plhCallId" runat="server">
                    <div class="row" >
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblCallID" runat="server" AssociatedControlID="txtCallID"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtCallID" runat="server" />
                            <asp:RegularExpressionValidator ID="revBookingReference" runat="server" ControlToValidate="txtCallID" ValidationGroup="HospitalityBookingEnquiry" Display="Static" CssClass="error" ></asp:RegularExpressionValidator>
                        </div>
                    </div>
                </asp:PlaceHolder>
               
                <!-- Default to last 30 days -->
                <asp:PlaceHolder ID="plhFromdate" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblFromdate" runat="server" AssociatedControlID="txtFromdate"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtFromdate" runat="server" class="datepicker" />   
                            <asp:RangeValidator runat="server" ID="rgvFromDate" controltovalidate="txtFromdate" type="Date" cssclass="error" validationgroup="HospitalityBookingEnquiry" />                     
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhToDate" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblToDate" runat="server" AssociatedControlID="txtToDate"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtToDate" runat="server"  class="datepicker" />
                            <asp:RangeValidator runat="server" ID="rgvToDate" controltovalidate="txtToDate" type="Date" cssclass="error" validationgroup="HospitalityBookingEnquiry" />
						    <asp:CompareValidator runat="server" ID="cmpToDate" controltovalidate="txtFromDate" controltocompare="txtToDate" cssclass="error" operator="LessThanEqual" type="Date" validationgroup="HospitalityBookingEnquiry" />
                        </div>
                    </div>
                </asp:PlaceHolder>
               
                <asp:PlaceHolder ID="plhStatus" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblStatus" runat="server" AssociatedControlID="ddlStatus"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:DropDownList ID="ddlStatus" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhMarkOrderFor" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblMarkOrderFor" runat="server" AssociatedControlID="ddlMarkOrderFor"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:DropDownList ID="ddlMarkOrderFor" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                             
                <asp:PlaceHolder ID="plhCustomer" runat="server">  
                    <div class="row">                      
                        <div class="medium-4 large-12 columns">
                            <asp:Label ID="lblCustomer" runat="server" class="medium-only-middle" AssociatedControlID="txtCustomer"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtCustomer" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
               
                <asp:PlaceHolder ID="plhPackage" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblPackage" runat="server" AssociatedControlID="txtPackage"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtPackage" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
              
                <asp:PlaceHolder ID="plhProduct" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblProduct" runat="server" AssociatedControlID="txtProduct"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtProduct" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="plhProductGroup" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblProductGroup" runat="server" AssociatedControlID="txtProductGroup"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:TextBox ID="txtProductGroup" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="plhQandAStatus" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblQandAStatus" runat="server" AssociatedControlID="ddlQandAStatus"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:DropDownList ID="ddlQandAStatus" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhPrintStatus" runat="server">
                    <div class="row">
                        <div class="medium-4 large-12 columns">
                            <asp:Label class="medium-only-middle" ID="lblPrintStatus" runat="server" AssociatedControlID="ddlPrintStatus"/>
                        </div>
                        <div class="medium-8 large-12 columns">
                            <asp:DropDownList ID="ddlPrintStatus" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="button-group large-expanded">
                    <asp:Button ID="btnClear" runat="server" CssClass="button ebiz-muted-action" OnClientClick="return ClearDataTableState()"/>
					<asp:Button ID="btnSearch" runat="server" CssClass="button ebiz-primary-action" Validationgroup="HospitalityBookingEnquiry" />
                </div>
            </asp:Panel>
        </div>

        <div class="large-9 columns">
            <div class="panel">
                <div class="button-group mb3">
                    <asp:PlaceHolder ID="plhSendQAndAReminder" runat="server">
                        <asp:LinkButton ID="lbtnSendQAndAReminder" ClientIDMode="Static" CssClass="button ebiz-send-qa-reminder" runat="server" OnClientClick="SendQAndAReminderButtonClick(); return false;"><i class="fa fa-envelope" aria-hidden="true"></i></asp:LinkButton>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPrintAllTickets" runat="server">                      
                        <asp:LinkButton ID="lbtnPrintAllTickets" runat="server" ClientIDMode="Static" CssClass="button ebiz-print-all-tickets" OnClientClick="PrintBookingsClick(); return false;"><i class="fa fa-print" aria-hidden="true"></i></asp:LinkButton>                           
                    </asp:PlaceHolder>
                </div>                                                                          

                <asp:PlaceHolder ID="plhBookingList" runat="server">
                    <asp:repeater ID="rptHospitalityBookings" runat="server">
					    <HeaderTemplate>
                            <table class="datatable ebiz-responsive-table">
							    <thead>
									<tr>
                                        <th scope="col" ID="CallIdColumnHeader" runat="server"><%=CallIDColumnHeading%></th>
					                    <th scope="col" id="PackageColumnHeader" runat="server"><%=PackageColumnHeading%></th>
					                    <th scope="col" id="ProductColumnHeader" runat="server"><%=ProductColumnHeading %></th>
					                    <th scope="col" id="AgentColumnHeader" runat="server"><%=AgentColumnHeading%></th>
					                    <th scope="col" id="DateColumnHeader" runat="server"><%=DateColumnHeading%></th>
                                        <th scope="col" id="ValueColumnHeader" runat="server"><%=ValueColumnHeading%></th>
                                        <th scope="col" id="StatusColumnHeader" runat="server"><%=StatusColumnHeading%></th>
                                        <th scope="col" id="QandAStatusColumnHeader" runat="server"><%=QandAStatusColumnHeading%></th>
                                        <th scope="col" id="PrintStatusColumnHeader" runat="server"><%=PrintStatusColumnHeading%></th>
                                        <th scope="col">&nbsp;</th> 
                                        <th scope="col">&nbsp;</th>
                                        <th scope="col" style="display:none;">&nbsp;</th>
									</tr>
								</thead>
								<tbody>
						</HeaderTemplate>
						<ItemTemplate>
                                    <tr>                             
                                        <td ID="CallIdColumn" runat="server"><%# Eval("BookingRef")%></td>
                                        <td ID="PackageColumn" runat="server"><%# Eval("PackageLabel")%></td>
                                        <td ID="ProductColumn" runat="server"><%# Eval("ProductLabel")%></td>
                                        <td ID="AgentColumn" runat="server"><%# Eval("ProcessedByLabel")%></td>
                                        <td ID="DateColumn" runat="server"><%# Eval("FormattedDate")%></td>
                                        <td ID="ValueColumn" runat="server" class="large-text-right"><%# Eval("FormattedGrossValueInclDiscount")%></td>
                                        <td ID="StatusColumn" runat="server"><%# Eval("StatusDescription")%></td>
                                        <td ID="QandAStatusColumn" runat="server"><%# Eval("QandAStatusDescription")%></td>
                                        <td ID="PrintStatusColumn" runat="server"><%# Eval("PrintStatus")%></td>
                                        <td><%# Eval("ProcessDate")%></td>
                                        <td>
                                            <!-- Line actions -->
                                            <ul class="menu o-table-menu-icons">
                                            <asp:PlaceHolder ID="plhViewBooking" runat="server">
                                                <li>
                                                    <a href="#" onclick="DetailsButtonClick('<%#ResolveUrl(Eval("BookingForwardingUrl"))%>', '<%#Eval("RequiresLogin")%>'); return false;"><i class="fa fa-info-circle" aria-hidden="true"></i></a>
                                                </li>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhGeneratePDFForBooking" runat="server">
                                                <li>
                                                    <asp:LinkButton ID="lbtnGeneratePDFForBooking" runat="server" CommandName="GeneratePDFForBooking" CommandArgument='<%# Eval("BookingRef")%>'><i class="fa fa-file-pdf-o" aria-hidden="true"></i></asp:LinkButton>
                                                    <asp:HiddenField ID="hdfProductCode" runat="server" Value='<%# Eval("ProductCode")%>' />
                                                    <asp:HiddenField ID="hdfPackageId" runat="server" Value='<%# Eval("PackageDefId")%>' />
                                                    <asp:HiddenField ID="hdfCustomerNumber" runat="server" Value='<%# Eval("TicketingCustomerMember")%>' />
                                                </li>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhPrintSingleBooking" runat="server">
                                                <li><a href="#" onclick="PrintSingleBookingClick('<%# Eval("BookingRef")%>','<%# Eval("TicketsInBooking")%>'); return false;" title="<%=PrintSingleBookingTitleText%>"><i class="fa fa-print" aria-hidden="true"></i></a></li>                     
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhGenerateDocumentForBooking" runat="server">
                                                <li><a href="#" onclick="GenerateDocumentForBookingClick('<%# Eval("BookingRef")%>'); return false;" title="<%=SingleBookingDocumentTitleText%>"><i class="fa fa-file-word-o" aria-hidden="true"></i></a></li>                     
                                            </asp:PlaceHolder>
                                            </ul>
                                        </td>
                                        <td style="display:none;"><%# Eval("RequiredQAndAReminder")%></td>									
							        </tr>
						</ItemTemplate>
						<FooterTemplate>
								</tbody>
							</table>
					    </FooterTemplate>
				    </asp:repeater>
                </asp:PlaceHolder>              
            </div>   
        </div>
    </div>
    
    <asp:HiddenField ID="hdfDataTablesInfoEmpty" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfForwardingUrl" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyTitle" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessage" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOK" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancel" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfHideColumns" runat="server" ClientIDMode="Static"/>    
    <asp:HiddenField ID="hdfClearDataTableState" runat="server" ClientIDMode="Static"/>    
    <asp:HiddenField ID="hdfSendQAndAReminder" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfListOfCallIds" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyTitleForQAndAReminder" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessageForQAndAReminder" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOKForQAndAReminder" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancelForQAndAReminder" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfQAndAReminderDisabledMessage" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfPrintBookings" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyTitleForPrintAll" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessageForPrintAll" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOKForPrintAll" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancelForPrintAll" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfPrintSingleBooking" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyTitleForPrintSingleBooking" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessageForPrintSingleBooking" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOKForPrintSingleBooking" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancelForPrintSingleBooking" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfSelectedCallIdToBePrinted" runat="server" ClientIDMode="Static" />    
    <asp:HiddenField ID="hdfNumberOfTicketsInBooking" runat="server" ClientIDMode="Static" />    
    <asp:HiddenField ID="hdfPrintAllDisabledMessage" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfPrintStatusNotPrinted" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfPrintStatusPartiallyPrinted" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfPrintStatusFullyPrinted" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfBookingStatus" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfGenerateBookingDocument" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyTitleForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessageForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOKForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancelForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfCallIdForDocumentProduction" runat="server" ClientIDMode="Static" />    
    <asp:HiddenField ID="hdfMergedDocumentPath" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfCreatePDFForBooking" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAlertifyTitleForCreatePDF" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyMessageForCreatePDF" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyOKForCreatePDF" runat="server" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdfAlertifyCancelForCreatePDF" runat="server" ClientIDMode="Static"/>
</asp:content>

