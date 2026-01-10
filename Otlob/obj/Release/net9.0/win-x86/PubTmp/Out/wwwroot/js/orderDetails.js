// Order Details Page

document.addEventListener('DOMContentLoaded', function () {
    // Toast notification system
    const toastContainer = document.createElement('div');
    toastContainer.className = 'toast-container';
    document.body.appendChild(toastContainer);

    function showToast(message, type = 'success') {
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

    // Confirmation Modal Functions
    let confirmCallback = null;

    window.showConfirmModal = function (options) {
        const modal = document.getElementById('confirmModal');
        const messageEl = document.getElementById('confirmMessage');
        const statusChangeEl = document.getElementById('confirmStatusChange');
        const currentStatusEl = document.getElementById('currentStatus');
        const newStatusEl = document.getElementById('newStatus');
        const warningEl = document.getElementById('confirmWarning');
        const warningTextEl = document.getElementById('confirmWarningText');
        const confirmBtn = document.getElementById('confirmButton');

        messageEl.textContent = options.message || 'Are you sure you want to proceed?';

        if (options.currentStatus && options.newStatus) {
            statusChangeEl.style.display = 'flex';
            currentStatusEl.textContent = options.currentStatus;
            newStatusEl.textContent = '? ' + options.newStatus;
        } else {
            statusChangeEl.style.display = 'none';
        }

        if (options.warning) {
            warningEl.style.display = 'flex';
            warningTextEl.textContent = options.warning;
        } else {
            warningEl.style.display = 'none';
        }

        confirmBtn.className = 'confirm-btn confirm';
        if (options.isDanger) {
            confirmBtn.classList.add('danger');
        }

        confirmCallback = options.onConfirm;
        modal.classList.add('show');
    };

    window.closeConfirmModal = function () {
        const modal = document.getElementById('confirmModal');
        modal.classList.remove('show');
        confirmCallback = null;
    };

    window.confirmAction = function () {
        if (confirmCallback) {
            confirmCallback();
        }
        closeConfirmModal();
    };

    const confirmModal = document.getElementById('confirmModal');
    if (confirmModal) {
        confirmModal.addEventListener('click', function (e) {
            if (e.target === confirmModal) {
                closeConfirmModal();
            }
        });
    }

    // Update order status from details page with confirmation modal
    window.updateOrderStatusFromDetails = async function (orderKey, newStatus, button) {
        const statusMessages = {
            'Preparing': { action: 'start preparing', label: 'Start Preparing' },
            'Shipped': { action: 'mark as shipped', label: 'Mark as Shipped' },
            'Delivered': { action: 'mark as delivered', label: 'Mark as Delivered' },
            'Cancelled': { action: 'cancel', label: 'Cancel' }
        };

        const statusInfo = statusMessages[newStatus] || { action: 'change status', label: newStatus };
        const currentStatus = document.querySelector('.status-badge').textContent.trim();

        showConfirmModal({
            message: `Are you sure you want to ${statusInfo.action} this order?`,
            currentStatus: currentStatus,
            newStatus: newStatus,
            warning: newStatus === 'Cancelled' ? 'This action cannot be undone. The order will be permanently cancelled.' : null,
            isDanger: newStatus === 'Cancelled',
            onConfirm: async function () {
                const originalContent = button.innerHTML;
                button.disabled = true;
                button.innerHTML = '<div class="spinner"></div> Processing...';

                try {
                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                    
                    const response = await fetch('/RestaurantAdmin/RestaurantOrders/UpdateStatus', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': token
                        },
                        body: `orderKey=${encodeURIComponent(orderKey)}&newStatus=${encodeURIComponent(newStatus)}`
                    });

                    const result = await response.json();

                    if (result.success) {
                        showToast(result.message, 'success');
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showToast(result.message || 'Failed to update order status', 'error');
                        button.disabled = false;
                        button.innerHTML = originalContent;
                    }
                } catch (error) {
                    console.error('Error updating order status:', error);
                    showToast('An error occurred. Please try again.', 'error');
                    button.disabled = false;
                    button.innerHTML = originalContent;
                }
            }
        });
    };

    // Print order
    window.printOrder = function () {
        window.print();
    };

    // Copy order ID
    window.copyOrderId = function (orderId) {
        navigator.clipboard.writeText(orderId).then(() => {
            showToast('Order ID copied to clipboard', 'success');
        }).catch(() => {
            showToast('Failed to copy Order ID', 'error');
        });
    };
});
