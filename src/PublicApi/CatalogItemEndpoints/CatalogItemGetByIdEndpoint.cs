using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Fiamma.ApplicationCore.Entities;
using Fiamma.ApplicationCore.Interfaces;

namespace Fiamma.PublicApi.CatalogItemEndpoints;

/// <summary>
/// Get a Catalog Item by Id
/// </summary>
public class CatalogItemGetByIdEndpoint(IRepository<CatalogItem> itemRepository, IUriComposer uriComposer)
    : Endpoint<GetByIdCatalogItemRequest, Results<Ok<GetByIdCatalogItemResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("api/catalog-items/{catalogItemId}");
        AllowAnonymous();
        Description(d =>
            d.Produces<GetByIdCatalogItemResponse>()
            .WithTags("CatalogItemEndpoints"));
    }

    public override async Task<Results<Ok<GetByIdCatalogItemResponse>, NotFound>> ExecuteAsync(GetByIdCatalogItemRequest request, CancellationToken ct)
    {
        var response = new GetByIdCatalogItemResponse(request.CorrelationId());

        var item = await itemRepository.GetByIdAsync(request.CatalogItemId, ct);
        if (item is null)
            return TypedResults.NotFound();

        response.CatalogItem = new CatalogItemDto
        {
            Id = item.Id,
            CatalogBrandId = item.CatalogBrandId,
            CatalogTypeId = item.CatalogTypeId,
            Description = item.Description,
            Name = item.Name,
            PictureUri = uriComposer.ComposePicUri(item.PictureUri),
            Price = item.Price,
            Options = item.Options.Select(o => new ProductOptionDto
            {
                Id = o.Id,
                CatalogItemId = o.CatalogItemId,
                Type = o.Type.ToString(),
                Value = o.Value,
                DisplayName = o.DisplayName,
                AdditionalPrice = o.AdditionalPrice
            }).ToList()
        };
        return TypedResults.Ok(response);
    }
}

