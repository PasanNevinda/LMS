


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
    const params = { CourseId: courseId, ModuleId: moduleId };
    const url = '/Course/CreateorEditModule';

    AjaxService.get(url, params)
        .then(result => {
            if (result.success) {
                const html = result.data; // This is the partial view HTML
                document.getElementById('module-form-body').innerHTML = html;

                const modalEl = document.getElementById('module-form-model');
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                const form = document.getElementById('addModuleForm');
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitAddModuleForm(this, modal);
                });
            } else {
                console.error('Unexpected response:', result);
            }
        })
        .catch(err => {
            console.error("Error loading module form:", err);
            // Global handler will also trigger if set
        });

}


// Submit Add Module form (POST via fetch)
function submitAddModuleForm(form, modal) {
    const formData = new FormData(form);

    const moduleId = parseInt(form.querySelector('[name="Id"]').value);

    AjaxService.post(form.action, formData, { isFormData: true })
        .then(result => {
            const { success, html, isNew } = result.data; // This is the returned partial HTML or form with errors

            //console.log(`success = ${success}`);
            //console.log(`html = ${html}`);
            //console.log(`isNew = ${isNew}`);

            if (!success) {
                // Validation errors → reload form with messages
                document.getElementById("module-form-body").innerHTML = html;

                // Rebind submit handler
                const newForm = document.getElementById("addModuleForm");
                newForm.addEventListener("submit", function (e) {
                    e.preventDefault();
                    submitAddModuleForm(newForm, modal);
                });
            } else {
                const moduleEl = document.getElementById(`module-${moduleId}`);

                if (isNew) {
                    // Add new module
                    document.getElementById("modulesList").insertAdjacentHTML("beforeend", html);
                } else {
                    // Replace updated module
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
            // Global handler will also trigger if set
        });

}

function showAddItemModal(courseId,moduleId) {

    const url = `/Course/AddItem?CourseId=${courseId}&ModuleId=${moduleId}`;

    fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Server returned ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            document.getElementById('content-form-body').innerHTML = html;
            
            const modalEl = document.getElementById('content-form-model');
            const modal = new bootstrap.Modal(modalEl);
            modal.show();

            const form = document.getElementById('addContentForm');
            const fileInput = document.getElementById('fileInput');
            
            
       /*     fileInput.addEventListener('change', () => {
                // Clear previous list
             
                    
                });
            });*/
            // const fileInput = document.getElementById('fileInput');
            
            fileInput.addEventListener('change', () => {
               // console.log(document.getElementById('fileInput'));
                refreshTable(fileInput.files);
            });

            form.addEventListener('submit', function (e) {
                e.preventDefault();
                
                const formData = new FormData(this);
                const stagenameMap = {};
               
                Array.from(fileInput.files).forEach((file, index) => {
                    // Find the custom name input for this index
                    const customInput = document.querySelector(`input[data-index='${index}']`);
                    const stageName = customInput && customInput.value
                        ? customInput.value
                        : file.name;
                    stagenameMap[file.name] = stageName;
                    // Append with custom name
                    formData.append("files", file);
                });
                formData.append("stagenameMap", JSON.stringify(stagenameMap));
                // submitAddModuleForm(formData, modal);
                //formData.append("__RequestVerificationToken", document.querySelector('input[name="__RequestVerificationToken"]').value);
                //AJAX part
                fetch('/Course/AddContent', {
                    method: 'POST',
                    body: formData
                })
                    .then(res => res.json())
                    .then(result => {
                        alert("Upload success!");
                        modal?.hide(); // if inside a modal
                    })
                    .catch(err => console.error(err));
            });
            
        })
        .catch(err => console.error("Error loading content form:", err));

}

function refreshTable(files) {
    fileList.innerHTML = '';
    fileListtable.innerHTML = '';

    Array.from(fileInput.files).forEach((file, index) => {
        const tr = document.createElement('tr');
        const td_file = document.createElement('td');
        const td_rm_btn = document.createElement('td');
        const customCell = document.createElement('td');
        const rm_btn = document.createElement('button');

        rm_btn.textContent = "Remove";
        rm_btn.className = "btn btn-sm btn-danger";
        rm_btn.addEventListener('click', () => removeFile(index));

        td_rm_btn.appendChild(rm_btn);

        const customInput = document.createElement('input');
        customInput.type = "text";
        customInput.className = "form-control";
        customInput.placeholder = "Enter stage name for this file";
        customInput.dataset.index = index;
        customCell.appendChild(customInput);

        tr.className = 'table-group';
        // td1.className = 'table-group';
        // td2.className = 'table-group';
        td_file.textContent = `${file.name} (${Math.round(file.size / 1024)} KB)`;

        tr.appendChild(td_file);
        tr.appendChild(customCell)
        tr.appendChild(td_rm_btn);
        fileListtable.appendChild(tr);
    });
}

function removeFile(indexToRemove) {
    const dt = new DataTransfer();
    Array.from(fileInput.files).forEach((file, index) => {
        if (index !== indexToRemove) {
            dt.items.add(file);
        }
    });
    fileInput.files = dt.files;
    refreshTable(fileInput.files);
}






// Initialize edit mode toggle
document.getElementById('toggleEditMode').addEventListener('click', toggleEditMode);

document.addEventListener('DOMContentLoaded', function () {
    if (enableEditMode) {
        // Ensure editMode flag is correct and call toggle function
        if (!editMode) toggleEditMode();
    }
});


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