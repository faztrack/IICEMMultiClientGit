var globalAllDay;
var currentStarDate;
var IsDisplayEventTime;
var calDefaultView;
var calHeader;

$(document).ready(function () {

    // alert("msg");
    var date = new Date();

    if (document.getElementById("head_hdnEventStartDate").value != '') {
        date = new Date(document.getElementById("head_hdnEventStartDate").value);
    }

    if (document.getElementById("head_hdnCustCalWeeklyView").value === '3') {
        IsDisplayEventTime = false;
        calDefaultView = 'listWeek';
        calHeader = '';
    }
    else {
        IsDisplayEventTime = true;
        calDefaultView = 'month';
        calHeader = 'month,basicWeek,basicDay,listWeek';
    }

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
                //alert("msg");
                $('#loading').show();
            }
            else {
                $('#loading').hide();
            }

            if (isLoading) {
                $('#calendar').fullCalendar('removeEvents');
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
            right: calHeader,//'month,basicWeek,basicDay,listWeek' // arefin050719
        },
        displayEventTime: IsDisplayEventTime, // arefin050719
        //listDayFormat: true, // arefin050719
        //listDayAltFormat: true, // arefin050719
        //noEventsMessage: "No events to display",
        //buttonText: {
        //    prevYear: parseInt(new Date().getFullYear(), 10) - 1,
        //    nextYear: parseInt(new Date().getFullYear(), 10) + 1
        //},
        viewDisplay: function (view) {
            //var d = $('#calendar').fullCalendar('getDate');

            //$(".fc-button-prevYear .fc-button-content").text(parseInt(d.getFullYear(), 10) - 1);
            //$(".fc-button-nextYear .fc-button-content").text(parseInt(d.getFullYear(), 10) + 1);
        },

        ignoreTimezone: false,
        defaultView: calDefaultView, // 'month',//'agendaWeek', // //arefin050719

        selectable: false,
        selectHelper: false,
        editable: false,
        events: "JsonResponseCustomer.ashx",
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

    $('.fc-WeekListTitleSpan').text('Project Start Date: ' + $('#head_StartofJob').val()); // arefin050719

    if (document.getElementById("head_hdnCustCalWeeklyView").value === '3') {
        $('.fc-today-button').hide();
        $('.fc-center-ext').css("margin-left", "-130px");
    }
}
);


