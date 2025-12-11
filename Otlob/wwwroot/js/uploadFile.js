function setupDragDrop(uploadAreaId, fileInputId, filePreviewId, fileNameId, removeFileId) {
    const uploadArea = document.getElementById(uploadAreaId);
    const fileInput = document.getElementById(fileInputId);
    const filePreview = document.getElementById(filePreviewId);
    const fileName = document.getElementById(fileNameId);
    const removeFile = document.getElementById(removeFileId);

    // Click to browse
    uploadArea.addEventListener('click', () => {
        fileInput.click();
    });

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    // Highlight drop area when item is dragged over it
    ['dragenter', 'dragover'].forEach(eventName => {
        uploadArea.addEventListener(eventName, () => {
            uploadArea.classList.add('drag-over');
        }, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, () => {
            uploadArea.classList.remove('drag-over');
        }, false);
    });

    // Handle dropped files
    uploadArea.addEventListener('drop', (e) => {
        const dt = e.dataTransfer;
        const files = dt.files;
        handleFiles(files);
    }, false);

    // Handle selected files
    fileInput.addEventListener('change', (e) => {
        handleFiles(e.target.files);
    });

    function handleFiles(files) {
        if (files.length > 0) {
            const file = files[0];
            fileName.textContent = file.name;
            filePreview.classList.add('show');
        }
    }

    // Remove file
    removeFile.addEventListener('click', (e) => {
        e.stopPropagation();
        fileInput.value = '';
        filePreview.classList.remove('show');
    });
}

setupDragDrop('uploadArea1', 'fileInput1', 'filePreview1', 'fileName1', 'removeFile1');
setupDragDrop('uploadArea2', 'fileInput2', 'filePreview2', 'fileName2', 'removeFile2');

// Form submission
document.getElementById('uploadForm').addEventListener('submit', (e) => {
    e.preventDefault();
    // Handle form submission here
    alert('Form submitted! (In production, this would send data to your server)');
});