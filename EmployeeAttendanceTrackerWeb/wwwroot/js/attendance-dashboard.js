document.addEventListener('DOMContentLoaded', function () {

    // Today's Attendance Doughnut Chart
    const todayChartCanvas = document.getElementById('todayAttendanceChart');

    if (todayChartCanvas) {

        // Read dashboard data from the canvas attributes.
        const labels = JSON.parse(todayChartCanvas.dataset.labels);
        const values = JSON.parse(todayChartCanvas.dataset.values);

        new Chart(todayChartCanvas, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: [
                        '#00d25b',
                        '#fc424a'
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: {
                    position: 'bottom',
                    labels: {
                        fontColor: '#9c9fa6',
                        usePointStyle: true,
                        padding: 20
                    }
                },
                cutoutPercentage: 70
            }
        });
    }

    // Weekly Attendance Stacked Bar Chart
    const weeklyChartCanvas = document.getElementById('weeklyAttendanceChart');

    if (weeklyChartCanvas) {

        // Read weekly attendance data from the canvas attributes.
        const labels = JSON.parse(weeklyChartCanvas.dataset.labels);
        const presentData = JSON.parse(weeklyChartCanvas.dataset.present);
        const absentData = JSON.parse(weeklyChartCanvas.dataset.absent);

        new Chart(weeklyChartCanvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Present',
                        data: presentData,
                        backgroundColor: '#00d25b',
                        borderRadius: 4,
                        barThickness: 18
                    },
                    {
                        label: 'Absent',
                        data: absentData,
                        backgroundColor: '#fc424a',
                        borderRadius: 4,
                        barThickness: 18
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        stacked: true,
                        gridLines: {
                            display: false
                        },
                        ticks: {
                            fontColor: '#9c9fa6'
                        }
                    }],
                    yAxes: [{
                        stacked: true,
                        ticks: {
                            beginAtZero: true,
                            fontColor: '#9c9fa6',
                            precision: 0
                        },
                        gridLines: {
                            color: 'rgba(255,255,255,0.08)'
                        }
                    }]
                },
                legend: {
                    labels: {
                        fontColor: '#9c9fa6',
                        usePointStyle: true
                    }
                }
            }
        });
    }   
});
