/**
 * Cart Page JavaScript
 * Handles: Increment, Decrement, Remove operations via AJAX
 * Updates: Cart total, item count, minimum order validation, checkout button state
 */

(function () {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', initCart);

    function initCart() {
        // Attach click handlers to all cart action buttons
        document.querySelectorAll('.ajax-cart-btn').forEach(function (btn) {
            btn.addEventListener('click', handleCartAction);
        });
    }

    /**
     * Handle increment/decrement/remove button clicks
     */
    async function handleCartAction(e) {
        e.preventDefault();

        const btn = e.currentTarget;

        // Prevent double-clicks
        if (btn.disabled) return;
        btn.disabled = true;

        const itemId = btn.dataset.id;
        const url = btn.dataset.url;

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                window.location.reload();
                return;
            }

            const data = await response.json();

            if (data.success) {
                // Update all UI elements with server response
                updateUI(itemId, data);
            } else {
                alert(data.message || 'Error updating cart');
            }
        } catch (error) {
            console.error('Cart action error:', error);
            window.location.reload();
        } finally {
            btn.disabled = false;
        }
    }

    /**
     * Update all UI elements after a successful cart operation
     */
    function updateUI(itemId, data) {
        // 1. Update cart total price display
        updateTotalPrice(data.cartTotal);

        // 2. Update cart item count display
        updateItemCount(data.cartCount);

        // 3. Update minimum order validation (warning + checkout button)
        updateMinOrderValidation(data.cartTotal, data.minPrice, data.isLessThanMin);

        // 4. Handle item-specific updates
        if (data.removed) {
            removeItemFromDOM(itemId, data.cartCount);
        } else {
            updateItemDetails(itemId, data);
        }
    }

    /**
     * Update the cart total price display
     */
    function updateTotalPrice(cartTotal) {
        const el = document.getElementById('cart-total-price');
        if (el && cartTotal !== undefined) {
            el.textContent = formatPrice(cartTotal);
        }
    }

    /**
     * Update the cart item count display
     */
    function updateItemCount(count) {
        const el = document.getElementById('cart-count');
        if (el && count !== undefined) {
            el.textContent = count + ' items';
        }
    }

    /**
     * Update minimum order validation UI
     * - Show/hide warning message
     * - Update remaining amount
     * - Enable/disable checkout button
     */
    function updateMinOrderValidation(cartTotal, minPrice, isLessThanMin) {
        const warningEl = document.getElementById('min-price-warning');
        const remainingEl = document.getElementById('min-price-remaining');
        const btnContainer = document.getElementById('checkout-btn-container');

        // Ensure we have numeric values
        const total = parseFloat(cartTotal) || 0;
        const min = parseFloat(minPrice) || 0;

        // Update warning visibility and remaining amount
        if (warningEl) {
            if (isLessThanMin) {
                warningEl.style.display = 'flex';
                if (remainingEl) {
                    const remaining = Math.max(0, min - total);
                    remainingEl.textContent = formatPrice(remaining);
                }
            } else {
                warningEl.style.display = 'none';
            }
        }

        // Update checkout button state
        if (btnContainer) {
            const checkoutUrl = btnContainer.dataset.checkoutUrl || '/Customer/CheckOut/CheckOut';

            if (isLessThanMin) {
                // Disabled button
                btnContainer.innerHTML = '<button type="button" class="checkout-btn disabled" id="checkout-btn" disabled>Proceed to Checkout</button>';
            } else {
                // Enabled link
                btnContainer.innerHTML = '<a href="' + checkoutUrl + '" class="checkout-btn" id="checkout-btn">Proceed to Checkout</a>';
            }
        }
    }

    /**
     * Remove an item from the DOM with animation
     */
    function removeItemFromDOM(itemId, cartCount) {
        const itemRow = document.getElementById('cart-item-' + itemId);

        if (itemRow) {
            // Animate removal
            itemRow.style.transition = 'opacity 0.3s, transform 0.3s';
            itemRow.style.opacity = '0';
            itemRow.style.transform = 'translateX(20px)';

            setTimeout(function () {
                itemRow.remove();

                // If cart is now empty, reload to show empty cart view
                if (cartCount === 0) {
                    window.location.reload();
                }
            }, 300);
        } else if (cartCount === 0) {
            window.location.reload();
        }
    }

    /**
     * Update item quantity and price display
     */
    function updateItemDetails(itemId, data) {
        // Update quantity display
        const qtyEl = document.getElementById('qty-val-' + itemId);
        if (qtyEl) {
            qtyEl.textContent = data.quantity;
        }

        // Update item price display
        const priceEl = document.getElementById('item-price-' + itemId);
        if (priceEl && data.itemTotal !== undefined) {
            priceEl.textContent = formatPrice(data.itemTotal);
        }

        // Update decrease button (trash icon when qty=1, minus icon otherwise)
        updateDecreaseButton(itemId, data.quantity);
    }

    /**
     * Update the decrease button appearance based on quantity
     * - Quantity = 1: Show trash icon, link to Remove action
     * - Quantity > 1: Show minus icon, link to Decrement action
     */
    function updateDecreaseButton(itemId, quantity) {
        const row = document.getElementById('cart-item-' + itemId);
        if (!row) return;

        const decreaseBtn = row.querySelector('[data-action="decrease"]');
        if (!decreaseBtn) return;

        if (quantity === 1) {
            // Show trash icon
            decreaseBtn.classList.add('trash');
            decreaseBtn.innerHTML = '<i class="bi bi-trash"></i>';
            decreaseBtn.title = 'Remove Item';
            // Update URL to Remove action
            decreaseBtn.dataset.url = decreaseBtn.dataset.url.replace('Decrement', 'Remove');
        } else {
            // Show minus icon
            decreaseBtn.classList.remove('trash');
            decreaseBtn.innerHTML = '<i class="bi bi-dash"></i>';
            decreaseBtn.title = 'Decrease Quantity';
            // Update URL to Decrement action
            decreaseBtn.dataset.url = decreaseBtn.dataset.url.replace('Remove', 'Decrement');
        }
    }

    /**
     * Format a number as price with 2 decimal places
     */
    function formatPrice(value) {
        return parseFloat(value).toFixed(2) + ' L.E';
    }

})();