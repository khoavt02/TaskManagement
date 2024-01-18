$(function () {
    $('#menu-dashboardss').addClass("active");
    $.ajax({
        url: '/Home/GetChartData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data.totalTasksByStatus);
            data.totalTasksByStatus.forEach(function (task) {
                switch (task.status) {
                    case 'COMPLETE':
                        $("#total-task-complete").html(task.totalTasks);
                        $("#percent-task-complete").html(task.percentTask + "%");
                        $('#progress-task-complete').css('width', task.percentTask + "%");
                        break;
                    case 'PROCESSING':
                        $("#total-task-processing").html(task.totalTasks);
                        $("#percent-task-processing").html(task.percentTask + "%");
                        $('#progress-task-processing').css('width', task.percentTask + "%");
                        break;
                    case 'EVALUATE':
                        $("#total-task-evaluated").html(task.totalTasks);
                        $("#percent-task-evaluated").html(task.percentTask + "%");
                        $('#progress-task-evaluated').css('width', task.percentTask + "%");
                        break;
                    case 'NEW':
                        $("#total-task-new").html(task.totalTasks);
                        $("#percent-task-new").html(task.percentTask + "%");
                        $('#progress-task-new').css('width', task.percentTask + "%");
                        break;
                    default:
                        break;
                }
            })
            //var chart = new Chart(document.getElementById("chartjs-dashboard-bar-devices"), {
            //    type: "bar",
            //    data: {
            //        labels: data.labels,
            //        datasets: data.datasets
            //    },
            //    options: {
            //        maintainAspectRatio: false,
            //        legend: {
            //            display: false
            //        },
            //        scales: {
            //            yAxes: [{
            //                gridLines: {
            //                    display: false
            //                },
            //                stacked: false,
            //                ticks: {
            //                    stepSize: 20
            //                }
            //            }],
            //            xAxes: [{
            //                barPercentage: .75,
            //                categoryPercentage: .5,
            //                stacked: false,
            //                gridLines: {
            //                    color: "transparent"
            //                }
            //            }]
            //        }
            //    }
            //});

            new Chart(document.getElementById("chartjs-dashboard-bar-devices"), {
                type: "bar",
                data: {
                    labels: data.labels,
                    datasets: data.datasets
                },
                options: {
                    maintainAspectRatio: false,
                    legend: {
                        display: false
                    },
                    scales: {
                        yAxes: [{
                            gridLines: {
                                display: false
                            },
                            stacked: false,
                            ticks: {
                                stepSize: 20
                            }
                        }],
                        xAxes: [{
                            barPercentage: .75,
                            categoryPercentage: .5,
                            stacked: false,
                            gridLines: {
                                color: "transparent"
                            }
                        }]
                    }
                }
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
});


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