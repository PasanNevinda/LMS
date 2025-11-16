

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
    const categoryId = $('#categoryFilter').val() || null;
    const search = $('#courseSearch').val() || "";
    const pageSize = 5;  


    AjaxService.get('/Student/BrowseCourse', { page, pageSize, categoryId, search })
        .then(result => {
            console.log('AJAX result:', result);
            console.log('AJAX result:', result.data);
            if (result.success) {
                $('body').html(result.data);
                
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
    $('#courseSearch').on('input', debounce(() => loadCourses(1), 1000));
});


function showToast(message, type = 'success') {
    const toastContainer = document.querySelector('.toast-container');

    let icon, color;

    switch (type) {
        case 'success':
            icon = 'check-circle-fill';
            color = 'var(--success-color)';
            break;
        case 'info':
            icon = 'info-circle-fill';
            color = 'var(--primary-color)';
            break;
        case 'warning':
            icon = 'exclamation-triangle-fill';
            color = 'var(--warning-color)';
            break;
        case 'danger': // 🔥 Added danger
            icon = 'exclamation-octagon-fill';
            color = 'var(--danger-color)'; // define: --danger-color: #dc3545;
            break;
        default:
            icon = 'info-circle-fill';
            color = 'var(--primary-color)';
    }

    const toastHTML = `
        <div class="toast show" role="alert" style="border-left-color: ${color};">
            <div class="toast-body d-flex align-items-center gap-2">
                <i class="bi bi-${icon}" style="color: ${color}; font-size: 20px;"></i>
                <span>${message}</span>
                <button type="button" class="btn-close ms-auto" onclick="this.parentElement.parentElement.remove()"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHTML);

    setTimeout(() => {
        const toasts = toastContainer.querySelectorAll('.toast');
        if (toasts.length > 0) {
            toasts[0].remove();
        }
    }, 3000);
}
