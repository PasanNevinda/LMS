// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// get request to Add method in ModuleController
showInPopUp_Module = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        // Add method return a view, we add it to this div
        success: function (res) {
            $("#module-form-model .modal-body").html(res);
            $("#module-form-model .modal-title").html(title);
            $("#module-form-model").modal("show");
        }
    })
}



$(function () {
    if (!document.querySelector("#modulesAccordion")) return;

    $(document).on("submit", "#moduleAddForm", function (e) {
        e.preventDefault();
        var $form = $(this);
        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: $form.attr("action"),
            method: $form.attr("method") || "POST",
            data: $form.serialize(),
            headers: { "RequestVerificationToken": token },
        })
            .done(function (response) {
                if (response.isValid) {
                    $("#modulesAccordion").append(response.html);

                    var modalEl = document.getElementById("module-form-model");
                    var modalInstance = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                    modalInstance.hide();

                    if (response.moduleId) {
                        var sel = "#module-" + response.moduleId;
                        var el = document.querySelector(sel);
                        if (el) el.scrollIntoView({ behavior: "smooth", block: "center" });
                    }
                } else {
                    $("#module-form-model .modal-body").html(response.html);
                }
            })
            .fail(function () {
                alert("Server error — please try again.");
            });
    });
});

