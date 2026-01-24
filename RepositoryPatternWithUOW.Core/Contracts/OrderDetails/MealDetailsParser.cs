using System.Text.Json;
using System.Text.Json.Serialization;

namespace Otlob.Core.Contracts.OrderDetails;

public class MealDetailsParser
{
    [JsonPropertyName("items")]
    public List<MealOptionDetail> Items { get; set; } = [];
    
    [JsonPropertyName("addOns")]
    public List<MealAddOnDetail> AddOns { get; set; } = [];

    public static MealDetailsParser? Parse(string? jsonDetails)
    {
        if (string.IsNullOrEmpty(jsonDetails))
            return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<MealDetailsParser>(jsonDetails, options);
        }
        catch
        {
            return null;
        }
    }
}

public class MealOptionDetail
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class MealAddOnDetail
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
