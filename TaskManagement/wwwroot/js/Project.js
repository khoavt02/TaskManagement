
$(function () {
    $('#menu-job').addClass("active");
    $('#menu-job-project').addClass("active");
    $('.select2').select2();
});
function js_closeModalUpdate() {
    $('#ModalUpdateDepartment').modal('hide');
}
function js_AddProject() {
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
    $.ajax({
        type: 'POST',
        url: "/ProjectCreate/AddProject",
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
    //$.ajax({
    //    url: '/User/GetListUser',
    //    type: 'Get',
    //    //data: JSON.stringify({  }),
    //    contentType: 'application/json, charset=utf-8',
    //    beforeSend: function () {
    //        //js_Loading(true, 1);
    //    },
    //    success: function (data) {
    //        //$("#table-content").find('tbody').empty();
    //        $('#data-table').data(data);
    //        //js_ReloadTable();
    //        //js_Loading(false, 1);
    //    },
    //    error: function () {
    //        //js_Loading(false, 1);
    //    }
    //});
    var objTable = $("#data-table");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Department/GetListDepartment',
        queryParams: function (p) {
            var param = $.extend(true, {
                //keyword: $('#SearchAcademicLevel').val(),
                offset: p.offset,
                limit: p.limit,
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
                field: "departmentCode",
                title: "Mã phòng ban",
                align: 'left',
                valign: 'left',

            },
            {
                field: "departmentName",
                title: "Tên phòng ban",
                align: 'left',
                valign: 'left',

            },
            {
                field: "manager",
                title: "Trưởng phòng",
                align: 'left',
                valign: 'left',

            },
            {
                field: "createdDate",
                title: "Ngày tạo",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.CreatedDate != '') {
                        return moment(row.CreatedDate).format('DD/MM/YYYY');
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
                    var action = '<a class=" btn btn-primary btn-sm btnEdit" title="Sửa"><i class="align-middle fas fa-fw fa-pencil-alt"></i></a>'
                            //<a href="javascript:void(0)" class=" btn btn-danger btn-sm button btnDelete" title="Xóa"><i class="align-middle fas fa-fw fa-trash-alt"></i></a>'
                    return action;
                },
                events: {

                    //'click .btnDelete': function (e, value, row, index) {
                    //    selectedId = row.Id;
                    //    $('#NameAcademicLevelsdelete').text(row.Name);
                    //    $('#confirmDeleteModals').modal('show');
                    //},
                    'click .btnEdit': function (e, value, row, index) {
                        console.log(row);
                        $.ajax({
                            type: 'Get',
                            url: '/Department/GetDetailDepartmentById',
                            data: {
                                id: row.id
                            },
                            success: function (rp) {
                                if (rp.status) {
                                    console.log(row.status, typeof (row.status), row)
                                    $("#ModalUpdateDepartment").modal("show");
                                    $('#name_u').val(row.departmentName);
                                    $('#code_u').val(row.departmentCode);
                                    $('#management_u').val(row.mannager);
                                    var status = row.status == true ? 1 : 0;
                                    $('#status_u').val(status);
                                    $('#department_id').val(row.id);
                                }
                                else {
                                    toastr.error(rp.message);
                                }
                            }
                        })
                    },
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

function js_UpdateDepartment() {
    var name = $('#name_u').val();
    var code = $('#code_u').val();
    var management = $('#management_u').val();
    var status = $('#status_u').val();
    var id = $('#department_id').val();
    var formData = new FormData();
    formData.append("name", name);
    formData.append("code", code);
    formData.append("management", management);
    formData.append("status", status);
    formData.append("id", id);
    $.ajax({
        type: 'POST',
        url: "/Department/UpdateDepartment",
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
                $("#ModalUpdateDepartment").modal("hide");
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

