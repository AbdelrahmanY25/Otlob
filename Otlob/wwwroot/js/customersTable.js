/**
 * Customers Table Management
 * Handles search, sort, filter, and AJAX toggle operations
 */

(function () {
    'use strict';

    // Configuration
    const config = {
        searchDelay: 300,
        animationDuration: 300,
        itemsPerPage: 10
    };

    // State
    let state = {
        customers: [],
        filteredCustomers: [],
        currentPage: 1,
        sortField: 'userName',
        sortDirection: 'asc',
        searchQuery: '',
        statusFilter: 'all'
    };

    // Cache DOM elements
    const elements = {
        tableCard: null,
        tableBody: null,
        searchInput: null,
        sortDropdown: null,
        filterButtons: null,
        pagination: null,
        showingInfo: null,
        totalCount: null,
        activeCount: null,
        blockedCount: null
    };

    // Initialize on DOM ready
    document.addEventListener('DOMContentLoaded', init);

    function init() {
        cacheElements();
        loadCustomersFromTable();
        setupEventListeners();
        updateStats();
    }

    function cacheElements() {
        elements.tableCard = document.querySelector('.table-card');
        elements.tableBody = document.querySelector('.customers-table tbody');
        elements.searchInput = document.querySelector('#searchInput');
        elements.sortDropdown = document.querySelector('.sort-dropdown');
        elements.filterButtons = document.querySelectorAll('.filter-btn[data-filter]');
        elements.pagination = document.querySelector('.pagination');
        elements.showingInfo = document.querySelector('.showing-info');
        elements.totalCount = document.querySelector('.stat-badge.total .count');
        elements.activeCount = document.querySelector('.stat-badge.active .count');
        elements.blockedCount = document.querySelector('.stat-badge.blocked .count');
    }

    function loadCustomersFromTable() {
        const rows = document.querySelectorAll('.customers-table tbody tr[data-customer-id]');
        state.customers = [];

        rows.forEach(row => {
            state.customers.push({
                id: row.dataset.customerId,
                userName: row.dataset.userName || '',
                email: row.dataset.email || '',
                phone: row.dataset.phone || '',
                image: row.dataset.image || '',
                lockoutEnabled: row.dataset.lockoutEnabled === 'true',
                emailConfirmed: row.dataset.emailConfirmed === 'true',
                element: row
            });
        });

        state.filteredCustomers = [...state.customers];
        applySorting();
    }

    function setupEventListeners() {
        // Search input with debounce
        if (elements.searchInput) {
            let searchTimeout;
            elements.searchInput.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    state.searchQuery = e.target.value.toLowerCase().trim();
                    state.currentPage = 1;
                    applyFilters();
                }, config.searchDelay);
            });

            // Clear search on escape
            elements.searchInput.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    e.target.value = '';
                    state.searchQuery = '';
                    state.currentPage = 1;
                    applyFilters();
                }
            });
        }

        // Sort dropdown
        if (elements.sortDropdown) {
            const sortBtn = elements.sortDropdown.querySelector('.filter-btn');
            const sortItems = elements.sortDropdown.querySelectorAll('.dropdown-item');

            if (sortBtn) {
                sortBtn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    elements.sortDropdown.classList.toggle('open');
                });
            }

            sortItems.forEach(item => {
                item.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const field = item.dataset.sort;
                    
                    if (state.sortField === field) {
                        state.sortDirection = state.sortDirection === 'asc' ? 'desc' : 'asc';
                    } else {
                        state.sortField = field;
                        state.sortDirection = 'asc';
                    }
                    
                    updateSortUI(item);
                    applySorting();
                    renderTable();
                    elements.sortDropdown.classList.remove('open');
                });
            });

            // Close dropdown on outside click
            document.addEventListener('click', (e) => {
                if (!elements.sortDropdown.contains(e.target)) {
                    elements.sortDropdown.classList.remove('open');
                }
            });
        }

        // Filter buttons
        elements.filterButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                elements.filterButtons.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                state.statusFilter = btn.dataset.filter;
                state.currentPage = 1;
                applyFilters();
            });
        });

        // Table header sorting
        document.querySelectorAll('.customers-table th.sortable').forEach(th => {
            th.addEventListener('click', () => {
                const field = th.dataset.sort;
                if (state.sortField === field) {
                    state.sortDirection = state.sortDirection === 'asc' ? 'desc' : 'asc';
                } else {
                    state.sortField = field;
                    state.sortDirection = 'asc';
                }
                updateHeaderSortUI();
                applySorting();
                renderTable();
            });
        });

        // Action buttons delegation
        elements.tableBody?.addEventListener('click', handleActionClick);
    }

    function handleActionClick(e) {
        const btn = e.target.closest('.action-btn');
        if (!btn) return;

        const row = btn.closest('tr');
        const customerId = row?.dataset.customerId;

        if (!customerId) return;

        if (btn.classList.contains('toggle-block')) {
            toggleBlockStatus(customerId, btn, row);
        } else if (btn.classList.contains('toggle-email')) {
            toggleEmailConfirmation(customerId, btn, row);
        }
    }

    function toggleBlockStatus(userId, btn, row) {
        const isCurrentlyBlocked = btn.classList.contains('blocked');
        const action = isCurrentlyBlocked ? 'unblock' : 'block';

        // Confirm action
        if (!confirm(`Are you sure you want to ${action} this user?`)) {
            return;
        }

        setLoading(true);
        btn.disabled = true;

        $.ajax({
            url: '/SuperAdmin/Users/ToggleUserBlockStatus',
            type: 'POST',
            data: { id: userId },
            success: function () {
                // Update UI
                const newBlockedState = !isCurrentlyBlocked;
                btn.classList.toggle('blocked', newBlockedState);
                btn.setAttribute('data-tooltip', newBlockedState ? 'Unblock User' : 'Block User');
                btn.innerHTML = `<i class="fas fa-${newBlockedState ? 'unlock' : 'ban'}"></i>`;

                // Update status badge in row
                const statusCell = row.querySelector('.status-badge.active, .status-badge.blocked');
                if (statusCell) {
                    statusCell.className = `status-badge ${newBlockedState ? 'blocked' : 'active'}`;
                    statusCell.innerHTML = `<i class="fas fa-circle"></i> ${newBlockedState ? 'Blocked' : 'Active'}`;
                }

                // Update data attribute
                row.dataset.lockoutEnabled = newBlockedState.toString();

                // Update customer state
                const customer = state.customers.find(c => c.id === userId);
                if (customer) {
                    customer.lockoutEnabled = newBlockedState;
                }

                // Update stats
                updateStats();

                // Show success notification
                showToast('success', `User ${newBlockedState ? 'blocked' : 'unblocked'} successfully`);
            },
            error: function (xhr) {
                showToast('error', 'Failed to update user status. Please try again.');
                console.error('Toggle block status failed:', xhr);
            },
            complete: function () {
                setLoading(false);
                btn.disabled = false;
            }
        });
    }

    function toggleEmailConfirmation(userId, btn, row) {
        const isCurrentlyVerified = btn.classList.contains('verified');
        const action = isCurrentlyVerified ? 'unverify' : 'verify';

        if (!confirm(`Are you sure you want to ${action} this user's email?`)) {
            return;
        }

        setLoading(true);
        btn.disabled = true;

        $.ajax({
            url: '/SuperAdmin/Users/ToggleConfirmEmail',
            type: 'POST',
            data: { id: userId },
            success: function () {
                const newVerifiedState = !isCurrentlyVerified;
                btn.classList.toggle('verified', newVerifiedState);
                btn.setAttribute('data-tooltip', newVerifiedState ? 'Mark as Unverified' : 'Verify Email');
                btn.innerHTML = `<i class="fas fa-${newVerifiedState ? 'check-circle' : 'envelope'}"></i>`;

                // Update email status badge
                const emailBadge = row.querySelector('.status-badge.verified, .status-badge.unverified');
                if (emailBadge) {
                    emailBadge.className = `status-badge ${newVerifiedState ? 'verified' : 'unverified'}`;
                    emailBadge.innerHTML = `<i class="fas fa-${newVerifiedState ? 'check' : 'clock'}"></i> ${newVerifiedState ? 'Verified' : 'Unverified'}`;
                }

                // Update data attribute
                row.dataset.emailConfirmed = newVerifiedState.toString();

                // Update customer state
                const customer = state.customers.find(c => c.id === userId);
                if (customer) {
                    customer.emailConfirmed = newVerifiedState;
                }

                showToast('success', `Email ${newVerifiedState ? 'verified' : 'unverified'} successfully`);
            },
            error: function (xhr) {
                showToast('error', 'Failed to update email status. Please try again.');
                console.error('Toggle email confirmation failed:', xhr);
            },
            complete: function () {
                setLoading(false);
                btn.disabled = false;
            }
        });
    }

    function applyFilters() {
        state.filteredCustomers = state.customers.filter(customer => {
            // Search filter
            const matchesSearch = !state.searchQuery ||
                customer.userName.toLowerCase().includes(state.searchQuery) ||
                customer.email.toLowerCase().includes(state.searchQuery) ||
                customer.phone.toLowerCase().includes(state.searchQuery);

            // Status filter
            let matchesStatus = true;
            if (state.statusFilter === 'active') {
                matchesStatus = !customer.lockoutEnabled;
            } else if (state.statusFilter === 'blocked') {
                matchesStatus = customer.lockoutEnabled;
            }

            return matchesSearch && matchesStatus;
        });

        applySorting();
        renderTable();
        updatePagination();
    }

    function applySorting() {
        state.filteredCustomers.sort((a, b) => {
            let valueA = a[state.sortField] || '';
            let valueB = b[state.sortField] || '';

            if (typeof valueA === 'string') {
                valueA = valueA.toLowerCase();
                valueB = valueB.toLowerCase();
            }

            if (valueA < valueB) return state.sortDirection === 'asc' ? -1 : 1;
            if (valueA > valueB) return state.sortDirection === 'asc' ? 1 : -1;
            return 0;
        });
    }

    function renderTable() {
        if (!elements.tableBody) return;

        const startIndex = (state.currentPage - 1) * config.itemsPerPage;
        const endIndex = startIndex + config.itemsPerPage;
        const pageCustomers = state.filteredCustomers.slice(startIndex, endIndex);

        // Hide all rows first
        state.customers.forEach(customer => {
            if (customer.element) {
                customer.element.style.display = 'none';
            }
        });

        // Show only the relevant rows in order
        pageCustomers.forEach((customer, index) => {
            if (customer.element) {
                customer.element.style.display = '';
                customer.element.style.animationDelay = `${index * 50}ms`;
                elements.tableBody.appendChild(customer.element);
            }
        });

        // Show empty state if no results
        showEmptyState(state.filteredCustomers.length === 0);
        updateShowingInfo();
    }

    function showEmptyState(show) {
        let emptyState = elements.tableCard?.querySelector('.empty-state');

        if (show) {
            if (!emptyState) {
                emptyState = document.createElement('div');
                emptyState.className = 'empty-state';
                emptyState.innerHTML = `
                    <div class="empty-icon">
                        <i class="fas fa-users-slash"></i>
                    </div>
                    <h3>No customers found</h3>
                    <p>Try adjusting your search or filter criteria</p>
                `;
                elements.tableBody?.parentElement?.after(emptyState);
            }
            emptyState.style.display = 'block';
            elements.tableBody.parentElement.style.display = 'none';
        } else {
            if (emptyState) {
                emptyState.style.display = 'none';
            }
            if (elements.tableBody?.parentElement) {
                elements.tableBody.parentElement.style.display = '';
            }
        }
    }

    function updateStats() {
        const total = state.customers.length;
        const blocked = state.customers.filter(c => c.lockoutEnabled).length;
        const active = total - blocked;

        if (elements.totalCount) elements.totalCount.textContent = total;
        if (elements.activeCount) elements.activeCount.textContent = active;
        if (elements.blockedCount) elements.blockedCount.textContent = blocked;
    }

    function updateShowingInfo() {
        if (!elements.showingInfo) return;

        const total = state.filteredCustomers.length;
        const startIndex = (state.currentPage - 1) * config.itemsPerPage + 1;
        const endIndex = Math.min(state.currentPage * config.itemsPerPage, total);

        if (total === 0) {
            elements.showingInfo.innerHTML = 'No results';
        } else {
            elements.showingInfo.innerHTML = `Showing <strong>${startIndex}</strong> to <strong>${endIndex}</strong> of <strong>${total}</strong> customers`;
        }
    }

    function updatePagination() {
        if (!elements.pagination) return;

        const totalPages = Math.ceil(state.filteredCustomers.length / config.itemsPerPage);

        let html = `
            <button class="pagination-btn" data-page="prev" ${state.currentPage === 1 ? 'disabled' : ''}>
                <i class="fas fa-chevron-left"></i>
            </button>
        `;

        // Generate page numbers
        const maxVisiblePages = 5;
        let startPage = Math.max(1, state.currentPage - Math.floor(maxVisiblePages / 2));
        let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

        if (endPage - startPage + 1 < maxVisiblePages) {
            startPage = Math.max(1, endPage - maxVisiblePages + 1);
        }

        if (startPage > 1) {
            html += `<button class="pagination-btn" data-page="1">1</button>`;
            if (startPage > 2) {
                html += `<span class="pagination-ellipsis">...</span>`;
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            html += `<button class="pagination-btn ${i === state.currentPage ? 'active' : ''}" data-page="${i}">${i}</button>`;
        }

        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                html += `<span class="pagination-ellipsis">...</span>`;
            }
            html += `<button class="pagination-btn" data-page="${totalPages}">${totalPages}</button>`;
        }

        html += `
            <button class="pagination-btn" data-page="next" ${state.currentPage === totalPages || totalPages === 0 ? 'disabled' : ''}>
                <i class="fas fa-chevron-right"></i>
            </button>
        `;

        elements.pagination.innerHTML = html;

        // Add event listeners to pagination buttons
        elements.pagination.querySelectorAll('.pagination-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const page = btn.dataset.page;
                if (page === 'prev') {
                    state.currentPage = Math.max(1, state.currentPage - 1);
                } else if (page === 'next') {
                    state.currentPage = Math.min(totalPages, state.currentPage + 1);
                } else {
                    state.currentPage = parseInt(page);
                }
                renderTable();
                updatePagination();

                // Scroll to top of table
                elements.tableCard?.scrollIntoView({ behavior: 'smooth', block: 'start' });
            });
        });
    }

    function updateSortUI(activeItem) {
        const sortItems = elements.sortDropdown?.querySelectorAll('.dropdown-item');
        sortItems?.forEach(item => {
            item.classList.remove('active');
            const icon = item.querySelector('i:last-child');
            if (icon) icon.className = 'fas fa-sort';
        });

        activeItem.classList.add('active');
        const icon = activeItem.querySelector('i:last-child');
        if (icon) {
            icon.className = `fas fa-sort-${state.sortDirection === 'asc' ? 'up' : 'down'}`;
        }

        // Update button text
        const sortBtn = elements.sortDropdown?.querySelector('.filter-btn span');
        if (sortBtn) {
            sortBtn.textContent = activeItem.textContent.trim();
        }
    }

    function updateHeaderSortUI() {
        document.querySelectorAll('.customers-table th.sortable').forEach(th => {
            th.classList.remove('sorted', 'asc', 'desc');
            if (th.dataset.sort === state.sortField) {
                th.classList.add('sorted', state.sortDirection);
            }
        });
    }

    function setLoading(loading) {
        elements.tableCard?.classList.toggle('loading', loading);
    }

    function showToast(type, message) {
        if (typeof toastr !== 'undefined') {
            toastr.options = {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-top-right',
                timeOut: 3000
            };
            toastr[type](message);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }

    // Expose functions for external use if needed
    window.CustomersTable = {
        refresh: function () {
            loadCustomersFromTable();
            applyFilters();
        },
        getState: function () {
            return { ...state };
        }
    };

})();
