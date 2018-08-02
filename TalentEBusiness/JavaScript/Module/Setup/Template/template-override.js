
$(document).ready(function () {
    document.getElementById("hdfTemplateOverrideType").value = "Package"
    document.getElementById("createTemplateOverrideErrors").style = "display:none";
    document.getElementById("updateTemplateOverrideErrors").style = "display:none";
    document.getElementById("yes-no").checked = true;
    $(".js-non-product-specific-override").hide();
    $(".js-product-specific-override").show();
});

$(document).keyup(function (e) {
    if (e.keyCode == 27) { // escape key maps to keycode `27`
        clearControls();
    }
});

$('#yes-no').click(function () {
    if ($(this).is(':checked')) {
        $(".js-non-product-specific-override").hide();
        $(".js-product-specific-override").show();
        document.getElementById("hdfTemplateOverrideType").value = "Package"
    } else {
        $(".js-product-specific-override").hide();
        $(".js-non-product-specific-override").show();
        document.getElementById("hdfTemplateOverrideType").value = "Ticketing"
    }
    $(".js-select2--tags-no-creation").val('').trigger("change");  
    document.getElementById("createTemplateOverrideErrors").style = "display:none";
    document.getElementById("lblCreateTemplateOverrideError").innerHTML = "";
});

$('#yes-no-for-update').click(function () {
    if ($(this).is(':checked')) {
        $(".js-non-product-specific-override").hide();
        $(".js-product-specific-override").show();
        document.getElementById("hdfTemplateOverrideType").value = "Package"
    } else {
        $(".js-product-specific-override").hide();
        $(".js-non-product-specific-override").show();
        document.getElementById("hdfTemplateOverrideType").value = "Ticketing"
    }
});


function validateCreateTemplateOverride() {
    var overrideType = document.getElementById("hdfTemplateOverrideType").value
    var templateOverrideName = document.getElementById("txtTemplateOverrideDescription").value
    var productSubType = document.getElementById("ddlProductSubType");
    var productType = document.getElementById("ddlProductType");
    var product = document.getElementById("ddlProductGroup");
    var stadium = document.getElementById("ddlStadium");
    var hospitalityPackage = document.getElementById("ddlPackage");
    var emailTemplate = document.getElementById("ddlEmailConfirmation");
    var qAndATemplate = document.getElementById("ddlQAndATemplate");
    var dataCaptureTemplate = document.getElementById("ddlDataCaptureTemplate");
    var isValid = true;
    var isTemplateNameAvailable = true;
    document.getElementById("lblCreateTemplateOverrideError").innerHTML = "";

    if (overrideType == "Ticketing") {
        if (templateOverrideName.trim() == "") {
            isTemplateNameAvailable = false;
        }

        if (productSubType.value + productType.value + stadium.value == "") {
            isValid = false;
        }

        if (emailTemplate.value + qAndATemplate.value + dataCaptureTemplate.value == "") {
            isValid = false;
        }

        if ((!isValid) && (!isTemplateNameAvailable)) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfCreateTemplateOverrideErrorMessage").value + "<br\>" + document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else if (!isValid) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfCreateTemplateOverrideErrorMessage").value
        }
        else if (!isTemplateNameAvailable) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else {
            document.getElementById("createTemplateOverrideErrors").style = "display:none";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = "";
        }
    }

    if (overrideType == "Package") {

        if (templateOverrideName.trim() == "") {
            isTemplateNameAvailable = false;
        }

        if (product.value + hospitalityPackage.value == "") {
            isValid = false;
        }

        if (emailTemplate.value + qAndATemplate.value + dataCaptureTemplate.value == "") {
            isValid = false;
        }

        if ((!isValid) && (!isTemplateNameAvailable)) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfCreateTemplateOverrideErrorMessage").value + "<br\>" + document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else if (!isValid) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfCreateTemplateOverrideErrorMessage").value
        }
        else if (!isTemplateNameAvailable) {
            document.getElementById("createTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else {
            document.getElementById("createTemplateOverrideErrors").style = "display:none";
            document.getElementById("lblCreateTemplateOverrideError").innerHTML = "";
        }
    }

    if ((!isValid) || (!isTemplateNameAvailable))
        return false;
    else
    {
       $("#createTemplateOverrideModal").foundation('close');
       return true;
    }       
}

function setMessagesOnTemplateClick()
{
    var isSuccessMessage = document.getElementsByClassName("alert-box success");
    if (isSuccessMessage.length > 0) {
        document.getElementsByClassName("alert-box success")[0].style = "display:none";
    }
}

function addTemplateOverrideClick()
{
    var isSuccessMessage = document.getElementsByClassName("alert-box success");
    if (isSuccessMessage.length > 0) {
        document.getElementsByClassName("alert-box success")[0].style = "display:none";
    }
}

function clearControls() {
    document.getElementById("txtTemplateOverrideDescription").value = "";
    $(".js-select2--tags-no-creation").val('').trigger("change");
    $('.js-select2--default').val('').trigger("change");
    document.getElementById("createTemplateOverrideErrors").style = "display:none";
    document.getElementById("lblCreateTemplateOverrideError").innerHTML = "";
    document.getElementById("yes-no").checked = true;
    $(".js-non-product-specific-override").hide();
    $(".js-product-specific-override").show();
    document.getElementById("hdfTemplateOverrideType").value = "Package"
}


function populateUpdateModal(templateOverrideId, templateOverrideDescription, productPackageSpecific, productCriterias, packageCriteras, productSubTypeCriterias, productTypeCriterias, stadiumCriterias, saleConfirmationId, qAndATemplateId, dataCaptureTemplateId)
{
    setMessagesOnTemplateClick();
    document.getElementById("hdfTemplateOverrideId").value = templateOverrideId
    document.getElementById("txtTemplateDescriptionForEdit").value = templateOverrideDescription.trim();
    document.getElementById("yes-no-for-update").value = JSON.parse(productPackageSpecific); 
    $("#ddlEmailTemplateForEdit").val('').trigger("change");
    $("#ddlQandATemplateForEdit").val('').trigger("change");
    $("#ddlDataCaptureTemplateForEdit").val('').trigger("change");
    document.getElementById("updateTemplateOverrideErrors").style = "display:none";
    document.getElementById("lblUpdateTemplateOverrideError").innerHTML = ""
    if (productPackageSpecific != null)
    {
        if (productPackageSpecific == "true")
        {
            document.getElementById("yes-no-for-update").checked = true;
            $(".js-non-product-specific-override").hide();
            $(".js-product-specific-override").show();

            $("#ddlProductGroupForEdit").select2().val(productCriterias.split(',')).trigger("change")
            $("#ddlPackageForEdit").select2().val(packageCriteras.split(',')).trigger("change")
            $("#ddlProductSubTypeForEdit").select2().val('').trigger("change")
            $("#ddlProductTypeForEdit").select2().val('').trigger("change")
            $("#ddlStadiumForEdit").select2().val('').trigger("change")

            document.getElementById("hdfTemplateOverrideType").value = "Package"
        }
        else
        {
            document.getElementById("yes-no-for-update").checked = false;
            $(".js-product-specific-override").hide();
            $(".js-non-product-specific-override").show();
          
            $("#ddlProductSubTypeForEdit").select2().val(productSubTypeCriterias.split(',')).trigger("change")
            $("#ddlProductTypeForEdit").select2().val(productTypeCriterias.split(',')).trigger("change")
            $("#ddlStadiumForEdit").select2().val(stadiumCriterias.split(',')).trigger("change")
            $("#ddlProductGroupForEdit").select2().val('').trigger("change")
            $("#ddlPackageForEdit").select2().val('').trigger("change")
         
            document.getElementById("hdfTemplateOverrideType").value = "Ticketing"
        }
        
    }
    if (saleConfirmationId != null && saleConfirmationId != 0)
    {
        $("#ddlEmailTemplateForEdit").val(saleConfirmationId).trigger("change");
    }
   
    if (qAndATemplateId != null && qAndATemplateId != 0) {
        $("#ddlQandATemplateForEdit").val(qAndATemplateId).trigger("change");
    }

    if (dataCaptureTemplateId != null && dataCaptureTemplateId != 0) {
        $("#ddlDataCaptureTemplateForEdit").val(dataCaptureTemplateId).trigger("change");
    }
  
}

function validateUpdateTemplateOverride() {
    var overrideType = document.getElementById("hdfTemplateOverrideType").value
    var templateOverrideName = document.getElementById("txtTemplateDescriptionForEdit").value
    var product = document.getElementById("ddlProductGroupForEdit");
    var hospitalityPackage = document.getElementById("ddlPackageForEdit");
    var productSubType = document.getElementById("ddlProductSubTypeForEdit");
    var productType = document.getElementById("ddlProductTypeForEdit");
    var stadium = document.getElementById("ddlStadiumForEdit");
    
    var emailTemplate = document.getElementById("ddlEmailTemplateForEdit");
    var qAndATemplate = document.getElementById("ddlQandATemplateForEdit");
    var dataCaptureTemplate = document.getElementById("ddlDataCaptureTemplateForEdit");
    var isValid = true;
    var isTemplateNameAvailable = true;
    document.getElementById("lblUpdateTemplateOverrideError").innerHTML = "";

    if (overrideType == "Ticketing") {
        if (templateOverrideName.trim() == "") {
            isTemplateNameAvailable = false;
        }

        if (productSubType.value + productType.value + stadium.value == "") {
            isValid = false;
        }

        if (emailTemplate.value + qAndATemplate.value + dataCaptureTemplate.value == "") {
            isValid = false;
        }

        if ((!isValid) && (!isTemplateNameAvailable)) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfUpdateTemplateOverrideErrorMessage").value + "<br\>" + document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else if (!isValid) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfUpdateTemplateOverrideErrorMessage").value
        }
        else if (!isTemplateNameAvailable) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else {
            document.getElementById("updateTemplateOverrideErrors").style = "display:none";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = "";
        }
    }

    if (overrideType == "Package") {

        if (templateOverrideName.trim() == "") {
            isTemplateNameAvailable = false;
        }

        if (product.value + hospitalityPackage.value == "") {
            isValid = false;
        }

        if (emailTemplate.value + qAndATemplate.value + dataCaptureTemplate.value == "") {
            isValid = false;
        }

        if ((!isValid) && (!isTemplateNameAvailable)) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfUpdateTemplateOverrideErrorMessage").value + "<br\>" + document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else if (!isValid) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfUpdateTemplateOverrideErrorMessage").value
        }
        else if (!isTemplateNameAvailable) {
            document.getElementById("updateTemplateOverrideErrors").style = "display:block";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = document.getElementById("hdfTemplateNameRequiredErrorMessage").value;
        }
        else {
            document.getElementById("updateTemplateOverrideErrors").style = "display:none";
            document.getElementById("lblUpdateTemplateOverrideError").innerHTML = "";
        }
    }

    if ((!isValid) || (!isTemplateNameAvailable))
        return false;
    else {
        $("#editTemplateOverrideModal").foundation('close');
        return true;
    }
}