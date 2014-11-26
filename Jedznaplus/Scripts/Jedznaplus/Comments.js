function clearContent() {
    $("#Content").val('');
    $('#dodajKomentarz').attr("disabled", "disabled");
}

$(function () {
    $("#Content").keyup(function () {
        if ($(this).val().trim() == '')
            $('#dodajKomentarz').attr("disabled", "disabled");
        else
            $('#dodajKomentarz').removeAttr("disabled");
    });
});

$(function () {
    $('#dodajKomentarz').attr("disabled", "disabled");

});