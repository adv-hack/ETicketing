var ValidationTypes = { "RFV": 1, "MinLV": 2, "MaxLV": 3, "REV": 4, "Num": 5, "MinV": 6, "MaxV": 7 }; // list of validation supported on client-side

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var queryParams = location.search.toUpperCase();
    name = name.toUpperCase();
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
		results = regex.exec(queryParams);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
};

function getHostPath() {
    var currentUrl, path;
    currentUrl = document.location.href.toString();
    path = currentUrl.substring(0, currentUrl.lastIndexOf("/"));
    return path;
};

function validate() {
    var arrHidden, minL, maxL, re, isMandatory, hasRE, hasNum, hasMinL, hasMaxL, hasMin, hasMax, min, max, controlID, control, controlType, hasError, tabToOpen;
    hasError = false;
    tabToOpen = "";

    try {
        /* hide all previously shown errors */
        $("span[type='err']").hide();

        /* Read all hidden fields having validations flags set to "1" */
        arrHidden = $("input[hasRFV='1'],input[hasNum='1'],input[hasRE='1'],input[hasMinL='1'],input[hasMaxL='1'],input[hasMin='1'],input[hasMax='1']");

        if (arrHidden.length > 0) {
            for (var i = 0; i < arrHidden.length; i++) {
                //reset
                isMandatory = false;
                hasMinL = false;
                hasMaxL = false;
                hasNum = false;
                hasRE = false;
                hasMin = false;
                hasMax = false;
                minL = null;
                maxL = null;
                min = null;
                max = null;
                re = null;
                controlID = getControlID(arrHidden[i].getAttribute("id"));
                control = document.getElementById(controlID);
                controlType = control.getAttribute("type");

                if (arrHidden[i].getAttribute("hasRFV")) {
                    if (arrHidden[i].getAttribute("hasRFV") == "1") {
                        isMandatory = true;
                    }
                }

                if (arrHidden[i].getAttribute("hasNum")) {
                    if (arrHidden[i].getAttribute("hasNum") == "1") {
                        hasNum = true;
                    }
                    if (arrHidden[i].getAttribute("hasMin")) {
                        if (arrHidden[i].getAttribute("hasMin") == "1") {
                            hasMin = true;
                            min = parseInt(arrHidden[i].getAttribute("Min"));
                        }
                    }
                    if (arrHidden[i].getAttribute("hasMax")) {
                        if (arrHidden[i].getAttribute("hasMax") == "1") {
                            hasMax = true;
                            max = parseInt(arrHidden[i].getAttribute("Max"));
                        }
                    }
                }

                if (arrHidden[i].getAttribute("hasRE")) {
                    if (arrHidden[i].getAttribute("hasRE") == "1") {
                        hasRE = true;
                        re = new RegExp(arrHidden[i].getAttribute("RE"));
                    }
                }
                if (arrHidden[i].getAttribute("hasMinL")) {
                    if (arrHidden[i].getAttribute("hasMinL") == "1") {
                        hasMinL = true;
                        minL = parseInt(arrHidden[i].getAttribute("MinL"));
                    }
                }
                if (arrHidden[i].getAttribute("hasMaxL")) {
                    if (arrHidden[i].getAttribute("hasMaxL") == "1") {
                        hasMaxL = true;
                        maxL = parseInt(arrHidden[i].getAttribute("MaxL"));
                    }
                }

                // Required field validation(1)
                if (isMandatory) {
                    if ((controlType != "checkbox" && (!(control.value) || control.value == "")) || (controlType == "checkbox" && !control.checked)) {
                        showError(controlID, ValidationTypes.RFV);
                        hasError = true;
                        if (tabToOpen == "") {
                            tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                        }
                    }
                }

                if (hasError) { continue; } // don`t go forward for this control if you`ve already caught an error. Pick up next configuration

                // Regular Expression Validation (4)
                if (hasRE && (control.value) && control.value != "") {
                    if (!re.test(control.value)) {
                        showError(controlID, ValidationTypes.REV);
                        hasError = true;
                        if (tabToOpen == "") {
                            tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                        }
                    }
                }
                if (hasError) { continue; } // don`t go forward for this control if you`ve already caught an error. Pick up next configuration

                // If Regular Expression is ACTIVE, don`t check other validations 
                if (!hasRE && controlType == "text" || control.type == "textarea") {
                    //Numeric Validation (5)
                    if (hasNum && (control.value) && control.value != "") {
                        if (!$.isNumeric(control.value)) {
                            showError(controlID, ValidationTypes.Num);
                            hasError = true;
                            if (tabToOpen == "") {
                                tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                            }
                        }
                        if (hasError) { continue; } // don`t go forward for this control if you`ve already caught an error. Pick up next configuration

                        if (hasMin || hasMax && (control.value) && control.value != "") {
                            if (hasMin && parseInt(control.value) < min) {
                                showError(controlID, ValidationTypes.MinV, min, max);
                                hasError = true;
                                if (tabToOpen == "") {
                                    tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                                }
                            }
                            if (hasError) { continue; } // don`t go forward for this control if you`ve already caught an error. Pick up next configuration

                            if (hasMax && parseInt(control.value) > max) {
                                showError(controlID, ValidationTypes.MaxV, min, max);
                                hasError = true;
                                if (tabToOpen == "") {
                                    tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                                }
                            }
                            if (hasError) { continue; } // don`t go forward for this control if you`ve already caught an error. Pick up next configuration
                        }
                    }

                    // Length validation (2-3)
                    if (!hasRE && (hasMinL || hasMaxL) && (control.value) && control.value != "") {
                        if (control.value.length < minL) {
                            showError(controlID, ValidationTypes.MinLV, minL, maxL);
                            hasError = true;
                            if (tabToOpen == "") {
                                tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                            }
                        }
                        if (control.value.length > maxL) {
                            showError(controlID, ValidationTypes.MaxLV, minL, maxL);
                            hasError = true;
                            if (tabToOpen == "") {
                                tabToOpen = $("#" + arrHidden[i].getAttribute("id")).parents("section").attr("id");
                            }
                        }
                    }
                }
            }
        }
        if (control) { control.focus(); }
        if (tabToOpen != "") {
            $("#lnk" + tabToOpen).click();
        }
    }
    catch (ex) {
        alert("error occurred: \n" + ex.message); // Unhandled exception occurred. Please contact administrator
        hasError = true;
    }
    return (hasError ? false : true); // if there is an error return "false" --> return !hasError
};

function getControlID(obj) {
    return obj.replace("hf_", "");
};

function showError(controlId, errorType, min, max) {
    var errorControlID, errorMsg, controlName;
    var errMsgRequiredFieldValidation = $("#hdnMsgRequiredFieldValidation").val();
    var errMsgLengthValidationMinL = $("#hdnMsgMinLengthValidation").val();
    var errMsgLengthValidationMaxL = $("#hdnMsgMaxLengthValidation").val();
    var errMsgRegularExpression = $("#hdnMsgREValidation").val();
    var errMsgNumericValidation = $("#hdnMsgNumericValidation").val();
    var errMsgMaxValueValidation = $("#hdnMsgMaxValueValidation").val();
    var errMsgMinValueValidation = $("#hdnMsgMinValueValidation").val();

    errorControlID = "#err_" + controlId;
    controlName = $("label[for='" + controlId + "']").text();

    switch (errorType) {
        case 1:
            errorMsg = errMsgRequiredFieldValidation.replace("{0}", controlName);
            break;
        case 2:
            errorMsg = errMsgLengthValidationMinL.replace("{0}", controlName).replace("{1}", min);
            break;
        case 3:
            errorMsg = errMsgLengthValidationMaxL.replace("{0}", controlName).replace("{1}", max);
            break;
        case 4:
            errorMsg = errMsgRegularExpression.replace("{0}", controlName);
            break;
        case 5:
            errorMsg = errMsgNumericValidation.replace("{0}", controlName);
            break;
        case 6:
            errorMsg = errMsgMinValueValidation.replace("{0}", min);
            break;
        case 7:
            errorMsg = errMsgMaxValueValidation.replace("{0}", max);
            break;
        default:
            errorMsg = "";
    }
    $(errorControlID).text(errorMsg);
    $(errorControlID).show();
};
