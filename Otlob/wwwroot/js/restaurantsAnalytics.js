// Restaurants Analytics Dashboard JavaScript

document.addEventListener('DOMContentLoaded', function () {
    initCharts();
    initFilters();
    initSearch();
});

// Chart instances
let trendChart = null;
let salesDistributionChart = null;
let ordersComparisonChart = null;

// Color palette for charts
const chartColors = [
    '#4F46E5', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6',
    '#06B6D4', '#EC4899', '#14B8A6', '#F97316', '#6366F1'
];

function initCharts() {
    initTrendChart();
    initSalesDistributionChart();
    initOrdersComparisonChart();
}

function initTrendChart() {
    const canvas = document.getElementById('trendChart');
    if (!canvas) return;

    const data = JSON.parse(canvas.dataset.chartData || '[]');
    if (!data.length) return;

    const ctx = canvas.getContext('2d');

    trendChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.month),
            datasets: [
                {
                    label: 'Total Sales',
                    data: data.map(d => d.sales),
                    borderColor: '#4F46E5',
                    backgroundColor: 'rgba(79, 70, 229, 0.1)',
                    fill: true,
                    tension: 0.4,
                    yAxisID: 'y'
                },
                {
                    label: 'Total Orders',
                    data: data.map(d => d.orders),
                    borderColor: '#10B981',
                    backgroundColor: 'rgba(16, 185, 129, 0.1)',
                    fill: false,
                    tension: 0.4,
                    yAxisID: 'y1'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        usePointStyle: true,
                        padding: 20
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) label += ': ';
                            if (context.datasetIndex === 0) {
                                label += context.parsed.y.toLocaleString() + ' L.E';
                            } else {
                                label += context.parsed.y.toLocaleString() + ' orders';
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: { display: false }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Sales (L.E)'
                    },
                    ticks: {
                        callback: value => value.toLocaleString()
                    }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Orders'
                    },
                    grid: { drawOnChartArea: false }
                }
            }
        }
    });
}

function initSalesDistributionChart() {
    const canvas = document.getElementById('salesDistributionChart');
    if (!canvas) return;

    const data = JSON.parse(canvas.dataset.chartData || '[]');
    if (!data.length) return;

    const ctx = canvas.getContext('2d');

    salesDistributionChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: data.map(d => d.name),
            datasets: [{
                data: data.map(d => d.sales),
                backgroundColor: chartColors,
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        usePointStyle: true,
                        padding: 15,
                        font: { size: 11 },
                        generateLabels: function (chart) {
                            const datasets = chart.data.datasets;
                            return chart.data.labels.map((label, i) => ({
                                text: label.length > 15 ? label.substring(0, 15) + '...' : label,
                                fillStyle: datasets[0].backgroundColor[i],
                                hidden: false,
                                index: i
                            }));
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const value = context.parsed;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return `${context.label}: ${value.toLocaleString()} L.E (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
}

function initOrdersComparisonChart() {
    const canvas = document.getElementById('ordersComparisonChart');
    if (!canvas) return;

    const data = JSON.parse(canvas.dataset.chartData || '[]');
    if (!data.length) return;

    const ctx = canvas.getContext('2d');

    ordersComparisonChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(d => d.name.length > 12 ? d.name.substring(0, 12) + '...' : d.name),
            datasets: [{
                label: 'Completed Orders',
                data: data.map(d => d.orders),
                backgroundColor: chartColors,
                borderRadius: 8,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            indexAxis: 'y',
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        title: function (context) {
                            return data[context[0].dataIndex].name;
                        },
                        label: function (context) {
                            return `Orders: ${context.parsed.x.toLocaleString()}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: { display: false },
                    ticks: {
                        callback: value => value.toLocaleString()
                    }
                },
                y: {
                    grid: { display: false }
                }
            }
        }
    });
}

function initFilters() {
    const applyBtn = document.getElementById('applyFilters');
    if (!applyBtn) return;

    applyBtn.addEventListener('click', async function () {
        const year = document.getElementById('yearFilter').value;
        const month = document.getElementById('monthFilter').value;

        this.disabled = true;
        this.innerHTML = '<i class="bi bi-hourglass-split"></i> Loading...';

        try {
            let url = '';
            if (month === '0') {
                url = `/SuperAdmin/RestaurantsAnalytics/GetAnalyticsByYear?year=${year}`;
            } else {
                url = `/SuperAdmin/RestaurantsAnalytics/GetAnalyticsByMonth?year=${year}&month=${month}`;
            }

            const response = await fetch(url);
            const data = await response.json();

            updateDashboard(data);
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            this.disabled = false;
            this.innerHTML = '<i class="bi bi-funnel"></i> Apply';
        }
    });
}

function updateDashboard(data) {
    // Update stats
    updateElement('totalRestaurants', data.totalRestaurants);
    updateElement('totalOrders', formatNumber(data.totalCompletedOrders));
    updateElement('totalSales', formatNumber(data.totalSales) + ' <small>L.E</small>');
    updateElement('totalRevenue', formatNumber(data.totalRevenue) + ' <small>L.E</small>');
    updateElement('cancelledOrders', formatNumber(data.totalCancelledOrders));

    // Update period stats
    updateElement('monthOrders', formatNumber(data.currentMonthOrders));
    updateElement('monthSales', formatNumber(data.currentMonthSales) + ' L.E');
    updateElement('yearOrders', formatNumber(data.currentYearOrders));
    updateElement('yearSales', formatNumber(data.currentYearSales) + ' L.E');

    // Update charts
    updateTrendChart(data.monthlyTrends);
    updateTopLists(data.topRestaurantsBySales, data.topRestaurantsByOrdersCount);
    updateSalesDistributionChart(data.topRestaurantsBySales);
    updateOrdersComparisonChart(data.topRestaurantsByOrdersCount);
    updateTable(data.allRestaurantsAnalytics);
}

function updateElement(id, value) {
    const el = document.getElementById(id);
    if (el) el.innerHTML = value;
}

function formatNumber(num) {
    return num ? num.toLocaleString() : '0';
}

function updateTrendChart(trends) {
    if (!trendChart || !trends) return;

    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    trendChart.data.labels = trends.map(t => `${monthNames[t.month - 1]} ${t.year}`);
    trendChart.data.datasets[0].data = trends.map(t => t.totalSales);
    trendChart.data.datasets[1].data = trends.map(t => t.totalOrders);
    trendChart.update();
}

function updateTopLists(topSales, topOrders) {
    const salesList = document.getElementById('topSalesList');
    const ordersList = document.getElementById('topOrdersList');

    if (salesList) {
        salesList.innerHTML = generateTopListHTML(topSales, 'sales');
    }

    if (ordersList) {
        ordersList.innerHTML = generateTopListHTML(topOrders, 'orders');
    }
}

function generateTopListHTML(data, type) {
    if (!data || !data.length) {
        return `
            <div class="no-data">
                <i class="bi bi-shop"></i>
                <p>No restaurants data available</p>
            </div>
        `;
    }

    return data.map((r, index) => {
        const rank = index + 1;
        const rankClass = rank <= 3 ? `rank-${rank}` : '';
        const avatarContent = r.restaurantImage
            ? `<img src="/images/${r.restaurantImage}" alt="${r.restaurantName}" />`
            : '<i class="bi bi-shop"></i>';

        const valueSection = type === 'sales'
            ? `<div class="restaurant-sales">
                <span class="sales-amount">${r.totalOrdersSales.toLocaleString()}</span>
                <span class="sales-currency">L.E</span>
               </div>`
            : `<div class="restaurant-orders">
                <span class="orders-count">${r.completedOrdersCount.toLocaleString()}</span>
                <span class="orders-label">orders</span>
               </div>`;

        const statContent = type === 'sales'
            ? `<i class="bi bi-bag-check"></i> ${r.completedOrdersCount} orders`
            : `<i class="bi bi-currency-dollar"></i> ${r.totalOrdersSales.toLocaleString()} L.E`;

        return `
            <div class="restaurant-item">
                <div class="rank-badge ${rankClass}">${rank}</div>
                <div class="restaurant-avatar">${avatarContent}</div>
                <div class="restaurant-info">
                    <h4 class="restaurant-name">${r.restaurantName}</h4>
                    <div class="restaurant-stats">
                        <span class="stat-item">${statContent}</span>
                    </div>
                </div>
                ${valueSection}
            </div>
        `;
    }).join('');
}

function updateSalesDistributionChart(data) {
    if (!salesDistributionChart || !data) return;

    salesDistributionChart.data.labels = data.map(d => d.restaurantName);
    salesDistributionChart.data.datasets[0].data = data.map(d => d.totalOrdersSales);
    salesDistributionChart.update();
}

function updateOrdersComparisonChart(data) {
    if (!ordersComparisonChart || !data) return;

    ordersComparisonChart.data.labels = data.map(d =>
        d.restaurantName.length > 12 ? d.restaurantName.substring(0, 12) + '...' : d.restaurantName
    );
    ordersComparisonChart.data.datasets[0].data = data.map(d => d.completedOrdersCount);
    ordersComparisonChart.update();
}

function updateTable(data) {
    const tbody = document.querySelector('#restaurantsTable tbody');
    if (!tbody) return;

    if (!data || !data.length) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" class="text-center">
                    <div class="no-data">
                        <i class="bi bi-inbox"></i>
                        <p>No data available</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = data.map((r, index) => {
        const avatarContent = r.restaurantImage
            ? `<img src="/images/${r.restaurantImage}" alt="${r.restaurantName}" />`
            : '<i class="bi bi-shop"></i>';

        return `
            <tr>
                <td>${index + 1}</td>
                <td>
                    <div class="table-restaurant">
                        <div class="table-restaurant-avatar">${avatarContent}</div>
                        <span>${r.restaurantName}</span>
                    </div>
                </td>
                <td><span class="badge badge-success">${r.completedOrdersCount}</span></td>
                <td><span class="badge badge-danger">${r.cancelledOrdersCount}</span></td>
                <td><strong>${r.totalOrdersSales.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</strong> L.E</td>
                <td><strong>${r.totalOrdersRevenue.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</strong> L.E</td>
            </tr>
        `;
    }).join('');
}

function initSearch() {
    const searchInput = document.getElementById('searchRestaurant');
    if (!searchInput) return;

    searchInput.addEventListener('input', function () {
        const searchTerm = this.value.toLowerCase();
        const rows = document.querySelectorAll('#restaurantsTable tbody tr');

        rows.forEach(row => {
            const restaurantName = row.querySelector('.table-restaurant span');
            if (restaurantName) {
                const name = restaurantName.textContent.toLowerCase();
                row.style.display = name.includes(searchTerm) ? '' : 'none';
            }
        });
    });
}
