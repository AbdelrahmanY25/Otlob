const statusColors = {
    Pending: '#f39c12',
    Preparing: '#3498db',
    Shipped: '#e74c3c',
    Delivered: '#2ecc71'
};

const statusGradients = {
    Pending: ['#f39c12', '#f1c40f'],
    Preparing: ['#3498db', '#2980b9'],
    Shipped: ['#e74c3c', '#c0392b'],
    Delivered: ['#2ecc71', '#27ae60']
};

let currentChart = null;
let currentChartType = 'doughnut';

function createGradient(ctx, color1, color2) {
    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, color1);
    gradient.addColorStop(1, color2);
    return gradient;
}

function initializeChart() {
    const ctx = document.getElementById("orderStatusChart").getContext("2d");
    const labels = Object.keys(orderStatusData);
    const values = Object.values(orderStatusData);

    const backgroundColors = labels.map(label => {
        const colors = statusGradients[label];
        return createGradient(ctx, colors[0], colors[1]);
    });

    const chartConfig = {
        type: currentChartType,
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: backgroundColors,
                borderColor: '#ffffff',
                borderWidth: 3,
                hoverOffset: currentChartType === 'bar' ? 0 : 15,
                spacing: currentChartType === 'bar' ? 0 : 2,
                hoverBorderWidth: 4,
                hoverBorderColor: '#ffffff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: currentChartType === 'doughnut' ? '60%' : 0,
            plugins: {
                legend: {
                    display: true,
                    position: window.innerWidth <= 480 ? 'bottom' : 'bottom',
                    labels: {
                        usePointStyle: true,
                        boxWidth: window.innerWidth <= 480 ? 12 : 15,
                        padding: window.innerWidth <= 480 ? 15 : 20,
                        font: {
                            size: window.innerWidth <= 480 ? 12 : window.innerWidth <= 768 ? 13 : 14,
                            family: 'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
                            weight: '500'
                        }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(255, 255, 255, 0.95)',
                    titleColor: '#2c3e50',
                    bodyColor: '#2c3e50',
                    borderColor: 'rgba(0, 0, 0, 0.1)',
                    borderWidth: 1,
                    cornerRadius: 10,
                    displayColors: true,
                    titleFont: {
                        size: window.innerWidth <= 480 ? 12 : 14,
                        weight: '600'
                    },
                    bodyFont: {
                        size: window.innerWidth <= 480 ? 11 : 13,
                        weight: '500'
                    },
                    padding: window.innerWidth <= 480 ? 8 : 12,
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.parsed}%`;
                        }
                    }
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true,
                duration: 1000,
                easing: 'easeOutQuart'
            },
            onHover: (event, elements) => {
                event.native.target.style.cursor = elements.length > 0 ? 'pointer' : 'default';
            },
            onClick: (event, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    const status = labels[index];
                    window.location.href = `SuperAdmin/OrdersStatus/Index?status=${status}`;
                }
            }
        }
    };

    if (currentChartType === 'bar') {
        chartConfig.options.scales = {
            y: {
                beginAtZero: true,
                grid: {
                    color: 'rgba(0, 0, 0, 0.1)'
                },
                ticks: {
                    font: {
                        size: window.innerWidth <= 480 ? 10 : window.innerWidth <= 768 ? 11 : 12,
                        family: 'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
                    }
                }
            },
            x: {
                grid: {
                    display: false
                },
                ticks: {
                    font: {
                        size: window.innerWidth <= 480 ? 10 : window.innerWidth <= 768 ? 11 : 12,
                        family: 'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif'
                    },
                    maxRotation: window.innerWidth <= 480 ? 45 : 0
                }
            }
        };
    }

    currentChart = new Chart(ctx, chartConfig);
}

function toggleChartType(type) {
    document.querySelectorAll('.control-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    event.target.classList.add('active');

    currentChartType = type;

    if (currentChart) {
        currentChart.destroy();
    }

    initializeChart();
}

document.addEventListener('DOMContentLoaded', function () {
    initializeChart();
});

/*========================================================== */

const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'EGP'
    }).format(amount);
}

function getMonthName(month) {
    return monthNames[month - 1];
}

function calculateStats(data) {
    const totalOrders = data.reduce((sum, item) => sum + item.totalOrders, 0);
    const totalRevenue = data.reduce((sum, item) => sum + item.totalRevenue, 0);
    const avgOrderValue = totalRevenue / totalOrders;
    const bestMonth = data.reduce((max, item) =>
        item.totalRevenue > max.totalRevenue ? item : max
    );

    document.getElementById('totalOrders').textContent = totalOrders.toLocaleString();
    document.getElementById('totalRevenue').textContent = formatCurrency(totalRevenue);
    document.getElementById('avgOrderValue').textContent = formatCurrency(avgOrderValue);
    document.getElementById('bestMonth').textContent = `${getMonthName(bestMonth.month)} ${bestMonth.year}`;
}

function createCharts(data) {
    const labels = data.map(item => `${getMonthName(item.month)} ${item.year}`);
    const ordersData = data.map(item => item.totalOrders);
    const revenueData = data.map(item => item.totalRevenue);

    const ordersCtx = document.getElementById('ordersChart').getContext('2d');
    new Chart(ordersCtx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Orders',
                data: ordersData,
                borderColor: '#3498db',
                backgroundColor: 'rgba(52, 152, 219, 0.1)',
                borderWidth: 3,
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 10
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });

    const revenueCtx = document.getElementById('revenueChart').getContext('2d');
    new Chart(revenueCtx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Revenue',
                data: revenueData,
                backgroundColor: '#2ecc71',
                borderColor: '#27ae60',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return formatCurrency(value);
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
}

function populateTable(data) {
    const tbody = document.getElementById('dataTableBody');
    tbody.innerHTML = '';

    const currentDate = new Date();
    const currentMonth = currentDate.getMonth() + 1;
    const currentYear = currentDate.getFullYear();

    data.forEach((item, index) => {
        const prevItem = data[index - 1];
        const growthRate = prevItem ?
            ((item.totalRevenue - prevItem.totalRevenue) / prevItem.totalRevenue * 100) : 0;

        const isCurrentMonth = item.month === currentMonth && item.year === currentYear;

        const row = document.createElement('tr');
        if (isCurrentMonth) {
            row.className = 'current-month';
        }

        row.innerHTML = `
                    <td>
                        <strong>${getMonthName(item.month)} ${item.year}</strong>
                        ${isCurrentMonth ? '<span class="current-month-badge">Current Month</span>' : ''}
                    </td>
                    <td>${item.totalOrders}</td>
                    <td>${formatCurrency(item.totalRevenue)}</td>
                    <td>${formatCurrency(item.totalRevenue / item.totalOrders)}</td>
                    <td style="color: ${growthRate >= 0 ? '#27ae60' : '#e74c3c'}">
                        ${growthRate >= 0 ? '+' : ''}${growthRate.toFixed(1)}%
                    </td>
                `;
        tbody.appendChild(row);
    });
}

calculateStats(sampleData);
createCharts(sampleData);
populateTable(sampleData);