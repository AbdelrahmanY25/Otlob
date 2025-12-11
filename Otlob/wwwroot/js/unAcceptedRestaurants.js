// Unaccepted Restaurants Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initializeUnacceptedRestaurants();
});

function initializeUnacceptedRestaurants() {
    const searchInput = document.getElementById('restaurantSearch');
    const restaurantCards = document.querySelectorAll('.restaurant-card');
    const restaurantsGrid = document.querySelector('.restaurants-grid');
    const noResults = document.querySelector('.no-results');
    const countBadge = document.querySelector('.count-badge');
    const totalCount = restaurantCards.length;

    // Initialize count
    updateCount(totalCount);

    // Search functionality
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase().trim();
            filterRestaurants(searchTerm);
        });

        // Clear search on ESC key
        searchInput.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                searchInput.value = '';
                filterRestaurants('');
                searchInput.blur();
            }
        });
    }

    // Filter restaurants based on search term
    function filterRestaurants(searchTerm) {
        let visibleCount = 0;

        restaurantCards.forEach((card, index) => {
            const restaurantName = card.querySelector('.restaurant-name').textContent.toLowerCase();
            const isMatch = restaurantName.includes(searchTerm);

            if (isMatch) {
                card.style.display = 'block';
                // Add stagger animation
                card.style.animation = `fadeIn 0.4s ease-out ${index * 0.05}s both`;
                visibleCount++;
            } else {
                card.style.display = 'none';
            }
        });

        // Update count
        updateCount(visibleCount);

        // Show/hide no results message
        if (visibleCount === 0 && searchTerm !== '') {
            noResults.classList.add('show');
            restaurantsGrid.style.display = 'none';
        } else {
            noResults.classList.remove('show');
            restaurantsGrid.style.display = 'grid';
        }
    }

    // Update count badge
    function updateCount(count) {
        if (countBadge) {
            countBadge.textContent = count;
            
            // Animate count change
            countBadge.style.transform = 'scale(1.2)';
            setTimeout(() => {
                countBadge.style.transform = 'scale(1)';
            }, 200);
        }
    }

    // Add click animation to cards
    restaurantCards.forEach(card => {
        card.addEventListener('mousedown', function() {
            this.style.transform = 'translateY(-8px) scale(0.98)';
        });

        card.addEventListener('mouseup', function() {
            this.style.transform = 'translateY(-10px) scale(1)';
        });

        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Smooth scroll to top button (if needed)
    const scrollToTopBtn = createScrollToTopButton();

    window.addEventListener('scroll', function() {
        if (window.pageYOffset > 300) {
            scrollToTopBtn.classList.add('show');
        } else {
            scrollToTopBtn.classList.remove('show');
        }
    });

    // Lazy loading images
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src || img.src;
                img.classList.add('loaded');
                observer.unobserve(img);
            }
        });
    }, {
        rootMargin: '50px'
    });

    document.querySelectorAll('.restaurant-image-wrapper img').forEach(img => {
        imageObserver.observe(img);
    });

    // Add keyboard navigation
    document.addEventListener('keydown', function(e) {
        // Focus search on Ctrl/Cmd + K
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
    });

    // Performance optimization: Debounce search
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

    // Apply debounce to search if many restaurants
    if (totalCount > 20 && searchInput) {
        const debouncedFilter = debounce(function(searchTerm) {
            filterRestaurants(searchTerm);
        }, 300);

        searchInput.removeEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase().trim();
            filterRestaurants(searchTerm);
        });

        searchInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase().trim();
            debouncedFilter(searchTerm);
        });
    }
}

// Create scroll to top button
function createScrollToTopButton() {
    const scrollBtn = document.createElement('button');
    scrollBtn.className = 'scroll-to-top';
    scrollBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
    scrollBtn.setAttribute('aria-label', 'Scroll to top');
    
    const style = document.createElement('style');
    style.textContent = `
        .scroll-to-top {
            position: fixed;
            bottom: 30px;
            right: 30px;
            width: 50px;
            height: 50px;
            background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
            color: white;
            border: none;
            border-radius: 50%;
            cursor: pointer;
            opacity: 0;
            visibility: hidden;
            transition: all 0.3s ease;
            z-index: 1000;
            box-shadow: 0 4px 12px rgba(240, 67, 53, 0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
        }
        
        .scroll-to-top.show {
            opacity: 1;
            visibility: visible;
        }
        
        .scroll-to-top:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(240, 67, 53, 0.4);
        }
        
        .scroll-to-top:active {
            transform: translateY(-2px);
        }
        
        @media (max-width: 768px) {
            .scroll-to-top {
                width: 45px;
                height: 45px;
                bottom: 20px;
                right: 20px;
                font-size: 16px;
            }
        }
    `;
    
    document.head.appendChild(style);
    document.body.appendChild(scrollBtn);
    
    scrollBtn.addEventListener('click', function() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
    
    return scrollBtn;
}

// Export functions for external use
window.UnacceptedRestaurants = {
    initializeUnacceptedRestaurants,
    createScrollToTopButton
};
