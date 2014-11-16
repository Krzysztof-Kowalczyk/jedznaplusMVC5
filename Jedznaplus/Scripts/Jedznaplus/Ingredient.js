$(function () {
    $("#addAnother").click(function () {
        $.get('/Recipes/IngredientEntryRow', function (template) {
            $("#ingredientEditor").append(template);
        })
        .fail(function () {
            alert("error");
        })

        $('INPUT[type="submit"]').removeAttr('disabled');
    });
});

$(function () {
    $('form').submit(function (e) {
        var prepMethod = $("#PreparationMethod").val();
        var list = ($("#ingredientEditor li"));
        if(prepMethod=="" || list.length == 0)
        {

            alert("Pola Lista składników oraz Sposób przygotowania nie mogą być puste")
            e.preventDefault();
        }
    });
});

