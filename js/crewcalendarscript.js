var globalAllDay;
var currentStarDate;

function SetLandingPageData(event, element) {
    console.log(event);
    var custid = event.CustomerID;
    $.ajax({
        type: "POST",
        url: "mcrewschedulecalendar.aspx/SetLandingPageByCustIdandUserId",
        data: "{'custid':'" + custid + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log("SetLandingPageByCustIdandUserId:" + data.d);
            var result = data.d;


            if (result == "Ok") {
                window.location = "mlandingpage.aspx";
            }

        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
        }
    });
}

$(document).ready(function () {

    $('body').on('click', 'button.fc-basicWeek-button', function () {
        var vSDate = $('#calendar').fullCalendar('getDate');
        var viewCurrentDate = new Date(vSDate);
        var nCurrentDate = new Date();

        var viewName = (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month");

        if (nCurrentDate.getMonth() == viewCurrentDate.getMonth() && (viewName == 'basicWeek' || viewName == 'basicDay')) {
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

        if (nCurrentDate.getMonth() == viewCurrentDate.getMonth() && (viewName == 'basicWeek' || viewName == 'basicDay')) {
            //console.log(view.name, currentDate);

            $('.fc-today-button').click();

            // console.log(nCount);
        }
    });
    //alert("msg");
    var date = new Date();

    if (document.getElementById("head_hdnEventStartDate").value != '') {
        date = new Date(document.getElementById("head_hdnEventStartDate").value);
    }
    var currentDate = moment(new Date()).format('YYYY-MM-DD');
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    var tt = date.getTime();
    var firstHour = date.getHours();

    var calendar = $('#calendar').fullCalendar({
        //        dayClick: function (date, jsEvent, view) {
        //           alert(date.format('DD-MM-YYYY'));
        //        },
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
        //eventBackgroundColor: '#378006',
        //year: y,
        //month: m,
        //date: d,
        //scrollTime: tt,
        firstHour: firstHour,
        theme: true,
        header: {
          //  left: 'prevYear, nextYear',
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
        defaultView: 'listWeek',//'agendaWeek',
        eventClick: SetLandingPageData,
        selectable: false,
        selectHelper: false,       
        editable: false,
        events: "JsonResponseCrew.ashx",
        eventRender: function (event, element) {
           // alert("msg");
           // console.log(event.title);
            element.qtip({
                content: event.description,
                position: { corner: { tooltip: 'bottomLeft', target: 'topRight' } },
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
    });
    $('.fc-EventIconSpan').hide();
    $('.fc-ResourceIconSpan').hide();
    $('.fc-UnassignedSpan').hide();
}
);


