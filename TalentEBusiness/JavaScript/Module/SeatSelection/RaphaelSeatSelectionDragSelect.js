var box;
var selections;
var mat;
var multiSelectRectWidth = 0
var multiSelectRectHeight = 0

//DRAG FUNCTIONS
//when mouse goes down over background, start drawing selection box
function dragstart(x, y, event) {
    if (multiSelectOn === true) {
        //originalZoom
        zoomScaleFactor = zoomHeight / originalZoom;
        var xTemp = seatingAreaCanvas._viewBox[0];
        if (zoomScaleFactor > 0) {
            x += (seatingAreaCanvas._viewBox[0] / zoomScaleFactor);
            y += (seatingAreaCanvas._viewBox[1] / zoomScaleFactor);
        }
        box = seatingAreaCanvas.rect((x - $('#select').offset().left) * zoomScaleFactor, (y - $('#select').offset().top) * zoomScaleFactor, 0, 0);
        //if (isNaN(parseFloat((x - $('#select').offset().left) * zoomScaleFactor)) || isNaN(parseFloat((y - $('#select').offset().top) * zoomScaleFactor)))
        //    alert("! NaN error !");
    }
}

function multiSelectOnClick() {
    if (multiSelectOn === true) {
        multiSelectOn = false;
        document.getElementById("btnMultiSelect").value = multiSelectOffText;
        $(".ebiz-svg-container").removeClass("ebiz-multiselect-on");
        $(".ebiz-svg-container").addClass("ebiz-multiselect-off");
        if (mat)
            mat.remove();

    } else {
        // Correct ratio so the multi select drag rectangle covers the viewBox
        if (seatingAreaCanvasWidth > seatingAreaCanvasHeight) {
            multiSelectRectHeight = zoomHeight
            multiSelectRectWidth = (zoomHeight / seatingAreaCanvasHeight) * seatingAreaCanvasWidth
        } else {
            multiSelectRectWidth =  zoomWidth
            multiSelectRectHeight = (zoomWidth / seatingAreaCanvasWidth) * seatingAreaCanvasHeight
        }


        mat = seatingAreaCanvas.rect(xPOSVIEW, yPOSVIEW, multiSelectRectWidth, multiSelectRectHeight).attr("fill", "#FFF").attr("stroke-width", 0);
        mat.toBack();
        mat.drag(dragmove, dragstart, dragend);
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        multiSelectOn = true;
        $(".ebiz-svg-container").removeClass("ebiz-multiselect-off");
        $(".ebiz-svg-container").addClass("ebiz-multiselect-on");

        document.getElementById("btnMultiSelect").value = multiSelectOnText;
    }

}

//when mouse moves during drag, adjust box. If to left or above original point, you have to translate the whole box and invert the dx or dy values since .rect() doesn't take negative width or height
function dragmove(dx, dy, x, y, event) {
    if (multiSelectOn === true) {
        dx = dx * zoomScaleFactor;
        dy = dy * zoomScaleFactor;
        var xoffset = 0,
            yoffset = 0;
        if (dx < 0) {
            xoffset = dx;
            dx = -1 * dx;

        }
        if (dy < 0) {
            yoffset = dy;
            dy = -1 * dy;
        }

        box.transform("T" + xoffset + "," + yoffset);
        box.attr("width", dx);
        box.attr("height", dy);
    }
}



function dragend(event) {
    if (multiSelectOn === true) {     
        //get the bounds of the selections
        var bounds = box.getBBox();
        box.remove();

        $("#loading-image").show();
        for (var c in set.items) {
            //here, we want to get the x,y vales of each object regardless of what sort of shape it is, but rect uses rx and ry, circle uses cx and cy, etc
            //so we'll see if the bounding boxes intercept instead
            if (basket.length >= basketMax) {
                alert(ExceededBasketLimitText);
                break;
            } else {
                //                if (basket.length > multiSelectMax) {
                //                    alert(mulitSelectMaxText + multiSelectMax);
                //                    break;
                //                }
            }
            if (set[c]) {
                var mybounds = set[c].node.getBBox();
                //do bounding boxes overlap?
                //is one of this object's x extremes between the selection's xextremes?
                if (mybounds.y >= bounds.y && mybounds.y <= bounds.y2 || mybounds.y2 >= bounds.y && mybounds.y2 <= bounds.y2) {

                    //same for x
                    if (mybounds.x >= bounds.x && mybounds.x <= bounds.x2 || mybounds.x2 >= bounds.x && mybounds.x2 <= bounds.x2) {
                        var avail = set[c].data("avail");
                        var scode = set[c].data("seatCode");
                        var priceBreakId = set[c].data("priceBreakId");
                        if (!existsInBasket(scode) && avail != "." && avail != ".2" && avail != "B" && avail != "T") {
                            set[c].attr("fill", seatSelectedFill);
                            set[c].attr("stroke", seatSelectedStroke);
                            var tempstandAndArea = stand.split("-");
                            var tempScode = scode.split("/");
                            var basketItem = {
                                SeatInfo: scode, Price: "",
                                PriceBreakId: priceBreakId,
                                Stand: tempstandAndArea[0], Area: tempstandAndArea[1],
                                Row: tempScode[1], Seat: tempScode[2],
                                PriceBand: defaultPriceBand
                            };
                            basket.push(basketItem);

                            if (basket.length <= 10) {
                                addHTMLRowItemToMiniBasketTable(basketItem);
                            }

                            $("#btnBuy").show();
                            $("#btnClearSeats").show();
                            $("#detailed-seat-list-panel").show();
                        }
                    }
                }
            }
        }
        sortResults('SeatInfo', true);
        displayedStandAndAreaBasket = [];
        updateNumberOfSelectedSeats();
        $("#loading-image").hide();
    }
}
