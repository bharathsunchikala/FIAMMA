using System.Text.Json;
using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Fiamma.ApplicationCore.Entities.BasketAggregate;

public class BasketItem : BaseEntity
{

    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public int CatalogItemId { get; private set; }
    public int BasketId { get; private set; }
    public string SelectedOptionsJson { get; private set; } = "[]";

    public BasketItem(int catalogItemId, int quantity, decimal unitPrice)
    {
        CatalogItemId = catalogItemId;
        UnitPrice = unitPrice;
        SetQuantity(quantity);
    }

    public BasketItem(int catalogItemId, int quantity, decimal unitPrice, string selectedOptionsJson)
    {
        CatalogItemId = catalogItemId;
        UnitPrice = unitPrice;
        SetQuantity(quantity);
        SelectedOptionsJson = string.IsNullOrWhiteSpace(selectedOptionsJson) ? "[]" : selectedOptionsJson;
    }

    public void AddQuantity(int quantity)
    {
        Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

        Quantity = quantity;
    }

    public void SetSelectedOptions(string selectedOptionsJson)
    {
        SelectedOptionsJson = string.IsNullOrWhiteSpace(selectedOptionsJson) ? "[]" : selectedOptionsJson;
    }

    public List<SelectedOption> GetSelectedOptions()
    {
        try
        {
            return JsonSerializer.Deserialize<List<SelectedOption>>(SelectedOptionsJson) ?? new List<SelectedOption>();
        }
        catch
        {
            return new List<SelectedOption>();
        }
    }

    public decimal GetTotalPrice()
    {
        var options = GetSelectedOptions();
        decimal optionsUpcharge = options.Sum(o => o.AdditionalPrice);
        return UnitPrice + optionsUpcharge;
    }

    public readonly record struct SelectedOption
    {
        public int ProductOptionId { get; init; }
        public string Type { get; init; }
        public string Value { get; init; }
        public string DisplayName { get; init; }
        public decimal AdditionalPrice { get; init; }
    }
}

