$(function () {
    $('form').submit(function (e) {
        var ingred = $('#ingredientEditor li INPUT[type = "text"]');
        var prepMethod = $('#PreparationMethod').val().trim();
        if (ingred.length == 0 && prepMethod == "") {
            alert("Lista składnikow  oraz sposób przygotowania nie mogą być puste");
            e.preventDefault();
        }
        else if (ingred.length != 0 && prepMethod == "") {
            alert("Musisz podać sposób przygotowania potrawy");
            e.preventDefault();
        }
        else if (ingred.length == 0 && prepMethod != "") {
            alert("Lista składników nie może byc pusta");
            e.preventDefault();
        }
        else if (ingred.length > 0 && prepMethod != "") {
            ingred.each(function () {
                if ($(this).val().trim() == "") {
                    alert("Nie może istnieć składnik bez nazwy i ilości. Wypełnij wymagane dane lub usuń niepotrzebne pole.");
                    e.preventDefault();
                }


            });

        }
    });
});
