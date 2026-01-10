// Restaurant Admin Dashboard Charts & Interactions

document.addEventListener('DOMContentLoaded', function () {
    // Theme detection for Chart.js
    const isDarkMode = () => document.body.classList.contains('adm-dark-mode');

    // Chart colors
    const chartColors = {
        primary: '#4F46E5',
        success: '#10B981',
        warning: '#F59E0B',
        danger: '#EF4444',
        purple: '#8B5CF6',
        cyan: '#06B6D4',
        pink: '#EC4899',
        gray: '#6B7280'
    };

    // Get theme-aware colors
    function getTextColor() {
        return isDarkMode() ? '#F9FAFB' : '#1F2937';
    }

    function getGridColor() {
        return isDarkMode() ? '#374151' : '#E5E7EB';
    }

    // Common chart options
    const commonOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                labels: {
                    color: getTextColor(),
                    font: { family: "'Segoe UI', sans-serif", size: 12 }
                }
            },
            tooltip: {
                backgroundColor: isDarkMode() ? '#1F2937' : '#FFFFFF',
                titleColor: getTextColor(),
                bodyColor: getTextColor(),
                borderColor: isDarkMode() ? '#374151' : '#E5E7EB',
                borderWidth: 1,
                padding: 12,
                cornerRadius: 8
            }
        }
    };

    // --- 1. Orders Status Pie Chart ---
    const pieCtx = document.getElementById('ordersStatusChart');
    let pieChart = null;

    if (pieCtx) {
        const pieData = JSON.parse(pieCtx.dataset.chartData || '{}');

        pieChart = new Chart(pieCtx, {
            type: 'doughnut',
            data: {
                labels: ['Pending', 'Preparing', 'Shipping', 'Delivered', 'Cancelled'],
                datasets: [{
                    data: [
                        pieData.pending || 0,
                        pieData.preparing || 0,
                        pieData.shipping || 0,
                        pieData.delivered || 0,
                        pieData.cancelled || 0
                    ],
                    backgroundColor: [
                        chartColors.warning,
                        chartColors.cyan,
                        chartColors.primary,
                        chartColors.success,
                        chartColors.danger
                    ],
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '65%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: getTextColor(),
                            padding: 15,
                            usePointStyle: true,
                            pointStyle: 'circle'
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const value = context.parsed;
                                const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                                return `${context.label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    }

    // --- 2. Monthly Sales Curve Chart ---
    const salesCtx = document.getElementById('monthlySalesChart');
    let salesChart = null;

    if (salesCtx) {
        const salesData = JSON.parse(salesCtx.dataset.chartData || '[]');

        salesChart = new Chart(salesCtx, {
            type: 'line',
            data: {
                labels: salesData.map(d => d.month),
                datasets: [{
                    label: 'Sales (L.E)',
                    data: salesData.map(d => d.sales),
                    borderColor: chartColors.primary,
                    backgroundColor: 'rgba(79, 70, 229, 0.1)',
                    tension: 0.4,
                    fill: true,
                    pointBackgroundColor: chartColors.primary,
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    pointRadius: 5,
                    pointHoverRadius: 7
                }]
            },
            options: {
                ...commonOptions,
                scales: {
                    x: {
                        grid: { color: getGridColor(), drawBorder: false },
                        ticks: { color: getTextColor() }
                    },
                    y: {
                        grid: { color: getGridColor(), drawBorder: false },
                        ticks: {
                            color: getTextColor(),
                            callback: function (value) {
                                return value.toLocaleString() + ' L.E';
                            }
                        },
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // --- 3. Monthly Orders Bar Chart ---
    const ordersCtx = document.getElementById('monthlyOrdersChart');
    let ordersChart = null;

    if (ordersCtx) {
        const ordersData = JSON.parse(ordersCtx.dataset.chartData || '[]');

        ordersChart = new Chart(ordersCtx, {
            type: 'bar',
            data: {
                labels: ordersData.map(d => d.month),
                datasets: [{
                    label: 'Orders Count',
                    data: ordersData.map(d => d.count),
                    backgroundColor: chartColors.success,
                    borderRadius: 6,
                    maxBarThickness: 40
                }]
            },
            options: {
                ...commonOptions,
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { color: getTextColor() }
                    },
                    y: {
                        grid: { color: getGridColor(), drawBorder: false },
                        ticks: { color: getTextColor() },
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // --- 4. Revenue Comparison Chart ---
    const revenueCtx = document.getElementById('revenueComparisonChart');
    let revenueChart = null;

    if (revenueCtx) {
        const revenueData = JSON.parse(revenueCtx.dataset.chartData || '[]');

        revenueChart = new Chart(revenueCtx, {
            type: 'bar',
            data: {
                labels: revenueData.map(d => d.month),
                datasets: [
                    {
                        label: 'Total Sales',
                        data: revenueData.map(d => d.sales),
                        backgroundColor: chartColors.primary,
                        borderRadius: 6
                    },
                    {
                        label: 'Revenue',
                        data: revenueData.map(d => d.revenue),
                        backgroundColor: chartColors.success,
                        borderRadius: 6
                    }
                ]
            },
            options: {
                ...commonOptions,
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { color: getTextColor() }
                    },
                    y: {
                        grid: { color: getGridColor(), drawBorder: false },
                        ticks: {
                            color: getTextColor(),
                            callback: function (value) {
                                return value.toLocaleString() + ' L.E';
                            }
                        },
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // --- Date Picker for Daily Analytics ---
    const datePicker = document.getElementById('dailyDatePicker');
    if (datePicker) {
        datePicker.addEventListener('change', async function () {
            const selectedDate = this.value;
            if (!selectedDate) return;

            try {
                const response = await fetch(`/RestaurantAdmin/Home/GetDailyAnalytics?date=${selectedDate}`);

                if (response.ok) {
                    const data = await response.json();
                    updateDailyStats(data);
                } else if (response.status === 404) {
                    showNoDataMessage();
                }
            } catch (error) {
                console.error('Error fetching daily analytics:', error);
            }
        });
    }

    function updateDailyStats(data) {
        const ordersEl = document.getElementById('dailyOrders');
        const salesEl = document.getElementById('dailySales');
        const revenueEl = document.getElementById('dailyRevenue');
        const avgEl = document.getElementById('dailyAverage');

        if (ordersEl) ordersEl.textContent = data.completedOrdersCount || 0;
        if (salesEl) salesEl.textContent = (data.totalOrdersSales || 0).toLocaleString() + ' L.E';
        if (revenueEl) revenueEl.textContent = (data.totalOrdersRevenue || 0).toLocaleString() + ' L.E';
        if (avgEl) avgEl.textContent = (data.averageOrderPrice || 0).toLocaleString() + ' L.E';

        // Update pie chart
        if (pieChart) {
            pieChart.data.datasets[0].data = [
                data.pendingOrders || 0,
                data.preparingOrders || 0,
                data.shippingOrders || 0,
                data.deliveredOrders || 0,
                data.cancelledOrders || 0
            ];
            pieChart.update();
        }
    }

    function showNoDataMessage() {
        const ordersEl = document.getElementById('dailyOrders');
        const salesEl = document.getElementById('dailySales');
        const revenueEl = document.getElementById('dailyRevenue');
        const avgEl = document.getElementById('dailyAverage');

        if (ordersEl) ordersEl.textContent = '0';
        if (salesEl) salesEl.textContent = '0 L.E';
        if (revenueEl) revenueEl.textContent = '0 L.E';
        if (avgEl) avgEl.textContent = '0 L.E';
    }

    // --- Dark Mode Observer ---
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.attributeName === 'class') {
                // Update all charts when theme changes
                [pieChart, salesChart, ordersChart, revenueChart].forEach(chart => {
                    if (chart) {
                        if (chart.config.type === 'doughnut') {
                            chart.options.plugins.legend.labels.color = getTextColor();
                        } else {
                            chart.options.scales.x.ticks.color = getTextColor();
                            chart.options.scales.y.ticks.color = getTextColor();
                            chart.options.scales.x.grid.color = getGridColor();
                            chart.options.scales.y.grid.color = getGridColor();
                            chart.options.plugins.legend.labels.color = getTextColor();
                        }
                        chart.update();
                    }
                });
            }
        });
    });

    observer.observe(document.body, { attributes: true });
});
