$(function () {
    $("#addAnother").click(function () {
        $.get('/Recipes/ExcludeIngredientEntryRow', function (template) {
            $("#excludeingredientEditor").append(template);
        })
        .fail(function () {
            alert("error");
        })

    });
});
