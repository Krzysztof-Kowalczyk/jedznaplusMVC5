$(function () {
    $(".VoteSystem").bind("DOMSubtreeModified", function() {
        if ($('#rateWord:contains("Oceniłeś")').length != 0)
        {
            alert('changed');
        }
    });
    //  if ($("#Now").find("#rateWord").html().contains('Oceniłeś'))
    //  alert("lol");
});