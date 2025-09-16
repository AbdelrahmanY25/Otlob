document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('sidebarToggle');
    const body = document.body;

    // Create overlay for mobile/tablet
    const overlay = document.createElement('div');
    overlay.className = 'sidebar-overlay';
    overlay.id = 'sidebarOverlay';
    document.body.appendChild(overlay);

    // Check if user is on mobile/tablet or desktop
    function isMobile() {
        return window.innerWidth <= 991;
    }

    // Initialize sidebar state based on screen size and saved preference
    function initializeSidebar() {
        const savedState = localStorage.getItem('sidebarOpen');
        const shouldBeOpen = savedState === 'true';

        if (isMobile()) {
            // On mobile/tablet, sidebar is always hidden by default
            sidebar.classList.remove('show', 'hidden');
            body.classList.remove('sidebar-open', 'sidebar-closed');
            overlay.classList.remove('show');
        } else {
            // On desktop, respect saved state
            if (shouldBeOpen) {
                sidebar.classList.remove('hidden');
                sidebar.classList.add('show');
                body.classList.add('sidebar-open');
                body.classList.remove('sidebar-closed');
            } else {
                sidebar.classList.add('hidden');
                sidebar.classList.remove('show');
                body.classList.add('sidebar-closed');
                body.classList.remove('sidebar-open');
            }
        }
    }

    // Toggle sidebar function
    function toggleSidebar() {
        if (isMobile()) {
            // Mobile behavior
            sidebar.classList.toggle('show');
            overlay.classList.toggle('show');
            // Don't save state on mobile
        } else {
            // Desktop behavior
            const isHidden = sidebar.classList.contains('hidden');

            if (isHidden) {
                sidebar.classList.remove('hidden');
                sidebar.classList.add('show');
                body.classList.add('sidebar-open');
                body.classList.remove('sidebar-closed');
                localStorage.setItem('sidebarOpen', 'true');
            } else {
                sidebar.classList.add('hidden');
                sidebar.classList.remove('show');
                body.classList.add('sidebar-closed');
                body.classList.remove('sidebar-open');
                localStorage.setItem('sidebarOpen', 'false');
            }
        }
    }

    // Close sidebar when clicking overlay (mobile/tablet)
    overlay.addEventListener('click', function () {
        if (isMobile()) {
            sidebar.classList.remove('show');
            overlay.classList.remove('show');
        }
    });

    // Toggle button event listener
    if (toggleBtn) {
        toggleBtn.addEventListener('click', toggleSidebar);
    }

    // Handle window resize
    window.addEventListener('resize', function () {
        // Reinitialize sidebar on window resize
        setTimeout(initializeSidebar, 100);
    });

    // Menu item click handlers
    document.querySelectorAll('.menu-item').forEach(item => {
        item.addEventListener('click', function () {
            // Remove active class from all items
            document.querySelectorAll('.menu-item').forEach(el => el.classList.remove('active'));

            // Add active class to clicked item
            this.classList.add('active');

            // Save active link
            const linkText = this.querySelector('a, span') ?
                (this.querySelector('a') ? this.querySelector('a').innerText.trim() : this.querySelector('span').innerText.trim()) :
                this.innerText.trim();
            localStorage.setItem('activeLink', linkText);

            // Close sidebar on mobile after clicking a link
            if (isMobile()) {
                setTimeout(() => {
                    sidebar.classList.remove('show');
                    overlay.classList.remove('show');
                }, 150);
            }
        });
    });

    // Restore active link
    const savedLink = localStorage.getItem('activeLink');
    if (savedLink) {
        document.querySelectorAll('.menu-item').forEach(item => {
            const linkText = item.querySelector('a, span') ?
                (item.querySelector('a') ? item.querySelector('a').innerText.trim() : item.querySelector('span').innerText.trim()) :
                item.innerText.trim();
            if (linkText === savedLink) {
                item.classList.add('active');
            }
        });
    }

    // Initialize sidebar state
    initializeSidebar();

    // Handle escape key to close sidebar on mobile
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && isMobile() && sidebar.classList.contains('show')) {
            sidebar.classList.remove('show');
            overlay.classList.remove('show');
        }
    });
});