// Order Status Progress Animation
document.addEventListener('DOMContentLoaded', function () {
    initStatusProgress();
});

function initStatusProgress() {
    const progressContainer = document.querySelector('.progress-steps');
    if (!progressContainer) return;

    const status = progressContainer.dataset.status;
    const steps = document.querySelectorAll('.progress-step');
    const progressLineFill = document.querySelector('.progress-line-fill');

    if (!steps.length) return;

    const statusOrder = ['Pending', 'Preparing', 'Shipped', 'Delivered'];
    const currentIndex = statusOrder.indexOf(status);

    // Handle cancelled status
    if (status === 'Cancelled') {
        steps.forEach(step => {
            step.classList.add('cancelled');
        });
        if (progressLineFill) {
            progressLineFill.style.width = '100%';
            progressLineFill.style.background = '#ef4444';
        }
        return;
    }

    // Calculate progress percentage
    let progressPercent = 0;
    if (currentIndex >= 0) {
        progressPercent = (currentIndex / (statusOrder.length - 1)) * 100;
    }

    // Animate progress line
    if (progressLineFill) {
        setTimeout(() => {
            progressLineFill.style.width = `calc(${progressPercent}% - 30px)`;
        }, 300);
    }

    // Mark steps as completed or active
    steps.forEach((step, index) => {
        const stepStatus = step.dataset.step;
        const stepIndex = statusOrder.indexOf(stepStatus);

        setTimeout(() => {
            if (stepIndex < currentIndex) {
                step.classList.add('completed');
            } else if (stepIndex === currentIndex) {
                step.classList.add('active');
            }
        }, 100 * (index + 1));
    });
}

// Format date helper
function formatOrderDate(dateString) {
    const date = new Date(dateString);
    const options = {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };
    return date.toLocaleDateString('en-US', options);
}

// Parse meal details JSON
function parseMealDetails(mealDetailsJson) {
    if (!mealDetailsJson) return '';

    try {
        const data = JSON.parse(mealDetailsJson);
        const items = [];

        if (data.items && Array.isArray(data.items)) {
            data.items.forEach(item => {
                if (item.Name) items.push(item.Name);
            });
        }

        if (data.addOns && Array.isArray(data.addOns)) {
            data.addOns.forEach(addon => {
                if (addon.Name) items.push(addon.Name);
            });
        }

        return items.join(', ');
    } catch {
        return '';
    }
}
