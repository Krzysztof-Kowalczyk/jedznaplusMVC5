$(function () {
    $('#DodajAvatar').prop("disabled", true);
});

$('INPUT[type="file"]').change(function () {
    var ext = this.value.match(/\.(.+)$/)[1];
    if (this.files[0].size > 3000000)
    {
        alert('Dopuszczalne są tylko pliki o wielkości do 3 MB');
        this.value = '';
        return;
    }
    switch (ext.toLowerCase()) {
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'gif':
            $('#DodajAvatar').prop("disabled", false);
            break;
        default:
            alert('Dopuszczalne są tylko pliki typu jpg oraz png');
            this.value = '';
    }
});
