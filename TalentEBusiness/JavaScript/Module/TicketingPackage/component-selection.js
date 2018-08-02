$(document).ready(function () {
    $(".compDiscSlider").each(function () {
        var val = $(this).next("input").val();
        $(this).slider({
            range: "min",
            animate: true,
            step: 1,
            min: 0.00,
            max: 100.00,
            value: val,
            slide: function (event, ui) {
                $(this).next("input").val(ui.value);
            }
        })
    });
    $(".compDiscTB").change(function () {
        $(this).prev().slider("value", parseFloat(this.value));
    });
    $('.compDiscTB').keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    $(".packageDiscSlider").each(function () {
        var val = $(this).next("input").val();
        $(this).slider({
            range: "min",
            animate: true,
            step: 1,
            min: 0.00,
            max: 100.00,
            value: val,
            slide: function (event, ui) {
                $(this).next("input").val(ui.value);
            }
        })
    });
    $(".packageDiscTB").change(function () {
        $(this).prev().slider("value", parseFloat(this.value));
    });
    $('.packageDiscTB').keydown(function(event){ 
        if(event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});