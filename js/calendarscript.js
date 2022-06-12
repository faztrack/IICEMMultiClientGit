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
var ChildEventID = 0;
var addChildEventID = 0;
var AutoComSection;
var AutoComUser;
var AutoComSuperintendent;
var isSelectable;
var ParentEventName;
var ParentEventId;
var LinkType;
var addLinkType = "";
var SelectedSectionName;
var nCusomertId = 0;
var nEstimateId = 0;
var HasParentEvent = 0;
var HasChildEvent = 0;

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

    //debugger;
    document.getElementById("btnParentEventLink").click(); // Click On Link Tab
    $('#ChildLinkTbl tr').remove();
    $('#ParentLinkTbl tr').remove();


    //console.log(event);

    //console.log("updateEvent, event.employee_id: " + event.employee_id);

    // Check Calendar is Online or Offline
    //if (!isSelectable) {
    //    return false;
    //}
    /////////
    //console.log(event.title);
    //console.log("event.start: " + event.start);
    var stdate = new Date(event.start);
    // var nStartDate = dateformatting(stdate);

    //console.log(nStartDate);

    // if ($(this).data("qtip")) $(this).qtip("destroy");

    if (event.CustomerID != null && event.CustomerID != 0)
        nCusomertId = event.CustomerID;

    if (event.EstimateID != null && event.EstimateID != 0)
        nEstimateId = event.EstimateID;

    currentUpdateEvent = event;

    var boolIsScheduleDayException = (event.IsScheduleDayException.toLowerCase() === 'true');
    var boolis_complete = (event.is_complete.toLowerCase() === 'true');

    $("#chkScheduleDayException").prop('checked', boolIsScheduleDayException);
    $("#chkComplete").prop('checked', boolis_complete);
    //console.log(event.has_parent_link.toLowerCase());
    //////////////////////////////check visibility check box control
    if (boolIsScheduleDayException) {
        ExceptiondayWeekends();
    }
    else {
        $('#dvCheckBoxListControl').empty();
    }

    if (event.has_parent_link.toLowerCase() === 'true' || event.has_child_link.toLowerCase() === 'true') {
        $("#divScheduleDayException").hide();
    }
    else {
        $("#divScheduleDayException").show();
    }

    if (event.has_parent_link.toLowerCase() === 'true') {
        $('#trParentSection').css('display', 'none');
        $("#ParentLinkTbl").css('display', '');
        $('#btnUpdateParentLink').css('display', '');
        $('#btnDeleteParentLink').css('display', '');

    }
    else {
        $('#trParentSection').css('display', '');
        $("#ParentLinkTbl").css('display', 'none');
        $('#btnUpdateParentLink').css('display', 'none');
        $('#btnDeleteParentLink').css('display', 'none');
    }

    if (event.has_child_link.toLowerCase() === 'true') {
        $('#btnUpdateLink').css('display', '');
        $('#btnDeleteLink').css('display', '');
    }
    else {
        $('#btnUpdateLink').css('display', 'none');
        $('#btnDeleteLink').css('display', 'none');
    }


    if (event.TypeID == 1) {
        if (!boolIsScheduleDayException) {
            $('#EventLinkSection').show();
            loadParentEventLinkTable(event);
            loadChildEventLinkTable(event);

        }
        else {
            $('#EventLinkSection').hide();
            $("#divScheduleDayException").show();
        }
    }
    else {
        $('#EventLinkSection').hide();
        $("#divScheduleDayException").hide();
    }

    $('#updatedialog').dialog('open');

    //debugger;
    if (event.EstimateID == "0" || event.EstimateID == "") {
        $("#eventName").val(decodeHtml(event.section_name).replace("-", "").replace("[", "").replace("]", "").trim());
    }
    else {
        $("#eventName").val(decodeHtml(event.section_name));
    }

    $("#eventLocation").val(decodeHtml(event.location_name));

    ParentEventName = event.section_name + ' (' + event.location_name + ')';//$("#eventName").val();
    ParentEventId = event.id;

    //console.log(" ParentEventName: " + ParentEventName + ", ParentEventId: " + ParentEventId);

    $("#eventDesc").val(decodeHtml(event.description));

    $("#txtNotes").val(decodeHtml(event.operation_notes));

    //  $("#txtTradePartner").val(decodeHtml(event.trade_partner));

    $("#eventId").val(event.id);
    $("#eventStart").val(dateformatting(event.start));//+ " " + timeformatting(event.start));

    $("select#eventStartTime option").each(function () {
        this.selected = (this.text == timeformatting(event.start).replace(/^0/, ""))
    });

    $("select#ddlEventColor option").each(function () {
        if ("fc-" + this.text === event.className[0]) {
            this.selected = ("fc-" + this.text === event.className[0]);
            $("#ddlEventColor").removeClass();
            var SelectedClass = event.className[0];
            $("#ddlEventColor").addClass(SelectedClass);
        }
    });

    $("#eventSalesPerson").val(event.employee_name);
    SalesPersonID = event.employee_id;




    //console.log("updateEvent, event.employee_name: " + event.employee_name);
    var testDate = dateformatting(event.start);

    updateStartDate = new Date(dateformatting(event.start) + " " + timeformatting(event.start));

    if (event.end === null) {
        $("#eventEnd").val(dateformatting(event.start));
        $("select#eventEndTime option").each(function () {
            this.selected = (this.text == timeformatting(event.start).replace(/^0/, ""));
        });
        updateEndDate = new Date(dateformatting(event.start) + " " + timeformatting(event.start));
    }
    else {
        $("#eventEnd").val(dateformatting(event.end));// + " " + timeformatting(event.end));
        $("select#eventEndTime option").each(function () {
            this.selected = (this.text == timeformatting(event.end).replace(/^0/, ""))
        });
        updateEndDate = new Date(dateformatting(event.end) + " " + timeformatting(event.end));
    }

    //console.log(timeformatting(event.start));
    //If Time is 00:00
    if (timeformatting(event.start).indexOf('00:00') != -1) {
        setEventStartTime();

    }

    var updtStartDate = new Date($("#eventStart").val())
    var updtEndDate = new Date($("#eventEnd").val());

    if (updtStartDate.getDate() === updtEndDate.getDate()) {
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

    //    var date = moment(strDate).format('YYYY-MM-DD');


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
    // debugger;
    var strDate = nstrDate;
    //console.log("nstrDate: " + nstrDate);
    var formatter = 'YYYY-MM-DD[T]HH:mm:ss';
    var date = new Date(nstrDate);
    var formatedDate = moment(nstrDate._i).format();

    strDate = new Date(formatedDate);
    //strDate = new Date(moment(strDate).format());




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


    // var time = hh + ":" + mm + ":" + ss + " " + tt;
    var time = hh + ":" + mm + " " + tt;

    return time;
}

function timeformatting2(nstrDate) { // without moment function


    strDate = new Date(nstrDate);
    //strDate = new Date(moment(strDate).format());




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


    // var time = hh + ":" + mm + ":" + ss + " " + tt;
    var time = hh + ":" + mm + " " + tt;

    return time;
}

function updateSuccess(updateResult) {
    // debugger;
    // console.log(parseInt(updateResult, 10));

    if (parseInt(updateResult, 10) < 0) {
        alert("Child Link Date must be later than Parent Link Date. You may de-link to move this item.");
        $('#loading').hide();
    }
    //console.log(updateResult);



    $("#head_btnHdn").click();

    //UpdateEventLink();

    $('#loading').hide();
    //  console.log(parseInt(updateResult, 10));
}

function updateSuccessNotes(updateResult) {
    //console.log(updateResult);    
}

function updateSuccessTradePartner(updateResult) {
    //console.log(updateResult);    
}

function deleteSuccess(deleteResult) {
    //$('#loading').hide();
    //console.log(deleteResult);
    $("#head_btnHdn").click();
}

function cancelSuccess(deleteResult) {
    //console.log(deleteResult);
}

function getSuccess(getResult) {
    // debugger;
    var test = moment(getResult.date).format('MM/DD/YYYY');

    // console.log(test);

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

function addSuccess(addResult) {

    // if addresult is -1, means event was not added
    //console.log("globalAllDay: " + $("#head_hdnCustomerID").val());
    var vlocation = '';
    if ($("#addLocation").val() != '')
        vlocation = ' (' + $("#addLocation").val() + ')';
    if (addResult != -1) {
        $('#calendar').fullCalendar('renderEvent',
            {
                title: decodeHtml($("#addEventName").val() + vlocation),
                section_name: $("#addEventName").val(),
                location_name: $("#addLocation").val(),
                start: addStartDate,
                end: addEndDate,
                id: addResult,
                description: $("#addEventDesc").val(),
                allDay: false,
                EstimateID: $("#head_hdnEstimateID").val(),
                CustomerID: $("#head_hdnCustomerID").val(),
                employee_id: SalesPersonID,
                employee_name: $("#addSalesPersonName").val(),
                TypeID: $("#head_hdnTypeID").val(),
                className: 'fc-default'//$("#head_hdnServiceCssClass").val()
            },
            true // make the event "stick"
        );


        $('#calendar').fullCalendar('unselect');
        document.getElementById("addEventName").value = '';
        document.getElementById("addLocation").value = '';
        document.getElementById("addEventDesc").value = '';
        document.getElementById("addSalesPersonName").value = '';
        document.getElementById("lbladdEventName").className = 'hidden';
        document.getElementById("lbladdEventDesc").className = 'hidden';
        document.getElementById("lblTime").className = 'hidden';
        document.getElementById("lblRequired").className = 'hidden';
        //document.getElementById("head_hdnEstimateID").value = '0';
        //document.getElementById("head_hdnCustomerID").value = '0';
        //document.getElementById("head_hdnEmployeeID").value = '0';
        //document.getElementById("head_hdnTypeID").value = '0';
        document.getElementById("head_hdnServiceCssClass").value = '';
    }
    //setTimeout(function () {
    //    $('#loading').hide();
    //}, 5000);

    $("#head_btnHdn").click();

}

function UpdateTimeSuccess(updateResult) {
    //debugger;
    // console.log(parseInt(updateResult, 10));

    if (parseInt(updateResult, 10) < 0) {
        alert("Child Link Date must be later than Parent Link Date. You may de-link to move this item.");
        $('#loading').hide();
    }
    //debugger;
    //console.log(updateResult);
    //var theHtml = updateResult;
    //$("#" + gridID).html(theHtml);
    $("#head_btnHdn").click();


    //  hideimagefunction();
}

function UpdateTimeSuccessAll(updateResult) {

    $('#calendar').fullCalendar('refetchEvents');

    $('#loading').hide();
    hideimagefunction();
}

function addeventOnDrop(start, title, sectionname, locationname, cssClassName, customer_id, estimate_id, client_id) {

    var addStartDate = new Date(start + " " + "8:00:00 AM");
    var addEndDate = new Date(start + " " + "9:00:00 AM");

    var eventToAdd = {
        title: title,
        section_name: sectionname,
        location_name: locationname,
        cssClassName: cssClassName,
        description: '',
        start: addStartDate.format("dd-MM-yyyy hh:mm:ss tt"),
        end: addEndDate.format("dd-MM-yyyy hh:mm:ss tt"),
        allDay: false,
        employee_id: 0,
        employee_name: 'TBD TBD',
        customer_id: customer_id,
        estimate_id: estimate_id,
        client_id: client_id
    };
    console.log("addeventOnDrop");
    console.log(eventToAdd);
    PageMethods.addEvent(eventToAdd, addSuccess);

    //document.getElementById("eventName").value = sectionname;
    //document.getElementById("eventLocation").value = locationname;
    //document.getElementById("eventSalesPerson").value = 'TBD TBD';
}

function selectDate(start, end, allDay) {
    document.getElementById("btnaddParentEventLink").click(); // Click On Link Tab
    //debugger;
    //  start = moment(start).format('YYYY/MM/DD hh:mm');
    //   end = moment(end).format('YYYY/MM/DD hh:mm');

    //console.log("selectDate- start: " + start, "end: " + end);

    $('#addDialog').dialog('open');

    //    $("#addEventStartDate").text("" + start.toLocaleString());
    //    $("#addEventEndDate").text("" + end.toLocaleString());

    $("#addEventStartDate").val("" + dateformatting(start));
    $("#addEventEndDate").val("" + dateformatting(start));

    //set default value in "addEventStartTime" dropdown
    // $("select#addEventStartTime option")
    //.each(function () { this.selected = (this.text == "6:30 AM"); });

    addStartDate = new Date($("#addEventStartDate").val() + " " + $("#addEventStartTime option:selected").text());
    addEndDate = new Date($("#addEventEndDate").val() + " " + $("#addEventEndTime option:selected").text());
    globalAllDay = allDay;
    //console.log("selectDate: " + $("#addEventStartDate").val() + ", " + $("#addEventEndDate").val());
}

function setAddEventStartTime(obj) {
    debugger;
    addStartDate = new Date($("#addEventStartDate").val() + " " + $("#addEventStartTime option:selected").text());




    addEndDate = new Date($("#addEventEndDate").val());

    var addStartDate = new Date($("#addEventStartDate").val())
    var addEndDate = new Date($("#addEventEndDate").val());

    if (addStartDate.getDate() === addEndDate.getDate()) {
        $("#addEventEndTime").show();

        var nStartDate = addStartDate;

        var endTime = timeformatting2(addStartDate.addHours(1)).replace(/^0/, ""); //"8:00 AM";//
        if (obj === "Time")
            endTime = $("#addEventEndTime option:selected").text();

        $("select#addEventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }
    else {

        $("#addEventEndTime").hide();
        var endTime = "11:30 PM";

        $("select#addEventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }



    addStartDate = new Date($("#addEventStartDate").val() + " " + $("#addEventStartTime option:selected").text());

    addEndDate = new Date($("#addEventEndDate").val() + " " + $("#addEventEndTime option:selected").text());

    if (addStartDate >= addEndDate) {
        document.getElementById("lblTime").className = 'show';
    }
    else {
        document.getElementById("lblTime").className = 'hidden';
    }
}

function setAddEventEndTime(obj) {
    // debugger;

    addStartDate = new Date($("#addEventStartDate").val() + " " + $("#addEventStartTime option:selected").text());
    addEndDate = new Date($("#addEventEndDate").val());

    var adddtStartDate = new Date($("#addEventStartDate").val())
    var adddtEndDate = new Date($("#addEventEndDate").val());
    if (adddtStartDate.getDate() === adddtEndDate.getDate()) {
        $("#addEventEndTime").show();

        var nStartDate = addStartDate;

        var endTime = timeformatting2(nStartDate.addHours(1)).replace(/^0/, ""); //"8:00 AM";//
        if (obj === "Time")
            endTime = $("#addEventEndTime option:selected").text();

        $("select#addEventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }
    else {

        $("#addEventEndTime").hide();
        var endTime = "11:30 PM";

        $("select#addEventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }
    addStartDate = new Date($("#addEventStartDate").val() + " " + $("#addEventStartTime option:selected").text());
    addEndDate = new Date($("#addEventEndDate").val() + " " + $("#addEventEndTime option:selected").text());
    //console.log(addEndDate);
    if (addStartDate >= addEndDate) {
        document.getElementById("lblTime").className = 'show';
    }
    else {
        document.getElementById("lblTime").className = 'hidden';
    }
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}

function setEventStartTime(obj) {

    updateStartDate = new Date($("#eventStart").val() + " " + $("#eventStartTime option:selected").text());
    //console.log("StartDate: " + updateStartDate + ", EndDate: " + updateEndDate);
    updateEndDate = new Date($("#eventEnd").val());


    ////////////////////////////////////////////
    var updtStartDate = new Date($("#eventStart").val())
    var updtEndDate = new Date($("#eventEnd").val());

    if (updtStartDate.getDate() === updtEndDate.getDate()) {
        $("#eventEndTime").show();

        var nStartDate = updateStartDate;

        var endTime = timeformatting2(nStartDate.addHours(1)).replace(/^0/, ""); //"8:00 AM";//
        if (obj === "Time")
            endTime = $("#eventEndTime option:selected").text();

        $("select#eventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }
    else {

        $("#eventEndTime").hide();
        //var endTime = timeformatting(updateEndDate.addHours(11)).replace(/^0/, ""); //"8:00 AM";//
        var endTime = "11:30 PM";

        $("select#eventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }

    updateStartDate = new Date($("#eventStart").val() + " " + $("#eventStartTime option:selected").text());
    updateEndDate = new Date($("#eventEnd").val() + " " + $("#eventEndTime option:selected").text());
    ////////////////////////////////////////
    if (updateStartDate >= updateEndDate) {
        //console.log(updateStartDate);
        document.getElementById('lblRequired').innerHTML = 'End time must be later than start time';
        document.getElementById("lblRequired").className = 'show';
    }
    else {
        document.getElementById("lblRequired").className = 'hidden';



    }
    //console.log("StartDate: " + updateStartDate + ", EndDate: " + updateEndDate);
}

function setEventEndTime(obj) {
    ///////////////
    debugger;

    updateStartDate = new Date($("#eventStart").val() + " " + $("#eventStartTime option:selected").text());
    updateEndDate = new Date($("#eventEnd").val());



    var updtStartDate = new Date($("#eventStart").val())
    var updtEndDate = new Date($("#eventEnd").val());

    //=============
    var test1 = updtStartDate.getDate();
    var test2 = updtEndDate.getDate();
    //================


    if (updtStartDate.getDate() === updtEndDate.getDate()) {
        $("#eventEndTime").show();
        // debugger;

        var nStartDate = updateStartDate;

        //var time = updtStartDate.getHours()+1;

        var endTime = timeformatting2(nStartDate.addHours(1)).replace(/^0/, ""); //"8:00 AM";//

        if (obj === "Time")
            endTime = $("#eventEndTime option:selected").text();


        $("select#eventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }
    else {

        $("#eventEndTime").hide();
        //var endTime = timeformatting(updateEndDate.addHours(11)).replace(/^0/, ""); //"8:00 AM";//
        var endTime = "11:30 PM";

        $("select#eventEndTime option").each(function () {
            this.selected = (this.text === endTime)
        });
    }


    ///////////////////////////
    updateStartDate = new Date($("#eventStart").val() + " " + $("#eventStartTime option:selected").text());
    updateEndDate = new Date($("#eventEnd").val() + " " + $("#eventEndTime option:selected").text());
    //console.log("StartDate: " + updateStartDate + ", EndDate: " + updateEndDate);
    if (updateStartDate >= updateEndDate) {
        //console.log(updateStartDate);
        document.getElementById('lblRequired').innerHTML = 'End time must be later than start time';
        document.getElementById("lblRequired").className = 'show';
    }
    else {
        document.getElementById("lblRequired").className = 'hidden';
    }
    updateStartEndRangeWeekends(updtStartDate, updtEndDate);

}

function updateStartEndRangeWeekends(start, end) {

    var Start = moment(start).format('YYYYMMDD');
    var End = moment(end).format('YYYYMMDD');
    var test = currentUpdateEvent;
    var Id = parseInt(currentUpdateEvent.id);
    var cid = parseInt(currentUpdateEvent.CustomerID);
    $('#loading').show();
    var isSelected = $("#chkScheduleDayException").prop('checked');
    if (isSelected === true) {
        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/SetImmediateWeekEnds",
            data: "{'isSelected':'" + isSelected + "', 'eventId':'" + Id + "', 'ncid':'" + cid + "', 'Start':'" + Start + "', 'End':'" + End + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: AjaxSucceeded,
            error: AjaxFailed,
        });
    }
    else {
        $('#dvCheckBoxListControl').empty();
    }
    $('#loading').hide();
}

function ExceptiondayWeekends() {
    var test = currentUpdateEvent;
    var Id = parseInt(currentUpdateEvent.id);
    var cid = parseInt(currentUpdateEvent.CustomerID);
    $('#loading').show();
    var isSelected = $("#chkScheduleDayException").prop('checked');
    if (isSelected === true) {
        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/GetWeekEnds",
            //data:"",
            data: "{'isSelected':'" + isSelected + "', 'eventId':'" + Id + "', 'ncid':'" + cid + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: AjaxSucceeded,

            //function (response) {
            //    AjaxSucceeded;
            //    $('#loading').hide();
            //},
            error: AjaxFailed,

            //function (e) {

            //    console.log("there is some error");
            //    console.log(e);
            //    $('#loading').hide();
            //}
        });
    }
    else {
        $('#dvCheckBoxListControl').empty();
    }
    $('#loading').hide();
}

function AjaxSucceeded(result) {

    BindCheckBoxList(result);
    $('#loading').hide();
}
function AjaxFailed(result) {

    alert('Failed to load checkbox list');
    console.log("there is some error");
    console.log(e);
    $('#loading').hide();
}
function BindCheckBoxList(result) {
    var items = JSON.parse(result.d);
    CreateCheckBoxList(items);
}
function CreateCheckBoxList(checkboxlistItems) {
    $('#dvCheckBoxListControl').empty();
    if (checkboxlistItems[0].message.length > 0) {
        console.log(checkboxlistItems[0].message);
        return;
    }

    var div = $('<div></div>');
    var counter = 0;
    $(checkboxlistItems).each(function () {
        div.append($('<input>').attr({
            type: 'checkbox', name: 'chklistitem', value: this.weekends, id: 'chklistitem' + counter, 'class': 'chkListItem', checked: this.Checked
        })).append(
            $('<label>').attr({
                for: 'chklistitem' + counter++
            }).text(this.weekends));
    });

    $('#dvCheckBoxListControl').append(div);
    $('#dvCheckBoxListControl').show();
}

function hideimagefunction() {
    $('#loading').hide();
}

function updateEventOnDropResize(event, allDay) {

    //  console.log(event);

    // debugger;

    if (event.end == null) {
        event.end = event.start;
    }

    var start = new Date(dateformatting(event.start) + " " + timeformatting(event.start));//new Date(moment(event.start).format('YYYY-MM-DD'));
    var end = new Date(dateformatting(event.end) + " " + timeformatting(event.end));//new Date(moment(event.end).format('YYYY-MM-DD'));

    //console.log("start: " + start, "end: " + end);

    var nCustId = 0;
    var nEstId = 0;

    if (event.CustomerID === undefined || event.CustomerID === null) {
        nCustId = 0;
    }
    else {
        nCustId = event.CustomerID;
    }

    if (event.EstimateID === undefined || event.EstimateID === null) {
        nEstId = 0;
    }
    else {
        nEstId = event.EstimateID;
    }



    var eventToUpdate = {
        id: event.id,
        start: event.start,
        end: event.end,
        customer_id: nCustId,
        estimate_id: nEstId,
        IsScheduleDayException: event.IsScheduleDayException
    };

    var upStart = new Date(dateformatting(eventToUpdate.start) + " " + timeformatting(eventToUpdate.start));//new Date(moment(eventToUpdate.start).format('YYYY-MM-DD'));
    var upEnd = new Date(dateformatting(eventToUpdate.end) + " " + timeformatting(eventToUpdate.end)); //new Date(moment(eventToUpdate.end).format('YYYY-MM-DD'));

    nevent = event;
    if (event.allDay) {
        upStart.setHours(0, 0, 0);
    }
    //debugger;
    if (event.end === null) {
        upEnd = upStart;
    }
    else {
        upEnd = end;
        if (event.allDay) {
            upEnd.setHours(0, 0, 0);
        }
    }
    eventToUpdate.start = upStart.format("dd-MM-yyyy hh:mm:ss tt");
    eventToUpdate.end = upEnd.format("dd-MM-yyyy hh:mm:ss tt");

    PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);

    //if (eventToUpdate.estimate_id != "" && eventToUpdate.estimate_id != "0" && event.TypeID != 2) {

    //    $(function () {
    //        // $('#dialog-confirm').dialog('open');
    //        nUpdateEventTime(nevent);
    //        //$("#dialog-confirm").dialog({
    //        //    resizable: false,
    //        //    height: 150,
    //        //    modal: true,
    //        //    buttons: {
    //        //        Cancel: function () {
    //        //            nCancle(nevent);
    //        //            $(this).dialog("close");
    //        //        },
    //        //        No: function () {
    //        //            nUpdateEventTime(nevent);
    //        //            $(this).dialog("close");
    //        //        },
    //        //        Yes: function () {
    //        //            nUpdateEventTimeAll(nevent);
    //        //            $(this).dialog("close");
    //        //        }
    //        //    },
    //        //    close: function () {
    //        //        //nCancle();
    //        //    },
    //        //    dialogClass: 'no-close'
    //        //});
    //    });

    //    function nCancle(event) {
    //        $('#calendar').fullCalendar('refetchEvents');
    //    }
    //    function nUpdateEventTime(event) {

    //        debugger;

    //        var test

    //        var nStart = new Date(dateformatting(event.start) + " " + timeformatting(event.start));//new Date(moment(event.start).format('YYYY-MM-DD'));
    //        var nEnd = new Date(dateformatting(event.end) + " " + timeformatting(event.end));// new Date(moment(event.end).format('YYYY-MM-DD'));

    //        var eventToUpdate = {
    //            id: event.id,
    //            start: nStart,
    //            end: nEnd,
    //            customer_id: event.CustomerID,
    //            estimate_id: event.EstimateID
    //        };

    //        var nUpStart = new Date(dateformatting(eventToUpdate.start) + " " + timeformatting(eventToUpdate.start));//new Date(moment(eventToUpdate.start).format('YYYY-MM-DD'));
    //        var nUpEnd = new Date(dateformatting(eventToUpdate.end) + " " + timeformatting(eventToUpdate.end));//new Date(moment(eventToUpdate.end).format('YYYY-MM-DD'));

    //        var nevent = eventToUpdate;
    //        if (event.allDay) {
    //            nUpStart.setHours(0, 0, 0);

    //        }

    //        if (nEnd === null) {
    //            nUpEnd = nUpStart;
    //        }
    //        else {
    //            nUpEnd = nEnd;

    //            if (event.allDay) {
    //                nUpEnd.setHours(0, 0, 0);
    //            }
    //        }
    //        eventToUpdate.start = nUpStart.format("dd-MM-yyyy hh:mm:ss tt");
    //        eventToUpdate.end = nUpEnd.format("dd-MM-yyyy hh:mm:ss tt");

    //        // $('#loading').show();

    //        //console.log("nUpdateEventTime" + eventToUpdate.start, eventToUpdate.end);

    //        PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);
    //    }
    //    function nUpdateEventTimeAll(event) {
    //        var eventToUpdate = {
    //            id: event.id,
    //            start: event.start,
    //            customer_id: event.CustomerID,
    //            estimate_id: event.EstimateID
    //        };
    //        var nevent = eventToUpdate;
    //        if (allDay) {
    //            eventToUpdate.start.setHours(0, 0, 0);

    //        }

    //        if (event.end === null) {
    //            eventToUpdate.end = eventToUpdate.start;

    //        }
    //        else {
    //            eventToUpdate.end = event.end;
    //            if (allDay) {
    //                eventToUpdate.end.setHours(0, 0, 0);
    //            }
    //        }
    //        eventToUpdate.start = eventToUpdate.start.format("dd-MM-yyyy hh:mm:ss tt");
    //        eventToUpdate.end = eventToUpdate.end.format("dd-MM-yyyy hh:mm:ss tt");
    //        $('#loading').show();
    //        PageMethods.UpdateEventTimeAll(eventToUpdate, UpdateTimeSuccessAll);
    //    }
    //}
    //else {
    //    eventToUpdate.start = nUpStart.format("dd-MM-yyyy hh:mm:ss tt");
    //    eventToUpdate.end = nUpEnd.format("dd-MM-yyyy hh:mm:ss tt");

    //    PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);
    //}


}

function updateEventOnResize(event, allDay) {
    //console.log("id: "+event.id+", CustomerID: " + event.CustomerID + ", EstimateID: "+event.EstimateID);
    //console.log("allday: " + allDay);

    // debugger;

    var eventToUpdate = {
        id: event.id,
        start: event.start,
        customer_id: event.CustomerID,
        estimate_id: event.EstimateID
    };

    var start = new Date(moment(event.start).format('YYYY-MM-DD'));
    var end = new Date(moment(event.end).format('YYYY-MM-DD'));

    var upStart = new Date(moment(eventToUpdate.start).format('YYYY-MM-DD'));
    var upEnd = new Date(moment(eventToUpdate.end).format('YYYY-MM-DD'));

    nevent = event;
    if (event.allDay) {
        upStart.setHours(0, 0, 0);
    }

    if (event.end === null) {
        upEnd = upStart;
    }
    else {
        upEnd = end;
        if (allDay) {
            upStart.setHours(0, 0, 0);
        }
    }
    eventToUpdate.start = upStart.format("dd-MM-yyyy hh:mm:ss tt");
    eventToUpdate.end = upEnd.format("dd-MM-yyyy hh:mm:ss tt");

    if (eventToUpdate.estimate_id != "" && eventToUpdate.estimate_id != "0") {

        var eventToUpdate = {
            id: event.id,
            start: event.start,
            customer_id: event.CustomerID,
            estimate_id: event.EstimateID
        };
        var nevent = eventToUpdate;
        if (allDay) {
            eventToUpdate.start.setHours(0, 0, 0);

        }

        if (event.end === null) {
            eventToUpdate.end = eventToUpdate.start;

        }
        else {
            eventToUpdate.end = event.end;
            if (allDay) {
                eventToUpdate.end.setHours(0, 0, 0);
            }
        }
        eventToUpdate.start = eventToUpdate.start.format("dd-MM-yyyy hh:mm:ss tt");
        eventToUpdate.end = eventToUpdate.end.format("dd-MM-yyyy hh:mm:ss tt");
        // $('#loading').show();

        //console.log("updateEventOnResize" + eventToUpdate.start, eventToUpdate.end);

        PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);


    }
    else {
        //   $('#loading').show();

        //console.log("else, updateEventOnResize" + eventToUpdate.start, eventToUpdate.end);

        PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);
    }

    //console.log("Updated");
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

function eventDropped(event, dayDelta, minuteDelta, allDay, revertFunc) {
    $('#loading').show();
    //console.log(event);
    if (!isSelectable) {

        alert("Calendar is Online");
        return false;
    }
    // debugger;
    //console.log(allDay);
    if ($(this).data("qtip")) $(this).qtip("destroy");
    //console.log(event.start);
    updateEventOnDropResize(event, allDay);
}

function eventResized(event, dayDelta, minuteDelta, revertFunc) {

    if (!isSelectable) {
        alert("Calendar is Online");
        return false;
    }
    if ($(this).data("qtip")) $(this).qtip("destroy");

    //updateEventOnDropResize(event);
    updateEventOnResize(event);
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

function ddldependencyType_Onchange() {
    //console.log($("#ddldependencyType option:selected").val());

    if ($("#ddldependencyType option:selected").val() === "1") {
        $("#txtOffsetdays").val(0);
        $("#txtOffsetdays").hide();
    }
    else if ($("#ddldependencyType option:selected").val() === "2") {
        $("#txtOffsetdays").val(1);
        $("#txtOffsetdays").hide();
    }
    else if ($("#ddldependencyType option:selected").val() === "3") {
        $("#txtOffsetdays").val();
        $("#txtOffsetdays").show();
    }
}

function ddlParentdependencyType_Onchange() {
    //console.log($("#ddlParentdependencyType option:selected").val());

    if ($("#ddlParentdependencyType option:selected").val() === "1") {
        $("#txtParentOffsetdays").val(0);
        $("#txtParentOffsetdays").hide();
    }
    else if ($("#ddlParentdependencyType option:selected").val() === "2") {
        $("#txtParentOffsetdays").val(1);
        $("#txtParentOffsetdays").hide();
    }
    else if ($("#ddlParentdependencyType option:selected").val() === "3") {
        $("#txtParentOffsetdays").val();
        $("#txtParentOffsetdays").show();
    }
}

function ddladddependencyType_Onchange() {
    //console.log($("#ddldependencyType option:selected").val());

    if ($("#ddladddependencyType option:selected").val() === "1") {
        $("#txtaddOffsetdays").val(0);
        $("#txtaddOffsetdays").hide();
    }
    else if ($("#ddladddependencyType option:selected").val() === "2") {
        $("#txtaddOffsetdays").val(1);
        $("#txtaddOffsetdays").hide();
    }
    else if ($("#ddladddependencyType option:selected").val() === "3") {
        $("#txtaddOffsetdays").val();
        $("#txtaddOffsetdays").show();
    }
}

function ddladdParentdependencyType_Onchange() {
    //console.log($("#ddlParentdependencyType option:selected").val());

    if ($("#ddladdParentdependencyType option:selected").val() === "1") {
        $("#txtaddParentOffsetdays").val(0);
        $("#txtaddParentOffsetdays").hide();
    }
    else if ($("#ddladdParentdependencyType option:selected").val() === "2") {
        $("#txtaddParentOffsetdays").val(1);
        $("#txtaddParentOffsetdays").hide();
    }
    else if ($("#ddladdParentdependencyType option:selected").val() === "3") {
        $("#txtaddParentOffsetdays").val();
        $("#txtaddParentOffsetdays").show();
    }
}

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}

// Grid(grdCalLinkInfo) DropDown Change
function dependencyTypeChange(objType, objStart, objEnd, objOffset) {
    //  debugger;
    //Parent Event Start & End DateTime
    var vParentEventStart = new Date(document.getElementById("eventStart").value); // Date
    var dParentEventStart = vParentEventStart.getDay(); // Day

    var vParentEventEnd = new Date(document.getElementById("eventEnd").value);// Date
    var dParentEventEnd = vParentEventEnd.getDay();

    //Child Event Start & End DateTime in Grid
    //Gridview Element ID's
    var typeId = objType.id;
    var startId = objStart.id;
    var endId = objEnd.id;
    var offsetId = objOffset.id;

    //Drop Down
    var eTypeId = document.getElementById(typeId);
    var vType = eTypeId.options[eTypeId.selectedIndex].value;

    //Start Label
    var eStartId = document.getElementById(startId); // Id
    var vStartText = eStartId.innerText; // Value
    var dtStart = new Date(vStartText); // Date
    var dStartDate = dtStart.getDay(); // Day
    var tStartTime = timeformatting(vStartText); // Time

    //End Label
    var eEndId = document.getElementById(endId); // Id
    var vEndText = eEndId.innerText; // Value
    var dtEnd = new Date(vEndText); // Date
    var dEndDate = dtEnd.getDay(); // Day
    var tEndTime = timeformatting(vEndText); // Time

    //Date Difference
    var nDays = (dEndDate - dStartDate);

    //Offset Days
    var nOffsetDays = document.getElementById(offsetId).value;

    if (vType === "1")//Start Same Time
    {
        //var nStart = dateformatting(vParentEventStart);
        //var nEnd = dateformatting(vParentEventEnd);

        ////Set Datetime
        //eStartId.innerText = nStart + " " + tStartTime;
        //eEndId.innerText = nEnd + " " + tEndTime;
        $('#' + offsetId + '').css("display", "none");
        $('#' + offsetId + '').val(0);
    }
    else if (vType === "2")//Start After Finish
    {
        //var nStart = dateformatting(vParentEventEnd.addDays(1));
        //var nEnd = dateformatting(vParentEventEnd.addDays(nDays + 1));

        ////Set Datetime
        //eStartId.innerText = nStart + " " + tStartTime;
        //eEndId.innerText = nEnd + " " + tEndTime;
        $('#' + offsetId + '').css("display", "none");
        $('#' + offsetId + '').val(1);
    }
    else if (vType === "3")//Offset days
    {
        $('#' + offsetId + '').show();
    }

    //console.log(vParentEventStart);
    //console.log(eStartId);
    //console.log(vType + ", " + vStart + ", " + vEnd);
}

// Grid(grdCalLinkInfo) TextBox Change
function txtOffsetChange(objType, objStart, objEnd, objOffset) {
    // console.log(objType, objStart, objEnd, objOffset);


    return; // returned, as  this code is now in Code behind c#


    //debugger;
    //Parent Event Start & End DateTime
    var vParentEventStart = new Date(document.getElementById("eventStart").value);
    var dParentEventStart = vParentEventStart.getDay();

    var vParentEventEnd = new Date(document.getElementById("eventEnd").value);
    var dParentEventEnd = vParentEventEnd.getDay();

    //Gridview Element ID's
    var typeId = objType.id;
    var startId = objStart.id;
    var endId = objEnd.id;
    var offsetId = objOffset.id;

    //Drop Down
    var eTypeId = document.getElementById(typeId);
    var vType = eTypeId.options[eTypeId.selectedIndex].value;

    //Start Label
    var eStartId = document.getElementById(startId);
    var vStartText = eStartId.innerText;

    var dtStart = new Date(vStartText);

    var dStartDate = dtStart.getDay();
    var tStartTime = timeformatting(vStartText);

    //End Label
    var eEndId = document.getElementById(endId);
    var vEndText = eEndId.innerText;

    var dtEnd = new Date(vEndText);

    var dEndDate = dtEnd.getDay();
    var tEndTime = timeformatting(vEndText);

    //Date Difference
    var nDays = (dStartDate - dEndDate);

    //Offset Days

    var nOffsetDays = parseInt(document.getElementById(offsetId).value);
    //console.log("nOffsetDays: " + nOffsetDays);

    if (vType === "3")//Offset days
    {
        //  debugger;
        var nStart = dateformatting(vParentEventEnd.addDays(nOffsetDays + 1));
        var nEnd = dateformatting(vParentEventEnd.addDays(nDays + 1 + nOffsetDays));

        $('#' + eStartId.id + '').css("color", "orange");
        $('#' + eStartId.id + '').css("transition", "color 0.4s ease");

        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/GetDayOfWeek",
            data: "{'strdt':'" + nStart + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //console.log("nStart:" + data.d);
                var result = data.d;

                eStartId.innerText = result + " " + tStartTime;
                $('#' + eStartId.id + '').css('color', "Black");
            },
            error: function (e) {
                //console.log("there is some error");
                //console.log(e);
            }
        });
        $('#' + eEndId.id + '').css('color', "orange");
        $('#' + eEndId.id + '').css("transition", "color 0.4s ease");

        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/GetDayOfWeek",
            data: "{'strdt':'" + nEnd + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //console.log("nEnd:" + data.d);
                var result = data.d;

                eEndId.innerText = result + " " + tEndTime;
                $('#' + eEndId.id + '').css('color', "Black");
            },
            error: function (e) {
                //console.log("there is some error");
                //console.log(e);
            }
        });

        //  eStartId.innerText = nStart + " " + tStartTime;
        //  eEndId.innerText = nEnd + " " + tEndTime;

        $('#' + offsetId + '').css("display", "block");//.show();
        //Set Datetime
        //eStartId.innerText = "";
        //eEndId.innerText = "";
    }

    //console.log(vParentEventStart);
    //console.log(eStartId);
    //console.log(vType + ", " + vStart + ", " + vEnd);
}

// Grid(grdCalParentLinkInfo) DropDown Change
function parentDependencyTypeChange(objType, objStart, objEnd, objOffset) {
    // debugger;
    //Child Event Start & End DateTime
    var vChildEventStart = new Date(document.getElementById("eventStart").value);// Date
    var dChildEventStart = vChildEventStart.getDay();// Day

    var vChildEventEnd = new Date(document.getElementById("eventEnd").value);// Date
    var dChildEventEnd = vChildEventEnd.getDay();// Day

    //Parent Event Start & End DateTime in Grid
    //Gridview Element ID's
    var typeId = objType.id;
    var startId = objStart.id;
    var endId = objEnd.id;
    var offsetId = objOffset.id;

    //Drop Down
    var eTypeId = document.getElementById(typeId); // Id
    var vType = eTypeId.options[eTypeId.selectedIndex].value; // Value

    //Start Label
    var eStartId = document.getElementById(startId); // Id
    var vStartText = eStartId.innerText; // Value
    var dtStart = new Date(vStartText); // Date
    var dStartDate = dtStart.getDay(); // Day
    var tStartTime = timeformatting(vStartText); // Time

    //End Label
    var eEndId = document.getElementById(endId); // Id
    var vEndText = eEndId.innerText; // Value
    var dtEnd = new Date(vEndText);// Date
    var dEndDate = dtEnd.getDay();// Day
    var tEndTime = timeformatting(vEndText);// Time

    //Date Difference
    // var nDays = (dEndDate - dStartDate);

    var nDays = (dChildEventEnd - dChildEventStart);

    //Offset Days

    var nOffsetDays = document.getElementById(offsetId).value;

    if (vType === "1")//Start Same Time
    {
        var nStart = dateformatting(dtStart);
        var nEnd = dateformatting(dtEnd);

        //Set Datetime
        document.getElementById("eventStart").value = nStart;
        document.getElementById("eventEnd").value = nEnd;
        $('#' + offsetId + '').css("display", "none");
        $('#' + offsetId + '').val(0);
    }
    else if (vType === "2")//Start After Finish
    {
        var nStart = dateformatting(dtStart.addDays(1));
        var nEnd = dateformatting(dtEnd.addDays(nDays + 1));

        //Set Datetime
        document.getElementById("eventStart").value = nStart;
        document.getElementById("eventEnd").value = nEnd;
        $('#' + offsetId + '').css("display", "none");
        $('#' + offsetId + '').val(1);
    }
    else if (vType === "3")//Offset days
    {
        $('#' + offsetId + '').show();
    }

    //console.log(vChildEventStart);
    //console.log(eStartId);
    //console.log(vType + ", " + vStart + ", " + vEnd);
}

// Grid(grdCalParentLinkInfo) TextBox Change
function txtParentOffsetChange(objType, objStart, objEnd, objOffset) {
    ////  debugger;
    ////Child Event Start & End DateTime
    //var vChildEventStart = new Date(document.getElementById("eventStart").value); // Date
    //var dChildEventStart = vChildEventStart.getDay(); // Day

    //var vChildEventEnd = new Date(document.getElementById("eventEnd").value); // Date
    //var dChildEventEnd = vChildEventEnd.getDay(); // Day

    ////Parent Event Start & End DateTime in Grid
    ////Gridview Element ID's
    //var typeId = objType.id;
    //var startId = objStart.id;
    //var endId = objEnd.id;
    //var offsetId = objOffset.id;

    ////Drop Down
    //var eTypeId = document.getElementById(typeId); // Id
    //var vType = eTypeId.options[eTypeId.selectedIndex].value; // Value

    ////Start Label
    //var eStartId = document.getElementById(startId); // Id
    //var vStartText = eStartId.innerText; // Value
    //var dtStart = new Date(vStartText); // Date
    //var dStartDate = dtStart.getDay(); // Day
    //var tStartTime = timeformatting(vStartText); // Time

    ////End Label
    //var eEndId = document.getElementById(endId); // Id
    //var vEndText = eEndId.innerText; // Value
    //var dtEnd = new Date(vEndText); // Date
    //var dEndDate = dtEnd.getDay(); // Day
    //var tEndTime = timeformatting(vEndText); // Time

    ////Date Difference
    //// var nDays = (dEndDate - dStartDate);
    //var nDays = (dChildEventEnd - dChildEventStart);

    ////Offset Days
    //var nOffsetDays = parseInt(document.getElementById(offsetId).value);
    ////console.log("nOffsetDays: " + nOffsetDays);

    //if (vType === "3")//Offset days
    //{
    //    //  debugger;
    //    var nStart = dateformatting(dtEnd.addDays(nOffsetDays + 1));
    //    var nEnd = dateformatting(dtEnd.addDays(nDays + 1 + nOffsetDays));

    //    $('#eventStart').css("color", "orange");
    //    $('#eventStart').css("transition", "color 0.4s ease");

    //    $.ajax({
    //        type: "POST",
    //        url: "schedulecalendar.aspx/GetDayOfWeek",
    //        data: "{'strdt':'" + nStart + "'}",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (data) {
    //            //console.log("nStart:" + data.d);
    //            var result = data.d;

    //            document.getElementById("eventStart").value = result;
    //            $('#eventStart').css('color', "Black");
    //        },
    //        error: function (e) {
    //            //console.log(e);
    //        }
    //    });
    //    $('#eventEnd').css('color', "orange");
    //    $('#eventEnd').css("transition", "color 0.4s ease");

    //    $.ajax({
    //        type: "POST",
    //        url: "schedulecalendar.aspx/GetDayOfWeek",
    //        data: "{'strdt':'" + nEnd + "'}",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (data) {
    //            //console.log("nEnd:" + data.d);
    //            var result = data.d;

    //            document.getElementById("eventEnd").value = result;
    //            $('#eventEnd').css('color', "Black");
    //        },
    //        error: function (e) {
    //            //console.log(e);
    //        }
    //    });


    //    $('#' + offsetId + '').css("display", "block");

    //}


}

function CalStateAction() {
    var isCalOnline = (document.getElementById("head_hdnCalStateAction").value === 'false');

    //console.log(isCalOnline);

    var calStatus = "Go Online";

    if (!isCalOnline) {
        calStatus = "Go Offline";

        return confirm('Are You sure want to ' + calStatus + '?');
    }
    else {

        if (confirm('Would you like notify the Customer of this change?\nClick OK to send Notification')) {
            $("#head_btnHdnSendCalEmail").click();
            return false;
        }
        else {
            return true;
        }

    }
}


$(document).ready(function () {

    $("#head_ddlDivision").change(function () {

        if ($("#head_hdnTypeID").val() === "2") {
            console.log("ddlDivision");
            console.log($("#head_hdnTypeID").val());

            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/SetDivisionIdOnCahnge",
                data: "{'divisionId':'" + $("#head_ddlDivision").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("nStart:" + data.d);
                    var result = data.d;

                    window.location = "schedulecalendar.aspx?TypeID=" + $("#head_hdnTypeID").val();
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });


        }

        $("#head_txtSearch").val("");
        $("#head_txtSection").val("");
        $("#head_txtUser").val("");
        $("#head_txtSuperintendent").val("");

    });





    $("#ddlEventColor").change(function () {
        $(this).removeClass();
        var SelectedClass = $(this).val();
        $(this).addClass(SelectedClass);
    });
    $("#ddladdEventColor").change(function () {
        $(this).removeClass();
        var SelectedClass = $(this).val();
        $(this).addClass(SelectedClass);
    });

    $("#btnParentEventLink").click(function () {
        $("#linkToSubsequent").val('');
    });

    $("#btnEventLink").click(function () {
        $("#linkToParent").val('');
    });

    $("#btnaddParentEventLink").click(function () {
        $("#addlinkToSubsequent").val('');
    });

    $("#btnaddEventLink").click(function () {
        $("#addlinkToParent").val('');
    });

    //$(window).on("beforeunload", function () {
    //    localStorage.clear();
    //    console.log("you leaving this page");
    //});

    //$('body').on('click', 'button.fc-basicWeek-button', function () {
    //    alert("msg1");
    //});

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


    $('#head_txtProjectStartDate').datepicker({
        //showButtonPanel: true,       
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        // showButtonPanel: true,
        dateFormat: 'm/dd/yy'

    });


    $('#addEventStartDate').datepicker({
        //showButtonPanel: true,       
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        // showButtonPanel: true,
        dateFormat: 'm/dd/yy'

    });
    $('#addEventEndDate').datepicker({
        //showButtonPanel: true,      
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        // showButtonPanel: true,
        dateFormat: 'm/dd/yy'

    });

    $('#eventStart').datepicker({
        //showButtonPanel: true,       
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        // showButtonPanel: true,
        dateFormat: 'm/dd/yy'

    });
    $('#eventEnd').datepicker({
        //showButtonPanel: true,       
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        // showButtonPanel: true,
        dateFormat: 'm/dd/yy'

    });

    $("#miniCalendar").datepicker({
        showOn: "button",
        buttonImage: "images/calendar.gif",
        buttonImageOnly: true,
        buttonText: "Select date",
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        dateFormat: 'DD, d MM, yy',
        onSelect: function (dateText, dp) {
            $('#calendar').fullCalendar('gotoDate', new Date(Date.parse(dateText)));
            $('#calendar').fullCalendar('changeView', 'basicDay');
        }
    });

    //$('#miniCalendar').datepicker({
    //    //showButtonPanel: true,
    //    showOtherMonths: true,
    //    selectOtherMonths: true,
    //    changeMonth: true,
    //    changeYear: true,
    //    dateFormat: 'DD, d MM, yy',
    //    onSelect: function (dateText, dp) {
    //        $('#calendar').fullCalendar('gotoDate', new Date(Date.parse(dateText)));
    //        $('#calendar').fullCalendar('changeView', 'basicDay');
    //    }
    //});



    document.getElementById("addEventName").value = document.getElementById("head_hdnAddEventName").value;
    document.getElementById("addEventDesc").value = document.getElementById("head_hdnEventDesc").value;

    //$("#head_txtSearch").attr("placeholder", "Search by Last Name");
    //$("#head_txtSection").attr("placeholder", "Section");
    //var j = jQuery.noConflict();




    if (document.getElementById("head_hdnCustIDSelected").value != '')
        nCusomertId = parseInt(document.getElementById("head_hdnCustIDSelected").value);


    if (document.getElementById("head_hdnEstIDSelected").value != '')
        nEstimateId = parseInt(document.getElementById("head_hdnEstIDSelected").value);





    $("#addEventName").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetSectionByCustomerId",
                data: "{'keyword':'" + $("#addEventName").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // console.log(SelectedSectionName);
                    // debugger;
                    console.log(data.d);
                    var result = data.d;
                    response($.map(data.d, function (item) {
                        return {
                            label: item.section_name,
                            desc: item.section_name,
                            value: item.section_name
                        }
                    }));
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        select: function (event, ui) {
            // console.log(SelectedSectionName);
            if (ui.item != null)
                SelectedSectionName = ui.item.desc;
        },
        change: function (event, ui) {
            // console.log(SelectedSectionName);
            if (ui.item != null)
                SelectedSectionName = ui.item.desc;
        }
    });

    $("#addLocation").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetLocationByCustomerId",
                data: "{'keyword':'" + $("#addLocation").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);
                    var result = data.d;
                    response($.map(data.d, function (item) {
                        return {
                            label: item.location_name,
                            desc: item.location_name,
                            value: item.location_name
                        }
                    }));
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        //select: function (event, ui) {
        //    if (ui.item !== null) {
        //        SalesPersonID = ui.item.desc;
        //    }
        //},
        //change: function (event, ui) {
        //    if (ui.item !== null) {
        //        SalesPersonID = ui.item.desc;
        //    }
        //},
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }

    });

    $("#addSalesPersonName")
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    url: "schedulecalendar.aspx/GetSalesPerson",
                    data: "{'keyword':'" + extractLast($("#addSalesPersonName").val()) + "'}", // { keyword: extractLast(request.term) },//
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        //  console.log(data);
                        // response(data);

                        var result = data.d;
                        response($.map(data.d, function (item) {
                            return {
                                label: item.sales_person_name,
                                desc: item.sales_person_id,
                                value: item.sales_person_name
                            }
                        }));
                    },
                    error: function (e) {
                        //console.log("there is some error");
                        //console.log(e);
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                if (ui.item !== null) {
                    //console.log(SalesPersonID);
                    //SalesPersonID = ui.item.desc;

                    var terms = split(this.value);

                    // remove the current input
                    terms.pop();

                    // add the selected item
                    terms.push(ui.item.value);

                    // add placeholder to get the comma-and-space at the end
                    terms.push("");
                    this.value = terms.join(", ");
                    return false;
                }
            },
            search: function () {
                // custom minLength
                //var term = extractLast(this.value);
                //if (term.length < 2) {
                //    return false;
                //}
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            //change: function (event, ui) {
            //    if (ui.item !== null) {
            //        console.log(SalesPersonID);
            //        SalesPersonID = ui.item.desc;
            //    }
            //},
            messages: {
                noResults: "",
                results: function () { }
            },
            // search: function () { $(this).addClass('progress'); },
            open: function () { $(this).removeClass('progress'); },
            response: function () { $(this).removeClass('progress'); }

        });



    $("#eventName").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetSectionByCustomerId",
                data: "{'keyword':'" + $("#eventName").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // console.log(data.d);
                    var result = data.d;
                    response($.map(data.d, function (item) {
                        return {
                            label: item.section_name,
                            desc: item.section_name,
                            value: item.section_name
                        }
                    }));
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        select: function (event, ui) {
            if (ui.item != null)
                SelectedSectionName = ui.item.desc;
            else {
                SelectedSectionName = $("#eventName").val()
            }
        },
        change: function (event, ui) {
            if (ui.item != null)
                SelectedSectionName = ui.item.desc;
            else {
                SelectedSectionName = $("#eventName").val()
            }
        }
    });

    $("#eventLocation").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetLocationByCustomerId",
                data: "{'keyword':'" + $("#eventLocation").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);
                    var result = data.d;
                    response($.map(data.d, function (item) {
                        return {
                            label: item.location_name,
                            desc: item.location_name,
                            value: item.location_name
                        }
                    }));
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        //select: function (event, ui) {
        //    if (ui.item !== null) {
        //        SalesPersonID = ui.item.desc;
        //    }
        //},
        //change: function (event, ui) {
        //    if (ui.item !== null) {
        //        SalesPersonID = ui.item.desc;
        //    }
        //},
        messages: {
            noResults: "",
            results: function () { }
        },
        search: function () { $(this).addClass('progress'); },
        open: function () { $(this).removeClass('progress'); },
        response: function () { $(this).removeClass('progress'); }

    });

    function split(val) {
        return val.split(/,\s*/);
    }

    function extractLast(term) {
        return split(term).pop();
    }

    $("#eventSalesPerson")
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    url: "schedulecalendar.aspx/GetSalesPerson",
                    data: "{'keyword':'" + extractLast($("#eventSalesPerson").val()) + "'}", // { keyword: extractLast(request.term) },//
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        // console.log(data);
                        // response(data);

                        var result = data.d;
                        response($.map(data.d, function (item) {
                            return {
                                label: item.sales_person_name,
                                desc: item.sales_person_id,
                                value: item.sales_person_name
                            }
                        }));
                    },
                    error: function (e) {
                        //console.log("there is some error");
                        //console.log(e);
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                if (ui.item !== null) {
                    //console.log(SalesPersonID);
                    //SalesPersonID = ui.item.desc;

                    var terms = split(this.value);

                    // remove the current input
                    terms.pop();

                    // add the selected item
                    terms.push(ui.item.value);

                    // add placeholder to get the comma-and-space at the end
                    terms.push("");
                    this.value = terms.join(", ");
                    return false;
                }
            },
            search: function () {
                // custom minLength
                //var term = extractLast(this.value);
                //if (term.length < 2) {
                //    return false;
                //}
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            //change: function (event, ui) {
            //    if (ui.item !== null) {
            //        console.log(SalesPersonID);
            //        SalesPersonID = ui.item.desc;
            //    }
            //},
            messages: {
                noResults: "",
                results: function () { }
            },
            // search: function () { $(this).addClass('progress'); },
            open: function () { $(this).removeClass('progress'); },
            response: function () { $(this).removeClass('progress'); }

        });

    $("#linkToSubsequent").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetSubsequentSection",
                data: "{'keyword':'" + $("#linkToSubsequent").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "', 'ParentEventName':'" + ParentEventName + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);

                    if (data.d != null || data.d != undefined) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.section_name,
                                desc: item.event_id,
                                value: item.section_name
                            }
                        }));
                    }
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null) {
                ChildEventID = ui.item.desc;
                LinkType = "Child";
            }
        },
        change: function (event, ui) {
            if (ui.item != null) {
                ChildEventID = ui.item.desc;
                LinkType = "Child";
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

    $("#linkToParent").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetParentSection",
                data: "{'keyword':'" + $("#linkToParent").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "', 'ParentEventName':'" + ParentEventName + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);

                    if (data.d != null || data.d != undefined) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.section_name,
                                desc: item.event_id,
                                value: item.section_name
                            }
                        }));
                    }
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null) {
                ChildEventID = ui.item.desc;
                LinkType = "Parent";
            }
        },
        change: function (event, ui) {
            if (ui.item != null) {
                ChildEventID = ui.item.desc;
                LinkType = "Parent";
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

    $("#addlinkToSubsequent").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetSubsequentSection",
                data: "{'keyword':'" + $("#addlinkToSubsequent").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "', 'ParentEventName':'" + $("#addEventName").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);

                    if (data.d != null || data.d != undefined) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.section_name,
                                desc: item.event_id,
                                value: item.section_name
                            }
                        }));
                    }
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null) {
                addChildEventID = ui.item.desc;
                addLinkType = "Child";
            }
        },
        change: function (event, ui) {
            if (ui.item != null) {
                addChildEventID = ui.item.desc;
                addLinkType = "Child";
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

    $("#addlinkToParent").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetParentSection",
                data: "{'keyword':'" + $("#addlinkToParent").val() + "', 'nCustId':'" + nCusomertId + "', 'nEstId':'" + nEstimateId + "', 'ParentEventName':'" + $("#addEventName").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log("addEventName combobox:" + data.d);

                    if (data.d != null || data.d != undefined) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.section_name,
                                desc: item.event_id,
                                value: item.section_name
                            }
                        }));
                    }
                },
                error: function (e) {
                    //console.log("there is some error");
                    //console.log(e);
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            if (ui.item != null) {
                addChildEventID = ui.item.desc;
                addLinkType = "Parent";
            }
        },
        change: function (event, ui) {
            if (ui.item != null) {
                addChildEventID = ui.item.desc;
                addLinkType = "Parent";
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


    $("#head_txtSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "schedulecalendar.aspx/GetCustomer",
                data: "{'keyword':'" + $("#head_txtSearch").val() + "', 'divisionId':'" + $("#head_ddlDivision").val() + "'}",
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
                url: "schedulecalendar.aspx/GetSection",
                data: "{'keyword':'" + $("#head_txtSection").val() + "', 'divisionId':'" + $("#head_ddlDivision").val() + "'}",
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
                url: "schedulecalendar.aspx/GetUserName",
                data: "{'keyword':'" + $("#head_txtUser").val() + "', 'divisionId':'" + $("#head_ddlDivision").val() + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var result = data.d;
                    //  console.log(data.d);
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
                url: "schedulecalendar.aspx/GetSuperintendentName",
                data: "{'keyword':'" + $("#head_txtSuperintendent").val() + "', 'divisionId':'" + $("#head_ddlDivision").val() + "'}",
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

        if ($("#head_txtSearch").val() === '' && $("#head_txtSection").val() === '' && $("#head_txtUser").val() === '' && $("#head_txtSuperintendent").val() === '')
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
        // $('.fc-button-today span').click();
        window.location = "schedulecalendar.aspx?TypeID=1";

    });

    // update Dialog
    //  j('#updatedialog').dialog('open');
    $("#chkScheduleDayException").click(function () {

        var isChecked = $(this).prop("checked");
        //console.log(isChecked);

        if (isChecked) {
            $('#EventLinkSection').hide();
            $("#linkToParent").val('');
            $("#linkToSubsequent").val('');

            ChildEventID = 0;
        }
        else {
            $('#EventLinkSection').show();
            $('#trParentSection').css('display', '');
        }

    });

    $("#chkScheduleDayException").click(function () {
        var updtStartDate = new Date($("#eventStart").val());
        var updtEndDate = new Date($("#eventEnd").val());
        updateStartEndRangeWeekends(updtStartDate, updtEndDate);

    });

    $("#chkaddScheduleDayException").click(function () {

        var isChecked = $(this).prop("checked");
        //console.log(isChecked);

        if (isChecked) {
            $('#addEventLinkSection').hide();
            $("#addlinkToParent").val('');
            $("#addlinkToSubsequent").val('');
            addChildEventID = 0;

        }
        else {
            $('#addEventLinkSection').show();
            $('#traddParentSection').css('display', '');
        }

    });

    function GetSelection() {
        var allVals = "";
        $('.chkListItem:checked').each(function () {

            allVals += $(this).val() + ", ";
        });
        //$("#result").text("Selected Values: " + allVals);
        return allVals;
    }

    $('#updatedialog').dialog({
        autoOpen: false,
        width: 612,
        dialogClass: 'cssdialog',
        buttons: {
            "Close": function () {
                HasEvent = 0;
                $('#ChildLinkTbl tr').remove();
                $('#ParentLinkTbl tr').remove();
                $('#trParentSection').css('display', 'none');

                $('#btnUpdateLink').css('display', 'none');
                $('#btnDeleteLink').css('display', 'none');
                $('#btnUpdateParentLink').css('display', 'none');
                $('#btnDeleteParentLink').css('display', 'none');
                document.getElementById("head_hdnAddEventName").value = '';
                document.getElementById("head_hdnEventDesc").value = '';
                document.getElementById("addEventName").value = '';
                document.getElementById("addEventDesc").value = '';
                document.getElementById("lbleventName").className = 'hidden';
                document.getElementById("lbleventDesc").className = 'hidden';
                document.getElementById("lblRequired").className = 'hidden';
                document.getElementById("lblNotes").className = 'hidden';
                document.getElementById("lblTradePartner").className = 'hidden';

                document.getElementById("eventName").value = '';
                document.getElementById("eventDesc").value = '';
                document.getElementById("eventLocation").value = '';
                document.getElementById("eventSalesPerson").value = '';

                document.getElementById("linkToParent").value = '';
                $('#ddlParentdependencyType').prop('selectedIndex', 0);
                document.getElementById("txtParentOffsetdays").value = '0';

                document.getElementById("linkToSubsequent").value = '';
                $('#ddldependencyType').prop('selectedIndex', 0);
                document.getElementById("txtOffsetdays").value = '0';

                $('#calendar').fullCalendar('updateEvent', '');
                $(this).dialog("close");
            },
            "Update Event": function () {
                HasEvent = 0;
                document.getElementById("head_hdnAddEventName").value = '';
                document.getElementById("head_hdnEventDesc").value = '';
                if (document.getElementById("eventName").value == '') {
                    document.getElementById("lbleventName").className = 'show';
                }
                //else if (document.getElementById("eventDesc").value == '') {
                //    document.getElementById("lbleventDesc").className = 'show';
                //}
                else if (updateStartDate >= updateEndDate) {
                    document.getElementById("lblRequired").className = 'show';
                }
                else {
                    // console.log("updatedialog, eventSalesPerson: " + $("#eventName").val());
                    var eventToUpdate = {
                        id: currentUpdateEvent.id,
                        title: $("#eventName").val().replace("/", "\/"),
                        section_name: $("#eventName").val().replace("/", "\/"),
                        location_name: $("#eventLocation").val().replace("/", "\/"),
                        description: $("#eventDesc").val().replace("/", "\/").replace(/(\r\n|\n|\r)/gm, " "),
                        start: updateStartDate.format("dd-MM-yyyy hh:mm:ss tt"),//new Date($("#eventStart").val()).format("MM-dd-yyyy") + " " + $("#eventStartTime option:selected").text(), //
                        end: updateEndDate.format("dd-MM-yyyy hh:mm:ss tt"),//new Date($("#eventEnd").val()).format("MM-dd-yyyy") + " " + $("#eventEndTime option:selected").text(),//
                        employee_id: SalesPersonID,
                        employee_name: $("#eventSalesPerson").val(),
                        child_event_id: ChildEventID,
                        dependencyType: $("#ddldependencyType option:selected").val(),
                        offsetDays: $("#txtOffsetdays").val(),
                        parentDependencyType: $("#ddlParentdependencyType option:selected").val(),
                        parentOffsetDays: $("#txtParentOffsetdays").val(),
                        linkType: LinkType,
                        customer_id: currentUpdateEvent.CustomerID,
                        estimate_id: currentUpdateEvent.EstimateID,
                        IsScheduleDayException: $("#chkScheduleDayException").prop("checked"),
                        is_complete: $("#chkComplete").prop("checked"),
                        selectedweekends: GetSelection(),
                        cssClassName: $("#ddlEventColor option:selected").val()

                    };
                    $('#loading').show();
                    // console.log(eventToUpdate);

                    PageMethods.UpdateEvent(eventToUpdate, updateSuccess);

                    document.getElementById("lblRequired").className = 'hidden';


                    currentUpdateEvent.title = $("#eventName").val();
                    currentUpdateEvent.description = $("#eventDesc").val();
                    currentUpdateEvent.start = updateStartDate;
                    currentUpdateEvent.end = updateEndDate;
                    currentUpdateEvent.employee_name = $("#eventSalesPerson").val()
                    currentUpdateEvent.IsScheduleDayException = $("#chkScheduleDayException").prop("checked");
                    currentUpdateEvent.is_complete = $("#chkComplete").prop("checked");
                    currentUpdateEvent.selectedweekends = GetSelection();

                    $('#calendar').fullCalendar('updateEvent', currentUpdateEvent);





                }
            },
            "Delete Event": function () {
                if (confirm("do you really want to delete this event?")) {
                    $('#loading').show();
                    PageMethods.deleteEvent($("#eventId").val(), deleteSuccess);
                    document.getElementById("head_hdnAddEventName").value = '';
                    document.getElementById("head_hdnEventDesc").value = '';
                    $(this).dialog("close");

                    $('#calendar').fullCalendar('removeEvents', $("#eventId").val());
                }
            },
            "Save": function () {
                if (document.getElementById("txtTradePartner").value == '') {
                    document.getElementById('lblTradePartner').innerHTML = 'Required';
                    document.getElementById("lblTradePartner").className = 'show';
                    document.getElementById('lblRequired').innerHTML = '';

                }
                else {
                    var eventToUpdate = {
                        id: currentUpdateEvent.id,
                        start: updateStartDate.format("dd-MM-yyyy hh:mm:ss tt"),
                        end: updateEndDate.format("dd-MM-yyyy hh:mm:ss tt"),
                        trade_partner: $("#txtTradePartner").val().replace("/", "\/").replace(/(\r\n|\n|\r)/gm, " "),
                        operation_notes: $("#txtNotes").val().replace("/", "\/").replace(/(\r\n|\n|\r)/gm, " ")
                    };

                    //console.log(updateStartDate.format("dd-MM-yyyy hh:mm:ss tt"));

                    if (updateStartDate > updateEndDate) {
                        //console.log(updateStartDate);
                        document.getElementById('lblRequired').innerHTML = 'End time must be later than start time';
                        document.getElementById("lblRequired").className = 'show';
                        document.getElementById('lblTradePartner').innerHTML = '';
                    }
                    else {

                        PageMethods.UpdateTradePartner(eventToUpdate, updateSuccessTradePartner);
                        //console.log(currentUpdateEvent.title);
                        $("#eventName").val(currentUpdateEvent.temptitle + ' - ' + $("#txtTradePartner").val());
                        currentUpdateEvent.operation_notes = $("#txtNotes").val();
                        currentUpdateEvent.title = currentUpdateEvent.temptitle + ' - ' + $("#txtTradePartner").val(),
                            currentUpdateEvent.trade_partner = $("#txtTradePartner").val();
                        currentUpdateEvent.start = updateStartDate;
                        currentUpdateEvent.end = updateEndDate;

                        document.getElementById('lblTradePartner').innerHTML = '';

                        document.getElementById('lblRequired').innerHTML = 'Saved Successfully';
                        document.getElementById("lblRequired").className = 'showsuccess';
                        $('#calendar').fullCalendar('updateEvent', currentUpdateEvent);
                    }


                }
            },

            "Go to Activity log": function () {
                //debugger;
                // $('#loading').show();
                //console.log(currentUpdateEvent.TypeID);
                if (currentUpdateEvent.TypeID == "2") {
                    $(this).dialog("close");
                    window.location = "customer_details.aspx?cid=" + currentUpdateEvent.CustomerID + "&callid=" + currentUpdateEvent.EstimateID;
                }
            }
        }
    });
    $("#updatedialog").bind("dialogopen", function (event, ui) {
        //console.log(currentUpdateEvent.TypeID);
        if (currentUpdateEvent.TypeID == "1") {
            $('button:contains(Update Event)').show();
            $('button:contains(Go to Schedule)').hide();
            $('button:contains(Save Notes)').hide();
            $('button:contains(Save)').hide();
            $('button:contains(Go to Activity log)').hide();
            $('button:contains(Delete Event)').show();
            $('#trNotes').hide();
            $('#trTradePartner').hide();
            $("#txtOffsetdays").val(0);
            $("#txtOffsetdays").hide();
            $("#txtParentOffsetdays").val(0);
            $("#txtParentOffsetdays").hide();
            // $('#eventName').trigger('blur');
            // $("#eventName").attr("class", "txtReadOnly");
            //  $("#eventDesc").attr("class", "txtReadOnly");
            //   $("#eventName").attr("disabled", "disabled");
            //  $("#eventDesc").attr("disabled", "disabled");
        }
        else if (currentUpdateEvent.TypeID == "2") {
            $('button:contains(Update Event)').show();
            $('button:contains(Go to Schedule)').hide();
            $('button:contains(Save Notes)').hide();
            $('button:contains(Save)').hide();
            $('button:contains(Go to Activity log)').show();
            //  $("#eventName").attr("class", "txtReadOnly");
            $("#eventName").attr("class", "scTextArea");
            $("#eventDesc").attr("class", "scTextArea");
            //  $("#eventName").attr("disabled", "disabled");
            $("#eventName").removeAttr("disabled");
            $("#eventDesc").removeAttr("disabled");
            $('button:contains(Delete Event)').show();
            $('#trNotes').hide();
            $('#trTradePartner').hide();
        }
        else {
            $('button:contains(Update Event)').show();
            $('button:contains(Go to Schedule)').hide();
            $('button:contains(Save Notes)').hide();
            $('button:contains(Save)').hide();
            $('button:contains(Go to Activity log)').hide();
            $("#eventName").attr("class", "scTextArea");
            $("#eventDesc").attr("class", "scTextArea");
            $("#eventName").removeAttr("disabled");
            $("#eventDesc").removeAttr("disabled");
            $('button:contains(Delete Event)').show();
            $('#trNotes').hide();
            $('#trTradePartner').hide();
            $("#txtOffsetdays").val(0);
            $("#txtOffsetdays").hide();
            $("#txtParentOffsetdays").val(0);
            $("#txtParentOffsetdays").hide();
        }

    });
    //$("#updatedialog").droppable('disable')

    //$("#updatedialog").bind("dialogbeforeclose", function (event, ui) {
    //    $("#head_hdnUpdateDialogShow").val("false");
    //});

    //add dialog
    $('#addDialog').dialog({
        autoOpen: false,
        width: 485,
        dialogClass: 'cssdialog',
        buttons: {
            "Close": function () {
                document.getElementById("addEventName").value = '';
                document.getElementById("addLocation").value = '';
                document.getElementById("addSalesPersonName").value = '';

                document.getElementById("addEventDesc").value = '';
                if (document.getElementById("head_hdnEventDesc").value != '')
                    document.getElementById("addEventDesc").value = document.getElementById("head_hdnEventDesc").value

                document.getElementById("lbladdEventName").className = 'hidden';
                document.getElementById("lbladdEventDesc").className = 'hidden';
                document.getElementById("lblTime").className = 'hidden';

                document.getElementById("addlinkToParent").value = '';
                $('#ddladdParentdependencyType').prop('selectedIndex', 0);
                document.getElementById("txtaddParentOffsetdays").value = '0';

                document.getElementById("addlinkToSubsequent").value = '';
                $('#ddladddependencyType').prop('selectedIndex', 0);
                document.getElementById("txtaddOffsetdays").value = '0';


                $("select#addEventStartTime option")
                    .each(function () { this.selected = (this.text == "8:00 AM"); });

                $("select#addEventEndTime option")
                    .each(function () { this.selected = (this.text == "9:00 AM"); });

                $(this).dialog("close");
            },
            "Add": function () {
                //console.log(addEndDate);
                if (document.getElementById("addEventName").value == '') {
                    document.getElementById("lbladdEventName").className = 'show';
                }
                //else if (document.getElementById("addEventDesc").value == '') {
                //    document.getElementById("lbladdEventDesc").className = 'show';
                //}
                else if (addStartDate >= addEndDate) {
                    document.getElementById("lblTime").className = 'show';
                }
                else {
                    //console.log("addSalesPersonName: " + $("#addSalesPersonName").text() + ", " + $("#addSalesPersonName").val());
                    // $('#loading').show();
                    document.getElementById("lbladdEventName").className = 'hidden';
                    document.getElementById("lbladdEventDesc").className = 'hidden';
                    document.getElementById("lblTime").className = 'hidden';
                    var eventToAdd = {
                        title: $("#addEventName").val(),
                        section_name: $("#addEventName").val(),
                        location_name: $("#addLocation").val(),
                        description: $("#addEventDesc").val().replace(/(\r\n|\n|\r)/gm, " "),
                        start: addStartDate.format("dd-MM-yyyy hh:mm:ss tt"),
                        end: addEndDate.format("dd-MM-yyyy hh:mm:ss tt"),
                        allDay: false,
                        employee_id: 0,
                        employee_name: $("#addSalesPersonName").val(),
                        child_event_id: addChildEventID,
                        dependencyType: $("#ddladddependencyType option:selected").val(),
                        offsetDays: $("#txtaddOffsetdays").val(),
                        parentDependencyType: $("#ddladdParentdependencyType option:selected").val(),
                        parentOffsetDays: $("#txtaddParentOffsetdays").val(),
                        linkType: addLinkType,
                        IsScheduleDayException: $("#chkaddScheduleDayException").prop("checked"),
                        is_complete: $("#chkaddComplete").prop("checked"),
                        cssClassName: $("#ddladdEventColor option:selected").val(),
                        client_id: $("#head_ddlDivision").val()
                        //customer_id: currentUpdateEvent.CustomerID,
                        //estimate_id: currentUpdateEvent.EstimateID
                    };
                    $('#loading').show();

                    console.log(eventToAdd);
                    PageMethods.addEvent(eventToAdd, addSuccess);
                    $(this).dialog("close");
                }
            },
            "Cancel Schedule": function () {
                if (confirm("do you really want to cancel this event?")) {
                    PageMethods.cancelEvent(cancelSuccess);

                    document.getElementById("addEventName").value = '';
                    document.getElementById("addEventDesc").value = '';
                    document.getElementById("head_hdnAddEventName").value = '';
                    document.getElementById("head_hdnEventDesc").value = '';

                    $(this).dialog("close");
                    window.location = "customer_details.aspx?cid=" + document.getElementById("head_hdnCustomerID").value;
                    //$('#calendar').fullCalendar('removeEvents', $("#eventId").val());
                }
            }
        }
    });

    $("#addDialog").bind("dialogopen", function (event, ui) {


        $("#txtaddParentOffsetdays").val(0);
        $("#txtaddParentOffsetdays").hide();
        $("#txtaddOffsetdays").val(0);
        $("#txtaddOffsetdays").hide();

        //  var tesett = $("#head_hdnTypeID").val();

        //  console.log(tesett);
        if ($("#head_hdnTypeID").val() == "2" || $("#head_hdnTypeID").val() == "22" || $("#head_hdnTypeID").val() == "0") {
            $("#divaddScheduleDayException").hide();
            $("#addEventLinkSection").hide();


        }
        //  debugger;
        //console.log(document.getElementById("head_hdnEstimateID").value);
        if (document.getElementById("head_hdnEstimateID").value != "0") {
            $('button:contains(Cancel Schedule)').hide();
        }
        else {

            $('button:contains(Cancel Schedule)').show();

        }
    });

    $('#external-events .fc-drag').each(function () {

        // store data so the calendar knows to render an event upon drop
        $(this).data('event', {
            title: $.trim($(this).text()), // use the element's text as the event title
            stick: true, // maintain when user navigates (see docs on the renderEvent method)
            customer_id: $(this).closest("tr").find("input[type=hidden][id*=customer_id]").val(),
            estimate_id: $(this).closest("tr").find("input[type=hidden][id*=estimate_id]").val(),
            cssClassName: $(this).closest("tr").find("input[type=hidden][id*=cssClassName]").val(),
        });

        // make the event draggable using jQuery UI
        $(this).draggable({
            helper: 'clone',
            zIndex: 9999999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0,  //  original position after the drag
            drag: function (event, ui) {
                if (isSelectable) {
                    return true;
                }
                else {
                    alert("Calendar is Online");
                    return false;
                }
            }
        });

    });

    $('#estimate-sections .fc-event').each(function () {

        //  var nCustomer_id = $(this).closest("tr").find("input[type=hidden][id*=customer_id]").val();
        //  var nEstimate_id = $(this).closest("tr").find("input[type=hidden][id*=estimate_id]").val();
        //  console.log(nCustomer_id, nEstimate_id);
        // store data so the calendar knows to render an event upon drop
        $(this).data('event', {
            title: $.trim($(this).text()), // use the element's text as the event title
            stick: true, // maintain when user navigates (see docs on the renderEvent method)
            customer_id: $(this).closest("tr").find("input[type=hidden][id*=customer_id]").val(),
            estimate_id: $(this).closest("tr").find("input[type=hidden][id*=estimate_id]").val(),
            cssClassName: $(this).closest("tr").find("input[type=hidden][id*=cssClassName]").val(),
            client_id: $(this).closest("tr").find("input[type=hidden][id*=client_id]").val(),
        });

        // make the event draggable using jQuery UI
        $(this).draggable({
            helper: 'clone',
            zIndex: 9999999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0,  //  original position after the drag
            drag: function (event, ui) {
                if (isSelectable) {
                    return true;
                }
                else {
                    alert("Calendar is Online");
                    return false;
                }
            }
        });

    });

    var date = new Date();

    if (document.getElementById("head_hdnEventStartDate").value != '') {
        date = new Date(document.getElementById("head_hdnEventStartDate").value);
        //console.log("date: " + date);
    }


    isSelectable = (document.getElementById("head_hdnCalStateAction").value === 'false');
    console.log("head_hdnCalStateAction: " + document.getElementById("head_hdnCalStateAction").value);
    console.log("isSelectable: " + isSelectable);

    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    var tt = date.getTime();
    var firstHour = date.getHours();

    var currentDate = moment(new Date()).format('YYYY-MM-DD');
    //console.log(currentDate);
    //console.log(localStorage.getItem("fcDefaultDate"));
    //console.log(localStorage.getItem("fcDefaultView"));


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
        eventOrder: 'customer_last_name', // default is  title

        theme: true,
        header: {
            left: '',
            center: 'prev, title, next, today',
            right: 'month,basicWeek,basicDay,listWeek'
        },
        buttonText: {
            prevYear: parseInt(new Date().getFullYear(), 10) - 1,
            nextYear: parseInt(new Date().getFullYear(), 10) + 1
        },
        viewRender: function (view, element) {
            var vSDate = $('#calendar').fullCalendar('getDate');
            //  debugger;


            //  var vSelectedDate = dateformatting(vSDate);
            //   console.log("viewRender: " + moment(vSDate).format('YYYY-MM-DD'));

            // var view = $('#calendar').fullCalendar('getView');
            //console.log(view);
            //console.log(vSDate);
            //console.log(moment(vSDate).format('YYYY-MM-DD'));

            localStorage.setItem("fcDefaultView", view.name);
            localStorage.setItem("fcDefaultDate", moment(vSDate).format('YYYY-MM-DD'));
            // "2019-01-01"

        },
        viewDisplay: function (view) {
            var d = $('#calendar').fullCalendar('getDate');


            $(".fc-button-prevYear .fc-button-content").text(parseInt(d.getFullYear(), 10) - 1);
            $(".fc-button-nextYear .fc-button-content").text(parseInt(d.getFullYear(), 10) + 1);


        },

        ignoreTimezone: false,

        defaultDate: (localStorage.getItem("fcDefaultDate") != 'null' ? localStorage.getItem("fcDefaultDate") : currentDate),
        defaultView: (localStorage.getItem("fcDefaultView") != 'null' ? localStorage.getItem("fcDefaultView") : "month"),

        eventClick: updateEvent,
        selectable: isSelectable,
        selectHelper: true,
        select: selectDate,

        eventStartEditable: isSelectable,
        events: "JsonResponseScheduler.ashx",

        displayEventTime: true,

        eventDrop: eventDropped,

        eventResize: eventResized,
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
                        text: "Project: " + (event.customer_last_name.length != 0 ? event.customer_last_name + " (" + event.estimate_name + ")" : event.estimate_name) +
                            "<br/>Start: " + moment(event.start).format('MM-DD-YYYY') +
                            "<br/>End: " + moment(event.end).format('MM-DD-YYYY') +
                            "<br/> Status: " + (event.is_complete === 'True' ? "Complete" : "Not Complete") +
                            "<br/> Duration: " + (event.duration <= 1 ? event.duration + " Day" : event.duration + " Days") +
                            "<br/> Assigned To: " + event.employee_name +
                            "<br/> Notes: " + event.description

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
        droppable: isSelectable, // this allows things to be dropped onto the calendar
        drop: function (date, jsEvent, ui, resourceId) {
            //debugger;
            console.log("drop");
            console.log($(this).data('event'));
            //console.log(jsEvent);
            if ($(this).data('event') === undefined)
                return;

            var nCustomer_id = $(this).data('event').customer_id;
            var nEstimate_id = $(this).data('event').estimate_id;
            var cssClassName = $(this).data('event').cssClassName;
            var nclient_id = $(this).data('event').client_id;
            var id = jsEvent.target.id;
            var title = jsEvent.target.innerText;

            var arytitle = title.split("- (");
            var vtSection = "";
            var vtLocation = "";

            for (i = 0; i < arytitle.length; i++) {
                if (i == 0) {
                    vtSection = arytitle[i];
                }
                if (i == 1) {
                    vtLocation = arytitle[i];
                }
            }

            // console.log(arytitle, vtSection, vtLocation.replace(")", ""));
            // console.log(id);
            var sectionname = jsEvent.target.innerText;

            var strDate = dateformatting(date);
            var cssClassName = cssClassName;
            // console.log(cssClassName);
            //if (id.includes("head_grdDragTemplateSection_lblSection")) {
            //    $(this).remove();
            //    //console.log("Dropped on " + date + ", dateformatting:" + strDate + ", " + jsEvent.target.innerText);
            //    //console.log("===========================================================================");
            //    // console.log(jsEvent);
            //    // console.log(jsEvent.target.classList[1]);
            //    //console.log("===========================================================================");
            //    //console.log(jsEvent);
            //    //console.log("===========================================================================");
            //    //console.log(resourceId);
            //    // is the "remove after drop" checkbox checked?
            //    //if ($('#drop-remove').is(':checked')) {
            //    //    // if so, remove the element from the "Draggable Events" list
            //    //    $(this).remove();
            //    //}

            //    addeventOnDrop(strDate, title, sectionname, cssClassName);
            //   //  $('#calendar').fullCalendar('refetchEvents');
            //}

            //if (id.includes("head_grdProjectSection_lblSection")) {
            //  if (id.indexOf("head_grdProjectSection_lblSection") !== -1) {
            if (title.length > 0 && nCustomer_id != 0 && nEstimate_id != 0) {
                $('#loading').show();
                $(this).remove();
                //console.log("Dropped on " + date + ", dateformatting:" + strDate + ", " + jsEvent.target.innerText);
                //console.log("===========================================================================");
                // console.log(jsEvent);
                // console.log(jsEvent.target.classList[1]);
                //console.log("===========================================================================");
                //console.log(jsEvent);
                //console.log("===========================================================================");
                //console.log(resourceId);
                // is the "remove after drop" checkbox checked?
                //if ($('#drop-remove').is(':checked')) {
                //    // if so, remove the element from the "Draggable Events" list
                //    $(this).remove();
                //}

                addeventOnDrop(strDate, title, vtSection, vtLocation.replace(")", ""), cssClassName, nCustomer_id, nEstimate_id, nclient_id);
                $('#calendar').fullCalendar('refetchEvents');
            }
        }
    });


    $.ajax({
        type: "POST",
        url: 'schedulecalendar.aspx/GetUnassignedCheckboxState',
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            //  var boolIsUnassignedCheked = (response.d.toLowerCase() === 'true');
            $("#chkUnassigned").prop('checked', response.d);

        },
        error: function (e) {
            console.log("error: " + e);
        }
    });

    $("#chkUnassigned").click(function () {
        $('#loading').hide();
        $.ajax({
            type: "POST",
            url: 'schedulecalendar.aspx/SetUnassignedCheckboxState',
            data: "{'IsCheked':'" + this.checked + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log(response);
                //  var boolIsUnassignedCheked = (response.d.toLowerCase() === 'true');
                // $("#chkUnassigned").prop('checked', response.d);
                if (response.d.customer_id === 0)
                    window.location = 'schedulecalendar.aspx?TypeID=1';
                else
                    window.location = 'schedulecalendar.aspx?eid=' + response.d.estimate_id + '&cid=' + response.d.customer_id + '&TypeID=1';
            },
            error: function (e) {
                console.log("error: " + e);
            }
        });
    });
}
);


function AutoComplete(control) {

}

function AddEventLink() {
    if ($("#linkToSubsequent").val() === '') {
        return;
    }
    var eventToUpdate = {
        parent_event_id: currentUpdateEvent.id,
        child_event_id: ChildEventID,
        dependencyType: $("#ddldependencyType option:selected").val(),
        offsetDays: $("#txtOffsetdays").val(),
        linkType: LinkType,
        customer_id: currentUpdateEvent.CustomerID,
        estimate_id: currentUpdateEvent.EstimateID
    };

    $.ajax({
        type: "POST",
        url: "schedulecalendar.aspx/AddEventLink",
        data: "{'datascEventLinks':'" + JSON.stringify(eventToUpdate) + "'}",
        //  data: "{'parent_event_id':'" + currentUpdateEvent.id + "','child_event_id':'" + ChildEventID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var result = data.d;
            //console.log(data.d);
            if (result == "Ok") {
                loadParentEventLinkTable(currentUpdateEvent);
                loadChildEventLinkTable(currentUpdateEvent);
                // console.log("AddEventLink: " + HasParentEvent);
                $("#linkToSubsequent").val('');
                ChildEventID = 0;


            }

        },
        error: function (e) {
            //console.log("there is some error");
            //console.log(e);
        }
    });
}

function DeleteEventLink() {

    $('#loading').show();
    var scEventLinks = new Array();
    $("#ChildLinkTbl tr:has(td)").each(function () {


        var isChecked = $(this).find('input[type="checkbox"]').prop("checked");
        //console.log(isChecked);

        if (isChecked) {
            var scEventLink = {};

            scEventLink.parent_event_id = $(this).find("td:eq(4)").text();
            scEventLink.child_event_id = $(this).find("td:eq(5)").text();
            scEventLink.customer_id = $(this).find("td:eq(6)").text();
            scEventLink.estimate_id = $(this).find("td:eq(7)").text();
            scEventLink.link_id = $(this).find("td:eq(8)").text();

            scEventLinks.push(scEventLink);

        }

    });
    //  console.log(scEventLinks);


    if (scEventLinks.length > 0) {
        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/DeleteEventLink",
            data: "{'datascEventLinks':'" + JSON.stringify(scEventLinks) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //console.log("DeleteEventLink:" + data.d);
                var result = data.d;


                if (result == "Ok") {
                    loadParentEventLinkTable(currentUpdateEvent);
                    loadChildEventLinkTable(currentUpdateEvent);


                    $('#loading').hide();
                }

            },
            error: function (e) {
                //console.log("there is some error");
                //console.log(e);
                $('#loading').hide();
            }
        });
    }
    else {
        alert("Select Link for Delete");
        $('#loading').hide();
    }
    $('#loading').hide();
}

function UpdateEventLink() {

    if (document.getElementById("head_hdnCustIDSelected").value != '')
        nCusomertId = parseInt(document.getElementById("head_hdnCustIDSelected").value);

    if (document.getElementById("head_hdnEstIDSelected").value != '')
        nEstimateId = parseInt(document.getElementById("head_hdnEstIDSelected").value);

    var scEventLinks = new Array();


    $("#ChildLinkTbl tr:has(td)").each(function () {


        if ($(this).css('display') != 'none') {
            var scEventLink = {};

            var csslbltitle = $(this).find('.csslbltitle').prop("textContent");

            var ddlType = $(this).find('.cssddldependencyType').prop("id");
            var offsetDays = $(this).find('.csstxtOffsetdays').prop("id");

            var eventStart = $(this).find('.csslblStart').prop("id");
            var eventEnd = $(this).find('.csslblEnd').prop("id");

            var ilink_id = $(this).find('.csslink_id');

            var vDependencyType = $("#" + ddlType + " option:selected").val();
            var vOffsetDays = $("#" + offsetDays + "").val();

            var vEventStart = $("#" + eventStart + "").text();
            var vEventEnd = $("#" + eventEnd + "").text();


            scEventLink.parent_event_id = $(this).find("td:eq(4)").text();
            scEventLink.child_event_id = $(this).find("td:eq(5)").text();
            scEventLink.customer_id = $(this).find("td:eq(6)").text();
            scEventLink.estimate_id = $(this).find("td:eq(7)").text();
            scEventLink.link_id = $(this).find("td:eq(8)").text();
            //scEventLink.link_id = $(ilink_id).text(); //$(this).find("td:nth-child(5)").html();
            scEventLink.dependencyType = vDependencyType;
            scEventLink.event_start = vEventStart;//.trim().replace("/", "-").trim().replace("/", "-");
            scEventLink.event_end = vEventEnd;//.trim().replace("/", "-").trim().replace("/", "-");
            scEventLink.offsetdays = vOffsetDays;


            scEventLink.title = csslbltitle;

            scEventLinks.push(scEventLink);
        }

    });
    // console.log(scEventLinks)
    $.ajax({
        type: 'POST',
        url: "schedulecalendar.aspx/UpdateEventLink",
        data: "{'datascEventLinks':'" + JSON.stringify(scEventLinks) + "', 'id':'" + currentUpdateEvent.id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //console.log(data.d);
            var result = data.d;


            if (result == "Ok") {
                // $('#loading').hide();


                loadChildEventLinkTable(currentUpdateEvent);



                $("#head_btnHdn").click();
            }

        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
        }
    });
}

function AddParentEventLink() {
    if ($("#linkToParent").val() === '') {
        return;
    }
    var eventToUpdate = {
        parent_event_id: ChildEventID,
        child_event_id: currentUpdateEvent.id,
        dependencyType: $("#ddldependencyType option:selected").val(),
        offsetDays: $("#txtOffsetdays").val(),
        linkType: LinkType,
        customer_id: currentUpdateEvent.CustomerID,
        estimate_id: currentUpdateEvent.EstimateID
    };




    $.ajax({
        type: "POST",
        url: "schedulecalendar.aspx/AddEventLink",
        data: "{'datascEventLinks':'" + JSON.stringify(eventToUpdate) + "'}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var result = data.d;

            if (result == "Ok") {
                loadParentEventLinkTable(currentUpdateEvent);
                loadChildEventLinkTable(currentUpdateEvent);
                $("#linkToParent").val('');
                ChildEventID = 0;



                $("#head_btnHdn").click();

            }

        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
        }
    });





}

function DeleteParentEventLink() {
    var scEventLinks = new Array();
    $("#ParentLinkTbl tr:has(td)").each(function () {


        var isChecked = $(this).find('input[type="checkbox"]').prop("checked");
        //console.log(isChecked);

        if (isChecked) {
            //var strId = $(this).find("td:eq(5)").text();
            //strEventId += strId + ', ';

            var scEventLink = {};

            scEventLink.parent_event_id = $(this).find("td:eq(4)").text();
            scEventLink.child_event_id = $(this).find("td:eq(5)").text();
            scEventLink.customer_id = $(this).find("td:eq(6)").text();
            scEventLink.estimate_id = $(this).find("td:eq(7)").text();
            scEventLink.link_id = $(this).find("td:eq(8)").text();

            scEventLinks.push(scEventLink);
        }
        //console.log('strEventId: ' + strEventId);
    });

    //  console.log(scEventLinks);

    if (scEventLinks.length > 0) {
        $.ajax({
            type: "POST",
            url: "schedulecalendar.aspx/DeleteParentEventLink",
            data: "{'datascEventLinks':'" + JSON.stringify(scEventLinks) + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //console.log("DeleteParentEventLink:" + data.d);
                var result = data.d;


                if (result == "Ok") {

                    loadParentEventLinkTable(currentUpdateEvent);
                    loadChildEventLinkTable(currentUpdateEvent);


                }

            },
            error: function (e) {
                //console.log("there is some error");
                //console.log(e);
            }
        });
    }
    else {
        alert("Select Link for Delete");
    }
}

function UpdateParentEventLink() {
    // console.log("UpdateParentEventLink");
    if (document.getElementById("head_hdnCustIDSelected").value != '')
        nCusomertId = parseInt(document.getElementById("head_hdnCustIDSelected").value);

    if (document.getElementById("head_hdnEstIDSelected").value != '')
        nEstimateId = parseInt(document.getElementById("head_hdnEstIDSelected").value);

    //Child Event Start & End DateTime
    var vChildEventStart = new Date(document.getElementById("eventStart").value); // Date
    var vChildEventEnd = new Date(document.getElementById("eventEnd").value); // Date

    var scEventLinks = new Array();

    $("#ParentLinkTbl tr:has(td)").each(function () {

        if ($(this).css('display') != 'none') {
            var scEventLink = {};
            var ddlType = $(this).find('.cssddldependencyType').prop("id");
            var offsetDays = $(this).find('.csstxtOffsetdays').prop("id");

            var vDependencyType = $("#" + ddlType + " option:selected").val();
            var vOffsetDays = $("#" + offsetDays + "").val();

            scEventLink.link_id = ParentEventId;

            scEventLink.parent_event_id = $(this).find("td:eq(4)").text();
            scEventLink.child_event_id = $(this).find("td:eq(5)").text();
            scEventLink.customer_id = $(this).find("td:eq(6)").text();
            scEventLink.estimate_id = $(this).find("td:eq(7)").text();


            scEventLink.dependencyType = vDependencyType;
            scEventLink.event_start = vChildEventStart;
            scEventLink.event_end = vChildEventEnd;
            scEventLink.offsetdays = vOffsetDays;

            scEventLinks.push(scEventLink);
        }
        //console.log(scEventLinks);
    });
    //console.log(scEventLinks);
    $.ajax({
        type: 'POST',
        url: "schedulecalendar.aspx/UpdateParentEventLink",
        data: "{'datascEventLinks':'" + JSON.stringify(scEventLinks) + "', 'id':'" + currentUpdateEvent.id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //console.log(data.d);
            var result = data.d;


            if (result == "Ok") {
                // $('#loading').hide();




                loadParentEventLinkTable(currentUpdateEvent);

                $("#head_btnHdn").click();
            }

        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
        }
    });
}

function loadParentEventLinkTable(objEvent) {
    $('#loading').show();
    $.ajax({
        type: "POST",
        url: "schedulecalendar.aspx/GetParentEventTable",
        data: "{'childEventId':'" + objEvent.id + "','ncid':'" + objEvent.CustomerID + "','neid':'" + objEvent.EstimateID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $('#ParentLinkTbl tr').remove();
            //  console.log(response);
            //  debugger;
            if (response.d.length > 0) {
                var trHTML = "<tr>" +
                    "<th style='width: 5%; text-align: center;'>Link</th>" +
                    "<th style='width: 60%; text-align: center;'>Title</th>" +
                    "<th style='width: 15%; text-align: center;'>Start</th>" +
                    "<th style='width: 15%; text-align: center;'>End</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th style='width: 5%; text-align: center;'>&nbsp;</th>" +
                    "</tr>";
                $.each(response.d, function (i, item) {

                    // console.log(item);



                    trHTML += "<tr>" +
                        "<td align='left' style='width: 5%;'>" +
                        "<input id='chkLink" + item.link_id + "' type='checkbox' name=''>" +
                        "</td>" +
                        "<td style='width: 60%; text-align: left;'><label id='lbltitle" + item.link_id + "' class='csslbltitle'>" + item.title + "</label></td>" +
                        "<td style='width: 15%; text-align: center;'><label id='lblStart" + item.link_id + "' class='csslblStart'>" + item.start + "</label></td>" +
                        "<td style='width: 15%; text-align: center;'><label id='lblEnd" + item.link_id + "' class='csslblEnd'>" + item.end + "</label></td>" +
                        "<td class='hdnColumnCss parent'>" + item.parent_event_id + "</td>" +
                        "<td class='hdnColumnCss child'>" + item.child_event_id + "</td>" +
                        "<td class='hdnColumnCss customer'>" + item.customer_id + "</td>" +
                        "<td class='hdnColumnCss estimate'>" + item.estimate_id + "</td>" +
                        "<td class='hdnColumnCss link'>" + item.link_id + "</td>" +
                        "<td align='left' style='width: 5%;'>" +
                        "<select name='grdddldependencyType' id='ddldependencyType" + item.link_id + "' class='cssddldependencyType' onchange='dependencyTypeChange(ddldependencyType" + item.link_id + ", lblStart" + item.link_id + ", lblEnd" + item.link_id + ", txtOffsetdays" + item.link_id + ");'>" +
                        "<option value='1' " + (item.dependencyType == "1" ? "selected" : '') + ">Start Same Time</option>" +
                        "<option value='2' " + (item.dependencyType == "2" ? "selected" : '') + ">Start After Finish</option>" +
                        "<option value='3' " + (item.dependencyType == "3" ? "selected" : '') + ">Offset days</option>" +
                        "</select>" +
                        "<br/>" +
                        "<input id='txtOffsetdays" + item.link_id + "' class='csstxtOffsetdays " + (item.dependencyType == "3" ? "displayshow" : 'displayhide') + "' style='width:50px;' type='text' name='' value='" + item.offsetDays + "'>" +
                        "</td>" +
                        "</tr>";
                });
                $('#ParentLinkTbl').append(trHTML);


                $('#trParentSection').css('display', 'none');
                $("#ParentLinkTbl").css('display', '');
                $("#ParentLinkTbl").delay(2500).fadeIn();
                $('#btnUpdateParentLink').css('display', '');
                $("#btnUpdateParentLink").delay(2500).fadeIn();
                $('#btnDeleteParentLink').css('display', '');
                $("#btnDeleteParentLink").delay(2500).fadeIn();


            }
            else {
                $('#trParentSection').css('display', '');
                $("#trParentSection").delay(2500).fadeIn();
                $("#ParentLinkTbl").css('display', 'none');
                $('#btnUpdateParentLink').css('display', 'none');
                $('#btnDeleteParentLink').css('display', 'none');

            }

            var HasParentEvent = $('#ParentLinkTbl tr').length;
            var HasChildEvent = $('#ChildLinkTbl tr').length;

            if (HasParentEvent > 1 || HasChildEvent > 1) {
                $("#divScheduleDayException").hide();
            }


            $('#loading').hide();
        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
            $('#loading').hide();
        }
    });

    return HasParentEvent;

}

function loadChildEventLinkTable(objEvent) {
    //console.log(objEvent);
    $('#loading').show();
    $.ajax({
        type: "POST",
        url: "schedulecalendar.aspx/GetChildEventTable",
        data: "{'parentEventId':'" + objEvent.id + "','ncid':'" + objEvent.CustomerID + "','neid':'" + objEvent.EstimateID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            $('#ChildLinkTbl tr').remove();
            if (response.d.length > 0) {
                //  console.log(response);
                var trHTML = "<tr>" +
                    "<th style='width: 5%; text-align: center;'>Link</th>" +
                    "<th style='width: 60%; text-align: center;'>Title</th>" +
                    "<th style='width: 15%; text-align: center;'>Start</th>" +
                    "<th style='width: 15%; text-align: center;'>End</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th class='hdnColumnCss'>&nbsp;</th>" +
                    "<th style='width: 5%; text-align: center;'>&nbsp;</th>" +
                    "</tr>";
                $.each(response.d, function (i, item) {

                    // console.log(item);



                    trHTML += "<tr>" +
                        "<td align='left' style='width: 5%;'>" +
                        "<input id='chkLink" + item.link_id + "' type='checkbox' name=''>" +
                        "</td>" +
                        "<td style='width: 60%; text-align: left;'><label id='lbltitle" + item.link_id + "' class='csslbltitle'>" + item.title + "</label></td>" +
                        "<td style='width: 15%; text-align: center;'><label id='lblStart" + item.link_id + "' class='csslblStart'>" + item.start + "</label></td>" +
                        "<td style='width: 15%; text-align: center;'><label id='lblEnd" + item.link_id + "' class='csslblEnd'>" + item.end + "</label></td>" +
                        "<td class='hdnColumnCss parent'>" + item.parent_event_id + "</td>" +
                        "<td class='hdnColumnCss child'>" + item.child_event_id + "</td>" +
                        "<td class='hdnColumnCss customer'>" + item.customer_id + "</td>" +
                        "<td class='hdnColumnCss estimate'>" + item.estimate_id + "</td>" +
                        "<td class='hdnColumnCss link'>" + item.link_id + "</td>" +
                        "<td align='left' style='width: 5%;'>" +
                        "<select name='grdddldependencyType' id='ddldependencyType" + item.link_id + "' class='cssddldependencyType' onchange='dependencyTypeChange(ddldependencyType" + item.link_id + ", lblStart" + item.link_id + ", lblEnd" + item.link_id + ", txtOffsetdays" + item.link_id + ");'>" +
                        "<option value='1' " + (item.dependencyType == "1" ? "selected" : '') + ">Start Same Time</option>" +
                        "<option value='2' " + (item.dependencyType == "2" ? "selected" : '') + ">Start After Finish</option>" +
                        "<option value='3' " + (item.dependencyType == "3" ? "selected" : '') + ">Offset days</option>" +
                        "</select>" +
                        "<br/>" +
                        "<input id='txtOffsetdays" + item.link_id + "' class='csstxtOffsetdays " + (item.dependencyType == "3" ? "displayshow" : 'displayhide') + "' style='width:50px;' type='text' name='' value='" + item.offsetDays + "'>" +
                        "</td>" +
                        "</tr>";
                });
                $('#ChildLinkTbl').append(trHTML);

                $('#btnUpdateLink').css('display', '');
                $("#btnUpdateLink").delay(2500).fadeIn();
                $('#btnDeleteLink').css('display', '');
                $("#btnDeleteLink").delay(2500).fadeIn();




            }
            else {
                $('#btnUpdateLink').css('display', 'none');
                $('#btnDeleteLink').css('display', 'none');


            }

            var HasParentEvent = $('#ParentLinkTbl tr').length;
            var HasChildEvent = $('#ChildLinkTbl tr').length;

            if (HasParentEvent > 1 || HasChildEvent > 1) {
                $("#divScheduleDayException").hide();
            }
            else {
                $("#divScheduleDayException").show();
            }

            $('#loading').hide();
        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
            $('#loading').hide();
        }
    });
    return HasChildEvent;
}

