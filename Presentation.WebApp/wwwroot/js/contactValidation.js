$(function() {
    const $form = $("#contact-form");
    const $checkbox = $("#accept-terms");
    const $errorSpan = $('[data-valmsg-for="AcceptSavePersonalInformation"]');

    if ($.validator && $form.data("validator")) {
        $form.data("validator").settings.ignore = [];
    }

    function validateCheckbox() {
        const isChecked = $checkbox.is(":checked");
        const rangeMsg = $checkbox.attr("data-val-range");

        if (isChecked) {
            $(".checkbox-checkmark").removeClass("input-validation-error");
            $errorSpan.text("").removeClass("field-validation-error").addClass("field-validation-valid");
            return true;
        } else {
            $(".checkbox-checkmark").addClass("input-validation-error");
            $errorSpan.text(rangeMsg).addClass("field-validation-error").removeClass("field-validation-valid");
            return false;
        }
    }

    $checkbox.on("change", validateCheckbox);

    $form.on("submit", function (e) {
        const isFormValid = $form.valid();
        const isCheckboxValid = validateCheckbox();

        if (!isFormValid || !isCheckboxValid) {
            e.preventDefault();
        }
    });
});