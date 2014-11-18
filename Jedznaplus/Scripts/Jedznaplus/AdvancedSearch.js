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


$(function () {
    $('#frm').submit(function (e) {
        var excludeIngred = $('#excludeingredientEditor li INPUT[type = "text"]')

        if (excludeIngred.length > 0)
        {
            excludeIngred.each(function () {
                if ($(this).val().trim() == "")
                {
                    alert("Nie może istnieć wykluczoy składnik bez nazwy. Dodaj nazwę lub usuń niepotrzebne pole.");
                    e.preventDefault();
                }
                });

        }
    });
});

