namespace Inventory.Worker.Models;

public class InventoryItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int AvailableQuantity { get; set; }
}