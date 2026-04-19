$(function() {
    const $forms = $("#contact-form, .signin-form");
    const $checkbox = $("#accept-terms");
    const $checkmark = $(".checkbox-checkmark");

    if ($.validator && $.validator.unobtrusive) {
        $.validator.addMethod("mustbetrue", function(value, element) {
            return $(element).is(":checked");
        });
        $.validator.unobtrusive.adapters.addBool("mustbetrue");
    }

    // Säg till jQuery Validation att inte ignorera checkboxen
    $forms.each(function() {
        if ($.validator && $(this).data("validator")) {
            $(this).data("validator").settings.ignore = [];
        }
    });

    function validateCheckbox($form) {
        // Vi hittar namnet på propertyn (t.ex. TermsAndConditions) dynamiskt
        const name = $checkbox.attr("name");
        const $errorSpan = $form.find(`span[data-valmsg-for='${name}']`);
        
        const isChecked = $checkbox.is(":checked");
        const errorMsg = $checkbox.attr("data-val-range") || "You must accept the terms.";

        if (isChecked) {
            $checkmark.removeClass("input-validation-error");
            $errorSpan.text("")
                      .removeClass("field-validation-error")
                      .addClass("field-validation-valid");
            return true;
        } else {
            // Visa bara rött om man faktiskt har klickat på submit
            if ($form.data("submitted")) {
                $checkmark.addClass("input-validation-error");
                $errorSpan.text(errorMsg)
                          .addClass("field-validation-error")
                          .removeClass("field-validation-valid");
            }
            return false;
        }
    }

    $checkbox.on("change", function() {
        validateCheckbox($(this).closest('form'));
    });

    $forms.on("submit", function (e) {
        const $currentForm = $(this);
        $currentForm.data("submitted", true); 

        const isFormValid = $currentForm.valid();
        const isCheckboxValid = validateCheckbox($currentForm);

        if (!isFormValid || !isCheckboxValid) {
            e.preventDefault();
            
            // Om knappen känns död, kolla konsolen (F12) för att se exakt vad som stoppar
            console.warn("Valideringsfel i:", $currentForm.attr('class'), $currentForm.validate().errorList);
        }
    });
});