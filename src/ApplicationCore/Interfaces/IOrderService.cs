using System.Threading.Tasks;
using Fiamma.ApplicationCore.Entities.OrderAggregate;

namespace Fiamma.ApplicationCore.Interfaces;

public interface IOrderService
{
    Task CreateOrderAsync(int basketId, Address shippingAddress);
}

