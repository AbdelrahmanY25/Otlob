let groupIndex = 0;
let itemIndexes = {}; // Track item indexes per group
const MAX_OPTION_GROUPS = 25;
const MAX_OPTION_ITEMS_PER_GROUP = 20;
let validationErrors = [];

// Image preview function
function previewImage(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('mealLogo').src = e.target.result;
            document.querySelector('.am-media-upload-area').classList.add('has-image');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Toggle Option Groups visibility
function toggleOptionGroups(show) {
    const container = document.getElementById('optionGroupsContainer');
    const checkbox = document.getElementById('hasOptionGroupCheckbox');

    container.style.display = show ? 'block' : 'none';

    if (show) {
        // If showing and no groups exist, add one automatically
        if (document.querySelectorAll('.am-option-group-card').length === 0) {
            addOptionGroup();
        }
    } else {
        // Clear all groups when disabled
        document.getElementById('groupsList').innerHTML = '';
        groupIndex = 0;
        itemIndexes = {};
    }

    // Sync checkbox state
    if (checkbox) {
        checkbox.checked = show;
    }

    validateForm();
}

// Reindex all form fields to ensure sequential indices for model binding
function reindexAllFields() {
    const groups = document.querySelectorAll('.am-option-group-card');
    
    groups.forEach((group, groupIdx) => {
        // Update group data attribute
        group.setAttribute('data-group-index', groupIdx);
        
        // Update group ID
        group.id = `group-${groupIdx}`;
        
        // Update all group-level inputs
        const groupInputs = group.querySelectorAll('input[name^="OptionGroups["], select[name^="OptionGroups["]');
        groupInputs.forEach(input => {
            const name = input.name;
            // Match OptionGroups[X] pattern and replace X with new index
            const newName = name.replace(/OptionGroups\[\d+\]/, `OptionGroups[${groupIdx}]`);
            input.name = newName;
        });
        
        // Update items container ID
        const itemsContainer = group.querySelector('.am-option-items-container');
        if (itemsContainer) {
            itemsContainer.id = `items-container-${groupIdx}`;
        }
        
        // Update remove group button onclick
        const removeGroupBtn = group.querySelector('.am-btn-icon-danger');
        if (removeGroupBtn) {
            removeGroupBtn.setAttribute('onclick', `removeGroup(${groupIdx})`);
        }
        
        // Update add item button onclick
        const addItemBtn = group.querySelector('.am-add-item-btn');
        if (addItemBtn) {
            addItemBtn.setAttribute('onclick', `addOptionItem(${groupIdx})`);
        }
        
        // Update DisplayOrder hidden input value (automatic)
        const displayOrderInput = group.querySelector('input[name*=".DisplayOrder"]:not([name*="OptionItems"])');
        if (displayOrderInput) {
            displayOrderInput.value = groupIdx + 1;
        }
        
        // Reindex items within this group
        const items = itemsContainer ? itemsContainer.querySelectorAll('.am-option-item-row') : [];
        items.forEach((item, itemIdx) => {
            // Update item ID
            item.id = `item-${groupIdx}-${itemIdx}`;
            
            // Update all item-level inputs
            const itemInputs = item.querySelectorAll('input[name*="OptionItems["], select[name*="OptionItems["]');
            itemInputs.forEach(input => {
                const name = input.name;
                // Replace both group and item indices
                let newName = name.replace(/OptionGroups\[\d+\]/, `OptionGroups[${groupIdx}]`);
                newName = newName.replace(/OptionItems\[\d+\]/, `OptionItems[${itemIdx}]`);
                input.name = newName;
                
                // Update file input ID if it's the image input
                if (input.type === 'file' && name.includes('ImageRequest.Image')) {
                    input.id = `item-image-${groupIdx}-${itemIdx}`;
                }
            });
            
            // Update item DisplayOrder value (automatic)
            const itemDisplayOrderInput = item.querySelector('input[name*="OptionItems"][name*=".DisplayOrder"]');
            if (itemDisplayOrderInput) {
                itemDisplayOrderInput.value = itemIdx + 1;
            }
            
            // Update preview image ID
            const previewImg = item.querySelector('.am-mini-preview');
            if (previewImg) {
                previewImg.id = `item-preview-${groupIdx}-${itemIdx}`;
            }
            
            // Update remove item button onclick
            const removeItemBtn = item.querySelector('.am-btn-icon-subtle');
            if (removeItemBtn) {
                removeItemBtn.setAttribute('onclick', `removeItem(${groupIdx}, ${itemIdx})`);
            }
            
            // Update mini-upload onclick
            const miniUpload = item.querySelector('.am-mini-upload');
            if (miniUpload) {
                miniUpload.setAttribute('onclick', `triggerItemImageUpload(${groupIdx}, ${itemIdx})`);
            }
            
            // Update file input onchange
            const fileInput = item.querySelector('input[type="file"]');
            if (fileInput) {
                fileInput.setAttribute('onchange', `previewItemImage(${groupIdx}, ${itemIdx}, this)`);
            }
        });
    });
    
    // Update global counters
    groupIndex = groups.length;
    itemIndexes = {};
    groups.forEach((group, idx) => {
        const itemsContainer = group.querySelector('.am-option-items-container');
        const itemCount = itemsContainer ? itemsContainer.querySelectorAll('.am-option-item-row').length : 0;
        itemIndexes[idx] = itemCount;
    });
}

// Add new option group
function addOptionGroup() {
    const groupCount = document.querySelectorAll('.am-option-group-card').length;

    if (groupCount >= MAX_OPTION_GROUPS) {
        return;
    }

    const groupsList = document.getElementById('groupsList');
    const currentIndex = groupCount; // Use current count as index
    itemIndexes[currentIndex] = 0;

    const groupHtml = `
        <div class="am-option-group-card" id="group-${currentIndex}" data-group-index="${currentIndex}">
            <div class="am-group-header-row">
                <div class="am-form-group flex-grow">
                    <label class="am-label">Option Name</label>
                    <input name="OptionGroups[${currentIndex}].Name" 
                           class="am-input group-name-input" 
                           placeholder="e.g. Size" 
                           oninput="validateForm()"
                           required />
                </div>
                <button type="button" class="am-btn-icon-danger" onclick="removeGroup(${currentIndex})" title="Remove">
                    <i class="fas fa-trash-alt"></i>
                </button>
            </div>
            
            <div class="am-group-values-section">
                <label class="am-label">Option Values</label>
                <div class="am-option-items-container" id="items-container-${currentIndex}">
                    <!-- Items will be added here -->
                </div>
                <div class="am-add-item-wrapper">
                    <button type="button" class="am-btn-link am-add-item-btn" onclick="addOptionItem(${currentIndex})">
                        <i class="fas fa-plus"></i> Add value
                    </button>
                </div>
            </div>
            <input type="hidden" name="OptionGroups[${currentIndex}].DisplayOrder" value="${currentIndex + 1}" />
        </div>
    `;

    groupsList.insertAdjacentHTML('beforeend', groupHtml);
    
    groupIndex = currentIndex + 1;
    
    addOptionItem(currentIndex);

    updateAddGroupButton();
    validateForm();
}

// Remove option group
function removeGroup(groupIdx) {
    const group = document.getElementById(`group-${groupIdx}`);
    if (group) {
        group.remove();
    }

    // Reindex all fields after removal
    reindexAllFields();

    const remainingGroups = document.querySelectorAll('.am-option-group-card');
    if (remainingGroups.length === 0) {
        const checkbox = document.getElementById('hasOptionGroupCheckbox');
        if (checkbox) {
            checkbox.checked = false;
        }
        toggleOptionGroups(false);
    }

    updateAddGroupButton();
    validateForm();
}

// Add option item to a group
function addOptionItem(groupIdx) {
    const container = document.getElementById(`items-container-${groupIdx}`);
    if (!container) return;

    const currentItemCount = container.querySelectorAll('.am-option-item-row').length;
    if (currentItemCount >= MAX_OPTION_ITEMS_PER_GROUP) {
        return;
    }

    const itemIdx = currentItemCount; // Use current count as index

    const itemHtml = `
        <div class="am-option-item-row" id="item-${groupIdx}-${itemIdx}">
            <div class="am-form-group flex-grow">
                <input name="OptionGroups[${groupIdx}].OptionItems[${itemIdx}].Name" 
                       class="am-input" 
                       placeholder="Value (e.g. Small)"
                       oninput="validateForm()" 
                       required />
            </div>
            <div class="am-form-group w-medium">
                <div class="am-input-wrapper">
                    <span class="am-currency-symbol">EGP</span>
                    <input type="number" 
                           step="0.01"  
                           min="0"
                           name="OptionGroups[${groupIdx}].OptionItems[${itemIdx}].Price" 
                           class="am-input" 
                           placeholder="0.00"
                           value="0"
                           oninput="validateForm()"
                           required />
                </div>
            </div>
            <div class="am-item-actions">
                <label class="am-popular-toggle" title="Mark as Popular" onclick="togglePopular(this)">
                    <input type="checkbox" 
                           name="OptionGroups[${groupIdx}].OptionItems[${itemIdx}].IsPobular" 
                           value="true" />
                    <i class="fas fa-fire popular-icon"></i>
                    <span class="popular-text">Popular</span>
                </label>
                <div class="am-mini-upload" onclick="triggerItemImageUpload(${groupIdx}, ${itemIdx})">
                    <label class="am-icon-btn" title="Upload Image">
                        <i class="fas fa-image"></i>
                    </label>
                    <input type="file" 
                           name="OptionGroups[${groupIdx}].OptionItems[${itemIdx}].ImageRequest.Image"
                           id="item-image-${groupIdx}-${itemIdx}"
                           class="am-hidden-input" 
                           accept="image/*"
                           onchange="previewItemImage(${groupIdx}, ${itemIdx}, this)" />
                    <img id="item-preview-${groupIdx}-${itemIdx}" src="" class="am-mini-preview" style="display:none;" />
                </div>
                <button type="button" class="am-btn-icon-subtle" onclick="removeItem(${groupIdx}, ${itemIdx})" title="Remove">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <input type="hidden" name="OptionGroups[${groupIdx}].OptionItems[${itemIdx}].DisplayOrder" value="${itemIdx + 1}" />
        </div>
    `;

    container.insertAdjacentHTML('beforeend', itemHtml);
    
    // Update itemIndexes
    itemIndexes[groupIdx] = itemIdx + 1;

    updateAddItemButton(groupIdx);
    validateForm();
}

// Remove option item
function removeItem(groupIdx, itemIdx) {
    const item = document.getElementById(`item-${groupIdx}-${itemIdx}`);
    const container = document.getElementById(`items-container-${groupIdx}`);

    if (!item || !container) return;

    item.remove();
    
    // Reindex all fields after removal
    reindexAllFields();
    
    // Check if this was the last item in the group (after reindex)
    const remainingItems = container.querySelectorAll('.am-option-item-row');
    if (remainingItems.length === 0) {
        // Find the new group index after reindex
        const group = container.closest('.am-option-group-card');
        if (group) {
            const newGroupIdx = parseInt(group.getAttribute('data-group-index'));
            removeGroup(newGroupIdx);
        }
        return;
    }
    
    updateAddItemButton(groupIdx);
    validateForm();
}

// Preview item image
function previewItemImage(groupIdx, itemIdx, input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.getElementById(`item-preview-${groupIdx}-${itemIdx}`);
            const miniUpload = input.closest('.am-mini-upload');
            if (preview) {
                preview.src = e.target.result;
                preview.style.display = 'block';
            }
            if (miniUpload) {
                miniUpload.classList.add('has-image');
            }
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Trigger item image upload (for clicking on mini-upload container)
function triggerItemImageUpload(groupIdx, itemIdx) {
    event.stopPropagation();
    const input = document.getElementById(`item-image-${groupIdx}-${itemIdx}`);
    if (input) {
        input.click();
    }
}

// Toggle popular state with animation
function togglePopular(label) {
    const checkbox = label.querySelector('input[type="checkbox"]');
    if (checkbox.checked) {
        label.classList.add('is-popular');
    } else {
        label.classList.remove('is-popular');
    }
}

// Validation Functions
function validateForm() {
    validationErrors = [];
    validateBasicFields();

    const hasOptionGroupChecked = document.querySelector('input[name="HasOptionGroup"]')?.checked;
    if (hasOptionGroupChecked) {
        validateOptionGroups();
    }

    updateValidationUI();
    updateSubmitButton();
}

function validateBasicFields() {
    const nameField = document.querySelector('input[name="Name"]');
    const priceField = document.querySelector('input[name="Price"]');
    const categoryField = document.querySelector('select[name="SelectedCategoryKey"]');

    if (!nameField?.value.trim()) addValidationError("Title is required", nameField);
    if (!priceField?.value || parseFloat(priceField.value) < 0) addValidationError("Valid price is required", priceField);
    if (!categoryField?.value) addValidationError("Category is required", categoryField);
}

function validateOptionGroups() {
    const groups = document.querySelectorAll('.am-option-group-card');
    if (groups.length === 0) {
        addValidationError("At least one option group is required");
        return;
    }

    const groupNames = [];

    groups.forEach((group, index) => {
        const groupIndex = group.getAttribute('data-group-index');
        const nameInput = group.querySelector('.group-name-input');
        const groupName = nameInput?.value.trim();

        if (!groupName) {
            addValidationError("Option name is required", nameInput);
        } else if (groupNames.includes(groupName.toLowerCase())) {
            addValidationError("Duplicate option name", nameInput);
        } else {
            groupNames.push(groupName.toLowerCase());
        }

        validateOptionItems(groupIndex, groupName || `Option ${index + 1}`);
    });
}

function validateOptionItems(groupIndex, groupName) {
    const itemsContainer = document.getElementById(`items-container-${groupIndex}`);
    if (!itemsContainer) return;
    
    const items = itemsContainer.querySelectorAll('.am-option-item-row');

    if (items.length === 0) {
        addValidationError(`${groupName}: At least one value is required`);
        return;
    }

    const itemNames = [];
    items.forEach((item) => {
        const nameInput = item.querySelector(`input[name*=".Name"]`);
        const itemName = nameInput?.value.trim();

        if (!itemName) {
            addValidationError("Value name is required", nameInput);
        } else if (itemNames.includes(itemName.toLowerCase())) {
            addValidationError("Duplicate value", nameInput);
        } else {
            itemNames.push(itemName.toLowerCase());
        }
    });
}

function addValidationError(message, element) {
    validationErrors.push({ message, element });
}

function updateValidationUI() {
    document.querySelectorAll('.has-error').forEach(el => el.classList.remove('has-error'));
    document.querySelectorAll('.field-error-message').forEach(el => el.remove());

    validationErrors.forEach(error => {
        if (error.element) {
            error.element.classList.add('has-error');
        } else {
            showErrorToast(error.message);
        }
    });
}

function updateSubmitButton() {
    const submitBtn = document.getElementById('submitBtn');
    submitBtn.disabled = validationErrors.length > 0;
}

function updateAddGroupButton() {
    const btn = document.querySelector('.am-add-group-btn');
    if (btn) btn.style.display = document.querySelectorAll('.am-option-group-card').length >= MAX_OPTION_GROUPS ? 'none' : 'inline-flex';
}

function updateAddItemButton(groupIndex) {
    const container = document.getElementById(`items-container-${groupIndex}`);
    if (!container) return;
    
    const itemCount = container.querySelectorAll('.am-option-item-row').length;
    const groupCard = document.getElementById(`group-${groupIndex}`);
    if (!groupCard) return;
    
    const addItemButton = groupCard.querySelector('.am-add-item-btn');
    
    if (addItemButton) {
        addItemButton.style.display = itemCount >= MAX_OPTION_ITEMS_PER_GROUP ? 'none' : 'inline-flex';
    }
}

// Number Input Validation helper for dynamic elements
function attachNumberValidation(input) {
    if (!input || input.__numberValidationAttached) return;
    input.__numberValidationAttached = true;

    const step = parseFloat(input.getAttribute('step')) || 1;
    const min = parseFloat(input.getAttribute('min')) || 0;
    const max = parseFloat(input.getAttribute('max')) || Infinity;
    const isDecimal = step < 1;

    // Prevent invalid characters
    input.addEventListener('keydown', function (e) {
        if (['e', 'E', '+', '-'].includes(e.key)) {
            e.preventDefault();
        }
    });

    // Validate on input and clamp to min/max
    input.addEventListener('input', function () {
        const raw = this.value;
        if (raw === '') return;

        // For decimal inputs, allow numbers and one decimal point
        let value = raw;
        if (isDecimal) {
            value = value.replace(/[^0-9.]/g, '');
            // Ensure only one decimal point
            const parts = value.split('.');
            if (parts.length > 2) {
                value = parts[0] + '.' + parts.slice(1).join('');
            }
        } else {
            // For integer inputs, remove everything except digits
            value = value.replace(/[^0-9]/g, '');
        }

        this.value = value;

        const numValue = parseFloat(value);
        if (!isNaN(max) && !isNaN(numValue) && numValue > max) {
            this.value = max.toString();
            showErrorToast(`Maximum value is ${max}`);
        }

        if (!isNaN(min) && !isNaN(numValue) && numValue < min) {
            this.value = min.toString();
            showErrorToast(`Minimum value is ${min}`);
        }
    });

    // Validate on blur (final enforce)
    input.addEventListener('blur', function () {
        const raw = this.value;
        if (raw === '') return;
        let value = parseFloat(raw);
        
        if (isNaN(value) || value < min) {
            value = min;
            this.value = isDecimal ? value.toFixed(2) : value.toString();
            showErrorToast(`Minimum value is ${min}`);
        }

        if (!isNaN(max) && value > max) {
            value = max;
            this.value = isDecimal ? value.toFixed(2) : value.toString();
            showErrorToast(`Maximum value is ${max}`);
        }

        validateForm();
    });
}

// Number input validation on page load
function setupNumberValidation() {
    const numberInputs = document.querySelectorAll('input[type="number"]');
    numberInputs.forEach(input => attachNumberValidation(input));
}

// Custom Select Logic
function setupCustomSelects() {
    const selects = document.querySelectorAll('.am-select');

    selects.forEach(select => {
        if (select.dataset.customized === '1' || select.closest('.am-custom-select-wrapper')) return;
        select.dataset.customized = '1';
        
        // Hide original select
        select.classList.add('am-select-hidden');

        // Create custom select wrapper
        const wrapper = document.createElement('div');
        wrapper.className = 'am-custom-select-wrapper';
        select.parentNode.insertBefore(wrapper, select);
        wrapper.appendChild(select);

        // Create custom select container
        const customSelect = document.createElement('div');
        customSelect.className = 'am-custom-select';
        wrapper.appendChild(customSelect);

        // Create trigger
        const trigger = document.createElement('div');
        trigger.className = 'am-custom-select-trigger';
        const selectedOption = select.options[select.selectedIndex];
        trigger.innerHTML = `<span>${selectedOption ? selectedOption.text : 'Select...'}</span><i class="fas fa-chevron-down"></i>`;
        customSelect.appendChild(trigger);

        // Create options container
        const optionsContainer = document.createElement('div');
        optionsContainer.className = 'am-custom-options';
        customSelect.appendChild(optionsContainer);

        // Populate options
        Array.from(select.options).forEach(option => {
            if (option.disabled) return; // Skip disabled placeholders

            const customOption = document.createElement('div');
            customOption.className = 'am-custom-option';
            if (option.selected) customOption.classList.add('selected');
            customOption.textContent = option.text;
            customOption.dataset.value = option.value;

            customOption.addEventListener('click', function () {
                // Update original select
                select.value = this.dataset.value;
                select.dispatchEvent(new Event('change')); // Trigger change event for validation

                // Update trigger text
                trigger.querySelector('span').textContent = this.textContent;

                // Update selected state
                optionsContainer.querySelectorAll('.am-custom-option').forEach(opt => {
                    opt.classList.remove('selected');
                });
                this.classList.add('selected');

                // Close dropdown
                customSelect.classList.remove('open');
            });

            optionsContainer.appendChild(customOption);
        });

        // Toggle dropdown
        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            // Close other dropdowns
            document.querySelectorAll('.am-custom-select').forEach(el => {
                if (el !== customSelect) {
                    el.classList.remove('open');
                    // Reset z-index of other cards
                    const otherCard = el.closest('.am-card');
                    if (otherCard) otherCard.style.zIndex = '';
                }
            });

            customSelect.classList.toggle('open');

            // Handle parent card z-index to ensure dropdown appears on top
            const card = customSelect.closest('.am-card');
            if (card) {
                if (customSelect.classList.contains('open')) {
                    card.style.zIndex = '100';
                } else {
                    card.style.zIndex = '';
                }
            }
        });
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.am-custom-select')) {
            document.querySelectorAll('.am-custom-select').forEach(el => {
                el.classList.remove('open');
                // Reset z-index
                const card = el.closest('.am-card');
                if (card) card.style.zIndex = '';
            });
        }
    });
}

// Initial setup
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('mealForm');
    const optionGroupCheckbox = document.getElementById('hasOptionGroupCheckbox');

    // Listeners
    document.querySelectorAll('input, select, textarea').forEach(el => {
        el.addEventListener('input', validateForm);
        el.addEventListener('change', validateForm);
    });

    // Price and related fields validation
    document.querySelectorAll('input[name="Price"], input[name="OfferPrice"], input[name="NumberOfServings"]').forEach(input => {
        attachNumberValidation(input);
    });

    if (optionGroupCheckbox) {
        // Handle change event - when user tries to check/uncheck
        optionGroupCheckbox.addEventListener('change', function (e) {
            const groups = document.querySelectorAll('.am-option-group-card');

            if (this.checked) {
                // User checked the box - if no groups exist, add one
                if (groups.length === 0) {
                    toggleOptionGroups(true);
                } else {
                    // Groups already exist, just show the container
                    toggleOptionGroups(true);
                }
            } else {
                // User trying to uncheck - only allow if no groups exist
                if (groups.length > 0) {
                    e.preventDefault();
                    this.checked = true;
                    showErrorToast('Please remove all option groups before disabling options.');
                    return false;
                } else {
                    toggleOptionGroups(false);
                }
            }
        });

        // Prevent unchecking via click when groups exist
        optionGroupCheckbox.addEventListener('click', function (e) {
            // If checkbox is currently checked and user clicks to uncheck
            const groups = document.querySelectorAll('.am-option-group-card');
            if (this.checked && groups.length > 0) {
                // Check if they're actually trying to uncheck (click on checked box)
                setTimeout(() => {
                    if (!this.checked) {
                        e.preventDefault();
                        this.checked = true;
                        showErrorToast('Please remove all option groups before disabling options.');
                    }
                }, 0);
            }
        });
    }

    // Initial check
    const hasOptionGroup = optionGroupCheckbox ? optionGroupCheckbox.checked : false;
    toggleOptionGroups(hasOptionGroup);

    // Initialize new features
    setupNumberValidation();
    setupCustomSelects();

    // Form submit
    form.addEventListener('submit', function (e) {
        validateForm();
        if (validationErrors.length > 0) {
            e.preventDefault();
            showErrorToast("Please fix the errors before saving.");
        }
    });
});

// Simple error toast
function showErrorToast(message) {
    // Create container if not exists
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'am-toast-container';
        document.body.appendChild(container);
    }

    // Check if this message already exists
    const existingToasts = container.querySelectorAll('.am-toast');
    for (let toast of existingToasts) {
        if (toast.textContent.includes(message)) {
            return; // Don't show duplicate toasts
        }
    }

    const toast = document.createElement('div');
    toast.className = 'am-toast am-toast-error';
    toast.innerHTML = `<div class="am-toast-content"><i class="fas fa-exclamation-circle"></i> <span>${message}</span></div>`;
    container.appendChild(toast);

    // Auto-remove after 4 seconds
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => {
            toast.remove();
            // Remove container if empty
            if (container.children.length === 0) {
                container.remove();
            }
        }, 300);
    }, 4000);
}

function toggleAddOnsSection() {
    var checkbox = document.getElementById('hasAddOnsCheckbox');
    var container = document.getElementById('addOnsContainer');
    var addOnsGrid = document.getElementById('addOnsGrid');
    
    if (checkbox.checked) {
        // Show the container when checked
        container.style.display = 'block';
    } else {
        // Before hiding, check if there are selected add-ons
        if (addOnsGrid) {
            var selectedCount = addOnsGrid.querySelectorAll('input[type="checkbox"]:checked').length;
            if (selectedCount > 0) {
                // Prevent unchecking - revert the checkbox state
                checkbox.checked = true;
                container.style.display = 'block';
                showErrorToast('Please deselect all add-ons before disabling add-ons.');
                return;
            }
        }
        // Only hide if no add-ons are selected
        container.style.display = 'none';
        clearAddOnsValidationError();
    }
}

function toggleAddonCard(card) {
    var checkbox = card.querySelector('input[type="checkbox"]');
    checkbox.checked = !checkbox.checked;
    if (checkbox.checked) {
        card.classList.add('selected');
    } else {
        card.classList.remove('selected');
    }

    // Clear validation error if at least one is selected
    validateAddOnsSelection();
}

// Validate that at least one add-on is selected when HasAddOns is checked
function validateAddOnsSelection() {
    var hasAddOnsCheckbox = document.getElementById('hasAddOnsCheckbox');
    var addOnsGrid = document.getElementById('addOnsGrid');

    if (!hasAddOnsCheckbox || !hasAddOnsCheckbox.checked) {
        clearAddOnsValidationError();
        return true;
    }

    if (!addOnsGrid) {
        return true; // No add-ons available, validation passes
    }

    var selectedAddOns = addOnsGrid.querySelectorAll('input[type="checkbox"]:checked');

    if (selectedAddOns.length === 0) {
        showAddOnsValidationError();
        return false;
    } else {
        clearAddOnsValidationError();
        return true;
    }
}

function showAddOnsValidationError() {
    var addOnsGrid = document.getElementById('addOnsGrid');
    if (addOnsGrid && !document.getElementById('addons-validation-error')) {
        addOnsGrid.classList.add('has-validation-error');
        var errorDiv = document.createElement('div');
        errorDiv.id = 'addons-validation-error';
        errorDiv.className = 'am-addons-validation-error';
        errorDiv.innerHTML = '<i class="fas fa-exclamation-circle"></i> Please select at least one add-on';
        addOnsGrid.parentNode.insertBefore(errorDiv, addOnsGrid);
    }
}

function clearAddOnsValidationError() {
    var errorDiv = document.getElementById('addons-validation-error');
    if (errorDiv) {
        errorDiv.remove();
    }
    var addOnsGrid = document.getElementById('addOnsGrid');
    if (addOnsGrid) {
        addOnsGrid.classList.remove('has-validation-error');
    }
}

// Intercept form submission to validate add-ons
document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('mealForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validateAddOnsSelection()) {
                e.preventDefault();
                e.stopPropagation();

                // Scroll to add-ons section
                var addOnsContainer = document.getElementById('addOnsContainer');
                if (addOnsContainer) {
                    addOnsContainer.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }

                showErrorToast('Please select at least one add-on when add-ons are enabled');
                return false;
            }
            return true;
        });
    }

    // Handle HasAddOns checkbox - prevent unchecking when items are selected
    var hasAddOnsCheckbox = document.getElementById('hasAddOnsCheckbox');
    if (hasAddOnsCheckbox) {
        hasAddOnsCheckbox.addEventListener('change', function (e) {
            var addOnsGrid = document.getElementById('addOnsGrid');
            
            if (this.checked) {
                // User checked the box - show validation if no items selected
                if (addOnsGrid) {
                    var selectedCount = addOnsGrid.querySelectorAll('input[type="checkbox"]:checked').length;
                    if (selectedCount === 0) {
                        showAddOnsValidationError();
                    }
                }
            } else {
                // User trying to uncheck - only allow if no add-ons are selected
                if (addOnsGrid) {
                    var selectedCount = addOnsGrid.querySelectorAll('input[type="checkbox"]:checked').length;
                    if (selectedCount > 0) {
                        e.preventDefault();
                        this.checked = true;
                        showErrorToast('Please deselect all add-ons before disabling add-ons.');
                        return false;
                    }
                }
                clearAddOnsValidationError();
            }
        });

        // Prevent unchecking via click when add-ons are selected
        hasAddOnsCheckbox.addEventListener('click', function (e) {
            var addOnsGrid = document.getElementById('addOnsGrid');
            if (this.checked && addOnsGrid) {
                var selectedCount = addOnsGrid.querySelectorAll('input[type="checkbox"]:checked').length;
                if (selectedCount > 0) {
                    setTimeout(() => {
                        if (!this.checked) {
                            e.preventDefault();
                            this.checked = true;
                            // Re-show the container
                            var container = document.getElementById('addOnsContainer');
                            if (container) container.style.display = 'block';
                            showErrorToast('Please deselect all add-ons before disabling add-ons.');
                        }
                    }, 0);
                }
            }
        });
    }
});