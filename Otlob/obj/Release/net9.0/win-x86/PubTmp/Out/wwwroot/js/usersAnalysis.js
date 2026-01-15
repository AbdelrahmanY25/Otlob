// Users Analytics Dashboard JavaScript

document.addEventListener('DOMContentLoaded', () => {
    initActiveChart();
    initSearch();
});

function initActiveChart() {
    const canvas = document.getElementById('activeBarChart');
    if (!canvas) return;

    const raw = canvas.dataset.chart || '[]';
    let data = [];
    try { 
        data = JSON.parse(raw); 
    } catch { 
        data = []; 
    }
    if (!data.length) return;

    const labels = data.map(d => d.label);
    const values = data.map(d => d.value);

    new Chart(canvas.getContext('2d'), {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Users',
                data: values,
                backgroundColor: ['#10B981', '#EF4444'],
                borderRadius: 8,
                borderSkipped: false
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            plugins: { 
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    callbacks: {
                        label: function(context) {
                            return context.label + ': ' + context.parsed.x.toLocaleString() + ' users';
                        }
                    }
                }
            },
            scales: { 
                x: { 
                    ticks: { precision: 0 },
                    grid: { display: false }
                },
                y: {
                    grid: { display: false }
                }
            }
        }
    });
}

function initSearch() {
    const searchInput = document.getElementById('searchUser');
    if (!searchInput) return;

    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        const userItems = document.querySelectorAll('.user-item');

        userItems.forEach(item => {
            const username = item.getAttribute('data-username') || '';
            item.style.display = username.includes(searchTerm) ? '' : 'none';
        });
    });
}
