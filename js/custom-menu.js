
$(document).ready(function () {    
    var str = location.href.toLowerCase();
    alert(str);
    $('#menuBar td a').each(function () {
        if (str.indexOf(this.href.toLowerCase()) > -1) {
            alert(str);
            $("a.highlight").removeClass("selected");
            $(this).parent().addClass("selected");          
        }
    });
});


