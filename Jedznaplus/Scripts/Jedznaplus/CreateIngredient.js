$(function () {
    $('form').submit(function (e) {
        var Ingred = $('#ingredientEditor li INPUT[type = "text"]')
        var PrepMethod = $('#PreparationMethod').val().trim()

       if (Ingred.length == 0 && PrepMethod=="")
       {
            alert("Lista składnikow  oraz sposób przygotowania nie mogą być puste");
            e.preventDefault();
       }
       else if (Ingred.length != 0 && PrepMethod == "")
       {
           alert("Musisz podać sposób przygotowania potrawy");
           e.preventDefault();
       }
       else if(Ingred.length == 0 && PrepMethod != "")
       {
           alert("Lista składników nie może byc pusta");
           e.preventDefault();
       }
       else if (Ingred.length > 0 && PrepMethod != "") {
            Ingred.each(function () {
                if ($(this).val().trim() == "")
                {
                    alert("Nie może istnieć składnik bez nazwy i ilości. Wypełnij wymagane dane lub usuń niepotrzebne pole.");
                    e.preventDefault();
                }
                

            });

        }
    });
});