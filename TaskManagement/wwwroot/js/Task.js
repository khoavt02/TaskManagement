$(function () {
    $('#menu-management').addClass("active");
    $('#menu-management-department').addClass("active");
    $('.select2').select2();
    $('#datetimepicker-date-1').datetimepicker({
        format: 'L'
    });
    $('#datetimepicker-date-2').datetimepicker({
        format: 'L'
    });
    js_GetList();
});
function js_closeModalUpdate() {
    $('#ModalUpdateDepartment').modal('hide');
}
function js_AddTask() {
    var point = $('#point').val();
    var priority_level = $('#priority_level').val();
    var department = $('#department').val();
    var manager = $('#manager').val();
    var users = $('#users').val();
    var end_date = $('#end_date').val();
    var start_date = $('#start_date').val();
    var task_description = $('#task_description').val();
    var estimate_time = $('#estimate_time').val();
    var task_name = $('#task_name').val();
    var project_id = $('#project').val();

    var formData = new FormData();
    formData.append("point", point);
    formData.append("priority_level", priority_level);
    formData.append("department", department);
    formData.append("manager", manager);
    formData.append("assigned_user", users);
    formData.append("end_date", end_date);
    formData.append("start_date", start_date);
    formData.append("task_description", task_description);
    formData.append("estimate_time", estimate_time);
    formData.append("task_name", task_name);
    formData.append("project_id", project_id);
    $.ajax({
        type: 'POST',
        url: "/Task/AddTask",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "";
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    timeOut: 3000
                });
                //$("#sizedModalMd").modal("hide");
                //js_GetList();
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

function js_AddTaskChild() {
    var point = $('#point').val();
    var priority_level = $('#priority_level').val();
    var department = $('#department').val();
    var users = $('#users').val();
    var end_date = $('#end_date').val();
    var start_date = $('#start_date').val();
    var task_description = $('#task_description').val();
    var estimate_time = $('#estimate_time').val();
    var task_name = $('#task_name').val();
    var task_parent_code = $('#task_parent_code').val();
    var project_id = $('#project_id').val();
    var task_parent_id = $('#task_parent_id').val();
    var task_parent_code = $('#task_parent_code').val();

    var formData = new FormData();
    formData.append("point", point);
    formData.append("priority_level", priority_level);
    formData.append("department", department);
    formData.append("assigned_user", users);
    formData.append("end_date", end_date);
    formData.append("start_date", start_date);
    formData.append("task_description", task_description);
    formData.append("estimate_time", estimate_time);
    formData.append("task_name", task_name);
    formData.append("task_parent_id", task_parent_id);
    formData.append("task_parent_code", task_parent_code);
    formData.append("project_id", project_id);
    $.ajax({
        type: 'POST',
        url: "/Task/AddTaskChild",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "";
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    timeOut: 3000
                });
                //$("#sizedModalMd").modal("hide");
                //js_GetList();
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
function js_GetList() {
    review_s = $('#review_s').val();
    priority_level_s = $('#priority_level_s').val();
    status_s = $('#status_s').val();
    to_date = $('#from-date').val();
    from_date = $('#to-date').val();
    name_s = $('#name_s').val();
    console.log(review_s, priority_level_s, status_s, to_date, from_date, name_s);
    var objTable = $("#data-table");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Task/GetListTask',
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
                review: review_s
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
                field: "taskCode",
                title: "Mã công việc",
                align: 'left',
                valign: 'left',
            },
            {
                field: "taskName",
                title: "Tên công việc",
                align: 'left',
                valign: 'left',
            },
            //{
            //    field: "department",
            //    title: "Phòng ban",
            //    align: 'left',
            //    valign: 'left',
            //},
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
                field: "estimateTime",
                title: "Thời gian(h)",
                align: 'left',
                valign: 'left',
            },
            {
                field: "processPercent",
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
                    if (row.completeTime != '') {
                        return moment(row.completeTime).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            {
                field: "level",
                title: "Mức độ",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    var html = "";
                    if (row.level == "NORMAL") {
                        html += "<span class= ''>Bình thường</span>"
                    } else if (row.level == "IMPORTANT") {
                        html += "<span class= ''>Quan trọng</span>"
                    }
                    else if (row.level == "HIGH") {
                        html += "<span class= ''>Cao</span>"
                    }
                    else if (row.level == "LOW") {
                        html += "<span class= ''>Thấp</span>"
                    }
                    return html;
                }

            },
            {
                field: "points",
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
                field: "",
                title: "Trạng thái",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    var currentDate = new Date();
                    //currentDate.setDate(currentDate.getDate() + 1);
                    var formattedDate = currentDate.toISOString().split('T')[0] + 'T00:00:00';
                    console.log(formattedDate);

                    var html = "";
                    console.log( row.startTime, row.endTime, row.completeTime);
                    if (row.status == "COMPLETE" && row.completeTime <= row.endTime) {
                        html += "<span class= 'badge badge-pill badge-success'>Hoàn thành</span>";
                    } else if (row.status != "COMPLETE" && ( row.endTime < formattedDate)) {
                        html += "<span class= 'badge badge-pill badge-danger'>Trễ hạn</span>";
                    } else if (row.status == "COMPLETE" && row.completeTime >= row.endTime) {
                        html += "<span class= 'badge badge-pill badge-secondary'>Hoàn thành trễ</span>";
                    } else {
                        html += "<span class= 'badge badge-pill badge-primary'>Đang thực hiện</span>";
                    }
                    return html;
                }

            },
            {
                title: "Thao tác",
                valign: 'middle',
                align: 'center',
                width: '100px',
                class: 'CssAction',
                formatter: function (value, row, index) {
                    var action = '<a class=" btn btn-primary btn-sm btnEdit" title="Chi tiết" href="/Task/TaskDetail?id=' + row.id + '"><i class="align-middle fas fa-fw fa-file"></i></a>'
                    //<a href="javascript:void(0)" class=" btn btn-danger btn-sm button btnDelete" title="Xóa"><i class="align-middle fas fa-fw fa-trash-alt"></i></a>'
                    if (row.status == "COMPLETE") {
                        action += '<a class=" btn btn-success btn-sm ml-1 btnReview" title="Đánh giá"><i class="align-middle fas fa-fw fa-pencil-alt"></i></a>'
                    }
                    return action;
                },
                events: {

                    'click .btnReview': function (e, value, row, index) {
                        try {
                            selectedId = row.id;
                            console.log(row.id, row.projectId);
                            $('#task_id_review').val(row.id);
                            $('#project_id_review').val(row.projectId);
                            $('#ModalAddReview').modal('show');
                        } catch (error) {
                            console.error('Error in click.btnReview:', error);
                        }
                    }
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

function js_AddTaskReview() {
    var review_description = $('#review_description').val();
    var complete_level = $('#complete_level').val();
    var point_review = $('#point_review').val();
    var task_id_review = $('#task_id_review').val();
    var project_id_review = $('#project_id_review').val();
    var formData = new FormData();
    formData.append("task_id_review", task_id_review);
    formData.append("point_review", point_review);
    formData.append("complete_level", complete_level);
    formData.append("review_description", review_description);
    formData.append("project_id_review", project_id_review);
    $.ajax({
        type: 'POST',
        url: "/Task/AddReviewTask",
        contentType: false,
        processData: false,
        cache: false,
        data: formData,
        success: function (rp) {
            if (rp.status == true) {
                console.log(rp.message)
                var message = rp.message;
                var title = "";
                toastr["success"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                    timeOut: 3000
                });
                $("#ModalAddReview").modal("hide");
                js_GetList();
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

