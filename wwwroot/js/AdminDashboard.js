
// initaialize tooltips
const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
});

// Hover functionality for sidebar
const sidebar = document.getElementById('sidebar');
const mainContent = document.getElementById('mainContent');

sidebar.addEventListener('mouseenter', function () {
    sidebar.classList.remove('collapsed');
    mainContent.classList.remove('expanded');
});

sidebar.addEventListener('mouseleave', function () {
    sidebar.classList.add('collapsed');
    mainContent.classList.add('expanded');
});

// Manual toggle (optional, keeps the button functional)
document.getElementById('sidebarToggle').addEventListener('click', function () {
    sidebar.classList.toggle('collapsed');
    mainContent.classList.toggle('expanded');
});


// Toast notification function
function showToast(message, type = 'success') {
    const toastContainer = document.querySelector('.toast-container');
    const toastId = 'toast-' + Date.now();

    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0" id="${toastId}" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi ${type === 'success' ? 'bi-check-circle-fill' : type === 'danger' ? 'bi-exclamation-circle-fill' : 'bi-info-circle-fill'} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    // Remove toast from DOM after it hides
    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}


// Debounce utility
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

// Function to load courses via AJAX
function loadCourses(page = 1) {
    const status = $('#statusFilter').val() || null;
    const categoryId = $('#categoryFilter').val() || null;
    const search = $('#courseSearch').val() || "";
    const pageSize = 5;  // Match server default

    console.log('Loading courses with params:', { page, pageSize, status, categoryId, search });  // Added for debug

    AjaxService.get('/Admin/ManageCourse', { page, pageSize, status, categoryId, search })
        .then(result => {
            console.log('AJAX result:', result);
            console.log('AJAX result:', result.data);
            if (result.success) {
                $('#courseTableContainer').html(result.data);

                // Re-initialize tooltips for new elements
                const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });
            } else {
                showToast('Failed to load courses.', 'danger');
            }
        })
        .catch(err => {
            console.error('AJAX error:', err);
            showToast('Error loading courses: ' + err.message, 'danger');
        });
}

// Event listeners for filters (auto-update on change)
$(document).ready(function () {
    $('#statusFilter').change(() => loadCourses(1));
    $('#categoryFilter').change(() => loadCourses(1));
    $('#courseSearch').on('input', debounce(() => loadCourses(1), 300));
});




//function reviewCourse(courseId) {

//    const modal = new bootstrap.Modal(document.getElementById('courseReviewModal'));
//    modal.show();
//}

//function requestChanges(courseId) {
//    alert(`Request changes sent for course ${courseId}`);
//}

//function showVideoModal(moduleName, videoId) {
//    document.getElementById('videoTitle').textContent = moduleName + ' - Video';
//    // For YouTube videos, use the video ID in the iframe
//    document.getElementById('videoFrame').src = `https://www.youtube.com/embed/${videoId}`;
//    const modal = new bootstrap.Modal(document.getElementById('videoModal'));
//    modal.show();
//}

//function showPdfModal(moduleName, pdfFile) {
//    document.getElementById('pdfTitle').textContent = moduleName + ' - PDF';
//    // Replace with your actual PDF URL
//    const pdfUrl = `/pdfs/${pdfFile}`; // Change this path to your PDF location
//    document.getElementById('pdfFrame').src = pdfUrl;
//    document.getElementById('pdfDownload').href = pdfUrl;
//    const modal = new bootstrap.Modal(document.getElementById('pdfModal'));
//    modal.show();
//}

document.querySelectorAll('[data-page]').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelectorAll('.sidebar-nav .nav-link').forEach(nav => nav.classList.remove('active'));
        this.classList.add('active');
    });
});


function reviewCourse(courseId) {
    const params = { courseId: courseId };
    const url = '/Admin/ReviewCourse';

    AjaxService.get(url, params)
        .then(result => {
            console.log('AJAX result:', result);

            if (result.success) {
                const html = result.data; // This is the partial view HTML
                document.getElementById('reviewmodel-body').innerHTML = html;

                const modalEl = document.getElementById('courseReviewModal');
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                //const form = document.getElementById('addModuleForm');
                //form.addEventListener('submit', function (e) {
                //    e.preventDefault();
                //    submitAddModuleForm(this, modal);
                //});
            } else {
                console.error('Unexpected response:', result);
            }
        })
        .catch(err => {
            console.error("Error loading module form:", err);
            // Global handler will also trigger if set
        });
}


function handleClick(ItemName, ItemType, Url) {
    if (ItemType === "Video") {
        // Set video title
        document.getElementById('videoTitle').textContent = "🎬 " + ItemName;
        // Set iframe src
        document.getElementById('videoFrame').src = Url;

        // Show video modal
        const videoModal = new bootstrap.Modal(document.getElementById('videoModal'));
        videoModal.show();

        // Clear src when modal closes to stop playback
        const modalEl = document.getElementById('videoModal');
        modalEl.addEventListener('hidden.bs.modal', function () {
            document.getElementById('videoFrame').src = "";
        }, { once: true });

    } else if (ItemType === "Document") {
        // Set iframe src
        document.getElementById('pdfFrame').src = Url;
        // Set download link
        document.getElementById('pdfDownload').href = Url;

        // Show PDF modal
        const pdfModal = new bootstrap.Modal(document.getElementById('pdfModal'));
        pdfModal.show();
    } else {
        console.warn("Unknown content type:", ItemType);
    }
}

//showToast

function openRejectModal(courseId) {
    const params = { CourseId: courseId};
    const url = '/Admin/RejectCourse';

    AjaxService.get(url, params)
        .then(result => {
            if (result.success) {
                const html = result.data; // This is the partial view HTML
                document.getElementById('course-reject-body').innerHTML = html;

                const modalEl = document.getElementById('courseRejectModal');
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                const confirmBtn = document.getElementById('confirmRejectBtn');
                confirmBtn.onclick = () => submitRejectForm(courseId, modal);
            } else {
                showToast(result.error.message || 'Failed to load form', 'danger');
                console.warn('Server error:', result.error);
            }
        })
        .catch(err => {
            console.error("Error loading module form:", err);
            showToast('Connection failed. Please try again.', 'danger');
        });
}

function submitRejectForm(courseId, rejectModal) {
    const form = document.getElementById('rejectCourseForm');
    if (!form) return;

    const formData = new FormData(form);
    // add courseId if you prefer to send it explicitly (already in hidden field)
    // formData.append('courseId', courseId);

    AjaxService.post('/Admin/RejectCourse', formData)
        .then(r => {
            console.log('Full response:', r);
            if (r.data && r.data.success === true) {
                // 1. close reject modal
                rejectModal.hide();

                // 2. close review modal if open
                const reviewModalEl = document.getElementById('courseReviewModal');
                const reviewInstance = bootstrap.Modal.getInstance(reviewModalEl);
                if (reviewInstance) reviewInstance.hide();

                // 3. update / remove table row
                updateCourseRowAfterRejectAccept(courseId, "reject");
                showToast('Rejected Successfully');
            } else {
                showToast(r.error.message || 'Rejection failed.', 'danger');
            }
        })
        .catch(err => {
            console.error(err);
            showToast('Network error.');
        });
}

function approveCourse(courseId) {
    const url = '/Admin/ChangeCourseStatus';
    const data = {
        courseId: courseId,  
        status: "approve"      
    };


    AjaxService.post(url, data)
        .then(r => {
            console.log('Full response:', r);
            if (r.data && r.data.success === true) {

                // 2. close review modal if open
                const reviewModalEl = document.getElementById('courseReviewModal');
                const reviewInstance = bootstrap.Modal.getInstance(reviewModalEl);
                if (reviewInstance) reviewInstance.hide();

                // 3. update / remove table row
                updateCourseRowAfterRejectAccept(courseId, "accept");
                showToast('Accepted Successfully');
            } else {
                showToast(r.error.message || 'Acceptance failed.', 'danger');
            }
        })
        .catch(err => {
            console.error(err);
            showToast('Network error.');
        });
}

function rollbackToPending(courseId) {
    const url = '/Admin/ChangeCourseStatus';
    const data = {
        courseId: courseId,
        status: "pending"
    };

    AjaxService.post(url, data)
        .then(r => {
            console.log('Full response:', r);
            if (r.data && r.data.success === true) {
                // 2. close review modal if open
                const reviewModalEl = document.getElementById('courseReviewModal');
                const reviewInstance = bootstrap.Modal.getInstance(reviewModalEl);
                if (reviewInstance) reviewInstance.hide();

                // 3. update row to Pending + show Approve/Reject buttons
                updateCourseRowToPending(courseId);
                showToast('Rolled back to Pending', 'info');
            } else {
                showToast(r.error?.message || 'Rollback failed.', 'danger');
            }
        })
        .catch(err => {
            console.error(err);
            showToast('Network error.', 'danger');
        });
}

function updateCourseRowAfterRejectAccept(courseId, action) {
    const row = document.querySelector(`tr[data-course-id="${courseId}"]`);
    if (!row) return;

    const statusFilter = document.getElementById('statusFilter')?.value ?? '';
    const badge = row.querySelector('.status-badge');
    const actionButtons = row.querySelector('.btn-group');

    if (statusFilter && statusFilter !== '' && statusFilter !== 'All') {
        row.remove();
    } else {
        if (badge) {
            const newStatus = action === "accept" ? "Approved" : "Rejected";
            const statusClass = action === "accept" ? "status-approved" : "status-rejected";

            badge.textContent = newStatus;
            badge.className = `status-badge ${statusClass}`;
        }

        row.querySelectorAll('.btn-success-custom, .btn-danger-custom').forEach(btn => {
            disposeTooltips(btn);
            btn.remove();
        });

        if (!actionButtons.querySelector('.btn-rollback')) {
            const rollbackBtn = document.createElement('button');
            rollbackBtn.type = 'button';
            rollbackBtn.className = 'btn btn-sm btn-warning-custom';
            rollbackBtn.dataset.bsToggle = 'tooltip';
            rollbackBtn.title = 'Rollback to Pending';
            rollbackBtn.innerHTML = '<i class="bi bi-arrow-counterclockwise"></i>';
            rollbackBtn.onclick = () => rollbackToPending(courseId);

            actionButtons.appendChild(rollbackBtn);

            new bootstrap.Tooltip(rollbackBtn);
        }
    }
    refreshPaginationInfo();
}

function updateCourseRowToPending(courseId) {
    const row = document.querySelector(`tr[data-course-id="${courseId}"]`);
    if (!row) return;

    const badge = row.querySelector('.status-badge');
    const btnGroup = row.querySelector('.btn-group');

    // Update badge
    if (badge) {
        badge.textContent = 'Pending';
        badge.className = 'status-badge status-pending';
    }

    // Remove rollback button
    const rollbackBtn = btnGroup.querySelector('.btn-rollback, .btn-warning-custom');
    if (rollbackBtn) {
        disposeTooltips(rollbackBtn);
        rollbackBtn.remove();
    }

    // Add Approve button
    const approveBtn = document.createElement('button');
    approveBtn.type = 'button';
    approveBtn.className = 'btn btn-sm btn-success-custom';
    approveBtn.dataset.bsToggle = 'tooltip';
    approveBtn.title = 'Approve';
    approveBtn.innerHTML = '<i class="bi bi-check-lg"></i>';
    approveBtn.onclick = () => approveCourse(courseId);
    btnGroup.appendChild(approveBtn);

    // Add Reject button
    const rejectBtn = document.createElement('button');
    rejectBtn.type = 'button';
    rejectBtn.className = 'btn btn-sm btn-danger-custom';
    rejectBtn.dataset.bsToggle = 'tooltip';
    rejectBtn.title = 'Reject with Notes';
    rejectBtn.innerHTML = '<i class="bi bi-x-lg"></i>';
    rejectBtn.onclick = () => openRejectModal(courseId);
    btnGroup.appendChild(rejectBtn);

    // Re-init tooltips
    new bootstrap.Tooltip(approveBtn);
    new bootstrap.Tooltip(rejectBtn);

    // Refresh pagination
    refreshPaginationInfo();
}

function refreshPaginationInfo() {
    const info = document.querySelector('.pagination span.text-muted');
    if (!info) return;

    const total = parseInt(info.textContent.match(/of (\d+)/)?.[1] ?? '0', 10);
    const visible = document.querySelectorAll('#coursesTableBody tr').length;

    const start = visible > 0 ? 1 : 0;
    info.innerHTML = `Showing ${start} to ${visible} of ${total} results`;
}

function disposeTooltips(element) {
    const tooltip = bootstrap.Tooltip.getInstance(element);
    if (tooltip) tooltip.dispose();
}



