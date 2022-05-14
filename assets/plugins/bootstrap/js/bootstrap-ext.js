


$(document).click(function (event) {
    var clickover = $(event.target);
    var _opened = $("#bs-example-navbar-collapse-1").hasClass("navbar-collapse collapse in");
    if (_opened === true && !clickover.hasClass("navbar-toggle")) {
        $("button.navbar-toggle").click();
    }
});

