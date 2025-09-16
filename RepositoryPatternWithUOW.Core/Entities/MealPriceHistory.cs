﻿namespace Otlob.Core.Entities;

public class MealPriceHistory
{
    public int Id { get; set; }
    public int MealId { get; set; }

    public decimal Price { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; } = null;

    public Meal Meal { get; set; } = null!;
}
