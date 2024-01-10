$(function () {
    $('#menu-dashboard').addClass("active");
	new Chart(document.getElementById("chartjs-dashboard-bar-devices"), {
		type: "bar",
		data: {
			labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
			datasets: [{
				label: "Mobile",
				backgroundColor: window.theme.primary,
				borderColor: window.theme.primary,
				hoverBackgroundColor: window.theme.primary,
				hoverBorderColor: window.theme.primary,
				data: [54, 67, 41, 55, 62, 45, 55, 73, 60, 76, 48, 79]
			}, {
				label: "Desktop",
				backgroundColor: "#E8EAED",
				borderColor: "#E8EAED",
				hoverBackgroundColor: "#E8EAED",
				hoverBorderColor: "#E8EAED",
				data: [69, 66, 24, 48, 52, 51, 44, 53, 62, 79, 51, 68]
			}]
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