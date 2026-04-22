namespace Fiamma.Web.ViewModels;

public class OrderDetailViewModel : OrderViewModel
{
    public List<OrderItemViewModel> OrderItems { get; set; } = new();
}

