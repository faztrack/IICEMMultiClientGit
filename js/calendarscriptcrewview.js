var currentUpdateEvent;
var addStartDate;
var addEndDate;
var updateStartDate;
var updateEndDate;
var globalAllDay;
var IsSerTktNumber = false;
var currentStarDate;
var nevent;
var AutoComCustID;
var SalesPersonID;
var SalesPersonName;
var ChildEventID;
var AutoComSection;
var AutoComUser;
var AutoComSuperintendent;
var ParentEventName;
var ParentEventId;
var LinkType;
var SelectedSectionName;
var nCusomertId = 0;
var nEstimateId = 0;

function swapStyleSheet(sheet) {
    document.getElementById("pagestyle").setAttribute("href", sheet);

    //// console.log("swapStyleSheet");
}

function initate() {
    var base = document.getElementById("base");
    var blacktie = document.getElementById("blacktie");
    var blitzer = document.getElementById("blitzer");
    var cupertino = document.getElementById("cupertino");
    var darkhive = document.getElementById("darkhive");
    var dotluv = document.getElementById("dotluv");
    var eggplant = document.getElementById("eggplant");
    var excitebike = document.getElementById("excitebike");
    var flick = document.getElementById("flick");
    var hotsneaks = document.getElementById("hotsneaks");
    var humanity = document.getElementById("humanity");
    var lefrog = document.getElementById("lefrog");
    var mintchoc = document.getElementById("mintchoc");
    var overcast = document.getElementById("overcast");
    var peppergrinder = document.getElementById("peppergrinder");
    var redmond = document.getElementById("redmond");
    var smoothness = document.getElementById("smoothness");
    var southstreet = document.getElementById("southstreet");
    var start = document.getElementById("start");
    var sunny = document.getElementById("sunny");
    var swankypurse = document.getElementById("swankypurse");
    var trontastic = document.getElementById("trontastic");
    var uidarkness = document.getElementById("uidarkness");
    var uilightness = document.getElementById("uilightness");
    var vader = document.getElementById("vader");


    // console.log(style1);

    base.onclick = function () { swapStyleSheet("jquerycalendar/themes/base/jquery-ui.css") };
    blacktie.onclick = function () { swapStyleSheet("jquerycalendar/themes/black-tie/jquery-ui.css") };
    blitzer.onclick = function () { swapStyleSheet("jquerycalendar/themes/blitzer/jquery-ui.css") };
    cupertino.onclick = function () { swapStyleSheet("jquerycalendar/themes/cupertino/jquery-ui.css") };
    darkhive.onclick = function () { swapStyleSheet("jquerycalendar/themes/dark-hive/jquery-ui.css") };
    dotluv.onclick = function () { swapStyleSheet("jquerycalendar/themes/dot-luv/jquery-ui.css") };
    eggplant.onclick = function () { swapStyleSheet("jquerycalendar/themes/eggplant/jquery-ui.css") };
    excitebike.onclick = function () { swapStyleSheet("jquerycalendar/themes/excite-bike/jquery-ui.css") };
    flick.onclick = function () { swapStyleSheet("jquerycalendar/themes/flick/jquery-ui.css") };
    hotsneaks.onclick = function () { swapStyleSheet("jquerycalendar/themes/hot-sneaks/jquery-ui.css") };
    humanity.onclick = function () { swapStyleSheet("jquerycalendar/themes/humanity/jquery-ui.css") };
    lefrog.onclick = function () { swapStyleSheet("jquerycalendar/themes/le-frog/jquery-ui.css") };
    mintchoc.onclick = function () { swapStyleSheet("jquerycalendar/themes/mint-choc/jquery-ui.css") };
    overcast.onclick = function () { swapStyleSheet("jquerycalendar/themes/overcast/jquery-ui.css") };
    peppergrinder.onclick = function () { swapStyleSheet("jquerycalendar/themes/pepper-grinder/jquery-ui.css") };
    redmond.onclick = function () { swapStyleSheet("jquerycalendar/themes/redmond/jquery-ui.css") };
    smoothness.onclick = function () { swapStyleSheet("jquerycalendar/themes/smoothness/jquery-ui.css") };
    southstreet.onclick = function () { swapStyleSheet("jquerycalendar/themes/south-street/jquery-ui.css") };
    start.onclick = function () { swapStyleSheet("jquerycalendar/themes/start/jquery-ui.css") };
    sunny.onclick = function () { swapStyleSheet("jquerycalendar/themes/sunny/jquery-ui.css") };
    swankypurse.onclick = function () { swapStyleSheet("jquerycalendar/themes/swanky-purse/jquery-ui.css") };
    trontastic.onclick = function () { swapStyleSheet("jquerycalendar/themes/trontastic/jquery-ui.css") };
    uidarkness.onclick = function () { swapStyleSheet("jquerycalendar/themes/ui-darkness/jquery-ui.css") };
    uilightness.onclick = function () { swapStyleSheet("jquerycalendar/themes/ui-lightness/jquery-ui.css") };
    vader.onclick = function () { swapStyleSheet("jquerycalendar/themes/vader/jquery-ui.css") };


    //console.log(style1);
    //console.log(style2);
}

window.onload = initate;

function openTab(evt, tabName) {
    //console.log(evt, tabName);
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";
}

function updateEvent(event, element) {
    debugger;
    document.getElementById("btnParentEventLink").click(); // Click On Link Tab

    //  console.log(event);

    //console.log("updateEvent, event.employee_id: " + event.employee_id);





   

    //console.log(event.title);
    //console.log("event.start: " + event.start);
    var stdate = new Date(event.start);
    // var nStartDate = dateformatting(stdate);

    //console.log(nStartDate);

    if ($(this).data("qtip")) $(this).qtip("destroy");

    if (event.CustomerID != null && event.CustomerID != 0)
        nCusomertId = event.CustomerID;

    if (event.EstimateID != null && event.EstimateID != 0)
        nEstimateId = event.EstimateID;

    currentUpdateEvent = event;

    $('#updatedialog').dialog('open');


    if (event.EstimateID == "0" || event.EstimateID == "") {
        $("#eventName").text(decodeHtml(event.section_name).replace("-", "").replace("[", "").replace("]", "").trim());
    }
    else {
        $("#eventName").text(decodeHtml(event.section_name));
    }

    $("#eventLocation").text(decodeHtml(event.location_name));

    ParentEventName = event.section_name + ' (' + event.location_name + ')';
    ParentEventId = event.id;



    $("#eventDesc").text(decodeHtml(event.description));

    $("#txtNotes").text(decodeHtml(event.operation_notes));



    $("#eventId").val(event.id);
    $("#eventStart").text(dateformatting(event.start) + " " + timeformatting(event.start));



    debugger;

    $("#eventSalesPerson").text(event.employee_name);
    SalesPersonID = event.employee_id;

    //console.log("updateEvent, event.employee_name: " + event.employee_name);


    updateStartDate = new Date(dateformatting(event.start) + " " + timeformatting(event.start));

    if (event.end === null) {
        $("#eventEnd").text(dateformatting(event.start) + " " + timeformatting(event.start));

        updateEndDate = new Date(dateformatting(event.start) + " " + timeformatting(event.start));
    }
    else {
        $("#eventEnd").text(dateformatting(event.end) + " " + timeformatting(event.end));

        updateEndDate = new Date(dateformatting(event.end) + " " + timeformatting(event.end));
    }

    //console.log(timeformatting(event.start));
    //If Time is 00:00
    if (timeformatting(event.start).indexOf('00:00') != -1) {
        setEventStartTime();

    }

    var updtStartDate = new Date($("#eventStart").val())
    var updtEndDate = new Date($("#eventEnd").val());

    if (updtStartDate.getDay() === updtEndDate.getDay()) {
        $("#eventEndTime").show();


    }
    else {

        $("#eventEndTime").hide();

    }


   
    
}

function dateformatting(strDate) {
    //console.log("dateformatting, strDate: " + strDate)
    //var date = (strDate.getMonth() + 1) + "/" + strDate.getDate() + "/" + strDate.getFullYear();

    var date = moment(strDate).format('MM/DD/YYYY');

    //  var date = new Date(unix_timestamp * 1000);


    //var a = new Date(strDate * 1000);
    //var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    //var year = a.getFullYear();
    //var month = months[a.getMonth()];
    //var date = a.getDate();
    //var hour = a.getHours();
    //var min = a.getMinutes();
    //var sec = a.getSeconds();
    // var time = date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec;

    //  var time = year + '/' + month + '/' + date ;

    //console.log("dateformatting, date: " + date);

    return date;
}

function timeformatting(nstrDate) {
    var strDate = nstrDate;
    //console.log("nstrDate: " + nstrDate);
    var formatter = 'YYYY-MM-DD[T]HH:mm:ss';
    var date = new Date(nstrDate);
    var formatedDate = moment(nstrDate._i).format();

    strDate = new Date(formatedDate);

    //strDate = new Date(moment(strDate).format('YYYY-MM-DD'));




    //console.log("datetimeNow: " + datetimeNow);
    //console.log("timeformatting: " + strDate);

    var hh = "";

    if (strDate.getHours() > 12) {
        hh = strDate.getHours() - 12;

        if (hh < 10)
            hh = "0" + hh;
    }
    else if (strDate.getHours() < 10)
        hh = "0" + strDate.getHours();
    else
        hh = strDate.getHours();

    var mm = "";
    if (strDate.getMinutes() === 0) {
        mm = "00";
    }
    else if (strDate.getMinutes() < 10)
        mm = "0" + strDate.getMinutes();
    else
        mm = strDate.getMinutes();

    var ss = "";
    if (strDate.getSeconds() === 0)
        ss = "00";
    else if (strDate.getSeconds() < 10)
        ss = "0" + strDate.getSeconds();
    else
        ss = strDate.getSeconds();

    var tt = strDate.getHours() < 12 ? "AM" : "PM";


    //var time = hh + ":" + mm + ":" + ss + " " + tt;
    var time = hh + ":" + mm + " " + tt;

    return time;
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}

function hideimagefunction() {
    $('#loading').hide();
}

function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
    //console.log("woke up!");
}

function checkForSpecialChars(stringToCheck) {
    //var pattern = /[^A-Za-z0-9 ]/;
    // var pattern = /[^(A-Za-z0-9 ?<=^| )\d+(\.\d+)?(?=$| )|(?<=^| )\.\d+(?=$|.) ]/;
    var pattern = /[^A-Za-z0-9_\-.?'" ]/;

    return pattern.test(stringToCheck);
}

function decodeHtml(html) {
    var txt = document.createElement("textarea");
    txt.innerHTML = html;
    //console.log(txt.value);
    return txt.value;
}

function txtChange(id) {
    if (document.getElementById(id).value == '') {
        document.getElementById("lbl" + id).className = 'show';
    }
    else {
        document.getElementById("lbl" + id).className = 'hidden';
    }
}

function txtLocationChange(id) {
    if (document.getElementById(id).value == '') {
        document.getElementById("lbl" + id).className = 'show';
    }
    else {
        document.getElementById("lbl" + id).className = 'hidden';
    }
}

function searchKeyPress(e) {

    // look for window.event in case event isn't passed in
    e = e || window.event;
    if (e.keyCode == 13) {
        document.getElementById("head_btnSearch").click();
        return false;
    }
    return true;
}

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}

function getSuccess(getResult) {
    // debugger;
    var test = moment(getResult.date).format('MM/DD/YYYY');

    console.log(test);

    var vGotoDate = new Date();
    if (getResult.date != "") {

        vGotoDate = new Date(moment(getResult.date).format('MM/DD/YYYY'));
    }
    // console.log($(".fc-header-title").text());
    // console.log($(".fc-header-title h2").text());
    if (getResult.estimate_id != "" && getResult.customer_id != "") {
        $('#linkCalendarProjectLink').show();
        $('#linkCalendarProjectLink').html('Go to ' + $("#head_txtSearch").val() + ' Schedule');
        $('#linkCalendarProjectLink').attr('title', 'Go to ' + $("#head_txtSearch").val() + ' Schedule');
        $('#linkCalendarProjectLink').attr('href', 'schedulecalendar.aspx?eid=' + getResult.estimate_id + '&cid=' + getResult.customer_id + '&TypeID=1');
    }
    else {
        $('#linkCalendarProjectLink').hide();
        $('#linkCalendarProjectLink').html('title');
        $('#linkCalendarProjectLink').attr('title', 'title');
        $('#linkCalendarProjectLink').attr('href', '#');
    }
    var view = $('#calendar').fullCalendar('getView');
    var strTitleDate = $(".fc-header-toolbar h2").text();
    var aryTitleDate = strTitleDate.split(" ");
    var strTitleMonth = aryTitleDate[0];
    var strTitleYear = aryTitleDate[1];

    var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var shortMonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    if (strTitleMonth != monthNames[vGotoDate.getMonth()]) {
        if (view.name == 'listWeek') {
            $('#calendar').fullCalendar('refetchEvents');
        }
        else {
            $('#calendar').fullCalendar('gotoDate', vGotoDate);
        }

    }
    else {
        $('#calendar').fullCalendar('refetchEvents');

    }
}

$(document).ready(function () {

    $('body').on('click', 'button.fc-basicWeek-button', function () {
        var vSDate = $('#calendar').fullCalendar('getDate');
        var viewCurrentDate = new Date(vSDate);
        var nCurrentDate = new Date();

        var viewName = (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month");

        if (nCurrentDate.getMonth() == viewCurrentDate.getMonth() && (viewName == 'basicWeek' || viewName == 'basicDay' || viewName == 'listWeek')) {
            //console.log(view.name, currentDate);

            $('.fc-today-button').click();

            //  console.log(nCount);
        }
    });

    $('body').on('click', 'button.fc-basicDay-button', function () {
        var vSDate = $('#calendar').fullCalendar('getDate');
        var viewCurrentDate = new Date(vSDate);
        var nCurrentDate = new Date();

        var viewName = (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month");

        if (nCurrentDate.getMonth() == viewCurrentDate.getMonth() && (viewName == 'basicWeek' || viewName == 'basicDay' || viewName == 'listWeek')) {
            //console.log(view.name, currentDate);

            $('.fc-today-button').click();

            // console.log(nCount);
        }
    });

    $('body').on('click', 'button.fc-listWeek-button', function () {
        var vSDate = $('#calendar').fullCalendar('getDate');
        var viewCurrentDate = new Date(vSDate);
        var nCurrentDate = new Date();

        var viewName = (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month");

        if (nCurrentDate.getMonth() == viewCurrentDate.getMonth() && (viewName == 'basicWeek' || viewName == 'basicDay' || viewName == 'listWeek')) {
            //console.log(view.name, currentDate);

            $('.fc-today-button').click();

            // console.log(nCount);
        }
    });

    if (document.getElementById("head_hdnCustIDSelected").value != '')
        nCusomertId = parseInt(document.getElementById("head_hdnCustIDSelected").value);

    if (document.getElementById("head_hdnEstIDSelected").value != '')
        nEstimateId = parseInt(document.getElementById("head_hdnEstIDSelected").value);

    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }

    $("#head_txtSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendarreadcrewview.aspx/GetCustomer",
                data: "{'keyword':'" + $("#head_txtSearch").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var result = data.d;
                    response($.map(data.d, function (item) {
                        return {
                            label: item.customer_name,
                            desc: item.customer_id,
                            value: item.customer_name
                        }
                    }));
                },
                error: function () {
                    //console.log("there is some error");
                    //console.log("there is some error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null) {
                AutoComCustID = ui.item.desc;
            }
            else {
                AutoComCustID = 0;
            }
        },
        change: function (event, ui) {
            if (ui.item != null) {
                AutoComCustID = ui.item.desc;
            }
            else {
                AutoComCustID = 0;
            }
        },
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }
    });

    $("#head_txtSection").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendarreadcrewview.aspx/GetSection",
                data: "{'keyword':'" + $("#head_txtSection").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var result = data.d;
                    console.log(data.d);
                    response($.map(data.d, function (item) {
                        return {
                            label: item.section_name,
                            desc: item.section_name,
                            value: item.section_name
                        }
                    }));
                },
                error: function () {
                    //console.log("there is some error");
                    //console.log("there is some error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null)
                AutoComSection = ui.item.desc;
            else {
                AutoComSection = $("#head_txtSection").val();
            }
            //console.log(AutoComSection);
        },
        change: function (event, ui) {
            if (ui.item != null)
                AutoComSection = ui.item.desc;
            else {
                AutoComSection = $("#head_txtSection").val();
            }
            // console.log(AutoComSection);
        },
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }
    });

    $("#head_txtUser").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendarreadcrewview.aspx/GetUserName",
                data: "{'keyword':'" + $("#head_txtUser").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var result = data.d;
                    console.log(data.d);
                    response($.map(data.d, function (item) {
                        return {
                            label: item.sales_person_name,
                            desc: item.sales_person_name,
                            value: item.sales_person_name
                        }
                    }));
                },
                error: function () {
                    //console.log("there is some error");
                    //console.log("there is some error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null)
                AutoComUser = ui.item.desc;
            else {
                AutoComUser = $("#head_txtUser").val();
            }
            //console.log(AutoComSection);
        },
        change: function (event, ui) {
            if (ui.item != null)
                AutoComUser = ui.item.desc;
            else {
                AutoComUser = $("#head_txtUser").val();
            }
            // console.log(AutoComSection);
        },
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }
    });

    $("#head_txtSuperintendent").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendarreadcrewview.aspx/GetSuperintendentName",
                data: "{'keyword':'" + $("#head_txtSuperintendent").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var result = data.d;
                    console.log(data.d);
                    response($.map(data.d, function (item) {
                        return {
                            label: item.sales_person_name,
                            desc: item.sales_person_name,
                            value: item.sales_person_name
                        }
                    }));
                },
                error: function () {
                    //console.log("there is some error");
                    //console.log("there is some error");
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null)
                AutoComSuperintendent = ui.item.desc;
            else {
                AutoComSuperintendent = $("#head_txtSuperintendent").val();
            }
            //console.log(AutoComSection);
        },
        change: function (event, ui) {
            if (ui.item != null)
                AutoComSuperintendent = ui.item.desc;
            else {
                AutoComSuperintendent = $("#head_txtSuperintendent").val();
            }
            // console.log(AutoComSection);
        },
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }
    });

    $("#head_btnSearch").click(function () {

        if ($("#head_txtSearch").val() === '' && $("#head_txtSection").val() === '' && $("#head_txtUser").val() === '' && $("#head_txtSuperintendent").val() ==='')
            return;

        //  console.log($("#head_txtSection").val());
        var nCustId = 0;
        var SectionName = '';
        var UserName = '';
        var SuperintendentName = '';


        if ($("#head_txtSearch").val() != '')
            nCustId = AutoComCustID;
        if ($("#head_txtSection").val() != '')
            SectionName = AutoComSection;
        if ($("#head_txtUser").val() != '')
            UserName = AutoComUser;
        if ($("#head_txtSuperintendent").val() != '')
            SuperintendentName = AutoComSuperintendent;


        //  console.log(AutoComSection);

        //console.log(AutoComCustID);
        PageMethods.GetEvent(nCustId, SectionName, UserName, SuperintendentName, getSuccess);

    });

    $("#head_lnkViewAll").click(function () {
        //$("#head_txtSearch").val('');
        //$("#head_txtSection").val('');
        //$("#head_txtUser").val('');
        //var nCustId = 0;
        //var SectionName = '';
        //var UserName = '';
        //PageMethods.GetEvent(nCustId, SectionName, UserName, getSuccess);
        window.location = "schedulecalendarreadcrewview.aspx?TypeID=1";
        // $('.fc-button-today span').click();

    });


    // update Dialog
    //  j('#updatedialog').dialog('open');
    $('#updatedialog').dialog({
        autoOpen: false,
        width: 612,
        dialogClass: 'cssdialog',
        buttons: {
            "Close": function () {


                $('#calendar').fullCalendar('updateEvent', '');
                $(this).dialog("close");
            },

        }
    });


    var date = new Date();

    if (document.getElementById("head_hdnEventStartDate").value != '') {
        date = new Date(document.getElementById("head_hdnEventStartDate").value);
        //console.log("date: " + date);
    }

    var currentDate = moment(new Date()).format('YYYY-MM-DD');
    
   
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    var tt = date.getTime();
    var firstHour = date.getHours();
    //console.log(d);
    var calendar = $('#calendar').fullCalendar({

        loading: function (isLoading, view) {
            if (isLoading) {
                $('#calendar').fullCalendar('removeEvents');
                $('#loading').show();
            }
            else {
                //console.log("msg");
                $('#loading').hide();
            }
        },
        eventOrder: 'title', // default is  title

        theme: true,
        header: {
            left: '',
            center: 'prev, title, next, today',
            right: 'month,basicWeek,basicDay,listWeek'
        },
        //buttonText: {
        //    prevYear: parseInt(new Date().getFullYear(), 10) - 1,
        //    nextYear: parseInt(new Date().getFullYear(), 10) + 1
        //},
        viewRender: function (view, element) {
            var vSDate = $('#calendar').fullCalendar('getDate');
            //  debugger;


            // var vSelectedDate = dateformatting(vSDate);
            //  console.log("viewRender: " + moment(vSDate).format('YYYY-MM-DD'));

            var view = $('#calendar').fullCalendar('getView');
            //  console.log(view.name);
            // console.log(view);

            localStorage.setItem("fcDefaultView", view.name);
            localStorage.setItem("fcDefaultDate", moment(vSDate).format('YYYY-MM-DD'));



        },
        viewDisplay: function (view) {
            var d = $('#calendar').fullCalendar('getDate');


            $(".fc-button-prevYear .fc-button-content").text(parseInt(d.getFullYear(), 10) - 1);
            $(".fc-button-nextYear .fc-button-content").text(parseInt(d.getFullYear(), 10) + 1);


        },

        ignoreTimezone: false,

        defaultDate: (localStorage.getItem("fcDefaultDate") != 'null' ? localStorage.getItem("fcDefaultDate") : currentDate),
        defaultView: (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month"),

        eventClick: false,//updateEvent,
        selectable: false,
        selectHelper: true,
        select: false,

        eventStartEditable: false,
        events: "JsonResponsecrewview.ashx",

        //  displayEventTime : false,

        eventDrop: false,

        eventResize: false,
        eventAfterRender: function (event, element) {
            //var touchavailable = ("ontouchend" in document);
            //if (touchavailable) {
            //    $(element).addtouch();
            //}
            // console.log(event.section_name);

            if (event.customer_last_name === undefined) {
                event.customer_last_name = '';
            }

            if (event.estimate_name === undefined) {
                event.estimate_name = '';
            }

            if (event.section_name !== undefined && event.location_name !== undefined) {

                element.qtip({
                    content:
                        {
                            title: event.section_name + (event.location_name.length != 0 ? " (" + event.location_name + ")" : ""),
                            text: //"Prject: " + (event.customer_last_name.length != 0 ? event.customer_last_name + " (" + event.estimate_name + ")" : event.estimate_name) +
                                //"<br/>"+
                                "Start: " + moment(event.start).format('MM-DD-YYYY') +
                                "<br/>End: " + moment(event.end).format('MM-DD-YYYY') +
                               // "<br/> Status: " + (event.is_complete === 'True' ? "Complete" : "Not Complete") +
                                "<br/> Duration: " + (event.duration <= 1 ? event.duration + " Day" : event.duration + " Days") +
                                "<br/> Superintendent: " + event.employee_name

                        },
                    position: {
                        my: 'top center',
                        at: 'bottom center'
                    },
                    style: {
                        border: {
                            width: 1,
                            radius: 3,
                            color: '#2779AA'
                        },
                        padding: 10,
                        textAlign: 'left',
                        tip: true, // Give it a speech bubble tip with automatic corner detection
                        name: 'cream', // Style it according to the preset 'cream' style
                        width: 300
                    }

                });
            }
        },
        droppable: false, // this allows things to be dropped onto the calendar

    });

    $('.fc-EventIconSpan').hide();
    $('.fc-ResourceIconSpan').hide();
    $('.fc-UnassignedSpan').hide();
}
);





