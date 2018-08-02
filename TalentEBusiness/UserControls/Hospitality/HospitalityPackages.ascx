<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityPackages.ascx.vb" Inherits="UserControls_HospitalityPackages" %>
<aside>
    <div class="c-hosp-item c-hosp-item--packages <%=DynamicLayoutClass %>">
        <div class="row">
            <asp:PlaceHolder ID="plhMessages" runat="server">
                <div class="alert-box warning">
                    <asp:Literal ID="ltlNoPackageMessage" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
            <div class="column">
                <div class="row c-hosp-item__row" data-equalizer="packages" data-equalize-on="medium">
                    <asp:Repeater ID="rptPackages" runat="server">
                        <ItemTemplate>
                            <div class="column c-hosp-item__column">
                                <div id="hlkViewCol" runat="server" class="ebiz-view">
                                    <div class="reveal" data-reveal id='view-area-<%#Eval("PackageID") %>'>
                                        <asp:Image ID="imgViewArea" runat="server" ImageUrl='<%#Eval("ViewAreaImageUrl") %>' />
                                        <button class="close-button" data-close aria-label="Close modal" type="button">
                                            <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
                                        </button>
                                    </div>
                                </div>
                                <div class="<%# If(ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode")).Contains("NOI.gif"), "c-hosp-item__card c-hosp-item__noimg", "c-hosp-item__card") %>">
                                    <asp:HyperLink ID="hplPackageHeader" runat="server">
                                        <div class="c-hosp-item__h">
                                            <h2><%# Eval("PackageDescription")%></h2>
                                        </div>

                                        <div class="c-hosp-item__body" data-equalizer-watch="packages">
                                            <div class="c-hosp-item__img">
                                                <asp:Image ID="imgPackage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODPACKAGE", Eval("PackageCode"))%>' />
                                            </div>
                                            <div class="c-hosp-item__info">
                                                <asp:PlaceHolder ID="plhProductPrice" runat="server" Visible='<%# IsPackageListByProductGroup = False %>'>
                                                    <div class="c-hosp-item__price"><%# Eval("DisplayPrice")%></div>
                                                </asp:PlaceHolder>
                                                <div class="c-hosp-item__pp"><strong><%# Eval("GroupSizeDescription")%></strong></div>

                                                <asp:PlaceHolder runat="server" Visible='<%# Not (String.IsNullOrEmpty(Eval("Comment1")) AndAlso String.IsNullOrEmpty(Eval("Comment2"))) %>'>
                                                    <div class="c-hosp-item__blurb">
                                                        <%# Eval("Comment1")%>
                                                        <%# Eval("Comment2")%>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                            <asp:PlaceHolder ID="plhSoldOutFixtures" runat="server" Visible='<%# Eval("HasSoldOutProducts")%>'>
                                                <div class="c-hosp-item__warning">
                                                    <div style="display: inline">
                                                        <%#GetText("SoldOutProductText", Container.DataItem)%>
                                                        <strong><%# Eval("SoldOutProducts")%></strong>
                                                    </div>
                                                </div>
                                            </asp:PlaceHolder>
                                        </div>
                                    </asp:HyperLink>
                                    <asp:PlaceHolder ID="plhPackageAvailability" runat="server" Visible='<%# IsPackageListByProductGroup = False %>'>
                                        <div class="c-hosp-item__footer">
                                            <div class="row">
                                                <div class="large-6 columns">
                                                    <div class="c-hosp-item__avail-container">
                                                        <span class="c-hosp-item__avail c-hosp-item__avail--<%# Eval("AvailabilityDetail.AvailabilityCSS")%>" data-toggle="reveal-<%# Eval("PackageID")%>" style="background-color: <%# Eval("AvailabilityDetail.AvailabilityColour")%>;">
                                                            <%# Eval("AvailabilityDetail.AvailabilityText")%>
                                                        </span>
                                                        <asp:PlaceHolder ID="plhAvailabilityDropDown" runat="server" Visible="<%#AgentProfile.IsAgent %>">
                                                            <div class="dropdown-pane" id="reveal-<%# Eval("PackageID")%>" data-dropdown="" data-hover="true" data-hover-pane="true">
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
                                                    <!-- /.c-hosp-item__avail-container -->
                                                </div>

                                                <div class="large-6 columns large-text-right">
                                                    <asp:PlaceHolder runat="server" Visible='<%# Eval("ShowViewFromArea")%>'>
                                                        <div class="c-hosp-item__view-from-area">
                                                            <asp:HyperLink ID='hlkView' Text='<%# ViewFromAreaText  %>' data-open='<%# "view-area-" + Eval("PackageID").ToString()%>' Visible='<%#Eval("ShowViewFromArea") %>' runat='server' />
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- /.c-hosp-item__footer -->
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</aside>

