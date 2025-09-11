let editMode = false;
let currentModuleId = null;

// Toggle edit mode
function toggleEditMode() {
    editMode = !editMode;
    const body = document.body;
    const button = document.getElementById('toggleEditMode');

    if (editMode) {
        body.classList.add('edit-mode');
        button.innerHTML = '<i class="fas fa-eye me-2"></i>Preview Mode';
        button.classList.add('edit-mode-active');

        // Show add content buttons for expanded modules
        document.querySelectorAll('.module-content.show').forEach(content => {
            const addButton = content.querySelector('.text-center');
            if (addButton) addButton.style.display = 'block';
        });
    } else {
        body.classList.remove('edit-mode');
        button.innerHTML = '<i class="fas fa-edit me-2"></i>Edit Course';
        button.classList.remove('edit-mode-active');

        // Hide add content buttons
        document.querySelectorAll('.text-center').forEach(button => {
            button.style.display = 'none';
        });
    }
}

// Toggle module content
function toggleModule(moduleId) {
    const content = document.getElementById(`content-${moduleId}`);
    const toggle = document.getElementById(`toggle-${moduleId}`);
    const addButton = content.querySelector('.text-center');

    if (content.classList.contains('show')) {
        content.classList.remove('show');
        toggle.classList.remove('expanded');
        if (addButton) addButton.style.display = 'none';
    } else {
        content.classList.add('show');
        toggle.classList.add('expanded');
        if (addButton && editMode) addButton.style.display = 'block';
    }
}



// Show add item modal
function showAddItemModal(moduleId) {
    currentModuleId = moduleId;
    const modal = new bootstrap.Modal(document.getElementById('addItemModal'));
    modal.show();
}

// Toggle content fields based on type
function toggleContentFields() {
    const type = document.querySelector('select[name="type"]').value;
    const fileField = document.getElementById('fileField');
    const linkField = document.getElementById('linkField');

    fileField.style.display = (type === 'video' || type === 'document') ? 'block' : 'none';
    linkField.style.display = (type === 'link') ? 'block' : 'none';
}


function openAddModuleModal(courseId, moduleId = 0) {
    const url = `/Course/CreateorEditModule?CourseId=${courseId}&ModuleId=${moduleId}`;

    fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' }})   
        .then(response => {
            if (!response.ok) {
                throw new Error(`Server returned ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            document.getElementById('module-form-body').innerHTML = html;

            const modalEl = document.getElementById('module-form-model');
            const modal = new bootstrap.Modal(modalEl);
            modal.show();

            const form = document.getElementById('addModuleForm');
            form.addEventListener('submit', function (e) {
                e.preventDefault();
                submitAddModuleForm(this, modal);
            });
        })
        .catch(err => console.error("Error loading module form:", err));
}


// Submit Add Module form (POST via fetch)
function submitAddModuleForm(form, modal) {
    const formData = new FormData(form);

    const moduleId = parseInt(form.querySelector('[name="Id"]').value);

    fetch(form.action, {
        method: "POST",
        body: formData,
        headers: { "X-Requested-With": "XMLHttpRequest" }
    })
        .then(response => response.text())
        .then(html => {
            if (html.includes("addModuleForm")) {
                // ❌ Validation errors → reload form with messages
                document.getElementById("module-form-body").innerHTML = html;

                // Rebind submit handler
                const newForm = document.getElementById("addModuleForm");
                newForm.addEventListener("submit", function (e) {
                    e.preventDefault();
                    submitAddModuleForm(newForm, modal);
                });
            } else {

                if (moduleId === 0) {
                    // Add new module
                    document.getElementById("modulesList").insertAdjacentHTML("beforeend", html);
                } else {
                    // Replace updated module
                    const moduleEl = document.getElementById(`module-${moduleId}`);
                    if (moduleEl) moduleEl.outerHTML = html;
                }

                // Hide modal
                modal.hide();
                document.getElementById("module-form-body").innerHTML = "";

                // Hide empty-state if it was showing
                const emptyState = document.querySelector(".empty-state");
                if (emptyState) emptyState.style.display = "none";
            }
        })
        .catch(err => {
            console.error("Error submitting module form:", err);
            alert("Something went wrong while creating the module.");
        });
}

// Initialize edit mode toggle
document.getElementById('toggleEditMode').addEventListener('click', toggleEditMode);



//// Initialize the page
//document.addEventListener('DOMContentLoaded', function () {
//    // Expand first module by default
//    toggleModule(1);

//    // Set up modal cleanup
//    document.querySelectorAll('.modal').forEach(modal => {
//        modal.addEventListener('hidden.bs.modal', function () {
//            const forms = this.querySelectorAll('form');
//            forms.forEach(form => form.reset());
//            toggleContentFields();
//        });
//    });
//});