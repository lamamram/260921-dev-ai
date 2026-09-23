using System.Net;
using System.Net.Http.Json;
using ECommerce.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests;

/// <summary>
/// Tests d'intégration de la route POST /orders/checkout via WebApplicationFactory.
/// Chaque test crée sa propre factory pour isoler le panier et les commandes en mémoire.
/// </summary>
public sealed class OrdersCheckoutIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdersCheckoutIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Checkout_PanierVide_Retourne400()
    {
        // Act
        var response = await _client.PostAsync("/orders/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Panier vide");
    }

    [Fact]
    public async Task Checkout_PanierPlein_Retourne201AvecCommande()
    {
        // Arrange
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 1, Quantity: 2));

        // Act
        var response = await _client.PostAsync("/orders/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var order = await response.Content.ReadFromJsonAsync<Order>();
        order.Should().NotBeNull();
        order!.Id.Should().Be(1);
        order.Items.Should().HaveCount(1);
        order.Items[0].ProductId.Should().Be(1);
        order.Items[0].Quantity.Should().Be(2);
        order.Total.Should().Be(159.80m);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Checkout_CalculeLeTotal_AvecPlusieursProduits()
    {
        // Arrange — 2×79.90 + 1×29.90 = 189.70
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 1, Quantity: 2));
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 2, Quantity: 1));

        // Act
        var response = await _client.PostAsync("/orders/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<Order>();
        order.Should().NotBeNull();
        order!.Items.Should().HaveCount(2);
        order.Total.Should().Be(189.70m);
    }

    [Fact]
    public async Task Checkout_VideLePanier()
    {
        // Arrange
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 3, Quantity: 1));

        // Act
        await _client.PostAsync("/orders/checkout", null);
        var cartResponse = await _client.GetAsync("/cart");

        // Assert
        cartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await cartResponse.Content.ReadFromJsonAsync<List<CartItem>>();
        cart.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Checkout_AjouteLaCommande_ALaListeDesOrders()
    {
        // Arrange
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 4, Quantity: 3));

        // Act
        await _client.PostAsync("/orders/checkout", null);
        var ordersResponse = await _client.GetAsync("/orders");

        // Assert
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await ordersResponse.Content.ReadFromJsonAsync<List<Order>>();
        orders.Should().NotBeNull().And.HaveCount(1);
        orders![0].Items[0].ProductId.Should().Be(4);
        orders[0].Total.Should().Be(179.70m);
    }

    [Fact]
    public async Task Checkout_DeuxFois_IncrementeLesIdentifiantsDeCommande()
    {
        // Arrange
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 1, Quantity: 1));
        var first = await _client.PostAsync("/orders/checkout", null);
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 2, Quantity: 1));

        // Act
        var second = await _client.PostAsync("/orders/checkout", null);

        // Assert
        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.Created);

        var firstOrder = await first.Content.ReadFromJsonAsync<Order>();
        var secondOrder = await second.Content.ReadFromJsonAsync<Order>();
        firstOrder!.Id.Should().Be(1);
        secondOrder!.Id.Should().Be(2);
        secondOrder.Id.Should().NotBe(firstOrder.Id);
    }

    [Fact]
    public async Task Checkout_PuisUnDeuxiemeCheckout_SansAjout_Retourne400()
    {
        // Arrange
        await _client.PostAsJsonAsync("/cart/items", new CartItem(ProductId: 1, Quantity: 1));
        await _client.PostAsync("/orders/checkout", null);

        // Act — le panier a été vidé par le premier checkout
        var response = await _client.PostAsync("/orders/checkout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Panier vide");
    }
}
