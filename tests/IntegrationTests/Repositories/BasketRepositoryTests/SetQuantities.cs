using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fiamma.ApplicationCore.Entities.BasketAggregate;
using Fiamma.ApplicationCore.Services;
using Fiamma.Infrastructure.Data;
using Fiamma.UnitTests.Builders;
using Xunit;

namespace Fiamma.IntegrationTests.Repositories.BasketRepositoryTests;

public class SetQuantities
{
    private readonly CatalogContext _catalogContext;
    private readonly EfRepository<Basket> _basketRepository;
    private readonly BasketBuilder _basketBuilder = new();

    public SetQuantities()
    {
        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: "TestCatalog")
            .Options;
        _catalogContext = new CatalogContext(dbOptions);
        _basketRepository = new EfRepository<Basket>(_catalogContext);
    }

    [Fact]
    public async Task RemoveEmptyQuantities()
    {
        var basket = _basketBuilder.WithOneBasketItem();
        var basketService = new BasketService(_basketRepository, null);
        await _basketRepository.AddAsync(basket, TestContext.Current.CancellationToken);
        _catalogContext.SaveChanges();

        await basketService.SetQuantities(_basketBuilder.BasketId, new Dictionary<string, int>() { { _basketBuilder.BasketId.ToString(), 0 } });

        Assert.Empty(basket.Items);
    }
}


