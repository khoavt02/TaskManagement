// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function DisplayPersonalNotification(message, title) {
    console.log("OK");
    setTimeout(function () {
        toastr["success"](title, message, {
            positionClass: 'toast-top-right',
            closeButton: true,
            progressBar: true,
            newestOnTop: true,
            timeOut: 3000
        });

    }, 1300);
}

