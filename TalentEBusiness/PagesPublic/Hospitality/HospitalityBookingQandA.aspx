<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityBookingQandA.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityBookingQandA"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixturePackageHeader.ascx" TagName="HospitalityFixtureDetails" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <Talent:HospitalityFixtureDetails ID="HospitalityFixtureDetails" runat="server" />
    <div class="c-hosp-bkng-qa">
        <div class="row">
            <div class="column">
                <div class="o-hosp-cont">
                    <h1>Hospitality Questions and Answers</h1>
                    <h2>A Few Details</h2>
                    <div class="c-hosp-bkng-qa__deets-quest">
                        <input type="checkbox" id="efg" /><label for="efg">Wheelchair access please?</label>
                    </div>
                    <h2>Dietary Requirements</h2>
                    <div class="row">
                        <div class="columns o-hosp-lab-col">
                            <label for="xyz">Gluten Free</label>
                        </div>
                        <div class="columns o-hosp-val-col">
                            <select id="xyz">
                              <option value="">0</option>
                              <option value="">1</option>
                              <option value="">2</option>
                              <option value="">3</option>
                              <option value="">4</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns o-hosp-lab-col">
                            <label for="xyz">Lactose Intolerant</label>
                        </div>
                        <div class="columns o-hosp-val-col">
                            <select id="xyz">
                              <option value="">0</option>
                              <option value="">1</option>
                              <option value="">2</option>
                              <option value="">3</option>
                              <option value="">4</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns o-hosp-lab-col">
                            <label for="xyz">Vegetarian</label>
                        </div>
                        <div class="columns o-hosp-val-col">
                            <select id="xyz">
                              <option value="">0</option>
                              <option value="">1</option>
                              <option value="">2</option>
                              <option value="">3</option>
                              <option value="">4</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns o-hosp-lab-col">
                            <label for="xyz">Vegan</label>
                        </div>
                        <div class="columns o-hosp-val-col">
                            <select id="xyz">
                              <option value="">0</option>
                              <option value="">1</option>
                              <option value="">2</option>
                              <option value="">3</option>
                              <option value="">4</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns o-hosp-lab-col">
                            <label for="xyz">Gluten Free</label>
                        </div>
                        <div class="columns o-hosp-val-col">
                            <select id="xyz">
                              <option value="">0</option>
                              <option value="">1</option>
                              <option value="">2</option>
                              <option value="">3</option>
                              <option value="">4</option>
                            </select>
                        </div>
                    </div>
                    <h2>Is there anything else you would like us to know about you?</h2>
                    <div class="c-hosp-bkng-qa__cmnt">
                        <label for="exampleQWE">Comments</label>
                        <textarea placeholder="Type away...!" id="exampleQWE"></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="c-hosp-bkng-qa__btn-grp">
        <div class="button-group">
            <a href="../../PagesPublic/Hospitality/HospitalityBooking.aspx" class="button ebiz-muted-action">Back</a>
            <a data-open="exampleModalPackageActions" class="button">Save For Later</a>
            <a href="#" class="button ebiz-primary-action">Continue</a>
        </div>
    </div>
    <div class="reveal large" id="exampleModalPackageActions" data-reveal>
        <h1>Package Actions</h1>
        <ul class="tabs" data-tabs id="example-tabsX">
            <li class="tabs-title is-active c-hosp-bkng-qa__tab-tit-wf"><a href="#panel1x" aria-selected="true">Workflow</a></li>
            <li class="tabs-title c-hosp-bkng-qa__tab-tit-eml"><a href="#panel2x"> Email</a></li>
        </ul>
        <div class="tabs-content" data-tabs-content="example-tabsX">
            <div class="tabs-panel is-active" id="panel1x">
                <div class="c-hosp-bkng-qa__cmnt">
                    <label for="exampleZXC">Comments</label>
                    <textarea placeholder="Type away...!" id="exampleZXC"></textarea>
                </div>
                <fieldset class="o-radio-group c-hosp-bkng-qa__rg">
                    <legend>Choose Your Favorite</legend>
                    <div class="o-radio-group__item">
                        <input type="radio" name="pokemon" value="Red" id="pokemonRed" required><label for="pokemonRed">Reservation</label>
                    </div>
                    <div class="o-radio-group__item">
                        <input type="radio" name="pokemon" value="Blue" id="pokemonBlue"><label for="pokemonBlue">Quote</label>
                    </div>
                    <div class="o-radio-group__item">
                        <input type="radio" name="pokemon" value="Yellow" id="pokemonYellow"><label for="pokemonYellow">Enquiry</label>
                    </div>
                    <div class="o-radio-group__item">
                        <input type="radio" name="pokemon" value="Yellow" id="pokemonYellow"><label for="pokemonYellow">Sale</label>
                    </div>
                </fieldset>
                <div class="c-hosp-bkng-qa__sub">
                    <a href="#" class="button ebiz-primary-action">Submit</a>
                </div>
            </div>
            <div class="tabs-panel" id="panel2x">
                <div class="row c-hosp-bkng-fm__row">
                    <div class="columns c-hosp-bkng-fm__lab-col">
                        <label for="exampleVBN">Email Address</label>
                    </div>
                    <div class="columns c-hosp-bkng-fm__inp-col">
                        <input type="email" placeholder="Email Address" id="exampleVBN" />
                    </div>
                </div>
                <div class="c-hosp-bkng-qa__msg">
                    <label for="exampleIOP">Message</label>
                    <textarea placeholder="Type away...!" id="exampleIOP"></textarea>
                </div>
                <div class="button-group">
                    <a href="#" class="button">View PDF</a>
                    <a href="#" class="button ebiz-primary-action">Send</a>
                </div>
            </div>
        </div>
        <button class="close-button" data-close aria-label="Close modal" type="button">
            <i class="fa fa-times" aria-hidden="true"></i>
        </button>
    </div>
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>

