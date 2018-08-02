var checkboxList = [];

window.onload = function (e) {
    $('input[type=checkbox]').each(function (index) {
        checkboxList.push({isChecked: $(this).is(":checked") });
    });
}

window.onbeforeunload = function (e) {
    if (!isPageDirty())
        return "x";
}


function isPageDirty() {
    var isDirty = false;
    var currentCheckboxStateList = [];
    $('input[type=checkbox]').each(function (index) {
        var currentCheckboxStateItem = $(this);
        currentCheckboxStateList.push({ isChecked: $(this).is(":checked") });
    });
    if (currentCheckboxStateList.length > 0) {
        for (var c = 0; c < currentCheckboxStateList.length; c++) {
            if (currentCheckboxStateList[c].isChecked != checkboxList[c].isChecked) {
                isDirty = true;
                break;
            }
        }
    }
    return isDirty;
}