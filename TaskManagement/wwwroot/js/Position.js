$(function () {
    $('#menu-management').addClass("active");
    $('#menu-management-position').addClass("active");

    js_GetList();
});
function js_closeModalUpdate() {
    $('#ModalUpdatePosition').modal('hide');
}
function js_AddPosition() {
    var name = $('#name').val();
    var code = $('#code').val();
    var formData = new FormData();
    formData.append("name", name);
    formData.append("code", code);
    $.ajax({
        type: 'POST',
        url: "/Position/AddPosition",
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
                $("#ModalAddPosition").modal("hide");
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
        url: '/Position/GetListPosition',
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
                title: "Tên chức vụ",
                align: 'left',
                valign: 'left',

            },
            {
                field: "code",
                title: "Mã chức vụ",
                align: 'left',
                valign: 'left',
            },
            {
                field: "createdName",
                title: "Người tạo",
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
                            url: '/Position/GetDetailPositionId',
                            data: {
                                id: row.id
                            },
                            success: function (rp) {
                                if (rp.status) {
                                    $("#ModalUpdatePosition").modal("show");
                                    $('#name_u').val(row.name);
                                    $('#code_u').val(row.code);
                                    $('#position_id').val(row.id);
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

function js_UpdatePosition() {
    var name = $('#name_u').val();
    var code = $('#code_u').val();
    var id = $('#position_id').val();
    var formData = new FormData();
    formData.append("name", name);
    formData.append("code", code);
    formData.append("id", id);
    $.ajax({
        type: 'POST',
        url: "/Position/UpdatePostion",
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
                $("#ModalUpdatePosition").modal("hide");
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

