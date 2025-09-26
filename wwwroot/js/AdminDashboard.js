
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


function showReviewCourseModal() {
    const url = `/Admin/ReviewCourse`;

    fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Server returned ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            document.getElementById('review-body').innerHTML = html;

            const modalEl = document.getElementById('courseReviewModal');
            const modal = new bootstrap.Modal(modalEl);
            modal.show();

        })
        .catch(err => {
            console.error("Error loading module form:", err);
            showToast('Failed to load course review form.', 'danger');
        });
}

