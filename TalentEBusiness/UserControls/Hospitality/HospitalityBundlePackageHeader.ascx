<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityBundlePackageHeader.ascx.vb" Inherits="UserControls_Hospitality_HospitalityBundlePackageHeader" %>

<aside>
    <div id="container" runat="server" class="c-hosp-fix">
        <div class="row">
            <div class="column">
                <div class="c-hosp-fix__cont">
                    <%-- <asp:PlaceHolder ID="plhBookingID" runat="server">
                        <div class="alert-box info c-hosp-fix__customer-ref">
                            <asp:Literal ID="ltlBookingRef" runat="server" />
                        </div>
                    </asp:PlaceHolder>--%>
                    <div class="c-hosp-fix__logo">
                        <asp:Image ID="imgOpposition" runat="server" />
                    </div>
                    <div class="c-hosp-fix__info">
                        <div class="row c-hosp-fix__fix">
                            <div class="columns">
                                <h2>
                                    <asp:Literal ID="ltlProductGroupDescription" runat="server" /></h2>
                            </div>
                        </div>
                        <div class="row c-hosp-fix__fix-address-stuff">
                            <div class="columns c-hosp-fix__fix-stuff">
                            </div>
                            <div class="columns c-hosp-fix__customer-stuff">
                                <asp:PlaceHolder ID="plhCompanyName" runat="server">
                                    <div class="row c-hosp-fix__co">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCompanyNameLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span>
                                                <asp:Literal ID="ltlCompanyNameValue" runat="server" /></span>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCustomerName" runat="server">
                                    <div class="row c-hosp-fix__cust">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCustomerNameLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span>
                                                <asp:Literal ID="ltlCustomerNameValue" runat="server" /></span>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCustomerAddress1" runat="server">
                                    <div class="row c-hosp-fix__cust-address">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCustomerAddressLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span class="c-hosp-fix__cust-address1">
                                                <asp:Literal ID="ltlCustomerAddress1" runat="server" /></span>
                                            <asp:PlaceHolder ID="plhCustomerAddress2" runat="server">
                                                <span class="c-hosp-fix__cust-address2">,
                                                                <asp:Literal ID="ltlCustomerAddress2" runat="server" /></span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhCustomerCity" runat="server">
                                                <span class="c-hosp-fix__cust-city">,
                                                                <asp:Literal ID="ltlCustomerCity" runat="server" /></span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhCustomerState" runat="server">
                                                <span class="c-hosp-fix__cust-state">,
                                                                <asp:Literal ID="ltlCustomerState" runat="server" /></span>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhCustomerPostCode" runat="server">
                                                <span class="c-hosp-fix__cust-postcode">,
                                                                <asp:Literal ID="ltlCustomerPostCode" runat="server" /></span>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCustomerMobile" runat="server">
                                    <div class="row c-hosp-fix__cust-mobile">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCustomerMobileLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span>
                                                <asp:Literal ID="ltlCustomerMobile" runat="server" /></span>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCustomerHome" runat="server">
                                    <div class="row c-hosp-fix__cust-phone">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCustomerHomeLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span>
                                                <asp:Literal ID="ltlCustomerHome" runat="server" /></span>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCustomerWork" runat="server">
                                    <div class="row c-hosp-fix__cust-work">
                                        <div class="columns c-hosp-fix__lab">
                                            <asp:Literal ID="ltlCustomerWorkLabel" runat="server" />
                                        </div>
                                        <div class="columns c-hosp-fix__dta">
                                            <span>
                                                <asp:Literal ID="ltlCustomerWork" runat="server" /></span>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</aside>
