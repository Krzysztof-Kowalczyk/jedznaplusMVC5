$(function () {
    $('form').submit(function (e) {
        var ingred = $('#ingredientEditor li INPUT[type = "text"]');
        var prepMethod = $('#PreparationMethod').val().trim();

        if (ingred.length == 0 && prepMethod == "") {
            $("#validationJS").html("Lista składnikow  oraz sposób przygotowania nie mogą być puste");
            e.preventDefault();
        }
        else if (ingred.length != 0 && prepMethod == "") {
            $("#validationJS").html("Musisz podać sposób przygotowania potrawy");
            e.preventDefault();
        }
        else if (ingred.length == 0 && prepMethod != "") {
            $("#validationJS").html("Lista składników nie może byc pusta");
            e.preventDefault();
        }
        else if (ingred.length > 0 && prepMethod != "") {
            ingred.each(function () {
                if ($(this).val().trim() == "") {
                    $("#validationJS").html("Nie może istnieć składnik bez nazwy i ilości. Wypełnij wymagane dane lub usuń niepotrzebne pole.");
                    e.preventDefault();
                }
                if ($(this).attr("id")==("Quantity") && !($(this).val().match(/^\d+[\.\,]{1}\d+$|^\d+\/{1}\d+$|^\d+$/))) {
                   
                   // alert("Pole ilość musi zawierać wartość liczbową(liczba całkowitą, ułamek zwykły lub dziesiętny)");
                    $("#validationJS").html("Pole ilość musi zawierać wartość liczbową(liczba całkowitą, ułamek zwykły lub dziesiętny)");
                    e.preventDefault();
                }


            });

        }
    });
});
