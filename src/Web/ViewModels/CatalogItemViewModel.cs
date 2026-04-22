namespace Fiamma.Web.ViewModels;

public class CatalogItemViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? PictureUri { get; set; }
    public decimal Price { get; set; }
    public string SelectedOptionsJson { get; set; } = "[]";
    public List<ProductOptionViewModel> Options { get; set; } = new();
}

public class ProductOptionViewModel
{
    public int Id { get; set; }
    public string Type { get; set; } = "";
    public string Value { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public decimal AdditionalPrice { get; set; }
}

