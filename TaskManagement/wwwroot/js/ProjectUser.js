$(document).ready(function () {
    $('#menu-job-user').addClass("active");
    $('#menu-job-project-user').addClass("active");
    $('.select2').select2();
    $('#datetimepicker-date-1').datetimepicker({
        format: 'L'
    });
    $('#datetimepicker-date-2').datetimepicker({
        format: 'L'
    });
    js_GetList();
});

//$(function () {
   
//});
function js_closeModalUpdate() {
    $('#ModalUpdateDepartment').modal('hide');
}
function js_AddProject() {
    console.log('zzzzzzzzz');
    var point = $('#point').val();
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
    formData.append("point", point);
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
        url: "/ProjectCreate/AddProject",
        contentType: false,
        processData: false,
        cache: false,
        //contentType: 'application/json',
        //dataType: "json",
        data: formData,
        //data: {
        //    point: point,
        //    priority_level: priority_level,
        //    department: department,
        //    manager: manager,
        //    users: users,
        //    end_date: end_date,
        //    start_date: start_date,
        //    project_description: project_description,
        //    project_code: project_code,
        //    project_name: project_name,
        //    file: file
        //},
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "Thông báo";
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    setTimeout: 3000
                });
                //$("#sizedModalMd").modal("hide");
                //js_GetList();
                window.location.href = "/Project/ListProject"
            } else {
                var message = rp.message;
                var title = "Thông báo";
                toastr["error"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    setTimeout: 3000
                });
            }
        }
    })
}

function js_GetList() {
   // department_s = $('#deparment_f').val();
    priority_level_s = $('#priority_level_f').val();
    status_s = $('#status_f').val();
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    name_s = $('#name_f').val();
    var objTable = $("#table-projects");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Project/GetListProjectUser',
        queryParams: function (p) {
            var param = $.extend(true, {
                //keyword: $('#SearchAcademicLevel').val(),
                offset: p.offset,
                limit: p.limit,
                name: name_s,
                from_date: from_date,
                to_date: to_date,
                status: status_s,
                priority_level: priority_level_s,
                //department_s: department_s,
            }, p);
            return param;
        },
        striped: true,
        sidePagination: 'server',
        pagination: true,
        paginationVAlign: 'bottom',
        search: false,
        pageSize: 10,
        pageList: [10, 50, 100],
        columns: [

            {
                field: "projectCode",
                title: "Mã dự án",
                align: 'left',
                valign: 'left',
            },
            {
                field: "projectName",
                title: "Tên dự án",
                align: 'left',
                valign: 'left',
            },
            {
                field: "managerName",
                title: "Quản lý",
                align: 'left',
                valign: 'left',
            },
            {
                field: "departmentName",
                title: "Phòng ban",
                align: 'left',
                valign: 'left',
            },
            {
                field: "startTime",
                title: "Ngày bắt đầu",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.startTime != '') {
                        return moment(row.startTime).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            {
                field: "endTime",
                title: "Ngày kết thúc",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.endTime != '') {
                        return moment(row.endTime).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            {
                field: "process",
                title: "Tiến độ",
                align: 'left',
                valign: 'left',
            },
            {
                field: "completeTime",
                title: "Ngày hoàn thành",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.endTime != '') {
                        return moment(row.endTime).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            {
                field: "priorityLevel",
                title: "Mức độ",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    switch (row.priorityLevel) {
                        case "IMPORTANT":
                            return "Quan trọng";
                        case "HIGH":
                            return "Cao";
                        case "NORMAL":
                            return "Bình thường";
                        case "LOW":
                            return "Thấp";
                        default:
                            return ""; 
                    }
                }
            },
            {
                field: "point",
                title: "Điểm số",
                align: 'left',
                valign: 'left',
            },
            {
                field: "createdDate",
                title: "Ngày tạo",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.createdDate != '') {
                        console.log(row.createdDate);
                        return moment(row.createdDate).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            {
                field: "createdName",
                title: "Người tạo",
                align: 'left',
                valign: 'left',

            },
            {
                title: "Thao tác",
                valign: 'middle',
                align: 'center',
                width: '100px',
                class: 'CssAction',
                formatter: function (value, row, index) {
                    var action = '<a class=" btn btn-primary btn-sm btnEdit" title="Chi tiết" href="/Project/ProjectDetail?id=' + row.id + '"><i class="align-middle fas fa-fw fa-pencil-alt"></i></a>'
                    if (row.linkFiles != null) {
                        action += '<a class=" btn btn-success btn-sm ml-1" href="/uploads/' + row.linkFiles + '"  download target="_blank" title="File đính kèm"><i class="align-middle fas fa-fw fa-file-alt"></i></a>'
                    }
                    return action;
                },
                events: {

                    //'click .btnDelete': function (e, value, row, index) {
                    //    selectedId = row.Id;
                    //    $('#NameAcademicLevelsdelete').text(row.Name);
                    //    $('#confirmDeleteModals').modal('show');
                    //},
                    //'click .btnEdit': function (e, value, row, index) {
                    //    console.log(row);
                    //    $.ajax({
                    //        type: 'Get',
                    //        url: '/Department/GetDetailDepartmentById',
                    //        data: {
                    //            id: row.id
                    //        },
                    //        success: function (rp) {
                    //            if (rp.status) {
                    //                console.log(row.status, typeof (row.status), row)
                    //                $("#ModalUpdateDepartment").modal("show");
                    //                $('#name_u').val(row.departmentName);
                    //                $('#code_u').val(row.departmentCode);
                    //                $('#management_u').val(row.mannager);
                    //                var status = row.status == true ? 1 : 0;
                    //                $('#status_u').val(status);
                    //                $('#department_id').val(row.id);
                    //            }
                    //            else {
                    //                toastr.error(rp.message);
                    //            }
                    //        }
                    //    })
                    //},
                }
            },

        ],
        formatNoMatches: function () {
            return 'Hiện tại chưa có dữ liệu';
        },
        onLoadSuccess: function (data) {

        },
    })
}

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
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "Thông báo";
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                });
                //$("#sizedModalMd").modal("hide");
                //js_GetList();
                setTimeout(function () {
                    window.location.href = "/Project/ListProject"
                }, 2000);
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

function js_ExportExcel() {
    priority_level_s = $('#priority_level_f').val();
    status_s = $('#status_f').val();
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    name_s = $('#name_f').val();
    $.ajax({
        url: '/Project/ExcelListProjectUser',
        type: 'GET',
        data: {
            name: name_s,
            from_date: from_date,
            to_date: to_date,
            status: status_s,
            priority_level: priority_level_s,
        },
        xhrFields: {
            responseType: 'blob' // Set the response type to blob
        },
        success: function (response, status, xhr) {
            // Check if the content type is correct
            if (response.status == false) {
                Toast("Thông báo", response.message, "error")
            }
            var contentType = xhr.getResponseHeader('Content-Type');
            if (contentType === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                // Trigger the file download
                var blob = new Blob([response], { type: contentType });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = 'Danh_sách_dự_án.xlsx';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            } else {
                console.error('Invalid content type: ' + contentType);
            }
        },
        error: function (xhr, status, error) {
            // Handle AJAX error
            console.error(xhr.responseText);
            Toast("Thông báo", "Bạn không có quyền xuất excel công việc!", "error")
        }
    });
}
