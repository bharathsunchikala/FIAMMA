using System.Collections.Generic;
using System.Linq;
using Fiamma.ApplicationCore.Interfaces;

namespace Fiamma.ApplicationCore.Entities.BasketAggregate;

public class Basket : BaseEntity, IAggregateRoot
{
    public string BuyerId { get; private set; }
    private readonly List<BasketItem> _items = new List<BasketItem>();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public int TotalItems => _items.Sum(i => i.Quantity);


    public Basket(string buyerId)
    {
        BuyerId = buyerId;
    }

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1, string selectedOptionsJson = "[]")
    {
        selectedOptionsJson = string.IsNullOrWhiteSpace(selectedOptionsJson) ? "[]" : selectedOptionsJson;

        if (!Items.Any(i => i.CatalogItemId == catalogItemId && i.SelectedOptionsJson == selectedOptionsJson))
        {
            _items.Add(new BasketItem(catalogItemId, quantity, unitPrice, selectedOptionsJson));
            return;
        }
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId && i.SelectedOptionsJson == selectedOptionsJson);
        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems()
    {
        _items.RemoveAll(i => i.Quantity == 0);
    }

    public void SetNewBuyerId(string buyerId)
    {
        BuyerId = buyerId;
    }
}

