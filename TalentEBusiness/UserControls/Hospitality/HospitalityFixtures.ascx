<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityFixtures.ascx.vb" Inherits="UserControls_HospitalityFixtures" %>

<asp:PlaceHolder ID="plhErrorMessages" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="blErrorMessages" runat="server" />
    </div>
</asp:PlaceHolder>

<aside>
    <div class="c-hosp-item c-hosp-item--fixtures <%=DynamicLayoutClass %>">
        <div class="row">
            <div class="column">
                <h1><asp:Literal ID="ltlFixturesHeader" runat="server" /></h1>
                <asp:PlaceHolder ID="plhAddNFixturesToProductGroupHeader" runat="server" Visible='<%# If(Usage="HospitalityBundleFixtures.aspx", "True", "False") %>'>
                    <div class="alert-box info">
                        <asp:Literal ID="ltlAddNFixturesToProductGroupMessage" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <div class="row c-hosp-item__row">
                    <asp:Repeater ID="rptFixturesHome" runat="server" OnItemDataBound="rptFixturesHome_ItemDataBound">
                        <ItemTemplate>
                            <div class="column c-hosp-item__column">                                
                                <div id="divFixtureItem" runat="server">
                                    <asp:HyperLink ID="hlkFixturePackageDetails" runat="server">                                    
                                        <div class="c-hosp-item__h">
                                          <h2>                                              
                                              <asp:Label ID="lblProductDescription" runat="server" Text='<%# Eval("ProductDescription")%>' />
                                          </h2>
                                        </div>             
                                                                              
                                        <div class="c-hosp-item__body">
                                          <div class="c-hosp-item__img">
                                              <asp:Image ID="imgOppositionHome" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode"))%>' />
                                          </div>
                                          <div class="c-hosp-item__info">
                                              <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductCompetitionDesc")) %>'>
                                                <div class="c-hosp-item__comp"><%# Eval("ProductCompetitionDesc")%></div>
                                              </asp:PlaceHolder>    
                                              <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductDate")) %>'>                                        
                                                <div class="c-hosp-item__date"><%# Eval("ProductDate")%></div>  
                                              </asp:PlaceHolder>
                                              <asp:PlaceHolder ID="plhHomeFixtureSubType" runat="server" Visible='<%# DisplaySubType AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubType"))) AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubTypeDesc")))%>'>                                                
                                                  <div class="c-hosp-item__comp"><%# Eval("ProductSubTypeDesc")%></div>                                          
                                              </asp:PlaceHolder>
                                          </div>
                                          <asp:PlaceHolder ID="plhHomeFixtureEligibility" runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductReqdMemDesc")) %>'>
                                                <div class="c-hosp-item__prereq"><%#GetProductEligibilityText(Eval("ProductReqdMemDesc"))%></div>
                                          </asp:PlaceHolder>
                                        </div>                                    
                                    </asp:HyperLink>
                                    <asp:PlaceHolder ID="plhHospitalityBundleFixtureItem" runat="server">                                    
                                        <div class="c-hosp-item__h">
                                          <h2>
                                              <asp:CheckBox ID="chkAddFixtureToHospitalityBundleBooking" runat="server" class=" hospitality-fixture-select"/>
                                              <asp:Label ID="lblProductDescriptionForBundle" runat="server" Text='<%# Eval("ProductDescription")%>' />
                                          </h2>
                                        </div>             
                                                                              
                                        <div class="c-hosp-item__body">
                                          <div class="c-hosp-item__img">
                                              <asp:Image ID="imgOppositionHomeForBundle" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode"))%>' />
                                          </div>
                                          <div class="c-hosp-item__info">
                                              <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductCompetitionDesc")) %>'>
                                                <div class="c-hosp-item__comp"><%# Eval("ProductCompetitionDesc")%></div>
                                              </asp:PlaceHolder>    
                                              <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductDate")) %>'>                                        
                                                <div class="c-hosp-item__date"><%# Eval("ProductDate")%></div>  
                                              </asp:PlaceHolder>
                                              <asp:PlaceHolder ID="plhHomeFixtureSubTypeForBundle" runat="server" Visible='<%# DisplaySubType AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubType"))) AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubTypeDesc")))%>'>                                                
                                                  <div class="c-hosp-item__comp"><%# Eval("ProductSubTypeDesc")%></div>                                          
                                              </asp:PlaceHolder>
                                          </div>
                                          <asp:PlaceHolder ID="plhHomeFixtureEligibilityForBundle" runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductReqdMemDesc")) %>'>
                                                <div class="c-hosp-item__prereq"><%#GetProductEligibilityText(Eval("ProductReqdMemDesc"))%></div>
                                          </asp:PlaceHolder>
                                        </div>                                    
                                    </asp:PlaceHolder>

                                  <div class="c-hosp-item__footer">

                                            <div class="c-hosp-item__avail-container">
                                                <span class="c-hosp-item__avail c-hosp-item__avail--<%# Eval("AvailabilityDetail.AvailabilityCSS")%>" data-toggle="reveal-<%# Eval("ProductCode")%>" style="background-color: <%# Eval("AvailabilityDetail.AvailabilityColour")%>;">
                                                    <%# Eval("AvailabilityDetail.AvailabilityText")%>
                                                </span>
                                                <asp:PlaceHolder ID="plhAvailabilityDropDown" runat="server" Visible="<%#AgentProfile.IsAgent %>">
                                                    <div class="dropdown-pane" id="reveal-<%# Eval("ProductCode")%>" data-dropdown data-hover="true" data-hover-pane="true">
                                                        <div class="row">
                                                            <div class="columns small-6">
                                                                <%#GetText("Sold", Container.DataItem)%>
                                                            </div>
                                                            <div class="columns small-6">
                                                                <%# Eval("AvailabilityDetail.Sold")%>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="columns small-6">
                                                                <%# GetText("Reserved", Container.DataItem)%>
                                                            </div>
                                                            <div class="columns small-6">
                                                                <%# Eval("AvailabilityDetail.Reserved")%>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="columns small-6">
                                                                <%# GetText("Cancelled", Container.DataItem) %>
                                                            </div>
                                                            <div class="columns small-6">
                                                                <%# Eval("AvailabilityDetail.Cancelled")%>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="columns small-6">
                                                                <%# GetText("Availability", Container.DataItem) %>
                                                            </div>
                                                            <div class="columns small-6">
                                                                <%# GetText("AvailabilityMask", Container.DataItem) %>
                                                            </div>
                                                        </div>
                                                        <asp:PlaceHolder ID="plhSoldOut" runat="server" Visible='<%# Eval("AvailabilityDetail.haveSoldOutComponents")%>'>
                                                            <div class="row">
                                                                <div class="columns small-6">
                                                                    <%# GetText("SoldOutComponents", Container.DataItem) %>
                                                                </div>
                                                                <div class="columns small-6">
                                                                    <%# Eval("AvailabilityDetail.SoldOutComponents")%>
                                                                </div>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                    </div>
                                </div>                                
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="rptFixturesSeason" runat="server">
                        <ItemTemplate>
                            <div class="column c-hosp-item__column">

                                <div class="c-hosp-item__card">
                                    <a href="../../PagesPublic/Hospitality/HospitalityPackageDetails.aspx?product=<%# Eval("ProductCode")%>&packageid=<%# Request.QueryString("PackageId") %>&availabilitycomponentid=<%# Request.QueryString("availabilitycomponentid")%>">
                                        <div class="c-hosp-item__h">
                                            <h2><%# Eval("ProductDescription")%></h2>
                                        </div>
                                        <div class="c-hosp-item__img">
                                            <asp:Image ID="imgOppositionSeason" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode"))%>' />
                                        </div>
                                        <div class="c-hosp-item__info">
                                            <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductCompetitionDesc")) %>' >
                                              <div class="c-hosp-item__comp"><%# Eval("ProductCompetitionDesc")%></div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductDate")) %>'>
                                              <div class="c-hosp-item__date"><%# Eval("ProductDate")%></div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhSeasonFixtureSubType" runat="server" Visible='<%# DisplaySubType AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubType"))) AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubTypeDesc")))%>'>                                                
                                                <div class="c-hosp-item__comp"><%# Eval("ProductSubTypeDesc")%></div>                                          
                                            </asp:PlaceHolder>
                                            <%--  <div class="c-hosp-item__avail"><span class="c-hosp-item__avail--good">Good Availability</span></div>--%>                                            
                                        </div>
                                        <asp:PlaceHolder ID="plhSeasonFixtureEligibility" runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductReqdMemDesc")) %>'>
                                            <div class="c-hosp-item__prereq"><%#GetProductEligibilityText(Eval("ProductReqdMemDesc"))%></div>
                                        </asp:PlaceHolder>
                                    </a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:PlaceHolder ID="plhAddNFixturesToProductGroupFooter" runat="server" Visible='<%# If(Usage="HospitalityBundleFixtures.aspx", "True", "False") %>'>
                    <div class="alert-box info">
                        <asp:Literal ID="ltlAddNFixturesToProductGroupMessageFooter" runat="server" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>
</aside>