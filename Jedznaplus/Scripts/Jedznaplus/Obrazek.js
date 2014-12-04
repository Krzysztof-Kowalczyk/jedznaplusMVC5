$(function () {
    $('#bigImgLink').click(function (e) {
        e.preventDefault();
        var zrodlo = $(this).attr('href');
        $('#pokaz').attr('src', zrodlo);
        $("#przescieradlo").css('visibility', 'visible');

        $('#zamknij').click(function() {

            $("#przescieradlo").css('visibility', 'hidden');
        });
    });

});