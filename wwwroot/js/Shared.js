

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
        case 'danger': 
            icon = 'exclamation-octagon-fill';
            color = 'var(--danger-color)';
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

async function addToCart(courseId) {
    const btn = event.currentTarget;

    try {
        const result = await AjaxService.post('/Student/AddToCart', { CourseId:courseId });

        if (result.success && result.data.success) {
            showToast("Course added to cart!", "success");

            const newBtnHtml = `
                <button class="btn-primary-custom disabled" 
                        style="cursor: not-allowed; background-color: gray; color: white; border-color: gray;">
                    <i class="bi bi-cart-plus me-2"></i>In the Cart
                </button>
            `;
            btn.outerHTML = newBtnHtml;

            // Update the cart badge
            const badge = document.getElementById("cartBadge");
            if (badge) {
                let count = parseInt(badge.textContent) || 0;
                badge.textContent = count + 1;

                // Optional: small animation effect
                badge.classList.add("cart-bounce");
                setTimeout(() => badge.classList.remove("cart-bounce"), 300);
            }
        } else {
            console.log(result);
            
            showToast(result.error?.message || "Failed to add to cart", "danger");
        }

    } catch (err) {
        showToast("Something went wrong! Try again.", "danger");
        console.error(err);
    }
}
