//Variables required between functions
var _currentTicketNumber = '';
var _currentTicketNumberCSV = '';
var _currentMembershipRFID = '';
var _currentMembershipMagScan = '';
var _currentMembershipMetalBadge = '';
var _nextTabIndex = '';
var _scanSeriesBarcodePrefix = '';
var _scanSeriesFirstTicketNumberId = '';
var _scanSeriesFirstIndex = '';
var _scanSeriesLastSequenceValue = '';
var _isMarkAsSent = false;
var _isMarkAsNotForDespatch = false;
var existingCardNumber;


window.onload = function () {
    $(".print-button").hide();
    $(".ebiz-print-label-confirm").hide();
    if (document.getElementById('txtMembershipRFID'))
        existingCardNumber = document.getElementById('txtMembershipRFID').value;
}

//Validate at least one search option has been entered
function validateSearchOptions(sender, args) {
    args.IsValid = false;
    if (document.getElementById('txtDespatchNoteID').value.length > 0) {
        args.IsValid = true;
    }
    if (document.getElementById('txtPaymentReference').value.length > 0) {
        args.IsValid = true;
    }
    if (document.getElementById('txtTicketNumber').value.length > 0) {
        args.IsValid = true;
    }
}


//Tab on enter functionality and re-focus when error occurs on ticket number fields.
function tabOnEnter(field, evt, current_csvTicketNumber, current_txtTicketNumber) {
    if (evt.keyCode === 13) {
        if (evt.preventDefault) {
            evt.preventDefault();
        } else if (evt.stopPropagation) {
            evt.stopPropagation();
        } else {
            evt.returnValue = false;
        }

        var nextElement = $('[tabindex="' + (document.getElementById(current_txtTicketNumber).tabIndex + 1) + '"]');
        if (nextElement.length) {
            nextElement.focus();
        } else {
            $('[tabindex="1"]').focus();
        }

        if (document.getElementById(current_csvTicketNumber).style.visibility == 'visible') {
            var ticketNumber = document.getElementById(current_txtTicketNumber);
            ticketNumber.focus();
        }

        return false;
    } else {
        return true;
    }
}


//Tab on enter functionality and re-focus when error occurs on ticket number fields.
function scanSeriestabOnEnter(field, evt, current_csvTicketNumber, current_txtTicketNumber) {
    if (evt.keyCode === 13) {
        if (evt.preventDefault) {
            evt.preventDefault();
        } else if (evt.stopPropagation) {
            evt.stopPropagation();
        } else {
            evt.returnValue = false;
        }

        var nextElement = $('[tabindex="' + (document.getElementById(current_txtTicketNumber).tabIndex + 1) + '"]');
        if (nextElement.length) {
            nextElement.focus();
        } else {
            $('[tabindex="30001"]').focus();
            $('[tabindex="30002"]').focus();
        }

        if (document.getElementById(current_csvTicketNumber).style.visibility == 'visible') {
            var ticketNumber = document.getElementById(current_txtTicketNumber);
            ticketNumber.focus();
        } else {
            closeScanSeries();
        }
        return false;
    } else {
        return true;
    }
}


//Set the current ticket number value onblur of a Ticket Number field
function setCurrentTicketNumber(id, csvId) {
    _currentTicketNumber = id;
    _currentTicketNumberCSV = csvId;
}


//Set the not for despatch status
function markAsNotForDespatch(textValue) {
    document.getElementById(_currentTicketNumber).value = textValue;
    var csvTicketNumber = document.getElementById(_currentTicketNumberCSV);
    var args = Object;
    args.Value = document.getElementById(_currentTicketNumber).value;
    args.IsValid = true;
    _isMarkAsNotForDespatch = true;
    if (!validateTicketNumber(csvTicketNumber, args)) {
        document.getElementById(_currentTicketNumberCSV).style.visibility = 'visible';
    }
    _isMarkAsNotForDespatch = false;
}


//Set the sent status
function markAsSent(textValue) {
    document.getElementById(_currentTicketNumber).value = textValue;
    var csvTicketNumber = document.getElementById(_currentTicketNumberCSV);
    var args = Object;
    args.Value = document.getElementById(_currentTicketNumber).value;
    args.IsValid = true;
    _isMarkAsSent = true;
    if (!validateTicketNumber(csvTicketNumber, args)) {
        document.getElementById(_currentTicketNumberCSV).style.visibility = 'visible';
    }
    _isMarkAsSent = false;
}


//Move to next tab index
function skipNext() {
    var nextElement = $('[tabindex="' + (document.getElementById(_currentTicketNumber).tabIndex + 1) + '"]');
    if (nextElement.length) {
        nextElement.focus();
    } else {
        document.getElementsById(txtNotes).focus();
    }
}


//Move to previou tab index
function skipBack() {
    var nextElement = $('[tabindex="' + (document.getElementById(_currentTicketNumber).tabIndex - 1) + '"]');
    if (nextElement.length) {
        nextElement.focus();
    }
}

function hideMembershipOptions() {
    document.getElementById('membership-options').style.display = 'none';
}
//Show the membership fields
function showMembershipOptions(membershipRFID, membershipMagScan, membershipMetalBadge, ticketNumber, membershipLabel, printForDespatch) {
    if (document.getElementById('txtMetalBadgeNumber') && document.getElementById('txtMetalBadgeNumber') === document.activeElement) {
        document.getElementById('txtMetalBadgeNumber').onblur();
    }
    document.getElementById(ticketNumber).value = membershipLabel;
    _currentMembershipRFID = membershipRFID;
    _currentMembershipMagScan = membershipMagScan;
    _currentMembershipMetalBadge = membershipMetalBadge;
    document.getElementById('txtMembershipRFID').value = document.getElementById(membershipRFID).value;
    if (printForDespatch === "True") {
        $('.ebiz-scan-magscan').hide();
        document.getElementById('txtMembershipRFID').readOnly = true;
    } else {
        $('.ebiz-scan-magscan').show();
        document.getElementById('txtMembershipRFID').readOnly = false;
    }
    document.getElementById('txtMembershipMagScan').value = document.getElementById(membershipMagScan).value;
    document.getElementById('txtMetalBadgeNumber').value = document.getElementById(membershipMetalBadge).value;
    document.getElementById('membership-options').style.display = 'block';
    document.getElementById('txtMembershipRFID').focus();
}


//Move focus to Mag Scan field
function focusMagScan(txtMembershipRFID, txtMembershipMagScan) {
    document.getElementById(_currentMembershipRFID).value = txtMembershipRFID.value;
}


//Move focus to Metal Badge field
function focusMetalBadge(txtMembershipMagScan, txtMetalBadgeNumber) {
    document.getElementById(_currentMembershipMagScan).value = txtMembershipMagScan.value;
}


//Move focus back to next tab index
function focusNextTicketNumber(txtMetalBadgeNumber) {
    document.getElementById(_currentMembershipMetalBadge).value = txtMetalBadgeNumber.value;
    document.getElementById('membership-options').style.display = 'none';
}


//Ticket Number validation function
//Get the barcode prefix, validate it against the ticket number, de-dupe the scanned ticket fields.
//If ticket number is invalid clear the text box fail the args and play a sound
function validateTicketNumber(sender, args) {
    var txtTicketNumberId = sender.controltovalidate;
    var lblBarcodePrefixId = txtTicketNumberId.replace('txtTicketNumber', 'lblBarcodePrefix');

    var barcodePrefix = document.getElementById(lblBarcodePrefixId).innerHTML;
    var stand = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfStand')).value;
    var area = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfArea')).value;
    var row = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfRowNumber')).value;
    var seat = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfSeat')).value;
    var alphaSuffix = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfAlphaSuffix')).value;
    var seatValidationCode = document.getElementById(txtTicketNumberId.replace('txtTicketNumber', 'hdfSeatValidationCode')).value;

    var expressionString = '';
    var passesRegExValidation = false;
    var passesDuplicateTicketValidation = false;
    var passesSeatValidation = false

    if (seatValidationCode != null) {
        var ticketNumber = args.Value;
        var ticket = ticketNumber.split("-");
        var lastTicketPart = ticket[ticket.length - 1];

        switch (seatValidationCode) {
            case "0":
                if (lastTicketPart && lastTicketPart.length > 0) {
                    passesSeatValidation = true;
                }
                break;
            case "1":
                if (lastTicketPart && lastTicketPart.length > 1) {
                    if (lastTicketPart == area.trim() + '$' + row.trim() + seat.trim()) {
                        passesSeatValidation = true;
                    }
                }
                break;
            case "2":
                if (lastTicketPart && lastTicketPart.length > 1) {
                    if (lastTicketPart == row.trim() + seat.trim()) {
                        passesSeatValidation = true;
                    }
                }
                break;
            case "3":
                if (lastTicketPart && lastTicketPart.length > 1) {
                    if (lastTicketPart == seat) {
                        passesSeatValidation = true;
                    }
                }
                break;
        }
    }
    if (passesSeatValidation) {
        if (barcodePrefix.length > 0) {
            expressionString = '^(' + _membershipNumberLabel + '|' + barcodePrefix + '.*)$';
            var re = new RegExp(expressionString);
            var ticketNumber = args.Value;
            if (ticketNumber.match(re)) {
                passesRegExValidation = true;
            } else {
                passesRegExValidation = false;
            }
        } else {
            passesRegExValidation = true;
        }
    }


    if (passesRegExValidation) {
        if (_isMarkAsNotForDespatch || _isMarkAsSent) {
            //Only de-dupe the ticket if it is not "Not For Despatch" or "Sent"
            passesRegExValidation = true;
            passesDuplicateTicketValidation = true;
        } else {
            if (_isMarkAsNotForDespatch || _isMarkAsSent) {
                passesRegExValidation = true;
            }
            var txtTicketNumberObjects = document.getElementsByClassName('ebiz-ticket-number-field');
            var ticketNumberArray = [];
            for (var i = 0; i < txtTicketNumberObjects.length; i++) {
                if (txtTicketNumberObjects[i].value.length > 0) {
                    if (txtTicketNumberObjects[i].value != _membershipNumberLabel) {
                        ticketNumberArray.push(txtTicketNumberObjects[i].value);
                    }
                }
            }
            var sortedValues = ticketNumberArray.sort();
            var results = [];
            for (var i = 0; i < ticketNumberArray.length - 1; i++) {
                if (sortedValues[i + 1] == sortedValues[i]) {
                    results.push(sortedValues[i]);
                }
            }
            if (results.length > 0) {
                passesDuplicateTicketValidation = false;
            } else {
                passesDuplicateTicketValidation = true;
            }
        }
    } else {
        passesRegExValidation = false;
    }

    if (!passesRegExValidation || !passesDuplicateTicketValidation) {
        document.getElementById(sender.controltovalidate).value = '';
        document.getElementById(sender.controltovalidate).focus();
        document.getElementById('dummy').innerHTML = " <audio controls autoplay><source src=\"error.mp3\" type=\"audio/mpeg\">Your browser does not support the audio element.</audio> ";
        return args.IsValid = false;
    }
    else if (!passesSeatValidation) {
        document.getElementById(sender.controltovalidate).value = '';
        document.getElementById(sender.controltovalidate).focus();
        document.getElementById('dummy').innerHTML = " <audio controls autoplay><source src=\"error.mp3\" type=\"audio/mpeg\">Invalid seat.</audio> ";
    }
    else {
        return args.IsValid = true;
    }
}


//Validate the first in a series of tickets when scanning series
//Get the barcode prefix, validate it against the ticket number.
//If ticket number is invalid clear the text box fail the args and play a sound
function validateScanSeriesTicketNumber(sender, args) {
    var expressionString = '';
    var passesRegExValidation = false;

    if (_scanSeriesBarcodePrefix.length > 0) {
        expressionString = '^(' + _membershipNumberLabel + '|' + _scanSeriesBarcodePrefix + '.*)$';
        var re = new RegExp(expressionString);
        var ticketNumber = args.Value;
        if (ticketNumber.match(re)) {
            passesRegExValidation = true;
        } else {
            passesRegExValidation = false;
        }
    } else {
        passesRegExValidation = true;
    }

    if (passesRegExValidation == false) {
        document.getElementById(sender.controltovalidate).value = '';
        document.getElementById(sender.controltovalidate).focus();
        document.getElementById('dummy').innerHTML = " <audio controls autoplay><source src=\"error.mp3\" type=\"audio/mpeg\">Your browser does not support the audio element.</audio> ";
        return args.IsValid = false;
    } else {
        return args.IsValid = true;
    }
}


//Scan series option event function
function scanSeriesOpen(barcodePrefix, firstTicketNumberId, firstFieldIndex, currentTicketSequence, lastInSequenceValue) {
    _scanSeriesBarcodePrefix = barcodePrefix;
    _scanSeriesFirstTicketNumberId = firstTicketNumberId;
    _scanSeriesFirstIndex = firstFieldIndex;
    _scanSeriesLastSequenceValue = lastInSequenceValue - (currentTicketSequence - 1); //get the number of tickets can be scanned based on where the user has clicked
    document.getElementById('txtFirst').value = '';
    document.getElementById('txtLast').value = '';
    document.getElementById('csvFirstTicketNumber').style.display = 'none';
    document.getElementById('csvLastTicketNumber').style.display = 'none';
    document.getElementById('ticketLimitError').style.display = 'none';
}


//Close the scan series reveal modal and set all the ticket values
function closeScanSeries() {
    var lastTicketNumberId = '';
    var ticketNumberGenericId = 'SiteMasterBody_ContentPlaceHolder1_uscDespatchProcess_rptItemstoDespatch_txtTicketNumber_';
    var firstScannedTicketValue = document.getElementById('txtFirst').value;
    var firstScannedTicketNumberStr = firstScannedTicketValue.replace(_scanSeriesBarcodePrefix, '');
    var firstScannedTicketNumberInt = parseInt(firstScannedTicketNumberStr);
    var firstScannedTicketNumberLength = firstScannedTicketNumberStr.length;
    var lastScannedTicketValue = document.getElementById('txtLast').value;
    var lastScannedTicketNumberStr = lastScannedTicketValue.replace(_scanSeriesBarcodePrefix, '');
    var lastScannedTicketNumberInt = parseInt(lastScannedTicketNumberStr);
    var ticketsScannedRange = lastScannedTicketNumberInt - firstScannedTicketNumberInt

    if (!isNaN(ticketsScannedRange)) {
        if (ticketsScannedRange >= _scanSeriesLastSequenceValue) {
            document.getElementById('ticketLimitError').style.display = 'block';
            document.getElementById('txtLast').value = '';
            document.getElementById('txtLast').focus();
            document.getElementById('dummy').innerHTML = " <audio controls autoplay><source src=\"error.mp3\" type=\"audio/mpeg\">Your browser does not support the audio element.</audio> ";
        } else {
            document.getElementById('ticketLimitError').style.display = 'none';
            for (var i = 0; i < _scanSeriesLastSequenceValue; i++) {
                if (ticketsScannedRange < i) {
                    //if the user doesn't scan enough tickets leave the loop
                    break;
                } else {
                    var currentField = '';
                    var currentId = _scanSeriesFirstIndex + i;
                    var currentTicket = 0;
                    currentField = ticketNumberGenericId + currentId;
                    currentTicket = firstScannedTicketNumberInt + i;
                    document.getElementById(currentField).value = _scanSeriesBarcodePrefix + padLeft(currentTicket.toString(), firstScannedTicketNumberLength, "0");
                    lastTicketNumberId = currentField;
                }
            }
            $('#scan-series').foundation('close');
            var nextElement = $('[tabindex="' + (document.getElementById(lastTicketNumberId).tabIndex + 1) + '"]');
            if (nextElement.length) {
                nextElement.focus();
            } else {
                document.getElementById(lastTicketNumberId).focus();
            }
        }
    }


}


//Basic Pad Left function
function padLeft(nr, n, str) {
    return Array(n - String(nr).length + 1).join(str || '0') + nr;
}


//Confirm Message when completing a part despatched order
function confirmOrder(warningMessageText) {
    var hasEmptyTicketField = false;
    var hasFilledTicketField = false;
    var txtTicketNumberObjects = document.getElementsByClassName('ebiz-ticket-number-field');
    var hdfMembershipRFIDObjects = document.getElementsByClassName('ebiz-membership-rfid');
    var hdfMembershipLabel = document.getElementById('hdfMembershipLabel');
    for (var i = 0; i < txtTicketNumberObjects.length; i++) {
        if (txtTicketNumberObjects[i].value.length == 0) {
            hasEmptyTicketField = true;
        } else {
            if (hdfMembershipLabel.value == txtTicketNumberObjects[i].value) {
                // validation doesn't count on membership items
                if ($(hdfMembershipRFIDObjects[i].innerHTML).val() == "") {
                    hasEmptyTicketField = true;
                }
                else {
                    hasFilledTicketField = true;
                }
            }
            else {
                hasFilledTicketField = true;
            }
        }
    }
    if (hasEmptyTicketField && hasFilledTicketField) {
        return confirm(warningMessageText);
    } else {
        return true;
    }
}


//Logging functionality based on the click of "Finish Order"
function logConfirmButtonClick() {
    if (window.console && window.console.log) {
        console.log('**************PROCESSING ORDER STARTED****************');
        console.log('Finish order was clicked');
        console.log('Despatch Note ID entered: ' + document.getElementById('txtDespatchNoteID').value);
        console.log('Payment Reference entered: ' + document.getElementById('txtPaymentReference').value);
        console.log('Ticket Number entered: ' + document.getElementById('txtTicketNumber').value);
        console.log('List the ticket number scanned into the fields on the order');
        var txtTicketNumberObjects = document.getElementsByClassName('ebiz-ticket-number-field');
        var lblSeatObjects = document.getElementsByClassName('ebiz-seat-label');
        for (var i = 0; i < txtTicketNumberObjects.length; i++) {
            console.log('------Item: ' + i);
            if (lblSeatObjects[i].textContent.length > 0) {
                console.log('Seat details: ' + lblSeatObjects[i].textContent);
            }
            if (txtTicketNumberObjects[i].value.length > 0) {
                console.log('Ticket value entered: ' + txtTicketNumberObjects[i].value);
            }
        }
        console.log('Finished listing all the tickets, now send form data back to the server');
        console.log('**************PROCESSING ORDER ENDED****************');
    }
}

function CallTeamCardPrintWebAPI(inputModel, APIURL, requestHyperlink, confirmHyperlink, completeHyperlink, index, mode, hdfMembershipRFID) {
    var loadingimage = "#loading-image" + index;
    var errordivId = "#print-card-error-text" + index;
    $(errordivId).hide();

    $(loadingimage).show();
    $(requestHyperlink).hide();
    $(confirmHyperlink).hide();
    $.ajax({
        type: "POST",
        url: APIURL,
        data: JSON.stringify(inputModel),
        contentType: "application/json",
        dataType: "json",
        cache: false,
        success: function (viewModel) {
            viewModel = JSON.parse(viewModel.d);
            if (viewModel) {
                if (mode == "Y") {
                    teamCardPrintRequest(requestHyperlink, confirmHyperlink, index, viewModel);
                } else if (mode == "C") {
                    teamCardConfirmRequest(index, viewModel, hdfMembershipRFID, completeHyperlink, confirmHyperlink);
                } else if (mode == "X") {
                    teamCardRenewRequest(requestHyperlink, hdfMembershipRFID, completeHyperlink, index, viewModel);
                }
            }
            $(loadingimage).hide();
        },
        error: function (xhr, status) {
            $(loadingimage).hide();
            alert(status + '  ' + xhr.responseText);
        }
    });


}

function teamCardPrintRequest(requestHyperlink, confirmHyperlink, index, viewModel) {
    if (viewModel[0].ErrorFlag.trim() == "E") {
        var errordivId = "#print-card-error-text" + index;
        $(errordivId).show();
    } else {
        $(confirmHyperlink).show();
        $(requestHyperlink).hide();
    }
}

function teamCardConfirmRequest(index, viewModel, hdfMembershipRFID, completeHyperlink, confirmHyperlink) {
    if (viewModel[0].NewCardNumberID.trim().length > 0) {
        document.getElementById("txtMembershipRFID").value = viewModel[0].NewCardNumberID;
        document.getElementById(hdfMembershipRFID).value = viewModel[0].NewCardNumberID;
        $(completeHyperlink).show();
    } else if (viewModel[0].ErrorFlag.trim() == "E") {
        var errordivId = "#card-number-retrieval-error-text" + index;
        $(confirmHyperlink).show();
        $(errordivId).show();
    }
}

function teamCardRenewRequest(requestHyperlink, hdfMembershipRFID, completeHyperlink, index, viewModel) {
    if (viewModel[0].NewCardNumberID.trim().length > 0) {
        document.getElementById("txtMembershipRFID").value = viewModel[0].NewCardNumberID;
        document.getElementById(hdfMembershipRFID).value = viewModel[0].NewCardNumberID;
        $(completeHyperlink).show();
    } else if (viewModel[0].ErrorFlag.trim() == "E") {
        var errordivId = "#card-number-retrieval-error-text" + index;
        $(errordivId).show();
        $(requestHyperlink).show();
    }
}

// Checks all or unchecks all print checkboxes
function selectPrintAll(isChecked) {
    selectAll(isChecked, '.ebiz-print-checkbox');
}