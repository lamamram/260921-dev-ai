namespace ECommerce.Api.Models;

public record Order(int Id, List<CartItem> Items, decimal Total, DateTime CreatedAt);
