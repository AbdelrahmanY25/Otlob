/**
 * menuForCustomer.js - Universal Food App Menu Interactions
 * Handles scroll spy, category navigation, collapse/expand, and scroll effects
 */

document.addEventListener('DOMContentLoaded', function () {
    initScrollSpy();
    initCollapse();
    initScrollEffects();
    initOfferCarouselDrag();
});

/**
 * Initialize scroll spy for category navigation
 */
function initScrollSpy() {
    const filterButtons = document.querySelectorAll('.category-btn');
    const categorySections = document.querySelectorAll('.category-section');
    const offersSection = document.getElementById('offers-section');
    const navBar = document.querySelector('.category-filter-bar');

    // Combine offers section with category sections for scroll detection
    const allSections = offersSection
        ? [offersSection, ...categorySections]
        : [...categorySections];

    // Handle Button Click - Smooth Scroll to Section
    filterButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const targetId = btn.getAttribute('data-target');
            const targetElement = document.getElementById(targetId);

            if (targetElement) {
                // Remove active from all buttons first
                filterButtons.forEach(b => b.classList.remove('active'));
                btn.classList.add('active');

                // Calculate offset accounting for sticky header
                const offset = navBar.getBoundingClientRect().height + 20;
                const elementTop = targetElement.getBoundingClientRect().top + window.scrollY;

                window.scrollTo({
                    top: elementTop - offset,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Handle Scroll Spy - Highlight active category on scroll
    let isScrolling = false;
    let scrollTimeout;

    window.addEventListener('scroll', () => {
        if (!isScrolling) {
            window.requestAnimationFrame(() => {
                updateActiveButton(allSections, filterButtons, navBar);
                isScrolling = false;
            });
            isScrolling = true;
        }

        // Debounce for performance
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(() => {
            updateActiveButton(allSections, filterButtons, navBar);
        }, 100);
    });

    // Initial active state check
    updateActiveButton(allSections, filterButtons, navBar);
}

/**
 * Update the active state of category buttons based on scroll position
 */
function updateActiveButton(sections, buttons, navBar) {
    let currentId = '';
    const offset = navBar.getBoundingClientRect().height + 60;

    sections.forEach(section => {
        const sectionTop = section.offsetTop;
        const sectionHeight = section.offsetHeight;

        // Check if we're within this section
        if (window.scrollY >= sectionTop - offset &&
            window.scrollY < sectionTop + sectionHeight - offset) {
            currentId = section.getAttribute('id');
        }
    });

    // Edge case: If at the very bottom, highlight the last section
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100) {
        if (sections.length > 0) {
            currentId = sections[sections.length - 1].getAttribute('id');
        }
    }

    // Update button states
    buttons.forEach(btn => {
        const targetId = btn.getAttribute('data-target');

        if (targetId === currentId) {
            if (!btn.classList.contains('active')) {
                btn.classList.add('active');
                scrollToActiveFilter(btn, navBar);
            }
        } else {
            btn.classList.remove('active');
        }
    });
}

/**
 * Horizontally scroll the filter bar to keep active button visible
 */
function scrollToActiveFilter(activeBtn, navBar) {
    const wrapper = navBar.querySelector('.filter-wrapper');
    if (!wrapper) return;

    const wrapperRect = wrapper.getBoundingClientRect();
    const btnRect = activeBtn.getBoundingClientRect();

    // Calculate relative position
    const btnLeftRelative = btnRect.left - wrapperRect.left;
    const btnRightRelative = btnLeftRelative + btnRect.width;

    // Check if button is out of view and center it
    if (btnLeftRelative < 0 || btnRightRelative > wrapperRect.width) {
        const scrollLeft = activeBtn.offsetLeft - (wrapper.clientWidth / 2) + (activeBtn.clientWidth / 2);
        wrapper.scrollTo({
            left: scrollLeft,
            behavior: 'smooth'
        });
    }
}

/**
 * Initialize collapse/expand functionality for category sections
 */
function initCollapse() {
    const headers = document.querySelectorAll('.category-header');

    headers.forEach(header => {
        header.addEventListener('click', (e) => {
            // Prevent event bubbling issues
            e.stopPropagation();

            const section = header.parentElement;
            const grid = section.querySelector('.meals-grid');
            const icon = header.querySelector('.collapse-btn i');

            if (!grid) return;

            // Toggle visibility with animation
            if (grid.style.display === 'none') {
                grid.style.display = 'grid';
                grid.style.opacity = '0';
                grid.style.transform = 'translateY(-10px)';

                // Trigger reflow for animation
                void grid.offsetWidth;

                grid.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
                grid.style.opacity = '1';
                grid.style.transform = 'translateY(0)';

                // Update icon
                if (icon) {
                    icon.classList.remove('fa-chevron-down');
                    icon.classList.add('fa-chevron-up');
                }
            } else {
                grid.style.transition = 'opacity 0.2s ease, transform 0.2s ease';
                grid.style.opacity = '0';
                grid.style.transform = 'translateY(-10px)';

                setTimeout(() => {
                    grid.style.display = 'none';
                }, 200);

                // Update icon
                if (icon) {
                    icon.classList.remove('fa-chevron-up');
                    icon.classList.add('fa-chevron-down');
                }
            }
        });
    });
}

/**
 * Initialize scroll effects for the sticky nav bar
 */
function initScrollEffects() {
    const navBar = document.querySelector('.category-filter-bar');

    if (!navBar) return;

    let lastScrollY = window.scrollY;

    window.addEventListener('scroll', () => {
        const currentScrollY = window.scrollY;

        // Add shadow when scrolled
        if (currentScrollY > 50) {
            navBar.classList.add('scrolled');
        } else {
            navBar.classList.remove('scrolled');
        }

        lastScrollY = currentScrollY;
    });
}

/**
 * Initialize drag-to-scroll for offers carousel on touch/mouse
 */
function initOfferCarouselDrag() {
    const carousel = document.querySelector('.offers-scroll-container');

    if (!carousel) return;

    let isDown = false;
    let startX;
    let scrollLeft;

    // Mouse events
    carousel.addEventListener('mousedown', (e) => {
        isDown = true;
        carousel.style.cursor = 'grabbing';
        startX = e.pageX - carousel.offsetLeft;
        scrollLeft = carousel.scrollLeft;
        // prevent text selection and image drag
        e.preventDefault();
    });

    carousel.addEventListener('mouseleave', () => {
        isDown = false;
        carousel.style.cursor = 'grab';
    });

    carousel.addEventListener('mouseup', () => {
        isDown = false;
        carousel.style.cursor = 'grab';
    });

    carousel.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault();
        const x = e.pageX - carousel.offsetLeft;
        const walk = (x - startX) * 1.5; // Scroll speed multiplier
        carousel.scrollLeft = scrollLeft - walk;
    });

    // Translate vertical scroll to horizontal scroll (Standard Desktop UX)
    carousel.addEventListener('wheel', (e) => {
        if (e.deltaY !== 0) {
            e.preventDefault();
            carousel.scrollLeft += e.deltaY;
        }
    });

    // Set initial cursor
    carousel.style.cursor = 'grab';

    // Touch events for mobile (native scrolling works, but this improves UX)
    carousel.addEventListener('touchstart', (e) => {
        startX = e.touches[0].pageX - carousel.offsetLeft;
        scrollLeft = carousel.scrollLeft;
    }, { passive: true });

    carousel.addEventListener('touchmove', (e) => {
        const x = e.touches[0].pageX - carousel.offsetLeft;
        const walk = (x - startX) * 1.2;
        carousel.scrollLeft = scrollLeft - walk;
    }, { passive: true });
}

/**
 * Utility: Check if element is in viewport
 */
function isInViewport(element) {
    const rect = element.getBoundingClientRect();
    return (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
        rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
}

/**
 * Utility: Debounce function for performance optimization
 */
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
