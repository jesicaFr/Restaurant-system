namespace RestaurantManagement.Api.Models;

public static class RestaurantValues
{
    public const string Available = "Disponible";
    public const string Occupied = "Ocupada";
    public const string Pending = "Pendiente";
    public const string Preparing = "En preparación";
    public const string Delivered = "Entregado";
    public const string Cash = "Efectivo";
    public const string Card = "Tarjeta";

    public static readonly string[] OrderStatuses = [Pending, Preparing, Delivered];
    public static readonly string[] PaymentMethods = [Cash, Card];
}
