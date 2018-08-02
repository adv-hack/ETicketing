<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityBookingExtras.ascx.vb" Inherits="UserControls_HospitalityBookingExtras" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBookingApplyDiscount.ascx" TagName="HospitalityBookingApplyDiscount" TagPrefix="Talent" %>

<aside>
    <div class="c-hosp-bkng-ext">
        <div class="row">
            <div class="column">
                <div class="o-hosp-cont">
                    <h1>Extras</h1>
                    <div class="c-hosp-bkng-ext__ext c-hosp-bkng-ext--sng">
                        <h2>Package Extras</h2>
                        <div class="c-hosp-bkng-i">
                            <a data-open="exampleModalExtraBlurb"><i class="fa fa-info" aria-hidden="true"></i> Find out about package extras.</a>
                        </div>
                        <div class="reveal large" id="exampleModalExtraBlurb" data-reveal>
                            <div class="c-hosp-bkng-ext__blurb">
                                <h1>Lorem Ipsum</h1>
                                <img src="http://placehold.it/350x150">
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas vitae lobortis ipsum. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Suspendisse sit amet molestie est. Etiam consectetur lectus dui, ac rutrum mauris interdum non. Aliquam erat volutpat. Vestibulum nibh neque, placerat ac sollicitudin ac, semper at urna. Vestibulum fringilla arcu lacus, in tempus libero pulvinar ac. Sed rhoncus lectus mattis elementum maximus. Phasellus elementum lacus est, vitae pulvinar mi faucibus ac. Sed molestie metus eu lectus efficitur, vitae faucibus tellus vestibulum. Morbi tempus dolor at nulla laoreet rutrum. Aenean nisl ipsum, pulvinar sed orci eu, euismod volutpat augue.</p>
                                <p>Aliquam accumsan ex et metus vulputate congue. Etiam sollicitudin nec tortor eu lacinia. Aenean ullamcorper pellentesque feugiat. Aliquam erat volutpat. Nam mattis, purus ut scelerisque maximus, eros justo iaculis mi, in porttitor ipsum ante a nulla. Cras condimentum mi mi, eu maximus eros blandit vestibulum. Suspendisse in purus et risus euismod dictum ornare nec justo. Quisque hendrerit euismod arcu a viverra. Nam tristique pulvinar nisl, vitae sagittis erat suscipit nec. Phasellus vel augue sed felis varius lobortis eget sed diam. Sed sodales sed ex et accumsan. Morbi vitae neque finibus tortor aliquam hendrerit. Sed molestie malesuada dui at convallis.</p>
                            </div>
                            <button class="close-button" data-close aria-label="Close modal" type="button">
                            <i class="fa fa-times" aria-hidden="true"></i>
                          </button>
                        </div>
                        <div class="row c-hosp-bkng-i">
                            <div class="columns o-hosp-lab-col">
                                <span class="c-hosp-bkng-i__badge-cont"><span class="badge">1</span></span><span class="c-hosp-bkng-i__lab-cont"><a data-open="exampleModal2">Tea, Coffee and Morning Rolls</a> &ndash; <span class="u-hosp-disp-h">20% Discount Applied</span></span>
                            </div>
                            <div class="columns o-hosp-val-col">
                                <span class="u-hosp-disp-s">&pound;30</span> <span class="u-hosp-disp-h">&pound;24</span>
                            </div>
                            <div class="columns o-hosp-act-col">
                                <ul class="menu">
                                    <li><a data-open="exampleModal2"><i class="fa fa-pencil" aria-hidden="true"></i> Edit</a></li>
                                    <li><a href="#"><i class="fa fa-trash" aria-hidden="true"></i> Delete</a></li>
                                </ul>
                            </div>
                        </div>
                        <div class="reveal large" id="exampleModal2" data-reveal>
                          <aside>
                              <div class="o-hosp-cont">
                                  <h1>Tea, Coffee and Morning Rolls</h1>
                                  <Talent:HospitalityBookingApplyDiscount ID="HospitalityBookingApplyDiscount2" runat="server" />
                            </div>
                          </aside>
                          <button class="close-button" data-close aria-label="Close modal" type="button">
                            <i class="fa fa-times" aria-hidden="true"></i>
                          </button>
                        </div>
                        <div class="row">
                            <div class="columns c-hosp-bkng-ext__des">
                                <select id="xyz">
                                    <option value="">Complimentary Matchday Programme and Team Sheets £10.00</option>
                                    <option value="">Lorem Ipsum</option>
                                </select>
                            </div>
                            <div class="columns c-hosp-bkng-ext__qty">
                                <select id="abc">
                                    <option value="">1</option>
                                    <option value="">2</option>
                                    <option value="">3</option>
                                    <option value="">4</option>
                                </select>
                            </div>
                            <div class="columns c-hosp-bkng-ext__ad">
                                <a href="#" class="button">Add</a>
                            </div>
                        </div>
                        <div class="c-hosp-bkng-ext__ad-pak">
                            <a href="#" class="button">Add Package Extras To Your Package</a>
                        </div>
                    </div>

                    <div class="c-hosp-bkng-ext__ext c-hosp-bkng-ext--mult">
                        <h2>Car Parking</h2>
                        <div class="c-hosp-bkng-ext__i">
                            <a href="#"><i class="fa fa-info" aria-hidden="true"></i> Find out about car parking.</a>
                        </div>
                        <div class="row">
                            <div class="columns c-hosp-bkng-ext__des">
                                <span class="u-hosp-bkng-l"><a data-open="exampleModal3"><i class="fa fa-pencil" aria-hidden="true"></i> Posh Parking</a></span>
                            </div>
                            <div class="columns c-hosp-bkng-ext__pc">
                                <span class="u-hosp-bkng-l">&pound;100</span>
                            </div>
                            <div class="columns c-hosp-bkng-ext__qty">
                                <select id="abc">
                                    <option value="">0</option>
                                    <option value="">1</option>
                                    <option value="">2</option>
                                    <option value="">3</option>
                                    <option value="">4</option>
                                </select>
                            </div>
                            <div class="columns c-hosp-bkng-ext__tot">
                                <span class="u-hosp-bkng-l">&pound;0</span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="columns c-hosp-bkng-ext__des">
                                <span class="u-hosp-bkng-l"><a data-open="exampleModal3"><i class="fa fa-pencil" aria-hidden="true"></i> Approved Off-Stadium Parking &ndash; <span class="u-hosp-disp-h">15% Discount Applied</span></a></span>  
                            </div>
                            <div class="columns c-hosp-bkng-ext__pc">
                                <span class="u-hosp-bkng-l">&pound;30</span>
                            </div>
                            <div class="columns c-hosp-bkng-ext__qty">
                                <select id="abc">
                                    <option value="">0</option>
                                    <option value="" selected>1</option>
                                    <option value="">2</option>
                                    <option value="">3</option>
                                    <option value="">4</option>
                                </select>
                            </div>
                            <div class="columns c-hosp-bkng-ext__tot">
                                <span class="u-hosp-bkng-l"><span class="u-hosp-disp-s">&pound;30</span> <span class="u-hosp-disp-h">&pound;15</span></span>
                            </div>
                        </div>
                        <div class="reveal large" id="exampleModal3" data-reveal>
                          <aside>
                              <div class="o-hosp-cont">
                                  <h1>Approved Off-Stadium Parking</h1>
                                  <Talent:HospitalityBookingApplyDiscount ID="HospitalityBookingApplyDiscount1" runat="server" />
                            </div>
                          </aside>
                          <button class="close-button" data-close aria-label="Close modal" type="button">
                            <i class="fa fa-times" aria-hidden="true"></i>
                          </button>
                        </div>
                        <div class="row">
                            <div class="columns c-hosp-bkng-ext__des">
                                <span class="u-hosp-bkng-l"><a data-open="exampleModal3"><i class="fa fa-pencil" aria-hidden="true"></i> Fan Pick Up</a></span>
                            </div>
                            <div class="columns c-hosp-bkng-ext__pc">
                                <span class="u-hosp-bkng-l">&pound;240</span>
                            </div>
                            <div class="columns c-hosp-bkng-ext__qty">
                                <select id="abc">
                                    <option value="">0</option>
                                    <option value="">1</option>
                                    <option value="">2</option>
                                    <option value="">3</option>
                                    <option value="">4</option>
                                </select>
                            </div>
                            <div class="columns c-hosp-bkng-ext__tot">
                                <span class="u-hosp-bkng-l">&pound;0</span>
                            </div>
                        </div>
                        <div class="c-hosp-bkng-ext__upd">
                            <a href="#" class="button">Update Package Extras</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</aside>


