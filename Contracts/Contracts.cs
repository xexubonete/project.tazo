namespace Contracts;

public record OrderCreated(Guid OrderId, Guid ProductId, int Quantity);