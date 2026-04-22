using MediatR;
using Fiamma.Web.ViewModels;

namespace Fiamma.Web.Features.OrderDetails;

public class GetOrderDetails : IRequest<OrderDetailViewModel>
{
    public string UserName { get; set; }
    public int OrderId { get; set; }

    public GetOrderDetails(string userName, int orderId)
    {
        UserName = userName;
        OrderId = orderId;
    }
}

