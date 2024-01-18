$(document).ready(function () {
    $('#menu-job-statistic').addClass("active");
    $('#menu-job-task-statistic').addClass("active");
    $('.select2').select2();
    $('.select2').select2();
    $('#datetimepicker-date-1').datetimepicker({
        format: 'L'
    });
    $('#datetimepicker-date-2').datetimepicker({
        format: 'L'
    });
    js_GetList();
});


function js_GetList() {
   // department_s = $('#deparment_f').val();
    //priority_level_s = $('#priority_level_f').val();
    //status_s = $('#status_f').val();
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    //name_s = $('#name_f').val();
    var objTable = $("#table-projects");
    objTable.bootstrapTable('destroy');
    objTable.bootstrapTable({
        method: 'Get',
        url: '/Task/GetListTaskStatistic',
        queryParams: function (p) {
            var param = $.extend(true, {
                //keyword: $('#SearchAcademicLevel').val(),
                offset: p.offset,
                limit: p.limit,
                //name: name_s,
                from_date: from_date,
                to_date: to_date,
                //status: status_s,
                //priority_level: priority_level_s,
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
                field: "project",
                title: "Dự án",
                align: 'left',
                valign: 'left',
            },
            {
                field: "department",
                title: "Thuộc phòng ban",
                align: 'left',
                valign: 'left',
            },
            {
                field: "taskCount",
                title: "Tổng công việc",
                align: 'left',
                valign: 'left',
            },
            {
                field: "importantPriorityTask",
                title: "Mức quan trọng",
                align: 'left',
                valign: 'left',
            },
            {
                field: "highPriorityTask",
                title: "Mức cao",
                align: 'left',
                valign: 'left',
            },
            {
                field: "mediumPriorityTask",
                title: "Mức bình thường",
                align: 'left',
                valign: 'left',
            },
            {
                field: "lowPriorityTask",
                title: "Mức thấp",
                align: 'left',
                valign: 'left',
            },
            {
                field: "newTask",
                title: "Công việc mới",
                align: 'left',
                valign: 'left',
            },
            {
                field: "processTask",
                title: "Đang thực hiên",
                align: 'left',
                valign: 'left',
            },
            {
                field: "completeTask",
                title: "Đã hoàn thành",
                align: 'left',
                valign: 'left',
            },
            {
                field: "evaluateTask",
                title: "Đã đánh giá",
                align: 'left',
                valign: 'left',
            },
            {
                field: "totalPoint",
                title: "Tổng điểm",
                align: 'left',
                valign: 'left',

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
   
    to_date = $('#to-date').val();
    from_date = $('#from-date').val();
    $.ajax({
        url: '/Task/ExcelTaskStatistic',
        type: 'GET',
        data: {
            from_date: from_date,
            to_date: to_date,
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
                link.download = 'Báo cáo_thống_kê_công_việc.xlsx';
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
