using Fiamma.ApplicationCore.Entities.BasketAggregate;
using Fiamma.Web.Pages.Basket;

namespace Fiamma.Web.Interfaces;

public interface IBasketViewModelService
{
    Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    Task<int> CountTotalBasketItems(string username);
    Task<BasketViewModel> Map(Basket basket);
}

