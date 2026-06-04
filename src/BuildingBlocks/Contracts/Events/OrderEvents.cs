namespace Contracts.Events;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record OrderSubmitted(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    List<OrderItemDto> Items,
    DateTime CreatedAt
);

public record InventoryReserved(
    Guid OrderId,
    DateTime ReservedAt
);

public record InventoryRejected(
    Guid OrderId,
    string Reason,
    DateTime RejectedAt
);

public record PaymentApproved(
    Guid OrderId,
    Guid TransactionId,
    DateTime ApprovedAt
);

public record PaymentRejected(
    Guid OrderId,
    string Reason,
    DateTime RejectedAt
);