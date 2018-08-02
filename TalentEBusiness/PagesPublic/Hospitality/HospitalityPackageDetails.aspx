<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityPackageDetails.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityPackageDetails" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixturePackageHeader.ascx" TagName="HospitalityPackageHeader" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackages.ascx" TagName="HospitalityPackages" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityPackageHTMLInclude.ascx" TagName="HospitalityPackageHTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixtures.ascx" TagName="HospitalityFixtures" TagPrefix="Talent" %>

<asp:content id="ContentHead" contentplaceholderid="ContentPlaceHolder2" runat="server">
</asp:content>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    
    <asp:Panel ID="pnlHospitalityPackageDetails" runat="server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorList" runat="server">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsBookingErrors" runat="server" ValidationGroup="HospitalityBooking" ShowSummary="true" CssClass="alert-box alert ebiz-booking-errors" />

    <Talent:HospitalityPackageHeader ID="uscHospitalityPackageHeader" runat="server" />

    <div class="row">
        <div class="columns o-hosp-fix-pak__cont">
            <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude1" runat="server" Sequence="1"/>
            <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude2" runat="server" Sequence="2"/>
        </div>
        <div class="columns o-hosp-fix-pak__sb c-hosp-pak--sb">

            <asp:PlaceHolder ID="plhPackageBookingOptions" runat="server">
            <div class="c-hosp-fix-pak-atb__cont">
                <h1><asp:Literal ID="ltlPackageDescription" runat="server" /></h1>
                <div class="c-hosp-fix-pak-atb__opp"><asp:Literal ID="ltlProductDescription" runat="server" /></div>
                <div class="c-hosp-fix-pak-atb__com"><asp:Literal ID="ltlProductCompeitionDescription" runat="server" /></div>
                <asp:Placeholder ID="plhProductDate" runat="server">
                    <div class="c-hosp-fix-pak-atb__d"><asp:Literal ID="ltlProductDate" runat="server" /></div>
                </asp:Placeholder>
                <asp:PlaceHolder ID="plhQuantityBox" runat="server">
                     <div class="row c-hosp-fix-pak-atb__con">
                    <div class="columns c-hosp-fix-pak-atb__con-lab">
                        <asp:Label ID="lblQuantity" AssociatedControlID="txtQuantity" runat="server" />
                    </div>
                    <div class="columns c-hosp-fix-pak-atb__con-tb">
                        <asp:TextBox ID="txtQuantity" runat="server" type="number" min="1" ValidationGroup="HospitalityBooking" MaxLength="2" />
                        <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity" ValidationGroup="HospitalityBooking" Display="None" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" />
                    </div>
                </div>
                </asp:PlaceHolder>
               
               <asp:PlaceHolder ID="plhBookHospitality" runat="server">
                  <div class="c-hosp-fix-pak-atb_btn-book">
                    <asp:LinkButton ID="lbtnBookHospitality" CssClass="button" runat="server" ValidationGroup="HospitalityBooking"><asp:Literal ID="ltlBookHospitality" runat="server" /> <i class="fa fa-chevron-right" aria-hidden="true"></i></asp:LinkButton>                    
                  </div>
               </asp:PlaceHolder>
               <asp:PlaceHolder ID="plhPreBooking" runat="server">
                  <div class="c-hosp-fix-pak-atb_btn-prebook">
                    <asp:LinkButton ID="lbtnPreBooking" CssClass="button" runat="server"><asp:Literal ID="ltlHospitalityPreBooking" runat="server" /> <i class="fa fa-chevron-right" aria-hidden="true"></i></asp:LinkButton>
                  </div>
               </asp:PlaceHolder>
               
            </div>
            </asp:PlaceHolder>
            
            <Talent:HospitalityFixtures ID="uscHospitalityFixtures" runat="server" DynamicLayoutClass="c-hosp-item--sidebar" />

            <asp:PlaceHolder id="plhViewFromArea" runat="server">
                <div class="panel">
                    <asp:Image ID="imgViewAreaSmall" runat="server" data-open="view-area"/>
                      <div class="f2 c-hosp-pak__view-from-area">
                           <asp:HyperLink ID ="hlViewFromArea" runat="server" data-open="view-area"></asp:HyperLink>
                      </div>   
                      <div class="reveal" data-reveal id='view-area'>
                          <asp:Image ID="imgViewAreaBig" runat="server" />
                            <button class="close-button" data-close aria-label="Close modal" type="button">
                                    <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
                            </button>
                      </div>
                </div>
            </asp:PlaceHolder>
             
            <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude3" runat="server" Sequence="3"/>
            <Talent:HospitalityPackageHTMLInclude ID="uscHospitalityPackageHTMLInclude4" runat="server" Sequence="4"/>                      
        </div>
    </div>
    
    <Talent:HospitalityPackages ID="uscHospitalityPackages" Usage="FilteredPackages" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />

    </asp:Panel>    
</asp:content>
