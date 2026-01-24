namespace Otlob.Core.Contracts.MobileApp.Order;

public record OrderDetailRequest
(
    string MealKey,
    int Quantity,
    string? Note,
    IEnumerable<string>? SelectedOptionItems,
    IEnumerable<string>? SelectedAddons
);