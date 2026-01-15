/* =========================================
   Admin User Orders JavaScript
   ========================================= */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize any interactive elements
    initializeOrderCards();
});

/**
 * Initialize order card interactions
 */
function initializeOrderCards() {
    const orderCards = document.querySelectorAll('.admin-order-card');
    
    orderCards.forEach(card => {
        // Add subtle animation on load
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.4s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100 * Array.from(orderCards).indexOf(card));
    });
}

/**
 * Format currency display
 * @param {number} amount - The amount to format
 * @returns {string} - Formatted currency string
 */
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-EG', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(amount) + ' L.E';
}

/**
 * Copy order ID to clipboard
 * @param {string} orderId - The order ID to copy
 */
function copyOrderId(orderId) {
    navigator.clipboard.writeText(orderId).then(() => {
        showNotification('Order ID copied to clipboard!', 'success');
    }).catch(() => {
        showNotification('Failed to copy order ID', 'error');
    });
}

/**
 * Show notification toast
 * @param {string} message - The message to display
 * @param {string} type - The notification type (success, error, info)
 */
function showNotification(message, type = 'info') {
    // Create notification element if it doesn't exist
    let notification = document.querySelector('.admin-notification');
    
    if (!notification) {
        notification = document.createElement('div');
        notification.className = 'admin-notification';
        document.body.appendChild(notification);
    }
    
    // Set notification content and style
    notification.textContent = message;
    notification.className = `admin-notification ${type}`;
    notification.style.cssText = `
        position: fixed;
        bottom: 20px;
        right: 20px;
        padding: 12px 24px;
        border-radius: 8px;
        color: white;
        font-weight: 500;
        z-index: 9999;
        animation: slideIn 0.3s ease;
        background: ${type === 'success' ? '#059669' : type === 'error' ? '#dc2626' : '#4F46E5'};
    `;
    
    // Remove notification after delay
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Add notification animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from { transform: translateX(100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
    @keyframes slideOut {
        from { transform: translateX(0); opacity: 1; }
        to { transform: translateX(100%); opacity: 0; }
    }
`;
document.head.appendChild(style);
