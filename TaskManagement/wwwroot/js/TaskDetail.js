$(function () {
    $('.select2').select2();
    js_GetList();
});

function js_GetList() {
    var id = $('#task_id').val();
    var objTable = $("#data-table");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Task/GetListProccessOfTask',
        queryParams: function (p) {
            var param = $.extend(true, {
                id:id,
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

            //{
            //    field: "createdDate",
            //    title: "Thời gian",
            //    align: 'left',
            //    valign: 'left',

            //},
            {
                field: "processPercent",
                title: "Tiến độ",
                align: 'center',
                valign: 'center',

            },
            {
                field: "estimate",
                title: "Thời gian thực hiện",
                align: 'center',
                valign: 'center',

            },
            {
                field: "description",
                title: "Mô tả",
                align: 'left',
                valign: 'left',

            },
            {
                field: "fileAttach",
                title: "File",
                align: 'left',
                valign: 'left',

            },
            {
                field: "createdDate",
                title: "Thời gian tạo",
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

function js_AddTaskProccesss() {
    var id = $('#task_id').val();
    var project_id = $('#project_id').val();
    var proccess = $('#proccess_percent').val();
    var estimate = $('#proccess_estimate').val();
    var description = $('#proccess_description').val();
    var formData = new FormData();
    formData.append("id", id);
    formData.append("project_id", project_id);
    formData.append("proccess", proccess);
    formData.append("estimate", estimate);
    formData.append("description", description);
    $.ajax({
        type: 'POST',
        url: "/Task/AddProccessTask",
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
                $("#ModalAddProccess").modal("hide");
                js_GetList();
                window.location.reload();
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

function uploadFile() {
    var fileInput = document.getElementById('fileInput');
    var file = fileInput.files[0];

    if (file) {
        var formData = new FormData();
        formData.append('file', file);

        fetch('/Task/Upload', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.status) {
                    document.getElementById('message').innerText = data.message;
                } else {
                    document.getElementById('message').innerText = 'Upload failed: ' + data.message;
                }
            })
            .catch(error => {
                console.error('Error:', error);
                document.getElementById('message').innerText = 'An error occurred during the upload.';
            });
    } 
}