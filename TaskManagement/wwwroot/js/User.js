$(function () {
    $('#menu-management').addClass("active");
    $('#menu-management-user').addClass("active");
    js_GetList();
});
function js_closeModalUpdate() {
    $('#ModalUpdateUser').modal('hide');
}
function js_AddUser() {
    var name = $('#name').val();
    var code = $('#code').val();
    var postion = $('#postion').val();
    var postion_name = $('#postion').find(':selected').html();
    var department = $('#department').val();
    var department_name = $('#department').find(':selected').html();
    var password = $('#password').val();
    var account = $('#account').val();
    var role = $('#role').val();
    var status = $('#status').val();
    var formData = new FormData();
    formData.append("account", account);
    formData.append("password", password);
    formData.append("name", name);
    formData.append("code", code);
    formData.append("postion", postion);
    formData.append("postion_name", postion_name);
    formData.append("department", department);
    formData.append("department_name", department_name);
    formData.append("role", role);
    formData.append("status", status);
    $.ajax({
        type: 'POST',
        url: "/User/AddUser",
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
                $("#sizedModalMd").modal("hide");
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
        url: '/User/GetListUser',
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
                field: "userCode",
                title: "Mã nhân viên",
                align: 'left',
                valign: 'left',

            },
            {
                field: "userName",
                title: "Tên nhân viên",
                align: 'left',
                valign: 'left',

            },
            {
                field: "account",
                title: "Tài khoản",
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
                field: "positionName",
                title: "Vị trí",
                align: 'left',
                valign: 'left',

            },
            {
                field: "roleName",
                title: "Nhóm quyền",
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
                field: "createdBy",
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
                            /*<a href="javascript:void(0)" class=" btn btn-danger btn-sm button btnDelete" title="Xóa"><i class="align-middle fas fa-fw fa-trash-alt"></i></a>*/
                    return action;
                },
                events: {

                    'click .btnDelete': function (e, value, row, index) {
                        selectedId = row.Id;
                        $('#NameAcademicLevelsdelete').text(row.Name);
                        $('#confirmDeleteModals').modal('show');
                    },
                    'click .btnEdit': function (e, value, row, index) {
                        console.log(row);
                        $.ajax({
                            type: 'Get',
                            url: '/User/GetDetailUserById',
                            data: {
                                id: row.id
                            },
                            success: function (rp) {
                                if (rp.status) {
                                    console.log(row.status, typeof (row.status), row)
                                    $("#ModalUpdateUser").modal("show");
                                    $('#name_u').val(row.userName);
                                    $('#code_u').val(row.userCode);
                                    $('#postion_u').val(row.positionCode);
                                    $('#department_u').val(row.departmentCode);
                                    $('#account_u').val(row.account);
                                    $('#role_u').val(row.role);
                                    var status = row.status == true ? 1 : 0;
                                    $('#status_u').val(status);
                                    $('#user_id').val(row.id);
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

function js_UpdateUser() {
    var user_id = $('#user_id').val();
    var name = $('#name_u').val();
    var code = $('#code_u').val();
    var postion = $('#postion_u').val();
    var postion_name = $('#postion_u').find(':selected').html();
    var department = $('#department_u').val();
    var department_name = $('#department_u').find(':selected').html();
    var account = $('#account_u').val();
    var role = $('#role_u').val();
    var status = $('#status_u').val();
    var formData = new FormData();
    formData.append("account", account);
    formData.append("id", user_id);
    formData.append("password", password);
    formData.append("name", name);
    formData.append("code", code);
    formData.append("postion", postion);
    formData.append("postion_name", postion_name);
    formData.append("department", department);
    formData.append("department_name", department_name);
    formData.append("role", role);
    formData.append("status", status);
    $.ajax({
        type: 'POST',
        url: "/User/UpdateUser",
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
                $("#ModalUpdateUser").modal("hide");
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


function js_sentNoti() {
    $.ajax({
        type: "POST",
        url: "/Home/SentNoti", // Replace ControllerName with the actual name of your controller
        success: function (response) {
            if (response.status === true) {
                console.log("Notification sent successfully: " + response.message);
            } else {
                console.error("Error sending notification: " + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX error:", xhr, status, error);
        }
    });
}