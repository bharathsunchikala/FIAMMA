using Ardalis.GuardClauses;
using Fiamma.ApplicationCore.Interfaces;

namespace Fiamma.ApplicationCore.Entities;

public class ProductOption : BaseEntity, IAggregateRoot
{
    public int CatalogItemId { get; private set; }
    public CatalogItem? CatalogItem { get; private set; }
    public OptionType Type { get; private set; }
    public string Value { get; private set; }
    public string DisplayName { get; private set; }
    public decimal AdditionalPrice { get; private set; }

    private ProductOption() { }

    public ProductOption(int catalogItemId, OptionType type, string value, string displayName, decimal additionalPrice = 0)
    {
        Guard.Against.Zero(catalogItemId, nameof(catalogItemId));
        Guard.Against.NullOrWhiteSpace(value, nameof(value));

        CatalogItemId = catalogItemId;
        Type = type;
        Value = value;
        DisplayName = displayName;
        AdditionalPrice = additionalPrice;
    }

    public enum OptionType
    {
        Size = 0,
        Color = 1,
        CustomText = 2
    }
}

