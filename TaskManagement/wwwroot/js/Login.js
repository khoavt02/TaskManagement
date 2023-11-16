function js_Login() {
    var account = $('#account').val();
    var password = $('#password').val();
    var formData = new FormData();
    formData.append("account", $("#account").val());
    formData.append("password", $("#password").val());
    console.log(account, password);
    $.ajax({
        //type: 'POST',
        //url: "/Login/Login",
        //data: JSON.stringify({
        //    Account: account,
        //    Password: password
        //}),
        //contentType: 'application/json, charset=utf-8',
        //dataType: 'json',
        type: 'POST',
        url: "/Login/Login",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                window.location.href = "/Home";
            } else {
                    var message = rp.message;
                    var title = "";
                    toastr["error"](message, title, {
                        positionClass: 'toast-top-right',
                        closeButton: true,
                        progressBar: true,
                        newestOnTop: true,
                        timeOut: 3000
                    });
            }
        }
    })
}

function js_ChangPassword() {
    var oldPassword = $('#old-password').val();
    var newPassword = $('#new-password').val();
    var rePassword = $('#re-password').val();
    if (oldPassword == "" || newPassword == "" || rePassword == "") {
        toastr["error"]("Bạn chưa nhập đủ thông tin!", "", {
            positionClass: 'toast-top-right',
            closeButton: true,
            progressBar: true,
            newestOnTop: true,
            timeOut: 3000
        });
        return;
    }else if (oldPassword == newPassword) {
        toastr["error"]("Mật khẩu cũ không được giống mật khẩu cũ!", "", {
            positionClass: 'toast-top-right',
            closeButton: true,
            progressBar: true,
            newestOnTop: true,
            timeOut: 3000
        });
        return;
    } else if (rePassword != newPassword) {
        toastr["error"]("Bạn nhập lại mật khẩu mới chưa khớp!", "", {
            positionClass: 'toast-top-right',
            closeButton: true,
            progressBar: true,
            newestOnTop: true,
            timeOut: 3000
        });
        return;
    }
    var formData = new FormData();
    formData.append("old_password", oldPassword);
    formData.append("new_password", newPassword);
    formData.append("re_password", rePassword);
    $.ajax({
        //type: 'POST',
        //url: "/Login/Login",
        //data: JSON.stringify({
        //    Account: account,
        //    Password: password
        //}),
        //contentType: 'application/json, charset=utf-8',
        //dataType: 'json',
        type: 'POST',
        url: "/Login/ActionChangePassword",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                toastr["success"]("Đổi mật khẩu thành công!", "", {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    timeOut: 3000
                });
                window.location.href = '/Login';
            } else {
                console.log(rp.message)
                toastr["success"](rp.message, "", {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    timeOut: 3000
                });
            }
        }
    })
}