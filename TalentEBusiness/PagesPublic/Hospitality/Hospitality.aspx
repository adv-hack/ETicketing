<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="Hospitality.aspx.vb" Inherits="PagesPublic_Hospitality_Hospitality" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>



<asp:content id="ContentHead" contentplaceholderid="ContentPlaceHolder2" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.isotope/3.0.2/isotope.pkgd.min.js"></script>
</asp:content>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" /> 

    <aside>
        <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrorMessages" runat="server"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhResults" runat="server">
            <div class="hosp-isotope">
                <div class="row">
                    <div class="column">

                      <div class="panel hosp-isotope__panel">

                        <asp:HiddenField ClientIDMode="Static" ID="hdfSeasonProductCount" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMinDate" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMaxDate" runat="server" />            
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMinGroupSizeHome" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMaxGroupSizeHome" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMinGroupSizeSeason" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMaxGroupSizeSeason" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMinBudgetHome" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMaxBudgetHome" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMinBudgetSeason" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfMaxBudgetSeason" runat="server" />
                        <asp:HiddenField runat="server" ID="hdfCurrencySymbol" ClientIDMode="Static" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfFilterGroupType" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfFilterGroupsubType" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfFilterGroupCompetition" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfFilterGroupOpposition" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeGroupMin" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeGroupMax" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeBudgetMin" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeBudgetMax" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeDateMin" runat="server" />
                        <asp:HiddenField ClientIDMode="Static" ID="hdfRangeDateMax" runat="server" />

                        <div class="row c-hosp-isotope__btn-grp-container">

                          <!-- -->
                          <div class="columns c-hosp-isotope__fixs-pak-fltr-cont">
                            <div class="button-group c-hosp-isotope__btn-grp" role="group" data-filter-group="type">                        
                              <a class="button c-hosp-isotope__btn-fltr c-hosp-isotope__btn-fltr-fixs is-checked" data-filter=".c-hosp-isotope__fixs__H">
                                 <asp:Literal ID="ltlFixtureTab" runat="server" />
                              </a>
                              <a class="button c-hosp-isotope__btn-fltr c-hosp-isotope__btn-fltr-pak" data-filter=".c-hosp-isotope__pak">
                                <asp:Literal ID="ltlPackagesTab" runat="server" />
                              </a>
                              <a class="button c-hosp-isotope__btn-fltr c-hosp-isotope__btn-fltr-bundles" data-filter=".c-hosp-isotope__bundles">
                                <asp:Literal ID="ltlProductGroupTab" runat="server" />
                              </a>
                              <a class="button c-hosp-isotope__btn-fltr c-hosp-isotope__btn-fltr-fixsSeason" data-filter=".c-hosp-isotope__fixs__S">
                                <asp:Literal ID="ltlFixSeasonTab" runat="server" />    
                              </a>
                              <a class="button c-hosp-isotope__btn-fltr c-hosp-isotope__btn-fltr-pakSeason" data-filter=".c-hosp-isotope__pak__S">
                                <asp:Literal ID="ltlPakSeasonTab" runat="server" />
                              </a>
                            </div>
                          </div>

                          <!-- Sub Type Filter -->
                          <asp:PlaceHolder ID="plhSubtype" runat="server">
                            <div class="columns c-hosp-isotope__subtype-fltr-cont">
                              <div class="c-hosp-isotope__subtype-btn-grp-cont">
                                <div class="c-hosp-isotope__fltr-h">
                                  <asp:Literal ID="ltlSubType" runat="server" />
                                </div>
                                <div class="button-group c-hosp-isotope__btn-grp" role="group" data-filter-group="subtype">
                                  <a class="button c-hosp-isotope__btn-fltr is-checked" id="subTypeAny" data-filter="">
                                    <asp:Literal ID="ltlAnySubTypeTabA" runat="server" />
                                  </a>
                                  <asp:Repeater ID="rptSubTypeA" runat="server">
                                    <ItemTemplate> 
                                      <a class="button c-hosp-isotope__btn-fltr" data-filter=".c-hosp-isotope-subtype__<%# Eval("ProductSubType")%>"><%# Eval("ProductSubTypeDesc")%></a>
                                    </ItemTemplate>
                                  </asp:Repeater>
                                </div>
                              </div>
                              <select class="c-hosp-isotope__fltr" data-filter-group="subtype">
                                <option value="">
                                  <asp:Literal ID="ltlAnySubTypeTabB" runat="server" />
                                </option>
                                <asp:Repeater ID="rptSubTypeB" runat="server">
                                  <ItemTemplate> 
                                    <option value=".c-hosp-isotope-subtype__<%# Eval("ProductSubType")%>"><%# Eval("ProductSubTypeDesc")%></option>
                                  </ItemTemplate>
                                </asp:Repeater>
                              </select>
                            </div>
                          </asp:PlaceHolder>
                              
                          <!-- Competition Filter -->
                          <asp:PlaceHolder ID="plhCompetition" runat="server">
                            <div class="columns c-hosp-isotope__comp-fltr-cont">
                              <div class="c-hosp-isotope__comp-btn-grp-cont">
                                <div class="c-hosp-isotope__fltr-h">
                                  <asp:Literal ID="ltlCompetition" runat="server" />
                                </div>
                                <div class="button-group c-hosp-isotope__btn-grp" role="group" data-filter-group="competition">
                                  <a class="button c-hosp-isotope__btn-fltr is-checked" id="competitionAny" data-filter="">
                                    <asp:Literal ID="ltlAnyCompetitionTabA" runat="server" />
                                  </a>
                                  <asp:Repeater ID="rptCompetitionA" runat="server">
                                    <ItemTemplate> 
                                      <a class="button c-hosp-isotope__btn-fltr" data-filter=".c-hosp-isotope-comp__<%# Eval("ProductCompetitionCode")%>"><%# Eval("ProductCompetitionDesc")%></a>
                                    </ItemTemplate>
                                  </asp:Repeater>
                                </div>
                              </div>
                              <select class="c-hosp-isotope__fltr" data-filter-group="competition">
                                <option value="">
                                  <asp:Literal ID="ltlAnyCompetitionTabB" runat="server" />
                                </option>
                                <asp:Repeater ID="rptCompetitionB" runat="server">
                                  <ItemTemplate> 
                                    <option value=".c-hosp-isotope-comp__<%# Eval("ProductCompetitionCode")%>"><%# Eval("ProductCompetitionDesc")%></option>
                                  </ItemTemplate>
                                </asp:Repeater>
                              </select>
                            </div>
                          </asp:PlaceHolder>
                              
                          <!-- Opposition Filter -->
                          <asp:PlaceHolder ID="plhOpposition" runat="server">
                            <div class="columns c-hosp-isotope__opp-fltr-cont">
                              <div class="c-hosp-isotope__opp-btn-grp-cont">
                                <div class="c-hosp-isotope__fltr-h">Opposition</div>
                                <div class="button-group c-hosp-isotope__btn-grp" role="group" data-filter-group="opposition">
                                  <a class="button c-hosp-isotope__btn-fltr is-checked" id="oppositionAny" data-filter="">
                                    <asp:Literal ID="ltlAnyOpposition" runat="server" />
                                  </a>
                                  <asp:Repeater ID="rptOppositionFilterA" runat="server">
                                    <ItemTemplate>
                                      <a class="button c-hosp-isotope__btn-fltr" data-filter=".c-hosp-isotope-oppo__<%# Eval("ProductOppositionCode")%>"><%# Eval("ProductOppositionDesc")%></a>
                                    </ItemTemplate>
                                  </asp:Repeater>
                                </div>
                              </div>
                              <select class="c-hosp-isotope__fltr" data-filter-group="opposition">
                                <option value="">
                                  <asp:Literal ID="ltlAnyOppositionTab" runat="server" />
                                </option>
                                <asp:Repeater ID="rptOppositionFilterB" runat="server">
                                  <ItemTemplate>
                                    <option value=".c-hosp-isotope-oppo__<%# Eval("ProductOppositionCode")%>"><%# Eval("ProductOppositionDesc")%></option>
                                  </ItemTemplate>
                                </asp:Repeater>
                              </select>
                            </div>
                          </asp:PlaceHolder>

                        </div><!-- /.c-hosp-isotope__btn-grp-container -->
                              
                        <!-- Package Sliders -->
                        <div class="c-hosp-isotope__pak-sliders row">

                          <!-- Group Size Filter -->
                          <asp:PlaceHolder ID="plhGroupsize" runat="server">
                            <div class="columns medium-6">
                              <div class="c-hosp-isotope__slider-cont">
                                <div class="row">
                                  <div class="columns c-hosp-isotope__slider-lab">
                                    <span class="c-hosp-isotope__slider-lab-group"><asp:Literal id="ltlGroupSliderLabel" runat="server" /></span>
                                  </div>
                                  <div class="columns c-hosp-isotope__slider-val" id="filter-group-amount"></div>
                                </div>
                                <div class="c-hosp-isotope__slider-inner-cont">
                                  <div id="filter-group" data-filter-group="group" class="c-hosp-isotope__slider"></div>
                                </div>
                              </div>
                            </div>
                          </asp:PlaceHolder>

                          <!-- Budget Filter -->
                          <asp:PlaceHolder ID="plhBudget" runat="server">
                            <div class="columns medium-6">
                              <div class="c-hosp-isotope__slider-cont">
                                <div class="row">
                                  <div class="columns c-hosp-isotope__slider-lab">
                                    <span class="c-hosp-isotope__slider-lab-budget"><asp:Literal id="ltlBudgetSliderLabel" runat="server" /></span>
                                  </div>
                                  <div class="columns c-hosp-isotope__slider-val" id="filter-budget-amount"></div>
                                </div>
                                <div class="c-hosp-isotope__slider-inner-cont">
                                  <div id="filter-budget" data-filter-group="budget" class="c-hosp-isotope__slider"></div>
                                </div>
                              </div>
                            </div>
                          </asp:PlaceHolder>  

                        </div><!-- /.c-hosp-isotope__pak-sliders -->

                        <!-- Date Filter -->
                        <asp:PlaceHolder ID="plhDate" runat="server">
                          <div class="row c-hosp-isotope__fixs-slider">
                            <div class="columns">                    
                              <div class="c-hosp-isotope__slider-cont">                        
                                <div class="row">
                                  <div class="columns c-hosp-isotope__slider-lab">
                                    <span class="c-hosp-isotope__slider-lab-date"><asp:Literal id="ltlDateSliderLabel" runat="server" /></span>
                                  </div>
                                  <div class="columns c-hosp-isotope__slider-val" id="filter-date-amount"></div>
                                </div>
                                <div class="c-hosp-isotope__slider-inner-cont">
                                 <div id="filter-date" data-filter-group="dateInNumbers" class="c-hosp-isotope__slider"></div>
                               </div>
                              </div>
                            </div>
                          </div>
                        </asp:PlaceHolder>
                      
                        <!-- Reset Option -->
                        <div class="row">
                          <div class="columns">
                            <a class="button ebiz-muted-action c-hosp-isotope__btn-rset">
                              <asp:Literal id="ltlResetOptionText" runat="server" />
                            </a>
                          </div>
                        </div>

                      </div><!-- /.hosp-isotope__panel -->


                      <%-- Isotope error and info messages --%>

                      <div class="alert-box alert c-hosp-isotope__no-results" style="display:none">
                        <asp:Literal ID="ltlNoResults" runat="server" />
                      </div>

                      <div class="alert-box info c-hosp-isotope__btn-fltr-fixs" style="display:none">
                        <asp:Literal id="ltlNoFixtures" runat="server" />
                      </div>

                      <div class="alert-box info c-hosp-isotope__btn-fltr-fixsSeason" style="display:none">
                        <asp:Literal id="ltlNoSeason" runat="server" />
                      </div>

                      <div class="alert-box info c-hosp-isotope__btn-fltr-pak" style="display:none">
                        <asp:Literal id="ltlNoPackages" runat="server" />
                      </div>

                      <div class="alert-box info c-hosp-isotope__btn-fltr-pakSeason" style="display:none">
                        <asp:Literal id="ltlNoPackagesSeason" runat="server"/>
                      </div>

                        <div class="alert-box info c-hosp-isotope__btn-fltr-bundles" style="display:none">
                        <asp:Literal id="ltlNoProductGroup" runat="server"/>
                      </div>



                      <!-- Isotope for results -->
                      <div class="row c-hosp-isotope__g c-hosp-item__row c-hosp-item--isotope">              
                        <!-- Fixtures Home -->
                        <asp:Repeater ID="rptFixturesHome" runat="server">
                          <ItemTemplate> 
                            <div class="column c-hosp-item__column c-hosp-isotope__i c-hosp-isotope__fixs__<%# Eval("ProductType")%> c-hosp-isotope-subtype__<%# Eval("ProductSubType")%> c-hosp-isotope-comp__<%# Eval("ProductCompetitionCode")%> c-hosp-isotope-oppo__<%# Eval("ProductOppositionCode")%>" data-budget="" data-group="" data-date="<%# Eval("ProductDateSearch")%>">  
                                <div class="<%# If(ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">                    
                                <a href="../../PagesPublic/Hospitality/HospitalityFixturePackages.aspx?product=<%# Eval("ProductCode")%>">
                                <div class="c-hosp-item__h">                                    
                                  <h2><%# Eval("ProductDescription")%></h2>
                                </div>
                                <div class="c-hosp-item__img-info-container">
                                  <div class="c-hosp-item__img">
                                    <asp:Image ID="imgOppositionHome" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode"))%>' />
                                  </div>
                                  <div class="c-hosp-item__info">
                                    <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductCompetitionDesc")) %>'>
                                      <div class="c-hosp-item__comp"><%# Eval("ProductCompetitionDesc")%></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhFixtureHomeSubType" runat="server" Visible='<%# ShowSubType AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubType"))) AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubTypeDesc"))) %>'>
                                      <div class="c-hosp-item__subtype"><%# Eval("ProductSubTypeDesc") %></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhProductDate" runat="server" Visible='<%# Not Eval("HideDate")%>'>
                                      <div class="c-hosp-item__date"><%# Eval("ProductDate")%></div>
                                    </asp:PlaceHolder>
                                    <%--<div class="c-hosp-item__avail"><span class="c-hosp-item__avail--good">Good Availability</span></div>--%>
                                    
                                  </div>
                                  <asp:PlaceHolder ID="plhHomeFixtureEligibility" runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductReqdMemDesc")) %>'>
                                    <div class="c-hosp-item__prereq"><%#GetProductEligibilityText(Eval("ProductReqdMemDesc"))%></div>   
                                  </asp:PlaceHolder>                    
                                </div>
                                </a>
                              </div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>

                        <!-- Fixtures Season -->
                        <asp:Repeater ID="rptFixturesSeason" runat="server">
                          <ItemTemplate> 
                            <div class="column c-hosp-item__column c-hosp-isotope__i c-hosp-isotope__fixs__<%# Eval("ProductType")%> " data-budget="" data-group="" data-date="">
                              <div class="<%# If(ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">
                                <a href="../../PagesPublic/Hospitality/HospitalityFixturePackages.aspx?product=<%# Eval("ProductCode")%>">
                                  <div class="c-hosp-item__h">                                    
                                  <h2><%# Eval("ProductDescription")%></h2>
                                </div>
                                <div class="c-hosp-item__img-info-container">
                                  <div class="c-hosp-item__img">
                                    <asp:Image ID="imgOppositionSeason" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Eval("ProductOppositionCode"))%>' />
                                  </div>
                                  <div class="c-hosp-item__info">
                                    <asp:PlaceHolder runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductCompetitionDesc")) %>'>
                                      <div class="c-hosp-item__comp"><%# Eval("ProductCompetitionDesc")%></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhFixtureSeasonSubType" runat="server" Visible='<%# ShowSubType AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubType"))) AndAlso (Not String.IsNullOrEmpty(Eval("ProductSubTypeDesc"))) %>'>
                                      <div class="c-hosp-item__subtype"><%# Eval("ProductSubTypeDesc") %></div>
                                    </asp:PlaceHolder>
                                    <%--<div class="c-hosp-item__date"><%# Eval("ProductDate")%></div>--%>
                                    <%--<div class="c-hosp-item__avail"><span class="c-hosp-item__avail--good">Good Availability</span></div>--%>
                                  </div>
                                  <asp:PlaceHolder ID="plhSeasonFixtureEligibility" runat="server" Visible='<%# Not String.IsNullOrEmpty(Eval("ProductReqdMemDesc")) %>'>
                                    <div class="c-hosp-item__prereq"><%#GetProductEligibilityText(Eval("ProductReqdMemDesc"))%></div>
                                  </asp:PlaceHolder>
                                </div>
                                </a>
                              </div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>
                      

                        <!-- Packages Fixtures -->
                        <asp:Repeater ID="rptPackages" runat="server">
                          <ItemTemplate>    
                            <div class="column c-hosp-item__column c-hosp-isotope__i c-hosp-isotope__pak <%# String.Format(Eval("OppositionCodesString"), "c-hosp-isotope__")%> <%# String.Format(Eval("SubTypeCodesString"), "c-hosp-isotope__")%> <%# String.Format(Eval("CompetitionCodesString"), "c-hosp-isotope__") %>" data-budget="<%# Eval("Price")%>" data-group="<%# Eval("GroupSize")%>" data-date="">
                             <div class="<%# If(ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">
                                <a href="../../PagesPublic/Hospitality/HospitalityPackageDetails.aspx?packageid=<%# Server.UrlEncode(Eval("PackageID"))%>&producttype=H&availabilitycomponentid=<%# Server.UrlEncode(Eval("AvailabilityComponentID"))%>">
                                  <div class="c-hosp-item__h">
                                    <h2><%# Eval("PackageDescription")%></h2>
                                  </div>
                                  <div class="c-hosp-item__img-info-container">
                                    <div class="c-hosp-item__img">
                                     <asp:Image ID="imgPackage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode"))%>' />
                                    </div>
                                    <div class="c-hosp-item__info">
                                      <div class="c-hosp-item__price"><%# Eval("DisplayPrice")%></div>
                                      <div class="c-hosp-item__pp"><%# Eval("GroupSizeDescription")%></div>
                                      <asp:PlaceHolder runat="server" Visible='<%# Not (String.IsNullOrEmpty(Eval("Comment1")) AndAlso String.IsNullOrEmpty(Eval("Comment2")))%>'>
                                        <div class="c-hosp-item__blurb"><%# Eval("Comment1")%><%# Eval("Comment2")%></div>
                                      </asp:PlaceHolder>
                                      <%--<div class="c-hosp-item__avail"><span class="c-hosp-item__avail--good">Good Availability</span></div>--%>
                                    </div>
                                  </div>
          						          </a>
                              </div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>

                        <!-- Packages Season -->
                        <asp:Repeater ID="rptPackagesSeason" runat="server">
                          <ItemTemplate>    
                            <div class="column c-hosp-item__column c-hosp-isotope__i c-hosp-isotope__pak__S" data-budget="<%# Eval("Price")%>" data-group="<%# Eval("GroupSize")%>" data-date="">
                              <div class="<%# If(ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">
                                <asp:HyperLink ID="hplSeasonPackages" runat="server">
                                  <div class="c-hosp-item__h">
                                  <h2><%# Eval("PackageDescription")%></h2>
                                </div>
                                <div class="c-hosp-item__img-info-container">
                                  <div class="c-hosp-item__img">
                                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode"))%>' />
                                  </div>
                                  <div class="c-hosp-item__info">
                                    <div class="c-hosp-item__price"><%# Eval("DisplayPrice")%></div>
                                    <div class="c-hosp-item__pp"><%# Eval("GroupSizeDescription")%></div>
                                    <asp:PlaceHolder runat="server" Visible='<%# Not (String.IsNullOrEmpty(Eval("Comment1")) AndAlso String.IsNullOrEmpty(Eval("Comment2")))%>'>
                                      <div class="c-hosp-item__blurb"><%# Eval("Comment1")%><%# Eval("Comment2")%></div>
                                    </asp:PlaceHolder>
                                    <%--<div class="c-hosp-item__avail"><span class="c-hosp-item__avail--good">Good Availability</span></div>--%>
                                  </div>
                                </div>
                                </asp:HyperLink>
                              </div>
                            </div>
                          </ItemTemplate>
                        </asp:Repeater>

                        <!-- Product Groups -->
                        <asp:Repeater ID="rptProductGroups" runat="server" OnItemDataBound="rptProductGroups_ItemDataBound">
                            <ItemTemplate>
                                <div class="column c-hosp-item__column c-hosp-isotope__i c-hosp-isotope__bundles c-hosp-isotope-subtype__HOME" data-budget="" data-group="" data-date="">
                                    <div class="<%# If(ProductDetail.GetImageURL("PRODGROUP", Eval("GroupId")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">
                                        <asp:HyperLink ID="hplProductGroups" runat="server">
                                            <div class="c-hosp-item__h">
                                                <h2><%# Eval("GroupDescription")%></h2>
                                            </div>
                                            <div class="c-hosp-item__img-info-container">
                                                <div class="c-hosp-item__img">
                                                    <asp:Image ID="imgProductGroup" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODGROUP", Eval("GroupId"))%>' />
                                                </div>                          
                                                <div class="c-hosp-item__info" runat="server">
                                                    <div class="c-hosp-item__bundle-fixtures">
                                                        <asp:Literal id="ltlProductGrpFixturesAvailableMsg" runat="server" />
                                                        <ul class="c-hosp-item__bundle-fixtures-list">
                                                            <asp:Repeater ID="rptProductGroupFixtures" runat="server">                                                        
                                                                <ItemTemplate>                                                                
                                                                    <li><%# Eval("ProductDescription")%></li>                                                               
                                                                </ItemTemplate>
                                                            </asp:Repeater>    
                                                        </ul>   
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                      </div><!-- ./c-hosp-isotope__g -->

                    </div><!-- /.column -->
                </div><!-- /.row -->
            </div><!-- /.hosp-isotope -->
        </asp:PlaceHolder>   
    </aside>
    
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:content>
