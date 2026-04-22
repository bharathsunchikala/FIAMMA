namespace Fiamma.PublicApi.CatalogItemEndpoints;

using System.Collections.Generic;

public class CatalogItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public int CatalogBrandId { get; set; }
    public List<ProductOptionDto> Options { get; set; } = new();
}

