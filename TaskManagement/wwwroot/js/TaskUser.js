$(function () {
    $('#menu-job-user').addClass("active");
    $('#menu-job-task-user').addClass("active");
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
   
    //var formData = new FormData();
    //formData.append("point", point);
    //formData.append("priority_level", priority_level);
    //formData.append("department", department);
    //formData.append("manager", manager);
    //formData.append("assigned_user", users);
    //formData.append("end_date", end_date);
    //formData.append("start_date", start_date);
    //formData.append("task_description", task_description);
    //formData.append("estimate_time", estimate_time);
    //formData.append("task_name", task_name);
    //formData.append("project_id", project_id);
    $.ajax({
        type: 'POST',
        url: "/Task/AddTaskV2",
        dataType: "json",
        //contentType: false,
        //processData: false,
        //cache: false,
        //contentType: 'application/json', // Thêm kiểu dữ liệu
        data: {
            point: point,
            priority_level: priority_level,
            department: department,
            manager: manager,
            users: users,
            end_date: end_date,
            start_date: start_date,
            task_description: task_description,
            estimate_time: estimate_time,
            task_name: task_name,
            project_id: project_id
        }, // Chuyển đối tượng thành JSON
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
                });
                setTimeout(function () {
                    window.location.href = "/Task/ListTask";
                }, 2000);
            } else {
                var message = rp.message;
                var title = "";
                toastr["error"](message, title, {
                    positionClass: 'toast-top-right',
                    closeButton: true,
                    progressBar: true,
                    newestOnTop: true,
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('AJAX Error:', textStatus, errorThrown);
        },
        complete: function () {
            console.log('AJAX Complete');
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
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    name_s = $('#name_s').val();
    project_id = $('#project_s').val();
    console.log(review_s, priority_level_s, status_s, to_date, from_date, name_s);
    var objTable = $("#data-table");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Task/GetListTaskUser',
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
                review: review_s,
                project_id: project_id
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
            {
                field: "taskParentName",
                title: "Công việc cha",
                align: 'left',
                valign: 'left',
            },
            {
                field: "projectName",
                title: "Dự án",
                align: 'left',
                valign: 'left',
            },
            {
                field: "startTime",
                title: "Bắt đầu",
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
                title: "Kết thúc",
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
                title: "Thời gian",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {

                    return row.estimateTime + ' (h)';

                }
            },
            {
                field: "processPercent",
                title: "Tiến độ",
                align: 'left',
                valign: 'left',
            },
            {
                field: "completeTime",
                title: "Hoàn thành",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.completeTime != null) {
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
            //{
            //    field: "createdDate",
            //    title: "Ngày tạo",
            //    align: 'left',
            //    valign: 'left',
            //    formatter: function (value, row, index) {
            //        if (row.createdDate != '') {
            //            return moment(row.createdDate).format('DD/MM/YYYY');
            //        }
            //        else {
            //            return '';
            //        }
            //    }

            //},
            //{
            //    field: "createdName",
            //    title: "Người tạo",
            //    align: 'left',
            //    valign: 'left',

            //},
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
                    if ((row.status == "COMPLETE" || row.status == "EVALUATE") && row.completeTime <= row.endTime) {
                        html += "<span class= 'badge badge-pill badge-success'>Hoàn thành</span>";
                    } else if (row.status != "COMPLETE" && row.status != "EVALUATE" && (row.endTime < formattedDate)) {
                        html += "<span class= 'badge badge-pill badge-danger'>Trễ hạn</span>";
                    } else if ((row.status == "COMPLETE" || row.status == "EVALUATE") && row.completeTime >= row.endTime) {
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
                    if (row.isEvaluated == true) {
                        action += '<a class=" btn btn-success btn-sm ml-1 btnReviewDetail" title="Đánh giá"><i class="align-middle fas fa-fw fa-pencil-alt"></i></a>'
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
                            $('#point_review').val("");
                            $('#complete_level').val("");
                            $('#review_description').val("");
                            $('#ModalAddReview').modal('show');
                        } catch (error) {
                            console.error('Error in click.btnReview:', error);
                        }
                    },
                    'click .btnReviewDetail': function (e, value, row, index) {
                        try {
                            selectedId = row.id;
                            console.log(row.id, row.projectId);
                            $.ajax({
                                type: 'Get',
                                url: '/Task/GetDetailReviewById',
                                data: {
                                    id: row.id
                                },
                                success: function (rp) {
                                    if (rp.status) {
                                        console.log(rp.data);
                                        $('#point_review_d').val(rp.data.taskEvaluate.points);
                                        $('#complete_level_d').val(rp.data.taskEvaluate.content);
                                        $('#complete_level_d').select2();
                                        $('#review_description_d').val(rp.data.taskEvaluate.description);
                                        const inputDate = moment(rp.data.taskEvaluate.createdDate);
                                        const formattedDate = inputDate.format('DD-MM-YYYY HH:mm:ss');
                                        $('#date_review_d').val(formattedDate);
                                        $('#user_review_d').val(rp.data.createdByName);
                                    }
                                    else {
                                        toastr.error(rp.message);
                                    }
                                }
                            })
                            $('#task_id_review').val(row.id);
                            $('#project_id_review').val(row.projectId);
                            $('#point_review').val("");
                            $('#complete_level').val("");
                            $('#review_description').val("");
                            $('#ModalDetailReview').modal('show');
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

function js_ExportExcel() {
    priority_level_s = $('#priority_level_s').val();
    status_s = $('#status_s').val();
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    name_s = $('#name_s').val();
    project_id = $('#project_s').val();
    review_s = $('#review_s').val();
    $.ajax({
        url: '/Task/ExcelListTaskUser',
        type: 'GET',
        data: {
            name: name_s,
            from_date: from_date,
            to_date: to_date,
            status: status_s,
            priority_level: priority_level_s,
            review: review_s,
            project_id: project_id
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
                link.download = 'Danh_sách_công_việc.xlsx';
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


