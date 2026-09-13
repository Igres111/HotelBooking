(function () {
    var canvas = document.getElementById('statisticsChart');
    if (!canvas) {
        return;
    }

    var total = Number(canvas.dataset.total);
    var confirmed = Number(canvas.dataset.confirmed);
    var cancelled = Number(canvas.dataset.cancelled);

    new Chart(canvas, {
        type: 'bar',
        data: {
            labels: ['Total', 'Confirmed', 'Cancelled'],
            datasets: [{
                label: 'Bookings',
                data: [total, confirmed, cancelled],
                backgroundColor: ['#0d6efd', '#198754', '#dc3545']
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            },
            plugins: {
                legend: { display: false }
            }
        }
    });
})();
