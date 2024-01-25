
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.start().then(function (connectionId) {
    console.log('connected to hub with connectionId:' + connectionId);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("OnConnected", function (connectionId) {
    console.log('connected to hub with connectionId:' + connectionId);
   OnConnected();
});

function OnConnected() {
    var username = $('#hfUsername').val();
   connection.invoke("SaveUserConnection", username).catch(function (err) {
        return console.error(err.toString());
    })
}


connection.on("ReceivedPersonalNotification", function (title, message) {
    console.log(message);
    DisplayPersonalNotification(title, message);
    setTimeout(function () {
        getListNotificationByUser();
    }, 5000);
});
function formatDateTime(dateTimeString) {
    const options = {
        hour: 'numeric',
        minute: 'numeric',
        day: 'numeric',
        month: 'numeric',
        year: 'numeric',
        hour12: false
    };
    return new Intl.DateTimeFormat('en-US', options).format(new Date(dateTimeString));
}
function getListNotificationByUser() {
    $.ajax({
        type: "GET",
        url: "/Home/GetListNoti",
        success: function (result) {
            if (result.status) {
                // Clear existing notifications
                //$("#noti-by-user").empty();

                // Update the indicator count
                $("#total-noti").text(result.data.length + " thông báo");
                
                var notiNoRead = 0;
                // Append new notifications
                result.data.forEach(function (notification) {
                    var dateFormat = formatDateTime(notification.notificationDateTime);
                    var html = '';
                    console.log(notification.isRead);
                    if (notification.isRead == true) {
                        html = '<a href="#" onclick="markNotificationAsRead(' + notification.id + ', \'' + notification.link + '\')" class="list-group-item" style="position: relative;">' +
                            '<div class="row no-gutters align-items-center">' +
                            '<div class="col-2">' +
                            '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-bell text-warning" b-7opx5vu9t6=""><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path><path d="M13.73 21a2 2 0 0 1-3.46 0"></path></svg>' +
                            '</div>' +
                            '<div class="col-9">' +
                            '<div class="text-dark">Thông báo</div>' +
                            '<div class="text-muted small mt-1">' + notification.message + '</div>' +
                            '<div class="text-muted small mt-1">' + dateFormat + '</div>' +
                            '</div>' +
                            '<div class="col-1" style="position: absolute; top: 10; right: 0;">' +
                            '<i class="align-middle mr-2 fas fa-fw fa-check text-success"></i>' +
                            '</div>' +
                            '</div>' +
                            '</a>';
                    } else {
                        notiNoRead += 1;
                        html = '<a href="#" onclick="markNotificationAsRead(' + notification.id + ', \'' + notification.link + '\')" class="list-group-item" style="background: aliceblue !important;">' +
                            '<div class="row no-gutters align-items-center">' +
                            '<div class="col-2">' +
                            '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-bell text-warning" b-7opx5vu9t6=""><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path><path d="M13.73 21a2 2 0 0 1-3.46 0"></path></svg>' +
                            '</div>' +
                            '<div class="col-10">' +
                            '<div class="text-dark">Thông báo</div>' +
                            '<div class="text-muted small mt-1">' + notification.message + '</div>' +
                            '<div class="text-muted small mt-1">' + dateFormat + '</div>' +
                            '</div>' +
                            '</div>' +
                            '</a>';
                    }
                   

                    $("#noti-by-user").append(html);
                    $("#alertsDropdown .indicator").text(notiNoRead);
                });
            } else {
                // Handle error or empty data case
                $("#alertsDropdown .list-group").empty();
                $("#alertsDropdown .indicator").text(0);
                $("#total-noti").text(0 + " thông báo");
            }
        },
        error: function () {
            // Handle AJAX error
            console.log("Error fetching notifications.");
        }
    });
}

function markNotificationAsRead(notificationId, link) {
    console.log(notificationId, link);
    var formData = new FormData();
    formData.append("id", notificationId); 
    $.ajax({
        type: "POST",
        url: "/Home/MarkNotificationAsRead",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (result) {
            if (result.status) {
                console.log("Notification marked as read successfully.");
                window.location.href = link;
                // Thực hiện bất kỳ công việc cập nhật giao diện hoặc xử lý nào khác ở đây
            } else {
                console.error("Error marking notification as read.");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('AJAX Error:', textStatus, errorThrown);
        },
    });
}