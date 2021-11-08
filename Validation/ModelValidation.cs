using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using dotnet_api_test.Exceptions.ExceptionResponses;
using dotnet_api_test.Models.Dtos;

namespace dotnet_api_test.Validation
{
    public static class ModelValidation
    {
        public static void ValidateCreateDishDto(CreateDishDto entity)
        {
            var validationResults = new List<ValidationResult>();
            ValidateEntityPropertyIsNotNull(nameof(entity.Name), entity.Name, validationResults);
            ValidateStringIsNotEmpty(nameof(entity.Name), entity.Name, validationResults);
            ValidateEntityPropertyIsNotNegative(nameof(entity.Cost), entity.Cost, validationResults);
            ValidateEntityPropertyIsNotNull(nameof(entity.MadeBy), entity.MadeBy, validationResults);
            ValidateStringIsNotEmpty(nameof(entity.MadeBy), entity.MadeBy, validationResults);
            if (validationResults.Count > 0)
                throw new BadRequestExceptionResponse("Model is not valid because following properties are missing: " +
                                                      string.Join(", ",
                                                          validationResults.Select(s => s.ErrorMessage).ToArray()));
        }

        private static void ValidateStringIsNotEmpty(string entityName, string? value,
            ICollection<ValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(value)) validationResults.Add(new ValidationResult($"{entityName} was empty."));
        }

        public static void IsDishCostLessThanPercentageMax(double currentCost, double updatedCost,
            double percentageIncreaseMax)
        {
            if (currentCost * percentageIncreaseMax < updatedCost)
                throw new BadRequestExceptionResponse(
                    $"Error: The new dish cost was above {percentageIncreaseMax} * current cost.");
        }

        public static void ValidateUpdateDishDto(UpdateDishDto entity)
        {
            var validationResults = new List<ValidationResult>();
            ValidateEntityPropertyIsNotNull(nameof(entity.Name), entity.Name, validationResults);
            ValidateStringIsNotEmpty(nameof(entity.Name), entity.Name, validationResults);
            ValidateEntityPropertyIsNotNegative(nameof(entity.Cost), entity.Cost, validationResults);
            ValidateEntityPropertyIsNotNull(nameof(entity.MadeBy), entity.MadeBy, validationResults);
            ValidateStringIsNotEmpty(nameof(entity.MadeBy), entity.MadeBy, validationResults);
            if (validationResults.Count > 0)
            {
                throw new BadRequestExceptionResponse("Model is not valid because following properties are missing: " +
                                                      string.Join(", ",
                                                          validationResults.Select(s => s.ErrorMessage).ToArray()));
            }
        }

        private static void ValidateEntityPropertyIsNotNegative(string entityName, double? value,
            ICollection<ValidationResult> validationResults)
        {
            if (value is < 0) validationResults.Add(new ValidationResult(entityName + " was a negative value."));
        }

        private static void ValidateEntityPropertyIsNotNull<T>(string entityName, T value,
            ICollection<ValidationResult> validationResults)
        {
            if (value == null) validationResults.Add(new ValidationResult(entityName));
        }

        public static void ValidateDishNameIsUnique(string dishName, IEnumerable<Dish> allDishes)
        {
            if (allDishes.Any(e => e.Name.Equals(dishName)))
                throw new BadRequestExceptionResponse($"Dish with name {dishName} already exists.");
        }
    }
}