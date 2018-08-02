/*
    Validation variables for available buttons, error messages, and success-forwarding URLS.
*/
var printAllowed, cancelAllowed, transferAllowed, amendAllowed, seatHistoryAllowed, seatPrintHistoryAllowed = true;
var printAllowedErr, cancelAllowedErr, transferAllowedErr, amendAllowedErr, seatHistoryAllowedErr, seatPrintHistoryAllowedErr = "";
var cancelURL, transferURL, amendURL, seatHistoryURL, seatPrintHistoryURL = "";
var selectionMandatoryErr, singleSelectOnlyErr, singleSelectOnlyBulkHdrErr, singleSelectOnlyCorpHdrErr, fullCancelOnlyErr, maxSelectionLimitErr = "";
var fullCancelOnly = false;
var stopCodeWarning = true;
var stopCodeDiagTitle = stopCodeDiagMessage = stopCodeDiagOkBtnText = stopCodeDiagCancelBtnText = "";

/*
    Reset the validation variables
*/
function resetValidationVars() {
    printAllowed = cancelAllowed = transferAllowed = amendAllowed = seatHistoryAllowed = seatPrintHistoryAllowed = true;
    printAllowedErr = cancelAllowedErr = transferAllowedErr = amendAllowedErr = seatHistoryAllowedErr = seatPrintHistoryAllowedErr = "";
    cancelURL = transferURL = amendURL = seatHistoryURL = seatPrintHistoryURL = "";
}

/*
    Initial load of page - set inital button availability
*/
$(document).ready(function () {
    if (document.getElementById("hdfSelectionMandatoryErr")) {
        selectionMandatoryErr = document.getElementById('hdfSelectionMandatoryErr').value;
    }
    if (document.getElementById('hdfSingleSelectOnlyErr')) {
        singleSelectOnlyErr = document.getElementById('hdfSingleSelectOnlyErr').value;
    }
    if (document.getElementById('hdfsingleSelectOnlyCorpHdrErr')) {
        singleSelectOnlyCorpHdrErr = document.getElementById('hdfsingleSelectOnlyCorpHdrErr').value;
    }
    if (document.getElementById("hdfSingleSelectOnlyBulkHdrErr")) {
        singleSelectOnlyBulkHdrErr = document.getElementById('hdfSingleSelectOnlyBulkHdrErr').value;
    }
    if (document.getElementById('hdffullCancelOnly')) {
        fullCancelOnly = document.getElementById('hdffullCancelOnly').value.toLowerCase();
    }
    if (document.getElementById("hdffullCancelOnlyErr")) {
        fullCancelOnlyErr = document.getElementById('hdffullCancelOnlyErr').value;
    }
    if (document.getElementById("hdfmaxSelectionLimitErr")) {
        maxSelectionLimitErr = document.getElementById('hdfmaxSelectionLimitErr').value;
    }
    if (document.getElementById("hdfStopCodeDiagTitle")) {
        stopCodeDiagTitle = document.getElementById('hdfStopCodeDiagTitle').value;
    }
    if (document.getElementById("hdfStopCodeDiagMessage")) {
        stopCodeDiagMessage = document.getElementById('hdfStopCodeDiagMessage').value;
    }
    if (document.getElementById("hdfStopCodeOkBtnText")) {
        stopCodeDiagOkBtnText = document.getElementById('hdfStopCodeOkBtnText').value;
    }
    if (document.getElementById("hdfStopCodeCancBtnText")) {
        stopCodeDiagCancelBtnText = document.getElementById('hdfStopCodeCancBtnText').value;
    }
    if (document.getElementById("hdfCustomerIsOnStop")) {
        stopCodeWarning = document.getElementById('hdfCustomerIsOnStop').value.toLowerCase();
    }
    
    validateSelChkBox("", "", ".ebiz-item.select", ".ebiz-order-details");
});


/*
    Checkbox validation
*/
function validateSelChkBox(checkingChild, selectAllCBId, selectBoxClass, tableClass) {

    //First perform 'Select All' processing.
    if ((checkingChild == true) || (checkingChild == false)) {
        validateSelAllChkBox(checkingChild, selectAllCBId, selectBoxClass);
    }

    // Validate which options are available.
    resetValidationVars();
    validateActions(tableClass, selectBoxClass);

    if ($(".ebiz-print-selected").length) {
        setActionButton(".ebiz-print-selected", printAllowed, printAllowedErr);
    }
    if ($(".ebiz-cancel-selected").length) {
        setActionButton(".ebiz-cancel-selected", cancelAllowed, cancelAllowedErr);
    }
    if ($(".ebiz-transfer-selected").length) {
        setActionButton(".ebiz-transfer-selected", transferAllowed, transferAllowedErr);
    }
    if ($(".ebiz-amend-selected").length) {
        setActionButton(".ebiz-amend-selected", amendAllowed, amendAllowedErr);
    }
    if ($(".ebiz-seat-hist-selected").length) {
        setActionButton(".ebiz-seat-hist-selected", seatHistoryAllowed, seatHistoryAllowedErr);
    }
    if ($(".ebiz-seat-print-hist-selected").length) {
        setActionButton(".ebiz-seat-print-hist-selected", seatPrintHistoryAllowed, seatPrintHistoryAllowedErr);
    }
    if ($(".resend-email").length) {
        setActionButton(".resend-email", true, "");
    }
}

function okTest() {
    stopCodeWarning = false;
    $('.ebiz-print-selected')[0].click();
}

/*
    Button Click Validation
    Any changes here need to be reviewed also in
    PurchaseDetails.ascx.vb  : validateButtonAction()
*/
function validateButtonAction(buttonAction, selectBoxClass, tableClass) {

    // Validate which options are available.
    resetValidationVars();
    validateActions(tableClass, selectBoxClass);

    // Display any errors for the selected button.
    switch (buttonAction) {
        case "print":
            if (!printAllowed) {
                alertify.notify(printAllowedErr, '-default', 4);
                return false;
            }

            if (stopCodeWarning == "true") {
                alertify.confirm(stopCodeDiagTitle,
                                 stopCodeDiagMessage,
                                 okTest,
                                 function () { })
                    .set('labels', { ok: stopCodeDiagOkBtnText, cancel: stopCodeDiagCancelBtnText });
                return false;
            }
            
            break;
        case "cancel":
            if (!cancelAllowed) {
                alertify.notify(cancelAllowedErr, '-default', 4);
                return false;
            }
            break;
        case "transfer":
            if (!transferAllowed) {
                alertify.notify(transferAllowedErr, '-default', 4);
                return false;
            }
            break;
        case "amend":
            if (!amendAllowed) {
                alertify.notify(amendAllowedErr, '-default', 4);
                return false;
            }
            break;
        case "seatHist":
            if (!seatHistoryAllowed) {
                alertify.notify(seatHistoryAllowedErr, '-default', 4);
            } else {
                launchModal(seatHistoryURL, "ebiz-seat-hist-selected", "seat-history-modal");
            }
            return false;
        case "seatPrintHist":
            if (!seatPrintHistoryAllowed) {
                alertify.notify(seatPrintHistoryAllowedErr, '-default', 4);
            } else {
                launchModal(seatPrintHistoryURL, "ebiz-seat-print-hist-selected", "seat-print-history-modal");
            }
            return false; 
            
        case "email":
            break;
    }
    return true;
}


/*
    Main validation of selected options
    Loop through all checkboxes and determine which actions should be valid. Inc any error messages and forwarding URLS
*/
function validateActions(tableClass, selectBoxClass) {
    var corporateItemSelected = false;
    var bulkHdrItemSelected = false;

    $(tableClass).find('tr').each(function () {
        var row = $(this);
        if (row.find('input[type="checkbox"]').is(':checked')) {

            // return without processing if dealing with teh select-all header box
            var allowed;
            allowed = row.find('input[name$=hdfPrintAllowed]').val();
            if (!allowed) {
                return;
            }

            // Keep Track of Nay Bulk Header Items or Corporate Items selected
            if (!corporateItemSelected && !(row.find('input[name$=hdfCallID]').val() == "0")) {
                corporateItemSelected = true;
            }
            if (!bulkHdrItemSelected && !(row.find('input[name$=hdfBulkID]').val() == "0")) {
                bulkHdrItemSelected = true;
            }

            allowed = row.find('input[name$=hdfPrintAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                printAllowed = false;
                printAllowedErr = row.find('input[name$=hdfPrintAllowedError]').val();
            }

            allowed = row.find('input[name$=hdfCancelAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                cancelAllowed = false;
                cancelAllowedErr = row.find('input[name$=hdfCancelAllowedError]').val();
            }
            cancelURL = row.find('input[name$=hdfCancelHistURL]').val();

            allowed = row.find('input[name$=hdfTransferAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                transferAllowed = false;
                transferAllowedErr = row.find('input[name$=hdfTransferAllowedError]').val();
            }
            transferURL = row.find('input[name$=hdfTransferURL]').val();

            allowed = row.find('input[name$=hdfAmendAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                amendAllowed = false;
                amendAllowedErr = row.find('input[name$=hdfAmendAllowedError]').val();
            }
            amendURL = row.find('input[name$=hdfAmendURL]').val();

            allowed = row.find('input[name$=hdfSeatHistoryAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                seatHistoryAllowed = false;
                seatHistoryAllowedErr = row.find('input[name$=hdfSeatHistoryAllowedError]').val();
            }
            seatHistoryURL = row.find('input[name$=hdfSeatHistoryURL]').val();

            allowed = row.find('input[name$=hdfSeatPrintHistoryAllowed]').val();
            if (!allowed || !(allowed.toLowerCase() == "true")) {
                seatPrintHistoryAllowed = false;
                seatPrintHistoryAllowedErr = row.find('input[name$=hdfSeatPrintHistoryAllowedError]').val();
            }
            seatPrintHistoryURL = row.find('input[name$=hdfSeatPrintHistoryURL]').val();
        }
    });

    //Certain actions are single selection only.
    if (!(countCheckedBoxSelected(selectBoxClass) == 1)) {
        transferAllowed = false;
        amendAllowed = false;
        seatHistoryAllowed = false;
        seatPrintHistoryAllowed = false;
        transferAllowedErr = singleSelectOnlyErr;
        amendAllowedErr = singleSelectOnlyErr;
        seatHistoryAllowedErr = singleSelectOnlyErr;
        seatPrintHistoryAllowedErr = singleSelectOnlyErr;

        if (bulkHdrItemSelected) {
            cancelAllowed = false;
            cancelAllowedErr = singleSelectOnlyBulkHdrErr;
        }

        if (corporateItemSelected) {
            cancelAllowed = false;
            cancelAllowedErr = singleSelectOnlyCorpHdrErr;
        }
    }


    //500 individual item limit for cancellations.
    if (countCheckedBoxSelected(selectBoxClass) > 500) {
        cancelAllowed = false;
        cancelAllowedErr = maxSelectionLimitErr;
    }

    // All actions (apart from email) require at least 1 selection.
    if (!isCheckedBoxSelected(selectBoxClass)) {
        printAllowed = false;
        cancelAllowed = false;
        transferAllowed = false;
        amendAllowed = false;
        seatHistoryAllowed = false;
        seatPrintHistoryAllowed = false;
        printAllowedErr = selectionMandatoryErr;
        cancelAllowedErr = selectionMandatoryErr;
        transferAllowedErr = selectionMandatoryErr;
        amendAllowedErr = selectionMandatoryErr;
        seatHistoryAllowedErr = selectionMandatoryErr;
        seatPrintHistoryAllowedErr = selectionMandatoryErr;
    }

    // Full cancellation only is required for this order.
    if (fullCancelOnly == "true" && !areAllCheckBoxSelected(selectBoxClass)) {
        cancelAllowed = false;
        cancelAllowedErr = fullCancelOnlyErr;
    }
}

/*
    Set button class to unavailable/availble with error message hvioer text.
*/
function setActionButton(buttonClass, actionAllowed, actionError) {
    if (actionAllowed) {

        if ($(buttonClass).is(".ebiz-muted-action")) {
            $(buttonClass).foundation('destroy');
        }

        $(buttonClass).removeClass("ebiz-muted-action").addClass("ebiz-primary-action");
        $(buttonClass).attr("title", "");

    } else {
        $(buttonClass).removeClass("ebiz-primary-action").addClass("ebiz-muted-action");
        $(buttonClass).attr("title", actionError);
        $(buttonClass).removeAttr("data-open");

        var elem = new Foundation.Tooltip($(buttonClass));
    }
}

/*
  Manually launch a modal reveal form a button click.
  We need to strip out any viewstate data before rendering the HTML
*/
function launchModal(url, buttonClassName, modalName) {

    $("." + buttonClassName).attr("data-open", modalName);
    var modal = "#" + modalName;

    $.ajax(url).done(function (data) {
        var jHtmlObject = jQuery(data);
        var editor = jQuery("<p>").append(jHtmlObject);
        editor.find(".aspNetHidden").remove();
        var newHtml = editor.html();

        $(modal).html(newHtml).foundation();
    });
}
// validates the print icon only to avoid checkbox checks and other row centred validation
function validatePrintIcon(sender) {
    launchModal(sender.href, 'ebiz-open-modal', 'promotion-modal')
}