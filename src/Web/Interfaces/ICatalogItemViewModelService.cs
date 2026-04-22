using Fiamma.Web.ViewModels;

namespace Fiamma.Web.Interfaces;

public interface ICatalogItemViewModelService
{
    Task UpdateCatalogItem(CatalogItemViewModel viewModel);
}

