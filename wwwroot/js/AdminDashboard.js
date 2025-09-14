
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