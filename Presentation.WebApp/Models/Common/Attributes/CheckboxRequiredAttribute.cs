using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Common.Attributes;

public class CheckboxRequiredAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value) => value is bool b && b;

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-mustbetrue", ErrorMessage ?? "You must accept the terms.");
    }

    private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
