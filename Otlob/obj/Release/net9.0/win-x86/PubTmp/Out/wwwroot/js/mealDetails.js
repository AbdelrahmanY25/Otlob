/**
 * mealDetails.js - Meal Detail Page Functionality
 * Handles option selection, add-ons, quantity, price calculation, and cart addition
 */

document.addEventListener('DOMContentLoaded', function () {
    // ===== DOM ELEMENTS =====
    const form = document.getElementById('mealOrderForm');
    const quantityValue = document.getElementById('quantityValue');
    const decreaseBtn = document.getElementById('decreaseQty');
    const increaseBtn = document.getElementById('increaseQty');
    const addToCartBtn = document.getElementById('addToCartBtn');
    const totalPriceDisplay = document.getElementById('totalPriceDisplay');
    const specialInstructions = document.getElementById('specialInstructions');
    const charCount = document.getElementById('charCount');

    // ===== STATE =====
    let quantity = 1;
    const basePrice = parseFloat(form?.dataset.basePrice) || 0;
    const mealKey = form?.dataset.mealKey || '';

    // ===== INITIALIZATION =====
    init();

    function init() {
        setupQuantityControls();
        setupOptionListeners();
        setupAddonListeners();
        setupCharCounter();
        setupAddToCart();

        // Auto-select single options and check validation state
        autoSelectSingleOptions();
        updateAddToCartButtonState();
        updateTotalPrice();
    }

    // ===== QUANTITY CONTROLS =====
    const MIN_QUANTITY = 1;
    const MAX_QUANTITY = 99;
    const quantityInput = document.getElementById('quantityInput');

    function setupQuantityControls() {
        decreaseBtn?.addEventListener('click', () => {
            if (quantity > MIN_QUANTITY) {
                quantity--;
                updateQuantityDisplay();
                syncQuantityInput();
                updateTotalPrice();
            }
        });

        increaseBtn?.addEventListener('click', () => {
            if (quantity < MAX_QUANTITY) {
                quantity++;
                updateQuantityDisplay();
                syncQuantityInput();
                updateTotalPrice();
            }
        });
    }

    function syncQuantityInput() {
        if (quantityInput) {
            quantityInput.value = quantity;
        }
    }

    function updateQuantityDisplay() {
        if (quantityValue) {
            quantityValue.textContent = quantity;

            // Animate the quantity change
            quantityValue.style.transform = 'scale(1.2)';
            setTimeout(() => {
                quantityValue.style.transform = 'scale(1)';
            }, 150);
        }

        // Update decrease button state
        if (decreaseBtn) {
            decreaseBtn.style.opacity = quantity <= MIN_QUANTITY ? '0.5' : '1';
            decreaseBtn.style.pointerEvents = quantity <= MIN_QUANTITY ? 'none' : 'auto';
        }

        // Update increase button state
        if (increaseBtn) {
            increaseBtn.style.opacity = quantity >= MAX_QUANTITY ? '0.5' : '1';
            increaseBtn.style.pointerEvents = quantity >= MAX_QUANTITY ? 'none' : 'auto';
        }
    }

    // ===== OPTION LISTENERS =====
    function setupOptionListeners() {
        const optionRadios = document.querySelectorAll('.option-radio');

        optionRadios.forEach(radio => {
            radio.addEventListener('change', () => {
                // Clear validation error for this group
                const group = radio.closest('.option-group');
                if (group) {
                    group.classList.remove('invalid');
                    const errorEl = group.querySelector('.group-validation-error');
                    if (errorEl) errorEl.style.display = 'none';
                }

                updateTotalPrice();
                updateAddToCartButtonState();
            });
        });
    }

    // ===== ADDON LISTENERS =====
    function setupAddonListeners() {
        const addonCheckboxes = document.querySelectorAll('.addon-checkbox');

        addonCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', () => {
                updateTotalPrice();
            });
        });
    }

    // ===== AUTO SELECT SINGLE OPTIONS =====
    function autoSelectSingleOptions() {
        const optionGroups = document.querySelectorAll('.option-group');
        optionGroups.forEach(group => {
            const radios = group.querySelectorAll('.option-radio');
            if (radios.length === 1 && !radios[0].checked) {
                radios[0].checked = true;
                // Trigger change event to update state/price
                radios[0].dispatchEvent(new Event('change'));
            }
        });
    }

    // ===== BUTTON STATE VALIDATION =====
    function updateAddToCartButtonState() {
        if (!addToCartBtn) return;

        const isValid = checkValidationState();

        if (isValid) {
            addToCartBtn.disabled = false;
            addToCartBtn.style.opacity = '1';
            addToCartBtn.style.cursor = 'pointer';
            addToCartBtn.title = '';
        } else {
            addToCartBtn.disabled = true;
            addToCartBtn.style.opacity = '0.6';
            addToCartBtn.style.cursor = 'not-allowed';
            addToCartBtn.title = 'Please select all required options';
        }
    }

    function checkValidationState() {
        const optionGroups = document.querySelectorAll('.option-group');
        let allSatisfied = true;

        optionGroups.forEach(group => {
            const selectedOption = group.querySelector('.option-radio:checked');
            if (!selectedOption) {
                allSatisfied = false;
            }
        });

        return allSatisfied;
    }

    // ===== PRICE CALCULATION =====
    function calculateTotalPrice() {
        let total = basePrice;

        // Add selected option prices
        const selectedOptions = document.querySelectorAll('.option-radio:checked');
        selectedOptions.forEach(option => {
            const price = parseFloat(option.dataset.price) || 0;
            total += price;
        });

        // Add selected addon prices
        const selectedAddons = document.querySelectorAll('.addon-checkbox:checked');
        selectedAddons.forEach(addon => {
            const price = parseFloat(addon.dataset.price) || 0;
            total += price;
        });

        // Multiply by quantity
        total *= quantity;

        return total;
    }

    function updateTotalPrice() {
        const total = calculateTotalPrice();
        if (totalPriceDisplay) {
            totalPriceDisplay.textContent = `EGP ${total.toFixed(2)}`;
        }
    }

    // ===== CHARACTER COUNTER =====
    function setupCharCounter() {
        if (specialInstructions && charCount) {
            specialInstructions.addEventListener('input', () => {
                const length = specialInstructions.value.length;
                charCount.textContent = length;

                // Change color when approaching limit
                if (length > 220) {
                    charCount.parentElement.style.color = '#ff385d';
                } else {
                    charCount.parentElement.style.color = '';
                }
            });
        }
    }

    // ===== VALIDATION =====
    function validateForm() {
        const optionGroups = document.querySelectorAll('.option-group');
        let isValid = true;
        let firstInvalidGroup = null;

        optionGroups.forEach(group => {
            const groupId = group.dataset.groupId;
            const selectedOption = group.querySelector('.option-radio:checked');

            if (!selectedOption) {
                isValid = false;
                group.classList.add('invalid');

                const errorEl = group.querySelector('.group-validation-error');
                if (errorEl) errorEl.style.display = 'flex';

                if (!firstInvalidGroup) {
                    firstInvalidGroup = group;
                }
            } else {
                group.classList.remove('invalid');
                const errorEl = group.querySelector('.group-validation-error');
                if (errorEl) errorEl.style.display = 'none';
            }
        });

        // Scroll to first invalid group
        if (firstInvalidGroup) {
            firstInvalidGroup.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }

        return isValid;
    }

    // ===== ADD TO CART =====
    function setupAddToCart() {
        addToCartBtn?.addEventListener('click', () => {
            // Validate quantity is within range
            const qty = parseInt(quantityValue.textContent);
            if (qty < MIN_QUANTITY || qty > MAX_QUANTITY) {
                showToast(`Quantity must be between ${MIN_QUANTITY} and ${MAX_QUANTITY}`, 'error');
                return;
            }

            // Additional safety check
            if (!checkValidationState()) {
                validateForm(); // Highlight errors
                showToast('Please select all required options', 'error');
                return;
            }

            // Ensure quantity input is synced before form submission
            quantity = qty;
            syncQuantityInput();

            // Show loading state on button
            addToCartBtn.disabled = true;
            const originalText = addToCartBtn.querySelector('.btn-text').textContent;
            addToCartBtn.querySelector('.btn-text').textContent = 'Adding...';

            if (form) {
                // Before submitting, we need to handle the OptionItemsIds binding
                // because the radio buttons have unique names to prevent unchecking each other.

                // 1. Remove any existing dynamically added OptionItemsIds hidden inputs to avoid duplicates
                form.querySelectorAll('input[name="OptionItemsIds"]').forEach(el => el.remove());

                // 2. Add hidden inputs for each selected radio button with the name 'OptionItemsIds'
                const selectedOptions = form.querySelectorAll('.option-radio:checked');
                selectedOptions.forEach(radio => {
                    const hiddenInput = document.createElement('input');
                    hiddenInput.type = 'hidden';
                    hiddenInput.name = 'OptionItemsIds';
                    hiddenInput.value = radio.value;
                    form.appendChild(hiddenInput);
                });

                // 3. Addon checkboxes already have name="AddOnsIds", so they will submit correctly.

                // Submit the form
                form.submit();
            }
        });
    }

    // ===== TOAST NOTIFICATIONS =====
    function showToast(message, type = 'info') {
        // Remove existing toasts
        const existingToast = document.querySelector('.meal-toast');
        if (existingToast) existingToast.remove();

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `meal-toast meal-toast-${type}`;
        toast.innerHTML = `
            <i class="fa-solid ${type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle'}"></i>
            <span>${message}</span>
        `;

        // Add styles
        Object.assign(toast.style, {
            position: 'fixed',
            top: '20px',
            left: '50%',
            transform: 'translateX(-50%) translateY(-20px)',
            padding: '14px 24px',
            background: type === 'success' ? '#00b894' : '#ff385d',
            color: '#fff',
            borderRadius: '12px',
            boxShadow: '0 6px 20px rgba(0,0,0,0.2)',
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
            fontSize: '0.95rem',
            fontWeight: '600',
            zIndex: '9999',
            opacity: '0',
            transition: 'all 0.3s ease'
        });

        document.body.appendChild(toast);

        // Animate in
        requestAnimationFrame(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(-50%) translateY(0)';
        });

        // Remove after delay
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(-50%) translateY(-20px)';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
});
