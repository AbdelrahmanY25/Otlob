using System.Text.Json;

namespace Otlob.Core.Contracts.OrderDetails;

public class MealDetailsParser
{
    public List<MealOptionDetail> Options { get; set; } = [];
    public List<MealAddOnDetail> AddOns { get; set; } = [];

    public static MealDetailsParser? Parse(string? jsonDetails)
    {
        if (string.IsNullOrEmpty(jsonDetails))
            return null;

        try
        {
            return JsonSerializer.Deserialize<MealDetailsParser>(jsonDetails, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    public string GetFormattedDetails()
    {
        var parts = new List<string>();

        if (Options.Any())
        {
            var optionTexts = Options.Select(o => $"{o.GroupName}: {o.ItemName}");
            parts.AddRange(optionTexts);
        }

        if (AddOns.Any())
        {
            var addOnTexts = AddOns.Select(a => $"+ {a.Name}");
            parts.AddRange(addOnTexts);
        }

        return string.Join(" • ", parts);
    }
}

public class MealOptionDetail
{
    public string GroupName { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class MealAddOnDetail
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}
