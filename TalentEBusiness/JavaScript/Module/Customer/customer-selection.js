$(document).ready(function () {
    var talentClubs;
    var showCompanySearch = document.getElementById('hdfShowCompanySearch').value;
    if (showCompanySearch !== '' && showCompanySearch !== undefined && showCompanySearch == 'true') {
        $(".company-allocation").addClass("is-active");
        $("#company").addClass("is-active");
        $("#company").attr("aria-hidden", "false");
    } else {
        $(".customer-selection").addClass("is-active");
        $("#customer").addClass("is-active");
        $("#customer").attr("aria-hidden", "false");
    }
    // add to CSS
    $(".ebiz-customer-search-results").hide();
    $(".ebiz-company-search-results").hide();
    $("#clientside-errors-wrapper").hide();
    $("#customer-search-warning").hide();

    var showParentCompanySearch = Boolean(document.getElementById('hdfShowParentCompanySearch').value);
    if (showParentCompanySearch) {
        $("#search-tabs").hide();
    }

    var showSubsidaries = Boolean(document.getElementById('hdfShowSubsidaries').value);
    if (showSubsidaries) {
        $("#search-tabs").hide();
        processCompanySearch();
    }

});

$(".customer-selection").click(function () {
    // destroy company table to remove pagination buttons & information
    var companyTable = $(".ebiz-company-search-results").DataTable();
    companyTable.destroy();
    $(".ebiz-company-search-results").hide();
});
$(".company-allocation").click(function () {
    // destroy customer table to remove pagination buttons & information
    var customerTable = $(".ebiz-customer-search-results").DataTable();
    customerTable.destroy();
    $(".ebiz-customer-search-results").hide();
});

function getSessionID() {
    return document.getElementById('hdfSessionID').value;
}