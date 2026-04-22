using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Fiamma.ApplicationCore.Entities;
using Fiamma.ApplicationCore.Interfaces;
using Fiamma.Web.Services;
using Fiamma.Web.ViewModels;

namespace Fiamma.Web.Pages;

public class IndexModel : PageModel
{
    private static readonly string[] TShirtImageOrder =
    {
        "/images/products/fiamma-ember-pocket-tee.png",
        "/images/products/fiamma-crimson-classic-tee.png",
        "/images/products/fiamma-electric-flame-tee.png",
        "/images/products/fiamma-inferno-graphic-tee.png",
        "/images/products/fiamma-midnight-polo-shirt.png"
    };

    private static readonly HashSet<string> TShirtNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Fiamma Ember Pocket T-Shirt",
        "Fiamma Crimson Classic T-Shirt",
        "Fiamma Electric Flame T-Shirt",
        "Fiamma Inferno Graphic T-Shirt",
        "Fiamma Midnight Polo Shirt"
    };

    private static readonly string[] BottomsImageOrder =
    {
        "/images/products/fiamma-shadow-cargo-bottoms.png",
        "/images/products/fiamma-frost-cargo-bottoms.png",
        "/images/products/fiamma-graphite-utility-bottoms.png",
        "/images/products/fiamma-mocha-cargo-bottoms.png"
    };

    private static readonly HashSet<string> BottomsNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Fiamma Shadow Cargo Bottoms",
        "Fiamma Frost Cargo Bottoms",
        "Fiamma Graphite Utility Bottoms",
        "Fiamma Mocha Cargo Bottoms"
    };

    private static readonly string[] JacketsImageOrder =
    {
        "/images/products/fiamma-obsidian-hooded-jacket.png",
        "/images/products/fiamma-storm-navy-jacket.png",
        "/images/products/fiamma-olive-ridge-jacket.png"
    };

    private static readonly HashSet<string> JacketsNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Fiamma Obsidian Hooded Jacket",
        "Fiamma Storm Navy Jacket",
        "Fiamma Olive Ridge Jacket"
    };

    private readonly ICatalogViewModelService _catalogViewModelService;
    private readonly IRepository<CatalogItem> _catalogItemRepository;
    private readonly IRepository<CatalogBrand> _catalogBrandRepository;
    private readonly IRepository<CatalogType> _catalogTypeRepository;

    public IndexModel(
        ICatalogViewModelService catalogViewModelService,
        IRepository<CatalogItem> catalogItemRepository,
        IRepository<CatalogBrand> catalogBrandRepository,
        IRepository<CatalogType> catalogTypeRepository)
    {
        _catalogViewModelService = catalogViewModelService;
        _catalogItemRepository = catalogItemRepository;
        _catalogBrandRepository = catalogBrandRepository;
        _catalogTypeRepository = catalogTypeRepository;
    }

    public required CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();
    public IEnumerable<SelectListItem> AvailableBrands { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AvailableTypes { get; set; } = Enumerable.Empty<SelectListItem>();

    [BindProperty]
    public CreateProductInputModel NewProduct { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGet(int? pageId, int? brandFilterApplied, int? typesFilterApplied, string? search, string? collection)
    {
        if (await TryLoadCollectionAsync(collection))
        {
            return;
        }

        await LoadCatalogAsync(pageId ?? 0, brandFilterApplied, typesFilterApplied, search);
    }

    public async Task<IActionResult> OnPostCreateProduct()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Page("/Index") });
        }

        if (!ModelState.IsValid)
        {
            await LoadCatalogAsync(0, null, null, null);
            return Page();
        }

        var brands = await _catalogBrandRepository.ListAsync();
        var types = await _catalogTypeRepository.ListAsync();

        var fallbackBrandId = brands.Select(b => b.Id).FirstOrDefault();
        var fallbackTypeId = types.Select(t => t.Id).FirstOrDefault();

        var catalogBrandId = NewProduct.CatalogBrandId ?? fallbackBrandId;
        var catalogTypeId = NewProduct.CatalogTypeId ?? fallbackTypeId;

        if (catalogBrandId <= 0 || catalogTypeId <= 0)
        {
            ModelState.AddModelError(string.Empty, "At least one brand and type must exist before adding products.");
            await LoadCatalogAsync(0, null, null, null);
            return Page();
        }

        var product = new CatalogItem(
            catalogTypeId,
            catalogBrandId,
            string.IsNullOrWhiteSpace(NewProduct.Description) ? $"{NewProduct.Name} by Fiamma." : NewProduct.Description.Trim(),
            NewProduct.Name.Trim(),
            NewProduct.Price,
            string.IsNullOrWhiteSpace(NewProduct.PictureUri) ? "images/products/1.png" : NewProduct.PictureUri.Trim());

        await _catalogItemRepository.AddAsync(product);
        StatusMessage = $"Added \"{product.Name}\" to Fiamma.";

        return RedirectToPage("/Index");
    }

    private async Task LoadCatalogAsync(int pageId, int? brandFilterApplied, int? typesFilterApplied, string? search)
    {
        CatalogModel = await _catalogViewModelService.GetCatalogItems(
            pageId,
            Constants.ITEMS_PER_PAGE,
            brandFilterApplied,
            typesFilterApplied,
            search);

        AvailableBrands = CatalogModel.Brands?.Where(b => !string.IsNullOrWhiteSpace(b.Value)).ToList()
            ?? new List<SelectListItem>();
        AvailableTypes = CatalogModel.Types?.Where(t => !string.IsNullOrWhiteSpace(t.Value)).ToList()
            ?? new List<SelectListItem>();
    }

    private async Task<bool> TryLoadCollectionAsync(string? collection)
    {
        if (string.IsNullOrWhiteSpace(collection))
        {
            return false;
        }

        var normalizedCollection = collection.Trim().ToLowerInvariant();
        var targetType = normalizedCollection switch
        {
            "tshirts" => "Clothing",
            "bottoms" => "Bottoms",
            "jackets" => "Jackets",
            _ => null
        };

        if (targetType is null)
        {
            return false;
        }

        var types = await _catalogTypeRepository.ListAsync();
        var targetTypeId = types.FirstOrDefault(t => t.Type.Equals(targetType, StringComparison.OrdinalIgnoreCase))?.Id;

        CatalogModel = await _catalogViewModelService.GetCatalogItems(
            pageIndex: 0,
            itemsPage: 1000,
            brandId: null,
            typeId: targetTypeId,
            searchTerm: null);

        if (normalizedCollection == "tshirts")
        {
            FilterToTShirtsOnly();
        }
        else if (normalizedCollection == "bottoms")
        {
            FilterToBottomsOnly();
        }
        else if (normalizedCollection == "jackets")
        {
            FilterToJacketsOnly();
        }

        AvailableBrands = CatalogModel.Brands?.Where(b => !string.IsNullOrWhiteSpace(b.Value)).ToList()
            ?? new List<SelectListItem>();
        AvailableTypes = CatalogModel.Types?.Where(t => !string.IsNullOrWhiteSpace(t.Value)).ToList()
            ?? new List<SelectListItem>();

        return true;
    }

    private void FilterToTShirtsOnly()
    {
        var orderedItems = TShirtImageOrder
            .Select(path => CatalogModel.CatalogItems.FirstOrDefault(item => MatchesCollectionAsset(item, path)))
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();

        if (orderedItems.Count == 0)
        {
            orderedItems = CatalogModel.CatalogItems
                .Where(item => item.Name is not null && TShirtNames.Contains(item.Name))
                .ToList();
        }

        ApplyCollectionListState(orderedItems);
    }

    private void FilterToBottomsOnly()
    {
        var orderedItems = BottomsImageOrder
            .Select(path => CatalogModel.CatalogItems.FirstOrDefault(item => MatchesCollectionAsset(item, path)))
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();

        if (orderedItems.Count == 0)
        {
            orderedItems = CatalogModel.CatalogItems
                .Where(item => item.Name is not null && BottomsNames.Contains(item.Name))
                .ToList();
        }

        ApplyCollectionListState(orderedItems);
    }

    private void FilterToJacketsOnly()
    {
        var orderedItems = JacketsImageOrder
            .Select(path => CatalogModel.CatalogItems.FirstOrDefault(item => MatchesCollectionAsset(item, path)))
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();

        if (orderedItems.Count == 0)
        {
            orderedItems = CatalogModel.CatalogItems
                .Where(item => item.Name is not null && JacketsNames.Contains(item.Name))
                .ToList();
        }

        ApplyCollectionListState(orderedItems);
    }

    private void ApplyCollectionListState(List<CatalogItemViewModel> orderedItems)
    {
        CatalogModel.CatalogItems = orderedItems;
        CatalogModel.Search = null;
        CatalogModel.BrandFilterApplied = 0;
        CatalogModel.TypesFilterApplied = 0;

        CatalogModel.PaginationInfo ??= new PaginationInfoViewModel();
        CatalogModel.PaginationInfo.ActualPage = 0;
        CatalogModel.PaginationInfo.ItemsPerPage = orderedItems.Count;
        CatalogModel.PaginationInfo.TotalItems = orderedItems.Count;
        CatalogModel.PaginationInfo.TotalPages = orderedItems.Count > 0 ? 1 : 0;
        CatalogModel.PaginationInfo.Previous = "is-disabled";
        CatalogModel.PaginationInfo.Next = "is-disabled";
    }

    private static bool MatchesCollectionAsset(CatalogItemViewModel item, string targetPath)
    {
        var picture = item.PictureUri;
        if (string.IsNullOrWhiteSpace(picture))
        {
            return false;
        }

        var normalizedTarget = targetPath.Trim();
        var targetWithoutLeadingSlash = normalizedTarget.TrimStart('/');

        return picture.Equals(normalizedTarget, StringComparison.OrdinalIgnoreCase) ||
               picture.EndsWith(normalizedTarget, StringComparison.OrdinalIgnoreCase) ||
               picture.EndsWith(targetWithoutLeadingSlash, StringComparison.OrdinalIgnoreCase);
    }

    public class CreateProductInputModel
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999")]
        public decimal Price { get; set; }

        public int? CatalogBrandId { get; set; }
        public int? CatalogTypeId { get; set; }

        [StringLength(400)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? PictureUri { get; set; }
    }
}

