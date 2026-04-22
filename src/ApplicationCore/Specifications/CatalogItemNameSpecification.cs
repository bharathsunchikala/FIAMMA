using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities;

namespace Fiamma.ApplicationCore.Specifications;

public class CatalogItemNameSpecification : Specification<CatalogItem>
{
    public CatalogItemNameSpecification(string catalogItemName)
    {
        Query.Where(item => catalogItemName == item.Name);
    }
}

