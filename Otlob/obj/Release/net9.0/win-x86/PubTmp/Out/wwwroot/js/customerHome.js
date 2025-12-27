document.addEventListener('DOMContentLoaded', function () {
    const state = {
        searchQuery: '',
        selectedCategory: null,
        freeDelivery: false,
        fastDelivery: false,
        businessType: null
    };

    const searchInput = document.getElementById('searchInput');
    let categoryItems = document.querySelectorAll('.ch-category-item');
    const categoriesScroll = document.querySelector('.ch-categories-scroll');
    const filterButtons = document.querySelectorAll('.ch-filter-btn');
    const restaurantsContainer = document.getElementById('restaurantsGrid');
    // Store original order for resetting sort
    const restaurantCards = Array.from(document.querySelectorAll('.ch-restaurant-card'));
    const noResults = document.getElementById('noResults');

    // Search Listener
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            state.searchQuery = e.target.value.toLowerCase();
            applyFilters();
        });
    }

    // Filter Buttons Listener
    filterButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const filterType = btn.dataset.filter;
            const filterValue = btn.dataset.value;

            if (filterType === 'freeDelivery') {
                state.freeDelivery = !state.freeDelivery;
                btn.classList.toggle('active');
            } else if (filterType === 'fastDelivery') {
                state.fastDelivery = !state.fastDelivery;
                btn.classList.toggle('active');
            } else if (filterType === 'businessType') {
                if (state.businessType === filterValue) {
                    state.businessType = null;
                    btn.classList.remove('active');
                } else {
                    state.businessType = filterValue;
                    // Remove active from other business type buttons
                    document.querySelectorAll('[data-filter="businessType"]').forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');
                }
            }
            applyFilters();
        });
    });

    // Enable horizontal scroll with mouse wheel and drag for categories
    if (categoriesScroll) {
        // Wheel -> horizontal scroll
        categoriesScroll.addEventListener('wheel', function (e) {
            if (Math.abs(e.deltaY) > Math.abs(e.deltaX)) {
                e.preventDefault();
                categoriesScroll.scrollLeft += e.deltaY;
            }
        }, { passive: false });

        // Drag to scroll (pointer events)
        let isPointerDown = false;
        let startX = 0;
        let startScrollLeft = 0;
        let hasMoved = false;

        categoriesScroll.addEventListener('pointerdown', (e) => {
            // Don't interfere with clicks on category items
            if (e.target.closest('.ch-category-item')) {
                return;
            }
            
            isPointerDown = true;
            hasMoved = false;
            categoriesScroll.setPointerCapture(e.pointerId);
            startX = e.clientX;
            startScrollLeft = categoriesScroll.scrollLeft;
            categoriesScroll.style.cursor = 'grabbing';
        });

        categoriesScroll.addEventListener('pointermove', (e) => {
            if (!isPointerDown) return;
            const dx = e.clientX - startX;
            
            // Mark as moved if dragged more than 5px
            if (Math.abs(dx) > 5) {
                hasMoved = true;
            }
            
            categoriesScroll.scrollLeft = startScrollLeft - dx;
        });

        const releasePointer = (e) => {
            if (!isPointerDown) return;
            isPointerDown = false;
            try { categoriesScroll.releasePointerCapture(e.pointerId); } catch { }
            categoriesScroll.style.cursor = 'grab';
        };

        categoriesScroll.addEventListener('pointerup', releasePointer);
        categoriesScroll.addEventListener('pointercancel', releasePointer);

        // Set initial cursor
        categoriesScroll.style.cursor = 'grab';
        
        // Event delegation for category clicks
        categoriesScroll.addEventListener('click', (e) => {
            const item = e.target.closest('.ch-category-item');
            if (!item || hasMoved) return; // Ignore if dragged
            
            const category = item.dataset.category;
            
            // Toggle category
            if (state.selectedCategory === category) {
                state.selectedCategory = null;
                item.classList.remove('active');
            } else {
                state.selectedCategory = category;
                categoryItems.forEach(i => i.classList.remove('active'));
                item.classList.add('active');
            }
            applyFilters();
        });
    }

    function applyFilters() {
        let visibleCount = 0;
        
        // 1. Determine visibility based on filters
        const visibleCards = [];
        const hiddenCards = [];

        restaurantCards.forEach(card => {
            const name = card.dataset.name.toLowerCase();
            const categories = card.dataset.categories.toLowerCase();
            const deliveryFee = parseFloat(card.dataset.deliveryFee);
            const businessType = card.dataset.businessType;

            let isVisible = true;

            // Search
            if (state.searchQuery && !name.includes(state.searchQuery)) {
                isVisible = false;
            }

            // Category
            if (isVisible && state.selectedCategory && !categories.includes(state.selectedCategory.toLowerCase())) {
                isVisible = false;
            }

            // Free Delivery
            if (isVisible && state.freeDelivery && deliveryFee > 0) {
                isVisible = false;
            }

            // Business Type
            if (isVisible && state.businessType && businessType !== state.businessType) {
                isVisible = false;
            }

            if (isVisible) {
                visibleCards.push(card);
                visibleCount++;
            } else {
                hiddenCards.push(card);
            }
        });

        // 2. Sort visible cards if needed
        if (state.fastDelivery) {
            visibleCards.sort((a, b) => {
                const timeA = parseFloat(a.dataset.deliveryDuration);
                const timeB = parseFloat(b.dataset.deliveryDuration);
                return timeA - timeB;
            });
        } else {
            // Restore original order (by index in original array)
            visibleCards.sort((a, b) => {
                return restaurantCards.indexOf(a) - restaurantCards.indexOf(b);
            });
        }

        // 3. Update DOM
        // Clear container
        restaurantsContainer.innerHTML = '';
        
        // Append visible sorted cards
        visibleCards.forEach(card => {
            card.style.display = '';
            restaurantsContainer.appendChild(card);
        });

        // Append hidden cards (hidden) to keep them in DOM if needed, or just don't append
        // Better to append them hidden so they are there for next filter pass if we were using live list
        // But since we use `restaurantCards` array as source of truth, we can just append visible ones?
        // Actually, let's append hidden ones too with display:none to maintain DOM integrity if other scripts rely on it.
        hiddenCards.forEach(card => {
            card.style.display = 'none';
            restaurantsContainer.appendChild(card);
        });

        // Show/Hide No Results
        if (visibleCount === 0) {
            noResults.style.display = 'block';
        } else {
            noResults.style.display = 'none';
        }
    }
});
