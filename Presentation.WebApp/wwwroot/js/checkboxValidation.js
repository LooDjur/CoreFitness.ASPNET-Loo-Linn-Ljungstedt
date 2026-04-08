$(function() {
    const $form = $("#contact-form, .signin-form");
    const $checkbox = $("#accept-terms");

    const $errorSpan = $checkbox.siblings('span[data-valmsg-for]');

    if ($.validator && $form.data("validator")) {
        $form.data("validator").settings.ignore = [];
    }

    function validateCheckbox() {
        const isChecked = $checkbox.is(":checked");
        const errorMsg = $checkbox.attr("data-val-range");

        if (isChecked) {
            $(".checkbox-checkmark").removeClass("input-validation-error");
            $errorSpan.text("")
                      .removeClass("field-validation-error")
                      .addClass("field-validation-valid");
            return true;
        } else {
            $(".checkbox-checkmark").addClass("input-validation-error");
            $errorSpan.text(errorMsg)
                      .addClass("field-validation-error")
                      .removeClass("field-validation-valid");
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