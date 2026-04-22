using Ardalis.Specification;
using Fiamma.ApplicationCore.Entities.OrderAggregate;

namespace Fiamma.ApplicationCore.Specifications;

public class CustomerOrdersSpecification : Specification<Order>
{
    public CustomerOrdersSpecification(string buyerId)
    {
        Query.Where(o => o.BuyerId == buyerId)
            .Include(o => o.OrderItems);
    }
}

