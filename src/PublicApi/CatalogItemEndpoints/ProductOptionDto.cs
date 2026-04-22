namespace Fiamma.PublicApi.CatalogItemEndpoints;

public class ProductOptionDto
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string DisplayName { get; set; }
    public decimal AdditionalPrice { get; set; }
}

