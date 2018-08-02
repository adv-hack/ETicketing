var seatAvailableFill;
var seatAvailableStroke;
var seatBasketFill;
var seatBasketStroke;
var seatRestrictedFill;
var seatRestrictedStroke;
var seatUnavailableFill;
var seatUnavailableStroke;
var seatUnavailable2Stroke;
var seatUnAvailable2Fill;
var seatDisabledStroke;
var seatDisabledFill;
var seatReservedStroke;
var seatReservedFill;
var seatTransferableFill;
var seatTransferableStroke;
var seatExceptionFill;
var seatExceptionStroke;
var seatTicketExchangeFill;
var seatTicketExchangeStroke;
var seatPartialSeasonTicketFill;
var seatPartialSeasonTicketStroke;

var seatingAreaCanvas;
var seatingAreaXML;
var tipText = "";
var seatInfo;
var basket = [];
var multiSelect = new Array();
var multiSelectNo = 0;
var multiSelectOn = false;

var SeatTipAreaCode;
var SeatTipStandCode;
var SeatTipSeatRow;
var SeatTipNumber;
var SeatTipPrice;
var gTransfer = false;
var miniSeatingAreaCanvas;
var start;
var end;
var seatingAreaStandDesc;
var seatingAreaAreaDesc;
var displayedStandAndAreaBasket = [];

var restrictionCode;
var restrictionDesc;
var reservationCode;
var reservationDesc;
var priceBreaks = [];
var seatingEndTime;
var seatingStartTime;
var ticketExchangeAndSilding = false;

/*Stand code: Responsible for seating area creation and functionality...   */
// Similar purpose to onload - although it primarily retrieves seating data and draws the seating area
function createSeatSelection(standAndAreaCode, standAndAreaOptions) {
    var tempString;
    var seatAjaxUrl;
    var hdfDescriptionsXML = document.getElementById("hdfDescriptionsXML").value;
    var ticketExchangeMin;
    var ticketExchangeMax;
    var packageId = GetParameterByName("packageId");
    var componentId = GetParameterByName("componentId");
    var changeAllSeats = GetParameterByName("changeallseats");

    // removes canvase
    if (seatingAreaCanvas) {
        seatingAreaCanvas.remove();
    }
    stand = standAndAreaCode;
    tempString = standAndAreaCode.split("-");
    seatingAreaCanvas = Raphael('select', seatingAreaCanvasWidth, seatingAreaCanvasHeight);
    var hdfDescriptionsXML = document.getElementById("hdfDescriptionsXML").value;    // retieving stand and area descriptions for seat tool tip
    $(hdfDescriptionsXML).find('saa').each(function () {
        var $entry = $(this);
        var standCode = $entry.attr('sc');
        var standDescription = $entry.attr('sd');
        var areaCode = $entry.attr('ac');
        var areaDescription = $entry.attr('ad');

        if (standCode == tempString[0])
            seatingAreaStandDesc = standDescription;

        if (areaCode == tempString[1])
            seatingAreaAreaDesc = areaDescription;
    });
    
    if (isSeatSelectionOnly == true){
        seatAjaxUrl = "seatSelection.aspx/GetSeating";
        standAndAreaOptions = {
            SelectedMinimumPrice: GetParameterByName("selectedMinimumPrice"),
            SelectedMaximumPrice: GetParameterByName("selectedMaximumPrice"),
            SelectedPriceBreak: GetParameterByName("priceBreakId"),
            SelectedStand: tempString[0],
            SelectedArea: tempString[1],
            RefreshMinimumPriceList: true,
            RefreshMaximumPriceList: true,
            RefreshAreaList: true,
            RefreshStandList: true,
            RefreshPriceBreakList: true,
            IncludeTicketExchangeSeats: false,
            NewAreaForSeats: true
        }
    } else {
        seatAjaxUrl = "VisualSeatSelection.aspx/GetSeating";
        if (standAndAreaOptions == undefined) {
            standAndAreaOptions = GetAPISettingsFromStandAndAreaOptions(false, false, false, false, false, true, false);
        }
    }
    if (standAndAreaOptions.IncludeTicketExchangeSeats == true) {
        ticketExchangeMin = $('#slider-handle-ticket-exchange-min').val();
        ticketExchangeMax = $('#slider-handle-ticket-exchange-max').val();
        if (ticketExchangeAndSilding)
            seatingStartTime = new Date().getTime();
    }
    //retrieves XML doc via AJAX
    $("#loading-image").show();
    $.ajax({
        type: "POST",
        url: seatAjaxUrl,
        data: '{data: "' + standAndAreaCode + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","campaignCode":"' + campaignCode + '","callId":"' + callId + '","currentExceptionSeat":"' + currentExceptionSeat + '","priceBreakId":"' + standAndAreaOptions.SelectedPriceBreak +
            '","includeTicketExchangeSeats":"' + standAndAreaOptions.IncludeTicketExchangeSeats + '","selectedMinimumPrice":"' + standAndAreaOptions.SelectedMinimumPrice + '","selectedMaximumPrice":"' + standAndAreaOptions.SelectedMaximumPrice + '","ticketExchangeMin":"' + ticketExchangeMin + '","ticketExchangeMax":"' + ticketExchangeMax + '","packageId":"' + packageId + '","componentId":"' + componentId + '","changeAllSeats":"' + changeAllSeats + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (msg) {
            // seating area is created with retrieved xml document
            createSeatingArea(msg.d);
            createMiniSeatingArea(msg.d);
            $("#loading-image").hide();
            if (standAndAreaOptions.IncludeTicketExchangeSeats == true) {
                if (ticketExchangeAndSilding) {
                    seatingEndTime = new Date().getTime();
                    var elapsedTime = seatingStartTime - seatingEndTime;
                    setTimeout(function () {
                        $(".slider").removeClass("disabled");
                    }, 1000);
                } else {
                    $(".slider").removeClass("disabled");
                }  
            }
            
            var PriceBreakList = [];
            $(seatingAreaXML).find('pbreak').each(function () {
                var priceBreak = $(this);
                PriceBreakList.push({ PriceBreakId: priceBreak.attr('id'), PriceBreakDescription: priceBreak.attr('d'), PriceBreakDefaultPriceBandPrice: priceBreak.attr('p'), DisplayPrice: priceBreak.attr('f') });
            });
            PriceBreakList.sort(function (a, b) {
                return parseFloat(a.PriceBreakDefaultPriceBandPrice) - parseFloat(b.PriceBreakDefaultPriceBandPrice)
            })
            SetupDynamicStandAndAreaOptions(undefined, standAndAreaOptions, false, false, false, PriceBreakList, false, isStadiumExpaned);
        },
        error: function (xhr, status) {
            $("#loading-image").hide();
            $(".slider").removeClass("disabled");
            alert(status + '  ' + xhr.responseText);
        }
    });

    //intialisation code for drag navigate
    seatingAreaCanvas.setViewBox(0, 0, seatingAreaCanvas.width, seatingAreaCanvas.height);
    viewBoxWidth = seatingAreaCanvas.width;
    viewBoxHeight = seatingAreaCanvas.height;
    mousedown = false;
    oX = 0, oY = 0, oWidth = viewBoxWidth, oHeight = viewBoxHeight;
    viewBox = seatingAreaCanvas.setViewBox(oX, oY, viewBoxWidth, viewBoxHeight);
    viewBox.X = oX;
    viewBox.Y = oY;

    endSeatSelection(standAndAreaCode);
    $(".ebiz-priceband").each(function () { $(this).hide(); });
}


// Create mini map for usability on large areas: it is a cut down version of createSeatingArea
function createMiniSeatingArea(seatingData) {
    $("#display").show();
    if (miniSeatingAreaCanvas) {
        miniSeatingAreaCanvas.remove();

    }
    miniSeatingAreaCanvas = Raphael('display', seatingAreaCanvasWidth, seatingAreaCanvasHeight);

    var seatingDataXMLDoc;
    if (window.DOMParser) {
        parser = new DOMParser();
        seatingDataXMLDoc = parser.parseFromString(seatingData, "text/xml");
    }
    else // Internet Explorer
    {
        seatingDataXMLDoc = new ActiveXObject("Microsoft.XMLDOM");
        seatingDataXMLDoc.async = "false";
        seatingDataXMLDoc.loadXML(seatingData);
    }
    seatingAreaXML = seatingDataXMLDoc;

    $(seatingDataXMLDoc).find('s').each(function () {
        var $seat = $(this);
        var seatDrawing;
        var availibility = $seat.attr('a');
        var x = $seat.attr('x');
        var y = $seat.attr('y');
        var scode = stand + "/" + $seat.attr('r') + "/" + $seat.attr('n');

        if (existsInBasket(scode)) {
            seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                .attr({ "fill": seatSelectedStroke })
        } else {
            switch (availibility) {
                case 'A':
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                    break;
                case ".":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatUnavailableFill, stroke: seatUnavailableStroke });
                    break;
                case "B":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatBasketFill, stroke: seatBasketStroke });
                    break;
                case "X":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatRestrictedFill, stroke: seatRestrictedStroke });
                    break;
                case ".2":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatUnAvailable2Fill, stroke: seatUnavailable2Stroke });
                    break;
                case "D":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatDisabledFill, stroke: seatDisabledStroke });
                    break;
                case "R":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatReservedFill, stroke: seatReservedStroke });
                    break;
                case "C":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatCustomerReservedFill, stroke: seatCustomerReservedStroke });
                    break;
                case "S":
                    seatDrawing = miniSeatingAreaCanvas.circle(x, y, 8)
                        .attr({ fill: seatPartialSeasonTicketFill, stroke: seatPartialSeasonTicketStroke });
                    break;
            }
        }
    });
    miniSeatingAreaCanvas.setViewBox(0, 0, document.getElementById("maxX").value, document.getElementById("maxY").value, false);
    miniSeatingAreaCanvas.setSize(stadiumCanvasHeight * 0.35, stadiumCanvasWidth * 0.35);
    zoomScaleFactor = zoomHeight / prevCanvasHeight;
    miniMat = miniSeatingAreaCanvas.rect(0, 0, (400 * zoomScaleFactor), (300 * zoomScaleFactor)).attr("stroke-width", 1).attr("stroke", "#ff0000").attr("fill-opacity", 0.5).attr("fill", "#ff0000");
    miniMat.drag(minidragmove, minidragstart, minidragend);
}


// function containing a list of settings to be set at the end of seat selection process
function endSeatSelection(standAndAreaCode) {
    if (isSeatSelectionOnly == false) {
        stadiumCanvas.setViewBox(0, 0, stadiumCanvasWidth, stadiumCanvasHeight, false);
        stadiumCanvas.setSize(stadiumCanvasWidthResized, stadiumCanvasHeightResized);
        if (isStadiumExpaned == true)
            $("#backButton").show();
    }
    isStadiumExpaned = false;
    $("#pitchStands").hide();
    $(".ebiz-match-text").hide();
    $(".ebiz-teams-wrapper").hide();
    $("#select").show();
    $(".ebiz-zoom-options").show();
    $(".ebiz-seating-key").show();
    $("#btnViewFromArea").show();
    $("#btnChangeStandView").hide();
    $("#navigation-options").show();
    $("#quick-buy-option").hide();
    $("#btnResetSeating").show();
    $(".c-ticket-exchange").hide();
    if (selectedStandAreaTEAllowed) {
        $(".c-ticket-exchange").show();
    }
    $(".c-price-band-best-available").hide();
    $(".c-price-band-concession").hide();
    if (isAgent) {
        $("#btnMultiSelect").show();
        if (isRowSeatOnSVG) {
            $("#btnRowSeatCodes").show();
        }
    }

    //populate view from area url string
    viewFromAreaUrl = "";
    var tempString = standAndAreaCode.split("-");
    viewFromAreaUrl = viewFromAreaUrlPrefix + tempString[0] + tempString[1] + ".jpg";
    $(".ebiz-stand-availability").hide();
    $(".ebiz-stand-pricing").hide();
    $(".ebiz-svg-container").addClass("ebiz-seat-view");
    $(".ebiz-svg-container").removeClass("ebiz-area-view");
    $("#svg-controls").removeClass("large-12 columns");
    $("#svg-controls").addClass("medium-3 columns");
    $("#svg-wrapper").addClass("medium-9 columns");
    $("#svg-wrapper").show();

    multiSelectOn = false;
    document.getElementById("btnMultiSelect").value = multiSelectOffText;
    document.getElementById("quick-buy-quantity").innerHTML = basketMin;

    //3D ticketing seat view
    if (use3DSeatView == true) {
        $("#ticketing3DSeatView").show();
        tk3d.fixedAspectRatio(true);
        tk3d.size(700, 300);
        tk3d.load('S_' + tempString[1]);
    }
}


//View from area button click
function ViewFromAreaClick() {
    Shadowbox.open({ player: 'img', content: viewFromAreaUrl });
}


//Mouse over seat function for tool tip
$(function () {
    $("#select").on('mouseover', function (e) {
        if (over) {
            $("#tip #tip-area .ebiz-data").text(seatingAreaAreaDesc);
            $("#tip #tip-stand .ebiz-data").text(seatingAreaStandDesc);
            $("#tip #tip-row .ebiz-data").text(SeatTipSeatRow);
            $("#tip #tip-seat-number .ebiz-data").text(SeatTipNumber);

            if (SeatTipPrice) {
                $("#tip #tip-seat-price").show();
                $("#tip #tip-seat-price .ebiz-data").text(SeatTipPrice);
            } else {
                $("#tip #tip-seat-price").hide();
            }

            if (restrictionCode) {
                $("#tip-restriction-code").show();
                $("#tip-restriction-desc").show();
                if (isAgent)
                    $("#tip-restriction-desc").text(restrictionDesc + "(" + restrictionCode + ")");
                else
                    $("#tip-restriction-desc").text(restrictionDesc);

            } else {
                $("#tip-restriction-code").hide();
                $("#tip-restriction-desc").hide();
            }

            if (reservationCode) {
                $("#tip-reservation-code").show();
                $("#tip-reservation-desc").show();
                if (isAgent)
                    $("#tip-reservation-desc").text(reservationDesc + "(" + reservationCode + ")");
                else
                    $("#tip-reservation-desc").text(reservationDesc);
            } else {
                $("#tip-reservation-code").hide();
                $("#tip-reservation-desc").hide();
            }

            var xTemp = e.pageX;
            var yTemp = e.pageY;
            var selectWidth = $('#select').width();
            var selectHeight = $('#select').height();
            var selectWidth = $('#select').width();
            var selectHeight = $('#select').height();
            var tipWidth = $('#tip').width();
            var tipHeight = $('#tip').height();
            var xTemp = (xTemp - $('#select').offset().left);
            var yTemp = (yTemp - $('#select').offset().top);

            // tip adjustments to show regardless of where cursor is pointing on seating area
            if ((tipWidth + xTemp > selectWidth) && (tipHeight + yTemp > selectHeight)) {
                $("#tip").css("left", xTemp - 17 - tipWidth).css("top", yTemp - 35 - tipHeight);
            } else if (tipWidth + xTemp > selectWidth) {
                $("#tip").css("left", xTemp - 17 - tipWidth).css("top", yTemp + 17);
            } else if (tipHeight + yTemp > selectHeight) {
                $("#tip").css("left", xTemp + 17).css("top", yTemp - 35 - tipHeight);
            } else {
                $("#tip").css("left", xTemp + 17).css("top", yTemp + 17);
            }

            return true;
        }
    });
});


// Tool tip mouse over event responsible for showing/hiding the the tool tip and setting the text
function addTip(area, txt, rxCode, rxDesc, rvCode, rvDesc, seatPrice) {
    var tempScode = txt.split("/");
    var tempStandAndArea = tempScode[0].split("-");
    $(area).on('mouseover', function () {
        SeatTipAreaCode = tempStandAndArea[0];
        SeatTipStandCode = tempStandAndArea[1];
        SeatTipSeatRow = tempScode[1];
        SeatTipNumber = tempScode[2];
        SeatTipPrice = seatPrice
        restrictionCode = rxCode;
        restrictionDesc = rxDesc;
        reservationCode = rvCode;
        reservationDesc = rvDesc;
        $("#tip").show();
        over = true;
    });
    $(area).on('mouseout', function () {
        $("#tip").hide();
        over = false;
    });

}


//Return boolean if the given price break id exists in the list of price breaks
function doesPriceBreakExistInPriceBreakList(priceBreakID)
{
    for (t = 0; t < priceBreaks.length; t++)
    {
        if (priceBreakID == priceBreaks[t].PriceBreakId)
            return true;
    }
    return false;
}


// draws seating area, attaches events to seats
function createSeatingArea(seatingData) {
    // set used for drag select functionality, the set can be rotated
    var seatingDataXMLDoc;
    set = seatingAreaCanvas.set();
    if (window.DOMParser) {
        parser = new DOMParser();
        seatingDataXMLDoc = parser.parseFromString(seatingData, "text/xml");
    }
    else // Internet Explorer
    {
        seatingDataXMLDoc = new ActiveXObject("Microsoft.XMLDOM");
        seatingDataXMLDoc.async = "false";
        seatingDataXMLDoc.loadXML(seatingData);
    }
    seatingAreaXML = seatingDataXMLDoc;
    var c = 0;
    var priceBreakId;
    var tempPriceBreaks = [];
    $(seatingDataXMLDoc).find('pbreak').each(function () {
        var priceBreak = $(this);
        var priceBreakPriceBandItems = [];
        priceBreak.find("d").each(function () {
            var priceBreakPriceBandItem = $(this);
            var priceBandDescription;
            $(seatingDataXMLDoc).find('pband').children("d").each(function () {
                var priceBand = $(this);
                if (priceBand.attr('b') == priceBreakPriceBandItem.attr('b')) {
                    priceBandDescription = priceBand.attr('desc');
                    return false;
                }
            });
            priceBreakPriceBandItems.push({ Price: priceBreakPriceBandItem.attr('p'), Description: priceBandDescription, PriceBand: priceBreakPriceBandItem.attr('b'), DisplayPrice: priceBreakPriceBandItem.attr('f') });
        });
        tempPriceBreaks.push({ PriceBreakId: priceBreak.attr('id'), PriceBreakPriceBandItems: priceBreakPriceBandItems });
    });
    for (j = 0; j < tempPriceBreaks.length; j++)
    {
        if (!doesPriceBreakExistInPriceBreakList(tempPriceBreaks[j].PriceBreakId))
        {
            priceBreaks.push(tempPriceBreaks[j]);
        }
    }
    // builds each seat using XML seatingData
    c = 0;
    var maxX;
    var maxY;
    if (isRowSeatOnSVG) {
        $(seatingDataXMLDoc).find('t').each(function () {
            var $seatCode = $(this);
            var xCode = $seatCode.attr('x');
            var yCode = $seatCode.attr('y');
            var valueCode = $seatCode.attr('v');
            var typeCode = $seatCode.attr('t');

            var textCode = seatingAreaCanvas.text(xCode, yCode, valueCode);
            if (typeCode == 's')
                textCode.transform("r45");
        });
    }
    $(seatingDataXMLDoc).find('s').each(function () {
        var $seat = $(this);
        var seatDrawing;
        var availibility = $seat.attr('a');
        var priceBreak = $seat.attr('pb');
        var x = $seat.attr('x');
        var y = $seat.attr('y');
        var row = $seat.attr('r');
        var seatNumber = $seat.attr('n');
        var seatPrice = $seat.attr('p');
        var scode = stand + "/" + $seat.attr('r') + "/" + $seat.attr('n');
        var seatRestrictionCode = $seat.attr('v');
        var seatRestrictionDesc = $seat.attr('vDesc');
        var seatReservationCode = $seat.attr('rs');
        var seatReservationDesc = $seat.attr('rsDesc');

        // retrieves max width and height seating area data and colour scheme for different seats from first row of xml document
        if (c == 0) {
            c = 1;

            maxX = $seat.attr('cn');
            maxY = $seat.attr('rn');
            seatUnavailableFill = $seat.attr('uo');
            seatUnavailableStroke = $seat.attr('uc');
            seatAvailableStroke = $seat.attr('ac');
            seatAvailableFill = $seat.attr('ao');

            //if seat colour is undefined then it becomes the unavailable colour
            if (seatAvailableFill == undefined && seatAvailableStroke == undefined) {
                seatAvailableStroke = seatUnavailableStroke;
                seatAvailableFill = seatUnavailableFill;
            }
            seatCustomerReservedStroke = $seat.attr('cc');
            seatCustomerReservedFill = $seat.attr('co');
            if (seatCustomerReservedFill == undefined && seatCustomerReservedStroke == undefined) {
                seatCustomerReservedStroke = seatUnavailableStroke;
                seatCustomerReservedFill = seatUnavailableFill;
            }
            seatBasketFill = $seat.attr('bo');
            seatBasketStroke = $seat.attr('bc');
            if (seatBasketFill == undefined && seatBasketStroke == undefined) {
                seatBasketStroke = seatUnavailableStroke;
                seatBasketFill = seatUnavailableFill;
            }
            seatRestrictedFill = $seat.attr('ro');
            seatRestrictedStroke = $seat.attr('rc');
            if (seatRestrictedFill == undefined && seatRestrictedStroke == undefined) {
                seatRestrictedStroke = seatUnavailableStroke;
                seatRestrictedFill = seatUnavailableFill;
            }
            seatUnavailable2Stroke = $seat.attr('u2c');
            seatUnAvailable2Fill = $seat.attr('u2o');
            if (seatUnAvailable2Fill == undefined && seatUnavailable2Stroke == undefined) {
                seatUnavailable2Stroke = seatUnavailableStroke;
                seatUnAvailable2Fill = seatUnavailableFill;
            }
            seatDisabledStroke = $seat.attr('dc');
            seatDisabledFill = $seat.attr('do');
            if (seatDisabledFill == undefined && seatDisabledStroke == undefined) {
                seatDisabledStroke = seatUnavailableStroke;
                seatDisabledFill = seatUnavailableFill;
            }
            seatReservedStroke = $seat.attr('vc');
            seatReservedFill = $seat.attr('vo');
            if (seatReservedFill == undefined && seatReservedStroke == undefined) {
                seatReservedStroke = seatUnavailableStroke;
                seatReservedFill = seatUnavailableFill;
            }
            seatHoverFill = $seat.attr('ho');
            seatHoverStroke = $seat.attr('hc');
            if (seatHoverFill == undefined && seatHoverStroke == undefined) {
                seatHoverStroke = seatUnavailableStroke;
                seatHoverFill = seatUnavailableFill;
            }
            seatSelectedFill = $seat.attr('selo');
            seatSelectedStroke = $seat.attr('selc');
            if (seatSelectedFill == undefined && seatSelectedStroke == undefined) {
                seatSelectedStroke = seatUnavailableStroke;
                seatSelectedFill = seatUnavailableFill;
            }
            seatTransferableFill = $seat.attr('to');
            seatTransferableStroke = $seat.attr('tc');
            if (seatTransferableFill == undefined && seatTransferableStroke == undefined) {
                seatTransferableStroke = seatUnavailableStroke;
                seatTransferableFill = seatUnavailableFill;
            }
            seatExceptionFill = $seat.attr('eo');
            seatExceptionStroke = $seat.attr('ec');
            if (seatExceptionFill == undefined && seatExceptionStroke == undefined) {
                seatExceptionStroke = seatUnavailableStroke;
                seatExceptionFill = seatUnavailableFill;
            }
            seatTicketExchangeFill = $seat.attr('txo');
            seatTicketExchangeStroke = $seat.attr('txc');
            if (seatTicketExchangeFill == undefined && seatTicketExchangeStroke == undefined) {
                seatTicketExchangeStroke = seatUnavailableStroke;
                seatTicketExchangeFill = seatUnavailableFill;
            }
            seatPartialSeasonTicketFill = $seat.attr('pso');
            seatPartialSeasonTicketStroke = $seat.attr('psc');
            if (seatPartialSeasonTicketFill == undefined && seatPartialSeasonTicketStroke == undefined) {
                seatPartialSeasonTicketStroke = seatUnavailableStroke;
                seatPartialSeasonTicketFill = seatUnavailableFill;
            }

        }

        var tempScode = scode.split("/");
        var tempStandAndArea = tempScode[0].split("-");

        if (existsInBasket(scode)) {
            //necessary for persisting seat selection across stands
            if (availibility == "T") {
                seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatTransferableFill, stroke: seatTransferableStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatTransferableFill)
                    .data("orginalStroke", seatTransferableStroke)
                    .data("avail", availibility);

                $(seatDrawing.node).on('mouseout', function () {
                    if (existsInBasket(scode))
                        seatDrawing.attr({ fill: seatTransferableFill, stroke: seatTransferableStroke });
                });
                $(seatDrawing.node).on('click', function (e) {
                    $(".ebiz-seat-details").hide();
                    $(".ebiz-restriction-details").hide();
                    seatClick($seat, seatDrawing, scode, seatTransferableFill, seatTransferableStroke, true);
                });
                addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);

            } else {
                seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatSelectedFill, stroke: seatSelectedStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("avail", availibility);

                $(seatDrawing.node).on('mouseout', function () {
                    if (!existsInBasket(scode))
                        seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                    $("#tip").hide();
                    over = false;
                });
                $(seatDrawing.node).on('mouseover', function () {
                    SeatTipAreaCode = tempStandAndAreaA[0];
                    SeatTipStandCode = tempStandAndAreaA[1];
                    SeatTipSeatRow = tempScodeA[1];
                    SeatTipNumber = tempScodeA[2];
                    SeatTipPrice = seatPrice;
                    restrictionCode = undefined;
                    restrictionDesc = undefined;
                    reservationCode = undefined;
                    reservationDesc = undefined;
                    $("#tip").show();
                    over = true;
                });
                $(seatDrawing.node).on('click', function () {
                    $(".ebiz-seat-details").hide();
                    $(".ebiz-restriction-details").hide();
                    seatClick($seat, seatDrawing, scode, seatAvailableFill, seatAvailableStroke, false);
                });

                if (isAgent && isRowSeatOnSVG) {
                    var seatText = trimStartZerosfromSeat(seatNumber);
                    var seatTextObject = seatingAreaCanvas.text(x, y, seatText);

                    $(seatTextObject.node).on('click', function () {
                        seatClick($seat, seatDrawing, scode, seatAvailableFill, seatAvailableStroke, false);
                    });

                    $(seatTextObject.node).on('mouseover', function () {
                        if (!existsInBasket(scode)) {
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                            seatTextObject.node.style.cursor = 'default';
                        }
                    });

                    $(seatTextObject.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                    });

                    seatTextObject.mouseover(function () {
                        if (!("ontouchstart" in document.documentElement)) {
                            SeatTipAreaCode = tempStandAndAreaA[0];
                            SeatTipStandCode = tempStandAndAreaA[1];
                            SeatTipSeatRow = tempScodeA[1];
                            SeatTipNumber = tempScodeA[2];
                            SeatTipPrice = seatPrice;
                            restrictionCode = undefined;
                            restrictionDesc = undefined;
                            reservationCode = undefined;
                            reservationDesc = undefined;
                            $("#tip").show();
                            over = true;
                        }
                    });
                }

                $(seatDrawing.node).on('mouseover', function () {
                    if (!existsInBasket(scode))
                        seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                });
            }
            $(seatDrawing.node).on('mouseover', function () {
                if (!existsInBasket(scode))
                    seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
            });

            $(seatDrawing.node).on('mouseout', function () {
                if (!existsInBasket(scode))
                    seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke});
            });
        } else {
            switch (availibility) {

                case 'A': //Available
                    var tempScodeA = scode.split("/");
                    var tempStandAndAreaA = tempScode[0].split("-");
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatAvailableFill, stroke: seatAvailableStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                     .data("orginalFill", seatAvailableFill)
                    .data("orginalStroke", seatAvailableStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                        $("#tip").hide();
                        over = false;
                    });
                    $(seatDrawing.node).on('mouseover', function () {
                        SeatTipAreaCode = tempStandAndAreaA[0];
                        SeatTipStandCode = tempStandAndAreaA[1];
                        SeatTipSeatRow = tempScodeA[1];
                        SeatTipNumber = tempScodeA[2];
                        SeatTipPrice = seatPrice;
                        restrictionCode = undefined;
                        restrictionDesc = undefined;
                        reservationCode = undefined;
                        reservationDesc = undefined;
                        $("#tip").show();
                        over = true;
                    });
                    $(seatDrawing.node).on('click', function () {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatAvailableFill, seatAvailableStroke, false);
                    });

                    if (isAgent && isRowSeatOnSVG) {
                        var seatText = trimStartZerosfromSeat(seatNumber);
                        var seatTextObject = seatingAreaCanvas.text(x, y, seatText);

                        $(seatTextObject.node).on('click', function () {
                            seatClick($seat, seatDrawing, scode, seatAvailableFill, seatAvailableStroke, false);
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            if (!existsInBasket(scode)) {
                                seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                                seatTextObject.node.style.cursor = 'default';
                            }
                        });

                        $(seatTextObject.node).on('mouseout', function () {
                            if (!existsInBasket(scode))
                                seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            seatTextObject.node.style.cursor = 'default';
                            SeatTipAreaCode = tempStandAndAreaA[0];
                            SeatTipStandCode = tempStandAndAreaA[1];
                            SeatTipSeatRow = tempScodeA[1];
                            SeatTipNumber = tempScodeA[2];
                            SeatTipPrice = seatPrice;
                            restrictionCode = undefined;
                            restrictionDesc = undefined;
                            reservationCode = undefined;
                            reservationDesc = undefined;
                            $("#tip").show();
                            over = true;
                        });

                        seatTextObject.mouseover(function () {
                            if (!("ontouchstart" in document.documentElement)) {
                                SeatTipAreaCode = tempStandAndAreaA[0];
                                SeatTipStandCode = tempStandAndAreaA[1];
                                SeatTipSeatRow = tempScodeA[1];
                                SeatTipNumber = tempScodeA[2];
                                SeatTipPrice = seatPrice;
                                restrictionCode = undefined;
                                restrictionDesc = undefined;
                                reservationCode = undefined;
                                reservationDesc = undefined;
                                $("#tip").show();
                                over = true;
                            }
                        });
                    }

                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });

                    break;

                case ".": //Unavailble
                    var tempScodeA = scode.split("/");
                    var tempStandAndAreaA = tempScode[0].split("-");
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatUnavailableFill, stroke: seatUnavailableStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatUnavailableFill)
                    .data("orginalStroke", seatUnavailableStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatUnavailableFill, stroke: seatUnavailableStroke });
                        $("#tip").hide();
                        over = false;
                    });
                    $(seatDrawing.node).on('mouseover', function () {
                        SeatTipAreaCode = tempStandAndAreaA[0];
                        SeatTipStandCode = tempStandAndAreaA[1];
                        SeatTipSeatRow = tempScodeA[1];
                        SeatTipNumber = tempScodeA[2];
                        SeatTipPrice = seatPrice;
                        restrictionCode = undefined;
                        restrictionDesc = undefined;
                        reservationCode = undefined;
                        reservationDesc = undefined;
                        $("#tip").show();
                        over = true;
                    });
                    $(seatDrawing.node).on('click', function (e) {
                        // get seating details for unavailable seats
                        if (showCustomerDetails == true) {
                            $.ajax({
                                type: "POST",
                                url: "VisualSeatSelection.aspx/GetSeatInformation",
                                cache: false,
                                data: '{data: "' + scode + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","campaignCode":"' + campaignCode + '","changeAllSeats":"' + changeAllSeats + '"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    getSeatingDetails(msg.d);
                                },
                                error: function (xhr, status) {
                                    alert(status + '  ' + xhr.responseText);
                                }
                            });
                        }
                    });

                    if (isAgent && isRowSeatOnSVG) {
                        var seatText = trimStartZerosfromSeat(seatNumber);
                        var seatTextObject = seatingAreaCanvas.text(x, y, seatText);

                        $(seatTextObject.node).on('click', function () {
                            // get seating details for unavailable seats
                            if (showCustomerDetails == true) {
                                $.ajax({
                                    type: "POST",
                                    url: "VisualSeatSelection.aspx/GetSeatInformation",
                                    cache: false,
                                    data: '{data: "' + scode + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","campaignCode":"' + campaignCode + '","changeAllSeats":"' + changeAllSeats + '"}',
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        getSeatingDetails(msg.d);
                                    },
                                    error: function (xhr, status) {
                                        alert(status + '  ' + xhr.responseText);
                                    }
                                });
                            }
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            seatTextObject.node.style.cursor = 'default';
                            SeatTipAreaCode = tempStandAndAreaA[0];
                            SeatTipStandCode = tempStandAndAreaA[1];
                            SeatTipSeatRow = tempScodeA[1];
                            SeatTipNumber = tempScodeA[2];
                            SeatTipPrice = seatPrice;
                            restrictionCode = undefined;
                            restrictionDesc = undefined;
                            reservationCode = undefined;
                            reservationDesc = undefined;
                            $("#tip").show();
                            over = true;
                        });

                        seatTextObject.mouseover(function () {
                            if (!("ontouchstart" in document.documentElement)) {
                                SeatTipAreaCode = tempStandAndAreaA[0];
                                SeatTipStandCode = tempStandAndAreaA[1];
                                SeatTipSeatRow = tempScodeA[1];
                                SeatTipNumber = tempScodeA[2];
                                SeatTipPrice = seatPrice;
                                restrictionCode = undefined;
                                restrictionDesc = undefined;
                                reservationCode = undefined;
                                reservationDesc = undefined;
                                $("#tip").show();
                                over = true;
                            }
                        });
                    }
                    break;

                case "B": //In the basket
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatBasketFill, stroke: seatBasketStroke })
                    .data("seatCode", scode)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        seatDrawing.attr({ fill: seatBasketFill, stroke: seatBasketStroke });
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "X": //Restricted
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatRestrictedFill, stroke: seatRestrictedStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatRestrictedFill)
                    .data("orginalStroke", seatRestrictedStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('click', function (e) {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatRestrictedFill, seatRestrictedStroke, false);
                    });
                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatRestrictedFill, stroke: seatRestrictedStroke });
                    });
                    addTip(seatDrawing.node, scode, seatRestrictionCode, seatRestrictionDesc, undefined, undefined, seatPrice);

                    if (isAgent && isRowSeatOnSVG) {
                        var textRes = seatingAreaCanvas.text(x, y, seatRestrictionCode);
                        textRes.click(function (e) {
                            seatClick($seat, seatDrawing, scode, seatRestrictedFill, seatRestrictedStroke, false);
                        });
                        textRes.hover(function () {
                            if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                        });
                        textRes.mouseout(function () {
                            if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatRestrictedFill, stroke: seatRestrictedStroke });
                        });
                        addTip(textRes, scode, seatRestrictionCode, seatRestrictionDesc, undefined, undefined, seatPrice);
                    }
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case ".2": //Sold/Reserved (to you or F&F)
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatUnAvailable2Fill, stroke: seatUnavailable2Stroke })
                    .data("seatCode", scode)
                    .data("orginalFill", seatUnAvailable2Fill)
                    .data("orginalStroke", seatUnavailable2Stroke)
                    .data("avail", availibility);
                    $(seatDrawing.node).on('mouseout', function () {
                        seatDrawing.attr({ fill: seatUnAvailable2Fill, stroke: seatUnavailable2Stroke });
                    });

                    $(seatDrawing.node).on('click', function (e) {
                        // get seating details for unavailable seats
                        if (showCustomerDetails == true) {
                            $.ajax({
                                type: "POST",
                                url: "VisualSeatSelection.aspx/GetSeatInformation",
                                data: '{data: "' + scode + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","campaignCode":"' + campaignCode + '","changeAllSeats":"' + changesAllSeats + '"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    getSeatingDetails(msg.d);
                                },
                                error: function (xhr, status) {
                                    alert(status + '  ' + xhr.responseText);
                                }
                            });
                        }
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "D": //Disabled Seat
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatDisabledFill, stroke: seatDisabledStroke })
                    .data("seatCode", scode)
                    .data("orginalFill", seatDisabledFill)
                    .data("orginalStroke", seatDisabledStroke)
                    .data("priceBreakId", priceBreak)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('click', function (e) {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatDisabledFill, seatDisabledStroke, false);
                    });
                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatDisabledFill, stroke: seatDisabledStroke });
                    });
                    addTip(seatDrawing.node, scode, seatRestrictionCode, seatRestrictionDesc, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "R": //Reserved
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatReservedFill, stroke: seatReservedStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatReservedFill)
                    .data("orginalStroke", seatReservedStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('click', function (e) {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        if (isAgent == true)
                            seatClick($seat, seatDrawing, scode, seatReservedFill, seatReservedStroke, false);
                    })
                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatReservedFill, stroke: seatReservedStroke });
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, seatReservationCode, seatReservationDesc, seatPrice);
                    if (isAgent && isRowSeatOnSVG) {
                        var textRev = seatingAreaCanvas.text(x, y, seatReservationCode);
                        textRev.click(function (e) {
                            seatClick($seat, seatDrawing, scode, seatReservedFill, seatReservedStroke, false);
                        });
                        textRev.hover(function () {
                            if (!existsInBasket(scode)) { seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke }); }
                        });
                        textRev.mouseout(function () {
                            if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatReservedFill, stroke: seatReservedStroke });
                        });
                        addTip(textRev, scode, undefined, undefined, seatReservationCode, seatReservationDesc, seatPrice);
                    }
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "C": //Customer Reserved
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatCustomerReservedFill, stroke: seatCustomerReservedStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatCustomerReservedFill)
                    .data("orginalStroke", seatCustomerReservedStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        seatDrawing.attr({ fill: seatCustomerReservedFill, stroke: seatCustomerReservedStroke });
                    });

                    $(seatDrawing.node).on('click', function (e) {
                        seatClick($seat, seatDrawing, scode, seatAvailableFill, seatAvailableStroke, false);
                        // get seating details for unavailable seats
                        if (showCustomerDetails == true) {
                            $.ajax({
                                type: "POST",
                                url: "VisualSeatSelection.aspx/GetSeatInformation",
                                data: '{data: "' + scode + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","campaignCode":"' + campaignCode + '","changeAllSeats":"' + changeAllSeats +'"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {
                                    getSeatingDetails(msg.d);
                                },
                                error: function (xhr, status) {
                                    alert(status + '  ' + xhr.responseText);
                                }
                            });
                        }
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "T": //Transferable Seat
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatAvailableFill, stroke: seatAvailableStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatAvailableFill)
                    .data("orginalStroke", seatAvailableStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatAvailableFill, stroke: seatAvailableStroke });
                    });

                    $(seatDrawing.node).on('click', function (e) {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatTransferableFill, seatTransferableStroke, true);
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "EX": //ST Exception Seat
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatExceptionFill, stroke: seatExceptionStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatExceptionFill)
                    .data("orginalStroke", seatExceptionStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        seatDrawing.attr({ fill: seatExceptionFill, stroke: seatExceptionStroke });
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    break;

                case "TX": //Ticket Exchange Listed Seat
                    var tempScodeA = scode.split("/");
                    var tempStandAndAreaA = tempScode[0].split("-");
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatTicketExchangeFill, stroke: seatTicketExchangeStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                    .data("orginalFill", seatTicketExchangeFill)
                    .data("orginalStroke", seatTicketExchangeStroke)
                    .data("avail", availibility);

                    if (isAgent && isRowSeatOnSVG) {
                        var seatText = trimStartZerosfromSeat(seatNumber);
                        var seatTextObject = seatingAreaCanvas.text(x, y, seatText);

                        $(seatTextObject.node).on('click', function () {
                            seatClick($seat, seatDrawing, scode, seatTicketExchangeFill, seatTicketExchangeStroke, false);
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            if (!existsInBasket(scode)) {
                                seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                                seatTextObject.node.style.cursor = 'default';
                            }
                        });

                        $(seatTextObject.node).on('mouseout', function () {
                            if (!existsInBasket(scode))
                                seatDrawing.attr({ fill: seatTicketExchangeFill, stroke: seatTicketExchangeStroke });
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            seatTextObject.node.style.cursor = 'default';
                            SeatTipAreaCode = tempStandAndAreaA[0];
                            SeatTipStandCode = tempStandAndAreaA[1];
                            SeatTipSeatRow = tempScodeA[1];
                            SeatTipNumber = tempScodeA[2];
                            SeatTipPrice = seatPrice;
                            restrictionCode = undefined;
                            restrictionDesc = undefined;
                            reservationCode = undefined;
                            reservationDesc = undefined;
                            $("#tip").show();
                            over = true;
                        });

                        seatTextObject.mouseover(function () {
                            if (!("ontouchstart" in document.documentElement)) {
                                SeatTipAreaCode = tempStandAndAreaA[0];
                                SeatTipStandCode = tempStandAndAreaA[1];
                                SeatTipSeatRow = tempScodeA[1];
                                SeatTipNumber = tempScodeA[2];
                                SeatTipPrice = seatPrice;
                                restrictionCode = undefined;
                                restrictionDesc = undefined;
                                reservationCode = undefined;
                                reservationDesc = undefined;
                                $("#tip").show();
                                over = true;
                            }
                        });
                    }

                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatTicketExchangeFill, stroke: seatTicketExchangeStroke });
                    });
                    addTip(seatDrawing.node, scode, undefined, undefined, undefined, undefined, seatPrice);
                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode)) seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });
                    $(seatDrawing.node).on('click', function () {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatTicketExchangeFill, seatTicketExchangeStroke, false);
                    });
                    break;

                case 'S': //Partial Available
                    var tempScodeA = scode.split("/");
                    var tempStandAndAreaA = tempScode[0].split("-");
                    seatDrawing = seatingAreaCanvas.circle(x, y, 8)
                    .attr({ fill: seatPartialSeasonTicketFill, stroke: seatPartialSeasonTicketStroke })
                    .data("seatCode", scode)
                    .data("priceBreakId", priceBreak)
                     .data("orginalFill", seatPartialSeasonTicketFill)
                    .data("orginalStroke", seatPartialSeasonTicketStroke)
                    .data("avail", availibility);

                    $(seatDrawing.node).on('mouseout', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatPartialSeasonTicketFill, stroke: seatPartialSeasonTicketStroke });
                        $("#tip").hide();
                        over = false;
                    });
                    $(seatDrawing.node).on('mouseover', function () {
                        SeatTipAreaCode = tempStandAndAreaA[0];
                        SeatTipStandCode = tempStandAndAreaA[1];
                        SeatTipSeatRow = tempScodeA[1];
                        SeatTipNumber = tempScodeA[2];
                        SeatTipPrice = seatPrice;
                        restrictionCode = undefined;
                        restrictionDesc = undefined;
                        reservationCode = undefined;
                        reservationDesc = undefined;
                        $("#tip").show();
                        over = true;
                    });
                    $(seatDrawing.node).on('click', function () {
                        $(".ebiz-seat-details").hide();
                        $(".ebiz-restriction-details").hide();
                        seatClick($seat, seatDrawing, scode, seatPartialSeasonTicketFill, seatPartialSeasonTicketStroke, false);
                    });

                    if (isAgent && isRowSeatOnSVG) {
                        var seatText = trimStartZerosfromSeat(seatNumber);
                        var seatTextObject = seatingAreaCanvas.text(x, y, seatText);

                        $(seatTextObject.node).on('click', function () {
                            seatClick($seat, seatDrawing, scode, seatPartialSeasonTicketFill, seatPartialSeasonTicketStroke, false);
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            if (!existsInBasket(scode)) {
                                seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                                seatTextObject.node.style.cursor = 'default';
                            }
                        });

                        $(seatTextObject.node).on('mouseout', function () {
                            if (!existsInBasket(scode))
                                seatDrawing.attr({ fill: seatPartialSeasonTicketFill, stroke: seatPartialSeasonTicketStroke });
                        });

                        $(seatTextObject.node).on('mouseover', function () {
                            seatTextObject.node.style.cursor = 'default';
                            SeatTipAreaCode = tempStandAndAreaA[0];
                            SeatTipStandCode = tempStandAndAreaA[1];
                            SeatTipSeatRow = tempScodeA[1];
                            SeatTipNumber = tempScodeA[2];
                            SeatTipPrice = seatPrice;
                            restrictionCode = undefined;
                            restrictionDesc = undefined;
                            reservationCode = undefined;
                            reservationDesc = undefined;
                            $("#tip").show();
                            over = true;
                        });

                        seatTextObject.mouseover(function () {
                            if (!("ontouchstart" in document.documentElement)) {
                                SeatTipAreaCode = tempStandAndAreaA[0];
                                SeatTipStandCode = tempStandAndAreaA[1];
                                SeatTipSeatRow = tempScodeA[1];
                                SeatTipNumber = tempScodeA[2];
                                SeatTipPrice = seatPrice;
                                restrictionCode = undefined;
                                restrictionDesc = undefined;
                                reservationCode = undefined;
                                reservationDesc = undefined;
                                $("#tip").show();
                                over = true;
                            }
                        });
                    }

                    $(seatDrawing.node).on('mouseover', function () {
                        if (!existsInBasket(scode))
                            seatDrawing.attr({ fill: seatHoverFill, stroke: seatHoverStroke });
                    });

                    break;
            }
        }
        set.push(seatDrawing);
    });
    maxX = (maxX * 8) + (maxX * 25);
    maxY = (maxY * 8) + (maxY * 25);
    $('#maxX').val(maxX);
    $('#maxY').val(maxY);
    resetSlider(false);
}


// Persists selected seats across stand changes
function selectionPersistence(scode, selectedColour, availableColour) {
    for (var i = 0; i < basket.length; i++) {
        if (basket[i] == scode)
            return selectedColour;
    }
    return availableColour; //unselected colour
}


// Delete a selected seat from the selected table list
function deleteRow(row) {
    var scode = row.parentNode.parentNode.id;
    var index = basket.map(function (d) { return d['SeatInfo']; }).indexOf(scode);
    basket.splice(index, 1);
    var changeAllSeats = GetParameterByName("changeallseats");
    //show and update seat items-total
    updateNumberOfSelectedSeats();

    // re-populate selected seats hidden field with seats needed for seat transfers
    //updateSelectedSeatsHiddenField()

    // find seat on screen to display as unselected
    var tempScode = scode.split("/");
    if (stand === tempScode[0]) {
        for (var i in set.items) {
            var seatDrawing = set.items[i];
            if (seatDrawing.data("seatCode") === scode) {
                if (changeAllSeats = "Y") {
                    seatDrawing.attr("fill", seatAvailableFill);
                    seatDrawing.attr("stroke", seatAvailableStroke);
                }
                else{
                seatDrawing.attr("fill", seatDrawing.data("orginalFill"));
                seatDrawing.attr("stroke", seatDrawing.data("orginalStroke"));
                }
                break;
            }
        }
    }

    // delete all items from table
    var miniBasketContent = $(".c-price-band-selection .row");
    miniBasketContent.each(function () {
        $(this).remove();
    });

    miniBasketContent = $(".c-price-band-selection .c-price-band-selection__block");
    miniBasketContent.each(function () {
        $(this).remove();
    });

    // re populate the table with basket items
    for (var i = 0; i < basket.length; i++) {
        if (i < 10) {
            addHTMLRowItemToMiniBasketTable(basket[i]);
        }
    }
    displayedStandAndAreaBasket = [];
}


//Sorts basket items by property
function sortResults(prop, asc) {
    basket = basket.sort(function (a, b) {
        if (asc) {
            return (a[prop] > b[prop]) ? 1 : ((a[prop] < b[prop]) ? -1 : 0);
        } else {
            return (b[prop] > a[prop]) ? 1 : ((b[prop] < a[prop]) ? -1 : 0);
        }
    });
}


// Add a seat into the list
function addRow(basketItem)
{
    // Order elements
    sortResults('SeatInfo', true);

    // delete all items from table
    var miniBasketContent = $(".c-price-band-selection .row");
    miniBasketContent.each(function () {
        $(this).remove();
    });

    miniBasketContent = $(".c-price-band-selection .c-price-band-selection__block");
    miniBasketContent.each(function () {
        $(this).remove();
    });

    // re populate the table with basket items
    for (var i = 0; i < basket.length; i++) {
        if (i < 10) {
            addHTMLRowItemToMiniBasketTable(basket[i]);
        }
    }
    displayedStandAndAreaBasket = [];
}


//checks if stand and area has title displayed
function isStandAndAreaBasketDisplayed(scode){
    for (d = 0; d < displayedStandAndAreaBasket.length; d++) {
        if (displayedStandAndAreaBasket[d] == scode)
            return true;
    }
    return false;
}


//Add the HTML row to the visible selected "mini-basket" seats
function addHTMLRowItemToMiniBasketTable(basketItem) {
    var rowText = document.getElementById("hdfRowText").value;
    var seatText = document.getElementById("hdfSeatText").value;
    var localAreaDesc = "";
    var localStandDesc = "";

    $(document.getElementById("hdfDescriptionsXML").value).find('saa').each(function () {
        var $entry = $(this);
        if ($entry.attr('sc') == basketItem.Stand)
            localStandDesc = $entry.attr('sd');

        if ($entry.attr('ac') == basketItem.Area)
            localAreaDesc = $entry.attr('ad');

        if (localAreaDesc != "" && localStandDesc != "")
            return false;
    });

    if (displayedStandAndAreaBasket.length > 0) {
        if (!isStandAndAreaBasketDisplayed(basketItem.Stand + "-" + basketItem.Area)) {
            $(".c-price-band-selection").append("<h3 class='c-price-band-selection__block'>" + localStandDesc + "&ndash;" + localAreaDesc + "</h3>");
            displayedStandAndAreaBasket.push(basketItem.Stand + "-" + basketItem.Area);
        }
    } else {
        // display header
        $(".c-price-band-selection").append("<h3 class='c-price-band-selection__block'>" + localStandDesc + "&ndash;" + localAreaDesc + "</h3>");
        displayedStandAndAreaBasket.push(basketItem.Stand + "-" + basketItem.Area);
    }

    //Loop Price Bands available for this basket item, load array
    var itemAvailablePriceBands = [];
    var itemAvailablePriceBandDescriptions = [];
    var itemAvailablePriceBandDisplayPrice = [];
    for (j = 0; j < priceBreaks.length; j++) {
        if (priceBreaks[j].PriceBreakId == basketItem.PriceBreakId) {
            for (l = 0; l < priceBreaks[j].PriceBreakPriceBandItems.length; l++) {
                itemAvailablePriceBands.push(priceBreaks[j].PriceBreakPriceBandItems[l].PriceBand);
                itemAvailablePriceBandDescriptions.push(priceBreaks[j].PriceBreakPriceBandItems[l].Description);
                itemAvailablePriceBandDisplayPrice.push(priceBreaks[j].PriceBreakPriceBandItems[l].DisplayPrice);
            }
        }
    }

    // Default the customer PB if an option within the list.
    // Product Default Price Band Otherwise.
    if (basketItem.PriceBand == "") {  
        var ProductDefaultPriceBandIndex;
        customerPriceBandIndex = itemAvailablePriceBands.indexOf(loggedInCustomerPriceBand)
        ProductDefaultPriceBandIndex = itemAvailablePriceBands.indexOf(defaultPriceBand)
        if (customerPriceBandIndex > -1 && loggedInCustomerPriceBand != "") {
            basketItem.PriceBand = loggedInCustomerPriceBand;
        } else if (ProductDefaultPriceBandIndex > -1 && ProductDefaultPriceBandIndex != "") {
            basketItem.PriceBand = defaultPriceBand;
        } else if (itemAvailablePriceBands.length > 0) {
            basketItem.PriceBand = itemAvailablePriceBands[0];
        }
    }

    var priceBandDescription = defaultPriceBandDescription;
    var index = itemAvailablePriceBands.indexOf(basketItem.PriceBand)
    if (index > -1) {
        priceBandDescription = itemAvailablePriceBandDescriptions[index];
    } 

    var priceBandOption = "";
    var seatCode = basketItem.SeatInfo.replace('\/', '');
    seatCode = seatCode.replace('\/', '');//This is intentional as the replace function does 1 replace and not multiple

    // Priceband selection up front is only allowed:
    // - Concession sales active 
    // - Not in Bulk Mode
    // - Not a Ticket Exchange Sale
    if (allowPriceBandSelection && allowPriceBandSelection == "True" && !isBulkMode && priceBreaks.length > 0 && basketItem.PriceBreakId != "" && !(basketItem.AvailabilityCode && basketItem.AvailabilityCode == "TX")) {
            priceBandOption = '<div class="columns c-price-band-selection__band"><select class="' + seatCode + '" onchange="updatePriceBandOnBasketItem(this,\'' + basketItem.SeatInfo + '\');">';
            for (j = 0; j < itemAvailablePriceBands.length; j++) {
                priceBandOption = priceBandOption + "<option value='" + itemAvailablePriceBands[j] + "'>" + itemAvailablePriceBandDescriptions[j] + "  " + itemAvailablePriceBandDisplayPrice[j] + "</option>";
            }
            priceBandOption = priceBandOption + '</select></div>';
    } else {
        priceBandOption = '<div class="columns c-price-band-selection__band--single-concession"><span>' + priceBandDescription + '</span></div>';
    }

    $(".c-price-band-selection").append('<div id=\'' + basketItem.SeatInfo + '\' class="row c-price-band-selection__grid-row">' +
                    '<div class="columns c-price-band-selection__row-seat">' +
                        '<div class="c-price-band-selection__container">' +
                            '<span class="c-price-band-selection__row"><span data-tooltip aria-haspopup="true" class="has-tip right" data-disable-hover="false" title="Row">' + basketItem.Row + '</span></span>' +
                            '<span class="c-price-band-selection__seat2><span data-tooltip aria-haspopup="true" class="has-tip right" data-disable-hover="false" title="Seat">' + basketItem.Seat + '</span></span>' +
                        '</div>' +
                    '</div>' +
                        priceBandOption +
                    '<div class="columns c-price-band-selection__delete">' +
                        '<a onclick="deleteRow(this)"><i class="fa fa-times" aria-hidden="true"></i></a>' +
                    '</div>' +
                '</div>');
    var selectedItem = "div.c-price-band-selection__band ." + seatCode;
    $(selectedItem).val(basketItem.PriceBand);
}


//Update the price band on the mini basket selected item
function updatePriceBandOnBasketItem(selectedItem, seatInfo){
    for (j = 0; j < basket.length; j++) {
        if (basket[j].SeatInfo == seatInfo)
        {
            basket[j].PriceBand = selectedItem.value;
        }
    }
}


//Update the selected seats hidden field
function updateSelectedSeatsHiddenField() {
    var seatString = "";
    if (basket.length > 0) {
        document.getElementById("hdfSelectedSeats").defaultValue = "";
        for (var i = 0; i < basket.length; i++) {
            if (i > 0) {
                seatString = seatString + ",";
            }
            seatString = seatString + basket[i].SeatInfo + "#" + basket[i].PriceBand;
            document.getElementById("hdfSelectedSeats").defaultValue = seatString;
        }
    }
    else {
        document.getElementById("hdfSelectedSeats").defaultValue = "";
    }
}


// The user has clicked a seat
function seatClick(seatObj, seatDrawing, scode, orginalFill, originalStroke, transfer) {
    if ((basket.length < basketMax) || existsInBasket(scode)) {
        if (seatObj.attr('a') != "." || seatObj.attr('a') != ".2") {
            seatInfo = "";
            seatInfo = stand + "/" + seatObj.attr('r') + "/" + seatObj.attr('n');
            tempstandAndArea = stand.split("-");
            //checks if in transfer mode
            gTransfer = transfer;
            var seatPrice = "";
            if (seatObj.attr('p')) {
                seatPrice = seatObj.attr('p');
            }
            if (!transfer) {
                if (!existsInBasket(scode)) {
                    seatDrawing.attr("fill", seatSelectedFill);
                    seatDrawing.attr("stroke", seatSelectedStroke);
                    var basketItem = {
                        SeatInfo: seatInfo,
                        Price: seatPrice,
                        PriceBreakId: seatObj.attr('pb'),
                        Stand: tempstandAndArea[0],
                        Area: tempstandAndArea[1],
                        Row: seatObj.attr('r'),
                        Seat: seatObj.attr('n'),
                        PriceBand: "",
                        AvailabilityCode: seatObj.attr('a')
                    };
                    basket.push(basketItem);

                    updateNumberOfSelectedSeats();
                    // Order elements
                    sortResults('SeatInfo', true);
                    addRow(basketItem);
                } else {
                    seatDrawing.attr("fill", orginalFill);
                    seatDrawing.attr("stroke", originalStroke);
                    var index = basket.map(function (d) { return d['SeatInfo']; }).indexOf(seatInfo);
                    basket.splice(index, 1);      
                    updateNumberOfSelectedSeats();

                    // delete all items from table
                    var miniBasketContent = $(".c-price-band-selection .row");
                    miniBasketContent.each(function () {
                        $(this).remove();
                    });
                    miniBasketContent = $(".c-price-band-selection .c-price-band-selection__block");
                    miniBasketContent.each(function () {
                        $(this).remove();
                    });

                    // re populate the table with basket items
                    for (var i = 0; i < basket.length; i++) {
                        if (i <= 10) {
                            addHTMLRowItemToMiniBasketTable(basket[i]);
                        }
                    }
                    displayedStandAndAreaBasket = [];
                }
            } else {
                if (existsInBasket(scode)) {
                    seatDrawing.attr("fill", seatAvailableFill);
                    seatDrawing.attr("stroke", seatAvailableStroke);
                    var index = basket.map(function (d) { return d['SeatInfo']; }).indexOf(seatInfo);
                    basket.splice(index, 1);

                    updateSelectedSeatsHiddenField();
                    updateNumberOfSelectedSeats();
                    // delete all items from table
                    var miniBasketContent = $(".c-price-band-selection .row");
                    miniBasketContent.each(function () {
                        $(this).remove();
                    });

                    miniBasketContent = $(".c-price-band-selection .c-price-band-selection__block");
                    miniBasketContent.each(function () {
                        $(this).remove();
                    });

                    for (var i = 0; i < basket.length; i++) {
                        if (i < 10) {
                            addRow(basket[i]);
                        }
                    }
                } else {
                    seatDrawing.attr("fill", orginalFill);
                    seatDrawing.attr("stroke", originalStroke);
                    var basketItem = {
                        SeatInfo: seatInfo,
                        Price: seatPrice,
                        PriceBreakId: seatObj.attr('pb'),
                        Stand: tempstandAndArea[0],
                        Area: tempstandAndArea[1],
                        Row: seatObj.attr('r'),
                        Seat: seatObj.attr('n'),
                        PriceBand: "",
                        AvailabilityCode: seatObj.attr('a')
                    };
                    basket.push(basketItem);
                    updateNumberOfSelectedSeats();
                    if (basket.length > 10) {
                        if (use3DSeatView == true) {
                            var viewToLoad = 'S_' + tempstandAndArea[1] + '-' + seatObj.attr('r') + '-' + seatObj.attr('n');
                            tk3d.hasVisual(viewToLoad, function (hasView) {
                                if (hasView) {
                                    tk3d.load(viewToLoad);
                                }
                            })
                        }
                    } else {
                        tempstandAndArea = stand.split("-");
                        addRow(basketItem);
                        if (use3DSeatView == true) {
                            tk3d.load('S_' + tempstandAndArea[1]);
                        }
                    }
                }
            }
        }
    } else {
        if (basketRangeErrorText.length > 0)
            alert(basketRangeErrorText);
    }
}


// From the seat details populate the information on the page
function getSeatingDetails(seatDetails) {
    var seatingDataXMLDoc;
    if (window.DOMParser) {
        parser = new DOMParser();
        seatingDetailsXMLDoc = parser.parseFromString(seatDetails, "text/xml");
    }
    else // Internet Explorer
    {
        seatingDetailsXMLDoc = new ActiveXObject("Microsoft.XMLDOM");
        seatingDetailsXMLDoc.async = "false";
        seatingDetailsXMLDoc.loadXML(seatDetails);
    }
    $(seatingDetailsXMLDoc).find('s').each(function () {
        var $details = $(this);
        if ($details.attr('c')) {
            $(".ebiz-seat-details").show();
            $(".ebiz-restriction-details").show();
            document.getElementById("customer-number").innerHTML = $details.attr('c');
            document.getElementById("customer-forename").innerHTML = $details.attr('fn');
            document.getElementById("customer-surname").innerHTML = $details.attr('sn');
            document.getElementById("customer-address1").innerHTML = $details.attr('ad1');
            document.getElementById("customer-address2").innerHTML = $details.attr('ad2');
            document.getElementById("customer-address3").innerHTML = $details.attr('ad3');
            document.getElementById("season-ticket").innerHTML = $details.attr('sts');
            document.getElementById("book-number").innerHTML = $details.attr('stn');
            document.getElementById("restriction-details").innerHTML = $details.attr('rd');
            document.getElementById("restriction-code").innerHTML = $details.attr('rc');
            document.getElementById("restriction-description").innerHTML = $details.attr('rds');
        } else {
            $(".ebiz-seat-details").hide();
            $(".ebiz-restriction-details").show();
            document.getElementById("restriction-details").innerHTML = $details.attr('rd');
            document.getElementById("restriction-code").innerHTML = $details.attr('rc');
            document.getElementById("restriction-description").innerHTML = $details.attr('rds');
        }
        if ($details.attr('resd')) {
            $(".ebiz-customer-reservation-time").show();
            document.getElementById("reserved-date").innerHTML = $details.attr('resd');
            document.getElementById("reserved-time").innerHTML = $details.attr('rest');
        }

    });
}


// determines whether a seat exists in the basket by seat code
function existsInBasket(scode) {
    for (var i = 0; i < basket.length; i++) {
        if (basket[i].SeatInfo == scode)
            return true;
    }
    return false;
}


//Perform the redirect to the login page
function registerLoginRedirect() {
    window.location.href = loginLink;
}


// buys seat (redirects to gateway) and orphan validation check
function buyOnClick() {
    if (document.getElementById("hdfIsBuyButtonClicked").defaultValue == "False") {
        if (basket.length > basketMax || basket.length < basketMin) {
            alert(BasketRangeErrorText);
            return false;
        }
        if (basket.length == 0) {
            alert(noSeatsSelected);
            return false;
        }
        if (isSeasonTicket && isAnonymous) {
            $('#login-register-prompt').foundation('open');
            return false;
        } else {
            // javascript orphan seat validation
            var orphanValidation = true;
            if (orphanSeatFlag == true && standPassedOrphanBenchmark == true) {
                var i = 0;
                orphanValidation = true;
                while (i < basket.length) {
                    var tempScode = basket[i].SeatInfo.split("/");
                    $(seatingAreaXML).find('s[n|="' + tempScode[2] + '"][r|="' + tempScode[1] + '"]:first').each(function () {
                        var $seat = $(this);
                        var resultObj = validate(seatingAreaXML, $seat);
                        if (resultObj == false) {
                            orphanValidation = false;
                        }
                    });
                    if (orphanValidation == false) {
                        break;
                    }
                    i++;
                }
            }
            // Ok to buy the seats
            if (orphanValidation) {
                updateSelectedSeatsHiddenField();
                document.getElementById("hdfIsBuyButtonClicked").defaultValue = "True";
                return true;
            } else {
                alert(orphanedSeatErrorText);
                return false;
            }
        }
    }
}


// Validate the seat the seating area XML
function validate(seatingAreaXML, seat) {
    seatValidationResult = {
        leftSeat1Status: true,
        leftSeat2Status: true,
        rightSeat1Status: true,
        rightSeat2Status: true,
        message: ""
    };
    var currentIndex = seat.index();
    var occuSeat = $(seatingAreaXML).find('s').eq(currentIndex - 1);
    if (!isSeatAvailable(seat, occuSeat, false, true)) {
        seatValidationResult.leftSeat1Status = false;
    }
    occuSeat = $(seatingAreaXML).find('s').eq(currentIndex - 2);
    if (!isSeatAvailable(seat, occuSeat, true, true)) {
        seatValidationResult.leftSeat2Status = false;
    }
    if (seatValidationResult.leftSeat1Status == true && seatValidationResult.leftSeat2Status == false) {
        return false;
    }
    occuSeat = $(seatingAreaXML).find('s').eq(currentIndex + 1);
    if (!isSeatAvailable(seat, occuSeat, false, false)) {
        seatValidationResult.rightSeat1Status = false;
    }
    occuSeat = $(seatingAreaXML).find('s').eq(currentIndex + 2);
    if (!isSeatAvailable(seat, occuSeat, true, false)) {
        seatValidationResult.rightSeat2Status = false;
    }
    if (seatValidationResult.rightSeat1Status == true && seatValidationResult.rightSeat2Status == false) {
        return false;
    }
    return true;
    seatValidationResult.message = "validated";
}


// central to orphan validation
function isSeatAvailable(currentSeat, occuSeat, isLast, left) {
    if (occuSeat.attr('n')) {
        var countCheckNo = 3;
        var isSafe = false;
        var safeCount = 0;
        var avilll = occuSeat.attr('a');
        var occuScode = stand + "/" + occuSeat.attr('r') + "/" + occuSeat.attr('n');
        var occuIndex = occuSeat.index();
        var currIndex = currentSeat.index();
        var xDist = parseInt(currentSeat.attr('x')) - parseInt(occuSeat.attr('x'));
        var xDistVal = 26;
        var currentAjustedby1;
        var currentAjustedby2;
        if (left) {
            currentAjustedby1 = currIndex - 1;
            currentAjustedby2 = currIndex - 2;
        } else {
            currentAjustedby1 = currIndex + 1;
            currentAjustedby2 = currIndex + 2;
        }

        if (isLast) {
            xDistVal = 51;
        }
        if (xDist < 0)
            xDist = xDist * -1;


        //not separated by a gangway
        if (xDist < xDistVal) {

        } else {
            //accounts for eq inability to index out of range on negative index values
            if (isLast) {
                if (currentAjustedby2 != occuIndex)
                    return false;
            } else {
                if (currentAjustedby1 != occuIndex)
                    return false;
            }
        }

        //the same row
        if (currentSeat.attr('r') == occuSeat.attr('r')) {

            // not in basket array
            if (!existsInBasket(occuScode)) {
                safeCount++;
            }
            // not in .net basket
            if (occuSeat.attr('a') != "B") {
                safeCount++;
            }
            // available
            if (occuSeat.attr('a') == "A" || occuSeat.attr('a') == "X") {
                safeCount++;
            }

            if (safeCount == countCheckNo)
                return true;
            else
                return false;

        } else {
            return false;
        }
    } else {
        return false;
    }
}


// Updates Quick buy visual representation of tickets (ticket number)
function qBuyTicketQuant(sign) {
    var count = document.getElementById("quick-buy-quantity").innerHTML;
    if (sign === "add")
        count++;
    else
        count--;
    if (count < 0) { count = 0; }
    if (count > basketMaxQuickBuy) {
        count = basketMax;
        if (basketRangeErrorText.length > 0)
            alert(basketRangeErrorText);
    }
    if (count < basketMin) {
        count = basketMin;
        if (basketRangeErrorText.length > 0)
            alert(basketRangeErrorText);
    }
    document.getElementById("quick-buy-quantity").innerHTML = count;
    document.getElementById("hdfQuickBuyQuantity").defaultValue = count;
}


// Clears all selected seats - reloads seatingAreaCanvas via AJAX call if not in quick buy mode
function ClearSeatsClick() {
    if (!isQuickBuy) {
        var standAndArea;
        document.getElementById("hdfSelectedSeats").defaultValue = "";
        document.getElementById("btnMultiSelect").value = multiSelectOnText;
        // delete all items from table
        var miniBasketContent = $(".c-price-band-selection .row");
        miniBasketContent.each(function () {
            $(this).remove();
        });

        miniBasketContent = $(".c-price-band-selection .c-price-band-selection__block");
        miniBasketContent.each(function () {
            $(this).remove();
        });
        
        if (!isSeatSelectionOnly) {
            if (standAreaId != undefined) {
                var standArea = stadiumCanvas.getById(standAreaId);
                standAndArea = standArea.data("id");
            } else {
                if (areaId != undefined) {
                    var area = stadiumCanvas.getById(areaId);
                    standAndArea = area.data("id");
                } else {
                    standAndArea = document.getElementById('hdfStandAndAreaCode').defaultValue;
                }
            }
            basket = [];
            if (!isStadiumExpaned) {
                //re-create the seat selection using the filter options 
                var standAndAreaOptions = GetAPISettingsFromStandAndAreaOptions(false, false, false, false, false, false, false);
                createSeatSelection(stand, standAndAreaOptions);
            }
        } else {
            basket = [];
            createSeatSelection(document.getElementById('hdfStandAndAreaCode').defaultValue, undefined);
        }
        updateNumberOfSelectedSeats();
    } else {
        document.getElementById("quick-buy-quantity").innerHTML = basketMin;
        basket = [];
    }
}


// Removes the seatingAreaCanvas and displays the Quick Buy table
function createQuickBuy(standAndAreaCode) {
    // removes canvas if one already exists
    $("#quick-buy-option").show();
    document.getElementById("quick-buy-quantity").innerHTML = basketMin;
    document.getElementById("hdfQuickBuyStandAreaCode").defaultValue = standAndAreaCode;
    stadiumExpand();
    return true;
}


// Legend Toggle
$(function () {
    $('.ebiz-toggle-legend').click(function () {
        var iteration = $(this).data('iteration') || 1
        switch (iteration) {
            case 1:
                $(this).parent().animate({ right: '-9' }, { queue: false, duration: 200 });
                $(this).children('i').toggleClass("fa-chevron-left fa-chevron-right");
                $(this).toggleClass("ebiz-open ebiz-closed");
                break;

            case 2:
                $(this).parent().animate({ right: '-294' }, { queue: false, duration: 200 });
                $(this).children('i').toggleClass("fa-chevron-right fa-chevron-left");
                $(this).toggleClass("ebiz-closed ebiz-open");
                break;
        }
        iteration++;
        if (iteration > 2) iteration = 1
        $(this).data('iteration', iteration)
    })
});


// Clear the quick buy box
function clearQuickBuy() {
    $("#quick-buy-option").hide();
    document.getElementById("quick-buy-quantity").innerHTML = basketMin;
    document.getElementById("hdfQuickBuyQuantity").defaultValue = 0;
}


// Add the transfered seat to the basket items
function addTransferableBasketItemsToSeatList(BasketItems) {
    var tempString = BasketItems.split(">");
    for (var i = 0; i < tempString.length; i++) {
        var itemT = tempString[i];
        var tempBasketItemsString = itemT.split("/");
        var tempStringStandArea = tempBasketItemsString[0].split("-");
        var transferSeatInfo = tempBasketItemsString[0] + "/" + tempBasketItemsString[1] + "/" + tempBasketItemsString[2];
        var transferbasketItem = {
            SeatInfo: transferSeatInfo, Price: "",
            PriceBand: "",
            PriceBreakId: "",
            Stand: tempStringStandArea[0], Area: tempStringStandArea[1],
            Row: tempBasketItemsString[1], Seat: tempBasketItemsString[2]
        };
        basket.push(transferbasketItem);
        if (basket.length < 11) {
            addRow(transferbasketItem);
        }
    }
    updateNumberOfSelectedSeats();
}


//Update the number of selected seats based on the basket object length and show or hide the selected seats panel
function updateNumberOfSelectedSeats()
{
    if (basket.length > 0) {
        $("#detailed-seat-list-panel").show();
    } else {
        $("#detailed-seat-list-panel").hide();
    }
    $(".c-price-band-selection__items-total").text(basket.length);
}


// Functionality to show or hide the row and seat numbers on the area
function rowSeatCodesOnClick() {
    if (isRowSeatOnSVG) {
        isRowSeatOnSVG = false;
        document.getElementById("btnRowSeatCodes").value = rowSeatRowOnSVGOnText;
    } else {
        isRowSeatOnSVG = true;
        document.getElementById("btnRowSeatCodes").value = rowSeatRowOnSVGOffText;
    }
    //re-create the seat selection using the filter options 
    var standAndAreaOptions = GetAPISettingsFromStandAndAreaOptions(false, false, false, false, false, false, false);
    createSeatSelection(stand, standAndAreaOptions);
}


// Trim the leading zeros from the seat number
function trimStartZerosfromSeat(seatNumber) {
    var result = seatNumber;
    for (var i = 0, len = seatNumber.length; i < len; i++) {
        result = seatNumber.slice(i, seatNumber.length);
        var dChar = result.charAt(0);
        if (dChar != "0") {
            break;
        }
    }
    return result;
}

function ReDrawStandAndArea(standAndAreaCode) {
    document.getElementById('hdfStandAndAreaCode').value = standAndAreaCode;
    createSeatSelection(document.getElementById('hdfStandAndAreaCode').defaultValue, undefined);
}