<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityComponentDefinitionList.ascx.vb" Inherits="UserControls_HospitalityComponentDefinitionList" %>

<aside>
    <div class="c-dat-tbl-act"">
        <div class="row">
            <div class="column">
                <a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=add" class="button">Add New Component</a>
                <div class="panel">
                    <div class="c-dat-tbl-act__blk">
                        <div class="row">
                            <div class="column">
                                <select>
                                    <option>Bulk Actions</option>
                                    <option>Edit</option>
                                    <option>Move To Trash</option>
                                </select>
                                <a class="button js-c-dat-tbl-act__blk-act">Apply</a>
                            </div>
                        </div>
                    </div>
                    <div class="c-dat-tbl-act__blk-ed js-dat-tbl-act__blk-ed">
                        <h2>Bulk Edit</h2>
                        <div class="row">
                            <div class="columns c-dat-tbl-act__blk-ed-tit">
                                <ul class="no-bullet">
                                    <li><a href="#"><i class="fa fa-times" aria-hidden="true"></i></a> Unlimted Alcohol After The Match</li>
                                    <li><a href="#"><i class="fa fa-times" aria-hidden="true"></i></a> Stadium Parking</li>
                                    <li><a href="#"><i class="fa fa-times" aria-hidden="true"></i></a> Celebration Cake</li>
                                </ul>
                            </div>
                            <div class="columns c-dat-tbl-act__blk-ed-opt">
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>Stadium Code</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <select>
                                            <option>CS</option>
                                            <option>AB</option>
                                            <option>CD</option>
                                            <option>EF</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>Ticket Required</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <select>
                                            <option>No</option>
                                            <option>Yes</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>Default Discount</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <div class="c-slider c-slider--f c-slider--pct">
                                            <div class="c-slider__slider"></div>
                                            <div class="c-slider__amount"></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>Ticket Type</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <select>
                                            <option>00</option>
                                            <option>01</option>
                                            <option>02</option>
                                            <option>03</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>VAT Code</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <select>
                                            <option>01</option>
                                            <option>02</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-lab">
                                        <label>No. of Stubs</label>
                                    </div>
                                    <div class="columns c-dat-tbl-act__blk-ed-opt-opt">
                                        <select>
                                            <option>0</option>
                                            <option>1</option>
                                            <option>2</option>
                                            <option>3</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <table id="component_id" class="display c-dat-tbl-act__tbl js-dat-tbl-act__tbl">
                        <thead>
                            <tr>
                                <th class="no-sort">
                                    <input type="checkbox" name="component" value="all"></th>
                                <th>Description</th>
                                <th>Component</th>
                                <th>Seat Based</th>
                                <th>Group Based</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="a"></td>
                                <td>
                                    <a href="#" class="c-dat-tbl-act__h">Unlimted Alcohol After The Match</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>ALCOHOL</td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                                <td data-order="yes"><i class="fa fa-check" aria-hidden="true"></i></td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="b"></td>
                                <td><a href="#" class="c-dat-tbl-act__h">Stadium Parking</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>CAR PARK</td>
                                <td data-order="yes"><i class="fa fa-check" aria-hidden="true"></i></td>
                                <td data-order="yes"><i class="fa fa-check" aria-hidden="true"></i></td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="c"></td>
                                <td><a href="#" class="c-dat-tbl-act__h">Celebration Cake</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>CAKE</td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="d"></td>
                                <td><a href="#" class="c-dat-tbl-act__h">All You Can Eat Buffet</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>BUFFET
                                </td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="e"></td>
                                <td><a href="#" class="c-dat-tbl-act__h">Tray of Complimentary Teas, Coffee, Biscuits and Mints</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>COFFEE</td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="checkbox" name="component" value="f"></td>
                                <td><a href="#" class="c-dat-tbl-act__h">Programme</a>
                                    <div class="c-dat-tbl-act__ed">
                                        <ul class="menu">
                                            <li><a href="../../PagesPublic/Hospitality/HospitalityComponentDefinitionDetail.aspx?type=edit"><i class="fa fa-pencil" aria-hidden="true"></i>Edit</a></li>
                                            <li><a class="c-dat-tbl-act__ed-quick"><i class="fa fa-pencil" aria-hidden="true"></i>Quick Edit</a></li>
                                            <li><a href="#"><i class="fa fa-trash-o" aria-hidden="true"></i>Trash</a></li>
                                        </ul>
                                    </div>
                                </td>
                                <td>PROGRAMME</td>
                                <td data-order="no"><i class="fa fa-times" aria-hidden="true"></i></td>
                                <td data-order="yes"><i class="fa fa-check" aria-hidden="true"></i></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</aside>

<script>

    // https://datatables.net/examples/api/row_details.html

    function format(d) {
        // `d` is the original data object for the row
        return '<div class="panel">' +
            '<div class="row">' +
            '<div class="columns">' +
            '<label>Description</label>' +
            '<input type="text" placeholder="Description" />' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="columns">' +
            '<label>Comments</label>' +
            '<textarea placeholder="Comments"></textarea>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="columns large-4">' +
            '<label>Stadium Code</label>' +
            '<select>' +
            '<option value="volvo">GS</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Allocate Automatically</label>' +
            '<select>' +
            '<option value="volvo">1</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Department</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="columns large-4">' +
            '<label>Ticket Required</label>' +
            '<select>' +
            '<option value="volvo">No</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Ticket Type</label>' +
            '<select>' +
            '<option value="volvo">00</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>No. of Stubs</label>' +
            '<select>' +
            '<option value="volvo">0</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="columns large-4">' +
            '<label>Default Discount</label>' +
            '<div class="c-slider c-slider--f c-slider--pct">' +
            '<div class="c-slider__slider"></div>' +
            '<div class="c-slider__amount"></div>' +
            '</div>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>VAT Code</label>' +
            '<select>' +
            '<option value="volvo">01</option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Output Queue</label>' +
            '<input type="text" placeholder="Output Queue" />' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="columns large-4">' +
            '<label>Default Price Code</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Default Price Band</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '<div class="columns  large-4">' +
            '<label>Ticket Layout</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="column large-6 c-hosp-comp-def-is__nl-c">' +
            '<label>N/L Code</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '<input type="text" />' +
            '<input type="text" />' +
            '<input type="text" />' +
            '<input type="text" />' +
            '<input type="text" />' +
            '</div>' +
            '<div class="column large-6 c-hosp-comp-def-is__ovrd-elt">' +
            '<label>Override Element</label>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '<select>' +
            '<option value="volvo"></option>' +
            '<option value="saab">Saab</option>' +
            '<option value="mercedes">Mercedes</option>' +
            '<option value="audi">Audi</option>' +
            '</select>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="column large-6 ">' +
            '<input type="checkbox" name="vehicle" value="Bike" id="jYUxQ7Es">' +
            '<label for="jYUxQ7Es">Print On Invoice</label>' +
            '</div>' +
            '<div class="column large-6 ">' +
            '<input type="checkbox" name="vehicle" value="Bike" id="vsuBUdAY">' +
            '<label for="vsuBUdAY">Use Ticket Designer</label>' +
            '</div>' +
            '</div>' +
            '<div class="button-group">' +
            '<a href="#" class="button">Cancel</a>' +
            '<a href="#" class="button">Update</a>' +
            '</div>' +
            '</div>';
    }

    $(document).ready(function () {

        var table =  $('#component_id').DataTable({
            "columnDefs": [{
                "targets": 'no-sort',
                "orderable": false,
                "order": []
            }]
        });

        $('#component_id').on('click', '.c-dat-tbl-act__ed-quick', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child(format(row.data())).show();
                tr.addClass('shown');
                reload_slider();
            }
        });

        $(".js-dat-tbl-act__tbl tbody tr").hover(function () {
            $(this).find(".c-dat-tbl-act__ed").css("visibility", "visible");
        }, function () {
            $(this).find(".c-dat-tbl-act__ed").css("visibility", "hidden");
            });

        $(".js-c-dat-tbl-act__blk-act").click(function () {
            $(".js-dat-tbl-act__blk-ed").toggle();
        });

        $(".c-dat-tbl-act__blk-ed-tit ul").css("height", $(".c-dat-tbl-act__blk-ed-opt").innerHeight());
        $(".js-dat-tbl-act__blk-ed").hide();

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

    function reload_slider() {
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
    }

</script>

