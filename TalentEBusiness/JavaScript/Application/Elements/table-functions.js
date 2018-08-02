var triggeredByChild = false;
var triggeredByParent = false;


// Checks all or unchecks all checkboxes
function selectAll(isChecked, selectBoxClass)
{
    if (!triggeredByChild) {
        var triggeredByParent = true;
    }

    if (isChecked) {
        var canUseiCheck = true;
        try {
            $(selectBoxClass).iCheck("check");
        }
        catch (e) {
            canUseiCheck = false;
        }
        if (canUseiCheck == false) {
            $(selectBoxClass + " input").prop("checked", true);
        }
    }
    else {
        if (!triggeredByChild) {
            var canUseiCheck = true;
            try {
                $(selectBoxClass).iCheck("uncheck");
            }
            catch (e) {
                canUseiCheck = false;
            }
            if (canUseiCheck == false) {
                $(selectBoxClass + " input").prop("checked", false);
            }
        }
    }
    triggeredByChild = false;
    triggeredByParent = false;
}


// Is the checkbox selected
function isCheckedBoxSelected(selectBoxClass)
{
    var numberOfCheckedItems = countCheckedBoxSelected(selectBoxClass);
    if (numberOfCheckedItems == 0) {
        return false
    }
    return true;
}


// Are all checkbox selected
function areAllCheckBoxSelected(selectBoxClass) {
    return ($(selectBoxClass + " input:checkbox:not(:checked)").length == 0);
}


// Count of checkboxes selected
function countCheckedBoxSelected(selectBoxClass) {
    return $(selectBoxClass + " input:checked").length;
}


// Removed the checked state from "All" if any checkbox is unchecked
// Check the 'selectall' checkbox if we have just manually selected the last individual one.
function validateSelAllChkBox(checkingChild, selectAllCBId, selectBoxClass) {
    if (!triggeredByParent) {
		var canUseiCheck = true;
        if (!checkingChild) {
            triggeredByChild = true;
			try {
            	$(selectAllCBId).iCheck('uncheck');
			} catch (e) {
				canUseiCheck = false;
			}
			if (canUseiCheck == false) {
            	$(selectAllCBId).prop("checked", false);
        	}
        }

        if (checkingChild) {
            if ($(selectBoxClass + " input:checked").length == $(selectBoxClass + " input:visible").length) {
				try {
                	$(selectAllCBId).iCheck('check');
				} catch (e) {
					canUseiCheck = false;
				}
				if (canUseiCheck == false) {
            		$(selectAllCBId).prop("checked", true);
        		}
            }
        }
    
    }
    
    if (checkingChild) {
        if ($(selectBoxClass + " input:checked").length == $(selectBoxClass + " input:visible").length) {
            var canUseiCheck = true;
            try {
                $(selectAllCBId).iCheck("check");
            }
            catch (e) {
                canUseiCheck = false;
            }
            if (canUseiCheck == false) {
                $(selectAllCBId).prop("checked", true);
            }
        }
    }
}