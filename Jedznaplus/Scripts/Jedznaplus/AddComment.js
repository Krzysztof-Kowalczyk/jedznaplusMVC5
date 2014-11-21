       $(function () {
            $('#dodajKomentarz').on("click", function () {

                var form = $(this).parent("form");

                $.ajax({
                    type: "POST",
                    url: form.attr('action'),
                    data: form.serialize()
                })
                    .success(function (html) {
                        $('.current-price').replaceWith(html);
                    })
                    .error(function () {
                        alert("Błąd dodawania komentarza");
                    });

                return false;
            });
        });