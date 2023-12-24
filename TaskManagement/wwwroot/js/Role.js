$(function () {
    $('#menu-management').addClass("active");
    $('#menu-management-role').addClass("active");
    $("#chk_All_New").click(function () {
        var cboxes = document.getElementsByName('check_add');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_New').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });

    $("#chk_All_Edit").click(function () {
        var cboxes = document.getElementsByName('check_edit');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_Edit').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });

    $("#chk_All_View").click(function () {
        var cboxes = document.getElementsByName('check_view');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_View').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });
    $("#chk_All_Delete").click(function () {
        var cboxes = document.getElementsByName('check_delete');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_Delete').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });

    $("#chk_All_Export").click(function () {
        var cboxes = document.getElementsByName('check_export');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_Export').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });

    $("#chk_All_Review").click(function () {
        var cboxes = document.getElementsByName('check_review');
        for (var i = 0; i < cboxes.length; i++) {
            if ($('#chk_All_Review').is(":checked")) {
                cboxes[i].checked = true;
            } else {
                cboxes[i].checked = false;
            }
        }
    });

    js_GetList();
});
function js_closeModalUpdate() {
    $('#ModalUpdateRole').modal('hide');
}
function js_AddRole() {
    var name = $('#role_name').val();
    var status = $('#status').val();
    var formData = new FormData();
    formData.append("name", name);
    formData.append("status", status);
    $.ajax({
        type: 'POST',
        url: "/Role/AddRoleGroup",
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
                $("#ModalAddRole").modal("hide");
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
    var objTable = $("#data-table");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Role/GetListRoleGroup',
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
                field: "name",
                title: "Nhóm quyền",
                align: 'left',
                valign: 'left',

            },
            {
                field: "status",
                title: "Trạng thái",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    var html = "";
                    if (row.status == true) {
                        html += "<span class= 'badge badge-pill badge-success'>Đang hoạt động</span>";
                    } else {
                        html += "<span class= 'badge badge-pill badge-danger'>Ngưng hoạt động</span>";
                    } 
                    return html;
                }

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
                field: "updatedName",
                title: "Người cập nhật",
                align: 'left',
                valign: 'left',

            },
            {
                field: "updatedDate",
                title: "Ngày cập nhật",
                align: 'left',
                valign: 'left',
                formatter: function (value, row, index) {
                    if (row.updatedDate != null) {
                        return moment(row.updatedDate).format('DD/MM/YYYY');
                    }
                    else {
                        return '';
                    }
                }

            },
            
            {
                title: "Thao tác",
                valign: 'middle',
                align: 'center',
                width: '100px',
                class: 'CssAction',
                formatter: function (value, row, index) {
                    var action = '<a class=" btn btn-primary btn-sm mr-1 btnEdit" title="Sửa"><i class="align-middle fas fa-fw fa-pencil-alt"></i></a>'
                    action += '<a class=" btn btn-success btn-sm" title="Chi tiết" href="/Role/RoleGroupDetail?id=' + row.id + '"><i class="align-middle fas fa-fw fa-file"></i></a>'
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
                            url: '/Role/GetDetailRoleGroupById',
                            data: {
                                id: row.id
                            },
                            success: function (rp) {
                                if (rp.status) {
                                    $("#ModalUpdateRole").modal("show");
                                    $('#role_name_u').val(row.name);
                                    var status = row.status == true ? 1 : 0;
                                    $('#status_u').val(status);
                                    $('#role_id').val(row.id);
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

function js_UpdateRole() {
    var name = $('#role_name_u').val();
    var status = $('#status_u').val();
    var id = $('#role_id').val();
    var formData = new FormData();
    formData.append("name", name);
    formData.append("status", status);
    formData.append("id", id);
    $.ajax({
        type: 'POST',
        url: "/Role/UpdateRoleGroup",
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
                $("#ModalUpdateRole").modal("hide");
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

function js_updateRoleDetail() {
    var modulesData = [];
    var rows = document.querySelectorAll('tbody tr');
    rows.forEach(function (row) {
        var moduleId = row.querySelector('[name^="module_"]').value;
        var moduleName = row.querySelector('[name^="moduleName_"]').value;
        var add = row.querySelector('[name="check_add"]').checked;
        var edit = row.querySelector('[name="check_edit"]').checked;
        var view = row.querySelector('[name="check_view"]').checked;
        var del = row.querySelector('[name="check_delete"]').checked;
        var exportModule = row.querySelector('[name="check_export"]').checked;
        var review = row.querySelector('[name="check_review"]').checked;

        modulesData.push({
            Id: moduleId,
            DisplayName: "",
            ModuleName: moduleName,
            Add: add,
            Delete: del,
            View: view,
            Edit: edit,
            Export: exportModule,
            Review: review,
            Comment: false
        });
    });
    var roleGroupId = $('#roleGroupId').val();
    var id = parseInt(roleGroupId);
    console.log(modulesData, roleGroupId, JSON.stringify(modulesData))
    var formData = new FormData();
    formData.append("modules", JSON.stringify(modulesData));
    formData.append("roleGroupId", roleGroupId);
    $.ajax({
        url: '/Role/UpdateRoleDetail', // Thay đổi đường dẫn tùy vào cấu trúc route của bạn
        type: 'POST',
        //contentType: 'application/json',
        //dataType: 'json',
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
        },
        error: function (error) {
            // Xử lý lỗi nếu có
            console.error(error);
            var message = error;
            var title = "";
            toastr["error"](message, title, {
                positionClass: 'toast-top-right',
                closeButton: true,
                progressBar: true,
                newestOnTop: true,
                timeOut: 3000
            });
        }
    });
}