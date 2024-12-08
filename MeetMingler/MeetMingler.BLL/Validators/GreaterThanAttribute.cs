namespace MeetMingler.BLL.Validators;

using System.ComponentModel.DataAnnotations;

public class GreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public GreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = (IComparable)value;
            
        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
            throw new ArgumentException($"Property with name {_comparisonProperty} not found");

        var comparisonValue = (IComparable)property.GetValue(validationContext.ObjectInstance);

        return currentValue.CompareTo(comparisonValue) <= 0
            ? new ValidationResult(ErrorMessage ?? $"The value of {validationContext.DisplayName} must be greater than the value of {_comparisonProperty}")
            : ValidationResult.Success;
    } 
}
