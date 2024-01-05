$(function () {
    $('#menu-job').addClass("active");
    $('#menu-job-task').addClass("active");
    $('.select2').select2();
    $('#datetimepicker-date-1').datetimepicker({
        format: 'L'
    });
    $('#datetimepicker-date-2').datetimepicker({
        format: 'L'
    });
   
    js_GetList();
});
function closeAllModals() {
    $('.modal').modal('hide');
}

function js_GetList() {
    $.ajax({
        url: "/Task/GetListTaskKaban", // Thay thế ControllerName bằng tên thực tế của Controller
        type: "GET",
        data: {  name: "", from_date: "", to_date: "" },
        success: function (data) {
            // Cập nhật phần nội dung của div taskListContainer với dữ liệu mới
                $("#list-task-kaban").html(data);
        },
        error: function (error) {
            console.log("Error: " + error);
        }
    });
}




