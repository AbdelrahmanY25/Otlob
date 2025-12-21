const MAX_OPTION_GROUPS = 25;
const MAX_OPTION_ITEMS_PER_GROUP = 20;
let validationErrors = [];
let selectedMealImageFile = null;

// Trigger meal image file input
function triggerMealImageUpload(event) {
    // Don't trigger if clicking on buttons or the remove button
    if (event.target.closest('.am-update-image-btn') || 
        event.target.closest('.am-remove-preview-btn') ||
        event.target.closest('button')) {
        return;
    }
    document.getElementById('imageFileInput').click();
}

// Image preview function (for main meal image)
function previewImage(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('mealLogo').src = e.target.result;
            document.querySelector('.am-media-upload-area').classList.add('has-image');
        };
        reader.readAsDataURL(input.files[0]);
        
        updateSubmitButtonState();
    }
}

// Preview meal image for separate upload form
function previewMealImage(input) {
    if (input.files && input.files[0]) {
        selectedMealImageFile = input.files[0];
        
        const reader = new FileReader();
        reader.onload = function (e) {
            const uploadArea = document.querySelector('.am-media-upload-area');
            document.getElementById('mealLogo').src = e.target.result;
            uploadArea.classList.add('has-image');
            uploadArea.classList.add('has-new-image');
            
            // Show the action buttons
            const updateBtn = document.getElementById('updateImageBtn');
            const removeBtn = document.getElementById('removePreviewBtn');
            if (updateBtn) updateBtn.classList.add('visible');
            if (removeBtn) removeBtn.classList.add('visible');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Remove selected image (revert to original or default)
function removeSelectedImage(event) {
    event.stopPropagation();
    event.preventDefault();
    
    // Clear the selected file
    selectedMealImageFile = null;
    
    // Reset the file input
    const fileInput = document.getElementById('imageFileInput');
    if (fileInput) fileInput.value = '';
    
    // Reset the hidden input
    const hiddenInput = document.getElementById('hiddenImageInput');
    if (hiddenInput) hiddenInput.value = '';
    
    // Get original image from data attribute or use default
    const mealLogo = document.getElementById('mealLogo');
    const uploadArea = document.querySelector('.am-media-upload-area');
    const originalSrc = mealLogo.getAttribute('data-original-src');
    
    // Check if there was an original image
    if (originalSrc && !originalSrc.includes('Default.jpg')) {
        mealLogo.src = originalSrc;
        uploadArea.classList.add('has-image');
        uploadArea.classList.remove('has-new-image');
    } else {
        mealLogo.src = '/images/Default.jpg';
        uploadArea.classList.remove('has-image');
        uploadArea.classList.remove('has-new-image');
    }
    
    // Hide the action buttons
    const updateBtn = document.getElementById('updateImageBtn');
    const removeBtn = document.getElementById('removePreviewBtn');
    if (updateBtn) updateBtn.classList.remove('visible');
    if (removeBtn) removeBtn.classList.remove('visible');
}

// Submit image update form
function submitImageUpdate(event) {
    event.stopPropagation();
    event.preventDefault();
    
    if (!selectedMealImageFile) {
        showErrorToast('Please select an image first');
        return;
    }
    
    // Create a new DataTransfer to set the file
    const hiddenInput = document.getElementById('hiddenImageInput');
    const dataTransfer = new DataTransfer();
    dataTransfer.items.add(selectedMealImageFile);
    hiddenInput.files = dataTransfer.files;
    
    // Submit the hidden form
    const form = document.getElementById('imageUpdateForm');
    if (form && hiddenInput.files.length > 0) {
        form.submit();
    } else {
        showErrorToast('Failed to upload image. Please try again.');
    }
}

// Toggle Option Groups visibility
function toggleOptionGroups(show) {
    const container = document.getElementById('optionGroupsContainer');
    container.style.display = show ? 'block' : 'none';
    
    if (show) {
        if (document.querySelectorAll('.am-option-group-card').length === 0) {
            addOptionGroup();
        }
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
                <div class="am-option-items-container" id="items-container-${currentIndex}"></div>
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
    updateSubmitButtonState();
}

// Remove option group
function removeGroup(groupIdx) {
    const group = document.getElementById(`group-${groupIdx}`);
    if (group) {
        group.remove();
    }

    // Reindex all fields after removal
    reindexAllFields();
    
    syncCheckboxWithGroups();
    updateAddGroupButton();
    validateForm();
    updateSubmitButtonState();
}

// Add option item to a group
function addOptionItem(groupIdx) {
    const container = document.getElementById(`items-container-${groupIdx}`);
    if (!container) return;
    
    const currentItemCount = container.querySelectorAll('.am-option-item-row').length;
    if (currentItemCount >= MAX_OPTION_ITEMS_PER_GROUP) return;
    
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
    updateSubmitButtonState();
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
    updateSubmitButtonState();
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
            
            // Clear the ExistingImage hidden input since we're uploading a new image
            const itemRow = input.closest('.am-option-item-row');
            const existingImageInput = itemRow?.querySelector('input[name*=".ExistingImage"]');
            if (existingImageInput) {
                existingImageInput.value = '';
            }
        };
        reader.readAsDataURL(input.files[0]);
        
        updateSubmitButtonState();
    }
}

// Trigger item image upload
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

// Attach number validation to an input field
function attachNumberValidation(inputElement) {
    if (!inputElement || inputElement.__numberValidationAttached) return;
    inputElement.__numberValidationAttached = true;
    
    const min = parseInt(inputElement.getAttribute('min')) || 0;
    const max = parseInt(inputElement.getAttribute('max')) || Infinity;

    inputElement.addEventListener('keydown', function (e) {
        if (['e', 'E', '+', '-'].includes(e.key)) {
            e.preventDefault();
        }
    });

    inputElement.addEventListener('input', function (e) {
        let value = this.value;
        value = value.replace(/[^0-9]/g, '');
        let numValue = parseInt(value) || 0;
        
        if (numValue > max) {
            numValue = max;
            showErrorToast(`Maximum value is ${max}`);
        }
        
        this.value = numValue;
        validateForm();
    });

    inputElement.addEventListener('blur', function () {
        let numValue = parseInt(this.value) || min;
        
        if (numValue < min) {
            numValue = min;
            showErrorToast(`Minimum value is ${min}`);
        }
        
        if (numValue > max) {
            numValue = max;
            showErrorToast(`Maximum value is ${max}`);
        }
        
        this.value = numValue;
        validateForm();
    });

    inputElement.addEventListener('paste', function (e) {
        e.preventDefault();
        const pastedText = (e.clipboardData || window.clipboardData).getData('text');
        const numValue = parseInt(pastedText.replace(/[^0-9]/g, '')) || min;
        
        if (numValue >= min && numValue <= max) {
            this.value = numValue;
        } else {
            showErrorToast(`Value must be between ${min} and ${max}`);
        }
        validateForm();
    });
}

// Custom Select Logic
function setupCustomSelects() {
    const selects = document.querySelectorAll('.am-select');

    selects.forEach(select => {
        if (select.dataset.customized === '1' || select.closest('.am-custom-select-wrapper')) return;
        select.dataset.customized = '1';

        select.classList.add('am-select-hidden');

        const wrapper = document.createElement('div');
        wrapper.className = 'am-custom-select-wrapper';
        select.parentNode.insertBefore(wrapper, select);
        wrapper.appendChild(select);

        const customSelect = document.createElement('div');
        customSelect.className = 'am-custom-select';
        wrapper.appendChild(customSelect);

        const trigger = document.createElement('div');
        trigger.className = 'am-custom-select-trigger';
        const selectedOption = select.options[select.selectedIndex];
        trigger.innerHTML = `<span>${selectedOption ? selectedOption.text : 'Select...'}</span><i class="fas fa-chevron-down"></i>`;
        customSelect.appendChild(trigger);

        const optionsContainer = document.createElement('div');
        optionsContainer.className = 'am-custom-options';
        customSelect.appendChild(optionsContainer);

        Array.from(select.options).forEach(option => {
            if (option.disabled) return;

            const customOption = document.createElement('div');
            customOption.className = 'am-custom-option';
            customOption.textContent = option.text;
            customOption.dataset.value = option.value;
            if (option.selected) customOption.classList.add('selected');

            customOption.addEventListener('click', function () {
                select.value = this.dataset.value;
                select.dispatchEvent(new Event('change'));

                trigger.querySelector('span').textContent = this.textContent;

                optionsContainer.querySelectorAll('.am-custom-option').forEach(opt => opt.classList.remove('selected'));
                this.classList.add('selected');

                customSelect.classList.remove('open');
            });

            optionsContainer.appendChild(customOption);
        });

        select.addEventListener('change', function () {
            const sel = select.options[select.selectedIndex];
            trigger.querySelector('span').textContent = sel ? sel.text : 'Select...';
            optionsContainer.querySelectorAll('.am-custom-option').forEach(opt => {
                opt.classList.toggle('selected', opt.dataset.value === select.value);
            });
        });

        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            document.querySelectorAll('.am-custom-select').forEach(el => {
                if (el !== customSelect) {
                    el.classList.remove('open');
                    const otherCard = el.closest('.am-card');
                    if (otherCard) otherCard.style.zIndex = '';
                }
            });

            customSelect.classList.toggle('open');

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

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.am-custom-select')) {
            document.querySelectorAll('.am-custom-select').forEach(el => {
                el.classList.remove('open');
                const card = el.closest('.am-card');
                if (card) card.style.zIndex = '';
            });
        }
    });
}

// Initial setup
document.addEventListener('DOMContentLoaded', function () {
    captureInitialFormState();
    
    // Store original meal image source for reset functionality
    const mealLogo = document.getElementById('mealLogo');
    if (mealLogo) {
        mealLogo.setAttribute('data-original-src', mealLogo.src);
    }
    
    document.querySelectorAll('input[name="Price"], input[name="NumberOfServings"]').forEach(input => {
        attachNumberValidation(input);
    });

    setupCustomSelects();
    
    validateForm();

    const form = document.getElementById('mealForm');
    form.addEventListener('input', function () {
        validateForm();
    });
    
    form.addEventListener('change', function () {
        validateForm();
    });

    const hasCheckbox = document.getElementById('hasOptionGroupCheckbox');
    if (hasCheckbox) {
        hasCheckbox.addEventListener('change', function (e) {
            const groups = document.querySelectorAll('.am-option-group-card');
            
            if (this.checked) {
                if (groups.length === 0) {
                    toggleOptionGroups(true);
                } else {
                    toggleOptionGroups(true);
                }
            } else {
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

        hasCheckbox.addEventListener('click', function (e) {
            const groups = document.querySelectorAll('.am-option-group-card');
            if (this.checked && groups.length > 0) {
                setTimeout(() => {
                    if (!this.checked) {
                        e.preventDefault();
                        this.checked = true;
                        showErrorToast('Please remove all option groups before disabling options.');
                    }
                }, 0);
            }
        });

        syncCheckboxWithGroups();
    }
});

// Synchronize checkbox state with the existence of option groups
function syncCheckboxWithGroups() {
    const hasCheckbox = document.getElementById('hasOptionGroupCheckbox');
    const groups = document.querySelectorAll('.am-option-group-card');
    
    if (hasCheckbox) {
        hasCheckbox.checked = groups.length > 0;
        const container = document.getElementById('optionGroupsContainer');
        container.style.display = groups.length > 0 ? 'block' : 'none';
    }
}

// Update the "Add Group" button visibility
function updateAddGroupButton() {
    const groupCount = document.querySelectorAll('.am-option-group-card').length;
    const addGroupButton = document.querySelector('.am-add-group-btn');
    
    if (addGroupButton) {
        addGroupButton.style.display = groupCount >= MAX_OPTION_GROUPS ? 'none' : 'block';
    }
}

// Update the "Add Item" button visibility within a group
function updateAddItemButton(groupIdx) {
    const container = document.getElementById(`items-container-${groupIdx}`);
    if (!container) return;
    
    const itemCount = container.querySelectorAll('.am-option-item-row').length;
    const groupCard = document.getElementById(`group-${groupIdx}`);
    if (!groupCard) return;
    
    const addItemButton = groupCard.querySelector('.am-add-item-btn');
    
    if (addItemButton) {
        addItemButton.style.display = itemCount >= MAX_OPTION_ITEMS_PER_GROUP ? 'none' : 'inline-flex';
    }
}

// Validate the form
function validateForm() {
    validationErrors = [];
    
    validateBasicFields();
    
    const hasOptionGroupChecked = document.getElementById('hasOptionGroupCheckbox')?.checked;
    if (hasOptionGroupChecked) {
        validateOptionGroups();
    }
    
    displayValidationErrors();
    updateSubmitButtonState();
}

// Validate basic form fields
function validateBasicFields() {
    const nameField = document.querySelector('input[name="Name"]');
    const priceField = document.querySelector('input[name="Price"]');
    const categoryField = document.querySelector('select[name="SelectedCategoryKey"]');
    
    if (!nameField?.value.trim()) addValidationError("Title is required", nameField);
    if (!priceField?.value || parseFloat(priceField.value) < 0) addValidationError("Valid price is required", priceField);
    if (!categoryField?.value) addValidationError("Category is required", categoryField);
}

// Validate option groups
function validateOptionGroups() {
    const groups = document.querySelectorAll('.am-option-group-card');
    
    if (groups.length === 0) {
        addValidationError('At least one option group is required when options are enabled.');
        return;
    }

    const groupNames = [];

    groups.forEach((group, index) => {
        const groupIndex = group.getAttribute('data-group-index');
        
        const nameInput = group.querySelector('.group-name-input');
        const groupName = nameInput?.value.trim().toLowerCase();
        
        if (!groupName) {
            addValidationError(`Option group ${index + 1}: Name is required.`, nameInput);
        } else if (groupNames.includes(groupName)) {
            addValidationError(`Duplicate group name "${nameInput.value}".`, nameInput);
        } else {
            groupNames.push(groupName);
            nameInput?.classList.remove('am-input-error');
        }

        validateOptionItems(groupIndex, nameInput?.value || `Group ${index + 1}`);
    });
}

// Validate option items within a group
function validateOptionItems(groupIndex, groupName) {
    const itemsContainer = document.getElementById(`items-container-${groupIndex}`);
    if (!itemsContainer) return;

    const items = itemsContainer.querySelectorAll('.am-option-item-row');
    
    if (items.length === 0) {
        addValidationError(`${groupName}: At least one option item is required.`);
        return;
    }

    const itemNames = [];

    items.forEach((item, index) => {
        const nameInput = item.querySelector(`input[name*=".Name"]`);
        const itemName = nameInput?.value.trim().toLowerCase();
        
        if (!itemName) {
            addValidationError(`${groupName}: Option item ${index + 1} name is required.`, nameInput);
        } else if (itemNames.includes(itemName)) {
            addValidationError(`In ${groupName}: Duplicate item "${nameInput.value}".`, nameInput);
        } else {
            itemNames.push(itemName);
            nameInput?.classList.remove('am-input-error');
        }

        const priceInput = item.querySelector(`input[name*=".Price"]`);
        const priceValue = parseFloat(priceInput?.value) || 0;
        
        if (priceValue < 0) {
            addValidationError(`In ${groupName}: Item ${index + 1} price must be >= 0.`, priceInput);
        } else {
            priceInput?.classList.remove('am-input-error');
        }
    });
}

// Add validation error
function addValidationError(message, input = null) {
    validationErrors.push(message);
    if (input) {
        input.classList.add('am-input-error');
    }
}

// Display validation errors
function displayValidationErrors() {
    const errorContainer = document.querySelector('.am-validation-errors');
    errorContainer.innerHTML = '';

    if (validationErrors.length === 0) {
        errorContainer.style.display = 'none';
        return;
    }

    const errorList = document.createElement('ul');
    errorList.classList.add('am-error-list');

    validationErrors.forEach((error) => {
        const listItem = document.createElement('li');
        listItem.textContent = error;
        errorList.appendChild(listItem);
    });

    errorContainer.appendChild(errorList);
    errorContainer.style.display = 'block';
}

// Track initial form state
let initialFormState = null;

// Capture initial form state
function captureInitialFormState() {
    const formData = new FormData(document.getElementById('mealForm'));
    initialFormState = {};
    
    for (let [key, value] of formData.entries()) {
        if (key.includes('ImageRequest.Image') || key === 'image') continue;
        initialFormState[key] = value;
    }
}

// Update submit button state
function updateSubmitButtonState() {
    const submitButton = document.getElementById('submitBtn');
    if (validationErrors.length > 0) {
        submitButton.disabled = true;
        submitButton.classList.add('am-btn-disabled');
        submitButton.title = 'Please fix validation errors';
    } else {
        submitButton.disabled = false;
        submitButton.classList.remove('am-btn-disabled');
        submitButton.title = 'Save changes';
    }
}

// Show error toast
function showErrorToast(message) {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'am-toast-container';
        document.body.appendChild(container);
    }

    const existingToasts = container.querySelectorAll('.am-toast');
    for (let toast of existingToasts) {
        if (toast.textContent.includes(message)) {
            return;
        }
    }

    const toast = document.createElement('div');
    toast.className = 'am-toast am-toast-error';
    toast.innerHTML = `<div class="am-toast-content"><i class="fas fa-exclamation-circle"></i> <span>${message}</span></div>`;
    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => {
            toast.remove();
            if (container.children.length === 0) {
                container.remove();
            }
        }, 300);
    }, 4000);
}