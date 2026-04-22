using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities.OrderAggregate;

namespace Fiamma.ApplicationCore.Specifications;

public class OrderWithItemsByIdSpec : Specification<Order>
{
    public OrderWithItemsByIdSpec(int orderId)
    {
        Query
            .Where(order => order.Id == orderId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered);
    }
}

