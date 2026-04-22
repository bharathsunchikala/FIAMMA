using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities.OrderAggregate;
using Fiamma.ApplicationCore.Interfaces;
using Fiamma.Web.Features.MyOrders;
using NSubstitute;
using Xunit;

namespace Fiamma.UnitTests.MediatorHandlers.OrdersTests;

public class GetMyOrders
{
    private readonly IReadRepository<Order> _mockOrderRepository = Substitute.For<IReadRepository<Order>>();

    public GetMyOrders()
    {
        var item = new OrderItem(new CatalogItemOrdered(1, "ProductName", "URI"), 10.00m, 10);
        var address = new Address("", "", "", "", "");
        Order order = new Order("buyerId", address, new List<OrderItem> { item });
              
        _mockOrderRepository.ListAsync(Arg.Any<ISpecification<Order>>(), Arg.Any<CancellationToken>()).Returns(new List<Order> { order });
    }

    [Fact]
    public async Task NotReturnNullIfOrdersArePresIent()
    {
        var request = new Fiamma.Web.Features.MyOrders.GetMyOrders("SomeUserName");

        var handler = new GetMyOrdersHandler(_mockOrderRepository);

        var result = await handler.Handle(request, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
    }
}



