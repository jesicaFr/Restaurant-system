namespace RestaurantManagement.Api.Models;

public enum OrderStatus
{
    Pending,
    Preparing,
    Delivered
}

public static class OrderStatusExtensions
{
    public static string ToDisplayName(this OrderStatus status) =>
        status switch
        {
            OrderStatus.Pending => "Pendiente",
            OrderStatus.Preparing => "En preparación",
            OrderStatus.Delivered => "Entregado",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

    public static bool TryParseDisplayName(string? value, out OrderStatus status)
    {
        status = value?.Trim() switch
        {
            "Pendiente" => OrderStatus.Pending,
            "En preparación" => OrderStatus.Preparing,
            "Entregado" => OrderStatus.Delivered,
            _ => (OrderStatus)(-1)
        };

        return Enum.IsDefined(status);
    }
}
