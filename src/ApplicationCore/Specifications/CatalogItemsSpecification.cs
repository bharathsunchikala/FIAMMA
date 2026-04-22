using System.Linq;
using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities;

namespace Fiamma.ApplicationCore.Specifications;

public class CatalogItemsSpecification : Specification<CatalogItem>
{
    public CatalogItemsSpecification(params int[] ids)
    {
        Query.Where(c => ids.Contains(c.Id));
    }
}

