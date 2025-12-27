function goBack() {
    window.history.back();
}

document.addEventListener('DOMContentLoaded', () => {
    const buttons = document.querySelectorAll('.ajax-cart-btn');

    buttons.forEach(btn => {
        btn.addEventListener('click', async (e) => {
            e.preventDefault();

            // Prevent double clicks
            if (btn.disabled) return;
            btn.disabled = true;

            const action = btn.dataset.action; // 'increase' or 'decrease'
            const itemId = btn.dataset.id;
            const url = btn.dataset.url;

            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (response.ok) {
                    const data = await response.json();

                    if (data.success) {
                        updateCartUI(itemId, data, action);
                    } else {
                        console.error('Operation failed:', data.message);
                        alert(data.message || 'Error updating cart');
                    }
                } else {
                    // Fallback for non-ajax redirect or error
                    window.location.reload();
                }
            } catch (error) {
                console.error('Error:', error);
            } finally {
                btn.disabled = false;
            }
        });
    });
});

function updateCartUI(itemId, data, action) {
    // If item was removed (quantity 0)
    if (data.removed) {
        const itemRow = document.getElementById(`cart-item-${itemId}`);
        if (itemRow) {
            itemRow.style.transition = 'opacity 0.3s, transform 0.3s';
            itemRow.style.opacity = '0';
            itemRow.style.transform = 'translateX(20px)';
            setTimeout(() => itemRow.remove(), 300);
        }

        // If cart is empty now
        if (data.cartCount === 0) {
            setTimeout(() => window.location.reload(), 300);
            return;
        }
    } else {
        // Update quantity
        const qtyEl = document.getElementById(`qty-val-${itemId}`);
        if (qtyEl) qtyEl.textContent = data.quantity;

        // Update item total price
        const priceEl = document.getElementById(`item-price-${itemId}`);
        if (priceEl && data.itemTotal !== undefined) {
            priceEl.textContent = `${data.itemTotal.toFixed(2)} L.E`;
        }

        // Handle button state (trash icon vs minus icon)
        if (data.quantity === 1) {
            const row = document.getElementById(`cart-item-${itemId}`);
            const decreaseBtn = row.querySelector('[data-action="decrease"]');
            if (decreaseBtn) {
                decreaseBtn.classList.add('trash');
                decreaseBtn.innerHTML = '<i class="bi bi-trash"></i>';
                decreaseBtn.title = "Remove Item";
                // Update URL to remove action
                decreaseBtn.dataset.url = decreaseBtn.dataset.url.replace('Decrement', 'Remove');
            }
        } else {
            const row = document.getElementById(`cart-item-${itemId}`);
            const decreaseBtn = row.querySelector('[data-action="decrease"]');
            if (decreaseBtn) {
                decreaseBtn.classList.remove('trash');
                decreaseBtn.innerHTML = '<i class="bi bi-dash"></i>';
                decreaseBtn.title = "Decrease Quantity";
                // Update URL back to decrement action
                decreaseBtn.dataset.url = decreaseBtn.dataset.url.replace('Remove', 'Decrement');
            }
        }
    }

    // Update Cart Total
    const totalEl = document.getElementById('cart-total-price');
    if (totalEl && data.cartTotal !== undefined) {
        totalEl.textContent = `${data.cartTotal.toFixed(2)} L.E`;
    }

    // Update Cart Count
    const countEl = document.getElementById('cart-count');
    if (countEl && data.cartCount !== undefined) {
        countEl.textContent = `${data.cartCount} items`;
    }
}
