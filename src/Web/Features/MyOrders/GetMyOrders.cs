using MediatR;
using Fiamma.Web.ViewModels;

namespace Fiamma.Web.Features.MyOrders;

public class GetMyOrders : IRequest<IEnumerable<OrderViewModel>>
{
    public string UserName { get; set; }

    public GetMyOrders(string userName)
    {
        UserName = userName;
    }
}

