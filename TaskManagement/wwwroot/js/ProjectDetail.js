$(document).ready(function () {
    $('#menu-job').addClass("active");
    $('#menu-job-project').addClass("active");
    $('.select2').select2();
   
});

function js_UpdateProject() {
    var id = $('#project-id').val();
    var point = $('#point').val();
    var status = $('#status').val();
    var process = $('#process-percent').val();
    var priority_level = $('#priority_level').val();
    var department = $('#department').val();
    var manager = $('#manager').val();
    var users = $('#users').val();
    var end_date = $('#end_date').val();
    var start_date = $('#start_date').val();
    var project_description = $('#project_description').val();
    var project_code = $('#project_code').val();
    var project_name = $('#project_name').val();
    var fileInput = document.getElementById('file_attachment');
    var file = fileInput.files[0];
    console.log(start_date, end_date)
    var formData = new FormData();
    formData.append("id", id);
    formData.append("point", point);
    formData.append("status", status);
    formData.append("process", process);
    formData.append("priority_level", priority_level);
    formData.append("department", department);
    formData.append("manager", manager);
    formData.append("users", users);
    formData.append("end_date", end_date);
    formData.append("start_date", start_date);
    formData.append("project_description", project_description);
    formData.append("project_code", project_code);
    formData.append("project_name", project_name);
    formData.append("file", file);
    $.ajax({
        type: 'POST',
        url: "/Project/UpdateProject",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            console.log(rp);
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "Thông báo";
                //$("#sizedModalMd").modal("hide");
                //js_GetList();
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    setTimeout: 2000
                });
                window.location.href = "/Project/ListProject"
                
            } else {
                var message = rp.message;
                var title = "Thông báo";
                toastr["error"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                });
            }
        }
    })
}

