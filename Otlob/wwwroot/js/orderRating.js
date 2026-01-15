// Order Rating Page JavaScript

document.addEventListener('DOMContentLoaded', function () {
    initializeRatingForm();
});

function initializeRatingForm() {
    const form = document.getElementById('ratingForm');
    const checkboxes = document.querySelectorAll('.rating-tag input[type="checkbox"]');
    const commentBox = document.getElementById('commentBox');
    const charCount = document.getElementById('charCount');
    const validationMessage = document.getElementById('tagValidation');
    const submitBtn = document.getElementById('submitBtn');

    if (!form) return;

    // Character counter for comment box
    if (commentBox && charCount) {
        commentBox.addEventListener('input', function () {
            charCount.textContent = this.value.length;
        });
    }

    // Add visual feedback on tag selection
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            // Hide validation message when a tag is selected
            if (validationMessage) {
                validationMessage.style.display = 'none';
            }

            // Add ripple effect
            const card = this.nextElementSibling;
            if (this.checked) {
                addRippleEffect(card);
            }
        });
    });

    // Form submission validation
    form.addEventListener('submit', function (e) {
        const anyChecked = Array.from(checkboxes).some(cb => cb.checked);

        if (!anyChecked) {
            e.preventDefault();

            if (validationMessage) {
                validationMessage.style.display = 'flex';
            }

            // Scroll to tags section
            const tagsSection = document.querySelector('.rating-tags-section');
            if (tagsSection) {
                tagsSection.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }

            // Shake animation on tags
            const tagsGrid = document.querySelector('.rating-tags-grid');
            if (tagsGrid) {
                tagsGrid.classList.add('shake');
                setTimeout(() => tagsGrid.classList.remove('shake'), 500);
            }

            return false;
        }

        // Show loading state
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="bi bi-hourglass-split"></i> Submitting...';
        }
    });
}

function addRippleEffect(element) {
    const ripple = document.createElement('span');
    ripple.classList.add('tag-ripple');
    element.appendChild(ripple);

    setTimeout(() => {
        ripple.remove();
    }, 600);
}

// Add shake animation styles dynamically
const style = document.createElement('style');
style.textContent = `
    @keyframes shake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(-5px); }
        50% { transform: translateX(5px); }
        75% { transform: translateX(-5px); }
    }
    
    .shake {
        animation: shake 0.5s ease;
    }
    
    .tag-ripple {
        position: absolute;
        top: 50%;
        left: 50%;
        width: 100%;
        height: 100%;
        background: radial-gradient(circle, rgba(255, 56, 93, 0.3) 0%, transparent 70%);
        transform: translate(-50%, -50%) scale(0);
        animation: ripple 0.6s ease-out;
        pointer-events: none;
        border-radius: 16px;
    }
    
    @keyframes ripple {
        0% { transform: translate(-50%, -50%) scale(0); opacity: 1; }
        100% { transform: translate(-50%, -50%) scale(2); opacity: 0; }
    }
`;
document.head.appendChild(style);
