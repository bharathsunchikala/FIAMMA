using System.ComponentModel.DataAnnotations;

namespace Fiamma.Web.Pages.Basket;

public class BasketItemViewModel
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be bigger than 0")]
    public int Quantity { get; set; }

    public string? PictureUrl { get; set; }
    public List<SelectedOptionViewModel> SelectedOptions { get; set; } = new();
}

public class SelectedOptionViewModel
{
    public int ProductOptionId { get; set; }
    public string Type { get; set; } = "";
    public string Value { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public decimal AdditionalPrice { get; set; }
}

