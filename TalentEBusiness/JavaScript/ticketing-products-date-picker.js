$(document).ready(function () {
    $(".datepicker").datepicker({ beforeShowDay: setAvailableDays, dateFormat: 'dd/mm/yy' });
});
function setAvailableDays(date) {
    var isAvailable = false;
    if (availableDays != null) {
        for (i = 0; i < availableDays.length; i++) {
            if (date.getDate() == availableDays[i][1] && date.getMonth() == availableDays[i][0] - 1 && date.getFullYear() == availableDays[i][2]) {
                isAvailable = true;
            }
        }
    }
    if (isAvailable) return [true, 'open']; else return [false, 'closed'];
}