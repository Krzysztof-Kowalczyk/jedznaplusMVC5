$(function () {
    $("#addAnother").click(function () {
        $.get('/Recipes/IngredientEntryRow', function (template) {
            $("#ingredientEditor").append(template);
        })
        .fail(function () {
            alert("error");
        })
    });
});