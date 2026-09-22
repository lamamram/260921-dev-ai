using ECommerce.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// --- Données en mémoire (à remplacer par une vraie persistance dans les exercices) ---
var products = new List<Product>
{
    new(1, "Clavier mécanique", 79.90m, 25),
    new(2, "Souris sans fil", 29.90m, 40),
    new(3, "Écran 27\" 4K", 349.00m, 10),
    new(4, "Casque audio", 59.90m, 15),
};

var cart = new List<CartItem>();
var orders = new List<Order>();
var nextOrderId = 1;

// --- Endpoints ---
app.MapGet("/", () => "ECommerce demo API — voir /products, /cart, /orders");

app.MapGet("/products", () => products);

app.MapGet("/products/{id:int}", (int id) =>
    products.FirstOrDefault(p => p.Id == id) is { } product
        ? Results.Ok(product)
        : Results.NotFound());

app.MapGet("/cart", () => cart);

app.MapPost("/cart/items", (CartItem item) =>
{
    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
    if (product is null)
    {
        return Results.NotFound("Produit inconnu");
    }

    if (item.Quantity <= 0)
    {
        return Results.BadRequest("Quantité invalide");
    }

    var existing = cart.FirstOrDefault(c => c.ProductId == item.ProductId);
    if (existing is not null)
    {
        cart.Remove(existing);
        cart.Add(existing with { Quantity = existing.Quantity + item.Quantity });
    }
    else
    {
        cart.Add(item);
    }

    return Results.Ok(cart);
});

app.MapDelete("/cart/items/{productId:int}", (int productId) =>
{
    cart.RemoveAll(c => c.ProductId == productId);
    return Results.Ok(cart);
});

app.MapPost("/orders/checkout", () =>
{
    if (cart.Count == 0)
    {
        return Results.BadRequest("Panier vide");
    }

    var total = cart.Sum(c => c.Quantity * products.First(p => p.Id == c.ProductId).Price);
    var order = new Order(nextOrderId++, cart.ToList(), total, DateTime.UtcNow);
    orders.Add(order);
    cart.Clear();

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/orders", () => orders);

app.Run();
