$('INPUT[type="file"]').change(function () {
    var ext = this.value.match(/\.(.+)$/)[1];
    switch (ext.toLowerCase()) {
        case 'jpg':
        case 'jpeg':
        case 'png':
            break;
        default:
            alert('Dopuszczalne są tylko pliki typu jpg oraz png');
            this.value = '';
    }
});

