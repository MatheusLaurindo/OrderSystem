namespace Billing.Worker.Models;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}