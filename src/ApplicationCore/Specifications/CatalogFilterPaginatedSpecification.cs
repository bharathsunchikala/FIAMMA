using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities;

namespace Fiamma.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId, string? searchTerm = null)
        : base()
    {
        if (take == 0)
        {
            take = int.MaxValue;
        }

        var normalizedSearch = searchTerm?.Trim().ToLowerInvariant();

        Query
            .Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId) &&
            (string.IsNullOrEmpty(normalizedSearch) ||
             i.Name.ToLower().Contains(normalizedSearch) ||
             i.Description.ToLower().Contains(normalizedSearch)))
            .Skip(skip).Take(take);
    }
}

