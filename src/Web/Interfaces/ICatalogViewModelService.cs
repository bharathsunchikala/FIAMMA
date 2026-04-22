using Microsoft.AspNetCore.Mvc.Rendering;
using Fiamma.Web.ViewModels;

namespace Fiamma.Web.Services;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId, string? searchTerm = null);
    Task<IEnumerable<SelectListItem>> GetBrands();
    Task<IEnumerable<SelectListItem>> GetTypes();
}

