document.addEventListener('DOMContentLoaded', function() {
    const body = document.querySelector('body'),
          sidebar = body.querySelector('.adm-sidebar'),
          toggle = body.querySelector('.toggle'),
          modeSwitch = body.querySelector('.adm-mode'),
          modeText = body.querySelector('.adm-mode-text'),
          dropdowns = document.querySelectorAll('.adm-dropdown-toggle');

    // --- State Persistence Logic ---

    // 1. Restore Dark Mode
    if (localStorage.getItem('admTheme') === 'dark') {
        body.classList.add('adm-dark-mode');
        if (modeText) modeText.innerText = "Light Mode";
    }

    // 2. Restore Sidebar State (Collapsed/Expanded)
    if (localStorage.getItem('admSidebarClosed') === 'true') {
        if (sidebar) sidebar.classList.add('close');
    }

    // 3. Restore Open Dropdowns
    const openDropdownIndexes = JSON.parse(localStorage.getItem('admOpenDropdowns') || '[]');
    dropdowns.forEach((dropdown, index) => {
        if (openDropdownIndexes.includes(index)) {
            const dropdownContainer = dropdown.closest('.adm-dropdown');
            if (dropdownContainer) dropdownContainer.classList.add('open');
        }
    });

    // --- Function to Save Dropdowns State ---
    function saveDropdownState() {
        const openIndexes = [];
        document.querySelectorAll('.adm-dropdown-toggle').forEach((btn, index) => {
            const container = btn.closest('.adm-dropdown');
            if (container && container.classList.contains('open')) {
                openIndexes.push(index);
            }
        });
        localStorage.setItem('admOpenDropdowns', JSON.stringify(openIndexes));
    }


    // --- Event Listeners ---

    // Sidebar Toggle
    if (toggle) {
        toggle.addEventListener('click', () => {
            sidebar.classList.toggle('close');
            // Save state
            localStorage.setItem('admSidebarClosed', sidebar.classList.contains('close'));
        });
    }

    // Dark Mode Toggle
    if (modeSwitch) {
        modeSwitch.addEventListener('click', () => {
            body.classList.toggle('adm-dark-mode');
            
            if (body.classList.contains('adm-dark-mode')) {
                if (modeText) modeText.innerText = "Light Mode";
                localStorage.setItem('admTheme', 'dark');
            } else {
                if (modeText) modeText.innerText = "Dark Mode";
                localStorage.setItem('admTheme', 'light');
            }
        });
    }

    // Dropdown Toggle
    dropdowns.forEach((dropdown) => {
        dropdown.addEventListener('click', function(e) {
            e.preventDefault(); // Prevent default anchor behavior
            
            // If sidebar is closed, open it to show dropdown
            if (sidebar.classList.contains('close')) {
                sidebar.classList.remove('close');
                localStorage.setItem('admSidebarClosed', 'false');
            }
            
            const dropdownContainer = this.closest('.adm-dropdown');
            dropdownContainer.classList.toggle('open');
            
            // Save state after toggle
            saveDropdownState();
        });
    });
});
