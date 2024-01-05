
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


connection.on("ReceivedPersonalNotification", function (message, username) {
    console.log(message);
    DisplayPersonalNotification(message, 'Hey ' + username);
});

function getListNotificationByUser() {

}