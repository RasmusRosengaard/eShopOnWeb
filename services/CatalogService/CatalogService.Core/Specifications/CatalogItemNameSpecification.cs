using Ardalis.Specification;
using CatalogService.Core.Entities;

namespace CatalogService.Core.Specifications;

public class CatalogItemNameSpecification : Specification<CatalogItem>
{
    public CatalogItemNameSpecification(string catalogItemName)
    {
        Query.Where(item => catalogItemName == item.Name);
    }
}
