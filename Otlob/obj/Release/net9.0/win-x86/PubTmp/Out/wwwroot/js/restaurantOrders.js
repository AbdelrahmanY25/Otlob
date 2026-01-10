// Restaurant Orders Management

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

        // Set message
        messageEl.textContent = options.message || 'Are you sure you want to proceed?';

        // Show/hide status change
        if (options.currentStatus && options.newStatus) {
            statusChangeEl.style.display = 'flex';
            currentStatusEl.textContent = options.currentStatus;
            newStatusEl.textContent = '? ' + options.newStatus;
        } else {
            statusChangeEl.style.display = 'none';
        }

        // Show/hide warning
        if (options.warning) {
            warningEl.style.display = 'flex';
            warningTextEl.textContent = options.warning;
        } else {
            warningEl.style.display = 'none';
        }

        // Set button style
        confirmBtn.className = 'confirm-btn confirm';
        if (options.isDanger) {
            confirmBtn.classList.add('danger');
        }

        // Store callback
        confirmCallback = options.onConfirm;

        // Show modal
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

    // Close modal on backdrop click
    const confirmModal = document.getElementById('confirmModal');
    if (confirmModal) {
        confirmModal.addEventListener('click', function (e) {
            if (e.target === confirmModal) {
                closeConfirmModal();
            }
        });
    }

    // Update order status with confirmation modal
    window.updateOrderStatus = async function (orderId, newStatus, button) {
        const card = button.closest('.order-card');
        const currentStatus = card.dataset.status;

        const statusMessages = {
            'Preparing': { action: 'start preparing', label: 'Start Preparing' },
            'Shipped': { action: 'mark as shipped', label: 'Mark as Shipped' },
            'Delivered': { action: 'mark as delivered', label: 'Mark as Delivered' }
        };

        const statusInfo = statusMessages[newStatus] || { action: 'change status', label: newStatus };

        showConfirmModal({
            message: `Are you sure you want to ${statusInfo.action} this order?`,
            currentStatus: currentStatus,
            newStatus: newStatus,
            isDanger: false,
            onConfirm: async function () {
                const originalContent = button.innerHTML;
                button.classList.add('btn-loading');
                button.innerHTML = '<div class="spinner"></div>';

                try {
                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                    
                    const response = await fetch('/RestaurantAdmin/RestaurantOrders/UpdateStatus', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': token
                        },
                        body: `orderKey=${encodeURIComponent(orderId)}&newStatus=${encodeURIComponent(newStatus)}`
                    });

                    const result = await response.json();

                    if (result.success) {
                        showToast(result.message, 'success');
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showToast(result.message || 'Failed to update order status', 'error');
                        button.classList.remove('btn-loading');
                        button.innerHTML = originalContent;
                    }
                } catch (error) {
                    console.error('Error updating order status:', error);
                    showToast('An error occurred. Please try again.', 'error');
                    button.classList.remove('btn-loading');
                    button.innerHTML = originalContent;
                }
            }
        });
    };

    // Cancel order with confirmation modal
    window.cancelOrder = async function (orderId, button) {
        const card = button.closest('.order-card');
        const currentStatus = card.dataset.status;

        showConfirmModal({
            message: 'Are you sure you want to cancel this order?',
            currentStatus: currentStatus,
            newStatus: 'Cancelled',
            warning: 'This action cannot be undone. The order will be permanently cancelled.',
            isDanger: true,
            onConfirm: async function () {
                const originalContent = button.innerHTML;
                button.classList.add('btn-loading');
                button.innerHTML = '<div class="spinner"></div>';

                try {
                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                    
                    const response = await fetch('/RestaurantAdmin/RestaurantOrders/UpdateStatus', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': token
                        },
                        body: `orderKey=${encodeURIComponent(orderId)}&newStatus=Cancelled`
                    });

                    const result = await response.json();

                    if (result.success) {
                        showToast('Order cancelled successfully', 'success');
                        setTimeout(() => window.location.reload(), 1000);
                    } else {
                        showToast(result.message || 'Failed to cancel order', 'error');
                        button.classList.remove('btn-loading');
                        button.innerHTML = originalContent;
                    }
                } catch (error) {
                    console.error('Error cancelling order:', error);
                    showToast('An error occurred. Please try again.', 'error');
                    button.classList.remove('btn-loading');
                    button.innerHTML = originalContent;
                }
            }
        });
    };

    // Show user info modal
    window.showUserInfo = async function (orderId) {
        const modal = document.getElementById('userInfoModal');
        const content = document.getElementById('userInfoContent');
        
        content.innerHTML = '<div style="text-align: center; padding: 2rem;"><div class="spinner" style="width: 30px; height: 30px; margin: 0 auto;"></div><p style="margin-top: 1rem; color: var(--adm-text-secondary);">Loading...</p></div>';
        modal.classList.add('show');

        try {
            const response = await fetch(`/RestaurantAdmin/RestaurantOrders/GetUserInfo?orderId=${orderId}`);
            
            if (response.ok) {
                const html = await response.text();
                content.innerHTML = html;
            } else {
                content.innerHTML = '<div style="text-align: center; padding: 2rem; color: var(--adm-text-secondary);"><i class="bi bi-exclamation-circle" style="font-size: 2rem;"></i><p>Failed to load user info</p></div>';
            }
        } catch (error) {
            console.error('Error fetching user info:', error);
            content.innerHTML = '<div style="text-align: center; padding: 2rem; color: var(--adm-text-secondary);"><i class="bi bi-exclamation-circle" style="font-size: 2rem;"></i><p>An error occurred</p></div>';
        }
    };

    // Close user info modal
    window.closeUserInfoModal = function () {
        const modal = document.getElementById('userInfoModal');
        modal.classList.remove('show');
    };

    // Close modal on backdrop click
    const userInfoModal = document.getElementById('userInfoModal');
    if (userInfoModal) {
        userInfoModal.addEventListener('click', function (e) {
            if (e.target === userInfoModal) {
                closeUserInfoModal();
            }
        });
    }

    // Search functionality - works with integer IDs
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', applyFilters);
    }

    // Status filter
    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) {
        statusFilter.addEventListener('change', applyFilters);
    }

    // Payment filter
    const paymentFilter = document.getElementById('paymentFilter');
    if (paymentFilter) {
        paymentFilter.addEventListener('change', applyFilters);
    }

    // Price range filter
    const priceRange = document.getElementById('priceRange');
    const priceValue = document.getElementById('priceValue');
    if (priceRange && priceValue) {
        priceRange.addEventListener('input', function () {
            priceValue.textContent = parseFloat(this.value).toLocaleString() + ' L.E';
            applyFilters();
        });
    }

    // Reset filters
    const resetFilters = document.getElementById('resetFilters');
    if (resetFilters) {
        resetFilters.addEventListener('click', function () {
            if (searchInput) searchInput.value = '';
            if (statusFilter) statusFilter.value = '';
            if (paymentFilter) paymentFilter.value = '';
            if (priceRange) {
                priceRange.value = priceRange.max;
                priceValue.textContent = parseFloat(priceRange.max).toLocaleString() + ' L.E';
            }
            applyFilters();
        });
    }

    // Apply all filters
    function applyFilters() {
        const searchTerm = searchInput ? searchInput.value.trim() : '';
        const statusValue = statusFilter ? statusFilter.value : '';
        const paymentValue = paymentFilter ? paymentFilter.value : '';
        const maxPrice = priceRange ? parseFloat(priceRange.value) : Infinity;

        const orderCards = document.querySelectorAll('.order-card');
        let visibleCount = 0;

        orderCards.forEach(card => {
            const orderId = card.dataset.orderId;
            const status = card.dataset.status;
            const payment = card.dataset.payment;
            const price = parseFloat(card.dataset.price);

            const matchesSearch = !searchTerm || orderId.toString().includes(searchTerm);
            const matchesStatus = !statusValue || status === statusValue;
            const matchesPayment = !paymentValue || payment === paymentValue;
            const matchesPrice = price <= maxPrice;

            if (matchesSearch && matchesStatus && matchesPayment && matchesPrice) {
                card.style.display = '';
                visibleCount++;
            } else {
                card.style.display = 'none';
            }
        });

        // Show/hide empty state for filtered results
        const ordersGrid = document.getElementById('ordersGrid');
        let filterEmptyState = document.querySelector('.filter-empty-state');

        if (visibleCount === 0 && ordersGrid) {
            if (!filterEmptyState) {
                filterEmptyState = document.createElement('div');
                filterEmptyState.className = 'empty-state filter-empty-state';
                filterEmptyState.innerHTML = `
                    <i class="bi bi-funnel"></i>
                    <h3>No matching orders</h3>
                    <p>Try adjusting your filters to see more results</p>
                `;
                ordersGrid.parentNode.insertBefore(filterEmptyState, ordersGrid.nextSibling);
            }
            filterEmptyState.style.display = '';
            ordersGrid.style.display = 'none';
        } else {
            if (filterEmptyState) {
                filterEmptyState.style.display = 'none';
            }
            if (ordersGrid) {
                ordersGrid.style.display = '';
            }
        }
    }

    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = document.querySelectorAll('[title]');
        tooltipTriggerList.forEach(el => {
            el.setAttribute('data-bs-toggle', 'tooltip');
            new bootstrap.Tooltip(el);
        });
    }
});
