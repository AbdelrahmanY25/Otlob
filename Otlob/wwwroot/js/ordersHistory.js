// Global functions for cancel modal (available to onclick handlers)
function showCancelModal(orderId) {
    const modal = document.getElementById('cancelModal');
    const orderIdInput = document.getElementById('cancelOrderId');
    const confirmBtn = document.getElementById('confirmCancelBtn');
    
    if (!modal) return;

    // Reset form
    if (orderIdInput) orderIdInput.value = orderId;
    
    document.querySelectorAll('input[name="cancelReason"]').forEach(radio => {
        radio.checked = false;
    });
    
    if (confirmBtn) confirmBtn.disabled = true;
    
    modal.classList.add('show');
}

function closeCancelModal() {
    const modal = document.getElementById('cancelModal');
    if (modal) {
        modal.classList.remove('show');
    }
}

async function confirmCancelOrder() {
    const orderId = document.getElementById('cancelOrderId')?.value;
    const selectedReason = document.querySelector('input[name="cancelReason"]:checked');
    
    if (!selectedReason) {
        showToast('Please select a cancellation reason', 'error');
        return;
    }

    if (!orderId) {
        showToast('Order ID not found', 'error');
        return;
    }

    const reason = selectedReason.value;
    const confirmBtn = document.getElementById('confirmCancelBtn');
    const originalContent = confirmBtn.innerHTML;
    
    confirmBtn.classList.add('btn-loading');
    confirmBtn.innerHTML = '<span class="spinner"></span> Processing...';
    confirmBtn.disabled = true;

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        if (!token) {
            throw new Error('Anti-forgery token not found');
        }

        const response = await fetch('/Customer/OrdersHistory/CancelOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: `orderId=${encodeURIComponent(orderId)}&reason=${encodeURIComponent(reason)}`
        });

        const result = await response.json();

        if (result.success) {
            closeCancelModal();
            showToast(result.message, 'success');
            setTimeout(() => window.location.reload(), 1500);
        } else {
            showToast(result.message || 'Failed to cancel order', 'error');
            confirmBtn.classList.remove('btn-loading');
            confirmBtn.innerHTML = originalContent;
            confirmBtn.disabled = false;
        }
    } catch (error) {
        console.error('Error cancelling order:', error);
        showToast('An error occurred. Please try again.', 'error');
        confirmBtn.classList.remove('btn-loading');
        confirmBtn.innerHTML = originalContent;
        confirmBtn.disabled = false;
    }
}

// Attach to window to be 100% sure for inline handlers
window.showCancelModal = showCancelModal;
window.closeCancelModal = closeCancelModal;
window.confirmCancelOrder = confirmCancelOrder;

// Attachment of event listeners for cancel buttons (using delegation)
document.addEventListener('click', function(event) {
    const cancelBtn = event.target.closest('.cancel-order-btn');
    if (cancelBtn && cancelBtn.hasAttribute('data-order-id')) {
        event.preventDefault();
        event.stopPropagation();
        const orderId = cancelBtn.getAttribute('data-order-id');
        showCancelModal(orderId);
    }
});

// Order Status Progress Animation
document.addEventListener('DOMContentLoaded', function () {
    initStatusProgress();
    initCancelModal();
    initToastSystem();
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

// Cancel Order Modal Functions
function initCancelModal() {
    const cancelReasonRadios = document.querySelectorAll('input[name="cancelReason"]');
    const confirmBtn = document.getElementById('confirmCancelBtn');
    
    if (!cancelReasonRadios.length) return;

    cancelReasonRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            if (confirmBtn) {
                confirmBtn.disabled = false;
            }
        });
    });

    // Close modal on backdrop click
    const cancelModal = document.getElementById('cancelModal');
    if (cancelModal) {
        cancelModal.addEventListener('click', function (e) {
            if (e.target === cancelModal) {
                closeCancelModal();
            }
        });
    }

    // Close modal with Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            if (cancelModal && cancelModal.classList.contains('show')) {
                closeCancelModal();
            }
        }
    });
}

// Toast Notification System
function initToastSystem() {
    if (!document.querySelector('.toast-container')) {
        const toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }
}

function showToast(message, type = 'success') {
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) return;

    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `
        <i class="bi ${type === 'success' ? 'bi-check-circle' : 'bi-exclamation-circle'}"></i>
        <span>${message}</span>
    `;
    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideIn 0.3s ease reverse';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
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
