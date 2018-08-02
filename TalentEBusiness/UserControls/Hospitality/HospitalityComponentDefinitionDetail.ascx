<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityComponentDefinitionDetail.ascx.vb" Inherits="UserControls_HospitalityComponentDefinitionDetail" %>

<aside>
    <div class="c-comp-def-deets">
        <div class="row">
            <div class="column">
                <div class="panel">
                    <h2>Add Component&hellip;</h2>
                    <div class="row">
                        <div class="columns">
                            <label>Description</label>
                            <input type="text" placeholder="Description" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns">
                            <label>Comments</label>
                            <textarea placeholder="Comments"></textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns large-4">
                            <label>Stadium Code</label>
                            <select>
                              <option value="volvo">GS</option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>Allocate Automatically</label>
                            <select>
                              <option value="volvo">1</option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>Department</label>
                            <select>
                              <option value="volvo"></option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns large-4">
                            <label>Ticket Required</label>
                            <div class="switch">
                              <input class="switch-input" id="ticket-required" type="checkbox" name="exampleSwitch">
                              <label class="switch-paddle" for="ticket-required">
                                <span class="show-for-sr">Ticket Required</span>
                                <span class="switch-active" aria-hidden="true">Yes</span>
                                <span class="switch-inactive" aria-hidden="true">No</span>
                              </label>
                            </div>
                        </div>
                        <div class="columns  large-4">
                            <label>Ticket Type</label>
                            <select>
                              <option value="volvo">00</option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>No. of Stubs</label>
                            <select>
                              <option value="volvo">0</option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns large-4">
                            <label>Default Discount</label>
                            <div class="c-slider c-slider--f c-slider--pct">
                                <div class="c-slider__slider"></div>
                                <div class="c-slider__amount"></div>
                            </div>
                        </div>
                        <div class="columns  large-4">
                            <label>VAT Code</label>
                            <select>
                              <option value="volvo">01</option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>Output Queue</label>
                            <input type="text" placeholder="Output Queue" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="columns large-4">
                            <label>Default Price Code</label>
                            <select>
                              <option value="volvo"></option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>Default Price Band</label>
                            <select>
                              <option value="volvo"></option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="columns  large-4">
                            <label>Ticket Layout</label>
                            <select>
                              <option value="volvo"></option>
                              <option value="saab">Saab</option>
                              <option value="mercedes">Mercedes</option>
                              <option value="audi">Audi</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="column large-6 c-hosp-comp-def-is__nl-c">
                            <label>N/L Code</label>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                            <input type="text" />
                            <input type="text" />
                            <input type="text" />
                            <input type="text" />
                            <input type="text" />
                        </div>
                        <div class="column large-6 c-hosp-comp-def-is__ovrd-elt">
                            <label>Override Element</label>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                            <select>
                                <option value="volvo"></option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">
                         <div class="column large-6 ">
                             <label for="jYUxQ7Es">Print On Invoice</label>
                             <div class="switch">
                              <input class="switch-input" id="print-invoice" type="checkbox" name="exampleSwitch">
                              <label class="switch-paddle" for="print-invoice">
                                <span class="show-for-sr">Ticket Required</span>
                                <span class="switch-active" aria-hidden="true">Yes</span>
                                <span class="switch-inactive" aria-hidden="true">No</span>
                              </label>
                            </div>
                        </div>
                        <div class="column large-6 ">
                            <label for="vsuBUdAY">Use Ticket Designer</label>
                            <div class="switch">
                              <input class="switch-input" id="ticket-designer" type="checkbox" name="exampleSwitch">
                              <label class="switch-paddle" for="ticket-designer">
                                <span class="show-for-sr">Ticket Required</span>
                                <span class="switch-active" aria-hidden="true">Yes</span>
                                <span class="switch-inactive" aria-hidden="true">No</span>
                              </label>
                            </div>
                        </div>
                    </div>
                    <div class="n-group">
                        <a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionList.aspx" class="button">Back</a>
                        <a href="#" class="button js-comp-def-deets__t">Add</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</aside>

<script>

    $(function(){
        $(".c-slider__slider").slider({
            value: 0,
            min: 0,
            max: 100,
            step: 1,
            slide: function (event, ui) {
                $(".c-slider__amount").text(ui.value + "%");
            }
        });
        $(".c-slider__amount").text($(".c-slider__slider").slider("value") + "%"); 
    });

    function getUrlVars() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    $(function () {
        var type = getUrlVars()["type"];
        if (type == "add") {
            $("h1").html("Components Detail &mdash; Add");
            $(".c-comp-def-deets h2").html("Add Component&hellip;");
            $(".js-comp-def-deets__t").text("Add");
        } else if (type == "edit") {
            $("h1").html("Components Detail &mdash; Edit");
            $(".c-comp-def-deets h2").html("Edit Component&hellip;");
            $(".js-comp-def-deets__t").text("Update");
        } 
    });

    $(function () {
        $("#ticket-required").change(function () {
            $(".c-hosp-comp-def-deets-ad-s").toggleClass("js-hosp-comp-def-deets-ad-s--required", this.checked).toggleClass("js-hosp-comp-def-deets-ad-s--not-required", !this.checked);
            if ($("#ticket-required").is(":checked")) {
                $(".c-hosp-comp-def-deets-ad-s__not-required").hide();
                $(".c-hosp-comp-def-deets-ad-s__required").show();
            } else {
                $(".c-hosp-comp-def-deets-ad-s__required").hide();
                $(".c-hosp-comp-def-deets-ad-s__not-required").show();
            }
        }).change()
    });

</script>

